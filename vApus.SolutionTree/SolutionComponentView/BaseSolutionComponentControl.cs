/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace vApus.SolutionTree
{
    /// <summary>
    /// The base for each control that is drawn in a base solution component view (direct or indirect).
    /// This is because the solution component view manager is subscribed to the value changed event tobe able to update the parent of the control's underlying type (solution component).
    /// For example update the items collection (provided in each solution component) or a property of the solution component.
    /// </summary>
    [ToolboxItem(false)]
    public class BaseSolutionComponentControl : UserControl
    {
        protected SolutionComponent _target;

        public SolutionComponent Target
        {
            get { return _target; }
            set { _target = value; }
        }
        protected void InvokeSolutionComponentEdited()
        {
            _target.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        public BaseSolutionComponentControl() { }
        /// <summary>
        /// The base for each control that is drawn in a base solution component view (direct or indirect).
        /// This is because the solution component view manager is subscribed to the value changed event tobe able to update the parent of the control's underlying type (solution component).
        /// For example update the items collection (provided in each solution component) or a property of the solution component.
        /// </summary>
        public BaseSolutionComponentControl(SolutionComponent target)
        {
            _target = target;
        }
        /// <summary>
        /// If the new solution component is of the same type as the old one, this control can be recycled.
        /// </summary>
        /// <param name="newTarget"></param>
        protected virtual void Recycle(SolutionComponent newTarget)
        {
            if (newTarget.GetType() != _target.GetType())
                throw new Exception("Recycling is only possible with a solution component of the same type as the previous.");

            _target = newTarget;
        }
    }
}
