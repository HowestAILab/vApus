/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

namespace vApus.DistributedTesting
{
    [ToolboxItem(false)]
    public partial class ClientsAndSlavesTreeViewItem : UserControl, ITreeViewItem
    {
        #region Events
        /// <summary>
        /// Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;
        public event EventHandler AddClientClicked;
        #endregion

        #region Fields
        /// <summary>
        /// Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;
        private ClientsAndSlaves _clientsAndSlaves = new ClientsAndSlaves();
        private List<ClientTreeViewItem> _childControls = new List<ClientTreeViewItem>();

        //To know when this can be enabled again.
        private int _refreshingClientCount = 0;
        #endregion

        #region Properties
        public ClientsAndSlaves ClientsAndSlaves
        {
            get { return _clientsAndSlaves; }
        }
        public List<ClientTreeViewItem> ChildControls
        {
            get { return _childControls; }
        }
        #endregion

        #region Constructor
        public ClientsAndSlavesTreeViewItem()
        {
            InitializeComponent();
        }
        public ClientsAndSlavesTreeViewItem(ClientsAndSlaves clientsAndSlaves)
            :this()
        {
            _clientsAndSlaves = clientsAndSlaves;
        }
        #endregion

        #region Functions
        private void ClientsAndSlavesTreeViewItem_Click(object sender, EventArgs e)
        {
            this.Select();
            _Enter(sender, e);
        }
        private void _Enter(object sender, EventArgs e)
        {
            if (AfterSelect != null)
                AfterSelect(this, null);
        }
        public void Unfocus()
        {
        }
        public void SetVisibleControls()
        {
        }
        public void SetVisibleControls(bool visible)
        {
        }
        public void RefreshGui()
        {
        }

        private void picAddClient_Click(object sender, EventArgs e)
        {
            this.Focus();
            if (AddClientClicked != null)
                AddClientClicked(this, null);
        }
        private void picRefresh_Click(object sender, EventArgs e)
        {
            SetHostNameAndIP();
        }
        private void _KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
                _ctrl = true;
        }
        private void _KeyUp(object sender, KeyEventArgs e)
        {
            if (_ctrl && e.KeyCode == Keys.I)
                if (AddClientClicked != null)
                    AddClientClicked(this, null);
            else if (e.KeyCode == Keys.F5)
                SetHostNameAndIP();
        }
        /// <summary>
        /// Set the host name and the ip in the gui, check if the computer is online.
        /// </summary>
        public void SetHostNameAndIP()
        {
            this.Select();

            EnableControls(false);

            _refreshingClientCount = _childControls.Count;
            foreach (var ctvi in _childControls)
            {
                ctvi.HostNameAndIPSet += new EventHandler(ctvi_HostNameAndIPSet);
                if (!ctvi.SetHostNameAndIP())
                    --_refreshingClientCount;
            }

            if (_refreshingClientCount <= 0)
                EnableControls(true);
        }

        private void ctvi_HostNameAndIPSet(object sender, EventArgs e)
        {
            var ctvi = sender as ClientTreeViewItem;
            ctvi.HostNameAndIPSet -= ctvi_HostNameAndIPSet;          

            if (--_refreshingClientCount <= 0)
            {
                EnableControls(true);

                //Only select this if non is focused.
                foreach (var control in _childControls)
                    if (control.Focused)
                        return;

                this.Select();
            }
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
