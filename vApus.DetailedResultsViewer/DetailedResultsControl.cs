/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using RandomUtils.Log;
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

namespace vApus.StressTest {
    public partial class DetailedResultsControl : UserControl {
        public event EventHandler ResultsDeleted;

        private event EventHandler<OnResultsEventArgs> OnResults;

        #region Fields
        private readonly object _lock = new object();
        private Thread _workerThread;
        private ManualResetEvent _waitHandle = new ManualResetEvent(true);

        private KeyValuePairControl[] _config = new KeyValuePairControl[0];
        private ResultsHelper _resultsHelper;

        private int[] _stressTestIds = new int[0];

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

            cboShow.HandleCreated += cboShow_HandleCreated;

            OnResults += DetailedResultsControl_OnResults;

            fctxtCellView.DefaultContextMenu(true);

            //Stupid workaround.
            dgvDetailedResults.ColumnHeadersDefaultCellStyle.Font = new Font(dgvDetailedResults.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
        }
        #endregion

        #region Functions
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        private void DetailedResultsControl_VisibleChanged(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;
            if (dgvDetailedResults.Columns.Count < 100) dgvDetailedResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            SizeColumns();
        }
        private void cboShow_HandleCreated(object sender, EventArgs e) {
            SendMessageWrapper.SetComboboxCueBar(cboShow.Handle, "Please, select a result set...");
        }

        private void lbtnDescription_ActiveChanged(object sender, EventArgs e) {
            if (lbtnDescription.Active) SetDescriptionConfig();
        }
        private void SetDescriptionConfig() {
            string description = _resultsHelper.GetDescription();

            SetConfig(description);

            var btnEditDescription = new Button();
            btnEditDescription.Text = "Edit...";
            btnEditDescription.Tag = description;
            btnEditDescription.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnEditDescription.AutoSize = true;
            btnEditDescription.Click += btnEditDescription_Click;

            flpConfiguration.Controls.Add(btnEditDescription);
        }
        private void btnEditDescription_Click(object sender, EventArgs e) {
            var fromTextDialog = new FromTextDialog();
            fromTextDialog.Text = "Please enter a description";
            fromTextDialog.SetText((sender as Button).Tag.ToString());
            if (fromTextDialog.ShowDialog() == DialogResult.OK) {
                _resultsHelper.SetDescriptionAndTags(fromTextDialog.GetText().Trim(), _resultsHelper.GetTags().ToArray());

                SetDescriptionConfig();
            }
        }

        private void lbtnTags_ActiveChanged(object sender, EventArgs e) { if (lbtnTags.Active)  SetTagsConfig(); }
        private void SetTagsConfig() {
            List<string> tags = _resultsHelper.GetTags();

            SetConfig(tags);

            var btnEditTags = new Button();
            btnEditTags.Text = "Edit...";
            btnEditTags.Tag = tags;
            btnEditTags.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnEditTags.AutoSize = true;
            btnEditTags.Click += btnEditTags_Click;

            flpConfiguration.Controls.Add(btnEditTags);
        }
        private void btnEditTags_Click(object sender, EventArgs e) {
            var inputDialog = new InputDialog("Please enter comma-seperated tags:", string.Empty, ((sender as Button).Tag as List<string>).Combine(", "));
            if (inputDialog.ShowDialog() == DialogResult.OK) {
                string[] arr = inputDialog.Input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i != arr.Length; i++)
                    arr[i] = arr[i].Trim();

                _resultsHelper.SetDescriptionAndTags(_resultsHelper.GetDescription(), arr);

                SetTagsConfig();
            }
        }

        private void lbtnvApusInstance_ActiveChanged(object sender, EventArgs e) { if (lbtnvApusInstance.Active)  SetConfig(_resultsHelper.GetvApusInstances()); }
        private void lbtnStressTest_ActiveChanged(object sender, EventArgs e) { if (lbtnStressTest.Active)  SetConfig(_resultsHelper.GetStressTestConfigurations()); }
        private void lbtnMonitors_ActiveChanged(object sender, EventArgs e) { if (lbtnMonitors.Active)  SetConfig(_resultsHelper.GetMonitors()); }

        private void SetConfig(string value) {
            foreach (Control ctrl in flpConfiguration.Controls)
                if (ctrl is Button) {
                    flpConfiguration.Controls.Remove(ctrl);
                    break;
                }

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
            foreach (Control ctrl in flpConfiguration.Controls)
                if (ctrl is Button) {
                    flpConfiguration.Controls.Remove(ctrl);
                    break;
                }

            foreach (var v in _config) flpConfiguration.Controls.Remove(v);
            _config = new KeyValuePairControl[values.Count];
            for (int i = 0; i != _config.Length; i++) _config[i] = new KeyValuePairControl(values[i], string.Empty) { BackColor = SystemColors.Control };
            flpConfiguration.Controls.AddRange(_config);
        }
        private void SetConfig(List<KeyValuePair<string, string>> keyValues) {
            LockWindowUpdate(Handle);
            foreach (Control ctrl in flpConfiguration.Controls)
                if (ctrl is Button) {
                    flpConfiguration.Controls.Remove(ctrl);
                    break;
                }

            foreach (var v in _config) flpConfiguration.Controls.Remove(v);
            _config = new KeyValuePairControl[keyValues.Count];
            int i = 0;
            foreach (var kvp in keyValues) _config[i++] = new KeyValuePairControl(kvp.Key, kvp.Value) { BackColor = SystemColors.Control };
            flpConfiguration.Controls.AddRange(_config);
            LockWindowUpdate(IntPtr.Zero);
        }

        private void btnCollapseExpand_Click(object sender, EventArgs e) {
            if (btnCollapseExpand.Text == "-") {
                btnCollapseExpand.Text = "+";

                splitContainer.SplitterDistance = splitContainer.Panel1MinSize;
                splitContainer.IsSplitterFixed = true;
                splitContainer.BackColor = Color.White;
            } else ExpandConfig();
        }

        private void cboShow_SelectedIndexChanged(object sender, EventArgs e) {
            lock (_lock) {
                _waitHandle.Reset();
                this.Enabled = false;

                if (_cancellationTokenSource != null) _cancellationTokenSource.Cancel();

                lblLoading.Visible = false;
                flpConfiguration.Enabled = pnlBorderCollapse.Enabled = splitQueryData.Enabled = chkAdvanced.Enabled = btnSaveDisplayedResults.Enabled = btnExportToExcel.Enabled = btnDeleteResults.Enabled = true;
                dgvDetailedResults.Select();

                FillCellView();

                _cancellationTokenSource = new CancellationTokenSource();

                if (cboShow.SelectedIndex != _currentSelectedIndex) {
                    _currentSelectedIndex = cboShow.SelectedIndex;

                    dgvDetailedResults.DataSource = null;
                    dgvDetailedResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                    flpConfiguration.Enabled = pnlBorderCollapse.Enabled = splitQueryData.Enabled = chkAdvanced.Enabled = btnSaveDisplayedResults.Enabled = btnExportToExcel.Enabled = btnDeleteResults.Enabled = false;
                    lblLoading.Visible = true;


                    DetermineDataSource();
                } else {
                    _waitHandle.Set();

                    this.Enabled = true;
                }

                if (_currentSelectedIndex == 6) {
                    pnlBorderMonitors.Visible = true;
                    if (cboMonitors.Items.Count == 0) {
                        object first = "All monitors";
                        first.SetTag(0);
                        cboMonitors.Items.Add(first);
                        foreach (var kvp in _resultsHelper.GetMonitors(_stressTestIds)) {
                            object item = kvp.Value;
                            item.SetTag(kvp.Key);
                            cboMonitors.Items.Add(item);
                        }
                        cboMonitors.SelectedIndex = 0;
                    }
                } else {
                    foreach (object item in cboMonitors.Items)
                        item.RemoveTag();

                    cboMonitors.Items.Clear();
                }

                pnlBorderMonitors.Visible = _currentSelectedIndex == 6;
            }
        }
        private void cboMonitors_SelectedIndexChanged(object sender, EventArgs e) {
            int monitorId = (int)cboMonitors.SelectedItem.GetTag();

            if (_currentSelectedIndex == 6) {
                DataTable dt = null;
                if (monitorId == 0)
                    dt = _resultsHelper.GetAverageMonitorResults(_cancellationTokenSource.Token, _stressTestIds);
                else
                    dt = _resultsHelper.GetAverageMonitorResultsByMonitorId(_cancellationTokenSource.Token, monitorId);

                if (OnResults != null)
                    OnResults(this, new OnResultsEventArgs(dt));
            }
        }
        private void DetermineDataSource() {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested) {
                _workerThread = new Thread(() => {
                    try {
                        DataTable dt = null;
                        switch (_currentSelectedIndex) {
                            case 0: dt = _resultsHelper.GetAverageConcurrencyResults(_cancellationTokenSource.Token, _stressTestIds); break;
                            case 1: dt = _resultsHelper.GetAverageUserActionResults(_cancellationTokenSource.Token, _stressTestIds); break;
                            case 2: dt = _resultsHelper.GetAverageRequestResults(_cancellationTokenSource.Token, _stressTestIds); break;
                            case 3: dt = _resultsHelper.GetErrors(_cancellationTokenSource.Token, _stressTestIds); break;
                            case 4: dt = _resultsHelper.GetUserActionComposition(_cancellationTokenSource.Token, _stressTestIds); break;
                            case 5: dt = _resultsHelper.GetMachineConfigurations(_cancellationTokenSource.Token, _stressTestIds); break;
                            case 7: dt = _resultsHelper.GetMessages(_cancellationTokenSource.Token, _stressTestIds); break;
                        }
                        if (OnResults != null)
                            foreach (EventHandler<OnResultsEventArgs> del in OnResults.GetInvocationList())
                                if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                                    del(this, new OnResultsEventArgs(dt));
                    } catch (Exception ex) {
                        Loggers.Log(Level.Error, "Failed refreshing the results.", ex);
                    }
                    _waitHandle.Set();
                });
                _workerThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                _workerThread.Start();
            }
        }

        private void DetailedResultsControl_OnResults(object sender, DetailedResultsControl.OnResultsEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                //Stuff tends to happen out of order when cancelling, therefore this check, so we don't have an empty datagridview and retry 3 times.
                if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested && e.Results != null) {
                    if (e.Results.Columns.Count < 100) dgvDetailedResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    try {
                        dgvDetailedResults.DataSource = e.Results;
                    } catch {
                        dgvDetailedResults.DataSource = null;
                        var errorDt = new DataTable("Error");
                        errorDt.Columns.Add("Error");
                        errorDt.Rows.Add("This control cannot handle the amount (" + e.Results.Columns.Count + ") of columns in the result set. Exporting to Excel should not be a problem.");

                        dgvDetailedResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        dgvDetailedResults.DataSource = errorDt;
                    }
                }


                SizeColumns();

                lblLoading.Visible = false;
                flpConfiguration.Enabled = pnlBorderCollapse.Enabled = splitQueryData.Enabled = chkAdvanced.Enabled = btnSaveDisplayedResults.Enabled = btnExportToExcel.Enabled = btnDeleteResults.Enabled = true;
                dgvDetailedResults.Select();

                FillCellView();

                this.Enabled = true;
            }, null);
        }

        private void SizeColumns() {
            if (_tmrSizeColumns != null && dgvDetailedResults.Columns.Count < 100) {
                _tmrSizeColumns.Stop();

                if (dgvDetailedResults.Columns.Count > 1)
                    _tmrSizeColumns.Start();
            }
        }
        private void _tmrSizeColumns_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            // lock (_lock) {
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
            //  }
        }

        private void chkAdvanced_CheckedChanged(object sender, EventArgs e) { splitQueryData.Panel1Collapsed = !chkAdvanced.Checked; }

        private void btnSaveDisplayedResults_Click(object sender, EventArgs e) {
            if (cboShow.SelectedIndex == -1) return;

            var sfd = new SaveFileDialog();
            sfd.Title = "Where do you want to save the displayed results?";
            //sfd.FileName = kvpStressTest.Key.ReplaceInvalidWindowsFilenameChars('_');
            sfd.Filter = "TXT|*.txt";

            if (_resultsHelper.DatabaseName != null)
                sfd.FileName = _resultsHelper.DatabaseName + "_" + cboShow.SelectedItem.ToString().Replace(' ', '_').ToUpperInvariant() + ".txt";
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
        ///     Get the displayed results, description and tags ae also included.
        /// </summary>
        /// <returns></returns>
        private string GetDisplayedResults() {
            var sb = new StringBuilder();
            if (_resultsHelper.DatabaseName != null) {
                sb.AppendLine("Description:");
                sb.AppendLine(_resultsHelper.GetDescription());
                sb.AppendLine();
                sb.AppendLine("Tags:");
                sb.AppendLine(_resultsHelper.GetTags().Combine(" "));
                sb.AppendLine();
            }

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
            if (_resultsHelper != null)
                try {
                    var dialog = new RichExportToExcelDialog();
                    dialog.Init(_resultsHelper);
                    dialog.ShowDialog();
                    dialog = null;
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed exporting to Excel.", ex);
                    MessageBox.Show(string.Empty, "Failed exporting to Excel.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        public void AutoExportToExcel(string folder) {
            if (Directory.Exists(folder)) {
                var dialog = new RichExportToExcelDialog();
                dialog.Init(_resultsHelper);
                dialog.AutoExportToExcel(folder);
            }
        }

        async private void btnExecute_Click(object sender, EventArgs e) {
            flpConfiguration.Enabled = pnlBorderCollapse.Enabled = splitQueryData.Enabled = chkAdvanced.Enabled = btnSaveDisplayedResults.Enabled = btnExportToExcel.Enabled = btnDeleteResults.Enabled = false;

            dgvDetailedResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            try {
                dgvDetailedResults.DataSource = await Task.Run<DataTable>(() => { return ExecuteQuery(codeTextBox.Text, cultureInfo); });
            } catch (Exception ex) {
                Loggers.Log(Level.Warning, "Failed executing query.", ex, new object[] { sender, e });
            }

            SizeColumns();

            lblLoading.Visible = false;
            flpConfiguration.Enabled = pnlBorderCollapse.Enabled = splitQueryData.Enabled = chkAdvanced.Enabled = btnSaveDisplayedResults.Enabled = btnExportToExcel.Enabled = btnDeleteResults.Enabled = true;
        }
        private DataTable ExecuteQuery(string query, CultureInfo cultureInfo) {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            return _resultsHelper.ExecuteQuery(query);
        }

        private void btnDeleteResults_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to delete the results database?\nThis CANNOT be reverted!", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
                this.Enabled = false;
                _resultsHelper.DeleteResults();

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
            } catch (Exception ex) {
                Loggers.Log(Level.Warning, "Failed filling cell view.", ex);
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
            if (_resultsHelper != null) _resultsHelper.ClearCache(); //Keeping the cache as clean as possible.
        }
        /// <summary>
        /// Refresh after testing.
        /// </summary>
        /// <param name="resultsHelper">Give hte helper that made the db</param>
        /// <param name="stressTestIds">Filter on one or more stress tests, if this is empty no filter is applied.</param>
        public void RefreshResults(ResultsHelper resultsHelper, params int[] stressTestIds) {
            if (_cancellationTokenSource != null) _cancellationTokenSource.Cancel();

            this.Enabled = false;

            _resultsHelper = resultsHelper;
            if (_resultsHelper != null) _resultsHelper.ClearCache(); //Keeping the cache as clean as possible.
            _stressTestIds = stressTestIds;
            foreach (var ctrl in flpConfiguration.Controls)
                if (ctrl is LinkButton) {
                    var lbtn = ctrl as LinkButton;
                    if (lbtn.Active) {
                        lbtn.PerformClick();
                        break;
                    }
                }
            _currentSelectedIndex = int.MinValue;

            _waitHandle.WaitOne();

            this.Enabled = true;

            cboShow.SelectedIndex = -1; //If this is disabled the event will not take place.
        }
        #endregion

        private class OnResultsEventArgs : EventArgs {
            public DataTable Results { get; private set; }
            public OnResultsEventArgs(DataTable results) {
                Results = results;
            }
        }
    }
}
