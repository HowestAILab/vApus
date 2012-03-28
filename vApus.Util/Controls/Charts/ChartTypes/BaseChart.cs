/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace vApus.Util
{
    [ToolboxItem(false)]
    public abstract partial class BaseChart : Panel
    {
        #region Fields
        protected float StartXOffset;
        protected List<Series> SeriesCollection = new List<Series>();

        protected float _maximumY;

        protected const float NORMALSTROKEWITH = 3.0f, HIGHGLIGHTSTROKEWIDTH = 5.0f;
        private ChartViewState _chartViewState;

        private float _xDrawOffset, _viewDrawOffset;
        #endregion

        #region Properties
        public ChartViewState ChartViewState
        {
            get { return _chartViewState; }
            set { _chartViewState = value; }
        }
        public float XDrawOffset
        {
            get { return _xDrawOffset; }
            set { _xDrawOffset = value; }
        }
        public float ViewDrawOffset
        {
            get { return _viewDrawOffset; }
            set { _viewDrawOffset = value; }
        }
        #endregion

        #region Functions
        public void AddSeries(Series series)
        {
            if (!SeriesCollection.Contains(series))
            {
                SeriesCollection.Add(series);
                series.SeriesPropertiesChanged += new EventHandler(series_SeriesPropertiesChanged);
            }
        }
        public void series_SeriesPropertiesChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
        public Series AddSeriesXYValue(int seriesIndex, string xValue, float yValue)
        {
            Series series = SeriesCollection[seriesIndex];
            series.Add(xValue, yValue);
            series.ScaleMaximumY(yValue);
            return series;
        }
        public void AddSeriesXYValue(Series series, string xValue, float yValue)
        {
            if (!series.ContainsKey(xValue))
            {
                series.Add(xValue, yValue);
                series.ScaleMaximumY(yValue);
            }
        }
        public Series GetSeriesAt(int index)
        {
            return SeriesCollection[index];
        }
        /// <summary>
        /// Use this in a OnPaint Event or a Message Loop mechanism.
        /// Make sure the device is created or the graphics is not null and all that before calling this!
        /// </summary>
        protected abstract void Render();
        /// <summary>
        /// Normally used in Render(), this could be done in a Base<InsertDrawingLibHere>ChartControlClass, unlike DrawSeries()
        /// </summary>
        protected abstract void DrawGridLines();
        /// <summary>
        /// Normally used in Render(), this serves to draw a single Series.
        /// </summary>
        /// <param name="Series"></param>
        protected abstract void DrawSeries(Series Series);
        #endregion
    }
}
