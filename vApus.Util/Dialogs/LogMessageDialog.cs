/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace vApus.Util
{
    public partial class LogMessageDialog : Form
    {
        /// <summary>
        /// The form cannot be closed if this is true.
        /// </summary>
        private bool _reporting = false;

        public override string Text
        {
            get { return rtxt == null ? string.Empty : rtxt.Text; }
            set { rtxt.Text = value; }
        }
        public string Title
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        #region Constructor
        public LogMessageDialog()
        {
            InitializeComponent();
            NewIssue.Done += new EventHandler<ActiveObject.OnResultEventArgs>(NewIssue_Done);
        }
        #endregion

        public bool ReportThisBugVisible
        {
            get { return btnReportThisBug.Visible; }
            set { btnReportThisBug.Visible = false; }
        }

        #region Functions
        private void btnReportThisBug_Click(object sender, EventArgs e)
        {
            _reporting = true;

            btnReportThisBug.Text = "Reporting...";
            btnReportThisBug.Width = 103;
            btnReportThisBug.Enabled = false;

            NewIssue.Post(rtxt.Text);
        }
        private void NewIssue_Done(object sender, ActiveObject.OnResultEventArgs e)
        {
            if (e != null && e.Returned != null)
            {
                if (e.Exception == null)
                {
                    btnReportThisBug.Width = 93;
                    btnReportThisBug.Text = "Reported!";

                    llblBug.Text = e.Returned.ToString();
                }
                else
                {
                    btnReportThisBug.Width = 123;
                    btnReportThisBug.Text = "Report this bug";
                    btnReportThisBug.Enabled = true;

                    LogWrapper.LogByLevel("[" + this + "] " + e.Exception.ToString(), LogLevel.Error);
                    MessageBox.Show(e.Exception.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                _reporting = false;
            }
        }
        private void llblBug_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(llblBug.Text);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void LogMessageDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = _reporting;
        }
        #endregion
    }
}
