/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Data;
using vApus.Util;

namespace vApus.Results {
    /// <summary>
    /// Only used for building a new results database.
    /// </summary>
    internal static class Schema {
        /// <summary>
        /// </summary>
        /// <returns>The database name.</returns>
        internal static string Build() {
            DatabaseActions databaseActions = GetDatabaseActions();
            string databaseName = CreateDatabase(databaseActions);
            ReleaseConnection(databaseActions);

            databaseActions = GetDatabaseActionsUsingDatabase(databaseName);
            CreateDescriptionTable(databaseActions);
            CreateTagsTable(databaseActions);

            CreatevApusInstancesTable(databaseActions);

            CreateStresstestsTable(databaseActions);
            CreateStresstestResultsTable(databaseActions);
            CreateConcurrencyResultsTable(databaseActions);
            CreateRunResultsTable(databaseActions);
            CreateLogEntryResultsTable(databaseActions);

            CreateMonitorsTable(databaseActions);
            CreateMonitorResultsTable(databaseActions);
            ReleaseConnection(databaseActions);

            return databaseName;
        }

        /// <summary>
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <returns>Database name</returns>
        private static string CreateDatabase(DatabaseActions databaseActions) {
            string databaseName = "vapus" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fffffff");// DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss_fffffff");
            databaseActions.ExecuteSQL("Create Database " + databaseName);
            return databaseName;
        }

        private static void CreateDescriptionTable(DatabaseActions databaseActions) {
            if (!TableExists("Description", databaseActions))
                databaseActions.ExecuteSQL("Create Table Description(Description longtext NOT NULL)");
        }

        private static void CreateTagsTable(DatabaseActions databaseActions) {
            if (!TableExists("Tags", databaseActions))
                databaseActions.ExecuteSQL("Create Table Tags(Tag varchar(255) NOT NULL UNIQUE)");
        }

        private static void CreatevApusInstancesTable(DatabaseActions databaseActions) {
            if (!TableExists("vApusInstances", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table vApusInstances(Id int NOT NULL AUTO_INCREMENT, PRIMARY KEY(Id),
HostName varchar(255) NOT NULL, IP varchar(255) NOT NULL, Port int NOT NULL, Version varchar(255) NOT NULL, Channel varchar(7) NOT NULL,
IsMaster bool NOT NULL)");
        }

        private static void CreateStresstestsTable(DatabaseActions databaseActions) {
            if (!TableExists("Stresstests", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table Stresstests(Id int NOT NULL AUTO_INCREMENT, PRIMARY KEY(Id), vApusInstanceId int NOT NULL, 
FOREIGN KEY(vApusInstanceId) REFERENCES vApusInstances(Id), Stresstest varchar(255) NOT NULL, RunSynchronization varchar(20) NOT NULL, Connection varchar(255) NOT NULL,
ConnectionProxy varchar(255) NOT NULL, ConnectionString longtext NOT NULL, Log varchar(255) NOT NULL, LogRuleSet varchar(255) NOT NULL, Concurrencies longtext NOT NULL,
Runs int NOT NULL, MinimumDelayInMilliseconds int NOT NULL, MaximumDelayInMilliseconds int NOT NULL, Shuffle bool NOT NULL, Distribute char(4) NOT NULL, MonitorBeforeInMinutes int NOT NULL, MonitorAfterInMinutes int NOT NULL)");
        }

        private static void CreateStresstestResultsTable(DatabaseActions databaseActions) {
            if (!TableExists("StresstestResults", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table StresstestResults(Id int NOT NULL AUTO_INCREMENT, PRIMARY KEY(Id), StresstestId int NOT NULL,
FOREIGN KEY(StresstestId) REFERENCES Stresstests(Id), StartedAt datetime(6) NOT NULL, StoppedAt datetime(6) NOT NULL, Status varchar(9) NOT NULL, StatusMessage longtext NOT NULL)");
        }

        private static void CreateConcurrencyResultsTable(DatabaseActions databaseActions) {
            if (!TableExists("ConcurrencyResults", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table ConcurrencyResults(Id int NOT NULL AUTO_INCREMENT, PRIMARY KEY (Id), StresstestResultId int NOT NULL,
FOREIGN KEY(StresstestResultID) REFERENCES StresstestResults(Id), Concurrency int NOT NULL, StartedAt datetime(6) NOT NULL, StoppedAt datetime(6) NOT NULL)");
        }

        private static void CreateRunResultsTable(DatabaseActions databaseActions) {
            if (!TableExists("RunResults", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table RunResults(Id int NOT NULL AUTO_INCREMENT, PRIMARY KEY(Id), ConcurrencyResultId int NOT NULL,
FOREIGN KEY(ConcurrencyResultId) REFERENCES ConcurrencyResults(Id), Run int NOT NULL, TotalLogEntryCount bigint UNSIGNED NOT NULL, RerunCount int NOT NULL, StartedAt datetime(6) NOT NULL, StoppedAt datetime(6) NOT NULL)");
        }

        private static void CreateLogEntryResultsTable(DatabaseActions databaseActions) {
            if (!TableExists("LogEntryResults", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table LogEntryResults(Id serial, PRIMARY KEY(Id), RunResultId int NOT NULL, 
FOREIGN KEY(RunResultId) REFERENCES RunResults(Id),VirtualUser varchar(255) NOT NULL, UserAction longtext NOT NULL, LogEntryIndex varchar(255) NOT NULL, SameAsLogEntryIndex varchar(255) NOT NULL, LogEntry longtext NOT NULL,
SentAt datetime(6) NOT NULL, TimeToLastByteInTicks bigint NOT NULL, DelayInMilliseconds int NOT NULL, Error longtext NOT NULL, Rerun int NOT NULL)");
        }

        private static void CreateMonitorsTable(DatabaseActions databaseActions) {
            if (!TableExists("Monitors", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table Monitors (Id int NOT NULL AUTO_INCREMENT, PRIMARY KEY(Id), StresstestId int NOT NULL,
FOREIGN KEY(StresstestId) REFERENCES Stresstests(Id),Monitor varchar(255) NOT NULL, MonitorSource varchar(255) NOT NULL, ConnectionString longtext NOT NULL, MachineConfiguration longtext NOT NULL, 
ResultHeaders longtext NOT NULL)");
        }

        private static void CreateMonitorResultsTable(DatabaseActions databaseActions) {
            if (!TableExists("MonitorResults", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table MonitorResults(Id serial, PRIMARY KEY(Id), MonitorId int NOT NULL, FOREIGN KEY(MonitorId) REFERENCES Monitors(Id),
TimeStamp datetime(6) NOT NULL, Value longtext NOT NULL)");
        }

        private static DatabaseActions GetDatabaseActions() {
            string connectionString = ConnectionStringManager.GetCurrentConnectionString();

            if (string.IsNullOrEmpty(connectionString)) throw new Exception("No MySQL connection was set.");

            return new DatabaseActions { ConnectionString = connectionString };
        }

        internal static DatabaseActions GetDatabaseActionsUsingDatabase(string databaseName) {
            string connectionString = ConnectionStringManager.GetCurrentConnectionString(databaseName);

            if (string.IsNullOrEmpty(connectionString)) throw new Exception("No MySQL connection was set.");

            var databaseActions = new DatabaseActions {  ConnectionString = connectionString };

            return databaseActions;
        }

        private static void ReleaseConnection(DatabaseActions databaseActions) {
            try { databaseActions.ReleaseConnection(); } catch { }
            databaseActions = null;
        }

        private static bool TableExists(string tableName, DatabaseActions databaseActions) {
            tableName = tableName.ToLower();
            DataTable tables = databaseActions.GetDataTable("Show Tables");
            foreach (DataRow row in tables.Rows)
                if (row.ItemArray[0].ToString().ToLower() == tableName) return true;
            return false;
        }

        /// <summary>
        /// To remove a schema (after a test is cancelled or failed for instance)
        /// </summary>
        /// <param name="name"></param>
        internal static void Drop(string databaseName, DatabaseActions databaseActions) {
            databaseActions.ExecuteSQL("DROP SCHEMA " + databaseName);
        }
    }
}