/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using vApus.Results;
using vApus.Stresstest;

namespace vApus.REST.Convert {
    public static class Converter {
        private static readonly string _writeDir = Path.Combine(Application.StartupPath, "REST");

        public static string WriteDir { get { return _writeDir; } }

        public static void SetTestConfig(ConverterCollection testConfigCache, string runSynchronization,
                                         string tileStresstest, Connection connection, string connectionProxy,
                                         Monitor.Monitor[] monitors, string slave,
                                         Log log, string logRuleSet, int[] concurrency, int run, int minimumDelay,
                                         int maximumDelay, bool shuffle,
                                         ActionAndLogEntryDistribution distribute, int monitorBefore, int monitorAfter) {
            if (testConfigCache.Count == 0)
                testConfigCache.Add("RunSynchronization", runSynchronization);

            var newMonitors = new string[monitors.Length];
            for (int i = 0; i != monitors.Length; i++)
                newMonitors[i] = monitors[i].ToString();

            var testConfig = new TestConfig {
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
            testConfigCache.Add(tileStresstest, testConfig);
        }

        public static void SetTestProgress(ConverterCollection testProgressCache, StresstestMetrics metrics, RunStateChange runStateChange, StresstestStatus stresstestStatus) {
            var concurrencyCache = AddSubCache(metrics.Concurrency, testProgressCache);
            var testProgress = new TestProgress {
                StartMeasuringTime = metrics.StartMeasuringTime,
                MeasuredTime = metrics.MeasuredTime,
                EstimatedTimeLeft = metrics.EstimatedTimeLeft,
                AverageResponseTime = metrics.AverageResponseTime,
                MaxResponseTime = metrics.MaxResponseTime,
                AverageDelay = metrics.AverageDelay,
                LogEntries = metrics.LogEntries,
                LogEntriesProcessed = metrics.LogEntriesProcessed,
                ResponsesPerSecond = metrics.ResponsesPerSecond,
                UserActionsPerSecond = metrics.UserActionsPerSecond,
                Errors = metrics.Errors,
                RunStateChange = runStateChange.ToString(),
                StresstestResult = stresstestStatus.ToString()
            };

            int run = metrics.Run + 1;
            concurrencyCache.Add(run, testProgress);
            //if (concurrencyCache.Contains(run)) concurrencyCache[run] = testProgress;
            //else concurrencyCache.Add(run, testProgress);
        }

        public static void SetMonitorConfig(ConverterCollection monitorConfigCache, Monitor.Monitor monitor) {
            var monitorConfig = new MonitorConfig {
                MonitorSource = monitor.MonitorSource == null ? "N/A" : monitor.MonitorSource.ToString(),
                Parameters = monitor.Parameters
            };
            monitorConfigCache.Add(monitor.ToString(), monitorConfig);
        }

        public static void SetMonitorProgress(ConverterCollection monitorProgressCache, Monitor.Monitor monitor, string[] headers, Dictionary<DateTime, float[]> values) {
            var monitorProgress = new MonitorProgress {
                Headers = headers,
                Values = values
            };
            monitorProgressCache.Add(monitor.ToString(), monitorProgress);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parent"></param>
        /// <returns>The sub cache</returns>
        public static ConverterCollection AddSubCache(object key, ConverterCollection parent) {
            var child = new ConverterCollection();
            parent.Add(key, child);
            return child;
        }

        public static void WriteToFile(object cache, string fileName) {
            if (!Directory.Exists(_writeDir)) Directory.CreateDirectory(_writeDir);
            using (var sw = new StreamWriter(Path.Combine(_writeDir, fileName)))
                sw.Write(JsonConvert.SerializeObject(cache));
        }

        public static void ClearWrittenFiles() {
            int retried = 0;
        Retry:
            try {
                if (Directory.Exists(_writeDir)) Directory.Delete(_writeDir, true);
            } catch {
                if (++retried != 3) {
                    Thread.Sleep(1000 * retried);
                    goto Retry;
                }
            }
        }

        public struct MonitorConfig {
            public string MonitorSource;
            public object[] Parameters;
        }

        public struct MonitorProgress {
            public string[] Headers;
            public Dictionary<DateTime, float[]> Values;
        }

        public struct TestConfig {
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

        public struct TestProgress {
            public TimeSpan AverageDelay;
            public TimeSpan AverageResponseTime;

            public long Errors;
            public TimeSpan EstimatedTimeLeft;

            public TimeSpan MaxResponseTime;

            public TimeSpan MeasuredTime;

            public string RunStateChange;
            public DateTime StartMeasuringTime;
            public string StresstestResult;

            public long LogEntries, LogEntriesProcessed;
            public double ResponsesPerSecond, UserActionsPerSecond;
        }
    }
}