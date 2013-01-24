using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Results;
using WeifenLuo.WinFormsUI.Docking;

namespace vApus.DetailedResultsViewer {
    public partial class ResultsPanel : DockablePanel {
        public ResultsPanel() {
            InitializeComponent();
        }
        public void ClearReport() {
            detailedResultsControl.ClearResults();
        }
        public void RefreshReport() {
            detailedResultsControl.RefreshResults();
        }
    }
}
