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
        public NumericValueControl() {
            InitializeComponent();
        }

        public void Init(Value value) {
            base.__Value = value;

            //Only take the value into account, the other properties are taken care off.
            //Keep control recycling in mind.
            FixedNumericUpDown nud = null;

            if (base.ValueControl == null) {
                nud = new FixedNumericUpDown();

                if (value.__Value is short) {
                    nud.Minimum = short.MinValue;
                    nud.Maximum = short.MaxValue;
                } else if (value.__Value is int) {
                    nud.Minimum = value.AllowedMinimum;
                    nud.Maximum = value.AllowedMaximum;
                } else if (value.__Value is long) {
                    nud.Minimum = long.MinValue;
                    nud.Maximum = long.MaxValue;
                } else if (value.__Value is ushort) {
                    nud.Minimum = ushort.MinValue;
                    nud.Maximum = ushort.MaxValue;
                } else if (value.__Value is uint) {
                    nud.Minimum = uint.MinValue;
                    nud.Maximum = uint.MaxValue;
                } else if (value.__Value is ulong) {
                    nud.Minimum = ulong.MinValue;
                    nud.Maximum = ulong.MaxValue;
                } else {
                    nud.Minimum = decimal.MinValue;
                    nud.Maximum = decimal.MaxValue;

                    nud.DecimalPlaces = 3;
                }

                nud.Dock = DockStyle.Fill;

                nud.Leave += nud_Leave;
                nud.KeyUp += nud_KeyUp;
                nud.ValueChanged += nud_ValueChanged;
            } else {
                nud = base.ValueControl as FixedNumericUpDown;
            }

            nud.ValueChanged -= nud_ValueChanged;
            nud.Value = Convert.ToDecimal(value.__Value);
            nud.ValueChanged += nud_ValueChanged;

            base.ValueControl = nud;
        }

        private void nud_KeyUp(object sender, KeyEventArgs e) {
            var nud = sender as FixedNumericUpDown;
            base.HandleKeyUp(e.KeyCode, ConvertToNumericType(nud.Value));
        }

        private void nud_Leave(object sender, EventArgs e) {
            HandleValueChangedAndFocusLoss(sender as FixedNumericUpDown);
        }
        private void nud_ValueChanged(object sender, EventArgs e) {
            HandleValueChangedAndFocusLoss(sender as FixedNumericUpDown);
        }

        private void HandleValueChangedAndFocusLoss(FixedNumericUpDown nud) {
            try {
                object value = ConvertToNumericType(nud.Value);

                if (nud.Focused && __Value.__Value != null && value != null && !__Value.__Value.Equals(value)) {
                    base.HandleValueChanged(value);

                    //Ugly but works.
                    if (ParentForm != null)
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
                    nud.Select();
                }
            } catch { }
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
            nud.ValueChanged -= nud_ValueChanged;
            nud.Value = Convert.ToDecimal(base.__Value.DefaultValue);
            nud.ValueChanged += nud_ValueChanged;
        }
    }
}