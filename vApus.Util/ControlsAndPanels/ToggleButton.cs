using System;
using System.Drawing;
using System.Windows.Forms;
/*
 * Copyright 2006 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */

namespace vApus.Util
{
    public class ToggleButton : CheckBox
    {
        public ToggleButton()
        {
            Appearance = Appearance.Button;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            TextAlign = ContentAlignment.MiddleCenter;
            AutoSize = true;

            SetUncheckedStyle();
        }

        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            base.OnKeyUp(kevent);
            if (kevent.KeyCode == Keys.Enter)
                Checked = !Checked;
        }

        protected override void OnCheckedChanged(EventArgs e)
        {
            if (Checked) SetCheckedStyle();
            else SetUncheckedStyle();
            base.OnCheckedChanged(e);
        }

        private void SetUncheckedStyle()
        {
            ForeColor = Color.Blue;
            BackColor = Color.White;
            Font = new Font(Font, FontStyle.Regular | FontStyle.Underline);
        }

        private void SetCheckedStyle()
        {
            ForeColor = Color.Black;
            BackColor = SystemColors.Control;
            Font = new Font(Font, FontStyle.Bold);
        }
    }
}