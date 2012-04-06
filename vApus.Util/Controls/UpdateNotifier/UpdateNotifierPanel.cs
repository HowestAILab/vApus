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

namespace vApus.Util
{
    public partial class UpdateNotifierPanel : Panel
    {
        private Win32WindowMessageHandler _msgHandler;
        private delegate void RefreshDel();
        private RefreshDel _refreshDel;

        public UpdateNotifierPanel()
        {
            InitializeComponent();
            _refreshDel = new RefreshDel(UpdateNotifier.Refresh);

            this.HandleCreated += new EventHandler(UpdateNotifierPanel_HandleCreated);
        }
        private void UpdateNotifierPanel_HandleCreated(object sender, EventArgs e)
        {
            _msgHandler = new Win32WindowMessageHandler();

            string host, username, password;
            int port, channel;
            UpdateNotifier.GetCredentials(out host, out port, out username, out password, out channel);
            txtHost.Text = host;
            nudPort.Value = port;
            txtUsername.Text = username;
            txtPassword.Text = password;
            cboChannel.SelectedIndex = channel;

            SetEnabled();
            SetUpdatePanel();
        }
        public override string ToString()
        {
            return "Update Notifier";
        }
        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            txtPassword.Enabled = txtUsername.Text.Length > 0;
            if (!txtPassword.Enabled)
                txtPassword.Text = string.Empty;
        }
        private void cboChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            string host, username, password;
            int port, channel;
            UpdateNotifier.GetCredentials(out host, out port, out username, out password, out channel);

            if (cboChannel.SelectedIndex != channel)
                if (MessageBox.Show("Are you sure you want to change the channel?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    SetEnabled();
                else 
                    cboChannel.SelectedIndex = channel;
        }
        private void _KeyUp(object sender, KeyEventArgs e)
        {
            SetEnabled();
        }
        private void SetEnabled()
        {
            if (txtHost.Text.Length != 0 &&
            txtUsername.Text.Length != 0 && txtPassword.Text.Length != 0)
            {
                string host, username, password;
                int port, channel;
                UpdateNotifier.GetCredentials(out host, out port, out username, out password, out channel);

                pnlRefresh.Enabled = true;
                btnSet.Enabled = (txtHost.Text != host || nudPort.Value != port ||
                txtUsername.Text != username || txtPassword.Text != password ||
                cboChannel.SelectedIndex != channel);
            }
            else
            {
                pnlRefresh.Enabled = false;
                btnSet.Enabled = false;
            }
            btnClear.Enabled = (txtHost.Text.Length != 0 ||
                txtUsername.Text.Length != 0 || txtPassword.Text.Length != 0);
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            btnSet.Enabled = false;

            pic.Image = vApus.Util.Properties.Resources.Warning;
            lbl.Text = "Working...";

            UpdateNotifier.SetCredentials(txtHost.Text, (int)nudPort.Value, txtUsername.Text, txtPassword.Text, cboChannel.SelectedIndex);

            StaticActiveObjectWrapper.ActiveObject.OnResult += new EventHandler<ActiveObject.OnResultEventArgs>(ActiveObject_OnResult);
            StaticActiveObjectWrapper.ActiveObject.Send(_refreshDel);
        }
        private void ActiveObject_OnResult(object sender, ActiveObject.OnResultEventArgs e)
        {
            StaticActiveObjectWrapper.ActiveObject.OnResult -= ActiveObject_OnResult;

            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                SetUpdatePanel();
                this.Cursor = Cursors.Default;
            });
        }

        private void SetUpdatePanel(bool enforceUpdate = true)
        {
            if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.Disabled ||
                            UpdateNotifier.UpdateNotifierState == UpdateNotifierState.FailedConnectingToTheUpdateServer)
                pic.Image = vApus.Util.Properties.Resources.Error;
            else if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.PleaseRefresh ||
                UpdateNotifier.UpdateNotifierState == UpdateNotifierState.NewUpdateFound)
                pic.Image = vApus.Util.Properties.Resources.Warning;
            else
                pic.Image = vApus.Util.Properties.Resources.OK;

            DescriptionAttribute[] attr = typeof(UpdateNotifierState).GetField(UpdateNotifier.UpdateNotifierState.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            lbl.Text = attr[0].Description;

            if (enforceUpdate)
                if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.NewUpdateFound &&
                    MessageBox.Show("New update found!\nDo you want to update?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    ShowUpdateDialog(enforceUpdate);
        }
        private void ShowUpdateDialog(bool autoUpdate)
        {
            this.Cursor = Cursors.WaitCursor;
            string path = Path.Combine(Application.StartupPath, "vApus.UpdateToolLoader.exe");
            if (File.Exists(path))
            {
                Process process = new Process();
                process.EnableRaisingEvents = true;
                int port = (int)nudPort.Value;
                if (pnlRefresh.Enabled)
                    process.StartInfo = new ProcessStartInfo(path, "{A84E447C-3734-4afd-B383-149A7CC68A32} " + txtHost.Text + " " +
                        port + " " + txtUsername.Text + " " + txtPassword.Text + " " + cboChannel.SelectedIndex + " " + autoUpdate);
                else
                    process.StartInfo = new ProcessStartInfo(path, "{A84E447C-3734-4afd-B383-149A7CC68A32} " + cboChannel.SelectedIndex);

                this.Enabled = false;

                process.Exited += new EventHandler(updateProcess_Exited);
                process.Start();
            }
            else
            {
                MessageBox.Show("vApus could not be updated because the update tool was not found!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            this.Cursor = Cursors.Default;
        }
        private void updateProcess_Exited(object sender, EventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                this.Enabled = true;
                _msgHandler.PostMessage();

                UpdateNotifier.Refresh();

                SetUpdatePanel(false);
            });
        }
        private void btnUpdateManually_Click(object sender, EventArgs e)
        {
            if (btnSet.Enabled)
            {
                btnSet.Enabled = false;
                UpdateNotifier.SetCredentials(txtHost.Text, (int)nudPort.Value, txtUsername.Text, txtPassword.Text, cboChannel.SelectedIndex);
            }
            ShowUpdateDialog(false);
        }
        private void btnSet_Click(object sender, EventArgs e)
        {
            btnSet.Enabled = false;
            UpdateNotifier.SetCredentials(txtHost.Text, (int)nudPort.Value, txtUsername.Text, txtPassword.Text, cboChannel.SelectedIndex);
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtHost.Text = string.Empty;
            nudPort.Value = 5222;
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;

            UpdateNotifier.SetCredentials(txtHost.Text, 5222, txtUsername.Text, txtPassword.Text, cboChannel.SelectedIndex);

            pnlRefresh.Enabled = false;
            btnSet.Enabled = false;
            btnClear.Enabled = false;
        }
    }
}
