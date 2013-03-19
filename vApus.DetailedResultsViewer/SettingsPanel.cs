/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using vApus.Results;
using vApus.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace vApus.DetailedResultsViewer {
    public partial class SettingsPanel : DockablePanel {

        public event EventHandler<ResultsSelectedEventArgs> ResultsSelected;

        private MySQLServerDialog _mySQLServerDialog = new MySQLServerDialog();
        [ThreadStatic]
        private static FilterDatabasesWorkItem _filterDatabasesWorkItem;
        private AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private readonly object _lock = new object();

        private ResultsHelper _resultsHelper;

        private bool _initing = true;

        private DataTable _dataSource = null;
        private DataRow _currentRow = null; //RowEnter happens multiple times for some strange reason, use this to not execute the refresh code when not needed.

        public ResultsHelper ResultsHelper {
            get { return _resultsHelper; }
            set { _resultsHelper = value; }
        }
        /// <summary>
        /// Don't forget to set ResultsHelper.
        /// </summary>
        public SettingsPanel() {
            InitializeComponent();
            RefreshDatabases(true);
            _initing = false;
        }
        ~SettingsPanel() { try { _waitHandle.Dispose(); } catch { } }

        private void lblConnectToMySQL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            string connectionString = _mySQLServerDialog.ConnectionString;
            _mySQLServerDialog.ShowDialog();
            if (connectionString != _mySQLServerDialog.ConnectionString) RefreshDatabases(true);
        }
        private void llblRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) { RefreshDatabases(true); }
        private void filterDatabases_FilterChanged(object sender, EventArgs e) { RefreshDatabases(false); }
        private void RefreshDatabases(bool setAvailableTags) {
            var databaseActions = SetServerConnectStateInGui();

            _dataSource = null;
            dgvDatabases.DataSource = null;
            if (databaseActions == null || setAvailableTags) filterResults.ClearAvailableTags();
            if (databaseActions != null) {
                if (setAvailableTags) filterResults.SetAvailableTags(databaseActions);
                FillDatabasesDataGridView(databaseActions);
            }
        }
        private DatabaseActions SetServerConnectStateInGui() {
            lblConnectToMySQL.Text = "Connect to a Results MySQL Server...";
            toolTip.SetToolTip(lblConnectToMySQL, null);

            if (_mySQLServerDialog.Connected) {
                lblConnectToMySQL.Text = "Results Server Connected!";
                toolTip.SetToolTip(lblConnectToMySQL, "Click to choose another MySQL server.");

                return new DatabaseActions() { ConnectionString = _mySQLServerDialog.ConnectionString };
            }

            if (!_initing) MessageBox.Show("Could not connect to the database server.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;

        }

        private void FillDatabasesDataGridView(DatabaseActions databaseActions) {
            Cursor = Cursors.WaitCursor;
            try {
                _dataSource = null;
                dgvDatabases.DataSource = null;
                string[] filter = filterResults.Filter;
                _dataSource = new DataTable("dataSource");
                _dataSource.Columns.Add("CreatedAt");
                _dataSource.Columns.Add("Tags");
                _dataSource.Columns.Add("Description");
                _dataSource.Columns.Add("Database");

                DataTable dbs = new DataTable("dbs");
                dbs.Columns.Add("db");

                var temp = databaseActions.GetDataTable("Show Databases;");
                foreach (DataRow rrDB in temp.Rows) {
                    string db = rrDB.ItemArray[0] as string;
                    if (db.StartsWith("vapus", StringComparison.InvariantCultureIgnoreCase)) dbs.Rows.Add(db);
                }

                int count = dbs.Rows.Count;
                int done = 0;
                foreach (DataRow dbsr in dbs.Rows) {
                    string database = dbsr.ItemArray[0] as string;
                    var cultureInfo = Thread.CurrentThread.CurrentCulture;
                    ThreadPool.QueueUserWorkItem((object state) => {
                        if (done < count) {
                            try {
                                Thread.CurrentThread.CurrentCulture = cultureInfo;
                                if (_filterDatabasesWorkItem == null) _filterDatabasesWorkItem = new FilterDatabasesWorkItem();

                                var das = new DatabaseActions() { ConnectionString = databaseActions.ConnectionString };
                                var arr = _filterDatabasesWorkItem.FilterDatabase(das, state as string, filter);
                                lock (_lock) {
                                    if (arr != null) _dataSource.Rows.Add(arr);
                                    ++done;
                                }
                                das.ReleaseConnection();
                                if (done == count) _waitHandle.Set();
                            } catch {
                                try {
                                    lock (_lock) done = int.MaxValue;
                                    _waitHandle.Set();
                                } catch { }
                            }
                        }
                    }, database);
                }

                if (count != 0) _waitHandle.WaitOne();
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
            if (_dataSource != null && e.RowIndex != -1 && e.RowIndex < _dataSource.Rows.Count && _dataSource.Rows[e.RowIndex] != _currentRow) {
                DataRow row = _dataSource.Rows[e.RowIndex];
                _currentRow = row;
                string databaseName = row[3] as string;

                string user, host, password;
                int port;
                _mySQLServerDialog.GetCurrentCredentials(out user, out host, out port, out password);
                _resultsHelper.ConnectToExistingDatabase(host, port, databaseName, user, password);

                //Fill the stresstest cbo
                cboStresstest.SelectedIndexChanged -= cboStresstest_SelectedIndexChanged;
                cboStresstest.Items.Clear();
                var stresstests = _resultsHelper.GetStresstests();
                if (stresstests.Rows.Count == 0) {
                    if (ResultsSelected != null) ResultsSelected(this, new ResultsSelectedEventArgs(null, 0));
                } else {
                    cboStresstest.Items.Add("<All>");
                    foreach (DataRow stresstestRow in stresstests.Rows)
                        cboStresstest.Items.Add((string)stresstestRow.ItemArray[1] + " " + stresstestRow.ItemArray[2]);

                    cboStresstest.SelectedIndex = 1;

                    if (ResultsSelected != null) ResultsSelected(this, new ResultsSelectedEventArgs(databaseName, (ulong)cboStresstest.SelectedIndex));
                }

                cboStresstest.SelectedIndexChanged += cboStresstest_SelectedIndexChanged;
            }
        }
        private void cboStresstest_SelectedIndexChanged(object sender, EventArgs e) {
            if (cboStresstest.SelectedIndex > -1 && ResultsSelected != null) ResultsSelected(this, new ResultsSelectedEventArgs(_currentRow[3] as string, (ulong)cboStresstest.SelectedIndex));
        }

        private class FilterDatabasesWorkItem {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="databaseActions"></param>
            /// <param name="database"></param>
            /// <param name="filter"></param>
            /// <returns>Contents of a data row.</returns>
            public object[] FilterDatabase(DatabaseActions databaseActions, string database, string[] filter) {
                try {
                    object[] itemArray = new object[4];

                    //Get the tags
                    var tags = databaseActions.GetDataTable("Select Tag From " + database + ".Tags");
                    string t = string.Empty;
                    if (tags.Rows.Count != 0) {
                        int countMinusOne = tags.Rows.Count - 1;
                        if (tags.Rows.Count > countMinusOne)
                            for (int i = 0; i != countMinusOne; i++) t += tags.Rows[i].ItemArray[0] + ", ";
                        t += tags.Rows[countMinusOne].ItemArray[0];
                    }
                    itemArray[1] = t.Trim();

                    //Get the description
                    var description = databaseActions.GetDataTable("Select Description From " + database + ".Description");
                    foreach (DataRow dr in description.Rows) {
                        itemArray[2] = dr.ItemArray[0];
                        break;
                    }
                    bool canAdd = true;
                    if (filter.Length != 0) {
                        //First filter on tags.
                        string s = itemArray[1] as string;
                        for (int i = 0; i != filter.Length; i++) {
                            string p = "\\b" + Regex.Escape(filter[i]) + "\\b";
                            if (!Regex.IsMatch(s, p, RegexOptions.IgnoreCase)) {
                                canAdd = false;
                                break;
                            }
                        }
                        //If that fails filter on description.
                        if (!canAdd) {
                            s = itemArray[2] as string;
                            for (int i = 0; i != filter.Length; i++) {
                                string p = Regex.Escape(filter[i]);
                                if (Regex.IsMatch(s, p, RegexOptions.IgnoreCase)) {
                                    canAdd = true;
                                    break;
                                }
                            }
                        }
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
            public ulong StresstestId { get; private set; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="database"></param>
            /// <param name="stresstestId">0 for all</param>
            public ResultsSelectedEventArgs(string database, ulong stresstestId) {
                Database = database;
                StresstestId = stresstestId;
            }
        }
    }
}
