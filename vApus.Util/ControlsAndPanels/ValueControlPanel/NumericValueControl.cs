/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace vApus.Util {
    [ToolboxItem(false)]
    public partial class NumericValueControl : BaseValueControl, IValueControl {
        private object _prevValue;
        private FixedNumericUpDown _nud;

        public NumericValueControl() {
            InitializeComponent();
        }

        public void Init(Value value) {
            base.__Value = value;
            _prevValue = value;

            //Only take the value into account, the other properties are taken care off.
            //Keep control recycling in mind.
            _nud = null;

            if (base.ValueControl == null) {
                _nud = new FixedNumericUpDown();

                if (value.__Value is short) {
                    _nud.Minimum = short.MinValue;
                    _nud.Maximum = short.MaxValue;
                } else if (value.__Value is int) {
                    _nud.Minimum = value.AllowedMinimum;
                    _nud.Maximum = value.AllowedMaximum;
                } else if (value.__Value is long) {
                    _nud.Minimum = long.MinValue;
                    _nud.Maximum = long.MaxValue;
                } else if (value.__Value is ushort) {
                    _nud.Minimum = ushort.MinValue;
                    _nud.Maximum = ushort.MaxValue;
                } else if (value.__Value is uint) {
                    _nud.Minimum = uint.MinValue;
                    _nud.Maximum = uint.MaxValue;
                } else if (value.__Value is ulong) {
                    _nud.Minimum = ulong.MinValue;
                    _nud.Maximum = ulong.MaxValue;
                } else {
                    _nud.Minimum = decimal.MinValue;
                    _nud.Maximum = decimal.MaxValue;

                    _nud.DecimalPlaces = 3;
                }

                _nud.Dock = DockStyle.Fill;

                _nud.KeyUp += nud_KeyUp;

                HandleCreated += NumericValueControl_HandleCreated;
            } else {
                _nud = base.ValueControl as FixedNumericUpDown;
            }

            _nud.Value = Convert.ToDecimal(value.__Value);
            base.ValueControl = _nud;
        }

        private void NumericValueControl_HandleCreated(object sender, EventArgs e) {
            HandleCreated -= NumericValueControl_HandleCreated;

            if (ParentForm != null && !ParentForm.IsDisposed) {
                ParentForm.FormClosing += ParentForm_FormClosing;
                ParentForm.Leave += ParentForm_Leave;
                Leave += NumericValueControl_Leave;
            }
        }

        private void nud_KeyUp(object sender, KeyEventArgs e) {
            if (base.HandleKeyUp(e.KeyCode, ConvertToNumericType(_nud.Value)))
                _prevValue = __Value.__Value;
        }

        private void ParentForm_Leave(object sender, EventArgs e) { HandleValueChangedOnLeave(); }
        private void NumericValueControl_Leave(object sender, EventArgs e) { HandleValueChangedOnLeave(); }

        private void ParentForm_FormClosing(object sender, FormClosingEventArgs e) {
            ParentForm.FormClosing -= ParentForm_FormClosing;
            ParentForm.Leave -= ParentForm_Leave;
            Leave -= NumericValueControl_Leave;

            HandleValueChangedOnLeave();
        }

        private void HandleValueChangedOnLeave() {
            object value = ConvertToNumericType(_nud.Value);

            if (_nud.Focused && __Value.__Value != null && value != null && !__Value.__Value.Equals(value))
                base.HandleValueChanged(value);

            _prevValue = __Value.__Value;
        }
        private void HandleValueChangedAndFocusLoss() {
            try {
                object value = ConvertToNumericType(_nud.Value);

                bool focus = false;
                if (_nud.Focused && __Value.__Value != null && value != null) {
                    if (!__Value.__Value.Equals(_prevValue))
                        base.HandleValueChanged(_prevValue);
                    if (!__Value.__Value.Equals(value))
                        base.HandleValueChanged(value);
                    focus = true;
                }

                //Ugly but works.
                if (focus && ParentForm != null) {
                    try {
                        if (ParentForm.MdiParent == null) {
                            ParentForm.TopMost = true;
                            ParentForm.TopMost = false;
                            ParentForm.Activate();
                        } else {
                            ParentForm.MdiParent.TopMost = true;
                            ParentForm.MdiParent.TopMost = false;
                            ParentForm.MdiParent.Activate();
                        }
                    } catch {
                    }

                    Application.DoEvents();
                    _nud.Select();
                }
            } catch {
            }
        }

        private object ConvertToNumericType(decimal value) {
            Type numericType = base.__Value.__Value.GetType();
            if (numericType == typeof(short))
                return Convert.ToInt16(value);
            if (numericType == typeof(int))
                return Convert.ToInt32(value);
            if (numericType == typeof(long))
                return Convert.ToInt64(value);
            if (numericType == typeof(ushort))
                return Convert.ToUInt16(value);
            if (numericType == typeof(uint))
                return Convert.ToUInt32(value);
            if (numericType == typeof(ulong))
                return Convert.ToUInt64(value);
            if (numericType == typeof(float))
                return Convert.ToSingle(value);
            if (numericType == typeof(double))
                return Convert.ToDouble(value);
            return Convert.ToDecimal(value);
        }

        protected override void RevertToDefaultValueOnGui() {
            var nud = base.ValueControl as NumericUpDown;
            nud.Value = Convert.ToDecimal(base.__Value.DefaultValue);
        }
    }
}