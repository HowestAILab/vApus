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
        public CustomRandomParameterView()
        {
            InitializeComponent();
        }
        public CustomRandomParameterView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            InitializeComponent();

            this.HandleCreated += new EventHandler(CustomRandomParameterView_HandleCreated);
        }
        private void CustomRandomParameterView_HandleCreated(object sender, EventArgs e)
        {
            CustomRandomParameterPanel customRandomParameterPanel = new CustomRandomParameterPanel();
            this.Controls.Add(customRandomParameterPanel);
            customRandomParameterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            customRandomParameterPanel.Init(SolutionComponent);
        }
    }
}