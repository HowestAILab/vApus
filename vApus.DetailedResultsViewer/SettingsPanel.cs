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

        public SettingsPanel() {
            InitializeComponent();
            RefreshDatabases(true);
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

            MessageBox.Show("Could not connect to the database server.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;

        }

        private void FillDatabasesDataGridView(DatabaseActions databaseActions) {
            Cursor = Cursors.WaitCursor;
            try {
                dgvDatabases.DataSource = null;
                string[] filter = filterResults.Filter;
                var dataSource = new DataTable("dataSource");
                dataSource.Columns.Add(" ");
                dataSource.Columns.Add("Tags");
                dataSource.Columns.Add("Description");

                var dbs = databaseActions.GetDataTable("Show Databases Like 'vapus%';");
                int count = dbs.Rows.Count;
                int done = 0;
                foreach (DataRow dbsr in dbs.Rows) {
                    string database = dbsr.ItemArray[0] as string;
                    ThreadPool.QueueUserWorkItem((object state) => {
                        if (done < count) {
                            try {
                                if (_filterDatabasesWorkItem == null) _filterDatabasesWorkItem = new FilterDatabasesWorkItem();

                                var row = _filterDatabasesWorkItem.FilterDatabase(new DatabaseActions() { ConnectionString = databaseActions.ConnectionString }, state as string, filter);
                                lock (_lock) {
                                    if (row != null) dataSource.Rows.Add(row);
                                    ++done;
                                }
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
                dgvDatabases.DataSource = dataSource;
                if (dgvDatabases.Rows.Count == 0) {
                    if (ResultsSelected != null) ResultsSelected(this, new ResultsSelectedEventArgs(null));
                } else {
                    dgvDatabases.Sort(dgvDatabases.Columns[0], System.ComponentModel.ListSortDirection.Descending);
                    dgvDatabases.Rows[0].Selected = true;
                }
            } catch { }
            try { if (!Disposing && !IsDisposed) Cursor = Cursors.Arrow; } catch { }
        }

        private void dgvDatabases_CellEnter(object sender, DataGridViewCellEventArgs e) {
            DataTable dt = dgvDatabases.DataSource as DataTable;
            if (dt != null && e.RowIndex != -1 && e.RowIndex < dt.Rows.Count) {
                DataRow row = dt.Rows[e.RowIndex];
                string databaseName = row.GetTag() as string;

                string user, host, password;
                int port;
                _mySQLServerDialog.GetCurrentCredentials(out user, out host, out port, out password);
                ResultsHelper.ConnectToDatabase(host, port, databaseName, user, password);
                if (ResultsSelected != null) ResultsSelected(this, new ResultsSelectedEventArgs(databaseName));
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
            public object[] FilterDatabase(DatabaseActions databaseActions, string database, string[] filter) {
                try {
                    object[] itemArray = new object[3];

                    //Get the DateTime, to be formatted later.
                    string[] dtParts = database.Substring(5).Split('_');//vApusMM_dd_yyyy_HH_mm_ss_fffffff
                    int month = int.Parse(dtParts[0]);
                    int day = int.Parse(dtParts[1]);
                    int year = int.Parse(dtParts[2]);
                    int hour = int.Parse(dtParts[3]);
                    int minute = int.Parse(dtParts[4]);
                    int second = int.Parse(dtParts[5]);
                    DateTime dt = new DateTime(year, month, day, hour, minute, second);
                    itemArray[0] = dt.ToString();

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
                        itemArray.SetTag(database);
                        return itemArray;
                    }
                } catch {
                }
                return null;
            }
        }

        public class ResultsSelectedEventArgs : EventArgs {
            public string Database { get; private set; }
            public ResultsSelectedEventArgs(string database) {
                Database = database;
            }
        }
    }
}
