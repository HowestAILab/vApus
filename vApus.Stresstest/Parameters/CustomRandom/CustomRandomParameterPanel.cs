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
    public partial class CustomRandomParameterPanel : UserControl {
        private CSharpTextStyle _csharpTextStyle;

        /// <summary>
        ///     Call Init(...) to set.
        /// </summary>
        public CustomRandomParameter Parameter { get; private set; }

        public CustomRandomParameterPanel() {
            InitializeComponent();
        }

        public void Init(SolutionComponent solutionComponent) {
            ctxtGenerate.TextChangedDelayed -= ctxtGenerate_TextChangedDelayed;

            Parameter = solutionComponent as CustomRandomParameter;
            compileCustomRandom.Document = ctxtGenerate;
            compileCustomRandom.Parameter = Parameter;

            ctxtGenerate.ShowLineNumbers = true;
            _csharpTextStyle = new CSharpTextStyle(ctxtGenerate);
            ctxtGenerate.Text = Parameter.GenerateFunction;
            ctxtGenerate.TextChangedDelayed += ctxtGenerate_TextChangedDelayed;

            chkUnique.CheckedChanged -= chkUnique_CheckedChanged;
            chkUnique.Checked = Parameter.Unique;
            chkUnique.CheckedChanged += chkUnique_CheckedChanged;
        }

        private void ctxtGenerate_TextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
            Parameter.GenerateFunction = ctxtGenerate.Text;
            Parameter.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        private void compileCustomRandom_CompileErrorButtonClicked(object sender, TestCustomRandom.CompileErrorButtonClickedEventArgs e) {
            ctxtGenerate.SelectLine(e.LineNumber);
        }

        internal void TryCompileAndTestCode(out Exception exception) {
            compileCustomRandom.TryCompileAndTestCode(out exception);
        }

        private void chkUnique_CheckedChanged(object sender, EventArgs e) {
            Parameter.Unique = chkUnique.Checked;
            Parameter.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited, null);
        }
    }
}