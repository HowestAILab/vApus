/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

/*
 * The JSON messages from the 'Value publisher' ('Publish values'-panel in vApus options) are serialized from instances of the classes below.
 * The names of those classes and their properties should be descriptive enough.
 * 
 * You can use this file in your own listener implementation, a folder watcher and/or an UDP broadcast listener, to deserialize the JSON messages.
 * 
 * If you're implementing a listener in another programming language, you can use the classes below as an example.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace vApus.Publish {
    public class PublishItem {
        public static readonly DateTime EpochUtc = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// e.g. The tostring() of a stresstest.
        /// </summary>
        public string PublishItemId { get; set; }
        public string PublishItemType { get; set; }
        public long PublishItemTimestampInMillisecondsSinceEpochUTC { get; set; }
        public int vApusPID { get; set; }

        public void Init() {
            PublishItemType = this.GetType().Name;
            vApusPID = Process.GetCurrentProcess().Id;
            PublishItemTimestampInMillisecondsSinceEpochUTC = (long)(DateTime.UtcNow - EpochUtc).TotalMilliseconds;
        }
    }
    public class DistributedTestConfiguration : PublishItem {
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public bool UseRDP { get; set; }
        public string RunSynchronization { get; set; }
        public int MaximumRerunsBreakOnLast { get; set; }
        public string[] UsedTileStressTests { get; set; }
    }

    public class StressTestConfiguration : PublishItem {
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public string Connection { get; set; }
        public string ConnectionProxy { get; set; }
        public KeyValuePair<string, uint>[] ScenariosAndWeights { get; set; }
        public string ScenarioRuleSet { get; set; }
        public string[] Monitors { get; set; }
        public int[] Concurrencies { get; set; }
        public int Runs { get; set; }
        public int InitialMinimumDelay { get; set; }
        public int InitialMaximumDelay { get; set; }
        public int MinimumDelayInMilliseconds { get; set; }
        public int MaximumDelayInMilliseconds { get; set; }
        public bool Shuffle { get; set; }
        public bool ActionDistribution { get; set; }
        public int MaximumNumberOfUserActions { get; set; }
        public int MonitorBeforeInSeconds { get; set; }
        public int MonitorAfterInSeconds { get; set; }
        public bool UseParallelExecutionOfRequests { get; set; }
    }
    public class TileStressTestConfiguration : PublishItem {
        public string Connection { get; set; }
        public string ConnectionProxy { get; set; }
        public KeyValuePair<string, uint>[] ScenariosAndWeights { get; set; }
        public string ScenarioRuleSet { get; set; }
        public string[] Monitors { get; set; }
        public int[] Concurrencies { get; set; }
        public int Runs { get; set; }
        public int InitialMinimumDelay { get; set; }
        public int InitialMaximumDelay { get; set; }
        public int MinimumDelayInMilliseconds { get; set; }
        public int MaximumDelayInMilliseconds { get; set; }
        public bool Shuffle { get; set; }
        public bool ActionDistribution { get; set; }
        public int MaximumNumberOfUserActions { get; set; }
        public int MonitorBeforeInSeconds { get; set; }
        public int MonitorAfterInSeconds { get; set; }
        public bool UseParallelExecutionOfRequests { get; set; }

        public string SlaveIP { get; set; }
        public string SlaveHostName { get; set; }
        public int SlavePort { get; set; }
    }

    public class FastConcurrencyResults : PublishItem {
        public long StartMeasuringTimeInMillisecondsSinceEpochUTC { get; set; }
        public long EstimatedTimeLeftInMilliseconds { get; set; }
        /// <summary>
        /// For run sync break on last is this not only the run time, think time between the reruns are included. 
        /// </summary>
        public long MeasuredTimeInMilliseconds { get; set; }
        public int Concurrency { get; set; }

        public long RequestsProcessed { get; set; }
        public long Requests { get; set; }

        /// <summary>
        ///     Throughput.
        /// </summary>
        public double ResponsesPerSecond { get; set; }

        public double UserActionsPerSecond { get; set; }
        public long AverageResponseTimeInMilliseconds { get; set; }
        public long MaxResponseTimeInMilliseconds { get; set; }
        public long Percentile95thResponseTimesInMilliseconds { get; set; }
        public long Percentile99thResponseTimesInMilliseconds { get; set; }
        public long AverageTop5ResponseTimesInMilliseconds { get; set; }
        public long AverageDelayInMilliseconds { get; set; }
        public long Errors { get; set; }

        /// <summary>
        /// e.g. run initialized the first time (used in break in first)
        /// </summary>
        public string RunStateChange { get; set; }
        /// <summary>
        /// Busy, cancelled, failed, success
        /// </summary>
        public string StressTestStatus { get; set; }
    }
    public class FastRunResults : PublishItem {
        public long StartMeasuringTimeInMillisecondsSinceEpochUTC { get; set; }
        public long EstimatedTimeLeftInMilliseconds { get; set; }
        /// <summary>
        /// For run sync break on last is this not only the run time, think time between the reruns are included. 
        /// </summary>
        public long MeasuredTimeInMilliseconds { get; set; }
        public int Concurrency { get; set; }

        public int Run { get; set; }
        public int RerunCount { get; set; }

        public long RequestsProcessed { get; set; }
        public long Requests { get; set; }

        /// <summary>
        ///     Throughput.
        /// </summary>
        public double ResponsesPerSecond { get; set; }

        public double UserActionsPerSecond { get; set; }
        public long AverageResponseTimeInMilliseconds { get; set; }
        public long MaxResponseTimeInMilliseconds { get; set; }
        public long Percentile95thResponseTimesInMilliseconds { get; set; }
        public long Percentile99thResponseTimesInMilliseconds { get; set; }
        public long AverageTop5ResponseTimesInMilliseconds { get; set; }
        public long AverageDelayInMilliseconds { get; set; }
        public long Errors { get; set; }

        /// <summary>
        /// e.g. run initialized the first time (used in break in first)
        /// </summary>
        public string RunStateChange { get; set; }
        /// <summary>
        /// Busy, cancelled, failed, success
        /// </summary>
        public string StressTestStatus { get; set; }
    }

    public class ClientMonitorMetrics : PublishItem {
        public int BusyThreadCount { get; set; }
        public float CPUUsageInPercent { get; set; }
        public uint MemoryUsageInMB { get; set; }
        public uint TotalVisibleMemoryInMB { get; set; }
        public string Nic { get; set; }
        public int NicBandwidthInMbps { get; set; }
        public float NicSentInPercent { get; set; }
        public float NicReceivedInPercent { get; set; }
    }

    public class Message : PublishItem {
        /// <summary>
        /// 0 = info, 1 = warning, 2 = error
        /// </summary>
        public int Level { get; set; }
        public string Body { get; set; }
    }

    internal class ApplicationLogEntry : PublishItem {
        /// <summary>
        /// 0 = info, 1 = warning, 2 = error, 3 = fatal
        /// </summary>
        public int Level { get; set; }
        public string Description { get; set; }
        public string Exception { get; set; }
        public object[] Parameters { get; set; }
        public string Member { get; set; }
        public string SourceFile { get; set; }
        public int Line { get; set; }
    }

    public class MonitorConfiguration : PublishItem {
        public string MonitorSource { get; set; }
        //Do not put passwords in here.
        public KeyValuePair<string, string>[] Parameters { get; set; }
    }

    public class MonitorHardwareConfiguration : PublishItem {
        public string HardwareConfiguration { get; set; }
    }

    public class MonitorMetrics : PublishItem {
        public string[] Headers { get; set; }
        public object[] Values { get; set; }
    }

}
