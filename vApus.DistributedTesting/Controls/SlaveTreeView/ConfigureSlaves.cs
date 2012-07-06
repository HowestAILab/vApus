/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.SolutionTree;
using System.Net;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public partial class ConfigureSlaves : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        #region Fields
        private Client _client;
        private DistributedTestMode _distributedTestMode;

        /// <summary>
        /// To set the host name and ip
        /// </summary>
        private ActiveObject _activeObject = new ActiveObject();
        private delegate void SetHostNameAndIPDel(out string ip, out string hostname, out  bool online);
        private SetHostNameAndIPDel _SetHostNameAndIPDel;
        #endregion

        #region Properties
        public Client Client
        {
            get { return _client; }
        }
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
            if (sender == _client)
                SetClient(_client);
        }
        /// <summary>
        /// Sets the client and refreshes the gui.
        /// </summary>
        /// <param name="client"></param>
        public void SetClient(Client client, bool online)
        {
            if (online)
            {
                picStatus.Image = vApus.DistributedTesting.Properties.Resources.OK;
                toolTip.SetToolTip(picStatus, "Online <f5>");
            }
            else
            {
                picStatus.Image = vApus.DistributedTesting.Properties.Resources.Cancelled;
                toolTip.SetToolTip(picStatus, "Offline <f5>");
            }

            SetClient(client);
        }

        public void SetClient(Client client)
        {
            lblUsage.Visible = false;

            lblHostName.Visible =
            txtHostName.Visible =
            lblIP.Visible =
            txtIP.Visible =
            picStatus.Visible =
            picSort.Visible =
            flp.Visible = true;

            txtHostName.Text = client.HostName;
            txtIP.Text = client.IP;

            if (IsHandleCreated)
                LockWindowUpdate(this.Handle.ToInt32());

            _client = client;

            if (flp.Controls.Count < _client.Count)
            {
                var slaveTiles = new Control[_client.Count - flp.Controls.Count];
                for (int i = 0; i != slaveTiles.Length; i++)
                    slaveTiles[i] = CreateSlaveTile();
                flp.Controls.AddRange(slaveTiles);
            }
            else
            {
                var slaveTiles = new Control[flp.Controls.Count - _client.Count];
                for (int i = _client.Count; i != flp.Controls.Count; i++)
                    slaveTiles[i - _client.Count] = flp.Controls[i];

                //No layouting must happen this way.
                for (int i = slaveTiles.Length - 1; i != -1; i--)
                    flp.Controls.RemoveAt(i);
            }


            if (flp.Controls.Count != 0)
            {
                for (int i = 0; i != _client.Count; i++)
                {
                    SlaveTile st = flp.Controls[i] as SlaveTile;
                    st.SetSlave(_client[i] as Slave);
                    st.SetMode(_distributedTestMode);
                }
            }

            LockWindowUpdate(0);
        }
        private SlaveTile CreateSlaveTile()
        {
            var st = new SlaveTile();
            st.DuplicateClicked += new EventHandler(st_DuplicateClicked);
            st.DeleteClicked += new EventHandler(st_DeleteClicked);
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
            _client.InsertWithoutInvokingEvent(_client.IndexOf(st.Slave), clone);
            _client.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);
        }
        private void st_DeleteClicked(object sender, EventArgs e)
        {
            var st = sender as SlaveTile;
            _client.Remove(st.Slave);
        }

        public void ClearClient()
        {
            lblUsage.Visible = true;

            lblHostName.Visible =
            txtHostName.Visible =
            lblIP.Visible =
            txtIP.Visible =
            picStatus.Visible =
            picSort.Visible =
            flp.Visible = false;
        }

        private void picSort_Click(object sender, EventArgs e)
        {
            _client.Sort();
        }

        public void SetMode(DistributedTestMode distributedTestMode)
        {
            if (_distributedTestMode != distributedTestMode)
            {
                LockWindowUpdate(this.Handle.ToInt32());
                _distributedTestMode = distributedTestMode;

                txtHostName.ReadOnly =
                txtIP.ReadOnly =
                picStatus.Enabled = _distributedTestMode == DistributedTestMode.Edit;

                foreach (SlaveTile st in flp.Controls)
                    st.SetMode(_distributedTestMode);
                LockWindowUpdate(0);
            }
        }
        #endregion

        private void HostNameOrIP_KeyUp(object sender, KeyEventArgs e)
        {
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

            txtHostName.Enabled = txtIP.Enabled = false;

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
                    if (IPAddress.TryParse(txtIP.Text, out address))
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
                        hostname = txtHostName.Text;
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

                        _client.IP = txtIP.Text = ip;
                        _client.HostName = txtHostName.Text = hostname;

                        if (online)
                        {
                            picStatus.Image = vApus.DistributedTesting.Properties.Resources.OK;
                            toolTip.SetToolTip(picStatus, "Online <f5>");
                        }
                        else
                        {
                            picStatus.Image = vApus.DistributedTesting.Properties.Resources.Cancelled;
                            toolTip.SetToolTip(picStatus, "Offline <f5>");
                        }

                        if (changed && !this.IsDisposed)
                            _client.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

                        //if (HostNameAndIPSet != null)
                        //    HostNameAndIPSet(this, null);

                        txtHostName.Enabled = txtIP.Enabled = true;
                    }
                    catch { }
            }, null);
        }
    }
}
