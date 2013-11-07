/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    /// <summary>
    /// Properties of a user action can be set here, more advanced stuff and viewing log entries happens in EditUserAction.
    /// </summary>
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
        private readonly object _lock = new object();

        //Primary and secundary color for readability.
        private static Color _selectBackColor = Color.FromArgb(255, 240, 240, 240), _primaryBackColor = Color.FromArgb(255, 250, 250, 250), _secundaryBackColor = Color.FromArgb(255, 255, 255, 255);

        /// <summary>
        ///     Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrlKeyPressed;
        private Log _log;
        #endregion

        #region Properties
        public UserAction UserAction { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Design time constructor, for testing.
        /// </summary>
        public UserActionTreeViewItem() { InitializeComponent(); }
        /// <summary>
        /// Properties of a user action can be set here, more advanced stuff and viewing log entries happens in EditUserAction.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="userAction"></param>
        public UserActionTreeViewItem(Log log, UserAction userAction)
            : this() {
            _log = log;
            UserAction = userAction;
            SetLabel();

            SetPicValid();
            _log.LexicalResultChanged += _log_LexicalResultChanged;
        }
        #endregion

        #region Functions
        /// <summary>
        /// UserAction.ToString();
        /// </summary>
        internal void SetLabel() { lblUserAction.Text = UserAction.ToString(); }

        private void _log_LexicalResultChanged(object sender, Log.LexicalResultsChangedEventArgs e) { SetPicValid(); }
        private void SetPicValid() {
            var lexicalResult = LexicalResult.OK;

            //For a big log it is best that we do this in parallel.
            Parallel.ForEach(UserAction as ICollection<BaseItem>, (logEntry, loopState) => {
                if ((logEntry as LogEntry).LexicalResult == LexicalResult.Error)
                    lock (_lock) {
                        lexicalResult = LexicalResult.Error;
                        loopState.Break();
                    }
            });

            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                picValid.Image = lexicalResult == LexicalResult.OK ? null : global::vApus.Stresstest.Properties.Resources.LogEntryError;
            }, null);
        }

        public void Unfocus() {
            BackColor = Color.Transparent;
            SetVisibleControls();
        }
        public new void Focus() {
            base.Focus();
            BackColor = _selectBackColor;
            SetVisibleControls();

            if (AfterSelect != null) AfterSelect(this, null);
        }
        public void SetVisibleControls(bool visible) {
            SetLabel();
            UserAction linkedUserAction;
            if (UserAction.IsLinked(out linkedUserAction)) {
                picLinkColor.BackColor = UserAction.LinkColor;
                picLinkColor.Visible = true;
            } else {
                picLinkColor.Visible = false;
            }

            picDuplicate.Visible = picDelete.Visible = visible;
            picPin.Visible = UserAction.Pinned || visible;
            picPin.Image = UserAction.Pinned ? global::vApus.Stresstest.Properties.Resources.Pin : global::vApus.Stresstest.Properties.Resources.PinGreyedOut;
            nudOccurance.Visible = UserAction.Occurance != 1 || visible;
            nudOccurance.ValueChanged -= nudOccurance_ValueChanged;
            nudOccurance.Value = UserAction.Occurance;
            nudOccurance.ValueChanged += nudOccurance_ValueChanged;

            if (BackColor != _selectBackColor)
                BackColor = (((float)UserAction.Index % 2) == 0) ? _secundaryBackColor : _primaryBackColor;

            Control ctrl = null;
            if (picLinkColor.Visible) ctrl = picLinkColor;
            else if (picDuplicate.Visible) ctrl = picDuplicate;
            else ctrl = nudOccurance;

            int width = ctrl.Left - lblUserAction.Left;
            if (width != lblUserAction.Width) lblUserAction.Width = width;
        }
        public void SetVisibleControls() {
            if (IsDisposed) return;

            if (BackColor == _selectBackColor) SetVisibleControls(true);
            else SetVisibleControls(ClientRectangle.Contains(PointToClient(Cursor.Position)));
        }

        private void _Enter(object sender, EventArgs e) { Focus(); }
        private void _MouseEnter(object sender, EventArgs e) { SetVisibleControls(); }
        private void _MouseLeave(object sender, EventArgs e) { SetVisibleControls(); }

        private void picDuplicate_Click(object sender, EventArgs e) {
            var ua = UserAction.Clone(_log.LogRuleSet, true, true, false);
            int index = UserAction.Index;

            if (index < _log.Count) _log.InsertWithoutInvokingEvent(index, ua); else _log.AddWithoutInvokingEvent(ua);

            //Add to the link if any
            UserAction linkedUserAction;
            if (UserAction.IsLinked(out linkedUserAction)) {
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
            int index = UserAction.Index - 1;
            //Remove from the link if any
            UserAction linkedUserAction;
            if (UserAction.IsLinked(out linkedUserAction))
                linkedUserAction.RemoveFromLink(UserAction);

            _log.Remove(UserAction);

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

        private void nudOccurance_ValueChanged(object sender, EventArgs e) {
            UserAction.Occurance = (int)nudOccurance.Value;
            UserAction.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        private void picPin_Click(object sender, EventArgs e) {
            UserAction.Pinned = !UserAction.Pinned;
            picPin.Image = UserAction.Pinned ? global::vApus.Stresstest.Properties.Resources.Pin : global::vApus.Stresstest.Properties.Resources.PinGreyedOut;

            UserAction linkUserAction;
            if (UserAction.IsLinked(out linkUserAction)) {
                linkUserAction.Pinned = UserAction.Pinned;
                foreach (var ua in linkUserAction.LinkedToUserActions)
                    ua.Pinned = UserAction.Pinned;
            }
            UserAction.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        private void _KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.ControlKey)
                _ctrlKeyPressed = false;
            else if (_ctrlKeyPressed) {
                if (e.KeyCode == Keys.R)
                    picDelete_Click(picDelete, null);
                else if (e.KeyCode == Keys.D)
                    picDuplicate_Click(picDuplicate, null);
            }
        }
        private void _KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.ControlKey) _ctrlKeyPressed = true; }
        #endregion
    }
}