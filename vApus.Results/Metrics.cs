/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using vApus.Util;
using System.Runtime.Serialization;

namespace vApus.Results
{
    [Serializable]
    public struct Metrics : ISerializable
    {
        public DateTime StartMeasuringRuntime;
        public TimeSpan EstimatedTimeLeft;
        public TimeSpan MeasuredRunTime;
        public int ConcurrentUsers;
        public int Run;
        public long LogEntries;
        private long _logEntriesProcessed;
        /// <summary>
        /// Expressed in responses per second.
        /// </summary>
        public double Throughput;
        public TimeSpan AverageResponseTime;
        public TimeSpan MaxResponseTime;
        public TimeSpan AverageDelay;
        public long Errors;

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
            Throughput = sr.ReadDouble();
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
            sw.Write(Throughput);
            sw.Write(AverageResponseTime);
            sw.Write(MaxResponseTime);
            sw.Write(AverageDelay);
            sw.Write(Errors);
            sw.AddToInfo(info);
        }
    }
}
