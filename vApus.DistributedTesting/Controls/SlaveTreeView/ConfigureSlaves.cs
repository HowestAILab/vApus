/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public partial class ConfigureSlaves : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        public event EventHandler GoToAssignedTest;

        #region Fields
        private DistributedTest _distributedTest;
        private ClientTreeViewItem _clientTreeViewItem;
        private DistributedTestMode _distributedTestMode;

        /// <summary>
        /// To set the host name and ip
        /// </summary>
        private ActiveObject _activeObject = new ActiveObject();
        private delegate void SetHostNameAndIPDel(out string ip, out string hostname, out  bool online);
        private SetHostNameAndIPDel _SetHostNameAndIPDel;

        /// <summary>
        /// To refresh the status.
        /// </summary>
        private bool _ipChanged = true;
        #endregion

        #region Constructor
        public ConfigureSlaves()
        {
            InitializeComponent();
            SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);

            _SetHostNameAndIPDel = SetHostNameAndIP_Callback;
            _activeObject.OnResult += new EventHandler<ActiveObject.OnResultEventArgs>(_activeObject_OnResult);
        }
        #endregion

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            if (_clientTreeViewItem != null && sender == _clientTreeViewItem.Client)
                SetClient();
        }
        public void SetDistributedTest(DistributedTest distributedTest)
        {
            _distributedTest = distributedTest;
        }
        /// <summary>
        /// Sets the client and refreshes the gui.
        /// </summary>
        /// <param name="client"></param>
        public void SetClient(ClientTreeViewItem clientTreeViewItem)
        {
            _clientTreeViewItem = clientTreeViewItem;
            if (_clientTreeViewItem.Online)
            {
                picStatus.Image = vApus.DistributedTesting.Properties.Resources.OK;
                toolTip.SetToolTip(picStatus, "Client Online");
            }
            else
            {
                picStatus.Image = vApus.DistributedTesting.Properties.Resources.Cancelled;
                toolTip.SetToolTip(picStatus, "Client Offline");
            }

            SetClient();
        }

        private void SetClient()
        {
            lblUsage.Visible = false;

            pnlSettings.Visible =
            flp.Visible = true;

            txtHostName.Text = _clientTreeViewItem.Client.HostName;
            txtIP.Text = _clientTreeViewItem.Client.IP;

            txtUserName.Text = _clientTreeViewItem.Client.UserName;
            txtPassword.Text = _clientTreeViewItem.Client.Password;
            txtDomain.Text = _clientTreeViewItem.Client.Domain;

            if (IsHandleCreated)
                LockWindowUpdate(this.Handle.ToInt32());

            if (flp.Controls.Count < _clientTreeViewItem.Client.Count)
            {
                var slaveTiles = new Control[_clientTreeViewItem.Client.Count - flp.Controls.Count];
                for (int i = 0; i != slaveTiles.Length; i++)
                    slaveTiles[i] = CreateSlaveTile();
                flp.Controls.AddRange(slaveTiles);
            }
            else
            {
                var slaveTiles = new Control[flp.Controls.Count - _clientTreeViewItem.Client.Count];
                for (int i = _clientTreeViewItem.Client.Count; i != flp.Controls.Count; i++)
                    slaveTiles[i - _clientTreeViewItem.Client.Count] = flp.Controls[i];

                //No layouting must happen this way.
                for (int i = slaveTiles.Length - 1; i != -1; i--)
                    flp.Controls.RemoveAt(i);
            }


            if (flp.Controls.Count != 0)
            {
                for (int i = 0; i != _clientTreeViewItem.Client.Count; i++)
                {
                    SlaveTile st = flp.Controls[i] as SlaveTile;
                    st.SetSlave(_clientTreeViewItem.Client[i] as Slave);
                    st.SetMode(_distributedTestMode);
                }
            }

            LockWindowUpdate(0);
        }
        private SlaveTile CreateSlaveTile()
        {
            var st = new SlaveTile(_distributedTest);
            st.DuplicateClicked += new EventHandler(st_DuplicateClicked);
            st.DeleteClicked += new EventHandler(st_DeleteClicked);
            st.GoToAssignedTest += new EventHandler(st_GoToAssignedTest);
            return st;
        }
        private void st_DuplicateClicked(object sender, EventArgs e)
        {
            var st = sender as SlaveTile;

            Slave clone = st.Slave.Clone();
            //Choose another port so every new slave has a unique port.
            for (int port = clone.Port; port != int.MaxValue; port++)
            {
                bool portPresent = false;
                foreach (Slave sl in st.Slave.Parent)
                    if (sl.Port == port)
                    {
                        portPresent = true;
                        break;
                    }

                if (!portPresent)
                {
                    st.Slave.Port = port;
                    break;
                }
            }
            _clientTreeViewItem.Client.InsertWithoutInvokingEvent(_clientTreeViewItem.Client.IndexOf(st.Slave), clone);
            _clientTreeViewItem.Client.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);
        }
        private void st_DeleteClicked(object sender, EventArgs e)
        {
            var st = sender as SlaveTile;
            _clientTreeViewItem.Client.Remove(st.Slave);
        }
        private void st_GoToAssignedTest(object sender, EventArgs e)
        {
            if (GoToAssignedTest != null)
                GoToAssignedTest(sender, e);
        }
        public void ClearClient()
        {
            lblUsage.Visible = true;

            pnlSettings.Visible =
            flp.Visible = false;
        }

        /// <summary>
        /// Set this if you start or stop the distributed test.
        /// </summary>
        /// <param name="distributedTestMode"></param>
        public void SetMode(DistributedTestMode distributedTestMode)
        {
            if (_distributedTestMode != distributedTestMode)
            {
                LockWindowUpdate(this.Handle.ToInt32());
                _distributedTestMode = distributedTestMode;

                txtHostName.ReadOnly =
                txtIP.ReadOnly =
                txtUserName.ReadOnly = 
                txtPassword.ReadOnly = 
                txtDomain.ReadOnly =
                _distributedTestMode == DistributedTestMode.TestAndReport;

                picAddSlave.Enabled =
                picClearSlaves.Enabled =
                picSort.Enabled =
                _distributedTestMode == DistributedTestMode.Edit;

                foreach (SlaveTile st in flp.Controls)
                    st.SetMode(_distributedTestMode);
                LockWindowUpdate(0);
            }
        }

        private void txtHostName_KeyUp(object sender, KeyEventArgs e)
        {
            _ipChanged = false;
            if (e.KeyCode == Keys.Return)
                SetHostNameAndIP();
        }
        private void txtIP_KeyUp(object sender, KeyEventArgs e)
        {
            _ipChanged = true;
            if (e.KeyCode == Keys.Return)
                SetHostNameAndIP();
        }
        private void picStatus_Click(object sender, EventArgs e)
        {
            SetHostNameAndIP();
        }

        /// <summary>
        /// IP or Host Name can be filled in in txtclient.
        /// </summary>
        /// <returns>False if it was already busy doing it.</returns>
        public bool SetHostNameAndIP()
        {
            //Make sure this can not happen multiple times at the same time.
            if (!txtHostName.Enabled || !IsHandleCreated)
                return false;

            SettingHostNameAndIP(false);
            _clientTreeViewItem.SettingHostNameAndIP(false);


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
            ip = string.Empty;
            hostname = string.Empty;

            if (!this.IsDisposed)
                try
                {
                    IPAddress address;
                    if (_ipChanged && IPAddress.TryParse(txtIP.Text, out address))
                    {
                        ip = address.ToString();
                        try
                        {
                            hostname = Dns.GetHostByAddress(ip).HostName;
                            hostname = (hostname == null) ? string.Empty : hostname.ToLower();
                            online = true;
                        }
                        catch { }

                    }
                    else
                    {
                        hostname = txtHostName.Text.ToLower();
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

                        if (_clientTreeViewItem.Client.IP != ip || _clientTreeViewItem.Client.HostName != hostname)
                            changed = true;

                        _clientTreeViewItem.Client.IP = txtIP.Text = ip;
                        _clientTreeViewItem.Client.HostName = txtHostName.Text = hostname;

                        if (changed && !this.IsDisposed)
                            _clientTreeViewItem.Client.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);


                        _clientTreeViewItem.SettingHostNameAndIP(true, online);
                        SettingHostNameAndIP(true, online);
                    }
                    catch { }
            }, null);
        }
        /// <summary>
        /// Enable or disable controls when setting the host name or ip.
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="online"></param>
        public void SettingHostNameAndIP(bool enabled, bool online = false)
        {
            if (enabled)
            {
                if (online)
                {
                    picStatus.Image = vApus.DistributedTesting.Properties.Resources.OK;
                    toolTip.SetToolTip(picStatus, "Client Online");
                }
                else
                {
                    picStatus.Image = vApus.DistributedTesting.Properties.Resources.Cancelled;
                    toolTip.SetToolTip(picStatus, "Client Offline");
                }
            }
            else
            {
                picStatus.Image = vApus.DistributedTesting.Properties.Resources.Busy;
            }

            pnlSettings.Enabled =
            flp.Enabled = enabled;
        }

        private void txtRDC_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SetRDC();
        }

        private void txtRDC_Leave(object sender, EventArgs e)
        {
            SetRDC();
        }
        private void SetRDC()
        {
            _clientTreeViewItem.Client.UserName = txtUserName.Text;
            _clientTreeViewItem.Client.Password = txtPassword.Text;
            _clientTreeViewItem.Client.Domain = txtDomain.Text;

            _clientTreeViewItem.Client.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        private void picShowRD_Click(object sender, EventArgs e)
        {
            RemoteDesktopClient rdc = SolutionComponentViewManager.Show(_distributedTest, typeof(RemoteDesktopClient)) as RemoteDesktopClient;
            rdc.Text = "Remote Desktop Client";
            rdc.ShowRemoteDesktop(_clientTreeViewItem.Client.HostName, _clientTreeViewItem.Client.IP,
                _clientTreeViewItem.Client.UserName, _clientTreeViewItem.Client.Password, _clientTreeViewItem.Client.Domain);
        }


        private void picAddSlave_Click(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            Slave slave = new Slave();
            //Choose another port so every new slave has a unique port.
            for (int port = slave.Port; port != int.MaxValue; port++)
            {
                bool portPresent = false;
                foreach (Slave sl in _clientTreeViewItem.Client)
                    if (sl.Port == port)
                    {
                        portPresent = true;
                        break;
                    }

                if (!portPresent)
                {
                    slave.Port = port;
                    break;
                }
            }
            _clientTreeViewItem.Client.AddWithoutInvokingEvent(slave, false);
            _clientTreeViewItem.Client.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Added, true);

            LockWindowUpdate(0);
        }
        private void picClearSlaves_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the slaves?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                LockWindowUpdate(this.Handle.ToInt32());
                flp.Controls.Clear();

                _clientTreeViewItem.Client.ClearWithoutInvokingEvent(false);
                _clientTreeViewItem.Client.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Cleared);

                LockWindowUpdate(0);
            }
        }

        private void picSort_Click(object sender, EventArgs e)
        {
            _clientTreeViewItem.Client.Sort();
        }
        #endregion
    }
}
