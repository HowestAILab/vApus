﻿/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;

namespace vApus.Results {
#warning OBSOLETE
    public class MonitorMetrics {
        #region Properties
        /// <summary>
        /// The to string of the monitor as identifier.
        /// </summary>
        public string Monitor { get; set; }

        public DateTime StartMeasuringRuntime { get; set; }
        public TimeSpan MeasuredRunTime { get; set; }
        public int ConcurrentUsers { get; set; }

        /// <summary>
        ///     Stays 0 for concurrency level metrics.
        /// </summary>
        public int Run { get; set; }

        public string[] Headers { get; set; }
        public double[] AverageMonitorResults { get; set; }
        #endregion

        #region Constructor
        public MonitorMetrics() {
            Headers = new string[0];
            AverageMonitorResults = new double[0];
        }
        #endregion
    }
}