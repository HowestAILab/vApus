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
        #region Fields
        private TileStresstest _tileStresstest = new TileStresstest();
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
            RefreshLabel();
        }
        #endregion

        #region Functions
        private void txtTileStresstest_KeyUp(object sender, KeyEventArgs e)
        {
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
        }
        private void _Enter(object sender, EventArgs e)
        {
            if (sender == txtTileStresstest )
            {
                txtTileStresstest.ForeColor = SystemColors.WindowText;
                txtTileStresstest.Text = string.Empty;

                lblTileStresstest.Text = (_tileStresstest.Connection == null || _tileStresstest.Connection.IsEmpty) ?
                    string.Empty : _tileStresstest.Connection.ToString();
            }
            else
            {
                lblTileStresstest.Text = _tileStresstest.Label;
            }
            this.BackColor = SystemColors.Control;
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
                if (txtTileStresstest.Text == string.Empty)
                {
                    txtTileStresstest.Text = "Give this Tile Stresstest custom a label.";
                    txtTileStresstest.ForeColor = SystemColors.ControlDark;

                    lblTileStresstest.Text = (_tileStresstest.Connection == null || _tileStresstest.Connection.IsEmpty) ? string.Empty : _tileStresstest.Connection.ToString();
                }
                else 
                {
                    lblTileStresstest.Text = _tileStresstest.Label;
                }
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
            SetVisibleControls(ClientRectangle.Contains(PointToClient(Cursor.Position)));
        }

        public void RefreshLabel()
        {
            string label = string.Empty;
            if (_tileStresstest.Label == string.Empty)
            {
                label = (_tileStresstest.Connection == null || _tileStresstest.Connection.IsEmpty) ?
                    string.Empty : _tileStresstest.Connection.ToString();
            }
            else
            {
                txtTileStresstest.ForeColor = SystemColors.WindowText;
                label = _tileStresstest.Label;
            }
            if (txtTileStresstest.Text != label)
                txtTileStresstest.Text = label;
        }

        private void picDuplicate_Click(object sender, EventArgs e)
        {
        }
        private void picDelete_Click(object sender, EventArgs e)
        {

        }
        private void chk_CheckedChanged(object sender, EventArgs e)
        {
            _tileStresstest.Use = chk.Checked;
        }
        #endregion

    }
}
