/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vApus.Results;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    public class StresstestCore : IDisposable {

        #region Events
        public event EventHandler<StresstestResultEventArgs> StresstestStarted;
        public event EventHandler<ConcurrencyResultEventArgs> ConcurrencyStarted, ConcurrencyStopped;
        public event EventHandler<RunResultEventArgs> RunInitializedFirstTime, RunStarted, RunStopped;
        public event EventHandler RunDoneOnce, RerunDone;
        public event EventHandler<MessageEventArgs> Message;
        #endregion

        #region Fields
        /// <summary>
        ///     To be able to execute log entries parallel. This feature is not used at the time.
        /// </summary>
        [ThreadStatic]
        private static SyncAndAsyncWorkItem _syncAndAsyncWorkItem;

        private readonly Stresstest _stresstest;

        private Log _log;
        private LogEntry[] _logEntries;
        private TestableLogEntry[][] _testableLogEntries;

        private ResultsHelper _resultsHelper = new ResultsHelper();
        private StresstestResult _stresstestResult;
        /// <summary>
        /// The result of the current executing concurrency.
        /// </summary>
        private RunResult _runResult;
        /// <summary>
        /// The result of the current executing concurrency.
        /// </summary>
        private ConcurrencyResult _concurrencyResult;

        private TestPatternsAndDelaysGenerator _testPatternsAndDelaysGenerator;
        private int[][] _delays;
        private AutoResetEvent _sleepWaitHandle = new AutoResetEvent(false); //Better than Thread.Sleep to wait a delay after sending to / receiving from the server app.

        private StresstestThreadPool _threadPool;
        private ConnectionProxyPool _connectionProxyPool;

        /// <summary> Needed for break on last. </summary>
        private volatile bool _runDoneOnce;
        private volatile int _rerun = 0;
        /// <summary> Every time the execution is broken (Break) the continue counter is incremented by one. This to be sure that if a continue is send it is reacted on it the right way. </summary>
        private int _continueCounter;

        private AutoResetEvent _runSynchronizationContinueWaitHandle = new AutoResetEvent(false), _manyToOneWaitHandle = new AutoResetEvent(false);

        /// <summary>Measures intit actions and such to be able to output to the gui.</summary>
        private Stopwatch _sw = new Stopwatch();

        private volatile bool _break, _cancel, _completed, _isFailed, _isDisposed;

        //Parallel execution / communication to the server app. Not used feature atm.
        /// <summary> The number of all parallel connections, they will be disposed along the road. </summary>
        private int _parallelConnectionsModifier;
        #endregion

        #region Properties
        public ResultsHelper ResultsHelper {
            get { return _resultsHelper; }
            set { _resultsHelper = value; }
        }

        public StresstestResult StresstestResult { get { return _stresstestResult; } }

        public RunSynchronization RunSynchronization { get; set; }

        public int MaxRerunsBreakOnLast { get; set; }

        public int BusyThreadCount { get { return _threadPool == null ? 0 : _threadPool.BusyThreadCount; } }

        public bool IsDisposed { get { return _isDisposed; } }
        #endregion

        #region Con-/Destructor
        /// <summary>
        /// Don't forget to set the resultshelper if need be
        /// </summary>
        /// <param name="stresstest"></param>
        /// <param name="limitSimultaniousRunningToOne">Allow only one stresstest to run at a time.</param>
        public StresstestCore(Stresstest stresstest) {
            ObjectRegistrar.MaxRegistered = 1;
            ObjectRegistrar.Register(this);

            _stresstest = stresstest;
        }
        ~StresstestCore() {
            Dispose();
        }
        #endregion

        #region Functions

        #region Eventing
        private void SetStresstestStarted() {
            _stresstestResult = new StresstestResult();
            _resultsHelper.SetStresstestStarted(_stresstestResult);
            InvokeMessage("Starting the stresstest...");

            if (!_cancel && StresstestStarted != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(
                    delegate { StresstestStarted(this, new StresstestResultEventArgs(_stresstestResult)); }, null);
        }

        private void SetConcurrencyStarted(int concurrentUsersIndex) {
            int concurrentUsers = _stresstest.Concurrencies[concurrentUsersIndex];
            _concurrencyResult = new ConcurrencyResult(concurrentUsers, _stresstest.Runs);
            _stresstestResult.ConcurrencyResults.Add(_concurrencyResult);
            _resultsHelper.SetConcurrencyStarted(_concurrencyResult);
            InvokeMessage(
                string.Format("|-> {0} Concurrent Users... (Initializing the first run, be patient)", concurrentUsers),
                Color.MediumPurple);

            if (!_cancel && ConcurrencyStarted != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(
                    delegate { ConcurrencyStarted(this, new ConcurrencyResultEventArgs(_concurrencyResult)); }, null);
        }
        private void SetConcurrencyStopped() {
            _resultsHelper.SetConcurrencyStopped(_concurrencyResult);

            if (!_cancel && ConcurrencyStopped != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(
                    delegate { ConcurrencyStopped(this, new ConcurrencyResultEventArgs(_concurrencyResult)); }, null);
        }

        /// <summary>
        ///     For monitoring --> to know the time offset of the counters so a range can be linked to a run.
        ///     The current run result (_runResult) is also given with in the event's event args.
        /// </summary>
        private void SetRunStarted() {
            _resultsHelper.SetRunStarted(_runResult);
            if (_cancel && RunStarted != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(
                    delegate { RunStarted(this, new RunResultEventArgs(_runResult)); }, null);
        }

        /// <summary>
        ///     For monitoring --> to know the time offset of the counters so a range can be linked to a run.
        /// </summary>
        private void SetRunStopped() {
            StresstestMetrics metrics = StresstestMetricsHelper.GetMetrics(_runResult);
            InvokeMessage("|----> |Run Finished in " + metrics.MeasuredTime + "!", Color.MediumPurple);
            if (_resultsHelper.DatabaseName != null) InvokeMessage("|----> |Writing Results to Database...");
            _resultsHelper.SetRunStopped(_runResult);

            if (!_cancel && RunStopped != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(
                    delegate { RunStopped(this, new RunResultEventArgs(_runResult)); }, null);
        }

        /// <summary>
        ///     For run sync
        /// </summary>
        /// <param name="concurrentUsersIndex"></param>
        /// <param name="run"></param>
        private void SetRunInitializedFirstTime(int concurrentUsersIndex, int run) {
            InvokeMessage(string.Format("|----> |Run {0}...", run), Color.MediumPurple);

            int singleUserLogEntryCount = _testPatternsAndDelaysGenerator.PatternLength;
            int concurrentUsers = _stresstest.Concurrencies[concurrentUsersIndex];

            _runResult = new RunResult(run, concurrentUsers);
            for (int i = 0; i < concurrentUsers; i++) _runResult.VirtualUserResults[i] = new VirtualUserResult(singleUserLogEntryCount);

            _concurrencyResult.RunResults.Add(_runResult);

            if (!_cancel && RunInitializedFirstTime != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { RunInitializedFirstTime(this, new RunResultEventArgs(_runResult)); }, null);
        }

        /// <summary>
        ///     For run sync (break on first and last finished)
        ///     Returns true if run done once event invoked
        /// </summary>
        private bool SetRunDoneOnce() {
            if (!_runDoneOnce) {
                _runDoneOnce = true;
                if (!_cancel && RunDoneOnce != null)
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate { RunDoneOnce(this, null); }, null);
                return true;
            }
            return false;
        }
        /// <summary>
        ///     For run sync (break on last finished)
        ///     Do not use this in combination with run done once.
        /// </summary>
        private void SetRerunDone() {
            if (!_cancel && RerunDone != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { RerunDone(this, null); }, null);
        }

        private void _threadPool_ThreadWorkException(object sender, MessageEventArgs e) { InvokeMessage(e.Message, e.Color, e.LogLevel); }

        private void InvokeMessage(string message, LogLevel logLevel = LogLevel.Info) { InvokeMessage(message, Color.Empty, logLevel); }
        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color">can be Color.Empty</param>
        /// <param name="logLevel"></param>
        private void InvokeMessage(string message, Color color, LogLevel logLevel = LogLevel.Info) {
            try {
                LogWrapper.LogByLevel(message, logLevel);
                if (Message != null)
                    SynchronizationContextWrapper.SynchronizationContext.Send(
                        delegate { Message(this, new MessageEventArgs(message, color, logLevel)); }, null);
            } catch (Exception ex) {
                Debug.WriteLine("Failed invoking message: " + message + " at log level: " + logLevel + ".\n" + ex);
            }
        }
        #endregion

        #region Init
        /// <summary>
        ///     Do this before everything else.
        /// </summary>
        public void InitializeTest() {
            try {
                InvokeMessage("Initializing the Test.");
                InitializeLog();
                InitializeConnectionProxyPool();
            } catch {
                _isFailed = true;
                throw;
            }
        }

        /// <summary>
        ///     Will apply the log rule set.
        ///     Parallel connections will be determined here also.
        /// </summary>
        private void InitializeLog() {
            if (_cancel)
                return;

            InvokeMessage("Initializing Log...");
            _sw.Start();
            if (_stresstest.Log.LogRuleSet.IsEmpty) {
                var ex = new Exception("No rule set has been assigned to the selected log.");
                LogWrapper.LogByLevel(ex.ToString(), LogLevel.Error);
                throw ex;
            }
            if (_stresstest.Log.Count == 0) {
                var ex = new Exception("There are no entries in the selected log.");
                LogWrapper.LogByLevel(ex.ToString(), LogLevel.Error);
                throw ex;
            }

            _log = LogTimesOccurancies(_stresstest.Log, _stresstest.Distribute);

            //Parallel connections, check per user action
            _parallelConnectionsModifier = 0;
            var logEntries = new List<LogEntry>();
            foreach (BaseItem item in _log)
                if (item is UserAction)
                    foreach (LogEntry logEntry in item) {
                        logEntries.Add(logEntry);

                        if (logEntry.ExecuteInParallelWithPrevious)
                            ++_parallelConnectionsModifier;
                    } else if (item is LogEntry)
                    logEntries.Add(item as LogEntry);

            _logEntries = logEntries.ToArray();
            int actionCount = _stresstest.Distribute == UserActionDistribution.Fast ? _stresstest.Log.Count : _log.Count; //Needed for fast log entry distribution

            _testPatternsAndDelaysGenerator = new TestPatternsAndDelaysGenerator(_logEntries, actionCount, _stresstest.Shuffle, _stresstest.Distribute, _stresstest.MinimumDelay, _stresstest.MaximumDelay);
            _sw.Stop();
            InvokeMessage(string.Format(" ...Log Initialized in {0}.", _sw.Elapsed.ToLongFormattedString()));
            _sw.Reset();
        }
        /// <summary>
        ///     Expands the log (into a new one) times the occurance, this is only done if the Action Distribution is not equal to none.
        ///     Otherwise the original on is returned.
        ///     This also applies the log ruleset.
        /// </summary>
        /// <returns></returns>
        private Log LogTimesOccurancies(Log log, UserActionDistribution distribute) {
            if (distribute == UserActionDistribution.None) {
                log.ApplyLogRuleSet();
                return log;
            } else {
                Log newLog = log.Clone(false);
                var linkCloned = new Dictionary<UserAction, UserAction>(); //To add the right user actions to the link.
                foreach (UserAction action in log) {
                    var firstEntryClones = new List<LogEntry>(); //This is a complicated structure to be able to get averages when using distribute.
                    for (int i = 0; i != action.Occurance; i++) {
                        var actionClone = new UserAction(action.Label);
                        actionClone.Occurance = 1; //Must be one now, this value doesn't matter anymore.
                        actionClone.Pinned = action.Pinned;

                        bool canAddClones = firstEntryClones.Count == 0;
                        int logEntryIndex = 0;
                        foreach (LogEntry child in action) {
                            LogEntry childClone = child.Clone(log.LogRuleSet);

                            if (canAddClones)
                                firstEntryClones.Add(childClone);
                            else
                                childClone.SameAs = firstEntryClones[logEntryIndex];

                            actionClone.AddWithoutInvokingEvent(childClone, false);
                            ++logEntryIndex;
                        }

                        newLog.AddWithoutInvokingEvent(actionClone, false);

                        if (action.LinkedToUserActionIndices.Count != 0 && !linkCloned.ContainsKey(action)) {
                            linkCloned.Add(action, actionClone);
                        } else {
                            UserAction linkUserAction;
                            if (action.IsLinked(out linkUserAction) && linkCloned.ContainsKey(linkUserAction))
                                linkCloned[linkUserAction].LinkedToUserActionIndices.Add(newLog.IndexOf(actionClone) + 1);
                        }
                    }
                }
                return newLog;
            }
        }

        private void InitializeConnectionProxyPool() {
            if (_cancel)
                return;

            InvokeMessage("Initialize Connection Proxy Pool...");
            _sw.Start();
            _connectionProxyPool = new ConnectionProxyPool(_stresstest.Connection);
            CompilerResults compilerResults = _connectionProxyPool.CompileConnectionProxyClass(false);
            if (compilerResults.Errors.HasErrors) {
                var sb = new StringBuilder("Failed at compiling the connection proxy class:");
                sb.AppendLine();
                foreach (CompilerError compileError in compilerResults.Errors) {
                    sb.AppendFormat("Error number {0}, Line {1}, Column {2}: {3}", compileError.ErrorNumber, compileError.Line, compileError.Column, compileError.ErrorText);
                    sb.AppendLine();
                }
                _connectionProxyPool.Dispose();
                _connectionProxyPool = null;
                var ex = new Exception(sb.ToString());
                LogWrapper.LogByLevel(ex.ToString(), LogLevel.Error);
                // This handles notification to the user.
                throw ex;
            }
            string error;
            _connectionProxyPool.TestConnection(out error);
            if (error != null) {
                _connectionProxyPool.Dispose();
                _connectionProxyPool = null;
                var ex = new Exception(error);
                LogWrapper.LogByLevel(ex.ToString(), LogLevel.Error);
                throw ex;
            }
            _sw.Stop();
            InvokeMessage(string.Format(" ...Connection Proxy Pool Inititialized in {0}.", _sw.Elapsed.ToLongFormattedString()));
            _sw.Reset();
        }

        //Following init functions happen right before a run starts (ExecuteStresstest).
        private void DetermineTestableLogEntriessAndDelays(int concurrentUsers) {
            try {
                InvokeMessage(string.Format("       |Determining Test Patterns and Delays for {0} Concurrent Users...", concurrentUsers));
                _sw.Start();

                var testableLogEntries = new ConcurrentBag<TestableLogEntry[]>();
                var delays = new List<int[]>();

                //Get parameterized structures and patterns first sync first: no locking needed further on.
                var allParameterizedStructures = new ConcurrentBag<StringTree[]>();
                var allTestPatterns = new ConcurrentBag<int[]>();
                for (int t = 0; t != concurrentUsers; t++) {
                    allParameterizedStructures.Add(_log.GetParameterizedStructure());

                    int[] testPattern, delayPattern;
                    _testPatternsAndDelaysGenerator.GetPatterns(out testPattern, out delayPattern);
                    allTestPatterns.Add(testPattern);
                    delays.Add(delayPattern);

                    Thread.Sleep(1); //For the random in the pattern generator.
                }

                //Get all this stuff here, otherwise locking will slow down all following code.
                var logEntryIndices = new ConcurrentDictionary<LogEntry, string>();
                var logEntryParents = new ConcurrentDictionary<LogEntry, string>();
                string dot = ".", empty = " ", colon = ": ";

                Parallel.For(0, _log.Count, (userActionIndex) => {
                    var userAction = _log[userActionIndex] as UserAction;
                    if (!userAction.IsEmpty) {
                        string userActionString = string.Join(empty, userAction.Name, userActionIndex);
                        if (userAction.Label != string.Empty)
                            userActionString = string.Join(colon, userActionString, userAction.Label);

                        Parallel.For(0, userAction.Count, (logEntryIndex) => {
                            var logEntry = userAction[logEntryIndex] as LogEntry;

                            logEntryIndices.TryAdd(logEntry, string.Join(dot, userActionIndex, logEntryIndex));
                            logEntryParents.TryAdd(logEntry, userActionString);
                        });
                    }
                });

                //Get testable log entries  async: way faster.
                Parallel.For(0, concurrentUsers, (t, loopState) => {
                    try {
                        if (_cancel) loopState.Break();

                        StringTree[] parameterizedStructure;
                        allParameterizedStructures.TryTake(out parameterizedStructure);

                        int[] testPattern;
                        allTestPatterns.TryTake(out testPattern);

                        var tle = new TestableLogEntry[testPattern.Length];

                        Parallel.For(0, testPattern.Length, (i, loopState2) => {
                            try {
                                if (_cancel) loopState2.Break();

                                int testPatternIndex = testPattern[i];
                                var logEntry = _logEntries[testPatternIndex];

                                string sameAsLogEntryIndex = string.Empty;
                                if (logEntry.SameAs != null)
                                    sameAsLogEntryIndex = logEntryIndices[logEntry.SameAs];

                                tle[i] = new TestableLogEntry(logEntryIndices[logEntry], sameAsLogEntryIndex,
                                    parameterizedStructure[testPatternIndex], logEntryParents[logEntry], logEntry.ExecuteInParallelWithPrevious,
                                    logEntry.ParallelOffsetInMs, _rerun);
                            } catch (Exception ex2) {
                                LogWrapper.LogByLevel("Failed at determining test patterns>\n" + ex2, LogLevel.Error);
                                loopState.Break();
                            }
                        });

                        testableLogEntries.Add(tle);
                    } catch (Exception ex) {
                        LogWrapper.LogByLevel("Failed at determining test patterns>\n" + ex, LogLevel.Error);
                        loopState.Break();
                    }
                });

                _testableLogEntries = testableLogEntries.ToArray();
                _delays = delays.ToArray();

                testableLogEntries = null;
                delays = null;

                allParameterizedStructures = null;
                allTestPatterns = null;

                logEntryIndices = null;
                logEntryParents = null;

                GC.Collect();

                _sw.Stop();
                InvokeMessage(string.Format("       | ...Test Patterns and Delays Determined in {0}.", _sw.Elapsed.ToLongFormattedString()));
                _sw.Reset();
            } catch {
                if (!_cancel)
                    throw;
            }
        }
        private void SetThreadPool(int concurrentUsers) {
            if (_cancel) return;

            InvokeMessage(string.Format("       |Setting Thread Pool for {0} Concurrent Users...", concurrentUsers));
            _sw.Start();
            if (_threadPool == null) {
                _threadPool = new StresstestThreadPool(Work);
                _threadPool.ThreadWorkException += _threadPool_ThreadWorkException;
            }
            _threadPool.SetThreads(concurrentUsers);
            _sw.Stop();
            InvokeMessage(string.Format("       | ...Thread Pool Set in {0}.", _sw.Elapsed.ToLongFormattedString()));
            _sw.Reset();
        }
        private void SetConnectionProxyPool(int concurrentUsers) {
            if (_cancel) return;

            InvokeMessage(string.Format("       |Setting Connections for {0} Concurrent Users...", concurrentUsers));
            _sw.Start();
            try {
                _connectionProxyPool.SetAndConnectConnectionProxies(concurrentUsers, _parallelConnectionsModifier);
                InvokeMessage(string.Format("       | ...Connections Set in {0}.", _sw.Elapsed.ToLongFormattedString()));
            } catch {
                throw;
            } finally {
                _sw.Stop();
                _sw.Reset();
            }
        }
        #endregion

        #region Work
        public StresstestStatus ExecuteStresstest() {
            //No run started yet.
            _continueCounter = -1;
            SetStresstestStarted();
            //Loop all concurrent users
            for (int concurrentUsersIndex = 0;
                 concurrentUsersIndex != _stresstest.Concurrencies.Length;
                 concurrentUsersIndex++) {
                int concurrentUsers = _stresstest.Concurrencies[concurrentUsersIndex];
                if (_cancel) break;

                SetConcurrencyStarted(concurrentUsersIndex);
                //Loop 'run' times for the concurrent users.
                for (int run = 0; run != _stresstest.Runs; run++) {
                    if (_cancel) break;
                    _runDoneOnce = false;
                    _rerun = 0;

                Rerun:
                    //Initialize all for a new test.
                    if (_cancel) break;
                    //Initialize the log entries and delays (every time so the tests for different runs are not the same)
                    DetermineTestableLogEntriessAndDelays(concurrentUsers);
                    if (_cancel) break;
                    SetThreadPool(concurrentUsers);
                    if (_cancel) break;
                    //Initialize the connection proxy pool (every time, so dead or locked connections can't be used anymore)
                    SetConnectionProxyPool(concurrentUsers);
                    if (_cancel) break;

                    if (_runDoneOnce) {
                        InvokeMessage(string.Format("|----> | Rerunning Run {0}...", run + 1), Color.White);
                    } else {
                        ++_continueCounter;

                        //For many-to-one testing, keeping the run shared by divided tile stresstest in sync.
                        SetRunInitializedFirstTime(concurrentUsersIndex, run + 1);
                        if (_stresstest.IsDividedStresstest && !_cancel) {
                            InvokeMessage("Waiting for Continue Message from Master...");
                            _manyToOneWaitHandle.WaitOne();
                            InvokeMessage("Continuing...");
                        }
                        //Wait here untill the master sends continue when using run sync.
                        if (RunSynchronization != RunSynchronization.None && !_cancel) {
                            InvokeMessage("Waiting for Continue Message from Master...");
                            _runSynchronizationContinueWaitHandle.WaitOne();
                            InvokeMessage("Continuing...");
                        }

                        //Set this run started, used for for instance monitoring --> the range for this run can be determined this way.
                        SetRunStarted();
                    }

                    if (_cancel) break;

                    //Do the actual work and invoke when the run is finished.
                    try {
                        _threadPool.DoWorkAndWaitForIdle();
                    } catch (Exception ex) {
                        if (!_isDisposed)
                            InvokeMessage("|----> |Run Not Finished Succesfully!\n|Thread Pool Exception:\n" + ex, Color.Red, LogLevel.Error);
                    }

                    //For many-to-one testing, keeping the run shared by divided tile stresstest in sync.
                    if (_stresstest.IsDividedStresstest) {
                        SetRunDoneOnce();
                        SetRunStopped();

                        InvokeMessage("Waiting for Continue Message from Master...");
                        _manyToOneWaitHandle.WaitOne();
                        InvokeMessage("Continuing...");
                    }

                    //Wait here when the run is broken untill the master sends continue when using run sync.
                    if (RunSynchronization == RunSynchronization.BreakOnFirstFinished) {
                        ++_continueCounter;
                        SetRunDoneOnce();
                        SetRunStopped();

                        InvokeMessage("Waiting for Continue Message from Master...");
                        _runSynchronizationContinueWaitHandle.WaitOne();
                        InvokeMessage("Continuing...");
                    }
                        //Rerun untill the master sends a break. This is better than recurions --> no stack overflows.
                        //Also breaks after the rerun count == _maxRerunsBreakOnLast.
                    else if (RunSynchronization == RunSynchronization.BreakOnLastFinished && !_break) {
                        ++_rerun;
                        if (!SetRunDoneOnce())
                            SetRerunDone();

                        //Allow one last rerun, then wait for the master for a continue, or rerun nfinite.
                        if (MaxRerunsBreakOnLast == 0 || _rerun <= MaxRerunsBreakOnLast) {
                            InvokeMessage("Initializing Rerun...");
                            //Increase resultset
                            _runResult.PrepareForRerun();
                            _resultsHelper.SetRerun(_runResult);
                            goto Rerun;
                        } else {
                            SetRunStopped();
                        }
                    } else {
                        SetRunStopped();
                    }
                }
                SetConcurrencyStopped();
            }

            return Completed();
        }

        /// <summary>
        /// Break the current executing run. (Distributed testing)
        /// </summary>
        public void Break() {
            if (!_break && !(_completed | _cancel | _isFailed)) {
                _break = true;
                InvokeMessage("Breaking the execution for the current run...");

                _sleepWaitHandle.Set();
            }
        }
        /// <summary>
        /// Continue to newly inited or the next run. (Distributed Testing)
        /// </summary>
        /// <param name="continueCounter">Every time the execution is paused the continue counter is incremented by one.</param>
        public void Continue(int continueCounter) {
            if (_continueCounter == continueCounter && !(_completed | _cancel | _isFailed)) {
                _break = false;
                _runSynchronizationContinueWaitHandle.Set();
            }
        }

        /// <summary>
        /// Keeping the shared run for a divided tile stresstest in sync.
        /// </summary>
        public void ContinueDivided() {
            if (!(_completed | _cancel | _isFailed))
                _manyToOneWaitHandle.Set();
        }

        /// <summary>
        /// WorkItemCallback in StresstestCore, executes all log entries for the current running thread. Has some exception handling.
        /// </summary>
        /// <param name="threadIndex"></param>
        public void Work(int threadIndex) {
            try {
                IConnectionProxy connectionProxy = _connectionProxyPool[threadIndex];
                if (!connectionProxy.IsConnectionOpen) connectionProxy.OpenConnection();

                TestableLogEntry[] testableLogEntries = _testableLogEntries[threadIndex];
                int[] delays = _delays[threadIndex];

                int testableLogEntryIndex = 0;
                while (testableLogEntryIndex != testableLogEntries.Length) {
                    if (_cancel || _break) {
                        _sleepWaitHandle.Set();
                        return;
                    }

                    int incrementIndex;
                    ExecuteLogEntry(threadIndex, testableLogEntryIndex, connectionProxy, testableLogEntries, delays, out incrementIndex);
                    testableLogEntryIndex += incrementIndex;
                }
            } catch (Exception e) {
                if (!_cancel && !_break) InvokeMessage("Work failed for " + Thread.CurrentThread.Name + ".\n" + e, Color.Red, LogLevel.Error);
            }
        }

        /// <summary>
        ///     Decides if the log entry must be executed in parallel with others or not.
        /// </summary>
        /// <param name="threadIndex"></param>
        /// <param name="testableLogEntryIndex"></param>
        /// <param name="incrementIndex">Can be greater than 1 if some are parallel executed.</param>
        private void ExecuteLogEntry(int threadIndex, int testableLogEntryIndex, IConnectionProxy connectionProxy, TestableLogEntry[] testableLogEntries, int[] delays, out int incrementIndex) {
            incrementIndex = 1;

            if (_cancel || _break) {
                _sleepWaitHandle.Set();
                return;
            }

            TestableLogEntry testableLogEntry = testableLogEntries[testableLogEntryIndex];

            //Check if this log entry could be the first of a parallel executed range of entries
            //If so the range will be executed multi threaded
            if (!testableLogEntry.ExecuteInParallelWithPrevious || !_stresstest.UseParallelExecutionOfLogEntries) {
                int testableLogEntriesLength = 0, exclusiveEnd = 0;
                List<TestableLogEntry> parallelTestableLogEntries = null;

                // Make the range if following is true.
                if (_stresstest.UseParallelExecutionOfLogEntries) {
                    testableLogEntriesLength = testableLogEntries.Length;
                    exclusiveEnd = testableLogEntryIndex + 1;
                    parallelTestableLogEntries = new List<TestableLogEntry>();
                    //Do not go out of bounds
                    if (exclusiveEnd != testableLogEntriesLength)
                        while (testableLogEntries[exclusiveEnd].ExecuteInParallelWithPrevious) {
                            parallelTestableLogEntries.Add(testableLogEntries[exclusiveEnd]);
                            ++exclusiveEnd;
                            ++incrementIndex;
                            if (exclusiveEnd == testableLogEntriesLength)
                                break;
                        }
                }

                //Do work parallel if needed.
                if (incrementIndex == 1) {
                    if (_syncAndAsyncWorkItem == null)
                        _syncAndAsyncWorkItem = new SyncAndAsyncWorkItem();

                    //_sleepWaitHandle can be given here without a problem, the Set and Wait functions are thread specific. 
                    _syncAndAsyncWorkItem.ExecuteLogEntry(this, _sleepWaitHandle, _runResult, threadIndex, testableLogEntryIndex, testableLogEntry, connectionProxy, delays[testableLogEntryIndex]);
                } else {
                    //Get the connection proxies, this first one is not a parallel one but the connection proxy for the specific user, this way data (cookies for example) kan be saved for other log entries.
                    ParallelConnectionProxy[] pcps = _connectionProxyPool.ParallelConnectionProxies[threadIndex];
                    //Only use the ones that are needed, the first one is actually not parallel.
                    var parallelConnectionProxies = new ParallelConnectionProxy[exclusiveEnd - testableLogEntryIndex];
                    parallelConnectionProxies[0] = new ParallelConnectionProxy(connectionProxy);

                    //Check if already used, if not then choose and flag it "used"
                    int setAt = 1;
                    for (int pi = 0; pi != pcps.Length; pi++) {
                        ParallelConnectionProxy pcp = pcps[pi];
                        if (!pcp.Used) {
                            pcp.Used = true;
                            parallelConnectionProxies[setAt] = pcp;
                            if (++setAt == parallelConnectionProxies.Length)
                                break;
                        }
                    }

#warning Making threads on the fly is maybe not a very good idea, maybe this must reside in the thread pool (like parallel connection proxies are in the connection proxy pool)

                    //Make a mini thread pool (Thread pools in thread pools, what it this madness?! :p)
                    var pThreads = new Thread[parallelConnectionProxies.Length];
                    //if(pcpIndex == parallelConnectionProxies.length) it is finished.
                    var pThreadsSignalStart = new ManualResetEvent(false);
                    var pThreadsSignalFinished = new AutoResetEvent(false);

                    int pcpIndex = -1, pThreadIndex = 0;
                    int finished = 0;
                    string threadName = Thread.CurrentThread.Name + " [Parallel #";
                    for (int pleIndex = testableLogEntryIndex; pleIndex != exclusiveEnd; pleIndex++) {
                        //Anonymous delegate for the sake of simplicity, pleIndex in the state --> otherwise the wrong value can and will be picked
                        var pThread = new Thread(delegate(object state) {
                            var index = (int)state;
                            if (_syncAndAsyncWorkItem == null)
                                _syncAndAsyncWorkItem = new SyncAndAsyncWorkItem();

                            pThreadsSignalStart.WaitOne();

                            try {
                                //_sleepWaitHandle can be given here without a problem, the Set and Wait functions are thread specific. 
                                _syncAndAsyncWorkItem.ExecuteLogEntry(this, _sleepWaitHandle, _runResult, threadIndex, index, testableLogEntries[index], parallelConnectionProxies[Interlocked.Increment(ref pcpIndex)].ConnectionProxy, delays[index]);
                            } catch {
                                //when stopping a test...
                            }

                            if (Interlocked.Increment(ref finished) == parallelConnectionProxies.Length) pThreadsSignalFinished.Set();
                        });

                        //Add it to the pool, just for making sure they are kept in memory 
                        pThreads[pThreadIndex] = pThread;

                        pThread.Name = threadName + (++pThreadIndex) + "]";
                        pThread.IsBackground = true;
                        pThread.Start(pleIndex);
                    }

                    //Start them all at the same time
                    pThreadsSignalStart.Set();
                    pThreadsSignalFinished.WaitOne();

                    pThreadsSignalFinished.Dispose();
                    pThreadsSignalStart.Dispose();
                    pThreads = null;
                }
            }
        }

        public void Cancel() {
            if (!_cancel) {
                _cancel = true;
                _sleepWaitHandle.Set();
                _runSynchronizationContinueWaitHandle.Set();
                _manyToOneWaitHandle.Set();
                DisposeThreadPool();
                if (_connectionProxyPool != null) _connectionProxyPool.ShutDown();
            }
        }
        private StresstestStatus Completed() {
            _completed = true;
            _connectionProxyPool.ShutDown();
            DisposeThreadPool();
            _connectionProxyPool.Dispose();

            if (_cancel) {
                _resultsHelper.SetStresstestStopped(_stresstestResult, "Cancelled");
                return StresstestStatus.Cancelled;
            }
            if (_isFailed) {
                _resultsHelper.SetStresstestStopped(_stresstestResult, "Failed");
                return StresstestStatus.Error;
            }
            _resultsHelper.SetStresstestStopped(_stresstestResult);
            return StresstestStatus.Ok;
        }
        private void DisposeThreadPool() {
            if (_threadPool != null) try { _threadPool.Dispose(100); } catch { }
            _threadPool = null;
        }
        #endregion

        public void Dispose() {
            if (!_isDisposed) {
                try {
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

                    _manyToOneWaitHandle.Close();
                    _manyToOneWaitHandle.Dispose();
                    _manyToOneWaitHandle = null;

                    _logEntries = null;
                    _testableLogEntries = null;
                    _testPatternsAndDelaysGenerator.Dispose();
                    _testPatternsAndDelaysGenerator = null;
                    _stresstestResult = null;
                    _sw = null;
                } catch {
                }
            }
            ObjectRegistrar.Unregister(this);
        }
        #endregion

        /// <summary>
        ///     Log entry with metadata.
        /// </summary>
        private struct TestableLogEntry {
            #region Fields
            /// <summary>
            ///     Should be log.IndexOf(UserAction) + "." + UserAction.IndexOf(LogEntry); this must be unique.
            /// </summary>
            public readonly string LogEntryIndex;

            /// <summary>
            /// To be able to calcullate averages when using distribute. Cannot be null, string.empty is allowed.
            /// </summary>
            public readonly string SameAsLogEntryIndex;

            /// <summary>
            /// Used in IConnectionProxy.SendAndReceive.
            /// </summary>
            public readonly StringTree ParameterizedLogEntry;

            /// <summary>
            ///     Should be log.IndexOf(LogEntry) or log.IndexOf(UserAction) + "." + UserAction.IndexOf(LogEntry), this must be unique.
            /// </summary>
            public readonly string ParameterizedLogEntryString;

            /// <summary>
            ///     Cannot be null, string.empty is not allowed.
            /// </summary>
            public readonly string UserAction;

            /// <summary>
            ///     Execute parallel with immediate previous sibling. Not used feature atm.
            /// </summary>
            public readonly bool ExecuteInParallelWithPrevious;

            /// <summary>
            ///     The offset in ms before this 'parallel log entry' is executed (this simulates what browsers do).
            /// </summary>
            public readonly int ParallelOffsetInMs;

            /// <summary>
            /// 0 for all but Break on last runs (Distributed Testing).
            /// </summary>
            public readonly int Rerun;
            #endregion

            #region Constructor
            /// <summary>
            ///     Log entry with metadata.
            /// </summary>
            /// <param name="logEntryIndex"> Should be log.IndexOf(UserAction) + "." + UserAction.IndexOf(LogEntry); this must be unique.</param>
            /// <param name="sameAsLogEntryString">To be able to calcullate averages when using distribute. Cannot be null, string.empty is allowed.</param>
            /// <param name="parameterizedLogEntry">Used in IConnectionProxy.SendAndReceive.</param>
            /// <param name="userAction">Cannot be null, string.empty is not allowed.</param>
            /// <param name="executeInParallelWithPrevious">Execute parallel with immediate previous sibling. Not used feature atm.</param>
            /// <param name="parallelOffsetInMs">The offset in ms before this 'parallel log entry' is executed (this simulates what browsers do).</param>
            /// <param name="rerun">0 for all but Break on last runs (Distributed Testing).</param>
            public TestableLogEntry(string logEntryIndex, string sameAsLogEntryString, StringTree parameterizedLogEntry, string userAction, bool executeInParallelWithPrevious, int parallelOffsetInMs, int rerun) {
                if (userAction == null)
                    throw new ArgumentNullException("userAction");

                LogEntryIndex = logEntryIndex;
                SameAsLogEntryIndex = sameAsLogEntryString;

                ParameterizedLogEntry = parameterizedLogEntry;
                ParameterizedLogEntryString = ParameterizedLogEntry.CombineValues();
                UserAction = userAction;
                ExecuteInParallelWithPrevious = executeInParallelWithPrevious;
                ParallelOffsetInMs = parallelOffsetInMs;

                Rerun = rerun;
            }
            #endregion
        }

        /// <summary>
        /// An extra work item class (ThreadStatic) for executing log entries in parallel. (Not used feature atm)
        /// </summary>
        private class SyncAndAsyncWorkItem {
            public void ExecuteLogEntry(StresstestCore stresstestCore, AutoResetEvent sleepWaitHandle, RunResult runResult, int threadIndex, int testableLogEntryIndex, TestableLogEntry testableLogEntry, IConnectionProxy connectionProxy, int delayInMilliseconds) {
                DateTime sentAt = DateTime.Now;
                var timeToLastByte = new TimeSpan();
                Exception exception = null;

                if (stresstestCore._cancel || stresstestCore._break) {
                    sleepWaitHandle.Set();
                    return;
                }

                //Sleep the offset of parallel log entries.
                if (testableLogEntry.ExecuteInParallelWithPrevious) Thread.Sleep(testableLogEntry.ParallelOffsetInMs);

                bool retried = false;
            RetryOnce:
                try {
                    if (connectionProxy == null || connectionProxy.IsDisposed) {
                        exception = new Exception("Connectionproxy is disposed. Metrics for this log entry (" + testableLogEntry.ParameterizedLogEntryString + ") are not correct.");
                    } else {
                        StringTree parameterizedLogEntry = testableLogEntry.ParameterizedLogEntry;
                        connectionProxy.SendAndReceive(parameterizedLogEntry, out sentAt, out timeToLastByte, out exception);
                    }
                } catch (Exception ex) {
                    if (!retried && connectionProxy != null && !connectionProxy.IsDisposed) {
                        try {
                            retried = true;
                            connectionProxy.OpenConnection();
                            goto RetryOnce;
                        } catch (Exception e) {
                            exception = new Exception("An error in the connection proxy has occured and is now disposed due to for instance a time out or a bug in the connection proxy code; I tried to reopen the connection so testing could continue for this simulated user including a retry (once) for the current log entry, but it failed; Metrics for this log entry (" +
                                    testableLogEntry.ParameterizedLogEntryString + ") are not correct:\n" + ex + "\n\nReconnect failure:\n" + e);
                        }
                    } else {
                        try { if (connectionProxy != null)   connectionProxy.Dispose(); } catch { }
                        connectionProxy = null;
                        exception = new Exception("An error in the connection proxy has occured, after the second try, (and is now disposed) due to for instance a time out or a bug in the connection proxy code; Metrics for this log entry (" +
                                testableLogEntry.ParameterizedLogEntryString + ") are not correct:\n" + ex);
                    }
                    throw (exception);
                } finally {
                    VirtualUserResult result = runResult.VirtualUserResults[threadIndex];
                    result.VirtualUser = Thread.CurrentThread.Name;

                    var logEntryResult = new LogEntryResult {
                        LogEntryIndex = testableLogEntry.LogEntryIndex,
                        SameAsLogEntryIndex = testableLogEntry.SameAsLogEntryIndex,
                        LogEntry = testableLogEntry.ParameterizedLogEntryString,
                        UserAction = testableLogEntry.UserAction,
                        SentAt = sentAt,
                        TimeToLastByteInTicks = timeToLastByte.Ticks,
                        DelayInMilliseconds = delayInMilliseconds,
                        Error = (exception == null) ? string.Empty : exception.ToString(),
                        Rerun = testableLogEntry.Rerun
                    };
                    result.SetLogEntryResultAt(testableLogEntryIndex, logEntryResult);


                    if (delayInMilliseconds != 0 && !(stresstestCore._cancel || stresstestCore._break)) sleepWaitHandle.WaitOne(delayInMilliseconds);
                }
            }
        }
    }
}