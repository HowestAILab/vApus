/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Stresstest.Properties;

namespace vApus.Stresstest {
    public partial class UserActionControl : LogChildControlBase {
        public event EventHandler CollapsedChanged;

        #region Fields

        private readonly List<LogEntryControl> _logEntryControls = new List<LogEntryControl>();
        private readonly UserAction _userAction;

        #endregion

        #region Properties

        public override BaseItem LogChild {
            get { return _userAction; }
        }

        public override bool Checked {
            get { return chkIndex.Checked; }
            set { chkIndex.Checked = value; }
        }

        public CheckState CheckState {
            get { return chkIndex.CheckState; }
            set { chkIndex.CheckState = value; }
        }

        public bool Collapsed {
            get { return btnCollapseExpand.Text == "+"; }
            set {
                if (Collapsed != value) {
                    btnCollapseExpand.Text = value ? "+" : "-";
                    if (CollapsedChanged != null)
                        CollapsedChanged(this, null);
                }
            }
        }

        public List<LogEntryControl> LogEntryControls {
            get { return _logEntryControls; }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     An empty user action control, aught to use in design time only.
        /// </summary>
        public UserActionControl() {
            InitializeComponent();
        }

        /// <summary>
        ///     Visualizes a user action.
        /// </summary>
        /// <param name="userAction"></param>
        public UserActionControl(UserAction userAction) {
            InitializeComponent();
            _userAction = userAction;
            nudOccurance.Value = _userAction.Occurance;

            if (_userAction.Label != string.Empty) {
                txtUserAction.ForeColor = SystemColors.WindowText;
                txtUserAction.Text = _userAction.Label;
            }

            SetPicPin();
        }

        #endregion

        private void nudOccurance_ValueChanged(object sender, EventArgs e) {
            if (_userAction.Occurance != nudOccurance.Value) {
                _userAction.Occurance = (int)nudOccurance.Value;
                _userAction.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }

        private void picPin_Click(object sender, EventArgs e) {
            _userAction.Pinned = !_userAction.Pinned;
            _userAction.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            SetPicPin();
        }

        private void SetPicPin() { picPin.Image = _userAction.Pinned ? Resources.Pin : Resources.PinGreyedOut; }

        private void chkIndex_CheckedChanged(object sender, EventArgs e) { InvokeCheckedChanged(); }

        /// <summary>
        ///     Clears the user action. No events will be invoked, you should do this yourself afterwards.
        /// </summary>
        public override void ClearLogChild() { _userAction.ClearWithoutInvokingEvent(); }

        private void txtUserAction_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && _userAction.Label != txtUserAction.Text) {
                txtUserAction.Text = txtUserAction.Text.Trim();
                if (_userAction.Label != txtUserAction.Text) {
                    _userAction.Label = txtUserAction.Text;
                    _userAction.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                }
            }
        }

        private void txtUserAction_Enter(object sender, EventArgs e) {
            if (_userAction.Label == string.Empty) {
                txtUserAction.ForeColor = SystemColors.WindowText;
                txtUserAction.Text = string.Empty;
            }
        }

        private void txtUserAction_Leave(object sender, EventArgs e) {
            txtUserAction.Text = txtUserAction.Text.Trim();
            if (_userAction.Label != txtUserAction.Text) {
                _userAction.Label = txtUserAction.Text;
                var tmr = new System.Windows.Forms.Timer() { Interval = 100 }; //Don't like it, but otherwise gui freeze.
                tmr.Tick += tmr_Tick;
                tmr.Start();
            }
            if (txtUserAction.Text == string.Empty) {
                txtUserAction.Text = "Give this user action a label.";
                txtUserAction.ForeColor = SystemColors.ControlDark;
            }
        }
        private void tmr_Tick(object sender, EventArgs e) {
            try {
                (sender as System.Windows.Forms.Timer).Stop();
                _userAction.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            } catch { }
        }

        private void btnCollapseExpand_Click(object sender, EventArgs e) { Collapsed = !Collapsed; }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            chkIndex.Text = _userAction.Index.ToString();
            lblCount.Text = "Contains " + _userAction.Count + ((_userAction.Count == 1) ? " Log Entry" : " Log Entries");
        }

        protected override void Select(bool directed, bool forward) {
            base.Select(directed, forward);
            txtUserAction.Focus();
            txtUserAction.Select();
        }
    }
}