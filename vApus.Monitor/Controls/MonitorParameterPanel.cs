/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApusSMT.Base;

namespace vApus.Monitor
{
    /// <summary>
    /// This is a standard panel to edit a property of the types: string, char, bool, all numeric types and array or list of those.
    /// Furthermore, a single object having a parent of the type IEnumerable (GetParent() in vApus.Util.ObjectExtension), can be displayed also.
    /// The type of the property must be one of the above, or else an exception will be thrown. 
    /// Or else you can always make your own control derived from "BaseSolutionComponentPropertyControl".
    /// The value of the property may not be null or an exception will be thrown.
    /// </summary>
    public partial class ParameterPanel : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        public event EventHandler ParameterValueChanged;

        #region Fields
        private Dictionary<Parameter, object> _parametersWithValues;

        private bool _locked;
        #endregion

        #region Properties
        /// <summary>
        /// Sets the Gui if the panel is empty.
        /// </summary>
        public Dictionary<Parameter, object> ParametersWithValues
        {
            get { return _parametersWithValues; }
            set
            {
                _parametersWithValues = value;
                if (IsHandleCreated)
                    SetGui();
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
        public ParameterPanel()
        {
            InitializeComponent();
            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(SolutionComponentPropertyPanel_HandleCreated);
        }
        #endregion

        #region Functions
        private void SolutionComponentPropertyPanel_HandleCreated(object sender, EventArgs e)
        {
            SetGui();
            if (_locked)
                Lock();
            else
                Unlock();
        }
        private void SetGui()
        {
            LockWindowUpdate(this.Handle.ToInt32());

            flp.Controls.Clear();
            if (_parametersWithValues != null)
            {
                this.Cursor = Cursors.WaitCursor;
                foreach (Parameter parameter in _parametersWithValues.Keys)
                {
                    var ctrl = new ParameterControl(parameter, _parametersWithValues[parameter]);
                    ctrl.ValueChanged += new EventHandler(ctrl_ValueChanged);
                    flp.Controls.Add(ctrl);
                }

                this.Cursor = Cursors.Default;
            }
            if (flp.Controls.Count > 0)
                flp.Controls[0].VisibleChanged += new EventHandler(ParameterControl_VisibleChanged);

            flp.AutoScroll = true;

            LockWindowUpdate(0);
        }

        private void ctrl_ValueChanged(object sender, EventArgs e)
        {
            ParameterControl ctrl = sender as ParameterControl;
            if (_parametersWithValues.ContainsKey(ctrl.Parameter))
                if (_parametersWithValues[ctrl.Parameter] != ctrl.Value)
                {
                    _parametersWithValues[ctrl.Parameter] = ctrl.Value;
                    if (ParameterValueChanged != null)
                        ParameterValueChanged(this, null);
                }
        }
        private void ParameterControl_VisibleChanged(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (control.Visible)
            {
                control.VisibleChanged -= ParameterControl_VisibleChanged;
                control.Select();
            }
        }
        /// <summary>
        /// </summary>
        public void Lock()
        {
            _locked = true;
            foreach (Control control in flp.Controls)
                if (control.IsHandleCreated)
                    (control as ParameterControl).Lock();
                else
                    control.HandleCreated += new EventHandler(control_lock_HandleCreated);
        }
        private void control_lock_HandleCreated(object sender, EventArgs e)
        {
            ParameterControl control = sender as ParameterControl;
            control.HandleCreated -= control_lock_HandleCreated;
            control.Lock();
        }
        /// <summary>
        /// </summary>
        public void Unlock()
        {
            _locked = false;
            foreach (Control control in flp.Controls)
                if (control.IsHandleCreated)
                    (control as ParameterControl).Unlock();
                else
                    control.HandleCreated += new EventHandler(control_unlock_HandleCreated);
        }
        private void control_unlock_HandleCreated(object sender, EventArgs e)
        {
            ParameterControl control = sender as ParameterControl;
            control.HandleCreated -= control_lock_HandleCreated;
            control.Unlock();
        }
        #endregion
    }
}
