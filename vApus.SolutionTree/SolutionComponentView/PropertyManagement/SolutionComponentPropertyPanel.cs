/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.SolutionTree
{
    /// <summary>
    /// This is a standard panel to edit a property of the types: string, char, bool, all numeric types and array or list of those.
    /// Furthermore, a single object having a parent of the type IEnumerable (GetParent() in vApus.Util.ObjectExtension), can be displayed also.
    /// The type of the property must be one of the above, or else an exception will be thrown. 
    /// Or else you can always make your own control derived from "BaseSolutionComponentPropertyControl".
    /// The value of the property may not be null or an exception will be thrown.
    /// </summary>
    public partial class SolutionComponentPropertyPanel : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        #region Fields
        private SolutionComponent _solutionComponent;
        private bool _locked;
        #endregion

        #region Properties
        /// <summary>
        /// Sets the Gui if the panel is empty.
        /// </summary>
        public SolutionComponent SolutionComponent
        {
            get { return _solutionComponent; }
            set
            {
                if (_solutionComponent != value)
                {
                    bool recycle = _solutionComponent != null && value != null && value.GetType() == _solutionComponent.GetType();
                    _solutionComponent = value;
                    if (IsHandleCreated)
                        SetGui(recycle);
                }
            }
        }
        public FlowDirection FlowDirection
        {
            get { return flp.FlowDirection; }
            set { flp.FlowDirection = value; }
        }
        /// <summary>
        /// All the solution component property controls.
        /// </summary>
        public ControlCollection SolutionComponentPropertyControls
        {
            get { return flp.Controls; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This is a standard panel to edit a property of the types: string, char, bool, all numeric types and array or list of those.
        /// Furthermore, a single object having a parent of the type IEnumerable (GetParent() in vApus.Util.ObjectExtension), can be displayed also.
        /// The type of the property must be one of the above, or else an exception will be thrown. 
        /// Or else you can always make your own control derived from "BaseSolutionComponentPropertyControl".
        /// The value of the property may not be null or an exception will be thrown.
        /// </summary>
        public SolutionComponentPropertyPanel()
        {
            InitializeComponent();
            if (this.IsHandleCreated)
                SetGui(false);
            else
                this.HandleCreated += new EventHandler(SolutionComponentPropertyPanel_HandleCreated);
        }
        #endregion

        #region Functions
        private void SolutionComponentPropertyPanel_HandleCreated(object sender, EventArgs e)
        {
            SetGui(false);
            if (_locked)
                Lock();
            else
                Unlock();
        }
        private void SetGui(bool recycle)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            List<PropertyInfo> properties = null;
            if (_solutionComponent != null)
            {
                properties = new List<PropertyInfo>(_solutionComponent.GetType().GetProperties());
                properties.Sort(PropertyInfoComparer.GetInstance());
                properties.Sort(PropertyInfoDisplayIndexComparer.GetInstance());
            }
            if (recycle)
            {
                int controlIndex = 0;
                foreach (PropertyInfo info in properties)
                {
                    object[] attributes = info.GetCustomAttributes(typeof(PropertyControlAttribute), true);
                    PropertyControlAttribute propertyControlAttribute = (attributes.Length == 0) ? null : (attributes[0] as PropertyControlAttribute);
                    if (propertyControlAttribute != null)
                        if (propertyControlAttribute.UseCustomPropertyControlType)
                        {
                            Control customPropertyControl = flp.Controls[controlIndex++];
                            customPropertyControl = propertyControlAttribute.GetCustomPropertyControl(_solutionComponent, info);
                        }
                        else
                            try
                            {
                                (flp.Controls[controlIndex++] as SolutionComponentCommonPropertyControl).Recycle(_solutionComponent, info);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                }
            }
            else
            {
                flp.Controls.Clear();
                if (_solutionComponent != null)
                {
                    this.Cursor = Cursors.WaitCursor;
                    foreach (PropertyInfo info in properties)
                    {
                        object[] attributes = info.GetCustomAttributes(typeof(PropertyControlAttribute), true);
                        PropertyControlAttribute propertyControlAttribute = (attributes.Length == 0) ? null : (attributes[0] as PropertyControlAttribute);
                        if (propertyControlAttribute != null)
                            if (propertyControlAttribute.UseCustomPropertyControlType)
                            {
                                flp.Controls.Add(propertyControlAttribute.GetCustomPropertyControl(_solutionComponent, info));
                            }
                            else
                                try
                                {
                                    flp.Controls.Add(new SolutionComponentCommonPropertyControl(_solutionComponent, info));
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("\"SolutionComponentCommonPropertyControl\" is a standard control to edit a property of the types: string, char, bool, all numeric types and array or list of those.\nThe type of the property must be one of the above and may not be null.", ex);
                                }
                    }
                    this.Cursor = Cursors.Default;
                }
                if (flp.Controls.Count > 0)
                    flp.Controls[0].VisibleChanged += new EventHandler(SolutionComponentPropertyControl_VisibleChanged);
            }
            flp.AutoScroll = true;

            LockWindowUpdate(0);
        }
        private void SolutionComponentPropertyControl_VisibleChanged(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (control.Visible)
            {
                control.VisibleChanged -= SolutionComponentPropertyControl_VisibleChanged;
                control.Select();
            }
        }
        /// <summary>
        /// Lock all containing "BaseSolutionComponentPropertyControl"'s.
        /// </summary>
        public void Lock()
        {
            _locked = true;
            foreach (Control control in flp.Controls)
                if (control.IsHandleCreated)
                    (control as BaseSolutionComponentPropertyControl).Lock();
                else
                    control.HandleCreated += new EventHandler(control_lock_HandleCreated);
        }
        private void control_lock_HandleCreated(object sender, EventArgs e)
        {
            BaseSolutionComponentPropertyControl control = sender as BaseSolutionComponentPropertyControl;
            control.HandleCreated -= control_lock_HandleCreated;
            control.Lock();
        }
        /// <summary>
        /// Unlock all containing "BaseSolutionComponentPropertyControl"'s.
        /// </summary>
        public void Unlock()
        {
            _locked = false;
            foreach (Control control in flp.Controls)
                if (control.IsHandleCreated)
                    (control as BaseSolutionComponentPropertyControl).Unlock();
                else
                    control.HandleCreated += new EventHandler(control_unlock_HandleCreated);
        }
        private void control_unlock_HandleCreated(object sender, EventArgs e)
        {
            BaseSolutionComponentPropertyControl control = sender as BaseSolutionComponentPropertyControl;
            control.HandleCreated -= control_lock_HandleCreated;
            control.Unlock();
        }
        /// <summary>
        /// This is used in the solution component view manager, please implement this always.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            foreach (Control control in flp.Controls)
            {
                (control as BaseSolutionComponentPropertyControl).Target = _solutionComponent;
                control.Refresh();
            }
        }
        #endregion
    }
}
