/*
 * Copyright 2013 (c) Sizing Servers Lab
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Results;
using vApus.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace vApus.DetailedResultsViewer {
    public partial class SettingsPanel : DockablePanel {

        public event EventHandler<ResultsSelectedEventArgs> ResultsSelected;
        public event EventHandler CancelGettingResults, DisableResultsPanel;

        private MySQLServerDialog _mySQLServerDialog = new MySQLServerDialog();
        [ThreadStatic]
        private static FilterDatabasesWorkItem _filterDatabasesWorkItem;
        private AutoResetEvent _waitHandle = new AutoResetEvent(false);

        private ResultsHelper _resultsHelper;

        private bool _initing = true;

        private DataTable _dataSource = null;
        private DataRow _currentRow = null; //RowEnter happens multiple times for some strange reason, use this to not execute the refresh code when not needed.
        private System.Timers.Timer _rowEnterTimer = new System.Timers.Timer(1000);

        public ResultsHelper ResultsHelper {
            get { return _resultsHelper; }
            set { _resultsHelper = value; }
        }
        /// <summary>
        /// Don't forget to set ResultsHelper.
        /// </summary>
        public SettingsPanel() {
            InitializeComponent();
            _rowEnterTimer.Elapsed += _rowEnterTimer_Elapsed;
            RefreshDatabases(true);
            _initing = false;
        }

        ~SettingsPanel() {
            try {
                _waitHandle.Dispose();
                if (_rowEnterTimer != null) {
                    try {
                        _rowEnterTimer.Stop();
                        _rowEnterTimer.Dispose();
                    } catch {
                        //Fails only on gui closed.
                    }
                }
                _rowEnterTimer = null;
            } catch { }
        }

        private void lblConnectToMySQL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            string connectionString = _mySQLServerDialog.ConnectionString;
            _mySQLServerDialog.ShowDialog();
            if (connectionString != _mySQLServerDialog.ConnectionString) RefreshDatabases(true);
        }
        private void llblRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) { RefreshDatabases(true); }
        private void filterDatabases_FilterChanged(object sender, EventArgs e) { RefreshDatabases(false); }
        public void RefreshDatabases(bool setAvailableTags) {
            var databaseActions = SetServerConnectStateInGui();

            _dataSource = null;
            dgvDatabases.DataSource = null;
            cboStressTest.Items.Clear();
            cboStressTest.Enabled = false;
            btnOverviewExportToExcel.Enabled = btnDeleteSelectedDbs.Enabled = false;
            if (databaseActions == null || setAvailableTags) filterResults.ClearAvailableTags();
            if (databaseActions == null) {
                if (ResultsSelected != null) ResultsSelected(this, new ResultsSelectedEventArgs(null, 0));
            } else {
                if (setAvailableTags) filterResults.SetAvailableTags(databaseActions);
                FillDatabasesDataGridView(databaseActions);
                cboStressTest.Enabled = true;
                btnOverviewExportToExcel.Enabled = btnDeleteSelectedDbs.Enabled = _dataSource.Rows.Count != 0;
            }
        }
        private DatabaseActions SetServerConnectStateInGui() {
            lblConnectToMySQL.Text = "Connect to a results MySQL server...";
            toolTip.SetToolTip(lblConnectToMySQL, null);

            try {
                if (_mySQLServerDialog.Connected) {
                    lblConnectToMySQL.Text = "Results server Connected!";
                    toolTip.SetToolTip(lblConnectToMySQL, "Click to choose another MySQL server.");

                    return new DatabaseActions() { ConnectionString = _mySQLServerDialog.ConnectionString };
                }
                if (!_initing) MessageBox.Show("Could not connect to the results server.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch {
            }

            return null;
        }

        private void FillDatabasesDataGridView(DatabaseActions databaseActions) {
            Cursor = Cursors.WaitCursor;
            try {
                _dataSource = null;
                dgvDatabases.DataSource = null;
                string filter = filterResults.Filter;
                _dataSource = new DataTable("dataSource");
                _dataSource.Columns.Add("CreatedAt", typeof(DateTime));
                _dataSource.Columns.Add("Tags");
                _dataSource.Columns.Add("Description");
                _dataSource.Columns.Add("Database");

                DataTable dbs = new DataTable("dbs");
                dbs.Columns.Add("db");

                var temp = databaseActions.GetDataTable("Show Databases;");
                foreach (DataRow rrDB in temp.Rows) {
                    string db = rrDB.ItemArray[0] as string;
                    if (db.StartsWith("vapus", StringComparison.OrdinalIgnoreCase)) dbs.Rows.Add(db);
                }

                int count = dbs.Rows.Count;

                foreach (DataRow dbsr in dbs.Rows) {
                    string database = dbsr.ItemArray[0] as string;
                    try {
                        //Thread.CurrentThread.CurrentCulture = cultureInfo;
                        if (_filterDatabasesWorkItem == null) _filterDatabasesWorkItem = new FilterDatabasesWorkItem();

                        using (var dba = new DatabaseActions() { ConnectionString = databaseActions.ConnectionString }) {
                            var arr = _filterDatabasesWorkItem.FilterDatabase(dba, database, filter);
                            if (arr != null) _dataSource.Rows.Add(arr);
                        }
                    } catch {
                        //Ignore
                    }
                }

                _dataSource.DefaultView.Sort = "CreatedAt DESC";
                _dataSource = _dataSource.DefaultView.ToTable();
                dgvDatabases.DataSource = _dataSource;

                dgvDatabases.Columns[0].HeaderText = "Created At";
                dgvDatabases.Columns[3].Visible = false;

                if (dgvDatabases.Rows.Count == 0) {
                    if (ResultsSelected != null) ResultsSelected(this, new ResultsSelectedEventArgs(null, 0));
                } else {
                    foreach (DataGridViewColumn clm in dgvDatabases.Columns) clm.SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (dgvDatabases.Rows.Count != 0) dgvDatabases.Rows[0].Selected = true;
                }
            } catch { }
            try { if (!Disposing && !IsDisposed) Cursor = Cursors.Arrow; } catch { }
        }

        private void dgvDatabases_RowEnter(object sender, DataGridViewCellEventArgs e) {
            if (_dataSource != null && e.RowIndex != -1 && e.RowIndex < _dataSource.Rows.Count && _dataSource.Rows[e.RowIndex] == _currentRow)
                return;

            if (DisableResultsPanel != null) DisableResultsPanel(this, null);

            if (_rowEnterTimer != null) {
                _rowEnterTimer.Stop();
                _rowEnterTimer.Elapsed -= _rowEnterTimer_Elapsed;
                _rowEnterTimer.SetTag(e);
                _rowEnterTimer.Elapsed += _rowEnterTimer_Elapsed;
                _rowEnterTimer.Start();
            }
        }
        private void _rowEnterTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if (_rowEnterTimer != null) {
                _rowEnterTimer.Stop();
                _rowEnterTimer.Elapsed -= _rowEnterTimer_Elapsed;
                try {
                    dgvDatabases_RowEnterDelayed(_rowEnterTimer.GetTag() as DataGridViewCellEventArgs);
                } catch { }
            }
        }

        private void dgvDatabases_RowEnterDelayed(DataGridViewCellEventArgs e) {
            if (_dataSource != null && e.RowIndex != -1 && e.RowIndex < _dataSource.Rows.Count && _dataSource.Rows[e.RowIndex] == _currentRow)
                return;

            SynchronizationContextWrapper.SynchronizationContext.Send((y) => {
                if (CancelGettingResults != null) CancelGettingResults(this, null);
            }, null);

            _resultsHelper.KillConnection();

            _currentRow = _dataSource.Rows[e.RowIndex];
            string databaseName = _currentRow[3] as string;

            string user, host, password;
            int port;
            _mySQLServerDialog.GetCurrentConnectionString(out user, out host, out port, out password);
            _resultsHelper.ConnectToExistingDatabase(host, port, databaseName, user, password);

            var stressTests = _resultsHelper.GetStressTests();

            SynchronizationContextWrapper.SynchronizationContext.Send((y) => {
                //Fill the stress test cbo
                cboStressTest.SelectedIndexChanged -= cboStressTest_SelectedIndexChanged;
                cboStressTest.Items.Clear();

                if (stressTests == null || stressTests.Rows.Count == 0) {
                    if (MessageBox.Show("The selected database appears to be invalid.\nClick 'Yes' to delete it. This is irreversible!", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                        ThreadPool.QueueUserWorkItem((x) => {
                            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                                _resultsHelper.DeleteResults();
                                RefreshDatabases(true);
                            }, null);
                        }, null);

                    } else if (ResultsSelected != null) ResultsSelected(this, new ResultsSelectedEventArgs(null, 0));
                } else {
                    if (stressTests.Rows.Count == 1) {
                        cboStressTest.Items.Add((string)stressTests.Rows[0].ItemArray[1] + " " + stressTests.Rows[0].ItemArray[2]);
                    } else {
                        cboStressTest.Items.Add("<All>");
                        foreach (DataRow stressTestRow in stressTests.Rows)
                            cboStressTest.Items.Add((string)stressTestRow.ItemArray[1] + " " + stressTestRow.ItemArray[2]);
                    }
                    cboStressTest.SelectedIndex = 0;

                    InvokeResultsSelected();
                }

                cboStressTest.SelectedIndexChanged += cboStressTest_SelectedIndexChanged;
            }, null);
        }
        private void cboStressTest_SelectedIndexChanged(object sender, EventArgs e) { InvokeResultsSelected(); }
        private void InvokeResultsSelected() {
            if (ResultsSelected != null) {
                int stressTestId = cboStressTest.SelectedIndex;
                if (stressTestId != -1) {
                    if (cboStressTest.Items.Count == 1) ++stressTestId;
                    ResultsSelected(this, new ResultsSelectedEventArgs(_currentRow[3] as string, stressTestId));
                }
            }
        }

        private class FilterDatabasesWorkItem {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="databaseActions"></param>
            /// <param name="database"></param>
            /// <param name="filter"></param>
            /// <returns>Contents of a data row.</returns>
            public object[] FilterDatabase(DatabaseActions databaseActions, string database, string filter) {
                try {
                    object[] itemArray = new object[4];

                    //Get the tags
                    var tags = databaseActions.GetDataTable("Select Tag From " + database + ".tags");
                    string t = string.Empty;
                    if (tags.Rows.Count != 0) {
                        int countMinusOne = tags.Rows.Count - 1;
                        if (tags.Rows.Count > countMinusOne)
                            for (int i = 0; i != countMinusOne; i++) t += tags.Rows[i].ItemArray[0] + ", ";
                        t += tags.Rows[countMinusOne].ItemArray[0];
                    }
                    itemArray[1] = t.Trim();

                    //Get the description
                    var description = databaseActions.GetDataTable("Select Description From " + database + ".description");
                    foreach (DataRow dr in description.Rows) {
                        itemArray[2] = dr.ItemArray[0];
                        break;
                    }

                    bool canAdd = true;
                    if (filter.Length != 0) {
                        string tagsAndDescription = itemArray[1] as string + " " + itemArray[2] as string;

                        //Filter on tags and description.
                        List<int> rows, columns, matchLengths;
                        vApus.Util.FindAndReplace.Find(filter, tagsAndDescription, out rows, out columns, out matchLengths, false, true);
                        if (rows.Count == 0) canAdd = false;
                    }

                    if (canAdd) {
                        //Get the DateTime, to be formatted later.
                        string[] dtParts = database.Substring(5).Split('_');//yyyy_MM_dd_HH_mm_ss_fffffff or MM_dd_yyyy_HH_mm_ss_fffffff

                        bool yearFirst = dtParts[0].Length == 4;
                        int year = yearFirst ? int.Parse(dtParts[0]) : int.Parse(dtParts[2]);
                        int month = yearFirst ? int.Parse(dtParts[1]) : int.Parse(dtParts[0]);
                        int day = yearFirst ? int.Parse(dtParts[2]) : int.Parse(dtParts[1]);
                        int hour = int.Parse(dtParts[3]);
                        int minute = int.Parse(dtParts[4]);
                        int second = int.Parse(dtParts[5]);
                        double rest = double.Parse(dtParts[6]) / Math.Pow(10, dtParts[6].Length);
                        DateTime dt = new DateTime(year, month, day, hour, minute, second);
                        dt = dt.AddSeconds(rest);
                        itemArray[0] = dt.ToString();

                        itemArray[3] = database;
                        return itemArray;
                    }
                } catch {
                }
                return null;
            }
        }

        public class ResultsSelectedEventArgs : EventArgs {
            public string Database { get; private set; }
            /// <summary>
            /// 0 for all
            /// </summary>
            public int StressTestId { get; private set; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="database"></param>
            /// <param name="stressTestId">0 for all</param>
            public ResultsSelectedEventArgs(string database, int stressTestId) {
                Database = database;
                StressTestId = stressTestId;
            }
        }

        async private void btnDeleteSelectedDbs_Click(object sender, EventArgs e) {
            if (_resultsHelper != null &&
                MessageBox.Show("Are you sure you want to delete the selected results database(s)?\nThis CANNOT be reverted!", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
                Cursor = Cursors.WaitCursor;

                try {
                    foreach (DataGridViewRow row in dgvDatabases.SelectedRows) {
                        string db = row.Cells[3].Value as string;
                        await Task.Run(() => {
                            if (!_resultsHelper.DeleteResults(db))
                                throw new Exception();
                        });
                    }
                } catch {
                    MessageBox.Show(string.Empty, "Failed deleting selected database(s).", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                _currentRow = null;
                RefreshDatabases(true);
                Cursor = Cursors.Default;
            }
        }

        private void btnOverviewExportToExcel_Click(object sender, EventArgs e) {
            if (_resultsHelper != null)
                try {
                    var databaseNames = new SortedDictionary<int, string>();
                    foreach (DataGridViewRow row in dgvDatabases.SelectedRows)
                        databaseNames.Add(row.Index, row.Cells[3].Value as string);

                    var overviewExportDialog = new OverviewExportToExcelDialog();
                    overviewExportDialog.Init(_resultsHelper, databaseNames.Values);
                    overviewExportDialog.ShowDialog();
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed exporting overview to Excel.", ex);
                    MessageBox.Show(string.Empty, "Failed exporting overview to Excel.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }
    }
}
