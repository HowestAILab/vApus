/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Runtime.Serialization;
using vApus.Util;

namespace vApus.Results
{
    [Serializable]
    public class Metrics : ISerializable
    {
        public DateTime StartMeasuringRuntime { get; set; }
        public TimeSpan EstimatedTimeLeft { get; set; }
        public TimeSpan MeasuredRunTime { get; set; }
        public int ConcurrentUsers { get; set; }
        /// <summary>
        /// Stays 0 for concurrency level metrics.
        /// </summary>
        public int Run { get; set; }
        /// <summary>
        /// Stays 0 for concurrency level metrics.
        /// </summary>
        public int RerunCount { get; set; }
        public long LogEntries { get; set; }
        private long _logEntriesProcessed;
        /// <summary>
        /// Throughput.
        /// </summary>
        public double LogEntriesPerSecond { get; set; }
        /// <summary>
        /// Throughput.
        /// </summary>
        public double UserActionsPerSecond { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public TimeSpan MaxResponseTime { get; set; }
        public TimeSpan AverageDelay { get; set; }
        public long Errors { get; set; }

        /// <summary>
        /// The setter makes sure this cannot exceed the log entries count.
        /// </summary>
        public long LogEntriesProcessed
        {
            get { return _logEntriesProcessed; }
            set
            {
                if (value > LogEntries)
                    value = LogEntries;
                _logEntriesProcessed = value;
            }
        }

        public Metrics() { }
        public Metrics(SerializationInfo info, StreamingContext ctxt)
        {
            SerializationReader sr = SerializationReader.GetReader(info);
            StartMeasuringRuntime = sr.ReadDateTime();
            EstimatedTimeLeft = sr.ReadTimeSpan();
            MeasuredRunTime = sr.ReadTimeSpan();
            ConcurrentUsers = sr.ReadInt32();
            Run = sr.ReadInt32();
            LogEntries = sr.ReadInt64();
            _logEntriesProcessed = sr.ReadInt64();
            LogEntriesPerSecond = sr.ReadDouble();
            AverageResponseTime = sr.ReadTimeSpan();
            MaxResponseTime = sr.ReadTimeSpan();
            AverageDelay = sr.ReadTimeSpan();
            Errors = sr.ReadInt64();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter sw = SerializationWriter.GetWriter();
            sw.Write(StartMeasuringRuntime);
            sw.Write(EstimatedTimeLeft);
            sw.Write(MeasuredRunTime);
            sw.Write(ConcurrentUsers);
            sw.Write(Run);
            sw.Write(LogEntries);
            sw.Write(_logEntriesProcessed);
            sw.Write(LogEntriesPerSecond);
            sw.Write(AverageResponseTime);
            sw.Write(MaxResponseTime);
            sw.Write(AverageDelay);
            sw.Write(Errors);
            sw.AddToInfo(info);
        }
    }
}
