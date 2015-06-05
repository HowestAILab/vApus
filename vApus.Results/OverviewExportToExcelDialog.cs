/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vApus.Results {
    public partial class OverviewExportToExcelDialog : Form {
        private ResultsHelper _resultsHelper;
        private Form _parentToClose;
        private IEnumerable<string> _databaseNames;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public OverviewExportToExcelDialog() {
            InitializeComponent();
        }

        public void Init(ResultsHelper resultsHelper, IEnumerable<string> databaseNames, Form parentToClose) {
            _parentToClose = parentToClose;
            Init(resultsHelper, databaseNames, true);
            this.VisibleChanged += OverviewExportToExcelDialog_VisibleChanged;
        }

        private void OverviewExportToExcelDialog_VisibleChanged(object sender, EventArgs e) {
            if (this.Visible) {
                this.VisibleChanged -= OverviewExportToExcelDialog_VisibleChanged;
                _parentToClose.Hide();
                _parentToClose.Close();
            }
        }
        public void Init(ResultsHelper resultsHelper, IEnumerable<string> databaseNames, bool canSwitchToRichExport = false) {
            _resultsHelper = resultsHelper;
            _databaseNames = databaseNames;

            btnRichExport.Visible = canSwitchToRichExport;

            lblDescription.Text = "Export test and monitor results per concurrency for the " + _databaseNames.Count() + " selected results database(s).";
        }
        private void btnExportToExcel_Click(object sender, EventArgs e) { Export(); }
        async private void Export() {
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                btnExportToExcel.Enabled = btnRichExport.Enabled = false;
                btnExportToExcel.Text = "Saving, can take a while...";
                bool exceptionThrown = false;

                try {
                    _cancellationTokenSource = new CancellationTokenSource();
                    await Task.Run(() => OverviewExportToExcel.Do(saveFileDialog.FileName, _databaseNames, chkIncludeFullMonitorResults.Checked, _cancellationTokenSource.Token), _cancellationTokenSource.Token);
                    exceptionThrown = true;
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed exporting overview to Excel.", ex);
                    MessageBox.Show("Failed exporting overview to Excel.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                _cancellationTokenSource = new CancellationTokenSource();

                btnExportToExcel.Text = "Export to Excel...";
                btnExportToExcel.Enabled = btnRichExport.Enabled = true;

                GC.WaitForPendingFinalizers();
                GC.Collect();

                if (!exceptionThrown) {
                    if (MessageBox.Show("Results where exported to " + saveFileDialog.FileName + ".\nDo you want to view them?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        Process.Start(saveFileDialog.FileName);
                    this.Close();
                }
            }
        }

        private void OverviewExportToExcelDialog_FormClosing(object sender, FormClosingEventArgs e) { _cancellationTokenSource.Cancel(); }

        private void btnRichExport_Click(object sender, EventArgs e) {
            if (_resultsHelper != null)
                try {
                    var dialog = new RichExportToExcelDialog();
                    dialog.Init(_resultsHelper, this);
                    dialog.ShowDialog();
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed exporting to Excel.", ex);
                    MessageBox.Show(string.Empty, "Failed exporting to Excel.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }
    }
}
