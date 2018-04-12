/*
 * Copyright 2015 (c) Vandroemme Dieter
 * Technical University Kortrijk, department PIH
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Monitor {
    public partial class HardwareConfigurationDialog : Form {

        public HardwareConfigurationDialog(string configuration) {
            InitializeComponent();

            if (configuration.StartsWith("{\"") && configuration.EndsWith("]}")) {
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(configuration);
                configuration = string.Empty;
                foreach (var k in dic.Keys) {
                    configuration += k + "\n";
                    foreach (var v in dic[k]) configuration += "  " + v + "\n";
                    configuration += "\n";
                }
            }
            else if (configuration.StartsWith("<lines>")) { //Backwards compatible
                configuration = configuration.Replace("<lines>", "\n").Replace("</lines>", "").Replace("<line>", "").Replace("<\line>", "\n");
            }

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