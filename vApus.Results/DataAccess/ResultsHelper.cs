/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vApus.Util;

namespace vApus.Results {
    public class ResultsHelper {

        #region Fields
        private readonly object _lock = new object();

        private string _databaseName;

        private DatabaseActions _databaseActions;
        private string _passwordGUID = "{51E6A7AC-06C2-466F-B7E8-4B0A00F6A21F}";

        private readonly byte[] _salt = { 0x49, 0x16, 0x49, 0x2e, 0x11, 0x1e, 0x45, 0x24, 0x86, 0x05, 0x01, 0x03, 0x62 };

        private int _vApusInstanceId, _stresstestId;
        private ulong _stresstestResultId, _concurrencyResultId, _runResultId;
        #endregion

        #region Properties
        /// <summary>
        /// Returns null if not connected
        /// </summary>
        public string DatabaseName { get { return _databaseName; } }

        /// <summary>
        /// You can change this value for when you are distributed testing, so the right dataset is chosen.
        /// This value is not used when filtering results in the procedures eg GetAverageConcurrentUsers.
        /// </summary>
        public int StresstestId {
            get { return _stresstestId; }
            set { lock (_lock) _stresstestId = value; }
        }
        #endregion

        #region Initialize database before stresstest
        /// <summary>
        ///     Builds the schema if needed, if no MySQL target is found or no connection could be made an exception is returned.
        /// </summary>
        /// <returns></returns>
        public Exception BuildSchemaAndConnect() {
            lock (_lock) {
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
        }

        /// <summary>
        ///     Only inserts if connected (Call BuildSchemaAndConnect).
        /// </summary>
        /// <param name="description"></param>
        /// <param name="tags"></param>
        public void SetDescriptionAndTags(string description, string[] tags) {
            lock (_lock) {
                if (_databaseActions != null) {
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
        /// <returns>Id of the stresstest. 0 if BuildSchemaAndConnect was not called.</returns>
        public int SetvApusInstance(string hostName, string ip, int port, string version, string channel, bool isMaster) {
            lock (_lock) {
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
        /// <returns>Id of the stresstest. 0 if BuildSchemaAndConnect was not called.</returns>
        public int SetStresstest(string stresstest, string runSynchronization, string connection, string connectionProxy, string connectionString, string log, string logRuleSet, int[] concurrencies, int runs,
                                         int minimumDelayInMilliseconds, int maximumDelayInMilliseconds, bool shuffle, string distribute, int monitorBeforeInMinutes, int monitorAfterInMinutes) {
            lock (_lock) {
                if (_databaseActions != null) {
                    _databaseActions.ExecuteSQL(
                        string.Format(@"INSERT INTO stresstests(vApusInstanceId, Stresstest, RunSynchronization, Connection, ConnectionProxy, ConnectionString, Log, LogRuleSet, Concurrencies, Runs,
MinimumDelayInMilliseconds, MaximumDelayInMilliseconds, Shuffle, Distribute, MonitorBeforeInMinutes, MonitorAfterInMinutes)
VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}')",
                                      _vApusInstanceId, stresstest, runSynchronization, connection, connectionProxy, connectionString.Encrypt(_passwordGUID, _salt), log, logRuleSet,
                                      concurrencies.Combine(", "), runs, minimumDelayInMilliseconds, maximumDelayInMilliseconds, shuffle ? 1 : 0, distribute,
                                      monitorBeforeInMinutes, monitorAfterInMinutes)
                        );
                    _stresstestId = (int)_databaseActions.GetLastInsertId();
                    return _stresstestId;
                }
                return 0;
            }
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
        ///     0 if BuildSchemaAndConnect was not called..
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
        /// <returns>Id of the monitor. 0 if BuildSchemaAndConnect was not called.</returns>
        public ulong SetMonitor(int stresstestId, string monitor, string monitorSource, string connectionString, string machineConfiguration, string[] resultHeaders) {
            lock (_lock) {
                if (_databaseActions != null) {
                    _databaseActions.ExecuteSQL(
                        string.Format(
                            "INSERT INTO monitors(StresstestId, Monitor, MonitorSource, ConnectionString, MachineConfiguration, ResultHeaders) VALUES('{0}', ?Monitor, ?MonitorSource, ?ConnectionString, ?MachineConfiguration, ?ResultHeaders)", stresstestId),
                            CommandType.Text, new MySqlParameter("?Monitor", monitor), new MySqlParameter("?MonitorSource", monitorSource), new MySqlParameter("?ConnectionString", connectionString.Encrypt(_passwordGUID, _salt)),
                                new MySqlParameter("?MachineConfiguration", machineConfiguration), new MySqlParameter("?ResultHeaders", resultHeaders.Combine("; ", string.Empty))
                        );
                    return _databaseActions.GetLastInsertId();
                }
                return 0;
            }
        }
        #endregion

        #region Set Results

        #region Stresstest results
        /// <summary>
        ///     Started at datetime now.
        /// </summary>
        /// <param name="stresstestResult"></param>
        public void SetStresstestStarted(StresstestResult stresstestResult) {
            lock (_lock) {
                if (_databaseActions != null) {
                    _databaseActions.ExecuteSQL(
                        string.Format(
                            "INSERT INTO stresstestresults(StresstestId, StartedAt, StoppedAt, Status, StatusMessage) VALUES('{0}', '{1}', '{2}', 'OK', '')",
                            _stresstestId, Parse(stresstestResult.StartedAt), Parse(DateTime.MinValue))
                        );
                    _stresstestResultId = _databaseActions.GetLastInsertId();
                }
            }
        }

        /// <summary>
        ///     Stopped at datetime now.
        /// </summary>
        /// <param name="stresstestResult"></param>
        /// <param name="status"></param>
        /// <param name="statusMessage"></param>
        public void SetStresstestStopped(StresstestResult stresstestResult, string status = "OK", string statusMessage = "") {
            lock (_lock) {
                stresstestResult.StoppedAt = DateTime.Now;
                if (_databaseActions != null)
                    _databaseActions.ExecuteSQL(
                        string.Format(
                            "UPDATE stresstestresults SET StoppedAt='{1}', Status='{2}', StatusMessage='{3}' WHERE Id='{0}'",
                            _stresstestResultId, Parse(stresstestResult.StoppedAt), status, statusMessage)
                        );
            }
        }
        #endregion

        #region Concurrency results

        /// <summary>
        ///     Started at datetime now.
        /// </summary>
        /// <param name="stresstestResultId"></param>
        /// <param name="concurrencyResult"></param>
        public void SetConcurrencyStarted(ConcurrencyResult concurrencyResult) {
            lock (_lock) {
                if (_databaseActions != null) {
                    _databaseActions.ExecuteSQL(
                        string.Format(
                            "INSERT INTO concurrencyresults(StresstestResultId, Concurrency, StartedAt, StoppedAt) VALUES('{0}', '{1}', '{2}', '{3}')",
                            _stresstestResultId, concurrencyResult.Concurrency, Parse(concurrencyResult.StartedAt),
                            Parse(DateTime.MinValue))
                        );
                    _concurrencyResultId = _databaseActions.GetLastInsertId();
                }
            }
        }

        /// <summary>
        ///     Stopped at datetime now.
        /// </summary>
        /// <param name="concurrencyResult"></param>
        public void SetConcurrencyStopped(ConcurrencyResult concurrencyResult) {
            lock (_lock) {
                concurrencyResult.StoppedAt = DateTime.Now;
                if (_databaseActions != null)
                    _databaseActions.ExecuteSQL(
                        string.Format("UPDATE concurrencyresults SET StoppedAt='{1}' WHERE Id='{0}'", _concurrencyResultId,
                                      Parse(concurrencyResult.StoppedAt))
                        );
            }
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
            lock (_lock) {
                if (_databaseActions != null) {
                    _databaseActions.ExecuteSQL(
                        string.Format(
                            "INSERT INTO runresults(ConcurrencyResultId, Run, TotalLogEntryCount, ReRunCount, StartedAt, StoppedAt) VALUES('{0}', '{1}', '0', '0', '{2}', '{3}')",
                            _concurrencyResultId, runResult.Run, Parse(runResult.StartedAt), Parse(DateTime.MinValue))
                        );
                    _runResultId = _databaseActions.GetLastInsertId();
                }
            }
        }

        /// <summary>
        ///     Increase the rerun count value for the result using fx PrepareForRerun() before calling this fx.
        /// </summary>
        /// <param name="runResult"></param>
        public void SetRerun(RunResult runResult) {
            lock (_lock) {
                if (_databaseActions != null)
                    _databaseActions.ExecuteSQL(
                        string.Format("UPDATE runresults SET ReRunCount='{1}' WHERE Id='{0}'", _runResultId,
                                      runResult.RerunCount)
                        );
            }
        }

        /// <summary>
        ///     All the log entry results are save to the database doing this, only do this for the curent run.
        /// </summary>
        /// <param name="runResult"></param>
        public void SetRunStopped(RunResult runResult) {
            lock (_lock) {
                runResult.StoppedAt = DateTime.Now;
                if (_databaseActions != null) {
                    ulong totalLogEntryCount = 0;

                    foreach (VirtualUserResult virtualUserResult in runResult.VirtualUserResults) {
                        totalLogEntryCount += (ulong)virtualUserResult.LogEntryResults.LongLength;

                        if (virtualUserResult.VirtualUser != null) {
                            var rowsToInsert = new List<string>(); //Insert multiple values at once.
                            foreach (var logEntryResult in virtualUserResult.LogEntryResults)
                                if (logEntryResult.VirtualUser != null) {
                                    var sb = new StringBuilder("('");
                                    sb.Append(_runResultId);
                                    sb.Append("', '");
                                    sb.Append(virtualUserResult.VirtualUser);
                                    sb.Append("', '");
                                    sb.Append(MySQLEscapeString(logEntryResult.UserAction));
                                    sb.Append("', '");
                                    sb.Append(logEntryResult.LogEntryIndex);
                                    sb.Append("', '");
                                    sb.Append(logEntryResult.SameAsLogEntryIndex);
                                    sb.Append("', '");
                                    sb.Append(MySQLEscapeString(logEntryResult.LogEntry));
                                    sb.Append("', '");
                                    sb.Append(Parse(logEntryResult.SentAt));
                                    sb.Append("', '");
                                    sb.Append(logEntryResult.TimeToLastByteInTicks);
                                    sb.Append("', '");
                                    sb.Append(logEntryResult.DelayInMilliseconds);
                                    sb.Append("', '");
                                    sb.Append(MySQLEscapeString(logEntryResult.Error));
                                    sb.Append("', '");
                                    sb.Append(logEntryResult.Rerun);
                                    sb.Append("')");
                                    rowsToInsert.Add(sb.ToString());
                                }

                            _databaseActions.ExecuteSQL(string.Format("INSERT INTO logentryresults(RunResultId, VirtualUser, UserAction, LogEntryIndex, SameAsLogEntryIndex, LogEntry, SentAt, TimeToLastByteInTicks, DelayInMilliseconds, Error, Rerun) VALUES {0};",
                                rowsToInsert.Combine(", ")));
                        }
                    }
                    _databaseActions.ExecuteSQL(string.Format("UPDATE runresults SET TotalLogEntryCount='{1}', StoppedAt='{2}' WHERE Id='{0}'", _runResultId, totalLogEntryCount, Parse(runResult.StoppedAt)));
                }
            }
        }
        #endregion

        #region Monitor results

        /// <summary>
        ///     Do this at the end of the test.
        /// </summary>
        /// <param name="monitorResultCache">Should have a filled in monitor configuration id.</param>
        public void SetMonitorResults(MonitorResult monitorResultCache) {
            lock (_lock) {
                if (_databaseActions != null && monitorResultCache.Rows.Count != 0) {
                    ulong monitorConfigurationId = monitorResultCache.MonitorConfigurationId;
                    var rowsToInsert = new List<string>(); //Insert multiple values at once.
                    foreach (var row in monitorResultCache.Rows) {
                        var value = new List<float>();
                        for (int i = 1; i < row.Length; i++) value.Add((float)row[i]);

                        var sb = new StringBuilder("('");
                        sb.Append(monitorConfigurationId);
                        sb.Append("', '");
                        sb.Append(Parse((DateTime)row[0]));
                        sb.Append("', '");
                        sb.Append(MySQLEscapeString(MySQLEscapeString(value.Combine("; "))));
                        sb.Append("')");
                        rowsToInsert.Add(sb.ToString());
                    }
                    _databaseActions.ExecuteSQL(string.Format("INSERT INTO monitorresults(MonitorId, TimeStamp, Value) VALUES {0};", rowsToInsert.Combine(", ")));
                }
            }
        }

        #endregion

        #endregion

        //For getting stuff fom the database ReaderAndCombiner is used: You can execute a many-to-one distributed test (a tests workload divided over multiple slaves);
        //Results for such test must be combined before processing: it must look like a single test.
        #region Get Configuration and Formatted Results

        #region Configuration
        public string GetDescription() {
            lock (_lock) {
                if (_databaseActions != null) {
                    var dt = ReaderAndCombiner.GetDescription(_databaseActions); ;
                    foreach (DataRow row in dt.Rows) return row.ItemArray[0] as string;
                }
                return string.Empty;
            }
        }
        public List<string> GetTags() {
            lock (_lock) {
                var l = new List<string>();
                if (_databaseActions != null) {
                    var dt = ReaderAndCombiner.GetTags(_databaseActions);
                    foreach (DataRow row in dt.Rows) l.Add(row.ItemArray[0] as string);
                }
                return l;
            }
        }
        public List<int> GetvApusInstanceIds() {
            lock (_lock) {
                var l = new List<int>();
                if (_databaseActions != null) {
                    var dt = ReaderAndCombiner.GetvApusInstances(_databaseActions);
                    foreach (DataRow row in dt.Rows) l.Add((int)row.ItemArray[0]);
                }
                return l;
            }
        }
        public List<KeyValuePair<string, string>> GetvApusInstances() {
            lock (_lock) {
                var l = new List<KeyValuePair<string, string>>();
                if (_databaseActions != null) {
                    var dt = ReaderAndCombiner.GetvApusInstances(_databaseActions);
                    foreach (DataRow row in dt.Rows) {
                        l.Add(new KeyValuePair<string, string>(row.ItemArray[1] as string, string.Empty));
                        l.Add(new KeyValuePair<string, string>(row.ItemArray[2] + ":" + row[3], string.Empty));
                        l.Add(new KeyValuePair<string, string>("Version", row.ItemArray[4] as string));
                        l.Add(new KeyValuePair<string, string>("Channel", row.ItemArray[5] as string));
                        l.Add(new KeyValuePair<string, string>("Is Master", ((bool)row.ItemArray[6]) ? "Yes" : "No"));
                    }
                }
                return l;
            }
        }
        public List<KeyValuePair<string, string>> GetStresstestConfigurations() {
            lock (_lock) {
                var l = new List<KeyValuePair<string, string>>();
                if (_databaseActions != null) {
                    var dt = ReaderAndCombiner.GetStresstests(new CancellationToken(), _databaseActions);
                    foreach (DataRow row in dt.Rows) {
                        l.Add(new KeyValuePair<string, string>(row.ItemArray[2] as string, string.Empty));
                        l.Add(new KeyValuePair<string, string>("RunSynchronization", row.ItemArray[3] as string));
                        l.Add(new KeyValuePair<string, string>(row.ItemArray[4] as string, string.Empty));
                        l.Add(new KeyValuePair<string, string>(row.ItemArray[5] as string, string.Empty));
                        l.Add(new KeyValuePair<string, string>(row.ItemArray[7] as string, string.Empty));
                        l.Add(new KeyValuePair<string, string>(row.ItemArray[8] as string, string.Empty));
                        l.Add(new KeyValuePair<string, string>("Concurrencies", row.ItemArray[9] as string));
                        l.Add(new KeyValuePair<string, string>("Runs", row.ItemArray[10].ToString()));
                        int minDelay = (int)row.ItemArray[11];
                        int maxDelay = (int)row.ItemArray[12];
                        l.Add(new KeyValuePair<string, string>("Delay", minDelay == maxDelay ? minDelay + " ms" : minDelay + " - " + maxDelay + " ms"));
                        l.Add(new KeyValuePair<string, string>("Shuffle", ((bool)row.ItemArray[13]) ? "Yes" : "No"));
                        l.Add(new KeyValuePair<string, string>("Distribute", row.ItemArray[14] as string));
                        l.Add(new KeyValuePair<string, string>("Monitor Before", row.ItemArray[15] + " minutes"));
                        l.Add(new KeyValuePair<string, string>("Monitor After", row.ItemArray[16] + " minutes"));
                    }
                }
                return l;
            }
        }
        public List<string> GetMonitors() {
            lock (_lock) {
                var l = new List<string>();
                if (_databaseActions != null)
                    foreach (DataRow row in ReaderAndCombiner.GetMonitors(new CancellationToken(), _databaseActions).Rows)
                        l.Add(row.ItemArray[0] + " (" + row.ItemArray[1] + ")");

                return l;
            }
        }
        #endregion

        #region Formatted Results
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">Used in await Tast.Run...</param>
        /// <param name="stresstestIds">If none, all the results for all tests will be returned.</param>
        /// <returns></returns>
        public DataTable GetAverageConcurrencyResults(CancellationToken cancellationToken, params int[] stresstestIds) {
            lock (_lock) {
                if (_databaseActions != null) {
                    var stresstests = ReaderAndCombiner.GetStresstests(cancellationToken, _databaseActions, stresstestIds, "Id", "Stresstest", "Connection", "Runs");
                    if (stresstests == null || stresstests.Rows.Count == 0) return null;

                    var averageConcurrentUsers = CreateEmptyDataTable("AverageConcurrentUsers", "Stresstest", "Started At", "Measured Time (ms)", "Concurrency",
        "Log Entries Processed", "Log Entries", "Throughput (responses / s)", "User Actions / s", "Avg. Response Time (ms)",
        "Max. Response Time (ms)", "95th Percentile of the Response Times (ms)", "Avg. Delay (ms)", "Errors");

                    foreach (DataRow stresstestsRow in stresstests.Rows) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        int stresstestId = (int)stresstestsRow.ItemArray[0];

                        var stresstestResults = ReaderAndCombiner.GetStresstestResults(cancellationToken, _databaseActions, stresstestId, "Id");
                        if (stresstestResults == null || stresstestResults.Rows.Count == 0) continue;
                        int stresstestResultId = (int)stresstestResults.Rows[0].ItemArray[0];

                        string stresstest = string.Format("{0} {1}", stresstestsRow.ItemArray[1], stresstestsRow.ItemArray[2]);
                        int runs = (int)stresstestsRow.ItemArray[3];

                        var concurrencyResults = ReaderAndCombiner.GetConcurrencyResults(cancellationToken, _databaseActions, stresstestResultId, new string[] { "Id", "StartedAt", "StoppedAt", "Concurrency" });
                        if (concurrencyResults == null || concurrencyResults.Rows.Count == 0) continue;

                        foreach (DataRow crRow in concurrencyResults.Rows) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            ConcurrencyResult concurrencyResult = new ConcurrencyResult((int)crRow.ItemArray[3], runs);
                            concurrencyResult.StartedAt = (DateTime)crRow.ItemArray[1];
                            concurrencyResult.StoppedAt = (DateTime)crRow.ItemArray[2];

                            var runResults = ReaderAndCombiner.GetRunResults(cancellationToken, _databaseActions, (int)crRow.ItemArray[0], "Id", "Run", "TotalLogEntryCount");
                            if (runResults == null || runResults.Rows.Count == 0) continue;

                            var runResultIds = new List<int>(runResults.Rows.Count);
                            var totalLogEntryCountsPerUser = new List<ulong>(runResults.Rows.Count);
                            foreach (DataRow rrRow in runResults.Rows) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                runResultIds.Add((int)rrRow.ItemArray[0]);
                                concurrencyResult.RunResults.Add(new RunResult((int)rrRow.ItemArray[1], concurrencyResult.Concurrency));

                                totalLogEntryCountsPerUser.Add((ulong)rrRow.ItemArray[2] / (ulong)concurrencyResult.Concurrency);
                            }

                            var ler = ReaderAndCombiner.GetLogEntryResults(cancellationToken, _databaseActions, runResultIds.ToArray(),
                                new string[] { "RunResultId", "VirtualUser", "UserAction", "LogEntryIndex", "TimeToLastByteInTicks", "DelayInMilliseconds", "Error" });
                            if (ler == null || ler.Rows.Count == 0) continue;

                            for (int i = 0; i != concurrencyResult.RunResults.Count; i++) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                var runResult = concurrencyResult.RunResults[i];
                                int runResultId = runResultIds[i];
                                var virtualUserResults = new ConcurrentDictionary<string, VirtualUserResult>();
                                var logEntryResults = new ConcurrentDictionary<string, List<LogEntryResult>>(); //Key == virtual user.

                                foreach (DataRow lerRow in ler.Rows) {
                                    if (cancellationToken.IsCancellationRequested) return null;

                                    if ((int)lerRow.ItemArray[0] == runResultId) {
                                        string virtualUser = (lerRow.ItemArray[1] as string);
                                        logEntryResults.TryAdd(virtualUser, new List<LogEntryResult>());

                                        logEntryResults[virtualUser].Add(new LogEntryResult() {
                                            VirtualUser = virtualUser, UserAction = lerRow.ItemArray[2] as string, LogEntryIndex = lerRow.ItemArray[3] as string,
                                            TimeToLastByteInTicks = (long)lerRow.ItemArray[4], DelayInMilliseconds = (int)lerRow.ItemArray[5], Error = lerRow.ItemArray[6] as string
                                        });
                                    }
                                }

                                //Add empty ones for broken runs.
                                Parallel.ForEach(logEntryResults, (item, loopState) => {
                                    if (cancellationToken.IsCancellationRequested) loopState.Break();

                                    while ((ulong)item.Value.Count < totalLogEntryCountsPerUser[i])
                                        item.Value.Add(new LogEntryResult());
                                });

                                //Add the log entry result to the virtual users.
                                Parallel.ForEach(logEntryResults, (item, loopState) => {
                                    if (cancellationToken.IsCancellationRequested) loopState.Break();

                                    virtualUserResults.TryAdd(item.Key, new VirtualUserResult(logEntryResults[item.Key].Count) { VirtualUser = item.Key });

                                    for (int k = 0; k != item.Value.Count; k++)
                                        virtualUserResults[item.Key].LogEntryResults[k] = item.Value[k];
                                });
                                if (cancellationToken.IsCancellationRequested) return null;

                                runResult.VirtualUserResults = virtualUserResults.Values.ToArray();
                            }

                            var metrics = StresstestMetricsHelper.GetMetrics(concurrencyResult, true);

                            averageConcurrentUsers.Rows.Add(stresstest, metrics.StartMeasuringTime, Math.Round(metrics.MeasuredTime.TotalMilliseconds, 2),
                                metrics.Concurrency, metrics.LogEntriesProcessed, metrics.LogEntries, Math.Round(metrics.ResponsesPerSecond, 2), Math.Round(metrics.UserActionsPerSecond, 2),
                                Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2), Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2), Math.Round(metrics.Percentile95thResponseTimes.TotalMilliseconds, 2),
                                Math.Round(metrics.AverageDelay.TotalMilliseconds, 2), metrics.Errors);
                        }
                    }
                    return averageConcurrentUsers;
                }
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">Used in await Tast.Run...</param>
        /// <param name="stresstestIds">If none, all the results for all tests will be returned.</param>
        /// <returns></returns>
        public DataTable GetAverageUserActionResults(CancellationToken cancellationToken, params int[] stresstestIds) {
            lock (_lock) {
                if (_databaseActions != null) {
                    var stresstests = ReaderAndCombiner.GetStresstests(cancellationToken, _databaseActions, stresstestIds, "Id", "Stresstest", "Connection");
                    if (stresstests == null || stresstests.Rows.Count == 0) return null;

                    var averageUserActions = CreateEmptyDataTable("AverageUserActions", "Stresstest", "Concurrency", "User Action", "Avg. Response Time (ms)",
        "Max. Response Time (ms)", "95th Percentile of the Response Times (ms)", "Avg. Delay (ms)", "Errors");

                    foreach (DataRow stresstestsRow in stresstests.Rows) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        int stresstestId = (int)stresstestsRow.ItemArray[0];

                        var stresstestResults = ReaderAndCombiner.GetStresstestResults(cancellationToken, _databaseActions, stresstestId, "Id");
                        if (stresstestResults == null || stresstestResults.Rows.Count == 0) continue;
                        int stresstestResultId = (int)stresstestResults.Rows[0].ItemArray[0];

                        string stresstest = string.Format("{0} {1}", stresstestsRow.ItemArray[1], stresstestsRow.ItemArray[2]);

                        var concurrencyResults = ReaderAndCombiner.GetConcurrencyResults(cancellationToken, _databaseActions, stresstestResultId, "Id", "Concurrency");
                        if (concurrencyResults == null || concurrencyResults.Rows.Count == 0) continue;

                        foreach (DataRow crRow in concurrencyResults.Rows) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            int concurrencyResultId = (int)crRow.ItemArray[0];
                            int concurrency = (int)crRow.ItemArray[1];

                            var runResults = ReaderAndCombiner.GetRunResults(cancellationToken, _databaseActions, concurrencyResultId, "Id", "RerunCount");
                            if (runResults == null || runResults.Rows.Count == 0) continue;

                            var runResultIds = new List<int>(runResults.Rows.Count);
                            foreach (DataRow rrRow in runResults.Rows) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                runResultIds.Add((int)rrRow.ItemArray[0]);
                            }

                            var logEntryResults = ReaderAndCombiner.GetLogEntryResults(cancellationToken, _databaseActions, runResultIds.ToArray(),
                               new string[] { "RunResultId", "VirtualUser", "UserAction", "LogEntryIndex", "SameAsLogEntryIndex", "TimeToLastByteInTicks", "DelayInMilliseconds", "Error", "Rerun" });
                            if (logEntryResults == null || logEntryResults.Rows.Count == 0) continue;

                            //Place the log entry results under the right virtual user and the right user action
                            var userActions = new List<KeyValuePair<string, List<KeyValuePair<string, List<LogEntryResult>>>>>(); // <VirtualUser,<UserAction, LogEntryResult
                            foreach (DataRow rrRow in runResults.Rows) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                int runResultId = (int)rrRow.ItemArray[0];

                                //Keeping reruns in mind (break on last)
                                int runs = ((int)rrRow.ItemArray[1]) + 1;
                                var userActionsMap = new Dictionary<string, string>(); //Map duplicate user actions to the original ones.
                                for (int reRun = 0; reRun != runs; reRun++) {
                                    if (cancellationToken.IsCancellationRequested) return null;

                                    var uas = new Dictionary<string, Dictionary<string, List<LogEntryResult>>>(); // <VirtualUser,<UserAction, LogEntryResult
                                    foreach (DataRow lerRow in logEntryResults.Rows) {
                                        if (cancellationToken.IsCancellationRequested) return null;

                                        object[] row = lerRow.ItemArray;
                                        if ((int)row[0] == runResultId && (int)row[8] == reRun) {
                                            string virtualUser = row[1] + "-" + reRun; //Make "virtual" virtual users :), handy way to make a correct average doing it like this.
                                            string userAction = lerRow.ItemArray[2] as string;

                                            string logEntryIndex = lerRow.ItemArray[4] as string; //Combine results when using distribe like this.
                                            if (logEntryIndex == string.Empty) {
                                                logEntryIndex = lerRow.ItemArray[3] as string;

                                                //Make sure we have all the user actions before averages are calcullated, otherwise the duplicated user action names can be used.
                                                //Map using the log entry index
                                                if (!userActionsMap.ContainsKey(logEntryIndex)) userActionsMap.Add(logEntryIndex, userAction);
                                            }

                                            var logEntryResult = new LogEntryResult() { LogEntryIndex = logEntryIndex, TimeToLastByteInTicks = (long)lerRow.ItemArray[5], DelayInMilliseconds = (int)lerRow.ItemArray[6], Error = lerRow.ItemArray[7] as string };

                                            if (!uas.ContainsKey(virtualUser)) uas.Add(virtualUser, new Dictionary<string, List<LogEntryResult>>());
                                            if (!uas[virtualUser].ContainsKey(userAction)) uas[virtualUser].Add(userAction, new List<LogEntryResult>());

                                            uas[virtualUser][userAction].Add(logEntryResult);
                                        }
                                    }
                                    foreach (string virtualUser in uas.Keys) {
                                        if (cancellationToken.IsCancellationRequested) return null;

                                        var kvp = new KeyValuePair<string, List<KeyValuePair<string, List<LogEntryResult>>>>(virtualUser, new List<KeyValuePair<string, List<LogEntryResult>>>());
                                        foreach (string userAction in uas[virtualUser].Keys) {
                                            string mappedUserAction = userAction;
                                            var lers = uas[virtualUser][userAction];
                                            if (lers.Count != 0) {
                                                var leri = lers[0].LogEntryIndex;
                                                if (userActionsMap.ContainsKey(leri)) mappedUserAction = userActionsMap[leri];
                                            }
                                            kvp.Value.Add(new KeyValuePair<string, List<LogEntryResult>>(mappedUserAction, lers));
                                        }
                                        userActions.Add(kvp);
                                    }
                                }
                            }

                            //Calculate following for each user action
                            var userActionResults = CreateEmptyDataTable("UserActionResults", "UserAction", "TimeToLastByteInTicks", "DelayInMilliseconds", "Errors");
                            for (int i = 0; i != userActions.Count; i++) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                foreach (var kvp in userActions[i].Value) {
                                    if (cancellationToken.IsCancellationRequested) return null;

                                    long ttlb = 0;
                                    int delay = -1;
                                    long ers = 0;

                                    string userAction = kvp.Key;
                                    var lers = kvp.Value;

                                    for (int j = lers.Count - 1; j != -1; j--) {
                                        if (cancellationToken.IsCancellationRequested) return null;

                                        var ler = lers[j];
                                        if (delay == -1) {
                                            delay = ler.DelayInMilliseconds;
                                            ttlb = ler.TimeToLastByteInTicks;
                                        } else {
                                            ttlb += ler.TimeToLastByteInTicks + ler.DelayInMilliseconds;
                                        }
                                        if (!string.IsNullOrEmpty(ler.Error)) ++ers;
                                    }
                                    userActionResults.Rows.Add(userAction, ttlb, delay, ers);
                                }
                            }

                            var uniqueUserActionCounts = new Dictionary<string, int>(); //To make a correct average.
                            foreach (DataRow uarRow in userActionResults.Rows) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                string userAction = uarRow[0] as string;

                                if (uniqueUserActionCounts.ContainsKey(userAction)) ++uniqueUserActionCounts[userAction];
                                else uniqueUserActionCounts.Add(userAction, 1);
                            }

                            //Finally the averages
                            //The key of the entries for following collections are user actions.
                            var avgTimeToLastByteInTicks = new Dictionary<string, double>();
                            var maxTimeToLastByteInTicks = new Dictionary<string, long>();
                            var timeToLastBytesInTicks = new Dictionary<string, List<long>>();
                            var percTimeToLastBytesInTicks = new Dictionary<string, long>();

                            var avgDelay = new Dictionary<string, double>();
                            var errors = new Dictionary<string, long>();

                            foreach (DataRow uarRow in userActionResults.Rows) {
                                if (cancellationToken.IsCancellationRequested) return null;

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
                                if (cancellationToken.IsCancellationRequested) return null;

                                var l = timeToLastBytesInTicks[userAction];

                                int percent5 = (int)(l.Count * 0.05);
                                if (percent5 == 0)
                                    percTimeToLastBytesInTicks.Add(userAction, maxTimeToLastByteInTicks[userAction]);
                                else {
                                    l.Sort();
                                    percTimeToLastBytesInTicks.Add(userAction, l[l.Count - percent5 - 1]);
                                }
                            }

                            //Sort the user actions
                            List<string> sortedUserActions = avgTimeToLastByteInTicks.Keys.ToList();
                            sortedUserActions.Sort(UserActionComparer.GetInstance);

                            var correctedUserActions = new Dictionary<string, string>(); //user action, corrected user action
                            int correctIndex = 1;
                            string ua = "User Action ";
                            string nd = "None defined ";
                            foreach (string userAction in sortedUserActions) {
                                string part1 = ua;
                                string part2 = userAction;
                                if (part2.StartsWith(ua)) {
                                    part2 = part2.Substring(ua.Length);
                                } else if (part2.StartsWith(nd)) {
                                    part1 = nd;
                                    part2 = part2.Substring(nd.Length);
                                }

                                string[] split = part2.Split(':');
                                part2 = split[0];
                                string part3 = split.Length == 2 ? ":" + split[1] : string.Empty;

                                int index = int.Parse(split[0]);
                                correctedUserActions.Add(userAction, index == correctIndex ? userAction : part1 + correctIndex + part3);

                                ++correctIndex;
                            }

                            //Add the sorted user actions to the whole.
                            foreach (string s in sortedUserActions) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                averageUserActions.Rows.Add(stresstest, concurrency, correctedUserActions[s],
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
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">Used in await Tast.Run...</param>
        /// <param name="stresstestIds">If none, all the results for all tests will be returned.</param>
        /// <returns></returns>
        public DataTable GetAverageLogEntryResults(CancellationToken cancellationToken, params int[] stresstestIds) {
            lock (_lock) {
                if (_databaseActions != null) {
                    var stresstests = ReaderAndCombiner.GetStresstests(cancellationToken, _databaseActions, stresstestIds, "Id", "Stresstest", "Connection");
                    if (stresstests == null || stresstests.Rows.Count == 0) return null;

                    var averageLogEntries = CreateEmptyDataTable("AverageLogEntries", "Stresstest", "Concurrency", "User Action", "Log Entry", "Avg. Response Time (ms)",
    "Max. Response Time (ms)", "95th Percentile of the Response Times (ms)", "Avg. Delay (ms)", "Errors");

                    foreach (DataRow stresstestsRow in stresstests.Rows) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        int stresstestId = (int)stresstestsRow.ItemArray[0];

                        var stresstestResults = ReaderAndCombiner.GetStresstestResults(cancellationToken, _databaseActions, stresstestId, "Id");
                        if (stresstestResults == null || stresstestResults.Rows.Count == 0) continue;
                        int stresstestResultId = (int)stresstestResults.Rows[0].ItemArray[0];

                        string stresstest = string.Format("{0} {1}", stresstestsRow.ItemArray[1], stresstestsRow.ItemArray[2]);
                        var concurrencyResults = ReaderAndCombiner.GetConcurrencyResults(cancellationToken, _databaseActions, stresstestResultId, "Id", "Concurrency");
                        if (concurrencyResults == null || concurrencyResults.Rows.Count == 0) continue;

                        foreach (DataRow crRow in concurrencyResults.Rows) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            int concurrencyResultId = (int)crRow.ItemArray[0];
                            int concurrency = (int)crRow.ItemArray[1];

                            var runResults = ReaderAndCombiner.GetRunResults(cancellationToken, _databaseActions, concurrencyResultId, "Id");
                            if (runResults == null || runResults.Rows.Count == 0) continue;

                            var runResultIds = new List<int>(runResults.Rows.Count);
                            foreach (DataRow rrRow in runResults.Rows) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                runResultIds.Add((int)rrRow.ItemArray[0]);
                            }

                            var logEntryResults = ReaderAndCombiner.GetLogEntryResults(cancellationToken, _databaseActions, runResultIds.ToArray(),
                                new string[] { "LogEntryIndex", "SameAsLogEntryIndex", "UserAction", "LogEntry", "TimeToLastByteInTicks", "DelayInMilliseconds", "Error" });
                            if (logEntryResults == null || logEntryResults.Rows.Count == 0) continue;

                            //We don't need to keep the run ids for this one, it's much faster and simpler like this.
                            var uniqueLogEntryCounts = new Dictionary<string, int>(); //To make a correct average.
                            var userActions = new Dictionary<string, string>(); //log entry index, User Action
                            foreach (DataRow lerRow in logEntryResults.Rows) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                object[] row = lerRow.ItemArray;
                                string logEntryIndex = row[1] as string; //Combine results when using distribution like this.
                                if (logEntryIndex == string.Empty) {
                                    logEntryIndex = row[0] as string;

                                    //Make sure we have all the user actions before averages are calcullated, otherwise the duplicated user action names can be used. 
                                    if (!userActions.ContainsKey(logEntryIndex)) {
                                        string userAction = row[2] as string;
                                        userActions.Add(logEntryIndex, userAction);
                                    }
                                }

                                if (uniqueLogEntryCounts.ContainsKey(logEntryIndex)) ++uniqueLogEntryCounts[logEntryIndex];
                                else uniqueLogEntryCounts.Add(logEntryIndex, 1);
                            }

                            var logEntries = new Dictionary<string, string>(); //log entry index, log entry

                            var avgTimeToLastByteInTicks = new Dictionary<string, double>();
                            var maxTimeToLastByteInTicks = new Dictionary<string, long>();
                            var timeToLastBytesInTicks = new Dictionary<string, List<long>>();
                            var percTimeToLastBytesInTicks = new Dictionary<string, long>();

                            var avgDelay = new Dictionary<string, double>();
                            var errors = new Dictionary<string, long>();

                            foreach (DataRow lerRow in logEntryResults.Rows) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                object[] row = lerRow.ItemArray;
                                string logEntryIndex = row[1] as string; //Combine results when using distribution like this.
                                if (logEntryIndex == string.Empty) logEntryIndex = row[0] as string;

                                string userAction = row[2] as string;
                                string logEntry = row[3] as string;
                                long ttlb = (long)row[4];
                                int delay = (int)row[5];
                                string error = row[6] as string;

                                int uniqueLogEntryCount = uniqueLogEntryCounts[logEntryIndex];

                                if (!userActions.ContainsKey(logEntryIndex)) userActions.Add(logEntryIndex, userAction);
                                if (!logEntries.ContainsKey(logEntryIndex)) logEntries.Add(logEntryIndex, logEntry);

                                if (avgTimeToLastByteInTicks.ContainsKey(logEntryIndex)) avgTimeToLastByteInTicks[logEntryIndex] += (((double)ttlb) / uniqueLogEntryCount);
                                else avgTimeToLastByteInTicks.Add(logEntryIndex, (((double)ttlb) / uniqueLogEntryCount));

                                if (maxTimeToLastByteInTicks.ContainsKey(logEntryIndex)) { if (maxTimeToLastByteInTicks[logEntryIndex] < ttlb) maxTimeToLastByteInTicks[logEntryIndex] = ttlb; } else maxTimeToLastByteInTicks.Add(logEntryIndex, ttlb);

                                if (!timeToLastBytesInTicks.ContainsKey(logEntryIndex)) timeToLastBytesInTicks.Add(logEntryIndex, new List<long>(uniqueLogEntryCount));
                                timeToLastBytesInTicks[logEntryIndex].Add(ttlb);

                                if (avgDelay.ContainsKey(logEntryIndex)) avgDelay[logEntryIndex] += (((double)delay) / uniqueLogEntryCount);
                                else avgDelay.Add(logEntryIndex, ((double)delay) / uniqueLogEntryCount);


                                if (!errors.ContainsKey(logEntryIndex)) errors.Add(logEntryIndex, 0);
                                if (!string.IsNullOrEmpty(error)) ++errors[logEntryIndex];
                            }

                            //95th percentile
                            foreach (string logEntryIndex in timeToLastBytesInTicks.Keys) {
                                if (cancellationToken.IsCancellationRequested) return null;

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

                            //Correct the indices of the user actions
                            List<string> sortedUserActions = userActions.Values.ToList();
                            sortedUserActions.Sort(UserActionComparer.GetInstance);

                            var correctedUserActions = new Dictionary<string, string>(); //user action, corrected user action
                            int correctIndex = 1;
                            string ua = "User Action ";
                            string nd = "None defined ";
                            foreach (string userAction in sortedUserActions)
                                if (!correctedUserActions.ContainsKey(userAction)) {
                                    string part1 = ua;
                                    string part2 = userAction;
                                    if (part2.StartsWith(ua)) {
                                        part2 = part2.Substring(ua.Length);
                                    } else if (part2.StartsWith(nd)) {
                                        part1 = nd;
                                        part2 = part2.Substring(nd.Length);
                                    }

                                    string[] split = part2.Split(':');
                                    part2 = split[0];
                                    string part3 = split.Length == 2 ? ":" + split[1] : string.Empty;

                                    int index = int.Parse(split[0]);
                                    correctedUserActions.Add(userAction, index == correctIndex ? userAction : part1 + correctIndex + part3);

                                    ++correctIndex;
                                }

                            foreach (string s in sortedLogEntryIndices) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                averageLogEntries.Rows.Add(stresstest, concurrency, correctedUserActions[userActions[s]], logEntries[s],
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
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">Used in await Tast.Run...</param>
        /// <param name="stresstestIds">If none, all the results for all tests will be returned.</param>
        /// <returns></returns>
        public DataTable GetErrors(CancellationToken cancellationToken, params int[] stresstestIds) {
            lock (_lock) {
                if (_databaseActions != null) {
                    var stresstests = ReaderAndCombiner.GetStresstests(cancellationToken, _databaseActions, stresstestIds, "Id", "Stresstest", "Connection");
                    if (stresstests == null || stresstests.Rows.Count == 0) return null;

                    var errors = CreateEmptyDataTable("Error", "Stresstest", "Concurrency", "Run", "Virtual User", "User Action", "Log Entry", "Error");

                    foreach (DataRow stresstestsRow in stresstests.Rows) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        int stresstestId = (int)stresstestsRow.ItemArray[0];

                        var stresstestResults = ReaderAndCombiner.GetStresstestResults(cancellationToken, _databaseActions, stresstestId, "Id");
                        if (stresstestResults == null || stresstestResults.Rows.Count == 0) continue;
                        int stresstestResultId = (int)stresstestResults.Rows[0].ItemArray[0];

                        string stresstest = string.Format("{0} {1}", stresstestsRow.ItemArray[1], stresstestsRow.ItemArray[2]);
                        var concurrencyResults = ReaderAndCombiner.GetConcurrencyResults(cancellationToken, _databaseActions, stresstestResultId, "Id", "Concurrency");
                        if (concurrencyResults == null || concurrencyResults.Rows.Count == 0) continue;

                        foreach (DataRow crRow in concurrencyResults.Rows) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            int concurrencyResultId = (int)crRow.ItemArray[0];
                            object concurrency = crRow.ItemArray[1];

                            var runResults = ReaderAndCombiner.GetRunResults(cancellationToken, _databaseActions, concurrencyResultId, "Id", "Run");
                            if (runResults == null || runResults.Rows.Count == 0) continue;

                            foreach (DataRow rrRow in runResults.Rows) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                int runResultId = (int)rrRow.ItemArray[0];
                                object run = rrRow.ItemArray[1];

                                var logEntryResults = ReaderAndCombiner.GetLogEntryResults(cancellationToken, _databaseActions, "CHAR_LENGTH(Error) != 0", runResultId, "RunResultId", "VirtualUser", "UserAction", "LogEntry", "Error");
                                if (logEntryResults == null || logEntryResults.Rows.Count == 0) continue;

                                foreach (DataRow ldr in logEntryResults.Rows) {
                                    if (cancellationToken.IsCancellationRequested) return null;

                                    errors.Rows.Add(stresstest, concurrency, run, ldr.ItemArray[1], ldr.ItemArray[2], ldr.ItemArray[3], ldr.ItemArray[4]);
                                }
                            }
                        }
                    }
                    return errors;
                }
                return null;
            }
        }

        /// <summary>
        /// Get the user actions and the log entries within, these are asked for the first user of the first run, so if you cancel a test it will not be correct.
        /// However, this is the fastest way to do this and there are no problems with a finished test.
        /// </summary>
        /// <param name="cancellationToken">Used in await Tast.Run...</param>
        /// <param name="stresstestIds">If none, all the results for all tests will be returned.</param>
        /// <returns></returns>
        public DataTable GetUserActionComposition(CancellationToken cancellationToken, params int[] stresstestIds) {
            lock (_lock) {
                if (_databaseActions == null) return null;
                var stresstests = ReaderAndCombiner.GetStresstests(cancellationToken, _databaseActions, stresstestIds, "Id", "Stresstest", "Connection");
                if (stresstests == null || stresstests.Rows.Count == 0) return null;

                var userActionComposition = CreateEmptyDataTable("UserActionComposition", "Stresstest", "User Action", "Log Entry");

                foreach (DataRow stresstestsRow in stresstests.Rows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    int stresstestId = (int)stresstestsRow.ItemArray[0];

                    var stresstestResults = ReaderAndCombiner.GetStresstestResults(cancellationToken, _databaseActions, stresstestId, "Id");
                    if (stresstestResults == null || stresstestResults.Rows.Count == 0) continue;

                    int stresstestResultId = (int)stresstestResults.Rows[0].ItemArray[0];

                    string stresstest = string.Format("{0} {1}", stresstestsRow.ItemArray[1], stresstestsRow.ItemArray[2]);
                    var concurrencyResults = ReaderAndCombiner.GetConcurrencyResults(cancellationToken, _databaseActions, stresstestResultId, "Id");
                    if (concurrencyResults == null || concurrencyResults.Rows.Count == 0) continue;

                    foreach (DataRow crRow in concurrencyResults.Rows) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        var runResults = ReaderAndCombiner.GetRunResults(cancellationToken, _databaseActions, (int)crRow.ItemArray[0], "Id");
                        if (runResults == null || runResults.Rows.Count == 0) continue;

                        foreach (DataRow rrRow in runResults.Rows) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            var logEntryResults = ReaderAndCombiner.GetLogEntryResults(cancellationToken, _databaseActions, "VirtualUser='vApus Thread Pool Thread #1'", (int)rrRow.ItemArray[0], "UserAction", "LogEntry", "SameAsLogEntryIndex");
                            if (logEntryResults == null || logEntryResults.Rows.Count == 0) continue;

                            var userActions = new Dictionary<string, List<string>>();
                            foreach (DataRow lerRow in logEntryResults.Rows) {
                                if (cancellationToken.IsCancellationRequested) return null;
                                object[] row = lerRow.ItemArray;

                                if (row[2] as string == string.Empty) { //We don't want duplicates
                                    string userAction = row[0] as string;
                                    string logEntry = row[1] as string;
                                    if (!userActions.ContainsKey(userAction)) userActions.Add(userAction, new List<string>());
                                    if (!userActions[userAction].Contains(logEntry)) userActions[userAction].Add(logEntry);
                                }
                            }

                            //Sort the user actions
                            List<string> sortedUserActions = userActions.Keys.ToList();
                            sortedUserActions.Sort(UserActionComparer.GetInstance);

                            var correctedUserActions = new Dictionary<string, string>(); //user action, corrected user action
                            int correctIndex = 1;
                            string ua = "User Action ";
                            string nd = "None defined ";
                            foreach (string userAction in sortedUserActions)
                                if (!correctedUserActions.ContainsKey(userAction)) {
                                    string part1 = ua;
                                    string part2 = userAction;
                                    if (part2.StartsWith(ua)) {
                                        part2 = part2.Substring(ua.Length);
                                    } else if (part2.StartsWith(nd)) {
                                        part1 = nd;
                                        part2 = part2.Substring(nd.Length);
                                    }

                                    string[] split = part2.Split(':');
                                    part2 = split[0];
                                    string part3 = split.Length == 2 ? ":" + split[1] : string.Empty;

                                    int index = int.Parse(split[0]);
                                    correctedUserActions.Add(userAction, index == correctIndex ? userAction : part1 + correctIndex + part3);

                                    ++correctIndex;
                                }

                            foreach (string userAction in sortedUserActions) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                foreach (string logEntry in userActions[userAction]) {
                                    if (cancellationToken.IsCancellationRequested) return null;

                                    userActionComposition.Rows.Add(stresstest, correctedUserActions[userAction], logEntry);
                                }
                            }
                            break;
                        }
                        break;
                    }
                }
                return userActionComposition;
            }
        }
        /// <summary>
        /// Only works for the first stresstest. This is a known issue and it will not be fixed: 1 datatable per stressstest only, otherwise the overview is worth nothing. Use a loop to enumerate multiple stresstest ids.
        /// </summary>
        /// <param name="cancellationToken">Used in await Tast.Run...</param>
        /// <param name="stresstestIds">If none, all the results for all tests will be returned.</param>
        /// <returns></returns>
        public DataTable GetOverview(CancellationToken cancellationToken, params int[] stresstestIds) {
            lock (_lock) {
                if (_databaseActions == null) return null;

                if (stresstestIds.Length == 0) {
                    var stresstests = ReaderAndCombiner.GetStresstests(cancellationToken, _databaseActions, "Id");
                    if (stresstests == null || stresstests.Rows.Count == 0) return null;

                    stresstestIds = new int[stresstests.Rows.Count];
                    for (int i = 0; i != stresstestIds.Length; i++)
                        stresstestIds[i] = (int)stresstests.Rows[i].ItemArray[0];
                }

                var averageUserActions = GetAverageUserActionResults(cancellationToken, stresstestIds);
                if (averageUserActions == null) return null;

                var averageConcurrentUsers = GetAverageConcurrencyResults(cancellationToken, stresstestIds);
                if (averageConcurrentUsers == null) return null;

                var averageResponseTimesAndThroughput = CreateEmptyDataTable("AverageResponseTimesAndThroughput", "Stresstest", "Concurrency");
                int range = 0; //The range of values (avg response times) to place under the right user action
                foreach (DataRow uaRow in averageUserActions.Rows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    string userAction = (uaRow.ItemArray[2] as string).Substring(12); //Trim 'User Action: '
                    if (!averageResponseTimesAndThroughput.Columns.Contains(userAction)) {
                        averageResponseTimesAndThroughput.Columns.Add(userAction);
                        range++;
                    }
                }
                averageResponseTimesAndThroughput.Columns.Add("Throughput");
                averageResponseTimesAndThroughput.Columns.Add("Errors");

                for (int offset = 0; offset < averageUserActions.Rows.Count; offset += range) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    var row = new List<object>(range + 3);
                    row.Add(averageUserActions.Rows[offset].ItemArray[0]); //Add stresstest
                    row.Add(averageUserActions.Rows[offset].ItemArray[1]); //Add concurrency
                    for (int i = offset; i != offset + range; i++) { //Add the avg response times
                        if (cancellationToken.IsCancellationRequested) return null;

                        row.Add(i < averageUserActions.Rows.Count ? averageUserActions.Rows[i].ItemArray[3] : 0d);
                    }
                    row.Add(averageConcurrentUsers.Rows[averageResponseTimesAndThroughput.Rows.Count].ItemArray[6]); //And the throughput
                    row.Add(averageConcurrentUsers.Rows[averageResponseTimesAndThroughput.Rows.Count].ItemArray[12]); //And the errors: Bonus
                    averageResponseTimesAndThroughput.Rows.Add(row.ToArray());
                }


                return averageResponseTimesAndThroughput;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">Used in await Tast.Run...</param>
        /// <param name="stresstestIds">If none, all the results for all tests will be returned.</param>
        /// <returns></returns>
        public DataTable GetMachineConfigurations(CancellationToken cancellationToken, params int[] stresstestIds) {
            lock (_lock) {
                if (_databaseActions != null) {
                    var stresstests = ReaderAndCombiner.GetStresstests(cancellationToken, _databaseActions, stresstestIds, "Id", "Stresstest", "Connection");
                    if (stresstests == null || stresstests.Rows.Count == 0) return null;

                    var machineConfigurations = CreateEmptyDataTable("MachineConfigurations", "Stresstest", "Monitor", "Monitor Source", "Machine Configuration");
                    foreach (DataRow stresstestsRow in stresstests.Rows) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        int stresstestId = (int)stresstestsRow.ItemArray[0];
                        string stresstest = string.Format("{0} {1}", stresstestsRow.ItemArray[1], stresstestsRow.ItemArray[2]);

                        var monitors = ReaderAndCombiner.GetMonitors(cancellationToken, _databaseActions, stresstestId, "Monitor", "MonitorSource", "MachineConfiguration");
                        if (monitors == null || monitors.Rows.Count == 0) continue;

                        foreach (DataRow monitorRow in monitors.Rows) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            machineConfigurations.Rows.Add(stresstest, monitorRow.ItemArray[0], monitorRow.ItemArray[1], monitorRow.ItemArray[2]);
                        }
                    }

                    return machineConfigurations;
                }
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">Used in await Tast.Run...</param>
        /// <param name="stresstestIds">If none, all the results for all tests will be returned.</param>
        /// <returns></returns>
        public DataTable GetAverageMonitorResults(CancellationToken cancellationToken, params int[] stresstestIds) {
            lock (_lock) {
                if (_databaseActions == null) return null;
                var stresstests = ReaderAndCombiner.GetStresstests(cancellationToken, _databaseActions, stresstestIds, "Id", "Stresstest", "Connection", "MonitorBeforeInMinutes", "MonitorAfterInMinutes");
                if (stresstests == null || stresstests.Rows.Count == 0) return null;

                //var averageMonitorResults = CreateEmptyDataTable("AverageMonitorResults", "Stresstest", "Monitor", "Started At", "Measured Time (ms)", "Concurrency", "Headers", "Values");

                //Get the monitors + values
                var monitors = ReaderAndCombiner.GetMonitors(cancellationToken, _databaseActions, stresstestIds, "Id", "StresstestId", "Monitor", "ResultHeaders");
                if (monitors == null || monitors.Rows.Count == 0) return null;

                var columnNames = new List<string>(new string[] { "Monitor", "Started At", "Measured Time (ms)", "Concurrency" });
                var resultHeaders = new List<string>();
                var monitorColumnOffsets = new Dictionary<int, int>(); //key monitorID, value offset

                foreach (DataRow monitorRow in monitors.Rows) {
                    int monitorId = (int)monitorRow.ItemArray[0];
                    var rh = (monitorRow[3] as string).Split(new string[] { "; " }, StringSplitOptions.None);

                    monitorColumnOffsets.Add(monitorId, resultHeaders.Count);
                    resultHeaders.AddRange(rh);
                }

                columnNames.AddRange(resultHeaders);
                var averageMonitorResults = CreateEmptyDataTable("AverageMonitorResults", "Stresstest", columnNames.ToArray());

                foreach (DataRow stresstestsRow in stresstests.Rows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    int stresstestId = (int)stresstestsRow.ItemArray[0];
                    string stresstest = string.Format("{0} {1}", stresstestsRow.ItemArray[1], stresstestsRow.ItemArray[2]);
                    int monitorBeforeInMinutes = (int)stresstestsRow.ItemArray[3];
                    int monitorAfterInMinutes = (int)stresstestsRow.ItemArray[4];

                    var stresstestResults = ReaderAndCombiner.GetStresstestResults(cancellationToken, _databaseActions, stresstestId, "Id");
                    if (stresstestResults == null || stresstestResults.Rows.Count == 0) continue;
                    int stresstestResultId = (int)stresstestResults.Rows[0].ItemArray[0];

                    //Get the timestamps to calculate the averages
                    var concurrencyResults = ReaderAndCombiner.GetConcurrencyResults(cancellationToken, _databaseActions, stresstestResultId, "Id", "Concurrency", "StartedAt", "StoppedAt");
                    if (concurrencyResults == null || concurrencyResults.Rows.Count == 0) continue;

                    var concurrencyDelimiters = new Dictionary<int, KeyValuePair<DateTime, DateTime>>(concurrencyResults.Rows.Count);
                    var runDelimiters = new Dictionary<int, Dictionary<DateTime, DateTime>>(concurrencyResults.Rows.Count);
                    var concurrencies = new Dictionary<int, int>();
                    foreach (DataRow crRow in concurrencyResults.Rows) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        int concurrencyResultId = (int)crRow.ItemArray[0];
                        int concurrency = (int)crRow.ItemArray[1];
                        concurrencyDelimiters.Add(concurrencyResultId, new KeyValuePair<DateTime, DateTime>((DateTime)crRow.ItemArray[2], (DateTime)crRow.ItemArray[3]));

                        var runResults = ReaderAndCombiner.GetRunResults(cancellationToken, _databaseActions, concurrencyResultId, "StartedAt", "StoppedAt");
                        if (runResults == null || runResults.Rows.Count == 0) continue;

                        var d = new Dictionary<DateTime, DateTime>(runResults.Rows.Count);
                        foreach (DataRow rrRow in runResults.Rows) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            var start = (DateTime)rrRow.ItemArray[0];
                            if (!d.ContainsKey(start)) d.Add(start, (DateTime)rrRow.ItemArray[1]);
                        }
                        runDelimiters.Add(concurrencyResultId, d);
                        concurrencies.Add(concurrencyResultId, concurrency);
                    }

                    //Make a bogus run and concurrency to be able to calcullate averages for monitor before and after.
                    if (monitorAfterInMinutes != 0 && runDelimiters.Count != 0) {
                        int lastConcurrencyID = 0;
                        foreach (int concId in runDelimiters.Keys)
                            lastConcurrencyID = concId;

                        var lastConcurrency = runDelimiters[lastConcurrencyID];

                        int i = 0;
                        foreach (var lastStop in lastConcurrency.Values)
                            if (i++ == lastConcurrency.Count - 1) {
                                var bogusStart = lastStop.AddMilliseconds(1);
                                var bogusStop = bogusStart.AddMinutes(monitorAfterInMinutes);
                                var monitorAfterBogusRuns = new Dictionary<DateTime, DateTime>(0);
                                monitorAfterBogusRuns.Add(bogusStart, bogusStop);
                                int concurrencyId = lastConcurrencyID + 1;
                                runDelimiters.Add(concurrencyId, monitorAfterBogusRuns);
                                concurrencyDelimiters.Add(concurrencyId, new KeyValuePair<DateTime, DateTime>(bogusStart, bogusStop));
                                concurrencies.Add(concurrencyId, 0);
                            }
                    }
                    if (monitorBeforeInMinutes != 0 && runDelimiters.Count != 0) {
                        var newRunDelimiters = new Dictionary<int, Dictionary<DateTime, DateTime>>(concurrencyResults.Rows.Count);
                        var newConcurrencyDelimiters = new Dictionary<int, KeyValuePair<DateTime, DateTime>>(concurrencyResults.Rows.Count);
                        var newConcurrencies = new Dictionary<int, int>();

                        var firstConcurrency = runDelimiters[1];
                        foreach (var firstStart in firstConcurrency.Keys) {
                            var bogusStop = firstStart.Subtract(new TimeSpan(TimeSpan.TicksPerMillisecond));
                            var bogusStart = bogusStop.Subtract(new TimeSpan(monitorAfterInMinutes * TimeSpan.TicksPerMinute));
                            var monitorBeforeBogusRuns = new Dictionary<DateTime, DateTime>(0);
                            monitorBeforeBogusRuns.Add(bogusStart, bogusStop);
                            newRunDelimiters.Add(0, monitorBeforeBogusRuns);
                            newConcurrencyDelimiters.Add(0, new KeyValuePair<DateTime, DateTime>(bogusStart, bogusStop));
                            newConcurrencies.Add(0, 0);
                            break;
                        }
                        foreach (var concurrency in runDelimiters)
                            newRunDelimiters.Add(concurrency.Key, concurrency.Value);
                        foreach (var concurrency in concurrencyDelimiters)
                            newConcurrencyDelimiters.Add(concurrency.Key, concurrency.Value);
                        foreach (var concurrency in concurrencies)
                            newConcurrencies.Add(concurrency.Key, concurrency.Value);

                        runDelimiters = newRunDelimiters;
                        concurrencyDelimiters = newConcurrencyDelimiters;
                        concurrencies = newConcurrencies;
                    }

                    //Calcullate the averages
                    foreach (int concurrencyId in runDelimiters.Keys) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        var delimiterValues = runDelimiters[concurrencyId];
                        foreach (DataRow monitorRow in monitors.Rows) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            int monitorStresstestId = (int)monitorRow.ItemArray[1];
                            if (monitorStresstestId != stresstestId) continue;

                            int monitorId = (int)monitorRow.ItemArray[0];
                            object monitor = monitorRow.ItemArray[2];

                            var monitorResults = ReaderAndCombiner.GetMonitorResults(_databaseActions, monitorId, "TimeStamp", "Value");
                            if (monitorResults == null || monitorResults.Rows.Count == 0) continue;

                            var monitorValues = new List<KeyValuePair<DateTime, float[]>>(monitorResults.Rows.Count);
                            foreach (DataRow monitorResultsRow in monitorResults.Rows) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                var timeStamp = (DateTime)monitorResultsRow[0];

                                bool canAdd = false;
                                foreach (var start in delimiterValues.Keys) {
                                    if (cancellationToken.IsCancellationRequested) return null;

                                    if (timeStamp >= start && timeStamp <= delimiterValues[start]) {
                                        canAdd = true;
                                        break;
                                    }
                                }

                                if (canAdd) {
                                    string[] splittedValue = (monitorResultsRow[1] as string).Split(new string[] { "; " }, StringSplitOptions.None);
                                    float[] values = new float[splittedValue.Length];

                                    for (long l = 0; l != splittedValue.LongLength; l++) {
                                        if (cancellationToken.IsCancellationRequested) return null;

                                        values[l] = float.Parse(splittedValue[l].Trim());
                                    }
                                    monitorValues.Add(new KeyValuePair<DateTime, float[]>(timeStamp, values));
                                }
                            }

                            float[] averages = GetAverageMonitorResults(cancellationToken, monitorValues);
                            if (cancellationToken.IsCancellationRequested) return null;

                            var startedAt = concurrencyDelimiters[concurrencyId].Key;
                            var measuredRunTime = Math.Round((concurrencyDelimiters[concurrencyId].Value - startedAt).TotalMilliseconds, 2);

                            string concurrency = concurrencies[concurrencyId] == 0 ? "--" : concurrencies[concurrencyId].ToString();

                            var newRow = new List<object>(new object[] { stresstest, monitor, startedAt, measuredRunTime, concurrency });

                            var fragmentedAverages = new object[resultHeaders.Count];
                            for (long p = 0; p != fragmentedAverages.Length; p++)
                                fragmentedAverages[p] = "--";

                            int offset = monitorColumnOffsets[monitorId];
                            averages.CopyTo(fragmentedAverages, offset);

                            newRow.AddRange(fragmentedAverages);
                            averageMonitorResults.Rows.Add(newRow.ToArray());
                        }
                    }
                }

                return averageMonitorResults;
            }
        }
        /// <summary>
        /// From a 2 dimensional collection to an array of floats.
        /// </summary>
        /// <param name="cancellationToken">Used in await Tast.Run...</param>
        /// <param name="monitorValues"></param>        /// <returns></returns>
        private float[] GetAverageMonitorResults(CancellationToken cancellationToken, List<KeyValuePair<DateTime, float[]>> monitorValues) {
            var averageMonitorResults = new float[0];
            if (monitorValues.Count != 0) {
                //Average divider
                int valueCount = monitorValues.Count;
                averageMonitorResults = new float[valueCount];

                foreach (var kvp in monitorValues) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    var timeStamp = kvp.Key;
                    var floats = kvp.Value;

                    // The averages length must be the same as the floats length.
                    if (averageMonitorResults.Length != floats.Length) averageMonitorResults = new float[floats.Length];

                    for (long l = 0; l != floats.LongLength; l++) {
                        if (cancellationToken.IsCancellationRequested) return null;

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">Used in await Tast.Run...</param>
        /// <param name="stresstestIds">If none, all the results for all tests will be returned.</param>
        /// <returns></returns>
        public List<DataTable> GetMonitorResults(CancellationToken cancellationToken, params int[] stresstestIds) {
            lock (_lock) {
                if (_databaseActions == null) return null;

                var stresstests = ReaderAndCombiner.GetStresstests(cancellationToken, _databaseActions, stresstestIds, "Id", "Stresstest", "Connection");
                if (stresstests == null || stresstests.Rows.Count == 0) return null;

                var dts = new List<DataTable>();

                foreach (DataRow stresstestsRow in stresstests.Rows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    int stresstestId = (int)stresstestsRow.ItemArray[0];
                    string stresstest = string.Format("{0} {1}", stresstestsRow.ItemArray[1], stresstestsRow.ItemArray[2]);

                    var stresstestResults = ReaderAndCombiner.GetStresstestResults(cancellationToken, _databaseActions, stresstestId, "Id");

                    if (stresstestResults.Rows.Count == 0) continue;
                    object stresstestResultId = stresstestResults.Rows[0].ItemArray[0];

                    //Get the monitors + values
                    var monitors = ReaderAndCombiner.GetMonitors(cancellationToken, _databaseActions, stresstestId, "Id", "Monitor", "ResultHeaders");

                    if (monitors == null || monitors.Rows.Count == 0) continue;

                    foreach (DataRow monitorRow in monitors.Rows) {
                        int monitorId = (int)monitorRow.ItemArray[0];
                        object monitor = monitorRow.ItemArray[1];
                        string[] headers = (monitorRow.ItemArray[2] as string).Split(new string[] { "; " }, StringSplitOptions.None);

                        var columns = new List<string>();
                        columns.AddRange("Monitor", "Timestamp");

                        int headerIndex = 1;
                        foreach (string header in headers)
                            columns.Add((headerIndex++) + ") " + header);

                        var monitorResults = CreateEmptyDataTable("MonitorResults", "Stresstest", columns.ToArray());

                        var mrs = ReaderAndCombiner.GetMonitorResults(_databaseActions, monitorId, "TimeStamp", "Value");

                        var monitorValues = new Dictionary<DateTime, float[]>(mrs.Rows.Count);
                        foreach (DataRow monitorResultsRow in mrs.Rows) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            string[] values = (monitorResultsRow.ItemArray[1] as string).Split(new string[] { "; " }, StringSplitOptions.None);

                            var row = new List<object>();
                            row.Add(stresstest);
                            row.Add(monitor);
                            row.Add(monitorResultsRow.ItemArray[0]);
                            row.AddRange(values);

                            monitorResults.Rows.Add(row.ToArray());
                        }

                        dts.Add(monitorResults);
                    }
                }
                return dts;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="concurrencyAndRunsDic">To link the run index to the correct run. stresstest as key, List<concurrency.run> as value</param>
        /// <param name="stresstestIds"></param>
        /// <returns></returns>
        public DataTable GetRunsOverTime(CancellationToken cancellationToken, out Dictionary<string, List<string>> concurrencyAndRunsDic, params int[] stresstestIds) {
            lock (_lock) {
                concurrencyAndRunsDic = new Dictionary<string, List<string>>();

                if (_databaseActions == null) return null;

                var stresstests = ReaderAndCombiner.GetStresstests(cancellationToken, _databaseActions, stresstestIds, "Id", "Stresstest", "Connection");
                if (stresstests == null || stresstests.Rows.Count == 0) return null;

                DataTable runsOverTime = CreateEmptyDataTable("RunsOverTime", "Stresstest");

                var rows = new List<List<object>>();
                int longestRowCount = 0;
                foreach (DataRow stresstestsRow in stresstests.Rows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    int stresstestId = (int)stresstestsRow.ItemArray[0];
                    string stresstest = string.Format("{0} {1}", stresstestsRow.ItemArray[1], stresstestsRow.ItemArray[2]);

                    //To link the run index to the correct run
                    concurrencyAndRunsDic.Add(stresstest, new List<string>());

                    var row = new List<object>();
                    row.Add(stresstest);
                    var stoppedAts = new List<DateTime>();

                    var stresstestResults = ReaderAndCombiner.GetStresstestResults(cancellationToken, _databaseActions, stresstestId, "Id");

                    if (stresstestResults.Rows.Count == 0) continue;
                    int stresstestResultId = (int)stresstestResults.Rows[0].ItemArray[0];

                    var concurrencyResults = ReaderAndCombiner.GetConcurrencyResults(cancellationToken, _databaseActions, stresstestResultId, new string[] { "Id", "Concurrency" });
                    if (concurrencyResults == null || concurrencyResults.Rows.Count == 0) continue;

                    foreach (DataRow crRow in concurrencyResults.Rows) {
                        if (cancellationToken.IsCancellationRequested) return null;
                        int concurrencyResultId = (int)crRow.ItemArray[0];
                        int concurrency = (int)crRow.ItemArray[1];

                        var runResults = ReaderAndCombiner.GetRunResults(cancellationToken, _databaseActions, concurrencyResultId, "Run", "StartedAt", "StoppedAt");
                        if (runResults == null || runResults.Rows.Count == 0) continue;

                        foreach (DataRow rrRow in runResults.Rows) {
                            if (cancellationToken.IsCancellationRequested) return null;
                            int run = (int)rrRow.ItemArray[0];
                            var startedAt = (DateTime)rrRow.ItemArray[1];
                            var stoppedAt = (DateTime)rrRow.ItemArray[2];

                            if (stoppedAts.Count != 0) row.Add(startedAt - stoppedAts[stoppedAts.Count - 1]); //Add gap (test init time and write db results time)
                            stoppedAts.Add(stoppedAt);

                            row.Add(stoppedAt - startedAt); //run time

                            concurrencyAndRunsDic[stresstest].Add(concurrency + "." + run);
                        }
                    }

                    if (row.Count > longestRowCount) longestRowCount = row.Count;
                    rows.Add(row);
                }

                //Add run time and gap columns.
                float longestRowCountMod = ((float)longestRowCount / 2) + 0.5f;
                for (float f = 1f; f != longestRowCountMod; f += 0.5f) {
                    if (cancellationToken.IsCancellationRequested) return null;
                    int i = (int)f;
                    runsOverTime.Columns.Add((f - i == 0.5) ? "Init time" + i : i.ToString());
                }

                //Add the rows.
                foreach (var row in rows) {
                    if (cancellationToken.IsCancellationRequested) return null;
                    var newRow = new object[longestRowCount];
                    row.ToArray().CopyTo(newRow, 0);
                    runsOverTime.Rows.Add(newRow);
                }

                return runsOverTime;
            }
        }
        #endregion

        #endregion

        #region Other
        /// <summary>
        /// Connect to an existing database to execute the procedures on or add slave side data to it.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="databaseName"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public void ConnectToExistingDatabase(string host, int port, string databaseName, string user, string password) {
            lock (_lock) {
                try {
                    _databaseActions = new DatabaseActions() { ConnectionString = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};Pooling=True;UseCompression=True;", host, port, databaseName, user, password) };
                    if (_databaseActions.GetDataTable("Show databases").Rows.Count == 0) throw new Exception("A connection to the results server could not be made!");
                    _databaseName = databaseName;
                } catch {
                    KillConnection();
                    throw;
                }
            }
        }
        /// <summary>
        /// Last resort.
        /// </summary>
        public void KillConnection() {
            if (_databaseActions != null) {
                try { _databaseActions.ReleaseConnection(); } catch { }
                _databaseActions = null;
            }
        }

        /// <summary>
        /// Get all the stresstests: ID, Stresstest, Connection
        /// If the workload was divided over multiple slaves the datatable entries will be combined, in that case the first Id will be put in a row.
        /// </summary>
        /// <returns></returns>
        public DataTable GetStresstests() {
            lock (_lock) {
                if (_databaseActions != null)
                    return ReaderAndCombiner.GetStresstests(new CancellationToken(), _databaseActions, "Id", "Stresstest", "Connection");

                return null;
            }
        }
        /// <summary>
        /// Execute a query on the connected database.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable ExecuteQuery(string query) {
            lock (_lock) {
                if (_databaseActions == null) return null;
                return _databaseActions.GetDataTable(query);
            }
        }

        /// <summary>
        /// Deletes the current database.
        /// </summary>
        public void DeleteResults() { DeleteResults(_databaseName); }
        public void DeleteResults(string databaseName) {
            lock (_lock)
                try {
                    Schema.Drop(databaseName, _databaseActions);
                    _databaseName = null;
                } catch {
                }
        }

        private DataTable CreateEmptyDataTable(string name, string columnName1, params string[] columnNames) {
            var objectType = typeof(object);
            var dataTable = new DataTable(name);
            dataTable.Columns.Add(columnName1);
            foreach (string columnName in columnNames) dataTable.Columns.Add(columnName, objectType);
            return dataTable;
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
        #endregion

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