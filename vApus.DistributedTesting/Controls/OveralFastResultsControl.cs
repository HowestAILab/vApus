using RandomUtils.Log;
/*
 * Copyright 2009 (c) Sizing Servers Lab
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

namespace vApus.DistributedTesting {
    public partial class OveralFastResultsControl : UserControl {

        #region Fields
        private DistributedTest _distributedTest;

        //Caching the progress here.
        Dictionary<TileStresstest, FastStresstestMetricsCache> _progress = new Dictionary<TileStresstest, FastStresstestMetricsCache>();
        private List<object[]> _concurrencyStresstestMetricsRows = new List<object[]>();
        private List<object[]> _runStresstestMetricsRows = new List<object[]>();

        private List<int> _invalidateConcurrencyRows = new List<int>();
        private List<int> _invalidateRunRows = new List<int>();

        /// <summary>
        ///     Enables Auto scroll to end of fast results when appropriate.
        /// </summary>
        private bool _keepFastResultsAtEnd = true;
        #endregion

        #region Properties
        public DistributedTest DistributedTest {
            get { return _distributedTest; }
            set { _distributedTest = value; }
        }
        #endregion

        #region Constructor
        public OveralFastResultsControl() {
            InitializeComponent();
            (dgvFastResults).GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(dgvFastResults, true);
            cboDrillDown.SelectedIndex = 0;
        }
        #endregion

        #region Functions

        public void ClearFastResults() {
            dgvFastResults.RowCount = 0;

            _progress = new Dictionary<TileStresstest, FastStresstestMetricsCache>();

            _concurrencyStresstestMetricsRows = new List<object[]>();
            _runStresstestMetricsRows = new List<object[]>();

            _invalidateConcurrencyRows = new List<int>();
            _invalidateRunRows = new List<int>();
            eventView.ClearEvents();

            kvpOK.Visible = kvpCancelled.Visible = kvpFailed.Visible = false;

            _keepFastResultsAtEnd = true;
        }

        /// <summary>
        ///     Leave all empty for the default values.
        /// </summary>
        /// <param name="runningTests"></param>
        /// <param name="ok"></param>
        /// <param name="cancelled"></param>
        /// <param name="failed"></param>
        /// <param name="cpuUsage"></param>
        /// <param name="contextSwitchesPerSecond"></param>
        /// <param name="memoryUsage"></param>
        /// <param name="totalVisibleMemory"></param>
        /// <param name="nicsSent"></param>
        /// <param name="nicsReceived"></param>
        public void SetMasterMonitoring(int runningTests = 0, int ok = 0, int cancelled = 0, int failed = 0, float cpuUsage = -1f, float contextSwitchesPerSecond = -1f,
                                        int memoryUsage = -1, int totalVisibleMemory = -1, float nicsSent = -1, float nicsReceived = -1) {
            kvpRunningTests.Visible = runningTests != 0;
            kvpRunningTests.Value = runningTests.ToString();

            kvpOK.Visible = ok != 0;
            kvpOK.Value = ok.ToString();

            kvpCancelled.Visible = cancelled != 0;
            kvpCancelled.Value = cancelled.ToString();

            kvpFailed.Visible = failed != 0;
            kvpFailed.Value = failed.ToString();

            if (cpuUsage == -1) {
                kvmMasterCPUUsage.Value = "N/A";
            } else {
                kvmMasterCPUUsage.Value = Math.Round(cpuUsage, 2).ToString() + " %";

                if (cpuUsage < 60) {
                    kvmMasterCPUUsage.BackColor = Color.GhostWhite;
                } else {
                    kvmMasterCPUUsage.BackColor = Color.Orange;
                    AppendMessages(cpuUsage + " % CPU Usage", Level.Warning);
                }
            }
            kvmMasterContextSwitchesPerSecond.Value = (contextSwitchesPerSecond == -1) ? "N/A" : contextSwitchesPerSecond.ToString();

            if (memoryUsage == -1 || totalVisibleMemory == -1) {
                kvmMasterMemoryUsage.Value = "N/A";
            } else {
                kvmMasterMemoryUsage.Value = memoryUsage.ToString() + " / " + totalVisibleMemory + " MB";
                if (memoryUsage < 0.9 * totalVisibleMemory) {
                    kvmMasterMemoryUsage.BackColor = Color.GhostWhite;
                } else if (memoryUsage != 0) {
                    kvmMasterMemoryUsage.BackColor = Color.Orange;
                    AppendMessages(memoryUsage + " of " + totalVisibleMemory + " MB used", Level.Warning);
                }
            }
            if (nicsSent == -1) {
                kvmMasterNicsSent.Value = "N/A";
            } else {
                kvmMasterNicsSent.Value = Math.Round(nicsSent, 2).ToString() + " %";
                if (nicsSent < 90) {
                    kvmMasterNicsSent.BackColor = Color.GhostWhite;
                } else if (!float.IsPositiveInfinity(nicsSent) && !float.IsNegativeInfinity(nicsSent)) {
                    kvmMasterNicsSent.BackColor = Color.Orange;
                    AppendMessages(nicsSent + " % NIC Usage (Sent)", Level.Warning);
                }
            }
            if (nicsReceived == -1) {
                kvmMasterNicsReceived.Value = "N/A";
            } else {
                kvmMasterNicsReceived.Value = Math.Round(nicsReceived, 2).ToString() + " %";
                if (nicsSent < 90) {
                    kvmMasterNicsReceived.BackColor = Color.GhostWhite;
                } else if (!float.IsPositiveInfinity(nicsReceived) && !float.IsNegativeInfinity(nicsReceived)) {
                    kvmMasterNicsReceived.BackColor = Color.Orange;
                    AppendMessages(nicsReceived + " % NIC Usage (Received)", Level.Warning);
                }
            }
        }

        /// <summary>
        /// To show to which subset of results there is filtered. 
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title) {
            lblFastResults.Text = "Fast Results [" + title + "]";
        }
        /// <summary>
        /// </summary>
        /// <param name="title">Distributed test or the tostring of the tile</param>
        /// <param name="progress"></param>
        public void SetOverallFastResults(Dictionary<TileStresstest, FastStresstestMetricsCache> progress) {
            _progress = progress;

            RefreshRows();
            if (cboDrillDown.SelectedIndex == 0) SetOverallFastConcurrencyResults(); else SetOverallFastRunResults();
        }
        private void RefreshRows() {
            _concurrencyStresstestMetricsRows.Clear();
            _runStresstestMetricsRows.Clear();

            _invalidateConcurrencyRows.Clear();
            _invalidateRunRows.Clear();

            foreach (TileStresstest ts in _progress.Keys) {
                var metricsCache = _progress[ts];
                _concurrencyStresstestMetricsRows.AddRange(GetUsableRows(ts.ToString(), FastStresstestMetricsHelper.MetricsToRows(metricsCache.GetConcurrencyMetrics(), chkReadable.Checked, metricsCache.SimplifiedMetrics)));
                _runStresstestMetricsRows.AddRange(GetUsableRows(ts.ToString(), FastStresstestMetricsHelper.MetricsToRows(metricsCache.GetRunMetrics(), chkReadable.Checked, metricsCache.SimplifiedMetrics)));
            }
            for (int i = 0; i != _concurrencyStresstestMetricsRows.Count; i++)
                _invalidateConcurrencyRows.Add(i);
            for (int i = 0; i != _runStresstestMetricsRows.Count; i++)
                _invalidateRunRows.Add(i);
        }
        /// <summary>
        /// Puts the tile stresstest tostring in front of the rows.
        /// </summary>
        /// <param name="tileStresstest"></param>
        /// <param name="rows"></param>
        private List<object[]> GetUsableRows(string tileStresstest, List<object[]> rows) {
            var usableRows = new List<object[]>();
            foreach (var row in rows) {
                var newRow = new object[row.LongLength + 1];
                newRow[0] = tileStresstest;
                row.CopyTo(newRow, 1);
                usableRows.Add(newRow);
            }
            return usableRows;
        }
        private void SetOverallFastConcurrencyResults() {
            try {
                if (!IsDisposed) {
                    int count = _concurrencyStresstestMetricsRows.Count;
                    if (dgvFastResults.RowCount == count && count != 0)
                        foreach (int i in _invalidateConcurrencyRows)
                            dgvFastResults.InvalidateRow(i);
                    else dgvFastResults.RowCount = count;
                    dgvFastResults.AutoResizeColumns();
                }
            } catch { }
        }
        private void SetOverallFastRunResults() {
            try {
                if (!IsDisposed) {
                    int count = _runStresstestMetricsRows.Count;
                    if (dgvFastResults.RowCount == count && count != 0)
                        foreach (int i in _invalidateRunRows)
                            dgvFastResults.InvalidateRow(i);
                    else dgvFastResults.RowCount = count;
                    dgvFastResults.AutoResizeColumns();
                }
            } catch { }
        }

        private void cboDrillDown_SelectedIndexChanged(object sender, EventArgs e) {
            SetOverallFastResultsOnGuiInteraction(false);
        }
        private void chkReadable_CheckedChanged(object sender, EventArgs e) {
            SetOverallFastResultsOnGuiInteraction(true);
        }
        private void SetOverallFastResultsOnGuiInteraction(bool readableChanged) {
            //Set the headers.
            dgvFastResults.Columns.Clear();
            string[] columnHeaders = null;
            columnHeaders = cboDrillDown.SelectedIndex == 0 ? FastStresstestMetricsHelper.GetMetricsHeadersConcurrency(chkReadable.Checked)
                : FastStresstestMetricsHelper.GetMetricsHeadersRun(chkReadable.Checked);

            if (readableChanged) RefreshRows();

            string[] newColumnHeaders = new string[columnHeaders.LongLength + 1];
            newColumnHeaders[0] = "Tile Stresstest";
            columnHeaders.CopyTo(newColumnHeaders, 1);

            var clms = new DataGridViewColumn[newColumnHeaders.Length];
            string clmPrefix = ToString() + "clm";
            for (int headerIndex = 0; headerIndex != newColumnHeaders.Length; headerIndex++) {
                string header = newColumnHeaders[headerIndex];
                var clm = new DataGridViewTextBoxColumn();
                clm.Name = clmPrefix + header;
                clm.HeaderText = header;

                clm.SortMode = DataGridViewColumnSortMode.NotSortable;
                //To allow 2 power 32 columns.
                clm.FillWeight = 1;

                clms[headerIndex] = clm;
            }

            dgvFastResults.Columns.AddRange(clms);

            if (cboDrillDown.SelectedIndex == 0) SetOverallFastConcurrencyResults(); else SetOverallFastRunResults();
        }

        private void dgvFastResults_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            try {
                var metrics = cboDrillDown.SelectedIndex == 0 ? _concurrencyStresstestMetricsRows : _runStresstestMetricsRows;
                if (e.RowIndex < metrics.Count) {
                    var row = metrics[e.RowIndex];
                    if (e.ColumnIndex < row.Length)
                        e.Value = row[e.ColumnIndex];
                }
            } catch { }
        }

        private void dgvFastResults_Scroll(object sender, ScrollEventArgs e) {
            var verticalScrollBar = typeof(DataGridView).GetProperty("VerticalScrollBar", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                                                        .GetValue(dgvFastResults) as ScrollBar;
            _keepFastResultsAtEnd = (verticalScrollBar.Value + verticalScrollBar.LargeChange + 1) >= verticalScrollBar.Maximum;
        }
        /// <summary>
        ///     When the row count changes.
        /// </summary>
        private void KeepFastResultsAtEnd() {
            if (_keepFastResultsAtEnd && dgvFastResults.RowCount != 0) {
                dgvFastResults.Scroll -= dgvFastResults_Scroll;
                dgvFastResults.FirstDisplayedScrollingRowIndex = dgvFastResults.RowCount - 1;
                dgvFastResults.Scroll += dgvFastResults_Scroll;
            }
        }

        private void btnSaveDisplayedResults_Click(object sender, EventArgs e) {
            var sfd = new SaveFileDialog();
            sfd.Title = "Where do you want to save the displayed results?";
            if (_distributedTest != null)
                sfd.FileName = _distributedTest.ToString().ReplaceInvalidWindowsFilenameChars('_');
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
        /// <param name="appendHeaders"></param>
        /// <param name="addStresstestColumn"></param>
        /// <returns></returns>
        private string GetDisplayedResults() {
            var sb = new StringBuilder();
            //Write column headers
            foreach (DataGridViewColumn clm in dgvFastResults.Columns) {
                sb.Append(clm.HeaderText);
                sb.Append("\t");
            }
            sb.AppendLine();

            //Select and write rows.
            List<object[]> rows = new List<object[]>();
            //if (lbtnStresstest.Active)
            rows = cboDrillDown.SelectedIndex == 0 ? _concurrencyStresstestMetricsRows : _runStresstestMetricsRows;
            //else {
            //    string monitorToString = null;
            //    foreach (var lbtnMonitor in _monitorLinkButtons) if (lbtnMonitor.Active) { monitorToString = lbtnMonitor.Text; break; }
            //    if (monitorToString != null)
            //        rows = (cboDrillDown.SelectedIndex == 0 ? _concurrencyMonitorMetricsRows : _runMonitorMetricsRows)[monitorToString];
            //}
            foreach (var row in rows) {
                sb.Append(row.Combine("\t"));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public List<EventViewItem> GetEvents() { return eventView.GetEvents(); }
        public void AppendMessages(string message, Level logLevel = Level.Info) {
            try { eventView.AddEvent((EventViewEventType)logLevel, message); } catch { }
        }

        private void btnExport_Click(object sender, EventArgs e) { eventView.Export(); }

        #endregion
    }
}