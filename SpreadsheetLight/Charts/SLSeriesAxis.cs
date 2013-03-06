using System;
using System.Collections.Generic;
using A = DocumentFormat.OpenXml.Drawing;
using C = DocumentFormat.OpenXml.Drawing.Charts;

namespace SpreadsheetLight.Charts
{
    /// <summary>
    /// Encapsulates properties and methods for setting series axes in charts.
    /// This simulates the DocumentFormat.OpenXml.Drawing.Charts.SeriesAxis class.
    /// </summary>
    public class SLSeriesAxis : EGAxShared
    {
        internal ushort iTickLabelSkip;
        /// <summary>
        /// This is the interval between labels, and is at least 1. A suggested range is 1 to 255 (both inclusive).
        /// </summary>
        public ushort TickLabelSkip
        {
            get { return iTickLabelSkip; }
            set
            {
                iTickLabelSkip = value;
                if (iTickLabelSkip < 1) iTickLabelSkip = 1;
            }
        }

        internal ushort iTickMarkSkip;
        /// <summary>
        /// This is the interval between tick marks, and is at least 1. A suggested range is 1 to 31999 (both inclusive).
        /// </summary>
        public ushort TickMarkSkip
        {
            get { return iTickMarkSkip; }
            set
            {
                iTickMarkSkip = value;
                if (iTickMarkSkip < 1) iTickMarkSkip = 1;
            }
        }

        internal SLSeriesAxis(List<System.Drawing.Color> ThemeColors) : base(ThemeColors)
        {
            this.iTickLabelSkip = 1;
            this.iTickMarkSkip = 1;
        }

        internal C.SeriesAxis ToSeriesAxis()
        {
            C.SeriesAxis sa = new C.SeriesAxis();
            sa.AxisId = new C.AxisId() { Val = this.AxisId };

            sa.Scaling = new C.Scaling();
            sa.Scaling.Orientation = new C.Orientation() { Val = this.Orientation };
            if (this.LogBase != null) sa.Scaling.LogBase = new C.LogBase() { Val = this.LogBase.Value };
            if (this.MaxAxisValue != null) sa.Scaling.MaxAxisValue = new C.MaxAxisValue() { Val = this.MaxAxisValue.Value };
            if (this.MinAxisValue != null) sa.Scaling.MinAxisValue = new C.MinAxisValue() { Val = this.MinAxisValue.Value };

            sa.Delete = new C.Delete() { Val = this.Delete };

            sa.AxisPosition = new C.AxisPosition() { Val = this.AxisPosition };

            if (this.ShowMajorGridlines)
            {
                sa.MajorGridlines = this.MajorGridlines.ToMajorGridlines();
            }

            if (this.ShowMinorGridlines)
            {
                sa.MinorGridlines = this.MinorGridlines.ToMinorGridlines();
            }

            if (this.ShowTitle)
            {
                sa.Title = this.Title.ToTitle();
            }

            if (this.HasNumberingFormat)
            {
                sa.NumberingFormat = new C.NumberingFormat()
                {
                    FormatCode = this.FormatCode,
                    SourceLinked = this.SourceLinked
                };
            }

            sa.MajorTickMark = new C.MajorTickMark() { Val = this.MajorTickMark };
            sa.MinorTickMark = new C.MinorTickMark() { Val = this.MinorTickMark };
            sa.TickLabelPosition = new C.TickLabelPosition() { Val = this.TickLabelPosition };

            if (this.ShapeProperties.HasShapeProperties) sa.ChartShapeProperties = this.ShapeProperties.ToChartShapeProperties();

            if (this.Rotation != null || this.Vertical != null || this.Anchor != null || this.AnchorCenter != null)
            {
                sa.TextProperties = new C.TextProperties();
                sa.TextProperties.BodyProperties = new A.BodyProperties();
                if (this.Rotation != null) sa.TextProperties.BodyProperties.Rotation = (int)(this.Rotation.Value * SLConstants.DegreeToAngleRepresentation);
                if (this.Vertical != null) sa.TextProperties.BodyProperties.Vertical = this.Vertical.Value;
                if (this.Anchor != null) sa.TextProperties.BodyProperties.Anchor = this.Anchor.Value;
                if (this.AnchorCenter != null) sa.TextProperties.BodyProperties.AnchorCenter = this.AnchorCenter.Value;

                sa.TextProperties.ListStyle = new A.ListStyle();

                A.Paragraph para = new A.Paragraph();
                para.ParagraphProperties = new A.ParagraphProperties();
                para.ParagraphProperties.Append(new A.DefaultRunProperties());
                sa.TextProperties.Append(para);
            }

            sa.CrossingAxis = new C.CrossingAxis() { Val = this.CrossingAxis };

            if (this.IsCrosses != null)
            {
                if (this.IsCrosses.Value)
                {
                    sa.Append(new C.Crosses() { Val = this.Crosses });
                }
                else
                {
                    sa.Append(new C.CrossesAt() { Val = this.CrossesAt });
                }
            }

            if (this.iTickLabelSkip > 1) sa.Append(new C.TickLabelSkip() { Val = this.TickLabelSkip });
            if (this.iTickMarkSkip > 1) sa.Append(new C.TickMarkSkip() { Val = this.TickMarkSkip });

            return sa;
        }

        internal SLSeriesAxis Clone()
        {
            SLSeriesAxis sa = new SLSeriesAxis(this.ShapeProperties.listThemeColors);
            sa.Rotation = this.Rotation;
            sa.Vertical = this.Vertical;
            sa.Anchor = this.Anchor;
            sa.AnchorCenter = this.AnchorCenter;
            sa.AxisId = this.AxisId;
            sa.fLogBase = this.fLogBase;
            sa.Orientation = this.Orientation;
            sa.MaxAxisValue = this.MaxAxisValue;
            sa.MinAxisValue = this.MinAxisValue;
            sa.OtherAxisIsInReverseOrder = this.OtherAxisIsInReverseOrder;
            sa.OtherAxisCrossedAtMaximum = this.OtherAxisCrossedAtMaximum;
            sa.Delete = this.Delete;
            sa.AxisPosition = this.AxisPosition;
            sa.ShowMajorGridlines = this.ShowMajorGridlines;
            sa.MajorGridlines = this.MajorGridlines.Clone();
            sa.ShowMinorGridlines = this.ShowMinorGridlines;
            sa.MinorGridlines = this.MinorGridlines.Clone();
            sa.ShowTitle = this.ShowTitle;
            sa.Title = this.Title.Clone();
            sa.HasNumberingFormat = this.HasNumberingFormat;
            sa.sFormatCode = this.sFormatCode;
            sa.bSourceLinked = this.bSourceLinked;
            sa.MajorTickMark = this.MajorTickMark;
            sa.MinorTickMark = this.MinorTickMark;
            sa.TickLabelPosition = this.TickLabelPosition;
            sa.ShapeProperties = this.ShapeProperties.Clone();
            sa.CrossingAxis = this.CrossingAxis;
            sa.IsCrosses = this.IsCrosses;
            sa.Crosses = this.Crosses;
            sa.CrossesAt = this.CrossesAt;
            sa.OtherAxisIsCrosses = this.OtherAxisIsCrosses;
            sa.OtherAxisCrosses = this.OtherAxisCrosses;
            sa.OtherAxisCrossesAt = this.OtherAxisCrossesAt;

            sa.iTickLabelSkip = this.iTickLabelSkip;
            sa.iTickMarkSkip = this.iTickMarkSkip;

            return sa;
        }
    }
}
