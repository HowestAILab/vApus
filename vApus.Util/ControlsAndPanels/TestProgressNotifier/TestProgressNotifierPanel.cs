/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
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
    /// <summary>
    /// By using this panel you can set the behaviour of test progress notification and to which e-mail address notifications need to be sent. Used in the OptionsDialog.
    /// </summary>
    public partial class TestProgressNotifierPanel : Panel {
        #region Constructor
        /// <summary>
        /// By using this panel you can set the behaviour of test progress notification and to which e-mail address notifications need to be sent. Used in the OptionsDialog.
        /// </summary>
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
            btnSave.Enabled = (btnEnableDisable.Text == "Disable" &&
                (Regex.IsMatch(txtEmailaddress.Text.Trim(), @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase)) &&
                (txtSmtp.ForeColor != Color.DimGray && txtSmtp.Text.Trim().Length != 0));
        }
        private void EnableOrDisableTestbtn() {
            btnTest.Enabled = (btnEnableDisable.Text == "Disable" &&
                (Regex.IsMatch(txtEmailaddress.Text.Trim(), @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase)) &&
                (txtSmtp.ForeColor != Color.DimGray && txtSmtp.Text.Trim().Length != 0));
        }
        private void EnableOrDisableClearbtn() {
            btnClear.Enabled = btnEnableDisable.Text == "Disable" && ((txtEmailaddress.ForeColor != Color.DimGray && txtEmailaddress.Text.Trim().Length != 0) ||
                (txtSmtp.ForeColor != Color.DimGray && txtSmtp.Text.Trim().Length != 0) ||
                (txtUsername.ForeColor != Color.DimGray && txtUsername.Text.Trim().Length != 0) ||
                (txtPassword.ForeColor != Color.DimGray && txtPassword.Text.Trim().Length != 0) ||
                (nudPort.Value != 0) || chkSecure.Checked || chkAfterRun.Checked || chkAfterConcurrency.Checked || chkWhenTestFinished.Checked);
        }

        private void LoadSettings() {
            txtEmailaddress.TextChanged -= txt_TextChanged;
            txtUsername.TextChanged -= txt_TextChanged;
            txtPassword.TextChanged -= txt_TextChanged;
            txtSmtp.TextChanged -= txt_TextChanged;

            chkAfterRun.CheckedChanged -= chk_CheckedChanged;
            chkAfterConcurrency.CheckedChanged -= chk_CheckedChanged;
            chkWhenTestFinished.CheckedChanged -= chk_CheckedChanged;

            txtEmailaddress.Text = Settings.Default.PNEMailAddress;
            txtUsername.Text = Settings.Default.PNUsername;
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

            txtEmailaddress.TextChanged += txt_TextChanged;
            txtUsername.TextChanged += txt_TextChanged;
            txtPassword.TextChanged += txt_TextChanged;
            txtSmtp.TextChanged += txt_TextChanged;
        }
        private bool TestSettings() {
            try {
                var client = new SmtpClient(txtSmtp.Text, (int)nudPort.Value);
                client.EnableSsl = chkSecure.Checked;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                string username = (txtUsername.ForeColor == Color.DimGray) ? string.Empty : txtUsername.Text.Trim();
                if (username == string.Empty) username = (txtEmailaddress.ForeColor == Color.DimGray) ? string.Empty : txtEmailaddress.Text.Trim();
                if (username.Length != 0 && txtPassword.Text.Trim().Length != 0) {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(username, txtPassword.Text);
                }

                //Dns.GetHostName() does not always work.
                string hostName = Dns.GetHostEntry("127.0.0.1").HostName.Trim().Split('.')[0].ToLowerInvariant();

                var msg = new MailMessage("vapus@sizingservers.be", txtEmailaddress.Text, "A test mail", string.Concat("from vApus@", hostName, ":", NamedObjectRegistrar.Get<int>("Port")));
                msg.SubjectEncoding = msg.BodyEncoding = UTF8Encoding.UTF8;
                msg.IsBodyHtml = true;
                msg.Priority = MailPriority.High;
                msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                client.Send(msg);

                return true;
            } catch {
                //Handled later on.
            }
            return false;
        }
        private void SaveSettings() {
            Settings.Default.PNEMailAddress = (txtEmailaddress.ForeColor == Color.DimGray) ? string.Empty : txtEmailaddress.Text.Trim();
            Settings.Default.PNSMTP = (txtSmtp.ForeColor == Color.DimGray) ? string.Empty : txtSmtp.Text.Trim();
            Settings.Default.PNUsername = (txtUsername.ForeColor == Color.DimGray) ? string.Empty : txtUsername.Text.Trim();
            Settings.Default.PNPassword = (txtPassword.ForeColor == Color.DimGray) ? string.Empty : txtPassword.Text.Encrypt(TestProgressNotifier.PasswordGUID, TestProgressNotifier.Salt);
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
            txtEmailaddress.TextChanged -= txt_TextChanged;
            txtUsername.TextChanged -= txt_TextChanged;
            txtPassword.TextChanged -= txt_TextChanged;
            txtSmtp.TextChanged -= txt_TextChanged;

            chkAfterRun.CheckedChanged -= chk_CheckedChanged;
            chkAfterConcurrency.CheckedChanged -= chk_CheckedChanged;
            chkWhenTestFinished.CheckedChanged -= chk_CheckedChanged;

            txtEmailaddress.Text = txtUsername.Text = txtPassword.Text = txtSmtp.Text = string.Empty;

            nudPort.Value = 0;
            chkSecure.Checked = chkAfterRun.Checked = chkAfterConcurrency.Checked = chkWhenTestFinished.Checked = false;

            EnableOrDisableTestbtn();
            EnableOrDisableClearbtn();

            SaveSettings();

            chkAfterRun.CheckedChanged += chk_CheckedChanged;
            chkAfterConcurrency.CheckedChanged += chk_CheckedChanged;
            chkWhenTestFinished.CheckedChanged += chk_CheckedChanged;

            txtEmailaddress.TextChanged += txt_TextChanged;
            txtUsername.TextChanged += txt_TextChanged;
            txtPassword.TextChanged += txt_TextChanged;
            txtSmtp.TextChanged += txt_TextChanged;
        }

        public override string ToString() { return "Test progress notifier"; }
        #endregion
    }
}