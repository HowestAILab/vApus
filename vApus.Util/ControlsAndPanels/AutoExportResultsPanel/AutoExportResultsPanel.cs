/*
 * 2015 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    /// Used in vApus.Util.OptionsDialog.
    /// </summary>
    public partial class AutoExportResultsPanel : Panel {

        #region Constructor
        /// <summary>
        /// Used in vApus.Util.OptionsDialog.
        /// </summary>
        public AutoExportResultsPanel() {
            InitializeComponent();
            if (IsHandleCreated) SetGui();
            else HandleCreated += ExportingResultsPanel_HandleCreated;

        }
        #endregion

        #region Functions
        private void ExportingResultsPanel_HandleCreated(object sender, EventArgs e) {
            HandleCreated -= ExportingResultsPanel_HandleCreated;
            SetGui();
        }

        private void SetGui() {
            if (AutoExportResultsManager.Enabled) btnEnableDisable.PerformClick();
            txtFolder.Text = AutoExportResultsManager.Folder;
        }
        private void btnBrowse_Click(object sender, EventArgs e) {
            if (fbd.ShowDialog() == DialogResult.OK)
                txtFolder.Text = fbd.SelectedPath;
        }

        private void txtFolder_TextChanged(object sender, EventArgs e) {
            AutoExportResultsManager.Folder = txtFolder.Text;
        }

        private void btnEnableDisable_Click(object sender, EventArgs e) {
            if (btnEnableDisable.Text == "Disable") {
                btnEnableDisable.Text = "Enable";
                grp.Enabled = AutoExportResultsManager.Enabled = false;
                txtFolder.BackColor = SystemColors.Control;
            } else {
                btnEnableDisable.Text = "Disable";
                grp.Enabled = AutoExportResultsManager.Enabled = true;
                txtFolder.BackColor = Color.White;
            }
        }

        public override string ToString() {
            return "Auto-export test results";
        }

        #endregion
    }
}