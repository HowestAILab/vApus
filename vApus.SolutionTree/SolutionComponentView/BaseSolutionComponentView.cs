/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using WeifenLuo.WinFormsUI.Docking;

namespace vApus.SolutionTree
{
    /// <summary>
    /// Derive a new view from this (use the constructor WITH parameters), the "SolutionComponentViewManager" can then display the view in the dockpanel of the GUI and will ensure that no multiple instances can exist.
    /// When having a name that equals SolutionComponent.ToString + "View", you must not even specify the view type you want to show using the manager.
    /// There is already a standard view "StandardSolutionComponentView" provided for when you just want to edit properties.
    /// </summary>
    public class BaseSolutionComponentView : DockContent
    {
        #region Fields
        private SolutionComponent _solutionComponent;
        private object[] _args;
        #endregion

        #region Properties
        /// <summary>
        /// The 'owner' of this.
        /// </summary>
        public SolutionComponent SolutionComponent
        {
            get { return _solutionComponent; }
        }
        /// <summary>
        /// Contains any arg u have given with the manager.
        /// </summary>
        public object[] Args
        {
            get { return _args; }
            internal set { _args = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Design time only constructor.
        /// </summary>
        protected BaseSolutionComponentView()
        { }
        /// <summary>
        /// Derive a new view from this, the "SolutionComponentViewManager" can then display the view in the dockpanel of the GUI and will ensure that no multiple instances can exist.
        /// When having a name that equals SolutionComponent.ToString + "View", you must not even specify the view type you want to show using the manager.
        /// There is already a standard view "StandardSolutionComponentView" provided for when you just want to edit properties.
        /// </summary>
        protected BaseSolutionComponentView(SolutionComponent solutionComponent, params object[] args)
        {
            if (solutionComponent == null)
                throw new ArgumentNullException("solutionComponent");
            _solutionComponent = solutionComponent;
            _args = args;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Override this to do your own refreshing.
        /// This is used in the solution component view manager, please implement this always.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            Text = SolutionComponent.ToString();
        }
        #endregion
    }
}
