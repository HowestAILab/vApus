/*
 * Copyright 2012 (c) Sizing Servers Lab 
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
//Thanks to Hans Passant http://stackoverflow.com/users/17034/hans-passant
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace vApus.Util
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripStatusSpringLabel : ToolStripStatusLabel
    {
        public ToolStripStatusSpringLabel()
        {
            //this.Spring = true;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var flags = TextFormatFlags.Left | TextFormatFlags.EndEllipsis;
            var bounds = new Rectangle(0, 0, this.Bounds.Width, this.Bounds.Height);

            Font font = this.IsLink ? new Font(this.Font, FontStyle.Underline) : this.Font;
            Color foreColor = this.IsLink ? this.LinkColor : this.ForeColor;
            TextRenderer.DrawText(e.Graphics, this.Text, font, bounds, foreColor, flags);
        }
    }
}