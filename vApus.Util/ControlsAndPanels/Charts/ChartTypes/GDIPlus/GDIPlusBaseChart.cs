/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.ComponentModel;
using System.Drawing;

namespace vApus.Util
{
    [ToolboxItem(false)]
    public abstract partial class GDIPlusBaseChart : BaseChart
    {
        #region Fields
        protected Pen Pen = new Pen(Color.Gray);
        protected Graphics Graphics;
        #endregion

        #region Constructor
        public GDIPlusBaseChart()
        {
            this.DoubleBuffered = true;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Make sure the field Graphics is not null.
        /// </summary>
        protected override void DrawGridLines()
        {
            Pen.Color = Color.LightGray;
            Pen.Width = 0.5F;
            float step = (float)Height / 5;

            for (float y = 0; y <= Height; y += step)
            {
                if (y == Height)
                    y = Height - Pen.Width;
                Graphics.DrawLine(Pen, 0, y, Width, y);
            }
        }
        #endregion
    }
}
