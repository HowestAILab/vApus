/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.DistributedTesting
{
    public partial class LinkageOverview : Form
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        #region Fields
        private object _lock = new object();

        private DistributedTest _distributedTest;
        private Dictionary<string, LinkageControl> _linkageControls = new Dictionary<string, LinkageControl>();
        #endregion

        #region Constructors
        public LinkageOverview()
        {
            InitializeComponent();
        }
        public LinkageOverview(DistributedTest distributedTest)
            : this()
        {
            _distributedTest = distributedTest;
        }
        #endregion

        #region Functions
        private void LinkageOverview_FormClosing(object sender, FormClosingEventArgs e)
        {
            global::vApus.DistributedTesting.Properties.Settings.Default.Save();

            this.Hide();
            e.Cancel = true;
        }
        private void LinkageOverview_Shown(object sender, EventArgs e)
        {
            Startup();
            SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);
        }
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            if (sender is OldTileStresstest || sender is DistributedTest)
            {
                if (this.IsDisposed)
                    SolutionComponent.SolutionComponentChanged -= new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);
                else
                    Startup();
            }
        }

        private void Startup()
        {
            if (_distributedTest == null || this.IsDisposed)
                return;

            this.Text = "Linkage Overview - " + _distributedTest.ToString();

            LockWindowUpdate(this.Handle.ToInt32());

            var newLinkageControls = new Dictionary<string, LinkageControl>();
            flp.Controls.Clear();

            foreach (BaseItem item in _distributedTest.Tiles)
            {
                Tile tile = item as Tile;
                foreach (BaseItem child in tile)
                {
                    OldTileStresstest tileStresstest = child as OldTileStresstest;
                    string key = tileStresstest.SlaveIP + ':' + tileStresstest.SlavePort;

                    LinkageControl lc = null;
                    if (_linkageControls.ContainsKey(key))
                    {
                        lc = _linkageControls[key];
                    }
                    else if (newLinkageControls.ContainsKey(key))
                    {
                        lc = newLinkageControls[key];
                    }
                    else
                    {
                        lc = new LinkageControl(tileStresstest.SlaveIP, tileStresstest.SlavePort);
                        lc.Tag = key;
                        lc.Height = 200;

                        lc.CollapsedChanged += new EventHandler(lc_CollapsedChanged);
                        lc.StateChanged += new EventHandler(lc_StateChanged);
                    }

                    if (!newLinkageControls.ContainsKey(key))
                    {
                        lc.Label = string.Empty;
                        newLinkageControls.Add(key, lc);
                        lc.Unlock();
                    }

                    lc.Label += Environment.NewLine + tile + " - " +
                        (tileStresstest.Label == string.Empty ?
                        tileStresstest.Name + " " + tileStresstest.Index :
                        tileStresstest.Name + " " + tileStresstest.Index + ": " + tileStresstest.Label)
                        +
                        (btnConnectionStrings.Text == "Show Connection Strings" || tileStresstest.Connection.IsEmpty || tileStresstest.Connection.ConnectionString == string.Empty ?
                        string.Empty : " [" + tileStresstest.Connection.ConnectionString + "]" + Environment.NewLine);
                }
            }

            foreach (string key in _linkageControls.Keys)
                if (!newLinkageControls.ContainsKey(key))
                    _linkageControls[key].RegisterForKill();

            _linkageControls = newLinkageControls;

            foreach (string key in _linkageControls.Keys)
                flp.Controls.Add(_linkageControls[key]);

            LockWindowUpdate(0);

            SetStates();
        }

        private void ResetPBValue(int maximum)
        {
            this.Cursor = Cursors.WaitCursor;

            pb.Value = 0;
            pb.Maximum = maximum;

            btnConnectionStrings.Enabled = false;
            btnRefresh.Enabled = false;
            btnJumpStartAll.Enabled = false;
            btnKillAll.Enabled = false;
        }

        #region Set States
        /// <summary>
        /// Starts with a ping.
        /// </summary>
        private void SetStates()
        {
            ResetPBValue(_linkageControls.Count);

            foreach (LinkageControl lc in flp.Controls)
                lc.CheckForSlaves();
        }

        #endregion

        #region Jump Start
        private void btnJumpStartAll_Click(object sender, EventArgs e)
        {
            JumpStartAllSlaves();
        }
        public void JumpStartAllSlaves()
        {
            JumpStartOrKill.Done += new EventHandler(JumpStartOrKill_Done);

            btnJumpStartAll.Enabled = false;
            btnKillAll.Enabled = false;
            foreach (LinkageControl lc in flp.Controls)
                lc.RegisterForJumpStart();
            JumpStartOrKill.Do();
        }
        #endregion

        #region Kill
        private void btnKillAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will kill all slaves on the online computers, even the ones who weren't jump started!", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                KillAllSlaves(false);
        }
        public void KillAllSlaves(bool withProcessID = true)
        {
            JumpStartOrKill.Done += new EventHandler(JumpStartOrKill_Done);
            
            btnJumpStartAll.Enabled = false;
            btnKillAll.Enabled = false;
            foreach (LinkageControl lc in _linkageControls.Values)
                lc.RegisterForKill(withProcessID);
            JumpStartOrKill.Do();
        }
        #endregion

        private void JumpStartOrKill_Done(object sender, EventArgs e)
        {
            JumpStartOrKill.Done -= JumpStartOrKill_Done;

            foreach (LinkageControl lc in flp.Controls)
            {
                lc.StateChanged -= lc_StateChanged;
                lc.CheckForSlaves();
                lc.StateChanged += lc_StateChanged;
            }
        }

        private void btnConnectionStrings_Click(object sender, EventArgs e)
        {
            if (btnConnectionStrings.Text == "Show Connection Strings" &&
                MessageBox.Show("This will also show credentials if any.\nDo you want to proceed?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                btnConnectionStrings.Text = "Hide Connection Strings";
                ConnectionStrings();
            }
            else
            {
                btnConnectionStrings.Text = "Show Connection Strings";
                ConnectionStrings();
            }
        }
        private void ConnectionStrings()
        {
            foreach (LinkageControl lc in flp.Controls)
                lc.Label = string.Empty;

            foreach (BaseItem item in _distributedTest.Tiles)
            {
                Tile tile = item as Tile;
                foreach (BaseItem child in tile)
                {
                    OldTileStresstest tileStresstest = child as OldTileStresstest;
                    string key = tileStresstest.SlaveIP + ':' + tileStresstest.SlavePort;

                    LinkageControl lc = null;
                    if (_linkageControls.ContainsKey(key))
                    {
                        lc = _linkageControls[key];

                        lc.Label += Environment.NewLine + tile + " - " +
                            (tileStresstest.Label == string.Empty ?
                            tileStresstest.Name + ' ' + tileStresstest.Index :
                            tileStresstest.Name + ' ' + tileStresstest.Index + ": " + tileStresstest.Label)
                            +
                            (btnConnectionStrings.Text == "Show Connection Strings" || tileStresstest.Connection.IsEmpty || tileStresstest.Connection.ConnectionString == string.Empty ?
                            string.Empty : " [" + tileStresstest.Connection.ConnectionString + "]" + Environment.NewLine);
                    }
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Startup();
        }

        private void lc_CollapsedChanged(object sender, EventArgs e)
        {
            int collapsed = 0;
            foreach (LinkageControl lc in flp.Controls)
                if (lc.Collapsed)
                    ++collapsed;

            btnCollapseExpand.Text = collapsed == flp.Controls.Count ? "+" : "-";
        }
        private void lc_StateChanged(object sender, EventArgs e)
        {
            lock (_lock)
            {
                if (pb.Value != pb.Maximum)
                    ++pb.Value;

                btnJumpStartAll.Enabled = false;
                btnKillAll.Enabled = false;

                if (pb.Value == pb.Maximum)
                {
                    btnConnectionStrings.Enabled = true;
                    btnRefresh.Enabled = true;

                    foreach (LinkageControl linkageControl in flp.Controls)
                        if (linkageControl.__State == LinkageControl.State.OnlineComputer)
                            btnJumpStartAll.Enabled = true;
                        else if (linkageControl.__State == LinkageControl.State.OnlineSlave)
                            btnKillAll.Enabled = true;

                    this.Cursor = Cursors.Default;
                }
            }
        }
        private void btnCollapseExpand_Click(object sender, EventArgs e)
        {
            if (btnCollapseExpand.Text == "-")
            {
                btnCollapseExpand.Text = "+";

                foreach (LinkageControl lc in flp.Controls)
                {
                    lc.CollapsedChanged -= lc_CollapsedChanged;
                    lc.Collapsed = true;
                    lc.CollapsedChanged += lc_CollapsedChanged;
                }
            }
            else
            {
                btnCollapseExpand.Text = "-";

                foreach (LinkageControl lc in flp.Controls)
                {
                    lc.CollapsedChanged -= lc_CollapsedChanged;
                    lc.Collapsed = false;
                    lc.CollapsedChanged += lc_CollapsedChanged;
                }
            }
        }
        #endregion
    }
}