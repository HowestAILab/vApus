using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.DistributedTesting
{
    [ToolboxItem(false)]
    public partial class TileStresstestTreeViewItem : UserControl, ITestTreeViewItem
    {
        #region Events
        public event EventHandler AfterSelect;
        public event EventHandler DuplicateClicked;
        public event EventHandler DeleteClicked;
        #endregion

        #region Fields
        private TileStresstest _tileStresstest = new TileStresstest();

        /// <summary>
        /// Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;
        #endregion

        #region Properties
        public TileStresstest TileStresstest
        {
            get { return _tileStresstest; }
        }
        #endregion

        #region Constructors
        public TileStresstestTreeViewItem()
        {
            InitializeComponent();
        }

        public TileStresstestTreeViewItem(TileStresstest tileStresstest)
            : this()
        {
            _tileStresstest = tileStresstest;
            RefreshGui();

            chk.CheckedChanged -= chk_CheckedChanged;
            chk.Checked = _tileStresstest.Use;
            chk.CheckedChanged += chk_CheckedChanged;

            //Use if the parent is used explicitely.
            SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);
        }
        #endregion

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            if (sender == _tileStresstest.Parent)
            {
                Tile parent = sender as Tile;
                _tileStresstest.Use = parent.Use;
                if (chk.Checked != _tileStresstest.Use)
                {
                    chk.CheckedChanged -= chk_CheckedChanged;
                    chk.Checked = _tileStresstest.Use;
                    chk.CheckedChanged += chk_CheckedChanged;
                }
            }
        }
        private void _Enter(object sender, EventArgs e)
        {
            this.BackColor = SystemColors.Control;
            SetVisibleControls(true);

            if (AfterSelect != null)
                AfterSelect(this, null);
        }
        private void _Leave(object sender, EventArgs e)
        {
            if (sender == txtTileStresstest)
            {
                txtTileStresstest.Text = lblTileStresstest.Text = txtTileStresstest.Text.Trim();
                if (_tileStresstest.Label != txtTileStresstest.Text)
                {
                    _tileStresstest.Label = txtTileStresstest.Text;
                    _tileStresstest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                }
                lblTileStresstest.Text = _tileStresstest.Label;
            }
            this.BackColor = Color.Transparent;
        }
        private void _MouseEnter(object sender, EventArgs e)
        {
            SetVisibleControls(true);
        }
        private void _MouseLeave(object sender, EventArgs e)
        {
            if (!ClientRectangle.Contains(PointToClient(Cursor.Position)))
                SetVisibleControls(false);
        }
        public void SetVisibleControls(bool visible)
        {
            txtTileStresstest.Visible = picDuplicate.Visible = picDelete.Visible = visible;
        }

        public void SetVisibleControls()
        {
            if (this.Focused || txtTileStresstest.Focused)
                SetVisibleControls(true);
            else
                SetVisibleControls(ClientRectangle.Contains(PointToClient(Cursor.Position)));
        }

        public void RefreshGui()
        {
            string label = string.Empty;
            if (_tileStresstest.Label == string.Empty)
                label = (_tileStresstest.Connection == null || _tileStresstest.Connection.IsEmpty) ?
                    string.Empty : _tileStresstest.Connection.ToString();
            else
                label = _tileStresstest.Label;

            if (lblTileStresstest.Text != label)
            {
                lblTileStresstest.Text = label;
                if (!txtTileStresstest.Focused)
                    txtTileStresstest.Text = label;
            }
        }
        private void _KeyUp(object sender, KeyEventArgs e)
        {
            if (sender == txtTileStresstest)
                if (e.KeyCode == Keys.Enter && _tileStresstest.Label != txtTileStresstest.Text)
                {
                    txtTileStresstest.Text = txtTileStresstest.Text.Trim();
                    if (_tileStresstest.Label != txtTileStresstest.Text)
                    {
                        _tileStresstest.Label = txtTileStresstest.Text;
                        if (_tileStresstest.Label == string.Empty)
                            lblTileStresstest.Text = (_tileStresstest.Connection == null || _tileStresstest.Connection.IsEmpty) ? string.Empty : _tileStresstest.Connection.ToString();
                        else
                            lblTileStresstest.Text = _tileStresstest.Label;

                        _tileStresstest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                    }
                }

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
            _tileStresstest.Use = chk.Checked;
            _tileStresstest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        #endregion
    }
}
