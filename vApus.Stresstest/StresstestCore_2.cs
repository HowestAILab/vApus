/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest
{
    public class StresstestCore : IDisposable
    {
        #region Fields
        /// <summary>
        /// For handling fatal errors of a thread.
        /// </summary>
        private object _lock = new object();

        /// <summary>
        /// To be able to execute log entries parallel.
        /// </summary>
        [ThreadStatic]
        private static SyncAndAsyncWorkItem _syncAndAsyncWorkItem;

        private Stopwatch _sw = new Stopwatch();

        private Stresstest _stresstest;
        private ConnectionProxyPool _connectionProxyPool;
        private StresstestThreadPool _threadPool;

        private TestPatternsAndDelaysGenerator _testPatternsAndDelaysGenerator;
        private LogEntry[] _logEntries;
        private TestableLogEntry[][] _testableLogEntries;
        private int[][] _delays;

        private AutoResetEvent _sleepWaitHandle = new AutoResetEvent(false);
        private AutoResetEvent _runSynchronizationContinueWaitHandle = new AutoResetEvent(false);
        private RunSynchronization _runSynchronization;
        private int _currentConcurrentUsersIndex, _currentConcurrentUsers, _parallelConnectionsModifier;
        private int _currentRun = -1;
        private int[] _runsConcurrentUsers;

        private volatile bool _failed, _cancel, _completed, _break, _isDisposed;
        private volatile bool _workDoneOnce;

        private Exception _exceptionOnFailure;

        private StresstestResults _stresstestResults;
        private ConcurrentUsersResult _concurrentUsersResult;
        private PrecisionResult _precisionResult;
        private RunResult _runResult;
        #endregion

        #region Properties
        public RunSynchronization RunSynchronization
        {
            get { return _runSynchronization; }
            set { _runSynchronization = value; }
        }
        public Exception ExceptionOnFailure
        {
            get { return _exceptionOnFailure; }
        }
        public StresstestResults StresstestResults
        {
            get { return _stresstestResults; }
        }
        public int BusyThreadCount
        {
            get { return _threadPool == null ? 0 : _threadPool.BusyThreadCount; }
        }
        public int UsedThreadCount
        {
            get { return _threadPool == null ? 0 : _threadPool.UsedThreadCount; }
        }
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }
        #endregion

        #region Con-/Destructor
        public StresstestCore(Stresstest stresstest)
        {
            _stresstest = stresstest;
        }
        ~StresstestCore()
        {
            Dispose();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Do this before everything else.
        /// </summary>
        public void InitializeTest()
        {
            InvokeMessage("Initializing the Test.");
            InitializeLog();
            InitializeConnectionProxyPool();
            DetermineRuns();
        }
       /// <summary>
        /// Will apply the log rule set.
        /// Parallel connections will be determined here also.
        /// </summary> 
        private void InitializeLog()
        {
            InvokeMessage("Initializing Log...");
            _sw.Start();
            if (_stresstest.Log.LogRuleSet.IsEmpty)
            {
                Exception ex = new Exception("No rule set has been assigned to the selected log.");
                LogWrapper.LogByLevel(ex.ToString(), LogLevel.Error);
                throw ex;
            }
            if (_stresstest.Log.Count == 0)
            {
                Exception ex = new Exception("There are no entries in the selected log.");
                LogWrapper.LogByLevel(ex.ToString(), LogLevel.Error);
                throw ex;
            }

            _stresstest.Log.ApplyLogRuleSet();

            //Parallel connections
            _parallelConnectionsModifier = 0;
            List<LogEntry> logEntries = new List<LogEntry>();
            foreach (LogEntry logEntry in _stresstest.Log.GetAllLogEntries())
            {
                logEntries.Add(logEntry);
                if (logEntry.ExecuteParallel)
                    ++_parallelConnectionsModifier;
            }
            --_parallelConnectionsModifier;

            _logEntries = logEntries.ToArray();

            _testPatternsAndDelaysGenerator = new TestPatternsAndDelaysGenerator
            (
                _logEntries,
                _stresstest.Shuffle,
                _stresstest.Distribute,
                _stresstest.MinimumDelay,
                _stresstest.MaximumDelay
            );
            _sw.Stop();
            InvokeMessage(string.Format(" ...Log Initialized in {0}.", _sw.Elapsed.ToLongFormattedString()));
            _sw.Reset();
        }
        private void InitializeConnectionProxyPool()
        {
            InvokeMessage("Initialize Connection Proxy Pool...");
            _sw.Start();
            _connectionProxyPool = new ConnectionProxyPool(_stresstest.Connection);
            CompilerResults compilerResults = _connectionProxyPool.CompileConnectionProxyClass(false);
            if (compilerResults.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder("Failed at compiling the connection proxy class:");
                sb.AppendLine();
                foreach (CompilerError compileError in compilerResults.Errors)
                {
                    sb.AppendFormat("Error number {0}, Line {1}, Column {2}: {3}", compileError.ErrorNumber, compileError.Line, compileError.Column, compileError.ErrorText);
                    sb.AppendLine();
                }
                _connectionProxyPool.Dispose();
                _connectionProxyPool = null;
                Exception ex = new Exception(sb.ToString());
                LogWrapper.LogByLevel(ex.ToString(), LogLevel.Error);
                // This handles notification to the user.
                throw ex;
            }
            string error;
            _connectionProxyPool.TestConnection(out error);
            if (error != null)
            {
                _connectionProxyPool.Dispose();
                _connectionProxyPool = null;
                Exception ex = new Exception(error);
                LogWrapper.LogByLevel(ex.ToString(), LogLevel.Error);
                throw ex;
            }
            _sw.Stop();
            InvokeMessage(string.Format(" ...Connection Proxy Pool Inititialized in {0}.", _sw.Elapsed.ToLongFormattedString()));
            _sw.Reset();
        }
        private void DetermineRuns()
        {
            InvokeMessage("Determining Runs...");
            _sw.Start();
            _runsConcurrentUsers = new int[_stresstest.ConcurrentUsers.Length];
            for (int i = 0; i != _stresstest.ConcurrentUsers.Length; i++)
            {
                int runs = _stresstest.DynamicRunMultiplier / _stresstest.ConcurrentUsers[i];
                if (runs == 0)
                    runs = 1;
                _runsConcurrentUsers[i] = runs;
            }
            _sw.Stop();
            InvokeMessage(string.Format(" ...Runs Determined in {0}.", _sw.Elapsed.ToLongFormattedString()));
            _sw.Reset();
        }
        public StresstestResult ExecuteStresstest()
        {
            SetStresstestStarted();

            _cancel = false;
            _failed = false;

            BeginExecuteForConcurrentUsers();
            ExecuteWork();
            EndExecuteForConcurrentUsers();

            return Completed();
        }
        private void BeginExecuteForConcurrentUsers()
        {
            _currentConcurrentUsers = _stresstest.ConcurrentUsers[_currentConcurrentUsersIndex];

            SetThreadPool((int)_currentConcurrentUsers);
            if (_cancel)
                return;

            SetConcurrentUsersStarted();

            _break = false;
            _workDoneOnce = false;
        }
        private void ExecuteWork()
        {
            try
            {
            Loop:
                for (int p = 0; p != _stresstest.Precision; p++)
                {
                    if (_cancel || _break)
                        break;

                    SetPrecisionStarted(p);

                    for (int r = 0; r != _runsConcurrentUsers[_currentConcurrentUsersIndex]; r++)
                    {
                        if (_cancel || _break)
                            break;

                        DetermineTestableLogEntriessAndDelays();

                        if (_cancel || _break)
                            break;

                        SetConnectionProxyPool((int)_currentConcurrentUsers);

                        if (_cancel || _break)
                            break;

                        if (_runSynchronization != RunSynchronization.None && !_cancel)
                        {
                            InvokeMessage("Waiting for continue...");
                            SetRunStarted(r);
                            _runSynchronizationContinueWaitHandle.WaitOne();
                        }
                        else
                        {
                            SetRunStarted(r);
                        }

                        if (_cancel || _break)
                            break;


                        try
                        {
                            _threadPool.DoWorkAndWaitForIdle();
                        }
                        catch (Exception ex)
                        {
                            if (!_isDisposed)
                                InvokeMessage("|----> |Run Not Finished Succesfully!\n|Thread Pool Exception:\n" + ex, Color.Red, LogLevel.Error);
                        }
                        finally
                        {
                            InvokeMessage("|----> |Run Finished!", Color.LightGreen);
                        }


                        _runResult.StopTimeMeasurement();


                        if (_cancel || _break)
                            break;
                    }
                    _precisionResult.StopTimeMeasurement();
                }

                //Include this in set run stopped maybe?
                if (!_workDoneOnce)
                    SetWorkDoneOnce();

                if (_runSynchronization == RunSynchronization.BreakOnLastFinished && !(_break || _cancel))
                {
                    InvokeMessage("Looping...");
                    _stresstestResults.IncrementTotalLogEntries();
                    goto Loop;
                }
                _concurrentUsersResult.StopTimeMeasurement();
            }
            catch (Exception ex)
            {
                _exceptionOnFailure = ex;
                _failed = true;
            }
        }

        #region Work
        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadIndex">Used for choosing the right user result for adding the log entry result.</param>
        /// <param name="testAndDelayPatternIndex">Used for choosing the right test- and delay pattern.</param>
        //public void Work(int threadIndex)
        //{
        //    try
        //    {
        //        IConnectionProxy connectionProxy = _connectionProxyPool[threadIndex];
        //        if (!connectionProxy.IsConnectionOpen)
        //            connectionProxy.OpenConnection();
        //        TestableLogEntry[] testableLogEntries = _testableLogEntries[threadIndex];
        //        int[] delays = _delays[threadIndex];
        //        for (int i = 0; i != testableLogEntries.Length; i++)
        //        {
        //            DateTime sentAt = DateTime.Now;
        //            TimeSpan timeToLastByte = new TimeSpan();
        //            Exception exception;

        //            if (_cancel || _break)
        //            {
        //                _sleepWaitHandle.Set();
        //                break;
        //            }

        //            TestableLogEntry testableLogEntry = testableLogEntries[i];
        //            int delay = delays[i];

        //            try
        //            {
        //                if (connectionProxy == null || connectionProxy.IsDisposed)
        //                    exception = new Exception("Connectionproxy is disposed. Metrics for this log entry (" + testableLogEntry.ParameterizedLogEntryString + ") are not correct.");
        //                else
        //                    connectionProxy.SendAndReceive(testableLogEntry.ParameterizedLogEntry, out sentAt, out timeToLastByte, out exception);
        //            }
        //            catch (Exception ex)
        //            {
        //                exception = new Exception("An error in the connection proxy has occured, please check the connection proxy code for errors; Metrics for this log entry (" + testableLogEntry.ParameterizedLogEntryString + ") are not correct:\n" + ex.ToString());
        //            }

        //            UserResult result = _runResult[threadIndex];
        //            result.User = Thread.CurrentThread.Name;

        //            result.SetLogEntryResultAt(i, new LogEntryResult(testableLogEntry.LogEntryIndex, testableLogEntry.ParameterizedLogEntryString, testableLogEntry.UserAction, sentAt, timeToLastByte, delay, exception));

        //            if (delay != 0 && !(_cancel || _break))
        //                _sleepWaitHandle.WaitOne(delay);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        InvokeMessage("Work failed for " + Thread.CurrentThread.Name + ".\n" + e.ToString(), LogLevel.Error);
        //    }
        //}
        public void Work(int threadIndex)
        {
            try
            {
                IConnectionProxy connectionProxy = _connectionProxyPool[threadIndex];
                if (!connectionProxy.IsConnectionOpen)
                    connectionProxy.OpenConnection();

                TestableLogEntry[] testableLogEntries = _testableLogEntries[threadIndex];
                int[] delays = _delays[threadIndex];

                int testableLogEntryIndex = 0;
                while (testableLogEntryIndex != testableLogEntries.Length)
                {
                    if (_cancel || _break)
                    {
                        _sleepWaitHandle.Set();
                        return;
                    }

                    int incrementIndex;
                    ExecuteLogEntry(threadIndex, testableLogEntryIndex, connectionProxy, testableLogEntries, delays, out incrementIndex);
                    testableLogEntryIndex += incrementIndex;
                }
            }
            catch (Exception e)
            {
                InvokeMessage("Work failed for " + Thread.CurrentThread.Name + ".\n" + e.ToString(), Color.Red, LogLevel.Error);
            }
        }

        /// <summary>
        /// Decides if the log entry must be executed in parallel with others or not.
        /// </summary>
        /// <param name="threadIndex"></param>
        /// <param name="testableLogEntryIndex"></param>
        /// <param name="incrementIndex">Can be greater than 1 if some are parallel executed.</param>
        private void ExecuteLogEntry
            (
                int threadIndex,
                int testableLogEntryIndex,
                IConnectionProxy connectionProxy,
                TestableLogEntry[] testableLogEntries,
                int[] delays,
                out int incrementIndex
            )
        {
            incrementIndex = 1;

            if (_cancel || _break)
            {
                _sleepWaitHandle.Set();
                return;
            }

            TestableLogEntry testableLogEntry = testableLogEntries[testableLogEntryIndex];

            //Parallel
            if (testableLogEntry.ExecuteParallel)
            {
                // Make the range;
                int testableLogEntriesLength = testableLogEntries.Length;
                int exclusiveEnd = testableLogEntryIndex + 1;
                List<TestableLogEntry> parallelTestableLogEntries = new List<TestableLogEntry>();
                while (testableLogEntries[exclusiveEnd].ExecuteParallel)
                {
                    parallelTestableLogEntries.Add(testableLogEntries[exclusiveEnd]);
                    ++exclusiveEnd;
                    ++incrementIndex;
                    if (exclusiveEnd == testableLogEntriesLength)
                        break;
                }

                //Do work parallel if needed.
                if (incrementIndex != 1)
                {
                    string threadName = Thread.CurrentThread.Name + " [Parallel #";

                    //Get the connection proxies
                    var cp = _connectionProxyPool.ParallelConnectionProxies[threadIndex];
                    var parallelConnectionProxies = new IConnectionProxy[cp.Length + 1];
                    parallelConnectionProxies[0] = connectionProxy;
                    for (int i = 0; i != cp.Length; i++)
                        parallelConnectionProxies[i + 1] = cp[i];

                    //Do work here
                    int cpIndex = -1, pThreadIndex = 0;
                    Parallel.For(testableLogEntryIndex, exclusiveEnd, delegate(int pleIndex)
                    {
                        if (Thread.CurrentThread.Name == null)
                            Thread.CurrentThread.Name = threadName + Interlocked.Increment(ref pThreadIndex) + "]";

                        SyncAndAsyncWorkItem syncAndAsyncWorkItem = new SyncAndAsyncWorkItem();
                        syncAndAsyncWorkItem.ExecuteLogEntry(this, _sleepWaitHandle, _runResult, threadIndex, pleIndex, testableLogEntries[pleIndex], parallelConnectionProxies[Interlocked.Increment(ref cpIndex)], delays[pleIndex]);

                    });

                    //
                    return;
                    //
                }
            }

            //Sync
            if (_syncAndAsyncWorkItem == null)
            {
                _syncAndAsyncWorkItem = new SyncAndAsyncWorkItem();
            }

            _syncAndAsyncWorkItem.ExecuteLogEntry(this, _sleepWaitHandle, _runResult, threadIndex, testableLogEntryIndex, testableLogEntry, connectionProxy, delays[testableLogEntryIndex]);
        }
        private class SyncAndAsyncWorkItem
        {
            public void ExecuteLogEntry
                (
                    StresstestCore stresstestCore,
                    AutoResetEvent sleepWaitHandle,
                    RunResult runResult,
                    int threadIndex,
                    int testableLogEntryIndex,
                    TestableLogEntry testableLogEntry,
                    IConnectionProxy connectionProxy,
                    int delay
                )
            {
                DateTime sentAt = DateTime.Now;
                TimeSpan timeToLastByte = new TimeSpan();
                Exception exception = null;

                if (stresstestCore._cancel || stresstestCore._break)
                {
                    sleepWaitHandle.Set();
                    return;
                }

                try
                {
                    if (connectionProxy == null || connectionProxy.IsDisposed)
                        exception = new Exception("Connectionproxy is disposed. Metrics for this log entry (" + testableLogEntry.ParameterizedLogEntryString + ") are not correct.");
                    else
                        connectionProxy.SendAndReceive(testableLogEntry.ParameterizedLogEntry, out sentAt, out timeToLastByte, out exception);
                }
                catch (Exception ex)
                {
                    try
                    {
                        connectionProxy.Dispose();
                    }
                    catch { }
                    connectionProxy = null;
                    exception = new Exception("An error in the connection proxy has occured (and is now disposed) due to for instance a time out or a bug in the connection proxy code; Metrics for this log entry (" + testableLogEntry.ParameterizedLogEntryString + ") are not correct:\n" + ex.ToString());
                    throw (exception);
                }
                finally
                {
                    UserResult result = runResult[threadIndex];
                    result.User = Thread.CurrentThread.Name;

                    result.SetLogEntryResultAt(testableLogEntryIndex, new LogEntryResult(testableLogEntry.LogEntryIndex, testableLogEntry.ParameterizedLogEntryString, testableLogEntry.UserActionIndex, testableLogEntry.UserAction, sentAt, timeToLastByte, delay, exception));

                    if (delay != 0 && !(stresstestCore._cancel || stresstestCore._break))
                        sleepWaitHandle.WaitOne(delay);
                }
            }
        }
        #endregion

        private void EndExecuteForConcurrentUsers()
        {
            if (_cancel || _failed)
                return;


            if (_currentConcurrentUsersIndex + 1 != _stresstest.ConcurrentUsers.Length)
            {
                //Not the place for this but I need the check above.
                if (_runSynchronization != RunSynchronization.None)
                {
                    InvokeMessage("Waiting for continue...");
                    SetRunStopped();
                    _runSynchronizationContinueWaitHandle.WaitOne();
                }
                //Increment this.
                ++_currentConcurrentUsersIndex;
                BeginExecuteForConcurrentUsers();
                ExecuteWork();
                EndExecuteForConcurrentUsers();
            }
        }
        private StresstestResult Completed()
        {
            _completed = true;
            _connectionProxyPool.ShutDown();
            DisposeThreadPool();
            _connectionProxyPool.Dispose();

            _stresstestResults.StopTimeMeasurement();
            if (_cancel)
                return StresstestResult.Cancelled;
            if (_failed)
                return StresstestResult.Error;
            return StresstestResult.Ok;
        }

        public void Cancel()
        {
            if (!_cancel)
            {
                _cancel = true;
                _sleepWaitHandle.Set();
                _runSynchronizationContinueWaitHandle.Set();
                if (_threadPool != null)
                    DisposeThreadPool();
                if (_stresstestResults != null)
                    _stresstestResults.StopTimeMeasurement();
            }
        }
        public void Break(int atIndex)
        {
            if (_currentConcurrentUsersIndex == atIndex && !_break && !(_completed | _cancel | _failed))
            {
                InvokeMessage("Breaking the execution for the current run...");

                _break = true;
                _sleepWaitHandle.Set();
            }
        }
        public void Continue(int fromIndex)
        {
            if (_currentConcurrentUsersIndex == fromIndex && !(_completed | _cancel | _failed))
            {
                _break = false;
                _runSynchronizationContinueWaitHandle.Set();
            }
        }

        private void DetermineTestableLogEntriessAndDelays()
        {
            try
            {
                InvokeMessage(string.Format("       |Determining Test Patterns and Delays for {0} Concurrent Users...", _currentConcurrentUsers));
                _sw.Start();
                var testableLogEntries = new List<TestableLogEntry[]>();
                var delays = new List<int[]>();

                Random delaysRandom = new Random(DateTime.Now.Millisecond);

                for (int t = 0; t != _currentConcurrentUsers; t++)
                {
                    if (_cancel || _break)
                        return;

                    int[] testPatternIndices, delayPattern;
                    _testPatternsAndDelaysGenerator.GetPatterns(out testPatternIndices, out delayPattern);

                    var tle = new TestableLogEntry[testPatternIndices.Length];
                    int index = 0;

                    var parameterizedStructure = _stresstest.Log.GetParameterizedStructure();

                    for (int i = 0; i != testPatternIndices.Length; i++)
                    {
                        if (_cancel || _break)
                            return;

                        int j = testPatternIndices[i];

                        LogEntry logEntry = _logEntries[j];

                        string logEntryIndex = string.Empty;
                        StringTree parameterizedLogEntry = parameterizedStructure[j];
                        string userAction = string.Empty;
                        int userActionIndex = 0;

                        UserAction parent = logEntry.Parent as UserAction;
                        if (parent == null)
                        {
                            logEntryIndex = logEntry.Index.ToString();
                            userActionIndex = logEntry.Index;
                            userAction = "None defined " + userActionIndex;
                        }
                        else
                        {
                            logEntryIndex = parent.Index + "." + logEntry.Index;
                            userActionIndex = parent.Index;
                            userAction = parent.ToString();
                        }

                        tle[index++] = new TestableLogEntry(logEntryIndex, parameterizedLogEntry, userActionIndex, userAction, logEntry.ExecuteParallel);
                    }

                    testableLogEntries.Add(tle);
                    delays.Add(delayPattern);
                }


                _testableLogEntries = testableLogEntries.ToArray();
                _delays = delays.ToArray();

                _sw.Stop();
                InvokeMessage(string.Format("       | ...Test Patterns and Delays Determined in {0}.", _sw.Elapsed.ToLongFormattedString()));
                _sw.Reset();
            }
            catch
            {
                if (!_cancel && !_break)
                    throw;
            }
        }
        private void SetConnectionProxyPool(int connectionProxies)
        {
            InvokeMessage(string.Format("       |Setting Connections for {0} Concurrent Users...", _stresstest.ConcurrentUsers[_currentConcurrentUsersIndex]));
            _sw.Start();
            try
            {
                _connectionProxyPool.SetAndConnectConnectionProxies(connectionProxies, _parallelConnectionsModifier);
                InvokeMessage(string.Format("       | ...Connections Set in {0}.", _sw.Elapsed.ToLongFormattedString()));
            }
            catch
            {
                throw;
            }
            finally
            {
                _sw.Stop();
                _sw.Reset();
            }
        }
        private void SetThreadPool(int threads)
        {
            InvokeMessage(string.Format("Setting Thread Pool for {0} Concurrent Users...", _currentConcurrentUsers));
            _sw.Start();
            if (_threadPool == null)
            {
                _threadPool = new StresstestThreadPool(Work);
                _threadPool.ThreadWorkException += new EventHandler<MessageEventArgs>(_threadPool_ThreadWorkException);
            }
            _threadPool.SetThreads(threads);
            _sw.Stop();
            InvokeMessage(string.Format(" ...Thread Pool Set in {0}.", _sw.Elapsed.ToLongFormattedString()));
            _sw.Reset();
        }

        private void DisposeThreadPool()
        {
            if (_threadPool != null)
                try
                {
                    _threadPool.Dispose(100);
                }
                catch { }
            _threadPool = null;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                try
                {
                    _isDisposed = true;
                    Cancel();
                    _connectionProxyPool.Dispose();
                    _connectionProxyPool = null;

                    _sleepWaitHandle.Close();
                    _sleepWaitHandle.Dispose();
                    _sleepWaitHandle = null;

                    _runSynchronizationContinueWaitHandle.Close();
                    _runSynchronizationContinueWaitHandle.Dispose();
                    _runSynchronizationContinueWaitHandle = null;

                    _logEntries = null;
                    _testableLogEntries = null;
                    _testPatternsAndDelaysGenerator.Dispose();
                    _testPatternsAndDelaysGenerator = null;
                    _stresstestResults = null;
                    _sw = null;
                }
                catch { }
            }
        }

        #region Events
        public event EventHandler<StresstestStartedEventArgs> StresstestStarted;
        public event EventHandler<ConcurrentUsersStartedEventArgs> ConcurrentUsersStarted;
        public event EventHandler<PrecisionStartedEventArgs> PrecisionStarted;
        public event EventHandler<RunStartedEventArgs> RunStarted;
        public event EventHandler WorkDoneOnce;
        public event EventHandler RunStopped;
        public event EventHandler<MessageEventArgs> Message;

        private void SetStresstestStarted()
        {
            ulong totalLogEntries = 0;
            for (int c = 0; c != _stresstest.ConcurrentUsers.Length; c++)
                for (int concurrency = 0; concurrency != _stresstest.ConcurrentUsers[c]; concurrency++)
                    for (int p = 0; p != _stresstest.Precision; p++)
                        for (int r = 0; r != _runsConcurrentUsers[c]; r++)
                            totalLogEntries += (ulong)_testPatternsAndDelaysGenerator.PatternLength;

            _stresstestResults = new StresstestResults(Solution.ActiveSolution == null ? null : Solution.ActiveSolution.FileName, _stresstest, totalLogEntries, DateTime.Now);
            InvokeMessage(string.Format("Starting the Stresstest... ({0})", _stresstestResults.Metrics.StartMeasuringRuntime));

            if (!_cancel && StresstestStarted != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { StresstestStarted(this, new StresstestStartedEventArgs(_stresstestResults)); });
        }
        private void SetConcurrentUsersStarted()
        {
            ulong totalLogEntries = 0;
            for (int c = 0; c != _currentConcurrentUsers; c++)
                for (int p = 0; p != _stresstest.Precision; p++)
                    for (int r = 0; r != _runsConcurrentUsers[_currentConcurrentUsersIndex]; r++)
                        for (int t = 0; t != _testPatternsAndDelaysGenerator.PatternLength; t++)
                            ++totalLogEntries;

            _concurrentUsersResult = new ConcurrentUsersResult(_currentConcurrentUsers, totalLogEntries, DateTime.Now);
            _stresstestResults.ConcurrentUsersResults.Add(_concurrentUsersResult);
            InvokeMessage(string.Format("|-> {0} Concurrent Users... ({1})", _currentConcurrentUsers, _concurrentUsersResult.Metrics.StartMeasuringRuntime), Color.LightGreen);

            if (!_cancel && ConcurrentUsersStarted != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { ConcurrentUsersStarted(this, new ConcurrentUsersStartedEventArgs(_concurrentUsersResult)); });
        }
        private void SetPrecisionStarted(int precision)
        {
            ulong totalLogEntries = 0;
            for (int c = 0; c != _currentConcurrentUsers; c++)
                for (int r = 0; r != _runsConcurrentUsers[_currentConcurrentUsersIndex]; r++)
                    totalLogEntries += (ulong)_testPatternsAndDelaysGenerator.PatternLength;

            _precisionResult = new PrecisionResult(precision, totalLogEntries, DateTime.Now);
            _concurrentUsersResult.PrecisionResults.Add(_precisionResult);
            InvokeMessage(string.Format("|---> Precision {0}... ({1})", precision + 1, _precisionResult.Metrics.StartMeasuringRuntime), Color.LightGreen);

            if (!_cancel && PrecisionStarted != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { PrecisionStarted(this, new PrecisionStartedEventArgs(_precisionResult)); });
        }
        private void SetRunStarted(int run)
        {
            ulong totalLogEntries = 0;
            ulong singleUserLogEntryCount = 0;
            for (int c = 0; c != _currentConcurrentUsers; c++)
            {
                if (singleUserLogEntryCount == 0)
                    singleUserLogEntryCount = (ulong)_testPatternsAndDelaysGenerator.PatternLength;

                totalLogEntries += singleUserLogEntryCount;
            }

            _runResult = new RunResult(run, _currentConcurrentUsers, totalLogEntries, (int)singleUserLogEntryCount, DateTime.Now);
            _precisionResult.RunResults.Add(_runResult);
            InvokeMessage(string.Format("|----> |Run {0}... ({1})", run + 1, _runResult.Metrics.StartMeasuringRuntime), Color.LightGreen);

            if (!_cancel && RunStarted != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { RunStarted(this, new RunStartedEventArgs(_runResult)); });
        }
        private void SetWorkDoneOnce()
        {
            _workDoneOnce = true;
            if (!_cancel && WorkDoneOnce != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { WorkDoneOnce(this, null); });
        }
        private void SetRunStopped()
        {
            if (!_cancel && RunStopped != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { RunStopped(this, null); });
        }

        private void _threadPool_ThreadWorkException(object sender, MessageEventArgs e)
        {
            InvokeMessage(e.Message, e.Color, e.LogLevel);
        }
        private void InvokeMessage(string message, LogLevel logLevel = LogLevel.Info)
        {
            InvokeMessage(message, Color.Empty, logLevel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color">can be Color.Empty</param>
        /// <param name="logLevel"></param>
        private void InvokeMessage(string message, Color color, LogLevel logLevel = LogLevel.Info)
        {
            try
            {
                LogWrapper.LogByLevel(message, logLevel);
                if (Message != null)
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate { Message(this, new MessageEventArgs(message, color, logLevel)); });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed invoking message: " + message + " at log level: " + logLevel + ".\n" + ex.ToString());
            }
        }
        #endregion

        #endregion

        private struct TestableLogEntry
        {
            public int UserActionIndex;
            /// <summary>
            /// Should be log.IndexOf(LogEntry) or log.IndexOf(UserAction) + "." + UserAction.IndexOf(LogEntry), this must be unique
            /// </summary>
            public string LogEntryIndex, ParameterizedLogEntryString, UserAction;
            /// <summary>
            /// Execute parallel with immediate next or previous sibling who has this bool also set to true;
            /// </summary>
            public bool ExecuteParallel;
            public StringTree ParameterizedLogEntry;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="logEntryIndex">Should be log.IndexOf(LogEntry) or log.IndexOf(UserAction) + '.'. + UserAction.IndexOf(LogEntry), this must be unique</param>
            /// <param name="parameterizedLogEntry"></param>
            /// <param name="userAction">Can not be null, string.empty is allowed</param>
            /// <param name="userAction">Can not be null, string.empty is allowed</param>
            public TestableLogEntry(string logEntryIndex, StringTree parameterizedLogEntry, int userActionIndex, string userAction, bool executeParallel)
            {
                if (userAction == null)
                    throw new ArgumentNullException("userAction");

                LogEntryIndex = logEntryIndex;
                UserActionIndex = userActionIndex;

                ParameterizedLogEntry = parameterizedLogEntry;
                ParameterizedLogEntryString = ParameterizedLogEntry.CombineValues();
                UserAction = userAction;
                ExecuteParallel = executeParallel;
            }
        }
    }
}