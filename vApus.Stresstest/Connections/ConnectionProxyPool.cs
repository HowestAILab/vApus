/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Stresstest
{
    /// <summary>
    /// Used for stresstesting, the size of the pool must equal or be greater than the concurrent users count.
    /// </summary>
    public class ConnectionProxyPool : IDisposable
    {
        public event EventHandler<LogEntryTestedEventArgs> LogEntryTested;
        public event EventHandler TestWorkFinished;
        public event EventHandler<TestWorkExceptionEventArgs> TestWorkException;

        #region Fields
        private int _usedConnectionProxies;
        private CompilerUnit _compilerUnit = new CompilerUnit();
        private Connection _connection;
        private Assembly _connectionProxyAssembly;

        private IConnectionProxy[] _connectionProxies;
        private ParallelConnectionProxy[][] _parallelConnectionProxies;

        private bool _isShutdown, _isDisposed;

        private StringTree[] _unitTestParameterizedLogEntries;

        //Dispose multi threaded.
        [ThreadStatic]
        private static DisposeConnectionProxyWorkItem _disposeConnectionProxyWorkItem;
        #endregion

        #region Properties
        public IConnectionProxy this[int index]
        {
            get { return _connectionProxies[index]; }
        }
        /// <summary>
        /// The connection proxies for parallely executed log entries are kept in this array.
        /// The index of the connection proxy for the log entry that is on the start of the parallel executed range
        /// is used to get (and set) the right connection proxies
        /// </summary>
        public ParallelConnectionProxy[][] ParallelConnectionProxies
        {
            get { return _parallelConnectionProxies; }
            set { _parallelConnectionProxies = value; }
        }
        public bool IsShutdown
        {
            get { return _isShutdown; }
        }
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }
        /// <summary>
        /// Not all connection proxies are used, set connection proxies will not remove entries, if you connect the proxies later on te ones that are not needed will be closed.
        /// </summary>
        public int UsedConnectionProxies
        {
            get { return _usedConnectionProxies; }
        }
        public int PoolSize
        {
            get { return (_connectionProxies == null) ? 0 : _connectionProxies.Length; }
        }
        #endregion

        #region Con-/Destructor
        /// <summary>
        /// Used for stresstesting, the size of the pool must equal or be greater than the concurrent users count.
        /// Note, InitializeGivenConnection() must be called to be able to use this pool.
        /// </summary>
        /// <param name="connection"></param>
        public ConnectionProxyPool(Connection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            _connection = connection;
        }
        ~ConnectionProxyPool()
        {
            Dispose();
        }
        #endregion

        #region Functions
        public int IndexOf(IConnectionProxy connectionProxy)
        {
            if (_connectionProxies != null) //For Test Connection
                for (int i = 0; i != _connectionProxies.Length; i++)
                    if (_connectionProxies[i] == connectionProxy)
                        return i;
            return 0;
        }

        /// <summary>
        /// Use this before using everything else.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="deleteTempFiles">To delete the temp files if compiled with debug == true (generates temp files). (Fx can be called afterwards)</param>
        /// <returns></returns>
        public CompilerResults CompileConnectionProxyClass(bool debug, bool deleteTempFiles = true)
        {
            //Otherwise probing privatePath will not work --> monitorsources and ConnectionProxyPrerequisites sub folder.
            System.IO.Directory.SetCurrentDirectory(Application.StartupPath);

            CompilerResults compilerResults = null;
            _connectionProxyAssembly = _compilerUnit.Compile(_connection.BuildConnectionProxyClass(), debug, out compilerResults);
            if (deleteTempFiles)
                _compilerUnit.DeleteTempFiles();
            return compilerResults;
        }
        public void DeleteTempFiles()
        {
            if (_compilerUnit != null)
                _compilerUnit.DeleteTempFiles();
        }

        /// <summary>
        /// The connection proxy class must be compiled with debug == true. After this is finished al will be disposed.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="logEntryIndex">-1 == all log entries.</param>
        /// <param name="threadCount"></param>
        public void TestCode(Log log, int logEntryIndex = -1, int threadCount = 1)
        {
            if (log == null || log.IsEmpty)
                throw new Exception("Please select a log.");

            if (log.Count == 0)
                throw new Exception("This log does not contain any log entries.");

            if (logEntryIndex < -1 || logEntryIndex > log.Count)
                throw new ArgumentOutOfRangeException("logEntryIndex");

            if (threadCount < 1)
                throw new ArgumentOutOfRangeException("threadCount");

            Thread t = new Thread(delegate()
            {
                try
                {
                    var unitTestParameterizedLogEntries = new List<StringTree>();
                    log.ApplyLogRuleSet();


                    if (logEntryIndex == -1)
                    {
                        foreach (StringTree st in log.GetParameterizedStructure())
                            unitTestParameterizedLogEntries.Add(st);
                    }
                    else
                    {
                        int i = 0;
                        foreach (StringTree st in log.GetParameterizedStructure())
                            if (i++ == logEntryIndex)
                            {
                                unitTestParameterizedLogEntries.Add(st);
                                break;
                            }
                        if (unitTestParameterizedLogEntries.Count == 0)
                            throw new ArgumentOutOfRangeException("logEntryIndex");
                    }

                    _unitTestParameterizedLogEntries = unitTestParameterizedLogEntries.ToArray();

                    SetAndConnectConnectionProxies(threadCount);

                    StresstestThreadPool threadPool = new StresstestThreadPool(TestWork);
                    threadPool.SetThreads(threadCount);
                    threadPool.DoWorkAndWaitForIdle();
                }
                catch (Exception ex)
                {
                    if (!_isDisposed && TestWorkException != null)
                        TestWorkException(this, new TestWorkExceptionEventArgs(new Exception("Test work exception!\n" + ex)));
                }
                finally
                {
                    Dispose();
                    if (TestWorkFinished != null)
                        TestWorkFinished(this, null);
                }
            });
            t.IsBackground = true;
            t.Start();
        }
        /// <summary>
        /// Uses IConnectionProxy.TestSendAndReceive instead of IConnectionProxy.SendAndReceive;
        /// </summary>
        /// <param name="threadIndex"></param>
        /// <param name="patternIndex"></param>
        internal void TestWork(int threadIndex)
        {
            DateTime sentAt = DateTime.Now;
            TimeSpan timeToLastByte = new TimeSpan(); ;
            Exception exception = null;
            IConnectionProxy connectionProxy = this[threadIndex];
            for (int i = 0; i != _unitTestParameterizedLogEntries.Length; i++)
            {
                StringTree parameterizedLogEntry = _unitTestParameterizedLogEntries[i];
                try
                {
                    connectionProxy.TestSendAndReceive(parameterizedLogEntry, out sentAt, out timeToLastByte, out exception);
                    Debug.WriteLine(string.Format("{0}: {1}", Thread.CurrentThread.Name, parameterizedLogEntry.CombineValues()));
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                if (exception != null)
                    Debug.WriteLine(exception);

                if (LogEntryTested != null)
                    foreach (EventHandler<LogEntryTestedEventArgs> del in LogEntryTested.GetInvocationList())
                        del.BeginInvoke(this, new LogEntryTestedEventArgs(Thread.CurrentThread.Name, parameterizedLogEntry, sentAt, timeToLastByte, exception), null, null);
            }
        }
        /// <summary>
        /// Tests if a single connection can be establisht.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public void TestConnection(out string error)
        {
            error = null;
            IConnectionProxy connectionProxy = null;
            try
            {
                connectionProxy = _connectionProxyAssembly.CreateInstance("vApus.Stresstest.ConnectionProxy") as IConnectionProxy;
                connectionProxy.SetParent(this);
                connectionProxy.TestConnection(out error);
            }
            catch (Exception ex)
            {
                error = ex.ToString();
            }
            try
            {
                if (connectionProxy != null)
                    connectionProxy.Dispose();
            }
            catch { }
        }
        /// <summary>
        /// Will dispose the current connection proxies and open new ones.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="parallelConnectionsCount">The count of how many log entries are executed in parallel and need a unique connection proxy</param>
        public void SetAndConnectConnectionProxies(int count, int parallelConnectionsCount = 0)
        {
            DisposeConnectionProxies();

            _usedConnectionProxies = count;
            _connectionProxies = new IConnectionProxy[_usedConnectionProxies];
            _parallelConnectionProxies = new ParallelConnectionProxy[_usedConnectionProxies][];

            Exception exception;
            for (int i = 0; i != _usedConnectionProxies; i++)
            {
                if (_isDisposed || _isShutdown)
                    return;

                IConnectionProxy connectionProxy = _connectionProxyAssembly.CreateInstance("vApus.Stresstest.ConnectionProxy") as IConnectionProxy;
                connectionProxy.SetParent(this);
                _connectionProxies[i] = connectionProxy;

                exception = Connect(connectionProxy, i);
                if (exception != null)
                    throw (exception);

                if (parallelConnectionsCount != 0)
                {
                    var pcps = new ParallelConnectionProxy[parallelConnectionsCount];
                    for (int pi = 0; pi != parallelConnectionsCount; pi++)
                    {
                        if (_isDisposed || _isShutdown)
                            return;

                        IConnectionProxy cp = _connectionProxyAssembly.CreateInstance("vApus.Stresstest.ConnectionProxy") as IConnectionProxy;
                        cp.SetParent(this);

                        var pcp = new ParallelConnectionProxy(cp);
                        pcps[pi] = pcp;

                        exception = Connect(cp, i);
                        if (exception != null)
                            throw (exception);
                    }
                    _parallelConnectionProxies[i] = pcps;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionProxy"></param>
        /// <param name="index">Only used for messages, can be any value.</param>
        /// <returns></returns>
        private Exception Connect(IConnectionProxy connectionProxy, int index)
        {
            try
            {
                connectionProxy.OpenConnection();
                if (!connectionProxy.IsConnectionOpen)
                    return ReconnectOnce(connectionProxy, index, null);
            }
            catch (Exception ex)
            {
                return ReconnectOnce(connectionProxy, index, ex);
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionProxy"></param>
        /// <param name="index">Only used for messages, can be any value.</param>
        /// <returns></returns>
        private Exception ReconnectOnce(IConnectionProxy connectionProxy, int index, Exception ex)
        {
            if (ex == null)
                LogWrapper.LogByLevel("Connection for connection proxy #" + index + " could not be opened, trying to make a new one. (Expensive operation!)", LogLevel.Warning);
            else
                LogWrapper.LogByLevel("Connection for connection proxy #" + index + " could not be opened, trying to make a new one. (Expensive operation!)\n" + ex, LogLevel.Warning);
            try
            {
                connectionProxy = _connectionProxyAssembly.CreateInstance("vApus.Stresstest.ConnectionProxy") as IConnectionProxy;
                connectionProxy.SetParent(this);
                connectionProxy.OpenConnection();
                if (!connectionProxy.IsConnectionOpen)
                {
                    ex = new Exception("Reconnecting failed for connection proxy " + index + "!");
                    LogWrapper.LogByLevel(ex, LogLevel.Error);
                    return ex;
                }
            }
            catch (Exception e)
            {
                ex = new Exception("Reconnecting failed for connection proxy #" + index + "!\n" + e);
                LogWrapper.LogByLevel(ex, LogLevel.Error);
                return ex;
            }
            return null;
        }
        /// <summary>
        /// Dispose all connection proxies and clears the pool.
        /// </summary>
        public void ShutDown()
        {
            if (!_isShutdown)
            {
                _isShutdown = true;
                DisposeConnectionProxies();
            }
        }

        ///// <summary>
        ///// Single threaded solution
        ///// </summary>
        //private void DisposeConnectionProxies()
        //{
        //    try
        //    {
        //        if (_connectionProxies != null)
        //            for (int i = 0; i != _connectionProxies.Length; i++)
        //            {
        //                var cp = _connectionProxies[i];
        //                DisposeConnectionProxy(cp, i, false);
        //            }

        //        int i = 0;
        //        if (_parallelConnectionProxies != null)
        //            foreach (var pcp in _parallelConnectionProxies)
        //                if (pcp != null)
        //                    foreach (var pConnectionProxy in pcp)
        //                        DisposeConnectionProxy(pConnectionProxy, i++, true);
        //    }
        //    catch { }

        //    _connectionProxies = null;
        //    _parallelConnectionProxies = null;
        //}
        //public void DisposeConnectionProxy(IConnectionProxy connectionProxy, int index, bool parallel)
        //{
        //    if (connectionProxy != null)
        //        try
        //        {
        //            try
        //            {
        //                if (connectionProxy.IsConnectionOpen)
        //                    connectionProxy.CloseConnection();
        //            }
        //            catch { throw; }
        //            finally
        //            {
        //                if (!connectionProxy.IsDisposed)
        //                    connectionProxy.Dispose();
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            if (parallel)
        //                LogWrapper.LogByLevel("Parallel connection #" + index + " could not be closed and/or disposed.\n" + ex, LogLevel.Error);
        //            else
        //                LogWrapper.LogByLevel("Connection #" + index + " could not be closed and/or disposed.\n" + ex, LogLevel.Error);
        //        }
        //}

        /// <summary>
        /// Multi threaded solution
        /// </summary>
        private void DisposeConnectionProxies()
        {
            try
            {
                //Dispose multi threaded.
                if (_connectionProxies != null)
                {
                    AutoResetEvent disposeWaitHandle = new AutoResetEvent(false);
                    int count = _connectionProxies.Length;
                    int i = 0;
                    foreach (var cp in _connectionProxies)
                    {
                        object[] args = new object[] { cp, i, false };

                        Thread t = new Thread(delegate(object state)
                        {
                            try
                            {
                                _disposeConnectionProxyWorkItem = new DisposeConnectionProxyWorkItem();
                                _disposeConnectionProxyWorkItem.DisposeConnectionProxy(state as object[]);

                                if (Interlocked.Increment(ref i) == count)
                                    disposeWaitHandle.Set();
                            }
                            catch { }

                        });
                        t.IsBackground = true;
                        t.Start(args);
                    }
                    if (count != 0)
                        disposeWaitHandle.WaitOne();
                    disposeWaitHandle.Dispose();
                    disposeWaitHandle = null;
                }
                //Dispose multi threaded.
                if (_parallelConnectionProxies != null)
                {
                    AutoResetEvent disposeWaitHandle = new AutoResetEvent(false);
                    int count = 0;
                    foreach (var pcp in _parallelConnectionProxies)
                        if (pcp != null)
                            count += pcp.Length;

                    int i = 0;
                    foreach (var pcp in _parallelConnectionProxies)
                        if (pcp != null)
                            foreach (var pConnectionProxy in pcp)
                            {
                                object[] args = new object[] { pConnectionProxy, i, true };
                                Thread t = new Thread(delegate(object state)
                                {
                                    _disposeConnectionProxyWorkItem = new DisposeConnectionProxyWorkItem();
                                    _disposeConnectionProxyWorkItem.DisposeConnectionProxy(state as object[]);

                                    if (Interlocked.Increment(ref i) == count)
                                        disposeWaitHandle.Set();
                                });
                                t.IsBackground = true;
                                t.Start(args);
                            }
                    if (count != 0)
                        disposeWaitHandle.WaitOne();
                    disposeWaitHandle.Dispose();
                    disposeWaitHandle = null;
                }
            }
            catch { }

            try
            {
                _connectionProxies = null;
            }
            catch { }
            try
            {
                _parallelConnectionProxies = null;
            }
            catch { }
        }
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                ShutDown();
                _compilerUnit.DeleteTempFiles();
                _compilerUnit = null;

                _connection = null;
                _connectionProxyAssembly = null;
            }
        }
        #endregion

        public class LogEntryTestedEventArgs : EventArgs
        {
            public string Thread;
            public StringTree ParameterizedLogEntry;
            public DateTime SentAt;
            public TimeSpan TimeToLastByte;
            public Exception Exception;

            public LogEntryTestedEventArgs(string thread, StringTree parameterizedLogEntry, DateTime sentAt, TimeSpan timeToLastByte, Exception exception = null)
            {
                Thread = thread;
                ParameterizedLogEntry = parameterizedLogEntry;
                SentAt = sentAt;
                TimeToLastByte = timeToLastByte;
                Exception = exception;
            }
        }
        public class TestWorkExceptionEventArgs : EventArgs
        {
            public Exception Exception;

            public TestWorkExceptionEventArgs(Exception exception)
            {
                Exception = exception;
            }
        }
        private class DisposeConnectionProxyWorkItem
        {
            private static object _lock = new object();

            public void DisposeConnectionProxy(object[] args)
            {
                IConnectionProxy connectionProxy = null;
                if (args[0] is IConnectionProxy)
                    connectionProxy = args[0] as IConnectionProxy;
                else if (args[0] is ParallelConnectionProxy)
                    connectionProxy = ((ParallelConnectionProxy)args[0]).ConnectionProxy;

                int index = (int)args[1];
                bool parallel = (bool)args[2];

                if (connectionProxy != null)
                    try
                    {
                        try
                        {
                            if (connectionProxy.IsConnectionOpen)
                                connectionProxy.CloseConnection();
                        }
                        catch { throw; }
                        finally
                        {
                            if (!connectionProxy.IsDisposed)
                                connectionProxy.Dispose();
                        }

                    }
                    catch (Exception ex)
                    {
                        lock (_lock)
                        {
                            if (parallel)
                                LogWrapper.LogByLevel("Parallel connection #" + index + " could not be closed and/or disposed.\n" + ex, LogLevel.Error);
                            else
                                LogWrapper.LogByLevel("Connection #" + index + " could not be closed and/or disposed.\n" + ex, LogLevel.Error);
                        }
                    }
            }
        }
    }
}