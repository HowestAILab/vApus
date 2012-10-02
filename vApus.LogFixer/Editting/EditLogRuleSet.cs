/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.IO;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.LogFixer
{
    public partial class EditLogRuleSet : Form
    {
        private string _logRuleSetFileName;
        private XMLTextStyle _xmlTextStyle;

        public string LogRuleSetFileName
        {
            get { return _logRuleSetFileName; }
            protected set
            {
                _logRuleSetFileName = value;
                Text = "Edit - " + _logRuleSetFileName;
            }
        }

        public EditLogRuleSet()
        {
            InitializeComponent();
            _xmlTextStyle = new XMLTextStyle(fastColoredTextBoxEdit);
        }
        public EditLogRuleSet(string logRuleSetFileName)
            : this()
        {
            LogRuleSetFileName = logRuleSetFileName;

            string logRuleSet = string.Empty;
            using (var sr = new StreamReader(_logRuleSetFileName))
                logRuleSet = sr.ReadToEnd();

            fastColoredTextBoxEdit.Text = logRuleSet;

            SetView();
        }

        private void SetView()
        {
            string path = Path.Combine(Application.StartupPath, "temp.xml");
            using (var sw = new StreamWriter(path))
            {
                sw.Write(fastColoredTextBoxEdit.Text);
                sw.Flush();
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog() == DialogResult.OK)
                Save(sfd.FileName);
        }
        /// <summary>
        /// Sets _logRuleSetFileName if succesfully saved.
        /// </summary>
        /// <param name="filename"></param>
        private void Save(string filename)
        {
            try
            {
                using (var sw = new StreamWriter(filename))
                {
                    sw.Write(fastColoredTextBoxEdit.Text);
                    sw.Flush();
                }

                LogRuleSetFileName = filename;
            }
            catch
            {
                MessageBox.Show("Could not save the file because it is write-protected.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
