using RandomUtils;
/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using vApus.Util.Properties;

namespace vApus.Util {
    /// <summary>
    /// Use in OptionDialog.
    /// </summary>
    public partial class UpdateNotifierPanel : Panel {
        private delegate void RefreshDel();

        #region Fields
        private readonly RefreshDel _refreshDel;
        private Win32WindowMessageHandler _msgHandler;
        #endregion

        #region Constructor
        /// <summary>
        /// Use in OptionDialog.
        /// </summary>
        public UpdateNotifierPanel() {
            InitializeComponent();
            _refreshDel = UpdateNotifier.Refresh;

            HandleCreated += UpdateNotifierPanel_HandleCreated;
        }
        #endregion

        #region Functions
        private void UpdateNotifierPanel_HandleCreated(object sender, EventArgs e) {
            _msgHandler = new Win32WindowMessageHandler();

            string host, username, password;
            int port, channel;
            bool smartUpdate;
            UpdateNotifier.GetCredentials(out host, out port, out username, out password, out channel, out smartUpdate);
            txtHost.Text = host;
            nudPort.Value = port;
            txtUsername.Text = username;
            txtPassword.Text = password;
            cboChannel.SelectedIndex = channel;
            chkSmartUpdate.Checked = smartUpdate;

            SetEnabled();
            SetUpdatePanel(true);
        }

        private void txtUsername_TextChanged(object sender, EventArgs e) {
            txtPassword.Enabled = txtUsername.Text.Length > 0;
            if (!txtPassword.Enabled)
                txtPassword.Text = string.Empty;
        }

        private void cboChannel_SelectedIndexChanged(object sender, EventArgs e) {
            string host, username, password;
            int port, channel;
            bool smartUpdate;
            UpdateNotifier.GetCredentials(out host, out port, out username, out password, out channel, out smartUpdate);

            if (cboChannel.SelectedIndex != channel)
                if (MessageBox.Show("Are you sure you want to change the channel?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    SetEnabled();
                else
                    cboChannel.SelectedIndex = channel;
        }

        private void _KeyUp(object sender, KeyEventArgs e) { SetEnabled(); }
        private void chkSmartUpdate_CheckedChanged(object sender, EventArgs e) { SetEnabled(); }

        private void SetEnabled() {
            if (txtHost.Text.Length != 0 &&
                txtUsername.Text.Length != 0 && txtPassword.Text.Length != 0) {
                string host, username, password;
                int port, channel;
                bool smartUpdate;
                UpdateNotifier.GetCredentials(out host, out port, out username, out password, out channel, out smartUpdate);

                pnlRefresh.Enabled = btnForceUpdate.Enabled = true;
                btnSet.Enabled = (txtHost.Text != host || nudPort.Value != port ||
                                  txtUsername.Text != username || txtPassword.Text != password ||
                                  cboChannel.SelectedIndex != channel || chkSmartUpdate.Checked != smartUpdate);
            } else {
                pnlRefresh.Enabled = btnForceUpdate.Enabled = false;
                btnSet.Enabled = false;
            }
            btnClear.Enabled = (txtHost.Text.Length != 0 ||
                                txtUsername.Text.Length != 0 || txtPassword.Text.Length != 0);
        }

        private void btnRefresh_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;

            btnSet.Enabled = false;

            pic.Image = Resources.Warning;
            lbl.Text = "Working...";

            UpdateNotifier.SetCredentials(txtHost.Text, (int)nudPort.Value, txtUsername.Text, txtPassword.Text,
                                          cboChannel.SelectedIndex, chkSmartUpdate.Checked);

            BackgroundWorkQueueWrapper.BackgroundWorkQueue.OnWorkItemProcessed += BackgroundWorkQueue_OnWorkItemProcessed;
            BackgroundWorkQueueWrapper.BackgroundWorkQueue.EnqueueWorkItem(_refreshDel);
        }

        private void BackgroundWorkQueue_OnWorkItemProcessed(object sender, BackgroundWorkQueue.OnWorkItemProcessedEventArgs e) {
            BackgroundWorkQueueWrapper.BackgroundWorkQueue.OnWorkItemProcessed -= BackgroundWorkQueue_OnWorkItemProcessed;

            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                SetUpdatePanel(true);
                Cursor = Cursors.Default;
            }, null);
        }

        private void SetUpdatePanel(bool doUpdate) {
            if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.Disabled ||
                UpdateNotifier.UpdateNotifierState == UpdateNotifierState.FailedConnectingToTheUpdateServer)
                pic.Image = Resources.Error;
            else if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.PleaseRefresh ||
                     UpdateNotifier.UpdateNotifierState == UpdateNotifierState.NewUpdateFound)
                pic.Image = Resources.Warning;
            else
                pic.Image = Resources.OK;

            var attr =
                typeof(UpdateNotifierState).GetField(UpdateNotifier.UpdateNotifierState.ToString())
                                            .GetCustomAttributes(typeof(DescriptionAttribute), false) as
                DescriptionAttribute[];
            lbl.Text = attr[0].Description;

            if (doUpdate)
                if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.NewUpdateFound &&
                    UpdateNotifier.GetUpdateNotifierDialog().ShowDialog() == DialogResult.OK)
                    ShowUpdateDialog(false);
        }

        private void ShowUpdateDialog(bool forceUpdate) {
            Cursor = Cursors.WaitCursor;
            string path = Path.Combine(Application.StartupPath, "vApus.UpdateToolLoader.exe");
            if (File.Exists(path)) {
                var process = new Process();
                process.EnableRaisingEvents = true;
                var port = (int)nudPort.Value;
                process.StartInfo = new ProcessStartInfo(path, "{A84E447C-3734-4afd-B383-149A7CC68A32} " + txtHost.Text + " " + port + " " +
                    txtUsername.Text + " " + txtPassword.Text + " " + cboChannel.SelectedIndex + " " + forceUpdate + " " + false);

                Enabled = false;

                process.Exited += updateProcess_Exited;
                process.Start();
            } else {
                MessageBox.Show("vApus could not be updated because the update tool was not found!", string.Empty,
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            Cursor = Cursors.Default;
        }

        private void updateProcess_Exited(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                Enabled = true;
                _msgHandler.PostMessage();

                UpdateNotifier.Refresh();

                SetUpdatePanel(false);
            }, null);
        }

        private void btnUpdateManually_Click(object sender, EventArgs e) {
            if (btnSet.Enabled) {
                btnSet.Enabled = false;
                UpdateNotifier.SetCredentials(txtHost.Text, (int)nudPort.Value, txtUsername.Text, txtPassword.Text,
                                              cboChannel.SelectedIndex, chkSmartUpdate.Checked);
            }
            ShowUpdateDialog(true);
        }

        private void btnSet_Click(object sender, EventArgs e) {
            btnSet.Enabled = false;
            UpdateNotifier.SetCredentials(txtHost.Text, (int)nudPort.Value, txtUsername.Text, txtPassword.Text,
                                          cboChannel.SelectedIndex, chkSmartUpdate.Checked);
        }

        private void btnClear_Click(object sender, EventArgs e) {
            txtHost.Text = string.Empty;
            nudPort.Value = 22; //External port 5222
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;

            UpdateNotifier.SetCredentials(txtHost.Text, (int)nudPort.Value, txtUsername.Text, txtPassword.Text,
                                          cboChannel.SelectedIndex, chkSmartUpdate.Checked);

            pnlRefresh.Enabled = btnForceUpdate.Enabled = false;
            btnSet.Enabled = false;
            btnClear.Enabled = false;
        }

        public override string ToString() {
            return "Update Notifier";
        }
        #endregion
    }
}