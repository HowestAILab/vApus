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
    [Serializable]
    public class StresstestMetrics : ISerializable {
        private long _logEntriesProcessed;

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

        public long LogEntries { get; set; }

        /// <summary>
        ///     Throughput.
        /// </summary>
        public double ResponsesPerSecond { get; set; }

        public double UserActionsPerSecond { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public TimeSpan MaxResponseTime { get; set; }
        public TimeSpan Percentile95thResponseTimes { get; set; }
        public TimeSpan AverageDelay { get; set; }
        public long Errors { get; set; }

        /// <summary>
        ///     The setter makes sure this cannot exceed the log entries count.
        /// </summary>
        public long LogEntriesProcessed {
            get { return _logEntriesProcessed; }
            set {
                if (value > LogEntries) value = LogEntries;
                _logEntriesProcessed = value;
            }
        }

        public StresstestMetrics() { StartsAndStopsRuns = new List<KeyValuePair<DateTime, DateTime>>(); }

        public StresstestMetrics(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr = SerializationReader.GetReader(info);
            StartMeasuringTime = sr.ReadDateTime();
            EstimatedTimeLeft = sr.ReadTimeSpan();
            MeasuredTime = sr.ReadTimeSpan();
            Concurrency = sr.ReadInt32();
            Run = sr.ReadInt32();
            RerunCount = sr.ReadInt32();
            StartsAndStopsRuns = new List<KeyValuePair<DateTime, DateTime>>();
            StartsAndStopsRuns = sr.ReadCollection<KeyValuePair<DateTime, DateTime>>(StartsAndStopsRuns) as List<KeyValuePair<DateTime, DateTime>>;
            LogEntries = sr.ReadInt64();
            _logEntriesProcessed = sr.ReadInt64();
            ResponsesPerSecond = sr.ReadDouble();
            UserActionsPerSecond = sr.ReadDouble();
            AverageResponseTime = sr.ReadTimeSpan();
            MaxResponseTime = sr.ReadTimeSpan();
            Percentile95thResponseTimes = sr.ReadTimeSpan();
            AverageDelay = sr.ReadTimeSpan();
            Errors = sr.ReadInt64();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw = SerializationWriter.GetWriter();
            sw.Write(StartMeasuringTime);
            sw.Write(EstimatedTimeLeft);
            sw.Write(MeasuredTime);
            sw.Write(Concurrency);
            sw.Write(Run);
            sw.Write(RerunCount);
            sw.Write(StartsAndStopsRuns);
            sw.Write(LogEntries);
            sw.Write(_logEntriesProcessed);
            sw.Write(ResponsesPerSecond);
            sw.Write(UserActionsPerSecond);
            sw.Write(AverageResponseTime);
            sw.Write(MaxResponseTime);
            sw.Write(Percentile95thResponseTimes);
            sw.Write(AverageDelay);
            sw.Write(Errors);
            sw.AddToInfo(info);
        }
    }
}