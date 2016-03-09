/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using RandomUtils;
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using vApus.Publish;
using vApus.Monitor;
using vApus.Results;
using vApus.Communication.Shared;
using vApus.SolutionTree;
using vApus.StressTest;
using vApus.Util;

namespace vApus.DistributedTest {
    public partial class DistributedTestView : BaseSolutionComponentView {

        #region Fields
        private readonly object _lock = new object();
        private Win32WindowMessageHandler _msgHandler;

        private ScheduleDialog _schedule = null;

        private ITreeViewItem _selectedTestTreeViewItem;

        private DistributedTest _distributedTest = new DistributedTest();
        private DistributedTestCore _distributedTestCore;
        private DistributedTestMode _distributedTestMode;

        /// <summary>
        ///     In seconds how fast the stress test progress will be updated.
        /// </summary>
        private const int PROGRESSUPDATEDELAY = 5;
        /// <summary>
        ///     Countdown for the update.
        /// </summary>
        private int _progressCountDown;

        private Countdown _monitorBeforeCountDown, _monitorAfterCountDown;
        /// <summary>
        ///     The test can only start when this == 0.
        /// </summary>
        private int _pendingMonitorViewInitializations;

        /// <summary>
        ///     The monitors for the tests if any.
        /// </summary>
        private readonly Dictionary<TileStressTest, List<MonitorView>> _monitorViews = new Dictionary<TileStressTest, List<MonitorView>>();
        private Dictionary<TileStressTest, MonitorMetricsCache> _monitorMetricsCaches = new Dictionary<TileStressTest, MonitorMetricsCache>();

        private readonly AutoResetEvent _monitorViewsInitializedWaitHandle = new AutoResetEvent(false);

        private ConcurrencyResult _monitorBeforeBogusConcurrencyResult, _monitorAfterBogusConcurrencyResult;
        private RunResult _monitorBeforeBogusRunResult, _monitorAfterBogusRunResult;

        private Dictionary<TileStressTest, Monitor.Monitor> _usedMonitors = new Dictionary<TileStressTest, Monitor.Monitor>();
        private int _monitorBefore = 0, _monitorAfter = 0;
        #endregion

        #region Properties
        public DistributedTestMode DistributedTestMode {
            get { return _distributedTestMode; }
        }
        private int TileStressTestCount {
            get {
                int count = 0;
                foreach (Tile t in _distributedTest.Tiles) count += t.Count;
                return count;
            }
        }
        private int UsedTileStressTestCount {
            get {
                int count = 0;
                foreach (Tile t in _distributedTest.Tiles)
                    foreach (TileStressTest ts in t)
                        if (ts.Use) ++count;
                return count;
            }
        }
        private int SlaveCount {
            get {
                int count = 0;
                foreach (Client c in _distributedTest.Clients) count += c.Count;
                return count;
            }
        }
        private int UsedSlaveCount {
            get {
                int count = 0;
                foreach (Client client in _distributedTest.Clients)
                    foreach (Slave slave in client)
                        if (slave.TileStressTest != null) ++count;
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
        public DistributedTestView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            InitializeComponent();

            _msgHandler = new Win32WindowMessageHandler();

            SetDistributedTest(solutionComponent as DistributedTest);

            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;

            Shown += DistributedTestView_Shown; //if the test is empty, show the wizard.
        }
        #endregion

        #region General Functions
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        private void SetDistributedTest(DistributedTest distributedTest) {
            _distributedTest = distributedTest;
            testTreeView.SetDistributedTest(_distributedTest);
            slaveTreeView.SetDistributedTest(_distributedTest);
            configureSlaves.SetDistributedTest(_distributedTest);
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender == _distributedTest)
                SetDistributedTest(_distributedTest);
            else if (sender is Tile || sender is TileStressTest || sender is Client || sender is Slave)
                RefreshGui();
        }

        /// <summary>
        /// Show the wizard if this is a new (empty) distributed test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DistributedTestView_Shown(object sender, EventArgs e) {
            Shown -= DistributedTestView_Shown;
            if (_distributedTest.Tiles.Count == 0 && _distributedTest.Clients.Count == 0)
                ShowWizard();

            //Sometimes this fails.
            testTreeView.SelectDistributedTestTreeViewItem();

            var tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 200;
            tmr.Tick += TmrSelectDistributedTestTreeViewItem_Tick;
            tmr.Start();
        }

        private void TmrSelectDistributedTestTreeViewItem_Tick(object sender, EventArgs e) {
            (sender as System.Windows.Forms.Timer).Stop();
            testTreeView.SelectDistributedTestTreeViewItem();
        }

        private void btnWizard_Click(object sender, EventArgs e) { ShowWizard(); }
        private void ShowWizard() {
            var stressTestProject = Solution.ActiveSolution.GetProject("StressTestProject") as StressTestProject;
            if (stressTestProject.CountOf(typeof(StressTest.StressTest)) != 0)
                using (var wizard = new Wizard(_distributedTest))
                    wizard.ShowDialog();
        }

        /// <summary>
        ///     Show the gui to be able to schedule the test.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSchedule_Click(object sender, EventArgs e) {
            this.ActiveControl = tcTest;
            _schedule = (btnSchedule.Tag != null && btnSchedule.Tag is DateTime) ? new ScheduleDialog((DateTime)btnSchedule.Tag) : new ScheduleDialog();
            if (_schedule.ShowDialog() == DialogResult.OK) {
                if (_schedule.ScheduledAt > DateTime.Now) {
                    btnSchedule.Tag = _schedule.ScheduledAt;
                }
                else {
                    btnSchedule.Text = string.Empty;
                    btnSchedule.Tag = null;
                }
                _schedule = null;
                btnStart_Click(this, null);
            }
            else {
                btnSchedule.Text = string.Empty;
            }
            _schedule = null;
        }

        private void tpTree_SelectedIndexChanged(object sender, EventArgs e) {
            if (tcTree.SelectedIndex == 0) {
                configureTileStressTest.Visible = true;
                tileOverview.Visible = _selectedTestTreeViewItem is TileTreeViewItem || _selectedTestTreeViewItem is DistributedTestTreeViewItem;
                configureSlaves.Visible = false;
            }
            else {
                configureTileStressTest.Visible = false;
                tileOverview.Visible = false;
                configureSlaves.Visible = true;
            }
        }

        /// <summary>
        /// Set the gui according to the selected test tree view item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void testTreeView_AfterSelect(object sender, EventArgs e) {
            bool wasSelectedBefore = _selectedTestTreeViewItem == sender;
            _selectedTestTreeViewItem = sender as ITreeViewItem;
            if (sender is TileStressTestTreeViewItem) {
                tileOverview.SendToBack();
                var tstvi = sender as TileStressTestTreeViewItem;
                configureTileStressTest.SetTileStressTest(tstvi.TileStressTest);

                fastResultsControl.Visible = true;
                distributedTestControl.Visible = false;

                string tileStressTestToString = tstvi.TileStressTest.ToString();
                if (!wasSelectedBefore) {

                    fastResultsControl.SetConfigurationControlsAndMonitorLinkButtons(tileStressTestToString, tstvi.TileStressTest.BasicTileStressTest.Connection,
                       tstvi.TileStressTest.BasicTileStressTest.ConnectionProxy, tstvi.TileStressTest.AdvancedTileStressTest.Scenarios, tstvi.TileStressTest.AdvancedTileStressTest.ScenarioRuleSet,
                       tstvi.TileStressTest.BasicTileStressTest.Monitors, tstvi.TileStressTest.AdvancedTileStressTest.Concurrencies, tstvi.TileStressTest.AdvancedTileStressTest.Runs,
                       tstvi.TileStressTest.AdvancedTileStressTest.InitialMinimumDelay, tstvi.TileStressTest.AdvancedTileStressTest.InitialMaximumDelay,
                       tstvi.TileStressTest.AdvancedTileStressTest.MinimumDelay, tstvi.TileStressTest.AdvancedTileStressTest.MaximumDelay, tstvi.TileStressTest.AdvancedTileStressTest.Shuffle,
                       tstvi.TileStressTest.AdvancedTileStressTest.ActionDistribution, tstvi.TileStressTest.AdvancedTileStressTest.MaximumNumberOfUserActions, tstvi.TileStressTest.AdvancedTileStressTest.MonitorBefore,
                       tstvi.TileStressTest.AdvancedTileStressTest.MonitorAfter, tstvi.TileStressTest.AdvancedTileStressTest.UseParallelExecutionOfRequests, tstvi.TileStressTest.AdvancedTileStressTest.MaximumPersistentConnections, tstvi.TileStressTest.AdvancedTileStressTest.PersistentConnectionsPerHostname);
                    fastResultsControl.ClearEvents();

                    if (_distributedTestCore != null) {
                        fastResultsControl.ClearFastResults();

                        var testProgressMessage = _distributedTestCore.GetTestProgressMessage(tstvi.TileStressTest);
                        if (testProgressMessage.TileStressTestIndex != null)
                            SetSlaveProgress(tstvi.TileStressTest, testProgressMessage);
                    }
                }
            }
            else if (sender is TileTreeViewItem) {
                tileOverview.Init((sender as TileTreeViewItem).Tile);
                tileOverview.BringToFront();
                configureTileStressTest.ClearTileStressTest();

                fastResultsControl.Visible = false;
                distributedTestControl.Visible = true;

                SetOverallProgress();
            }
            else if (sender is DistributedTestTreeViewItem) {
                tileOverview.Init(_distributedTest);
                tileOverview.BringToFront();

                configureTileStressTest.ClearTileStressTest();

                fastResultsControl.Visible = false;
                distributedTestControl.Visible = true;

                SetOverallProgress();
            }
            
            this.Focus();
        }
        private void testTreeView_TileStressTestTreeViewItemDoubleClicked(object sender, EventArgs e) {
            tcTest.SelectedIndex = 0;
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
        private void slaveTreeView_ClientTreeViewItemDoubleClicked(object sender, EventArgs e) {
            tcTest.SelectedIndex = 0;
        }

        private void configureSlaves_GoToAssignedTest(object sender, EventArgs e) {
            TileStressTest ts = (sender as SlaveTile).Slave.TileStressTest;
            if (ts != null) {
                tcTree.SelectedIndex = 0;
                testTreeView.SelectTileStressTest(ts);
            }
        }

        private void tmrRefreshGui_Tick(object sender, EventArgs e) { RefreshGui(); }
        private void RefreshGui() {
            string tests = "Tests (" + UsedTileStressTestCount + "/" + TileStressTestCount + ")";
            if (tpTests.Text != tests) tpTests.Text = tests;

            string slaves = "Slaves (" + UsedSlaveCount + "/" + SlaveCount + ")";
            if (tpSlaves.Text != slaves) tpSlaves.Text = slaves;

            testTreeView.SetGui();
            slaveTreeView.SetGui();

            if (_distributedTestMode == DistributedTestMode.Edit) btnStart.Enabled = !testTreeView.Exclamation;
        }
        /// <summary>
        ///     Refresh some properties that are overriden in code.
        /// </summary>
        public override void Refresh() {
            base.Refresh();
            configureTileStressTest.Refresh();
        }

        /// <summary>
        ///     Set the gui for the different modes
        /// </summary>
        /// <param name="distributedTestMode"></param>
        /// <param name="scheduled">only for distributedTestMode.TestAndReport</param>
        private void SetMode(DistributedTestMode distributedTestMode, bool canEnableStop = false, bool scheduled = false) {
            LockWindowUpdate(Handle);

            if (IsDisposed) return;

            _distributedTestMode = distributedTestMode;

            if (_distributedTestMode == DistributedTestMode.Test) {
                btnStop.Enabled = canEnableStop;
                btnStart.Enabled = btnSchedule.Enabled = btnWizard.Enabled = false;
                if (scheduled) tmrSchedule.Start(); else btnSchedule.Text = string.Empty;
            }
            else {
                btnStop.Enabled = false;
                btnStart.Enabled = btnSchedule.Enabled = !testTreeView.Exclamation;
                btnWizard.Enabled = true;

                tmrSchedule.Stop();
                tmrProgress.Stop();
                tmrProgressDelayCountDown.Stop();

                btnSchedule.Text = string.Empty;
                btnSchedule.Tag = null;
            }

            testTreeView.SetMode(_distributedTestMode);
            slaveTreeView.SetMode(_distributedTestMode);
            configureTileStressTest.SetMode(_distributedTestMode);
            configureSlaves.SetMode(_distributedTestMode);

            LockWindowUpdate(IntPtr.Zero);
        }
        #endregion

        #region Start & Schedule
        private void btnStart_Click(object sender, EventArgs e) {
            this.ActiveControl = tcTest;
            StartDistributedTest(true);
        }
        public void StartDistributedTest(bool allowMessageBox) {
            if (allowMessageBox)
                if (_distributedTestCore != null && _distributedTestCore.HasResults &&
                               MessageBox.Show("Do you want to clear the previous results, before starting the test (at the scheduled date / time)?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;

            if (_distributedTest.RunSynchronization != RunSynchronization.None && !CheckNumberOfRuns()) {
                if (allowMessageBox)
                    MessageBox.Show("Could not start the distributed test because the number of runs for the different single stress tests are not equal to each other.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TestConnections();

            if (InitDatabaseBeforeStart(!allowMessageBox)) {
                if (btnSchedule.Tag != null && btnSchedule.Tag is DateTime && (DateTime)btnSchedule.Tag > DateTime.Now)
                    ScheduleTest();
                else
                    Start();
            }
        }
        /// <summary>
        ///     Check if the number of runs for the different single stress tests are equal to each other.
        ///     Use this when using run synchronization.
        /// </summary>
        /// <returns></returns>
        private bool CheckNumberOfRuns() {
            int numberOfRuns = -1;
            foreach (Tile t in _distributedTest.Tiles)
                if (t.Use) {
                    foreach (TileStressTest ts in t)
                        if (ts.Use) {
                            if (numberOfRuns == -1)
                                numberOfRuns = ts.AdvancedTileStressTest.Concurrencies.Length * ts.AdvancedTileStressTest.Runs;
                            else if (numberOfRuns != ts.AdvancedTileStressTest.Concurrencies.Length * ts.AdvancedTileStressTest.Runs)
                                return false;
                        }
                }
            return true;
        }

        private void TestConnections() {
            AppendMessages("Testing the tile stress test connections...");

            var l = new List<Connection>();
            foreach (var ts in _distributedTest.UsedTileStressTests) {
                var connection = ts.BasicTileStressTest.Connection;
                if (!l.Contains(connection))
                    l.Add(connection);
            }

            var testConnections = new TestConnections();
            testConnections.Message += testConnections_Message;

            testConnections.Test(l);
        }
        private void testConnections_Message(object sender, TestConnections.TestWorkItem.MessageEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                if (!e.Succes)
                    AppendMessages("The master cannot connect to " + e.Connection + ". It is likely that the slave won't be able to connect also!\nThe test will continue regardlessly.\nDetails: " + e.Message, Level.Warning);
            }, null);
        }

        /// <summary>
        ///     True on success or if user said there can be proceed without database.
        /// </summary>
        /// <returns></returns>
        private bool InitDatabaseBeforeStart(bool autoConfirmDialog) {
            var dialog = new DescriptionAndTagsInputDialog { Description = _distributedTest.Description, Tags = _distributedTest.Tags, AutoConfirm = autoConfirmDialog };
            if (dialog.ShowDialog() == DialogResult.Cancel) 
                return false;
            
            bool edited = false;
            if (_distributedTest.Description != dialog.Description) {
                _distributedTest.Description = dialog.Description;
                edited = true;
            }
            if (_distributedTest.Tags.Combine(", ") != dialog.Tags.Combine(", ")) {
                _distributedTest.Tags = dialog.Tags;
                edited = true;
            }

            if (edited) _distributedTest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            return true;
        }

        private void ScheduleTest() { SetMode(DistributedTestMode.Test, true, true); }
        private void tmrSchedule_Tick(object sender, EventArgs e) {
            var scheduledAt = (DateTime)btnSchedule.Tag;
            if (scheduledAt <= DateTime.Now) {
                btnSchedule.Text = string.Empty;
                btnSchedule.Tag = null;
                tmrSchedule.Stop();
                Start();
            }
            else {
                TimeSpan dt = scheduledAt - DateTime.Now;
                if (dt.Milliseconds != 0) {
                    dt = new TimeSpan(dt.Ticks - (dt.Ticks % TimeSpan.TicksPerSecond));
                    dt += new TimeSpan(0, 0, 1);
                }
                btnSchedule.Text = "Scheduled in " + dt.ToLongFormattedString(true);
            }
        }

        async private void Start() {
            try {
                Cursor = Cursors.WaitCursor;
                //Otherwise a handle problem can arise
                SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                    SetMode(DistributedTestMode.Test);
                }, null);

                tcTest.SelectedIndex = 1;

                if (_distributedTest.UseRDP) ShowRemoteDesktop();

                distributedTestControl.ClearFastResults();

                //Smart update
                UpdateNotifier.Refresh();
                string host, username, password;
                int port, channel;
                bool smartUpdate;
                UpdateNotifier.GetCredentials(out host, out port, out username, out password, out channel, out smartUpdate);

                if (smartUpdate) {
                    if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.NewUpdateFound) {
                        if (MessageBox.Show("In order to be able to update the used slaves the master must be up to date as well.\nDo you want to do this now?", "Smart Update Slaves",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                            Update(host, port, username, password, channel);

                        //In both cases the test cannot go on.
                        Stop();
                        return;
                    }
                    else if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.UpToDate) {
                        AppendMessages("Updating the slaves, this can take a while...");
                        var exceptions = await JumpStart.SmartUpdate(_distributedTest);

                        if (exceptions.Length != 0) {
                            string message = "Failed to update one or more slaves.\n";
                            foreach (Exception ex in exceptions)
                                message += ex.Message + "\n";

                            message = message.Trim();
                            AppendMessages(message, Level.Error);

                            throw new Exception(message);
                        }
                    }
                }

                AppendMessages("Jump Starting the slaves...");
                try {
                    //Jumpstart the slaves first, an event will be fired when this is done and the test will start
                    JumpStart.Done += JumpStart_Done;
                    JumpStart.Do(_distributedTest);
                }
                catch {
                    //Only one test can run at the same time.
                    JumpStart.Done -= JumpStart_Done;

                    AppendMessages("Failed to Jump Start one or more slaves.", Level.Error);
                    throw;
                }

            }
            catch (Exception ex) {
                Stop();
                Loggers.Log(Level.Error, "Failed starting the test.", ex);
            }
        }

        private void Update(string host, int port, string username, string password, int channel) {
            Cursor = Cursors.WaitCursor;
            string path = Path.Combine(Application.StartupPath, "vApus.UpdateToolLoader.exe");
            if (File.Exists(path)) {
                var process = new Process();
                process.EnableRaisingEvents = true;
                string solution = Solution.ActiveSolution == null ? string.Empty : " \"" + Solution.ActiveSolution.FileName + "\"";
                string arguments = "{A84E447C-3734-4afd-B383-149A7CC68A32} " + host + " " + port + " " +
                                    username + " " + password + " " + channel + " " + false + " " + false + solution;
                process.StartInfo = new ProcessStartInfo(path, arguments);

                Enabled = false;

                process.Exited += updateProcess_Exited;
                process.Start();
            }
            else {
                MessageBox.Show("vApus could not be updated because the update tool was not found!", string.Empty,
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            Cursor = Cursors.Default;
        }
        private void updateProcess_Exited(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                Enabled = true;
                _msgHandler.PostMessage();

                UpdateNotifier.Refresh();
            }, null);
        }
        /// <summary>
        ///     A remote desktop is needed in order for the distributed test to work.
        /// </summary>
        private void ShowRemoteDesktop() {
            AppendMessages("Opening remote desktop connection(s) to the client(s)...");

            foreach (Client client in _distributedTest.Clients)
                if (client.UsedSlaveCount != 0) {
                    RemoteDesktop.Show(client.IP, client.UserName, client.Password, client.Domain);
                    Thread.Sleep(1000);
                    RemoteDesktop.RemoveCredentials(client.IP);
                }
        }

        private void JumpStart_Done(object sender, JumpStart.DoneEventArgs e) {
            JumpStart.Done -= JumpStart_Done;

            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                try {
                    if (e.Exceptions.Length == 0) {
                        if (_distributedTestCore != null && !_distributedTestCore.IsDisposed) {
                            _distributedTestCore.Dispose();
                            _distributedTestCore = null;
                        }

                        _distributedTestCore = new DistributedTestCore(_distributedTest);
                        _distributedTestCore.Message += _distributedTestCore_Message;
                        _distributedTestCore.OnTestProgressMessageReceivedDelayed += _distributedTestCore_TestProgressMessageReceivedDelayed;
                        _distributedTestCore.OnListeningError += _distributedTestCore_OnListeningError;
                        _distributedTestCore.OnFinished += _distributedTestCore_OnFinished;

                        PublishConfiguration();

                        var t = new Thread(InitializeAndStartTest);
                        t.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                        t.IsBackground = true;
                        t.Start();
                    }
                    else {
                        //Failed jump starting slaves
                        foreach (Exception ex in e.Exceptions) {
                            string message = ex.Message + "\n\nSee " + Loggers.GetLogger<FileLogger>().CurrentLogFile;
                            AppendMessages(message, Level.Error);
                            Loggers.Log(Level.Error, message, ex);
                        }

                        Stop();
                    }
                }
                catch (Exception ex) {
                    //Only one test can run at the same time.
                    string message = "Cannot start this test because another one is still running.";
                    AppendMessages(message, Level.Error);
                    Loggers.Log(Level.Error, message, ex);
                    Stop();
                }
            }, null);
        }

        private void InitializeAndStartTest() {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate { btnStop.Enabled = true; }, null);
            //Avoid starting when pressed stop right before this.
            if (InitializeTest() && _pendingMonitorViewInitializations == 0 && _distributedTestMode != DistributedTestMode.Edit)
                StartTestAndMonitors();
        }
        private bool InitializeTest() {
            _pendingMonitorViewInitializations = 0;

            //Avoid starting when pressed stop right before this.
            if (_distributedTestMode == DistributedTestMode.Edit) return false;

            try {
                bool notACleanDivision;
                _distributedTestCore.Initialize(out notACleanDivision);
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                    if (notACleanDivision)
                        AppendMessages("The averages in the fast results for one or more tile stress tests will NOT be correct because one or more given concurrencies divided by the number of slaves is not an integer!" +
                            "Please use the detailed results.\nSee the log for more details.", Level.Warning);

                    fastResultsControl.SetStressTestInitialized();

                    //Initialize the monitors.
                    _monitorViews.Clear();
                    _monitorMetricsCaches.Clear();

                    foreach (TileStressTest tileStressTest in _distributedTest.UsedTileStressTests)
                        foreach (var monitor in tileStressTest.BasicTileStressTest.Monitors)
                            ShowAndInitMonitorView(tileStressTest, monitor);
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
        private void StartTestAndMonitors() {
            try {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                    try { LocalMonitor.StartMonitoring(PROGRESSUPDATEDELAY * 1000); } catch { AppendMessages("Could not initialize the local monitor, something is wrong with your WMI service.", Level.Error); }

                    _usedMonitors = new Dictionary<TileStressTest, Monitor.Monitor>();
                    _monitorBefore = 0;
                    _monitorAfter = 0;
                    if (_monitorViews != null) {
                        int runningMonitors = 0;
                        foreach (TileStressTest ts in _monitorViews.Keys) {
                            if (ts.AdvancedTileStressTest.MonitorBefore > _monitorBefore && ts.BasicTileStressTest.Monitors.Length != 0)
                                _monitorBefore = ts.AdvancedTileStressTest.MonitorBefore;

                            foreach (MonitorView monitorView in _monitorViews[ts])
                                if (monitorView != null && !monitorView.IsDisposed)
                                    try {
                                        if (monitorView.Start()) {
                                            AppendMessages(monitorView.Text + " is started.");
                                            ++runningMonitors;

                                            _usedMonitors.Add(ts, monitorView.Monitor);
                                        }
                                        else {
                                            try { monitorView.Stop(); } catch { }
                                        }
                                    }
                                    catch (Exception e) {
                                        Loggers.Log(Level.Error, monitorView.Text + " is not started.", e);
                                        AppendMessages(monitorView.Text + " is not started.");

                                        try {
                                            monitorView.Stop();
                                        }
                                        catch {
                                            //Dont't care at this point.
                                        }
                                    }
                        }

                        if (runningMonitors != 0 && _monitorBefore != 0) {
                            PublishMonitorBeforeTestStarted();
                            int countdownTime = _monitorBefore * 60000;

                            _monitorBeforeCountDown = new Countdown();
                            _monitorBeforeCountDown.Tick += monitorBeforeCountDown_Tick;
                            _monitorBeforeCountDown.Stopped += monitorBeforeCountDown_Stopped;

                            _monitorBeforeBogusConcurrencyResult = new ConcurrencyResult(-1, -1, 1);
                            _monitorBeforeBogusRunResult = new RunResult(-1, -1, 0);
                            _monitorBeforeBogusConcurrencyResult.RunResults.Add(_monitorBeforeBogusRunResult);

                            _monitorAfterBogusConcurrencyResult = null;
                            _monitorAfterBogusRunResult = null;

                            try {
                                foreach (var tileStressTest in _monitorMetricsCaches.Keys)
                                    foreach (var monitorResultCache in GetMonitorResultCaches(tileStressTest)) {
                                        var monitorMetricsCache = _monitorMetricsCaches[tileStressTest];
                                        monitorMetricsCache.AddOrUpdate(_monitorBeforeBogusConcurrencyResult, monitorResultCache);
                                        monitorMetricsCache.AddOrUpdate(_monitorBeforeBogusRunResult, monitorResultCache);

                                        if (_selectedTestTreeViewItem != null && _selectedTestTreeViewItem is TileStressTestTreeViewItem && (_selectedTestTreeViewItem as TileStressTestTreeViewItem).TileStressTest == tileStressTest) {
                                            fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, monitorMetricsCache.GetConcurrencyMetrics(monitorResultCache.Monitor));
                                            fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, monitorMetricsCache.GetRunMetrics(monitorResultCache.Monitor));
                                        }
                                    }
                            }
                            catch (Exception ex) {
                                Loggers.Log(Level.Error, "Failed updating metrics.", ex);
                            }

                            testTreeView.SetMonitoringBeforeAfter();
                            AppendMessages("Monitoring before the test starts: " + (_monitorBefore * 60) + " s.");
                            _monitorBeforeCountDown.Start(countdownTime, 5000);
                        }
                        else {
                            MonitorBeforeDone();
                        }
                    }
                    else {
                        MonitorBeforeDone();
                    }

                    Cursor = Cursors.Default;
                }, null);
            }
            catch (Exception ex) { HandleInitializeOrStartException(ex); }
        }

        private void HandleInitializeOrStartException(Exception exception) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                string message = exception.Message + "\n\nSee " + Loggers.GetLogger<FileLogger>().CurrentLogFile;
                AppendMessages(message, Level.Error);
                if (_distributedTestCore != null && !_distributedTestCore.IsDisposed) {
                    _distributedTestCore.Dispose();
                    _distributedTestCore = null;
                }

                Stop();
                Cursor = Cursors.Default;
            }, null);
        }
        #endregion

        #region Progress
        private void _distributedTestCore_Message(object sender, MessageEventArgs e) {
            AppendMessages(e.Message, e.Level);
        }
        private void AppendMessages(string message, Level level = Level.Info) {
            distributedTestControl.AppendMessages(message, level);
            PublishMessage((int)level, message);
        }

        private void _distributedTestCore_TestProgressMessageReceivedDelayed(object sender, EventArgs e) {
            Handle_distributedTestCore_TestProgressMessageReceivedDelayed(null, new TestProgressMessage());
        }
        private void _distributedTestCore_OnListeningError(object sender, ListeningErrorEventArgs e) {
            //Stop the distributed test (it is not valid anymore if a slave fails)
            StopDistributedTest();

            //Update the stress test result for the failed test and set the gui.
            var testProgressMessages = _distributedTestCore.GetAllTestProgressMessages();
            foreach (TileStressTest tileStressTest in testProgressMessages.Keys) {
                bool found = false;
                foreach (Slave slave in tileStressTest.BasicTileStressTest.Slaves)
                    if (slave.IP == e.SlaveIP && slave.Port == e.SlavePort) {
                        if (testProgressMessages.ContainsKey(tileStressTest)) {
                            TestProgressMessage testProgressMessage = testProgressMessages[tileStressTest];
                            testProgressMessage.StressTestStatus = StressTestStatus.Error;

                            Handle_distributedTestCore_TestProgressMessageReceivedDelayed(tileStressTest, testProgressMessage);
                        }
                        found = true;
                        break;
                    }
                if (found) break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tileStressTest">Can be null, but then the given test progress message will not be taken into account.</param>
        /// <param name="testProgressMessage"></param>
        private void Handle_distributedTestCore_TestProgressMessageReceivedDelayed(TileStressTest tileStressTest, TestProgressMessage testProgressMessage) {
            var testProgressMessages = _distributedTestCore.GetAllTestProgressMessages();
            if (_selectedTestTreeViewItem != null && _selectedTestTreeViewItem is TileStressTestTreeViewItem) {
                var tileStressTestTreeviewItem = _selectedTestTreeViewItem as TileStressTestTreeViewItem;
                if (tileStressTest == null) {
                    tileStressTest = tileStressTestTreeviewItem.TileStressTest;
                    testProgressMessage = testProgressMessages[tileStressTest];
                }

                if (tileStressTestTreeviewItem.TileStressTest == tileStressTest) {
                    SetSlaveProgress(tileStressTest, testProgressMessage);

                    if (testProgressMessage.StressTestStatus == StressTestStatus.Busy) {
                        tmrProgressDelayCountDown.Stop();
                        _progressCountDown = PROGRESSUPDATEDELAY;
                        fastResultsControl.SetCountDownProgressDelay(_progressCountDown);
                        tmrProgressDelayCountDown.Start();
                    }
                }
            }

            SetOverallProgress();

            //Add monitor metrics to db.
            WriteMonitorMetricsToDatabase();

            //Update the progress for all seperate tile stress tests, since a delayed invoke is used we must do this.
            foreach (var ts in _distributedTest.UsedTileStressTests)
                if (testProgressMessages.ContainsKey(ts)) {
                    tileStressTest = ts;
                    testProgressMessage = testProgressMessages[ts];

                    UpdateMonitorMetricsCaches(tileStressTest, testProgressMessage);

                    SetSlaveProgressInTreeView(tileStressTest, testProgressMessage);

                    //Notify by mail if set in the options panel.
                    if (testProgressMessage.StressTestStatus == StressTestStatus.Busy) {
                        if (testProgressMessage.RunFinished) {
                            var l = testProgressMessage.StressTestMetricsCache.GetRunMetrics(true);
                            var runMetrics = l[l.Count - 1];
                            string message = string.Concat(tileStressTest.ToString(), " - Run ", runMetrics.Run, " of concurrency ", runMetrics.Concurrency, " finished.");
                            TestProgressNotifier.Notify(TestProgressNotifier.What.RunFinished, message);
                        }
                        else if (testProgressMessage.ConcurrencyFinished) {
                            var l = testProgressMessage.StressTestMetricsCache.GetConcurrencyMetrics(true);
                            var concurrencyMetrics = l[l.Count - 1];
                            string message = string.Concat(tileStressTest.ToString(), " - Concurrency ", concurrencyMetrics.Concurrency, " finished.");
                            TestProgressNotifier.Notify(TestProgressNotifier.What.ConcurrencyFinished, message);
                        }
                    }
                    else {
                        TestProgressNotifier.Notify(TestProgressNotifier.What.TestFinished, string.Concat(tileStressTest.ToString(), " finished. Status: ", testProgressMessage.StressTestStatus, "."));
                    }
                }
        }

        private void UpdateMonitorMetricsCaches(TileStressTest tileStressTest, TestProgressMessage testProgressMessage) {
            if (_monitorViews.ContainsKey(tileStressTest)) {
                _monitorMetricsCaches[tileStressTest] = new MonitorMetricsCache();
                var monitorMetricsCache = _monitorMetricsCaches[tileStressTest];

                foreach (var monitorResultCache in GetMonitorResultCaches(tileStressTest)) {
                    if (_monitorBeforeBogusConcurrencyResult != null)
                        monitorMetricsCache.Add(MonitorMetricsHelper.GetMetrics(_monitorBeforeBogusConcurrencyResult, monitorResultCache));
                    foreach (var concurrencyMetrics in testProgressMessage.StressTestMetricsCache.GetConcurrencyMetrics(true))
                        monitorMetricsCache.Add(MonitorMetricsHelper.GetConcurrencyMetrics(monitorResultCache.Monitor, concurrencyMetrics, monitorResultCache));

                    if (_monitorAfterCountDown == null && _monitorAfterBogusConcurrencyResult != null)
                        monitorMetricsCache.Add(MonitorMetricsHelper.GetMetrics(_monitorAfterBogusConcurrencyResult, monitorResultCache));


                    if (_monitorBeforeBogusRunResult != null)
                        monitorMetricsCache.Add(MonitorMetricsHelper.GetMetrics(_monitorBeforeBogusRunResult, monitorResultCache));
                    foreach (var runMetrics in testProgressMessage.StressTestMetricsCache.GetRunMetrics(true))
                        monitorMetricsCache.Add(MonitorMetricsHelper.GetRunMetrics(monitorResultCache.Monitor, runMetrics, monitorResultCache));

                    if (_monitorAfterCountDown == null && _monitorAfterBogusRunResult != null)
                        monitorMetricsCache.Add(MonitorMetricsHelper.GetMetrics(_monitorAfterBogusRunResult, monitorResultCache));
                }
            }
        }

        private void SetOverallProgress() {
            if (_selectedTestTreeViewItem != null) {
                Dictionary<TileStressTest, TestProgressMessage> testProgressMessages = null;
                if (_selectedTestTreeViewItem is DistributedTestTreeViewItem) {
                    distributedTestControl.SetTitle("Distributed Test");
                    if (_distributedTestCore != null && !_distributedTestCore.IsDisposed) {
                        testProgressMessages = _distributedTestCore.GetAllTestProgressMessages();
                        var progress = new Dictionary<TileStressTest, FastStressTestMetricsCache>(testProgressMessages.Count);
                        foreach (TileStressTest tileStressTest in testProgressMessages.Keys) {
                            var metricsCache = testProgressMessages[tileStressTest].StressTestMetricsCache;
                            if (metricsCache != null) progress.Add(tileStressTest, metricsCache);
                        }

                        distributedTestControl.SetOverallFastResults(progress);
                    }
                }
                else if (_selectedTestTreeViewItem is TileTreeViewItem) {
                    var ttvi = _selectedTestTreeViewItem as TileTreeViewItem;
                    distributedTestControl.SetTitle(ttvi.Tile.Name + " " + ttvi.Tile.Index);
                    if (_distributedTestCore != null && !_distributedTestCore.IsDisposed) {
                        testProgressMessages = _distributedTestCore.GetAllTestProgressMessages();
                        var progress = new Dictionary<TileStressTest, FastStressTestMetricsCache>();
                        foreach (TileStressTest tileStressTest in testProgressMessages.Keys)
                            if (ttvi.Tile.Contains(tileStressTest)) {
                                var metricsCache = testProgressMessages[tileStressTest].StressTestMetricsCache;
                                if (metricsCache != null) progress.Add(tileStressTest, metricsCache);
                            }

                        distributedTestControl.SetOverallFastResults(progress);
                    }
                }

                PublishClientMonitoring();
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="tileStressTest"></param>
        /// <param name="testProgressMessage"></param>
        /// <param name="overalEndOfTimeFrame">The end of time frame for the full test.</param>
        private void SetSlaveProgress(TileStressTest tileStressTest, TestProgressMessage testProgressMessage) {
            lock (_lock) {
                //Build and add fast results. Do not show the updates in label if monitoring before.
                fastResultsControl.ClearFastResults(testProgressMessage.StartedAt == DateTime.MinValue);
                if (testProgressMessage.StressTestMetricsCache != null) {
                    fastResultsControl.UpdateFastConcurrencyResults(testProgressMessage.StressTestMetricsCache.GetConcurrencyMetrics(true), true);
                    fastResultsControl.UpdateFastRunResults(testProgressMessage.StressTestMetricsCache.GetRunMetrics(true), false);
                }
                var monitorResultCaches = GetMonitorResultCaches(tileStressTest);
                foreach (var monitorResultCache in monitorResultCaches) {
                    if (_monitorMetricsCaches.ContainsKey(tileStressTest)) {
                        var monitorMetricsCache = _monitorMetricsCaches[tileStressTest];
                        fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, monitorMetricsCache.GetConcurrencyMetrics(monitorResultCache.Monitor));
                        fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, monitorMetricsCache.GetRunMetrics(monitorResultCache.Monitor));
                    }
                }

                if (testProgressMessage.Events == null) fastResultsControl.ClearEvents();
                else fastResultsControl.SetEvents(testProgressMessage.Events);

                if (testProgressMessage.StartedAt != DateTime.MinValue) {
                    fastResultsControl.SetStressTestStarted(testProgressMessage.StartedAt);
                    if (testProgressMessage.StressTestStatus == StressTestStatus.Busy)
                        fastResultsControl.SetMeasuredRuntime(testProgressMessage.MeasuredRuntime);
                    else {
                        string message;
                        fastResultsControl.SetStressTestStopped(testProgressMessage.StressTestStatus, testProgressMessage.MeasuredRuntime, out message);
                    }
                }

                fastResultsControl.SetClientMonitoring(testProgressMessage.ThreadsInUse, testProgressMessage.CPUUsage,
                    (int)testProgressMessage.MemoryUsage, (int)testProgressMessage.TotalVisibleMemory, testProgressMessage.Nic, testProgressMessage.NicBandwidth,
                    testProgressMessage.NicSent, testProgressMessage.NicReceived);
            }
        }
        private void SetSlaveProgressInTreeView(TileStressTest tileStressTest, TestProgressMessage testProgressMessage) {
            lock (_lock) {
                DistributedTestTreeViewItem distributedTestTreeViewItem = null;
                TileStressTestTreeViewItem tileStressTestTreeViewItem = null;
                foreach (ITreeViewItem item in testTreeView.Items) {
                    if (item is DistributedTestTreeViewItem)
                        distributedTestTreeViewItem = item as DistributedTestTreeViewItem;
                    else if (item is TileStressTestTreeViewItem) {
                        var tstvi = item as TileStressTestTreeViewItem;
                        if (tstvi.TileStressTest == tileStressTest) {
                            tileStressTestTreeViewItem = tstvi;
                            break;
                        }
                    }
                }

                if (tileStressTestTreeViewItem != null) {
                    tileStressTestTreeViewItem.SetStressTestStatus(testProgressMessage.StressTestStatus);

                    //Build and add fast results.
                    if (testProgressMessage.StressTestMetricsCache != null) {
                        tileStressTestTreeViewItem.SetStressTestStarted(testProgressMessage.StartedAt);
                        tileStressTestTreeViewItem.SetEstimatedRunTimeLeft(testProgressMessage.MeasuredRuntime, testProgressMessage.EstimatedRuntimeLeft);

                        //Set the distributed test tree view item
                        distributedTestTreeViewItem.SetStressTestStarted();
                    }

                    if (testProgressMessage.Events == null) tileStressTestTreeViewItem.ClearEvents();
                    else tileStressTestTreeViewItem.SetEvents(testProgressMessage.Events);
                }
            }
        }

        private void testTreeView_ProgressEventClicked(object sender, EventProgressChart.ProgressEventEventArgs e) {
            if (sender == _selectedTestTreeViewItem && _selectedTestTreeViewItem is TileStressTestTreeViewItem) {
                tpStressTest.Select();
                fastResultsControl.ShowEvent(e.ProgressEvent.At);
            }
        }

        private void tmrProgressDelayCountDown_Tick(object sender, EventArgs e) {
            bool setCountDown = true;
            if (_selectedTestTreeViewItem != null && _selectedTestTreeViewItem is TileStressTestTreeViewItem)
                setCountDown = (_selectedTestTreeViewItem as TileStressTestTreeViewItem).StressTestResult == StressTestStatus.Busy;
            if (--_progressCountDown > 0 && setCountDown) fastResultsControl.SetCountDownProgressDelay(_progressCountDown);
        }
        private void tmrProgress_Tick(object sender, EventArgs e) {
            try {
                string lastWarning = distributedTestControl.SetMasterMonitoring(_distributedTestCore.Running, _distributedTestCore.OK, _distributedTestCore.Cancelled, _distributedTestCore.Failed,
                     LocalMonitor.CPUUsage, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.Nic, LocalMonitor.NicBandwidth, LocalMonitor.NicSent,
                     LocalMonitor.NicReceived);

                if (lastWarning.Length != 0) PublishMessage(1, lastWarning);
            }
            catch { } //Exception on false WMI. 
        }
        #endregion

        #region Stop
        private void btnStop_Click(object sender, EventArgs e) {
            StopDistributedTest();
        }
        public void StopDistributedTest() {
            if (!btnStop.Enabled) return;
            Cursor = Cursors.WaitCursor;
            AppendMessages("Stopping the test...");
            bool monitorBeforeRunning = _monitorBeforeCountDown != null;
            bool monitorAfterRunning = _monitorAfterCountDown != null;

            if (_distributedTestCore != null) {
                btnStart.Enabled = btnStop.Enabled = btnSchedule.Enabled = btnWizard.Enabled = false;

                try {
                    _distributedTestCore.Stop();
                }
                catch (Exception ex) {
                    string message = ex.Message + "\n\nSee " + Loggers.GetLogger<FileLogger>().CurrentLogFile;
                    AppendMessages(message, Level.Error);
                }

            }
            else {
                AppendMessages("Test Cancelled!", Level.Warning);

                btnStart.Enabled = btnSchedule.Enabled = btnWizard.Enabled = true;
                StopMonitorsUpdateDetailedResultsAndSetMode(false);
            }

            if (monitorBeforeRunning) {
                testTreeView.SetMonitorBeforeCancelled();
                StopMonitorsUpdateDetailedResultsAndSetMode(false);
            }
            if (monitorAfterRunning) 
                StopMonitorsUpdateDetailedResultsAndSetMode(false);
            
            Cursor = Cursors.Default;
        }
        private void _distributedTestCore_OnFinished(object sender, TestFinishedEventArgs e) {
            _distributedTestCore.OnFinished -= _distributedTestCore_OnFinished;

            Stop(e.Cancelled == 0 && e.Error == 0);

            if (e.Cancelled == 0 && e.Error == 0) {
                AppendMessages("Test finished!", Level.Info);
            }
            else {
                AppendMessages("Test Cancelled!", Level.Warning);
            }
        }

        private void DistributedTestView_FormClosing(object sender, FormClosingEventArgs e) {
            if (_distributedTestMode == DistributedTestMode.Edit ||
                MessageBox.Show("Are you sure you want to close a running test?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                tmrProgress.Stop();
                tmrProgressDelayCountDown.Stop();
                tmrSchedule.Stop();
                tmrRefreshGui.Stop();

                StopMonitorsUpdateDetailedResultsAndSetMode(true);

                if (_distributedTestCore != null)
                    try { _distributedTestCore.Stop(); }
                    catch (Exception ex) {
                        Loggers.Log(Level.Error, "Failed stopping the distributed test core.", ex);
                    }
            }
            else {
                Solution.ExplicitCancelFormClosing = true;
                e.Cancel = true;
            }
        }
        private void Stop(bool monitorAfter = false) {
            Cursor = Cursors.WaitCursor;

            if (_distributedTestCore != null)
                try {
                    _distributedTestCore.Stop();
                    try {
                        string lastWarning = distributedTestControl.SetMasterMonitoring(_distributedTestCore.Running, _distributedTestCore.OK,
                                                                           _distributedTestCore.Cancelled, _distributedTestCore.Failed,
                                                                           LocalMonitor.CPUUsage, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory,
                                                                           LocalMonitor.Nic, LocalMonitor.NicBandwidth,
                                                                           LocalMonitor.NicSent, LocalMonitor.NicReceived);

                        if (lastWarning.Length != 0) PublishMessage(1, lastWarning);
                    }
                    catch { } //Exception on false WMI. 
                }
                catch (Exception ex) {
                    string message = ex.Message + "\n\nSee " + Loggers.GetLogger<FileLogger>().CurrentLogFile;
                    AppendMessages(message, Level.Error);
                    monitorAfter = false;
                }

            Cursor = Cursors.Default;

            if (_monitorViews == null) return;

            int runningMonitors = 0;
            _monitorAfter = 0;
            foreach (TileStressTest ts in _monitorViews.Keys) {
                if (ts.AdvancedTileStressTest.MonitorAfter > _monitorAfter &&
                    ts.BasicTileStressTest.Monitors.Length != 0)
                    _monitorAfter = ts.AdvancedTileStressTest.MonitorAfter;
                foreach (MonitorView view in _monitorViews[ts])
                    if (view != null && !view.IsDisposed)
                        ++runningMonitors;
            }
            if (monitorAfter && _monitorAfter != 0 && runningMonitors != 0) {
                PublishMonitorAfterTestStarted();

                int countdownTime = _monitorAfter * 60000;

                _monitorAfterCountDown = new Countdown();
                _monitorAfterCountDown.Tick += _monitorAfterCountDown_Tick;
                _monitorAfterCountDown.Stopped += monitorAfterCountDown_Stopped;

                _monitorAfterBogusConcurrencyResult = new ConcurrencyResult(-1, -1, 1);
                _monitorAfterBogusRunResult = new RunResult(-1, -1, 0);
                _monitorAfterBogusConcurrencyResult.RunResults.Add(_monitorAfterBogusRunResult);

                try {
                    foreach (var tileStressTest in _monitorMetricsCaches.Keys)
                        foreach (var monitorResultCache in GetMonitorResultCaches(tileStressTest)) {
                            var monitorMetricsCache = _monitorMetricsCaches[tileStressTest];
                            monitorMetricsCache.AddOrUpdate(_monitorAfterBogusConcurrencyResult, monitorResultCache);
                            monitorMetricsCache.AddOrUpdate(_monitorAfterBogusRunResult, monitorResultCache);

                            if (_selectedTestTreeViewItem != null && _selectedTestTreeViewItem is TileStressTestTreeViewItem && (_selectedTestTreeViewItem as TileStressTestTreeViewItem).TileStressTest == tileStressTest) {
                                fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, monitorMetricsCache.GetConcurrencyMetrics(monitorResultCache.Monitor));
                                fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, monitorMetricsCache.GetRunMetrics(monitorResultCache.Monitor));
                            }
                        }
                }
                catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed setting metrics.", ex, new object[] { monitorAfter });
                }

                testTreeView.SetMonitoringBeforeAfter();

                AppendMessages("Monitoring after the test is finished: " + (_monitorAfter * 60) + " s.");
                _monitorAfterCountDown.Start(countdownTime, 5000);
            }
            else {
                StopMonitorsUpdateDetailedResultsAndSetMode(false);                
            }

            this.Focus();
        }
        #endregion

        #region Monitors
        private void ShowAndInitMonitorView(TileStressTest tileStressTest, Monitor.Monitor monitor) {
            //show the monitorview
            MonitorView monitorView;
            if (!MonitorViewAlreadyInited(monitor, out monitorView)) {
                ++_pendingMonitorViewInitializations;

                monitorView = SolutionComponentViewManager.Show(monitor) as MonitorView;
                this.Show();

                AppendMessages("Initializing " + monitorView.Text + "...");
                //For each view initialized, the distributed test view takes care of starting the test.
                monitorView.MonitorInitialized += new EventHandler<MonitorView.MonitorInitializedEventArgs>(monitorView_MonitorInitialized);
                monitorView.OnHandledException += new EventHandler<ErrorEventArgs>(monitorView_OnHandledException);
                monitorView.OnUnhandledException += new EventHandler<ErrorEventArgs>(monitorView_OnUnhandledException);
                monitorView.InitializeForStressTest(tileStressTest.ToString());
            }

            if (!_monitorViews.ContainsKey(tileStressTest))
                _monitorViews.Add(tileStressTest, new List<MonitorView>());
            _monitorViews[tileStressTest].Add(monitorView);

            if (!_monitorMetricsCaches.ContainsKey(tileStressTest))
                _monitorMetricsCaches.Add(tileStressTest, new MonitorMetricsCache());
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

        private void monitorView_OnHandledException(object sender, ErrorEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(
                (state) => {
                    //If the test is not yet started, break it if a monitor fails.
                    if (_pendingMonitorViewInitializations > 0) {
                        btnStop.Enabled = true;
                        if (_distributedTestCore != null) {
                            btnStart.Enabled = btnStop.Enabled = btnSchedule.Enabled = btnWizard.Enabled = false;

                            try {
                                _distributedTestCore.Stop();
                            }
                            catch (Exception ex) {
                                string message = ex.Message + "\n\nSee " + Loggers.GetLogger<FileLogger>().CurrentLogFile;
                                AppendMessages(message, Level.Error);
                            }
                        }
                        else {
                            AppendMessages("Test Failed!", Level.Error);

                            btnStart.Enabled = btnSchedule.Enabled = btnWizard.Enabled = true;
                            StopMonitorsUpdateDetailedResultsAndSetMode(false);
                        }
                        Show();
                    }
                    AppendMessages((sender as MonitorView).Text + ": A counter became unavailable while monitoring:\n" +
                        e.GetException(), _pendingMonitorViewInitializations > 0 ? Level.Error : Level.Warning);
                }, null);
        }

        private void monitorView_OnUnhandledException(object sender, ErrorEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(
                (state) => {
                    //If the test is not yet started, break it if a monitor fails.
                    if (_pendingMonitorViewInitializations > 0) {
                        btnStop.Enabled = true;
                        if (_distributedTestCore != null) {
                            btnStart.Enabled = btnStop.Enabled = btnSchedule.Enabled = btnWizard.Enabled = false;

                            try {
                                _distributedTestCore.Stop();
                            }
                            catch (Exception ex) {
                                string message = ex.Message + "\n\nSee " + Loggers.GetLogger<FileLogger>().CurrentLogFile;
                                AppendMessages(message, Level.Error);
                            }
                        }
                        else {
                            AppendMessages("Test Failed!", Level.Error);

                            btnStart.Enabled = btnSchedule.Enabled = btnWizard.Enabled = true;
                            StopMonitorsUpdateDetailedResultsAndSetMode(false);
                        }
                        Show();
                    }
                    AppendMessages((sender as MonitorView).Text + ": An error has occured while monitoring, monitor stopped!\n" +
                        e.GetException(), Level.Error);
                }, null);
        }

        /// <summary>
        /// Get all monitor result caches for al the running monitors.
        /// </summary>
        /// <returns></returns>
        private List<MonitorResult> GetMonitorResultCaches(TileStressTest tileStressTest) {
            var l = new List<MonitorResult>();
            if (_monitorViews != null)
                foreach (var ts in _monitorViews.Keys)
                    if (tileStressTest == ts)
                        foreach (var view in _monitorViews[ts])
                            l.Add(view.GetMonitorResultCache());
            return l;
        }

        private void monitorBeforeCountDown_Tick(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                try {
                    if (_monitorBeforeBogusConcurrencyResult != null)
                        foreach (var tileStressTest in _monitorMetricsCaches.Keys)
                            foreach (var monitorResultCache in GetMonitorResultCaches(tileStressTest)) {
                                var monitorMetricsCache = _monitorMetricsCaches[tileStressTest];
                                monitorMetricsCache.AddOrUpdate(_monitorBeforeBogusConcurrencyResult, monitorResultCache);
                                monitorMetricsCache.AddOrUpdate(_monitorBeforeBogusRunResult, monitorResultCache);

                                if (_selectedTestTreeViewItem != null && _selectedTestTreeViewItem is TileStressTestTreeViewItem && (_selectedTestTreeViewItem as TileStressTestTreeViewItem).TileStressTest == tileStressTest) {
                                    fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, monitorMetricsCache.GetConcurrencyMetrics(monitorResultCache.Monitor));
                                    fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, monitorMetricsCache.GetRunMetrics(monitorResultCache.Monitor));
                                }
                            }
                }
                catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed setting metrics.", ex, new object[] { sender, e });
                }

                int countdowntime = _monitorBeforeCountDown == null ? 0 : _monitorBeforeCountDown.CountdownTime;
                var ts = new TimeSpan(countdowntime * TimeSpan.TicksPerMillisecond);
                AppendMessages("Monitoring before the test starts: " + ts.ToShortFormattedString(true, "0 s") + ".");

                int runningMonitors = 0;
                foreach (TileStressTest tileStressTest in _monitorViews.Keys)
                    foreach (MonitorView view in _monitorViews[tileStressTest])
                        if (view != null && !view.IsDisposed)
                            runningMonitors++;

                if (runningMonitors == 0) {
                    if (_monitorBeforeCountDown != null) _monitorBeforeCountDown.Stop();
                    AppendMessages("All monitors were manually closed.");
                }
            }, null);
        }

        private void monitorBeforeCountDown_Stopped(object sender, EventArgs e) {
            if (_monitorBeforeCountDown != null) {
                _monitorBeforeCountDown.Dispose();
                _monitorBeforeCountDown = null;
            }

            if (_monitorBeforeBogusConcurrencyResult != null) {
                var stoppedAt = DateTime.Now;
                TimeSpan difference = stoppedAt - _monitorBeforeBogusConcurrencyResult.StartedAt;
                _monitorBeforeBogusConcurrencyResult.StoppedAt = stoppedAt.Subtract(new TimeSpan(difference.Milliseconds * TimeSpan.TicksPerMillisecond));

                difference = stoppedAt - _monitorBeforeBogusRunResult.StartedAt;
                _monitorBeforeBogusRunResult.StoppedAt = stoppedAt.Subtract(new TimeSpan(difference.Milliseconds * TimeSpan.TicksPerMillisecond));
            }

            SynchronizationContextWrapper.SynchronizationContext.Send((state) => MonitorBeforeDone(), null);
            PublishMonitorBeforeTestDone();
        }
        private void MonitorBeforeDone() {
            try {
                tmrProgress.Interval = PROGRESSUPDATEDELAY * 1000;
                tmrProgress.Start();

                tmrProgressDelayCountDown.Start();

                _progressCountDown = PROGRESSUPDATEDELAY - 1;
                _distributedTestCore.Start();
            }
            catch (Exception ex) { HandleInitializeOrStartException(ex); }
        }

        private void _monitorAfterCountDown_Tick(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                try {
                    if (_monitorAfterBogusConcurrencyResult != null)
                        foreach (var tileStressTest in _monitorMetricsCaches.Keys)
                            foreach (var monitorResultCache in GetMonitorResultCaches(tileStressTest)) {
                                var monitorMetricsCache = _monitorMetricsCaches[tileStressTest];
                                monitorMetricsCache.AddOrUpdate(_monitorAfterBogusConcurrencyResult, monitorResultCache);
                                monitorMetricsCache.AddOrUpdate(_monitorAfterBogusRunResult, monitorResultCache);

                                if (_selectedTestTreeViewItem != null && _selectedTestTreeViewItem is TileStressTestTreeViewItem && (_selectedTestTreeViewItem as TileStressTestTreeViewItem).TileStressTest == tileStressTest) {
                                    fastResultsControl.UpdateFastConcurrencyResults(monitorResultCache.Monitor, monitorMetricsCache.GetConcurrencyMetrics(monitorResultCache.Monitor));
                                    fastResultsControl.UpdateFastRunResults(monitorResultCache.Monitor, monitorMetricsCache.GetRunMetrics(monitorResultCache.Monitor));
                                }
                            }
                }
                catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed setting metrics.", ex, new object[] { sender, e });
                }

                var ts = new TimeSpan(_monitorAfterCountDown.CountdownTime * TimeSpan.TicksPerMillisecond);
                AppendMessages("Monitoring after the test is finished: " + ts.ToShortFormattedString(true, "0 s") + ".");

                int runningMonitors = 0;
                foreach (TileStressTest tileStressTest in _monitorViews.Keys)
                    foreach (MonitorView view in _monitorViews[tileStressTest])
                        if (view != null && !view.IsDisposed)
                            ++runningMonitors;

                if (runningMonitors == 0) {
                    _monitorAfterCountDown.Stop();
                    AppendMessages("All monitors were manually closed.");
                }
            }, null);
        }
        private void monitorAfterCountDown_Stopped(object sender, EventArgs e) {
            if (_monitorAfterBogusConcurrencyResult != null) {
                var stoppedAt = DateTime.Now;
                var difference = stoppedAt - _monitorAfterBogusConcurrencyResult.StartedAt;
                _monitorAfterBogusConcurrencyResult.StoppedAt = stoppedAt.Subtract(new TimeSpan((long)(difference.Milliseconds * TimeSpan.TicksPerMillisecond)));

                difference = stoppedAt - _monitorAfterBogusRunResult.StartedAt;
                _monitorAfterBogusRunResult.StoppedAt = stoppedAt.Subtract(new TimeSpan((long)(difference.Milliseconds * TimeSpan.TicksPerMillisecond)));
            }
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                AppendMessages("Finished.");

                StopMonitorsUpdateDetailedResultsAndSetMode(false);
            }, null);

            PublishMonitorAfterTestDone();
        }

        private void WriteMonitorMetricsToDatabase() {
            var validMonitorViews = new List<MonitorView>();
            if (_monitorViews != null)
                foreach (TileStressTest ts in _monitorViews.Keys)
                    foreach (MonitorView view in _monitorViews[ts])
                        if (view != null && !view.IsDisposed && !validMonitorViews.Contains(view))
                            validMonitorViews.Add(view);

            validMonitorViews = null;
        }
        private void StopMonitors() {
            var validMonitorViews = new List<MonitorView>();
            if (_monitorViews != null)
                foreach (TileStressTest ts in _monitorViews.Keys)
                    foreach (MonitorView view in _monitorViews[ts])
                        if (view != null && !view.IsDisposed && view.IsRunning && !validMonitorViews.Contains(view)) {
                            validMonitorViews.Add(view);
                            view.Stop();
                            AppendMessages(view.Text + " is stopped.");
                        }

            validMonitorViews = null;
        }

        /// <summary>
        ///     Only used in stop; this also saves the monitor results if any
        /// </summary>
        private void StopMonitorsUpdateDetailedResultsAndSetMode(bool disposing) {
            if (_monitorBeforeCountDown != null) {
                try { _monitorBeforeCountDown.Dispose(); } catch { } //Don't care.
                _monitorBeforeCountDown = null;
            }
            if (_monitorAfterCountDown != null) {
                try { _monitorAfterCountDown.Dispose(); } catch { } //Don't care.
                _monitorAfterCountDown = null;
            }

            StopMonitors();

            if (!disposing) {
                SetMode(DistributedTestMode.Edit, true);
                this.Focus();
            }
        }
        #endregion

        private void btnDetailedResultsViewer_Click(object sender, EventArgs e) {
            string path = Path.Combine(Application.StartupPath, "DetailedResultsViewer", "vApus.DetailedResultsViewer.exe");
            if (File.Exists(path)) {
                Process.Start(path);
            }
            else {
                string ex = "Detailed results viewer was not found!";
                MessageBox.Show(ex, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Loggers.Log(Level.Error, ex, null, new object[] { sender, e });
            }
        }

        #region Publish
        private string _resultSetId;
        private void PublishConfiguration() {
            if (Publisher.Settings.PublisherEnabled) {
                _resultSetId = Publisher.GenerateResultSetId();
                var publishItem = new DistributedTestConfiguration();
                publishItem.DistributedTest = _distributedTest.ToString();
                publishItem.Description = _distributedTest.Description;
                publishItem.Tags = _distributedTest.Tags;
                publishItem.UseRDP = _distributedTest.UseRDP;
                publishItem.RunSynchronization = _distributedTest.RunSynchronization.ToString();
                publishItem.MaximumRerunsBreakOnLast = _distributedTest.MaxRerunsBreakOnLast;

                var slaveHosts = new HashSet<string>();
                var tileStressTests = new HashSet<string>();
                foreach (Client client in _distributedTest.Clients)
                    foreach (Slave slave in client)
                        if (slave.TileStressTest != null && slave.TileStressTest.Use) {
                            slaveHosts.Add(string.IsNullOrEmpty(client.HostName) ? client.IP : client.HostName);
                            tileStressTests.Add(slave.TileStressTest.ToString());
                        }

                publishItem.SlaveHosts = new string[slaveHosts.Count];
                slaveHosts.CopyTo(publishItem.SlaveHosts);

                publishItem.TileStressTests = new string[tileStressTests.Count];
                tileStressTests.CopyTo(publishItem.TileStressTests);

                Publisher.Send(publishItem, _resultSetId);
            }
        }

        private void PublishClientMonitoring() {
            if (Publisher.Settings.PublisherEnabled) {
                var publishItem = new ClientMonitorMetrics();
                publishItem.Test = _distributedTest.ToString();
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

        private void PublishMonitorBeforeTestStarted() {
            if (Publisher.Settings.PublisherEnabled)
                foreach (TileStressTest ts in _usedMonitors.Keys) {
                    var publishItem = new MonitorEvent();
                    publishItem.Test = ts.ToString();
                    publishItem.Monitor = _usedMonitors[ts].ToString();
                    publishItem.MonitorEventType = (int)MonitorEventType.MonitorBeforeTestStarted;
                    publishItem.Parameters = new KeyValuePair<string, string>[] {
                        new KeyValuePair<string, string>("TimeToMonitorInMinutes", _monitorBefore.ToString())
                    };

                    Publisher.Send(publishItem, _resultSetId);
                }
        }

        private void PublishMonitorBeforeTestDone() {
            if (Publisher.Settings.PublisherEnabled)
                foreach (TileStressTest ts in _usedMonitors.Keys) {
                    var publishItem = new MonitorEvent();
                    publishItem.Test = ts.ToString();
                    publishItem.Monitor = _usedMonitors[ts].ToString();
                    publishItem.MonitorEventType = (int)MonitorEventType.MonitorBeforeTestDone;
                    publishItem.Parameters = new KeyValuePair<string, string>[0];

                    Publisher.Send(publishItem, _resultSetId);
                }
        }
        private void PublishMonitorAfterTestStarted() {
            if (Publisher.Settings.PublisherEnabled)
                foreach (TileStressTest ts in _usedMonitors.Keys) {
                    var publishItem = new MonitorEvent();
                    publishItem.Test = ts.ToString();
                    publishItem.Monitor = _usedMonitors[ts].ToString();
                    publishItem.MonitorEventType = (int)MonitorEventType.MonitorAfterTestStarted;
                    publishItem.Parameters = new KeyValuePair<string, string>[] {
                        new KeyValuePair<string, string>("TimeToMonitorInMinutes", _monitorAfter.ToString())
                    };

                    Publisher.Send(publishItem, _resultSetId);
                }
        }
        private void PublishMonitorAfterTestDone() {
            if (Publisher.Settings.PublisherEnabled)
                foreach (TileStressTest ts in _usedMonitors.Keys) {
                    var publishItem = new MonitorEvent();
                    publishItem.Test = ts.ToString();
                    publishItem.Monitor = _usedMonitors[ts].ToString();
                    publishItem.MonitorEventType = (int)MonitorEventType.MonitorAfterTestDone;
                    publishItem.Parameters = new KeyValuePair<string, string>[0];

                    Publisher.Send(publishItem, _resultSetId);
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">_distributedTest or tile stresstest</param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        private void PublishMessage(int level, string message) {
            if (Publisher.Settings.PublisherEnabled) {
                var publishItem = new TestEvent();
                publishItem.Test = _distributedTest.ToString();
                publishItem.TestEventType = (int)TestEventType.TestMessage;
                publishItem.Parameters = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("Level", level.ToString()),
                    new KeyValuePair<string, string>("Message", message)
                };

                Publisher.Send(publishItem, _resultSetId);
            }
        }
        #endregion
    }
}
