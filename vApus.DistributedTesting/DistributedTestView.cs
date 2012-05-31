/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;
using vApus.Stresstest;

namespace vApus.DistributedTesting
{
    public partial class NewDistributedTestView : BaseSolutionComponentView
    {
        #region Fields
        private DistributedTest _distributedTest = new DistributedTest();
        private DistributedTestCore _distributedTestCore;

        /// <summary>
        /// The test can only start when this == 0.
        /// </summary>
        private int _pendingMonitorViewInitializations;
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
        public NewDistributedTestView()
        {
            InitializeComponent();
        }
        public NewDistributedTestView(SolutionComponent solutionComponent, params object[] args)
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
            if (sender is TileStresstestTreeViewItem)
            {
                TileStresstestTreeViewItem tstvi = sender as TileStresstestTreeViewItem;
                configureTileStresstest.SetTileStresstest(tstvi.TileStresstest);
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
        #endregion

        #region Start
        private void btnStart_Click(object sender, EventArgs e)
        {
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
        }

        private void ScheduleTest()
        {
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

        #endregion

        #region Progress
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
        { }
        #endregion
    }
}
