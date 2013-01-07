using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace vApus.Util
{
    public partial class ProgressSpammerPanel : Panel
    {
        public ProgressSpammerPanel()
        {
            InitializeComponent();
        }

        private void txtEmailAddress_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(txtEmailAddress.Text, @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase))
            {
                btnSet.Enabled = true;
                txtEmailAddress.ForeColor = Color.Black;
            }
            else
            {
                btnSet.Enabled = false;
                txtEmailAddress.ForeColor = Color.Red;
            }
        }

        private void chkEnabled_CheckedChanged(object sender, EventArgs e)
        {
            grp.Enabled = chkEnable.Enabled;
            // btnSet.Enabled = chkEnable.Checked; 
        }
        public override string ToString()
        {
            return "Progress Spammer";
        }
    }
}