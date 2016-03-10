/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using RandomUtils;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using vApus.DistributedTest.Properties;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.DistributedTest {
    [ToolboxItem(false)]
    public partial class ClientTreeViewItem : UserControl, ITreeViewItem {

        #region Events

        /// <summary>
        ///     Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;

        public event EventHandler DuplicateClicked;
        public event EventHandler DeleteClicked;
        public event EventHandler HostNameAndIPSet;
        public event EventHandler DoubleClicked;

        #endregion

        #region Fields

        private readonly SetHostNameAndIPDel _SetHostNameAndIPDel;

        /// <summary>
        ///     To set the host name and ip
        /// </summary>
        private readonly BackgroundWorkQueue _backgroundWorkQueue = new BackgroundWorkQueue();

        private readonly Client _client = new Client();
        private ConfigureSlaves _configureSlaves;

        /// <summary>
        ///     Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;

        private DistributedTestMode _distributedTestMode;
        private bool _online;

        private delegate void SetHostNameAndIPDel(out string ip, out string hostname, out bool online);

        #endregion

        #region Properties

        public Client Client { get { return _client; } }

        public bool Online { get { return _online; } }

        public ConfigureSlaves ConfigureSlaves {
            get { return _configureSlaves; }
            set { _configureSlaves = value; }
        }

        #endregion

        #region Constructors

        public ClientTreeViewItem() {
            InitializeComponent();

            _SetHostNameAndIPDel = SetHostNameAndIP_Callback;
            _backgroundWorkQueue.OnWorkItemProcessed += _activeObject_OnResult;
        }

        public ClientTreeViewItem(Client client)
            : this() {
            _client = client;
            RefreshGui();

            lblClient.Text = _client + "  -  (" + _client.UsedSlaveCount + "/" + _client.Count + ")";
        }

        #endregion

        #region Functions

        public void Unfocus() {
            BackColor = Color.Transparent;
            SetVisibleControls();
        }

        public void SetVisibleControls(bool visible) {
            if (_distributedTestMode == DistributedTestMode.Edit) {
                picDuplicate.Visible = picDelete.Visible = visible;
                lblClient.Text = _client + "  -  (" + _client.UsedSlaveCount + "/" + _client.Count + ")";
            }
        }

        public void SetVisibleControls() {
            if (IsDisposed) return;

            if (BackColor == SystemColors.Control) SetVisibleControls(true);
            else SetVisibleControls(ClientRectangle.Contains(PointToClient(Cursor.Position)));
        }

        public void RefreshGui() {
            lblClient.Text = _client + "  -  (" + _client.UsedSlaveCount + "/" + _client.Count + ")";
        }

        public void SetDistributedTestMode(DistributedTestMode distributedTestMode) {
            _distributedTestMode = distributedTestMode;
            picDuplicate.Visible = picDelete.Visible = _distributedTestMode == DistributedTestMode.Edit;
        }

        private void _Enter(object sender, EventArgs e) {
            BackColor = SystemColors.Control;
            SetVisibleControls();

            if (AfterSelect != null) AfterSelect(this, null);
        }

        private void _DoubleClick(object sender, EventArgs e) {
            if (DoubleClicked != null) DoubleClicked.Invoke(this, null);
        }

        private void _MouseEnter(object sender, EventArgs e) { SetVisibleControls(); }

        private void _MouseLeave(object sender, EventArgs e) { SetVisibleControls(); }

        private void _KeyUp(object sender, KeyEventArgs e) {
            if (_distributedTestMode == DistributedTestMode.Test) {
                _ctrl = false;
                return;
            }

            if (e.KeyCode == Keys.ControlKey)
                _ctrl = false;
            else if (_ctrl) {
                if (e.KeyCode == Keys.R && DeleteClicked != null)
                    DeleteClicked(this, null);
                else if (e.KeyCode == Keys.D && DuplicateClicked != null)
                    DuplicateClicked(this, null);
            }
            if (e.KeyCode == Keys.F5) SetHostNameAndIP();
        }

        /// <summary>
        ///     IP or Host Name can be filled in in txtclient.
        /// </summary>
        /// <returns>False if it was already busy doing it.</returns>
        public bool SetHostNameAndIP() {
            //Make sure this can not happen multiple times at the same time.
            if (!Controls[0].Enabled || !IsHandleCreated) return false;

            SettingHostNameAndIP(false);
            if (_configureSlaves != null)
                _configureSlaves.SettingHostNameAndIP(false);

            string ip = string.Empty;
            string hostname = string.Empty;
            bool online = false;

            _backgroundWorkQueue.EnqueueWorkItem(_SetHostNameAndIPDel, ip, hostname, online);

            return true;
        }

        private void SetHostNameAndIP_Callback(out string ip, out string hostname, out bool online) {
            online = false;
            ip = _client.IP;
            hostname = string.Empty;

            if (!IsDisposed)
                try {
                    hostname = Dns.GetHostEntry(ip).HostName;
                    hostname = (hostname == null) ? string.Empty : hostname.ToLowerInvariant();
                    online = true;
                } catch {
                //Ignore resolve issues.
                }
        }

        private void _activeObject_OnResult(object sender, BackgroundWorkQueue.OnWorkItemProcessedEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                if (!IsDisposed)
                    try {
                        var ip = e.Parameters[0] as string;
                        var hostname = e.Parameters[1] as string;
                        var online = (bool)e.Parameters[2];

                        _client.IP = ip;
                        _client.HostName = hostname;


                        if (HostNameAndIPSet != null) HostNameAndIPSet(this, null);

                        if (_configureSlaves != null) _configureSlaves.SettingHostNameAndIP(true, online);
                        SettingHostNameAndIP(true, online);
                    } catch {
                        //Only on gui closed.
                    }
            }, null);
        }

        /// <summary>
        ///     Enabling or disabling the controls on this.
        ///     This way the next item in the panel does not auto get the focus.
        /// </summary>
        /// <param name="enabled"></param>
        public void SettingHostNameAndIP(bool enabled, bool online = false) {
            if (enabled) {
                if (online) {
                    _online = true;
                    picStatus.Image = Resources.OK;
                    toolTip.SetToolTip(picStatus, "Client online <f5>.");
                } else {
                    _online = false;
                    picStatus.Image = Resources.Cancelled;
                    toolTip.SetToolTip(picStatus, "Client offline <f5>.");
                }
            } else {
                picStatus.Image = Resources.Busy;
            }

            foreach (Control ctrl in Controls)
                if (ctrl != picStatus)
                    ctrl.Enabled = enabled;
        }

        private void _KeyDown(object sender, KeyEventArgs e) {
            if (_distributedTestMode == DistributedTestMode.Test) return;
            if (e.KeyCode == Keys.ControlKey) _ctrl = true;
        }

        private void picDuplicate_Click(object sender, EventArgs e) {
            if (DuplicateClicked != null) DuplicateClicked(this, null);
        }

        private void picDelete_Click(object sender, EventArgs e) {
            if (DeleteClicked != null) DeleteClicked(this, null);
        }

        private void picStatus_Click(object sender, EventArgs e) {
            SetHostNameAndIP();
        }

        private void picRemoteDesktop_Click(object sender, EventArgs e) {
            RemoteDesktop.Show(_client.IP, _client.UserName, _client.Password, _client.Domain);
            Thread.Sleep(1000);
            RemoteDesktop.RemoveCredentials(_client.IP);
        }

        #endregion

    }
}