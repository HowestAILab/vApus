/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using vApus.Util;

namespace vApus.Results {
    public class ResultsHelper {

        #region Fields

        private string _databaseName;

        private DatabaseActions _databaseActions;
        private string _passwordGUID = "{51E6A7AC-06C2-466F-B7E8-4B0A00F6A21F}";

        private readonly byte[] _salt =
            {
                0x49, 0x16, 0x49, 0x2e, 0x11, 0x1e, 0x45, 0x24, 0x86, 0x05, 0x01, 0x03, 0x62
            };

        private ulong _vApusInstanceId, _stresstestId, _stresstestResultId, _concurrencyResultId, _runResultId;

        #endregion

        /// <summary>
        /// Returns null if not connected
        /// </summary>
        public string DatabaseName {
            get { return _databaseName; }
        }

        /// <summary>
        /// You can change this value for when you are distributed testing, so the right dataset is chosen.
        /// This value is not used when filtering results in the procedures eg GetAverageConcurrentUsers.
        /// </summary>
        public ulong StresstestId {
            get { return _stresstestId; }
            set { _stresstestId = value; }
        }

        #region Initialize database before stresstest

        /// <summary>
        ///     Builds the schema if needed, if no db target is found or no connection could be made an exception is returned.
        /// </summary>
        /// <returns></returns>
        public Exception BuildSchemaAndConnect() {
            _databaseName = null;
            try {
                _databaseName = Schema.Build();
                _databaseActions = Schema.GetDatabaseActionsUsingDatabase(_databaseName);
            } catch (Exception ex) {
                if (_databaseActions != null) {
                    _databaseActions.ReleaseConnection();
                    _databaseActions = null;
                }
                return ex;
            }
            return null;
        }

        /// <summary>
        ///     Only inserts if connected (Call BuildSchema).
        /// </summary>
        /// <param name="description"></param>
        /// <param name="tags"></param>
        public void SetDescriptionAndTags(string description, string[] tags) {
            if (_databaseActions != null) {
                _databaseActions.ExecuteSQL("INSERT INTO Description(Description) VALUES('" + description + "')");
                foreach (string tag in tags)
                    _databaseActions.ExecuteSQL("INSERT INTO Tags(Tag) VALUES('" + tag + "')");
            }
        }

        /// <summary>
        /// Do this last for the master, this sets _vApusInstanceId used in this class.
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="version"></param>
        /// <param name="isMaster"></param>
        /// <returns>Id of the instance.</returns>
        public ulong SetvApusInstance(string hostName, string ip, int port, string version, string channel, bool isMaster) {
            if (_databaseActions != null) {
                _databaseActions.ExecuteSQL(
                    string.Format(
                        "INSERT INTO vApusInstances(HostName, IP, Port, Version, Channel, IsMaster) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                        hostName, ip, port, version, channel, isMaster ? 1 : 0)
                    );
                _vApusInstanceId = _databaseActions.GetLastInsertId();
                return _vApusInstanceId;
            }
            return 0;
        }

        /// <summary>
        /// This sets _stresstestId used in this class.
        /// </summary>
        /// <param name="vApusInstanceId"></param>
        /// <param name="stresstest"></param>
        /// <param name="runSynchronization"></param>
        /// <param name="connection"></param>
        /// <param name="connectionProxy"></param>
        /// <param name="connectionString">Will be encrypted.</param>
        /// <param name="log"></param>
        /// <param name="logRuleSet"></param>
        /// <param name="concurrencies"></param>
        /// <param name="runs"></param>
        /// <param name="minimumDelayInMilliseconds"></param>
        /// <param name="maximumDelayInMilliseconds"></param>
        /// <param name="shuffle"></param>
        /// <param name="distribute"></param>
        /// <param name="monitorBeforeInMinutes"></param>
        /// <param name="monitorAfterInMinutes"></param>
        /// <returns>Id of the stresstest.</returns>
        public ulong SetStresstest(string stresstest, string runSynchronization, string connection,
                                         string connectionProxy, string connectionString,
                                         string log, string logRuleSet, int[] concurrencies, int runs,
                                         int minimumDelayInMilliseconds, int maximumDelayInMilliseconds, bool shuffle,
                                         string distribute,
                                         int monitorBeforeInMinutes, int monitorAfterInMinutes) {
            if (_databaseActions != null) {
                _databaseActions.ExecuteSQL(
                    string.Format(@"INSERT INTO Stresstests(
vApusInstanceId, Stresstest, RunSynchronization, Connection, ConnectionProxy, ConnectionString, Log, LogRuleSet, Concurrencies, Runs,
MinimumDelayInMilliseconds, MaximumDelayInMilliseconds, Shuffle, Distribute, MonitorBeforeInMinutes, MonitorAfterInMinutes)
VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}')",
                                  _vApusInstanceId, stresstest, runSynchronization, connection, connectionProxy,
                                  connectionString.Encrypt(_passwordGUID, _salt), log, logRuleSet,
                                  concurrencies.Combine(", "), runs,
                                  minimumDelayInMilliseconds, maximumDelayInMilliseconds, shuffle ? 1 : 0, distribute,
                                  monitorBeforeInMinutes, monitorAfterInMinutes)
                    );
                _stresstestId = _databaseActions.GetLastInsertId();
                return _stresstestId;
            }
            return 0;
        }

        /// <summary>
        /// Use this for single tests.
        /// </summary>
        /// <param name="stresstestId"></param>
        /// <param name="monitor"></param>
        /// <param name="connectionString">Will be encrypted.</param>
        /// <param name="machineConfiguration"></param>
        /// <param name="resultHeaders"></param>
        /// <returns>
        ///     The monitor configuration id in the database, set this in the proper monitor result cache.
        ///     -1 if not connected.
        /// </returns>
        public ulong SetMonitor(string monitor, string monitorSource, string connectionString, string machineConfiguration, string[] resultHeaders) {
            return SetMonitor(_stresstestId, monitor, monitorSource, connectionString, machineConfiguration, resultHeaders);
        }
        /// <summary>
        /// Use this for distributed tests.
        /// </summary>
        /// <param name="stresstestId"></param>
        /// <param name="monitor"></param>
        /// <param name="monitorSource"></param>
        /// <param name="connectionString"></param>
        /// <param name="machineConfiguration"></param>
        /// <param name="resultHeaders"></param>
        /// <returns></returns>
        public ulong SetMonitor(ulong stresstestId, string monitor, string monitorSource, string connectionString, string machineConfiguration, string[] resultHeaders) {
            if (_databaseActions != null) {
                _databaseActions.ExecuteSQL(
                    string.Format(
                        "INSERT INTO Monitors(StresstestId, Monitor, MonitorSource, ConnectionString, MachineConfiguration, ResultHeaders) VALUES('{0}', ?Monitor, ?MonitorSource, ?ConnectionString, ?MachineConfiguration, ?ResultHeaders)", stresstestId),
                        CommandType.Text, new MySqlParameter("?Monitor", monitor), new MySqlParameter("?MonitorSource", monitorSource), new MySqlParameter("?ConnectionString", connectionString.Encrypt(_passwordGUID, _salt)),
                            new MySqlParameter("?MachineConfiguration", machineConfiguration), new MySqlParameter("?ResultHeaders", resultHeaders.Combine("; ", string.Empty))
                    );
                return _databaseActions.GetLastInsertId();
            }
            return 0;
        }

        #endregion

        //SET

        /// <summary>
        /// Connect to an existing database to execute the procedures on or add slave side data to it.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="databaseName"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public void ConnectToExistingDatabase(string host, int port, string databaseName, string user, string password) {
            try {
                _databaseActions = new DatabaseActions() { ConnectionString = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};Pooling=True;", host, port, databaseName, user, password) };
                if (_databaseActions.GetDataTable("Show databases").Rows.Count == 0) throw new Exception("A connection to MySQL could not be made!");
            } catch {
                try { _databaseActions.ReleaseConnection(); } catch { }
                _databaseActions = null;
                throw;
            }
        }

        #region Stresstest results

        /// <summary>
        ///     Started at datetime now.
        /// </summary>
        /// <param name="stresstestResult"></param>
        public void SetStresstestStarted(StresstestResult stresstestResult) {
            if (_databaseActions != null) {
                _databaseActions.ExecuteSQL(
                    string.Format(
                        "INSERT INTO StresstestResults(StresstestId, StartedAt, StoppedAt, Status, StatusMessage) VALUES('{0}', '{1}', '{2}', 'OK', '')",
                        _stresstestId, Parse(stresstestResult.StartedAt), Parse(DateTime.MinValue))
                    );
                _stresstestResultId = _databaseActions.GetLastInsertId();
            }
        }

        /// <summary>
        ///     Stopped at datetime now.
        /// </summary>
        /// <param name="stresstestResult"></param>
        /// <param name="status"></param>
        /// <param name="statusMessage"></param>
        public void SetStresstestStopped(StresstestResult stresstestResult, string status = "OK",
                                                string statusMessage = "") {
            stresstestResult.StoppedAt = DateTime.Now;
            if (_databaseActions != null)
                _databaseActions.ExecuteSQL(
                    string.Format(
                        "UPDATE StresstestResults SET StoppedAt='{1}', Status='{2}', StatusMessage='{3}' WHERE Id='{0}'",
                        _stresstestResultId, Parse(stresstestResult.StoppedAt), status, statusMessage)
                    );
        }

        #endregion

        #region Concurrency results

        /// <summary>
        ///     Started at datetime now.
        /// </summary>
        /// <param name="stresstestResultId"></param>
        /// <param name="concurrencyResult"></param>
        public void SetConcurrencyStarted(ConcurrencyResult concurrencyResult) {
            if (_databaseActions != null) {
                _databaseActions.ExecuteSQL(
                    string.Format(
                        "INSERT INTO ConcurrencyResults(StresstestResultId, Concurrency, StartedAt, StoppedAt) VALUES('{0}', '{1}', '{2}', '{3}')",
                        _stresstestResultId, concurrencyResult.Concurrency, Parse(concurrencyResult.StartedAt),
                        Parse(DateTime.MinValue))
                    );
                _concurrencyResultId = _databaseActions.GetLastInsertId();
            }
        }

        /// <summary>
        ///     Stopped at datetime now.
        /// </summary>
        /// <param name="concurrencyResult"></param>
        public void SetConcurrencyStopped(ConcurrencyResult concurrencyResult) {
            concurrencyResult.StoppedAt = DateTime.Now;
            if (_databaseActions != null)
                _databaseActions.ExecuteSQL(
                    string.Format("UPDATE ConcurrencyResults SET StoppedAt='{1}' WHERE Id='{0}'", _concurrencyResultId,
                                  Parse(concurrencyResult.StoppedAt))
                    );
        }

        #endregion

        #region Run results

        /// <summary>
        /// </summary>
        /// <param name="concurrencyResultId"></param>
        /// <param name="reRunCount"></param>
        /// <param name="startedAt"></param>
        /// <param name="stoppedAt"></param>
        /// <returns>Id of the run result.</returns>
        public void SetRunStarted(RunResult runResult) {
            if (_databaseActions != null) {
                _databaseActions.ExecuteSQL(
                    string.Format(
                        "INSERT INTO RunResults(ConcurrencyResultId, Run, TotalLogEntryCount, ReRunCount, StartedAt, StoppedAt) VALUES('{0}', '{1}', '0', '0', '{2}', '{3}')",
                        _concurrencyResultId, runResult.Run, Parse(runResult.StartedAt), Parse(DateTime.MinValue))
                    );
                _runResultId = _databaseActions.GetLastInsertId();
            }
        }

        /// <summary>
        ///     Increase the rerun count value for the result using fx PrepareForRerun() before calling this fx.
        /// </summary>
        /// <param name="runResult"></param>
        public void SetRerun(RunResult runResult) {
            if (_databaseActions != null)
                _databaseActions.ExecuteSQL(
                    string.Format("UPDATE RunResults SET ReRunCount='{1}' WHERE Id='{0}'", _runResultId,
                                  runResult.RerunCount)
                    );
        }

        /// <summary>
        ///     All the log entry results are save to the database doing this, only do this for the curent run.
        /// </summary>
        /// <param name="runResult"></param>
        public void SetRunStopped(RunResult runResult) {
            runResult.StoppedAt = DateTime.Now;
            if (_databaseActions != null) {
                ulong totalLogEntryCount = 0;
                foreach (VirtualUserResult virtualUserResult in runResult.VirtualUserResults) {
                    totalLogEntryCount += (ulong)virtualUserResult.LogEntryResults.LongLength;
                    foreach (LogEntryResult logEntryResult in virtualUserResult.LogEntryResults)
                        //mssn de multiple insert approach gebruiken bvb insert into tbl(a) values(1),(2),(3) voegt 3 rijen toe.
                        //deze manier is volgens mysql de snelste.
                        if (logEntryResult != null)
                            _databaseActions.ExecuteSQL(
                                string.Format(@"INSERT INTO LogEntryResults(RunResultId, VirtualUser, UserAction, LogEntryIndex, LogEntry, SentAt, TimeToLastByteInTicks, DelayInMilliseconds, Error)
VALUES('{0}', '{1}', ?userAction, '{2}', ?logEntry, '{3}', '{4}', '{5}', '{6}')",
                                              _runResultId, virtualUserResult.VirtualUser, logEntryResult.LogEntryIndex,
                                              Parse(logEntryResult.SentAt), logEntryResult.TimeToLastByteInTicks, logEntryResult.DelayInMilliseconds, logEntryResult.Error)
                                , CommandType.Text, new MySqlParameter("?userAction", logEntryResult.UserAction), new MySqlParameter("?logEntry", logEntryResult.LogEntry));
                }

                _databaseActions.ExecuteSQL(
                    string.Format("UPDATE RunResults SET TotalLogEntryCount='{1}', StoppedAt='{2}' WHERE Id='{0}'", _runResultId, totalLogEntryCount, Parse(runResult.StoppedAt))
                    );
            }
        }

        #endregion

        #region Monitor results

        /// <summary>
        ///     Do this at the end of the test.
        /// </summary>
        /// <param name="monitorResultCache">Should have a filled in monitor configuration id.</param>
        public void SetMonitorResults(MonitorResultCache monitorResultCache) {
            if (_databaseActions != null)
                foreach (var row in monitorResultCache.Rows) {
                    var timeStamp = (DateTime)row[0];

                    var value = new List<float>();
                    for (int i = 1; i < row.Length; i++) value.Add((float)row[i]);

                    _databaseActions.ExecuteSQL(
                        string.Format(
                            "INSERT INTO MonitorResults(MonitorId, TimeStamp, Value) VALUES('{0}', '{1}', '{2}')",
                            monitorResultCache.MonitorConfigurationId, Parse(timeStamp), value.ToArray().Combine("; "))
                        );
                }
        }

        #endregion

        //GET

        #region Configuration
        public string GetDescription() {
            if (_databaseActions != null) {
                var dt = _databaseActions.GetDataTable("Select * FROM Description");
                foreach (DataRow row in dt.Rows) return row.ItemArray[0] as string;
            }
            return string.Empty;
        }
        public List<string> GetTags() {
            var l = new List<string>();
            if (_databaseActions != null) {
                var dt = _databaseActions.GetDataTable("Select * FROM Tags");
                foreach (DataRow row in dt.Rows) l.Add(row.ItemArray[0] as string);
            }
            return l;
        }
        public List<int> GetvApusInstanceIds() {
            var l = new List<int>();
            if (_databaseActions != null) {
                var dt = _databaseActions.GetDataTable("Select Id FROM vApusInstances");
                foreach (DataRow row in dt.Rows) l.Add((int)row.ItemArray[0]);
            }
            return l;
        }
        public List<KeyValuePair<string, string>> GetvApusInstance(int Id) {
            var l = new List<KeyValuePair<string, string>>();
            if (_databaseActions != null) {
                var dt = _databaseActions.GetDataTable(string.Format(
                    "Select HostName, IP, Port, Version, Channel, IsMaster FROM vApusInstances WHERE ID = '{0}'", Id));
                object[] row = dt.Rows[0].ItemArray;
                l.Add(new KeyValuePair<string, string>(row[0] as string, string.Empty));
                l.Add(new KeyValuePair<string, string>(row[1] + ":" + row[2], string.Empty));
                l.Add(new KeyValuePair<string, string>("Version", row[3] as string));
                l.Add(new KeyValuePair<string, string>("Channel", row[4] as string));
                l.Add(new KeyValuePair<string, string>("Is Master", ((bool)row[5]) ? "Yes" : "No"));
            }
            return l;
        }
        public List<KeyValuePair<string, string>> GetStresstest(int vApusInstanceId) {
            var l = new List<KeyValuePair<string, string>>();
            if (_databaseActions != null) {
                var dt = _databaseActions.GetDataTable(string.Format(
                    @"Select Stresstest, RunSynchronization, Connection, ConnectionProxy, Log, LogRuleSet, Concurrencies,
Runs, MinimumDelayInMilliseconds, MaximumDelayInMilliseconds, Shuffle, Distribute, MonitorBeforeInMinutes, MonitorAfterInMinutes FROM Stresstests WHERE vApusInstanceId = '{0}'", vApusInstanceId));
                object[] row = dt.Rows[0].ItemArray;
                l.Add(new KeyValuePair<string, string>(row[0] as string, string.Empty));
                l.Add(new KeyValuePair<string, string>("RunSynchronization", row[1] as string));
                l.Add(new KeyValuePair<string, string>(row[2] as string, string.Empty));
                l.Add(new KeyValuePair<string, string>(row[3] as string, string.Empty));
                l.Add(new KeyValuePair<string, string>(row[4] as string, string.Empty));
                l.Add(new KeyValuePair<string, string>(row[5] as string, string.Empty));
                l.Add(new KeyValuePair<string, string>("Concurrencies", row[6] as string));
                l.Add(new KeyValuePair<string, string>("Runs", row[7].ToString()));
                int minDelay = (int)row[8];
                int maxDelay = (int)row[9];
                l.Add(new KeyValuePair<string, string>("Delay", minDelay == maxDelay ? minDelay + " ms" : minDelay + " - " + maxDelay + " ms"));
                l.Add(new KeyValuePair<string, string>("Shuffle", ((bool)row[10]) ? "Yes" : "No"));
                l.Add(new KeyValuePair<string, string>("Distribute", row[11] as string));
                l.Add(new KeyValuePair<string, string>("Monitor Before", row[12] + " minutes"));
                l.Add(new KeyValuePair<string, string>("Monitor After", row[13] + " minutes"));

            }
            return l;
        }
        public List<string> GetMonitors() {
            var l = new List<string>();
            if (_databaseActions != null) {
                var dt = _databaseActions.GetDataTable("Select Monitor, MonitorSource FROM Monitors");
                foreach (DataRow dr in dt.Rows) l.Add(dr.ItemArray[0] + " (" + dr.ItemArray[1] + ")");
            }
            return l;
        }
        #endregion

        private readonly object _lock = new object();
        #region Procedures
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stresstestIds">If none, all the results for all tests will be returned.</param>
        /// <returns></returns>
        public DataTable GetAverageConcurrentUsers(params ulong[] stresstestIds) {
            if (_databaseActions != null) {
                var stresstests = (stresstestIds.Length == 0) ? _databaseActions.GetDataTable("Select Id, Stresstest, Connection, Runs From Stresstests;") :
                _databaseActions.GetDataTable("Select Id, Stresstest, Connection, Runs From Stresstests WHERE Id IN(" + stresstestIds.Combine(", ") + ");");
                if (stresstests.Rows.Count == 0) return null;

                var averageConcurrentUsers = CreateEmptyDataTable("AverageConcurrentUsers", "Stresstest", "Started At", "Measured Time (ms)", "Concurrency",
    "Log Entries Processed", "Log Entries", "Throughput (responses / s)", "User Actions / s", "Avg. Response Time (ms)",
    "Max. Response Time (ms)", "95th Percentile of the Response Times (ms)", "Avg. Delay (ms)", "Errors");


                foreach (DataRow stresstestsRow in stresstests.Rows) {
                    object stresstestId = stresstestsRow.ItemArray[0];

                    var stresstestResults = _databaseActions.GetDataTable("Select Id From StresstestResults WHERE StresstestId=" + stresstestId);
                    if (stresstestResults.Rows.Count == 0) continue;
                    object stresstestResultId = stresstestResults.Rows[0].ItemArray[0];

                    string stresstest = (string)stresstestsRow.ItemArray[1] + " " + stresstestsRow.ItemArray[2];
                    int runs = (int)stresstestsRow.ItemArray[3];
                    var concurrencyResults = _databaseActions.GetDataTable("SELECT Id, StartedAt, StoppedAt, Concurrency FROM ConcurrencyResults WHERE StresstestResultId=" + stresstestResultId);
                    foreach (DataRow crRow in concurrencyResults.Rows) {
                        ConcurrencyResult concurrencyResult = new ConcurrencyResult((int)crRow.ItemArray[3], runs);
                        concurrencyResult.StartedAt = (DateTime)crRow.ItemArray[1];
                        concurrencyResult.StoppedAt = (DateTime)crRow.ItemArray[2];

                        var rr = _databaseActions.GetDataTable(string.Format("SELECT Id, Run, TotalLogEntryCount FROM RunResults WHERE ConcurrencyResultId={0}", crRow.ItemArray[0]));
                        var runResultIds = new List<int>(rr.Rows.Count);
                        foreach (DataRow rrRow in rr.Rows) {
                            runResultIds.Add((int)rrRow.ItemArray[0]);
                            concurrencyResult.RunResults.Add(new RunResult((int)rrRow.ItemArray[1], concurrencyResult.Concurrency));
                        }

                        var ler = _databaseActions.GetDataTable("Select RunResultId, VirtualUser, UserAction, LogEntryIndex, TimeToLastByteInTicks, DelayInMilliseconds, Error FROM LogEntryResults WHERE RunResultId IN(" + runResultIds.ToArray().Combine(", ") + ");");

                        for (int i = 0; i != concurrencyResult.RunResults.Count; i++) {
                            var runResult = concurrencyResult.RunResults[i];
                            int runResultId = runResultIds[i];
                            var virtualUserResults = new ConcurrentDictionary<string, VirtualUserResult>();
                            var logEntryResults = new ConcurrentDictionary<string, List<LogEntryResult>>(); //Key == virtual user.

                            //Parallel.ForEach(ler.AsEnumerable(), (lerRow) => {
                            foreach (DataRow lerRow in ler.Rows) {
                                if ((int)lerRow.ItemArray[0] == runResultId) {
                                    string virtualUser = (lerRow.ItemArray[1] as string);
                                    virtualUserResults.TryAdd(virtualUser, new VirtualUserResult(0) { VirtualUser = virtualUser });
                                    logEntryResults.TryAdd(virtualUser, new List<LogEntryResult>());

                                    var virtualUserResult = virtualUserResults[virtualUser];
                                    logEntryResults[virtualUser].Add(new LogEntryResult() {
                                        VirtualUser = virtualUser, UserAction = lerRow.ItemArray[2] as string, LogEntryIndex = lerRow.ItemArray[3] as string,
                                        TimeToLastByteInTicks = (long)lerRow.ItemArray[4], DelayInMilliseconds = (int)lerRow.ItemArray[5], Error = lerRow.ItemArray[6] as string
                                    });
                                }
                            }
                            //});

                            Parallel.ForEach(logEntryResults, (item, loopState) => {
                                virtualUserResults[item.Key].LogEntryResults = item.Value.ToArray();
                            });

                            runResult.VirtualUserResults = virtualUserResults.Values.ToArray();
                        }

                        var metrics = StresstestMetricsHelper.GetMetrics(concurrencyResult, true);

                        averageConcurrentUsers.Rows.Add(stresstest, metrics.StartMeasuringRuntime, Math.Round(metrics.MeasuredRunTime.TotalMilliseconds, 2),
                            metrics.ConcurrentUsers, metrics.LogEntriesProcessed, metrics.LogEntries, Math.Round(metrics.ResponsesPerSecond, 2), Math.Round(metrics.UserActionsPerSecond, 2),
                            Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2), Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2), Math.Round(metrics.Percentile95thResponseTimes.TotalMilliseconds, 2),
                            Math.Round(metrics.AverageDelay.TotalMilliseconds, 2), metrics.Errors);
                    }
                }
                return averageConcurrentUsers;
            }
            return null;
        }
        public DataTable GetAverageUserActions(params ulong[] stresstestIds) {
            if (_databaseActions != null) {
                var stresstests = (stresstestIds.Length == 0) ? _databaseActions.GetDataTable("Select Id, Stresstest, Connection From Stresstests;") :
                _databaseActions.GetDataTable("Select Id, Stresstest, Connection, Runs From Stresstests WHERE Id IN(" + stresstestIds.Combine(", ") + ");");
                if (stresstests.Rows.Count == 0) return null;

                var averageUserActions = CreateEmptyDataTable("AverageUserActions", "Stresstest", "Concurrency", "User Action", "Avg. Response Time (ms)",
    "Max. Response Time (ms)", "95th Percentile of the Response Times (ms)", "Avg. Delay (ms)", "Errors");

                foreach (DataRow stresstestsRow in stresstests.Rows) {
                    object stresstestId = stresstestsRow.ItemArray[0];

                    var stresstestResults = _databaseActions.GetDataTable("Select Id From StresstestResults WHERE StresstestId=" + stresstestId);
                    if (stresstestResults.Rows.Count == 0) return null;
                    object stresstestResultId = stresstestResults.Rows[0].ItemArray[0];

                    string stresstest = (string)stresstestsRow.ItemArray[1] + " " + stresstestsRow.ItemArray[2];
                    var concurrencyResults = _databaseActions.GetDataTable("SELECT Id, Concurrency FROM ConcurrencyResults WHERE StresstestResultId=" + stresstestResultId);
                    foreach (DataRow crRow in concurrencyResults.Rows) {
                        object concurrencyResultId = crRow.ItemArray[0];
                        int concurrency = (int)crRow.ItemArray[1];

                        var runResults = _databaseActions.GetDataTable("SELECT Id FROM RunResults WHERE ConcurrencyResultId=" + concurrencyResultId);
                        int runs = runResults.Rows.Count;

                        var userActions = new List<KeyValuePair<string, Dictionary<string, List<LogEntryResult>>>>(); // <VirtualUser,<UserAction, LogEntryResult
                        foreach (DataRow rrRow in runResults.Rows) {
                            object runResultId = rrRow.ItemArray[0];
                            var logEntryResults = _databaseActions.GetDataTable("SELECT VirtualUser, UserAction, TimeToLastByteInTicks, DelayInMilliseconds, Error FROM LogEntryResults WHERE RunResultId=" + runResultId);

                            var uas = new Dictionary<string, Dictionary<string, List<LogEntryResult>>>(); // <VirtualUser,<UserAction, LogEntryResult
                            foreach (DataRow lerRow in logEntryResults.Rows) {
                                string virtualUser = lerRow.ItemArray[0] as string;
                                string userAction = lerRow.ItemArray[1] as string;
                                var logEntryResult = new LogEntryResult() { TimeToLastByteInTicks = (long)lerRow.ItemArray[2], DelayInMilliseconds = (int)lerRow.ItemArray[3], Error = lerRow.ItemArray[4] as string };

                                if (!uas.ContainsKey(virtualUser)) uas.Add(virtualUser, new Dictionary<string, List<LogEntryResult>>());
                                if (!uas[virtualUser].ContainsKey(userAction)) uas[virtualUser].Add(userAction, new List<LogEntryResult>());

                                uas[virtualUser][userAction].Add(logEntryResult);
                            }
                            foreach (string virtualUser in uas.Keys) {
                                var kvp = new KeyValuePair<string, Dictionary<string, List<LogEntryResult>>>(virtualUser, new Dictionary<string, List<LogEntryResult>>());
                                foreach (string userAction in uas[virtualUser].Keys) kvp.Value.Add(userAction, uas[virtualUser][userAction]);
                                userActions.Add(kvp);
                            }
                        }

                        var userActionResults = CreateEmptyDataTable("UserActionResults", "UserAction", "TimeToLastByteInTicks", "DelayInMilliseconds", "Errors");
                        for (int i = 0; i != userActions.Count; i++)
                            foreach (var kvp in userActions[i].Value) {
                                long ttlb = 0;
                                int delay = -1;
                                long ers = 0;

                                string userAction = kvp.Key;
                                var logEntryResults = kvp.Value;

                                for (int j = logEntryResults.Count - 1; j != -1; j--) {
                                    var ler = logEntryResults[j];
                                    if (delay == -1) {
                                        delay = ler.DelayInMilliseconds;
                                        ttlb = ler.TimeToLastByteInTicks;
                                    } else ttlb += ler.TimeToLastByteInTicks + ler.DelayInMilliseconds;
                                    if (!string.IsNullOrEmpty(ler.Error)) ++ers;
                                }
                                userActionResults.Rows.Add(userAction, ttlb, delay, ers);
                            }

                        var uniqueUserActionCounts = new Dictionary<string, int>(); //To make a correct average.
                        foreach (DataRow uarRow in userActionResults.Rows) {
                            string userAction = uarRow[0] as string;

                            if (uniqueUserActionCounts.ContainsKey(userAction)) ++uniqueUserActionCounts[userAction];
                            else uniqueUserActionCounts.Add(userAction, 1);
                        }

                        //The key of the entries for following collections are user actions.
                        var avgTimeToLastByteInTicks = new Dictionary<string, double>();
                        var maxTimeToLastByteInTicks = new Dictionary<string, long>();
                        var timeToLastBytesInTicks = new Dictionary<string, List<long>>();
                        var percTimeToLastBytesInTicks = new Dictionary<string, long>();

                        var avgDelay = new Dictionary<string, double>();
                        var errors = new Dictionary<string, long>();

                        foreach (DataRow uarRow in userActionResults.Rows) {
                            object[] row = uarRow.ItemArray;
                            string userAction = row[0] as string;
                            long ttlb = (long)row[1];
                            int delay = (int)row[2];
                            long ers = (long)row[3];

                            if (avgTimeToLastByteInTicks.ContainsKey(userAction)) avgTimeToLastByteInTicks[userAction] += (((double)ttlb) / uniqueUserActionCounts[userAction]);
                            else avgTimeToLastByteInTicks.Add(userAction, (((double)ttlb) / uniqueUserActionCounts[userAction]));

                            if (maxTimeToLastByteInTicks.ContainsKey(userAction)) { if (maxTimeToLastByteInTicks[userAction] < ttlb) maxTimeToLastByteInTicks[userAction] = ttlb; } else maxTimeToLastByteInTicks.Add(userAction, ttlb);

                            if (!timeToLastBytesInTicks.ContainsKey(userAction)) timeToLastBytesInTicks.Add(userAction, new List<long>(uniqueUserActionCounts[userAction]));
                            timeToLastBytesInTicks[userAction].Add(ttlb);

                            if (avgDelay.ContainsKey(userAction)) avgDelay[userAction] += (((double)delay) / uniqueUserActionCounts[userAction]);
                            else avgDelay.Add(userAction, ((double)delay) / uniqueUserActionCounts[userAction]);

                            if (errors.ContainsKey(userAction)) errors[userAction] += ers;
                            else errors.Add(userAction, ers);
                        }

                        //95th percentile
                        foreach (string userAction in timeToLastBytesInTicks.Keys) {
                            var l = timeToLastBytesInTicks[userAction];

                            int percent5 = (int)(l.Count * 0.05);
                            if (percent5 == 0)
                                percTimeToLastBytesInTicks.Add(userAction, maxTimeToLastByteInTicks[userAction]);
                            else {
                                l.Sort();
                                percTimeToLastBytesInTicks.Add(userAction, l[l.Count - percent5 - 1]);
                            }
                        }

                        List<string> sortedUserActions = avgTimeToLastByteInTicks.Keys.ToList();
                        sortedUserActions.Sort(UserActionComparer.GetInstance);

                        foreach (string s in sortedUserActions) {
                            averageUserActions.Rows.Add(stresstest, concurrency, s,
                                Math.Round(avgTimeToLastByteInTicks[s] / TimeSpan.TicksPerMillisecond, 2),
                                Math.Round(((double)maxTimeToLastByteInTicks[s]) / TimeSpan.TicksPerMillisecond, 2),
                                Math.Round(((double)percTimeToLastBytesInTicks[s]) / TimeSpan.TicksPerMillisecond, 2),
                                Math.Round(avgDelay[s], 2),
                                errors[s]);
                        }
                    }
                }
                return averageUserActions;
            }
            return null;
        }

        public DataTable GetAverageLogEntries(params ulong[] stresstestIds) {
            if (_databaseActions != null) {
                var stresstests = (stresstestIds.Length == 0) ? _databaseActions.GetDataTable("Select Id, Stresstest, Connection From Stresstests;") :
                _databaseActions.GetDataTable("Select Id, Stresstest, Connection, Runs From Stresstests WHERE Id IN(" + stresstestIds.Combine(", ") + ");");
                if (stresstests.Rows.Count == 0) return null;

                var averageLogEntries = CreateEmptyDataTable("AverageLogEntries", "Stresstest", "Concurrency", "User Action", "Log Entry", "Avg. Response Time (ms)",
"Max. Response Time (ms)", "95th Percentile of the Response Times (ms)", "Avg. Delay (ms)", "Errors");

                foreach (DataRow stresstestsRow in stresstests.Rows) {
                    object stresstestId = stresstestsRow.ItemArray[0];

                    var stresstestResults = _databaseActions.GetDataTable("Select Id From StresstestResults WHERE StresstestId=" + stresstestId);
                    if (stresstestResults.Rows.Count == 0) continue;
                    object stresstestResultId = stresstestResults.Rows[0].ItemArray[0];

                    string stresstest = (string)stresstestsRow.ItemArray[1] + " " + stresstestsRow.ItemArray[2];
                    var concurrencyResults = _databaseActions.GetDataTable("SELECT Id, Concurrency FROM ConcurrencyResults WHERE StresstestResultId=" + stresstestResultId);
                    foreach (DataRow crRow in concurrencyResults.Rows) {
                        object concurrencyResultId = crRow.ItemArray[0];
                        int concurrency = (int)crRow.ItemArray[1];

                        var runResults = _databaseActions.GetDataTable("SELECT Id FROM RunResults WHERE ConcurrencyResultId=" + concurrencyResultId);

                        var logEntryResults = new DataTable();
                        foreach (DataRow rrRow in runResults.Rows) {
                            object runResultId = rrRow.ItemArray[0];
                            logEntryResults.Merge(_databaseActions.GetDataTable("SELECT LogEntryIndex, UserAction, LogEntry, TimeToLastByteInTicks, DelayInMilliseconds, Error FROM LogEntryResults WHERE RunResultId=" + runResultId));
                        }

                        var uniqueLogEntyCounts = new Dictionary<string, int>(); //To make a correct average.
                        foreach (DataRow lerRow in logEntryResults.Rows) {
                            string logEntryIndex = lerRow[0] as string;

                            if (uniqueLogEntyCounts.ContainsKey(logEntryIndex)) ++uniqueLogEntyCounts[logEntryIndex];
                            else uniqueLogEntyCounts.Add(logEntryIndex, 1);
                        }

                        //The key of the entries for following collections are log entry indices.
                        var userActions = new Dictionary<string, string>();
                        var logEntries = new Dictionary<string, string>(); //Val = log entry

                        var avgTimeToLastByteInTicks = new Dictionary<string, double>();
                        var maxTimeToLastByteInTicks = new Dictionary<string, long>();
                        var timeToLastBytesInTicks = new Dictionary<string, List<long>>();
                        var percTimeToLastBytesInTicks = new Dictionary<string, long>();

                        var avgDelay = new Dictionary<string, double>();
                        var errors = new Dictionary<string, long>();

                        foreach (DataRow lerRow in logEntryResults.Rows) {
                            object[] row = lerRow.ItemArray;
                            string logEntryIndex = row[0] as string;
                            string userAction = row[1] as string;
                            string logEntry = row[2] as string;
                            long ttlb = (long)row[3];
                            int delay = (int)row[4];
                            string error = row[5] as string;

                            if (!userActions.ContainsKey(logEntryIndex)) userActions.Add(logEntryIndex, userAction);
                            if (!logEntries.ContainsKey(logEntryIndex)) logEntries.Add(logEntryIndex, logEntry);

                            if (avgTimeToLastByteInTicks.ContainsKey(logEntryIndex)) avgTimeToLastByteInTicks[logEntryIndex] += (((double)ttlb) / uniqueLogEntyCounts[logEntryIndex]);
                            else avgTimeToLastByteInTicks.Add(logEntryIndex, (((double)ttlb) / uniqueLogEntyCounts[logEntryIndex]));

                            if (maxTimeToLastByteInTicks.ContainsKey(logEntryIndex)) { if (maxTimeToLastByteInTicks[logEntryIndex] < ttlb) maxTimeToLastByteInTicks[logEntryIndex] = ttlb; } else maxTimeToLastByteInTicks.Add(logEntryIndex, ttlb);

                            if (!timeToLastBytesInTicks.ContainsKey(logEntryIndex)) timeToLastBytesInTicks.Add(logEntryIndex, new List<long>(uniqueLogEntyCounts[logEntryIndex]));
                            timeToLastBytesInTicks[logEntryIndex].Add(ttlb);

                            if (avgDelay.ContainsKey(logEntryIndex)) avgDelay[logEntryIndex] += (((double)delay) / uniqueLogEntyCounts[logEntryIndex]);
                            else avgDelay.Add(logEntryIndex, ((double)delay) / uniqueLogEntyCounts[logEntryIndex]);


                            if (!errors.ContainsKey(logEntryIndex)) errors.Add(logEntryIndex, 0);
                            if (!string.IsNullOrEmpty(error)) ++errors[logEntryIndex];
                        }

                        //95th percentile
                        foreach (string logEntryIndex in timeToLastBytesInTicks.Keys) {
                            var l = timeToLastBytesInTicks[logEntryIndex];

                            int percent5 = (int)(l.Count * 0.05);
                            if (percent5 == 0)
                                percTimeToLastBytesInTicks.Add(logEntryIndex, maxTimeToLastByteInTicks[logEntryIndex]);
                            else {
                                l.Sort();
                                percTimeToLastBytesInTicks.Add(logEntryIndex, l[l.Count - percent5 - 1]);
                            }
                        }

                        List<string> sortedLogEntryIndices = logEntries.Keys.ToList();
                        sortedLogEntryIndices.Sort(LogEntryIndexComparer.GetInstance);

                        foreach (string s in sortedLogEntryIndices) {
                            averageLogEntries.Rows.Add(stresstest, concurrency, userActions[s], logEntries[s],
                                Math.Round(avgTimeToLastByteInTicks[s] / TimeSpan.TicksPerMillisecond, 2),
                                Math.Round(((double)maxTimeToLastByteInTicks[s]) / TimeSpan.TicksPerMillisecond, 2),
                                Math.Round(((double)percTimeToLastBytesInTicks[s]) / TimeSpan.TicksPerMillisecond, 2),
                                Math.Round(avgDelay[s], 2),
                                errors[s]);
                        }
                    }
                }
                return averageLogEntries;
            }
            return null;
        }
        public DataTable GetErrors(params ulong[] stresstestIds) {
            if (_databaseActions != null) {
                var stresstests = (stresstestIds.Length == 0) ? _databaseActions.GetDataTable("Select Id, Stresstest, Connection From Stresstests;") :
                _databaseActions.GetDataTable("Select Id, Stresstest, Connection, Runs From Stresstests WHERE Id IN(" + stresstestIds.Combine(", ") + ");");
                if (stresstests.Rows.Count == 0) return null;

                var errors = CreateEmptyDataTable("Error", "Stresstest", "Concurrency", "Run", "Virtual User", "User Action", "Log Entry", "Error");

                foreach (DataRow stresstestsRow in stresstests.Rows) {
                    object stresstestId = stresstestsRow.ItemArray[0];

                    var stresstestResults = _databaseActions.GetDataTable("Select Id From StresstestResults WHERE StresstestId=" + stresstestId);
                    if (stresstestResults.Rows.Count == 0) return errors;
                    object stresstestResultId = stresstestResults.Rows[0].ItemArray[0];

                    string stresstest = (string)stresstestsRow.ItemArray[1] + " " + stresstestsRow.ItemArray[2];
                    var concurrencyResults = _databaseActions.GetDataTable("SELECT Id, Concurrency FROM ConcurrencyResults WHERE StresstestResultId=" + stresstestResultId);
                    foreach (DataRow crRow in concurrencyResults.Rows) {
                        object concurrencyResultId = crRow.ItemArray[0];
                        object concurrency = crRow.ItemArray[1];

                        var runResults = _databaseActions.GetDataTable("SELECT Id, Run FROM RunResults WHERE ConcurrencyResultId=" + concurrencyResultId);

                        foreach (DataRow rrRow in runResults.Rows) {
                            object runResultId = rrRow.ItemArray[0];
                            object run = rrRow.ItemArray[1];

                            var ler = _databaseActions.GetDataTable("SELECT RunResultId, VirtualUser, UserAction, LogEntry, Error FROM logEntryResults WHERE RunResultId = " + runResultId + " AND  Error != ''");

                            foreach (DataRow ldr in ler.Rows)
                                errors.Rows.Add(stresstest, concurrency, run, ldr.ItemArray[1], ldr.ItemArray[2], ldr.ItemArray[3], ldr.ItemArray[4]);
                        }
                    }
                }
                return errors;
            }
            return null;
        }

        public DataTable GetMachineConfigurations(params ulong[] stresstestIds) {
            if (_databaseActions != null) {
                var stresstests = (stresstestIds.Length == 0) ? _databaseActions.GetDataTable("Select Id, Stresstest, Connection From Stresstests;") :
                _databaseActions.GetDataTable("Select Id, Stresstest, Connection From Stresstests WHERE Id IN(" + stresstestIds.Combine(", ") + ");");
                if (stresstests.Rows.Count == 0) return null;

                var machineConfigurations = CreateEmptyDataTable("MachineConfigurations", "Stresstest", "Monitor", "Monitor Source", "Machine Configuration");
                foreach (DataRow stresstestsRow in stresstests.Rows) {
                    object stresstestId = stresstestsRow.ItemArray[0];
                    string stresstest = (string)stresstestsRow.ItemArray[1] + " " + stresstestsRow.ItemArray[2];

                    var monitors = _databaseActions.GetDataTable("Select Monitor, MonitorSource, MachineConfiguration from monitors WHERE StresstestId = " + stresstestId);
                    foreach (DataRow monitorRow in monitors.Rows) machineConfigurations.Rows.Add(stresstest, monitorRow.ItemArray);
                }

                return machineConfigurations;
            }
            return null;
        }
        public DataTable GetAverageMonitorResults(params ulong[] stresstestIds) {
            if (_databaseActions != null) {
                var stresstests = (stresstestIds.Length == 0) ? _databaseActions.GetDataTable("Select Id, Stresstest, Connection From Stresstests;") :
                _databaseActions.GetDataTable("Select Id, Stresstest, Connection, Runs From Stresstests WHERE Id IN(" + stresstestIds.Combine(", ") + ");");
                if (stresstests.Rows.Count == 0) return null;

                var averageMonitorResults = CreateEmptyDataTable("AverageMonitorResults", "Stresstest", "Monitor", "Started At", "Measured Time (ms)", "Concurrency", "Headers", "Values");
                foreach (DataRow stresstestsRow in stresstests.Rows) {
                    object stresstestId = stresstestsRow.ItemArray[0];
                    string stresstest = (string)stresstestsRow.ItemArray[1] + " " + stresstestsRow.ItemArray[2];

                    var stresstestResults = _databaseActions.GetDataTable("Select Id From StresstestResults WHERE StresstestId=" + stresstestId);
                    if (stresstestResults.Rows.Count == 0) continue;
                    object stresstestResultId = stresstestResults.Rows[0].ItemArray[0];

                    //Get the monitors + values
                    var monitors = _databaseActions.GetDataTable("Select Id, Monitor, ResultHeaders From Monitors WHERE StresstestId=" + stresstestId);
                    if (monitors.Rows.Count == 0) continue;

                    //Get the timestamps to calculate the averages
                    var concurrencyResults = _databaseActions.GetDataTable("SELECT Id, Concurrency, StartedAt, StoppedAt FROM ConcurrencyResults WHERE StresstestResultId=" + stresstestResultId);
                    var delimiters = new Dictionary<int, KeyValuePair<DateTime, DateTime>>(concurrencyResults.Rows.Count);
                    var runDelimiters = new Dictionary<int, Dictionary<DateTime, DateTime>>(concurrencyResults.Rows.Count);
                    foreach (DataRow crRow in concurrencyResults.Rows) {
                        int concurrency = (int)crRow.ItemArray[1];
                        delimiters.Add(concurrency, new KeyValuePair<DateTime, DateTime>((DateTime)crRow.ItemArray[2], (DateTime)crRow.ItemArray[3]));
                        var runResults = _databaseActions.GetDataTable(string.Format("SELECT StartedAt, StoppedAt FROM RunResults WHERE ConcurrencyResultId={0}", crRow.ItemArray[0]));
                        var d = new Dictionary<DateTime, DateTime>(runResults.Rows.Count);
                        foreach (DataRow rrRow in runResults.Rows) {
                            var start = (DateTime)rrRow.ItemArray[0];
                            if (!d.ContainsKey(start)) d.Add(start, (DateTime)rrRow.ItemArray[1]);
                        }
                        runDelimiters.Add(concurrency, d);
                    }

                    //Calcullate the averages
                    foreach (int concurrency in runDelimiters.Keys) {
                        var delimiterValues = runDelimiters[concurrency];
                        foreach (DataRow monitorRow in monitors.Rows) {
                            object monitorId = monitorRow.ItemArray[0];
                            object monitor = monitorRow.ItemArray[1];
                            object headers = monitorRow.ItemArray[2];

                            var monitorResults = _databaseActions.GetDataTable("Select TimeStamp, Value From MonitorResults WHERE MonitorId=" + monitorId);
                            var monitorValues = new Dictionary<DateTime, float[]>(monitorResults.Rows.Count);
                            foreach (DataRow monitorResultsRow in monitorResults.Rows) {
                                var timeStamp = (DateTime)monitorResultsRow[0];

                                bool canAdd = false;
                                foreach (var start in delimiterValues.Keys)
                                    if (timeStamp >= start && timeStamp <= delimiterValues[start]) {
                                        canAdd = true;
                                        break;
                                    }

                                if (canAdd) {
                                    string[] splittedValue = (monitorResultsRow[1] as string).Split(';');
                                    float[] values = new float[splittedValue.Length];

                                    for (long l = 0; l != splittedValue.LongLength; l++) values[l] = float.Parse(splittedValue[l].Trim());
                                    monitorValues.Add(timeStamp, values);
                                }
                            }

                            string averages = GetAverageMonitorResults(monitorValues).Combine("; ");

                            var startedAt = delimiters[concurrency].Key;
                            var measuredRunTime = Math.Round((delimiters[concurrency].Value - startedAt).TotalMilliseconds, 2);
                            averageMonitorResults.Rows.Add(stresstest, monitor, startedAt, measuredRunTime, concurrency, headers, averages);
                        }
                    }
                }

                return averageMonitorResults;
            }
            return null;
        }
        /// <summary>
        /// From a 2 dimensional collection to an array of floats.
        /// </summary>
        /// <param name="monitorValues"></param>
        /// <returns></returns>
        private float[] GetAverageMonitorResults(Dictionary<DateTime, float[]> monitorValues) {
            var averageMonitorResults = new float[0];
            if (monitorValues.Count != 0) {
                //Average divider
                int valueCount = monitorValues.Count;
                averageMonitorResults = new float[valueCount];

                foreach (var key in monitorValues.Keys) {
                    var floats = monitorValues[key];

                    // The averages length must be the same as the floats length.
                    if (averageMonitorResults.Length != floats.Length) averageMonitorResults = new float[floats.Length];

                    for (long l = 0; l != floats.LongLength; l++) {
                        float value = floats[l], average = averageMonitorResults[l];

                        if (value == -1) //Detect invalid values.
                            averageMonitorResults[l] = -1;
                        else if (average != -1) //Add the value to the averages at the same index (i), divide it first (no overflow).
                            averageMonitorResults[l] = average + (value / valueCount);
                    }
                }
            }
            return averageMonitorResults;
        }
        public DataTable ExecuteQuery(string query) {
            if (_databaseActions == null) return null;
            return _databaseActions.GetDataTable(query);
        }

        private DataTable CreateEmptyDataTable(string name, string columnName1, params string[] columnNames) {
            var objectType = typeof(object);
            var dataTable = new DataTable(name);
            dataTable.Columns.Add(columnName1);
            foreach (string columnName in columnNames) dataTable.Columns.Add(columnName, objectType);
            return dataTable;
        }
        #endregion

        private string Parse(DateTime dateTime) { return dateTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff"); }

        /// <summary>
        /// Remove a schema (after cancel or failed)
        /// </summary>
        public void RemoveDatabase() {
            Schema.Drop(_databaseName, _databaseActions);
            _databaseName = null;
        }

        private class UserActionComparer : IComparer<string> {
            private static readonly UserActionComparer _userActionComparer = new UserActionComparer();

            public int Compare(string x, string y) {
                string ua = "User Action ";
                string nd = "None defined ";

                if (x.StartsWith(ua)) x = x.Substring(ua.Length);
                else if (x.StartsWith(nd)) x = x.Substring(nd.Length);

                if (y.StartsWith(ua)) y = y.Substring(ua.Length);
                else if (y.StartsWith(nd)) y = y.Substring(nd.Length);

                x = x.Split(':')[0];
                y = y.Split(':')[0];

                int i = int.Parse(x);
                int j = int.Parse(y);

                return i.CompareTo(j);
            }

            public static UserActionComparer GetInstance { get { return _userActionComparer; } }
        }
        private class LogEntryIndexComparer : IComparer<string> {
            private static readonly LogEntryIndexComparer _logEntryIndexComparer = new LogEntryIndexComparer();

            public int Compare(string x, string y) {
                string[] split1 = x.Split('.');
                string[] split2 = y.Split('.');

                int i = int.Parse(split1[0]);
                int j = int.Parse(split2[0]);
                if (i > j) return 1;
                else if (i < j) return -1;
                else {
                    i = int.Parse(split1[1]);
                    j = int.Parse(split2[1]);
                    return i.CompareTo(j);
                }
            }

            public static LogEntryIndexComparer GetInstance { get { return _logEntryIndexComparer; } }
        }
    }
}