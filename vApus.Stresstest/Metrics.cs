/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Runtime.Serialization;
using vApus.Util;

namespace vApus.Stresstest
{
    [Serializable]
    public struct Metrics : ISerializable
    {
        public DateTime StartMeasuringRuntime;
        public TimeSpan MeasuredRunTime, AverageTimeToLastByte, MaxTimeToLastByte, Percentile95MaxTimeToLastByte, AverageDelay;
        public ulong TotalLogEntries;
        private ulong _totalLogEntriesProcessed;
        public double TotalLogEntriesProcessedPerTick;
        public ulong Errors;

        /// <summary>
        /// The setter makes sure this cannot exceed the Total Log Entries.
        /// </summary>
        public ulong TotalLogEntriesProcessed
        {
            get { return _totalLogEntriesProcessed; }
            set
            {
                if (value > TotalLogEntries)
                    value = TotalLogEntries;
                _totalLogEntriesProcessed = value;
            }
        }

        public Metrics(SerializationInfo info, StreamingContext ctxt)
        {
            SerializationReader sr = SerializationReader.GetReader(info);
            StartMeasuringRuntime = sr.ReadDateTime();
            MeasuredRunTime = sr.ReadTimeSpan();
            AverageTimeToLastByte = sr.ReadTimeSpan();
            MaxTimeToLastByte = sr.ReadTimeSpan();
            Percentile95MaxTimeToLastByte = sr.ReadTimeSpan();
            AverageDelay = sr.ReadTimeSpan();
            TotalLogEntries = sr.ReadUInt64();
            _totalLogEntriesProcessed = sr.ReadUInt64();
            TotalLogEntriesProcessedPerTick = sr.ReadDouble();
            Errors = sr.ReadUInt64();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter sw = SerializationWriter.GetWriter();
            sw.Write(StartMeasuringRuntime);
            sw.Write(MeasuredRunTime);
            sw.Write(AverageTimeToLastByte);
            sw.Write(MaxTimeToLastByte);
            sw.Write(Percentile95MaxTimeToLastByte);
            sw.Write(AverageDelay);
            sw.Write(TotalLogEntries);
            sw.Write(_totalLogEntriesProcessed);
            sw.Write(TotalLogEntriesProcessedPerTick);
            sw.Write(Errors);
            sw.AddToInfo(info);
        }
    }
}
