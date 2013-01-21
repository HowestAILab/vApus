/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Windows.Forms;
using vApus.LogFixer.Properties;
using vApus.Stresstest;

namespace vApus.LogFixer
{
    public partial class FixLineControl : UserControl
    {
        #region Events

        public event EventHandler CheckedChanged;

        #endregion

        #region Fields

        private readonly Line _line;

        #endregion

        #region Properties

        public Line Line
        {
            get { return _line; }
        }

        public LexicalResult LexicalResult
        {
            get { return _line.LexicalResult; }
        }

        public bool Checked
        {
            get { return chkIndex.Checked; }
            set { chkIndex.Checked = value; }
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
                    Height = 30;
                }
                else
                {
                    btnCollapseExpand.Text = "-";
                    Height = 200;
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     An empty log entry control, aught to use only when designing.
        /// </summary>
        public FixLineControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Visualizes a line.
        /// </summary>
        public FixLineControl(Line line)
            : this()
        {
            _line = line;

            txtScrollingLogEntryFrom.Text = _line.ToString();
            rtxtLogEntryFrom.Text = txtScrollingLogEntryFrom.Text;

            SetImages();
            Collapsed = true;

            SizeChanged += LogEntryControl_SizeChanged;
        }

        #endregion

        #region Functions

        /// <summary>
        /// </summary>
        /// <param name="preview"></param>
        /// <returns>Error with the previewed change?</returns>
        public bool SetPreview(string preview)
        {
            txtScrollingLogEntryTo.Text = preview;
            rtxtLogEntryTo.Text = preview;

            var log = _line.LogEntry.Parent as Log;
            LogRuleSet logRuleSet = log.LogRuleSet;

            log = new Log();
            log.LogRuleSet = logRuleSet;

            var logEntry = new LogEntry(preview);
            log.AddWithoutInvokingEvent(logEntry);
            log.ApplyLogRuleSet();

            switch (log.LexicalResult)
            {
                case LexicalResult.OK:
                    picValidationTo.Image = Resources.LogEntryOK;
                    break;
                case LexicalResult.Error:
                    picValidationTo.Image = Resources.LogEntryError;
                    break;
            }

            return log.LexicalResult == LexicalResult.Error;
        }

        private void LogEntryControl_SizeChanged(object sender, EventArgs e)
        {
            if (Visible && splitContainer.Panel2Collapsed)
            {
                SizeChanged -= LogEntryControl_SizeChanged;
                splitContainer.Panel2Collapsed = false;
                splitContainer.Panel2Collapsed = true;
                SizeChanged += LogEntryControl_SizeChanged;
            }
        }

        private void SetImages()
        {
            switch (_line.LexicalResult)
            {
                case LexicalResult.OK:
                    picValidationFrom.Image = Resources.LogEntryOK;
                    picValidationTo.Image = Resources.LogEntryOK;
                    break;
                case LexicalResult.Error:
                    picValidationFrom.Image = Resources.LogEntryError;
                    picValidationTo.Image = Resources.LogEntryError;
                    break;
            }
        }

        private void btnCollapseExpand_Click(object sender, EventArgs e)
        {
            Collapsed = btnCollapseExpand.Text == "-";
        }

        private void chkIndex_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckedChanged != null)
                CheckedChanged(this, e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_line != null)
                chkIndex.Text = (_line.Parent.IndexOf(_line) + 1).ToString();
        }

        #endregion
    }
}