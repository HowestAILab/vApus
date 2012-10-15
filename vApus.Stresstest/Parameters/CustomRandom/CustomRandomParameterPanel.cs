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

namespace vApus.Stresstest
{
    public partial class CustomRandomParameterPanel : UserControl
    {
        private CustomRandomParameter _parameter;
        /// <summary>
        /// Call Init(...) to set.
        /// </summary>
        public CustomRandomParameter Parameter
        {
            get { return _parameter; }
        }

        public CustomRandomParameterPanel()
        {
            InitializeComponent();
        }
        public void Init(SolutionComponent solutionComponent)
        {
            cbGenerate.CodeTextChangedDelayed -= cbGenerate_CodeTextChangedDelayed;

            _parameter = solutionComponent as CustomRandomParameter;
            compileCustomRandom.Document = cbGenerate;
            compileCustomRandom.Parameter = _parameter;

            cbGenerate.ShowLineNumbers = true;
            cbGenerate.Code = _parameter.GenerateFunction;
            cbGenerate.RefreshLineNumbers(2);
            cbGenerate.CodeTextChangedDelayed += cbGenerate_CodeTextChangedDelayed;

            chkUnique.CheckedChanged -= chkUnique_CheckedChanged;
            chkUnique.Checked = _parameter.Unique;
            chkUnique.CheckedChanged += chkUnique_CheckedChanged;
        }
        private void cbGenerate_CodeTextChangedDelayed(object sender, EventArgs e)
        {
            cbGenerate.RefreshLineNumbers(2);
            _parameter.GenerateFunction = cbGenerate.Code;
            _parameter.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        private void compileCustomRandom_CompileErrorButtonClicked(object sender, TestCustomRandom.CompileErrorButtonClickedEventArgs e)
        {
            e.CodePart.SelectLine(e.LineNumber);
        }
        internal void TryCompileAndTestCode(out Exception exception)
        {
            compileCustomRandom.TryCompileAndTestCode(out exception);
        }

        private void chkUnique_CheckedChanged(object sender, EventArgs e)
        {
            _parameter.Unique = chkUnique.Checked;
            _parameter.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited, null);
        }
    }
}