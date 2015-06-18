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
using vApus.JSON;
using vApus.Monitor;
using vApus.Results;
using vApus.Server.Shared;
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

        private ResultsHelper _resultsHelper = new ResultsHelper();

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

        private System.Timers.Timer _refreshDetailedResultsTimer = new System.Timers.Timer(1000);

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
                foreach (Client c in _distributedTest.Clients)
                    foreach (Slave s in c)
                        if (s.TileStressTest != null) ++count;
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
                } else {
                    btnSchedule.Text = string.Empty;
                    btnSchedule.Tag = null;
                }
                _schedule = null;
                btnStart_Click(this, null);
            } else {
                btnSchedule.Text = string.Empty;
            }
            _schedule = null;
        }

        private void tpTree_SelectedIndexChanged(object sender, EventArgs e) {
            if (tcTree.SelectedIndex == 0) {
                configureTileStressTest.Visible = true;
                tileOverview.Visible = _selectedTestTreeViewItem is TileTreeViewItem;
                configureSlaves.Visible = false;
            } else {
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
                distributedStressTestControl.Visible = false;

                string tileStressTestToString = tstvi.TileStressTest.ToString();
                if (!wasSelectedBefore) {

                    fastResultsControl.SetConfigurationControlsAndMonitorLinkButtons(tileStressTestToString, tstvi.TileStressTest.BasicTileStressTest.Connection,
                       tstvi.TileStressTest.BasicTileStressTest.ConnectionProxy, tstvi.TileStressTest.AdvancedTileStressTest.Scenarios, tstvi.TileStressTest.AdvancedTileStressTest.ScenarioRuleSet,
                       tstvi.TileStressTest.BasicTileStressTest.Monitors, tstvi.TileStressTest.AdvancedTileStressTest.Concurrencies, tstvi.TileStressTest.AdvancedTileStressTest.Runs,
                       tstvi.TileStressTest.AdvancedTileStressTest.MinimumDelay, tstvi.TileStressTest.AdvancedTileStressTest.MaximumDelay, tstvi.TileStressTest.AdvancedTileStressTest.Shuffle,
                       tstvi.TileStressTest.AdvancedTileStressTest.ActionDistribution, tstvi.TileStressTest.AdvancedTileStressTest.MaximumNumberOfUserActions, tstvi.TileStressTest.AdvancedTileStressTest.MonitorBefore, tstvi.TileStressTest.AdvancedTileStressTest.MonitorAfter);
                    fastResultsControl.ClearEvents();

                    if (_distributedTestCore != null) {
                        fastResultsControl.ClearFastResults();

                        var testProgressMessage = _distributedTestCore.GetTestProgressMessage(tstvi.TileStressTest);
                        if (testProgressMessage.TileStressTestIndex == null) {
                            detailedResultsControl.ClearResults();
                            detailedResultsControl.Enabled = false;
                        } else {
                            SetSlaveProgress(tstvi.TileStressTest, testProgressMessage);
                        }
                    }
                }
            } else if (sender is TileTreeViewItem) {
                tileOverview.Init((sender as TileTreeViewItem).Tile);
                tileOverview.BringToFront();
                configureTileStressTest.ClearTileStressTest(false);

                fastResultsControl.Visible = false;
                distributedStressTestControl.Visible = true;

                detailedResultsControl.ClearResults();
                detailedResultsControl.Enabled = false;

                SetOverallProgress();
            } else if (sender is DistributedTestTreeViewItem) {
                tileOverview.Init(_distributedTest);
                tileOverview.BringToFront();

                bool showDescriptions = false;
                //if (sender is DistributedTestTreeViewItem) {
                //    var dttvi = sender as DistributedTestTreeViewItem;
                //    foreach (Control ctrl in dttvi.Controls)
                //        if ((ctrl is CheckBox && ctrl.Focused) || (ctrl is Panel && ctrl.Controls.Count != 0 && ctrl.Controls[0].Focused)) {
                //            showDescriptions = true;
                //            break;
                //        }
                //}
                configureTileStressTest.ClearTileStressTest(showDescriptions);

                fastResultsControl.Visible = false;
                distributedStressTestControl.Visible = true;

                detailedResultsControl.ClearResults();
                detailedResultsControl.Enabled = false;

                SetOverallProgress();
            }

            //Update the detailed results in the gui if any.
            RefreshDetailedResults();
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
            } else {
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
                //tcTree.SelectedTab = tpTests;
                fastResultsControl.SetEventFilter(EventViewEventType.Warning);
            } else {
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
            distributedStressTestControl.AppendMessages("Testing the tile stress test connections...");

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
                    distributedStressTestControl.AppendMessages("The master cannot connect to " + e.Connection + ". It is likely that the slave won't be able to connect also!\nThe test will continue regardlessly.\nDetails: " + e.Message, Level.Warning);
            }, null);
        }

        /// <summary>
        ///     True on success or if user said there can be proceed without database.
        /// </summary>
        /// <returns></returns>
        private bool InitDatabaseBeforeStart(bool autoConfirmDialog) {
            var dialog = new DescriptionAndTagsInputDialog { Description = _distributedTest.Description, Tags = _distributedTest.Tags, ResultsHelper = _resultsHelper, AutoConfirm = autoConfirmDialog };
            if (dialog.ShowDialog() == DialogResult.Cancel) {
                RemoveDatabase(false);
                return false;
            }

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
            } else {
                TimeSpan dt = scheduledAt - DateTime.Now;
                if (dt.Milliseconds != 0) {
                    dt = new TimeSpan(dt.Ticks - (dt.Ticks % TimeSpan.TicksPerSecond));
                    dt += new TimeSpan(0, 0, 1);
                }
                btnSchedule.Text = "Scheduled in " + dt.ToLongFormattedString();
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

                distributedStressTestControl.ClearFastResults();

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
                        RemoveDatabase(false);
                        Stop();
                        return;
                    } else if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.UpToDate) {
                        distributedStressTestControl.AppendMessages("Updating the slaves, this can take a while...");
                        var exceptions = await JumpStart.SmartUpdate(_distributedTest);

                        if (exceptions.Length != 0) {
                            string message = "Failed to update one or more slaves.\n";
                            foreach (Exception ex in exceptions)
                                message += ex.Message + "\n";

                            message = message.Trim();
                            distributedStressTestControl.AppendMessages(message, Level.Error);

                            throw new Exception(message);
                        }
                    }
                }

                distributedStressTestControl.AppendMessages("Jump Starting the slaves...");
                try {
                    //Jumpstart the slaves first, an event will be fired when this is done and the test will start
                    JumpStart.Done += JumpStart_Done;
                    JumpStart.Do(_distributedTest);
                } catch {
                    //Only one test can run at the same time.
                    JumpStart.Done -= JumpStart_Done;

                    distributedStressTestControl.AppendMessages("Failed to Jump Start one or more slaves.", Level.Error);
                    throw;
                }

            } catch (Exception ex) {
                RemoveDatabase(false);
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
            } else {
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
            distributedStressTestControl.AppendMessages("Opening remote desktop connection(s) to the client(s)...");

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

                        _distributedTestCore = new DistributedTestCore(_distributedTest, _resultsHelper);
                        _distributedTestCore.Message += _distributedTestCore_Message;
                        _distributedTestCore.OnTestProgressMessageReceivedDelayed += _distributedTestCore_TestProgressMessageReceivedDelayed;
                        _distributedTestCore.OnListeningError += _distributedTestCore_OnListeningError;
                        _distributedTestCore.OnFinished += _distributedTestCore_OnFinished;


                        var t = new Thread(InitializeAndStartTest);
                        t.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                        t.IsBackground = true;
                        t.Start();
                    } else {
                        //Failed jump starting slaves
                        foreach (Exception ex in e.Exceptions) {
                            string message = ex.Message + "\n\nSee " + Loggers.GetLogger<FileLogger>().CurrentLogFile;
                            distributedStressTestControl.AppendMessages(message, Level.Error);
                            Loggers.Log(Level.Error, message, ex);
                        }

                        RemoveDatabase();
                        Stop();
                    }
                } catch (Exception ex) {
                    //Only one test can run at the same time.
                    string message = "Cannot start this test because another one is still running.";
                    distributedStressTestControl.AppendMessages(message, Level.Error);
                    Loggers.Log(Level.Error, message, ex);
                    RemoveDatabase();
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
                        distributedStressTestControl.AppendMessages("The averages in the fast results for one or more tile stress tests will NOT be correct because one or more given concurrencies divided by the number of slaves is not an integer!" +
                            "Please use the detailed results.\nSee the log for more details.", Level.Warning);

                    fastResultsControl.SetStressTestInitialized();

                    //Initialize the monitors.
                    _monitorViews.Clear();
                    _monitorMetricsCaches.Clear();
                    detailedResultsControl.ClearResults();
                    detailedResultsControl.Enabled = false;

                    foreach (TileStressTest tileStressTest in _distributedTest.UsedTileStressTests)
                        foreach (var monitor in tileStressTest.BasicTileStressTest.Monitors)
                            ShowAndInitMonitorView(tileStressTest, monitor);
                }, null);

                if (_pendingMonitorViewInitializations != 0) _monitorViewsInitializedWaitHandle.WaitOne();
            } catch (Exception ex) {
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
                    try { LocalMonitor.StartMonitoring(PROGRESSUPDATEDELAY * 1000); } catch { fastResultsControl.AddEvent("Could not initialize the local monitor, something is wrong with your WMI service.", Level.Error); }

                    if (_monitorViews != null) {
                        int runningMonitors = 0;
                        int monitorBefore = 0;
                        foreach (TileStressTest ts in _monitorViews.Keys) {
                            if (ts.AdvancedTileStressTest.MonitorBefore > monitorBefore && ts.BasicTileStressTest.Monitors.Length != 0)
                                monitorBefore = ts.AdvancedTileStressTest.MonitorBefore;

                            foreach (MonitorView monitorView in _monitorViews[ts])
                                if (monitorView != null && !monitorView.IsDisposed)
                                    try {
                                        if (monitorView.Start()) {
                                            int dbId = _distributedTestCore.GetDbId(ts);
                                            if (dbId != -1)
                                                monitorView.GetMonitorResultCache().MonitorConfigurationId = _resultsHelper.SetMonitor(dbId, monitorView.Monitor.ToString(), monitorView.Monitor.MonitorSource.ToString(),
                                                monitorView.GetConnectionString(), monitorView.Configuration, monitorView.GetMonitorResultCache().Headers);

                                            distributedStressTestControl.AppendMessages(monitorView.Text + " is started.");
                                            ++runningMonitors;
                                        } else {
                                            try { monitorView.Stop(); } catch { }
                                        }
                                    } catch (Exception e) {
                                        Loggers.Log(Level.Error, monitorView.Text + " is not started.", e);
                                        distributedStressTestControl.AppendMessages(monitorView.Text + " is not started.");

                                        try {
                                            monitorView.Stop();
                                        } catch {
                                            //Dont't care at this point.
                                        }
                                    }
                        }

                        if (runningMonitors != 0 && monitorBefore != 0) {
                            int countdownTime = monitorBefore * 60000;
                            _monitorBeforeCountDown = new Countdown(countdownTime, 5000);
                            _monitorBeforeCountDown.Tick += monitorBeforeCountDown_Tick;
                            _monitorBeforeCountDown.Stopped += monitorBeforeCountDown_Stopped;

                            _monitorBeforeBogusConcurrencyResult = new ConcurrencyResult(-1, 1);
                            _monitorBeforeBogusRunResult = new RunResult(-1, 0);
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
                            } catch (Exception ex) {
                                Loggers.Log(Level.Error, "Failed updating metrics.", ex);
                            }

                            testTreeView.SetMonitoringBeforeAfter();
                            distributedStressTestControl.AppendMessages("Monitoring before the test starts: " + (monitorBefore * 60) + " s.");
                            _monitorBeforeCountDown.Start();
                        } else {
                            MonitorBeforeDone();
                        }
                    } else {
                        MonitorBeforeDone();
                    }

                    Cursor = Cursors.Default;
                }, null);
            } catch (Exception ex) { HandleInitializeOrStartException(ex); }
        }

        private void HandleInitializeOrStartException(Exception exception) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                string message = exception.Message + "\n\nSee " + Loggers.GetLogger<FileLogger>().CurrentLogFile;
                distributedStressTestControl.AppendMessages(message, Level.Error);
                if (_distributedTestCore != null && !_distributedTestCore.IsDisposed) {
                    _distributedTestCore.Dispose();
                    _distributedTestCore = null;
                }

                RemoveDatabase();
                Stop();
                Cursor = Cursors.Default;
            }, null);
        }
        #endregion

        #region Progress
        private void _distributedTestCore_Message(object sender, MessageEventArgs e) { distributedStressTestControl.AppendMessages(e.Message); }

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
                            var l = testProgressMessage.StressTestMetricsCache.GetRunMetrics();
                            var runMetrics = l[l.Count - 1];
                            string message = string.Concat(tileStressTest.ToString(), " - Run ", runMetrics.Run, " of concurrency ", runMetrics.Concurrency, " finished.");
                            TestProgressNotifier.Notify(TestProgressNotifier.What.RunFinished, message);
                        } else if (testProgressMessage.ConcurrencyFinished) {
                            var l = testProgressMessage.StressTestMetricsCache.GetConcurrencyMetrics();
                            var concurrencyMetrics = l[l.Count - 1];
                            string message = string.Concat(tileStressTest.ToString(), " - Concurrency ", concurrencyMetrics.Concurrency, " finished.");
                            TestProgressNotifier.Notify(TestProgressNotifier.What.ConcurrencyFinished, message);
                        }
                    } else {
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
                    foreach (var concurrencyMetrics in testProgressMessage.StressTestMetricsCache.GetConcurrencyMetrics())
                        monitorMetricsCache.Add(MonitorMetricsHelper.GetConcurrencyMetrics(monitorResultCache.Monitor, concurrencyMetrics, monitorResultCache));

                    if (_monitorAfterCountDown == null && _monitorAfterBogusConcurrencyResult != null)
                        monitorMetricsCache.Add(MonitorMetricsHelper.GetMetrics(_monitorAfterBogusConcurrencyResult, monitorResultCache));


                    if (_monitorBeforeBogusRunResult != null)
                        monitorMetricsCache.Add(MonitorMetricsHelper.GetMetrics(_monitorBeforeBogusRunResult, monitorResultCache));
                    foreach (var runMetrics in testProgressMessage.StressTestMetricsCache.GetRunMetrics())
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
                    distributedStressTestControl.SetTitle("Distributed Test");
                    if (_distributedTestCore != null && !_distributedTestCore.IsDisposed) {
                        testProgressMessages = _distributedTestCore.GetAllTestProgressMessages();
                        var progress = new Dictionary<TileStressTest, FastStressTestMetricsCache>(testProgressMessages.Count);
                        foreach (TileStressTest tileStressTest in testProgressMessages.Keys) {
                            var metricsCache = testProgressMessages[tileStressTest].StressTestMetricsCache;
                            if (metricsCache != null) progress.Add(tileStressTest, metricsCache);
                        }

                        distributedStressTestControl.SetOverallFastResults(progress);
                    }
                } else if (_selectedTestTreeViewItem is TileTreeViewItem) {
                    var ttvi = _selectedTestTreeViewItem as TileTreeViewItem;
                    distributedStressTestControl.SetTitle(ttvi.Tile.Name + " " + ttvi.Tile.Index);
                    if (_distributedTestCore != null && !_distributedTestCore.IsDisposed) {
                        testProgressMessages = _distributedTestCore.GetAllTestProgressMessages();
                        var progress = new Dictionary<TileStressTest, FastStressTestMetricsCache>();
                        foreach (TileStressTest tileStressTest in testProgressMessages.Keys)
                            if (ttvi.Tile.Contains(tileStressTest)) {
                                var metricsCache = testProgressMessages[tileStressTest].StressTestMetricsCache;
                                if (metricsCache != null) progress.Add(tileStressTest, metricsCache);
                            }

                        distributedStressTestControl.SetOverallFastResults(progress);
                    }
                }
                //#warning Enabled REST
                //  WriteRestProgress(testProgressMessages);
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
                    fastResultsControl.UpdateFastConcurrencyResults(testProgressMessage.StressTestMetricsCache.GetConcurrencyMetrics(), true, testProgressMessage.StressTestMetricsCache.SimplifiedMetrics);
                    fastResultsControl.UpdateFastRunResults(testProgressMessage.StressTestMetricsCache.GetRunMetrics(), false, testProgressMessage.StressTestMetricsCache.SimplifiedMetrics);
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
                        fastResultsControl.SetStressTestStopped(testProgressMessage.StressTestStatus, testProgressMessage.MeasuredRuntime);
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

            //#warning Enabled REST
            //            WriteMonitorRestProgress();
        }
        private void tmrProgress_Tick(object sender, EventArgs e) {
            try {
                distributedStressTestControl.SetMasterMonitoring(_distributedTestCore.Running, _distributedTestCore.OK, _distributedTestCore.Cancelled, _distributedTestCore.Failed,
                    LocalMonitor.CPUUsage, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.Nic, LocalMonitor.NicBandwidth, LocalMonitor.NicSent,
                    LocalMonitor.NicReceived);
            } catch { } //Exception on false WMI. 
        }
        #endregion

        #region Stop
        private void btnStop_Click(object sender, EventArgs e) {
            StopDistributedTest();
        }
        public void StopDistributedTest() {
            if (!btnStop.Enabled) return;
            Cursor = Cursors.WaitCursor;
            distributedStressTestControl.AppendMessages("Stopping the test...");
            bool monitorBeforeRunning = _monitorBeforeCountDown != null;
            bool monitorAfterRunning = _monitorAfterCountDown != null;

            if (_distributedTestCore != null) {
                btnStart.Enabled = btnStop.Enabled = btnSchedule.Enabled = btnWizard.Enabled = false;

                try {
                    _distributedTestCore.Stop();
                } catch (Exception ex) {
                    string message = ex.Message + "\n\nSee " + Loggers.GetLogger<FileLogger>().CurrentLogFile;
                    distributedStressTestControl.AppendMessages(message, Level.Error);
                }

            } else {
                distributedStressTestControl.AppendMessages("Test Cancelled!", Level.Warning);

                btnStart.Enabled = btnSchedule.Enabled = btnWizard.Enabled = true;
                StopMonitorsUpdateDetailedResultsAndSetMode(false);
                RemoveDatabase();
            }

            if (monitorBeforeRunning) {
                testTreeView.SetMonitorBeforeCancelled();
                StopMonitorsUpdateDetailedResultsAndSetMode(false);
                RemoveDatabase();
            }
            if (monitorAfterRunning) {
                StopMonitorsUpdateDetailedResultsAndSetMode(false);
                RemoveDatabase();
            }

            Cursor = Cursors.Default;
        }
        private void _distributedTestCore_OnFinished(object sender, TestFinishedEventArgs e) {
            _distributedTestCore.OnFinished -= _distributedTestCore_OnFinished;

            Stop(e.Cancelled == 0 && e.Error == 0);

            if (e.Cancelled == 0 && e.Error == 0) {
                distributedStressTestControl.AppendMessages("Test finished!", Level.Info);
            } else {
                distributedStressTestControl.AppendMessages("Test Cancelled!", Level.Warning);
                RemoveDatabase();
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
                    try { _distributedTestCore.Stop(); } catch (Exception ex) {
                        Loggers.Log(Level.Error, "Failed stopping the distributed test core.", ex);
                    }
            } else {
                Solution.ExplicitCancelFormClosing = true;
                e.Cancel = true;
            }
        }
        private void RemoveDatabase(bool confirm = true) {
            if (_resultsHelper != null && _resultsHelper.DatabaseName != null)
                if (!confirm || MessageBox.Show("Do you want to remove the results database?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
                    == DialogResult.Yes)
                    try {
                        _resultsHelper.DeleteResults();
                    } catch (Exception ex) {
                        Loggers.Log(Level.Error, "Failed removing the results database.", ex, new object[] { confirm });
                    }
        }
        private void Stop(bool monitorAfter = false) {
            Cursor = Cursors.WaitCursor;

            if (_distributedTestCore != null)
                try {
                    _distributedTestCore.Stop();
                    try {
                        distributedStressTestControl.SetMasterMonitoring(_distributedTestCore.Running, _distributedTestCore.OK,
                                                                         _distributedTestCore.Cancelled, _distributedTestCore.Failed,
                                                                         LocalMonitor.CPUUsage, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory,
                                                                         LocalMonitor.Nic, LocalMonitor.NicBandwidth,
                                                                         LocalMonitor.NicSent, LocalMonitor.NicReceived);
                    } catch { } //Exception on false WMI. 
                } catch (Exception ex) {
                    string message = ex.Message + "\n\nSee " + Loggers.GetLogger<FileLogger>().CurrentLogFile;
                    distributedStressTestControl.AppendMessages(message, Level.Error);
                    monitorAfter = false;
                }

            Cursor = Cursors.Default;

            if (_monitorViews == null) return;

            int runningMonitors = 0;
            int monitorAfterTime = 0;
            foreach (TileStressTest ts in _monitorViews.Keys) {
                if (ts.AdvancedTileStressTest.MonitorAfter > monitorAfterTime &&
                    ts.BasicTileStressTest.Monitors.Length != 0)
                    monitorAfterTime = ts.AdvancedTileStressTest.MonitorAfter;
                foreach (MonitorView view in _monitorViews[ts])
                    if (view != null && !view.IsDisposed)
                        ++runningMonitors;
            }
            if (monitorAfter && monitorAfterTime != 0 && runningMonitors != 0) {
                int countdownTime = monitorAfterTime * 60000;
                _monitorAfterCountDown = new Countdown(countdownTime, 5000);
                _monitorAfterCountDown.Tick += _monitorAfterCountDown_Tick;
                _monitorAfterCountDown.Stopped += monitorAfterCountDown_Stopped;

                _monitorAfterBogusConcurrencyResult = new ConcurrencyResult(-1, 1);
                _monitorAfterBogusRunResult = new RunResult(-1, 0);
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
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed setting metrics.", ex, new object[] { monitorAfter });
                }

                testTreeView.SetMonitoringBeforeAfter();
                distributedStressTestControl.AppendMessages("Monitoring after the test is finished: " + (monitorAfterTime * 60) + " s.");
                _monitorAfterCountDown.Start();
            } else {
                StopMonitorsUpdateDetailedResultsAndSetMode(false);

                if (AutoExportResultsManager.Enabled)
                    detailedResultsControl.AutoExportToExcel(AutoExportResultsManager.Folder);

            }

            this.Focus();
        }

        private void RefreshDetailedResults() {
            detailedResultsControl.Enabled = false;

            if (_refreshDetailedResultsTimer != null) {
                _refreshDetailedResultsTimer.Stop();
                _refreshDetailedResultsTimer.Elapsed -= _rowEnterTimer_Elapsed;

                _refreshDetailedResultsTimer.Elapsed += _rowEnterTimer_Elapsed;
                _refreshDetailedResultsTimer.Start();
            }
        }

        private void _rowEnterTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if (_refreshDetailedResultsTimer != null) {
                _refreshDetailedResultsTimer.Stop();
                _refreshDetailedResultsTimer.Elapsed -= _rowEnterTimer_Elapsed;
                try {
                    RefreshDetailedResultsDelayed();
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed refreshing detailed results.", ex, new object[] { sender, e });
                }
            }
        }
        private void RefreshDetailedResultsDelayed() {
            SynchronizationContextWrapper.SynchronizationContext.Send((y) => {
                int[] stressTestIds = null;
                if (_resultsHelper != null && _resultsHelper.DatabaseName != null && _distributedTestMode == DistributedTestMode.Edit && _selectedTestTreeViewItem != null) {
                    if (_selectedTestTreeViewItem is TileStressTestTreeViewItem) {
                        var tstvi = _selectedTestTreeViewItem as TileStressTestTreeViewItem;
                        int dbId = _distributedTestCore.GetDbId(tstvi.TileStressTest);
                        if (dbId != -1)
                            stressTestIds = new int[] { dbId };
                    } else if (_selectedTestTreeViewItem is TileTreeViewItem) {
                        var l = new List<int>();
                        var tile = (_selectedTestTreeViewItem as TileTreeViewItem).Tile;
                        foreach (TileStressTest ts in tile)
                            if (ts.Use) {
                                int dbId = _distributedTestCore.GetDbId(ts);
                                if (dbId != -1)
                                    l.Add(dbId);
                            }
                        stressTestIds = l.ToArray();
                    } else if (_selectedTestTreeViewItem is DistributedTestTreeViewItem) {
                        stressTestIds = new int[0];
                    }
                }
                if (stressTestIds != null) {
                    this.Enabled = false;
                    detailedResultsControl.RefreshResults(_resultsHelper, stressTestIds);
                    this.Enabled = true;
                }
                this.Show();
            }, null);
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

                distributedStressTestControl.AppendMessages("Initializing " + monitorView.Text + "...");
                //For each view initialized, the distributed test view takes care of starting the test.
                monitorView.MonitorInitialized += new EventHandler<MonitorView.MonitorInitializedEventArgs>(monitorView_MonitorInitialized);
                monitorView.OnHandledException += new EventHandler<ErrorEventArgs>(monitorView_OnHandledException);
                monitorView.OnUnhandledException += new EventHandler<ErrorEventArgs>(monitorView_OnUnhandledException);
                monitorView.InitializeForStressTest();
            }

            if (!_monitorViews.ContainsKey(tileStressTest))
                _monitorViews.Add(tileStressTest, new List<MonitorView>());
            _monitorViews[tileStressTest].Add(monitorView);

            if (!_monitorMetricsCaches.ContainsKey(tileStressTest))
                _monitorMetricsCaches.Add(tileStressTest, new MonitorMetricsCache());

            //#warning Enabled REST
            //            WriteMonitorRestConfig();
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
                            } catch (Exception ex) {
                                string message = ex.Message + "\n\nSee " + Loggers.GetLogger<FileLogger>().CurrentLogFile;
                                distributedStressTestControl.AppendMessages(message, Level.Error);
                            }
                        } else {
                            distributedStressTestControl.AppendMessages("Test Failed!", Level.Error);

                            btnStart.Enabled = btnSchedule.Enabled = btnWizard.Enabled = true;
                            StopMonitorsUpdateDetailedResultsAndSetMode(false);
                            RemoveDatabase();
                        }
                        Show();
                    }
                    distributedStressTestControl.AppendMessages((sender as MonitorView).Text + ": A counter became unavailable while monitoring:\n" +
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
                            } catch (Exception ex) {
                                string message = ex.Message + "\n\nSee " + Loggers.GetLogger<FileLogger>().CurrentLogFile;
                                distributedStressTestControl.AppendMessages(message, Level.Error);
                            }
                        } else {
                            distributedStressTestControl.AppendMessages("Test Failed!", Level.Error);

                            btnStart.Enabled = btnSchedule.Enabled = btnWizard.Enabled = true;
                            StopMonitorsUpdateDetailedResultsAndSetMode(false);
                            RemoveDatabase();
                        }
                        Show();
                    }
                    distributedStressTestControl.AppendMessages((sender as MonitorView).Text + ": An error has occured while monitoring, monitor stopped!\n" +
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
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed setting metrics.", ex, new object[] { sender, e });
                }

                int countdowntime = _monitorBeforeCountDown == null ? 0 : _monitorBeforeCountDown.CountdownTime;
                var ts = new TimeSpan(countdowntime * TimeSpan.TicksPerMillisecond);
                distributedStressTestControl.AppendMessages("Monitoring before the test starts: " + ts.ToShortFormattedString("0 s") + ".");

                int runningMonitors = 0;
                foreach (TileStressTest tileStressTest in _monitorViews.Keys)
                    foreach (MonitorView view in _monitorViews[tileStressTest])
                        if (view != null && !view.IsDisposed)
                            runningMonitors++;

                if (runningMonitors == 0) {
                    if (_monitorBeforeCountDown != null) _monitorBeforeCountDown.Stop();
                    distributedStressTestControl.AppendMessages("All monitors were manually closed.");
                }

                //#warning Enabled REST
                //                WriteMonitorRestConfig();
                //                WriteMonitorRestProgress();
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

            //#warning Enabled REST
            //            WriteMonitorRestConfig();
            //            WriteMonitorRestProgress();

            SynchronizationContextWrapper.SynchronizationContext.Send((state) => MonitorBeforeDone(), null);
        }
        private void MonitorBeforeDone() {
            try {
                tmrProgress.Interval = PROGRESSUPDATEDELAY * 1000;
                tmrProgress.Start();

                tmrProgressDelayCountDown.Start();

                _progressCountDown = PROGRESSUPDATEDELAY - 1;
                _distributedTestCore.Start();
            } catch (Exception ex) { HandleInitializeOrStartException(ex); }
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
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed setting metrics.", ex, new object[] { sender, e });
                }

                var ts = new TimeSpan(_monitorAfterCountDown.CountdownTime * TimeSpan.TicksPerMillisecond);
                distributedStressTestControl.AppendMessages("Monitoring after the test is finished: " + ts.ToShortFormattedString("0 s") + ".");

                int runningMonitors = 0;
                foreach (TileStressTest tileStressTest in _monitorViews.Keys)
                    foreach (MonitorView view in _monitorViews[tileStressTest])
                        if (view != null && !view.IsDisposed)
                            ++runningMonitors;

                if (runningMonitors == 0) {
                    _monitorAfterCountDown.Stop();
                    distributedStressTestControl.AppendMessages("All monitors were manually closed.");
                }

                //#warning Enabled REST
                //                WriteMonitorRestConfig();
                //                WriteMonitorRestProgress();
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
                StopMonitorsUpdateDetailedResultsAndSetMode(false);

                if (AutoExportResultsManager.Enabled)
                    detailedResultsControl.AutoExportToExcel(AutoExportResultsManager.Folder);


                fastResultsControl.AddEvent("Finished.");
            }, null);

            //#warning Enabled REST
            //            WriteMonitorRestConfig();
            //            WriteMonitorRestProgress();
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

            var validMonitorViews = new List<MonitorView>();
            if (_monitorViews != null)
                foreach (TileStressTest ts in _monitorViews.Keys)
                    foreach (MonitorView view in _monitorViews[ts])
                        if (view != null && !view.IsDisposed && view.IsRunning && !validMonitorViews.Contains(view)) {
                            validMonitorViews.Add(view);
                            view.Stop();
                            distributedStressTestControl.AppendMessages(view.Text + " is stopped.");
                        }
            foreach (MonitorView view in validMonitorViews)
                try { _resultsHelper.SetMonitorResults(view.GetMonitorResultCache()); } catch (Exception e) {
                    Loggers.Log(Level.Error, view.Text + ": Failed adding results to the database.", e);
                }

            validMonitorViews = null;

            if (!disposing) {
                SetMode(DistributedTestMode.Edit, true);

                //Update the detailed results in the gui if any.
                RefreshDetailedResults();

                this.Focus();
            }
        }
        #endregion

        #region REST
        private void WriteMonitorRestConfig() {
            try {
                var monitorConfigCache = new JSONObjectTree();
                var distributedTestCache = JSONObjectTreeHelper.AddSubCache(_distributedTest.ToString(), monitorConfigCache);

                foreach (TileStressTest key in _monitorViews.Keys)
                    foreach (MonitorView view in _monitorViews[key]) {
                        var monitorSource = view.Monitor.MonitorSource == null ? "N/A" : view.Monitor.MonitorSource.ToString();
                        var parameters = view.Monitor.ParameterValues;

                        JSONObjectTreeHelper.ApplyToRunningMonitorConfig(distributedTestCache, view.Monitor.ToString(), monitorSource, parameters);
                    }

                JSONObjectTreeHelper.RunningMonitorConfig = monitorConfigCache;
                JSONObjectTreeHelper.WriteToFile(monitorConfigCache, "MonitorConfig");
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed writing REST.", ex);
            }
        }
        private void WriteMonitorRestProgress() {
            try {
                var monitorProgressCache = new JSONObjectTree();
                var distributedTestCache = JSONObjectTreeHelper.AddSubCache(_distributedTest.ToString(), monitorProgressCache);

                int monitorCount = 0;

                if (_monitorViews != null)
                    foreach (TileStressTest key in _monitorViews.Keys)
                        foreach (MonitorView view in _monitorViews[key]) {
                            ++monitorCount;
                            JSONObjectTreeHelper.ApplyToRunningMonitorMetrics(distributedTestCache, view.Monitor.ToString(), view.GetMonitorResultCache().Headers, view.GetMonitorValues());
                        }
                if (monitorCount != 0) {
                    JSONObjectTreeHelper.RunningMonitorMetrics = monitorProgressCache;
                    JSONObjectTreeHelper.WriteToFile(monitorProgressCache, "MonitorProgress");
                }
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed writing REST.", ex);
            }
        }
        private void WriteRestProgress(Dictionary<TileStressTest, TestProgressMessage> testProgressMessages) {
            try {
                var testProgressCache = new JSONObjectTree();
                //var clientMonitorCache = new JSONObjectTree();
                //var messagesCache = new JSONObjectTree();

                var distributedTestProgressCache = JSONObjectTreeHelper.AddSubCache(_distributedTest.ToString(), testProgressCache);

                if (_distributedTestCore != null && !_distributedTestCore.IsDisposed) {
                    //JSONObjectTreeHelper.ApplyToRunningTestClientMonitorMetrics(clientMonitorCache, _distributedTest.ToString(), -1, LocalMonitor.CPUUsage,
                    //                    LocalMonitor.MemoryUsage, LocalMonitor.TotalVisibleMemory, LocalMonitor.Nic, LocalMonitor.NicSent, LocalMonitor.NicReceived);

                    //var events = distributedStressTestControl.GetEvents();
                    //var messages = new string[events.Count];
                    //for (int i = 0; i != messages.Length; i++) {
                    //    var e = events[i];
                    //    messages[i] = e.EventType + ": " + e.Message + " [" + e.At + "]";
                    //}
                    //JSONObjectTreeHelper.ApplyToRunningTestMessages(messagesCache, _distributedTest.ToString(), messages);

                    foreach (TileStressTest tileStressTest in testProgressMessages.Keys) {
                        string ts = "Tile " + (tileStressTest.Parent as Tile).Index + " stress test " +
                                                      tileStressTest.Index + " " + tileStressTest.BasicTileStressTest.Connection.Label;
                        var tileStressTestProgressCache = JSONObjectTreeHelper.AddSubCache(ts, distributedTestProgressCache);

                        var tpm = testProgressMessages[tileStressTest];
                        if (tpm.StressTestMetricsCache != null)
                            foreach (var metrics in tpm.StressTestMetricsCache.GetConcurrencyMetrics())
                                JSONObjectTreeHelper.ApplyToRunningTestFastConcurrencyResults(tileStressTestProgressCache, metrics, tpm.RunStateChange.ToString(), tpm.StressTestStatus.ToString());


                        //JSONObjectTreeHelper.ApplyToRunningTestClientMonitorMetrics(clientMonitorCache, ts, tpm.ThreadsInUse, tpm.CPUUsage,
                        //                    tpm.MemoryUsage, tpm.TotalVisibleMemory, tpm.Nic, tpm.NicSent, tpm.NicReceived);

                        //messages = new string[tpm.Events.Count];
                        //for (int i = 0; i != messages.Length; i++) {
                        //    var e = tpm.Events[i];
                        //    messages[i] = e.EventType + ": " + e.Message + " [" + e.At + "]";
                        //}
                        //JSONObjectTreeHelper.ApplyToRunningTestMessages(messagesCache, ts, messages);
                    }
                }
                JSONObjectTreeHelper.RunningTestFastConcurrencyResults = testProgressCache;
                //JSONObjectTreeHelper.RunningTestClientMonitorMetrics = clientMonitorCache;
                //JSONObjectTreeHelper.RunningTestMessages = messagesCache;
                JSONObjectTreeHelper.WriteToFile(testProgressCache, "TestProgress");
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed writing REST.", ex);
            }
        }
        #endregion
    }
}
