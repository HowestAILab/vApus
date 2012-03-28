/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public partial class SocketListenerManagerPanel : Panel
    {
        #region Fields
        SocketListener _socketListener;
        #endregion

        #region Constructor
        public SocketListenerManagerPanel()
        {
            _socketListener = SocketListener.GetInstance();

            InitializeComponent();
            if (this.IsHandleCreated)
                Init();
            else
                this.HandleCreated += new EventHandler(SocketListenerManager_HandleCreated);
        }
        #endregion

        #region Functions
        private void Init()
        {
            CheckRunning();
            _socketListener.IPChanged += new EventHandler<IPChangedEventArgs>(SocketListener_IPChanged);
            btnSet.Enabled = false;
        }
        private void SocketListenerManager_HandleCreated(object sender, EventArgs e)
        {
            Init();
        }
        private void SocketListener_IPChanged(object sender, IPChangedEventArgs e)
        {
            CheckRunning();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                _socketListener.Start();
                CheckRunning();
            }
            catch (Exception ex)
            {
                StatusStopped(true);
                MessageBox.Show(ex.Message);
            }
            this.Cursor = Cursors.Default;
        }
        private void btnRestart_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                StatusStopped(true);
                btnStart.Enabled = false;
                _socketListener.Restart();
                CheckRunning();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.Cursor = Cursors.Default;
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            StatusStopped(true);
            _socketListener.Stop();
            this.Cursor = Cursors.Default;
        }
        private void CheckRunning()
        {
            chkPreferred.Text = "Preferred (now -> " + _socketListener.PreferredIP + ":" + _socketListener.PreferredPort + ")";
            if (_socketListener.IsRunning)
            {
                this.Cursor = Cursors.WaitCursor;
                lblStatus.Text = "Status: Started";
                cboIP.Items.Clear();
                cboIP.Items.AddRange(_socketListener.AvailableIPs);
                cboIP.SelectedItem = _socketListener.IP;
                cboIP.Enabled = true;

                nudPort.Value = _socketListener.Port;
                nudPort.Enabled = true;

                btnStart.Enabled = false;
                btnRestart.Enabled = true;
                btnStop.Enabled = true;
                btnSet.Enabled = false;

                chkPreferred.Enabled = !(_socketListener.IP == _socketListener.PreferredIP && _socketListener.Port == _socketListener.PreferredPort);
                this.Cursor = Cursors.Default;
            }
            else
            {
                StatusStopped(true);
            }
        }
        private void StatusStopped(bool clearCBO)
        {
            this.Cursor = Cursors.WaitCursor;
            lblStatus.Text = "Status: Stopped";

            if (clearCBO)
                cboIP.Items.Clear();
            cboIP.Enabled = false;
            nudPort.Enabled = false;
            btnStart.Enabled = true;
            btnRestart.Enabled = false;
            btnStop.Enabled = false;
            btnSet.Enabled = false;
            chkPreferred.Enabled = false;
            this.Cursor = Cursors.Default;
        }
        private void cbo_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSet.Enabled = true;
            chkPreferred.Checked = _socketListener.CheckAgainstPreferred(cboIP.SelectedItem.ToString(), (int)nudPort.Value);
        }
        //Copy the ip to the clipboard.
        private void btnCopyIP_Click(object sender, EventArgs e)
        {
            ClipboardWrapper.SetDataObject(cboIP.SelectedItem.ToString());
        }
        private void nudPort_ValueChanged(object sender, EventArgs e)
        {
            btnSet.Enabled = true;
            chkPreferred.Checked = _socketListener.CheckAgainstPreferred(cboIP.SelectedItem.ToString(), (int)nudPort.Value);
        }
        private void btnSet_Click(object sender, EventArgs e)
        {
            _socketListener.IPChanged -= new EventHandler<IPChangedEventArgs>(SocketListener_IPChanged);
            StatusStopped(false);
            btnStart.Enabled = false;
            try
            {
                _socketListener.SetIPAndPort(cboIP.SelectedItem.ToString(), (int)nudPort.Value, chkPreferred.Checked);
            }
            catch
            {
                MessageBox.Show(string.Format("The socketlistening could not be bound to port {0} on IP {1}!", nudPort.Value, cboIP.SelectedItem), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                _socketListener.Start();
            }
            CheckRunning();
            chkPreferred.Checked = _socketListener.CheckAgainstPreferred(cboIP.SelectedItem.ToString(), (int)nudPort.Value);
            btnSet.Enabled = false;
            _socketListener.IPChanged += new EventHandler<IPChangedEventArgs>(SocketListener_IPChanged);
        }

        private void chkPreferred_CheckedChanged(object sender, EventArgs e)
        {
            if (cboIP.SelectedItem.ToString() == _socketListener.IP && (int)nudPort.Value == _socketListener.Port)
                chkPreferred.Enabled = !(_socketListener.IP == _socketListener.PreferredIP && _socketListener.Port == _socketListener.PreferredPort);
            else
                chkPreferred.Enabled = true;
            if (chkPreferred.Enabled)
                btnSet.Enabled = true;
        }
        public override string ToString()
        {
            return "Socket Listener Manager";

        }
        #endregion
    }
}