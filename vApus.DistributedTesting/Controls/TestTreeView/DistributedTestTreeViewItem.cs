/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

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
            cboRunSync.SelectedIndex = (int)distributedTest.RunSynchronization;

            cboRunSync.SelectedIndexChanged += new EventHandler(cboRunSync_SelectedIndexChanged);
        }

        #endregion

        #region Functions
        private void cboRunSync_SelectedIndexChanged(object sender, EventArgs e)
        {
            _distributedTest.RunSynchronization = (Stresstest.RunSynchronization)cboRunSync.SelectedIndex;
            _distributedTest.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);

            lblRunSync.Text = "" + cboRunSync.SelectedItem;
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
        private void cboRunSync_Leave(object sender, EventArgs e)
        {
            //no tostring for if cborun has no selected item; 
            lblRunSync.Text = "" + cboRunSync.SelectedItem;
        }
        private void _MouseEnter(object sender, EventArgs e)
        {
            SetVisibleControls();
        }
        private void _MouseLeave(object sender, EventArgs e)
        {
            SetVisibleControls();
        }
        public void SetVisibleControls()
        {
            if (this.BackColor == SystemColors.Control)
                SetVisibleControls(true);
            else
                SetVisibleControls(ClientRectangle.Contains(PointToClient(Cursor.Position)));
        }
        public void SetVisibleControls(bool visible)
        {
            pnlRunSync.Visible = picAddTile.Visible = visible;
        }
        public void RefreshGui()
        {
            //no tostring for if cborun has no selected item; 
            string label = "" + cboRunSync.SelectedItem;
            if (lblRunSync.Text != label)
                lblRunSync.Text = label;
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
        #endregion


        public void SetDistributedTestMode(DistributedTestMode distributedTestMode)
        {
        }

        public DistributedTestMode DistributedTestMode
        {
            get { throw new NotImplementedException(); }
        }
    }
}
