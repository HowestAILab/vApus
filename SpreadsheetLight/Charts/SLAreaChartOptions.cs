using System;

namespace SpreadsheetLight.Charts
{
    /// <summary>
    /// Chart customization options for area charts.
    /// </summary>
    public class SLAreaChartOptions
    {
        internal ushort iGapDepth;
        /// <summary>
        /// The gap depth between area clusters (between different data series) as a percentage of width, ranging between 0% and 500% (both inclusive). The default is 150%. This is only used for 3D chart version.
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
        /// Initializes an instance of SLAreaChartOptions.
        /// </summary>
        public SLAreaChartOptions()
        {
            this.iGapDepth = 150;
        }

        internal SLAreaChartOptions Clone()
        {
            SLAreaChartOptions aco = new SLAreaChartOptions();
            aco.iGapDepth = this.iGapDepth;

            return aco;
        }
    }
}
