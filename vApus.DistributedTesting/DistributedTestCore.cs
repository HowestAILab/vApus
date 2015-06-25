/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using RandomUtils.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Threading;
using vApus.Results;
using vApus.Communication.Shared;
using vApus.StressTest;
using vApus.Util;

namespace vApus.DistributedTest {
    /// <summary>
    /// Handles all messages coming from MasterSideCommunicationHandler and does run sync magic.
    /// Only one test can run at the same time, if this is called and another test (stress test core or distributed test core) exists (not disposed) an argument out of range exception will be thrown.
    /// </summary>
    public class DistributedTestCore {
        /// <summary>
        ///     Messages to the user on the message textbox
        /// </summary>
        public event EventHandler<MessageEventArgs> Message;
        /// <summary>
        /// Get the TileStressTestMessage from eg var tpms = _distributedTestCore.TileStressTestMessages; var tpm = tpms[e.TileStressTest].
        /// There is a delay of 500 ms.
        /// </summary>
        public event EventHandler OnTestProgressMessageReceivedDelayed;
        public event EventHandler<ListeningErrorEventArgs> OnListeningError;
        public event EventHandler<TestFinishedEventArgs> OnFinished;

        #region Fields
        private readonly DistributedTest _distributedTest;
        // For adding and getting results.
        private Dictionary<TileStressTest, int> _tileStressTestsWithDbIds;

        private volatile string[] _cancelled = new string[] { };
        private readonly object _communicationLock = new object();
        private volatile string[] _failed = new string[] { };

        //Invoke only once
        private bool _finishedHandled;
        private bool _hasResults;
        private bool _isDisposed;

        private volatile string[] _ok = new string[] { };

        private volatile string[] _runDoneOnce = new string[] { };
        private volatile string[] _runInitialized = new string[] { };
        private volatile ConcurrentDictionary<string, RerunCounter> _breakOnLastReruns = new ConcurrentDictionary<string, RerunCounter>();

        private Stopwatch _sw = new Stopwatch();

        /// <summary>
        ///     The messages pushed from the slaves.
        /// </summary>
        private Dictionary<TileStressTest, Dictionary<string, TestProgressMessage>> _testProgressMessages = new Dictionary<TileStressTest, Dictionary<string, TestProgressMessage>>();
        /// <summary>
        /// To know when a combined run is finished.
        /// </summary>
        private Dictionary<TileStressTest, List<string>> _dividedRunInitializedOrDoneOnce = new Dictionary<TileStressTest, List<string>>();

        private readonly object _testProgressMessagesLock = new object();
        private Dictionary<TileStressTest, TileStressTest> _usedTileStressTests = new Dictionary<TileStressTest, TileStressTest>(); //the divided stress tests and the originals
        private int _totalTestCount = 0; //This way we do not need a lock to get the count.

        private ResultsHelper _resultsHelper;

        private System.Timers.Timer _tmrOnInvokeTestProgressMessageReceivedDelayed = new System.Timers.Timer(500);
        #endregion

        #region Properties
        /// <summary>
        /// Key = divided stress test, Value = original
        /// </summary>
        public Dictionary<TileStressTest, TileStressTest> UsedTileStressTests { get { return _usedTileStressTests; } }

        /// <returns>-1 if not found or the first Id if the given is a divided tile stress test.</returns>
        public int GetDbId(TileStressTest tileStressTest) {
            string tileStressTestToString = ReaderAndCombiner.GetCombinedStressTestToString(tileStressTest.ToString());
            foreach (TileStressTest ts in _tileStressTestsWithDbIds.Keys)
                if (tileStressTestToString == ReaderAndCombiner.GetCombinedStressTestToString(ts.ToString()))
                    return _tileStressTestsWithDbIds[ts];
            return -1;
        }

        public bool IsDisposed { get { return _isDisposed; } }

        public int OK { get { return _ok.Length; } }

        public int Cancelled { get { return _cancelled.Length; } }

        public int Failed { get { return _failed.Length; } }

        /// <summary>
        ///     The stress tests that are not finished.
        /// </summary>
        public int Running { get { return _totalTestCount - Finished; } }

        public int Finished { get { return OK + Cancelled + Failed; } }

        /// <summary>
        ///     To check wheter you can close the distributed test view without a warning or not.
        /// </summary>
        public bool HasResults { get { return _hasResults; } }
        #endregion

        #region Con-/Destructor
        /// <summary>
        /// Handles all messages coming from MasterSideCommunicationHandler and does run sync magic.
        /// Only one test can run at the same time, if this is called and another test (stress test core or distributed test core) exists (not disposed) an argument out of range exception will be thrown.
        /// </summary>
        /// <param name="distributedTest"></param>
        /// <param name="resultsHelper"></param>
        public DistributedTestCore(DistributedTest distributedTest, ResultsHelper resultsHelper) {
            ObjectRegistrar.MaxRegistered = 1;
            ObjectRegistrar.Register(this);

            _distributedTest = distributedTest;
            _resultsHelper = resultsHelper;
            MasterSideCommunicationHandler.ListeningError += _masterCommunication_ListeningError;
            MasterSideCommunicationHandler.OnTestProgressMessageReceived += _masterCommunication_OnTestProgressMessageReceived;

            _tmrOnInvokeTestProgressMessageReceivedDelayed.Elapsed += _tmrOnInvokeTestProgressMessageReceivedDelayed_Elapsed;

            //#warning Enable REST
            // WriteRestConfig();
        }
        ~DistributedTestCore() {
            Dispose();
        }
        #endregion

        #region Functions

        #region Event Handling
        private void InvokeMessage(string message, Level logLevel = Level.Info) { InvokeMessage(message, Color.Empty, logLevel); }
        private void InvokeMessage(string message, Color color, Level logLevel = Level.Info) {
            try {
                if (logLevel == Level.Error) {
                    string[] split = message.Split(new[] { '\n', '\r' }, StringSplitOptions.None);
                    message = split[0] + "\n\nSee " + Loggers.GetLogger<FileLogger>().CurrentLogFile;
                }

                if (Message != null)
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                        try {
                            Message(this, new MessageEventArgs(message, color, logLevel));
                        } catch (Exception ex) {
                            Debug.WriteLine("Failed invoking message: " + message + " at log level: " + logLevel + ".\n" + ex);
                        }
                    }, null);

                Loggers.Log(logLevel, message);
                _resultsHelper.AddLogEntryInMemory((int)logLevel, message);
            } catch (Exception ex) {
                Debug.WriteLine("Failed invoking message: " + message + " at log level: " + logLevel + ".\n" + ex);
            }
        }

        private void InvokeOnTestProgressMessageReceivedDelayed() {
            if (OnTestProgressMessageReceivedDelayed != null && _tmrOnInvokeTestProgressMessageReceivedDelayed != null)
                try {
                    _tmrOnInvokeTestProgressMessageReceivedDelayed.Stop();
                    _tmrOnInvokeTestProgressMessageReceivedDelayed.Start();
                } catch {
                    //Only on gui closed.
                }
        }
        private void _tmrOnInvokeTestProgressMessageReceivedDelayed_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if (!_isDisposed && _tmrOnInvokeTestProgressMessageReceivedDelayed != null)
                try {
                    _tmrOnInvokeTestProgressMessageReceivedDelayed.Stop();
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate { OnTestProgressMessageReceivedDelayed(this, null); }, null);
                } catch {
                    //Only on gui closed.
                }
        }

        private void InvokeOnListeningError(ListeningErrorEventArgs listeningErrorEventArgs) {
            Loggers.Log(Level.Error, "A listening error occured", listeningErrorEventArgs.Exception);
            if (OnListeningError != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { OnListeningError(this, listeningErrorEventArgs); }, null);
        }

        private void InvokeOnFinished() {
            if (OnFinished != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(
                    delegate { OnFinished(this, new TestFinishedEventArgs(OK, Cancelled, Failed)); }, null);
        }
        #endregion

        #region Init
        /// <summary>
        ///     Connects + sends tests
        /// </summary>
        /// <returns>Stress tests and the IDs in the db</returns>
        public void Initialize(out bool notACleanDivision) {
            InvokeMessage("Initializing the test.");

            Connect(out notACleanDivision);
            SetvApusInstancesAndStressTestsInDb();
            SendAndReceiveInitializeTest();
        }
        private void Connect(out bool notACleanDivision) {
            InvokeMessage("Connecting slaves...");
            _sw.Start();

            MasterSideCommunicationHandler.Init();

            _usedTileStressTests = DivideEtImpera.DivideTileStressTestsOverSlaves(_distributedTest, out notACleanDivision);
            _totalTestCount = _usedTileStressTests.Count;

            foreach (var dividedTileStressTest in _usedTileStressTests.Keys) {
                Exception exception = null;
                int slaveCount = dividedTileStressTest.BasicTileStressTest.SlaveIndices.Length;

                string tileStressTestIndex = dividedTileStressTest.TileStressTestIndex;

                var slave = dividedTileStressTest.BasicTileStressTest.Slaves[0];
                int processID;
                MasterSideCommunicationHandler.ConnectSlave(slave.IP, slave.Port, out processID, out exception);

                if (exception == null) {
                    lock (_testProgressMessagesLock) {
                        var original = _usedTileStressTests[dividedTileStressTest];
                        if (!_testProgressMessages.ContainsKey(original))
                            _testProgressMessages.Add(original, new Dictionary<string, TestProgressMessage>());

                        _testProgressMessages[original].Add(tileStressTestIndex, new TestProgressMessage());
                    }
                    InvokeMessage(string.Format("|->Connected {0} - {1}", dividedTileStressTest.Parent, dividedTileStressTest));
                } else {
                    Dispose();
                    var ex = new Exception(string.Format("Could not connect to one of the slaves ({0} - {1})!{2}{3}", dividedTileStressTest.Parent, dividedTileStressTest, Environment.NewLine, exception));
                    string message = ex.Message + "\n" + ex.StackTrace + "\n\nSee " + Loggers.GetLogger<FileLogger>().CurrentLogFile;
                    InvokeMessage(message, Level.Error);
                    throw ex;
                }
            }
            if (_totalTestCount == 0) {
                var ex = new Exception("Please use at least one test!");
                string message = ex.Message + "\n" + ex.StackTrace + "\n\nSee " + Loggers.GetLogger<FileLogger>().CurrentLogFile;
                InvokeMessage(message, Level.Warning);
                throw ex;
            }
            _sw.Stop();
            InvokeMessage(string.Format(" ...Connected slaves in {0}.", _sw.Elapsed.ToShortFormattedString("0 ms")));
            _sw.Reset();
        }

        private void SetvApusInstancesAndStressTestsInDb() {
            _tileStressTestsWithDbIds = new Dictionary<TileStressTest, int>(_usedTileStressTests.Count);
            foreach (TileStressTest ts in _usedTileStressTests.Keys) {
                var slave = ts.BasicTileStressTest.Slaves[0];
                _resultsHelper.SetvApusInstance(slave.HostName, slave.IP, slave.Port, string.Empty, string.Empty, false);

                var scenarioKeys = new List<Scenario>(ts.AdvancedTileStressTest.Scenarios.Length);
                foreach (var kvp in ts.AdvancedTileStressTest.Scenarios)
                    scenarioKeys.Add(kvp.Key);

                int id = _resultsHelper.SetStressTest(ts.ToString(), _distributedTest.RunSynchronization.ToString(), ts.BasicTileStressTest.Connection.ToString(), ts.BasicTileStressTest.ConnectionProxy,
                        ts.BasicTileStressTest.Connection.ConnectionString, scenarioKeys.Combine(", "), ts.AdvancedTileStressTest.ScenarioRuleSet, ts.AdvancedTileStressTest.Concurrencies,
                        ts.AdvancedTileStressTest.Runs, ts.AdvancedTileStressTest.InitialMinimumDelay, ts.AdvancedTileStressTest.InitialMaximumDelay, ts.AdvancedTileStressTest.MinimumDelay, ts.AdvancedTileStressTest.MaximumDelay, 
                        ts.AdvancedTileStressTest.Shuffle, ts.AdvancedTileStressTest.ActionDistribution,
                        ts.AdvancedTileStressTest.MaximumNumberOfUserActions, ts.AdvancedTileStressTest.MonitorBefore, ts.AdvancedTileStressTest.MonitorAfter);

                _tileStressTestsWithDbIds.Add(ts, id);
            }

            _resultsHelper.SetvApusInstance(Dns.GetHostName(), NamedObjectRegistrar.Get<string>("IP"), NamedObjectRegistrar.Get<int>("Port"),
                    NamedObjectRegistrar.Get<string>("vApusVersion") ?? string.Empty, NamedObjectRegistrar.Get<string>("vApusChannel") ?? string.Empty,
                    true);
        }
        private void SendAndReceiveInitializeTest() {
            InvokeMessage("Initializing tests on slaves, please, be patient...");
            _sw.Start();
            List<int> stressTestIdsInDb = new List<int>(_usedTileStressTests.Count);
            foreach (var ts in _usedTileStressTests.Keys)
                stressTestIdsInDb.Add(_tileStressTestsWithDbIds.ContainsKey(ts) ? _tileStressTestsWithDbIds[ts] : 0);

            Exception exception = MasterSideCommunicationHandler.InitializeTests(_usedTileStressTests, stressTestIdsInDb, _resultsHelper.DatabaseName, _distributedTest.RunSynchronization, _distributedTest.MaxRerunsBreakOnLast);
            if (exception != null) {
                var ex = new Exception("Could not initialize one or more tests!\n" + exception);
                InvokeMessage(ex.ToString(), Level.Error);
                throw ex;
            }

            _sw.Stop();
            InvokeMessage(string.Format(" ...Test initialized in {0}.", _sw.Elapsed.ToShortFormattedString("0 ms")));
            _sw.Reset();
        }

        public void Start() {
            InvokeMessage("Starting the test...");
            Exception exception;
            MasterSideCommunicationHandler.StartTest(out exception);

            if (exception == null) {
                InvokeMessage(" ...Started!", Color.LightGreen);
            } else {
                Dispose();
                InvokeMessage(exception.ToString(), Level.Error);
                throw exception;
            }

            if (exception != null) {
                Dispose();
                var ex =
                    new Exception(string.Format("Could not start the Distributed Test.{0}{1}", Environment.NewLine,
                                                exception.ToString()));
                InvokeMessage(ex.ToString(), Level.Error);
                throw ex;
            }
        }
        #endregion

        #region Progress
        private void _masterCommunication_ListeningError(object sender, ListeningErrorEventArgs e) {
            if (_isDisposed)
                return;

            lock (_communicationLock) {
                try {
                    if (!_isDisposed && Finished < _totalTestCount) {

                        bool added = false;
                        foreach (TileStressTest tileStressTest in _usedTileStressTests.Keys) {
                            foreach (var slave in tileStressTest.BasicTileStressTest.Slaves)
                                if (slave.IP == e.SlaveIP && slave.Port == e.SlavePort) {
                                    _failed = AddUniqueToStringArray(_failed, tileStressTest.TileStressTestIndex);
                                    added = true;
                                    break;
                                }
                            if (added) break;
                        }

                        InvokeOnListeningError(e);

                        Stop();
                        //The test cannot be valid, therefore stop testing. It is stopped in the gui also, but stop it here explicitely for when the gui fails.

                        if (Finished == _totalTestCount)
                            HandleFinished();
                    }
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed handling communication error.", ex);
                }
            }
        }

        /// <summary>
        /// All the heavy lifting happens here (run sync).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _masterCommunication_OnTestProgressMessageReceived(object sender, TestProgressMessageReceivedEventArgs e) {
            if (_isDisposed)
                return;

            lock (_communicationLock) {
                try {
                    _hasResults = true;

                    TestProgressMessage tpm = e.TestProgressMessage;
                    TileStressTest originalTileStressTest = DivideEtImpera.GetOriginalTileStressTest(tpm.TileStressTestIndex, _usedTileStressTests);
                    RunStateChange combinedRunStateChanged = DivideEtImpera.PreProcessTestProgressMessage(_distributedTest.RunSynchronization, originalTileStressTest, tpm,
                        _testProgressMessages, _usedTileStressTests, _dividedRunInitializedOrDoneOnce);

                    bool okCancelError = true;
                    switch (tpm.StressTestStatus) {
                        case StressTestStatus.Busy:
                            okCancelError = false;
                            break;
                        case StressTestStatus.Ok:
                            _ok = AddUniqueToStringArray(_ok, tpm.TileStressTestIndex);
                            break;
                        case StressTestStatus.Cancelled:
                            _cancelled = AddUniqueToStringArray(_cancelled, tpm.TileStressTestIndex);
                            break;
                        case StressTestStatus.Error:
                            _failed = AddUniqueToStringArray(_failed, tpm.TileStressTestIndex);
                            break;
                    }

                    //Check if it is needed to do anything
                    if (Running != 0 && Cancelled == 0 && Failed == 0) {
                        string ts = "[TS " + tpm.TileStressTestIndex + "]";

                        //Threat this as stopped.
                        if (okCancelError) _runDoneOnce = AddUniqueToStringArray(_runDoneOnce, tpm.TileStressTestIndex);

                        switch (_distributedTest.RunSynchronization) {
                            case RunSynchronization.None:
                                if (tpm.RunStateChange == RunStateChange.ToRunInitializedFirstTime && _runInitialized != null) {//To definitely start them all at the same time.
                                    _runInitialized = AddUniqueToStringArray(_runInitialized, tpm.TileStressTestIndex);
                                    InvokeMessage("Received run initialized " + ts);
                                    if (_runInitialized.Length == Running) {
                                        _runInitialized = null;
                                        MasterSideCommunicationHandler.SendContinue();
                                        InvokeMessage("Sent continue");
                                    }
                                }
                                break;

                            //Send Break, wait for all stopped, send continue, wait for all started, send continue
                            case RunSynchronization.BreakOnFirstFinished:
                                if (tpm.RunStateChange == RunStateChange.ToRunInitializedFirstTime) {
                                    _runInitialized = AddUniqueToStringArray(_runInitialized, tpm.TileStressTestIndex);
                                    InvokeMessage("Received run initialized " + ts);
                                    if (_runInitialized.Length == Running) {
                                        _runInitialized = new string[] { };
                                        MasterSideCommunicationHandler.SendContinue();
                                        InvokeMessage("Sent continue");
                                    }
                                } else if (tpm.RunStateChange == RunStateChange.ToRunDoneOnce) {
                                    _runDoneOnce = AddUniqueToStringArray(_runDoneOnce, tpm.TileStressTestIndex);
                                    InvokeMessage("Received run done once " + ts);
                                    if (_runDoneOnce.Length == _totalTestCount) {
                                        _runDoneOnce = new string[] { };
                                        //Increment the index here to be able to continue to the next run.
                                        MasterSideCommunicationHandler.SendContinue();
                                        InvokeMessage("Sent continue");
                                    } else if (_runDoneOnce.Length == 1) {
                                        //Break the tests that are still in the previous run
                                        MasterSideCommunicationHandler.SendBreak();
                                        InvokeMessage("Sent break");
                                    }
                                }
                                break;

                            //Wait for all stopped, send continue, wait for all started, send continue
                            case RunSynchronization.BreakOnLastFinished:
                                if (tpm.RunStateChange == RunStateChange.ToRunInitializedFirstTime) {
                                    _runInitialized = AddUniqueToStringArray(_runInitialized, tpm.TileStressTestIndex);
                                    InvokeMessage("Received run initialized " + ts);
                                    if (_runInitialized.Length == Running) {
                                        _runInitialized = new string[] { };
                                        MasterSideCommunicationHandler.SendContinue();
                                        InvokeMessage("Sent continue");
                                    }
                                } else if (tpm.RunStateChange == RunStateChange.ToRunDoneOnce) {
                                    _runDoneOnce = AddUniqueToStringArray(_runDoneOnce, tpm.TileStressTestIndex);
                                    InvokeMessage("Received run done once " + ts);
                                    if (_runDoneOnce.Length == Running) {
                                        _runDoneOnce = new string[] { };
                                        _breakOnLastReruns = new ConcurrentDictionary<string, RerunCounter>();
                                        MasterSideCommunicationHandler.SendBreak();
                                        InvokeMessage("Sent break");
                                    }
                                } else if (tpm.RunStateChange == RunStateChange.ToRerunDone) {
                                    InvokeMessage("Received rerun done " + ts);
                                    if (IncrementReruns(tpm.TileStressTestIndex) == _distributedTest.MaxRerunsBreakOnLast) {
                                        _runDoneOnce = new string[] { };
                                        _breakOnLastReruns = new ConcurrentDictionary<string, RerunCounter>();
                                        MasterSideCommunicationHandler.SendBreak();
                                        InvokeMessage("Sent break");
                                    }
                                }
                                break;
                        }
                    }
                    //Take merging divided stress test loads into account, do not put the test progress message here as is.
                    InvokeOnTestProgressMessageReceivedDelayed();

                    if (Cancelled != 0 || Failed != 0) Stop(); //Test is invalid stop the test.
                    if (Finished == _totalTestCount) HandleFinished();
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Something went wrong when handling test progress in the distributed test core.", ex);
                }
            }
        }
        private int IncrementReruns(string tileStressTestIndex) {
            if (!_breakOnLastReruns.ContainsKey(tileStressTestIndex))
                _breakOnLastReruns.TryAdd(tileStressTestIndex, new RerunCounter());

            return _breakOnLastReruns[tileStressTestIndex].Increment();
        }

        /// <summary>
        ///     Use GetTestProgressMessage(TileStressTest) if you do not need all test progress messages, that is way faster. 
        ///     If you do need them all, put the return value in a new Dictionary, this to avoid recalcullations.
        ///     Key= index of the tile stress test. Value = pushed progress message from slave.
        ///     Divided progress is combined.
        /// </summary>
        public Dictionary<TileStressTest, TestProgressMessage> GetAllTestProgressMessages() {
            lock (_testProgressMessagesLock) {
                var testProgressMessages = new Dictionary<TileStressTest, TestProgressMessage>(_testProgressMessages.Count);

                bool sync = false;
                TimeSpan runSyncEstimatedRuntimeLeft = TimeSpan.MinValue;
                TimeSpan runSyncMeasuredRuntime = TimeSpan.MinValue;
                DateTime runSyncStartedAt = DateTime.MinValue;
                switch (_distributedTest.RunSynchronization) {
                    case RunSynchronization.BreakOnFirstFinished:
                        sync = true;
                        runSyncEstimatedRuntimeLeft = TimeSpan.MaxValue;
                        foreach (var ts in _testProgressMessages.Keys) {
                            var tpm = GetTestProgressMessage(ts);
                            if (tpm.TileStressTestIndex != null && tpm.EstimatedRuntimeLeft < runSyncEstimatedRuntimeLeft) {
                                runSyncEstimatedRuntimeLeft = tpm.EstimatedRuntimeLeft;
                                runSyncMeasuredRuntime = tpm.MeasuredRuntime;
                                runSyncStartedAt = tpm.StartedAt;
                            }
                        }
                        break;
                    case RunSynchronization.BreakOnLastFinished:
                        sync = true;
                        runSyncEstimatedRuntimeLeft = TimeSpan.MinValue;
                        foreach (var ts in _testProgressMessages.Keys) {
                            var tpm = GetTestProgressMessage(ts);
                            if (tpm.TileStressTestIndex != null && tpm.EstimatedRuntimeLeft > runSyncEstimatedRuntimeLeft) {
                                runSyncEstimatedRuntimeLeft = tpm.EstimatedRuntimeLeft;
                                runSyncMeasuredRuntime = tpm.MeasuredRuntime;
                                runSyncStartedAt = tpm.StartedAt;
                            }
                        } break;
                }

                foreach (var ts in _testProgressMessages.Keys) {
                    var tpm = GetTestProgressMessage(ts);
                    if (tpm.TileStressTestIndex != null) {
                        if (sync) {
                            tpm.EstimatedRuntimeLeft = runSyncEstimatedRuntimeLeft;
                            tpm.MeasuredRuntime = runSyncMeasuredRuntime;
                            tpm.StartedAt = runSyncStartedAt;
                        }
                        testProgressMessages.Add(ts, tpm);
                    }
                }
                return testProgressMessages;
            }
        }
        /// <summary>
        ///     If the test progress message is not found a new one with TileStressTestIndex == null is returned.
        ///     Divided progress is combined.
        /// </summary>
        /// <param name="tileStressTest"></param>
        /// <returns></returns>
        public TestProgressMessage GetTestProgressMessage(TileStressTest tileStressTest) {
            lock (_testProgressMessagesLock) {
                if (_testProgressMessages.ContainsKey(tileStressTest))
                    //_testProgressMessages[tileStressTest].Values = dictionary of divided stress test results, for when the same test is divided over 2 or more slaves.
                    return DivideEtImpera.GetMergedTestProgressMessage(tileStressTest, _testProgressMessages[tileStressTest].Values);

                return new TestProgressMessage();
            }
        }
        #endregion

        #region Finished
        /// <summary>
        ///     Stop the distributed test.
        /// </summary>
        public void Stop() { MasterSideCommunicationHandler.StopTest(); }
        private void HandleFinished() {
            if (!_finishedHandled) {
                _finishedHandled = true;

                MasterSideCommunicationHandler.ListeningError -= _masterCommunication_ListeningError;
                MasterSideCommunicationHandler.OnTestProgressMessageReceived -= _masterCommunication_OnTestProgressMessageReceived;

                ObjectRegistrar.Unregister(this);
                InvokeOnFinished();
            }
        }

        public void Dispose() {
            if (!_isDisposed) {
                try {
                    _isDisposed = true;

                    MasterSideCommunicationHandler.ListeningError -= _masterCommunication_ListeningError;
                    MasterSideCommunicationHandler.OnTestProgressMessageReceived -= _masterCommunication_OnTestProgressMessageReceived;

                    _hasResults = false;
                    _usedTileStressTests = null;
                    _totalTestCount = 0;
                    _testProgressMessages = null;
                    _sw = null;

                    _ok = null;
                    _cancelled = null;
                    _failed = null;

                    _runInitialized = null;
                    _runDoneOnce = null;

                    _tmrOnInvokeTestProgressMessageReceivedDelayed = null;
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed disposing distributed test core.", ex);
                }

                ObjectRegistrar.Unregister(this);
            }
        }
        #endregion

        #region Other
        private string[] AddUniqueToStringArray(string[] arr, string item) {
            foreach (string s in arr)
                if (s == item)
                    return arr;

            var newArr = new string[arr.Length + 1];
            for (int index = 0; index != arr.Length; index++)
                newArr[index] = arr[index];
            newArr[arr.Length] = item;

            return newArr;
        }
        //private void WriteRestConfig() {
        //    try {
        //        //Converter.ClearWrittenFiles();

        //        var testConfigCache = new JSONObjectTree();
        //        var distributedTestCache = JSONObjectTreeHelper.AddSubCache(_distributedTest.ToString(), testConfigCache);

        //        foreach (Tile tile in _distributedTest.Tiles)
        //            foreach (TileStressTest tileStressTest in tile)
        //                if (tileStressTest.Use) {
        //                    var slaves = tileStressTest.BasicTileStressTest.Slaves;
        //                    var newSlaves = new string[slaves.Length];
        //                    for (int i = 0; i != slaves.Length; i++)
        //                        newSlaves[i] = slaves[i].ToString();

        //                    var monitors = tileStressTest.BasicTileStressTest.Monitors;
        //                    var newMonitors = new string[monitors.Length];
        //                    for (int i = 0; i != monitors.Length; i++)
        //                        newMonitors[i] = monitors[i].ToString();

        //                    var scenarios = tileStressTest.AdvancedTileStressTest.Scenarios;
        //                    var newScenarios = new string[scenarios.Length];
        //                    for (int i = 0; i != scenarios.Length; i++)
        //                        newScenarios[i] = scenarios[i].Key.ToString();

        //                    JSONObjectTreeHelper.ApplyToRunningDistributedTestConfig(distributedTestCache,
        //                                            _distributedTest.RunSynchronization.ToString(),
        //                                            "Tile " + (tileStressTest.Parent as Tile).Index + " stress test " +
        //                                            tileStressTest.Index + " " +
        //                                            tileStressTest.BasicTileStressTest.Connection.Label,
        //                                            tileStressTest.BasicTileStressTest.Connection.ToString(),
        //                                            tileStressTest.BasicTileStressTest.ConnectionProxy,
        //                                            newMonitors,
        //                                            newSlaves,
        //                                            newScenarios,
        //                                            tileStressTest.AdvancedTileStressTest.ScenarioRuleSet,
        //                                            tileStressTest.AdvancedTileStressTest.Concurrencies,
        //                                            tileStressTest.AdvancedTileStressTest.Runs,
        //                                            tileStressTest.AdvancedTileStressTest.MinimumDelay,
        //                                            tileStressTest.AdvancedTileStressTest.MaximumDelay,
        //                                            tileStressTest.AdvancedTileStressTest.Shuffle,
        //                                            tileStressTest.AdvancedTileStressTest.ActionDistribution,
        //                                            tileStressTest.AdvancedTileStressTest.MaximumNumberOfUserActions,
        //                                            tileStressTest.AdvancedTileStressTest.MonitorBefore,
        //                                            tileStressTest.AdvancedTileStressTest.MonitorAfter);
        //                }
        //        JSONObjectTreeHelper.RunningTestConfig = testConfigCache;
        //        //Converter.WriteToFile(testConfigCache, "TestConfig");
        //    } catch {
        //    }
        //}
        #endregion

        #endregion

        /// <summary>
        /// For break on last max reruns functionality, Initial Value == 0 --> incremented on every run after the first (for a certain tile stress test)
        /// </summary>
        private class RerunCounter {
            private int _counter;
            public int Counter { get { return _counter; } }
            /// <summary>
            /// Increments and returns the counter thread-safe.
            /// </summary>
            /// <returns></returns>
            public int Increment() { return Interlocked.Increment(ref _counter); }
        }
    }
}