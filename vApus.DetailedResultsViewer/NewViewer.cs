/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace vApus.DetailedResultsViewer {
    public partial class NewViewer : Form {
        private SettingsPanel _settingsPanel = new SettingsPanel();
        private ResultsPanel _resultsPanel = new ResultsPanel();
        public NewViewer() {
            InitializeComponent();
            this.Shown += NewViewer_Shown;
        }

        private void NewViewer_Shown(object sender, EventArgs e) {
            this.Shown -= NewViewer_Shown;

            _settingsPanel.Show(dockPanel, DockState.DockLeft);
            _settingsPanel.CloseButtonVisible = false;
            _settingsPanel.ResultsSelected += _settingsPanel_ResultsSelected;

            _resultsPanel.Show(dockPanel);
            _resultsPanel.CloseButtonVisible = false;
            _resultsPanel.FormClosing += _resultsPanel_FormClosing;

            pnlMask.BringToFront();
        }

        private void _settingsPanel_ResultsSelected(object sender, SettingsPanel.ResultsSelectedEventArgs e) {
            if (e.Database == null) _resultsPanel.ClearReport(); else _resultsPanel.RefreshReport();
        }

        private void _resultsPanel_FormClosing(object sender, FormClosingEventArgs e) {
            this.Close();
        }
    }
}
