/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Gui.Properties;
using WeifenLuo.WinFormsUI.Docking;

namespace vApus.Gui {
    public partial class FirstStepsView : DockablePanel {
        private bool _formClosingEventHandlingEnabled = true;
        
        public FirstStepsView() {
            InitializeComponent();
        }

        public void DisableFormClosingEventHandling() {
            _formClosingEventHandlingEnabled = false;
        }

        private void FirstStepsView_FormClosing(object sender, FormClosingEventArgs e) {
            if (_formClosingEventHandlingEnabled) {
                //Do not show the next time if you don't want to
                Settings.Default.GreetWithWelcomePage =
                    MessageBox.Show("Would you like to hide the first steps view by default?", string.Empty,
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) ==
                    DialogResult.Cancel;

                Settings.Default.Save();
            }
        }

        private void llblContact_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("mailto:info@sizingservers.be");
        }
    }
}
