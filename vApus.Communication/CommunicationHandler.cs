/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using Newtonsoft.Json;
using RandomUtils;
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using vApus.DistributedTest;
using vApus.Monitor;
using vApus.Results;
using vApus.Communication.Shared;
using vApus.SolutionTree;
using vApus.StressTest;
using vApus.Util;

namespace vApus.Communication {
    public static class CommunicationHandler {
        private static AutoResetEvent _jobWaitHandle = new AutoResetEvent(false);

        private delegate string HandleMessageDelegate(string message);
        /// <summary>
        /// Holds the whole or the significant part of the path as Key. Specifics are handled by the matching delegate.
        /// </summary>
        private static Dictionary<string, HandleMessageDelegate> _delegates;

        static CommunicationHandler() {
            // Fill delegates
            _delegates = new Dictionary<string, HandleMessageDelegate>();

            // Send stuff that needs to be done. You receive an ack as plain text JSON.
            // Indices are one-based. If no index is given, you get all available indices and the string representation of the objects.
            // Most of these are, and will probably not be used.
            //_delegates.Add("/testconnection/#", TestConnection);

            _delegates.Add("/startdistributedtest/#", StartDistributedTest);
            _delegates.Add("/startstresstest/#", StartStressTest);
            _delegates.Add("/startmonitor/#", StartMonitor);

            //_delegates.Add("/startmonitor/#/#", StartMonitor);--> Time to monitor can be given with.
            //_delegates.Add("/stoptestandmonitors", StopTestAndMonitors);

            //// -----

            //// Get data back as plain text JSON
            //_delegates.Add("/applicationlog/info", ApplicationLog);
            //_delegates.Add("/applicationlog/warning", ApplicationLog);
            //_delegates.Add("/applicationlog/error", ApplicationLog);
            //_delegates.Add("/applicationlog/fatal", ApplicationLog);

            //_delegates.Add("/resultsdb", ResultsDB);

            //// For a single test and distributed test (as a whole)
            //_delegates.Add("/runningtest/config", RunningTestConfig);
            //_delegates.Add("/runningtest/fastresults", RunningTestFastResults);
            //_delegates.Add("/runningtest/clientmonitor", RunningTestClientMonitor);
            //_delegates.Add("/runningtest/messages/info", RunningTestMessages);
            //_delegates.Add("/runningtest/messages/warning", RunningTestMessages);
            //_delegates.Add("/runningtest/messages/error", RunningTestMessages);
            //_delegates.Add("/runningtest/fastmonitorresults/#", RunningTestFastMonitorResults);//


            //// For a tile stress test            
            //_delegates.Add("/runningtest/tile/#/tilestresstest/#/config", TileStressTestConfig);
            //_delegates.Add("/runningtest/tile/#/tilestresstest/#/fastresults", TileStressTestFastResults);
            //_delegates.Add("/runningtest/tile/#/tilestresstest/#/clientmonitor", TileStressTestClientMonitor);
            //_delegates.Add("/runningtest/tile/#/tilestresstest/#/messages/info", TilestressTestMessages);
            //_delegates.Add("/runningtest/tile/#/tilestresstest/#/messages/warning", TilestressTestMessages);
            //_delegates.Add("/runningtest/tile/#/tilestresstest/#/messages/error", TilestressTestMessages);
            //_delegates.Add("/runningtest/tile/#/tilestresstest/#/fastmonitorresults/#", TileFastMonitorResults);//

            //_delegates.Add("/runningmonitor/#/config", RunningMonitorConfig);
            //_delegates.Add("/runningmonitor/#/hardwareconfig", RunningMonitorHardwareConfig);
            //_delegates.Add("/runningmonitor/#/metrics", RunningMonitorMetrics);
        }

        /// <summary>
        /// Handles textual messages to automate vApus. If the message is not textual it supposes that it is a message for master slave communication and forwards it to   SlaveSideCommunicationHandler.HandleMessage(message).
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static object HandleMessage(object message) {
            if (message is string) {
                string msg = message as string;

                foreach (string path in _delegates.Keys)
                    if (Match(msg, path))
                        return _delegates[path].Invoke(msg);

                return GetRPCFailed(msg, new Exception("404 Not Found"));
            } else if (message is Message<Key>) {
                return SlaveSideCommunicationHandler.HandleMessage((Message<Key>)message);
            }
            throw new Exception("Communication message not of the type string or Message<Key>.");
        }
        private static bool Match(string message, string path) {
            if (message.StartsWith(path)) return true;

            //Walk the path in the message and find a match in the given path
            var messageNodes = message.Split('/');
            var pathNodes = path.Split('/');
            if (messageNodes.Length != pathNodes.Length) return false;

            for (int i = 0; i != messageNodes.Length; i++) {
                string messageNode = messageNodes[i];
                string pathNode = pathNodes[i];
                if (messageNode != pathNode) {
                    if (pathNode != "#") return false;

                    int result;
                    if (!int.TryParse(messageNode, out result)) return false;
                }
            }

            return true;
        }

        private static string StartDistributedTest(string message) {
            try {
                var distributedTestProject = Solution.ActiveSolution.GetProject("DistributedTestProject");
                int index = int.Parse(message.Split('/')[2]) - 1;
                var distributedTest = distributedTestProject[index] as DistributedTest.DistributedTest;

                DistributedTestView view = null;
                SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                    view = distributedTest.Activate() as DistributedTestView;
                    view.StartDistributedTest(false);
                }, null);

                return GetRPCSuccess(message);
            } catch (Exception ex) {
                return GetRPCFailed(message, ex);
            }
        }
        private static string StartStressTest(string message) {
            try {
                var stressTestProject = Solution.ActiveSolution.GetProject("StressTestProject");
                int index = int.Parse(message.Split('/')[2]) + 3;
                var stressTest = stressTestProject[index] as StressTest.StressTest;

                StressTestView view = null;
                SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                    view = stressTest.Activate() as StressTestView;
                    view.StartStressTest(false);
                }, null);

                return GetRPCSuccess(message);
            } catch (Exception ex) {
                return GetRPCFailed(message, ex);
            }
        }
        private static string StartMonitor(string message) {
            try {
                var monitorProject = Solution.ActiveSolution.GetProject("MonitorProject");
                string[] split = message.Split('/');
                int index = int.Parse(split[2]) - 1;
                var monitor = monitorProject[index] as Monitor.Monitor;

                MonitorView view = null;
                string error = string.Empty;
                SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                    view = monitor.Activate() as MonitorView;
                    view.InitializeForStressTest(null);
                    view.MonitorInitialized += (object sender, MonitorView.MonitorInitializedEventArgs e) => { _jobWaitHandle.Set(); };
                }, null);


                _jobWaitHandle.WaitOne();
                SynchronizationContextWrapper.SynchronizationContext.Send((state) => { view.Start(); }, null);
                return GetRPCSuccess(message);
            } catch (Exception ex) {
                return GetRPCFailed(message, ex);
            }
        }

        //private static Message<Key> TestConnection(string message) {
        //    var connections = Solution.ActiveSolution.GetSolutionComponent(typeof(Connections));
        //    int index = int.Parse(message.Split('/')[2]); // -1 not needed, connection proxies is the first item.
        //    var connection = connections[index] as Connection;
        //    ConnectionView view = null;
        //    SynchronizationContextWrapper.SynchronizationContext.Send((state) => { view = connection.Activate() as ConnectionView; }, null);

        //    string error = view.TestConnection(false);
        //    if (error.Length != 0)
        //        return new Message<Key>(Key.Other, SerializeFailed(message + " Details: " + error));

        //    return new Message<Key>(Key.Other, SerializeSucces(message));
        //}
        //private static Message<Key> StartDistributedTest(string message) {
        //    var distributedTestProject = Solution.ActiveSolution.GetProject("DistributedTestProject");
        //    int index = int.Parse(message.Split('/')[2]) - 1;
        //    var distributedTest = distributedTestProject[index] as DistributedTest.DistributedTest;

        //    DistributedTestView view = null;
        //    string error = string.Empty;
        //    SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
        //        view = distributedTest.Activate() as DistributedTestView;
        //        view.StartDistributedTest(false);
        //    }, null);

        //    while (view.DistributedTestMode == DistributedTestMode.Test)
        //        Thread.Sleep(1);

        //    //Wait for all progress messages to come in.
        //    Thread.Sleep(5000);

        //    if (error.Length != 0)
        //        return new Message<Key>(Key.Other, SerializeFailed(message + " Details: " + error));

        //    return new Message<Key>(Key.Other, SerializeSucces(message));
        //}
        //private static Message<Key> StartSingleTest(string message) {
        //    var stressTestProject = Solution.ActiveSolution.GetProject("StressTestProject");
        //    int index = int.Parse(message.Split('/')[2]) + 2;
        //    var stressTest = stressTestProject[index] as StressTest.StressTest;

        //    StressTestView view = null;
        //    string error = string.Empty;
        //    SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
        //        view = stressTest.Activate() as StressTestView;
        //        view.StartStressTest(false);
        //    }, null);

        //    while (view.StressTestStatus == StressTestStatus.Busy)
        //        Thread.Sleep(1);

        //    if (error.Length != 0)
        //        return new Message<Key>(Key.Other, SerializeFailed(message + " Details: " + error));

        //    return new Message<Key>(Key.Other, SerializeSucces(message));
        //}
        //private static Message<Key> StartMonitor(string message) {
        //    var monitorProject = Solution.ActiveSolution.GetProject("MonitorProject");
        //    string[] split = message.Split('/');
        //    int index = int.Parse(split[2]) - 1;
        //    int timeInSeconds = int.Parse(split[3]);
        //    var monitor = monitorProject[index] as Monitor.Monitor;

        //    MonitorView view = null;
        //    string error = string.Empty;
        //    SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
        //        view = monitor.Activate() as MonitorView;
        //        view.InitializeForStressTest();
        //        view.MonitorInitialized += (object sender, MonitorView.MonitorInitializedEventArgs e) => { _jobWaitHandle.Set(); };
        //    }, null);


        //    _jobWaitHandle.WaitOne();
        //    SynchronizationContextWrapper.SynchronizationContext.Send((state) => { view.Start(); }, null);


        //    if (error.Length != 0)
        //        return new Message<Key>(Key.Other, SerializeFailed(message + " Details: " + error));

        //    return new Message<Key>(Key.Other, SerializeSucces(message));
        //}

        //private static Message<Key> StopTestAndMonitors(string message) {
        //    _jobWaitHandle.Set();

        //    SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
        //        foreach (var view in SolutionComponentViewManager.GetAllViews())
        //            if (view is DistributedTestView) {
        //                var dtv = view as DistributedTestView;
        //                dtv.StopDistributedTest();
        //            } else if (view is StressTestView) {
        //                var stv = view as StressTestView;
        //                stv.StopStressTest();
        //            } else if (view is MonitorView) {
        //                var mv = view as MonitorView;
        //                mv.Stop();
        //            }
        //    }, null);

        //    string error = string.Empty;
        //    if (error.Length != 0)
        //        return new Message<Key>(Key.Other, SerializeFailed(message + " Details: " + error));

        //    return new Message<Key>(Key.Other, SerializeSucces(message));
        //}

        //private static Message<Key> ApplicationLog(string message) {
        //    string log = string.Empty;
        //    string currentLogFile = Loggers.GetLogger<FileLogger>().CurrentLogFile;
        //    if (File.Exists(currentLogFile))
        //        using (var sr = new StreamReader(currentLogFile))
        //            log = sr.ReadToEnd();

        //    return new Message<Key>(Key.Other, log);
        //}
        //private static Message<Key> ResultsDB(string message) {
        //    return new Message<Key>(Key.Other, JsonConvert.SerializeObject(ConnectionStringManager.GetCurrentConnectionString(ConnectionStringManager.CurrentDatabaseName)));
        //}

        //private static Message<Key> RunningTestConfig(string message) {
        //    return new Message<Key>(Key.Other, JsonConvert.SerializeObject(JSONObjectTreeHelper.RunningTestConfig));
        //}
        //private static Message<Key> RunningTestFastResults(string message) {
        //    return new Message<Key>(Key.Other, JsonConvert.SerializeObject(JSONObjectTreeHelper.RunningTestFastConcurrencyResults));
        //}
        //private static Message<Key> RunningTestClientMonitor(string message) {
        //    return new Message<Key>(Key.Other, JsonConvert.SerializeObject(JSONObjectTreeHelper.RunningTestClientMonitorMetrics));
        //}
        //private static Message<Key> RunningTestMessages(string message) {
        //    var runningTestMessages = JSONObjectTreeHelper.RunningTestMessages;
        //    if (!message.EndsWith("/info") && runningTestMessages != null) {
        //        object key = runningTestMessages.Cache[0].Key;
        //        ClientMessages value = (ClientMessages)runningTestMessages.Cache[0].Value;

        //        var l = new List<string>();

        //        string check = "Warning";
        //        if (message.EndsWith("/error"))
        //            check = "Error";

        //        foreach (string s in value.Messages)
        //            if (s.StartsWith(check))
        //                l.Add(s);

        //        value.Messages = l.ToArray();
        //        runningTestMessages.Cache[0] = new KeyValuePair<object, object>(key, value);
        //    }

        //    return new Message<Key>(Key.Other, JsonConvert.SerializeObject(runningTestMessages));
        //}
        //private static Message<Key> RunningTestFastMonitorResults(string message) {
        //    return new Message<Key>(Key.Other, JsonConvert.SerializeObject(JSONObjectTreeHelper.RunningTestMessages));
        //}

        //private static Message<Key> TileStressTestConfig(string message) {
        //    var runningTestConfig = JSONObjectTreeHelper.RunningTestConfig;
        //    if ((runningTestConfig.Cache[0].Key as string).StartsWith("Distributed test")) {
        //        string[] split = message.Split('/');
        //        int tileIndex = int.Parse(split[3]);
        //        int testIndex = int.Parse(split[5]);

        //        string tileStressTest = "Tile " + tileIndex + " StressTest " + testIndex;
        //        List<KeyValuePair<object, object>> tileStressTests = (runningTestConfig.Cache[0].Value as JSONObjectTree).Cache;

        //        foreach (var kvp in tileStressTests)
        //            if ((kvp.Key as string).StartsWith(tileStressTest))
        //                return new Message<Key>(Key.Other, JsonConvert.SerializeObject(kvp.Value));
        //    }

        //    return new Message<Key>(Key.Other, JsonConvert.SerializeObject(null));
        //}
        //private static Message<Key> TileStressTestFastResults(string message) {
        //    JSONObjectTree part = null;
        //    var runningTestFastConcurrencyResults = JSONObjectTreeHelper.RunningTestFastConcurrencyResults;
        //    if ((runningTestFastConcurrencyResults.Cache[0].Key as string).StartsWith("Distributed test")) {
        //        string[] split = message.Split('/');
        //        int tileIndex = int.Parse(split[3]);
        //        int testIndex = int.Parse(split[5]);

        //        string tileStressTest = "Tile " + tileIndex + " StressTest " + testIndex;
        //        List<KeyValuePair<object, object>> tileStressTests = (runningTestFastConcurrencyResults.Cache[0].Value as JSONObjectTree).Cache;

        //        foreach (var kvp in tileStressTests)
        //            if ((kvp.Key as string).StartsWith(tileStressTest)) {
        //                part = kvp.Value as JSONObjectTree;
        //                break;
        //            }
        //    }

        //    return new Message<Key>(Key.Other, JsonConvert.SerializeObject(part));
        //}
        //private static Message<Key> TileStressTestClientMonitor(string message) {
        //    var runningTestClientMonitor = JSONObjectTreeHelper.RunningTestClientMonitorMetrics;
        //    if ((runningTestClientMonitor.Cache[0].Key as string).StartsWith("Distributed test")) {
        //        string[] split = message.Split('/');
        //        int tileIndex = int.Parse(split[3]);
        //        int testIndex = int.Parse(split[5]);

        //        string tileStressTest = "Tile " + tileIndex + " StressTest " + testIndex;
        //        foreach (var kvp in runningTestClientMonitor.Cache)
        //            if ((kvp.Key as string).StartsWith(tileStressTest))
        //                return new Message<Key>(Key.Other, JsonConvert.SerializeObject(kvp.Value));
        //    }

        //    return new Message<Key>(Key.Other, JsonConvert.SerializeObject(null));
        //}
        //private static Message<Key> TilestressTestMessages(string message) {
        //    var runningTestMessages = JSONObjectTreeHelper.RunningTestMessages;
        //    if ((runningTestMessages.Cache[0].Key as string).StartsWith("Distributed test")) {
        //        string[] split = message.Split('/');
        //        int tileIndex = int.Parse(split[3]);
        //        int testIndex = int.Parse(split[5]);

        //        string tileStressTest = "Tile " + tileIndex + " StressTest " + testIndex;
        //        foreach (var kvp in runningTestMessages.Cache)
        //            if ((kvp.Key as string).StartsWith(tileStressTest))
        //                return new Message<Key>(Key.Other, JsonConvert.SerializeObject(kvp.Value));
        //    }

        //    return new Message<Key>(Key.Other, JsonConvert.SerializeObject(null));
        //}
        //private static Message<Key> TileFastMonitorResults(string message) {
        //    throw new NotImplementedException();
        //}

        //private static Message<Key> RunningMonitorConfig(string message) {
        //    string[] split = message.Split('/');
        //    int index = int.Parse(split[2]);

        //    JSONObjectTree part = GetPart(JSONObjectTreeHelper.RunningMonitorConfig, index);
        //    return new Message<Key>(Key.Other, JsonConvert.SerializeObject(part));
        //}
        //private static Message<Key> RunningMonitorHardwareConfig(string message) {
        //    string[] split = message.Split('/');
        //    int index = int.Parse(split[2]);

        //    JSONObjectTree part = GetPart(JSONObjectTreeHelper.RunningMonitorHardwareConfig, index);
        //    return new Message<Key>(Key.Other, JsonConvert.SerializeObject(part));
        //}
        //private static Message<Key> RunningMonitorMetrics(string message) {
        //    string[] split = message.Split('/');
        //    int index = int.Parse(split[split.Length - 2]);

        //    JSONObjectTree part = GetPart(JSONObjectTreeHelper.RunningMonitorMetrics, index);
        //    return new Message<Key>(Key.Other, JsonConvert.SerializeObject(part));
        //}
        //private static JSONObjectTree GetPart(JSONObjectTree tree, int index) {
        //    for (int i = 0; i != tree.Count; i++) {
        //        var kvp = tree.Cache[i];
        //        if ((int)kvp.Key == index)
        //            return kvp.Value as JSONObjectTree;
        //    }
        //    return null;
        //}

        private static string GetRPCSuccess(string message) { return "Success " + message; }
        private static string GetRPCFailed(string message, Exception ex) {
            Loggers.Log(Level.Error, "RPC communication failed: " + message, ex);
            return "Failed " + message;
        }
    }
}
