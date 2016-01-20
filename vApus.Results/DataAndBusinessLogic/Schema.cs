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

            CreateStressTestsTable(databaseActions);
            CreateMessagesTable(databaseActions);
            CreateStressTestResultsTable(databaseActions);
            CreateConcurrencyResultsTable(databaseActions);
            CreateRunResultsTable(databaseActions);
            CreateRequestResultsTable(databaseActions);

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
            if (!TableExists("description", databaseActions))
                databaseActions.ExecuteSQL("Create Table description(Id int NOT NULL AUTO_INCREMENT, PRIMARY KEY(Id), Description longtext NOT NULL) ROW_FORMAT=COMPRESSED");
        }

        private static void CreateTagsTable(DatabaseActions databaseActions) {
            if (!TableExists("tags", databaseActions))
                databaseActions.ExecuteSQL("Create Table tags(Tag varchar(255) NOT NULL UNIQUE) ROW_FORMAT=COMPRESSED");
        }

        private static void CreatevApusInstancesTable(DatabaseActions databaseActions) {
            if (!TableExists("vapusinstances", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table vapusinstances(Id int NOT NULL AUTO_INCREMENT, PRIMARY KEY(Id),
HostName varchar(255) NOT NULL, IP varchar(255) NOT NULL, Port int NOT NULL, Version varchar(255) NOT NULL, Channel varchar(7) NOT NULL,
IsMaster bool NOT NULL) ROW_FORMAT=COMPRESSED");
        }

        private static void CreateMessagesTable(DatabaseActions databaseActions) {
            if (!TableExists("messages", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table messages(Id int NOT NULL AUTO_INCREMENT, PRIMARY KEY(ID), vApusInstanceId int NOT NULL,
FOREIGN KEY(vApusInstanceId) REFERENCES vapusinstances(Id), Timestamp datetime(6) NOT NULL, Level tinyint NOT NULL, Message longtext NOT NULL) ROW_FORMAT=COMPRESSED"
                    );
        }

        private static void CreateStressTestsTable(DatabaseActions databaseActions) {
            if (!TableExists("stresstests", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table stresstests(Id int NOT NULL AUTO_INCREMENT, PRIMARY KEY(Id), vApusInstanceId int NOT NULL, 
FOREIGN KEY(vApusInstanceId) REFERENCES vapusinstances(Id), StressTest varchar(255) NOT NULL, RunSynchronization varchar(20) NOT NULL, Connection varchar(255) NOT NULL,
ConnectionProxy varchar(255) NOT NULL, ConnectionString longtext NOT NULL, Scenarios longtext NOT NULL, ScenarioRuleSet varchar(255) NOT NULL, Concurrencies longtext NOT NULL,
Runs int NOT NULL, InitialMinimumDelayInMilliseconds int NOT NULL, InitialMaximumDelayInMilliseconds int NOT NULL, MinimumDelayInMilliseconds int NOT NULL, MaximumDelayInMilliseconds int NOT NULL, Shuffle bool NOT NULL, ActionDistribution bool NOT NULL, MaximumNumberOfUserActions int NOT NULL,
MonitorBeforeInMinutes int NOT NULL, MonitorAfterInMinutes int NOT NULL, UseParallelExecutionOfRequests bool NOT NULL, MaximumPersistentConnections int NOT NULL, PersistentConnectionsPerHostname int NOT NULL) ROW_FORMAT=COMPRESSED");
        }

        private static void CreateStressTestResultsTable(DatabaseActions databaseActions) {
            if (!TableExists("stresstestresults", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table stresstestresults(Id int NOT NULL AUTO_INCREMENT, PRIMARY KEY(Id), StressTestId int NOT NULL,
FOREIGN KEY(StressTestId) REFERENCES stresstests(Id), StartedAt datetime(6) NOT NULL, StoppedAt datetime(6) NOT NULL, Status varchar(9) NOT NULL, StatusMessage longtext NOT NULL) ROW_FORMAT=COMPRESSED");
        }

        private static void CreateConcurrencyResultsTable(DatabaseActions databaseActions) {
            if (!TableExists("concurrencyresults", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table concurrencyresults(Id int NOT NULL AUTO_INCREMENT, PRIMARY KEY (Id), StressTestResultId int NOT NULL,
FOREIGN KEY(StressTestResultID) REFERENCES stresstestresults(Id), Concurrency int NOT NULL, StartedAt datetime(6) NOT NULL, StoppedAt datetime(6) NOT NULL) ROW_FORMAT=COMPRESSED");
        }

        private static void CreateRunResultsTable(DatabaseActions databaseActions) {
            if (!TableExists("runresults", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table runresults(Id int NOT NULL AUTO_INCREMENT, PRIMARY KEY(Id), ConcurrencyResultId int NOT NULL,
FOREIGN KEY(ConcurrencyResultId) REFERENCES concurrencyresults(Id), Run int NOT NULL, TotalRequestCount bigint UNSIGNED NOT NULL, RerunCount int NOT NULL, StartedAt datetime(6) NOT NULL, StoppedAt datetime(6) NOT NULL) ROW_FORMAT=COMPRESSED");
        }

        private static void CreateRequestResultsTable(DatabaseActions databaseActions) {
            if (!TableExists("requestresults", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table requestresults(Id serial, PRIMARY KEY(Id), RunResultId int NOT NULL, 
FOREIGN KEY(RunResultId) REFERENCES runresults(Id),VirtualUser varchar(255) NOT NULL, UserAction longtext NOT NULL, RequestIndex varchar(255) NOT NULL, SameAsRequestIndex varchar(255) NOT NULL, Request longtext NOT NULL,
InParallelWithPrevious bool NOT NULL, SentAt datetime(6) NOT NULL, TimeToLastByteInTicks bigint NOT NULL, Meta longtext NOT NULL, DelayInMilliseconds int NOT NULL, Error longtext NOT NULL, Rerun int NOT NULL) ROW_FORMAT=COMPRESSED");
        }

        private static void CreateMonitorsTable(DatabaseActions databaseActions) {
            if (!TableExists("monitors", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table monitors (Id int NOT NULL AUTO_INCREMENT, PRIMARY KEY(Id), StressTestId int NOT NULL,
FOREIGN KEY(StressTestId) REFERENCES stresstests(Id),Monitor varchar(255) NOT NULL, MonitorSource varchar(255) NOT NULL, ConnectionString longtext NOT NULL, MachineConfiguration longtext NOT NULL, 
ResultHeaders longtext NOT NULL) ROW_FORMAT=COMPRESSED");
        }

        private static void CreateMonitorResultsTable(DatabaseActions databaseActions) {
            if (!TableExists("monitorresults", databaseActions))
                databaseActions.ExecuteSQL(
                    @"Create Table monitorresults(Id serial, PRIMARY KEY(Id), MonitorId int NOT NULL, FOREIGN KEY(MonitorId) REFERENCES monitors(Id),
TimeStamp datetime(6) NOT NULL, Value longtext NOT NULL) ROW_FORMAT=COMPRESSED");
        }

        private static DatabaseActions GetDatabaseActions() {
            string connectionString = ConnectionStringManager.GetCurrentConnectionString();

            if (string.IsNullOrEmpty(connectionString)) throw new Exception("No MySQL connection was set.");

            return new DatabaseActions { ConnectionString = connectionString };
        }

        internal static DatabaseActions GetDatabaseActionsUsingDatabase(string databaseName) {
            string connectionString = ConnectionStringManager.GetCurrentConnectionString(databaseName);

            if (string.IsNullOrEmpty(connectionString)) throw new Exception("No MySQL connection was set.");

            var databaseActions = new DatabaseActions { ConnectionString = connectionString };

            return databaseActions;
        }

        private static void ReleaseConnection(DatabaseActions databaseActions) {
            try {
                databaseActions.ReleaseConnection();
            } catch {
                //Ignore.
            }
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