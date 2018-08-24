/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 *    
 * Thx 'NanoWizard' from stackoverflow
 */
using System;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    ///     Fixes the wrong increment/decrement of 3 (to 1) when using the mouse wheel.
    /// </summary>
    public class FixedNumericUpDown : NumericUpDown {
        protected const String UpKey = "{UP}";
        protected const String DownKey = "{DOWN}";

        protected override void OnMouseWheel(MouseEventArgs e_) {
            SendKeys.Send((e_.Delta < 0) ? DownKey : UpKey);
        }
    }
}