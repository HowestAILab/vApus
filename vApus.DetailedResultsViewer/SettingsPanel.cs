/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using vApus.Results;
using WeifenLuo.WinFormsUI.Docking;
using vApus.Util;
using System.Text.RegularExpressions;

namespace vApus.DetailedResultsViewer {
    public partial class SettingsPanel : DockablePanel {

        public event EventHandler<ResultsSelectedEventArgs> ResultsSelected;

        private MySQLServerDialog _mySQLServerDialog = new MySQLServerDialog();

        public SettingsPanel() {
            InitializeComponent();
            RefreshDatabases(true);
        }

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
            DatabaseActions databaseActions = null;
            if (_mySQLServerDialog.ConnectionFilledIn) {
                databaseActions = new DatabaseActions() { ConnectionString = _mySQLServerDialog.ConnectionString };
                try {
                    var dbs = databaseActions.GetDataTable("Show Databases;");
                    if (dbs.Columns.Count == 0) {
                        MessageBox.Show("Could not connect to the database server.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                } catch { }

                lblConnectToMySQL.Text = "Results Server Connected!";
                toolTip.SetToolTip(lblConnectToMySQL, "Click to choose another MySQL server.");
            }
            return databaseActions;
        }
        private void FillDatabasesDataGridView(DatabaseActions databaseActions) {
            dgvDatabases.DataSource = null;
            string[] filter = filterResults.Filter;
            var dataSource = new DataTable("dataSource");
            dataSource.Columns.Add(" ");
            dataSource.Columns.Add("Tags");
            dataSource.Columns.Add("Description");

            var dbs = databaseActions.GetDataTable("Show Databases Like 'vapus%';");
            foreach (DataRow dbsr in dbs.Rows) {
                string db = dbsr.ItemArray[0] as string;

                object[] itemArray = new object[3];

                //Get the DateTime, to be formatted later.
                string[] dtParts = db.Substring(5).Split('_');//vApusMM_dd_yyyy_HH_mm_ss_fffffff
                int month = int.Parse(dtParts[0]);
                int day = int.Parse(dtParts[1]);
                int year = int.Parse(dtParts[2]);
                int hour = int.Parse(dtParts[3]);
                int minute = int.Parse(dtParts[4]);
                int second = int.Parse(dtParts[5]);
                DateTime dt = new DateTime(year, month, day, hour, minute, second);
                itemArray[0] = dt.ToString();

                //Get the tags
                var tags = databaseActions.GetDataTable("Select Tag From " + db + ".Tags");
                string t = string.Empty;
                int countMinusOne = tags.Rows.Count - 1;
                if (tags.Rows.Count > countMinusOne)
                    for (int i = 0; i != countMinusOne; i++) t += tags.Rows[i].ItemArray[0] + ", ";
                t += tags.Rows[countMinusOne].ItemArray[0];
                itemArray[1] = t.Trim();

                //Get the description
                var description = databaseActions.GetDataTable("Select Description From " + db + ".Description");
                foreach (DataRow dr in description.Rows) {
                    itemArray[2] = dr.ItemArray[0];
                    break;
                }
                bool canAdd = true;
                if (filter.Length != 0) {
                    //First filter on tags.
                    string s = itemArray[1] as string;
                    foreach (string part in filter) {
                        string p = "\\b" + Regex.Escape(part) + "\\b";
                        if (!Regex.IsMatch(s, p, RegexOptions.IgnoreCase)) {
                            canAdd = false;
                            break;
                        }
                    }
                    //If that fails filter on description.
                    if (!canAdd) {
                        s = itemArray[2] as string;
                        foreach (string part in filter) {
                            string p = Regex.Escape(part);
                            if (Regex.IsMatch(s, p, RegexOptions.IgnoreCase)) {
                                canAdd = true;
                                break;
                            }
                        }
                    }
                }
                if (canAdd) {
                    DataRow dataSourceRow = dataSource.Rows.Add(itemArray);
                    dataSourceRow.SetTag(db);
                }
            }
            dgvDatabases.DataSource = dataSource;
            if (dgvDatabases.Rows.Count == 0)
                if (ResultsSelected != null) ResultsSelected(this, new ResultsSelectedEventArgs(null));
                else
                    dgvDatabases.Rows[0].Selected = true;
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

        public class ResultsSelectedEventArgs : EventArgs {
            public string Database { get; private set; }
            public ResultsSelectedEventArgs(string database) {
                Database = database;
            }
        }
    }
}
