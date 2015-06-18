/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;
using vApus.Results;
using WeifenLuo.WinFormsUI.Docking;

namespace vApus.DetailedResultsViewer {
    public partial class Viewer : Form {
        private SettingsPanel _settingsPanel = new SettingsPanel();
        private ResultsPanel _resultsPanel = new ResultsPanel();
        private ResultsHelper _resultsHelper = new ResultsHelper();
        public Viewer() {
            InitializeComponent();
            HandleCreated += NewViewer_HandleCreated;
        }

        private void NewViewer_HandleCreated(object sender, EventArgs e) {
            //All gui stuff is added here because this is easier to set the gui right.
            HandleCreated -= NewViewer_HandleCreated;

            _settingsPanel.ResultsHelper = _resultsHelper;
            _settingsPanel.Show(dockPanel, DockState.DockLeft);
            _settingsPanel.CloseButtonVisible = false;
            _settingsPanel.ResultsSelected += _settingsPanel_ResultsSelected;
            _settingsPanel.CancelGettingResults += _settingsPanel_CancelGettingResults;
            _settingsPanel.DisableResultsPanel += _settingsPanel_DisableResultsPanel;

            _resultsPanel.ResultsHelper = _resultsHelper;
            _resultsPanel.Enabled = false;
            _resultsPanel.Show(dockPanel);
            _resultsPanel.CloseButtonVisible = false;
            _resultsPanel.ResultsDeleted += _resultsPanel_ResultsDeleted;
            _resultsPanel.FormClosing += _resultsPanel_FormClosing;

            pnlMask.BringToFront();
        }

        private void _settingsPanel_ResultsSelected(object sender, SettingsPanel.ResultsSelectedEventArgs e) {
            if (e.Database == null) {
                _resultsPanel.ClearResults();
                _resultsPanel.Enabled = false;
            } else {
                _resultsPanel.RefreshResults(e.StressTestId);
                _resultsPanel.Enabled = true;
            }
        }
        private void _settingsPanel_CancelGettingResults(object sender, EventArgs e) {
            _resultsPanel.ClearResults();
            _resultsPanel.Enabled = false;
        }
        private void _settingsPanel_DisableResultsPanel(object sender, EventArgs e) {
            _resultsPanel.Enabled = false;
        }

        private void _resultsPanel_ResultsDeleted(object sender, EventArgs e) { _settingsPanel.RefreshDatabases(true); }

        private void _resultsPanel_FormClosing(object sender, FormClosingEventArgs e) { this.Close(); }
    }
}
