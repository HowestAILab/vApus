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

namespace vApus.Util
{
    [ToolboxItem(false)]
    public partial class NumericValueControl : BaseValueControl, IValueControl
    {
        public NumericValueControl()
        {
            InitializeComponent();
        }

        public void Init(BaseValueControl.Value value)
        {
            base.__Value = value;

            //Only take the value into account, the other properties are taken care off.
            //Keep control recycling in mind.
            FixedNumericUpDown nud = null;

            if (base.ValueControl == null)
            {
                nud = new FixedNumericUpDown();

                if (value.__Value is short)
                {
                    nud.Minimum = short.MinValue;
                    nud.Maximum = short.MaxValue;
                }
                else if (value.__Value is int)
                {
                    nud.Minimum = int.MinValue;
                    nud.Maximum = int.MaxValue;
                }
                else if (value.__Value is long)
                {
                    nud.Minimum = long.MinValue;
                    nud.Maximum = long.MaxValue;
                }
                else if (value.__Value is ushort)
                {
                    nud.Minimum = ushort.MinValue;
                    nud.Maximum = ushort.MaxValue;
                }
                else if (value.__Value is uint)
                {
                    nud.Minimum = uint.MinValue;
                    nud.Maximum = uint.MaxValue;
                }
                else if (value.__Value is ulong)
                {
                    nud.Minimum = ulong.MinValue;
                    nud.Maximum = ulong.MaxValue;
                }
                else
                {
                    nud.Minimum = decimal.MinValue;
                    nud.Maximum = decimal.MaxValue;

                    nud.DecimalPlaces = 3;
                }

                nud.Dock = DockStyle.Fill;

                //Use text changed rather then value changed, value changed is not invoked when the text is changed.
                nud.Leave += new EventHandler(nud_Leave);
                nud.KeyUp += new KeyEventHandler(nud_KeyUp);
            }
            else
            {
                nud = base.ValueControl as FixedNumericUpDown;
            }

            nud.Value = Convert.ToDecimal(value.__Value);

            base.ValueControl = nud;
        }

        private void nud_KeyUp(object sender, KeyEventArgs e)
        {
            FixedNumericUpDown nud = sender as FixedNumericUpDown;
            base.HandleKeyUp(e.KeyCode, ConvertToNumericType(nud.Value));
        }
        private void nud_Leave(object sender, EventArgs e)
        {
            FixedNumericUpDown nud = sender as FixedNumericUpDown;
            base.HandleValueChanged(ConvertToNumericType(nud.Value));
        }
        private object ConvertToNumericType(decimal value)
        {
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
    }
}
