/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    ///     This is a panel to edit values of the types: string, char, bool, all numeric types and array or list of those.
    ///     Furthermore, a single object having a parent of the type IEnumerable (GetParent() in vApus.Util.ObjectExtension), can be displayed also.
    ///     The type of the property must be one of the above, or else an exception will be thrown.
    ///     The value may not be null or an exception will be thrown.
    /// </summary>
    public class ValueControlPanel : FlowLayoutPanel {
        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        #region Fields

        //Filled with default controls --> these are encapsulated in ValueControls
        //key = value type, value = BaseValueControl impl type
        private bool _autoSelectControl = true;
        private Dictionary<Type, Type> _controlTypes;
        protected bool _locked;
        private BaseValueControl.Value[] _values = { };

        #endregion

        #region Properties

        public BaseValueControl.Value[] Values {
            get { return _values; }
        }

        public IEnumerable ControlTypes {
            get { foreach (Type t in _controlTypes.Keys) yield return t; }
        }

        /// <summary>
        ///     Use this read only please.
        /// </summary>
        public ControlCollection ValueControls {
            get { return Controls; }
        }

        public bool Locked {
            get { return _locked; }
        }

        [DefaultValue(true)]
        public bool AutoSelectControl {
            get { return _autoSelectControl; }
            set { _autoSelectControl = value; }
        }

        #endregion

        public ValueControlPanel() {
            FillControlTypes();
            HandleCreated += ValueControlPanel_HandleCreated;
        }

        #region Functions
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        private void ValueControlPanel_HandleCreated(object sender, EventArgs e) {
            if (_locked) Lock(); else Unlock();
        }

        private void FillControlTypes() {
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
        ///     There are a lot off dirrerent control types (ControlTypes property) already available, you can still add your own though.
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="controlType"></param>
        public void AddControlType(Type valueType, Type controlType) {
            _controlTypes.Add(valueType, controlType);
        }

        /// <summary>
        ///     Fills the panel with controls, recycles previous ones if possible.
        ///     Note: IList and Array -- If those types have a parent that is a collection (.SetParent()) the choice is limited to the items of that parent collection.
        ///     Object -- The given object must have a parent that is a collection (either Ilist or Array), the choice is limited to a value of that collection. (all items must have the collection for a parent)
        /// </summary>
        /// <param name="values"></param>
        public void SetValues(bool forceFullRefresh, params BaseValueControl.Value[] values) {
            try {
                _values = values;

                //Recycle controls, + 1 because solution component property panel adds a control afterwards. () not the right place for this + 1, but yeah...)
                bool partialRefresh = forceFullRefresh ? false : (_values.Length == Controls.Count || _values.Length + 1 == Controls.Count);
                AutoScroll = false;
                SetValues(partialRefresh);
            }
            catch (Exception ex) {
                Loggers.Log(Level.Error, "Generating GUI failed.", ex);
            }
            AutoScroll = true;
            LockWindowUpdate(IntPtr.Zero);
        }

        private void SetValues(bool partialRefresh) {
            if (partialRefresh) {
                LockWindowUpdate(this.Handle);
                //SendMessageWrapper.SetWindowRedraw(Handle, false);

                for (int i = 0; i != _values.Length; i++)
                    try {
                        (Controls[i] as IValueControl).Init(_values[i]);
                    }
                    catch {
                        //It is not a real problem if this fails. It is most likely due to a data type mismatch when recycling controls.
                        LockWindowUpdate(IntPtr.Zero);
                        SetValues(false);
                        return;
                    }

                //SendMessageWrapper.SetWindowRedraw(Handle, true);
                LockWindowUpdate(IntPtr.Zero);
                Invalidate();
            }
            else {
                LockWindowUpdate(Handle);

                //Keep the values here before adding them.
                var range = new List<BaseValueControl>(_values.Length);

                foreach (BaseValueControl.Value value in _values) {
                    BaseValueControl control = null;
                    Type valueType = value.__Value.GetType();
                    Type controlType = null;
                    while (controlType == null) {
                        if (_controlTypes.TryGetValue(valueType, out controlType))
                            break;

                        valueType = valueType.BaseType;
                    }

                    control = Activator.CreateInstance(controlType) as BaseValueControl;
                    control.ValueChanged += ValueControlPanel_ValueChanged;

                    (control as IValueControl).Init(value);

                    range.Add(control);
                }

                Controls.Clear();
                Controls.AddRange(range.ToArray());

                //Ensure it is selected when it becomes visible.
                if (_autoSelectControl && Controls.Count != 0) {
                    Control control = Controls[0];
                    if (control.IsHandleCreated && control.Visible) {
                        control.Focus();
                        control.Select();
                    }
                    else {
                        control.VisibleChanged += ValueControl_VisibleChanged;
                    }
                }

                LockWindowUpdate(IntPtr.Zero);
            }
        }

        public void ClearValues() { SetValues(true, new BaseValueControl.Value[0]); }

        /// <summary>
        ///     For dynamic descriptions, will only set it if not null and if not the same as the old one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="description"></param>
        public void SetDescriptionAt(int index, string description) {
            var valueControl = Controls[index] as BaseValueControl;
            if (valueControl.__Value.Description != null &&
                description != null &&
                !valueControl.__Value.Description.Equals(description))
                (valueControl as IValueControl).Init(new BaseValueControl.Value {
                    __Value = valueControl.__Value.__Value,
                    Description = description,
                    IsEncrypted = valueControl.IsEncrypted,
                    IsReadOnly = valueControl.IsReadOnly,
                    Label = valueControl.Label
                });
        }

        private void ValueControlPanel_ValueChanged(object sender, BaseValueControl.ValueChangedEventArgs e) {
            if (ValueChanged != null && Controls.Count != 0)
                ValueChanged(this, new ValueChangedEventArgs(Controls.IndexOf(sender as Control), e.OldValue, e.NewValue));
        }

        private void ValueControl_VisibleChanged(object sender, EventArgs e) {
            var control = sender as Control;
            if (control.Visible) {
                control.VisibleChanged -= ValueControl_VisibleChanged;

                control.Focus();
                control.Select();
            }
        }

        /// <summary>
        ///     Lock all containing "BaseValueControl"'s.
        /// </summary>
        public void Lock() {
            _locked = true;
            if (IsHandleCreated)
                foreach (Control control in Controls)
                    if (control is BaseValueControl) {
                        if (control.IsHandleCreated)
                            (control as BaseValueControl).Lock();
                        else
                            control.HandleCreated += control_lock_HandleCreated;
                    }
                    else
                        HandleCreated += ValueControlPanel_lock_HandleCreated;
        }

        private void ValueControlPanel_lock_HandleCreated(object sender, EventArgs e) {
            HandleCreated -= ValueControlPanel_lock_HandleCreated;
            foreach (Control control in Controls)
                if (control is BaseValueControl) {
                    if (control.IsHandleCreated)
                        (control as BaseValueControl).Lock();
                    else
                        control.HandleCreated += control_lock_HandleCreated;
                }
        }

        private void control_lock_HandleCreated(object sender, EventArgs e) {
            var control = sender as BaseValueControl;
            control.HandleCreated -= control_lock_HandleCreated;
            control.Lock();
        }

        /// <summary>
        ///     Unlock all containing "BaseValueControl"'s.
        /// </summary>
        public void Unlock() {
            _locked = false;
            if (IsHandleCreated)
                foreach (Control control in Controls)
                    if (control is BaseValueControl) {
                        if (control.IsHandleCreated)
                            (control as BaseValueControl).Unlock();
                        else
                            control.HandleCreated += control_unlock_HandleCreated;
                    }
                    else
                        HandleCreated += ValueControlPanel_unlock_HandleCreated;
        }

        private void ValueControlPanel_unlock_HandleCreated(object sender, EventArgs e) {
            foreach (Control control in Controls)
                if (control is BaseValueControl) {
                    if (control.IsHandleCreated)
                        (control as BaseValueControl).Unlock();
                    else
                        control.HandleCreated += control_unlock_HandleCreated;
                }
        }

        private void control_unlock_HandleCreated(object sender, EventArgs e) {
            var control = sender as BaseValueControl;
            control.HandleCreated -= control_lock_HandleCreated;
            control.Unlock();
        }

        #endregion

        public class ValueChangedEventArgs : EventArgs {
            public readonly int Index;
            public readonly object NewValue;
            public readonly object OldValue;

            public ValueChangedEventArgs(int index, object oldValue, object newValue) {
                Index = index;
                OldValue = oldValue;
                NewValue = newValue;
            }
        }
    }
}