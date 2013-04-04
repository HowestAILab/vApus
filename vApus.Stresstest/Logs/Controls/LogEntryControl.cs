/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Stresstest.Properties;

namespace vApus.Stresstest {
    public partial class LogEntryControl : LogChildControlBase {
        #region Events

        public event EventHandler LexicalResultChanged;
        public event EventHandler Removed;

        #endregion

        #region Fields

        public const int INDENTATIONOFFSET = 16;

        private const string VBLRn = "<16 0C 02 12n>";
        private const string VBLRr = "<16 0C 02 12r>";
        private readonly string _logChildDelimiter;

        private static Color _firstEntryColor = Color.FromArgb(255, 211, 211, 211);
        private static Color _primaryColor = Color.FromArgb(255, 240, 240, 240);
        private static Color _secundaryColor = Color.FromArgb(255, 227, 227, 227);

        //Replace above 3 values with this in the label;
        private string _delimiterReplacement = "◦";
        private uint _indentationLevel;
        private LogEntry _logEntry;
        private UserActionControl _userActionControl;

        #endregion

        #region Properties

        /// <summary>
        ///     The base item that it holds, in this case a log entry.
        /// </summary>
        public override BaseItem LogChild {
            get { return _logEntry; }
        }

        public override uint IndentationLevel {
            get { return _indentationLevel; }
        }

        public override bool Checked {
            get { return chkIndex.Checked; }
            set { chkIndex.Checked = value; }
        }

        public LexicalResult LexicalResult {
            get { return _logEntry.LexicalResult; }
        }

        public UserActionControl UserActionControl {
            get { return _userActionControl; }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     An empty log entry control, aught to use only when designing.
        /// </summary>
        public LogEntryControl() {
            InitializeComponent();
        }

        /// <summary>
        ///     Visualizes a log entry.
        /// </summary>
        public LogEntryControl(LogEntry logEntry) {
            InitializeComponent();
            _logEntry = logEntry;
            _logEntry.LexicalResultChanged += _logEntry_LexicalResultChanged;
            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;

            if (_logEntry.LogRuleSet != null && !_logEntry.LogRuleSet.IsEmpty)
                _logChildDelimiter = _logEntry.LogRuleSet.ChildDelimiter;

            SetLabel();

            //Backwards compatible.
            if (_logEntry.Parent is UserAction)
                _logEntry.Pinned = true;


            nudOccurance.Value = _logEntry.Occurance;
            nudParallelOffsetInMs.Value = _logEntry.ParallelOffsetInMs;
            SetImages();

            if (LexicalResultChanged != null)
                LexicalResultChanged(this, null);

            nudOccurance.ValueChanged += nudOccurance_ValueChanged;
            nudParallelOffsetInMs.ValueChanged += nudParallelOffsetInMs_ValueChanged;
        }

        #endregion

        #region Functions

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender == _logEntry || (sender is Log && (sender == _logEntry.Parent || (_logEntry.Parent is UserAction && sender == (_logEntry.Parent as UserAction).Parent)))) {
                SetImages();
                SetLabel();
            }
        }

        private void _logEntry_LexicalResultChanged(object sender, EventArgs e) {
            SetImages();
            if (LexicalResultChanged != null)
                LexicalResultChanged(this, null);
        }

        private void SetLabel() {
            lblLogEntry.Text = _logEntry.LogEntryString.Replace("&", "&&").Replace(VBLRn, _delimiterReplacement).Replace(VBLRr, _delimiterReplacement);

            if (_logChildDelimiter != null && _logChildDelimiter.Length != 0)
                lblLogEntry.Text = lblLogEntry.Text.Replace(_logChildDelimiter, _delimiterReplacement);
        }

        private void SetImages() {
            switch (_logEntry.LexicalResult) {
                case LexicalResult.OK:
                    picValidation.Image = Resources.LogEntryOK;
                    break;
                case LexicalResult.Error:
                    picValidation.Image = Resources.LogEntryError;
                    break;
            }

            //See OnPaint for picParallel
            picIgnoreDelay.Image = _logEntry.IgnoreDelay ? Resources.IgnoreDelay : Resources.Delay;
            picPin.Image = _logEntry.Pinned ? Resources.Pin : Resources.PinGreyedOut;
            picPin.Visible = _logEntry.Parent is Log;
        }

        private void chkIndex_CheckedChanged(object sender, EventArgs e) {
            InvokeCheckedChanged();
        }

        private void picPin_Click(object sender, EventArgs e) {
            _logEntry.Pinned = !_logEntry.Pinned;
            _logEntry.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            SetImages();
        }

        private void picIgnoreDelay_Click(object sender, EventArgs e) {
            if (!_logEntry.ExecuteInParallelWithPrevious) {
                _logEntry.IgnoreDelay = !_logEntry.IgnoreDelay;
                _logEntry.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                SetImages();
            }
        }

        private void picParallel_Click(object sender, EventArgs e) {
            _logEntry.ExecuteInParallelWithPrevious = !_logEntry.ExecuteInParallelWithPrevious;
            _logEntry.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            SetImages();
        }

        private void nudOccurance_ValueChanged(object sender, EventArgs e) {
            if (_logEntry.Occurance != nudOccurance.Value) {
                _logEntry.Occurance = (int)nudOccurance.Value;
                _logEntry.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }

        private void nudParallelOffsetInMs_ValueChanged(object sender, EventArgs e) {
            if (_logEntry.ParallelOffsetInMs != nudParallelOffsetInMs.Value) {
                _logEntry.ParallelOffsetInMs = (int)nudParallelOffsetInMs.Value;
                _logEntry.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }

        private void llblEdit_Click(object sender, EventArgs e) {
            Edit();
        }

        private void toolStripStatusLabelRemove_Click(object sender, EventArgs e) {
            if (Removed != null)
                Removed(this, null);
        }

        /// <summary>
        ///     Augment the indentlevel by one (or level of dependency) of the container of the control.
        /// </summary>
        public void SetUserActionControl(UserActionControl userActionControl) {
            _userActionControl = userActionControl;

            SetImages();
            Width -= INDENTATIONOFFSET;
            Margin = new Padding(Margin.Left + INDENTATIONOFFSET, 3, 3, 3);

            nudOccurance.ForeColor = Color.FromArgb(75, 75, 75);

            ++_indentationLevel;
        }

        /// <summary>
        ///     Reduce the indentlevel by one (or level of dependency) of the container of the control. If the indentlevel is smaller then zero it becomes zero.
        /// </summary>
        public void RemoveUserActionControl() {
            _userActionControl = null;
            if (Margin.Left != MINIMUMLEFTMARGIN) {
                SetImages();
                if (Margin.Left > MINIMUMLEFTMARGIN) {
                    Margin = new Padding(Margin.Left - INDENTATIONOFFSET, 3, 3, 3);
                    Width += INDENTATIONOFFSET;
                }

                nudOccurance.ForeColor = SystemColors.WindowText;

                if (_indentationLevel > 0)
                    --_indentationLevel;
            }
        }

        /// <summary>
        ///     To edit the value of the container of the control.
        /// </summary>
        public void Edit() {
            var addEditLogEntry = new AddEditLogEntry(LogChild as LogEntry);
            if (addEditLogEntry.ShowDialog(this) == DialogResult.OK) {
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
        ///     Clears the log entry. No events will be invoked, you should do this yourself afterwards.
        /// </summary>
        public override void ClearLogChild() {
            _logEntry.ClearWithoutInvokingEvent();
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            int index = _logEntry.Parent.IndexOf(_logEntry) + 1;
            chkIndex.Text = index.ToString();

#if EnableBetaFeature
#warning Parallel executions temp not available
            picParallel.Visible = _logEntry.Parent is UserAction;
            //In the case of index == 1 there is no previous entry to execute parallely with.
            //This must all be done here because the index is drawn here.
            picParallel.Enabled = index > 1 && picParallel.Visible;
            if (!picParallel.Enabled)
                _logEntry.ExecuteInParallelWithPrevious = false;

            if (_logEntry.ExecuteInParallelWithPrevious) {
                _logEntry.IgnoreDelay = true;
                picIgnoreDelay.Visible = false;
                nudParallelOffsetInMs.Visible = true;
                picParallel.Image = Resources.Parallel;
            } else {
                picIgnoreDelay.Visible = true;
                nudParallelOffsetInMs.Visible = false;
                picParallel.Image = Resources.NotParallel;
                if (index == 1 && _logEntry.Parent is UserAction && _logEntry.Parent.Count != 0)
                    BackColor = _firstEntryColor;
                else
                    BackColor = (((float)index % 2) == 0) ? _primaryColor : _secundaryColor;
            }
#else
            BackColor = (((float)index % 2) == 0) ? PrimaryColor : SecundaryColor;
#endif
        }

        protected override void Select(bool directed, bool forward) {
            base.Select(directed, forward);
        }

        #endregion
    }
}