using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using vApus.Stresstest;

namespace vApus.REST.Convert
{
    public static class Converter
    {
        private static readonly string _writeDir = Path.Combine(Application.StartupPath, "REST");

        public static string WriteDir
        {
            get { return _writeDir; }
        }

        public static void SetTestConfig(Hashtable testConfigCache, string distributedTest, string runSynchronization,
                                         string tileStresstest, Connection connection, string connectionProxy,
                                         Monitor.Monitor[] monitors, string slave,
                                         Log log, string logRuleSet, int[] concurrency, int run, int minimumDelay,
                                         int maximumDelay, bool shuffle,
                                         ActionAndLogEntryDistribution distribute, int monitorBefore, int monitorAfter)
        {
            Hashtable distributedTestCache = AddSubCache(distributedTest, testConfigCache);
            if (distributedTestCache.Count == 0)
                distributedTestCache.Add("RunSynchronization", runSynchronization);

            var newMonitors = new string[monitors.Length];
            for (int i = 0; i != monitors.Length; i++)
                newMonitors[i] = monitors[i].ToString();

            var testConfig = new TestConfig
                {
                    Connection = connection.ToString(),
                    ConnectionProxy = connectionProxy,
                    Monitors = newMonitors,
                    Slave = slave,
                    Log = log.ToString(),
                    LogRuleSet = logRuleSet,
                    Concurrency = concurrency,
                    Run = run,
                    MinimumDelayInMS = minimumDelay,
                    MaximumDelayInMS = maximumDelay,
                    Shuffle = shuffle,
                    Distribute = distribute.ToString(),
                    MonitorBeforeInMinutes = monitorBefore,
                    MonitorAfterInMinutes = monitorAfter
                };
            distributedTestCache.Add(tileStresstest, testConfig);
        }

        public static void SetTestProgress(Hashtable testProgressCache, string distributedTest, string tileStresstest,
                                           int concurrency, int run,
                                           Metrics metrics, TimeSpan estimatedRuntimeLeft, RunStateChange runStateChange,
                                           StresstestStatus stresstestStatus)
        {
            Hashtable concurrencyCache = AddSubCache(concurrency,
                                                     AddSubCache(tileStresstest,
                                                                 AddSubCache(distributedTest, testProgressCache)));
            var testProgress = new TestProgress
                {
                    StartMeasuringRuntime = metrics.StartMeasuringRuntime,
                    MeasuredRunTime = metrics.MeasuredRunTime,
                    EstimatedRuntimeLeft = estimatedRuntimeLeft,
                    AverageTimeToLastByte = metrics.AverageTimeToLastByte,
                    MaxTimeToLastByte = metrics.MaxTimeToLastByte,
                    Percentile95MaxTimeToLastByte = metrics.Percentile95MaxTimeToLastByte,
                    AverageDelay = metrics.AverageDelay,
                    TotalLogEntries = metrics.TotalLogEntries,
                    TotalLogEntriesProcessed = metrics.TotalLogEntriesProcessed,
                    TotalLogEntriesProcessedPerTick = metrics.TotalLogEntriesProcessedPerTick,
                    Errors = metrics.Errors,
                    RunStateChange = runStateChange.ToString(),
                    StresstestResult = stresstestStatus.ToString()
                };

            if (concurrencyCache.Contains(run)) concurrencyCache[run] = testProgress;
            else concurrencyCache.Add(run, testProgress);
        }

        public static void SetMonitorConfig(Hashtable monitorConfigCache, string distributedTest,
                                            Monitor.Monitor monitor)
        {
            Hashtable distributedTestCache = AddSubCache(distributedTest, monitorConfigCache);
            var monitorConfig = new MonitorConfig
                {
                    MonitorSource = monitor.MonitorSource == null ? "N/A" : monitor.MonitorSource.ToString(),
                    Parameters = monitor.Parameters
                };
            distributedTestCache.Add(monitor.ToString(), monitorConfig);
        }

        public static void SetMonitorProgress(Hashtable monitorProgressCache, string distributedTest,
                                              Monitor.Monitor monitor, string[] headers,
                                              Dictionary<DateTime, float[]> values)
        {
            Hashtable distributedTestCache = AddSubCache(distributedTest, monitorProgressCache);
            var monitorProgress = new MonitorProgress
                {
                    Headers = headers,
                    Values = values
                };
            distributedTestCache.Add(monitor.ToString(), monitorProgress);
        }

        private static Hashtable AddSubCache(object key, Hashtable parent)
        {
            if (!parent.Contains(key)) parent.Add(key, new Hashtable());
            return parent[key] as Hashtable;
        }

        public static void WriteToFile(Hashtable cache, string fileName)
        {
            if (!Directory.Exists(_writeDir)) Directory.CreateDirectory(_writeDir);

            using (var sw = new StreamWriter(Path.Combine(_writeDir, fileName)))
                sw.Write(JsonConvert.SerializeObject(cache));
        }

        public static void ClearWrittenFiles()
        {
            int retried = 0;
            Retry:
            try
            {
                if (Directory.Exists(_writeDir)) Directory.Delete(_writeDir, true);
            }
            catch
            {
                if (++retried != 3)
                {
                    Thread.Sleep(1000*retried);
                    goto Retry;
                }
            }
        }

        public struct MonitorConfig
        {
            public string MonitorSource;
            public object[] Parameters;
        }

        public struct MonitorProgress
        {
            public string[] Headers;
            public Dictionary<DateTime, float[]> Values;
        }

        public struct TestConfig
        {
            public int[] Concurrency;
            public string Connection, ConnectionProxy;
            public string Distribute;
            public string Log, LogRuleSet;
            public int MaximumDelayInMS;
            public int MinimumDelayInMS;
            public int MonitorAfterInMinutes;
            public int MonitorBeforeInMinutes;
            public string[] Monitors;
            public int Run;
            public bool Shuffle;
            public string Slave;
        }

        public struct TestProgress
        {
            public TimeSpan AverageDelay;
            public TimeSpan AverageTimeToLastByte;

            public ulong Errors;
            public TimeSpan EstimatedRuntimeLeft;

            public TimeSpan MaxTimeToLastByte;

            public TimeSpan MeasuredRunTime;

            public TimeSpan Percentile95MaxTimeToLastByte;

            public string RunStateChange;
            public DateTime StartMeasuringRuntime;
            public string StresstestResult;

            public ulong TotalLogEntries, TotalLogEntriesProcessed;
            public double TotalLogEntriesProcessedPerTick;
        }
    }
}