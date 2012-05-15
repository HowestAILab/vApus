using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace vApus.DistributedTesting
{
    [ToolboxItem(false)]
    public partial class AddTileStresstestTreeViewItem : UserControl, ITestTreeViewItem
    {
        public event EventHandler AddClick;

        public AddTileStresstestTreeViewItem()
        {
            InitializeComponent();
        }
        private void _Click(object sender, EventArgs e)
        {
            if (AddClick != null)
                AddClick(this, null);
        }
        private void _KeyUp(object sender, KeyEventArgs e)
        {
            if (AddClick != null)
                AddClick(this, null);
        }
        public void SetVisibleControls()
        {
        }
        public void SetVisibleControls(bool visible)
        {
            if (this.Visible != visible)
                this.Visible = visible;
            //if (picAdd.Visible != visible)
            //    picAdd.Visible = lbl.Visible = visible;
        }

        private void _MouseEnter(object sender, EventArgs e)
        {
            lbl.ForeColor = Color.Black;
        }

        private void _MouseLeave(object sender, EventArgs e)
        {
            lbl.ForeColor = SystemColors.ControlDark;
        }
        public void RefreshGui()
        {
        }
    }
}
