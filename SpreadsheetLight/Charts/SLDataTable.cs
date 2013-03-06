using System;
using System.Collections.Generic;
using A = DocumentFormat.OpenXml.Drawing;
using C = DocumentFormat.OpenXml.Drawing.Charts;
using SLA = SpreadsheetLight.Drawing;

namespace SpreadsheetLight.Charts
{
    /// <summary>
    /// Encapsulates properties and methods for setting the data table of charts.
    /// </summary>
    public class SLDataTable
    {
        internal SLA.SLShapeProperties ShapeProperties;

        /// <summary>
        /// Specifies if horizontal table borders are shown.
        /// </summary>
        public bool ShowHorizontalBorder { get; set; }

        /// <summary>
        /// Specifies if vertical table borders are shown.
        /// </summary>
        public bool ShowVerticalBorder { get; set; }

        /// <summary>
        /// Specifies if table outline borders are shown.
        /// </summary>
        public bool ShowOutlineBorder { get; set; }

        /// <summary>
        /// Specifies if legend keys are shown.
        /// </summary>
        public bool ShowLegendKeys { get; set; }

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

        /// <summary>
        /// 3D format properties.
        /// </summary>
        public SLA.SLFormat3D Format3D { get { return this.ShapeProperties.Format3D; } }

        internal SLFont Font { get; set; }

        internal SLDataTable(List<System.Drawing.Color> ThemeColors)
        {
            this.ShowHorizontalBorder = true;
            this.ShowVerticalBorder = true;
            this.ShowOutlineBorder = true;
            this.ShowLegendKeys = true;
            this.ShapeProperties = new SLA.SLShapeProperties(ThemeColors);
            this.Font = null;
        }

        /// <summary>
        /// Set font settings for the contents of the data table.
        /// </summary>
        /// <param name="Font">The SLFont containing the font settings.</param>
        public void SetFont(SLFont Font)
        {
            this.Font = Font.Clone();
        }

        internal C.DataTable ToDataTable()
        {
            C.DataTable dt = new C.DataTable();

            if (this.ShowHorizontalBorder) dt.ShowHorizontalBorder = new C.ShowHorizontalBorder() { Val = true };
            if (this.ShowVerticalBorder) dt.ShowVerticalBorder = new C.ShowVerticalBorder() { Val = true };
            if (this.ShowOutlineBorder) dt.ShowOutlineBorder = new C.ShowOutlineBorder() { Val = true };
            if (this.ShowLegendKeys) dt.ShowKeys = new C.ShowKeys() { Val = true };

            if (this.ShapeProperties.HasShapeProperties) dt.ChartShapeProperties = this.ShapeProperties.ToChartShapeProperties();

            if (this.Font != null)
            {
                dt.TextProperties = new C.TextProperties();
                dt.TextProperties.BodyProperties = new A.BodyProperties();
                dt.TextProperties.ListStyle = new A.ListStyle();

                dt.TextProperties.Append(this.Font.ToParagraph());
            }

            return dt;
        }

        internal SLDataTable Clone()
        {
            SLDataTable dt = new SLDataTable(this.ShapeProperties.listThemeColors);
            dt.ShapeProperties = this.ShapeProperties.Clone();
            dt.ShowHorizontalBorder = this.ShowHorizontalBorder;
            dt.ShowVerticalBorder = this.ShowVerticalBorder;
            dt.ShowOutlineBorder = this.ShowOutlineBorder;
            dt.ShowLegendKeys = this.ShowLegendKeys;
            if (this.Font != null) dt.Font = this.Font.Clone();

            return dt;
        }
    }
}
