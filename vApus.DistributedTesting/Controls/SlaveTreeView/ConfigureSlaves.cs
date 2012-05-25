/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

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
        }

        public void SetClient(Client client)
        {
            lblUsage.Visible = false;
            flp.Visible = true;

            if (_client != client)
            {
                if (IsHandleCreated)
                    LockWindowUpdate(this.Handle.ToInt32());

                _client = client;
                flp.Controls.Clear();

                while (flp.Controls.Count < _client.Count)
                    flp.Controls.Add(new SlaveTile());
                while (flp.Controls.Count > _client.Count)
                    flp.Controls.RemoveAt(0);

                for (int i = 0; i != _client.Count; i++)
                    (flp.Controls[i] as SlaveTile).SetSlave(_client[i] as Slave);

                LockWindowUpdate(0);
            }
        }
        public void ClearClient()
        {
            lblUsage.Visible = true;
            flp.Visible = false;
        }
    }
}
