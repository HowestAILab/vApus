using System;
using System.Collections.Generic;
using C = DocumentFormat.OpenXml.Drawing.Charts;
using SLA = SpreadsheetLight.Drawing;

namespace SpreadsheetLight.Charts
{
    /// <summary>
    /// Encapsulates properties and methods for setting chart legends.
    /// This simulates the DocumentFormat.OpenXml.Drawing.Charts.Legend class.
    /// </summary>
    public class SLLegend
    {
        /// <summary>
        /// The position of the legend.
        /// </summary>
        public C.LegendPositionValues LegendPosition { get; set; }

        /// <summary>
        /// Specifies if the legend is overlayed. True if the legend overlaps the plot area, false otherwise.
        /// </summary>
        public bool Overlay { get; set; }

        internal SLA.SLShapeProperties ShapeProperties;

        /// <summary>
        /// Fill properties.
        /// </summary>
        public SLA.SLFill Fill { get { return this.ShapeProperties.Fill; } }

        /// <summary>
        /// Border properties.
        /// </summary>
        public SLA.SLLinePropertiesType Border { get { return this.ShapeProperties.Outline; } }

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
        
        internal SLLegend(List<System.Drawing.Color> ThemeColors)
        {
            this.LegendPosition = C.LegendPositionValues.Right;
            this.Overlay = false;
            this.ShapeProperties = new SLA.SLShapeProperties(ThemeColors);
            this.ShapeProperties.Fill.BlipDpi = 0;
            this.ShapeProperties.Fill.BlipRotateWithShape = true;
        }

        internal C.Legend ToLegend()
        {
            C.Legend l = new C.Legend();
            l.LegendPosition = new C.LegendPosition() { Val = this.LegendPosition };

            l.Append(new C.Layout());
            l.Append(new C.Overlay() { Val = this.Overlay });

            if (this.ShapeProperties.HasShapeProperties) l.Append(this.ShapeProperties.ToChartShapeProperties());

            return l;
        }

        internal SLLegend Clone()
        {
            SLLegend l = new SLLegend(this.ShapeProperties.listThemeColors);
            l.LegendPosition = this.LegendPosition;
            l.Overlay = this.Overlay;
            l.ShapeProperties = this.ShapeProperties.Clone();

            return l;
        }
    }
}
