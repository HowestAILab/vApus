using RandomUtils.Log;
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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vApus.Results {
    public partial class OverviewExportToExcelDialog : Form {
        private ResultsHelper _resultsHelper;
        private IEnumerable<string> _databaseNames;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public OverviewExportToExcelDialog() {
            InitializeComponent();
        }
        public void Init(ResultsHelper resultsHelper, IEnumerable<string> databaseNames) {
            _resultsHelper = resultsHelper;
            _databaseNames = databaseNames;
        }
        private void btnExportToExcel_Click(object sender, EventArgs e) { Export(); }
        async private void Export() {
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                btnExportToExcel.Enabled = false;
                btnExportToExcel.Text = "Saving, can take a while...";

                try {
                    _cancellationTokenSource = new CancellationTokenSource();
                    await Task.Run(() => OverviewExportToExcel.Do(saveFileDialog.FileName, _databaseNames, chkIncludeFullMonitorResults.Checked, _cancellationTokenSource.Token), _cancellationTokenSource.Token);
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed exporting overview to Excel.", ex);
                    MessageBox.Show(string.Empty, "Failed exporting overview to Excel.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                _cancellationTokenSource = new CancellationTokenSource();

                btnExportToExcel.Text = "Export to Excel...";
                btnExportToExcel.Enabled = true;

                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void OverviewExportToExcelDialog_FormClosing(object sender, FormClosingEventArgs e) { _cancellationTokenSource.Cancel(); }
    }
}
