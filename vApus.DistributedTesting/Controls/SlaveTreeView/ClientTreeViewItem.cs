/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.DistributedTesting
{
    [ToolboxItem(false)]
    public partial class ClientTreeViewItem : UserControl, ITreeViewItem
    {
        #region Events
        /// <summary>
        /// Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;
        public event EventHandler AddSlaveClicked;
        public event EventHandler DuplicateClicked;
        public event EventHandler DeleteClicked;
        public event EventHandler HostNameAndIPSet;
        #endregion

        #region Fields
        private Client _client = new Client();
        /// <summary>
        /// Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;

        /// <summary>
        /// To set the host name and ip
        /// </summary>
        private ActiveObject _activeObject = new ActiveObject();
        private delegate void SetHostNameAndIPDel(out string ip, out string hostname, out  bool online);
        private SetHostNameAndIPDel _SetHostNameAndIPDel;

        private int _chosenImageIndex = 2;

        private bool _online = false;
        #endregion

        #region Properties
        public Client Client
        {
            get { return _client; }
        }
        public bool Online
        {
            get { return _online; }
        }
        private int UsedSlaveCount
        {
            get
            {
                int count = 0;
                foreach (Slave slave in _client)
                    if (slave.Use)
                        ++count;
                return count;
            }
        }
        #endregion

        #region Constructors
        public ClientTreeViewItem()
        {
            InitializeComponent();

            _SetHostNameAndIPDel = SetHostNameAndIP_Callback;
            _activeObject.OnResult += new EventHandler<ActiveObject.OnResultEventArgs>(_activeObject_OnResult);
        }
        public ClientTreeViewItem(Client client)
            : this()
        {
            _client = client;
            RefreshGui();

            chk.CheckedChanged -= chk_CheckedChanged;
            chk.Checked = _client.Use;
            chk.CheckedChanged += chk_CheckedChanged;

            lblClient.Text = txtClient.Visible ? "Host Name or IP:" : _client.ToString() + "  -  (#" + UsedSlaveCount + "/" + _client.Count + ")";

            txtClient.Text = (_client.IP == string.Empty) ? _client.HostName : _client.IP;

            //To check if the use has changed of the child controls.
            SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);
        }
        #endregion

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            //To set if the client is used or not.
            if (sender is Slave)
            {
                Slave slave = sender as Slave;
                if (_client.Contains(slave))
                {
                    _client.Use = false;
                    foreach (Slave sl in _client)
                        if (sl.Use)
                        {
                            _client.Use = true;
                            break;
                        }
                    if (chk.Checked != _client.Use)
                    {
                        chk.CheckedChanged -= chk_CheckedChanged;
                        chk.Checked = _client.Use;
                        chk.CheckedChanged += chk_CheckedChanged;
                    }
                }
            }
        }
        private void _Enter(object sender, EventArgs e)
        {
            this.BackColor = SystemColors.Control;
            SetVisibleControls();

            if (AfterSelect != null)
                AfterSelect(this, null);
        }
        public void Unfocus()
        {
            this.BackColor = Color.Transparent;
            SetVisibleControls();
        }
        private void txtClient_Leave(object sender, EventArgs e)
        {
            SetHostNameAndIP();
            txtClient.Text = (_client.IP == string.Empty) ? _client.HostName : _client.IP;
            SetVisibleControls();
        }
        private void _MouseEnter(object sender, EventArgs e)
        {
            SetVisibleControls();
        }
        private void _MouseLeave(object sender, EventArgs e)
        {
            SetVisibleControls();
        }
        public void SetVisibleControls(bool visible)
        {
            txtClient.Visible = picAddSlave.Visible = picDuplicate.Visible = picDelete.Visible = visible;
            lblClient.Text = visible ? "Host Name or IP:" : _client.ToString() + "  -  (#" + UsedSlaveCount + "/" + _client.Count + ")";
        }
        public void SetVisibleControls()
        {
            if (this.IsDisposed)
                return;

            if (this.BackColor == SystemColors.Control)
                SetVisibleControls(true);
            else
                SetVisibleControls(ClientRectangle.Contains(PointToClient(Cursor.Position)));
        }

        public void RefreshGui()
        {
            lblClient.Text = txtClient.Visible ? "Host Name or IP:" : _client.ToString() + "  -  (#" + UsedSlaveCount + "/" + _client.Count + ")";

            _client.Use = UsedSlaveCount != 0;
            if (_client.Use != chk.Checked)
            {
                chk.CheckedChanged -= chk_CheckedChanged;
                chk.Checked = _client.Use;
                chk.CheckedChanged += chk_CheckedChanged;
            }
        }
        private void _KeyUp(object sender, KeyEventArgs e)
        {
            if (sender == txtClient && e.KeyCode == Keys.Enter)
                SetHostNameAndIP();

            if (e.KeyCode == Keys.ControlKey)
                _ctrl = false;
            else if (_ctrl)
            {
                if (e.KeyCode == Keys.I && AddSlaveClicked != null)
                    AddSlaveClicked(this, null);
                if (e.KeyCode == Keys.R && DeleteClicked != null)
                    DeleteClicked(this, null);
                else if (e.KeyCode == Keys.D && DuplicateClicked != null)
                    DuplicateClicked(this, null);
                else if (e.KeyCode == Keys.U)
                    chk.Checked = !chk.Checked;
            }
            if (e.KeyCode == Keys.F5)
                SetHostNameAndIP();
        }
        /// <summary>
        /// IP or Host Name can be filled in in txtclient.
        /// </summary>
        /// <returns>False if it was already busy doing it.</returns>
        public bool SetHostNameAndIP()
        {
            //Make sure this can not happen multiple times at the same time.
            if (!this.Controls[0].Enabled || !IsHandleCreated)
                return false;

            EnableControls(false);

            //Reset this.
            _chosenImageIndex = 2;
            tmrRotateStatus.Start();

            picStatus.Image = imageListStatus.Images[_chosenImageIndex];

            txtClient.Text = txtClient.Text.Trim().ToLower();

            string ip = string.Empty;
            string hostname = string.Empty;
            bool online = false;

            _activeObject.Send(_SetHostNameAndIPDel, ip, hostname, online);

            return true;
        }
        private void SetHostNameAndIP_Callback(out string ip, out string hostname, out  bool online)
        {
            online = false;
            ip = string.Empty;
            hostname = string.Empty;

            if (!this.IsDisposed)
                try
                {
                    IPAddress address;
                    if (IPAddress.TryParse(txtClient.Text, out address))
                    {
                        ip = address.ToString();
                        try
                        {
                            hostname = Dns.GetHostByAddress(address).HostName.ToLower();
                            if (hostname == null) hostname = string.Empty;
                            online = true;
                        }
                        catch { }

                    }
                    else
                    {
                        hostname = txtClient.Text;
                        IPAddress[] addresses = { };
                        try
                        {
                            if (hostname.Length != 0)
                                addresses = Dns.GetHostByName(hostname).AddressList;
                        }
                        catch { }

                        if (addresses != null && addresses.Length != 0)
                        {
                            ip = addresses[0].ToString();
                            online = true;
                        }
                    }
                }
                catch { }
        }
        private void _activeObject_OnResult(object sender, ActiveObject.OnResultEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                if (!this.IsDisposed)
                    try
                    {
                        string ip = e.Arguments[0] as string;
                        string hostname = e.Arguments[1] as string;
                        bool online = (bool)e.Arguments[2];
                        bool changed = false;

                        if (_client.IP != ip || _client.HostName != hostname)
                            changed = true;

                        _client.IP = ip;
                        _client.HostName = hostname;

                        tmrRotateStatus.Stop();
                        if (online)
                        {
                            _online = true;
                            picStatus.Image = imageListStatus.Images[1];
                            toolTip.SetToolTip(picStatus, "Online <f5>");
                        }
                        else
                        {
                            _online = false;
                            picStatus.Image = imageListStatus.Images[0];
                            toolTip.SetToolTip(picStatus, "Offline <f5>");
                        }

                        if (changed && !this.IsDisposed)
                            _client.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

                        if (HostNameAndIPSet != null)
                            HostNameAndIPSet(this, null);

                        EnableControls(true);
                    }
                    catch { }
            }, null);
        }
        private void tmrRotateOnlineOffline_Tick(object sender, EventArgs e)
        {
            //Rotate it to visualize it is refreshing
            _chosenImageIndex = _chosenImageIndex == 2 ? 3 : 2;
            picStatus.Image = imageListStatus.Images[_chosenImageIndex];
        }
        /// <summary>
        /// Enabling or disabling the controls on this.
        /// This way the next item in the panel does not auto get the focus.
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableControls(bool enabled)
        {
            foreach (Control ctrl in this.Controls)
                ctrl.Enabled = enabled;
        }

        private void _KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
                _ctrl = true;
        }
        private void picAddSlave_Click(object sender, EventArgs e)
        {
            this.Focus();
            if (AddSlaveClicked != null)
                AddSlaveClicked(this, null);
        }
        private void picDuplicate_Click(object sender, EventArgs e)
        {
            if (DuplicateClicked != null)
                DuplicateClicked(this, null);
        }
        private void picDelete_Click(object sender, EventArgs e)
        {
            if (DeleteClicked != null)
                DeleteClicked(this, null);
        }
        private void picStatus_Click(object sender, EventArgs e)
        {
            SetHostNameAndIP();
        }
        private void chk_CheckedChanged(object sender, EventArgs e)
        {
            if (_client.Use != chk.Checked)
            {
                _client.Use = chk.Checked;
                foreach (TileStresstest ts in _client)
                    ts.Use = _client.Use;

                _client.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }
        #endregion


        public void SetDistributedTestMode(DistributedTestMode distributedTestMode)
        {
        }

        public DistributedTestMode DistributedTestMode
        {
            get { throw new NotImplementedException(); }
        }
    }
}
