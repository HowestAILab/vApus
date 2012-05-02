/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Drawing;

namespace vApus.Util
{
    /// <summary>
    /// key = x value, value = y value
    /// </summary>
    public class Series : Dictionary<string, float>
    {
        internal event EventHandler SeriesPropertiesChanged;

        #region Fields
        public string Label;
        public string Instance;
        public Color Color;
        public bool Visible = true;
        public bool IsHighlighted;
        private float _maximumY = 100.0F;

        public const float XVALUEMARGIN = 10F;
        #endregion

        #region Properties
        internal float MaximumY
        {
            get { return _maximumY; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// key = x value, value = y value
        /// </summary>
        /// <param name="label"></param>
        public Series(string label, string instance = "")
        {
            if (label == null || label == string.Empty)
                throw new ArgumentNullException("label");
            Label = label;
            Instance = instance;
        }
        #endregion

        #region Functions
        internal void ScaleMaximumY(float newYValue)
        {
            if (newYValue > _maximumY)
                _maximumY = newYValue;
        }        
        /// <summary>
        /// To let the chart control and the x axis know a property has changed.
        /// </summary>
        internal void InvokeSeriesPropertiesChanged()
        {
            if (SeriesPropertiesChanged != null)
                SeriesPropertiesChanged(this, null);
        }
        /// <summary>
        /// To let the chart control and the x axis know a property has changed.
        /// </summary>
        internal void InvokeAllSeriesPropertiesChanged()
        {
            if (SeriesPropertiesChanged != null)
                SeriesPropertiesChanged(null, null);
        }
        #endregion
    }
}
