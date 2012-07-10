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

            castvi.SetHostNameAndIP();
            castvi.Select();

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

            LockWindowUpdate(0);
        }

        private void CreateAndAddClientTreeViewItem(Client client)
        {
            var cvi = new ClientTreeViewItem(client);
            //Used for handling collapsing and expanding.
            cvi.SetParent(largeList);
            cvi.AfterSelect += new EventHandler(_AfterSelect);
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
            cvi.DuplicateClicked += new EventHandler(cvi_DuplicateClicked);
            cvi.DeleteClicked += new EventHandler(cvi_DeleteClicked);
            cvi.HostNameAndIPSet += new EventHandler(cvi_HostNameAndIPSet);

            (largeList[0][0] as ClientsAndSlavesTreeViewItem).ChildControls.Add(cvi);

            largeList.Insert(cvi, index);

            cvi.SetHostNameAndIP();
        }
        private void cvi_DuplicateClicked(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            ClientTreeViewItem cvi = sender as ClientTreeViewItem;
            if (cvi.Client.Parent != null)
            {
                var clone = cvi.Client.Clone();
                var parent = cvi.Client.Parent as ClientsAndSlaves;
                int cloneIndex = parent.IndexOf(cvi.Client) + 1;

                if (parent.Count == cloneIndex)
                    parent.AddWithoutInvokingEvent(clone, false);
                else
                    parent.InsertWithoutInvokingEvent(cloneIndex, clone, false);


                var cloneIndexForLargeList = largeList.ParseFlatIndex(largeList.ParseIndex(largeList.IndexOf(cvi)) + 1);
                if (cloneIndexForLargeList.Key == -1)
                    CreateAndAddClientTreeViewItem(clone);
                else
                    CreateAndInsertClientTreeViewItem(clone, cloneIndexForLargeList);

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

            var previousIndex = largeList.ParseFlatIndex(largeList.FlatIndexOf(cvi) - 1);
            largeList.Remove(cvi);

            largeList[previousIndex.Key][previousIndex.Value].Select();

            LockWindowUpdate(0);
        }

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
                LockWindowUpdate(this.Handle.ToInt32());
                _distributedTestMode = distributedTestMode;
                foreach (ITreeViewItem item in largeList.AllControls)
                    item.SetDistributedTestMode(_distributedTestMode);
                LockWindowUpdate(0);
            }
        }
        #endregion
    }
}
