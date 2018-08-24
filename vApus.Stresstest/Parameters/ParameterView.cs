/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    /// <summary>
    /// A view with auto generated gui for the different parameter flavors.
    /// </summary>
    public partial class ParameterView : BaseSolutionComponentView {

        #region Fields
        private CustomListGeneratorDialog _customListGeneratorDialog;
        private BaseParameter _parameter;
        #endregion

        #region Constructors
        /// <summary>
        /// Design time constructor.
        /// </summary>
        public ParameterView() { InitializeComponent(); }
        /// <summary>
        /// A view with auto generated gui for the different parameter flavors.
        /// </summary>
        /// <param name="solutionComponent"></param>
        public ParameterView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            InitializeComponent();
            _parameter = solutionComponent as BaseParameter;

            if (_parameter is CustomListParameter) 
                solutionComponentPropertyPanel.AddControlType(typeof(CustomListParameter), typeof(LinkToCustomListParameterControl));

            solutionComponentPropertyPanel.SolutionComponent = solutionComponent;
            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;

            SetGui();
        }
        #endregion

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            solutionComponentPropertyPanel.Refresh();
            SetGui();
        }

        private void SetGui() {
            if (_parameter is CustomListParameter) {
                pnlCustomList.Visible = true;

                var parameter = _parameter as CustomListParameter;
                btnClear.Enabled = parameter.CustomList.Length > 0;
                var entries = new HashSet<string>();
                foreach (string s in parameter.CustomList)
                    if (!entries.Add(s)) {
                        btnRemoveDuplicates.Enabled = true;
                        return;
                    }
                btnRemoveDuplicates.Enabled = false;
            } else {
                pnlCustomList.Visible = false;
            }
        }

        private void btnAddFromText_Click(object sender, EventArgs e) {
            var parameter = _parameter as CustomListParameter;

            var fromTextDialog = new FromTextDialog();
            fromTextDialog.ShowDialog();
            var entries = new List<string>(parameter.CustomList);
            entries.AddRange(fromTextDialog.Entries);
            parameter.CustomList = entries.ToArray();
            _parameter = parameter;
            SolutionComponent.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            solutionComponentPropertyPanel.Refresh();
        }
        private void btnGenerate_Click(object sender, EventArgs e) {
            if (_customListGeneratorDialog == null)
                _customListGeneratorDialog = new CustomListGeneratorDialog(_parameter as CustomListParameter);

            if (_customListGeneratorDialog.ShowDialog() == DialogResult.OK) {
                SolutionComponent.InvokeSolutionComponentChangedEvent(
                    SolutionComponentChangedEventArgs.DoneAction.Edited);
                solutionComponentPropertyPanel.Refresh();
            }
        }
        private void btnRemoveDuplicates_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            var parameter = _parameter as CustomListParameter;
            var entries = new HashSet<string>();
            foreach (string value in parameter.CustomList)
                entries.Add(value);
            parameter.CustomList = new string[entries.Count];
            entries.CopyTo(parameter.CustomList);
            _parameter = parameter;
            SolutionComponent.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            solutionComponentPropertyPanel.Refresh();
            btnRemoveDuplicates.Enabled = false;
            Cursor = Cursors.Default;
        }
        private void btnClear_Click(object sender, EventArgs e) {
            (_parameter as CustomListParameter).CustomList = new string[] { };
            SolutionComponent.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            solutionComponentPropertyPanel.Refresh();
        }
        private void solutionComponentPropertyPanel_ValueChanged(object sender, ValueControlPanel.ValueChangedEventArgs e) {
            solutionComponentPropertyPanel.Refresh();
        }
        #endregion

    }
}