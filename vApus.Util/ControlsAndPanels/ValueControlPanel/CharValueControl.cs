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
    public partial class CharValueControl : BaseValueControl, IValueControl
    {
        public CharValueControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     This inits the control with event handling.
        /// </summary>
        /// <param name="value"></param>
        public void Init(Value value)
        {
            base.__Value = value;

            //Only take the value into account, the other properties are taken care off.
            //Keep control recycling in mind.
            TextBox txt = null;

            if (base.ValueControl == null)
            {
                txt = new TextBox();

                txt.MaxLength = 1;
                txt.Dock = DockStyle.Fill;

                txt.Leave += txt_Leave;
                txt.KeyUp += txt_KeyUp;
            }
            else
            {
                txt = base.ValueControl as TextBox;
            }

            txt.Text = value.__Value.ToString();

            base.ValueControl = txt;
        }

        private void txt_KeyUp(object sender, KeyEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt.Text.Length != 0)
                base.HandleKeyUp(e.KeyCode, txt.Text[0]);
        }

        private void txt_Leave(object sender, EventArgs e)
        {
            try
            {
                var txt = sender as TextBox;
                if (txt.Text.Length == 0)
                    txt.Text = base.__Value.__Value.ToString();
                else
                    base.HandleValueChanged(txt.Text[0]);
            }
            catch
            {
            }
        }
    }
}