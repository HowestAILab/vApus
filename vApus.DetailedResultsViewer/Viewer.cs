/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Results;

namespace vApus.DetailedResultsViewer {
    public partial class Viewer : Form {
        private MySQLServerDialog _mySQLServerDialog = new MySQLServerDialog();
        public Viewer() {
            InitializeComponent();
            SetDatabaseConnectStateInGui();
        }
        private void SetDatabaseConnectStateInGui() {
            mySQLServerToolStripMenuItem.Text = "MySQL Server Not Connected!";
            databaseToolStripMenuItem.Text = "No Database Selected";
            databaseToolStripMenuItem.Enabled = false;
            databaseToolStripMenuItem.DropDownItems.Clear();

            if (_mySQLServerDialog.Connected) {
                var databaseActions = new DatabaseActions() { ConnectionString = _mySQLServerDialog.ConnectionString };
                try {
                    var dbs = databaseActions.GetDataTable("Show Databases;");
                    if (dbs.Columns.Count == 0) return;

                    foreach (DataRow row in dbs.Rows) {
                        string dbName = row.ItemArray[0] as string;
                        if (dbName.StartsWith("vapus")) {
                            var item = databaseToolStripMenuItem.DropDownItems.Add(dbName);
                            item.Click += item_Click;
                        }
                    }
                    mySQLServerToolStripMenuItem.Text = "MySQL Server Connected!";
                    databaseToolStripMenuItem.Enabled = true;
                    if (databaseToolStripMenuItem.HasDropDownItems) databaseToolStripMenuItem.DropDownItems[0].PerformClick();
                } catch { }
            }
        }

        private void item_Click(object sender, EventArgs e) {
            string databaseName = (sender as ToolStripItem).Text;
            databaseToolStripMenuItem.Text = "Database: " + databaseName;

            string user, host, password;
            int port;
            _mySQLServerDialog.GetCurrentCredentials(out user, out host, out port, out password);
            ResultsHelper.ConnectToDatabase(host, port, databaseName, user, password);
            detailedResultsControl.RefreshReport();
        }

        private void mySQLServerToolStripMenuItem_Click(object sender, EventArgs e) {
            _mySQLServerDialog.ShowDialog();
            SetDatabaseConnectStateInGui();
        }
    }
}
