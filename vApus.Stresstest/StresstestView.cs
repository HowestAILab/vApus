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

namespace vApus.Stresstest {
    public partial class StresstestView : BaseSolutionComponentView {

        #region Fields
        private Stresstest _stresstest;

        private ScheduleDialog _scheduleDialog;

        /// <summary>
        ///     In seconds how fast the stresstest progress will be updated.
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
        ///     Caching the results to visualize in the stresstestcontrol.
        /// </summary>
        private FastStresstestMetricsCache _stresstestMetricsCache;
        private StresstestCore _stresstestCore;
        private StresstestResult _stresstestResult;
        private StresstestStatus _stresstestStatus; //Set on calling Stop(...);

        private MonitorMetricsCache _monitorMetricsCache;
        private readonly List<MonitorView> _monitorViews = new List<MonitorView>();
        private Countdown _monitorBeforeCountDown, _monitorAfterCountDown;
        private int _monitorsInitialized;
        private ConcurrencyResult _monitorBeforeBogusConcurrencyResult, _monitorAfterBogusConcurrencyResult;
        private RunResult _monitorBeforeBogusRunResult, _monitorAfterBogusRunResult;
        #endregion

        #region Properties
        public StresstestStatus StresstestStatus {
            get { return _stresstestStatus; }
        }
        #endregion

        #region Constructor
        /// <summary>
        ///     Designer time constructor.
        /// </summary>
        public StresstestView() {
            InitializeComponent();
        }
        public StresstestView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            Solution.RegisterForCancelFormClosing(this);
            _stresstest = SolutionComponent as Stresstest;

            InitializeComponent();

            if (IsHandleCreated)
                SetGui();
            else
                HandleCreated += StresstestProjectView_HandleCreated;
        }
        #endregion

        #region Functions

        #region Set the Gui
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        private void StresstestProjectView_HandleCreated(object sender, EventArgs e) {
            SetGui();
        }
        private void SetGui() {
            Text = SolutionComponent.ToString();
            solutionComponentPropertyPanel.SolutionComponent = SolutionComponent;

            btnStart.Enabled = !btnStop.Enabled;

            if (_stresstest.Connection.IsEmpty || _stresstest.Connection.ConnectionProxy.IsEmpty ||
                _stresstest.Logs.Length == 0 || _stresstest.Logs[0].Key.LogRuleSet.IsEmpty)
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
            StartStresstest(true);
        }
        async public void StartStresstest(bool allowMessageBox) {
            if (fastResultsControl.HasResults && (!allowMessageBox ||
                MessageBox.Show("Starting the test will clear the previous results.\nDo you want to continue?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                ) return;

            if (InitDatabase(!allowMessageBox)) {
                StopProgressDelayCountDown();

                _resultsHelper.SetvApusInstance(Dns.GetHostName(), NamedObjectRegistrar.Get<string>("IP"), NamedObjectRegistrar.Get<int>("Port"),
                    NamedObjectRegistrar.Get<string>("vApusVersion") ?? string.Empty, NamedObjectRegistrar.Get<string>("vApusChannel") ?? string.Empty,
                    false);

                var logKeys = new List<Log>(_stresstest.Logs.Length);
                foreach (var kvp in _stresstest.Logs)
                    logKeys.Add(kvp.Key);

                await Task.Run(() => {
                    _resultsHelper.SetStresstest(_stresstest.ToString(), "None", _stresstest.Connection.ToString(), _stresstest.ConnectionProxy, _stresstest.Connection.ConnectionString,
                         logKeys.Combine(", "), _stresstest.LogRuleSet, _stresstest.Concurrencies, _stresstest.Runs, _stresstest.MinimumDelay,
                         _stresstest.MaximumDelay, _stresstest.Shuffle, _stresstest.ActionDistribution, _stresstest.MaximumNumberOfUserActions, _stresstest.MonitorBefore, _stresstest.MonitorAfter);
                });


                if (_stresstest.Monitors.Length == 0) {
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
            var dialog = new DescriptionAndTagsInputDialog { Description = _stresstest.Description, Tags = _stresstest.Tags, ResultsHelper = _resultsHelper, AutoConfirm = autoConfirmDialog };
            if (dialog.ShowDialog() == DialogResult.Cancel) {
                RemoveDatabase(false);
                return false;
            }

            bool edited = false;
            if (_stresstest.Description != dialog.Description) {
                _stresstest.Description = dialog.Description;
                edited = true;
            }
            if (_stresstest.Tags.Combine(", ") != dialog.Tags.Combine(", ")) {
                _stresstest.Tags = dialog.Tags;
                edited = true;
            }

            if (edited) _stresstest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            return true;
        }

        private void SetGuiForStart(bool enableStop) {
            LockWindowUpdate(Handle);

            if (enableStop) btnStop.Enabled = true;
            btnStart.Enabled = btnSchedule.Enabled = false;
            btnSchedule.Text = string.Empty;

            fastResultsControl.SetStresstestInitialized();

            _stresstestResult = null;
            _stresstestMetricsCache = new FastStresstestMetricsCache();
            _monitorMetricsCache = new MonitorMetricsCache();
            detailedResultsControl.ClearResults();
            detailedResultsControl.Enabled = false;


            fastResultsControl.SetConfigurationControls(_stresstest);

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
                ScheduleStresstest();
            else
                StartStresstest();
        }

        /// <summary>
        ///     Set up al the Gui stuff (enable and disable buttons, clear previous results, ...).
        ///     Initialize the test in this thread, start the test in another (otherwise waiting on the test finished blocks this thread)
        /// </summary>
        private void StartStresstest() {
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
                _stresstestCore = new StresstestCore(_stresstest);
                _stresstestCore.ResultsHelper = _resultsHelper;
                _stresstestCore.StresstestStarted += _stresstestCore_StresstestStarted;
                _stresstestCore.ConcurrencyStarted += _stresstestCore_ConcurrentUsersStarted;
                _stresstestCore.ConcurrencyStopped += _stresstestCore_ConcurrencyStopped;
                _stresstestCore.RunInitializedFirstTime += _stresstestCore_RunInitializedFirstTime;
                _stresstestCore.RunStarted += _stresstestCore_RunStarted;
                _stresstestCore.RunStopped += _stresstestCore_RunStopped;
                _stresstestCore.Message += _stresstestCore_Message;

                _stresstestCore.TestInitialized += _stresstestCore_TestInitialized;
                ThreadPool.QueueUserWorkItem((state) => { _stresstestCore.InitializeTest(); }, null);


            } catch (Exception ex) {
                //Only one test can run at the same time.
                if (ex is ArgumentOutOfRangeException) {
                    fastResultsControl.AddEvent("Cannot start this test because another one is still running.", Level.Error);
                    Stop(StresstestStatus.Error, null);
                } else {
                    Stop(StresstestStatus.Error, ex);
                }
            }
        }

        private void _stresstestCore_TestInitialized(object sender, TestInitializedEventArgs e) {
            _stresstestCore.TestInitialized -= _stresstestCore_TestInitialized;
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                if (e.Exception == null) {
                    StartMonitors();
                } else {
                    //Only one test can run at the same time.
                    if (e.Exception is ArgumentOutOfRangeException) {
                        fastResultsControl.AddEvent("Cannot start this test because another one is still running.", Level.Error);
                        Stop(StresstestStatus.Error, null);
                    } else {
                        Stop(StresstestStatus.Error, e.Exception);
                    }
                }
                Cursor = Cursors.Default;
            }, null);
        }

        /// <summary>
        ///     Allows the stresstest to start.
        /// </summary>
        private void MonitorBeforeDone() {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                Cursor = Cursors.WaitCursor;
                try {
                    //The stresstest threadpool is blocking so we run this on another thread.
                    var stresstestThread = new Thread(() => {
                        var stresstestStatus = StresstestStatus.Busy;
                        Exception ex = null;
                        try {
                            stresstestStatus = _stresstestCore.ExecuteStresstest();
                        } catch (Exception e) {
                            stresstestStatus = StresstestStatus.Error;
                            ex = e;
                        } finally {
                            if (_stresstestCore != null && !_stresstestCore.IsDisposed) {
                                try {
                                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                                        Stop(stresstestStatus, ex, stresstestStatus == StresstestStatus.Ok && _stresstest.MonitorAfter != 0);
                                    }, null);
                                } catch {
                                    //Ignore. If the synchronization context is disposed / null. (Gui closed)
                                }
                            }
                        }
                    });

                    stresstestThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                    stresstestThread.IsBackground = true;
                    stresstestThread.Start();
                } catch (Exception ex) {
                    //Only one test can run at the same time.
                    if (ex is ArgumentOutOfRangeException) {
                        fastResultsControl.AddEvent(
                            "Cannot start this test because another one is still running.", Level.Error);
                        ex = null;
                    }
                    Stop(StresstestStatus.Error, ex);
                }
                Cursor = Cursors.Default;
            }, null);
        }

        /// <summary>
        ///     Will lock the Gui and start the stresstest at the scheduled time.
        /// </summary>
        private void ScheduleStresstest() {
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
                StartStresstest();
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

            foreach (Monitor.Monitor monitor in _stresstest.Monitors) {
                var monitorView = SolutionComponentViewManager.Show(monitor) as MonitorView;
                Show();

                fastResultsControl.AddEvent("Initializing " + monitorView.Text + "...");
                _monitorViews.Add(monitorView);

                //Initialize
                monitorView.MonitorInitialized += monitorView_MonitorInitialized;
                monitorView.OnHandledException += monitorView_OnHandledException;
                monitorView.OnUnhandledException += monitorView_OnUnhandledException;
                monitorView.InitializeForStresstest();
            }
        }
        private void monitorView_MonitorInitialized(object sender, MonitorView.MonitorInitializedEventArgs e) {
            var monitorView = sender as MonitorView;
            monitorView.MonitorInitialized -= monitorView_MonitorInitialized;

            if (e.ErrorMessage != null && e.ErrorMessage.Length != 0) Stop(StresstestStatus.Error, new Exception(e.ErrorMessage));
            else {
                fastResultsControl.AddEvent(monitorView.Text + " is initialized.");
                if (++_monitorsInitialized == _stresstest.Monitors.Length) {
                    btnStop.Enabled = true;
                    Start();
                }
            }
        }
        private void monitorView_OnHandledException(object sender, ErrorEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(
                (state) => {
                    //If the test is not yet started, break it if a monitor fails.
                    if (_monitorsInitialized < _stresstest.Monitors.Length) {
                        if (_stresstestCore != null) _stresstestCore.Cancel();
                        btnStop.Enabled = true;
                        Stop(StresstestStatus.Error, e.GetException());
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
                    if (_monitorsInitialized < _stresstest.Monitors.Length) {
                        if (_stresstestCore != null) _stresstestCore.Cancel();
                        btnStop.Enabled = true;
                        Stop(StresstestStatus.Error, e.GetException());
                        Show();
                    } else {
                        fastResultsControl.AddEvent((sender as MonitorView).Text + ": An error has occured while monitoring, monitor stopped!\n" +
                            e.GetException(), Level.Error);
                    }
                }, null);
        }

        /// <summary>
        ///     Used in stresstest started eventhandling.
        /// </summary>
        private void StartMonitors() {
            if (_monitorViews != null && _stresstest.Monitors.Length != 0) {
                int runningMonitors = 0;
                foreach (MonitorView monitorView in _monitorViews)
                    if (monitorView != null && !monitorView.IsDisposed)
                        try {
                            monitorView.Start();

                            monitorView.GetMonitorResultCache().MonitorConfigurationId =
                                _resultsHelper.SetMonitor(monitorView.Monitor.ToString(), monitorView.Monitor.MonitorSource.ToString(),
                                monitorView.GetConnectionString(), monitorView.Configuration, monitorView.GetMonitorResultCache().Headers);

                            fastResultsControl.AddEvent(monitorView.Text + " is started.");
                            ++runningMonitors;
                        } catch (Exception e) {
                            try {
                                Loggers.Log(Level.Error, monitorView.Text + " is not started.", e);
                                fastResultsControl.AddEvent(monitorView.Text + " is not started.");

                                try { monitorView.Stop(); } catch { }
                            } catch {
                                //On gui closed. Ignore.
                            }
                        }

                if (runningMonitors != 0 && _stresstest.MonitorBefore != 0) {
                    int countdownTime = _stresstest.MonitorBefore * 60000;
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
                    fastResultsControl.AddEvent("Monitoring before the test starts: " + (_stresstest.MonitorBefore * 60) + " s.");
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
                fastResultsControl.AddEvent("Monitoring before the test starts: " + ts.ToShortFormattedString() + ".");

                int runningMonitors = 0;
                if (_monitorViews != null && _stresstest.Monitors.Length != 0)
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
        private void _stresstestCore_StresstestStarted(object sender, StresstestResultEventArgs e) {
            _simplifiedMetricsReturned = false;
            _stresstestResult = e.StresstestResult;
            fastResultsControl.SetStresstestStarted(e.StresstestResult.StartedAt);
        }

        private void _stresstestCore_ConcurrentUsersStarted(object sender, ConcurrencyResultEventArgs e) {
            _progressCountDown = PROGRESSUPDATEDELAY;
            StopProgressDelayCountDown();

            //Purge the previous concurrent user results from memory, we don't need it anymore.
            foreach (var concurrencyResult in _stresstestResult.ConcurrencyResults)
                if (concurrencyResult.StoppedAt != DateTime.MinValue) {
                    _stresstestResult.ConcurrencyResults.Remove(concurrencyResult);
                    break;
                }

            //Update the metrics.
            fastResultsControl.UpdateFastConcurrencyResults(_stresstestMetricsCache.AddOrUpdate(e.Result), true, _stresstestMetricsCache.SimplifiedMetrics);
            foreach (var monitorResultCache in GetMonitorResultCaches())
                fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, _monitorMetricsCache.AddOrUpdate(e.Result, monitorResultCache));
        }
        private void _stresstestCore_ConcurrencyStopped(object sender, ConcurrencyResultEventArgs e) {
            string message = string.Concat(_stresstest.ToString(), " - Concurrency ", e.Result.Concurrency, " finished.");
            TestProgressNotifier.Notify(TestProgressNotifier.What.ConcurrencyFinished, message);

            //#warning Enable REST
            //WriteRestProgress(RunStateChange.None);
        }
        private void _stresstestCore_RunInitializedFirstTime(object sender, RunResultEventArgs e) {
            StopProgressDelayCountDown();

            fastResultsControl.UpdateFastRunResults(_stresstestMetricsCache.AddOrUpdate(e.Result), true, _stresstestMetricsCache.SimplifiedMetrics);
            fastResultsControl.UpdateFastConcurrencyResults(_stresstestMetricsCache.GetConcurrencyMetrics(), false, _stresstestMetricsCache.SimplifiedMetrics);
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
        private void _stresstestCore_RunStarted(object sender, RunResultEventArgs e) { _canUpdateMetrics = true; }
        private void _stresstestCore_RunStopped(object sender, RunResultEventArgs e) {
            _canUpdateMetrics = false;
            int concurrency = _stresstestResult.ConcurrencyResults[_stresstestResult.ConcurrencyResults.Count - 1].Concurrency;
            string message = string.Concat(_stresstest.ToString(), " - Run ", e.Result.Run, " of concurrency ", concurrency, " finished.");
            TestProgressNotifier.Notify(TestProgressNotifier.What.RunFinished, message);

            //#warning Enable REST
            //WriteRestProgress(RunStateChange.None);
        }

        private void tmrProgressDelayCountDown_Tick(object sender, EventArgs e) { fastResultsControl.SetCountDownProgressDelay(_progressCountDown--); }

        private void tmrProgress_Tick(object sender, EventArgs e) {
            if (_stresstestCore != null) {
                try {
                    fastResultsControl.SetClientMonitoring(_stresstestCore.BusyThreadCount, LocalMonitor.CPUUsage,
                                                          (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.Nic, LocalMonitor.NicBandwidth, LocalMonitor.NicSent, LocalMonitor.NicReceived);
                } catch { } //Exception on false WMI. 

                if (_canUpdateMetrics) {
                    fastResultsControl.UpdateFastConcurrencyResults(_stresstestMetricsCache.GetConcurrencyMetrics(), true, _stresstestMetricsCache.SimplifiedMetrics);
                    fastResultsControl.UpdateFastRunResults(_stresstestMetricsCache.GetRunMetrics(), false, _stresstestMetricsCache.SimplifiedMetrics);
                    foreach (var monitorResultCache in GetMonitorResultCaches()) {
                        fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, _monitorMetricsCache.GetConcurrencyMetrics(monitorResultCache.Monitor));
                        fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, _monitorMetricsCache.GetRunMetrics(monitorResultCache.Monitor));
                    }

                    if (_stresstestMetricsCache.SimplifiedMetrics && !_simplifiedMetricsReturned) {
                        _simplifiedMetricsReturned = true;
                        fastResultsControl.AddEvent("It takes too long to calculate the fast results, therefore they are simplified!", Level.Warning);
                    }
                }
                _progressCountDown = PROGRESSUPDATEDELAY;

                ////#warning Enable REST
                //WriteRestProgress(RunStateChange.None);
            }
        }

        private void _stresstestCore_Message(object sender, MessageEventArgs e) {
            if (e.Color == Color.Empty) fastResultsControl.AddEvent(e.Message, e.LogLevel); else fastResultsControl.AddEvent(e.Message, e.Color, e.LogLevel);
        }
        #endregion

        #region Stop
        private void StresstestView_FormClosing(object sender, FormClosingEventArgs e) {
            if (btnStart.Enabled || _stresstestCore == null || _stresstestCore.IsDisposed ||
                MessageBox.Show("Are you sure you want to close a running test?", string.Empty, MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes) {
                StopMonitorsAndUnlockGui(null, true);
                Stop_Stresstest();

                tmrProgress.Stop();
                tmrProgressDelayCountDown.Stop();
                tmrSchedule.Stop();

                if (_stresstestCore != null && !_stresstestCore.IsDisposed) {
                    _stresstestCore.Dispose();
                    _stresstestCore = null;
                }
            } else {
                Solution.ExplicitCancelFormClosing = true;
                e.Cancel = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e) {
            StopStresstest();
        }
        public void StopStresstest() {
            if (!btnStop.Enabled) return;
            toolStrip.Enabled = false;
            if (_stresstestCore == null || (_monitorAfterCountDown != null && _monitorAfterCountDown.CountdownTime != 0)) {
                Stop(StresstestStatus.Cancelled);
            } else if (_monitorBeforeCountDown != null && _monitorBeforeCountDown.CountdownTime != 0) {
                _stresstestCore.Cancel();
                Stop(StresstestStatus.Cancelled);
            } else {
                // Can only be cancelled once, calling multiple times is not a problem.
                _stresstestCore.Cancel();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="ex">The exception if failed.</param>
        private void Stop(StresstestStatus stresstestStatus = StresstestStatus.Ok, Exception ex = null, bool monitorAfter = false) {
            _stresstestStatus = stresstestStatus;
            if (btnStop.Enabled || !toolStrip.Enabled) {
                Cursor = Cursors.WaitCursor;

                Stop_Stresstest();

                tmrProgress.Stop();

                if (_stresstestCore != null && !_stresstestCore.IsDisposed) {
                    _stresstestCore.Dispose();
                    _stresstestCore = null;
                }

                StopProgressDelayCountDown();


                tmrSchedule.Stop();
                btnSchedule.Text = string.Empty;
                btnSchedule.Tag = null;


                fastResultsControl.SetStresstestStopped(stresstestStatus, ex);

                toolStrip.Enabled = true;

                Cursor = Cursors.Default;

                //#warning Enable REST
                // WriteRestProgress(RunStateChange.None);

                int runningMonitors = 0;
                if (_monitorViews != null && _stresstest.Monitors.Length != 0)
                    foreach (MonitorView view in _monitorViews)
                        if (view != null && !view.IsDisposed)
                            runningMonitors++;

                if (monitorAfter && _stresstest.MonitorAfter != 0 && runningMonitors != 0 && stresstestStatus != StresstestStatus.Cancelled && stresstestStatus != StresstestStatus.Error) {
                    int countdownTime = _stresstest.MonitorAfter * 60000;
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
                        Loggers.Log(Level.Error, "Failed updating fast monitor results.", ex, new object[] { stresstestStatus, ex, monitorAfter });
                    }

                    fastResultsControl.ExpandEventPanel();
                    fastResultsControl.AddEvent("Monitoring after the test is finished: " + (_stresstest.MonitorAfter * 60) + " s.");
                    _monitorAfterCountDown.Start();
                } else {
                    StopMonitorsAndUnlockGui(ex, false);
                }
                this.Focus();

                if (stresstestStatus == StresstestStatus.Cancelled || stresstestStatus == StresstestStatus.Error)
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
                fastResultsControl.AddEvent("Monitoring after the test is finished: " + ts.ToShortFormattedString() + ".");

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
                }
            }, null);
        }

        /// <summary>
        ///     Only used in stop
        /// </summary>
        private void Stop_Stresstest() {
            btnSchedule.Tag = null;
            btnSchedule.Text = string.Empty;
            tmrSchedule.Stop();

            if (_stresstestCore != null && !_stresstestCore.IsDisposed) {
                try {
                    fastResultsControl.SetClientMonitoring(_stresstestCore.BusyThreadCount, LocalMonitor.CPUUsage, (int)LocalMonitor.MemoryUsage,
                                                          (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.Nic, LocalMonitor.NicBandwidth, LocalMonitor.NicSent, LocalMonitor.NicReceived);
                } catch { } //Exception on false WMI. 

                _stresstestMetricsCache.AllowSimplifiedMetrics = false;
                fastResultsControl.UpdateFastConcurrencyResults(_stresstestMetricsCache.GetConcurrencyMetrics(), true, _stresstestMetricsCache.SimplifiedMetrics);
                fastResultsControl.UpdateFastRunResults(_stresstestMetricsCache.GetRunMetrics(), false, _stresstestMetricsCache.SimplifiedMetrics);
                foreach (var monitorResultCache in GetMonitorResultCaches()) {
                    fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, _monitorMetricsCache.GetConcurrencyMetrics(monitorResultCache.Monitor));
                    fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, _monitorMetricsCache.GetRunMetrics(monitorResultCache.Monitor));
                }

                // Can only be cancelled once, calling multiple times is not a problem.
                if (_stresstestCore != null && !_stresstestCore.IsDisposed) try { _stresstestCore.Cancel(); } catch {
                        //Ignore. Should / can never happen.
                    }
            }

            fastResultsControl.SetStresstestStopped();
            _stresstestResult = null;
            _canUpdateMetrics = false;
        }

        /// <summary>
        ///     Only used in stop, stops the monitors if any, saves the results; Unlocks the gui so changes can be made to the stresstest.
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
            if (_monitorViews != null && _stresstest.Monitors.Length != 0)
                foreach (MonitorView view in _monitorViews)
                    if (view != null && !view.IsDisposed && view.IsRunning) {
                        view.Stop();
                        fastResultsControl.AddEvent(view.Text + " is stopped.");
                        validMonitorViews.Add(view);
                    }
            foreach (MonitorView view in validMonitorViews)
                try { _resultsHelper.SetMonitorResults(view.GetMonitorResultCache()); } catch (Exception e) {
                    Loggers.Log(Level.Error, view.Text + ": Failed adding results to the database.", e);
                    _stresstestStatus = StresstestStatus.Error;
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
                }

                if (exception == null) {
                    TestProgressNotifier.Notify(TestProgressNotifier.What.TestFinished, string.Concat(_stresstest.ToString(), " finished. Status: ", _stresstestStatus, "."));
                } else {
                    //Loggers.Log(Level.Error, _stresstest.ToString() + " Failed.", exception);
                    TestProgressNotifier.Notify(TestProgressNotifier.What.TestFinished, string.Concat(_stresstest.ToString(), " finished. Status: ", _stresstestStatus, "."), exception);
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

        //        if (_stresstestCore != null && !_stresstestCore.IsDisposed) {
        //            var stresstestCache = JSONObjectTreeHelper.AddSubCache(_stresstest.ToString(), testProgressCache);

        //            foreach (var metrics in _stresstestMetricsCache.GetConcurrencyMetrics())
        //                JSONObjectTreeHelper.ApplyToRunningTestFastConcurrencyResults(stresstestCache, metrics, runStateChange.ToString(), _stresstestStatus.ToString());

        //            JSONObjectTreeHelper.ApplyToRunningTestClientMonitorMetrics(clientMonitorCache, _stresstest.ToString(), _stresstestCore.BusyThreadCount, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond,
        //                                                  LocalMonitor.MemoryUsage, LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);

        //            var events = fastResultsControl.GetEvents();
        //            var messages = new string[events.Count];
        //            for (int i = 0; i != messages.Length; i++) {
        //                var e = events[i];
        //                messages[i] = e.EventType + ": " + e.Message + " [" + e.At + "]";
        //            }
        //            JSONObjectTreeHelper.ApplyToRunningTestMessages(messagesCache, _stresstest.ToString(), messages);

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