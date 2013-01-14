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

namespace vApus.Stresstest.Controls
{
    public partial class DetailedResultsControl : UserControl
    {
        public DetailedResultsControl()
        {
            InitializeComponent();

            //Double buffer the datagridview.
            (dgvDetailedResults).GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(dgvDetailedResults, true);

            btnCollapseExpand.PerformClick();
            chkReadable.Checked = false;
            cboShow.SelectedIndex = 0;
        }

        private void chkReadable_CheckedChanged(object sender, EventArgs e)
        {
            splitQueryData.Panel1Collapsed = !chkReadable.Checked;
        }

        private void lbtnDescription_ActiveChanged(object sender, EventArgs e)
        {
            if (btnCollapseExpand.Text == "+")
            {
                btnCollapseExpand.Text = "-";
                splitContainer.SplitterDistance = 85;
                splitContainer.IsSplitterFixed = false;
                splitContainer.BackColor = SystemColors.Control;
            }
        }
        private void btnCollapseExpand_Click(object sender, EventArgs e)
        {
            if (btnCollapseExpand.Text == "-")
            {
                btnCollapseExpand.Text = "+";

                splitContainer.SplitterDistance = splitContainer.Panel1MinSize;
                splitContainer.IsSplitterFixed = true;
                splitContainer.BackColor = Color.White;
            }
            else
            {
                btnCollapseExpand.Text = "-";
                splitContainer.SplitterDistance = 85;
                splitContainer.IsSplitterFixed = false;
                splitContainer.BackColor = SystemColors.Control;
            }
        }

        private void btnSaveDisplayedResults_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Title = "Where do you want to save the displayed results?";
            //sfd.FileName = kvpStresstest.Key.ReplaceInvalidWindowsFilenameChars('_');
            sfd.Filter = "TXT|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
                try
                {
                    using (var sw = new StreamWriter(sfd.FileName))
                    {
                        sw.Write(GetDisplayedResults());
                        sw.Flush();
                    }
                }
                catch
                {
                    MessageBox.Show("Could not access file: " + sfd.FileName, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        /// <summary>
        ///     Get the displayed results.
        /// </summary>
        /// <returns></returns>
        private string GetDisplayedResults()
        {
            var sb = new StringBuilder();
            //Write column headers
            foreach (DataGridViewColumn clm in dgvDetailedResults.Columns)
            {
                sb.Append(clm.HeaderText);
                sb.Append("\t");
            }
            sb.AppendLine();

            //Select and write rows.
            //List<object[]> rows = new List<object[]>();
            //if (lbtnStresstest.Active)
            //    rows = cboDrillDown.SelectedIndex == 0 ? _concurrencyStresstestMetricsRows : _runStresstestMetricsRows;
            //else
            //{
            //    string monitorToString = null;
            //    foreach (var lbtnMonitor in _monitorLinkButtons) if (lbtnMonitor.Active) { monitorToString = lbtnMonitor.Text; break; }
            //    if (monitorToString != null)
            //        rows = (cboDrillDown.SelectedIndex == 0 ? _concurrencyMonitorMetricsRows : _runMonitorMetricsRows)[monitorToString];
            //}
            //foreach (var row in rows)
            //{
            //    sb.Append(row.Combine("\t"));
            //    sb.AppendLine();
            //}

            return sb.ToString();
        }

        private KeyValuePairControl[] _config = new KeyValuePairControl[0];
        private void lbtnDescription_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetConfig(ResultsHelper.GetDescription());
        }

        private void lbtnTags_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetConfig(ResultsHelper.GetTags());
        }

        private void lbtnvApusInstances_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //ResultsHelper.GetvApusInstanceIds();
            SetConfig(ResultsHelper.GetvApusInstance(1));
        }
        private void SetConfig(string value)
        {
            foreach (var v in _config) flpConfiguration.Controls.Remove(v);
            _config = new KeyValuePairControl[] { new KeyValuePairControl(value, string.Empty) { BackColor = SystemColors.Control } };
            flpConfiguration.Controls.AddRange(_config);
        }
        private void lbtnStresstest_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetConfig(ResultsHelper.GetStresstest(1));
        }
        private void lbtnMonitors_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetConfig(ResultsHelper.GetMonitors());
        }
        private void SetConfig(List<string> values)
        {
            foreach (var v in _config) flpConfiguration.Controls.Remove(v);
            _config = new KeyValuePairControl[values.Count];
            for (int i = 0; i != _config.Length; i++) _config[i] = new KeyValuePairControl(values[i], string.Empty) { BackColor = SystemColors.Control };
            flpConfiguration.Controls.AddRange(_config);
        }
        private void SetConfig(List<KeyValuePair<string, string>> keyValues)
        {
            foreach (var v in _config) flpConfiguration.Controls.Remove(v);
            _config = new KeyValuePairControl[keyValues.Count];
            int i = 0;
            foreach (var kvp in keyValues) _config[i++] = new KeyValuePairControl(kvp.Key, kvp.Value) { BackColor = SystemColors.Control };
            flpConfiguration.Controls.AddRange(_config);
        }

        private void cboShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboShow.SelectedIndex == 0) dgvDetailedResults.DataSource = ResultsHelper.GetAverageConcurrentUsers();
            else if(cboShow.SelectedIndex == 1)dgvDetailedResults.DataSource = ResultsHelper.GetAverageUserActions();
            else if (cboShow.SelectedIndex == 2) dgvDetailedResults.DataSource = ResultsHelper.GetAverageLogEntries();
            else dgvDetailedResults.DataSource = ResultsHelper.GetErrors();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            try { dgvDetailedResults.DataSource = ResultsHelper.ExecuteQuery(codeTextBox.Text); }
            catch { }
        }

        /// <summary>
        /// Clear before testing.
        /// </summary>
        public void ClearReport()
        {
            foreach (var v in _config) flpConfiguration.Controls.Remove(v);
            _config = new KeyValuePairControl[0];

            dgvDetailedResults.DataSource = null;
        }
        /// <summary>
        /// Refresh after testing.
        /// </summary>
        public void RefreshReport()
        {
            foreach(var ctrl in flpConfiguration.Controls)
                if (ctrl is LinkButton)
                {
                    var lbtn = ctrl as LinkButton;
                    if (lbtn.Active)
                    {
                        lbtn.PerformClick();
                        break;
                    }
                }
            cboShow_SelectedIndexChanged(null, null);
        }
    }
}
