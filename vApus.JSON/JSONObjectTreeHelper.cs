/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using vApus.Results;
using vApus.Util;

namespace vApus.JSON {
    /// <summary>
    /// Serves at prepping and storing data to be served through vApus.Server. Storing data you must do yourself after the data is prepped (Apply... fxs).
    /// </summary>
    public static class JSONObjectTreeHelper {

        public static JSONObjectTree RunningTestConfig { get; set; }
        public static JSONObjectTree RunningTestFastConcurrencyResults { get; set; }
        public static JSONObjectTree RunningMonitorConfig { get; set; }
        public static JSONObjectTree RunningMonitorMetrics { get; set; }
        public static JSONObjectTree RunningClientMonitorMetrics { get; set; }
        public static JSONObjectTree RunningTestMessages { get; set; }
        public static JSONObjectTree RunningMonitorHardwareConfig { get; set; }

        public static void ApplyToRunningDistributedTestConfig(JSONObjectTree testConfigCache, string runSynchronization, string tileStresstest, string connection, string connectionProxy,
                                         string[] monitors, string[] slaves, string[] logs, string logRuleSet, int[] concurrency, int run, int minimumDelay,
                                         int maximumDelay, bool shuffle, bool actionDistribution, int maximumNumberOfUserAction, int monitorBefore, int monitorAfter) {
            if (testConfigCache.Count == 0)
                testConfigCache.Add("RunSynchronization", runSynchronization);

            var testConfig = new TestConfig {
                Connection = connection,
                ConnectionProxy = connectionProxy,
                Monitors = monitors,
                Slaves = slaves,
                Logs = logs,
                LogRuleSet = logRuleSet,
                Concurrency = concurrency,
                Run = run,
                MinimumDelayInMS = minimumDelay,
                MaximumDelayInMS = maximumDelay,
                Shuffle = shuffle,
                ActionDistribution = actionDistribution,
                MaximumNumberOfUserActions = maximumNumberOfUserAction,
                MonitorBeforeInMinutes = monitorBefore,
                MonitorAfterInMinutes = monitorAfter
            };
            testConfigCache.Add(tileStresstest, testConfig);

            RunningTestConfig = testConfigCache;
        }

        public static void ApplyToRunningStresstestConfig(JSONObjectTree testConfigCache, string stresstest, string connection, string connectionProxy,
                                 string[] monitors, string[] logs, string logRuleSet, int[] concurrency, int run, int minimumDelay,
                                 int maximumDelay, bool shuffle, bool actionDistribution, int maximumNumberOfUserAction, int monitorBefore, int monitorAfter) {
            var testConfig = new TestConfig {
                Connection = connection,
                ConnectionProxy = connectionProxy,
                Monitors = monitors,
                Logs = logs,
                LogRuleSet = logRuleSet,
                Concurrency = concurrency,
                Run = run,
                MinimumDelayInMS = minimumDelay,
                MaximumDelayInMS = maximumDelay,
                Shuffle = shuffle,
                ActionDistribution = actionDistribution,
                MaximumNumberOfUserActions = maximumNumberOfUserAction,
                MonitorBeforeInMinutes = monitorBefore,
                MonitorAfterInMinutes = monitorAfter
            };
            testConfigCache.Add(stresstest, testConfig);
        }

        public static void ApplyToRunningTestFastConcurrencyResults(JSONObjectTree testProgressCache, StresstestMetrics metrics, string runStateChange, string stresstestStatus) {
            var concurrencyCache = AddSubCache(metrics.Concurrency, testProgressCache);
            var testProgress = new TestFastConcurrencyResults {
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
                RunStateChange = runStateChange,
                StresstestResult = stresstestStatus
            };

            int run = metrics.Run + 1;
            concurrencyCache.Add(run, testProgress);
            //if (concurrencyCache.Contains(run)) concurrencyCache[run] = testProgress;
            //else concurrencyCache.Add(run, testProgress);
        }

        public static void ApplyToRunningMonitorConfig(JSONObjectTree monitorConfigCache, string monitor, string monitorSource, object[] parameters) {
            int index = -1;
            for (int i = 0; i != monitorConfigCache.Count; i++) {
                var kvp = monitorConfigCache.Cache[i];
                var subTree = kvp.Value as JSONObjectTree;
                if (subTree.Cache[0].Key as string == monitor) {
                    index = i;
                    break;
                }
            } 
            var monitorConfig = new MonitorConfig {
                MonitorSource = monitorSource,
                Parameters = parameters
            };
            if (index == -1) {
                index = monitorConfigCache.Count;

                var subTree = AddSubCache(index + 1, monitorConfigCache);
                subTree.Add(monitor, monitorConfig);
            } else {
                var kvp = monitorConfigCache.Cache[index];
                var subTree = kvp.Value as JSONObjectTree;
                subTree.Cache[0] = new KeyValuePair<object, object>(monitor, monitorConfig);
            }
        }
        public static void ApplyToRunningMonitorHardwareConfig(JSONObjectTree runningMonitorHardwareConfig, string monitor, string config) {
            int index = -1;
            for (int i = 0; i != runningMonitorHardwareConfig.Count; i++) {
                var kvp = runningMonitorHardwareConfig.Cache[i];
                var subTree = kvp.Value as JSONObjectTree;
                if (subTree.Cache[0].Key as string == monitor) {
                    index = i;
                    break;
                }
            }
            if (index == -1) {
                index = runningMonitorHardwareConfig.Count;

                var subTree = AddSubCache(index + 1, runningMonitorHardwareConfig);
                subTree.Add(monitor, config);
            } else {
                var kvp = runningMonitorHardwareConfig.Cache[index];
                var subTree = kvp.Value as JSONObjectTree;
                subTree.Cache[0] = new KeyValuePair<object, object>(monitor, config);
            }
        }
        public static void ApplyToRunningMonitorMetrics(JSONObjectTree monitorProgressCache, string monitor, string[] headers, Dictionary<DateTime, float[]> values) {
            int index = -1;
            for (int i = 0; i != monitorProgressCache.Count; i++) {
                var kvp = monitorProgressCache.Cache[i];
                var subTree = kvp.Value as JSONObjectTree;
                if (subTree.Cache[0].Key as string == monitor) {
                    index = i;
                    break;
                }
            }
            var monitorProgress = new MonitorMetrics {
                Headers = headers,
                Values = values
            };
            if (index == -1) {
                index = monitorProgressCache.Count;

                var subTree = AddSubCache(index + 1, monitorProgressCache);
                subTree.Add(monitor, monitorProgress);
            } else {
                var kvp = monitorProgressCache.Cache[index];
                var subTree = kvp.Value as JSONObjectTree;
                subTree.Cache[0] = new KeyValuePair<object, object>(monitor, monitorProgress);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientMonitorCache"></param>
        /// <param name="stresstest">or tile stresstest</param>
        /// <param name="busyThreadCount"></param>
        /// <param name="cpuUsage"></param>
        /// <param name="contextSwitchesPerSecond"></param>
        /// <param name="memoryUsage"></param>
        /// <param name="totalVisibleMemory"></param>
        /// <param name="nicsSent"></param>
        /// <param name="nicsReceived"></param>
        public static void ApplyToRunningClientMonitorMetrics(JSONObjectTree clientMonitorCache, string stresstest, int busyThreadCount, float cpuUsage, float contextSwitchesPerSecond,
            uint memoryUsage, uint totalVisibleMemory, float nicsSent, float nicsReceived) {
            var clientMonitorMetrics = new ClientMonitorMetrics() {
                BusyThreadCount = busyThreadCount,
                CPUUsage = cpuUsage,
                ContextSwitchesPerSecond = contextSwitchesPerSecond,
                MemoryUsage = memoryUsage,
                TotalVisibleMemory = totalVisibleMemory,
                NicsSent = nicsSent,
                NicsReceived = nicsReceived
            };
            clientMonitorCache.Add(stresstest.ToString(), clientMonitorMetrics);
        }

        public static void ApplyToRunningTestMessages(JSONObjectTree messagesCache, string stresstest, string[] messages) {
            var m = new ClientMessages() { Messages = messages };
            messagesCache.Add(stresstest.ToString(), m);
        }

        public static JSONObjectTree AddSubCache(object key, JSONObjectTree parent) {
            var child = new JSONObjectTree();
            parent.Add(key, child);
            return child;
        }

    }
}