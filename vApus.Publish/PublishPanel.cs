/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace vApus.Publish {
    public partial class PublishPanel : Panel {
        public PublishPanel() {
            InitializeComponent();

            this.HandleCreated += PublicPanel_HandleCreated;
        }

        private void PublicPanel_HandleCreated(object sender, EventArgs e) {
            if (this.IsHandleCreated) {
                this.HandleCreated -= PublicPanel_HandleCreated;
                LoadSettings();
            }
        }

        private void LoadSettings() {
            txtTcpHost.Text = Publisher.Settings.TcpHost;
            nudTcpPort.Value = Publisher.Settings.TcpPort;

            EnableDisable(Publisher.Settings.PublisherEnabled);
        }
        private void SaveSettings() {
            Publisher.Settings.TcpHost = txtTcpHost.Text.ToLowerInvariant().Trim();
            Publisher.Settings.TcpPort = (ushort)nudTcpPort.Value;

            Publisher.Settings.Save();
        }

        private void btnSet_Click(object sender, EventArgs e) {
            SaveSettings();
            if (Publisher.Poll()) {
                string host = Publisher.Settings.TcpHost;
                if ((host == "localhost" || host == "127.0.0.1" || host == "::1" || host == "0:0:0:0:0:0:0:1"))
                    MessageBox.Show("The endpoint server must be reachable from a remote location, otherwise distributed testing won't work!\nBe sure that '" + host + "' is what you want.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } else {
                MessageBox.Show("Failed to connect to the given endpoint.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public override string ToString() { return "Publish values"; }

        private void btnEnable_Click(object sender, EventArgs e) { EnableDisable(btnEnable.Text == "Enable"); }

        private void EnableDisable(bool enable) {
            btnEnable.Text = enable ? "Disable" : "Enable";

            Publisher.Settings.PublisherEnabled = grp.Enabled = btnSet.Enabled = enable;

            Publisher.Settings.Save();
        }

        private void llblDeserialize_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            string path = Path.Combine(Application.StartupPath, "PublishItems.cs");
            if (!File.Exists(path)) {
                string error = "PublishItems.cs was not found in the root directory if vApus.";
                Loggers.Log(Level.Error, error, null, new object[] { sender, e });
                MessageBox.Show(error, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try {
                Process.Start(path);
            } catch (Exception ex) {
                string error = "Failed to open PublishItems.cs";
                Loggers.Log(Level.Error, error, ex, new object[] { sender, e });
                MessageBox.Show(error, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
