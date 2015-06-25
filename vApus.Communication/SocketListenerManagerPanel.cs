/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Communication {
    /// <summary>
    /// Sets the socket listener options. Used in OptionDialog.
    /// </summary>
    public partial class SocketListenerManagerPanel : Panel {

        #region Fields
        private readonly SocketListener _socketListener;
        #endregion

        #region Constructor
        public SocketListenerManagerPanel() {
            _socketListener = SocketListener.GetInstance();

            InitializeComponent();
            if (IsHandleCreated)
                Init();
            else
                HandleCreated += SocketListenerManager_HandleCreated;
        }
        #endregion

        #region Functions
        private void Init() {
            CheckRunning();
            btnSet.Enabled = false;
        }
        private void SocketListenerManager_HandleCreated(object sender, EventArgs e) { Init(); }

        //Copy the ip to the clipboard.
        private void btnCopyHost_Click(object sender, EventArgs e) { ClipboardWrapper.SetDataObject(txtHost.Text); }

        private void llblIPAddresses_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) { Process.Start("cmd", "/k ipconfig"); }

        private void nudPort_ValueChanged(object sender, EventArgs e) {
            btnSet.Enabled = true;
            chkPreferred.Checked = _socketListener.CheckAgainstPreferred((int)nudPort.Value);
        }

        private void chkPreferred_CheckedChanged(object sender, EventArgs e) {
            if ((int)nudPort.Value == _socketListener.Port)
                chkPreferred.Enabled = _socketListener.Port != _socketListener.PreferredPort;
            else
                chkPreferred.Enabled = true;
            if (chkPreferred.Enabled)
                btnSet.Enabled = true;
        }

        private void btnSet_Click(object sender, EventArgs e) {
            StatusStopped();
            btnStart.Enabled = false;
            try {
                _socketListener.SetPort((int)nudPort.Value, chkPreferred.Checked);
            } catch {
                MessageBox.Show(
                    string.Format("The socket listening could not be bound to port {0}!", nudPort.Value), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
                _socketListener.Start();
            }
            CheckRunning();
            chkPreferred.Checked = _socketListener.CheckAgainstPreferred((int)nudPort.Value);
            btnSet.Enabled = false;
        }

        private void btnStart_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            try {
                _socketListener.Start();
                CheckRunning();
            } catch (Exception ex) {
                StatusStopped();
                MessageBox.Show(ex.Message);
            }
            Cursor = Cursors.Default;
        }

        private void btnRestart_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            try {
                StatusStopped();
                btnStart.Enabled = false;
                _socketListener.Restart();
                CheckRunning();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            Cursor = Cursors.Default;
        }

        private void btnStop_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            StatusStopped();
            _socketListener.Stop();
            Cursor = Cursors.Default;
        }

        private void CheckRunning() {
            chkPreferred.Text = "Preferred (now -> " + _socketListener.PreferredPort + ")";
            if (_socketListener.IsRunning) {
                Cursor = Cursors.WaitCursor;
                lblStatus.Text = "Status: started";

                txtHost.Text = Dns.GetHostName();

                nudPort.Value = _socketListener.Port;
                nudPort.Enabled = true;

                btnCopyHost.Enabled = true;
                btnStart.Enabled = false;
                btnRestart.Enabled = true;
                btnStop.Enabled = true;
                btnSet.Enabled = false;

                chkPreferred.Enabled = _socketListener.Port != _socketListener.PreferredPort;
                Cursor = Cursors.Default;
            } else {
                StatusStopped();
            }
        }

        private void StatusStopped() {
            Cursor = Cursors.WaitCursor;
            lblStatus.Text = "Status: stopped";

            txtHost.Text = string.Empty;

            btnCopyHost.Enabled = false;
            nudPort.Enabled = false;
            btnStart.Enabled = true;
            btnRestart.Enabled = false;
            btnStop.Enabled = false;
            btnSet.Enabled = false;
            chkPreferred.Enabled = false;
            Cursor = Cursors.Default;
        }

        public override string ToString() {
            return "Socket listener manager";
        }
        #endregion
    }
}