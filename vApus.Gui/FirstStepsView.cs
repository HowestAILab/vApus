/*
 * 2015 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using RandomUtils.Log;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using vApus.Gui.Properties;
using WeifenLuo.WinFormsUI.Docking;

namespace vApus.Gui {
    public partial class FirstStepsView : DockablePanel {
        public event EventHandler<LinkClickedEventArgs> LinkClicked;

        private bool _formClosingEventHandlingEnabled = true, _cancelFormClosing = false;

        public FirstStepsView() {
            InitializeComponent();
        }

        public void DisableFormClosingEventHandling() {
            _formClosingEventHandlingEnabled = false;
        }
        public void CancelFormClosing() {
            _cancelFormClosing = true;
            base.CancelHide();
        }

        private void FirstStepsView_FormClosing(object sender, FormClosingEventArgs e) {
            if (_cancelFormClosing) {
                e.Cancel = true;
                _cancelFormClosing = false;
                return;
            }
            if (_formClosingEventHandlingEnabled) {
                //Do not show the next time if you don't want to
                Settings.Default.GreetWithFirstStepsView =
                    MessageBox.Show("Would you like to hide the first steps view by default?", string.Empty,
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) ==
                    DialogResult.Cancel;

                Settings.Default.Save();
            }
        }

        private void llblContact_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("mailto:info@sizingservers.be");
        }

        private void llblHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                Process.Start(Path.Combine(Application.StartupPath, "Help\\Help.htm"));
            }
            catch {
                Loggers.Log(Level.Error, "Help file not found.");
            }
        }

        private void llbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (LinkClicked != null) {
                var llbl = sender as LinkLabel;
                int options = 0;
                if (llbl.Tag is int) options = (int)llbl.Tag;
                else int.TryParse(llbl.Tag.ToString(), out options);

                LinkClicked(this, new LinkClickedEventArgs(options));
            }
        }

        private void picNMCT_Click(object sender, EventArgs e) {
            Process.Start("https://www.nmct.be");
        }

        private void picSSL_Click(object sender, EventArgs e) {
            Process.Start("https://www.sizingservers.be");
        }

        private void picHowest_Click(object sender, EventArgs e) {
            Process.Start("https://www.howest.be/en");
        }

        public class LinkClickedEventArgs : EventArgs {
            public int OptionsIndex { get; private set; }

            public LinkClickedEventArgs(int optionsIndex) {
                OptionsIndex = optionsIndex;
            }
        }
    }
}
