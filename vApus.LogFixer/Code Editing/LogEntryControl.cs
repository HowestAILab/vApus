/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;
using vApus.SolutionTree;
using System.Drawing;

namespace vApus.Stresstest
{
    public partial class LogEntryControl : LogChildControlBase
    {
        #region Events
        public event EventHandler LexicalResultChanged;
        public event EventHandler Removed;
        #endregion

        #region Fields
        public const int INDENTATIONOFFSET = 16;
        private UserActionControl _userActionControl;
        private LogEntry _logEntry;
        private uint _indentationLevel;
        #endregion

        #region Properties
        /// <summary>
        /// The base item that it holds, in this case a log entry.
        /// </summary>
        public override BaseItem LogChild
        {
            get { return _logEntry; }
        }
        public override uint IndentationLevel
        {
            get { return _indentationLevel; }
        }
        public override bool Checked
        {
            get { return chkIndex.Checked; }
            set { chkIndex.Checked = value; }
        }
        public LexicalResult LexicalResult
        {
            get { return _logEntry.LexicalResult; }
        }
        public bool Collapsed
        {
            get { return splitContainer.Panel2Collapsed; }
            set
            {
                splitContainer.Panel2Collapsed = value;
                if (splitContainer.Panel2Collapsed)
                {
                    btnCollapseExpand.Text = "+";
                    this.Height = 30;
                }
                else
                {
                    btnCollapseExpand.Text = "-";
                    this.Height = 200;
                }
            }
        }

        public UserActionControl UserActionControl
        {
            get { return _userActionControl; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// An empty log entry control, aught to use only when designing.
        /// </summary>
        public LogEntryControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Visualizez a log entry.
        /// </summary>
        public LogEntryControl(LogEntry logEntry)
        {
            InitializeComponent();
            _logEntry = logEntry;
            _logEntry.LexicalResultChanged += new EventHandler(_logEntry_LexicalResultChanged);
            SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);

            txtScrollingLogEntry.Text = _logEntry.LogEntryString;
            txtLogEntry.Text = _logEntry.LogEntryString;

            //Backwards compatible.
            if (_logEntry.Parent is UserAction)
                _logEntry.Pinned = true;

            nudOccurance.Value = _logEntry.Occurance;
            SetImages();
            Collapsed = true;

            if (LexicalResultChanged != null)
                LexicalResultChanged(this, null);

            this.SizeChanged += new EventHandler(LogEntryControl_SizeChanged);
        }
        #endregion

        #region Functions
        private void LogEntryControl_SizeChanged(object sender, EventArgs e)
        {
            if (Visible && splitContainer.Panel2Collapsed)
            {
                this.SizeChanged -= LogEntryControl_SizeChanged;
                splitContainer.Panel2Collapsed = false;
                splitContainer.Panel2Collapsed = true;
                this.SizeChanged += LogEntryControl_SizeChanged;
            }
        }
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            if (sender == _logEntry
            || (sender is Log && (sender == _logEntry.Parent
            || (_logEntry.Parent is UserAction && sender == (_logEntry.Parent as UserAction).Parent))))
            {
                SetImages();
                txtScrollingLogEntry.Text = _logEntry.LogEntryString;
                txtLogEntry.Text = _logEntry.LogEntryString;
            }
        }
        private void _logEntry_LexicalResultChanged(object sender, EventArgs e)
        {
            SetImages();
            if (LexicalResultChanged != null)
                LexicalResultChanged(this, null);
        }
        private void SetImages()
        {
            switch (_logEntry.LexicalResult)
            {
                case LexicalResult.OK:
                    picValidation.Image = global::vApus.Stresstest.Properties.Resources.LogEntryOK;
                    break;
                case LexicalResult.Error:
                    picValidation.Image = global::vApus.Stresstest.Properties.Resources.LogEntryError;
                    break;
            }
            if (_logEntry.ExecuteParallel)
            {
                picParallel.Image = global::vApus.Stresstest.Properties.Resources.Parallel;
                BackColor = Color.FromArgb(144, 238, 144);
            }
            else
            {
                picParallel.Image = global::vApus.Stresstest.Properties.Resources.NotParallel;
                BackColor = SystemColors.Control;
            }
            picParallel.Visible = _logEntry.Parent is UserAction;
            picIgnoreDelay.Image = _logEntry.IgnoreDelay ? global::vApus.Stresstest.Properties.Resources.IgnoreDelay : global::vApus.Stresstest.Properties.Resources.Delay;
            picPin.Image = _logEntry.Pinned ? global::vApus.Stresstest.Properties.Resources.Pin : global::vApus.Stresstest.Properties.Resources.PinGreyedOut;
            picPin.Visible = _logEntry.Parent is Log;
        }
        private void chkIndex_CheckedChanged(object sender, EventArgs e)
        {
            InvokeCheckedChanged();
        }
        private void btnCollapseExpand_Click(object sender, EventArgs e)
        {
            Collapsed = btnCollapseExpand.Text == "-";
        }
        private void picPin_Click(object sender, EventArgs e)
        {
            _logEntry.Pinned = !_logEntry.Pinned;
            _logEntry.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            SetImages();
        }
        private void picIgnoreDelay_Click(object sender, EventArgs e)
        {
            _logEntry.IgnoreDelay = !_logEntry.IgnoreDelay;
            _logEntry.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            SetImages();
        }
        private void picParallel_Click(object sender, EventArgs e)
        {
            _logEntry.ExecuteParallel = !_logEntry.ExecuteParallel;
            _logEntry.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            SetImages();
        }
        private void nudOccurance_ValueChanged(object sender, EventArgs e)
        {
            if (_logEntry.Occurance != nudOccurance.Value)
            {
                _logEntry.Occurance = (int)nudOccurance.Value;
                _logEntry.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            Edit();
        }
        private void toolStripStatusLabelRemove_Click(object sender, EventArgs e)
        {
            if (Removed != null)
                Removed(this, null);
        }
        /// <summary>
        /// Augment the indentlevel by one (or level of dependency) of the container of the control.
        /// </summary>
        public void SetUserActionControl(UserActionControl userActionControl)
        {
            _userActionControl = userActionControl;

            SetImages();
            this.Width -= INDENTATIONOFFSET;
            this.Margin = new Padding(this.Margin.Left + INDENTATIONOFFSET, 3, 3, 3);
            ++_indentationLevel;
        }
        /// <summary>
        /// Reduce the indentlevel by one (or level of dependency) of the container of the control. If the indentlevel is smaller then zero it becomes zero.
        /// </summary>
        public void RemoveUserActionControl()
        {
            _userActionControl = null;
            if (this.Margin.Left != MINIMUMLEFTMARGIN)
            {
                SetImages();
                if (this.Margin.Left > MINIMUMLEFTMARGIN)
                {
                    this.Margin = new Padding(this.Margin.Left - INDENTATIONOFFSET, 3, 3, 3);
                    this.Width += INDENTATIONOFFSET;
                }
                if (_indentationLevel > 0)
                    --_indentationLevel;
            }
        }
        /// <summary>
        /// To edit the value of the container of the control.
        /// </summary>
        public void Edit()
        {
            AddEditLogEntry addEditLogEntry = new AddEditLogEntry(LogChild as LogEntry);
            if (addEditLogEntry.ShowDialog(this) == DialogResult.OK)
            {
                SolutionComponent parent = _logEntry.Parent;
                int index = parent.IndexOf(_logEntry);
                parent.RemoveWithoutInvokingEvent(_logEntry);

                _logEntry = addEditLogEntry.LogEntry;
                if (parent.Count == 0)
                    parent.AddWithoutInvokingEvent(_logEntry);
                else
                    parent.InsertWithoutInvokingEvent(index, _logEntry);

                _logEntry.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }
        /// <summary>
        /// Clears the log entry. No events will be invoked, you should do this yourself afterwards.
        /// </summary>
        public override void ClearLogChild()
        {
            _logEntry.ClearWithoutInvokingEvent();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            chkIndex.Text = (_logEntry.Parent.IndexOf(_logEntry) + 1).ToString();
        }
        protected override void Select(bool directed, bool forward)
        {
            base.Select(directed, forward);
            txtScrollingLogEntry.Focus();
            txtScrollingLogEntry.Select();
        }
        #endregion

    }
}
