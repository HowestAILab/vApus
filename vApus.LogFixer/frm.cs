/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Text;
using System.Windows.Forms;

namespace vApus.LogFixer
{
    public partial class frm : Form
    {
        private string[] _logFileNames;

        public frm()
        {
            InitializeComponent();
        }

        private void btnChooseLogFiles_Click(object sender, EventArgs e)
        {
            if (ofdLogFiles.ShowDialog() == DialogResult.OK)
            {
                _logFileNames = ofdLogFiles.FileNames;

                if (_logFileNames.Length == 1)
                {
                    txtLogFiles.Text = _logFileNames[0];
                }
                else
                {
                    var sb = new StringBuilder();
                    foreach (string fn in _logFileNames)
                    {
                        sb.Append("\"");
                        sb.Append(fn);
                        sb.Append("\" ");
                    }
                    txtLogFiles.Text = sb.ToString();
                }

                if (txtLogRuleSet.Text.Length != 0)
                    DoStuff();
            }
        }

        private void btnLogRuleSet_Click(object sender, EventArgs e)
        {
            if (ofdLogRuleSet.ShowDialog() == DialogResult.OK)
            {
                txtLogRuleSet.Text = ofdLogRuleSet.FileName;

                if (txtLogFiles.Text.Length != 0)
                    DoStuff();
            }
        }

        private void DoStuff()
        {
            tc.TabPages.Clear();
            foreach (string lfn in _logFileNames)
            {
                var ft = new FixTab();
                ft.Init(lfn, txtLogRuleSet.Text);

                tc.TabPages.Add(ft);
            }
        }

        private void txtLogRuleSet_TextChanged(object sender, EventArgs e)
        {
            btnEditLogRuleSet.Enabled = txtLogRuleSet.Text.Length != 0;
        }

        private void btnEditLogRuleSet_Click(object sender, EventArgs e)
        {
            var editLogRuleSet = new EditLogRuleSet(txtLogRuleSet.Text);
            editLogRuleSet.ShowDialog();

            txtLogRuleSet.Text = editLogRuleSet.LogRuleSetFileName;
            if (txtLogFiles.Text.Length != 0)
                DoStuff();
        }
    }
}