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
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.DistributedTest {
    public partial class SlaveTreeView : UserControl {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        /// <summary>
        ///     The selected item is the sender
        /// </summary>
        public event EventHandler AfterSelect;
        public event EventHandler ClientTreeViewItemDoubleClicked;

        public event EventHandler ClientHostNameAndIPSet;

        #region Fields

        private DistributedTestMode _distributedTestMode;

        #endregion

        #region Constructors

        public SlaveTreeView() {
            InitializeComponent();
        }

        #endregion

        #region Functions

        public void SetDistributedTest(DistributedTest distributedTest) {
            if (IsDisposed)
                return;

            LockWindowUpdate(Handle);
            largeList.Clear();
            var castvi = new ClientsAndSlavesTreeViewItem(distributedTest);
            castvi.AfterSelect += _AfterSelect;
            castvi.AddClientClicked += castvi_AddClientClicked;
            largeList.Add(castvi);

            foreach (Client client in distributedTest.Clients)
                CreateAndAddClientTreeViewItem(client);

            SetGui();

            castvi.SetHostNameAndIP();
            castvi.Select();

            LockWindowUpdate(IntPtr.Zero);
        }

        private void castvi_AddClientClicked(object sender, EventArgs e) {
            LockWindowUpdate(Handle);

            var castvi = sender as ClientsAndSlavesTreeViewItem;

            var client = new Client();
            client.AddWithoutInvokingEvent(new Slave());
            CreateAndAddClientTreeViewItem(client);

            castvi.Clients.Add(client);

            LockWindowUpdate(IntPtr.Zero);
        }

        private void CreateAndAddClientTreeViewItem(Client client) {
            var cvi = new ClientTreeViewItem(client);
            //Used for handling collapsing and expanding.
            cvi.SetParent(largeList);
            cvi.AfterSelect += _AfterSelect;
            cvi.DoubleClicked += cvi_DoubleClicked;
            cvi.DuplicateClicked += cvi_DuplicateClicked;
            cvi.DeleteClicked += cvi_DeleteClicked;
            cvi.HostNameAndIPSet += cvi_HostNameAndIPSet;

            (largeList[0][0] as ClientsAndSlavesTreeViewItem).ChildControls.Add(cvi);

            largeList.Add(cvi);
        }

        private void CreateAndInsertClientTreeViewItem(Client client, KeyValuePair<int, int> index) {
            var cvi = new ClientTreeViewItem(client);
            //Used for handling collapsing and expanding.
            cvi.SetParent(largeList);
            cvi.AfterSelect += _AfterSelect;
            cvi.DoubleClicked += cvi_DoubleClicked;
            cvi.DuplicateClicked += cvi_DuplicateClicked;
            cvi.DeleteClicked += cvi_DeleteClicked;
            cvi.HostNameAndIPSet += cvi_HostNameAndIPSet;

            (largeList[0][0] as ClientsAndSlavesTreeViewItem).ChildControls.Add(cvi);

            largeList.Insert(cvi, index);

            cvi.SetHostNameAndIP();
        }

        private void cvi_DoubleClicked(object sender, EventArgs e) {
            if (ClientTreeViewItemDoubleClicked != null) ClientTreeViewItemDoubleClicked.Invoke(this, null);
        }

        private void cvi_DuplicateClicked(object sender, EventArgs e) {
            LockWindowUpdate(Handle);

            var cvi = sender as ClientTreeViewItem;
            if (cvi.Client.Parent != null) {
                Client clone = cvi.Client.Clone();
                var parent = cvi.Client.Parent as Clients;
                int cloneIndex = parent.IndexOf(cvi.Client) + 1;

                if (parent.Count == cloneIndex)
                    parent.AddWithoutInvokingEvent(clone);
                else
                    parent.InsertWithoutInvokingEvent(cloneIndex, clone);


                KeyValuePair<int, int> cloneIndexForLargeList =
                    largeList.ParseFlatIndex(largeList.ParseIndex(largeList.IndexOf(cvi)) + 1);
                if (cloneIndexForLargeList.Key == -1)
                    CreateAndAddClientTreeViewItem(clone);
                else
                    CreateAndInsertClientTreeViewItem(clone, cloneIndexForLargeList);

                parent.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);
            }

            LockWindowUpdate(IntPtr.Zero);
        }

        private void cvi_DeleteClicked(object sender, EventArgs e) {
            LockWindowUpdate(Handle);

            var cvi = sender as ClientTreeViewItem;
            if (cvi.Client.Parent != null)
                cvi.Client.Parent.Remove(cvi.Client);

            (largeList[0][0] as ClientsAndSlavesTreeViewItem).ChildControls.Remove(cvi);

            KeyValuePair<int, int> previousIndex = largeList.ParseFlatIndex(largeList.FlatIndexOf(cvi) - 1);
            largeList.Remove(cvi);

            largeList[previousIndex.Key][previousIndex.Value].Select();

            LockWindowUpdate(IntPtr.Zero);
        }

        private void _AfterSelect(object sender, EventArgs e) {
            LockWindowUpdate(Handle);

            foreach (ITreeViewItem item in largeList.AllControls)
                if (item != sender)
                    item.Unfocus();

            if (AfterSelect != null)
                AfterSelect(sender, null);

            LockWindowUpdate(IntPtr.Zero);
        }

        private void cvi_HostNameAndIPSet(object sender, EventArgs e) {
            if (ClientHostNameAndIPSet != null)
                ClientHostNameAndIPSet(sender, e);
        }

        public void SetGui() {
            foreach (ITreeViewItem ctrl in largeList.AllControls) {
                ctrl.SetVisibleControls();
                //To determine what add tile stress test control can be visible
                ctrl.RefreshGui();
            }
        }

        public void SetMode(DistributedTestMode distributedTestMode) {
            if (_distributedTestMode != distributedTestMode) {
                LockWindowUpdate(Handle);
                _distributedTestMode = distributedTestMode;
                foreach (ITreeViewItem item in largeList.AllControls)
                    item.SetDistributedTestMode(_distributedTestMode);
                LockWindowUpdate(IntPtr.Zero);
            }
        }

        #endregion
    }
}