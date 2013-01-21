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

namespace vApus.DetailedResultsViewer {
    public partial class SettingsPanel : DockablePanel {

        public event EventHandler DatabaseSelected;

        private MySQLServerDialog _mySQLServerDialog = new MySQLServerDialog();
        public SettingsPanel() {
            InitializeComponent();
            var databaseActions = SetServerConnectStateInGui();
            lvwDatabases.Items.Clear();
            if (databaseActions != null) {
                filterDatabases.SetAvailableTags(databaseActions);
                FillDatabasesListView(databaseActions, null);
            }
        }

        private void lblConnectToMySQL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _mySQLServerDialog.ShowDialog();
            var databaseActions = SetServerConnectStateInGui();
            lvwDatabases.Items.Clear();
            if (databaseActions != null) {
                filterDatabases.SetAvailableTags(databaseActions);
                FillDatabasesListView(databaseActions, null);
            }
        }
        private DatabaseActions SetServerConnectStateInGui() {
            lblConnectToMySQL.Text = "Connect to a MySQL server...";
            DatabaseActions databaseActions = null;
            if (_mySQLServerDialog.Connected) {
                databaseActions = new DatabaseActions() { ConnectionString = _mySQLServerDialog.ConnectionString };
                try {
                    var dbs = databaseActions.GetDataTable("Show Databases;");
                    if (dbs.Rows.Count == 0) return null;
                } catch { }

                lblConnectToMySQL.Text = "MySQL Server Connected!";
                filterDatabases.ClearAvailableTags();
            }
            return databaseActions;
        }

        private void filterDatabases_CollapsedOrExpandedTags(object sender, EventArgs e) {
            picDatabases.Top = lblDatabases.Top = filterDatabases.Bottom + 6;
            lvwDatabases.Top = picDatabases.Bottom + 3;
            lvwDatabases.Height = Height - lvwDatabases.Top - 6;
        }
        private void FillDatabasesListView(DatabaseActions databaseActions, string filter) {
            lvwDatabases.Items.Clear();
            var dbs = databaseActions.GetDataTable("Show Databases Like 'vapus%';");
            foreach (DataRow row in dbs.Rows) lvwDatabases.Items.Add(row.ItemArray[0] as string);
        }

        private void lvwDatabases_ItemActivate(object sender, EventArgs e) {
            string databaseName = (sender as ListViewItem).Text;

            string user, host, password;
            int port;
            _mySQLServerDialog.GetCurrentCredentials(out user, out host, out port, out password);
            ResultsHelper.ConnectToDatabase(host, port, databaseName, user, password);
            //detailedResultsControl.RefreshReport();
        }
        public class DatabaseSelectedEventArgs : EventArgs {
            //public readonly string Database { get; set; }
            public DatabaseSelectedEventArgs(string database) {
              //  Database = database;
            }
        }
    }
}
