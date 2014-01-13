/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    /// <summary>
    /// Code is formatted on this panel. A bit of debugging can also be done (TestCustomRandomPanel).
    /// </summary>
    public partial class CustomRandomParameterPanel : UserControl {

        #region Fields
        private CSharpTextStyle _csharpTextStyle;
        #endregion

        #region Properties
        /// <summary>
        ///     Call Init(...) to set.
        /// </summary>
        public CustomRandomParameter Parameter { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Code is formatted on this panel. A bit of debugging can also be done (TestCustomRandomPanel).
        /// </summary>
        public CustomRandomParameterPanel() { InitializeComponent(); }
        #endregion

        #region Functions
        public void Init(SolutionComponent solutionComponent) {
            ctxtGenerate.TextChangedDelayed -= ctxtGenerate_TextChangedDelayed;

            Parameter = solutionComponent as CustomRandomParameter;
            compileCustomRandom.Document = ctxtGenerate;
            compileCustomRandom.Parameter = Parameter;

            ctxtGenerate.ShowLineNumbers = true;
            _csharpTextStyle = new CSharpTextStyle(ctxtGenerate);
            ctxtGenerate.Text = Parameter.Code;
            ctxtGenerate.DelayedTextChangedInterval = 1000;
            ctxtGenerate.TextChangedDelayed += ctxtGenerate_TextChangedDelayed;

            chkUnique.CheckedChanged -= chkUnique_CheckedChanged;
            chkUnique.Checked = Parameter.Unique;
            chkUnique.CheckedChanged += chkUnique_CheckedChanged;
        }

        private void ctxtGenerate_TextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
            Parameter.Code = ctxtGenerate.Text;
            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
            Parameter.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender == Parameter && e.__DoneAction == SolutionComponentChangedEventArgs.DoneAction.Edited) {
                this.ParentForm.Activate();
                ctxtGenerate.Focus();
               // SolutionComponent.SolutionComponentChanged -= SolutionComponent_SolutionComponentChanged;
            }
        }

        private void chkUnique_CheckedChanged(object sender, EventArgs e) {
            Parameter.Unique = chkUnique.Checked;
            Parameter.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited, null);
        }

        private void compileCustomRandom_CompileErrorButtonClicked(object sender, TestCustomRandomPanel.CompileErrorButtonClickedEventArgs e) {
            ctxtGenerate.SelectLine(e.LineNumber);
        }

        internal void TryCompileAndTestCode(out Exception exception) { compileCustomRandom.TryCompileAndTestCode(out exception); }

        #endregion
    }
}