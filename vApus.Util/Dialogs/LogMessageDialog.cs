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

namespace vApus.Util
{
    public partial class LogMessageDialog : Form
    {
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
        }
        #endregion

        #region Functions
        private void btnReportThisBug_Click(object sender, EventArgs e)
        {
            //string host = "http://redmine.sizingservers.be";
            //string user = "vapususer";
            //string pass = "si4droed;";

            //var manager = new RedmineManager(host, user, pass);

            //var customFields = new List<CustomField>();
            //customFields.Add(new CustomField { Name = "Software version", Value = "N/A" });

            //string[] split = rtxt.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Split('\n');

            //var newIssue = new Issue
            //{
            //    Subject = split[0],
            //    Description = rtxt.Text.Substring(split[0].Length).TrimStart(),
            //    CustomFields = customFields,
            //    //Tracker = new IdentifiableName { Name = "Bug" },
            //    Project = new IdentifiableName { Name = "@Tickets" }
            //};
            //manager.CreateObject(newIssue);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
