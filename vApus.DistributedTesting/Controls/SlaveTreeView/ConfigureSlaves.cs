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

        private Client _client;

        public Client Client
        {
            get { return _client; }
        }

        public ConfigureSlaves()
        {
            InitializeComponent();
            SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);
        }

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

            while (flp.Controls.Count < _client.Count)
                flp.Controls.Add(CreateSlaveTile());
            while (flp.Controls.Count > _client.Count)
                flp.Controls.RemoveAt(0);


            if (flp.Controls.Count != 0)
            {
                for (int i = 0; i != _client.Count; i++)
                    (flp.Controls[i] as SlaveTile).SetSlave(_client[i] as Slave);

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
    }
}
