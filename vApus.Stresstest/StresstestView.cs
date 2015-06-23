/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Monitor;
using vApus.Results;
using vApus.SolutionTree;
using vApus.Util;
using RandomUtils.Log;
using RandomUtils;

namespace vApus.StressTest {
    public partial class StressTestView : BaseSolutionComponentView {

        #region Fields
        private StressTest _stressTest;

        private ScheduleDialog _scheduleDialog;

        /// <summary>
        ///     In seconds how fast the stress test progress will be updated.
        /// </summary>
        private const int PROGRESSUPDATEDELAY = 5;
        /// <summary>
        ///     Countdown for the update.
        /// </summary>
        private int _progressCountDown;

        private bool _canUpdateMetrics = false; //Can only be updated when a run is busy.
        private bool _simplifiedMetricsReturned = false; //Only send a warning to the user once.

        private ResultsHelper _resultsHelper = new ResultsHelper();

        /// <summary>
        ///     Caching the results to visualize in the fast results control.
        /// </summary>
        private FastStressTestMetricsCache _stressTestMetricsCache;
        private StressTestCore _stressTestCore;
        private StressTestResult _stressTestResult;
        private StressTestStatus _stressTestStatus; //Set on calling Stop(...);

        private MonitorMetricsCache _monitorMetricsCache;
        private readonly List<MonitorView> _monitorViews = new List<MonitorView>();
        private Countdown _monitorBeforeCountDown, _monitorAfterCountDown;
        private int _monitorsInitialized;
        private ConcurrencyResult _monitorBeforeBogusConcurrencyResult, _monitorAfterBogusConcurrencyResult;
        private RunResult _monitorBeforeBogusRunResult, _monitorAfterBogusRunResult;
        #endregion

        #region Properties
        public StressTestStatus StressTestStatus {
            get { return _stressTestStatus; }
        }
        #endregion

        #region Constructor
        /// <summary>
        ///     Designer time constructor.
        /// </summary>
        public StressTestView() {
            InitializeComponent();
        }
        public StressTestView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            Solution.RegisterForCancelFormClosing(this);
            _stressTest = SolutionComponent as StressTest;

            InitializeComponent();

            if (IsHandleCreated)
                SetGui();
            else
                HandleCreated += StressTestView_HandleCreated;
        }
        #endregion

        #region Functions

        #region Set the Gui
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        private void StressTestView_HandleCreated(object sender, EventArgs e) {
            SetGui();
        }
        private void SetGui() {
            Text = SolutionComponent.ToString();
            solutionComponentPropertyPanel.SolutionComponent = SolutionComponent;

            btnStart.Enabled = !btnStop.Enabled;

            if (_stressTest.Connection.IsEmpty || _stressTest.Connection.ConnectionProxy.IsEmpty ||
                _stressTest.Scenarios.Length == 0 || _stressTest.Scenarios[0].Key.ScenarioRuleSet.IsEmpty)
                btnStart.Enabled = false;
        }
        public override void Refresh() {
            base.Refresh();
            SetGui();
            solutionComponentPropertyPanel.Refresh();
        }
        #endregion

        /// <summary>
        /// Get all monitor result caches for al the running monitors.
        /// </summary>
        /// <returns></returns>
        private List<MonitorResult> GetMonitorResultCaches() {
            var l = new List<MonitorResult>();
            if (_monitorViews != null) foreach (var view in _monitorViews) l.Add(view.GetMonitorResultCache());
            return l;
        }

        #region Start
        /// <summary>
        ///     Schedule the test at another time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSchedule_Click(object sender, EventArgs e) {
            this.ActiveControl = tc;
            _scheduleDialog = (btnSchedule.Tag is DateTime)
                                    ? new ScheduleDialog((DateTime)btnSchedule.Tag)
                                    : new ScheduleDialog();
            if (_scheduleDialog.ShowDialog() == DialogResult.OK) {
                if (_scheduleDialog.ScheduledAt > DateTime.Now) {
                    btnSchedule.Tag = _scheduleDialog.ScheduledAt;
                } else {
                    btnSchedule.Text = string.Empty;
                    btnSchedule.Tag = null;
                }
                btnStart_Click(this, null);
            } else {
                btnSchedule.Text = string.Empty;
            }
            _scheduleDialog = null;
        }

        /// <summary>
        ///     Start a test with or without monitoring it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e) {
            this.ActiveControl = tc;
            StartStressTest(true);
        }
        async public void StartStressTest(bool allowMessageBox) {
            if (fastResultsControl.HasResults && (!allowMessageBox ||
                MessageBox.Show("Starting the test will clear the previous results.\nDo you want to continue?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                ) return;

            if (InitDatabase(!allowMessageBox)) {
                StopProgressDelayCountDown();

                _resultsHelper.SetvApusInstance(Dns.GetHostName(), NamedObjectRegistrar.Get<string>("IP"), NamedObjectRegistrar.Get<int>("Port"),
                    NamedObjectRegistrar.Get<string>("vApusVersion") ?? string.Empty, NamedObjectRegistrar.Get<string>("vApusChannel") ?? string.Empty,
                    false);

                var scenarioKeys = new List<Scenario>(_stressTest.Scenarios.Length);
                foreach (var kvp in _stressTest.Scenarios)
                    scenarioKeys.Add(kvp.Key);

                await Task.Run(() => {
                    _resultsHelper.SetStressTest(_stressTest.ToString(), "None", _stressTest.Connection.ToString(), _stressTest.ConnectionProxy, _stressTest.Connection.ConnectionString,
                         scenarioKeys.Combine(", "), _stressTest.ScenarioRuleSet, _stressTest.Concurrencies, _stressTest.Runs, _stressTest.InitialMinimumDelay, _stressTest.InitialMaximumDelay, _stressTest.MinimumDelay,
                         _stressTest.MaximumDelay, _stressTest.Shuffle, _stressTest.ActionDistribution, _stressTest.MaximumNumberOfUserActions, _stressTest.MonitorBefore, _stressTest.MonitorAfter);
                });


                if (_stressTest.Monitors.Length == 0) {
                    SetGuiForStart(true);
                    Start();
                } else {
                    SetGuiForStart(false);
                    InitializeMonitors();
                }
            }
        }

        /// <summary>
        ///     True on success or if user said there can be proceed without database.
        /// </summary>
        /// <returns></returns>
        private bool InitDatabase(bool autoConfirmDialog) {
            var dialog = new DescriptionAndTagsInputDialog { Description = _stressTest.Description, Tags = _stressTest.Tags, ResultsHelper = _resultsHelper, AutoConfirm = autoConfirmDialog };
            if (dialog.ShowDialog() == DialogResult.Cancel) {
                RemoveDatabase(false);
                return false;
            }

            bool edited = false;
            if (_stressTest.Description != dialog.Description) {
                _stressTest.Description = dialog.Description;
                edited = true;
            }
            if (_stressTest.Tags.Combine(", ") != dialog.Tags.Combine(", ")) {
                _stressTest.Tags = dialog.Tags;
                edited = true;
            }

            if (edited) _stressTest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            return true;
        }

        private void SetGuiForStart(bool enableStop) {
            LockWindowUpdate(Handle);

            if (enableStop) btnStop.Enabled = true;
            btnStart.Enabled = btnSchedule.Enabled = false;
            btnSchedule.Text = string.Empty;

            fastResultsControl.SetStressTestInitialized();

            _stressTestResult = null;
            _stressTestMetricsCache = new FastStressTestMetricsCache();
            _monitorMetricsCache = new MonitorMetricsCache();
            detailedResultsControl.ClearResults();
            detailedResultsControl.Enabled = false;


            fastResultsControl.SetConfigurationControls(_stressTest);

            _monitorViews.Clear();

            solutionComponentPropertyPanel.Lock();

            tc.SelectedIndex = 1;

            _progressCountDown = PROGRESSUPDATEDELAY - 1;

            LockWindowUpdate(IntPtr.Zero);
        }

        /// <summary>
        ///     Start or schedule the test.
        /// </summary>
        private void Start() {
            if (btnSchedule.Tag != null && btnSchedule.Tag is DateTime && (DateTime)btnSchedule.Tag > DateTime.Now)
                ScheduleStressTest();
            else
                StartStressTest();
        }

        /// <summary>
        ///     Set up al the Gui stuff (enable and disable buttons, clear previous results, ...).
        ///     Initialize the test in this thread, start the test in another (otherwise waiting on the test finished blocks this thread)
        /// </summary>
        private void StartStressTest() {
            Cursor = Cursors.WaitCursor;

            try {
                LocalMonitor.StartMonitoring(PROGRESSUPDATEDELAY * 1000);
            } catch {
                fastResultsControl.AddEvent(
                    "Could not initialize the local monitor, something is wrong with your WMI.", Level.Error);
            }
            tmrProgress.Interval = PROGRESSUPDATEDELAY * 1000;
            tmrProgress.Start();
            try {
                _stressTestCore = new StressTestCore(_stressTest);
                _stressTestCore.ResultsHelper = _resultsHelper;
                _stressTestCore.StressTestStarted += _stressTestCore_StressTestStarted;
                _stressTestCore.ConcurrencyStarted += _stressTestCore_ConcurrentUsersStarted;
                _stressTestCore.ConcurrencyStopped += _stressTestCore_ConcurrencyStopped;
                _stressTestCore.RunInitializedFirstTime += _stressTestCore_RunInitializedFirstTime;
                _stressTestCore.RunStarted += _stressTestCore_RunStarted;
                _stressTestCore.RunStopped += _stressTestCore_RunStopped;
                _stressTestCore.Message += _stressTestCore_Message;

                _stressTestCore.TestInitialized += _stressTestCore_TestInitialized;
                ThreadPool.QueueUserWorkItem((state) => { _stressTestCore.InitializeTest(); }, null);


            } catch (Exception ex) {
                //Only one test can run at the same time.
                if (ex is ArgumentOutOfRangeException) {
                    fastResultsControl.AddEvent("Cannot start this test because another one is still running.", Level.Error);
                    Stop(StressTestStatus.Error, null);
                } else {
                    Stop(StressTestStatus.Error, ex);
                }
            }
        }

        private void _stressTestCore_TestInitialized(object sender, TestInitializedEventArgs e) {
            _stressTestCore.TestInitialized -= _stressTestCore_TestInitialized;
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                if (e.Exception == null) {
                    StartMonitors();
                } else {
                    //Only one test can run at the same time.
                    if (e.Exception is ArgumentOutOfRangeException) {
                        fastResultsControl.AddEvent("Cannot start this test because another one is still running.", Level.Error);
                        Stop(StressTestStatus.Error, null);
                    } else {
                        Stop(StressTestStatus.Error, e.Exception);
                    }
                }
                Cursor = Cursors.Default;
            }, null);
        }

        /// <summary>
        ///     Allows the stress test to start.
        /// </summary>
        private void MonitorBeforeDone() {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                Cursor = Cursors.WaitCursor;
                try {
                    //The stress test threadpool is blocking so we run this on another thread.
                    var stressTestThread = new Thread(() => {
                        var stressTestStatus = StressTestStatus.Busy;
                        Exception ex = null;
                        try {
                            stressTestStatus = _stressTestCore.ExecuteStressTest();
                        } catch (Exception e) {
                            stressTestStatus = StressTestStatus.Error;
                            ex = e;
                        } finally {
                            if (_stressTestCore != null && !_stressTestCore.IsDisposed) {
                                try {
                                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                                        Stop(stressTestStatus, ex, stressTestStatus == StressTestStatus.Ok && _stressTest.MonitorAfter != 0);
                                    }, null);
                                } catch {
                                    //Ignore. If the synchronization context is disposed / null. (Gui closed)
                                }
                            }
                        }
                    });

                    stressTestThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                    stressTestThread.IsBackground = true;
                    stressTestThread.Start();
                } catch (Exception ex) {
                    //Only one test can run at the same time.
                    if (ex is ArgumentOutOfRangeException) {
                        fastResultsControl.AddEvent(
                            "Cannot start this test because another one is still running.", Level.Error);
                        ex = null;
                    }
                    Stop(StressTestStatus.Error, ex);
                }
                Cursor = Cursors.Default;
            }, null);
        }

        /// <summary>
        ///     Will lock the Gui and start the stress test at the scheduled time.
        /// </summary>
        private void ScheduleStressTest() {
            btnStop.Enabled = true;
            btnStart.Enabled = false;
            btnSchedule.Enabled = false;
            solutionComponentPropertyPanel.Lock();
            tmrSchedule.Start();
        }

        private void tmrSchedule_Tick(object sender, EventArgs e) {
            var scheduledAt = (DateTime)btnSchedule.Tag;
            if (scheduledAt <= DateTime.Now) {
                btnSchedule.Text = string.Empty;
                btnSchedule.Tag = null;
                tmrSchedule.Stop();
                StartStressTest();
            } else {
                TimeSpan dt = scheduledAt - DateTime.Now;
                if (dt.Milliseconds != 0) {
                    dt = new TimeSpan(dt.Ticks - (dt.Ticks % TimeSpan.TicksPerSecond));
                    dt += new TimeSpan(0, 0, 1);
                }
                btnSchedule.Text = "Scheduled in " + dt.ToLongFormattedString();
            }
        }

        /// <summary>
        ///     Initialize the monitor (if chosen to monitor), if so the test can start.
        /// </summary>
        private void InitializeMonitors() {
            _monitorsInitialized = 0;
            foreach (MonitorView monitorView in _monitorViews)
                try {
                    monitorView.MonitorInitialized -= monitorView_MonitorInitialized;
                    monitorView.OnHandledException -= monitorView_OnHandledException;
                    monitorView.OnUnhandledException -= monitorView_OnUnhandledException;
                } catch {
                    //Should / can never happen. Ignore.
                }

            foreach (Monitor.Monitor monitor in _stressTest.Monitors) {
                var monitorView = SolutionComponentViewManager.Show(monitor) as MonitorView;
                Show();

                fastResultsControl.AddEvent("Initializing " + monitorView.Text + "...");
                _monitorViews.Add(monitorView);

                //Initialize
                monitorView.MonitorInitialized += monitorView_MonitorInitialized;
                monitorView.OnHandledException += monitorView_OnHandledException;
                monitorView.OnUnhandledException += monitorView_OnUnhandledException;
                monitorView.InitializeForStressTest();
            }
        }
        private void monitorView_MonitorInitialized(object sender, MonitorView.MonitorInitializedEventArgs e) {
            var monitorView = sender as MonitorView;
            monitorView.MonitorInitialized -= monitorView_MonitorInitialized;

            if (e.ErrorMessage != null && e.ErrorMessage.Length != 0) {
                fastResultsControl.AddEvent(e.ErrorMessage, Level.Warning);
                fastResultsControl.ExpandEventPanel();
            } else
                fastResultsControl.AddEvent(monitorView.Text + " is initialized.");

            if (++_monitorsInitialized == _stressTest.Monitors.Length) {
                btnStop.Enabled = true;
                Start();
            }
        }
        private void monitorView_OnHandledException(object sender, ErrorEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(
                (state) => {
                    //If the test is not yet started, break it if a monitor fails.
                    if (_monitorsInitialized < _stressTest.Monitors.Length) {
                        if (_stressTestCore != null) _stressTestCore.Cancel();
                        btnStop.Enabled = true;
                        Stop(StressTestStatus.Error, e.GetException());
                        Show();
                    } else {
                        fastResultsControl.AddEvent((sender as MonitorView).Text + ": A counter became unavailable while monitoring:\n" +
                            e.GetException(), Level.Warning);
                    }
                }, null);
        }
        private void monitorView_OnUnhandledException(object sender, ErrorEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(
                (state) => {
                    //If the test is not yet started, break it if a monitor fails.
                    if (_monitorsInitialized < _stressTest.Monitors.Length) {
                        if (_stressTestCore != null) _stressTestCore.Cancel();
                        btnStop.Enabled = true;
                        Stop(StressTestStatus.Error, e.GetException());
                        Show();
                    } else {
                        fastResultsControl.AddEvent((sender as MonitorView).Text + ": An error has occured while monitoring, monitor stopped!\n" +
                            e.GetException(), Level.Error);
                    }
                }, null);
        }

        /// <summary>
        ///     Used in stress test started eventhandling.
        /// </summary>
        private void StartMonitors() {
            if (_monitorViews != null && _stressTest.Monitors.Length != 0) {
                int runningMonitors = 0;
                foreach (MonitorView monitorView in _monitorViews)
                    if (monitorView != null && !monitorView.IsDisposed)
                        try {
                            if (monitorView.Start()) {
                                monitorView.GetMonitorResultCache().MonitorConfigurationId =
                                    _resultsHelper.SetMonitor(monitorView.Monitor.ToString(), monitorView.Monitor.MonitorSource.ToString(),
                                    monitorView.GetConnectionString(), monitorView.Configuration, monitorView.GetMonitorResultCache().Headers);

                                fastResultsControl.AddEvent(monitorView.Text + " is started.");
                                ++runningMonitors;
                            } else {
                                try { monitorView.Stop(); } catch { }
                            }
                        } catch (Exception e) {
                            try {
                                Loggers.Log(Level.Error, monitorView.Text + " is not started.", e);
                                fastResultsControl.AddEvent(monitorView.Text + " is not started.");

                                try { monitorView.Stop(); } catch { }
                            } catch {
                                //On gui closed. Ignore.
                            }
                        }

                if (runningMonitors != 0 && _stressTest.MonitorBefore != 0) {
                    int countdownTime = _stressTest.MonitorBefore * 60000;
                    _monitorBeforeCountDown = new Countdown(countdownTime, 5000);
                    _monitorBeforeCountDown.Tick += monitorBeforeCountDown_Tick;
                    _monitorBeforeCountDown.Stopped += monitorBeforeCountDown_Stopped;

                    _monitorBeforeBogusConcurrencyResult = new ConcurrencyResult(-1, 1);
                    _monitorBeforeBogusRunResult = new RunResult(-1, 0);
                    _monitorBeforeBogusConcurrencyResult.RunResults.Add(_monitorBeforeBogusRunResult);

                    _monitorAfterBogusConcurrencyResult = null;
                    _monitorAfterBogusRunResult = null;

                    try {
                        foreach (var monitorResultCache in GetMonitorResultCaches()) {
                            fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, _monitorMetricsCache.AddOrUpdate(_monitorBeforeBogusConcurrencyResult, monitorResultCache));
                            fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, _monitorMetricsCache.AddOrUpdate(_monitorBeforeBogusRunResult, monitorResultCache));
                        }
                    } catch {
                    }

                    fastResultsControl.ExpandEventPanel();
                    fastResultsControl.AddEvent("Monitoring before the test starts: " + (_stressTest.MonitorBefore * 60) + " s.");
                    _monitorBeforeCountDown.Start();
                } else {
                    MonitorBeforeDone();
                }
            } else {
                MonitorBeforeDone();
            }
        }
        private void monitorBeforeCountDown_Tick(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                if (_monitorBeforeBogusConcurrencyResult != null)
                    foreach (var monitorResultCache in GetMonitorResultCaches()) {
                        fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, _monitorMetricsCache.GetConcurrencyMetrics(monitorResultCache.Monitor));
                        fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, _monitorMetricsCache.GetRunMetrics(monitorResultCache.Monitor));
                    }

                int countdowntime = _monitorBeforeCountDown == null ? 0 : _monitorBeforeCountDown.CountdownTime;
                var ts = new TimeSpan(countdowntime * TimeSpan.TicksPerMillisecond);
                fastResultsControl.AddEvent("Monitoring before the test starts: " + ts.ToShortFormattedString("0 s") + ".");

                int runningMonitors = 0;
                if (_monitorViews != null && _stressTest.Monitors.Length != 0)
                    foreach (MonitorView view in _monitorViews)
                        if (view != null && !view.IsDisposed)
                            runningMonitors++;

                if (runningMonitors == 0) {
                    if (_monitorBeforeCountDown != null) _monitorBeforeCountDown.Stop();
                    fastResultsControl.AddEvent("All monitors were manually closed.");
                }
            }, null);
        }
        private void monitorBeforeCountDown_Stopped(object sender, EventArgs e) {
            if (_monitorBeforeCountDown != null) {
                _monitorBeforeCountDown.Dispose();
                _monitorBeforeCountDown = null;
            }
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                if (_monitorBeforeBogusConcurrencyResult != null) {
                    var stoppedAt = DateTime.Now;
                    TimeSpan difference = stoppedAt - _monitorBeforeBogusConcurrencyResult.StartedAt;
                    _monitorBeforeBogusConcurrencyResult.StoppedAt = stoppedAt.Subtract(new TimeSpan(difference.Milliseconds * TimeSpan.TicksPerMillisecond));

                    difference = stoppedAt - _monitorBeforeBogusRunResult.StartedAt;
                    _monitorBeforeBogusRunResult.StoppedAt = stoppedAt.Subtract(new TimeSpan(difference.Milliseconds * TimeSpan.TicksPerMillisecond));

                    foreach (var monitorResultCache in GetMonitorResultCaches()) {
                        fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, _monitorMetricsCache.GetConcurrencyMetrics(monitorResultCache.Monitor));
                        fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, _monitorMetricsCache.GetRunMetrics(monitorResultCache.Monitor));
                    }
                }
            }, null);

            MonitorBeforeDone();
        }
        #endregion

        #region Progress
        private void _stressTestCore_StressTestStarted(object sender, StressTestResultEventArgs e) {
            _simplifiedMetricsReturned = false;
            _stressTestResult = e.StressTestResult;
            fastResultsControl.SetStressTestStarted(e.StressTestResult.StartedAt);
        }

        private void _stressTestCore_ConcurrentUsersStarted(object sender, ConcurrencyResultEventArgs e) {
            _progressCountDown = PROGRESSUPDATEDELAY;
            StopProgressDelayCountDown();

            //Purge the previous concurrent user results from memory, we don't need it anymore.
            foreach (var concurrencyResult in _stressTestResult.ConcurrencyResults)
                if (concurrencyResult.StoppedAt != DateTime.MinValue) {
                    _stressTestResult.ConcurrencyResults.Remove(concurrencyResult);
                    break;
                }

            //Update the metrics.
            fastResultsControl.UpdateFastConcurrencyResults(_stressTestMetricsCache.AddOrUpdate(e.Result), true, _stressTestMetricsCache.SimplifiedMetrics);
            foreach (var monitorResultCache in GetMonitorResultCaches())
                fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, _monitorMetricsCache.AddOrUpdate(e.Result, monitorResultCache));
        }
        private void _stressTestCore_ConcurrencyStopped(object sender, ConcurrencyResultEventArgs e) {
            string message = string.Concat(_stressTest.ToString(), " - Concurrency ", e.Result.Concurrency, " finished.");
            TestProgressNotifier.Notify(TestProgressNotifier.What.ConcurrencyFinished, message);

            //#warning Enable REST
            //WriteRestProgress(RunStateChange.None);
        }
        private void _stressTestCore_RunInitializedFirstTime(object sender, RunResultEventArgs e) {
            StopProgressDelayCountDown();

            fastResultsControl.UpdateFastRunResults(_stressTestMetricsCache.AddOrUpdate(e.Result), true, _stressTestMetricsCache.SimplifiedMetrics);
            fastResultsControl.UpdateFastConcurrencyResults(_stressTestMetricsCache.GetConcurrencyMetrics(), false, _stressTestMetricsCache.SimplifiedMetrics);
            foreach (var monitorResultCache in GetMonitorResultCaches()) {
                fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, _monitorMetricsCache.AddOrUpdate(e.Result, monitorResultCache));
                fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, _monitorMetricsCache.GetConcurrencyMetrics(monitorResultCache.Monitor));
            }

            //#warning Enable REST
            //WriteRestProgress(RunStateChange.ToRunInitializedFirstTime);

            tmrProgress.Stop();
            _progressCountDown = PROGRESSUPDATEDELAY;
            fastResultsControl.SetCountDownProgressDelay(_progressCountDown--);
            tmrProgressDelayCountDown.Start();
            tmrProgress.Start();
        }
        private void _stressTestCore_RunStarted(object sender, RunResultEventArgs e) { _canUpdateMetrics = true; }
        private void _stressTestCore_RunStopped(object sender, RunResultEventArgs e) {
            _canUpdateMetrics = false;
            int concurrency = _stressTestResult.ConcurrencyResults[_stressTestResult.ConcurrencyResults.Count - 1].Concurrency;
            string message = string.Concat(_stressTest.ToString(), " - Run ", e.Result.Run, " of concurrency ", concurrency, " finished.");
            TestProgressNotifier.Notify(TestProgressNotifier.What.RunFinished, message);

            //#warning Enable REST
            //WriteRestProgress(RunStateChange.None);
        }

        private void tmrProgressDelayCountDown_Tick(object sender, EventArgs e) { fastResultsControl.SetCountDownProgressDelay(_progressCountDown--); }

        private void tmrProgress_Tick(object sender, EventArgs e) {
            if (_stressTestCore != null) {
                try {
                    fastResultsControl.SetClientMonitoring(_stressTestCore.BusyThreadCount, LocalMonitor.CPUUsage,
                                                          (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.Nic, LocalMonitor.NicBandwidth, LocalMonitor.NicSent, LocalMonitor.NicReceived);
                } catch { } //Exception on false WMI. 

                if (_canUpdateMetrics) {
                    fastResultsControl.UpdateFastConcurrencyResults(_stressTestMetricsCache.GetConcurrencyMetrics(), true, _stressTestMetricsCache.SimplifiedMetrics);
                    fastResultsControl.UpdateFastRunResults(_stressTestMetricsCache.GetRunMetrics(), false, _stressTestMetricsCache.SimplifiedMetrics);
                    foreach (var monitorResultCache in GetMonitorResultCaches()) {
                        fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, _monitorMetricsCache.GetConcurrencyMetrics(monitorResultCache.Monitor));
                        fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, _monitorMetricsCache.GetRunMetrics(monitorResultCache.Monitor));
                    }

                    if (_stressTestMetricsCache.SimplifiedMetrics && !_simplifiedMetricsReturned) {
                        _simplifiedMetricsReturned = true;
                        fastResultsControl.AddEvent("It takes too long to calculate the fast results, therefore they are simplified!", Level.Warning);
                    }
                }
                _progressCountDown = PROGRESSUPDATEDELAY;

                ////#warning Enable REST
                //WriteRestProgress(RunStateChange.None);
            }
        }

        private void _stressTestCore_Message(object sender, MessageEventArgs e) {
            if (e.Color == Color.Empty) fastResultsControl.AddEvent(e.Message, e.LogLevel); else fastResultsControl.AddEvent(e.Message, e.Color, e.LogLevel);
        }
        #endregion

        #region Stop
        private void StressTestView_FormClosing(object sender, FormClosingEventArgs e) {
            if (btnStart.Enabled || _stressTestCore == null || _stressTestCore.IsDisposed ||
                MessageBox.Show("Are you sure you want to close a running test?", string.Empty, MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes) {
                StopMonitorsAndUnlockGui(null, true);
                Stop_StressTest();

                tmrProgress.Stop();
                tmrProgressDelayCountDown.Stop();
                tmrSchedule.Stop();

                if (_stressTestCore != null && !_stressTestCore.IsDisposed) {
                    _stressTestCore.Dispose();
                    _stressTestCore = null;
                }
            } else {
                Solution.ExplicitCancelFormClosing = true;
                e.Cancel = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e) {
            StopStressTest();
        }
        public void StopStressTest() {
            if (!btnStop.Enabled) return;
            toolStrip.Enabled = false;
            if (_stressTestCore == null || (_monitorAfterCountDown != null && _monitorAfterCountDown.CountdownTime != 0)) {
                Stop(StressTestStatus.Cancelled);
            } else if (_monitorBeforeCountDown != null && _monitorBeforeCountDown.CountdownTime != 0) {
                _stressTestCore.Cancel();
                Stop(StressTestStatus.Cancelled);
            } else {
                // Can only be cancelled once, calling multiple times is not a problem.
                _stressTestCore.Cancel();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="ex">The exception if failed.</param>
        private void Stop(StressTestStatus stressTestStatus = StressTestStatus.Ok, Exception ex = null, bool monitorAfter = false) {
            _stressTestStatus = stressTestStatus;
            if (btnStop.Enabled || !toolStrip.Enabled) {
                Cursor = Cursors.WaitCursor;

                Stop_StressTest();

                tmrProgress.Stop();

                if (_stressTestCore != null && !_stressTestCore.IsDisposed) {
                    _stressTestCore.Dispose();
                    _stressTestCore = null;
                }

                StopProgressDelayCountDown();


                tmrSchedule.Stop();
                btnSchedule.Text = string.Empty;
                btnSchedule.Tag = null;


                fastResultsControl.SetStressTestStopped(stressTestStatus);

                toolStrip.Enabled = true;

                Cursor = Cursors.Default;

                //#warning Enable REST
                // WriteRestProgress(RunStateChange.None);

                int runningMonitors = 0;
                if (_monitorViews != null && _stressTest.Monitors.Length != 0)
                    foreach (MonitorView view in _monitorViews)
                        if (view != null && !view.IsDisposed)
                            runningMonitors++;

                if (monitorAfter && _stressTest.MonitorAfter != 0 && runningMonitors != 0 && stressTestStatus != StressTestStatus.Cancelled && stressTestStatus != StressTestStatus.Error) {
                    int countdownTime = _stressTest.MonitorAfter * 60000;
                    _monitorAfterCountDown = new Countdown(countdownTime, 5000);
                    _monitorAfterCountDown.Tick += monitorAfterCountdown_Tick;
                    _monitorAfterCountDown.Stopped += monitorAfterCountdown_Stopped;

                    _monitorAfterBogusConcurrencyResult = new ConcurrencyResult(-1, 1);
                    _monitorAfterBogusRunResult = new RunResult(-1, 0);
                    _monitorAfterBogusConcurrencyResult.RunResults.Add(_monitorAfterBogusRunResult);

                    try {
                        foreach (var monitorResultCache in GetMonitorResultCaches()) {
                            fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, _monitorMetricsCache.AddOrUpdate(_monitorAfterBogusConcurrencyResult, monitorResultCache));
                            fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, _monitorMetricsCache.AddOrUpdate(_monitorAfterBogusRunResult, monitorResultCache));
                        }
                    } catch {
                        Loggers.Log(Level.Error, "Failed updating fast monitor results.", ex, new object[] { stressTestStatus, ex, monitorAfter });
                    }

                    fastResultsControl.ExpandEventPanel();
                    fastResultsControl.AddEvent("Monitoring after the test is finished: " + (_stressTest.MonitorAfter * 60) + " s.");
                    _monitorAfterCountDown.Start();
                } else {
                    StopMonitorsAndUnlockGui(ex, false);
                }
                this.Focus();

                if (stressTestStatus == StressTestStatus.Cancelled || stressTestStatus == StressTestStatus.Error)
                    RemoveDatabase();
            }
        }
        private void RemoveDatabase(bool confirm = true) {
            if (_resultsHelper != null && _resultsHelper.DatabaseName != null)
                if (!confirm || MessageBox.Show("Do you want to remove the results database?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
                    try { _resultsHelper.DeleteResults(); } catch (Exception ex) {
                        Loggers.Log(Level.Warning, "Failed deleting results.", ex, new object[] { confirm });
                    }
                    detailedResultsControl.ClearResults();
                    detailedResultsControl.Enabled = false;
                }
        }
        private void monitorAfterCountdown_Tick(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                if (_monitorAfterBogusConcurrencyResult != null)
                    foreach (var monitorResultCache in GetMonitorResultCaches()) {
                        fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, _monitorMetricsCache.GetConcurrencyMetrics(monitorResultCache.Monitor));
                        fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, _monitorMetricsCache.GetRunMetrics(monitorResultCache.Monitor));
                    }

                var ts = new TimeSpan(_monitorAfterCountDown.CountdownTime * TimeSpan.TicksPerMillisecond);
                fastResultsControl.AddEvent("Monitoring after the test is finished: " + ts.ToShortFormattedString("0 s") + ".");

                int runningMonitors = 0;
                if (_monitorViews != null)
                    foreach (MonitorView view in _monitorViews)
                        if (view != null && !view.IsDisposed)
                            runningMonitors++;

                if (runningMonitors == 0) {
                    _monitorAfterCountDown.Stop();
                    fastResultsControl.AddEvent("All monitors were manually closed.");
                }
            }, null);
        }

        private void monitorAfterCountdown_Stopped(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                StopMonitorsAndUnlockGui(null, false);

                this.Focus();

                if (_monitorAfterBogusConcurrencyResult != null) {
                    var stoppedAt = DateTime.Now;
                    var difference = stoppedAt - _monitorAfterBogusConcurrencyResult.StartedAt;
                    _monitorAfterBogusConcurrencyResult.StoppedAt = stoppedAt.Subtract(new TimeSpan((long)(difference.Milliseconds * TimeSpan.TicksPerMillisecond)));

                    difference = stoppedAt - _monitorAfterBogusRunResult.StartedAt;
                    _monitorAfterBogusRunResult.StoppedAt = stoppedAt.Subtract(new TimeSpan((long)(difference.Milliseconds * TimeSpan.TicksPerMillisecond)));

                    foreach (var monitorResultCache in GetMonitorResultCaches()) {
                        fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, _monitorMetricsCache.GetConcurrencyMetrics(monitorResultCache.Monitor));
                        fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, _monitorMetricsCache.GetRunMetrics(monitorResultCache.Monitor));
                    }

                    fastResultsControl.AddEvent("Finished.");
                }
            }, null);
        }

        /// <summary>
        ///     Only used in stop
        /// </summary>
        private void Stop_StressTest() {
            btnSchedule.Tag = null;
            btnSchedule.Text = string.Empty;
            tmrSchedule.Stop();

            if (_stressTestCore != null && !_stressTestCore.IsDisposed) {
                try {
                    fastResultsControl.SetClientMonitoring(_stressTestCore.BusyThreadCount, LocalMonitor.CPUUsage, (int)LocalMonitor.MemoryUsage,
                                                          (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.Nic, LocalMonitor.NicBandwidth, LocalMonitor.NicSent, LocalMonitor.NicReceived);
                } catch { } //Exception on false WMI. 

                _stressTestMetricsCache.AllowSimplifiedMetrics = false;
                fastResultsControl.UpdateFastConcurrencyResults(_stressTestMetricsCache.GetConcurrencyMetrics(), true, _stressTestMetricsCache.SimplifiedMetrics);
                fastResultsControl.UpdateFastRunResults(_stressTestMetricsCache.GetRunMetrics(), false, _stressTestMetricsCache.SimplifiedMetrics);
                foreach (var monitorResultCache in GetMonitorResultCaches()) {
                    fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, _monitorMetricsCache.GetConcurrencyMetrics(monitorResultCache.Monitor));
                    fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, _monitorMetricsCache.GetRunMetrics(monitorResultCache.Monitor));
                }

                // Can only be cancelled once, calling multiple times is not a problem.
                if (_stressTestCore != null && !_stressTestCore.IsDisposed) try { _stressTestCore.Cancel(); } catch {
                        //Ignore. Should / can never happen.
                    }
            }

            fastResultsControl.SetStressTestStopped();
            _stressTestResult = null;
            _canUpdateMetrics = false;
        }

        /// <summary>
        ///     Only used in stop, stops the monitors if any, saves the results; Unlocks the gui so changes can be made to the stress test.
        /// </summary>
        private void StopMonitorsAndUnlockGui(Exception exception, bool disposing) {
            if (_monitorBeforeCountDown != null) {
                try { _monitorBeforeCountDown.Dispose(); } catch {
                    //Ignore.
                }
                _monitorBeforeCountDown = null;
            }
            if (_monitorAfterCountDown != null) {
                try { _monitorAfterCountDown.Dispose(); } catch {
                    //Ignore.
                }
                _monitorAfterCountDown = null;
            }

            var validMonitorViews = new List<MonitorView>();
            if (_monitorViews != null && _stressTest.Monitors.Length != 0)
                foreach (MonitorView view in _monitorViews)
                    if (view != null && !view.IsDisposed && view.IsRunning) {
                        view.Stop();
                        fastResultsControl.AddEvent(view.Text + " is stopped.");
                        validMonitorViews.Add(view);
                    }
            foreach (MonitorView view in validMonitorViews)
                try { _resultsHelper.SetMonitorResults(view.GetMonitorResultCache()); } catch (Exception e) {
                    Loggers.Log(Level.Error, view.Text + ": Failed adding results to the database.", e);
                    _stressTestStatus = StressTestStatus.Error;
                }

            validMonitorViews = null;

            if (!disposing) {
                solutionComponentPropertyPanel.Unlock();
                btnStop.Enabled = false;
                btnStart.Enabled = true;
                btnSchedule.Enabled = true;

                if (_resultsHelper.DatabaseName == null) {
                    detailedResultsControl.ClearResults();
                    detailedResultsControl.Enabled = false;
                } else {
                    this.Enabled = false;
                    detailedResultsControl.RefreshResults(_resultsHelper);
                    this.Enabled = true;

                    if (AutoExportResultsManager.Enabled && _stressTestStatus == StressTestStatus.Ok)
                        detailedResultsControl.AutoExportToExcel(AutoExportResultsManager.Folder);
                }

                if (exception == null) {
                    TestProgressNotifier.Notify(TestProgressNotifier.What.TestFinished, string.Concat(_stressTest.ToString(), " finished. Status: ", _stressTestStatus, "."));
                } else {
                    //Loggers.Log(Level.Error, _stress test.ToString() + " Failed.", exception);
                    TestProgressNotifier.Notify(TestProgressNotifier.What.TestFinished, string.Concat(_stressTest.ToString(), " finished. Status: ", _stressTestStatus, "."), exception);
                    fastResultsControl.AddEvent(exception.ToString(), Level.Error);
                }
            }
        }

        /// <summary>
        /// Sets the updates in label.
        /// </summary>
        private void StopProgressDelayCountDown() {
            try {
                tmrProgressDelayCountDown.Stop();
                if (fastResultsControl != null && !fastResultsControl.IsDisposed)
                    fastResultsControl.SetCountDownProgressDelay(-1);
            } catch {
                //Ignore.
            }
        }
        #endregion

        //private void WriteRestProgress(RunStateChange runStateChange) {
        //    try {
        //        var testProgressCache = new JSONObjectTree();
        //        var clientMonitorCache = new JSONObjectTree();
        //        var messagesCache = new JSONObjectTree();

        //        if (_stress testCore != null && !_stressTestCore.IsDisposed) {
        //            var stressTestCache = JSONObjectTreeHelper.AddSubCache(_stressTest.ToString(), testProgressCache);

        //            foreach (var metrics in _stressTestMetricsCache.GetConcurrencyMetrics())
        //                JSONObjectTreeHelper.ApplyToRunningTestFastConcurrencyResults(stressTestCache, metrics, runStateChange.ToString(), _stressTestStatus.ToString());

        //            JSONObjectTreeHelper.ApplyToRunningTestClientMonitorMetrics(clientMonitorCache, _stressTest.ToString(), _stressTestCore.BusyThreadCount, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond,
        //                                                  LocalMonitor.MemoryUsage, LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);

        //            var events = fastResultsControl.GetEvents();
        //            var messages = new string[events.Count];
        //            for (int i = 0; i != messages.Length; i++) {
        //                var e = events[i];
        //                messages[i] = e.EventType + ": " + e.Message + " [" + e.At + "]";
        //            }
        //            JSONObjectTreeHelper.ApplyToRunningTestMessages(messagesCache, _stressTest.ToString(), messages);

        //            JSONObjectTreeHelper.RunningTestFastConcurrencyResults = testProgressCache;
        //            JSONObjectTreeHelper.RunningTestClientMonitorMetrics = clientMonitorCache;
        //            JSONObjectTreeHelper.RunningTestMessages = messagesCache;
        //        }
        //    } catch {
        //    }
        //}

        #endregion
    }
}