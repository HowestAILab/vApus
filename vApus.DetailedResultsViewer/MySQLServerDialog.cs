/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vApus.DetailedResultsViewer {
    public partial class MySQLServerDialog : Form {
        public bool Connected { get { return savingResultsPanel.Connected; } }
        public string ConnectionString { get { return savingResultsPanel.ConnectionString; } }
        public void GetCurrentCredentials(out string user, out string host, out int port, out string password) {
            savingResultsPanel.GetCurrentCredentials(out user, out host, out port, out password);
        }
        public MySQLServerDialog() {
            InitializeComponent();
        }
    }
}
