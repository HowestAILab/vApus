/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using vApus.Results.Properties;
using vApus.Util;

namespace vApus.Results {
    /// <summary>
    /// Used in vApus.Util.OptionsDialog. Servers at managing connection strings.
    /// </summary>
    public partial class SavingResultsPanel : Panel {

        #region Fields
        private bool _showDescription = true, _showLocalHostWarning = true;
        #endregion

        #region Properties
        public bool Connected {
            get {

                string connectionString = ConnectionStringManager.GetCurrentConnectionString();
                if (connectionString != null) {
                    var databaseActions = new DatabaseActions() { ConnectionString = connectionString, CommandTimeout = 10 };
                    return databaseActions.CanConnect();
                }
                return false;
            }
        }
        /// <summary>
        /// A database name is not included in this connection string. Wil throw an exception if there is no current connection string or ConnectionStringManager.Enabled is set to false.
        /// </summary>
        public string ConnectionString {
            get {
                string connectionString = ConnectionStringManager.GetCurrentConnectionString();

                if (string.IsNullOrEmpty(connectionString)) throw new Exception("No MySQL connection was set.");

                return connectionString;
            }
        }
        #endregion

        #region Properties
        //Following can be disabled for different use cases.
        [DefaultValue(true)]
        public bool ShowDescription { get { return _showDescription; } set { _showDescription = value; } }
        [DefaultValue(true)]
        public bool ShowLocalHostWarning { get { return _showLocalHostWarning; } set { _showLocalHostWarning = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Used in vApus.Util.OptionsDialog. Servers at managing connection strings.
        /// </summary>
        public SavingResultsPanel() {
            InitializeComponent();
            if (IsHandleCreated) SetGui(); else HandleCreated += SavingResultsPanel_HandleCreated;
        }
        #endregion

        #region Functions

        private void SavingResultsPanel_HandleCreated(object sender, EventArgs e) {
            SetGui();
        }

        private void SetGui() {
            if (_showDescription) {
                lblDescription.Visible = true;
                grp.Top = 63;
            } else {
                lblDescription.Visible = false;
                grp.Top = 13;
            }
            grp.Enabled = true;
            grp.Height = btnTest.Top - grp.Top - 6;

            cboConnectionString.Items.Clear();
            foreach (string connectionString in ConnectionStringManager.GetFormattedConnectionStrings())
                cboConnectionString.Items.Add(connectionString);

            cboConnectionString.Items.Add("<New>");
            cboConnectionString.SelectedIndex = Settings.Default.ConnectionStringIndex;
            btnSave.Enabled = false;

            txt_TextChanged(txtHost, null);
            if (ConnectionStringManager.Enabled) {
                btnEnableDisable.Text = "Disable";
                btnDelete.Enabled = cboConnectionString.Items.Count != 1 && cboConnectionString.SelectedIndex != cboConnectionString.Items.Count - 1;
            } else {
                btnEnableDisable.Text = "Enable";

                grp.Enabled = btnTest.Enabled = btnSave.Enabled = btnDelete.Enabled = btnSave.Enabled = false;
            }
        }

        public void GetCurrentConnectionString(out string user, out string host, out int port, out string password) {
            ConnectionStringManager.GetCurrentConnectionString(out user, out host, out port, out password);
        }

        private void cboConnectionStrings_SelectedIndexChanged(object sender, EventArgs e) {
            if (cboConnectionString.SelectedIndex == cboConnectionString.Items.Count - 1) {
                btnSave.Enabled = false;
                txtUser.Text = txtHost.Text = txtPassword.Text = string.Empty;
                nudPort.Value = 3306;

                btnTest.Enabled = btnDelete.Enabled = false;
            } else {
                string user, host, password;
                int port;
                ConnectionStringManager.GetConnectionString(cboConnectionString.SelectedIndex, out user, out host, out port, out password, true);

                txtUser.Text = user;
                txtHost.Text = host;
                nudPort.Value = port;
                txtPassword.Text = password;

                btnSave.Enabled = btnTest.Enabled = btnDelete.Enabled = true;
            }
        }

        private void txt_TextChanged(object sender, EventArgs e) {
            string host = txtHost.Text.Trim().ToLower();
            if (cboConnectionString.SelectedIndex != cboConnectionString.Items.Count - 1) {
                string user, password;
                int port;
                ConnectionStringManager.GetConnectionString(cboConnectionString.SelectedIndex, out user, out host, out port, out password);

                btnSave.Enabled = txtUser.Text != user || txtHost.Text != host || (int)nudPort.Value != port || txtPassword.Text != password;
            }

            txtPassword.Enabled = txtUser.Text.Trim().Length != 0;
            if (!txtPassword.Enabled) txtPassword.Text = string.Empty;
            btnSave.Enabled = btnTest.Enabled = txtUser.Text.Trim().Length != 0 && txtHost.Text.Trim().Length != 0 && txtPassword.Text.Trim().Length != 0;
        }

        private void btnTest_Click(object sender, EventArgs e) {
            if (btnSave.Enabled) btnSave.PerformClick();

            var dba = new DatabaseActions {
                ConnectionString = string.Format("Server={0};Port={1};Uid={2};Pwd={3};", txtHost.Text, (int)nudPort.Value, txtUser.Text, txtPassword.Text)
            };
            bool succes = dba.CanConnect();
            dba = null;

            if (succes)
                MessageBox.Show("The connection has been established! and closed again successfully.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            else
                MessageBox.Show("Could not connect to the database server.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnSave_Click(object sender, EventArgs e) {
            if (cboConnectionString.SelectedIndex == cboConnectionString.Items.Count - 1)
                ConnectionStringManager.AddConnectionString(txtUser.Text, txtHost.Text, (int)nudPort.Value, txtPassword.Text);
            else
                ConnectionStringManager.EditConnectionString(cboConnectionString.SelectedIndex, txtUser.Text, txtHost.Text, (int)nudPort.Value, txtPassword.Text);
            SetGui();

            string host = txtHost.Text.Trim().ToLower();
            if (_showLocalHostWarning && (host == "localhost" || host == "127.0.0.1" || host == "::1" || host == "0:0:0:0:0:0:0:1"))
                MessageBox.Show("The results server must be reachable from a remote location, otherwise distributed testing won't work!\nBe sure that '" + txtHost.Text.Trim() + "' is what you want.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnDelete_Click(object sender, EventArgs e) {
            ConnectionStringManager.DeleteConnectionString(cboConnectionString.SelectedIndex);
            SetGui();
        }

        private void btnEnableDisable_Click(object sender, EventArgs e) {
            if (btnEnableDisable.Text == "Disable") {
                btnEnableDisable.Text = "Enable";
                grp.Enabled = btnTest.Enabled = btnSave.Enabled = btnDelete.Enabled = ConnectionStringManager.Enabled = false;
            } else {
                btnEnableDisable.Text = "Disable";
                grp.Enabled = ConnectionStringManager.Enabled = true;
                btnDelete.Enabled = cboConnectionString.Items.Count != 1 && cboConnectionString.SelectedIndex != cboConnectionString.Items.Count - 1;
                txt_TextChanged(txtHost, null);
            }
        }

        public override string ToString() {
            return "Saving test results";
        }

        #endregion
    }
}