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

namespace vApus.DistributedTesting
{
    public partial class ConfigureSlaves : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        #region Fields
        private Client _client;
        private DistributedTestMode _distributedTestMode;
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
        public void SetClient(Client client)
        {
            lblUsage.Visible = false;
            flp.Visible = picSort.Visible = true;

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
                for(int i = slaveTiles.Length - 1; i != -1; i--)
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

                SetClientStatus((flp.Controls[0] as SlaveTile).ClientOnline);
            }

            LockWindowUpdate(0);
        }
        /// <summary>
        /// Online or offline
        /// </summary>
        /// <param name="online"></param>
        public void SetClientStatus(bool online)
        {
            for (int i = 0; i != _client.Count; i++)
                (flp.Controls[i] as SlaveTile).SetClientStatus(online);
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
            flp.Visible = picSort.Visible = false;
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
                foreach (SlaveTile st in flp.Controls)
                    st.SetMode(_distributedTestMode);
                LockWindowUpdate(0);
            }
        }
        #endregion
    }
}
