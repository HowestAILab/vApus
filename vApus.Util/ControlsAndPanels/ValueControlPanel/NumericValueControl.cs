/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;

namespace vApus.Util
{
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
            NumericUpDown nud = null;

            if (base.ValueControl == null)
            {
                nud = new NumericUpDown();

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
                nud.TextChanged += new EventHandler(nud_TextChanged);
                nud.Leave += new EventHandler(nud_Leave);
                nud.KeyUp += new KeyEventHandler(nud_KeyUp);
            }
            else
            {
                nud = base.ValueControl as NumericUpDown;
            }

            nud.Value = Convert.ToDecimal(value.__Value);

            base.ValueControl = nud;
        }

        private void nud_TextChanged(object sender, EventArgs e)
        {
            NumericUpDown nud = sender as NumericUpDown;
            base.HandleValueChanged(nud.Value);
        }
        private void nud_KeyUp(object sender, KeyEventArgs e)
        {
            NumericUpDown nud = sender as NumericUpDown;
            base.HandleKeyUp(e.KeyCode, nud.Value);
        }
        private void nud_Leave(object sender, EventArgs e)
        {
            NumericUpDown nud = sender as NumericUpDown;
            base.HandleValueChanged(nud.Value);
        }
    }
}
