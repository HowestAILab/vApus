/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using vApus.DistributedTesting;
using vApus.JSON;
using vApus.Monitor;
using vApus.Results;
using vApus.Server.Shared;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.Server {
    public static class CommunicationHandler {
        private static AutoResetEvent _jobWaitHandle = new AutoResetEvent(false);

        private delegate Message<Key> HandleMessageDelegate(string message);
        /// <summary>
        /// Holds the whole or the significant part of the path as Key. Specifics are handled by the matching delegate.
        /// </summary>
        private static Dictionary<string, HandleMessageDelegate> _delegates;

        static CommunicationHandler() {
            // Fill delegates
            _delegates = new Dictionary<string, HandleMessageDelegate>();

            // Send stuff that needs to be done. You receive an ack as plain text JSON.
            // Indices are one-based. If no index is given, you get all available indices and the string representation of the objects.
            _delegates.Add("/testconnection/#", TestConnection);
            _delegates.Add("/startdistributedtest/#", StartDistributedTest);
            _delegates.Add("/startsingletest/#", StartSingleTest);
            _delegates.Add("/startmonitor/#/#", StartMonitor);
            _delegates.Add("/stoptestandmonitors", StopTestAndMonitors);

            // -----

            // Get data back as plain text JSON
            _delegates.Add("/applicationlog/info", ApplicationLog);
            _delegates.Add("/applicationlog/warning", ApplicationLog);
            _delegates.Add("/applicationlog/error", ApplicationLog);
            _delegates.Add("/applicationlog/fatal", ApplicationLog);
            _delegates.Add("/resultsdb", ResultsDB);

            // For a single test and distributed test (as a whole)
            _delegates.Add("/runningtest/config", RunningTestConfig);
            _delegates.Add("/runningtest/fastresults", RunningTestFastResults);
            _delegates.Add("/runningtest/clientmonitor", RunningTestClientMonitor);
            _delegates.Add("/runningtest/messages/info", RunningTestMessages);
            _delegates.Add("/runningtest/messages/warning", RunningTestMessages);
            _delegates.Add("/runningtest/messages/error", RunningTestMessages);
            _delegates.Add("/runningtest/fastmonitorresults/#", RunningTestFastMonitorResults);


            // For a tile stresstest            
            _delegates.Add("/runningtest/tile/#/tilestresstest/#/config", TileStresstestConfig);
            _delegates.Add("/runningtest/tile/#/tilestresstest/#/fastresults", TileStresstestFastResults);
            _delegates.Add("/runningtest/tile/#/tilestresstest/#/clientmonitor", RunningTestClientMonitor);
            _delegates.Add("/runningtest/tile/#/tilestresstest/#/messages/info", RunningTestMessages);
            _delegates.Add("/runningtest/tile/#/tilestresstest/#/messages/warning", RunningTestMessages);
            _delegates.Add("/runningtest/tile/#/tilestresstest/#/messages/error", RunningTestMessages);
            _delegates.Add("/runningtest/tile/#/tilestresstest/#/fastmonitorresults/#", TestConnection);


            _delegates.Add("/runningmonitor/#/config", RunningMonitorConfig);
            _delegates.Add("/runningmonitor/#/hardwareconfig", RunningMonitorHardwareConfig);
            _delegates.Add("/runningmonitor/#/metrics", RunningMonitorMetrics);
        }

        public static Message<Key> HandleMessage(SocketWrapper receiver, Message<Key> message) {
            if (message.Key == Key.Other) {
                string msg = message.Content as string;
                foreach (string path in _delegates.Keys) {
                    if (Match(msg, path))
                        try {
                            return _delegates[path].Invoke(msg);
                        } catch (Exception ex) {
                            return new Message<Key>(Key.Other, SerializeFailed("500 Internal Server Error. " + msg + " Details: " + ex.Message));
                        }
                }
                return new Message<Key>(Key.Other, SerializeFailed("404 Not Found. " + msg));
            }
            return SlaveSideCommunicationHandler.HandleMessage(receiver, message);
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

        private static Message<Key> TestConnection(string message) {
            var connections = Solution.ActiveSolution.GetSolutionComponent(typeof(Connections));
            int index = int.Parse(message.Split('/')[2]); // -1 not needed, connection proxies is the first item.
            var connection = connections[index] as Connection;
            ConnectionView view = null;
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => { view = connection.Activate() as ConnectionView; }, null);

            string error = view.TestConnection(false);
            if (error.Length != 0)
                return new Message<Key>(Key.Other, SerializeFailed(message + " Details: " + error));

            return new Message<Key>(Key.Other, SerializeSucces(message));
        }
        private static Message<Key> StartDistributedTest(string message) {
            var distributedTestingProject = Solution.ActiveSolution.GetProject("DistributedTestingProject");
            int index = int.Parse(message.Split('/')[2]) - 1;
            var distributedTest = distributedTestingProject[index] as DistributedTest;

            DistributedTestView view = null;
            string error = string.Empty;
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                view = distributedTest.Activate() as DistributedTestView;
                view.StartDistributedTest(false);
            }, null);

            while (view.DistributedTestMode == DistributedTestMode.Test)
                Thread.Sleep(1);

            //Wait for all progress messages to come in.
            Thread.Sleep(30000);


            if (error.Length != 0)
                return new Message<Key>(Key.Other, SerializeFailed(message + " Details: " + error));

            return new Message<Key>(Key.Other, SerializeSucces(message));
        }
        private static Message<Key> StartSingleTest(string message) {
            var stresstestProject = Solution.ActiveSolution.GetProject("StresstestProject");
            int index = int.Parse(message.Split('/')[2]) + 2;
            var stresstest = stresstestProject[index] as Stresstest.Stresstest;

            StresstestView view = null;
            string error = string.Empty;
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                view = stresstest.Activate() as StresstestView;
                view.StartStresstest(false);
            }, null);

            while (view.StresstestStatus == StresstestStatus.Busy)
                Thread.Sleep(1);

            if (error.Length != 0)
                return new Message<Key>(Key.Other, SerializeFailed(message + " Details: " + error));

            return new Message<Key>(Key.Other, SerializeSucces(message));
        }

        private static Message<Key> StartMonitor(string message) {
            var monitorProject = Solution.ActiveSolution.GetProject("MonitorProject");
            string[] split = message.Split('/');
            int index = int.Parse(split[2]) - 1;
            int timeInSeconds = int.Parse(split[3]);
            var monitor = monitorProject[index] as Monitor.Monitor;

            MonitorView view = null;
            string error = string.Empty;
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                view = monitor.Activate() as MonitorView;
                view.InitializeForStresstest();
                view.MonitorInitialized += (object sender, MonitorView.MonitorInitializedEventArgs e) => { _jobWaitHandle.Set(); };
            }, null);


            _jobWaitHandle.WaitOne();
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => { view.Start(); }, null);
            _jobWaitHandle.WaitOne(timeInSeconds * 1000);
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => { view.Stop(); }, null);


            if (error.Length != 0)
                return new Message<Key>(Key.Other, SerializeFailed(message + " Details: " + error));

            return new Message<Key>(Key.Other, SerializeSucces(message));
        }

        private static Message<Key> StopTestAndMonitors(string message) {
            _jobWaitHandle.Set();

            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                foreach (var view in SolutionComponentViewManager.GetAllViews())
                    if (view is DistributedTestView) {
                        var dtv = view as DistributedTestView;
                        dtv.StopDistributedTest();
                    } else if (view is StresstestView) {
                        var stv = view as StresstestView;
                        stv.StopStresstest();
                    } else if (view is MonitorView) {
                        var mv = view as MonitorView;
                        mv.Stop();
                    }
            }, null);

            string error = string.Empty;
            if (error.Length != 0)
                return new Message<Key>(Key.Other, SerializeFailed(message + " Details: " + error));

            return new Message<Key>(Key.Other, SerializeSucces(message));
        }

        private static Message<Key> ApplicationLog(string message) {
            FileInfo fi = null;
            if (File.Exists(LogWrapper.Default.Logger.LogFile))
                fi = new FileInfo(LogWrapper.Default.Logger.LogFile);
            else if (Directory.Exists(LogWrapper.Default.Logger.Location))
                foreach (string file in Directory.GetFiles(LogWrapper.Default.Logger.Location)) {
                    var tempfi = new FileInfo(file);
                    if (fi == null || tempfi.CreationTime > fi.CreationTime)
                        if (IsLog(tempfi.Name)) {
                            fi = tempfi;
                            break;
                        }
                }

            string latestLog = null;
            if (fi != null)
                latestLog = fi.FullName;

            string logEntries = string.Empty;
            if (File.Exists(latestLog)) {
                //Fast read this, if it fails once it is not a problem.
                try {
                    LogWrapper.Default.Logger.CloseWriter();
                    using (var sr = new StreamReader(latestLog))
                        logEntries = sr.ReadToEnd().Trim();
                } catch {
                } finally {
                    try {
                        LogWrapper.Default.Logger.OpenOrReOpenWriter();
                    } catch {
                    }
                }

                if (!message.EndsWith("info")) {
                    string[] lines = logEntries.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    logEntries = string.Empty;

                    int chosenLogLevel = 0;
                    if (message.EndsWith("warning")) chosenLogLevel = 1;
                    else if (message.EndsWith("error")) chosenLogLevel = 2;
                    else if (message.EndsWith("fatal")) chosenLogLevel = 3;

                    var tempOutput = new StringBuilder();
                    foreach (string line in lines) {
                        string[] entry = line.Split(';');
                        if (entry.Length >= 3) {
                            DateTime timeStamp;
                            LogLevel logLevel;

                            string[] timeStampSplit = entry[0].Split(',');
                            string dateTimePart = timeStampSplit[0];
                            if (DateTime.TryParse(dateTimePart, out timeStamp) && Enum.TryParse(entry[1], out logLevel))
                                if ((int)logLevel >= chosenLogLevel) {
                                    tempOutput.AppendLine(line);
                                    //Continue if valid line
                                    continue;
                                }
                        }

                        string s = tempOutput.ToString();
                        if (s.Length != 0)
                            logEntries += s + "\n";
                        tempOutput.Clear();
                    }
                    logEntries.TrimEnd();
                }
            }

            return new Message<Key>(Key.Other, JsonConvert.SerializeObject(logEntries));
        }
        private static bool IsLog(string file) {
            if (file.EndsWith(".txt")) {
                string[] split = file.Split(' ');
                if (split.Length == 2) {
                    DateTime timestamp;
                    return (DateTime.TryParse(split[0], out timestamp) && split[1].StartsWith("PID_"));
                }
            }
            return false;
        }
        private static Message<Key> ResultsDB(string message) {
            return new Message<Key>(Key.Other, JsonConvert.SerializeObject(ConnectionStringManager.GetCurrentConnectionString(ConnectionStringManager.CurrentDatabaseName)));
        }

        private static Message<Key> RunningTestConfig(string message) {
            return new Message<Key>(Key.Other, JsonConvert.SerializeObject(JSONObjectTreeHelper.RunningTestConfig));
        }
        private static Message<Key> RunningTestFastResults(string message) {
            return new Message<Key>(Key.Other, JsonConvert.SerializeObject(JSONObjectTreeHelper.RunningTestFastConcurrencyResults));
        }
        private static Message<Key> RunningTestClientMonitor(string message) {
            return new Message<Key>(Key.Other, JsonConvert.SerializeObject(JSONObjectTreeHelper.RunningTestClientMonitorMetrics));
        }
        private static Message<Key> RunningTestMessages(string message) {
            var runningTestMessages = JSONObjectTreeHelper.RunningTestMessages;
            if (!message.EndsWith("/info") && runningTestMessages != null) {
                object key = runningTestMessages.Cache[0].Key;
                ClientMessages value = (ClientMessages)runningTestMessages.Cache[0].Value;

                var l = new List<string>();

                string check = "Warning";
                if (message.EndsWith("/error"))
                    check = "Error";

                foreach (string s in value.Messages)
                    if (s.StartsWith(check))
                        l.Add(s);

                value.Messages = l.ToArray();
                runningTestMessages.Cache[0] = new KeyValuePair<object, object>(key, value);
            }

            return new Message<Key>(Key.Other, JsonConvert.SerializeObject(runningTestMessages));
        }
        private static Message<Key> RunningTestFastMonitorResults(string message) {
            return new Message<Key>(Key.Other, JsonConvert.SerializeObject(JSONObjectTreeHelper.RunningTestMessages));
        }

        private static Message<Key> TileStresstestConfig(string message) {
            throw new NotImplementedException();
        }
        private static Message<Key> TileStresstestFastResults(string message) {
            JSONObjectTree part = null;
            var runningTestFastConcurrencyResults = JSONObjectTreeHelper.RunningTestFastConcurrencyResults;
            if ((runningTestFastConcurrencyResults.Cache[0].Key as string).StartsWith("Distributed Test")) {
                string[] split = message.Split('/');
                int tileIndex = int.Parse(split[3]);
                int testIndex = int.Parse(split[5]);

                string tileStresstest = "Tile " + tileIndex + " Stresstest " + testIndex;
                List<KeyValuePair<object, object>> tileStresstests = (runningTestFastConcurrencyResults.Cache[0].Value as JSONObjectTree).Cache;

                foreach (var kvp in tileStresstests)
                    if ((kvp.Key as string).StartsWith(tileStresstest)) {
                        part = kvp.Value as JSONObjectTree;
                        break;
                    }
            }

            return new Message<Key>(Key.Other, JsonConvert.SerializeObject(part));
        }

        private static Message<Key> RunningMonitorConfig(string message) {
            string[] split = message.Split('/');
            int index = int.Parse(split[2]);

            JSONObjectTree part = GetPart(JSONObjectTreeHelper.RunningMonitorConfig, index);
            return new Message<Key>(Key.Other, JsonConvert.SerializeObject(part));
        }
        private static Message<Key> RunningMonitorHardwareConfig(string message) {
            string[] split = message.Split('/');
            int index = int.Parse(split[2]);

            JSONObjectTree part = GetPart(JSONObjectTreeHelper.RunningMonitorHardwareConfig, index);
            return new Message<Key>(Key.Other, JsonConvert.SerializeObject(part));
        }
        private static Message<Key> RunningMonitorMetrics(string message) {
            string[] split = message.Split('/');
            int index = int.Parse(split[split.Length - 2]);

            JSONObjectTree part = GetPart(JSONObjectTreeHelper.RunningMonitorMetrics, index);
            return new Message<Key>(Key.Other, JsonConvert.SerializeObject(part));
        }
        private static JSONObjectTree GetPart(JSONObjectTree tree, int index) {
            for (int i = 0; i != tree.Count; i++) {
                var kvp = tree.Cache[i];
                if ((int)kvp.Key == index)
                    return kvp.Value as JSONObjectTree;
            }
            return null;
        }

        private static string SerializeSucces(string message) { return SerializeStatusMessage("succes", message); }
        private static string SerializeFailed(string message) { return SerializeStatusMessage("failed", message); }
        private static string SerializeStatusMessage(string status, string message) { return JsonConvert.SerializeObject(new statusmessage() { status = status, message = message }); }

        private struct statusmessage {
            public string status;
            public string message;
        }
    }
}
