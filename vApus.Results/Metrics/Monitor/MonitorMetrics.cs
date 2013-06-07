/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;

namespace vApus.Results {
    public class MonitorMetrics {
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
        public float[] AverageMonitorResults { get; set; }

        public MonitorMetrics() {
            Headers = new string[0];
            AverageMonitorResults = new float[0];
        }
    }
}