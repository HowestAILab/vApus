/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;

namespace vApus.Util {
    public partial class LogPanel : Panel {
        public LogPanel() {
            InitializeComponent();
            this.VisibleChanged += LogPanel_VisibleChanged;
        }

        private void LogPanel_VisibleChanged(object sender, EventArgs e) {
            if (IsHandleCreated && Visible)
                fileLoggerPanel.SelectNewestLog();
        }

        public override string ToString() {
            return "Application logging";
        }
    }
}
