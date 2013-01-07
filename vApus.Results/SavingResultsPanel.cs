/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using vApus.Results.Properties;

namespace vApus.Results
{
    public partial class SavingResultsPanel : Panel
    {
        public SavingResultsPanel()
        {
            InitializeComponent();
            if (IsHandleCreated)
                SetGui();
            else
                HandleCreated += SavingResultsPanel_HandleCreated;
        }

        public bool Connected
        {
            get { return Settings.Default.ConnectionStringIndex > -1 && Settings.Default.ConnectionStringIndex < SettingsManager.GetConnectionStrings().Count; }
        }

        #region Functions

        private void SavingResultsPanel_HandleCreated(object sender, EventArgs e)
        {
            SetGui();
        }

        private void SetGui()
        {
            cboConnectionString.Items.Clear();
            foreach (string connectionString in SettingsManager.GetConnectionStrings())
                cboConnectionString.Items.Add(connectionString);

            cboConnectionString.Items.Add("<New>");
            cboConnectionString.SelectedIndex = Settings.Default.ConnectionStringIndex;
        }

        private void cboConnectionStrings_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            if (cboConnectionString.SelectedIndex == cboConnectionString.Items.Count - 1)
            {
                txtUser.Text = txtHost.Text = txtPassword.Text = string.Empty;
                nudPort.Value = 3306;

                txtUser.InvokeOnLeave();
                txtHost.InvokeOnLeave();
                txtPassword.InvokeOnLeave();

                btnTest.Enabled = btnDelete.Enabled = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;

                string user, host, password;
                int port;
                SettingsManager.GetCredentials(cboConnectionString.SelectedIndex, out user, out host, out port,
                                               out password);

                txtUser.Text = user;
                txtHost.Text = host;
                nudPort.Value = port;
                txtPassword.Text = password;

                btnTest.Enabled = btnDelete.Enabled = true;
            }
        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            if (txtPassword.Text.Length == 0)
                txtPassword.UseSystemPasswordChar = true;
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            if (txtPassword.ForeColor == Color.DimGray)
                txtPassword.UseSystemPasswordChar = false;
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            btnTest.Enabled = btnSave.Enabled =
                              (txtUser.ForeColor != Color.DimGray && txtUser.Text.Trim().Length != 0) &&
                              (txtHost.ForeColor != Color.DimGray && txtHost.Text.Trim().Length != 0) &&
                              (txtPassword.ForeColor != Color.DimGray && txtPassword.Text.Trim().Length != 0);

            if (cboConnectionString.SelectedIndex != cboConnectionString.Items.Count - 1)
            {
                string user, host, password;
                int port;
                SettingsManager.GetCredentials(cboConnectionString.SelectedIndex, out user, out host, out port,
                                               out password);

                btnSave.Enabled = txtUser.Text != user ||
                                  txtHost.Text != host ||
                                  (int)nudPort.Value != port ||
                                  txtPassword.Text != password;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            bool succes = true;
            var dba = new DatabaseActions
                {
                    ConnectionString =
                        string.Format("Server={0};Port={1};DataBase=mysql;Uid={2};Pwd={3}", txtHost.Text,
                                      (int)nudPort.Value, txtUser.Text, txtPassword.Text)
                };
            try
            {
                DataTable datatable = dba.GetDataTable("Show Tables", CommandType.Text);
            }
            catch
            {
                succes = false;
            }
            dba.ReleaseConnection();
            dba = null;

            if (succes)
                MessageBox.Show("The connection has been established! and closed again successfully.", string.Empty,
                                MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            else
                MessageBox.Show("Could not connect to the database server", string.Empty, MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboConnectionString.SelectedIndex == cboConnectionString.Items.Count - 1)
                SettingsManager.AddCredentials(txtUser.Text, txtHost.Text, (int)nudPort.Value, txtPassword.Text);
            else
                SettingsManager.EditCredentials(cboConnectionString.SelectedIndex, txtUser.Text, txtHost.Text,
                                                (int)nudPort.Value, txtPassword.Text);
            SetGui();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            SettingsManager.DeleteCredentials(cboConnectionString.SelectedIndex);
            SetGui();
        }

        public override string ToString()
        {
            return "Saving Test Results";
        }

        #endregion
    }
}