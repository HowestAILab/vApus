/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using vApus.SolutionTree;

namespace vApus.Stresstest
{
    public partial class CustomRandomParameterView : BaseSolutionComponentView
    {
        private CustomRandomParameter _parameter;

        public CustomRandomParameterView()
        {
            InitializeComponent();
        }
        public CustomRandomParameterView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            InitializeComponent();

            _parameter = solutionComponent as CustomRandomParameter;
            compileCustomRandom.Document = cbGenerate;
            compileCustomRandom.Parameter = _parameter;

            this.HandleCreated += new EventHandler(CustomRandomParameterView_HandleCreated);
        }
        private void CustomRandomParameterView_HandleCreated(object sender, EventArgs e)
        {
            cbGenerate.Code = _parameter.GenerateFunction;
            cbGenerate.RefreshLineNumbers();
            cbGenerate.CodeTextChanged += new EventHandler(cbGenerate_CodeTextChanged);
        }
        private void cbGenerate_CodeTextChanged(object sender, EventArgs e)
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