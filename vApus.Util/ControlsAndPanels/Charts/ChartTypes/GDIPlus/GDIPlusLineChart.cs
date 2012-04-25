/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Util
{
    [ToolboxItem(false)]
    public partial class GDIPlusLineChart : GDIPlusBaseChart
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics = e.Graphics;

            Render();
        }
        /// <summary>
        /// Make sure the field Graphics is not null.
        /// </summary>
        protected override void Render()
        {
            if (!this.IsHandleCreated)
                throw new Exception("No windowhandle created.");

            Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

            Graphics.Clear(Color.White);

            DrawGridLines();

            Series HighlightedSeries = null;
            List<Series> toDraw = new List<Series>();
            _maximumY = 0F;
            foreach (Series series in SeriesCollection)
                if (series.Count > 1)
                    if (series.Visible)
                    {
                        if (series.IsHighlighted)
                            HighlightedSeries = series;
                        else
                            toDraw.Add(series);
                        if (_maximumY < series.MaximumY)
                            _maximumY = series.MaximumY;
                    }
            foreach (Series series in toDraw)
                DrawSeries(series);

            if (HighlightedSeries != null)
                DrawSeries(HighlightedSeries);
        }
        protected override void DrawSeries(Series series)
        {
            Pen.Color = series.Color;
            Pen.Width = series.IsHighlighted ? HIGHGLIGHTSTROKEWIDTH : NORMALSTROKEWITH;

            float viewDrawOffset = 0F;
            if (ChartViewState == ChartViewState.Expanded)
                viewDrawOffset = ViewDrawOffset;

            float currentX = 0;
            float previousY = -1F;
            foreach (float y in series.Values)
            {
                if (currentX - viewDrawOffset > Width)
                    break;
                if (previousY > -1F)
                {
                    if (currentX >= viewDrawOffset - XDrawOffset)
                        Graphics.DrawLine(Pen, currentX - viewDrawOffset, Height - Height * (previousY / _maximumY),
                            (currentX - viewDrawOffset + XDrawOffset), Height - Height * (y / _maximumY));
                    currentX += XDrawOffset;
                }
                previousY = y;
            }
        }
    }
}
