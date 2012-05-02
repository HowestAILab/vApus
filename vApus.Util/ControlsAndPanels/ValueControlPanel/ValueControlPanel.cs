/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vApus.Util
{
    /// <summary>
    /// This is a panel to edit values of the types: string, char, bool, all numeric types and array or list of those.
    /// Furthermore, a single object having a parent of the type IEnumerable (GetParent() in vApus.Util.ObjectExtension), can be displayed also.
    /// The type of the property must be one of the above, or else an exception will be thrown. 
    /// The value may not be null or an exception will be thrown.
    /// </summary>
    public class ValueControlPanel : FlowLayoutPanel
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        #region Fields
        private BaseValueControl.Value[] _values = { };
        //Filled with default controls --> these are encapsulated in ValueControls
        //key = value type, value = BaseValueControl impl type
        private Dictionary<Type, Type> _controlTypes;
        private bool _locked;
        #endregion

        #region Properties
        public BaseValueControl.Value[] Values
        {
            get { return _values; }
        }
        #endregion

        public ValueControlPanel()
        {
            FillControlTypes();
            this.HandleCreated += new EventHandler(ValueControlPanel_HandleCreated);
        }

        #region Functions
        private void ValueControlPanel_HandleCreated(object sender, EventArgs e)
        {
            if (_locked)
                Lock();
            else
                Unlock();
        }
        private void FillControlTypes()
        {
            _controlTypes = new Dictionary<Type, Type>();
            AddControlType(typeof(bool), typeof(BoolValueControl));
            AddControlType(typeof(char), typeof(CharValueControl));
            AddControlType(typeof(string), typeof(StringValueControl));
            //AddControl(typeof(Enum), typeof(ComboBox));

        }
        public void AddControlType(Type valueType, Type controlType)
        {
            _controlTypes.Add(valueType, controlType);
        }

        /// <summary>
        /// Fills the panel with controls, recycles previous ones if possible.
        /// </summary>
        /// <param name="values"></param>
        public void SetValues(params BaseValueControl.Value[] values)
        {
            LockWindowUpdate(this.Handle.ToInt32());
            _values = values;
            //Keep the values here before adding them.
            var range = new List<BaseValueControl>(_values.Length);

            foreach (BaseValueControl.Value value in _values)
            {
                BaseValueControl control = null;
                Type controlType = _controlTypes[value.__Value.GetType()];
                //Find a control with the right type if any.
                foreach (BaseValueControl ctrl in this.Controls)
                    if (controlType == ctrl.GetType() && !range.Contains(ctrl))
                    {
                        control = ctrl;
                        break;
                    }
                //Otherwise make a new one.
                if (control == null)
                {
                    control = Activator.CreateInstance(controlType) as BaseValueControl;
                    control.ValueChanged += new EventHandler<BaseValueControl.ValueChangedEventArgs>(ValueControlPanel_ValueChanged);
                }
                (control as IValueControl).Init(value);

                range.Add(control);
            }

            this.Controls.Clear();
            this.Controls.AddRange(range.ToArray());
            LockWindowUpdate(0);
        }
        private void ValueControlPanel_ValueChanged(object sender, BaseValueControl.ValueChangedEventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, new ValueChangedEventArgs(this.Controls.IndexOf(sender as Control), e.OldValue, e.NewValue));
        }

        /// <summary>
        /// Lock all containing "BaseValueControl"'s.
        /// </summary>
        public void Lock()
        {
            _locked = true;
            foreach (BaseValueControl control in this.Controls)
                if (control.IsHandleCreated)
                    control.Lock();
                else
                    control.HandleCreated += new EventHandler(control_lock_HandleCreated);
        }
        private void control_lock_HandleCreated(object sender, EventArgs e)
        {
            BaseValueControl control = sender as BaseValueControl;
            control.HandleCreated -= control_lock_HandleCreated;
            control.Lock();
        }
        /// <summary>
        /// Unlock all containing "BaseValueControl"'s.
        /// </summary>
        public void Unlock()
        {
            _locked = false;
            foreach (BaseValueControl control in this.Controls)
                if (control.IsHandleCreated)
                    control.Unlock();
                else
                    control.HandleCreated += new EventHandler(control_unlock_HandleCreated);
        }
        private void control_unlock_HandleCreated(object sender, EventArgs e)
        {
            BaseValueControl control = sender as BaseValueControl;
            control.HandleCreated -= control_lock_HandleCreated;
            control.Unlock();
        }
        #endregion

        public class ValueChangedEventArgs : EventArgs
        {
            public readonly int Index;
            public readonly object OldValue, NewValue;
            public ValueChangedEventArgs(int index, object oldValue, object newValue)
            {
                Index = index;
                OldValue = oldValue;
                NewValue = newValue;
            }
        }
    }
}
