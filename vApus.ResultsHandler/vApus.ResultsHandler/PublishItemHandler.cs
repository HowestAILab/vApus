using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading;
using vApus.Publish;
using vApus.Results;
using vApus.Util;
using RandomUtils;

namespace vApus.ResultsHandler {
    internal static class PublishItemHandler {
        private static ConcurrentDictionary<string, HandleObject> _handleObjects = new ConcurrentDictionary<string, HandleObject>();

        private static string _passwordGUID = "{51E6A7AC-06C2-466F-B7E8-4B0A00F6A21F}";

        private static readonly byte[] _salt = { 0x49, 0x16, 0x49, 0x2e, 0x11, 0x1e, 0x45, 0x24, 0x86, 0x05, 0x01, 0x03, 0x62 };

        static PublishItemHandler() {
            ConnectionStringManager.AddConnectionString("root", "127.0.0.1", 3306, "BDaEWS2015!");
        }

        public static void Handle(PublishItem[] items) {
            foreach (var item in items) {
                string id = item.ResultSetId + item.vApusHost + item.vApusPort;
                if (!_handleObjects.ContainsKey(id))
                    _handleObjects.TryAdd(id, new HandleObject());

                _handleObjects[id].Handle(item);
            }
        }

        private class HandleObject {
            private DatabaseActions _databaseActions;
            private int _vApusInstanceId = -1, _testId = -1, _concurrencyId = -1, _run = -1;

            private MessageQueue _requestsMessageQueue = new MessageQueue();

            private HashSet<string> _monitorsMissingHeaders = new HashSet<string>();
            private ConcurrentDictionary<string, ulong> _monitorsWithIds = new ConcurrentDictionary<string, ulong>();

            private MessageQueue _monitorMessageQueue = new MessageQueue();

            public HandleObject() {
                _databaseActions = Schema.GetDatabaseActionsUsingDatabase(Schema.Build());
                _requestsMessageQueue.OnDequeue += _requestsMessageQueue_OnDequeue;
                _monitorMessageQueue.OnDequeue += _monitorMessageQueue_OnDequeue;
            }

            public void Handle(PublishItem item) {
                switch (item.PublishItemType) {
                    case "DistributedTestConfiguration":
                        HandleDistributedTestConfiguration(item);
                        break;
                    case "StressTestConfiguration":
                        HandleStressTestConfiguration(item);
                        break;
                    case "TileStressTestConfiguration":
                        HandleTileStressTestConfiguration(item);
                        break;
                    case "FastConcurrencyResults":
                        HandleFastConcurrencyResults(item);
                        break;
                    case "FastRunResults":
                        HandleFastRunResults(item);
                        break;
                    case "RequestResults":
                        HandleRequestResults(item);
                        break;
                    case "ClientMonitorMetrics":
                        HandleClientMonitorMetrics(item);
                        break;
                    case "TestMessage":
                        HandleTestMessage(item);
                        break;
                    case "ApplicationLogEntry":
                        HandleApplicationLogEntry(item);
                        break;
                    case "MonitorConfiguration":
                        HandleMonitorConfiguration(item);
                        break;
                    case "MonitorMetrics":
                        HandleMonitorMetrics(item);
                        break;
                }
            }

            private void HandleDistributedTestConfiguration(PublishItem item) {
                var pi = item as DistributedTestConfiguration;
                SetvApusInstance(pi.vApusHost, pi.vApusHost, pi.vApusPort, pi.vApusVersion, pi.vApusChannel, pi.vApusIsMaster);
                SetDescriptionAndTags(pi.Description, pi.Tags);
            }
            private void HandleStressTestConfiguration(PublishItem item) {
                var pi = item as StressTestConfiguration;
                SetvApusInstance(pi.vApusHost, pi.vApusHost, pi.vApusPort, pi.vApusVersion, pi.vApusChannel, pi.vApusIsMaster);
                SetDescriptionAndTags(pi.Description, pi.Tags);
                SetStressTest(pi.StressTest, "None", pi.Connection, pi.ConnectionProxy, "", pi.ScenariosAndWeights.ToString(), pi.ScenarioRuleSet,
                    pi.Concurrencies, pi.Runs, pi.InitialMinimumDelayInMilliseconds, pi.InitialMaximumDelayInMilliseconds, pi.MinimumDelayInMilliseconds, pi.MaximumDelayInMilliseconds, pi.Shuffle, pi.ActionDistribution, pi.MaximumNumberOfUserActions,
                    pi.MonitorBeforeInMinutes, pi.MonitorAfterInMinutes, pi.UseParallelExecutionOfRequests, pi.MaximumPersistentConnections, pi.PersistentConnectionsPerHostname);

            }
            private void HandleTileStressTestConfiguration(PublishItem item) {
                var pi = item as TileStressTestConfiguration;
                SetvApusInstance(pi.vApusHost, pi.vApusHost, pi.vApusPort, pi.vApusVersion, pi.vApusChannel, pi.vApusIsMaster);
                SetStressTest(pi.TileStressTest, "None", pi.Connection, pi.ConnectionProxy, "", pi.ScenariosAndWeights.ToString(), pi.ScenarioRuleSet,
                    pi.Concurrencies, pi.Runs, pi.InitialMinimumDelayInMilliseconds, pi.InitialMaximumDelayInMilliseconds, pi.MinimumDelayInMilliseconds, pi.MaximumDelayInMilliseconds, pi.Shuffle, pi.ActionDistribution, pi.MaximumNumberOfUserActions,
                    pi.MonitorBeforeInMinutes, pi.MonitorAfterInMinutes, pi.UseParallelExecutionOfRequests, pi.MaximumPersistentConnections, pi.PersistentConnectionsPerHostname);
            }
            private void HandleFastConcurrencyResults(PublishItem item) { }
            private void HandleFastRunResults(PublishItem item) { }
            private void HandleRequestResults(PublishItem item) {
                if (_testId != -1)
                    _requestsMessageQueue.Enqueue(item as RequestResults);
            }

            private void _requestsMessageQueue_OnDequeue(object sender, MessageQueue.OnDequeueEventArgs e) {

            }

            private void HandleClientMonitorMetrics(PublishItem item) { }
            private void HandleTestMessage(PublishItem item) { }
            private void HandleApplicationLogEntry(PublishItem item) { }
            private void HandleMonitorConfiguration(PublishItem item) {
                if (_testId != -1) {
                    var pi = item as MonitorConfiguration;
                    ulong id = SetMonitor(_testId, pi.Monitor, pi.MonitorSource, "", pi.HardwareConfiguration);
                    _monitorsMissingHeaders.Add(pi.Monitor);
                    _monitorsWithIds.TryAdd(pi.Monitor, id);
                }
            }
            private void HandleMonitorMetrics(PublishItem item) {
                if (_testId != -1) {
                    var pi = item as Publish.MonitorMetrics;
                    ulong monitorId = _monitorsWithIds[pi.Monitor];
                    if (_monitorsMissingHeaders.Contains(pi.Monitor)) {
                        SetMonitor(monitorId, pi.Headers);
                        _monitorsMissingHeaders.Remove(pi.Monitor);
                    }
                    _monitorMessageQueue.Enqueue(pi);
                }
            }

            private void _monitorMessageQueue_OnDequeue(object sender, MessageQueue.OnDequeueEventArgs e) { SetMonitorResults(e.Messages as Publish.MonitorMetrics[]); }

            private void SetDescriptionAndTags(string description, string[] tags) {
                _databaseActions.ExecuteSQL("DELETE FROM description");
                _databaseActions.ExecuteSQL("DELETE FROM tags");

                _databaseActions.ExecuteSQL("INSERT INTO description(Description) VALUES('" + description + "')");
                var rowsToInsert = new List<string>(); //Insert multiple values at once.

                foreach (string tag in tags) {
                    var sb = new StringBuilder("('");
                    sb.Append(tag);
                    sb.Append("')");
                    rowsToInsert.Add(sb.ToString());
                }
                _databaseActions.ExecuteSQL(string.Format("INSERT INTO tags(Tag) VALUES {0};", rowsToInsert.Combine(", ")));
            }
            public int SetvApusInstance(string hostName, string ip, int port, string version, string channel, bool isMaster) {
                if (_databaseActions != null) {
                    _databaseActions.ExecuteSQL(
                        string.Format("INSERT INTO vapusinstances(HostName, IP, Port, Version, Channel, IsMaster) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                        hostName, ip, port, version, channel, isMaster ? 1 : 0)
                        );
                    _vApusInstanceId = (int)_databaseActions.GetLastInsertId();
                    return _vApusInstanceId;
                }
                return 0;
            }
            private int SetStressTest(string stressTest, string runSynchronization, string connection, string connectionProxy, string connectionString, string scenarios, string scenarioRuleSet, int[] concurrencies, int runs,
                                             int initialMinimumDelayInMilliseconds, int initialMaximumDelayInMilliseconds, int minimumDelayInMilliseconds, int maximumDelayInMilliseconds, bool shuffle, bool actionDistribution,
                int maximumNumberOfUserActions, int monitorBeforeInMinutes, int monitorAfterInMinutes, bool useParallelExecutionOfRequests, int maximumPersistentConnections, int persistentConnectionsPerHostname) {
                if (_databaseActions != null) {
                    _databaseActions.ExecuteSQL(
                        string.Format(@"INSERT INTO stresstests(vApusInstanceId, StressTest, RunSynchronization, Connection, ConnectionProxy, ConnectionString, Scenarios, ScenarioRuleSet, Concurrencies, Runs,
InitialMinimumDelayInMilliseconds, InitialMaximumDelayInMilliseconds, MinimumDelayInMilliseconds, MaximumDelayInMilliseconds, Shuffle, ActionDistribution, MaximumNumberOfUserActions, MonitorBeforeInMinutes, MonitorAfterInMinutes,
UseParallelExecutionOfRequests, MaximumPersistentConnections, PersistentConnectionsPerHostname)
VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}')",
                                      _vApusInstanceId, stressTest, runSynchronization, connection, connectionProxy, connectionString.Encrypt(_passwordGUID, _salt), scenarios, scenarioRuleSet,
                                      concurrencies.Combine(", "), runs, initialMinimumDelayInMilliseconds, initialMaximumDelayInMilliseconds, minimumDelayInMilliseconds, maximumDelayInMilliseconds, shuffle ? 1 : 0, actionDistribution ? 1 : 0,
                                      maximumNumberOfUserActions, monitorBeforeInMinutes, monitorAfterInMinutes, useParallelExecutionOfRequests ? 1 : 0, maximumPersistentConnections, persistentConnectionsPerHostname)
                        );
                    _testId = (int)_databaseActions.GetLastInsertId();
                    return _testId;
                }
                return 0;
            }

            private ulong SetMonitor(int stressTestId, string monitor, string monitorSource, string connectionString, string machineConfiguration) {
                if (_databaseActions != null) {
                    if (machineConfiguration == null) machineConfiguration = string.Empty;
                    _databaseActions.ExecuteSQL(
                        string.Format(
                            "INSERT INTO monitors(StressTestId, Monitor, MonitorSource, ConnectionString, MachineConfiguration) VALUES('{0}', ?Monitor, ?MonitorSource, ?ConnectionString, ?MachineConfiguration)", stressTestId),
                            CommandType.Text, new MySqlParameter("?Monitor", monitor), new MySqlParameter("?MonitorSource", monitorSource), new MySqlParameter("?ConnectionString", connectionString.Encrypt(_passwordGUID, _salt)),
                                new MySqlParameter("?MachineConfiguration", machineConfiguration)
                        );
                    return _databaseActions.GetLastInsertId();
                }
                return 0;
            }

            private ulong SetMonitor(ulong monitorId, string[] resultHeaders) {
                if (_databaseActions != null) {
                    _databaseActions.ExecuteSQL(
                        string.Format(
                            "UPDATE monitors SET ResultHeaders = ?ResultHeaders where Id = {0}", monitorId),
                            CommandType.Text, new MySqlParameter("?ResultHeaders", resultHeaders.Combine("; ", string.Empty))
                        );
                    return _databaseActions.GetLastInsertId();
                }
                return 0;
            }

            public void SetMonitorResults(Publish.MonitorMetrics[] monitorMetrics) {
                //Store monitor values with a ',' for decimal seperator.
                CultureInfo prevCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("nl-BE");
                if (_databaseActions != null && monitorMetrics.Length != 0) {
                    var rowsToInsert = new List<string>(); //Insert multiple values at once.
                    foreach (var metrics in monitorMetrics) {
                        var value = new List<string>();
                        for (int i = 1; i < metrics.Values.Length; i++) {
                            object o = metrics.Values[i];
                            value.Add((o is double) ? StringUtil.DoubleToLongString((double)o) : o.ToString());
                        }

                        var sb = new StringBuilder("('");
                        sb.Append(_monitorsWithIds[metrics.Monitor]);
                        sb.Append("', '");
                        sb.Append(Parse(PublishItem.EpochUtc.AddMilliseconds(metrics.PublishItemTimestampInMillisecondsSinceEpochUtc).ToLocalTime()));
                        sb.Append("', '");
                        sb.Append(MySQLEscapeString(MySQLEscapeString(value.Combine("; "))));
                        sb.Append("')");
                        rowsToInsert.Add(sb.ToString());
                    }
                    _databaseActions.ExecuteSQL(string.Format("INSERT INTO monitorresults(MonitorId, TimeStamp, Value) VALUES {0};", rowsToInsert.Combine(", ")));
                }
                Thread.CurrentThread.CurrentCulture = prevCulture;
            }

            /// <summary>
            /// Parse a date to a valid string for in a MySQL db.
            /// </summary>
            /// <param name="dateTime"></param>
            /// <returns></returns>
            private string Parse(DateTime dateTime) { return dateTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff"); }
            /// <summary>
            ///Mimics PHP's mysql_real_escape_string();
            /// </summary>
            /// <param name="s"></param>
            /// <returns></returns>
            private static string MySQLEscapeString(string s) { return System.Text.RegularExpressions.Regex.Replace(s, @"[\r\n\x00\x1a\\'""]", @"\$0"); }
        }
    }
}
