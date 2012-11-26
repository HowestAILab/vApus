/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using vApus.Util;

namespace vApus.Results
{
    public static class ResultsHelper
    {
        #region Fields
        private static DatabaseActions _databaseActions;
        private static string _passwordGUID = "{51E6A7AC-06C2-466F-B7E8-4B0A00F6A21F}";
        private static byte[] _salt = { 0x49, 0x16, 0x49, 0x2e, 0x11, 0x1e, 0x45, 0x24, 0x86, 0x05, 0x01, 0x03, 0x62 };

        private static int _vApusInstanceId, _stresstestId, _stresstestResultId, _concurrencyResultId, _runResultId;
        #endregion

        #region Initialize database before stresstest
        /// <summary>
        /// Builds the schema if needed, if no db target is found or no connection could be made an exception is returned.
        /// </summary>
        /// <returns></returns>
        public static Exception BuildSchemaAndConnect()
        {
            try
            {
                string databaseName = Schema.Build();
                _databaseActions = Schema.GetDatabaseActionsUsingDatabase(databaseName);
            }
            catch (Exception ex)
            {
                if (_databaseActions != null)
                {
                    _databaseActions.ReleaseConnection();
                    _databaseActions = null;
                }
                return ex;
            }
            return null;
        }

        /// <summary>
        /// Only inserts if connected (Call BuildSchema).
        /// </summary>
        /// <param name="description"></param>
        /// <param name="tags"></param>
        public static void SetDescriptionAndTags(string description, string[] tags)
        {
            if (_databaseActions != null)
            {
                _databaseActions.ExecuteSQL("INSERT INTO Description(Description) VALUES('" + description + "')");
                foreach (string tag in tags)
                    _databaseActions.ExecuteSQL("INSERT INTO Tags(Tag) VALUES('" + tag + "')");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="version"></param>
        /// <param name="isMaster"></param>
        /// <returns>Id of the instance.</returns>
        public static void SetvApusInstance(string hostName, string ip, int port, string version, bool isMaster)
        {
            if (_databaseActions != null)
            {
                _databaseActions.ExecuteSQL(
                    string.Format("INSERT INTO vApusInstances(HostName, IP, Port, Version, IsMaster) VALUES('{0}', '{1}', '{2}', '{3}', '{4}')", hostName, ip, port, version, isMaster ? 1 : 0)
                                );
                _vApusInstanceId = GetLastInsertId();
            }
        }

        /// <summary>
        /// 
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
        public static void SetStresstest(string stresstest, string runSynchronization, string connection, string connectionProxy, string connectionString,
            string log, string logRuleSet, int[] concurrencies, int runs, int minimumDelayInMilliseconds, int maximumDelayInMilliseconds, bool shuffle, string distribute,
            int monitorBeforeInMinutes, int monitorAfterInMinutes)
        {
            if (_databaseActions != null)
            {
                _databaseActions.ExecuteSQL(
                                string.Format(@"INSERT INTO Stresstests(
vApusInstanceId, Stresstest, RunSynchronization, Connection, ConnectionProxy, ConnectionString, Log, LogRuleSet, Concurrencies, Runs,
MinimumDelayInMilliseconds, MaximumDelayInMilliseconds, Shuffle, Distribute, MonitorBeforeInMinutes, MonitorAfterInMinutes)
VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}')",
                _vApusInstanceId, stresstest, runSynchronization, connection, connectionProxy, connectionString.Encrypt(_passwordGUID, _salt), log, logRuleSet, concurrencies.Combine(", "), runs,
                minimumDelayInMilliseconds, maximumDelayInMilliseconds, shuffle ? 1 : 0, distribute, monitorBeforeInMinutes, monitorAfterInMinutes)
                                );
                _stresstestId = GetLastInsertId();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stresstestId"></param>
        /// <param name="monitor"></param>
        /// <param name="connectionString">Will be encrypted.</param>
        /// <param name="machineConfiguration"></param>
        /// <param name="resultHeaders"></param>
        /// <returns>The monitor configuration id in the database, set this in the proper monitor result cache.
        /// -1 if not connected.</returns>
        public static int SetMonitor(string monitor, string connectionString, string machineConfiguration, string[] resultHeaders)
        {
            if (_databaseActions != null)
            {
                _databaseActions.ExecuteSQL(
                                string.Format("INSERT INTO Monitors(StresstestId, Monitor, ConnectionString, MachineConfiguration, ResultHeaders) VALUES('{0}', '{1}', '{2}', '{3}', '{4}')",
                                _stresstestId, monitor, connectionString.Encrypt(_passwordGUID, _salt), machineConfiguration, resultHeaders.Combine("; ", string.Empty))
                                );
                return GetLastInsertId();
            }
            return -1;
        }
        #endregion

        #region Stresstest results
        /// <summary>
        /// Started at datetime now.
        /// </summary>
        /// <param name="stresstestResult"></param>
        public static void SetStresstestStarted(StresstestResult stresstestResult)
        {
            if (_databaseActions != null)
            {
                _databaseActions.ExecuteSQL(
                   string.Format("INSERT INTO StresstestResults(StresstestId, StartedAt, StoppedAt, Status, StatusMessage) VALUES('{0}', '{1}', '{2}', 'OK', '')",
                   _stresstestId, Parse(stresstestResult.StartedAt), Parse(DateTime.MinValue))
                    );
                _stresstestResultId = GetLastInsertId();
            }
        }
        /// <summary>
        /// Stopped at datetime now.
        /// </summary>
        /// <param name="stresstestResult"></param>
        /// <param name="status"></param>
        /// <param name="statusMessage"></param>
        public static void SetStresstestStopped(StresstestResult stresstestResult, string status = "OK", string statusMessage = "")
        {
            stresstestResult.StoppedAt = DateTime.Now;
            if (_databaseActions != null)
                _databaseActions.ExecuteSQL(
                    string.Format("UPDATE StresstestResults SET StoppedAt='{1}', Status='{2}', StatusMessage='{3}' WHERE Id='{0}'",
                    _stresstestResultId, Parse(stresstestResult.StoppedAt), status, statusMessage)
                );
        }
        #endregion

        #region Concurrency results
        /// <summary>
        /// Started at datetime now.
        /// </summary>
        /// <param name="stresstestResultId"></param>
        /// <param name="concurrencyResult"></param>
        public static void SetConcurrencyStarted(ConcurrencyResult concurrencyResult)
        {
            if (_databaseActions != null)
            {
                _databaseActions.ExecuteSQL(
                              string.Format("INSERT INTO ConcurrencyResults(StresstestResultId, ConcurrentUsers, StartedAt, StoppedAt) VALUES('{0}', '{1}', '{2}', '{3}')",
                              _stresstestResultId, concurrencyResult.ConcurrentUsers, Parse(concurrencyResult.StartedAt), Parse(DateTime.MinValue))
                               );
                _concurrencyResultId = GetLastInsertId();
            }
        }
        /// <summary>
        /// Stopped at datetime now.
        /// </summary>
        /// <param name="concurrencyResult"></param>
        public static void SetConcurrencyStopped(ConcurrencyResult concurrencyResult)
        {
            concurrencyResult.StoppedAt = DateTime.Now;
            if (_databaseActions != null)
                _databaseActions.ExecuteSQL(
                                string.Format("UPDATE ConcurrencyResults SET StoppedAt='{1}' WHERE Id='{0}'", _concurrencyResultId, Parse(concurrencyResult.StoppedAt))
                            );
        }
        #endregion

        #region Run results
        /// <summary>
        /// 
        /// </summary>
        /// <param name="concurrencyResultId"></param>
        /// <param name="reRunCount"></param>
        /// <param name="startedAt"></param>
        /// <param name="stoppedAt"></param>
        /// <returns>Id of the run result.</returns>
        public static void SetRunStarted(RunResult runResult)
        {
            if (_databaseActions != null)
            {
                _databaseActions.ExecuteSQL(
                               string.Format("INSERT INTO RunResults(ConcurrencyResultId, TotalLogEntryCount, ReRunCount, StartedAt, StoppedAt) VALUES('{0}', '0', '0', '{1}', '{2}')",
                               _concurrencyResultId, Parse(runResult.StartedAt), Parse(DateTime.MinValue))
                                );
                _runResultId = GetLastInsertId();
            }
        }
        /// <summary>
        /// Increase the rerun count value for the result using fx PrepareForRerun() before calling this fx.
        /// </summary>
        /// <param name="runResult"></param>
        public static void SetRerun(RunResult runResult)
        {
            if (_databaseActions != null)
                _databaseActions.ExecuteSQL(
                                string.Format("UPDATE RunResults SET ReRunCount='{1}' WHERE Id='{0}'", _runResultId, runResult.RerunCount)
                            );
        }
        /// <summary>
        /// All the log entry results are save to the database doing this, only do this for the curent run.
        /// </summary>
        /// <param name="runResult"></param>
        public static void SetRunStopped(RunResult runResult)
        {
            runResult.StoppedAt = DateTime.Now;
            if (_databaseActions != null)
            {
                ulong totalLogEntryCount = 0;
                foreach (var virtualUserResult in runResult.VirtualUserResults)
                {
                    totalLogEntryCount += (ulong)virtualUserResult.LogEntryResults.LongLength;
                    foreach (var logEntryResult in virtualUserResult.LogEntryResults)
                        //mssn de multiple insert approach gebruiken bvb insert into tbl(a) values(1),(2),(3) voegt 3 rijen toe.
                        //deze manier is volgens mysql de snelste.
                        if (logEntryResult != null)
                            _databaseActions.ExecuteSQL(
                                string.Format(@"INSERT INTO LogEntryResults(RunResultId, VirtualUser, UserAction, LogEntryIndex, LogEntry, SentAt, TimeToLastByteInTicks, DelayInMilliseconds, Exception)
VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')",
                                _runResultId, virtualUserResult.VirtualUser, logEntryResult.UserAction, logEntryResult.LogEntryIndex, logEntryResult.LogEntry, Parse(logEntryResult.SentAt),
                                logEntryResult.TimeToLastByteInTicks, logEntryResult.DelayInMilliseconds, logEntryResult.Exception)
                                );
                }

                _databaseActions.ExecuteSQL(
                        string.Format("UPDATE RunResults SET TotalLogEntryCount='{1}', StoppedAt='{2}' WHERE Id='{0}'", _runResultId, totalLogEntryCount, Parse(runResult.StoppedAt))
                    );
            }
        }
        #endregion

        #region Monitor results
        /// <summary>
        /// Do this at the end of the test.
        /// </summary>
        /// <param name="monitorResultCache">Should have a filled in monitor configuration id.</param>
        public static void SetMonitorResults(MonitorResultCache monitorResultCache)
        {
            if (_databaseActions != null)
                foreach (object[] row in monitorResultCache.Rows)
                {
                    var timeStamp = (DateTime)row[0];

                    var value = new List<float>();
                    for (int i = 1; i < row.Length; i++) value.Add((float)row[i]);

                    _databaseActions.ExecuteSQL(
                        string.Format("INSERT INTO MonitorResults(MonitorId, TimeStamp, Value) VALUES('{0}', '{1}', '{2}')",
                        monitorResultCache.MonitorConfigurationId, Parse(timeStamp), value.ToArray().Combine("; "))
                        );
                }
        }
        #endregion

        /// <summary>
        /// Works with a data reader, releases the connection.
        /// </summary>
        /// <returns></returns>
        private static int GetLastInsertId()
        {
            int id = 0;
            var dr = _databaseActions.GetDataReader("SELECT LAST_INSERT_ID()");
            while (dr.Read())
            {
                id = dr.GetInt32(0);
                break;
            }
            _databaseActions.ReleaseConnection();
            return id;
        }
        private static string Parse(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
        }
    }
}
