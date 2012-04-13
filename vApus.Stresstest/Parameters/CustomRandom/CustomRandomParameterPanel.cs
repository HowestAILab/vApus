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
        }
        private void cbGenerate_CodeTextChangedDelayed(object sender, EventArgs e)
        {
            cbGenerate.RefreshLineNumbers();
            _parameter.GenerateFunction = cbGenerate.Code;
            _parameter.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        private void compileCustomRandom_CompileErrorButtonClicked(object sender, CompileCustomRandom.CompileErrorButtonClickedEventArgs e)
        {
            e.CodePart.SelectLine(e.LineNumber);
        }
    }
}