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
    public partial class StringValueControl : BaseValueControl, IValueControl
    {
        public StringValueControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This inits the control with event handling.
        /// </summary>
        /// <param name="value"></param>
        public void Init(BaseValueControl.Value value)
        {
            base.__Value = value;

            //Only take the value into account, the other properties are taken care off.
            //Keep control recycling in mind.
            TextBox txt = null;

            if (base.ValueControl == null)
            {
                txt = new TextBox();
                txt.Dock = DockStyle.Fill;

                txt.TextChanged += new EventHandler(txt_TextChanged);
                txt.Leave += new EventHandler(txt_Leave);
                txt.KeyUp += new KeyEventHandler(txt_KeyUp);
            }
            else
            {
                txt = base.ValueControl as TextBox;
            }

            txt.TextChanged -= txt_TextChanged;
            txt.Text = value.__Value as string;
            txt.TextChanged += txt_TextChanged;

            base.ValueControl = txt;
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            base.HandleValueChanged(txt.Text);
        }
        private void txt_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            base.HandleKeyUp(e.KeyCode, txt.Text);
        }
        private void txt_Leave(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            base.HandleValueChanged(txt.Text);
        }
    }
}
