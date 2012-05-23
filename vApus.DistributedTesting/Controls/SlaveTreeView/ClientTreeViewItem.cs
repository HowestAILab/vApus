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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.DistributedTesting
{
    [ToolboxItem(false)]
    public partial class ClientTreeViewItem : UserControl, ITreeViewItem
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        #region Events
        /// <summary>
        /// Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;
        public event EventHandler DuplicateClicked;
        public event EventHandler DeleteClicked;
        #endregion

        #region Fields
        private Client _client = new Client();
        /// <summary>
        /// Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;
        #endregion

        #region Properties
        public Client Client
        {
            get { return _client; }
        }
        private int UsedSlaveCount
        {
            get
            {
                int count = 0;
                foreach (Slave slave in _client)
                    if (slave.Use)
                        ++count;
                return count;
            }
        }
        #endregion

        #region Constructors
        public ClientTreeViewItem()
        {
            InitializeComponent();
        }
        public ClientTreeViewItem(Client client)
            : this()
        {
            _client = client;
            RefreshGui();

            chk.CheckedChanged -= chk_CheckedChanged;
            chk.Checked = _client.Use;
            chk.CheckedChanged += chk_CheckedChanged;

            //To check if the use has changed of the child controls.
            SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);
        }
        #endregion

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            //To set if the client is used or not.
            if (sender is Slave)
            {
                Slave slave = sender as Slave;
                if (_client.Contains(slave))
                {
                    _client.Use = false;
                    foreach (Slave sl in _client)
                        if (sl.Use)
                        {
                            _client.Use = true;
                            break;
                        }
                    if (chk.Checked != _client.Use)
                    {
                        chk.CheckedChanged -= chk_CheckedChanged;
                        chk.Checked = _client.Use;
                        chk.CheckedChanged += chk_CheckedChanged;
                    }
                }
            }
        }
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
        }
        private void txtClient_Leave(object sender, EventArgs e)
        {
            txtClient.Text = lblTile.Text = txtClient.Text.Trim();
#warning fix this
            //if (_client.Label != txtClient.Text)
            //{
            //    _client.Label = txtClient.Text;
            //    _client.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            //}
            lblTile.Text = _client.ToString() + " (#" + UsedSlaveCount + "/" + _client.Count + ")";
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
            txtClient.Visible = picDuplicate.Visible = picDelete.Visible = visible;
        }
        public void SetVisibleControls()
        {
            if (this.BackColor == SystemColors.Control)
                SetVisibleControls(true);
            else
                SetVisibleControls(ClientRectangle.Contains(PointToClient(Cursor.Position)));
        }

        public void RefreshGui()
        {
            string label = string.Empty;
#warning fix this
            //if (_client.Label == string.Empty)
            //    label = _client.ToString() + " (#" + UsedSlaveCount + "/" + _client.Count + ")";
            //else
            //    label = _client.Label + " (#" + UsedSlaveCount + "/" + _client.Count + ")";

            //if (lblTile.Text != label)
            //{
            //    lblTile.Text = label;
            //    txtClient.Text = (_client.Label == string.Empty) ? _client.ToString() : _client.Label;
            //}

            _client.Use = UsedSlaveCount != 0;
            if (_client.Use != chk.Checked)
            {
                chk.CheckedChanged -= chk_CheckedChanged;
                chk.Checked = _client.Use;
                chk.CheckedChanged += chk_CheckedChanged;
            }
        }
        private void _KeyUp(object sender, KeyEventArgs e)
        {
#warning fix this
            //if (sender == txtClient)
            //    if (e.KeyCode == Keys.Enter && _client.Label != txtClient.Text)
            //    {
            //        txtClient.Text = txtClient.Text.Trim();
            //        if (_client.Label != txtClient.Text)
            //        {
            //            _client.Label = txtClient.Text;
            //            lblTile.Text = (_client.Label == string.Empty ? _client.ToString() : _client.Label) + " (#" + _client.Count + ")";

            //            _client.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            //        }
            //    }


            if (e.KeyCode == Keys.ControlKey)
                _ctrl = false;
            else if (_ctrl)
                if (e.KeyCode == Keys.R && DeleteClicked != null)
                    DeleteClicked(this, null);
                else if (e.KeyCode == Keys.D && DuplicateClicked != null)
                    DuplicateClicked(this, null);
                else if (e.KeyCode == Keys.U)
                    chk.Checked = !chk.Checked;
        }
        private void _KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
                _ctrl = true;
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
        private void chk_CheckedChanged(object sender, EventArgs e)
        {
            if (_client.Use != chk.Checked)
            {
                _client.Use = chk.Checked;
                foreach (TileStresstest ts in _client)
                    ts.Use = _client.Use;

                _client.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
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
