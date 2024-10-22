﻿/*
 * 2015 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace vApus.Publish {
    public partial class PublishPanel : Panel {
        public bool Connected {
            get { return Publisher.Settings.PublisherEnabled && Publisher.Poll(); }
        }

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
            btnLaunchvApusPublishItemsHandler.Checked = Publisher.Settings.AutoLaunchvApusPublishItemsHandler;

            EnableDisable(Publisher.Settings.PublisherEnabled);
        }
        private void SaveSettings() {
            Publisher.Settings.TcpHost = txtTcpHost.Text.ToLowerInvariant().Trim();
            Publisher.Settings.TcpPort = (ushort)nudTcpPort.Value;
            Publisher.Settings.AutoLaunchvApusPublishItemsHandler = btnLaunchvApusPublishItemsHandler.Checked;

            Publisher.Settings.Save();
        }

        private void btnSet_Click(object sender, EventArgs e) {
            SaveSettings();
            if (AutoLaunchvApusPublishItemsHandler()) 
                Thread.Sleep(2000);

            if (Connected) {
                string host = Publisher.Settings.TcpHost;
                if ((host == "localhost" || host == "127.0.0.1" || host == "::1" || host == "0:0:0:0:0:0:0:1"))
                    MessageBox.Show("The endpoint server must be reachable from a remote location, otherwise distributed testing won't work!\nBe sure that '" + host + "' is what you want.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else {
                string warning = "Failed to connect to the given endpoint.";
                Loggers.Log(Level.Warning, warning, null, new object[] { sender, e });
            }
        }

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
            }
            catch (Exception ex) {
                string error = "Failed to open PublishItems.cs";
                Loggers.Log(Level.Error, error, ex, new object[] { sender, e });
                MessageBox.Show(error, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool AutoLaunchvApusPublishItemsHandler() {
            if (Publisher.Settings.PublisherEnabled && Publisher.Settings.AutoLaunchvApusPublishItemsHandler) {
                if (Process.GetProcessesByName("vApus.PublishItemsHandler").Length == 0) {
                    string path = Path.Combine(Application.StartupPath, "PublishItemsHandler\\vApus.PublishItemsHandler.exe");
                    if (File.Exists(path)) {
                        Process.Start(path, "autohide");
                        return true;
                    }
                }
            }
            return false;
        }

        public override string ToString() { return "Publish values"; }
    }
}
