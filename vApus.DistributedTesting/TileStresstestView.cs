/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using vApus.Monitor;
using vApus.Publish;
using vApus.Results;
using vApus.SolutionTree;
using vApus.StressTest;
using vApus.Util;

namespace vApus.DistributedTest {
    public partial class TileStressTestView : BaseSolutionComponentView {
        public event EventHandler<vApus.StressTest.TestInitializedEventArgs> TestInitialized;

        #region Fields
        /// <summary>
        ///     Lock break, continue, push message
        /// </summary>
        private readonly object _lock = new object();

        private readonly StressTest.StressTest _stressTest;
        private string _tileStressTest, _tileStressTestIndex, _resultSetId, _distributedTest;
        private string[] _monitors;

        /// <summary>
        ///     In seconds how fast the stress test progress will be updated.
        /// </summary>
        private const int PROGRESSUPDATEDELAY = 5;
        /// <summary>
        ///     Countdown for the update.
        /// </summary>
        private int _progressCountDown;

        private bool _canUpdateMetrics = false; //Can only be updated when a run is busy.

        private StressTestCore _stressTestCore;
        private StressTestResult _stressTestResult;
        /// <summary>
        ///     Caching the results to visualize in the fastResultsControl.
        /// </summary>
        private FastStressTestMetricsCache _stressTestMetricsCache;
        private StressTestStatus _stressTestStatus;

        private ResultsHelper _resultsHelper = new ResultsHelper();

        /// <summary>
        ///     Don't send push messages anymore if it is finished (stop on form closing);
        /// </summary>
        private bool _finishedSent;
        #endregion

        #region Properties

        /// <summary>
        ///     Store to identify the right stress test.
        /// </summary>
        public string TileStressTestIndex {
            get { return _tileStressTestIndex; }
            set { _tileStressTestIndex = value; }
        }
        public string TileStressTest {
            get { return _tileStressTest; }
            set { _tileStressTest = value; }
        }
        public string ResultSetId {
            get { return _resultSetId; }
            set { _resultSetId = value; }
        }
        public string DistributedTest {
            get { return _distributedTest; }
            set { _distributedTest = value; }
        }
        /// <summary>
        /// Monitors to string.
        /// </summary>
        public string[] Monitors {
            get { return _monitors; }
            set { _monitors = value; }
        }
        public RunSynchronization RunSynchronization { get; set; }
        public int MaxRerunsBreakOnLast { get; set; }
        public StressTestResult StressTestResult {
            get { return _stressTestResult; }
        }

        /// <summary>
        /// For adding results to the database.
        /// </summary>
        public int StressTestIdInDb {
            get { return _resultsHelper.StressTestId; }
            set { _resultsHelper.StressTestId = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        ///     Designer time constructor
        /// </summary>
        public TileStressTestView() { InitializeComponent(); }
        public TileStressTestView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            Solution.RegisterForCancelFormClosing(this);
            _stressTest = SolutionComponent as StressTest.StressTest;

            InitializeComponent();
            if (IsHandleCreated)
                SetGui();
            else
                HandleCreated += StressTestProjectView_HandleCreated;
        }
        #endregion

        #region Functions

        #region Set the Gui
        private void StressTestProjectView_HandleCreated(object sender, EventArgs e) {
            SetGui();
        }

        private void SetGui() {
            Text = SolutionComponent.ToString();
            // fastResultsControl.ResultsHelper = _resultsHelper;
        }

        public override void Refresh() {
            base.Refresh();
            SetGui();
        }
        #endregion

        #region Start
        public void ConnectToExistingDatabase(string host, int port, string databaseName, string user, string password) {
            try {
                _resultsHelper.ConnectToExistingDatabase(host, port, databaseName, user, password);
            } catch {
                throw new Exception("MAKE SURE THAT YOU DO NOT TARGET 'localhost', '127.0.0.1', '0:0:0:0:0:0:0:1' or '::1' IN 'Options' > 'Saving Test Results' and that the connection limit (max_connections setting in my.ini) is set high enough!\nA connection to the results server could not be made!");
            }
        }
        /// <summary>
        ///     Thread safe
        /// </summary>
        public void InitializeTest() {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                Cursor = Cursors.WaitCursor;
                btnStop.Enabled = true;
                try { LocalMonitor.StartMonitoring(PROGRESSUPDATEDELAY * 1000); } catch { AddEvent("Could not initialize the local monitor, something is wrong with your WMI.", Level.Error); }
                tmrProgress.Interval = PROGRESSUPDATEDELAY * 1000;
                tmrProgress.Start();

                fastResultsControl.SetStressTestInitialized();
                _stressTestResult = null;
                _stressTestMetricsCache = new FastStressTestMetricsCache();
                fastResultsControl.SetConfigurationControls(_stressTest);

                _progressCountDown = PROGRESSUPDATEDELAY - 1;
                try {
                    _stressTestCore = new StressTestCore(_stressTest);
                    _stressTestCore.WaitWhenInitializedTheFirstRun = true;
                    _stressTestCore.ResultsHelper = _resultsHelper;
                    _stressTestCore.RunSynchronization = RunSynchronization;
                    _stressTestCore.MaxRerunsBreakOnLast = MaxRerunsBreakOnLast;
                    _stressTestCore.StressTestStarted += _stressTestCore_StressTestStarted;
                    _stressTestCore.ConcurrencyStarted += _stressTestCore_ConcurrentUsersStarted;
                    _stressTestCore.ConcurrencyStopped += _stressTestCore_ConcurrencyStopped;
                    _stressTestCore.RunInitializedFirstTime += _stressTestCore_RunInitializedFirstTime;
                    _stressTestCore.RunStarted += _stressTestCore_RunStarted;
                    _stressTestCore.RunDoneOnce += _stressTestCore_RunDoneOnce;
                    _stressTestCore.RerunStarted += _stressTestCore_RerunStarted;
                    _stressTestCore.RerunDone += _stressTestCore_RerunDone;
                    _stressTestCore.RunStopped += _stressTestCore_RunStopped;
                    _stressTestCore.Message += _stressTestCore_Message;
                    _stressTestCore.OnRequest += _stressTestCore_OnRequest;

                    _stressTestCore.TestInitialized += _stressTestCore_TestInitialized;
                    ThreadPool.QueueUserWorkItem((state) => { _stressTestCore.InitializeTest(); }, null);

                    PublishConfiguration();
                } catch (Exception ex) {
                    Stop(ex);

                    if (TestInitialized != null) TestInitialized(this, new StressTest.TestInitializedEventArgs(ex));
                }
            }, null);
        }
        private void _stressTestCore_TestInitialized(object sender, StressTest.TestInitializedEventArgs e) {
            _stressTestCore.TestInitialized -= _stressTestCore_TestInitialized;
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                if (e.Exception == null) {
                    try {
                        fastResultsControl.SetClientMonitoring(_stressTestCore.BusyThreadCount, LocalMonitor.CPUUsage,
                                                              (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory,
                                                              LocalMonitor.Nic, LocalMonitor.NicBandwidth,
                                                              LocalMonitor.NicSent, LocalMonitor.NicReceived);
                    } catch { } //Exception on false WMI. 
                } else {
                    Stop(e.Exception);
                }
                Cursor = Cursors.Default;

                if (TestInitialized != null) TestInitialized(this, e);
            }, null);
        }

        /// <summary>
        ///     Thread safe
        /// </summary>
        public void StartTest() {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                if (_stressTestCore == null) return;

                _stressTestStatus = StressTestStatus.Busy;

                //The stress test threadpool is blocking so we run this on another thread.
                var stressTestThread = new Thread(() => {
                    Exception ex = null;
                    try {
                        _stressTestStatus = _stressTestCore.ExecuteStressTest();
                        _stressTestResult = _stressTestCore.StressTestResult;
                    } catch (Exception e) { ex = e; } finally {
                        if (_stressTestCore != null && !_stressTestCore.IsDisposed)
                            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                                Stop(ex);
                            }, null);
                    }
                });

                stressTestThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                stressTestThread.IsBackground = true;
                stressTestThread.Start();
            }, null);
        }
        #endregion

        /// <summary>
        ///     Thread safe
        /// </summary>
        public void Break() {
            lock (_lock)
                if (_stressTestStatus == StressTestStatus.Busy)
                    _stressTestCore.Break();

        }

        /// <summary>
        ///     Thread safe
        /// </summary>
        /// <param name="continueCounter">Every time the execution is paused the continue counter is incremented by one.</param>
        public void Continue(int continueCounter) {
            lock (_lock)
                if (_stressTestStatus == StressTestStatus.Busy)
                    _stressTestCore.Continue(continueCounter);
        }

        /// <summary>
        ///     Thread safe, Keeping the shared run for a divided tile stress test in sync.
        /// </summary>
        public void ContinueDivided() {
            lock (_lock)
                if (_stressTestStatus == StressTestStatus.Busy)
                    _stressTestCore.ContinueDivided();
        }

        #region Progress
        private void tmrProgressDelayCountDown_Tick(object sender, EventArgs e) { fastResultsControl.SetCountDownProgressDelay(_progressCountDown--); }
        private void tmrProgress_Tick(object sender, ElapsedEventArgs e) {
            try {
                fastResultsControl.SetClientMonitoring(
                    _stressTestCore == null ? 0 : _stressTestCore.BusyThreadCount, LocalMonitor.CPUUsage, (int)LocalMonitor.MemoryUsage,
                    (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.Nic, LocalMonitor.NicBandwidth, LocalMonitor.NicSent, LocalMonitor.NicReceived);
            } catch { } //Exception on false WMI. 

            if (_canUpdateMetrics) {
                fastResultsControl.UpdateFastConcurrencyResults(_stressTestMetricsCache.GetConcurrencyMetrics(_stressTest.SimplifiedFastResults), true);
                List<StressTestMetrics> runMetrics = _stressTestMetricsCache.GetRunMetrics(_stressTest.SimplifiedFastResults);
                fastResultsControl.UpdateFastRunResults(runMetrics, false);

                //Set rerunning
                fastResultsControl.SetRerunning(runMetrics.Count == 0 ? false : runMetrics[runMetrics.Count - 1].RerunCount != 0);
            }
            _progressCountDown = PROGRESSUPDATEDELAY;

            SendPushMessage(RunStateChange.None, false, false);

            PublishProgress(RunStateChange.None);
        }

        private void _stressTestCore_StressTestStarted(object sender, StressTestResultEventArgs e) {
            _stressTestResult = e.StressTestResult;
            fastResultsControl.SetStressTestStarted(e.StressTestResult.StartedAt);
        }

        private void _stressTestCore_ConcurrentUsersStarted(object sender, ConcurrencyResultEventArgs e) {
            _progressCountDown = PROGRESSUPDATEDELAY;
            StopProgressDelayCountDown();
            //Update the metrics.
            fastResultsControl.UpdateFastConcurrencyResults(_stressTestMetricsCache.AddOrUpdate(e.Result, _stressTest.SimplifiedFastResults), true);
            fastResultsControl.SetRerunning(false);
        }
        private void _stressTestCore_ConcurrencyStopped(object sender, ConcurrencyResultEventArgs e) {
            SendPushMessage(RunStateChange.None, false, true);

            PublishProgress(RunStateChange.None);
        }

        private void _stressTestCore_RunInitializedFirstTime(object sender, RunResultEventArgs e) {
            StopProgressDelayCountDown();

            fastResultsControl.UpdateFastRunResults(_stressTestMetricsCache.AddOrUpdate(e.Result, _stressTest.SimplifiedFastResults), true);
            fastResultsControl.UpdateFastConcurrencyResults(_stressTestMetricsCache.GetConcurrencyMetrics(_stressTest.SimplifiedFastResults), false);

            SendPushMessage(RunStateChange.ToRunInitializedFirstTime, false, false);

            PublishProgress(RunStateChange.ToRunInitializedFirstTime);

            fastResultsControl.SetRerunning(false);

            _progressCountDown = PROGRESSUPDATEDELAY;
            tmrProgress.Stop();
            fastResultsControl.SetCountDownProgressDelay(_progressCountDown--);
            tmrProgressDelayCountDown.Start();
            tmrProgress.Start();
        }

        private void _stressTestCore_RunStarted(object sender, RunResultEventArgs e) { _canUpdateMetrics = true; }
        private void _stressTestCore_RunDoneOnce(object sender, EventArgs e) {
            SendPushMessage(RunStateChange.ToRunDoneOnce, false, false);

            PublishProgress(RunStateChange.ToRunDoneOnce);
        }
        private void _stressTestCore_RerunStarted(object sender, EventArgs e) {

        }
        private void _stressTestCore_RerunDone(object sender, EventArgs e) {
            SendPushMessage(RunStateChange.ToRerunDone, false, false);

            PublishProgress(RunStateChange.ToRerunDone);
        }
        private void _stressTestCore_RunStopped(object sender, RunResultEventArgs e) {
            _canUpdateMetrics = false;
            SendPushMessage(RunStateChange.None, true, false);

            PublishProgress(RunStateChange.None);
        }

        private void _stressTestCore_OnRequest(object sender, OnRequestEventArgs e) {
            PublishRequest(e.RequestResults);
        }

        /// <summary>
        /// </summary>
        /// <param name="runStateChange"></param>
        private void SendPushMessage(RunStateChange runStateChange, bool runFinished, bool concurrencyFinished) {
            if (!_finishedSent) {
                var estimatedRuntimeLeft = FastStressTestMetricsHelper.GetEstimatedRuntimeLeft(_stressTestResult, _stressTest.Concurrencies.Length, _stressTest.Runs);
                var events = new List<EventPanelEvent>();
                try { events = fastResultsControl.GetEvents(); } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed getting events.", ex);
                }
                SlaveSideCommunicationHandler.SendPushMessage(_tileStressTestIndex, _stressTestMetricsCache, _stressTestStatus, fastResultsControl.StressTestStartedAt,
                    fastResultsControl.MeasuredRuntime, estimatedRuntimeLeft, _stressTestCore, events, runStateChange, runFinished, concurrencyFinished);
                if (_stressTestStatus != StressTestStatus.Busy) _finishedSent = true;
            }
        }

        /// <summary>
        ///     Refreshes the messages from the StressTestCore for a selected node and refreshes the listed results.
        /// </summary>
        private void _stressTestCore_Message(object sender, MessageEventArgs e) { AddEvent(e.Message, e.Color, e.Level); }

        private void AddEvent(string message, Level level = Level.Info) { AddEvent(message, Color.Empty, level); }

        private void AddEvent(string message, Color color, Level level = Level.Info) {
            if (color == Color.Empty) fastResultsControl.AddEvent(message, level); else fastResultsControl.AddEvent(message, color, level);

            _resultsHelper.AddMessageInMemory((int)level, message);
        }
        #endregion

        #region Stop
        private void TileStressTestView_FormClosing(object sender, FormClosingEventArgs e) {
            if (!btnStop.Enabled || MessageBox.Show("Are you sure you want to close a running test?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                StopStressTest();

                tmrProgress.Stop();
                StopProgressDelayCountDown();


                if (_stressTestCore != null && !_stressTestCore.IsDisposed) {
                    _stressTestCore.Dispose();
                    _stressTestCore = null;
                }

                SendPushMessage(RunStateChange.None, false, false);

                PublishProgress(RunStateChange.None);
            } else {
                Solution.ExplicitCancelFormClosing = true;
                e.Cancel = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e) {
            PerformStopClick();
        }

        /// <summary>
        ///     To stop the test from the slave side communication handler.
        /// </summary>
        public void PerformStopClick() {
            if (btnStop.Enabled) {
                int busyThreadCount = -1;
                if (_stressTestCore != null) {
                    busyThreadCount = _stressTestCore.BusyThreadCount;
                    _stressTestCore.Cancel(); // Can only be cancelled once, calling multiple times is not a problem.
                }

                //Nasty, but last resort.
                ThreadPool.QueueUserWorkItem((state) => {
                    for (int i = 0; i != 10001; i++) {
                        if (_stressTestCore == null) break;
                        Thread.Sleep(1);
                    }
                    if (_stressTestCore != null || busyThreadCount == 0)
                        SynchronizationContextWrapper.SynchronizationContext.Send((x) => {
                            Stop();
                        }, null);
                });
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="ex">The exception if failed.</param>
        private void Stop(Exception ex = null) {
            Cursor = Cursors.WaitCursor;
            try {
                StopStressTest();

                tmrProgress.Stop();
                StopProgressDelayCountDown();

                btnStop.Enabled = false;
                if (ex != null)
                    _stressTestStatus = StressTestStatus.Error;

                string message;
                fastResultsControl.SetStressTestStopped(_stressTestStatus, out message);
                _resultsHelper.AddMessageInMemory(0, message);
                _resultsHelper.DoAddMessagesToDatabase();

                if (_stressTestCore != null && !_stressTestCore.IsDisposed) {
                    _stressTestCore.Dispose();
                    _stressTestCore = null;
                }

                SendPushMessage(RunStateChange.None, false, false);

                PublishProgress(RunStateChange.None);
            } catch (Exception eeee) {
                MessageBox.Show(eeee.ToString());
                Loggers.Log(Level.Error, "Failed stopping the test.", eeee);
            }
            Cursor = Cursors.Default;
        }

        /// <summary>
        ///     Only used in stop
        /// </summary>
        private void StopStressTest() {
            if (_stressTestCore != null && !_stressTestCore.IsDisposed) {
                try {
                    fastResultsControl.SetClientMonitoring(_stressTestCore.BusyThreadCount, LocalMonitor.CPUUsage,
                                                          (int)LocalMonitor.MemoryUsage,
                                                          (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.Nic, LocalMonitor.NicBandwidth,
                                                          LocalMonitor.NicSent, LocalMonitor.NicReceived);
                } catch { } //Exception on false WMI. 

                fastResultsControl.UpdateFastConcurrencyResults(_stressTestMetricsCache.GetConcurrencyMetrics(false), true);
                fastResultsControl.UpdateFastRunResults(_stressTestMetricsCache.GetRunMetrics(false), false);

                fastResultsControl.SetRerunning(false);

                // Can only be cancelled once, calling multiple times is not a problem.
                if (_stressTestCore != null && !_stressTestCore.IsDisposed)
                    try {
                        _stressTestCore.Cancel();
                    } catch (Exception ex) {
                        Loggers.Log(Level.Error, "Failed cancelling the test.", ex);
                    }
            }

            fastResultsControl.SetStressTestStopped();
            _stressTestResult = null;
            _canUpdateMetrics = false;

            _resultsHelper.DoAddMessagesToDatabase();
        }

        private void StopProgressDelayCountDown() {
            try {
                tmrProgressDelayCountDown.Stop();
                if (fastResultsControl != null && !fastResultsControl.IsDisposed)
                    fastResultsControl.SetCountDownProgressDelay(-1);
            } catch {
                //Don't care.
            }
        }
        #endregion

        #region Publish
        private void PublishConfiguration() {
            if (Publisher.Settings.PublisherEnabled && Publisher.Settings.PublishTestsConfiguration) {
                var publishItem = new TileStressTestConfiguration();

                publishItem.DistributedTest = _distributedTest;
                publishItem.TileStressTest = _tileStressTest;
                publishItem.Connection = _stressTest.Connection.ToString();
                publishItem.ConnectionProxy = _stressTest.ConnectionProxy;

                var scenariosAndWeights = new KeyValuePair<string, uint>[_stressTest.Scenarios.Length];
                for (int i = 0; i != _stressTest.Scenarios.Length; i++) {
                    var kvp = _stressTest.Scenarios[i];
                    scenariosAndWeights[i] = new KeyValuePair<string, uint>(kvp.Key.ToString(), kvp.Value);
                }
                publishItem.ScenariosAndWeights = scenariosAndWeights;

                publishItem.ScenarioRuleSet = _stressTest.ScenarioRuleSet;

                publishItem.Monitors = _monitors;

                publishItem.Concurrencies = _stressTest.Concurrencies;
                publishItem.Runs = _stressTest.Runs;
                publishItem.InitialMinimumDelayInMilliseconds = _stressTest.InitialMinimumDelay;
                publishItem.InitialMaximumDelayInMilliseconds = _stressTest.InitialMaximumDelay;
                publishItem.MinimumDelayInMilliseconds = _stressTest.MinimumDelay;
                publishItem.MaximumDelayInMilliseconds = _stressTest.MaximumDelay;
                publishItem.Shuffle = _stressTest.Shuffle;
                publishItem.ActionDistribution = _stressTest.ActionDistribution;
                publishItem.MaximumNumberOfUserActions = _stressTest.MaximumNumberOfUserActions;
                publishItem.MonitorBeforeInMinutes = _stressTest.MonitorBefore;
                publishItem.MonitorAfterInMinutes = _stressTest.MonitorAfter;
                publishItem.UseParallelExecutionOfRequests = _stressTest.UseParallelExecutionOfRequests;
                publishItem.PersistentConnectionsPerHostname = _stressTest.PersistentConnectionsPerHostname;
                publishItem.MaximumPersistentConnections = _stressTest.MaximumPersistentConnections;

                Publisher.Send(publishItem, _resultSetId);
            }
        }

        private void PublishProgress(RunStateChange runStateChange) {
            //PublishFastConcurencyResults(runStateChange);
            //PublishFastRunResults(runStateChange);
            PublishClientMonitoring();
        }
        //private void PublishFastConcurencyResults(RunStateChange runStateChange) {
        //    if (Publisher.Settings.PublisherEnabled && Publisher.Settings.PublishTestsFastConcurrencyResults && _stressTestMetricsCache != null) {
        //        List<StressTestMetrics> metrics = _stressTestMetricsCache.GetConcurrencyMetrics(_stressTest.SimplifiedFastResults);
        //        if (metrics.Count != 0) {
        //            StressTestMetrics lastMetrics = metrics[metrics.Count - 1];
        //            var publishItem = new FastConcurrencyResults();
        //            publishItem.Test = _tileStressTest;
        //            publishItem.StartMeasuringTimeInMillisecondsSinceEpochUtc = (long)(lastMetrics.StartMeasuringTime.ToUniversalTime() - PublishItem.EpochUtc).TotalMilliseconds;
        //            publishItem.EstimatedTimeLeftInMilliseconds = (long)lastMetrics.EstimatedTimeLeft.TotalMilliseconds;
        //            publishItem.MeasuredTimeInMilliseconds = (long)lastMetrics.MeasuredTime.TotalMilliseconds;
        //            publishItem.Concurrency = lastMetrics.Concurrency;

        //            publishItem.RequestsProcessed = lastMetrics.RequestsProcessed;
        //            publishItem.Requests = lastMetrics.Requests;

        //            publishItem.ResponsesPerSecond = lastMetrics.ResponsesPerSecond;

        //            publishItem.UserActionsPerSecond = lastMetrics.UserActionsPerSecond;
        //            publishItem.AverageResponseTimeInMilliseconds = (long)lastMetrics.AverageResponseTime.TotalMilliseconds;
        //            publishItem.MaxResponseTimeInMilliseconds = (long)lastMetrics.MaxResponseTime.TotalMilliseconds;
        //            publishItem.Percentile95thResponseTimesInMilliseconds = (long)lastMetrics.Percentile95thResponseTimes.TotalMilliseconds;
        //            publishItem.Percentile99thResponseTimesInMilliseconds = (long)lastMetrics.Percentile99thResponseTimes.TotalMilliseconds;
        //            publishItem.AverageTop5ResponseTimesInMilliseconds = (long)lastMetrics.AverageTop5ResponseTimes.TotalMilliseconds;
        //            publishItem.AverageDelayInMilliseconds = (long)lastMetrics.AverageDelay.TotalMilliseconds;
        //            publishItem.Errors = (long)lastMetrics.Errors;

        //            publishItem.RunStateChange = runStateChange.ToString();
        //            publishItem.StressTestStatus = _stressTestStatus.ToString();

        //            Publisher.Post(publishItem, _resultSetId);
        //        }
        //    }
        //}
        //private void PublishFastRunResults(RunStateChange runStateChange) {
        //    if (Publisher.Settings.PublisherEnabled && Publisher.Settings.PublishTestsFastRunResults && _stressTestMetricsCache != null) {
        //        List<StressTestMetrics> metrics = _stressTestMetricsCache.GetRunMetrics(_stressTest.SimplifiedFastResults);
        //        if (metrics.Count != 0) {
        //            StressTestMetrics lastMetrics = metrics[metrics.Count - 1];
        //            var publishItem = new FastRunResults();
        //            publishItem.Test = _tileStressTest;
        //            publishItem.StartMeasuringTimeInMillisecondsSinceEpochUtc = (long)(lastMetrics.StartMeasuringTime.ToUniversalTime() - PublishItem.EpochUtc).TotalMilliseconds;
        //            publishItem.EstimatedTimeLeftInMilliseconds = (long)lastMetrics.EstimatedTimeLeft.TotalMilliseconds;
        //            publishItem.MeasuredTimeInMilliseconds = (long)lastMetrics.MeasuredTime.TotalMilliseconds;
        //            publishItem.Concurrency = lastMetrics.Concurrency;

        //            publishItem.Run = lastMetrics.Run;
        //            publishItem.RerunCount = lastMetrics.RerunCount;

        //            publishItem.RequestsProcessed = lastMetrics.RequestsProcessed;
        //            publishItem.Requests = lastMetrics.Requests;

        //            publishItem.ResponsesPerSecond = lastMetrics.ResponsesPerSecond;

        //            publishItem.UserActionsPerSecond = lastMetrics.UserActionsPerSecond;
        //            publishItem.AverageDelayInMilliseconds = (long)lastMetrics.AverageResponseTime.TotalMilliseconds;
        //            publishItem.MaxResponseTimeInMilliseconds = (long)lastMetrics.MaxResponseTime.TotalMilliseconds;
        //            publishItem.Percentile95thResponseTimesInMilliseconds = (long)lastMetrics.Percentile95thResponseTimes.TotalMilliseconds;
        //            publishItem.Percentile99thResponseTimesInMilliseconds = (long)lastMetrics.Percentile99thResponseTimes.TotalMilliseconds;
        //            publishItem.AverageTop5ResponseTimesInMilliseconds = (long)lastMetrics.AverageTop5ResponseTimes.TotalMilliseconds;
        //            publishItem.AverageDelayInMilliseconds = (long)lastMetrics.AverageDelay.TotalMilliseconds;
        //            publishItem.Errors = (long)lastMetrics.Errors;

        //            publishItem.RunStateChange = runStateChange.ToString();
        //            publishItem.StressTestStatus = _stressTestStatus.ToString();

        //            Publisher.Post(publishItem, _resultSetId);
        //        }
        //    }
        //}

        private void PublishClientMonitoring() {
            if (Publisher.Settings.PublisherEnabled && Publisher.Settings.PublishTestsClientMonitoring) {
                var publishItem = new ClientMonitorMetrics();
                publishItem.Test = _tileStressTest;
                publishItem.BusyThreadCount = _stressTestCore == null || _stressTestCore.IsDisposed ? 0 : _stressTestCore.BusyThreadCount;
                publishItem.CPUUsageInPercent = LocalMonitor.CPUUsage;
                publishItem.MemoryUsageInMB = LocalMonitor.MemoryUsage;
                publishItem.TotalVisibleMemoryInMB = LocalMonitor.TotalVisibleMemory;
                publishItem.Nic = LocalMonitor.Nic;
                publishItem.NicBandwidthInMbps = LocalMonitor.NicBandwidth;
                publishItem.NicSentInPercent = LocalMonitor.NicSent;
                publishItem.NicReceivedInPercent = LocalMonitor.NicReceived;

                Publisher.Send(publishItem, _resultSetId);
            }
        }

        private void PublishMessage(int level, string message) {
            if (Publisher.Settings.PublisherEnabled && Publisher.Settings.PublishTestsMessages && level >= Publisher.Settings.MessageLevel) {
                var publishItem = new TestEvent();
                publishItem.Test = _tileStressTest;
                publishItem.TestEventType = (int)TestEventType.TestMessage;
                publishItem.Parameters = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("Level", level.ToString()),
                    new KeyValuePair<string, string>("Message", message)
                };

                Publisher.Send(publishItem, _resultSetId);
            }
        }

        private void PublishRunDoneOnce(int concurrencyId, int run) {
            if (Publisher.Settings.PublisherEnabled && Publisher.Settings.PublishTestRequestResults) {
                var publishItem = new TestEvent();
                publishItem.Test = _stressTest.ToString();
                publishItem.TestEventType = (int)TestEventType.RunDoneOnce;
                publishItem.Parameters = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("ConcurrencyId", concurrencyId.ToString()),
                    new KeyValuePair<string, string>("Run", run.ToString())
                };

                Publisher.Send(publishItem, _resultSetId);
            }
        }
        private void PublishRerunDone(int concurrencyId, int run, int rerun) {
            if (Publisher.Settings.PublisherEnabled && Publisher.Settings.PublishTestRequestResults) {
                var publishItem = new TestEvent();
                publishItem.Test = _stressTest.ToString();
                publishItem.TestEventType = (int)TestEventType.RerunDone;
                publishItem.Parameters = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("ConcurrencyId", concurrencyId.ToString()),
                    new KeyValuePair<string, string>("Run", run.ToString()),
                    new KeyValuePair<string, string>("Rerun", rerun.ToString())
                };

                Publisher.Send(publishItem, _resultSetId);
            }
        }

        private void PublishRequest(RequestResults requestResults) {
            if (Publisher.Settings.PublisherEnabled && Publisher.Settings.PublishTestRequestResults) Publisher.Send(requestResults, _resultSetId);
        }

        #endregion

        #endregion
    }
}