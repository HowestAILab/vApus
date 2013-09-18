/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Stresstest {
    /// <summary>
    ///     Used for stresstesting, the size of the pool must equal or be greater than the concurrent users count of Stresstest.
    ///     Note, CompileConnectionProxyClass must be called to be able to use this pool.
    /// </summary>
    public class ConnectionProxyPool : IDisposable {

        #region Fields
        [ThreadStatic]
        private static DisposeConnectionProxyWorkItem _disposeConnectionProxyWorkItem;

        private CompilerUnit _compilerUnit = new CompilerUnit();
        private Assembly _connectionProxyAssembly;

        private Connection _connection;

        private IConnectionProxy[] _connectionProxies;
        private ParallelConnectionProxy[][] _parallelConnectionProxies;
        private int _usedConnectionProxies;

        private bool _isShutdown, _isDisposed;
        #endregion

        #region Properties
        public IConnectionProxy this[int index] { get { return _connectionProxies[index]; } }
        /// <summary>
        ///     The connection proxies for parallely executed log entries are kept in this array.
        ///     The index of the connection proxy for the log entry that is on the start of the parallel executed range
        ///     is used to get (and set) the right connection proxies
        /// </summary>
        public ParallelConnectionProxy[][] ParallelConnectionProxies {
            get { return _parallelConnectionProxies; }
            set { _parallelConnectionProxies = value; }
        }

        /// <summary>
        /// Sometimes needed in connection proxy code, MaxConnectionLimit for HttpWebRequests for instance.
        /// </summary>
        public int PoolSize { get { return (_connectionProxies == null) ? 0 : _connectionProxies.Length; } }

        public bool IsDisposed {
            get { return _isDisposed; }
        }
        #endregion

        #region Con-/Destructor
        /// <summary>
        ///     Used for stresstesting, the size of the pool must equal or be greater than the concurrent users count of Stresstest.
        ///     Note, CompileConnectionProxyClass must be called to be able to use this pool.
        /// </summary>
        /// <param name="connection"></param>
        public ConnectionProxyPool(Connection connection) {
            if (connection == null)
                throw new ArgumentNullException("connection");
            _connection = connection;
        }
        ~ConnectionProxyPool() { Dispose(); }
        #endregion

        #region Functions

        public void Dispose() {
            if (!_isDisposed) {
                _isDisposed = true;
                ShutDown();
                _compilerUnit.DeleteTempFiles();
                _compilerUnit = null;

                _connection = null;
                _connectionProxyAssembly = null;
            }
        }

        /// <summary>
        ///     Use this before using everything else.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="deleteTempFiles">To delete the temp files if compiled with debug == true (generates temp files). (Fx can be called afterwards)</param>
        /// <returns></returns>
        public CompilerResults CompileConnectionProxyClass(bool debug, bool deleteTempFiles = true) {
            //Otherwise probing privatePath will not work --> monitorsources and ConnectionProxyPrerequisites sub folder.
            Directory.SetCurrentDirectory(Application.StartupPath);

            CompilerResults compilerResults = null;
            if (deleteTempFiles)
                _compilerUnit.DeleteTempFiles();

            _connectionProxyAssembly = _compilerUnit.Compile(_connection.BuildConnectionProxyClass(), debug,
                                                             out compilerResults);
            return compilerResults;
        }

        /// <summary>
        ///     Tests if a single connection can be establisht.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public void TestConnection(out string error) {
            error = null;
            IConnectionProxy connectionProxy = null;
            try {
                connectionProxy = _connectionProxyAssembly.CreateInstance("vApus.Stresstest.ConnectionProxy") as IConnectionProxy;
                connectionProxy.SetParent(this);
                connectionProxy.TestConnection(out error);
            } catch (Exception ex) {
                error = ex.ToString();
            }
            try {
                if (connectionProxy != null)
                    connectionProxy.Dispose();
            } catch {
            }
        }

        /// <summary>
        ///     Will dispose the current connection proxies and open new ones.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="parallelConnectionsCount">The count of how many log entries are executed in parallel and need a unique connection proxy</param>
        public void SetAndConnectConnectionProxies(int count, int parallelConnectionsCount = 0) {
            DisposeConnectionProxies();

            _usedConnectionProxies = count;
            _connectionProxies = new IConnectionProxy[_usedConnectionProxies];
            _parallelConnectionProxies = new ParallelConnectionProxy[_usedConnectionProxies][];

            Exception exception;
            for (int i = 0; i != _usedConnectionProxies; i++) {
                if (_isDisposed || _isShutdown)
                    return;

                var connectionProxy = _connectionProxyAssembly.CreateInstance("vApus.Stresstest.ConnectionProxy") as IConnectionProxy;
                connectionProxy.SetParent(this);
                _connectionProxies[i] = connectionProxy;

                exception = Connect(connectionProxy, i);
                if (exception != null)
                    throw (exception);

                if (parallelConnectionsCount != 0) {
                    var pcps = new ParallelConnectionProxy[parallelConnectionsCount];
                    for (int pi = 0; pi != parallelConnectionsCount; pi++) {
                        if (_isDisposed || _isShutdown)
                            return;

                        var cp =
                            _connectionProxyAssembly.CreateInstance("vApus.Stresstest.ConnectionProxy") as
                            IConnectionProxy;
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
        /// </summary>
        /// <param name="connectionProxy"></param>
        /// <param name="index">Only used for messages, can be any value.</param>
        /// <returns></returns>
        private Exception Connect(IConnectionProxy connectionProxy, int index) {
            try {
                connectionProxy.OpenConnection();
                if (!connectionProxy.IsConnectionOpen)
                    return ReconnectOnce(connectionProxy, index, null);
            } catch (Exception ex) {
                return ReconnectOnce(connectionProxy, index, ex);
            }
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="connectionProxy"></param>
        /// <param name="index">Only used for messages, can be any value.</param>
        /// <returns></returns>
        private Exception ReconnectOnce(IConnectionProxy connectionProxy, int index, Exception ex) {
            if (ex == null)
                LogWrapper.LogByLevel(
                    "Connection for connection proxy #" + index +
                    " could not be opened, trying to make a new one. (Expensive operation!)", LogLevel.Warning);
            else
                LogWrapper.LogByLevel(
                    "Connection for connection proxy #" + index +
                    " could not be opened, trying to make a new one. (Expensive operation!)\n" + ex, LogLevel.Warning);
            try {
                connectionProxy =
                    _connectionProxyAssembly.CreateInstance("vApus.Stresstest.ConnectionProxy") as IConnectionProxy;
                connectionProxy.SetParent(this);
                connectionProxy.OpenConnection();
                if (!connectionProxy.IsConnectionOpen) {
                    ex = new Exception("Reconnecting failed for connection proxy " + index + "!");
                    LogWrapper.LogByLevel(ex, LogLevel.Error);
                    return ex;
                }
            } catch (Exception e) {
                ex = new Exception("Reconnecting failed for connection proxy #" + index + "!\n" + e);
                LogWrapper.LogByLevel(ex, LogLevel.Error);
                return ex;
            }
            return null;
        }

        /// <summary>
        ///     Dispose all connection proxies and clears the pool.
        /// </summary>
        public void ShutDown() {
            if (!_isShutdown) {
                _isShutdown = true;
                DisposeConnectionProxies();
            }
        }

        /// <summary>
        ///     Multi threaded solution
        /// </summary>
        private void DisposeConnectionProxies() {
            try {
                //Dispose multi threaded.
                if (_connectionProxies != null) {
                    var disposeWaitHandle = new AutoResetEvent(false);
                    int count = _connectionProxies.Length;
                    int i = 0;
                    foreach (IConnectionProxy cp in _connectionProxies) {
                        var args = new object[] { cp, i, false };

                        var t = new Thread(delegate(object state) {
                            try {
                                _disposeConnectionProxyWorkItem = new DisposeConnectionProxyWorkItem();
                                _disposeConnectionProxyWorkItem.DisposeConnectionProxy(state as object[]);
                            } catch {
                            }

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
                //Dispose multi threaded.
                if (_parallelConnectionProxies != null) {
                    var disposeWaitHandle = new AutoResetEvent(false);
                    int count = 0;
                    foreach (var pcp in _parallelConnectionProxies)
                        if (pcp != null)
                            count += pcp.Length;

                    int i = 0;
                    foreach (var pcp in _parallelConnectionProxies)
                        if (pcp != null)
                            foreach (ParallelConnectionProxy pConnectionProxy in pcp) {
                                var args = new object[] { pConnectionProxy, i, true };
                                var t = new Thread(delegate(object state) {
                                    try {
                                        _disposeConnectionProxyWorkItem = new DisposeConnectionProxyWorkItem();
                                        _disposeConnectionProxyWorkItem.DisposeConnectionProxy(state as object[]);
                                    } catch {
                                    }

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
            } catch {
            }

            try { _connectionProxies = null; } catch {
            }
            try { _parallelConnectionProxies = null; } catch {
            }
        }
        #endregion

        /// <summary>
        /// For Handling disposing connection proxy work items in parallel and faster. (This can be an issue with not responding server apps). 
        /// </summary>
        private class DisposeConnectionProxyWorkItem {
            private static readonly object _lock = new object();
            public void DisposeConnectionProxy(object[] args) {
                IConnectionProxy connectionProxy = null;
                if (args[0] is IConnectionProxy)
                    connectionProxy = args[0] as IConnectionProxy;
                else if (args[0] is ParallelConnectionProxy)
                    connectionProxy = ((ParallelConnectionProxy)args[0]).ConnectionProxy;

                var index = (int)args[1];
                var parallel = (bool)args[2];

                if (connectionProxy != null)
                    try {
                        try {
                            if (connectionProxy.IsConnectionOpen)
                                connectionProxy.CloseConnection();
                        } catch {
                            throw;
                        } finally {
                            if (!connectionProxy.IsDisposed)
                                connectionProxy.Dispose();
                        }
                    } catch (Exception ex) {
                        lock (_lock) {
                            if (parallel)
                                LogWrapper.LogByLevel(
                                    "Parallel connection #" + index + " could not be closed and/or disposed.\n" + ex,
                                    LogLevel.Error);
                            else
                                LogWrapper.LogByLevel(
                                    "Connection #" + index + " could not be closed and/or disposed.\n" + ex,
                                    LogLevel.Error);
                        }
                    }
            }
        }
    }
}