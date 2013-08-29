/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.Stresstest {
    public partial class CustomRandomParameterView : BaseSolutionComponentView {

        #region Constructors
        /// <summary>
        /// Design time constructor.
        /// </summary>
        public CustomRandomParameterView() {    InitializeComponent();       }
        public CustomRandomParameterView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            InitializeComponent();
            HandleCreated += CustomRandomParameterView_HandleCreated;
        }
        #endregion

        #region Functions
        /// <summary>
        /// All GUI items are added when the handle is created. This to avoid WinForms issues.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomRandomParameterView_HandleCreated(object sender, EventArgs e) {
            var customRandomParameterPanel = new CustomRandomParameterPanel();
            Controls.Add(customRandomParameterPanel);
            customRandomParameterPanel.Dock = DockStyle.Fill;
            customRandomParameterPanel.Init(SolutionComponent);
        }
        #endregion
    }
}