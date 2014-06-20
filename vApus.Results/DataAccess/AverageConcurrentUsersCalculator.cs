/*
 * Copyright 2014 (c) Sizing Servers Lab
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
using System.Threading;
using System.Threading.Tasks;
using vApus.Util;

namespace vApus.Results {
    internal class AverageConcurrentUsersCalculator {
        public static DataTable Get(DatabaseActions _databaseActions, CancellationToken cancellationToken, params int[] stresstestIds) {
            DataTable averageConcurrentUsers = CreateEmptyDataTable("AverageConcurrentUsers", "Stresstest", "Started At", "Measured Time (ms)", "Concurrency",
"Log Entries Processed", "Log Entries", "Errors", "Throughput (responses / s)", "User Actions / s", "Avg. Response Time (ms)",
"Max. Response Time (ms)", "95th Percentile of the Response Times (ms)", "Avg. Delay (ms)");

            ConcurrentDictionary<ConcurrencyResult, string> results = GetResults(_databaseActions, cancellationToken, stresstestIds);
            if (cancellationToken.IsCancellationRequested) return null;

            ConcurrentDictionary<StresstestMetrics, string> metricsDic = GetMetrics(results, cancellationToken);
            results = null;
            if (cancellationToken.IsCancellationRequested) return null;

            foreach (StresstestMetrics metrics in metricsDic.Keys) {
                averageConcurrentUsers.Rows.Add(metricsDic[metrics], metrics.StartMeasuringTime, Math.Round(metrics.MeasuredTime.TotalMilliseconds, 2),
                    metrics.Concurrency, metrics.LogEntriesProcessed, metrics.LogEntries, metrics.Errors, Math.Round(metrics.ResponsesPerSecond, 2), Math.Round(metrics.UserActionsPerSecond, 2),
                    Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2), Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2), Math.Round(metrics.Percentile95thResponseTimes.TotalMilliseconds, 2),
                    Math.Round(metrics.AverageDelay.TotalMilliseconds, 2));
            }
            return averageConcurrentUsers;
        }
        private static DataTable CreateEmptyDataTable(string name, string columnName1, params string[] columnNames) {
            var objectType = typeof(object);
            var dataTable = new DataTable(name);
            dataTable.Columns.Add(columnName1);
            foreach (string columnName in columnNames) dataTable.Columns.Add(columnName, objectType);
            return dataTable;
        }

        private static ConcurrentDictionary<ConcurrencyResult, string> GetResults(DatabaseActions _databaseActions, CancellationToken cancellationToken, params int[] stresstestIds) {
            var stresstests = ReaderAndCombiner.GetStresstests(cancellationToken, _databaseActions, stresstestIds, "Id", "Stresstest", "Connection", "Runs");
            if (stresstests == null || stresstests.Rows.Count == 0) return null;

            var concurrencyResultsDic = new ConcurrentDictionary<ConcurrencyResult, string>();

            foreach (DataRow stresstestsRow in stresstests.Rows) {
                if (cancellationToken.IsCancellationRequested) return null;

                int stresstestId = (int)stresstestsRow.ItemArray[0];
                string stresstest = stresstestsRow.ItemArray[1] as string;

                DataTable stresstestResults = ReaderAndCombiner.GetStresstestResults(cancellationToken, _databaseActions, stresstestId, "Id");
                if (stresstestResults == null || stresstestResults.Rows.Count == 0) continue;
                int stresstestResultId = (int)stresstestResults.Rows[0].ItemArray[0];

                int runs = (int)stresstestsRow.ItemArray[3];

                DataTable concurrencyResults = ReaderAndCombiner.GetConcurrencyResults(cancellationToken, _databaseActions, stresstestResultId, new string[] { "Id", "StartedAt", "StoppedAt", "Concurrency" });
                if (concurrencyResults == null || concurrencyResults.Rows.Count == 0) continue;

                foreach (DataRow crRow in concurrencyResults.Rows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    int concurrencyResultId = (int)crRow.ItemArray[0];
                    int concurrency = (int)crRow.ItemArray[3];
                    ConcurrencyResult concurrencyResult = new ConcurrencyResult(concurrency, runs);
                    concurrencyResultsDic.TryAdd(concurrencyResult, stresstest);
                    concurrencyResult.StartedAt = (DateTime)crRow.ItemArray[1];
                    concurrencyResult.StoppedAt = (DateTime)crRow.ItemArray[2];

                    DataTable runResults = ReaderAndCombiner.GetRunResults(cancellationToken, _databaseActions, concurrencyResultId, "Id", "Run", "TotalLogEntryCount");
                    if (runResults == null || runResults.Rows.Count == 0) continue;

                    var runResultIds = new int[runResults.Rows.Count];
                    var totalLogEntryCountsPerUser = new ulong[runResults.Rows.Count];

                    for (int rrRowIndex = 0; rrRowIndex != runResults.Rows.Count; rrRowIndex++) {
                        if (cancellationToken.IsCancellationRequested) return null;
                        DataRow rrRow = runResults.Rows[rrRowIndex];

                        int runResultId = (int)rrRow.ItemArray[0];
                        runResultIds[rrRowIndex] = runResultId;
                        concurrencyResult.RunResults.Add(new RunResult((int)rrRow.ItemArray[1], concurrencyResult.Concurrency));

                        totalLogEntryCountsPerUser[rrRowIndex] = (ulong)rrRow.ItemArray[2] / (ulong)concurrencyResult.Concurrency;
                    }
                    runResults = null;

                    for (int i = 0; i != concurrencyResult.RunResults.Count; i++) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        var runResult = concurrencyResult.RunResults[i];
                        int runResultId = runResultIds[i];

                        DataTable ler = ReaderAndCombiner.GetLogEntryResults(cancellationToken, _databaseActions, runResultId, "VirtualUser", "UserAction", "LogEntryIndex", "TimeToLastByteInTicks", "DelayInMilliseconds", "Error");
                        if (ler == null || ler.Rows.Count == 0) continue;

                        var virtualUserResults = new ConcurrentDictionary<string, VirtualUserResult>();
                        var logEntryResults = new ConcurrentDictionary<string, SynchronizedCollection<LogEntryResult>>(); //Key == virtual user.

                        Parallel.ForEach(ler.AsEnumerable(), (lerRow, loopState) => {
                            if (cancellationToken.IsCancellationRequested) loopState.Break();

                            string virtualUser = lerRow["VirtualUser"] as string;
                            logEntryResults.TryAdd(virtualUser, new SynchronizedCollection<LogEntryResult>());

                            logEntryResults[virtualUser].Add(new LogEntryResult() {
                                VirtualUser = virtualUser, UserAction = lerRow["UserAction"] as string, LogEntryIndex = lerRow["LogEntryIndex"] as string,
                                TimeToLastByteInTicks = (long)lerRow["TimeToLastByteInTicks"], DelayInMilliseconds = (int)lerRow["DelayInMilliseconds"], Error = lerRow["Error"] as string
                            });
                        });
                        ler = null;

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
                            VirtualUserResult virtualUserResult = virtualUserResults[item.Key];

                            Parallel.For(0, item.Value.Count, (k, loopState2) => {
                                if (cancellationToken.IsCancellationRequested) loopState2.Break();
                                virtualUserResult.LogEntryResults[k] = item.Value[k];
                            });
                        });

                        runResult.VirtualUserResults = virtualUserResults.Values.ToArray();
                    }
                }
                concurrencyResults = null;
            }
            stresstests = null;

            return concurrencyResultsDic;
        }

        private static ConcurrentDictionary<StresstestMetrics, string> GetMetrics(ConcurrentDictionary<ConcurrencyResult, string> results, CancellationToken cancellationToken) {
            var metricsDic = new ConcurrentDictionary<StresstestMetrics, string>();
            Parallel.ForEach(results, (item, loopState) => {
                if (cancellationToken.IsCancellationRequested) loopState.Break();
                metricsDic.TryAdd(DetailedStresstestMetricsHelper.GetMetrics(item.Key, cancellationToken), item.Value);
            });
            if (cancellationToken.IsCancellationRequested) return null;
            return metricsDic;
        }
    }
}
