/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public partial class SlaveTreeView : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        /// <summary>
        /// The selected item is the sender
        /// </summary>
        public event EventHandler AfterSelect;
        public event EventHandler ClientHostNameAndIPSet;

        #region Fields
        private DistributedTestMode _distributedTestMode;
        #endregion

        #region Constructors
        public SlaveTreeView()
        {
            InitializeComponent();
        }
        #endregion

        #region Functions
        public void SetDistributedTest(DistributedTest distributedTest)
        {
            LockWindowUpdate(this.Handle.ToInt32());
            largeList.Clear();
            var castvi = new ClientsAndSlavesTreeViewItem(distributedTest.ClientsAndSlaves);
            castvi.AfterSelect += new EventHandler(_AfterSelect);
            castvi.AddClientClicked += new EventHandler(castvi_AddClientClicked);
            largeList.Add(castvi);

            foreach (Client client in distributedTest.ClientsAndSlaves)
                CreateAndAddClientTreeViewItem(client);

            SetGui();

            //select the first client tvi if any
            bool selected = false;
            foreach (Control control in largeList.AllControls)
                if (control is ClientTreeViewItem)
                {
                    control.Select();
                    selected = true;
                    break;
                }

            if (!selected)
                castvi.Select();

            castvi.SetHostNameAndIP();

            LockWindowUpdate(0);
        }

        private void castvi_AddClientClicked(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            var castvi = sender as ClientsAndSlavesTreeViewItem;

            Client client = new Client();
            client.AddWithoutInvokingEvent(new Slave(), false);
            CreateAndAddClientTreeViewItem(client);


            castvi.ClientsAndSlaves.Add(client);
            castvi.ClientsAndSlaves.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Added, true);

            LockWindowUpdate(0);
        }

        private void CreateAndAddClientTreeViewItem(Client client)
        {
            var cvi = new ClientTreeViewItem(client);
            //Used for handling collapsing and expanding.
            cvi.SetParent(largeList);
            cvi.AfterSelect += new EventHandler(_AfterSelect);
            cvi.AddSlaveClicked += new EventHandler(cvi_AddSlaveClicked);
            cvi.DuplicateClicked += new EventHandler(cvi_DuplicateClicked);
            cvi.DeleteClicked += new EventHandler(cvi_DeleteClicked);
            cvi.HostNameAndIPSet += new EventHandler(cvi_HostNameAndIPSet);

            (largeList[0][0] as ClientsAndSlavesTreeViewItem).ChildControls.Add(cvi);

            largeList.Add(cvi);
        }

        private void CreateAndInsertClientTreeViewItem(Client client, KeyValuePair<int, int> index)
        {
            var cvi = new ClientTreeViewItem(client);
            //Used for handling collapsing and expanding.
            cvi.SetParent(largeList);
            cvi.AfterSelect += new EventHandler(_AfterSelect);
            cvi.AddSlaveClicked += new EventHandler(cvi_AddSlaveClicked);
            cvi.DuplicateClicked += new EventHandler(cvi_DuplicateClicked);
            cvi.DeleteClicked += new EventHandler(cvi_DeleteClicked);
            cvi.HostNameAndIPSet += new EventHandler(cvi_HostNameAndIPSet);

            (largeList[0][0] as ClientsAndSlavesTreeViewItem).ChildControls.Add(cvi);

            largeList.Insert(cvi, index);

            cvi.Select();

            cvi.SetHostNameAndIP();
        }
        private void cvi_AddSlaveClicked(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            ClientTreeViewItem cvi = sender as ClientTreeViewItem;

            Slave slave = new Slave();
            //Choose another port so every new slave has a unique port.
            for (int port = slave.Port; port != int.MaxValue; port++)
            {
                bool portPresent = false;
                foreach (Slave sl in cvi.Client)
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
            cvi.Client.Add(slave);
            cvi.Client.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Added, true);

            LockWindowUpdate(0);
        }
        private void cvi_DuplicateClicked(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            ClientTreeViewItem cvi = sender as ClientTreeViewItem;
            if (cvi.Client.Parent != null)
            {
                var clone = cvi.Client.Clone();

                var parent = cvi.Client.Parent as ClientsAndSlaves;
                parent.InsertWithoutInvokingEvent(parent.IndexOf(cvi.Client), clone);

                CreateAndInsertClientTreeViewItem(clone, largeList.IndexOf(cvi));

                parent.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Added, true);
            }

            LockWindowUpdate(0);
        }
        private void cvi_DeleteClicked(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            ClientTreeViewItem cvi = sender as ClientTreeViewItem;
            if (cvi.Client.Parent != null)
                cvi.Client.Parent.Remove(cvi.Client);

            (largeList[0][0] as ClientsAndSlavesTreeViewItem).ChildControls.Remove(cvi);

            largeList.Remove(cvi);

            largeList.Select();

            LockWindowUpdate(0);
        }
        //private void cvi_DuplicateClicked(object sender, EventArgs e)
        //{
        //    LockWindowUpdate(this.Handle.ToInt32());

        //    TileTreeViewItem tvi = sender as TileTreeViewItem;
        //    if (tvi.Tile.Parent != null)
        //    {
        //        var clone = tvi.Tile.Clone();

        //        var parent = tvi.Tile.Parent as Tiles;
        //        parent.InsertWithoutInvokingEvent(parent.IndexOf(tvi.Tile), clone);

        //        CreateAndInsertTileTreeViewItem(clone, largeList.IndexOf(tvi));

        //        parent.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Added, true);
        //    }

        //    LockWindowUpdate(0);
        //}

        private void _AfterSelect(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            foreach (ITreeViewItem item in largeList.AllControls)
                if (item != sender)
                    item.Unfocus();

            if (AfterSelect != null)
                AfterSelect(sender, null);

            LockWindowUpdate(0);
        }
        private void cvi_HostNameAndIPSet(object sender, EventArgs e)
        {
            if (ClientHostNameAndIPSet != null)
                ClientHostNameAndIPSet(sender, e);
        }
        public void SetGui()
        {
            foreach (ITreeViewItem ctrl in largeList.AllControls)
            {
                ctrl.SetVisibleControls();
                //To determine what add tile stresstest control can be visible
                ctrl.RefreshGui();
            }
        }

        public void SetMode(DistributedTestMode distributedTestMode)
        {
            if (_distributedTestMode != distributedTestMode)
            {
                _distributedTestMode = distributedTestMode;
                foreach (ITreeViewItem item in largeList.AllControls)
                    item.SetDistributedTestMode(_distributedTestMode);
            }
        }
        #endregion
    }
}
