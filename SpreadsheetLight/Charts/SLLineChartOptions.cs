using System;

namespace SpreadsheetLight.Charts
{
    /// <summary>
    /// Chart customization options for line charts.
    /// </summary>
    public class SLLineChartOptions
    {
        internal ushort iGapDepth;
        /// <summary>
        /// The gap depth between line clusters (between different data series) as a percentage of bar or column width, ranging between 0% and 500% (both inclusive). The default is 150%. This is only used for 3D chart version.
        /// </summary>
        public ushort GapDepth
        {
            get { return iGapDepth; }
            set
            {
                iGapDepth = value;
                if (iGapDepth > 500) iGapDepth = 500;
            }
        }

        /// <summary>
        /// Whether the line connecting data points use C splines (instead of straight lines).
        /// </summary>
        public bool Smooth { get; set; }

        /// <summary>
        /// Initializes an instance of SLLineChartOptions.
        /// </summary>
        public SLLineChartOptions()
        {
            this.iGapDepth = 150;
            this.Smooth = false;
        }

        internal SLLineChartOptions Clone()
        {
            SLLineChartOptions lco = new SLLineChartOptions();
            lco.iGapDepth = this.iGapDepth;
            lco.Smooth = this.Smooth;

            return lco;
        }
    }
}
