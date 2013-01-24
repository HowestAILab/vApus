/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace vApus.Util {
    public partial class ProgressNotifierPanel : Panel {
        public ProgressNotifierPanel() {
            InitializeComponent();
        }
        private void chk_CheckedChanged(object sender, EventArgs e) {
            EnableOrDisableSavebtn();
        }
        private void txt_TextChanged(object sender, EventArgs e) {
            EnableOrDisableSavebtn();
        }
        private void txtPassword_Enter(object sender, EventArgs e) {
            if (txtPassword.Text.Length == 0)
                txtPassword.UseSystemPasswordChar = true;
        }
        private void txtPassword_Leave(object sender, EventArgs e) {
            if (txtPassword.ForeColor == Color.DimGray)
                txtPassword.UseSystemPasswordChar = false;
        }
        private void btnEnableDisable_Click(object sender, EventArgs e) {
            if (btnEnableDisable.Text == "Enable") {
                btnEnableDisable.Text = "Disable";
                grp.Enabled = true;
                EnableOrDisableSavebtn();
            } else {
                btnEnableDisable.Text = "Enable";
                grp.Enabled = btnSave.Enabled = false;
            }
        }
        private void EnableOrDisableSavebtn() {
            btnSave.Enabled = (txtEmailAddress.ForeColor != Color.DimGray &&
                Regex.IsMatch(txtEmailAddress.Text, @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase)) &&
                (txtSmtp.ForeColor != Color.DimGray && txtSmtp.Text.Trim().Length != 0) &&
                (txtPassword.ForeColor != Color.DimGray && txtPassword.Text.Trim().Length != 0) && 
                (chkAfterRun.Checked || chkAfterConcurrency.Checked || ChkWhenError.Checked || chkWhenTestFinished.Checked);
        }
        public override string ToString() {
            return "Progress Notifier";
        }
    }
}