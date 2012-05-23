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

namespace vApus.DistributedTesting
{
    [ToolboxItem(false)]
    public partial class SlaveCollectionTreeViewItem : UserControl, ITreeViewItem
    {
        #region Events
        /// <summary>
        /// Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;
        public event EventHandler AddClientClicked;
        #endregion

        #region Fields
        /// <summary>
        /// Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;
        #endregion

        #region Constructor
        public SlaveCollectionTreeViewItem()
        {
            InitializeComponent();
        }
        #endregion

        #region Functions
        private void _Enter(object sender, EventArgs e)
        {
            if (AfterSelect != null)
                AfterSelect(this, null);
        }
        public void Unfocus()
        {
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
            if (AddClientClicked != null)
                AddClientClicked(this, null);
        }
        private void _KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
                _ctrl = true;
        }
        private void _KeyUp(object sender, KeyEventArgs e)
        {
            if (_ctrl && e.KeyCode == Keys.I && AddClientClicked != null)
                AddClientClicked(this, null);

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
