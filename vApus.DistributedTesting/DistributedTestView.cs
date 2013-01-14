/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using vApus.Monitor;
using vApus.REST.Convert;
using vApus.Results;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting {
    public partial class DistributedTestView : BaseSolutionComponentView {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        #region Fields

        private readonly object _lock = new object();

        /// <summary>
        ///     The monitors for the tests if any.
        /// </summary>
        private readonly Dictionary<TileStresstest, List<MonitorView>> _monitorViews = new Dictionary<TileStresstest, List<MonitorView>>();
        private Dictionary<TileStresstest, MonitorMetricsCache> _monitorMetricsCaches = new Dictionary<TileStresstest, MonitorMetricsCache>();

        private readonly AutoResetEvent _monitorViewsInitializedWaitHandle = new AutoResetEvent(false);

        /// <summary>
        ///     Countdown for the update.
        /// </summary>
        private int _countDown;

        private DistributedTest _distributedTest = new DistributedTest();
        private DistributedTestCore _distributedTestCore;
        private DistributedTestMode _distributedTestMode;

        private Countdown _monitorBeforeCountDown;

        /// <summary>
        ///     The test can only start when this == 0.
        /// </summary>
        private int _pendingMonitorViewInitializations;

        private ITreeViewItem _selectedTestItem;
        #endregion

        #region Properties

        private int TileStresstestCount {
            get {
                int count = 0;
                foreach (Tile t in _distributedTest.Tiles)
                    count += t.Count;
                return count;
            }
        }

        private int UsedTileStresstestCount {
            get {
                int count = 0;
                foreach (Tile t in _distributedTest.Tiles)
                    foreach (TileStresstest ts in t)
                        if (ts.Use)
                            ++count;
                return count;
            }
        }

        private int SlaveCount {
            get {
                int count = 0;
                foreach (Client c in _distributedTest.Clients)
                    count += c.Count;
                return count;
            }
        }

        private int UsedSlaveCount {
            get {
                int count = 0;
                foreach (Client c in _distributedTest.Clients)
                    foreach (Slave s in c)
                        if (s.TileStresstest != null)
                            ++count;
                return count;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Desing time constructor
        /// </summary>
        public DistributedTestView() {
            InitializeComponent();
        }

        public DistributedTestView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args) {
            InitializeComponent();

            SetDistributedTest(solutionComponent as DistributedTest);

            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;

            RemoteDesktopClient.RdpException += RemoteDesktopClient_RdpException;

            //Jumpstart the slaves when starting the test
            JumpStart.Done += JumpStart_Done;

            Shown += DistributedTestView_Shown; //if the test is empty, show the wizard.
        }

        #endregion

        #region General Functions

        private void SetDistributedTest(DistributedTest distributedTest) {
            _distributedTest = distributedTest;
            testTreeView.SetDistributedTest(_distributedTest);
            slaveTreeView.SetDistributedTest(_distributedTest);
            configureSlaves.SetDistributedTest(_distributedTest);
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender is DistributedTest)
                SetDistributedTest(sender as DistributedTest);
            else if (sender is Tile || sender is TileStresstest || sender is Client || sender is Slave)
                SetGui();
        }

        private void btnWizard_Click(object sender, EventArgs e) { ShowWizard(); }

        private void ShowWizard() {
            using (var wizard = new Wizard()) {
                wizard.SetDistributedTest(_distributedTest);
                wizard.ShowDialog();
            }
        }

        private void DistributedTestView_Shown(object sender, EventArgs e) {
            Shown -= DistributedTestView_Shown;
            ShowWizardIfNeeded();
        }

        private void ShowWizardIfNeeded() {
            if (_distributedTest.Tiles.Count == 0 && _distributedTest.Clients.Count == 0)
                ShowWizard();
        }

        private void testTreeView_AfterSelect(object sender, EventArgs e) {
            _selectedTestItem = sender as ITreeViewItem;
            if (sender is TileStresstestTreeViewItem) {
                var tstvi = sender as TileStresstestTreeViewItem;
                configureTileStresstest.SetTileStresstest(tstvi.TileStresstest);

                fastResultsControl.Visible = true;
                distributedStresstestControl.Visible = false;

                fastResultsControl.SetConfigurationControlsAndMonitorLinkButtons(tstvi.TileStresstest.ToString(),
                   tstvi.TileStresstest.BasicTileStresstest.Connection,
                   tstvi.TileStresstest.BasicTileStresstest.ConnectionProxy,
                   tstvi.TileStresstest.AdvancedTileStresstest.Log,
                   tstvi.TileStresstest.AdvancedTileStresstest.LogRuleSet,
                   tstvi.TileStresstest.BasicTileStresstest.Monitors,
                   tstvi.TileStresstest.AdvancedTileStresstest.Concurrency,
                   tstvi.TileStresstest.AdvancedTileStresstest.Runs,
                   tstvi.TileStresstest.AdvancedTileStresstest.MinimumDelay,
                   tstvi.TileStresstest.AdvancedTileStresstest.MaximumDelay,
                   tstvi.TileStresstest.AdvancedTileStresstest.Shuffle,
                   tstvi.TileStresstest.AdvancedTileStresstest.Distribute,
                   tstvi.TileStresstest.AdvancedTileStresstest.MonitorBefore,
                   tstvi.TileStresstest.AdvancedTileStresstest.MonitorAfter);

                if (_distributedTestCore != null) {
                    if (_distributedTestCore.TestProgressMessages.ContainsKey(tstvi.TileStresstest))
                        SetSlaveProgress(tstvi.TileStresstest, _distributedTestCore.TestProgressMessages[tstvi.TileStresstest]);
                }
            }
            else {
                bool showDescriptions = false;
                if (sender is DistributedTestTreeViewItem) {
                    var dttvi = sender as DistributedTestTreeViewItem;
                    foreach (Control ctrl in dttvi.Controls)
                        if ((ctrl is CheckBox && ctrl.Focused) ||
                            (ctrl is Panel && ctrl.Controls.Count != 0 && ctrl.Controls[0].Focused)) {
                            showDescriptions = true;
                            break;
                        }
                }
                configureTileStresstest.ClearTileStresstest(showDescriptions);

                fastResultsControl.Visible = false;
                distributedStresstestControl.Visible = true;

                SetOverallProgress();
            }
        }

        private void slaveTreeView_AfterSelect(object sender, EventArgs e) {
            if (sender is ClientTreeViewItem) {
                var ctvi = sender as ClientTreeViewItem;
                ctvi.ConfigureSlaves = configureSlaves;
                configureSlaves.SetClient(ctvi);
            }
            else {
                configureSlaves.ClearClient();
            }
        }

        private void tmrSetGui_Tick(object sender, EventArgs e) { SetGui(); }

        private void SetGui() {
            string tests = "Tests (" + UsedTileStresstestCount + "/" + TileStresstestCount + ")";
            if (tpTests.Text != tests) tpTests.Text = tests;

            string slaves = "Slaves (" + UsedSlaveCount + "/" + SlaveCount + ")";
            if (tpSlaves.Text != slaves) tpSlaves.Text = slaves;

            testTreeView.SetGui();
            slaveTreeView.SetGui();

            if (_distributedTestMode == DistributedTestMode.Edit)
                btnStart.Enabled = !testTreeView.Exclamation;
        }

        /// <summary>
        ///     Refresh some properties that are overriden in code.
        /// </summary>
        public override void Refresh() {
            base.Refresh();
            configureTileStresstest.Refresh();
        }

        private void configureSlaves_GoToAssignedTest(object sender, EventArgs e) {
            TileStresstest ts = (sender as SlaveTile).Slave.TileStresstest;
            if (ts != null) {
                tcTree.SelectedIndex = 0;
                testTreeView.SelectTileStresstest(ts);
            }
        }

        private void tpTree_SelectedIndexChanged(object sender, EventArgs e) {
            if (tcTree.SelectedIndex == 0) {
                configureTileStresstest.Visible = true;
                configureSlaves.Visible = false;
            }
            else {
                configureTileStresstest.Visible = false;
                configureSlaves.Visible = true;
            }
        }

        /// <summary>
        ///     Set the gui for the different modes
        /// </summary>
        /// <param name="distributedTestMode"></param>
        /// <param name="scheduled">only for distributedTestMode.TestAndReport</param>
        private void SetMode(DistributedTestMode distributedTestMode, bool canEnableStop = false, bool scheduled = false) {
            if (IsDisposed) return;

            _distributedTestMode = distributedTestMode;

            btnSchedule.Enabled = distributedTestMode == DistributedTestMode.Edit;

            btnStop.Enabled = canEnableStop && distributedTestMode == DistributedTestMode.TestAndReport;

            if (_distributedTestMode == DistributedTestMode.TestAndReport) {
                btnStart.Enabled = false;
                if (scheduled) tmrSchedule.Start(); else btnSchedule.Text = "Schedule...";
                //tcTree.SelectedTab = tpTests;
            }
            else {
                btnStart.Enabled = !testTreeView.Exclamation;

                tmrSchedule.Stop();
                tmrProgress.Stop();
                tmrProgressDelayCountDown.Stop();
            }

            testTreeView.SetMode(_distributedTestMode, scheduled);
            slaveTreeView.SetMode(_distributedTestMode);
            configureTileStresstest.SetMode(_distributedTestMode);
            configureSlaves.SetMode(_distributedTestMode);
        }

        #endregion

        #region Start

        private void btnStart_Click(object sender, EventArgs e) {
            if (_distributedTestCore != null && _distributedTestCore.HasResults &&
                MessageBox.Show("Do you want to clear the previous results, before starting the test (at the scheduled date / time)?",
                    string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            if (_distributedTest.RunSynchronization != RunSynchronization.None && !CheckNumberOfRuns()) {
                MessageBox.Show("Could not start the distributed test because the number of runs for the different single stresstests are not equal to each other.",
                    string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (btnSchedule.Tag != null && btnSchedule.Tag is DateTime && (DateTime)btnSchedule.Tag > DateTime.Now)
                ScheduleTest();
            else
                Start();
        }

        private void Start() {
            try {
                Cursor = Cursors.WaitCursor;
                SetMode(DistributedTestMode.TestAndReport);

                tcTest.SelectedIndex = 1;

                distributedStresstestControl.ClearFastResults();

                if (_distributedTest.UseRDP) ShowRemoteDesktop();

                distributedStresstestControl.AppendMessages("Jump Starting the slaves...");
                //Jumpstart the slaves first.
                JumpStart.Do(_distributedTest);
            }
            catch {
                //Only one test can run at the same time.
                distributedStresstestControl.AppendMessages("Failed to Jump Start one or more slaves.", LogLevel.Error);
                Stop(true);
            }
        }

        /// <summary>
        ///     A remote desktop is needed in order for the distributed test to work.
        /// </summary>
        private void ShowRemoteDesktop() {
            distributedStresstestControl.AppendMessages("Opening remote desktop connection(s) to the client(s)...");

            var rdc = SolutionComponentViewManager.Show(_distributedTest, typeof(RemoteDesktopClient)) as RemoteDesktopClient;
            rdc.Text = "Remote Desktop Client";
            Show();
            rdc.ClearRemoteDesktops();
            foreach (Client client in _distributedTest.Clients)
                if (client.UsedSlaveCount != 0)
                    rdc.ShowRemoteDesktop(client.HostName, client.IP, client.UserName, client.Password, client.Domain);
        }

        private void RemoteDesktopClient_RdpException(object sender, Util.RemoteDesktopClient.RdpExceptionEventArgs e) {
            string message = "Cannot open a remote desktop connection to " + e.IP + ". (error code: " + e.ErrorCode + ") ";
            distributedStresstestControl.AppendMessages(message, LogLevel.Error);
            LogWrapper.LogByLevel(message, LogLevel.Error);
        }

        private void JumpStart_Done(object sender, JumpStart.DoneEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                try {
                    if (e.Exceptions.Length == 0) {
                        if (_distributedTestCore != null && !_distributedTestCore.IsDisposed) {
                            _distributedTestCore.Dispose();
                            _distributedTestCore = null;
                        }

                        _distributedTestCore = new DistributedTestCore(_distributedTest);
                        _distributedTestCore.Message += _distributedTestCore_Message;
                        _distributedTestCore.OnTestProgressMessageReceived += _distributedTestCore_TestProgressMessageReceivedEventArgs;
                        _distributedTestCore.OnListeningError += _distributedTestCore_OnListeningError;
                        _distributedTestCore.OnFinished += _distributedTestCore_OnFinished;


                        var t = new Thread(InitializeAndStartTest);
                        t.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                        t.IsBackground = true;
                        t.Start();
                    }
                    else {
                        //Failed jump starting slaves
                        foreach (Exception ex in e.Exceptions) {
                            string message = ex.ToString();
                            distributedStresstestControl.AppendMessages(message, LogLevel.Error);
                            LogWrapper.LogByLevel(message, LogLevel.Error);
                        }
                        Stop(true);
                    }
                }
                catch {
                    //Only one test can run at the same time.
                    string message = "Cannot start this test because another one is still running.";
                    distributedStresstestControl.AppendMessages(message, LogLevel.Error);
                    LogWrapper.LogByLevel(message, LogLevel.Error);
                    Stop(true);
                }
            }, null);
        }

        private void ScheduleTest() {
            SetMode(DistributedTestMode.TestAndReport, false, true);
        }

        private void tmrSchedule_Tick(object sender, EventArgs e) {
            var scheduledAt = (DateTime)btnSchedule.Tag;
            if (scheduledAt <= DateTime.Now) {
                btnSchedule.Text = "Scheduled at " + scheduledAt;
                tmrSchedule.Stop();
                StartTest();
            }
            else {
                TimeSpan dt = scheduledAt - DateTime.Now;
                if (dt.Milliseconds != 0) {
                    dt = new TimeSpan(dt.Ticks - (dt.Ticks % TimeSpan.TicksPerSecond));
                    dt += new TimeSpan(0, 0, 1);
                }
                btnSchedule.Text = "Scheduled in " + dt.ToLongFormattedString();
            }
        }

        /// <summary>
        ///     Show the gui to be able to schedule the test.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSchedule_Click(object sender, EventArgs e) {
            Schedule schedule = (btnSchedule.Tag != null && btnSchedule.Tag is DateTime)
                                    ? new Schedule((DateTime)btnSchedule.Tag)
                                    : new Schedule();
            if (schedule.ShowDialog() == DialogResult.OK) {
                if (schedule.ScheduledAt > DateTime.Now) {
                    btnSchedule.Text = "Scheduled at " + schedule.ScheduledAt;
                    btnSchedule.Tag = schedule.ScheduledAt;
                }
                else {
                    btnSchedule.Text = "Schedule...";
                    btnSchedule.Tag = null;
                }

                btnStart_Click(this, null);
            }
        }

        /// <summary>
        ///     Check if the number of runs for the different single stresstests are equal to each other.
        ///     Use this when using run synchronization.
        /// </summary>
        /// <returns></returns>
        private bool CheckNumberOfRuns() {
            int numberOfRuns = -1;
            foreach (Tile t in _distributedTest.Tiles)
                foreach (TileStresstest ts in t)
                    if (ts.Use) {
                        if (numberOfRuns == -1) numberOfRuns = ts.AdvancedTileStresstest.Runs;
                        else if (numberOfRuns != ts.AdvancedTileStresstest.Runs) return false;
                    }
            return true;
        }

        private void InitializeAndStartTest() {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate { btnStop.Enabled = true; }, null);
            if (InitializeTest() && _pendingMonitorViewInitializations == 0) StartTest();
        }

        private bool InitializeTest() {
            _pendingMonitorViewInitializations = 0;

            try {
                //_results.Clear();
                _distributedTestCore.Initialize();
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                    fastResultsControl.SetStresstestInitialized();
                    // stresstestReportControl.ClearReport();

                    //Initialize the monitors.
                    _monitorViews.Clear();
                    _monitorMetricsCaches.Clear();
                    foreach (TileStresstest tileStresstest in _distributedTestCore.UsedTileStresstests)
                        for (int i = 0; i != tileStresstest.BasicTileStresstest.Monitors.Length; i++)
                            ShowAndInitMonitorView(tileStresstest, tileStresstest.BasicTileStresstest.Monitors[i]);
                }, null);

                if (_pendingMonitorViewInitializations != 0) _monitorViewsInitializedWaitHandle.WaitOne();
            }
            catch (Exception ex) {
                HandleInitializeOrStartException(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Can only start after that all monitor views are initialized.
        /// </summary>
        private void StartTest() {
            try {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                    try { LocalMonitor.StartMonitoring(Stresstest.Stresstest.ProgressUpdateDelay * 1000); }
                    catch { fastResultsControl.AppendMessages("Could not initialize the local monitor, something is wrong with your WMI service.", LogLevel.Error); }
                    tmrProgress.Interval = Stresstest.Stresstest.ProgressUpdateDelay * 1000;
                    tmrProgress.Start();

                    tmrProgressDelayCountDown.Start();

                    _countDown = Stresstest.Stresstest.ProgressUpdateDelay - 1;

                    StartMonitors();

                    Cursor = Cursors.Default;
                }, null);
            }
            catch (Exception ex) { HandleInitializeOrStartException(ex); }
        }

        private void MonitorBeforeDone() {
            try { _distributedTestCore.Start(); }
            catch (Exception ex) { HandleInitializeOrStartException(ex); }
        }

        private void HandleInitializeOrStartException(Exception ex) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                string message = string.Format("The stresstest threw an exception:{0}{1}", Environment.NewLine,
                                               ex.Message);
                distributedStresstestControl.AppendMessages(message, LogLevel.Error);
                if (_distributedTestCore != null && !_distributedTestCore.IsDisposed) {
                    _distributedTestCore.Dispose();
                    _distributedTestCore = null;
                }

                Stop(true);
                Cursor = Cursors.Default;
            }, null);
        }

        #endregion

        #region Progress

        private void _distributedTestCore_Message(object sender, MessageEventArgs e) { distributedStresstestControl.AppendMessages(e.Message); }

        private void _distributedTestCore_TestProgressMessageReceivedEventArgs(object sender, TestProgressMessageReceivedEventArgs e) {
            Handle_distributedTestCore_TestProgressMessageReceivedEventArgs(e.TileStresstest, e.TestProgressMessage);
        }

        private void Handle_distributedTestCore_TestProgressMessageReceivedEventArgs(TileStresstest tileStresstest, TestProgressMessage testProgressMessage) {
            if (_selectedTestItem != null && _selectedTestItem is TileStresstestTreeViewItem && (_selectedTestItem as TileStresstestTreeViewItem).TileStresstest == tileStresstest) {
                SetSlaveProgress(tileStresstest, testProgressMessage);

                if (testProgressMessage.StresstestStatus == StresstestStatus.Busy) {
                    tmrProgressDelayCountDown.Stop();
                    _countDown = Stresstest.Stresstest.ProgressUpdateDelay;
                    fastResultsControl.SetCountDownProgressDelay(_countDown);
                    tmrProgressDelayCountDown.Start();
                }
            }

            UpdateMonitorMetricsCaches(tileStresstest, testProgressMessage);

            SetOverallProgress();
            SetSlaveProgressInTreeView(tileStresstest, testProgressMessage);
        }
        private void UpdateMonitorMetricsCaches(TileStresstest tileStresstest, TestProgressMessage testProgressMessage) {
            if (_monitorViews.ContainsKey(tileStresstest)) {
                _monitorMetricsCaches[tileStresstest] = new MonitorMetricsCache();
                var monitorMetricsCache = _monitorMetricsCaches[tileStresstest];

                foreach (var monitorResultCache in GetMonitorResultCaches(tileStresstest)) {
                    foreach (var concurrencyMetrics in testProgressMessage.StresstestMetricsCache.GetConcurrencyMetrics()) {
                        var monitorMetrics = MonitorMetricsHelper.GetConcurrencyMetrics(monitorResultCache.Monitor, concurrencyMetrics, monitorResultCache);
                        monitorMetricsCache.Add(monitorMetrics);
                    }
                    foreach (var runMetrics in testProgressMessage.StresstestMetricsCache.GetRunMetrics()) {
                        var monitorMetrics = MonitorMetricsHelper.GetRunMetrics(monitorResultCache.Monitor, runMetrics, monitorResultCache);
                        monitorMetricsCache.Add(monitorMetrics);
                    }
                }
            }
        }

        private void SetOverallProgress() {
            if (_selectedTestItem != null)
                if (_selectedTestItem is DistributedTestTreeViewItem) {
                    distributedStresstestControl.SetTitle("Distributed Test");
                    if (_distributedTestCore != null && !_distributedTestCore.IsDisposed) {
                        var progress = new Dictionary<TileStresstest, StresstestMetricsCache>(_distributedTestCore.TestProgressMessages.Count);
                        foreach (TileStresstest tileStresstest in _distributedTestCore.TestProgressMessages.Keys) {
                            var tpm = _distributedTestCore.TestProgressMessages[tileStresstest].StresstestMetricsCache;
                            if (tpm != null) progress.Add(tileStresstest, tpm);
                        }

                        distributedStresstestControl.SetOverallFastResults(progress);
                    }
                }
                else if (_selectedTestItem is TileTreeViewItem) {
                    var ttvi = _selectedTestItem as TileTreeViewItem;
                    distributedStresstestControl.SetTitle(ttvi.Tile.Name + " " + ttvi.Tile.Index);
                    if (_distributedTestCore != null && !_distributedTestCore.IsDisposed) {
                        var progress = new Dictionary<TileStresstest, StresstestMetricsCache>();
                        foreach (TileStresstest tileStresstest in _distributedTestCore.TestProgressMessages.Keys)
                            if (ttvi.Tile.Contains(tileStresstest)) {
                                var tpm = _distributedTestCore.TestProgressMessages[tileStresstest].StresstestMetricsCache;
                                if (tpm != null) progress.Add(tileStresstest, tpm);
                            }

                        distributedStresstestControl.SetOverallFastResults(progress);
                    }
                }
        }
        /// <summary>
        /// </summary>
        /// <param name="tileStresstest"></param>
        /// <param name="testProgressMessage"></param>
        /// <param name="overalEndOfTimeFrame">The end of time frame for the full test.</param>
        private void SetSlaveProgress(TileStresstest tileStresstest, TestProgressMessage testProgressMessage) {
            lock (_lock) {
                //Build and add fast results.
                fastResultsControl.ClearFastResults();
                if (testProgressMessage.StresstestMetricsCache != null) {
                    fastResultsControl.UpdateFastConcurrencyResults(testProgressMessage.StresstestMetricsCache.GetConcurrencyMetrics());
                    fastResultsControl.UpdateFastRunResults(testProgressMessage.StresstestMetricsCache.GetRunMetrics());
                }
                var monitorResultCaches = GetMonitorResultCaches(tileStresstest);
                foreach (var monitorResultCache in monitorResultCaches) {
                    if (_monitorMetricsCaches.ContainsKey(tileStresstest)) {
                        var monitorMetricsCache = _monitorMetricsCaches[tileStresstest];
                        fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, monitorMetricsCache.GetConcurrencyMetrics(monitorResultCache.Monitor));
                        fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, monitorMetricsCache.GetRunMetrics(monitorResultCache.Monitor));
                    }
                }

                if (testProgressMessage.Events == null) fastResultsControl.ClearEvents();
                else fastResultsControl.SetEvents(testProgressMessage.Events);

                fastResultsControl.SetStresstestStarted(testProgressMessage.StartedAt);
                if (testProgressMessage.StresstestStatus == StresstestStatus.Busy)
                    fastResultsControl.SetMeasuredRuntime(testProgressMessage.MeasuredRuntime);
                else
                    fastResultsControl.SetStresstestStopped(testProgressMessage.StresstestStatus, testProgressMessage.MeasuredRuntime);

                fastResultsControl.SetClientMonitoring(testProgressMessage.ThreadsInUse, testProgressMessage.CPUUsage,
                                                      testProgressMessage.ContextSwitchesPerSecond, (int)testProgressMessage.MemoryUsage,
                                                      (int)testProgressMessage.TotalVisibleMemory,
                                                      testProgressMessage.NicsSent, testProgressMessage.NicsReceived);
            }
        }
        private void SetSlaveProgressInTreeView(TileStresstest tileStresstest, TestProgressMessage testProgressMessage) {
            lock (_lock) {
                DistributedTestTreeViewItem distributedTestTreeViewItem = null;
                TileStresstestTreeViewItem tileStresstestTreeViewItem = null;
                foreach (ITreeViewItem item in testTreeView.Items) {
                    if (item is DistributedTestTreeViewItem)
                        distributedTestTreeViewItem = item as DistributedTestTreeViewItem;
                    else if (item is TileStresstestTreeViewItem) {
                        var tstvi = item as TileStresstestTreeViewItem;
                        if (tstvi.TileStresstest == tileStresstest) {
                            tileStresstestTreeViewItem = tstvi;
                            break;
                        }
                    }
                }

                if (tileStresstestTreeViewItem != null) {
                    tileStresstestTreeViewItem.SetStresstestStatus(testProgressMessage.StresstestStatus);

                    //Build and add fast results.
                    if (testProgressMessage.StresstestMetricsCache != null) {
                        tileStresstestTreeViewItem.SetStresstestStarted(testProgressMessage.StartedAt);
                        tileStresstestTreeViewItem.SetMeasuredRunTime(testProgressMessage.MeasuredRuntime);

                        //Set the distributed test tree view item
                        distributedTestTreeViewItem.SetStresstestStarted();
                    }

                    if (testProgressMessage.Events == null) tileStresstestTreeViewItem.ClearEvents();
                    else tileStresstestTreeViewItem.SetEvents(testProgressMessage.Events);
                }
            }
        }

        private void testTreeView_EventClicked(object sender, EventProgressChart.ProgressEventEventArgs e) {
            if (sender == _selectedTestItem && _selectedTestItem is TileStresstestTreeViewItem) {
                tpStresstest.Select();
                fastResultsControl.ShowEvent(e.ProgressEvent.At);
            }
        }

        private void _distributedTestCore_OnListeningError(object sender, ListeningErrorEventArgs e) {
            //Stop the distributed test (it is not valid anymore if a slave fails)
            btnStop_Click(btnStop, null);

            //Update the stresstest result for the failed test and set the gui.
            foreach (TileStresstest tileStresstest in _distributedTestCore.TestProgressMessages.Keys) {
                bool found = false;
                foreach (Slave slave in tileStresstest.BasicTileStresstest.Slaves)
                    if (slave.IP == e.SlaveIP && slave.Port == e.SlavePort) {
                        if (_distributedTestCore.TestProgressMessages.ContainsKey(tileStresstest)) {
                            TestProgressMessage testProgressMessage = _distributedTestCore.TestProgressMessages[tileStresstest];
                            testProgressMessage.StresstestStatus = StresstestStatus.Error;
                            _distributedTestCore.TestProgressMessages[tileStresstest] = testProgressMessage;

                            Handle_distributedTestCore_TestProgressMessageReceivedEventArgs(tileStresstest, testProgressMessage);
                        }
                        found = true;
                        break;
                    }
                if (found) break;
            }
        }

        private void _distributedTestCore_OnFinished(object sender, FinishedEventArgs e) {
            _distributedTestCore.OnFinished -= _distributedTestCore_OnFinished;

            Stop(true, e.Cancelled == 0 && e.Error == 0);
            try {
                distributedStresstestControl.SetMasterMonitoring(_distributedTestCore.Running, _distributedTestCore.OK,
                                                                 _distributedTestCore.Cancelled,
                                                                 _distributedTestCore.Failed, LocalMonitor.CPUUsage,
                                                                 LocalMonitor.ContextSwitchesPerSecond,
                                                                 (int)LocalMonitor.MemoryUsage,
                                                                 (int)LocalMonitor.TotalVisibleMemory,
                                                                 LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
            }
            catch { } //Exception on false WMI. 
        }
        private void tmrProgressDelayCountDown_Tick(object sender, EventArgs e) {
            bool setCountDown = true;
            if (_selectedTestItem != null && _selectedTestItem is TileStresstestTreeViewItem)
                setCountDown = (_selectedTestItem as TileStresstestTreeViewItem).StresstestResult == StresstestStatus.Busy;
            if (--_countDown > 0 && setCountDown) fastResultsControl.SetCountDownProgressDelay(_countDown);

            /*#if EnableBetaFeature
            WriteMonitorRestProgress();
            #endif
             */
        }

        private void tmrProgress_Tick(object sender, EventArgs e) {
            try {
                distributedStresstestControl.SetMasterMonitoring(_distributedTestCore.Running, _distributedTestCore.OK,
                                                                 _distributedTestCore.Cancelled,
                                                                 _distributedTestCore.Failed, LocalMonitor.CPUUsage,
                                                                 LocalMonitor.ContextSwitchesPerSecond,
                                                                 (int)LocalMonitor.MemoryUsage,
                                                                 (int)LocalMonitor.TotalVisibleMemory,
                                                                 LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
            }
            catch { } //Exception on false WMI. 
            _countDown = Stresstest.Stresstest.ProgressUpdateDelay;
        }
        #endregion

        #region Stop

        private void DistributedTestView_FormClosing(object sender, FormClosingEventArgs e) {
            if (_distributedTestMode == DistributedTestMode.Edit ||
                MessageBox.Show("Are you sure you want to close a running test?", string.Empty, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning) == DialogResult.Yes) {
                tmrProgress.Stop();
                tmrProgressDelayCountDown.Stop();
                tmrSchedule.Stop();
                tmrSetGui.Stop();

                StopMonitors();

                if (_distributedTestCore != null)
                    try { _distributedTestCore.Stop(); }
                    catch { }
            }
            else {
                Solution.ActiveSolution.ExplicitCancelFormClosing = true;
                e.Cancel = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e) {
            distributedStresstestControl.AppendMessages("Stopping the test...");
            if (_monitorBeforeCountDown != null) {
                _monitorBeforeCountDown.Stop();
                _monitorBeforeCountDown.Dispose();
                _monitorBeforeCountDown = null;
            }

            Stop(false);
            distributedStresstestControl.AppendMessages("Test Cancelled!", LogLevel.Warning);
        }

        private void Stop(bool canEnableStop, bool monitorAfter = false) {
            Cursor = Cursors.WaitCursor;

            SetMode(DistributedTestMode.Edit, canEnableStop);

            if (btnSchedule.Tag != null && tmrSchedule.Tag is DateTime) {
                var scheduledDateTime = (DateTime)btnSchedule.Tag;
                btnSchedule.Text = scheduledDateTime > DateTime.Now ? "Scheduled at " + scheduledDateTime : "Schedule...";
            }
            if (_distributedTestCore != null)
                try {
                    _distributedTestCore.Stop();
                    try {
                        distributedStresstestControl.SetMasterMonitoring(_distributedTestCore.Running, _distributedTestCore.OK,
                                                                         _distributedTestCore.Cancelled, _distributedTestCore.Failed,
                                                                         LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond,
                                                                         (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory,
                                                                         LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
                    }
                    catch { } //Exception on false WMI. 
                }
                catch (Exception ex) {
                    string message = string.Format("The stresstest threw an exception:{0}{1}", Environment.NewLine,
                                                   ex.Message);
                    distributedStresstestControl.AppendMessages(message, LogLevel.Error);
                    monitorAfter = false;
                }

            Cursor = Cursors.Default;

            int runningMonitors = 0;
            int monitorAfterTime = 0;
            foreach (TileStresstest ts in _monitorViews.Keys) {
                if (ts.AdvancedTileStresstest.MonitorAfter > monitorAfterTime &&
                    ts.BasicTileStresstest.Monitors.Length != 0)
                    monitorAfterTime = ts.AdvancedTileStresstest.MonitorAfter;
                foreach (MonitorView view in _monitorViews[ts])
                    if (view != null && !view.IsDisposed)
                        ++runningMonitors;
            }
            if (monitorAfter && monitorAfterTime != 0 && runningMonitors != 0) {
                int countdownTime = monitorAfterTime * 60000;
                var monitorAfterCountdown = new Countdown(countdownTime, 5000);
                monitorAfterCountdown.Tick += monitorAfterCountdown_Tick;
                monitorAfterCountdown.Stopped += monitorAfterCountdown_Stopped;
                monitorAfterCountdown.Start();
            }
            else { StopMonitors(); }
        }

        private void monitorAfterCountdown_Tick(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                var monitorAfterCountDown = sender as Countdown;
                var ts = new TimeSpan(monitorAfterCountDown.CountdownTime * TimeSpan.TicksPerMillisecond);
                distributedStresstestControl.AppendMessages("Monitoring after the test is finished: " +
                                                            ts.ToShortFormattedString() + ".");

                int runningMonitors = 0;
                foreach (TileStresstest tileStresstest in _monitorViews.Keys)
                    foreach (MonitorView view in _monitorViews[tileStresstest])
                        if (view != null && !view.IsDisposed)
                            ++runningMonitors;

                if (runningMonitors == 0) {
                    monitorAfterCountDown.Stop();
                    distributedStresstestControl.AppendMessages("All monitors were manually closed.");
                }

#if EnableBetaFeature
                WriteMonitorRestConfig();
                WriteMonitorRestProgress();
#endif
            }, null);
        }

        private void monitorAfterCountdown_Stopped(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate { StopMonitors(); }, null);

            var monitorAfterCountdown = sender as Countdown;
            monitorAfterCountdown.Dispose();
            monitorAfterCountdown = null;

#if EnableBetaFeature
            WriteMonitorRestConfig();
            WriteMonitorRestProgress();
#endif
        }

        #endregion

        #region Monitors

        private void ShowAndInitMonitorView(TileStresstest tileStresstest, Monitor.Monitor monitor) {
            //show the monitorview
            MonitorView monitorView;
            if (!MonitorViewAlreadyInited(monitor, out monitorView)) {
                ++_pendingMonitorViewInitializations;

                monitorView = SolutionComponentViewManager.Show(monitor) as MonitorView;
                this.Show();

                distributedStresstestControl.AppendMessages("Initializing " + monitorView.Text + "...");
                //For each view initialized, the distributed test view takes care of starting the test.
                monitorView.MonitorInitialized += new EventHandler<MonitorView.MonitorInitializedEventArgs>(monitorView_MonitorInitialized);
                monitorView.OnHandledException += new EventHandler<ErrorEventArgs>(monitorView_OnHandledException);
                monitorView.OnUnhandledException += new EventHandler<ErrorEventArgs>(monitorView_OnUnhandledException);
                monitorView.InitializeForStresstest();
            }

            if (!_monitorViews.ContainsKey(tileStresstest))
                _monitorViews.Add(tileStresstest, new List<MonitorView>());
            _monitorViews[tileStresstest].Add(monitorView);

            if (!_monitorMetricsCaches.ContainsKey(tileStresstest))
                _monitorMetricsCaches.Add(tileStresstest, new MonitorMetricsCache());

#if EnableBetaFeature
            WriteMonitorRestConfig();
#endif
        }

        /// <summary>
        ///     To init it only once.
        /// </summary>
        /// <param name="monitor"></param>
        /// <param name="monitorView">Out this if found.</param>
        /// <returns></returns>
        private bool MonitorViewAlreadyInited(Monitor.Monitor monitor, out MonitorView monitorView) {
            monitorView = null;
            foreach (var l in _monitorViews.Values)
                foreach (MonitorView mv in l)
                    if (mv.Monitor == monitor) {
                        monitorView = mv;
                        return true;
                    }
            return false;
        }

        private void monitorView_MonitorInitialized(object sender, MonitorView.MonitorInitializedEventArgs e) {
            var view = sender as MonitorView;
            view.MonitorInitialized -= monitorView_MonitorInitialized;
            if (--_pendingMonitorViewInitializations == 0) _monitorViewsInitializedWaitHandle.Set();
        }

        private void monitorView_OnHandledException(object sender, ErrorEventArgs e) { var view = sender as MonitorView; }

        private void monitorView_OnUnhandledException(object sender, ErrorEventArgs e) { var view = sender as MonitorView; }

        /// <summary>
        /// Get all monitor result caches for al the running monitors.
        /// </summary>
        /// <returns></returns>
        private List<MonitorResultCache> GetMonitorResultCaches(TileStresstest tileStresstest) {
            var l = new List<MonitorResultCache>();
            if (_monitorViews != null)
                foreach (var ts in _monitorViews.Keys)
                    if (tileStresstest == ts)
                        foreach (var view in _monitorViews[ts])
                            l.Add(view.GetMonitorResultCache());
            return l;
        }

        /// <summary>
        ///     Used in stresstest started eventhandling.
        /// </summary>
        private void StartMonitors() {
            if (_monitorViews != null) {
                int runningMonitors = 0;
                int monitorBefore = 0;
                foreach (TileStresstest ts in _monitorViews.Keys) {
                    if (ts.AdvancedTileStresstest.MonitorBefore > monitorBefore &&
                        ts.BasicTileStresstest.Monitors.Length != 0)
                        monitorBefore = ts.AdvancedTileStresstest.MonitorBefore;

                    foreach (MonitorView view in _monitorViews[ts])
                        if (view != null && !view.IsDisposed)
                            try {
                                view.Start();
                                distributedStresstestControl.AppendMessages(view.Text + " is started.");
                                ++runningMonitors;
                            }
                            catch (Exception e) {
                                LogWrapper.LogByLevel(view.Text + " is not started.\n" + e, LogLevel.Error);
                                distributedStresstestControl.AppendMessages(view.Text + " is not started.");

                                try { view.Stop(); }
                                catch { }
                            }
                }
                if (runningMonitors != 0 && monitorBefore != 0) {
                    int countdownTime = monitorBefore * 60000;
                    _monitorBeforeCountDown = new Countdown(countdownTime, 5000);
                    _monitorBeforeCountDown.Tick += monitorBeforeCountDown_Tick;
                    _monitorBeforeCountDown.Stopped += monitorBeforeCountDown_Stopped;
                    _monitorBeforeCountDown.Start();
                }
                else MonitorBeforeDone();
            }
            else MonitorBeforeDone();
        }

        private void monitorBeforeCountDown_Tick(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                var ts = new TimeSpan(_monitorBeforeCountDown.CountdownTime * TimeSpan.TicksPerMillisecond);
                distributedStresstestControl.AppendMessages("The test will start in " + ts.ToShortFormattedString() +
                                                            ", monitoring first.");

                int runningMonitors = 0;
                foreach (TileStresstest tileStresstest in _monitorViews.Keys)
                    foreach (MonitorView view in _monitorViews[tileStresstest])
                        if (view != null && !view.IsDisposed)
                            runningMonitors++;

                if (runningMonitors == 0) {
                    _monitorBeforeCountDown.Stop();
                    distributedStresstestControl.AppendMessages("All monitors were manually closed.");
                }

#if EnableBetaFeature
                WriteMonitorRestConfig();
                WriteMonitorRestProgress();
#endif
            }, null);
        }

        private void monitorBeforeCountDown_Stopped(object sender, EventArgs e) {
            if (_monitorBeforeCountDown != null) {
                _monitorBeforeCountDown.Dispose();
                _monitorBeforeCountDown = null;
            }

#if EnableBetaFeature
            WriteMonitorRestConfig();
            WriteMonitorRestProgress();
#endif

            MonitorBeforeDone();
        }

        /// <summary>
        ///     Only used in stop
        /// </summary>
        private void StopMonitors() {
            //Same view for multiple tilestresstests.
            var stoppedMonitorViews = new List<MonitorView>();
            if (_monitorViews != null)
                foreach (TileStresstest ts in _monitorViews.Keys)
                    foreach (MonitorView view in _monitorViews[ts])
                        if (view != null && !view.IsDisposed && !stoppedMonitorViews.Contains(view)) {
                            stoppedMonitorViews.Add(view);
                            view.Stop();
                            distributedStresstestControl.AppendMessages(view.Text + " is stopped.");
                        }
            stoppedMonitorViews = null;
        }
        #endregion

        #region REST

        private void WriteMonitorRestConfig() {
            try {
                var monitorConfigCache = new Hashtable(1);
                foreach (TileStresstest key in _monitorViews.Keys)
                    foreach (MonitorView view in _monitorViews[key])
                        Converter.SetMonitorConfig(monitorConfigCache, _distributedTest.ToString(), view.Monitor);

                Converter.WriteToFile(monitorConfigCache, "MonitorConfig");
            }
            catch { }
        }

        private void WriteMonitorRestProgress() {
            try {
                var monitorProgressCache = new Hashtable(1);
                int monitorCount = 0;

                if (_monitorViews != null)
                    foreach (TileStresstest key in _monitorViews.Keys)
                        foreach (MonitorView view in _monitorViews[key]) {
                            ++monitorCount;
                            Converter.SetMonitorProgress(monitorProgressCache, _distributedTest.ToString(), view.Monitor,
                                                         view.GetMonitorResultCache().Headers, view.GetMonitorValues());
                        }
                if (monitorCount != 0) Converter.WriteToFile(monitorProgressCache, "MonitorProgress");
            }
            catch { }
        }

        #endregion
    }
}
