/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Diagnostics;
using System.Windows.Forms;
using vApus.Gui.Properties;
using WeifenLuo.WinFormsUI.Docking;

namespace vApus.Gui {
    public partial class FirstStepsView : DockablePanel {
        public event EventHandler<LinkClickedEventArgs> LinkClicked;

        private bool _formClosingEventHandlingEnabled = true;

        public FirstStepsView() {
            InitializeComponent();

            lblCopyright.Text = "Copyright 2007-" + DateTime.Now.Year + " © Sizing Servers Lab at HoWest, the university-college of West-Flanders.";
        }

        public void DisableFormClosingEventHandling() {
            _formClosingEventHandlingEnabled = false;
        }

        private void FirstStepsView_FormClosing(object sender, FormClosingEventArgs e) {
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

        private void llbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (LinkClicked != null) {
                var llbl = sender as LinkLabel;
                int options = 0;
                if (llbl.Tag is int) options = (int)llbl.Tag;
                else int.TryParse(llbl.Tag.ToString(), out options);

                LinkClicked(this, new LinkClickedEventArgs(options));
            }
        }

        public class LinkClickedEventArgs : EventArgs {
            public int OptionsIndex { get; private set; }

            public LinkClickedEventArgs(int optionsIndex) {
                OptionsIndex = optionsIndex;
            }
        }
    }
}
