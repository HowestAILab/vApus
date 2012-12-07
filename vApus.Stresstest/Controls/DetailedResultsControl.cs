/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

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
                splitContainer.SplitterDistance = 68;
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
                splitContainer.SplitterDistance = 68;
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

    }
}
