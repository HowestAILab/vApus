/*
 * Copyright 2015 (c) Vandroemme Dieter
 * Technical University Kortrijk, department PIH
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using RandomUtils.Log;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using vApus.Util;

namespace vApus.Monitor {
    public partial class HardwareConfigurationDialog : Form {

        public HardwareConfigurationDialog(string configuration) {
            InitializeComponent();
            rtxt.Text = configuration.Trim();

            rtxt.DefaultContextMenu(true);
        }

        private void btnSave_Click(object sender, EventArgs e) {
            if (sfd.ShowDialog() == DialogResult.OK)
                using (var sw = new StreamWriter(sfd.FileName)) {
                    sw.Write(rtxt.Text);
                    sw.Flush();
                }
        }
    }
}