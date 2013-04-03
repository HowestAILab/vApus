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
using System.Net;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    [ToolboxItem(false)]
    public partial class UserActionTreeViewItem : UserControl {

        #region Events

        /// <summary>
        ///     Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;

        public event EventHandler ActionizeClicked;
        public event EventHandler DuplicateClicked;
        public event EventHandler DeleteClicked;

        #endregion

        #region Fields
        private static Color _primaryColor = Color.FromArgb(255, 250, 250, 250);
        private static Color _secundaryColor = Color.FromArgb(255, 255, 255, 255);

        /// <summary>
        ///     Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;
        private UserAction _userAction;

        #endregion

        #region Constructors

        public UserActionTreeViewItem() {
            InitializeComponent();
        }
        public UserActionTreeViewItem(UserAction userAction)
            : this() {
            _userAction = userAction;
            lblUserAction.Text = _userAction.ToString();
        }
        #endregion

        #region Functions

        public void Unfocus() {
            BackColor = Color.Transparent;
            SetVisibleControls();
        }

        public void SetVisibleControls(bool visible) {
            picActionize.Visible = picDuplicate.Visible = picDelete.Visible = visible;
            picPin.Visible = _userAction.Pinned || visible;
            picPin.Image = _userAction.Pinned ? global::vApus.Stresstest.Properties.Resources.Pin : global::vApus.Stresstest.Properties.Resources.PinGreyedOut;
            nudOccurance.Visible = _userAction.Occurance != 1 || visible;
            nudOccurance.Value = _userAction.Occurance;

            if (BackColor != SystemColors.Control)
                BackColor = (((float)_userAction.Index % 2) == 0) ? _secundaryColor : _primaryColor;
        }

        public void SetVisibleControls() {
            if (IsDisposed) return;

            if (BackColor == SystemColors.Control) SetVisibleControls(true);
            else SetVisibleControls(ClientRectangle.Contains(PointToClient(Cursor.Position)));
        }

        private void _Enter(object sender, EventArgs e) {
            BackColor = SystemColors.Control;
            SetVisibleControls();

            if (AfterSelect != null) AfterSelect(this, null);
        }

        private void _MouseEnter(object sender, EventArgs e) { SetVisibleControls(); }

        private void _MouseLeave(object sender, EventArgs e) { SetVisibleControls(); }

        private void _KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.ControlKey)
                _ctrl = false;
            else if (_ctrl) {
                if (e.KeyCode == Keys.R && DeleteClicked != null)
                    DeleteClicked(this, null);
                else if (e.KeyCode == Keys.D && DuplicateClicked != null)
                    DuplicateClicked(this, null);
            }
        }

        private void _KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.ControlKey) _ctrl = true;
        }

        private void picDuplicate_Click(object sender, EventArgs e) {
            if (DuplicateClicked != null) DuplicateClicked(this, null);
        }

        private void picDelete_Click(object sender, EventArgs e) {
            if (DeleteClicked != null) DeleteClicked(this, null);
        }

        private void picActionize_Click(object sender, EventArgs e) {
            Focus();
            if (ActionizeClicked != null) ActionizeClicked(this, null);
        }

        #endregion

        private void nudOccurance_ValueChanged(object sender, EventArgs e) {
            _userAction.Occurance = (int)nudOccurance.Value;
        }

        private void picPin_Click(object sender, EventArgs e) {
            _userAction.Pinned = !_userAction.Pinned;
            picPin.Image = _userAction.Pinned ? global::vApus.Stresstest.Properties.Resources.Pin : global::vApus.Stresstest.Properties.Resources.PinGreyedOut;
        }
    }
}