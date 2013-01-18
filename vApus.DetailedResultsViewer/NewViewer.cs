using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace vApus.DetailedResultsViewer {
    public partial class NewViewer : Form {
        private SettingsPanel _settingsPanel = new SettingsPanel();
        public NewViewer() {
            InitializeComponent();

            _settingsPanel.Show(dockPanel, DockState.DockLeft);
            _settingsPanel.CloseButtonVisible = false;
        }
    }
}
