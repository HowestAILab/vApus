using System;
using System.Collections.Generic;
using C = DocumentFormat.OpenXml.Drawing.Charts;
using SLA = SpreadsheetLight.Drawing;

namespace SpreadsheetLight.Charts
{
    /// <summary>
    /// Encapsulates properties and methods for setting chart minor gridlines in charts.
    /// This simulates the DocumentFormat.OpenXml.Drawing.Charts.MinorGridlines class.
    /// </summary>
    public class SLMinorGridlines
    {
        internal SLA.SLShapeProperties ShapeProperties { get; set; }

        /// <summary>
        /// Line properties.
        /// </summary>
        public SLA.SLLinePropertiesType Line { get { return this.ShapeProperties.Outline; } }

        /// <summary>
        /// Shadow properties.
        /// </summary>
        public SLA.SLShadowEffect Shadow { get { return this.ShapeProperties.EffectList.Shadow; } }

        /// <summary>
        /// Glow properties.
        /// </summary>
        public SLA.SLGlow Glow { get { return this.ShapeProperties.EffectList.Glow; } }

        /// <summary>
        /// Soft edge properties.
        /// </summary>
        public SLA.SLSoftEdge SoftEdge { get { return this.ShapeProperties.EffectList.SoftEdge; } }

        internal SLMinorGridlines(List<System.Drawing.Color> ThemeColors)
        {
            this.ShapeProperties = new SLA.SLShapeProperties(ThemeColors);
        }

        internal C.MinorGridlines ToMinorGridlines()
        {
            C.MinorGridlines mgl = new C.MinorGridlines();

            if (this.ShapeProperties.HasShapeProperties) mgl.ChartShapeProperties = this.ShapeProperties.ToChartShapeProperties();

            return mgl;
        }

        internal SLMinorGridlines Clone()
        {
            SLMinorGridlines mgl = new SLMinorGridlines(this.ShapeProperties.listThemeColors);
            mgl.ShapeProperties = this.ShapeProperties.Clone();

            return mgl;
        }
    }
}
