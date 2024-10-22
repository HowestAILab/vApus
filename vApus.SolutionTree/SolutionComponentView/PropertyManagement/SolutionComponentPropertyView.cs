﻿/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;

namespace vApus.SolutionTree {
    /// <summary>
    ///     Just a form with a docked solution component property panel.
    /// </summary>
    public partial class SolutionComponentPropertyView : BaseSolutionComponentView {

        #region Constructors
        /// <summary>
        ///     Design time only constructor.
        /// </summary>
        public SolutionComponentPropertyView() { InitializeComponent(); }
        /// <summary>
        ///     Just a form with a docked solution component property panel.
        /// </summary>
        /// <param name="solutionComponent"></param>
        /// <param name="args"></param>
        public SolutionComponentPropertyView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            InitializeComponent();
            if (IsHandleCreated)
                SetGui();
            else
                HandleCreated += SolutionComponentPropertyView_HandleCreated;
        }
        #endregion

        #region Functions
        private void SolutionComponentPropertyView_HandleCreated(object sender, EventArgs e) {
            SetGui();
        }
        private void SetGui() {
            Text = "Properties for " + SolutionComponent;
            solutionComponentPropertyPanel.SolutionComponent = SolutionComponent;
        }
        /// <summary>
        ///     This is used in the solution component view manager, please implement this always.
        /// </summary>
        public override void Refresh() {
            base.Refresh();
            SetGui();
            solutionComponentPropertyPanel.Refresh();
        }
        #endregion
    }
}