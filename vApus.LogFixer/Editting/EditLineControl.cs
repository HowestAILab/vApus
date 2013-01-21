/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using vApus.LogFixer.Properties;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.LogFixer
{
    public partial class EditLineControl : UserControl
    {
        #region Events

        public event EventHandler ButtonClicked;

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
        public EditLineControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Visualizes a line.
        /// </summary>
        public EditLineControl(Line line)
        {
            InitializeComponent();
            _line = line;

            txtScrollingLogEntry.Text = _line.ToString();
            rtxtLogEntry.Text = txtScrollingLogEntry.Text;

            SetImages();
            Collapsed = true;

            if (_line.LogEntry == null)
            {
                btnEdit.Visible = false;
                picValidation.Visible = false;
                int originalLeft = txtScrollingLogEntry.Left;
                txtScrollingLogEntry.Left = picValidation.Left;
                txtScrollingLogEntry.Width += btnEdit.Width + btnEdit.Margin.Left +
                                              (originalLeft - txtScrollingLogEntry.Left);
                txtScrollingLogEntry.BackColor = Color.WhiteSmoke;
                rtxtLogEntry.BackColor = Color.WhiteSmoke;
            }

            SizeChanged += LogEntryControl_SizeChanged;
        }

        #endregion

        #region Functions

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
            if (_line.LogEntry == null)
                picValidation.Visible = false;
            else
                switch (_line.LexicalResult)
                {
                    case LexicalResult.OK:
                        picValidation.Image = Resources.LogEntryOK;
                        break;
                    case LexicalResult.Error:
                        picValidation.Image = Resources.LogEntryError;
                        break;
                }
        }

        private void btnCollapseExpand_Click(object sender, EventArgs e)
        {
            Collapsed = btnCollapseExpand.Text == "-";
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var edit = new EditLogEntry(_line.Parent.IndexOf(_line) + 1, _line.LogEntry);
            if (edit.ShowDialog() == DialogResult.OK)
            {
                SetEdittedText(edit.LogEntry.LogEntryString);
                SetEdittedGui(true);

                if (ButtonClicked != null)
                    ButtonClicked(this, null);

                ApplyFix(edit.TrackedChanges);
            }
        }

        public void SetEdittedText(string newText)
        {
            if (_line.Comment == null)
                _line.LogEntry.LogEntryString = newText;
            else
                _line.Comment = newText;

            txtScrollingLogEntry.Text = _line.ToString();
            rtxtLogEntry.Text = txtScrollingLogEntry.Text;
        }

        public void SetEdittedGui(bool applyLogRuleSet = false)
        {
            if (applyLogRuleSet)
            {
                var log = _line.LogEntry.Parent as Log;
                log.ApplyLogRuleSet();
            }
            if (_line.Comment == null)
            {
                SetImages();

                if (_line.LogEntry.LogEntryString == _line.LogEntry.LogEntryStringAsImported && btnRestore.Visible)
                {
                    int move = btnRestore.Width + btnEdit.Margin.Right;
                    txtScrollingLogEntry.Width += move;
                    btnEdit.Left += move;

                    btnRestore.Visible = false;
                    BackColor = SystemColors.Control;
                }
                else if (!btnRestore.Visible)
                {
                    int move = btnRestore.Width + btnEdit.Margin.Right;
                    txtScrollingLogEntry.Width -= move;
                    btnEdit.Left -= move;

                    btnRestore.Visible = true;
                    BackColor = SystemColors.ControlDark;
                }
            }
        }

        private void ApplyFix(TrackedChanges trackedChanges)
        {
            var applyFix = new ApplyFix(_line, trackedChanges);
            if (applyFix.ShowDialog() == DialogResult.OK)
            {
                Control ctrl = Parent;
                while (!(ctrl is LargeList))
                    ctrl = ctrl.Parent;

                var l = new List<EditLineControl>();

                var largeList = ctrl as LargeList;
                for (int view = 0; view != largeList.ViewCount; view++)
                    foreach (EditLineControl elc in largeList[view])
                        foreach (FixLineControl flc in applyFix.FixLineControls())
                            if (elc.Line == flc.Line)
                            {
                                if (flc.Checked)
                                {
                                    elc.SetEdittedText(elc.Line.ToString());
                                    l.Add(elc);
                                }
                                break;
                            }

                var log = _line.LogEntry.Parent as Log;
                log.ApplyLogRuleSet();

                foreach (EditLineControl elc in l)
                    elc.SetEdittedGui();

                if (ButtonClicked != null)
                    ButtonClicked(this, null);
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            var log = _line.LogEntry.Parent as Log;
            _line.LogEntry.LogEntryString = _line.LogEntry.LogEntryStringAsImported;

            log.ApplyLogRuleSet();

            txtScrollingLogEntry.Text = _line.ToString();
            rtxtLogEntry.Text = txtScrollingLogEntry.Text;

            SetImages();

            int move = btnRestore.Width + btnEdit.Margin.Right;
            txtScrollingLogEntry.Width += move;
            btnEdit.Left += move;

            btnRestore.Visible = false;
            BackColor = SystemColors.Control;
            if (ButtonClicked != null)
                ButtonClicked(this, null);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_line != null)
                lblIndex.Text = (_line.Parent.IndexOf(_line) + 1).ToString();
        }

        #endregion
    }
}