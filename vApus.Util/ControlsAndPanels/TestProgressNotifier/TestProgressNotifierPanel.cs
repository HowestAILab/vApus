/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using vApus.Util.Properties;

namespace vApus.Util {
    public partial class TestProgressNotifierPanel : Panel {

        #region Constructor
        public TestProgressNotifierPanel() {
            InitializeComponent();
            LoadSettings();
        }
        #endregion

        #region Functions
        private void txt_TextChanged(object sender, EventArgs e) {
            EnableOrDisableSavebtn();
            EnableOrDisableTestbtn();
            EnableOrDisableClearbtn();
        }
        private void chk_CheckedChanged(object sender, EventArgs e) {
            EnableOrDisableSavebtn();
            EnableOrDisableClearbtn();
        }
        private void chkAfterConcurrency_CheckedChanged(object sender, EventArgs e) {
            EnableOrDisableSavebtn();
            EnableOrDisableClearbtn();

            if (chkAfterConcurrency.Checked) {
                chkAfterRun.CheckedChanged -= chkAfterRun_CheckedChanged;
                chkAfterRun.Checked = false;
                chkAfterRun.CheckedChanged += chkAfterRun_CheckedChanged;
            }
        }
        private void chkAfterRun_CheckedChanged(object sender, EventArgs e) {
            EnableOrDisableSavebtn();
            EnableOrDisableClearbtn();

            if (chkAfterRun.Checked) {
                chkAfterConcurrency.CheckedChanged -= chkAfterConcurrency_CheckedChanged;
                chkAfterConcurrency.Checked = false;
                chkAfterConcurrency.CheckedChanged += chkAfterConcurrency_CheckedChanged;
            }
        }

        private void nudPort_ValueChanged(object sender, EventArgs e) {
            EnableOrDisableSavebtn();
            EnableOrDisableClearbtn();
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
            } else {
                btnEnableDisable.Text = "Enable";
                grp.Enabled = false;
            }
            EnableOrDisableTestbtn();
            EnableOrDisableClearbtn();
            SaveSettings();
        }
        private void btnTest_Click(object sender, EventArgs e) {
            if (TestSettings()) MessageBox.Show("Test mail sent.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show("Failed sending the mail!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void btnSave_Click(object sender, EventArgs e) { SaveSettings(); }
        private void btnClear_Click(object sender, EventArgs e) { ClearSettings(); }

        private void EnableOrDisableSavebtn() {
            btnSave.Enabled = (btnEnableDisable.Text == "Disable" && (txtEmailAddress.ForeColor != Color.DimGray &&
                Regex.IsMatch(txtEmailAddress.Text.Trim(), @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase)) &&
                (txtSmtp.ForeColor != Color.DimGray && txtSmtp.Text.Trim().Length != 0) &&
                (txtPassword.ForeColor != Color.DimGray && txtPassword.Text.Trim().Length != 0));
        }
        private void EnableOrDisableTestbtn() {
            btnTest.Enabled = (btnEnableDisable.Text == "Disable" && (txtEmailAddress.ForeColor != Color.DimGray &&
                Regex.IsMatch(txtEmailAddress.Text.Trim(), @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase)) &&
                (txtSmtp.ForeColor != Color.DimGray && txtSmtp.Text.Trim().Length != 0) &&
                (txtPassword.ForeColor != Color.DimGray && txtPassword.Text.Trim().Length != 0));
        }
        private void EnableOrDisableClearbtn() {
            btnClear.Enabled = btnEnableDisable.Text == "Disable" && ((txtEmailAddress.ForeColor != Color.DimGray && txtEmailAddress.Text.Trim().Length != 0) ||
                (txtSmtp.ForeColor != Color.DimGray && txtSmtp.Text.Trim().Length != 0) ||
                (txtPassword.ForeColor != Color.DimGray && txtPassword.Text.Trim().Length != 0) ||
                (nudPort.Value != 0) || chkSecure.Checked || chkAfterRun.Checked || chkAfterConcurrency.Checked || chkWhenTestFinished.Checked);
        }

        private void LoadSettings() {
            txtEmailAddress.TextChanged -= txt_TextChanged;
            txtPassword.TextChanged -= txt_TextChanged;
            txtSmtp.TextChanged -= txt_TextChanged;

            chkAfterRun.CheckedChanged -= chk_CheckedChanged;
            chkAfterConcurrency.CheckedChanged -= chk_CheckedChanged;
            chkWhenTestFinished.CheckedChanged -= chk_CheckedChanged;

            txtEmailAddress.Text = Settings.Default.PNEMailAddress;
            txtPassword.Text = Settings.Default.PNPassword.Decrypt(TestProgressNotifier.PasswordGUID, TestProgressNotifier.Salt);
            txtSmtp.Text = Settings.Default.PNSMTP;
            nudPort.Value = Settings.Default.PNPort;
            chkSecure.Checked = Settings.Default.PNSecure;
            chkAfterRun.Checked = Settings.Default.PNAfterEachRun;
            chkAfterConcurrency.Checked = Settings.Default.PNAfterEachConcurrency;
            chkWhenTestFinished.Checked = Settings.Default.PNWhenTheTestIsFinished;

            if (Settings.Default.PNEnabled) {
                btnEnableDisable.Text = "Disable";
                grp.Enabled = true;
            } else {
                btnEnableDisable.Text = "Enable";
                grp.Enabled = false;
            }

            EnableOrDisableTestbtn();
            EnableOrDisableClearbtn();

            chkAfterRun.CheckedChanged += chk_CheckedChanged;
            chkAfterConcurrency.CheckedChanged += chk_CheckedChanged;
            chkWhenTestFinished.CheckedChanged += chk_CheckedChanged;

            txtEmailAddress.TextChanged += txt_TextChanged;
            txtPassword.TextChanged += txt_TextChanged;
            txtSmtp.TextChanged += txt_TextChanged;

        }
        private bool TestSettings() {
            try {
                var client = new SmtpClient(txtSmtp.Text, (int)nudPort.Value);
                client.EnableSsl = chkSecure.Checked;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(txtEmailAddress.Text, txtPassword.Text);

                var msg = new MailMessage("info@sizingservers.be", txtEmailAddress.Text, "A test mail", "from vApus@" + NamedObjectRegistrar.Get<string>("IP") + ":" + NamedObjectRegistrar.Get<int>("Port"));
                msg.SubjectEncoding = msg.BodyEncoding = UTF8Encoding.UTF8;
                msg.IsBodyHtml = true;
                msg.Priority = MailPriority.High;
                msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                client.Send(msg);

                return true;
            } catch { }
            return false;
        }
        private void SaveSettings() {
            Settings.Default.PNEMailAddress = txtEmailAddress.Text.Trim();
            Settings.Default.PNPassword = txtPassword.Text.Encrypt(TestProgressNotifier.PasswordGUID, TestProgressNotifier.Salt);
            Settings.Default.PNSMTP = txtSmtp.Text.Trim();
            Settings.Default.PNPort = (int)nudPort.Value;
            Settings.Default.PNSecure = chkSecure.Checked;
            Settings.Default.PNAfterEachRun = chkAfterRun.Checked;
            Settings.Default.PNAfterEachConcurrency = chkAfterConcurrency.Checked;
            Settings.Default.PNWhenTheTestIsFinished = chkWhenTestFinished.Checked;

            Settings.Default.PNEnabled = btnEnableDisable.Text == "Disable";

            Settings.Default.Save();
            btnSave.Enabled = false;
        }
        private void ClearSettings() {
            txtEmailAddress.TextChanged -= txt_TextChanged;
            txtPassword.TextChanged -= txt_TextChanged;
            txtSmtp.TextChanged -= txt_TextChanged;

            chkAfterRun.CheckedChanged -= chk_CheckedChanged;
            chkAfterConcurrency.CheckedChanged -= chk_CheckedChanged;
            chkWhenTestFinished.CheckedChanged -= chk_CheckedChanged;

            txtEmailAddress.Text = txtPassword.Text = txtSmtp.Text = string.Empty;
            nudPort.Value = 0;
            chkSecure.Checked = chkAfterRun.Checked = chkAfterConcurrency.Checked = chkWhenTestFinished.Checked = false;

            EnableOrDisableTestbtn();
            EnableOrDisableClearbtn();

            SaveSettings();

            chkAfterRun.CheckedChanged += chk_CheckedChanged;
            chkAfterConcurrency.CheckedChanged += chk_CheckedChanged;
            chkWhenTestFinished.CheckedChanged += chk_CheckedChanged;

            txtEmailAddress.TextChanged += txt_TextChanged;
            txtPassword.TextChanged += txt_TextChanged;
            txtSmtp.TextChanged += txt_TextChanged;
        }

        public override string ToString() { return "Test Progress Notifier"; }
        #endregion
    }
}