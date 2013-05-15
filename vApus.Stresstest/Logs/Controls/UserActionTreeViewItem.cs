/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Stresstest {
    [ToolboxItem(false)]
    public partial class UserActionTreeViewItem : UserControl {

        #region Events

        /// <summary>
        ///     Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;

        public event EventHandler<LogTreeView.AddUserActionEventArgs> DuplicateClicked;
        public event EventHandler DeleteClicked;

        #endregion

        #region Fields
        private static Color _selectedColor = Color.FromArgb(255, 240, 240, 240);
        private static Color _primaryColor = Color.FromArgb(255, 250, 250, 250);
        private static Color _secundaryColor = Color.FromArgb(255, 255, 255, 255);

        /// <summary>
        ///     Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;
        private Log _log;
        private UserAction _userAction;
        #endregion

        public UserAction UserAction {
            get { return _userAction; }
        }

        #region Constructors

        public UserActionTreeViewItem() {
            InitializeComponent();
        }
        public UserActionTreeViewItem(Log log, UserAction userAction)
            : this() {
            _log = log;
            _userAction = userAction;
            SetLabel();

            SetPicValid();
            _log.LexicalResultChanged += _log_LexicalResultChanged;
        }
        #endregion

        #region Functions
        public void SetLabel() {
            lblUserAction.Text = _userAction.ToString(); // +" (" + _userAction.Count + ")";
        }

        private void _log_LexicalResultChanged(object sender, Log.LexicalResultsChangedEventArgs e) {
            SetPicValid();
        }
        private void SetPicValid() {
            var lexicalResult = LexicalResult.OK;

            foreach (LogEntry logEntry in _userAction)
                if (logEntry.LexicalResult == LexicalResult.Error) {
                    lexicalResult = LexicalResult.Error;
                    break;
                }

            picValid.Image = lexicalResult == LexicalResult.OK ? null : global::vApus.Stresstest.Properties.Resources.LogEntryError;
        }

        public void Unfocus() {
            BackColor = Color.Transparent;
            SetVisibleControls();
        }
        public new void Focus() {
            base.Focus();
            BackColor = _selectedColor;
            SetVisibleControls();

            if (AfterSelect != null) AfterSelect(this, null);
        }
        public void SetVisibleControls(bool visible) {
            SetLabel();
            UserAction linkedUserAction;
            if (_userAction.IsLinked(out linkedUserAction)) {
                picLinkColor.BackColor = _userAction.LinkColor;
                picLinkColor.Visible = true;
            } else {
                picLinkColor.Visible = false;
            }

            picDuplicate.Visible = picDelete.Visible = visible;
            picPin.Visible = _userAction.Pinned || visible;
            picPin.Image = _userAction.Pinned ? global::vApus.Stresstest.Properties.Resources.Pin : global::vApus.Stresstest.Properties.Resources.PinGreyedOut;
            nudOccurance.Visible = _userAction.Occurance != 1 || visible;
            nudOccurance.ValueChanged -= nudOccurance_ValueChanged;
            nudOccurance.Value = _userAction.Occurance;
            nudOccurance.ValueChanged += nudOccurance_ValueChanged;

            if (BackColor != _selectedColor)
                BackColor = (((float)_userAction.Index % 2) == 0) ? _secundaryColor : _primaryColor;

            Control ctrl = null;
            if (picLinkColor.Visible) ctrl = picLinkColor;
            else if (picDuplicate.Visible) ctrl = picDuplicate;
            else ctrl = nudOccurance;

            int width = ctrl.Left - lblUserAction.Left;
            if (width != lblUserAction.Width)
                lblUserAction.Width = width;
        }
        public void SetVisibleControls() {
            if (IsDisposed) return;

            if (BackColor == _selectedColor) SetVisibleControls(true);
            else SetVisibleControls(ClientRectangle.Contains(PointToClient(Cursor.Position)));
        }

        private void _Enter(object sender, EventArgs e) {
            Focus();
        }

        private void _MouseEnter(object sender, EventArgs e) { SetVisibleControls(); }

        private void _MouseLeave(object sender, EventArgs e) { SetVisibleControls(); }

        private void _KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.ControlKey)
                _ctrl = false;
            else if (_ctrl) {
                if (e.KeyCode == Keys.R)
                    picDelete_Click(picDelete, null);
                else if (e.KeyCode == Keys.D)
                    picDuplicate_Click(picDuplicate, null);
            }
        }

        private void _KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.ControlKey) _ctrl = true;
        }

        private void picDuplicate_Click(object sender, EventArgs e) {
            var ua = _userAction.Clone();
            int index = _userAction.Index;

            if (index < _log.Count) _log.InsertWithoutInvokingEvent(index, ua); else _log.AddWithoutInvokingEvent(ua);

            //Add to the link if any
            UserAction linkedUserAction;
            if (_userAction.IsLinked(out linkedUserAction)) {
                linkedUserAction.LinkedToUserActionIndices.Add(linkedUserAction.LinkedToUserActionIndices[linkedUserAction.LinkedToUserActionIndices.Count - 1] + 1);
                ua.LinkColorRGB = linkedUserAction.LinkColorRGB;
                linkedUserAction.LinkedToUserActionIndices.Sort();

                index = linkedUserAction.LinkedToUserActionIndices[linkedUserAction.LinkedToUserActionIndices.Count - 1];
            }

            //Update the linked indices for the other user actions.
            for (int i = index; i != _log.Count; i++) {
                var ua2 = _log[i] as UserAction;
                var linkedIndices = ua2.LinkedToUserActionIndices.ToArray();
                for (int j = 0; j != linkedIndices.Length; j++)
                    ua2.LinkedToUserActionIndices[j] = linkedIndices[j] + 1;
            }

            _log.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
            _log.ApplyLogRuleSet();
            if (DuplicateClicked != null) DuplicateClicked(this, new LogTreeView.AddUserActionEventArgs(ua));
        }

        private void picDelete_Click(object sender, EventArgs e) {
            int index = _userAction.Index - 1;
            //Remove from the link if any
            UserAction linkedUserAction;
            if (_userAction.IsLinked(out linkedUserAction))
                linkedUserAction.RemoveFromLink(_userAction);

            _log.Remove(_userAction);

            //Update the linked indices for the other user actions.
            for (int i = index; i != _log.Count; i++) {
                var ua = _log[i] as UserAction;
                var linkedIndices = ua.LinkedToUserActionIndices.ToArray();
                for (int j = 0; j != linkedIndices.Length; j++)
                    ua.LinkedToUserActionIndices[j] = linkedIndices[j] - 1;
            }

            _log.ApplyLogRuleSet();
            if (DeleteClicked != null) DeleteClicked(this, null);
        }

        private void picPin_Click(object sender, EventArgs e) {
            _userAction.Pinned = !_userAction.Pinned;
            picPin.Image = _userAction.Pinned ? global::vApus.Stresstest.Properties.Resources.Pin : global::vApus.Stresstest.Properties.Resources.PinGreyedOut;

            UserAction linkUserAction;
            if (_userAction.IsLinked(out linkUserAction)) {
                linkUserAction.Pinned = _userAction.Pinned;
                foreach (var ua in linkUserAction.LinkedToUserActions)
                    ua.Pinned = _userAction.Pinned;
            }
            _userAction.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        private void nudOccurance_ValueChanged(object sender, EventArgs e) {
            _userAction.Occurance = (int)nudOccurance.Value;
            _userAction.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        #endregion
    }
}