using System.Drawing;
/*
 * Copyright 2006 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using System.Windows.Forms;

namespace vApus.Util
{
    public class ToggleButton : CheckBox
    {
        public ToggleButton()
        {
            this.Appearance = Appearance.Button;
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.AutoSize = true;

            SetUncheckedStyle();
        }
        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            base.OnKeyUp(kevent);
            if (kevent.KeyCode == Keys.Enter)
                Checked = !Checked;
        }
        protected override void OnCheckedChanged(System.EventArgs e)
        {
            if (Checked) SetCheckedStyle(); else SetUncheckedStyle();
            base.OnCheckedChanged(e);
        }
        private void SetUncheckedStyle() 
        {
            this.ForeColor = Color.Blue;
            this.BackColor = Color.White;
            this.Font = new Font(Font, FontStyle.Regular | FontStyle.Underline);
        }
        private void SetCheckedStyle() 
        {
            this.ForeColor = Color.Black;
            this.BackColor = SystemColors.Control;
            this.Font = new Font(Font, FontStyle.Bold);

        }
    }
}