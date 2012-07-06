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
using vApus.Stresstest;
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

        private bool _online = false;

        private DistributedTestMode _distributedTestMode;
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
                foreach (Slave s in _client)
                    if (s.TileStresstest != null)
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

            lblClient.Text = _client.ToString() + "  -  (" + UsedSlaveCount + "/" + _client.Count + ")";
        }
        #endregion

        #region Functions
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
            if (_distributedTestMode == DistributedTestMode.Edit)
            {
                picAddSlave.Visible = picDuplicate.Visible = picDelete.Visible = visible;
                lblClient.Text = _client.ToString() + "  -  (" + UsedSlaveCount + "/" + _client.Count + ")";
            }
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
            lblClient.Text = _client.ToString() + "  -  (" + UsedSlaveCount + "/" + _client.Count + ")";
        }
        private void _KeyUp(object sender, KeyEventArgs e)
        {
            if (_distributedTestMode == DistributedTestMode.TestAndReport)
            {
                _ctrl = false;
                return;
            }

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


            picStatus.Image = vApus.DistributedTesting.Properties.Resources.Busy;

            string ip = string.Empty;
            string hostname = string.Empty;
            bool online = false;

            _activeObject.Send(_SetHostNameAndIPDel, ip, hostname, online);

            return true;
        }
        private void SetHostNameAndIP_Callback(out string ip, out string hostname, out  bool online)
        {
            online = false;
            ip = _client.IP;
            hostname = string.Empty;

            if (!this.IsDisposed)
                try
                {
                    hostname = Dns.GetHostByAddress(ip).HostName.ToLower();
                    if (hostname == null) hostname = string.Empty;
                    online = true;
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

                        //                        if (_client.IP != ip || _client.HostName != hostname)

                        if (_client.HostName != hostname)
                            changed = true;

                        _client.IP = ip;
                        _client.HostName = hostname;

                        if (online)
                        {
                            _online = true;
                            picStatus.Image = vApus.DistributedTesting.Properties.Resources.OK;
                            toolTip.SetToolTip(picStatus, "Online <f5>");
                        }
                        else
                        {
                            _online = false;
                            picStatus.Image = vApus.DistributedTesting.Properties.Resources.Cancelled;
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
        /// <summary>
        /// Enabling or disabling the controls on this.
        /// This way the next item in the panel does not auto get the focus.
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableControls(bool enabled)
        {
            foreach (Control ctrl in this.Controls)
                if (ctrl != picStatus)
                    ctrl.Enabled = enabled;
        }

        private void _KeyDown(object sender, KeyEventArgs e)
        {
            if (_distributedTestMode == DistributedTestMode.TestAndReport)
                return;

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

        public void SetDistributedTestMode(DistributedTestMode distributedTestMode)
        {
            _distributedTestMode = distributedTestMode;
#warning Flag it or check if it is used in the tests.
            if (_distributedTestMode == DistributedTestMode.TestAndReport)
                picAddSlave.Visible =
                picDuplicate.Visible =
                picDelete.Visible = false;
        }
        #endregion
    }
}
