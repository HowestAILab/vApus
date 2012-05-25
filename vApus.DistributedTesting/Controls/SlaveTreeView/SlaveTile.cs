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
    public partial class SlaveTile : UserControl
    {
        private Slave _slave;

        public SlaveTile()
        {
            InitializeComponent();
        }

        public void SetSlave(Slave slave)
        {
            _slave = slave;

            chkUse.CheckedChanged -= chkUse_CheckedChanged;
            chkUse.Checked = _slave.Use;
            chkUse.CheckedChanged += chkUse_CheckedChanged;
        }

        private void chkUse_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
