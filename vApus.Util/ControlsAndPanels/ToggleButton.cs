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
            this.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.AutoSize = false;
        }
        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            base.OnKeyUp(kevent);
            if (kevent.KeyCode == Keys.Enter)
                Checked = !Checked;
        }
    }
}