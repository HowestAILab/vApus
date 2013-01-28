/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using vApus.Results;
using WeifenLuo.WinFormsUI.Docking;

namespace vApus.DetailedResultsViewer {
    public partial class ResultsPanel : DockablePanel {
        private ResultsHelper _resultsHelper;

        public ResultsHelper ResultsHelper {
            get { return _resultsHelper; }
            set { _resultsHelper = value; }
        }
        /// <summary>
        /// Don't forget to set ResultsHelper.
        /// </summary>
        public ResultsPanel() {
            InitializeComponent();
        }
        public void ClearReport() {
            detailedResultsControl.ClearResults();
        }
        public void RefreshReport() {
            detailedResultsControl.RefreshResults(_resultsHelper);
        }
    }
}
