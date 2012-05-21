/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using vApus.Monitor;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public partial class DistributedTestView : BaseSolutionComponentView
    {
        #region Fields
        private TileStresstestSelectorControl _focussedTileStresstestSelectorControl = null;

        private DistributedTest _distributedTest;
        private DistributedTestCore _distributedTestCore;

        private LinkageOverview _linkageOverview;

        private string _resultsPath;

        //Progress
        private int _countDown;

        /// <summary>
        /// The test can only start when this == 0.
        /// </summary>
        private int _pendingMonitorViewInitializations;

        private bool suscribedToTvwChecked = true;
        #endregion

        #region Constructor
        public DistributedTestView()
        {
            InitializeComponent();
        }
        public DistributedTestView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            Solution.ActiveSolution.RegisterForCancelFormClosing(this);
            _distributedTest = solutionComponent as DistributedTest;

            InitializeComponent();

            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(DistributedTestView_HandleCreated);
        }
        #endregion

        #region Functions
        private void DistributedTestView_Shown(object sender, EventArgs e)
        {
            distributedStresstestControl.Anchor = (AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top);
            stresstestReportControl.Anchor = (AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top);
        }
        private void DistributedTestView_HandleCreated(object sender, EventArgs e)
        {
            this.HandleCreated -= DistributedTestView_HandleCreated;
            SetGui();
        }
        private void SetGui()
        {
            cboRunSynchronization.SelectedIndex = (int)_distributedTest.RunSynchronization;
            SetTiles();

            _distributedTest.TilesSynchronized += new EventHandler(_distributedTest_TilesSynchronized);
            cboRunSynchronization.SelectedIndexChanged += new EventHandler(cboRunSynchronization_SelectedIndexChanged);
            SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);
        }
        public override void Refresh()
        {
            base.Refresh();
            propertiesSolutionComponentPropertyPanel.Refresh();
        }
        private void DistributedTestView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnStart.Enabled || MessageBox.Show("Are you sure you want to close a running test?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Stop();

                if (_linkageOverview != null)
                    _linkageOverview.KillAllSlaves();
            }
            else
            {
                Solution.ActiveSolution.ExplicitCancelFormClosing = true;
                e.Cancel = true;
            }
        }
        private void cboRunSynchronization_SelectedIndexChanged(object sender, EventArgs e)
        {
            _distributedTest.RunSynchronization = (RunSynchronization)cboRunSynchronization.SelectedIndex;
            SolutionComponent.SolutionComponentChanged -= new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);
            _distributedTest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);
        }
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            tvwTiles.AfterCheck -= tvwTiles_AfterCheck;
            try
            {
                if (tvwTiles != null && !tvwTiles.IsDisposed)
                {
                    foreach (TreeNode node in tvwTiles.Nodes)
                    {
                        node.Checked = (node.Tag as Tile).Use;
                        node.Text = node.Tag.ToString();
                        if (!node.Checked)
                            foreach (TreeNode childNode in node.Nodes)
                                (childNode.Tag as OldTileStresstest).Use = false;

                        foreach (TreeNode childNode in node.Nodes)
                        {
                            childNode.Text = childNode.Tag.ToString();
                            childNode.Checked = (childNode.Tag as OldTileStresstest).Use;
                        }
                    }
                }
            }
            catch { }
            tvwTiles.AfterCheck += tvwTiles_AfterCheck;
        }
        private void btnLinkageOverview_Click(object sender, EventArgs e)
        {
            if (_linkageOverview == null || _linkageOverview.IsDisposed)
                _linkageOverview = new LinkageOverview(_distributedTest);
            _linkageOverview.TopMost = true;
            _linkageOverview.Show();
            _linkageOverview.TopMost = false;
        }

        #region Tiles
        private void SetTiles()
        {
            tvwTiles.AfterCheck -= tvwTiles_AfterCheck;
            tvwTiles.Nodes.Clear();
            foreach (BaseItem item in _distributedTest.Tiles)
                AddTreeNode(item);
            if (tvwTiles.Nodes.Count > 0)
                tvwTiles.SelectedNode = tvwTiles.Nodes[0];
            tvwTiles.AfterCheck += tvwTiles_AfterCheck;
        }
        private void _distributedTest_TilesSynchronized(object sender, EventArgs e)
        {
            SetTiles();
        }
        private TreeNode AddTreeNode(BaseItem item)
        {
            TreeNode node = item.GetTreeNode();
            node.ContextMenuStrip = null;

            node.Checked = (item as Tile).Use;
            foreach (TreeNode childNode in node.Nodes)
            {
                childNode.NodeFont = new Font(tvwTiles.Font, FontStyle.Regular);
                childNode.Checked = (childNode.Tag as OldTileStresstest).Use;
            }

            tvwTiles.Nodes.Add(node);
            node.Expand();
            return node;
        }
        private void btnAddTile_Click(object sender, EventArgs e)
        {
            Tile tile = new Tile();
            _distributedTest.Add(tile);
            SolutionComponent.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);

            TreeNode node = AddTreeNode(tile);
            tvwTiles.SelectedNode = node;
        }
        private void tvwTiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            btnRemove.Enabled = tvwTiles.SelectedNode.Level == 0;
            propertiesSolutionComponentPropertyPanel.SolutionComponent = tvwTiles.SelectedNode.Tag as SolutionComponent;
            btnResetToDefaults.Enabled = btnStart.Enabled;
            tvwTiles.Focus();
        }
        private void tvwTiles_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            //Do not allow this if the test is started.
            e.Cancel = btnStop.Enabled;
        }
        private void tvwTiles_AfterCheck(object sender, TreeViewEventArgs e)
        {
            tvwTiles.Enabled = false;
            tmrCheckTvwTiles.Stop();
            suscribedToTvwChecked = false;

            try
            {
                tvwTiles.BeforeCheck -= tvwTiles_BeforeCheck;
                tvwTiles.AfterCheck -= tvwTiles_AfterCheck;
            }
            catch { }

            if (e.Node.Level == 0)
            {
                foreach (TreeNode node in e.Node.Nodes)
                    (node.Tag as OldTileStresstest).Use = e.Node.Checked;
            }
            else
            {
                (e.Node.Tag as OldTileStresstest).Use = e.Node.Checked;
            }
            tmrCheckTvwTiles.Start();
        }
        private void tmrCheckTvwTiles_Tick(object sender, EventArgs e)
        {
            if (!this.IsDisposed)
                try { HandleTvwCheck(); }
                catch { }
        }
        private void HandleTvwCheck()
        {
            bool changes = false;
            foreach (TreeNode node in tvwTiles.Nodes)
            {
                bool checkNode = false;

                foreach (TreeNode childNode in node.Nodes)
                {
                    bool checkChildNode = (childNode.Tag as OldTileStresstest).Use;
                    if (checkChildNode)
                        checkNode = true;

                    if (childNode.Checked != checkChildNode)
                    {
                        childNode.Checked = checkChildNode;
                        changes = true;
                    }
                }

                (node.Tag as Tile).Use = checkNode;
                if (node.Checked != checkNode)
                    node.Checked = checkNode;
            }

            if (changes)
            {
                propertiesSolutionComponentPropertyPanel.Refresh();
                SolutionComponent.SolutionComponentChanged -= SolutionComponent_SolutionComponentChanged;
                SolutionComponent.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
            }

            if (!suscribedToTvwChecked)
            {
                suscribedToTvwChecked = true;
                tvwTiles.AfterCheck += tvwTiles_AfterCheck;
                tvwTiles.BeforeCheck += tvwTiles_BeforeCheck;
            }
            tvwTiles.Enabled = true;
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            TreeNode node = tvwTiles.SelectedNode;
            tvwTiles.SelectedNode.Remove();
            if (tvwTiles.Nodes.Count == 0)
            {
                btnRemove.Enabled = false;
                propertiesSolutionComponentPropertyPanel.SolutionComponent = null;
                btnResetToDefaults.Enabled = false;
            }
            else
            {
                tvwTiles.SelectedNode = tvwTiles.Nodes[0];
            }
            _distributedTest.Remove(node.Tag as BaseItem);
        }
        #endregion

        #region Properties
        private void btnResetToDefaults_Click(object sender, EventArgs e)
        {
            bool refresh = false;
            switch (tvwTiles.SelectedNode.Level)
            {
                case 0:
                    if (MessageBox.Show("This will set the property values of the stresstests in this tile as they are in the solution (Monitor not included!). Continue?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        foreach (TreeNode node in tvwTiles.SelectedNode.Nodes)
                            (node.Tag as OldTileStresstest).SetDefaults();
                        refresh = true;
                    }
                    break;
                case 1:
                    if (MessageBox.Show("This will set the property values of the stresstest as they are in the solution (Monitor not included!). Continue?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        (tvwTiles.SelectedNode.Tag as OldTileStresstest).SetDefaults();
                        refresh = true;
                    }
                    break;
            }
            if (refresh)
            {
                propertiesSolutionComponentPropertyPanel.Refresh();
                SolutionComponent.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }
        #endregion

        #region Stresstest

        #region Start
        private void btnSchedule_Click(object sender, EventArgs e)
        {
            Schedule schedule = (btnSchedule.Tag is DateTime) ? new Schedule((DateTime)btnSchedule.Tag) : new Schedule();
            if (schedule.ShowDialog() == DialogResult.OK)
            {
                if (schedule.ScheduledAt > DateTime.Now)
                {
                    btnSchedule.Text = "Scheduled at " + schedule.ScheduledAt;
                    btnSchedule.Tag = schedule.ScheduledAt;
                }
                else
                {
                    btnSchedule.Text = "Schedule...";
                    btnSchedule.Tag = null;
                }

                btnStart_Click(this, null);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_linkageOverview != null && !_linkageOverview.IsDisposed && _linkageOverview.Visible)
                _linkageOverview.Close();

            if (distributedStresstestControl.FastResultsCount > 0 && MessageBox.Show("Do you want to clear the previous results, before starting the test (at the scheduled date / time)?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            if (_distributedTest.RunSynchronization != RunSynchronization.None && !CheckNumberOfRuns())
            {
                MessageBox.Show("Could not start the distributed test because the number of runs for the different single stresstests are not equal to each other.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(_resultsPath) && Directory.Exists(_resultsPath))
                fbd.SelectedPath = _resultsPath;

            if (fbd.ShowDialog() == DialogResult.OK)
                _resultsPath = fbd.SelectedPath;
            else
                return;

            distributedStresstestControl.DistributedTest = _distributedTest;

            if (btnSchedule.Tag != null && btnSchedule.Tag is DateTime && (DateTime)btnSchedule.Tag > DateTime.Now)
                ScheduleTest();
            else
                Start();
        }
        /// <summary>
        /// Check if the number of runs for the different single stresstests are equal to each other.
        /// Use this when using run synchronization.
        /// </summary>
        /// <returns></returns>
        private bool CheckNumberOfRuns()
        {
            int numberOfRuns = -1;
            foreach (Tile t in _distributedTest.Tiles)
                foreach (OldTileStresstest ts in t)
                    if (ts.Use)
                    {
                        //Get the amount of runs.  
                        int runs = 0;
                        foreach (int cu in ts.ConcurrentUsers)
                        {
                            int r = ts.DynamicRunMultiplier / cu;
                            runs += (r == 0) ? 1 : r;
                        }
                        runs *= ts.Precision;

                        if (numberOfRuns == -1)
                            numberOfRuns = runs;
                        else if (numberOfRuns != runs)
                            return false;
                    }
            return true;
        }

        #region Start Logic (with logic to start only when all monitors are initialized (if any))
        private void Start()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnStop.Enabled = true;
                btnStart.Enabled = false;
                btnSchedule.Enabled = false;
                btnSchedule.Text = "Schedule...";
                cboRunSynchronization.Enabled = false;
                btnLinkageOverview.Enabled = false;

                tvwTiles.ForeColor = Color.DimGray;
                btnAddTile.Enabled = false;
                btnRemove.Enabled = false;
                propertiesSolutionComponentPropertyPanel.Lock();
                btnResetToDefaults.Enabled = false;

                btnOpenRFilesFolder.Enabled = false;

                _distributedTestCore = new DistributedTestCore(_distributedTest);
                _distributedTestCore.Message += new EventHandler<MessageEventArgs>(_distributedTestCore_Message);
                _distributedTestCore.OnPushMessageReceived += new EventHandler<PushMessageReceivedEventArgs>(_distributedTestCore_PushMessageReceived);
                _distributedTestCore.OnListeningError += new EventHandler<ListeningErrorEventArgs>(_distributedTestCore_OnListeningError);
                _distributedTestCore.OnFinished += new EventHandler<FinishedEventArgs>(_distributedTestCore_OnFinished);

                tc.SelectedIndex = 1;
                distributedStresstestControl.SetSelectedTabIndex(0);

                distributedStresstestControl.Clear();

                Thread t = new Thread(InitializeAndStartTest);
                t.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                t.IsBackground = true;
                t.Start();
            }
            catch
            {
                //Only one test can run at the same time.
                distributedStresstestControl.AppendMasterMessages("Cannot start this test because another one is still running.", LogLevel.Error);
                Stop();
            }
        }
        private void InitializeAndStartTest()
        {

            Exception ex = InitializeTest();
            if (ex == null && _pendingMonitorViewInitializations == 0)
                StartTest();
        }
        private Exception InitializeTest()
        {
            _pendingMonitorViewInitializations = 0;

            try
            {
                _distributedTestCore.Initialize();
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {

                    distributedStresstestControl.SetSlaveInitialized();

                    stresstestReportControl.ClearReport();
                    flpStresstestTileStresstests.Controls.Clear();
                    flpReportTileStresstests.Controls.Clear();

                    foreach (OldTileStresstest tileStresstest in _distributedTestCore.UsedTileStresstests)
                    {
                        TileStresstestSelectorControl tileStresstestControl = new TileStresstestSelectorControl(tileStresstest);
                        tileStresstestControl.Margin = new Padding(0, 6, 6, 3);
                        tileStresstestControl.GotFocus += new EventHandler(tileStresstestSelectorControl_GotFocus);
                        flpStresstestTileStresstests.Controls.Add(tileStresstestControl);

                        tileStresstestControl.MonitorOnHandledException += new EventHandler<ErrorEventArgs>(tileStresstestControl_MonitorOnHandledException);
                        tileStresstestControl.MonitorOnUnhandledException += new EventHandler<ErrorEventArgs>(tileStresstestControl_MonitorOnUnhandledException);

                        //Determine the number of monitors first.
                        _pendingMonitorViewInitializations += tileStresstest.Monitors.Length;
                    }

                    //Afterwards initialize them.
                    foreach (TileStresstestSelectorControl tileStresstestControl in flpStresstestTileStresstests.Controls)
                        for (int i = 0; i != tileStresstestControl.TileStresstest.Monitors.Length; i++)
                        {
                            tileStresstestControl.MonitorInitialized += new EventHandler<MonitorView.MonitorInitializedEventArgs>(tileStresstestControl_MonitorInitialized);
                            tileStresstestControl.ShowMonitorView(tileStresstestControl.TileStresstest.Monitors[i]);
                        }

                    flpStresstestTileStresstests.Controls[0].Focus();
                });
            }
            catch (Exception ex)
            {
                HandleInitializeOrStartException(ex);
                return ex;
            }
            return null;
        }

        private void tileStresstestControl_MonitorOnHandledException(object sender, ErrorEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                distributedStresstestControl.AppendMasterMessages((sender as MonitorView).Text + ": A counter became unavailable while monitoring:\n" + e.GetException(), LogLevel.Warning);
            });
        }

        private void tileStresstestControl_MonitorOnUnhandledException(object sender, ErrorEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                distributedStresstestControl.AppendMasterMessages((sender as MonitorView).Text + ": An error has occured while monitoring, monitor stopped!\n" + e.GetException(), LogLevel.Error);
            });
        }

        private void tileStresstestControl_MonitorInitialized(object sender, MonitorView.MonitorInitializedEventArgs e)
        {
            if (Interlocked.Decrement(ref _pendingMonitorViewInitializations) == 0)
                StartTest();
        }
        #endregion

        /// <summary>
        /// Can only start after that all monitor views are initialized.
        /// </summary>
        private Exception StartTest()
        {
            try
            {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    LocalMonitor.StartMonitoring(Stresstest.Stresstest.ProgressUpdateDelay * 1000);
                    tmrProgress.Interval = Stresstest.Stresstest.ProgressUpdateDelay * 1000;
                    tmrProgress.Start();

                    tmrProgressDelayCountDown.Start();

                    _countDown = Stresstest.Stresstest.ProgressUpdateDelay - 1;

                    foreach (TileStresstestSelectorControl tileStresstestSelectorControl in flpStresstestTileStresstests.Controls)
                        tileStresstestSelectorControl.StartMonitorIfAny();

                    Cursor = Cursors.Default;
                });
                _distributedTestCore.Start();
            }
            catch (Exception ex)
            {
                HandleInitializeOrStartException(ex);
                return ex;
            }
            return null;
        }
        private void HandleInitializeOrStartException(Exception ex)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                string message = string.Format("The stresstest threw an exception:{0}{1}", Environment.NewLine, ex.Message);
                distributedStresstestControl.AppendMasterMessages(message, LogLevel.Error);
                if (_distributedTestCore != null && !_distributedTestCore.IsDisposed)
                {
                    _distributedTestCore.Dispose();
                    _distributedTestCore = null;
                }
                tmrProgress.Stop();
                StopProgressDelayCountDown();

                distributedStresstestControl.SetStresstestStopped();

                foreach (TileStresstestSelectorControl tileStresstestSelectorControl in flpStresstestTileStresstests.Controls)
                    tileStresstestSelectorControl.StopMonitorIfAny();

                btnStop.Enabled = false;
                btnSchedule.Enabled = true;
                cboRunSynchronization.Enabled = true;

                tvwTiles.ForeColor = this.ForeColor;
                btnAddTile.Enabled = true;
                btnRemove.Enabled = true;
                propertiesSolutionComponentPropertyPanel.Unlock();
                btnResetToDefaults.Enabled = true;

                btnLinkageOverview.Enabled = true;
                btnStart.Enabled = true;
                Cursor = Cursors.Default;
            });
        }

        private void ScheduleTest()
        {
            btnStop.Enabled = true;
            btnStart.Enabled = false;
            btnLinkageOverview.Enabled = false;
            btnSchedule.Enabled = false;
            cboRunSynchronization.Enabled = false;

            tvwTiles.ForeColor = this.ForeColor;
            btnAddTile.Enabled = false;
            btnRemove.Enabled = false;
            propertiesSolutionComponentPropertyPanel.Lock();
            btnResetToDefaults.Enabled = false;

            tmrSchedule.Start();
        }
        private void tmrSchedule_Tick(object sender, EventArgs e)
        {
            DateTime scheduledAt = (DateTime)btnSchedule.Tag;
            if (scheduledAt <= DateTime.Now)
            {
                btnSchedule.Text = "Scheduled at " + scheduledAt;
                tmrSchedule.Stop();
                Start();
            }
            else
            {
                TimeSpan dt = scheduledAt - DateTime.Now;
                if (dt.Milliseconds != 0)
                {
                    dt = new TimeSpan(dt.Ticks - (dt.Ticks % TimeSpan.TicksPerSecond));
                    dt += new TimeSpan(0, 0, 1);
                }
                btnSchedule.Text = "Scheduled in " + dt.ToLongFormattedString();
            }
        }
        #endregion

        #region Progress
        private void tmrProgressDelayCountDown_Tick(object sender, EventArgs e)
        {
            distributedStresstestControl.SetCountDownProgressDelay(_countDown);
            --_countDown;
        }
        private void tmrProgress_Tick(object sender, EventArgs e)
        {
            distributedStresstestControl.SetMasterMonitoring(_distributedTestCore.Running, _distributedTestCore.OK, _distributedTestCore.Cancelled, _distributedTestCore.Failed, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
            _countDown = Stresstest.Stresstest.ProgressUpdateDelay;
        }
        private void StopProgressDelayCountDown()
        {
            try
            {
                tmrProgressDelayCountDown.Stop();
                if (distributedStresstestControl != null && !distributedStresstestControl.IsDisposed)
                    distributedStresstestControl.SetCountDownProgressDelay(-1);
            }
            catch { }
        }
        private void _distributedTestCore_Message(object sender, MessageEventArgs e)
        {
            distributedStresstestControl.AppendMasterMessages(e.Message);
        }
        /// <summary>
        /// Gets the end of time frame for the overal progress.
        /// </summary>
        /// <returns></returns>
        private void GetTimeFrame(out DateTime beginOfTimeFrame, out DateTime endOfTimeFrame)
        {
            beginOfTimeFrame = DateTime.Now;
            if (_distributedTest.RunSynchronization == RunSynchronization.BreakOnFirstFinished)
            {
                endOfTimeFrame = DateTime.MaxValue;
                foreach (TileStresstestSelectorControl tileStresstestSelectorControl in flpStresstestTileStresstests.Controls)
                    if (endOfTimeFrame > tileStresstestSelectorControl.EndOfTimeFrame)
                    {
                        beginOfTimeFrame = tileStresstestSelectorControl.BeginOfTimeFrame;
                        endOfTimeFrame = tileStresstestSelectorControl.EndOfTimeFrame;
                    }
            }
            else
            {
                endOfTimeFrame = DateTime.MinValue;
                foreach (TileStresstestSelectorControl tileStresstestSelectorControl in flpStresstestTileStresstests.Controls)
                    if (endOfTimeFrame < tileStresstestSelectorControl.EndOfTimeFrame)
                    {
                        beginOfTimeFrame = tileStresstestSelectorControl.BeginOfTimeFrame;
                        endOfTimeFrame = tileStresstestSelectorControl.EndOfTimeFrame;
                    }
            }
        }
        private void _distributedTestCore_PushMessageReceived(object sender, PushMessageReceivedEventArgs e)
        {
            DateTime beginOfTimeFrame, endOfTimeFrame;
            GetTimeFrame(out beginOfTimeFrame, out endOfTimeFrame);
            foreach (TileStresstestSelectorControl tileStresstestSelectorControl in flpStresstestTileStresstests.Controls)
                if (tileStresstestSelectorControl.TileStresstest.OriginalHashCode == e.PushMessage.TileStresstestOriginalHashCode)
                {
                    tileStresstestSelectorControl.MostRecentPushMessage = e.PushMessage;
                    if (tileStresstestSelectorControl == _focussedTileStresstestSelectorControl)
                    {
                        distributedStresstestControl.SetSlaveMonitoring(e.PushMessage.ThreadsInUse, e.PushMessage.CPUUsage, e.PushMessage.ContextSwitchesPerSecond, (int)e.PushMessage.MemoryUsage, (int)e.PushMessage.TotalVisibleMemory, e.PushMessage.NicsSent, e.PushMessage.NicsReceived);
                        distributedStresstestControl.SetSlaveProgress(tileStresstestSelectorControl.TileStresstest, tileStresstestSelectorControl.MostRecentPushMessage, beginOfTimeFrame, endOfTimeFrame);

                        tmrProgressDelayCountDown.Stop();
                        _countDown = Stresstest.Stresstest.ProgressUpdateDelay;
                        distributedStresstestControl.SetCountDownProgressDelay(_countDown);
                        tmrProgressDelayCountDown.Start();
                    }
                    break;
                }

            SetOverallProgress();
        }
        private void SetOverallProgress()
        {
            var progress = new Dictionary<OldTileStresstest, TileStresstestProgressResults>(flpStresstestTileStresstests.Controls.Count);
            foreach (TileStresstestSelectorControl tileStresstestSelectorControl in flpStresstestTileStresstests.Controls)
                progress.Add(tileStresstestSelectorControl.TileStresstest, tileStresstestSelectorControl.MostRecentPushMessage.TileStresstestProgressResults);

            distributedStresstestControl.SetOverallFastResults(progress);
        }
        private void distributedStresstestControl_DrillDownChanged(object sender, EventArgs e)
        {
            SetOverallProgress();
        }
        private void _distributedTestCore_OnListeningError(object sender, ListeningErrorEventArgs e)
        {
            foreach (TileStresstestSelectorControl tileStresstestControl in flpStresstestTileStresstests.Controls)
                if (tileStresstestControl.TileStresstest.SlaveIP == e.SlaveIP && tileStresstestControl.TileStresstest.SlavePort == e.SlavePort)
                    tileStresstestControl.StresstestResult = StresstestResult.Error;
        }
        private void _distributedTestCore_OnFinished(object sender, FinishedEventArgs e)
        {
            _distributedTestCore.OnFinished -= _distributedTestCore_OnFinished;

            btnStop.Enabled = false;

            foreach (TileStresstestSelectorControl tileStresstestSelectorControl in flpStresstestTileStresstests.Controls)
                tileStresstestSelectorControl.StopMonitorIfAny();

            Thread t = new Thread(GetResultsAndFinish);
            t.IsBackground = true;
            t.Start(e);
        }
        private void GetResultsAndFinish(object parameter)
        {
            if (!Directory.Exists(_resultsPath))
                Directory.CreateDirectory(_resultsPath);

            try
            {
                foreach (ResultsMessage resultsMessage in _distributedTestCore.GetResults())
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                    {
                        foreach (TileStresstestSelectorControl tileStresstestSelectorControl in flpStresstestTileStresstests.Controls)
                        {
                            try
                            {
                                int index = resultsMessage.TileStresstestHashCodes.IndexOf(tileStresstestSelectorControl.TileStresstest.OriginalHashCode);
                                if (index != -1)
                                {
                                    if (resultsMessage.TorrentInfo.Count == 0)
                                    {
                                        distributedStresstestControl.AppendMasterMessages(string.Format("|-> Failed receiving results for {0} - {1}!\nPlease find the 'r' file slave-side in the vApus folder \\SlaveSideResults.", tileStresstestSelectorControl.TileStresstest.Parent, tileStresstestSelectorControl.TileStresstest));
                                    }
                                    else
                                    {
                                        TileStresstestSelectorControl reportTileStresstestSelectorControl = new TileStresstestSelectorControl(tileStresstestSelectorControl.TileStresstest);
                                        tileStresstestSelectorControl.Tag = reportTileStresstestSelectorControl;

                                        flpReportTileStresstests.Controls.Add(reportTileStresstestSelectorControl);

                                        reportTileStresstestSelectorControl.Enabled = false;
                                        reportTileStresstestSelectorControl.Margin = new Padding(0, 6, 6, 3);
                                        reportTileStresstestSelectorControl.MostRecentPushMessage = tileStresstestSelectorControl.MostRecentPushMessage;
                                        reportTileStresstestSelectorControl.MonitorViews = tileStresstestSelectorControl.MonitorViews;

                                        //Force update the progress bar.
                                        reportTileStresstestSelectorControl.HandleCreated += new EventHandler(reportTileStresstestSelectorControl_HandleCreated);
                                        reportTileStresstestSelectorControl.Click += new EventHandler(reportTileStresstestSelectorControl_Click);
                                        reportTileStresstestSelectorControl.EnabledChanged += new EventHandler(reportTileStresstestSelectorControl_EnabledChanged);

                                        tileStresstestSelectorControl.DownloadTorrent(resultsMessage.TorrentInfo[index], _resultsPath, _distributedTestCore);
                                        if (tileStresstestSelectorControl == _focussedTileStresstestSelectorControl)
                                        {
                                            DateTime beginOfTimeFrame, endOfTimeFrame;
                                            GetTimeFrame(out beginOfTimeFrame, out endOfTimeFrame);

                                            PushMessage pushMessage = tileStresstestSelectorControl.MostRecentPushMessage;
                                            distributedStresstestControl.SetSlaveMonitoring(pushMessage.ThreadsInUse, pushMessage.CPUUsage, pushMessage.ContextSwitchesPerSecond, (int)pushMessage.MemoryUsage, (int)pushMessage.TotalVisibleMemory, pushMessage.NicsSent, pushMessage.NicsReceived);
                                            distributedStresstestControl.SetSlaveProgress(tileStresstestSelectorControl.TileStresstest, tileStresstestSelectorControl.MostRecentPushMessage, beginOfTimeFrame, endOfTimeFrame);
                                        }

                                        distributedStresstestControl.AppendMasterMessages(string.Format("|-> Results receiving for {0} - {1}", tileStresstestSelectorControl.TileStresstest.Parent, tileStresstestSelectorControl.TileStresstest));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                distributedStresstestControl.AppendMasterMessages(string.Format("|-> Failed receiving results for {0} - {1}\n{2}", tileStresstestSelectorControl.TileStresstest.Parent, tileStresstestSelectorControl.TileStresstest, ex), LogLevel.Error);
                            }
                        }

                    });
            }
            catch
            {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    distributedStresstestControl.AppendMasterMessages("|-> Failed receiving results for various tests!", LogLevel.Error);
                });
            }
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                btnSchedule.Enabled = true;
                cboRunSynchronization.Enabled = true;

                tvwTiles.ForeColor = Color.DimGray;
                btnAddTile.Enabled = true;
                btnRemove.Enabled = true;
                propertiesSolutionComponentPropertyPanel.Unlock();
                btnResetToDefaults.Enabled = true;

                tmrProgress.Stop();
                StopProgressDelayCountDown();

                btnStart.Enabled = true;
                btnLinkageOverview.Enabled = true;


                FinishedEventArgs e = parameter as FinishedEventArgs;

                string message = "\nIf possible, the results are now (being) fetched from the slaves.";
                distributedStresstestControl.AppendMasterMessages(message, LogLevel.Warning);
                distributedStresstestControl.SetStresstestStopped();
                try
                {
                    distributedStresstestControl.SetMasterMonitoring(_distributedTestCore.Running, _distributedTestCore.OK, _distributedTestCore.Cancelled, _distributedTestCore.Failed, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
                }
                catch { }
            });
        }
        /// <summary>
        /// Force update the progress bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reportTileStresstestSelectorControl_HandleCreated(object sender, EventArgs e)
        {
            var ctrl = sender as TileStresstestSelectorControl;
            ctrl.MostRecentPushMessage = ctrl.MostRecentPushMessage;
        }
        private void tileStresstestSelectorControl_GotFocus(object sender, EventArgs e)
        {
            _focussedTileStresstestSelectorControl = sender as TileStresstestSelectorControl;
            OldTileStresstest tileStresstest = _focussedTileStresstestSelectorControl.TileStresstest;
            PushMessage pushMessage = _focussedTileStresstestSelectorControl.MostRecentPushMessage;
            DateTime beginOfTimeFrame, endOfTimeFrame;
            GetTimeFrame(out beginOfTimeFrame, out endOfTimeFrame);

            distributedStresstestControl.SetSlaveConfigurationControls(tileStresstest, _focussedTileStresstestSelectorControl.MonitorViews);
            distributedStresstestControl.SetSlaveMonitoring(pushMessage.ThreadsInUse, pushMessage.CPUUsage, pushMessage.ContextSwitchesPerSecond, (int)pushMessage.MemoryUsage, (int)pushMessage.TotalVisibleMemory, pushMessage.NicsSent, pushMessage.NicsReceived);
            distributedStresstestControl.SetSlaveProgress(tileStresstest, pushMessage, beginOfTimeFrame, endOfTimeFrame);
        }
        private void reportTileStresstestSelectorControl_EnabledChanged(object sender, EventArgs e)
        {
            TileStresstestSelectorControl reportTileStresstestSelectorControl = sender as TileStresstestSelectorControl;
            if (reportTileStresstestSelectorControl == flpReportTileStresstests.Controls[0] && (stresstestReportControl.Stresstest == null || stresstestReportControl.StresstestResults == null))
            {
                this.Cursor = Cursors.WaitCursor;

                reportTileStresstestSelectorControl.Select();

                while (tcReport.TabCount != 1)
                    tcReport.TabPages.RemoveAt(1);

                foreach (MonitorView view in
                    reportTileStresstestSelectorControl.LoadStresstestAndMonitorReport(stresstestReportControl))
                {

                    var monitorTabPage = new TabPage("Monitor Report: " + view.Text);
                    var monitorReportControl = view.Tag as MonitorReportControl;
                    monitorReportControl.Dock = DockStyle.Fill;

                    monitorTabPage.Controls.Add(monitorReportControl);
                    tcReport.TabPages.Add(monitorTabPage);
                }

                this.Cursor = Cursors.Default;
            }
            btnOpenRFilesFolder.Enabled = true;
        }
        private void reportTileStresstestSelectorControl_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            TileStresstestSelectorControl reportTileStresstestSelectorControl = sender as TileStresstestSelectorControl;

            while (tcReport.TabCount != 1)
                tcReport.TabPages.RemoveAt(1);

            foreach (MonitorView view in
                reportTileStresstestSelectorControl.LoadStresstestAndMonitorReport(stresstestReportControl))
            {

                var monitorTabPage = new TabPage("Monitor Report: " + view.Text);
                var monitorReportControl = view.Tag as MonitorReportControl;
                monitorReportControl.Dock = DockStyle.Fill;

                monitorTabPage.Controls.Add(monitorReportControl);
                tcReport.TabPages.Add(monitorTabPage);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Stop
        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }
        private void Stop()
        {
            Cursor = Cursors.WaitCursor;
            btnStop.Enabled = false;
            tmrSchedule.Stop();
            if (btnSchedule.Tag != null && tmrSchedule.Tag is DateTime)
            {
                DateTime scheduledDateTime = (DateTime)btnSchedule.Tag;
                btnSchedule.Text = (scheduledDateTime > DateTime.Now) ? "Scheduled at " + scheduledDateTime : "Schedule...";
            }
            if (_distributedTestCore != null)
                try
                {
                    _distributedTestCore.Stop();
                    distributedStresstestControl.SetMasterMonitoring(_distributedTestCore.Running, _distributedTestCore.OK, _distributedTestCore.Cancelled, _distributedTestCore.Failed, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
                }
                catch (Exception ex)
                {
                    string message = string.Format("The stresstest threw an exception:{0}{1}", Environment.NewLine, ex.Message);
                    distributedStresstestControl.AppendMasterMessages(message, LogLevel.Error);
                }

            btnSchedule.Enabled = true;
            cboRunSynchronization.Enabled = true;

            tvwTiles.ForeColor = Color.Black;
            btnAddTile.Enabled = true;
            btnRemove.Enabled = true;
            propertiesSolutionComponentPropertyPanel.Unlock();
            btnResetToDefaults.Enabled = true;

            btnStart.Enabled = true;

            tmrProgress.Stop();
            StopProgressDelayCountDown();

            distributedStresstestControl.SetStresstestStopped();

            btnLinkageOverview.Enabled = true;
            Cursor = Cursors.Default;
        }
        #endregion

        private void btnOpenRFilesFolder_Click(object sender, EventArgs e)
        {
            if (_resultsPath != null)
                Process.Start(_resultsPath);
        }

        #endregion

        #region Collapse/Expand headers
        private void btnCollapseExpandStresstest_Click(object sender, EventArgs e)
        {
            if (btnCollapseExpandStresstest.Text == "-")
            {
                btnCollapseExpandStresstest.Text = "+";
                splitStresstest.SplitterDistance = 100;
                splitStresstest.IsSplitterFixed = true;
            }
            else
            {
                int height = CalculateControlsHeight(flpStresstestTileStresstests);

                int splitterDistance = flpStresstestTileStresstests.Margin.Top + height + flpStresstestTileStresstests.Margin.Bottom +
                    flpStresstestTiles.Margin.Top + flpStresstestTiles.Height + flpStresstestTiles.Margin.Bottom;

                if (splitterDistance < 100)
                    return;

                btnCollapseExpandStresstest.Text = "-";

                splitStresstest.SplitterDistance = splitterDistance;
                splitStresstest.IsSplitterFixed = false;
            }
        }
        private void btnCollapseExpandReport_Click(object sender, EventArgs e)
        {
            if (btnCollapseExpandReport.Text == "-")
            {
                btnCollapseExpandReport.Text = "+";
                splitReport.SplitterDistance = 100;
                splitReport.IsSplitterFixed = true;
            }
            else
            {
                int height = CalculateControlsHeight(flpReportTileStresstests);

                int splitterDistance = flpReportTileStresstests.Margin.Top + height + flpReportTileStresstests.Margin.Bottom +
                    flpReportTiles.Margin.Top + flpReportTiles.Height + flpReportTiles.Margin.Bottom;

                if (splitterDistance < 100)
                    return;

                btnCollapseExpandReport.Text = "-";

                splitReport.SplitterDistance = splitterDistance;
                splitReport.IsSplitterFixed = false;
            }
        }
        private int CalculateControlsHeight(FlowLayoutPanel flp)
        {
            if (flp.Controls.Count == 0)
                return 0;

            flp.VerticalScroll.Value = 0;
            return flp.Controls[flp.Controls.Count - 1].Bottom;
        }
        #endregion

        #endregion
    }
}