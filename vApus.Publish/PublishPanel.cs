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
            chkTestsConfig.Checked = Publisher.Settings.PublishTestsConfiguration;
            chkTestsFastConcurrencyResults.Checked = Publisher.Settings.PublishTestsFastConcurrencyResults;
            chkTestsFastRunResults.Checked = Publisher.Settings.PublishTestsFastRunResults;
            chkTestsClientMonitoring.Checked = Publisher.Settings.PublishTestsClientMonitoring;
            chkTestsMessages.Checked = Publisher.Settings.PublishTestsMessages;
            cboMessageLevel.SelectedIndex = (int)Publisher.Settings.MessageLevel;

            chkMonitorsConfig.Checked = Publisher.Settings.PublishMonitorsConfiguration;
            chkMonitorsHWConfig.Checked = Publisher.Settings.PublishMonitorsHardwareConfiguration;
            chkMonitorsMetrics.Checked = Publisher.Settings.PublishMonitorsMetrics;

            chkApplicationLogs.Checked = Publisher.Settings.PublishApplicationLogs;
            cboLogLevel.SelectedIndex = (int)Publisher.Settings.LogLevel;

            chkTcp.Checked = Publisher.Settings.TcpOutput;
            txtTcpHost.Text = Publisher.Settings.TcpHost;
            nudTcpPort.Value = Publisher.Settings.TcpPort;

            chkUdpBroadcast.Checked = Publisher.Settings.UdpBroadcastOutput;
            nudBroadcastPort.Value = Publisher.Settings.UdpBroadcastPort;

            EnableDisable(Publisher.Settings.PublisherEnabled);
        }
        private void SaveSettings() {
            Publisher.Settings.PublishTestsConfiguration = chkTestsConfig.Checked;
            Publisher.Settings.PublishTestsFastConcurrencyResults = chkTestsFastConcurrencyResults.Checked;
            Publisher.Settings.PublishTestsFastRunResults = chkTestsFastRunResults.Checked;
            Publisher.Settings.PublishTestsClientMonitoring = chkTestsClientMonitoring.Checked;
            Publisher.Settings.PublishTestsMessages = chkTestsMessages.Checked;
            Publisher.Settings.MessageLevel = (ushort)cboMessageLevel.SelectedIndex;

            Publisher.Settings.PublishMonitorsConfiguration = chkMonitorsConfig.Checked;
            Publisher.Settings.PublishMonitorsHardwareConfiguration = chkMonitorsHWConfig.Checked;
            Publisher.Settings.PublishMonitorsMetrics = chkMonitorsMetrics.Checked;

            Publisher.Settings.PublishApplicationLogs = chkApplicationLogs.Checked;
            Publisher.Settings.LogLevel = (ushort)cboLogLevel.SelectedIndex;
            
            Publisher.Settings.TcpOutput = chkTcp.Checked;
            Publisher.Settings.TcpHost = txtTcpHost.Text;
            Publisher.Settings.TcpPort = (ushort)nudTcpPort.Value;

            Publisher.Settings.UdpBroadcastOutput = chkUdpBroadcast.Checked;
            Publisher.Settings.UdpBroadcastPort = (ushort)nudBroadcastPort.Value;

            Publisher.Settings.Save();
            Publisher.Clear();
        }

        private void btnSet_Click(object sender, EventArgs e) { SaveSettings(); }

        public override string ToString() { return "Publish values"; }

        private void btnEnable_Click(object sender, EventArgs e) { EnableDisable(btnEnable.Text == "Enable"); }

        private void EnableDisable(bool enable) {
            btnEnable.Text = enable ? "Disable" : "Enable";

            Publisher.Settings.PublisherEnabled = grp.Enabled = btnSet.Enabled = enable;

            Publisher.Settings.Save();
            Publisher.Clear();

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
