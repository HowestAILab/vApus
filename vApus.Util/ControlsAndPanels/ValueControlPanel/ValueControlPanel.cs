/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel;

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
        private bool _locked, _autoSelectControl = true;
        #endregion

        #region Properties
        public BaseValueControl.Value[] Values
        {
            get { return _values; }
        }
        public IEnumerable ControlTypes
        {
            get { foreach (Type t in _controlTypes.Keys) yield return t; }
        }
        /// <summary>
        /// Use this read only please.
        /// </summary>
        public ControlCollection ValueControls
        {
            get { return this.Controls; }
        }
        public bool Locked
        {
            get { return _locked; }
        }
        [DefaultValue(true)]
        public bool AutoSelectControl
        {
            get { return _autoSelectControl; }
            set { _autoSelectControl = value; }
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
            AddControlType(typeof(short), typeof(NumericValueControl));
            AddControlType(typeof(int), typeof(NumericValueControl));
            AddControlType(typeof(long), typeof(NumericValueControl));
            AddControlType(typeof(ushort), typeof(NumericValueControl));
            AddControlType(typeof(uint), typeof(NumericValueControl));
            AddControlType(typeof(ulong), typeof(NumericValueControl));
            AddControlType(typeof(float), typeof(NumericValueControl));
            AddControlType(typeof(double), typeof(NumericValueControl));
            AddControlType(typeof(decimal), typeof(NumericValueControl));
            AddControlType(typeof(Enum), typeof(EnumValueControl));
            AddControlType(typeof(IList), typeof(CollectionValueControl));
            AddControlType(typeof(Array), typeof(CollectionValueControl));
            AddControlType(typeof(object), typeof(CollectionItemValueControl));
        }
        /// <summary>
        /// There are a lot off dirrerent control types (ControlTypes property) already available, you can still add your own though.
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="controlType"></param>
        public void AddControlType(Type valueType, Type controlType)
        {
            _controlTypes.Add(valueType, controlType);
        }

        /// <summary>
        /// Fills the panel with controls, recycles previous ones if possible.
        /// 
        /// Note: IList and Array -- If those types have a parent that is a collection (.SetParent()) the choice is limited to the items of that parent collection.
        /// 
        /// Object -- The given object must have a parent that is a collection (either Ilist or Array), the choice is limited to a value of that collection. (all items must have the collection for a parent)
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
                Type valueType = value.__Value.GetType();
                Type controlType = null;
                while (controlType == null)
                {
                    if (_controlTypes.TryGetValue(valueType, out controlType))
                        break;

                    valueType = valueType.BaseType;
                }

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

            this.AutoScroll = true;

            //Ensure it is selected when it becomes visible.
            if (_autoSelectControl && this.Controls.Count != 0)
            {
                Control control = this.Controls[0];
                if (control.IsHandleCreated && control.Visible)
                    control.Select();
                else
                    control.VisibleChanged += new EventHandler(ValueControl_VisibleChanged);
            }

            LockWindowUpdate(0);
        }
        /// <summary>
        /// This allows recycling of the controls.
        /// </summary>
        /// <param name="values"></param>
        public void Set__Values(params object[] values)
        {
            LockWindowUpdate(this.Handle.ToInt32());
            for (int i = 0; i != this.Controls.Count; i++)
                Set__ValueAt(i, values[i]);
            LockWindowUpdate(0);
        }
        /// <summary>
        /// This allows recycling of the controls.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value">Must be of the correct data type for the present value control</param>
        private void Set__ValueAt(int index, object value)
        {
            BaseValueControl valueControl = this.Controls[index] as BaseValueControl;
            valueControl.Toggle(BaseValueControl.ToggleState.Collapse);

            //First check is a null check :).
            if (valueControl.__Value.__Value != null &&
                value != null &&
                !valueControl.__Value.__Value.Equals(value))
                (valueControl as IValueControl).Init(new BaseValueControl.Value
                {
                    __Value = value,
                    Description = valueControl.Description,
                    IsEncrypted = valueControl.IsEncrypted,
                    IsReadOnly = valueControl.IsReadOnly,
                    Label = valueControl.Label
                });
        }
        /// <summary>
        /// For dynamic descriptions, will only set it if not null and if not the same as the old one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="description"></param>
        public void SetDescriptionAt(int index, string description)
        {
            BaseValueControl valueControl = this.Controls[index] as BaseValueControl;
            if (valueControl.__Value.Description != null &&
                description != null &&
                !valueControl.__Value.Description.Equals(description))
                (valueControl as IValueControl).Init(new BaseValueControl.Value
                {
                    __Value = valueControl.__Value.__Value,
                    Description = description,
                    IsEncrypted = valueControl.IsEncrypted,
                    IsReadOnly = valueControl.IsReadOnly,
                    Label = valueControl.Label
                });
        }
        private void ValueControlPanel_ValueChanged(object sender, BaseValueControl.ValueChangedEventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, new ValueChangedEventArgs(this.Controls.IndexOf(sender as Control), e.OldValue, e.NewValue));
        }
        private void ValueControl_VisibleChanged(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (control.Visible)
            {
                control.VisibleChanged -= ValueControl_VisibleChanged;
                control.Select();
            }
        }
        /// <summary>
        /// Lock all containing "BaseValueControl"'s.
        /// </summary>
        public void Lock()
        {
            _locked = true;
            if (IsHandleCreated)
                foreach (BaseValueControl control in this.Controls)
                    if (control.IsHandleCreated)
                        control.Lock();
                    else
                        control.HandleCreated += new EventHandler(control_lock_HandleCreated);
            else
                this.HandleCreated += new EventHandler(ValueControlPanel_lock_HandleCreated);
        }
        private void ValueControlPanel_lock_HandleCreated(object sender, EventArgs e)
        {
            this.HandleCreated -= ValueControlPanel_lock_HandleCreated;
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
            if (IsHandleCreated)
                foreach (BaseValueControl control in this.Controls)
                    if (control.IsHandleCreated)
                        control.Unlock();
                    else
                        control.HandleCreated += new EventHandler(control_unlock_HandleCreated);
            else
                this.HandleCreated += new EventHandler(ValueControlPanel_unlock_HandleCreated);
        }
        private void ValueControlPanel_unlock_HandleCreated(object sender, EventArgs e)
        {
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
