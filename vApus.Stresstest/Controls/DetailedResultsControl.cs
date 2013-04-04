/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Results;
using vApus.Util;

namespace vApus.Stresstest {
    public partial class DetailedResultsControl : UserControl {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        private KeyValuePairControl[] _config = new KeyValuePairControl[0];
        private ResultsHelper _resultsHelper;

        private ulong[] _stresstestIds = new ulong[0];

        private int _currentSelectedIndex = -1; //The event is raised even when the index stays the same, this is used to avoid it;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource(); //Cancel refreshing the report.

        private System.Timers.Timer _tmr = new System.Timers.Timer(500);
        private readonly object _lock = new object();

        public DetailedResultsControl() {
            InitializeComponent();

            //Double buffer the datagridview.
            (dgvDetailedResults).GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(dgvDetailedResults, true);

            btnCollapseExpand.PerformClick();
            chkAdvanced.Checked = false;
            cboShow.SelectedIndex = 0;

            _tmr.Elapsed += _tmr_Elapsed;

            this.VisibleChanged += DetailedResultsControl_VisibleChanged;
        }

        private void DetailedResultsControl_VisibleChanged(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;
            dgvDetailedResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            SizeColumns();
        }

        private void chkAdvanced_CheckedChanged(object sender, EventArgs e) { splitQueryData.Panel1Collapsed = !chkAdvanced.Checked; }

        private void lbtnDescription_ActiveChanged(object sender, EventArgs e) { if (lbtnDescription.Active) SetConfig(_resultsHelper.GetDescription()); }
        private void lbtnTags_ActiveChanged(object sender, EventArgs e) { if (lbtnTags.Active)  SetConfig(_resultsHelper.GetTags()); }
        private void lbtnvApusInstance_ActiveChanged(object sender, EventArgs e) { if (lbtnvApusInstance.Active)  SetConfig(_resultsHelper.GetvApusInstances()); }
        private void lbtnStresstest_ActiveChanged(object sender, EventArgs e) { if (lbtnStresstest.Active)  SetConfig(_resultsHelper.GetStresstestConfigurations()); }
        private void lbtnMonitors_ActiveChanged(object sender, EventArgs e) { if (lbtnMonitors.Active)  SetConfig(_resultsHelper.GetMonitors()); }

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
            LockWindowUpdate(this.Handle.ToInt32());
            foreach (var v in _config) flpConfiguration.Controls.Remove(v);
            _config = new KeyValuePairControl[keyValues.Count];
            int i = 0;
            foreach (var kvp in keyValues) _config[i++] = new KeyValuePairControl(kvp.Key, kvp.Value) { BackColor = SystemColors.Control };
            flpConfiguration.Controls.AddRange(_config);
            LockWindowUpdate(0);
        }

        async private void cboShow_SelectedIndexChanged(object sender, EventArgs e) {
            if (cboShow.SelectedIndex != _currentSelectedIndex) {
                _currentSelectedIndex = cboShow.SelectedIndex;
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();

                dgvDetailedResults.DataSource = null;
                dgvDetailedResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                flpConfiguration.Enabled = pnlBorderCollapse.Enabled = splitQueryData.Enabled = chkAdvanced.Enabled = btnSaveDisplayedResults.Enabled = btnExportToExcel.Enabled = false;
                lblLoading.Visible = true;

                int retry = 0;
            Retry:
                if (_resultsHelper != null) {
                    DataTable dt = null;
                    var cultureInfo = Thread.CurrentThread.CurrentCulture;
                    try {
                        dt = await Task.Run<DataTable>(() => GetDataSource(_cancellationTokenSource.Token, cultureInfo), _cancellationTokenSource.Token);
                    } catch {
                    }

                    //Stuff tends to happen out of order when cancelling, therefore this check, so we don't have an empty datagridview and retry 3 times.
                    if (dt == null) {
                        if (retry++ < 2)
                            goto Retry;
                    } else {
                        dgvDetailedResults.DataSource = dt;
                    }
                }

                SizeColumns();

                lblLoading.Visible = false;
                flpConfiguration.Enabled = pnlBorderCollapse.Enabled = splitQueryData.Enabled = chkAdvanced.Enabled = btnSaveDisplayedResults.Enabled = btnExportToExcel.Enabled = true;
                dgvDetailedResults.Select();
            }
        }
        private DataTable GetDataSource(CancellationToken cancellationToken, CultureInfo cultureInfo) {
            if (!cancellationToken.IsCancellationRequested) {
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                switch (_currentSelectedIndex) {
                    case 0: return _resultsHelper.GetAverageConcurrentUsers(cancellationToken, _stresstestIds);
                    case 1: return _resultsHelper.GetAverageUserActions(cancellationToken, _stresstestIds);
                    case 2: return _resultsHelper.GetAverageLogEntries(cancellationToken, _stresstestIds);
                    case 3: return _resultsHelper.GetErrors(cancellationToken, _stresstestIds);
                    case 4: return _resultsHelper.GetUserActionComposition(cancellationToken, _stresstestIds);
                    case 5: return _resultsHelper.GetCummulativeResponseTimesVsAchievedThroughput(cancellationToken, _stresstestIds);
                    case 6: return _resultsHelper.GetMachineConfigurations(cancellationToken, _stresstestIds);
                    case 7: return _resultsHelper.GetAverageMonitorResults(cancellationToken, _stresstestIds);
                }
            }
            return null;
        }

        async private void btnExecute_Click(object sender, EventArgs e) {
            flpConfiguration.Enabled = pnlBorderCollapse.Enabled = splitQueryData.Enabled = chkAdvanced.Enabled = btnSaveDisplayedResults.Enabled = btnExportToExcel.Enabled = false;

            dgvDetailedResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            try {
                dgvDetailedResults.DataSource = await Task.Run<DataTable>(() => ExecuteQuery(codeTextBox.Text, cultureInfo), _cancellationTokenSource.Token);
            } catch { }

            SizeColumns();

            lblLoading.Visible = false;
            flpConfiguration.Enabled = pnlBorderCollapse.Enabled = splitQueryData.Enabled = chkAdvanced.Enabled = btnSaveDisplayedResults.Enabled = btnExportToExcel.Enabled = true;
        }
        private DataTable ExecuteQuery(string query, CultureInfo cultureInfo) {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            return _resultsHelper.ExecuteQuery(query);
        }
        private void SizeColumns() {
            if (_tmr != null) {
                _tmr.Stop();
                _tmr.Start();
            }
        }
        private void _tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            lock (_lock) {
                _tmr.Stop();
                SynchronizationContextWrapper.SynchronizationContext.Send((x) => {
                    int[] widths = new int[dgvDetailedResults.ColumnCount];
                    for (int i = 0; i != widths.Length; i++) {
                        int width = dgvDetailedResults.Columns[i].Width;
                        widths[i] = width > 500 ? 500 : width;
                    }
                    dgvDetailedResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    for (int i = 0; i != widths.Length; i++)
                        dgvDetailedResults.Columns[i].Width = widths[i];
                }, null);
            }
        }
        /// <summary>
        /// Clear before testing.
        /// </summary>
        public void ClearResults() {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            foreach (var v in _config) flpConfiguration.Controls.Remove(v);
            _config = new KeyValuePairControl[0];

            dgvDetailedResults.DataSource = null;
        }
        /// <summary>
        /// Refresh after testing.
        /// </summary>
        /// <param name="resultsHelper">Give hte helper that made the db</param>
        /// <param name="stresstestIds">Filter on one or more stresstests, if this is empty no filter is applied.</param>
        public void RefreshResults(ResultsHelper resultsHelper, params ulong[] stresstestIds) {
            _resultsHelper = resultsHelper;
            _stresstestIds = stresstestIds;
            foreach (var ctrl in flpConfiguration.Controls)
                if (ctrl is LinkButton) {
                    var lbtn = ctrl as LinkButton;
                    if (lbtn.Active) {
                        lbtn.PerformClick();
                        break;
                    }
                }
            _currentSelectedIndex = -1;
            cboShow_SelectedIndexChanged(null, null);
        }

        private void btnExportToExcel_Click(object sender, EventArgs e) {
            var dialog = new ExportToExcelDialog();
            dialog.Init(_resultsHelper);
            dialog.ShowDialog();
        }
    }
}
