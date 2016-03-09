/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
 using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using vApus.Results;
using vApus.Util;

namespace vApus.DistributedTest {
    public partial class OveralFastResultsControl : UserControl {

        #region Fields
        private DistributedTest _distributedTest;

        //Caching the progress here.
        Dictionary<TileStressTest, FastStressTestMetricsCache> _progress = new Dictionary<TileStressTest, FastStressTestMetricsCache>();
        private List<object[]> _concurrencyStressTestMetricsRows = new List<object[]>();
        private List<object[]> _runStressTestMetricsRows = new List<object[]>();

        private List<int> _invalidateConcurrencyRows = new List<int>();
        private List<int> _invalidateRunRows = new List<int>();
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
            dgvFastResults.ColumnHeadersDefaultCellStyle.Font = new Font(dgvFastResults.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
        }
        #endregion

        #region Functions
        public void ClearFastResults() {
            dgvFastResults.RowCount = 0;

            _progress = new Dictionary<TileStressTest, FastStressTestMetricsCache>();

            _concurrencyStressTestMetricsRows = new List<object[]>();
            _runStressTestMetricsRows = new List<object[]>();

            _invalidateConcurrencyRows = new List<int>();
            _invalidateRunRows = new List<int>();
            eventView.ClearEvents();

            kvpOK.Visible = kvpCancelled.Visible = kvpFailed.Visible = false;
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
        /// <returns>the last warning</returns>
        public string SetMasterMonitoring(int runningTests = 0, int ok = 0, int cancelled = 0, int failed = 0, float cpuUsage = -1f,
                                        int memoryUsage = -1, int totalVisibleMemory = -1, string nic = "NIC", float nicBandwidth = -1, float nicsSent = -1, float nicsReceived = -1) {

            string lastWarning = string.Empty;

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

                    lastWarning = cpuUsage + " % CPU Usage";
                    AppendMessages(lastWarning, Level.Warning);
                }
            }

            if (memoryUsage == -1 || totalVisibleMemory == -1) {
                kvmMasterMemoryUsage.Value = "N/A";
            } else {
                kvmMasterMemoryUsage.Value = memoryUsage.ToString() + " / " + totalVisibleMemory + " MB";
                if (memoryUsage < 0.9 * totalVisibleMemory) {
                    kvmMasterMemoryUsage.BackColor = Color.GhostWhite;
                } else if (memoryUsage != 0) {
                    kvmMasterMemoryUsage.BackColor = Color.Orange;

                    lastWarning = memoryUsage + " of " + totalVisibleMemory + " MB used";
                    AppendMessages(lastWarning, Level.Warning);
                }
            }
            kvmMasterNic.Key = nic;
            if (nicBandwidth == -1)
                kvmMasterNic.Value = "N/A";
            else
                kvmMasterNic.Value = nicBandwidth + " Mbps";
            if (nicsSent == -1) {
                kvmMasterNicsSent.Value = "N/A";
            } else {
                kvmMasterNicsSent.Value = Math.Round(nicsSent, 2).ToString() + " %";
                if (nicsSent < 90) {
                    kvmMasterNicsSent.BackColor = Color.GhostWhite;
                } else if (!float.IsPositiveInfinity(nicsSent) && !float.IsNegativeInfinity(nicsSent)) {
                    kvmMasterNicsSent.BackColor = Color.Orange;

                    lastWarning = nicsSent + " % NIC Usage (Sent)";
                    AppendMessages(lastWarning, Level.Warning);
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

                    lastWarning = nicsReceived + " % NIC Usage (Received)";
                    AppendMessages(lastWarning, Level.Warning);
                }
            }
            return lastWarning;
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
        public void SetOverallFastResults(Dictionary<TileStressTest, FastStressTestMetricsCache> progress) {
            _progress = progress;

            RefreshRows();
            if (cboDrillDown.SelectedIndex == 0) SetOverallFastConcurrencyResults(); else SetOverallFastRunResults();
        }
        private void RefreshRows() {
            _concurrencyStressTestMetricsRows.Clear();
            _runStressTestMetricsRows.Clear();

            _invalidateConcurrencyRows.Clear();
            _invalidateRunRows.Clear();

            foreach (TileStressTest ts in _progress.Keys) {
                var metricsCache = _progress[ts];
                _concurrencyStressTestMetricsRows.AddRange(GetUsableRows(ts.ToString(), FastStressTestMetricsHelper.MetricsToRows(metricsCache.GetConcurrencyMetrics(true), chkReadable.Checked)));
                _runStressTestMetricsRows.AddRange(GetUsableRows(ts.ToString(), FastStressTestMetricsHelper.MetricsToRows(metricsCache.GetRunMetrics(true), chkReadable.Checked)));
            }
            for (int i = 0; i != _concurrencyStressTestMetricsRows.Count; i++)
                _invalidateConcurrencyRows.Add(i);
            for (int i = 0; i != _runStressTestMetricsRows.Count; i++)
                _invalidateRunRows.Add(i);
        }
        /// <summary>
        /// Puts the tile stress test tostring in front of the rows.
        /// </summary>
        /// <param name="tileStressTest"></param>
        /// <param name="rows"></param>
        private List<object[]> GetUsableRows(string tileStressTest, List<object[]> rows) {
            var usableRows = new List<object[]>();
            foreach (var row in rows) {
                var newRow = new object[row.LongLength + 1];
                newRow[0] = tileStressTest;
                row.CopyTo(newRow, 1);
                usableRows.Add(newRow);
            }
            return usableRows;
        }
        private void SetOverallFastConcurrencyResults() {
            try {
                if (!IsDisposed) {
                    int count = _concurrencyStressTestMetricsRows.Count;
                    if (dgvFastResults.RowCount == count && count != 0)
                        foreach (int i in _invalidateConcurrencyRows)
                            dgvFastResults.InvalidateRow(i);
                    else dgvFastResults.RowCount = count;
                    dgvFastResults.AutoResizeColumns();
                }
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed setting overal fast concurrency results.", ex);
            }
        }
        private void SetOverallFastRunResults() {
            try {
                if (!IsDisposed) {
                    int count = _runStressTestMetricsRows.Count;
                    if (dgvFastResults.RowCount == count && count != 0)
                        foreach (int i in _invalidateRunRows)
                            dgvFastResults.InvalidateRow(i);
                    else dgvFastResults.RowCount = count;
                    dgvFastResults.AutoResizeColumns();
                }
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed setting overal fast run results.", ex);
            }
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
            columnHeaders = cboDrillDown.SelectedIndex == 0 ? FastStressTestMetricsHelper.GetMetricsHeadersConcurrency(chkReadable.Checked)
                : FastStressTestMetricsHelper.GetMetricsHeadersRun(chkReadable.Checked);

            if (readableChanged) RefreshRows();

            string[] newColumnHeaders = new string[columnHeaders.LongLength + 1];
            newColumnHeaders[0] = "Tile stress test";
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
                var metrics = cboDrillDown.SelectedIndex == 0 ? _concurrencyStressTestMetricsRows : _runStressTestMetricsRows;
                if (e.RowIndex < metrics.Count) {
                    var row = metrics[e.RowIndex];
                    if (e.ColumnIndex < row.Length)
                        e.Value = row[e.ColumnIndex];
                }
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed displaying cell value.", ex, new object[] { sender, e });
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
            var rows = cboDrillDown.SelectedIndex == 0 ? _concurrencyStressTestMetricsRows : _runStressTestMetricsRows;

            foreach (var row in rows) {
                sb.Append(row.Combine("\t"));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public EventPanelEvent[] GetEvents() { return eventView.GetEvents(); }
        public void AppendMessages(string message, Level logLevel = Level.Info) {
            try {
                eventView.AddEvent((EventViewEventType)logLevel, message);
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed appending event.", ex, new object[] { message, logLevel });
            }
        }

        private void btnExport_Click(object sender, EventArgs e) { eventView.Export(); }

        #endregion
    }
}