/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using vApus.Monitor;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;
using System.Collections.Generic;

namespace vApus.DistributedTesting
{
    public partial class DistributedTestView : BaseSolutionComponentView
    {
        #region Fields
        private object _lock = new object();

        private ITreeViewItem _selectedTestItem;
        private DistributedTest _distributedTest = new DistributedTest();
        private DistributedTestCore _distributedTestCore;
        /// <summary>
        /// Countdown for the update.
        /// </summary>
        private int _countDown;

        /// <summary>
        /// The test can only start when this == 0.
        /// </summary>
        private int _pendingMonitorViewInitializations;
        private AutoResetEvent _monitorViewsInitializedWaitHandle = new AutoResetEvent(false);
        /// <summary>
        /// The monitors for the tests if any.
        /// </summary>
        private List<MonitorView> _monitorViews = new List<MonitorView>(); 
        #endregion

        #region Properties
        private int TileStresstestCount
        {
            get
            {
                int count = 0;
                foreach (Tile t in _distributedTest.Tiles)
                    count += t.Count;
                return count;
            }
        }
        private int UsedTileStresstestCount
        {
            get
            {
                int count = 0;
                foreach (Tile t in _distributedTest.Tiles)
                    foreach (TileStresstest ts in t)
                        if (ts.Use)
                            ++count;
                return count;
            }
        }
        private int SlaveCount
        {
            get
            {
                int count = 0;
                foreach (Client c in _distributedTest.ClientsAndSlaves)
                    count += c.Count;
                return count;
            }
        }
        private int UsedSlaveCount
        {
            get
            {
                int count = 0;
                foreach (Client c in _distributedTest.ClientsAndSlaves)
                    foreach (Slave s in c)
                        if (s.Use)
                            ++count;
                return count;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Desing time constructor
        /// </summary>
        public DistributedTestView()
        {
            InitializeComponent();
        }
        public DistributedTestView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            InitializeComponent();

            _distributedTest = solutionComponent as DistributedTest;
            testTreeView.SetDistributedTest(_distributedTest);
            slaveTreeView.SetDistributedTest(_distributedTest);

            SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);
        }
        #endregion

        #region General Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            if (sender is Tile || sender is TileStresstest || sender is Client || sender is Slave)
                SetGui();
        }

        private void testTreeView_AfterSelect(object sender, EventArgs e)
        {
            _selectedTestItem = sender as ITreeViewItem;
            if (sender is TileStresstestTreeViewItem)
            {
                TileStresstestTreeViewItem tstvi = sender as TileStresstestTreeViewItem;
                configureTileStresstest.SetTileStresstest(tstvi.TileStresstest);

                stresstestControl.Visible = true;
                distributedStresstestControl.Visible = false;

                if (_distributedTestCore != null && _distributedTestCore.TestProgressMessages.ContainsKey(tstvi.TileStresstest.TileStresstestIndex))
                    SetSlaveProgress(tstvi.TileStresstest, _distributedTestCore.TestProgressMessages[tstvi.TileStresstest.TileStresstestIndex]);
            }
            else
            {
                bool showRunSyncDescription = false;
                if (sender is DistributedTestTreeViewItem)
                {
                    var dttvi = sender as DistributedTestTreeViewItem;
                    foreach (Control ctrl in dttvi.Controls)
                        if (ctrl is Panel && ctrl.Controls.Count != 0 && ctrl.Controls[0].Focused)
                        {
                            showRunSyncDescription = true;
                            break;
                        }
                }
                configureTileStresstest.ClearTileStresstest(showRunSyncDescription);

                stresstestControl.Visible = false;
                distributedStresstestControl.Visible = true;

                SetOverallProgress();
            }
        }
        private void slaveTreeView_AfterSelect(object sender, EventArgs e)
        {
            if (sender is ClientTreeViewItem)
            {
                ClientTreeViewItem ctvi = sender as ClientTreeViewItem;
                configureSlaves.SetClient(ctvi.Client);
                configureSlaves.SetClientStatus(ctvi.Online);
            }
            else
            {
                configureSlaves.ClearClient();
            }
        }
        private void slaveTreeView_ClientHostNameAndIPSet(object sender, EventArgs e)
        {
            if (sender is ClientTreeViewItem)
            {
                ClientTreeViewItem ctvi = sender as ClientTreeViewItem;
                if (configureSlaves.Client == ctvi.Client)
                    configureSlaves.SetClientStatus(ctvi.Online);
            }
        }

        private void tmrSetGui_Tick(object sender, EventArgs e)
        {
            SetGui();
        }
        private void SetGui()
        {
            string tests = "Tests (#" + UsedTileStresstestCount + "/" + TileStresstestCount + ")";
            if (tpTests.Text != tests)
                tpTests.Text = tests;

            string slaves = "Slaves (#" + UsedSlaveCount + "/" + SlaveCount + ")";
            if (tpSlaves.Text != slaves)
                tpSlaves.Text = slaves;

            testTreeView.SetGui();
            slaveTreeView.SetGui();
        }

        private void tpTree_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tpTree.SelectedIndex == 0)
            {
                configureTileStresstest.Visible = true;
                configureSlaves.Visible = false;
            }
            else
            {
                configureTileStresstest.Visible = false;
                configureSlaves.Visible = true;
            }
        }

        /// <summary>
        /// Set the gui for the different modes
        /// </summary>
        /// <param name="distributedTestMode"></param>
        /// <param name="scheduled">only for distributedTestMode.TestAndReport</param>
        private void SetMode(DistributedTestMode distributedTestMode, bool scheduled = false)
        {
            if (this.IsDisposed)
                return;

            btnStop.Enabled = distributedTestMode == DistributedTestMode.TestAndReport;
            btnStart.Enabled =
            btnSchedule.Enabled = !btnStop.Enabled;

            if (distributedTestMode == DistributedTestMode.TestAndReport)
            {
                if (scheduled)
                    tmrSchedule.Start();
                else
                    btnSchedule.Text = "Schedule...";

                //Solution.AutoHideStresstestingSolutionExplorer(this);
            }

            if (distributedTestMode == DistributedTestMode.Edit)
            {
                tmrSchedule.Stop();

                tmrProgress.Stop();
                StopProgressDelayCountDown();
            }

            testTreeView.SetMode(distributedTestMode, scheduled);
            slaveTreeView.SetMode(distributedTestMode);
            configureTileStresstest.SetMode(distributedTestMode);
            configureSlaves.SetMode(distributedTestMode);

        }
        #endregion

        #region Start
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_distributedTestCore != null && _distributedTestCore.HasResults &&
                MessageBox.Show("Do you want to clear the previous results, before starting the test (at the scheduled date / time)?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            if (_distributedTest.RunSynchronization != RunSynchronization.None && !CheckNumberOfRuns())
            {
                MessageBox.Show("Could not start the distributed test because the number of runs for the different single stresstests are not equal to each other.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (btnSchedule.Tag != null && btnSchedule.Tag is DateTime && (DateTime)btnSchedule.Tag > DateTime.Now)
                ScheduleTest();
            else
                Start();

        }

        private void Start()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                SetMode(DistributedTestMode.TestAndReport);

                _distributedTestCore = new DistributedTestCore(_distributedTest);
                _distributedTestCore.Message += new EventHandler<MessageEventArgs>(_distributedTestCore_Message);
                _distributedTestCore.OnTestProgressMessageReceived += new EventHandler<TestProgressMessageReceivedEventArgs>(_distributedTestCore_TestProgressMessageReceivedEventArgs);
                _distributedTestCore.OnListeningError += new EventHandler<ListeningErrorEventArgs>(_distributedTestCore_OnListeningError);
                _distributedTestCore.OnFinished += new EventHandler<FinishedEventArgs>(_distributedTestCore_OnFinished);

                tcTest.SelectedIndex = 1;

                distributedStresstestControl.Clear();

                stresstestControl.SetCountDownProgressDelay(-1);

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
        private void ScheduleTest()
        {
            SetMode(DistributedTestMode.TestAndReport, true);
        }
        private void tmrSchedule_Tick(object sender, EventArgs e)
        {
            DateTime scheduledAt = (DateTime)btnSchedule.Tag;
            if (scheduledAt <= DateTime.Now)
            {
                btnSchedule.Text = "Scheduled at " + scheduledAt;
                tmrSchedule.Stop();
                StartTest();
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
        /// <summary>
        /// Show the gui to be able to schedule the test.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSchedule_Click(object sender, EventArgs e)
        {
            Schedule schedule = (btnSchedule.Tag != null && btnSchedule.Tag is DateTime) ? new Schedule((DateTime)btnSchedule.Tag) : new Schedule();
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

        /// <summary>
        /// Check if the number of runs for the different single stresstests are equal to each other.
        /// Use this when using run synchronization.
        /// </summary>
        /// <returns></returns>
        private bool CheckNumberOfRuns()
        {
            int numberOfRuns = -1;
            foreach (Tile t in _distributedTest.Tiles)
                foreach (TileStresstest ts in t)
                    if (ts.Use)
                    {
                        //Get the amount of runs.  
                        int runs = 0;
                        foreach (int cu in ts.AdvancedTileStresstest.ConcurrentUsers)
                        {
                            int r = ts.AdvancedTileStresstest.DynamicRunMultiplier / cu;
                            runs += (r == 0) ? 1 : r;
                        }
                        runs *= ts.AdvancedTileStresstest.Precision;

                        if (numberOfRuns == -1)
                            numberOfRuns = runs;
                        else if (numberOfRuns != runs)
                            return false;
                    }
            return true;
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
                    stresstestControl.SetStresstestInitialized();
                    stresstestReportControl.ClearReport();

                    //Determine the number of monitors first.
                    foreach (TileStresstest tileStresstest in _distributedTestCore.UsedTileStresstests)
                       _pendingMonitorViewInitializations += tileStresstest.BasicTileStresstest.Monitors.Length;

                    //Afterwards initialize them.
                    foreach (TileStresstest tileStresstest in _distributedTestCore.UsedTileStresstests)
                        for (int i = 0; i != tileStresstest.BasicTileStresstest.Monitors.Length; i++)
                            ShowMonitorView(tileStresstest, tileStresstest.BasicTileStresstest.Monitors[i]);
                });

                if (_pendingMonitorViewInitializations != 0)
                    _monitorViewsInitializedWaitHandle.WaitOne();
            }
            catch (Exception ex)
            {
                HandleInitializeOrStartException(ex);
                return ex;
            }
            return null;
        }
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

                    StartMonitorsIfAny();

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

                Stop();
                Cursor = Cursors.Default;
            });
        }

        #endregion

        #region Progress
        private void _distributedTestCore_Message(object sender, MessageEventArgs e)
        {
            distributedStresstestControl.AppendMasterMessages(e.Message);
        }
        private void _distributedTestCore_TestProgressMessageReceivedEventArgs(object sender, TestProgressMessageReceivedEventArgs e)
        {
            DateTime beginOfTimeFrame, endOfTimeFrame;
            GetTimeFrame(out beginOfTimeFrame, out endOfTimeFrame);

            if (_selectedTestItem != null && _selectedTestItem is TileStresstestTreeViewItem &&
                (_selectedTestItem as TileStresstestTreeViewItem).TileStresstest == e.TileStresstest)
            {
                SetSlaveProgress(e.TileStresstest, e.TestProgressMessage);

                tmrProgressDelayCountDown.Stop();
                _countDown = Stresstest.Stresstest.ProgressUpdateDelay;
                stresstestControl.SetCountDownProgressDelay(_countDown);
                tmrProgressDelayCountDown.Start();
            }

            SetOverallProgress();
            SetSlaveProgressInTreeView(e.TileStresstest, e.TestProgressMessage);
        }
        private void SetOverallProgress()
        {
            if (_selectedTestItem != null && _distributedTestCore != null)
                if (_selectedTestItem is DistributedTestTreeViewItem)
                {
                    var progress = new Dictionary<TileStresstest, TileStresstestProgressResults>(_distributedTestCore.TestProgressMessages.Count);
                    foreach (string tileStresstestIndex in _distributedTestCore.TestProgressMessages.Keys)
                        progress.Add(_distributedTestCore.GetTileStresstest(tileStresstestIndex), _distributedTestCore.TestProgressMessages[tileStresstestIndex].TileStresstestProgressResults);

                    distributedStresstestControl.SetOverallFastResults(progress);
                }
                else if (_selectedTestItem is TileTreeViewItem)
                {
                    TileTreeViewItem ttvi = _selectedTestItem as TileTreeViewItem;
                    var progress = new Dictionary<TileStresstest, TileStresstestProgressResults>();
                    foreach (string tileStresstestIndex in _distributedTestCore.TestProgressMessages.Keys)
                    {
                        TileStresstest ts = _distributedTestCore.GetTileStresstest(tileStresstestIndex);
                        if (ttvi.Tile.Contains(ts))
                            progress.Add(ts, _distributedTestCore.TestProgressMessages[tileStresstestIndex].TileStresstestProgressResults);
                    }
                    distributedStresstestControl.SetOverallFastResults(progress);
                }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tileStresstest"></param>
        /// <param name="testProgressMessage"></param>
        /// <param name="overalEndOfTimeFrame">The end of time frame for the full test.</param>
        public void SetSlaveProgress(TileStresstest tileStresstest, TestProgressMessage testProgressMessage)
        {
            lock (_lock)
            {
                //Build and add fast results.
                stresstestControl.ClearFastResults();
                if (testProgressMessage.TileStresstestProgressResults != null)
                {
                    foreach (TileConcurrentUsersProgressResult tcr in testProgressMessage.TileStresstestProgressResults.TileConcurrentUsersProgressResults)
                    {
                        ConcurrentUsersResult cr = new ConcurrentUsersResult(tcr.ConcurrentUsers, tcr.Metrics.TotalLogEntries, tcr.Metrics.StartMeasuringRuntime);
                        cr.Metrics = tcr.Metrics;
                        stresstestControl.AddFastResult(cr);

                        foreach (TilePrecisionProgressResult tpr in tcr.TilePrecisionProgressResults)
                        {
                            PrecisionResult pr = new PrecisionResult(tpr.Precision, tpr.Metrics.TotalLogEntries, tpr.Metrics.StartMeasuringRuntime);
                            pr.Metrics = tpr.Metrics;
                            stresstestControl.AddFastResult(pr);

                            foreach (TileRunProgressResult trr in tpr.TileRunProgressResults)
                            {
                                RunResult rr = new RunResult(trr.Run, 1, trr.Metrics.TotalLogEntries, 1, trr.Metrics.StartMeasuringRuntime, trr.RunStartedAndStopped, trr.RunDoneOnce);
                                rr.Metrics = trr.Metrics;
                                stresstestControl.AddFastResult(rr);
                            }
                        }
                    }
                    stresstestControl.SetStresstestStarted(testProgressMessage.TileStresstestProgressResults.Metrics.StartMeasuringRuntime);
                    stresstestControl.SetMeasuredRunTime(testProgressMessage.TileStresstestProgressResults.Metrics.MeasuredRunTime, testProgressMessage.TileStresstestProgressResults.EstimatedRuntimeLeft);
                }


                if (testProgressMessage.Events == null)
                    stresstestControl.ClearEvents();
                else
                    stresstestControl.SetEvents(testProgressMessage.Events);

                stresstestControl.SetStresstestStopped(testProgressMessage.StresstestResult);

                stresstestControl.SetClientMonitoring(testProgressMessage.ThreadsInUse, testProgressMessage.CPUUsage, testProgressMessage.ContextSwitchesPerSecond, (int)testProgressMessage.MemoryUsage, (int)testProgressMessage.TotalVisibleMemory, testProgressMessage.NicsSent, testProgressMessage.NicsReceived);
                stresstestControl.SetConfigurationControls(tileStresstest.ToString(), tileStresstest.BasicTileStresstest.Connection,
                    tileStresstest.BasicTileStresstest.ConnectionProxy, tileStresstest.AdvancedTileStresstest.Log,
                    tileStresstest.AdvancedTileStresstest.LogRuleSet, tileStresstest.BasicTileStresstest.Monitors, tileStresstest.AdvancedTileStresstest.ConcurrentUsers,
                    tileStresstest.AdvancedTileStresstest.Precision, tileStresstest.AdvancedTileStresstest.DynamicRunMultiplier,
                    tileStresstest.AdvancedTileStresstest.MinimumDelay, tileStresstest.AdvancedTileStresstest.MaximumDelay,
                    tileStresstest.AdvancedTileStresstest.Shuffle, tileStresstest.AdvancedTileStresstest.Distribute);
            }
        }
        public void SetSlaveProgressInTreeView(TileStresstest tileStresstest, TestProgressMessage testProgressMessage)
        {
            lock (_lock)
            {
                TileStresstestTreeViewItem tileStresstestTreeViewItem = null;
                foreach (ITreeViewItem item in testTreeView.Items)
                    if (item is TileStresstestTreeViewItem)
                    {
                        var tstvi = item as TileStresstestTreeViewItem;
                        if (tstvi.TileStresstest == tileStresstest)
                        {
                            tileStresstestTreeViewItem = tstvi;
                            break;
                        }
                    }

                if (tileStresstestTreeViewItem != null)
                {
                    //Build and add fast results.
                    if (testProgressMessage.TileStresstestProgressResults != null)
                    {
                        tileStresstestTreeViewItem.SetStresstestStarted(testProgressMessage.TileStresstestProgressResults.Metrics.StartMeasuringRuntime);
                        tileStresstestTreeViewItem.SetMeasuredRunTime(testProgressMessage.TileStresstestProgressResults.EstimatedRuntimeLeft);
                    }

                    if (testProgressMessage.Events == null)
                        tileStresstestTreeViewItem.ClearEvents();
                    else
                        tileStresstestTreeViewItem.SetEvents(testProgressMessage.Events);
                }
            }
        }
        private void testTreeView_EventClicked(object sender, EventProgressBar.ProgressEventEventArgs e)
        {
            if (sender == _selectedTestItem && _selectedTestItem is TileStresstestTreeViewItem)
                stresstestControl.ShowEvent(e.ProgressEvent.At);
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
                foreach (TestProgressMessage tpm in _distributedTestCore.TestProgressMessages.Values)
                    if (tpm.TileStresstestProgressResults != null)
                    {
                        DateTime dt = DateTime.Now + tpm.TileStresstestProgressResults.EstimatedRuntimeLeft;
                        if (endOfTimeFrame > dt)
                        {
                            beginOfTimeFrame = tpm.TileStresstestProgressResults.Metrics.StartMeasuringRuntime;
                            endOfTimeFrame = dt;
                        }
                    }
            }
            else
            {
                endOfTimeFrame = DateTime.MinValue;
                foreach (TestProgressMessage tpm in _distributedTestCore.TestProgressMessages.Values)
                    if (tpm.TileStresstestProgressResults != null)
                    {
                        DateTime dt = DateTime.Now + tpm.TileStresstestProgressResults.EstimatedRuntimeLeft;
                        if (endOfTimeFrame < dt)
                        {
                            beginOfTimeFrame = tpm.TileStresstestProgressResults.Metrics.StartMeasuringRuntime;
                            endOfTimeFrame = dt;
                        }
                    }
            }
        }
        private void _distributedTestCore_OnListeningError(object sender, ListeningErrorEventArgs e)
        {
#warning Handle listening error
            //foreach (TileStresstestSelectorControl tileStresstestControl in flpStresstestTileStresstests.Controls)
            //    if (tileStresstestControl.TileStresstest.SlaveIP == e.SlaveIP && tileStresstestControl.TileStresstest.SlavePort == e.SlavePort)
            //        tileStresstestControl.StresstestResult = StresstestResult.Error;
        }
        private void _distributedTestCore_OnFinished(object sender, FinishedEventArgs e)
        {
            _distributedTestCore.OnFinished -= _distributedTestCore_OnFinished;

            btnStop.Enabled = false;

            StopMonitorsIfAny();

            Thread t = new Thread(GetResultsAndFinish);
            t.IsBackground = true;
            t.Start(e);
        }
        private void GetResultsAndFinish(object parameter)
        {
#warning Handle getting results
            if (!Directory.Exists(_distributedTest.ResultPath))
                Directory.CreateDirectory(_distributedTest.ResultPath);

            try
            {
                foreach (ResultsMessage resultsMessage in _distributedTestCore.GetResults())
                { }
                /* SynchronizationContextWrapper.SynchronizationContext.Send(delegate
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

                 });*/
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
                Stop();

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

        private void tmrProgressDelayCountDown_Tick(object sender, EventArgs e)
        {
            stresstestControl.SetCountDownProgressDelay(_countDown);
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
                if (stresstestControl != null && !stresstestControl.IsDisposed)
                    stresstestControl.SetCountDownProgressDelay(-1);
            }
            catch { }
        }

        #endregion

        #region Stop
        private void DistributedTestView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnStart.Enabled || MessageBox.Show("Are you sure you want to close a running test?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Stop();
            }
            else
            {
                Solution.ActiveSolution.ExplicitCancelFormClosing = true;
                e.Cancel = true;
            }
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }
        private void Stop()
        {
            this.Cursor = Cursors.WaitCursor;

            SetMode(DistributedTestMode.Edit);

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
            distributedStresstestControl.SetStresstestStopped();

            StopMonitorsIfAny();

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Monitors
        public void ShowMonitorView(TileStresstest tileStresstest, Monitor.Monitor monitor)
        {
            //show the monitorview
            var view = SolutionComponentViewManager.Show(monitor) as Monitor.MonitorView;
            this.Show();
            
            view.Text += " for " + this.Text;
                        
            view.Tag = new MonitorReportControl();
            //For each view initialized, the distributed test view takes care of starting the test.
            view.MonitorInitialized += new EventHandler<MonitorView.MonitorInitializedEventArgs>(view_MonitorInitialized);
            view.OnHandledException += new EventHandler<ErrorEventArgs>(view_OnHandledException);
            view.OnUnhandledException += new EventHandler<ErrorEventArgs>(view_OnUnhandledException);
            view.InitializeForStresstest();

            _monitorViews.Add(view);
        }

        private void view_MonitorInitialized(object sender, MonitorView.MonitorInitializedEventArgs e)
        {
            MonitorView view = sender as MonitorView;
            view.MonitorInitialized -= view_MonitorInitialized;
            if (--_pendingMonitorViewInitializations == 0)
                _monitorViewsInitializedWaitHandle.Set();
        }
        private void view_OnHandledException(object sender, ErrorEventArgs e)
        {
            MonitorView view = sender as MonitorView;
        }
        private void view_OnUnhandledException(object sender, ErrorEventArgs e)
        {
            MonitorView view = sender as MonitorView;
        }
        /// <summary>
        /// Used in stresstest started eventhandling.
        /// </summary>
        public void StartMonitorsIfAny()
        {
            if (_monitorViews != null)
                foreach (MonitorView view in _monitorViews)
                    if (view != null && !view.IsDisposed)
                        try
                        {
                            view.Start();
                        }
                        catch
                        {
                            try { view.Stop(); }
                            catch { }
                        }
        }
        /// <summary>
        /// Only used in stop
        /// </summary>
        public void StopMonitorsIfAny()
        {
            if (_monitorViews != null)
                foreach (MonitorView view in _monitorViews)
                    if (view != null && !view.IsDisposed)
                        view.Stop();
        }
        #endregion
    }
}
