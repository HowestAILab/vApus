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
    public partial class AddTileTreeViewItem : UserControl, ITestTreeViewItem
    {
        public AddTileTreeViewItem()
        {
            InitializeComponent();
        }

        private void _Click(object sender, EventArgs e)
        {

        }

        public void SetVisibleControls()
        {
        }
        public void SetVisibleControls(bool visible)
        {
            this.Visible = visible;
        }
        private void _MouseEnter(object sender, EventArgs e)
        {
            lbl.ForeColor = Color.Black;
        }

        private void _MouseLeave(object sender, EventArgs e)
        {
            lbl.ForeColor = SystemColors.ControlDarkDark;
        }
        public void RefreshLabel()
        {
        }
    }
}
