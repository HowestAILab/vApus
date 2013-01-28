/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using vApus.Results;
using vApus.Util;

namespace vApus.Stresstest.Controls {
    public partial class DetailedResultsControl : UserControl {
        private KeyValuePairControl[] _config = new KeyValuePairControl[0];
        private ResultsHelper _resultsHelper;

        public DetailedResultsControl() {
            InitializeComponent();

            //Double buffer the datagridview.
            (dgvDetailedResults).GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(dgvDetailedResults, true);

            btnCollapseExpand.PerformClick();
            chkAdvanced.Checked = false;
            cboShow.SelectedIndex = 0;
        }

        private void chkAdvanced_CheckedChanged(object sender, EventArgs e) {
            splitQueryData.Panel1Collapsed = !chkAdvanced.Checked;
        }

        private void lbtnDescription_ActiveChanged(object sender, EventArgs e) {
            SetConfig(_resultsHelper.GetDescription());
        }
        private void lbtnTags_ActiveChanged(object sender, EventArgs e) {
            SetConfig(_resultsHelper.GetTags());
        }
        private void lbtnvApusInstance_ActiveChanged(object sender, EventArgs e) {
            SetConfig(_resultsHelper.GetvApusInstance(1));
        }
        private void lbtnStresstest_ActiveChanged(object sender, EventArgs e) {
            SetConfig(_resultsHelper.GetStresstest(1));
        }
        private void lbtnMonitors_ActiveChanged(object sender, EventArgs e) {
            SetConfig(_resultsHelper.GetMonitors());
        }
        private void btnCollapseExpand_Click(object sender, EventArgs e) {
            if (btnCollapseExpand.Text == "-") {
                btnCollapseExpand.Text = "+";

                splitContainer.SplitterDistance = splitContainer.Panel1MinSize;
                splitContainer.IsSplitterFixed = true;
                splitContainer.BackColor = Color.White;
            } else ExpandConfig();
        }

        private void btnSaveDisplayedResults_Click(object sender, EventArgs e) {
            var sfd = new SaveFileDialog();
            sfd.Title = "Where do you want to save the displayed results?";
            //sfd.FileName = kvpStresstest.Key.ReplaceInvalidWindowsFilenameChars('_');
            sfd.Filter = "TXT|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
                try {
                    using (var sw = new StreamWriter(sfd.FileName)) {
                        sw.Write(GetDisplayedResults());
                        sw.Flush();
                    }
                } catch {
                    MessageBox.Show("Could not access file: " + sfd.FileName, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        /// <summary>
        ///     Get the displayed results.
        /// </summary>
        /// <returns></returns>
        private string GetDisplayedResults() {
            var sb = new StringBuilder();
            //Write column headers
            foreach (DataGridViewColumn clm in dgvDetailedResults.Columns) {
                sb.Append(clm.HeaderText);
                sb.Append("\t");
            }
            sb.AppendLine();

            foreach (DataGridViewRow row in dgvDetailedResults.Rows) sb.AppendLine(row.ToSV("\t"));
            return sb.ToString();
        }
        private void SetConfig(string value) {
            foreach (var v in _config) flpConfiguration.Controls.Remove(v);
            _config = new KeyValuePairControl[] { new KeyValuePairControl(value, string.Empty) { BackColor = SystemColors.Control } };
            flpConfiguration.Controls.AddRange(_config);

            ExpandConfig();
        }
        private void ExpandConfig() {
            if (btnCollapseExpand.Text == "+") {
                btnCollapseExpand.Text = "-";
                splitContainer.SplitterDistance = 85;
                splitContainer.IsSplitterFixed = false;
                splitContainer.BackColor = SystemColors.Control;
            }
        }
        private void SetConfig(List<string> values) {
            foreach (var v in _config) flpConfiguration.Controls.Remove(v);
            _config = new KeyValuePairControl[values.Count];
            for (int i = 0; i != _config.Length; i++) _config[i] = new KeyValuePairControl(values[i], string.Empty) { BackColor = SystemColors.Control };
            flpConfiguration.Controls.AddRange(_config);
        }
        private void SetConfig(List<KeyValuePair<string, string>> keyValues) {
            foreach (var v in _config) flpConfiguration.Controls.Remove(v);
            _config = new KeyValuePairControl[keyValues.Count];
            int i = 0;
            foreach (var kvp in keyValues) _config[i++] = new KeyValuePairControl(kvp.Key, kvp.Value) { BackColor = SystemColors.Control };
            flpConfiguration.Controls.AddRange(_config);
        }

        private void cboShow_SelectedIndexChanged(object sender, EventArgs e) {
            dgvDetailedResults.DataSource = null;
            if (_resultsHelper != null) {
                if (cboShow.SelectedIndex == 0) dgvDetailedResults.DataSource = _resultsHelper.GetAverageConcurrentUsers();
                else if (cboShow.SelectedIndex == 1) dgvDetailedResults.DataSource = _resultsHelper.GetAverageUserActions();
                else if (cboShow.SelectedIndex == 2) dgvDetailedResults.DataSource = _resultsHelper.GetAverageLogEntries();
                else dgvDetailedResults.DataSource = _resultsHelper.GetErrors();
            }
            dgvDetailedResults.Select();
        }

        private void btnExecute_Click(object sender, EventArgs e) {
            try { dgvDetailedResults.DataSource = _resultsHelper.ExecuteQuery(codeTextBox.Text); } catch { }
        }

        /// <summary>
        /// Clear before testing.
        /// </summary>
        public void ClearResults() {
            foreach (var v in _config) flpConfiguration.Controls.Remove(v);
            _config = new KeyValuePairControl[0];

            dgvDetailedResults.DataSource = null;
        }
        /// <summary>
        /// Refresh after testing.
        /// </summary>
        /// <param name="resultsHelper">Give hte helper that made the db</param>
        public void RefreshResults(ResultsHelper resultsHelper) {
            _resultsHelper = resultsHelper;
            foreach (var ctrl in flpConfiguration.Controls)
                if (ctrl is LinkButton) {
                    var lbtn = ctrl as LinkButton;
                    if (lbtn.Active) {
                        lbtn.PerformClick();
                        break;
                    }
                }
            cboShow_SelectedIndexChanged(null, null);
        }
    }
}
