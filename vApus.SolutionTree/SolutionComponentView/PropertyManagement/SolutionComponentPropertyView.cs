/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;

namespace vApus.SolutionTree
{
    /// <summary>
    /// Just a form with a docked solution component property panel.
    /// </summary>
    public partial class SolutionComponentPropertyView : BaseSolutionComponentView
    {
        /// <summary>
        /// Design time only constructor.
        /// </summary>
        public SolutionComponentPropertyView()
            : base()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Just a form with a docked solution component property panel.
        /// </summary>
        /// <param name="solutionComponent"></param>
        /// <param name="args"></param>
        public SolutionComponentPropertyView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            InitializeComponent();
            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(SolutionComponentPropertyView_HandleCreated);
        }
        private void SolutionComponentPropertyView_HandleCreated(object sender, EventArgs e)
        {
            SetGui();
        }
        /// <summary>
        /// This is used in the solution component view manager, please implement this always.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            SetGui();
            solutionComponentPropertyPanel.Refresh();
        }
        private void SetGui()
        {
            Text = "Properties for " + SolutionComponent.ToString();
            solutionComponentPropertyPanel.SolutionComponent = SolutionComponent;
        }
    }
}
