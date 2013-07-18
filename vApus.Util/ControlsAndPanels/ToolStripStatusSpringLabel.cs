/*
 * Copyright 2012 (c) Sizing Servers Lab 
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
//Thanks to Hans Passant http://stackoverflow.com/users/17034/hans-passant

using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace vApus.Util {
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