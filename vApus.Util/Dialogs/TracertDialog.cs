/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;

namespace vApus.Util {
    public partial class TracertDialog : Form {
        public event EventHandler CancelTraceRoute;

        public TracertDialog() { InitializeComponent(); }

        public void SetStarted() {
            btnCancelTraceRoute.Enabled = true;
            lvw.Items.Clear();
        }

        /// <summary>
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="hostName"></param>
        /// <param name="roundtripTime">Already formatted</param>
        public void AddHop(string ip, string hostName, string roundtripTime) {
            btnCancelTraceRoute.Enabled = true;
            lvw.Items.Add(new ListViewItem(new[] { (lvw.Items.Count + 1).ToString(), ip, hostName, roundtripTime }));
        }

        public void SetCompleted() { btnCancelTraceRoute.Enabled = false; }

        private void btnCancelTraceRoute_Click(object sender, EventArgs e) {
            btnCancelTraceRoute.Enabled = false;
            if (CancelTraceRoute != null)
                CancelTraceRoute(this, null);
        }
    }
}