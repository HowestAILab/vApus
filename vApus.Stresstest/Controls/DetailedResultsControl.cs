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
        public event EventHandler ResultsDeleted;

        #region Fields
        private readonly object _lock = new object();

        private KeyValuePairControl[] _config = new KeyValuePairControl[0];
        private ResultsHelper _resultsHelper;

        private int[] _stresstestIds = new int[0];

        private int _currentSelectedIndex = -1; //The event is raised even when the index stays the same, this is used to avoid it;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource(); //Cancel refreshing the report.


        private System.Timers.Timer _tmrSizeColumns = new System.Timers.Timer(500); //To not let the columns become to wide.
        #endregion

        #region Constructors
        public DetailedResultsControl() {
            InitializeComponent();

            //Double buffer the datagridview.
            (dgvDetailedResults).GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(dgvDetailedResults, true);

            //Set the GUI, do this after Initialization.
            btnCollapseExpand.PerformClick();
            chkAdvanced.Checked = false;

            _tmrSizeColumns.Elapsed += _tmrSizeColumns_Elapsed;

            this.VisibleChanged += DetailedResultsControl_VisibleChanged;
        }
        #endregion

        #region Functions
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        private void DetailedResultsControl_VisibleChanged(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;
            if (dgvDetailedResults.Columns.Count < 100) dgvDetailedResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            SizeColumns();
        }

        private void lbtnDescription_ActiveChanged(object sender, EventArgs e) { if (lbtnDescription.Active) SetConfig(_resultsHelper.GetDescription().Replace('\r', ' ').Replace('\n', ' ')); }
        private void lbtnTags_ActiveChanged(object sender, EventArgs e) { if (lbtnTags.Active)  SetConfig(_resultsHelper.GetTags()); }
        private void lbtnvApusInstance_ActiveChanged(object sender, EventArgs e) { if (lbtnvApusInstance.Active)  SetConfig(_resultsHelper.GetvApusInstances()); }
        private void lbtnStresstest_ActiveChanged(object sender, EventArgs e) { if (lbtnStresstest.Active)  SetConfig(_resultsHelper.GetStresstestConfigurations()); }
        private void lbtnMonitors_ActiveChanged(object sender, EventArgs e) { if (lbtnMonitors.Active)  SetConfig(_resultsHelper.GetMonitors()); }

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

        private void btnCollapseExpand_Click(object sender, EventArgs e) {
            if (btnCollapseExpand.Text == "-") {
                btnCollapseExpand.Text = "+";

                splitContainer.SplitterDistance = splitContainer.Panel1MinSize;
                splitContainer.IsSplitterFixed = true;
                splitContainer.BackColor = Color.White;
            } else ExpandConfig();
        }

        async private void cboShow_SelectedIndexChanged(object sender, EventArgs e) {
            if (cboShow.SelectedIndex != _currentSelectedIndex) {

                _currentSelectedIndex = cboShow.SelectedIndex;
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();

                dgvDetailedResults.DataSource = null;
                dgvDetailedResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                flpConfiguration.Enabled = pnlBorderCollapse.Enabled = splitQueryData.Enabled = chkAdvanced.Enabled = btnSaveDisplayedResults.Enabled = btnExportToExcel.Enabled = btnDeleteResults.Enabled = false;
                lblLoading.Visible = true;

                int retry = 0;
            Retry:
                if (cboShow.SelectedIndex != -1 && _resultsHelper != null) {
                    DataTable dt = null;
                    Exception exception = null;
                    var cultureInfo = Thread.CurrentThread.CurrentCulture;
                    try {
                        dt = await Task.Run<DataTable>(() => GetDataSource(_cancellationTokenSource.Token, cultureInfo), _cancellationTokenSource.Token);
                    } catch (Exception ex) {
                        exception = ex;
                    }

                    //Stuff tends to happen out of order when cancelling, therefore this check, so we don't have an empty datagridview and retry 3 times.
                    if (dt == null) {
                        if (retry++ < 2)
                            goto Retry;
                        else if (exception != null)
                            try {
                                LogWrapper.LogByLevel("Failed loading detailed results.\n" + exception.Message + "\n" + exception.StackTrace, LogLevel.Error);
                            } catch { }
                    } else {
                        if (dt.Columns.Count < 100) dgvDetailedResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        dgvDetailedResults.DataSource = dt;
                    }
                }

                SizeColumns();

                lblLoading.Visible = false;
                flpConfiguration.Enabled = pnlBorderCollapse.Enabled = splitQueryData.Enabled = chkAdvanced.Enabled = btnSaveDisplayedResults.Enabled = btnExportToExcel.Enabled = btnDeleteResults.Enabled = true;
                dgvDetailedResults.Select();

                FillCellView();
            }
        }
        private DataTable GetDataSource(CancellationToken cancellationToken, CultureInfo cultureInfo) {
            if (!cancellationToken.IsCancellationRequested) {
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Dictionary<string, List<string>> stub;
                switch (_currentSelectedIndex) {
                    case 0: return _resultsHelper.GetOverview(cancellationToken, _stresstestIds);
                    case 1: return _resultsHelper.GetAverageConcurrencyResults(cancellationToken, _stresstestIds);
                    case 2: return _resultsHelper.GetAverageUserActionResults(cancellationToken, _stresstestIds);
                    case 3: return _resultsHelper.GetAverageLogEntryResults(cancellationToken, _stresstestIds);
                    case 4: return _resultsHelper.GetErrors(cancellationToken, _stresstestIds);
                    case 5: return _resultsHelper.GetUserActionComposition(cancellationToken, _stresstestIds);
                    case 6: return _resultsHelper.GetMachineConfigurations(cancellationToken, _stresstestIds);
                    case 7: return _resultsHelper.GetAverageMonitorResults(cancellationToken, _stresstestIds);
                    case 8: return _resultsHelper.GetRunsOverTime(cancellationToken, out stub, _stresstestIds);
                }
            }
            return null;
        }
        private void SizeColumns() {
            if (_tmrSizeColumns != null && dgvDetailedResults.Columns.Count < 100) {
                _tmrSizeColumns.Stop();

                if (dgvDetailedResults.Columns.Count > 1)
                    _tmrSizeColumns.Start();
            }
        }
        private void _tmrSizeColumns_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            lock (_lock) {
                _tmrSizeColumns.Stop();
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

        private void chkAdvanced_CheckedChanged(object sender, EventArgs e) { splitQueryData.Panel1Collapsed = !chkAdvanced.Checked; }

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

        private void btnExportToExcel_Click(object sender, EventArgs e) {
            var dialog = new ExportToExcelDialog();
            dialog.Init(_resultsHelper);
            dialog.ShowDialog();
        }

        async private void btnExecute_Click(object sender, EventArgs e) {
            flpConfiguration.Enabled = pnlBorderCollapse.Enabled = splitQueryData.Enabled = chkAdvanced.Enabled = btnSaveDisplayedResults.Enabled = btnExportToExcel.Enabled = btnDeleteResults.Enabled = false;

            dgvDetailedResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            try {
                dgvDetailedResults.DataSource = await Task.Run<DataTable>(() => ExecuteQuery(codeTextBox.Text, cultureInfo), _cancellationTokenSource.Token);
            } catch { }

            SizeColumns();

            lblLoading.Visible = false;
            flpConfiguration.Enabled = pnlBorderCollapse.Enabled = splitQueryData.Enabled = chkAdvanced.Enabled = btnSaveDisplayedResults.Enabled = btnExportToExcel.Enabled = btnDeleteResults.Enabled = true;
        }
        private DataTable ExecuteQuery(string query, CultureInfo cultureInfo) {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            return _resultsHelper.ExecuteQuery(query);
        }

        private void btnDeleteResults_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to delete the results database?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                _resultsHelper.DeleteResults();
                this.Enabled = false;

                if (ResultsDeleted != null) ResultsDeleted(this, null);
            }
        }

        private void chkShowCellView_CheckedChanged(object sender, EventArgs e) { FillCellView(); }
        private void dgvDetailedResults_CellEnter(object sender, DataGridViewCellEventArgs e) { FillCellView(); }
        private void FillCellView() {
            try {
                //Sadly enough DIY control composition due to dodgy Winforms/Weifenluo.
                if (chkShowCellView.Checked) {
                    splitData.Panel2Collapsed = false;

                    fctxtCellView.Width = splitData.Panel2.Width - fctxtCellView.Left;
                    fctxtCellView.Height = splitData.Panel2.Height - fctxtCellView.Top;
                    fctxtCellView.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

                    //Set the text.
                    if (dgvDetailedResults.SelectedCells.Count == 1) dgvDetailedResults.CurrentCell = dgvDetailedResults.SelectedCells[0];
                    fctxtCellView.Enabled = dgvDetailedResults.CurrentCell != null && dgvDetailedResults.SelectedCells.Count == 1;
                    fctxtCellView.Text = fctxtCellView.Enabled ? dgvDetailedResults.CurrentCell.Value.ToString() : string.Empty;

                } else {
                    fctxtCellView.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    splitData.Panel2Collapsed = true;
                }
            } catch {
            }
        }

        /// <summary>
        /// Clear before testing.
        /// </summary>
        public void ClearResults() {
            if (_cancellationTokenSource != null) _cancellationTokenSource.Cancel();

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
        public void RefreshResults(ResultsHelper resultsHelper, params int[] stresstestIds) {
            this.Enabled = true;

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
            _currentSelectedIndex = int.MinValue;
            cboShow.SelectedIndex = -1;
        }
        #endregion
    }
}
