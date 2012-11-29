/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;

namespace vApus.Results.Metrics.Monitor
{
    public class MonitorMetrics
    {
        public DateTime StartMeasuringRuntime { get; set; }
        public TimeSpan EstimatedTimeLeft { get; set; }
        public TimeSpan MeasuredRunTime { get; set; }
        public int ConcurrentUsers { get; set; }

        /// <summary>
        ///     Stays 0 for concurrency level metrics.
        /// </summary>
        public int Run { get; set; }

        public List<float> AverageMonitorResults { get; set; }
    }
}