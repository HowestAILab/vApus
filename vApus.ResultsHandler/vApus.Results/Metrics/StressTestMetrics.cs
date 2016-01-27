/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using vApus.Util;

namespace vApus.Results {
    /// <summary>
    /// This is serializable because metrics are sent from slaves to master in a distributed test.
    /// </summary>
    [Serializable]
    public class StressTestMetrics {

        #region Fields
        private long _requestsProcessed;
        #endregion

        #region Properties
        public DateTime StartMeasuringTime { get; set; }
        public TimeSpan EstimatedTimeLeft { get; set; }
        /// <summary>
        /// For run sync break on last is this not only the run time, think time between the reruns are included. 
        /// </summary>
        public TimeSpan MeasuredTime { get; set; }
        public int Concurrency { get; set; }

        /// <summary>
        ///     Stays 0 for concurrency level metrics.
        /// </summary>
        public int Run { get; set; }

        /// <summary>
        ///     Stays 0 for concurrency level metrics.
        /// </summary>
        public int RerunCount { get; set; }

        public List<KeyValuePair<DateTime, DateTime>> StartsAndStopsRuns { get; set; }

        public long Requests { get; set; }

        /// <summary>
        ///     Throughput.
        /// </summary>
        public double ResponsesPerSecond { get; set; }

        public double UserActionsPerSecond { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public TimeSpan MaxResponseTime { get; set; }
        public TimeSpan Percentile95thResponseTimes { get; set; }
        public TimeSpan Percentile99thResponseTimes { get; set; }
        public TimeSpan AverageTop5ResponseTimes { get; set; }
        public TimeSpan AverageDelay { get; set; }
        public long Errors { get; set; }
        /// <summary>
        ///     The setter makes sure this cannot exceed the requests count.
        /// </summary>
        public long RequestsProcessed {
            get { return _requestsProcessed; }
            set {
                if (value > Requests) value = Requests;
                _requestsProcessed = value;
            }
        }

        public bool Simplified { get; set; }
        #endregion

        #region Constructors
        public StressTestMetrics() { StartsAndStopsRuns = new List<KeyValuePair<DateTime, DateTime>>(); }
        #endregion
    }
}