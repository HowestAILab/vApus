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

namespace vApus.Output {
    /// <summary>
    /// Serves at prepping and storing data to be served through vApus.Server. Storing data you must do yourself after the data is prepped (Apply... fxs).
    /// </summary>
    public static class JSONObjectTreeHelper {

        public static JSONObjectTree RunningTestConfig { get; set; }
        public static JSONObjectTree RunningTestFastConcurrencyResults { get; set; }
        public static JSONObjectTree RunningMonitorConfig { get; set; }
        public static JSONObjectTree RunningMonitorMetrics { get; set; }
        public static JSONObjectTree RunningTestClientMonitorMetrics { get; set; }
        public static JSONObjectTree RunningTestMessages { get; set; }
        public static JSONObjectTree RunningMonitorHardwareConfig { get; set; }

        public static void ApplyToRunningDistributedTestConfig(JSONObjectTree testConfigCache, string runSynchronization, string tileStressTest, string connection, string connectionProxy,
                                         string[] monitors, string[] slaves, string[] scenarios, string scenarioRuleSet, int[] concurrency, int run, int minimumDelay,
                                         int maximumDelay, bool shuffle, bool actionDistribution, int maximumNumberOfUserAction, int monitorBefore, int monitorAfter) {
            if (testConfigCache.Count == 0)
                testConfigCache.Add("RunSynchronization", runSynchronization);

            var testConfig = new TestConfig {
                Connection = connection,
                ConnectionProxy = connectionProxy,
                Monitors = monitors,
                Slaves = slaves,
                Scenarios = scenarios,
                ScenarioRuleSet = scenarioRuleSet,
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
            testConfigCache.Add(tileStressTest, testConfig);
        }

        public static void ApplyToRunningStressTestConfig(JSONObjectTree testConfigCache, string stressTest, string connection, string connectionProxy,
                                 string[] monitors, string[] scenarios, string scenarioRuleSet, int[] concurrency, int run, int minimumDelay,
                                 int maximumDelay, bool shuffle, bool actionDistribution, int maximumNumberOfUserAction, int monitorBefore, int monitorAfter) {
            var testConfig = new TestConfig {
                Connection = connection,
                ConnectionProxy = connectionProxy,
                Monitors = monitors,
                Scenarios = scenarios,
                ScenarioRuleSet = scenarioRuleSet,
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
            testConfigCache.Add(stressTest, testConfig);
        }

        public static void ApplyToRunningTestFastConcurrencyResults(JSONObjectTree testProgressCache, StressTestMetrics metrics, string runStateChange, string stressTestStatus) {
            var concurrencyCache = AddSubCache(metrics.Concurrency, testProgressCache);
            var testProgress = new TestFastConcurrencyResults {
                StartMeasuringTime = metrics.StartMeasuringTime,
                MeasuredTime = metrics.MeasuredTime,
                EstimatedTimeLeft = metrics.EstimatedTimeLeft,
                AverageResponseTime = metrics.AverageResponseTime,
                MaxResponseTime = metrics.MaxResponseTime,
                AverageDelay = metrics.AverageDelay,
                Requests = metrics.Requests,
                RequestsProcessed = metrics.RequestsProcessed,
                ResponsesPerSecond = metrics.ResponsesPerSecond,
                UserActionsPerSecond = metrics.UserActionsPerSecond,
                Errors = metrics.Errors,
                RunStateChange = runStateChange,
                StressTestResult = stressTestStatus
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
        public static void ApplyToRunningMonitorMetrics(JSONObjectTree monitorProgressCache, string monitor, string[] headers, Dictionary<DateTime, double[]> values) {
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
        /// <param name="stressTest">or tile stress test</param>
        /// <param name="threadsInUse"></param>
        /// <param name="cpuUsage"></param>
        /// <param name="contextSwitchesPerSecond"></param>
        /// <param name="memoryUsage"></param>
        /// <param name="totalVisibleMemory"></param>
        /// <param name="nicsSent"></param>
        /// <param name="nicsReceived"></param>
        public static void ApplyToRunningTestClientMonitorMetrics(JSONObjectTree clientMonitorCache, string stressTest, int threadsInUse, float cpuUsage,
            uint memoryUsage, uint totalVisibleMemory, string nic, float nicsSent, float nicsReceived) {
            var clientMonitorMetrics = new ClientMonitorMetrics() {
                BusyThreadCount = threadsInUse,
                CPUUsage = cpuUsage,
                MemoryUsage = memoryUsage,
                TotalVisibleMemory = totalVisibleMemory,
                Nic = nic,
                NicsSent = nicsSent,
                NicsReceived = nicsReceived
            };
            clientMonitorCache.Add(stressTest.ToString(), clientMonitorMetrics);
        }

        public static void ApplyToRunningTestMessages(JSONObjectTree messagesCache, string stressTest, string[] messages) {
            var m = new ClientMessages() { Messages = messages };
            messagesCache.Add(stressTest.ToString(), m);
        }

        public static JSONObjectTree AddSubCache(object key, JSONObjectTree parent) {
            var child = new JSONObjectTree();
            parent.Add(key, child);
            return child;
        }

        public static void WriteToFile(object cache, string fileName) {
            string writeDir = Path.Combine(Application.StartupPath, "REST");
            if (!Directory.Exists(writeDir)) Directory.CreateDirectory(writeDir);

            using (var sw = new StreamWriter(Path.Combine(writeDir, fileName)))
                sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(cache));
        }
    }
}