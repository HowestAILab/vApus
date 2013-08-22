/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using WeifenLuo.WinFormsUI.Docking;

namespace vApus.SolutionTree {
    /// <summary>
    ///     Derive a new view from this (use the constructor WITH parameters), the "SolutionComponentViewManager" can then display the view in the dockpanel of the GUI and will ensure that no multiple instances can exist.
    ///     When having a name that equals SolutionComponent.ToString + "View", you must not even specify the view type you want to show using the manager.
    ///     There is already a standard view "StandardSolutionComponentView" provided for when you just want to edit properties.
    /// </summary>
    public class BaseSolutionComponentView : DockContent {

        #region Fields
        private readonly SolutionComponent _solutionComponent;
        #endregion

        #region Properties
        /// <summary>
        ///     The 'owner' of this. This property is prerequisite.
        /// </summary>
        public SolutionComponent SolutionComponent { get { return _solutionComponent; } }
        #endregion

        #region Constructors

        /// <summary>
        ///     Design time only constructor.
        /// </summary>
        protected BaseSolutionComponentView() { }
        /// <summary>
        ///     Derive a new view from this, the "SolutionComponentViewManager" can then display the view in the dockpanel of the GUI and will ensure that no multiple instances can exist.
        ///     When having a name that equals SolutionComponent.ToString + "View", you must not even specify the view type you want to show using the manager.
        ///     There is already a standard view "StandardSolutionComponentView" provided for when you just want to edit properties.
        /// </summary>
        /// <param name="solutionComponent">Cannot be null!</param>
        protected BaseSolutionComponentView(SolutionComponent solutionComponent) {
            if (solutionComponent == null)
                throw new ArgumentNullException("solutionComponent");
            _solutionComponent = solutionComponent;
        }

        #endregion

        #region Functions
        /// <summary>
        ///     Override this to do your own refreshing.
        ///     This is used in the solution component view manager, please implement this always.
        /// </summary>
        public override void Refresh() {
            base.Refresh();
            Text = SolutionComponent.ToString();
        }
        #endregion
    }
}