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

namespace vApus.DistributedTesting
{
    [ToolboxItem(false)]
    public partial class DistributedTestTreeViewItem : UserControl, ITestTreeViewItem
    {
        #region Events
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

        private void cboRunSync_SelectedIndexChanged(object sender, EventArgs e)
        {
            _distributedTest.RunSynchronization = (Stresstest.RunSynchronization)cboRunSync.SelectedIndex;
            _distributedTest.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        #endregion

        #region Functions
        public void SetVisibleControls()
        { }
        public void SetVisibleControls(bool visible)
        { }
        public void RefreshGui()
        { }

        private void picAddTile_Click(object sender, EventArgs e)
        {
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

        private void _Enter(object sender, EventArgs e)
        {
            if (AfterSelect != null)
                AfterSelect(this, null);
        }
    }
}
