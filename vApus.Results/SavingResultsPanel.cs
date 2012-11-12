using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Results
{
    public partial class SavingResultsPanel : Panel
    {
        private string _passwordGUID = "{51E6A7AC-06C2-466F-B7E8-4B0A00F6A21F}";
        private byte[] _passwordSalt = { 0x49, 0x16, 0x49, 0x2e, 0x11, 0x1e, 0x45, 0x24, 0x86, 0x05, 0x01, 0x03, 0x62 };

        public SavingResultsPanel()
        {
            InitializeComponent();
            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += SavingResultsPanel_HandleCreated;
        }

        private void SavingResultsPanel_HandleCreated(object sender, EventArgs e)
        {
            SetGui();
        }
        private void SetGui()
        {
            cboConnectionString.Items.Clear();
            foreach (string connectionString in GetConnectionStrings())
                cboConnectionString.Items.Add(connectionString);

            cboConnectionString.Items.Add("<New>");
            cboConnectionString.SelectedIndex = vApus.Results.Properties.Settings.Default.ConnectionStringIndex;
        }
        private void txt_Enter(object sender, EventArgs e)
        {
            var txt = sender as TextBox;
            if (txt.ForeColor == Color.DimGray)
            {
                txt.Text = string.Empty;
                txt.ForeColor = Color.Black;
            }
        }
        private void txtUser_Leave(object sender, EventArgs e)
        {
            txtUser.Text = txtUser.Text.Trim();
            if (txtUser.Text.Length == 0)
            {
                txtUser.ForeColor = Color.DimGray;
                txtUser.Text = "User";
            }
        }
        private void txtHost_Leave(object sender, EventArgs e)
        {
            txtHost.Text = txtHost.Text.Trim();
            if (txtHost.Text.Length == 0)
            {
                txtHost.ForeColor = Color.DimGray;
                txtHost.Text = "Host";
            }
        }
        private void txtPassword_Enter(object sender, EventArgs e)
        {
            if (txtPassword.ForeColor == Color.DimGray)
            {
                txtPassword.Text = string.Empty;
                txtPassword.ForeColor = Color.Black;
                txtPassword.UseSystemPasswordChar = true;
            }
        }
        private void txtPassword_Leave(object sender, EventArgs e)
        {
            txtPassword.Text = txtPassword.Text.Trim();
            if (txtPassword.Text.Length == 0)
            {
                txtPassword.ForeColor = Color.DimGray;
                txtPassword.UseSystemPasswordChar = false;
                txtPassword.Text = "Password";
            }
        }

        #region Settings
        private StringCollection GetConnectionStrings()
        {
            var connectionStrings = vApus.Results.Properties.Settings.Default.ConnectionStrings;
            if (connectionStrings == null)
                connectionStrings = new StringCollection();

            return connectionStrings;
        }
        private StringCollection GetPasswords()
        {
            var passwords = vApus.Results.Properties.Settings.Default.Passwords;
            if (passwords == null)
                passwords = new StringCollection();

            return passwords;
        }
        private void AddCredentials(string user, string host, int port, string password)
        {
            EditCredentials(-1, user, host, port, password);
            SetGui();
        }
        private void EditCredentials(int connectionStringIndex, string user, string host, int port, string password)
        {
            string connectionString = user + "@" + host + ":" + port;
            password = password.Encrypt(_passwordGUID, _passwordSalt);

            var connectionStrings = GetConnectionStrings();
            var passwords = GetPasswords();

            if (connectionStringIndex == -1)
            {
                connectionStrings.Add(connectionString);
                passwords.Add(password);
                vApus.Results.Properties.Settings.Default.ConnectionStringIndex = connectionStrings.IndexOf(connectionString);
            }
            else
            {
                connectionStrings[connectionStringIndex] = connectionString;
                passwords[connectionStringIndex] = password;
            }

            vApus.Results.Properties.Settings.Default.ConnectionStrings = connectionStrings;
            vApus.Results.Properties.Settings.Default.Passwords = passwords;
        }
        private void GetCredentials(int connectionStringIndex, out string user, out string host, out int port, out string password)
        {
            string connectionString = GetConnectionStrings()[connectionStringIndex];
            user = connectionString.Split('@')[0];
            connectionString = connectionString.Substring(user.Length + 1);
            host = connectionString.Split(':')[0];
            port = int.Parse(connectionString.Substring(host.Length + 1));

            password = GetPasswords()[connectionStringIndex].Decrypt(_passwordGUID, _passwordSalt);
        }
        private void DeleteCredentials(int connectionStringIndex)
        {
            var connectionStrings = GetConnectionStrings();
            connectionStrings.RemoveAt(connectionStringIndex);
            vApus.Results.Properties.Settings.Default.ConnectionStrings = connectionStrings;

            var passwords = GetPasswords();
            passwords.RemoveAt(connectionStringIndex);
            vApus.Results.Properties.Settings.Default.Passwords = passwords;

            vApus.Results.Properties.Settings.Default.Save();
        }
        #endregion

        public override string ToString()
        {
            return "Saving Test Results";
        }

        private void cboConnectionStrings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboConnectionString.SelectedIndex == cboConnectionString.Items.Count - 1)
            {
                txtUser.Text = txtHost.Text = txtPassword.Text = string.Empty;
                nudPort.Value = 3306;

                txtUser_Leave(null, null);
                txtHost_Leave(null, null);
                txtPassword_Leave(null, null);
            }
            else
            {
                txtUser.ForeColor = txtHost.ForeColor = txtPassword.ForeColor = Color.Black;
                txtPassword.UseSystemPasswordChar = true;

                string user, host, password;
                int port;
                GetCredentials(cboConnectionString.SelectedIndex, out user, out host, out port, out password);

                txtUser.Text = user;
                txtHost.Text = host;
                nudPort.Value = port;
                txtPassword.Text = password;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            var dba = new DatabaseActions
            {
                ConnectionString =
                string.Format("Server={0};Port={1};DataBase=mysql;Uid={2};Pwd={3}", txtHost.Text, (int)nudPort.Value, txtUser.Text, txtPassword.Text)
            };
            try
            {
                var datatable = dba.GetDataTable("Show Tables", CommandType.Text);
            }
            catch
            {
                MessageBox.Show("Could not connect to the database server", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            dba.ReleaseConnection();
            dba = null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboConnectionString.SelectedIndex == cboConnectionString.Items.Count - 1)
                AddCredentials(txtUser.Text, txtHost.Text, (int)nudPort.Value, txtPassword.Text);
            else 
                EditCredentials(cboConnectionString.SelectedIndex, txtUser.Text, txtHost.Text, (int)nudPort.Value, txtPassword.Text);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteCredentials(cboConnectionString.SelectedIndex);
        }
    }
}
