/*
 * Copyright 2013 (c) Sizing Servers Lab
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
using vApus.Gui;
using WeifenLuo.WinFormsUI.Docking;

namespace vApus.DetailedResultsViewer {
    public partial class NewViewer : Form {
        private SettingsPanel _settingsPanel = new SettingsPanel();
        private ResultsPanel _resultsPanel = new ResultsPanel();
        public NewViewer() {
            InitializeComponent();

            this.HandleCreated += NewViewer_HandleCreated;
        }

        void NewViewer_HandleCreated(object sender, EventArgs e) {
            this.HandleCreated -= NewViewer_HandleCreated;
            
            _settingsPanel.Show(dockPanel, DockState.DockLeft);
            _settingsPanel.CloseButtonVisible = false;

            _resultsPanel.Show(dockPanel);
            _resultsPanel.CloseButtonVisible = false;
            _resultsPanel.FormClosing += _resultsPanel_FormClosing;
        }

        private void _resultsPanel_FormClosing(object sender, FormClosingEventArgs e) {
            this.Close();
        }
    }
}
