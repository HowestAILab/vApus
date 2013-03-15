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

        public static string WriteDir {
            get { return _writeDir; }
        }

        public static void SetTestConfig(List<KeyValuePair<object, object>> testConfigCache, string distributedTest, string runSynchronization,
                                         string tileStresstest, Connection connection, string connectionProxy,
                                         Monitor.Monitor[] monitors, string slave,
                                         Log log, string logRuleSet, int[] concurrency, int run, int minimumDelay,
                                         int maximumDelay, bool shuffle,
                                         ActionAndLogEntryDistribution distribute, int monitorBefore, int monitorAfter) {
            var distributedTestCache = AddSubCache(distributedTest, testConfigCache);
            if (distributedTestCache.Count == 0)
                distributedTestCache.Add(new KeyValuePair<object, object>("RunSynchronization", runSynchronization));

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
            distributedTestCache.Add(new KeyValuePair<object, object>(tileStresstest, testConfig));
        }

        public static void SetTestProgress(List<KeyValuePair<object, object>> testProgressCache, string distributedTest, string tileStresstest, StresstestMetrics metrics, RunStateChange runStateChange, StresstestStatus stresstestStatus) {
            var concurrencyCache = AddSubCache(metrics.Concurrency, AddSubCache(tileStresstest, AddSubCache(distributedTest, testProgressCache)));
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
            concurrencyCache.Add(new KeyValuePair<object, object>(run, testProgress));
            //if (concurrencyCache.Contains(run)) concurrencyCache[run] = testProgress;
            //else concurrencyCache.Add(run, testProgress);
        }

        public static void SetMonitorConfig(List<KeyValuePair<object, object>> monitorConfigCache, string distributedTest, Monitor.Monitor monitor) {
            var distributedTestCache = AddSubCache(distributedTest, monitorConfigCache);
            var monitorConfig = new MonitorConfig {
                MonitorSource = monitor.MonitorSource == null ? "N/A" : monitor.MonitorSource.ToString(),
                Parameters = monitor.Parameters
            };
            distributedTestCache.Add(new KeyValuePair<object, object>(monitor.ToString(), monitorConfig));
        }

        public static void SetMonitorProgress(List<KeyValuePair<object, object>> monitorProgressCache, string distributedTest, Monitor.Monitor monitor, string[] headers, Dictionary<DateTime, float[]> values) {
            var distributedTestCache = AddSubCache(distributedTest, monitorProgressCache);
            var monitorProgress = new MonitorProgress {
                Headers = headers,
                Values = values
            };
            distributedTestCache.Add(new KeyValuePair<object, object>(monitor.ToString(), monitorProgress));
        }

        private static List<KeyValuePair<object, object>> AddSubCache(object key, List<KeyValuePair<object, object>> parent) {
            var child = new List<KeyValuePair<object, object>>();
            parent.Add(new KeyValuePair<object, object>(key, child));
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