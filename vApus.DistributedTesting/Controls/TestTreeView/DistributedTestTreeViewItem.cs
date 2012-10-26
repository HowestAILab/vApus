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
using System.IO;
using System.Windows.Forms;

namespace vApus.DistributedTesting
{
    [ToolboxItem(false)]
    public partial class DistributedTestTreeViewItem : UserControl, ITreeViewItem
    {
        #region Events
        /// <summary>
        /// Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;
        public event EventHandler AddTileClicked;
        #endregion

        #region Fields
        private DistributedTest _distributedTest = new DistributedTest();
        /// <summary>
        /// Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;

        private DistributedTestMode _distributedTestMode;

        private bool _testStarted;

        #endregion

        #region Properties
        public DistributedTest DistributedTest
        {
            get { return _distributedTest; }
        }
        #endregion

        #region Constructor
        public DistributedTestTreeViewItem()
        {
            InitializeComponent();
        }
        public DistributedTestTreeViewItem(DistributedTest distributedTest)
            : this()
        {
            _distributedTest = distributedTest;

            chkUseRDP.Checked = _distributedTest.UseRDP;
            chkUseRDP.CheckedChanged += new EventHandler(chkUseRDP_CheckedChanged);

            toolTip.SetToolTip(picResultPath, "Result Path: " + _distributedTest.ResultPath);
        }
        #endregion

        #region Functions
        private void chkUseRDP_CheckedChanged(object sender, EventArgs e)
        {
            _distributedTest.UseRDP = chkUseRDP.Checked;
            _distributedTest.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        private void _Enter(object sender, EventArgs e)
        {
            this.BackColor = SystemColors.Control;
            if (AfterSelect != null)
                AfterSelect(this, null);
        }
        public void Unfocus()
        {
            this.BackColor = Color.Transparent;
        }
        public void SetVisibleControls()
        {
        }
        public void SetVisibleControls(bool visible)
        {
        }
        public void RefreshGui()
        {
        }

        private void picAddTile_Click(object sender, EventArgs e)
        {
            this.Focus();
            if (AddTileClicked != null)
                AddTileClicked(this, null);
        }
        private void _KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
                _ctrl = true;
        }
        private void _KeyUp(object sender, KeyEventArgs e)
        {
            if (_ctrl && e.KeyCode == Keys.I && AddTileClicked != null)
                AddTileClicked(this, null);

        }

        private void picResultPath_Click(object sender, EventArgs e)
        {
            if (_distributedTestMode == DistributedTestMode.TestAndReport)
                return;

            if (Directory.Exists(_distributedTest.ResultPath))
                folderBrowserDialog.SelectedPath = _distributedTest.ResultPath;
            else
                folderBrowserDialog.SelectedPath = Application.StartupPath;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK &&
                _distributedTest.ResultPath != folderBrowserDialog.SelectedPath)
            {
                _distributedTest.ResultPath = folderBrowserDialog.SelectedPath;
                toolTip.SetToolTip(picResultPath, "Result Path: " + _distributedTest.ResultPath);


                _distributedTest.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }

        public void SetStresstestStarted()
        {
            //Keep the running if this code at a minimum
            if (!_testStarted)
            {
                _testStarted = true;

                picStresstestStatus.Image = vApus.DistributedTesting.Properties.Resources.Busy;
                toolTip.SetToolTip(picStresstestStatus, "Busy Stresstesting");
            }
        }

        public void SetDistributedTestMode(DistributedTestMode distributedTestMode)
        {
            _distributedTestMode = distributedTestMode;
            if (_distributedTestMode == DistributedTestMode.Edit)
            {
                chkUseRDP.Enabled =
                picAddTile.Visible = true;

                picStresstestStatus.Visible = false;
            }
            else
            {
                chkUseRDP.Enabled =
                picAddTile.Visible = false;

                _testStarted = false;

                picStresstestStatus.Image = vApus.DistributedTesting.Properties.Resources.Wait;
                toolTip.SetToolTip(picStresstestStatus, string.Empty);

                picStresstestStatus.Visible = true;
            }
        }
        #endregion
    }
}
