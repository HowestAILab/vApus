using System.Collections.Generic;
using System.Text;
using vApus.Publish;
using vApus.Results;
using vApus.Util;

namespace vApus.ResultsHandler {
    internal static class PublishItemHandler {
        private static DatabaseActions _databaseActions;
        private static string _databaseName;
        private static int _vApusInstanceId, _stressTestId;

        private static string _passwordGUID = "{51E6A7AC-06C2-466F-B7E8-4B0A00F6A21F}";

        private static readonly byte[] _salt = { 0x49, 0x16, 0x49, 0x2e, 0x11, 0x1e, 0x45, 0x24, 0x86, 0x05, 0x01, 0x03, 0x62 };

        public static void Handle(PublishItem item) {
            if (_databaseActions == null) {
                ConnectionStringManager.AddConnectionString("root", "127.0.0.1", 3306, "BDaEWS2015!");
                _databaseName = Schema.Build();
                _databaseActions = Schema.GetDatabaseActionsUsingDatabase(_databaseName);
            }

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
                case "MonitorHardwareConfiguration":
                    HandleMonitorHardwareConfiguration(item);
                    break;
                case "MonitorMetrics":
                    HandleMonitorMetrics(item);
                    break;
            }
        }
        private static void HandleDistributedTestConfiguration(PublishItem item) { }
        private static void HandleStressTestConfiguration(PublishItem item) {
            var pi = item as StressTestConfiguration;
            SetDescriptionAndTags(pi.Description, pi.Tags);
            SetStressTest(pi.PublishItemId, "None", pi.Connection, pi.ConnectionProxy, "", pi.ScenariosAndWeights.ToString(), pi.ScenarioRuleSet,
                pi.Concurrencies, pi.Runs, pi.InitialMinimumDelay, pi.InitialMaximumDelay, pi.MinimumDelayInMilliseconds, pi.MaximumDelayInMilliseconds, pi.Shuffle, pi.ActionDistribution, pi.MaximumNumberOfUserActions, 
                pi.MonitorBeforeInSeconds / 60, pi.MonitorAfterInSeconds / 60, pi.UseParallelExecutionOfRequests, 0, 0);

        }
        private static void HandleTileStressTestConfiguration(PublishItem item) { }
        private static void HandleFastConcurrencyResults(PublishItem item) { }
        private static void HandleFastRunResults(PublishItem item) { }
        private static void HandleRequestResults(PublishItem item) { }
        private static void HandleClientMonitorMetrics(PublishItem item) { }
        private static void HandleTestMessage(PublishItem item) { }
        private static void HandleApplicationLogEntry(PublishItem item) { }
        private static void HandleMonitorConfiguration(PublishItem item) { }
        private static void HandleMonitorHardwareConfiguration(PublishItem item) { }
        private static void HandleMonitorMetrics(PublishItem item) { }

        private static void SetDescriptionAndTags(string description, string[] tags) {
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
        private static int SetStressTest(string stressTest, string runSynchronization, string connection, string connectionProxy, string connectionString, string scenarios, string scenarioRuleSet, int[] concurrencies, int runs,
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
                _stressTestId = (int)_databaseActions.GetLastInsertId();
                return _stressTestId;
            }
            return 0;
        }
    }
}
