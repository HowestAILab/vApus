/*
 * Copyright 2016 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

/*
 * The JSON messages from the 'Value publisher' ('Publish values'-panel in vApus options) are serialized from instances of the classes below.
 * The names of those classes and their properties should be descriptive enough.
 * 
 * You can use this file in your own listener implementation to deserialize the JSON messages.
 * 
 * If you're implementing a listener in another programming language, you can use the classes below as an example for correctly deserialing.
 */
using System;
using System.Collections.Generic;

namespace vApus.Publish {
    public class PublishItem {
        public static readonly DateTime EpochUtc = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public long PublishItemTimestampInMillisecondsSinceEpochUtc { get; set; }

        /// <summary>
        /// A unique Id to bind all PublishItems that belong to each other.
        /// </summary>
        public string ResultSetId { get; set; }

        public string PublishItemType { get; set; }

        public string vApusHost { get; set; }
        public int vApusPort { get; set; }
        public string vApusVersion { get; set; }
        public string vApusChannel { get; set; }
        public bool vApusIsMaster { get; set; }
    }
    /// <summary>
    /// To poll if a connected publish items handler is responding.
    /// </summary>
    public class Poll : PublishItem { }
    /// <summary>
    /// </summary>
    public class DistributedTestConfiguration : PublishItem {
        public string DistributedTest { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public bool UseRDP { get; set; }
        public string RunSynchronization { get; set; }
        public int MaximumRerunsBreakOnLast { get; set; }
        public string[] SlaveHosts { get; set; }
        public string[] TileStressTests { get; set; }
    }
    /// <summary>
    /// </summary>
    public class StressTestConfiguration : PublishItem {
        public string StressTest { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public string Connection { get; set; }
        public string ConnectionProxy { get; set; }
        public KeyValuePair<string, uint>[] ScenariosAndWeights { get; set; }
        public string ScenarioRuleSet { get; set; }
        public string[] Monitors { get; set; }
        public int[] Concurrencies { get; set; }
        public int Runs { get; set; }
        public int InitialMinimumDelayInMilliseconds { get; set; }
        public int InitialMaximumDelayInMilliseconds { get; set; }
        public int MinimumDelayInMilliseconds { get; set; }
        public int MaximumDelayInMilliseconds { get; set; }
        public bool Shuffle { get; set; }
        public bool ActionDistribution { get; set; }
        public int MaximumNumberOfUserActions { get; set; }
        public int MonitorBeforeInMinutes { get; set; }
        public int MonitorAfterInMinutes { get; set; }
        public bool UseParallelExecutionOfRequests { get; set; }
        public int PersistentConnectionsPerHostname { get; set; }
        public int MaximumPersistentConnections { get; set; }
    }
    public class TileStressTestConfiguration : PublishItem {
        public string DistributedTest { get; set; }
        public string TileStressTest { get; set; }
        public string Connection { get; set; }
        public string ConnectionProxy { get; set; }
        public KeyValuePair<string, uint>[] ScenariosAndWeights { get; set; }
        public string ScenarioRuleSet { get; set; }
        public string[] Monitors { get; set; }
        public int[] Concurrencies { get; set; }
        public int Runs { get; set; }
        public int InitialMinimumDelayInMilliseconds { get; set; }
        public int InitialMaximumDelayInMilliseconds { get; set; }
        public int MinimumDelayInMilliseconds { get; set; }
        public int MaximumDelayInMilliseconds { get; set; }
        public bool Shuffle { get; set; }
        public bool ActionDistribution { get; set; }
        public int MaximumNumberOfUserActions { get; set; }
        public int MonitorBeforeInMinutes { get; set; }
        public int MonitorAfterInMinutes { get; set; }
        public bool UseParallelExecutionOfRequests { get; set; }
        public int PersistentConnectionsPerHostname { get; set; }
        public int MaximumPersistentConnections { get; set; }
    }

    /// <summary>
    /// </summary>
    public class FastConcurrencyResults : PublishItem {
        /// <summary>
        /// TileStressTest- or StressTest.ToString(). Link to correct test using the vApus props (which slave for instance).
        /// </summary>
        public string Test { get; set; }
        public long StartMeasuringTimeInMillisecondsSinceEpochUtc { get; set; }
        public long EstimatedTimeLeftInMilliseconds { get; set; }
        /// <summary>
        /// For run sync break on last this is not only the run time. vApus processing time between the reruns are included. 
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
        public int TestEvent { get; set; }
        /// <summary>
        /// Busy, cancelled, failed, success
        /// </summary>
        public string StressTestStatus { get; set; }
    }

    /// <summary>
    /// Use the timestamps to link to the right concurrency.
    /// </summary>
    public class FastRunResults : PublishItem {
        /// <summary>
        /// TileStressTest- or StressTest.ToString(). Link to correct test using the vApus props (which slave for instance).
        /// </summary>
        public string Test { get; set; }
        public long StartMeasuringTimeInMillisecondsSinceEpochUtc { get; set; }
        public long EstimatedTimeLeftInMilliseconds { get; set; }
        /// <summary>
        /// For run sync break on last this is not only the run time. vApus processing time between the reruns are included. 
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
        /// e.g. run initialized the first time (used in break in first). 'Cast' to the TestEvents enum to know what event it is.
        /// </summary>
        public int TestEvent { get; set; }
        /// <summary>
        /// Busy, cancelled, failed, success
        /// </summary>
        public string StressTestStatus { get; set; }
    }

    public enum TestEventType {
        Unchanged = -1,
        TestMessage = 0,
        /// <summary>
        /// ValueStore Value.
        /// </summary>
        TestValue = 1,
        TestInitialized = 2,
        TestStarted = 3,
        ConcurrencyStarted = 4,
        RunInitializedFirstTime = 5,
        RunStarted = 6,
        /// <summary>
        /// For distributed tests.
        /// </summary>
        RunDoneOnce = 7,
        /// <summary>
        /// For distributed tests.
        /// </summary>
        RerunStarted = 8,
        /// <summary>
        /// For distributed tests.
        /// </summary>
        RerunDone = 9,
        RunStopped = 10,
        ConcurrencyStopped = 11,
        TestStopped = 12,
        MasterListeningError = 13
    }

    public class TestEvent : PublishItem {
        /// <summary>
        /// Distributed Test, TileStressTest- or StressTest.ToString(). Link to correct test using the vApus props (which slave for instance).
        /// </summary>
        public string Test { get; set; }
        /// <summary>
        /// 'Cast' to the TestEvents enum to know what event it is.
        /// </summary>
        public int TestEventType { get; set; }
        public KeyValuePair<string, string>[] Parameters { get; set; }
    }
    /// <summary>
    /// <para>PublishItemId should be StressTest.ToString() or TileStressTest.ToString().</para> 
    /// </summary>
    public class RequestResults : PublishItem {
        /// <summary>
        /// TileStressTest- or StressTest.ToString(). Link to correct test using the vApus props (which slave for instance).
        /// </summary>
        public string Test { get; set; }
        /// <summary>
        /// Must be unique for each concurrency. (The concurrency can be duplicate. That is why we have a seperate Id.)
        /// </summary>
        public int ConcurrencyId { get; set; }
        public int Concurrency { get; set; }
        public int Run { get; set; }
        public string VirtualUser { get; set; }
        public string UserAction { get; set; }
        public string RequestIndex { get; set; }
        /// <summary>
        /// Use this to make correct averages for a test with action distribution. This will be empty if not applicable.
        /// </summary>
        public string SameAsRequestIndex { get; set; }
        public string Request { get; set; }
        public bool InParallelWithPrevious { get; set; }
        /// <summary>
        /// Use this for calculations instead of PublishItemTimestampInMillisecondsSinceEpochUtc. PublishItemTimestampInMillisecondsSinceEpochUtc will skew.
        /// </summary>
        public long SentAtInMicrosecondsSinceEpochUtc { get; set; }
        /// <summary>
        /// A tenth of a microsecond.
        /// </summary>
        public long TimeToLastByteInTicks { get; set; }
        /// <summary>
        /// Can be anything. Preferably a clean JSON / BSON / XML string that can be used for creating visuals / reports.  
        /// </summary>
        public string Meta { get; set; }
        /// <summary>
        /// The time waited to fire the next request.
        /// </summary>
        public int DelayInMilliseconds { get; set; }
        public string Error { get; set; }
        /// <summary>
        /// Applicable for break on last run sync distributed testing. A test (for a certain concurrenty and run) will rerun until the slowest one is finished. This rerun count is needed to for instance correctly calculate averages.
        /// </summary>
        public int Rerun { get; set; }
    }
    /// <summary>
    /// </summary>
    public class ClientMonitorMetrics : PublishItem {
        /// <summary>
        /// DistributedTest-, TileStressTest- or StressTest.ToString(). Link to correct test using the vApus props (which slave for instance).
        /// </summary>
        public string Test { get; set; }
        public int BusyThreadCount { get; set; }
        public float CPUUsageInPercent { get; set; }
        public uint MemoryUsageInMB { get; set; }
        public uint TotalVisibleMemoryInMB { get; set; }
        public string Nic { get; set; }
        public int NicBandwidthInMbps { get; set; }
        public float NicSentInPercent { get; set; }
        public float NicReceivedInPercent { get; set; }
    }
    /// <summary>
    /// Belongs to the last generated result set Id if any. That way you can see what stuff went wrong during a test. Can contain false positives though.
    /// </summary>
    public class ApplicationLogEntry : PublishItem {
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
    /// <summary>
    /// </summary>
    public class MonitorConfiguration : PublishItem {
        /// <summary>
        /// TileStressTest- or StressTest.ToString(), if any. Link to correct test using the vApus props (which slave for instance).
        /// Even though this can be a tile stress test, monitors are always executed from the master. 
        /// You link a monitor to a certain test to get the time based averages for a certain test on the vApus GUI.
        /// </summary>
        public string Test { get; set; }
        public string Monitor { get; set; }
        public string MonitorSource { get; set; }
        /// <summary>
        /// Parameter names and values. Rather not put passwords in here.
        /// </summary>
        public KeyValuePair<string, string>[] Parameters { get; set; }
        public string HardwareConfiguration { get; set; }
    }
    public enum MonitorEventType {
        Unchanged = -1,
        MonitorInitialized = 0,
        MonitorStarted = 1,
        MonitorBeforeTestStarted = 2,
        MonitorBeforeTestDone = 3,
        MonitorAfterTestStarted = 4,
        MonitorAfterTestDone = 5,
        MonitorStopped = 6,
    }
    public class MonitorEvent : PublishItem {
        /// <summary>
        /// TileStressTest- or StressTest.ToString(), if any. Link to correct test using the vApus props (which slave for instance).
        /// Even though this can be a tile stress test, monitors are always executed from the master. 
        /// You link a monitor to a certain test to get the time based averages for a certain test on the vApus GUI.
        /// </summary>
        public string Test { get; set; }
        /// <summary>
        /// Monitor.ToString(). Link to correct test using the vApus props (which slave for instance).
        /// </summary>
        public string Monitor { get; set; }
        /// <summary>
        /// 'Cast' to the MonitorEvents enum to know what event it is.
        /// </summary>
        public int MonitorEventType { get; set; }
        public KeyValuePair<string, string>[] Parameters { get; set; }
    }
    /// <summary>
    /// </summary>
    public class MonitorMetrics : PublishItem {
        public string Monitor { get; set; }
        public string[] Headers { get; set; }
        public object[] Values { get; set; }
    }
}
