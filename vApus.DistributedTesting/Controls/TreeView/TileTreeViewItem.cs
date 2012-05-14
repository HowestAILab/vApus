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
    public partial class TileTreeViewItem : UserControl, ITestTreeViewItem
    {
        public event EventHandler CollapsedChanged;

        #region Fields
        private Tile _tile = new Tile();
        private bool _collapsed = false;

        private List<Control> _childControls = new List<Control>();

        #endregion

        public bool Collapsed
        {
            get { return _collapsed; }
            set
            {
                if (_collapsed != value)
                {
                    _collapsed = value;
                    picCollapseExpand.Image = _collapsed ?
                        global::vApus.DistributedTesting.Properties.Resources.Expand_small :
                        global::vApus.DistributedTesting.Properties.Resources.Collapse_small;

                    if (CollapsedChanged != null)
                        CollapsedChanged(this, null);
                }
            }
        }
        public List<Control> ChildControls
        {
            get { return _childControls; }
        }

        #region Constructors
        public TileTreeViewItem()
        {
            InitializeComponent();
        }
        public TileTreeViewItem(Tile tile)
            : this()
        {
            _tile = tile;
            RefreshLabel();
        }
        #endregion

        #region Functions
        private void txtTile_Enter(object sender, EventArgs e)
        {
            if (_tile.Label == string.Empty)
            {
                txtTile.ForeColor = SystemColors.WindowText;
                txtTile.Text = string.Empty;

                lblTile.Text = _tile.ToString() + " [#" + _tile.Count + "]";
            }
            else
            {
                lblTile.Text = _tile.Label + " [#" + _tile.Count + "]";
            }
        }
        private void txtTile_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && _tile.Label != txtTile.Text)
            {
                txtTile.Text = txtTile.Text.Trim();
                if (_tile.Label != txtTile.Text)
                {
                    _tile.Label = txtTile.Text;
                    lblTile.Text = (_tile.Label == string.Empty ? _tile.ToString() : _tile.Label) + " [#" + _tile.Count + "]";

                    _tile.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                }
            }
        }
        private void txtTile_Leave(object sender, EventArgs e)
        {
            txtTile.Text = lblTile.Text = txtTile.Text.Trim();
            if (_tile.Label != txtTile.Text)
            {
                _tile.Label = txtTile.Text;
                _tile.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
            if (txtTile.Text == string.Empty)
            {
                txtTile.Text = "Give this Tile a custom label.";
                txtTile.ForeColor = SystemColors.ControlDark;

                lblTile.Text = _tile.ToString() + " [#" + _tile.Count + "]";
            }
            else
            {
                lblTile.Text = _tile.Label + " [#" + _tile.Count + "]";
            }
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
            txtTile.Visible = picDuplicate.Visible = picDelete.Visible = visible;
        }

        public void SetVisibleControls()
        {
            SetVisibleControls(ClientRectangle.Contains(PointToClient(Cursor.Position)));
        }
        public void RefreshLabel()
        {
            string label = string.Empty;
            if (_tile.Label == string.Empty)
            {
               label = _tile.ToString() + " [#" + _tile.Count + "]";
            }
            else
            {
                txtTile.ForeColor = SystemColors.WindowText;
                label = _tile.Label + " [#" + _tile.Count + "]";
            }
            if (lblTile.Text != label)
                lblTile.Text = label;
        }

        private void picDuplicate_Click(object sender, EventArgs e)
        {

        }
        private void picDelete_Click(object sender, EventArgs e)
        {

        }
        private void picCollapseExpand_Click(object sender, EventArgs e)
        {
            Collapsed = !Collapsed;
        }
        #endregion
    }
}
