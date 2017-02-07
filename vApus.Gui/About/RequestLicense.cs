using IntelliLock.Licensing;
using RandomUtils.Log;
using System;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Gui {
    public partial class RequestLicense : Form {
        public RequestLicense() {
            InitializeComponent();

            ctxtFirstName.Text = Properties.Settings.Default.FirstName;
            ctxtLastName.Text = Properties.Settings.Default.LastName;
            ctxtCompany.Text = Properties.Settings.Default.Company;
            ctxtPhoneNumber.Text = Properties.Settings.Default.PhoneNumber;
            ctxtEmailAddress.Text = Properties.Settings.Default.EmailAddress;
            ctxtComments.Text = Properties.Settings.Default.Comments;

            ValidateForm();
        }

        private void btnSendRequest_Click(object sender, EventArgs e) {
            try {
                string body = "First name: " + ctxtFirstName.Text +
                    "\nLast name: " + ctxtLastName.Text +
                    "\nCompany: " + ctxtCompany.Text +
                    "\nPhone number:" + ctxtPhoneNumber.Text +
                    "\nE-mail address: " + ctxtEmailAddress.Text +
                    "\nComments: " + ctxtComments.Text;

                var msg = new MailMessage(ctxtEmailAddress.Text, "info@sizingservers.be", "vApus License Request", body);
                msg.SubjectEncoding = msg.BodyEncoding = System.Text.Encoding.UTF8;
                msg.Priority = MailPriority.High;
                msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                var client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;


                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("licenseactivator.sizingservers@gmail.com", "BunGk4kW!T%GplCDc1U2TUQK&y4mJ2nW&%0E*IsFR!Z%aHWWBOo1CSh*h1vA@2@R7qkNmWWZgTDO#OO60uBM4eixyVoavJ2u@hc!");

                client.Send(msg);

                MessageBox.Show("Request sent!", "", MessageBoxButtons.OK);
            }
            catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed sending request.", ex);
                MessageBox.Show("Failed sending request. Please e-mail info@sizingservers.be directly.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CheckTextbox(CueTextBox ctxt, bool extraCheck = true) {
            ctxt.Text = ctxt.Text.Trim();

            bool check = (ctxt.Text.Length != 0) && extraCheck;

            ctxt.BackColor = check ? Color.LightGreen : Color.MistyRose;

            return check;
        }

        private void ctxt_Leave(object sender, EventArgs e) { ValidateForm(); }

        private void ValidateForm() {
            CheckTextbox(ctxtFirstName);
            CheckTextbox(ctxtLastName);
            ctxtCompany.Text = ctxtCompany.Text.Trim();
            CheckTextbox(ctxtPhoneNumber,
                Regex.IsMatch(ctxtPhoneNumber.Text, @"^(?:(?:\(?(?:00|\+)([1-4]\d\d|[1-9]\d?)\)?)?[\-\.\ \\\/]?)?((?:\(?\d{1,}\)?[\-\.\ \\\/]?){0,})(?:[\-\.\ \\\/]?(?:#|ext\.?|extension|x)[\-\.\ \\\/]?(\d+))?$", RegexOptions.Singleline));

            ctxtEmailAddress.Text = ctxtEmailAddress.Text.ToLower();
            bool validMailAddress = true;
            try {
                var address = new MailAddress(ctxtEmailAddress.Text);
            }
            catch {
                validMailAddress = false;
            }
            CheckTextbox(ctxtEmailAddress, validMailAddress);
            ctxtComments.Text = ctxtComments.Text.Trim();

            btnSendRequest.Enabled = (ctxtFirstName.BackColor == Color.LightGreen &&
                ctxtLastName.BackColor == Color.LightGreen &&
                ctxtPhoneNumber.BackColor == Color.LightGreen &&
                ctxtEmailAddress.BackColor == Color.LightGreen);


            Properties.Settings.Default.FirstName = ctxtFirstName.Text;
            Properties.Settings.Default.LastName = ctxtLastName.Text;
            Properties.Settings.Default.Company = ctxtCompany.Text;
            Properties.Settings.Default.PhoneNumber = ctxtPhoneNumber.Text;
            Properties.Settings.Default.EmailAddress = ctxtEmailAddress.Text;
            Properties.Settings.Default.Comments = ctxtComments.Text;

            Properties.Settings.Default.Save();
        }
    }
}
