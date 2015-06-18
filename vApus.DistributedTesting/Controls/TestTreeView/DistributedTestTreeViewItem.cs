/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using vApus.DistributedTest.Properties;
using vApus.SolutionTree;
using vApus.StressTest;

namespace vApus.DistributedTest {
    [ToolboxItem(false)]
    public partial class DistributedTestTreeViewItem : UserControl, ITreeViewItem {
        #region Events

        /// <summary>
        ///     Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;

        public event EventHandler AddTileClicked;

        #endregion

        #region Fields

        private readonly DistributedTest _distributedTest = new DistributedTest();

        /// <summary>
        ///     Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;

        private DistributedTestMode _distributedTestMode;

        private bool _testStarted;

        #endregion

        #region Properties

        public DistributedTest DistributedTest {
            get { return _distributedTest; }
        }

        #endregion

        #region Constructor

        public DistributedTestTreeViewItem() {
            InitializeComponent();
        }

        public DistributedTestTreeViewItem(DistributedTest distributedTest)
            : this() {
            _distributedTest = distributedTest;

            chkUseRDP.Checked = _distributedTest.UseRDP;
            chkUseRDP.CheckedChanged += chkUseRDP_CheckedChanged;

            cboRunSync.SelectedIndex = (int)_distributedTest.RunSynchronization;
            cboRunSync.SelectedIndexChanged += cboRunSync_SelectedIndexChanged;

            nudMaxBreakOnLast.Value = _distributedTest.MaxRerunsBreakOnLast;
            nudMaxBreakOnLast.Visible = cboRunSync.SelectedIndex == 2;
            nudMaxBreakOnLast.ValueChanged += nudMaxBreakOnLast_ValueChanged;
        }

        #endregion

        #region Functions

        public void Unfocus() {
            BackColor = Color.Transparent;
        }

        public void SetVisibleControls() {
        }

        public void SetVisibleControls(bool visible) {
        }

        public void RefreshGui() {
        }

        public void SetDistributedTestMode(DistributedTestMode distributedTestMode) {
            _distributedTestMode = distributedTestMode;
            if (_distributedTestMode == DistributedTestMode.Edit) {
                chkUseRDP.Enabled =
                    pnlRunSync.Enabled =
                    nudMaxBreakOnLast.Enabled =
                    picAddTile.Visible = true;

                picStressTestStatus.Visible = false;
            } else {
                chkUseRDP.Enabled =
                    pnlRunSync.Enabled =
                    nudMaxBreakOnLast.Enabled =
                    picAddTile.Visible = false;

                _testStarted = false;

                picStressTestStatus.Image = Resources.Wait;
                toolTip.SetToolTip(picStressTestStatus, string.Empty);

                picStressTestStatus.Visible = true;
            }
        }
        /// <summary>
        /// Only call this if the tile stress test has monitors.
        /// </summary>
        public void SetMonitoringBeforeAfter() {
            picStressTestStatus.Image = Resources.Busy;
            toolTip.SetToolTip(picStressTestStatus, "Busy monitoring.");
        }

        private void chkUseRDP_CheckedChanged(object sender, EventArgs e) {
            _distributedTest.UseRDP = chkUseRDP.Checked;
            _distributedTest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        private void cboRunSync_SelectedIndexChanged(object sender, EventArgs e) {
            _distributedTest.RunSynchronization = (RunSynchronization)cboRunSync.SelectedIndex;
            _distributedTest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        private void nudMaxBreakOnLast_ValueChanged(object sender, EventArgs e) {
            _distributedTest.MaxRerunsBreakOnLast = (int)nudMaxBreakOnLast.Value;
            _distributedTest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        private void _Enter(object sender, EventArgs e) {
            BackColor = SystemColors.Control;
            if (AfterSelect != null)
                AfterSelect(this, null);
        }

        private void picAddTile_Click(object sender, EventArgs e) {
            Focus();
            if (AddTileClicked != null)
                AddTileClicked(this, null);
        }

        private void _KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.ControlKey)
                _ctrl = true;
        }

        private void _KeyUp(object sender, KeyEventArgs e) {
            if (_ctrl && e.KeyCode == Keys.I && AddTileClicked != null)
                AddTileClicked(this, null);
        }

        public void SetStressTestStarted() {
            //Keep the running if this code at a minimum
            if (!_testStarted) {
                _testStarted = true;

                picStressTestStatus.Image = Resources.Busy;
                foreach (Tile t in _distributedTest.Tiles)
                    if (t.Use)
                        foreach (TileStressTest ts in t)
                            if (ts.Use && ts.BasicTileStressTest.MonitorIndices.Length != 0) {
                                toolTip.SetToolTip(picStressTestStatus, "Busy stress testing and monitoring.");
                                return;
                            }

                toolTip.SetToolTip(picStressTestStatus, "Busy stress testing.");
            }
        }

        #endregion
    }
}