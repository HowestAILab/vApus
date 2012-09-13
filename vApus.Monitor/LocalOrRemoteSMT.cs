/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Net;
using System.Windows.Forms;

namespace vApus.Monitor
{
    public partial class LocalOrRemoteSMT : Form
    {
        public string IP
        {
            get { return rdbLocal.Checked ? "127.0.0.1" : txtIP.Text; }
        }
        public LocalOrRemoteSMT()
        {
            InitializeComponent();

            this.HandleCreated += new EventHandler(LocalOrRemoteSMT_HandleCreated);
        }

        void LocalOrRemoteSMT_HandleCreated(object sender, EventArgs e)
        {
            HandleCreated -= LocalOrRemoteSMT_HandleCreated;
            rdbRemote.Checked = !global::vApus.Monitor.Properties.Settings.Default.rdbLocal;
        }
        private void txtIP_TextChanged(object sender, EventArgs e)
        {
            rdbRemote.Checked = true;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtIP.Text == "127.0.0.1")
                rdbLocal.Checked = true;

            IPAddress ipAddress;
            if (IPAddress.TryParse(IP, out ipAddress))
            {
                if (chkSaveSettings.Checked)
                    global::vApus.Monitor.Properties.Settings.Default.Save();

                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("You have not filled in a valid IP address.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}
