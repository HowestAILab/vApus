using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace vApus.Util
{
    public partial class BoolValueControl : BaseValueControl
    {
        public BoolValueControl()
        {
            InitializeComponent();
        }

        public override void SetValueAndControl(object value)
        {
            base.Value = value;

            CheckBox chk = new CheckBox();
            chk.Checked = (bool)value;
            chk.Text = "[" + (chk.Checked ? "Checked " : "Unchecked ") + "equals " + chk.Checked + "]";
            chk.Dock = DockStyle.Top;
            chk.CheckedChanged += new EventHandler(chk_CheckedChanged);
            chk.Leave += new EventHandler(chk_Leave);
            chk.KeyUp += new KeyEventHandler(chk_KeyUp);
            base.ValueControl = chk;
        }
        private void chk_CheckedChanged(object sender, EventArgs e)
        {
            base.HandleValueChanged((sender as CheckBox).Checked);
        }
        private void chk_KeyUp(object sender, KeyEventArgs e)
        {
            base.HandleKeyUp(e.KeyCode);
        }
        private void chk_Leave(object sender, EventArgs e)
        {
            base.HandleValueChanged((sender as CheckBox).Checked);
        }
    }
}
