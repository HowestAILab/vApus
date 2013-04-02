/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.Stresstest {
    [ToolboxItem(false)]
    public partial class LogTreeViewItem : UserControl {
        #region Events

        /// <summary>
        ///     Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;

        public event EventHandler AddUserActionClicked;

        #endregion

        #region Fields


        /// <summary>
        ///     Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;
        private readonly Log _log;
        private bool _testStarted;

        #endregion

        #region Constructor

        public LogTreeViewItem() {
            InitializeComponent();
        }

        public LogTreeViewItem(Log log)
            : this() {
                _log = log;
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

        private void _Enter(object sender, EventArgs e) {
            BackColor = SystemColors.Control;
            if (AfterSelect != null)
                AfterSelect(this, null);
        }

        private void picAddTile_Click(object sender, EventArgs e) {
            Focus();
            if (AddUserActionClicked != null)
                AddUserActionClicked(this, null);
        }

        private void _KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.ControlKey)
                _ctrl = true;
        }

        private void _KeyUp(object sender, KeyEventArgs e) {
            if (_ctrl && e.KeyCode == Keys.I && AddUserActionClicked != null)
                AddUserActionClicked(this, null);
        }

        #endregion
    }
}