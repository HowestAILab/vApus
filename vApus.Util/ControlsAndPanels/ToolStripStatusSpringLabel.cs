/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT 
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
//Thanks to Hans Passant http://stackoverflow.com/users/17034/hans-passant
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace vApus.Util {
    /// <summary>
    /// To spring the label to the end.
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripStatusSpringLabel : ToolStripStatusLabel {
        protected override void OnPaint(PaintEventArgs e) {
            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.EndEllipsis;
            var bounds = new Rectangle(0, 0, Bounds.Width, Bounds.Height);

            Font font = IsLink ? new Font(Font, FontStyle.Underline) : Font;
            Color foreColor = IsLink ? LinkColor : ForeColor;
            TextRenderer.DrawText(e.Graphics, Text, font, bounds, foreColor, flags);
        }
    }
}