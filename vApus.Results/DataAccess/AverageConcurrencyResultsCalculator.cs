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
    internal sealed class AverageConcurrencyResultsCalculator : BaseResultSetCalculator {
        private static AverageConcurrencyResultsCalculator _instance = new AverageConcurrencyResultsCalculator();
        public static AverageConcurrencyResultsCalculator GetInstance() { return _instance; }
        private AverageConcurrencyResultsCalculator() { }
        public override DataTable Get(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stresstestIds) {
            DataTable averageConcurrencyResults = CreateEmptyDataTable("AverageConcurrencyResults", "Stresstest", "Started At", "Measured Time", "Measured Time (ms)", "Concurrency",
"Log Entries Processed", "Log Entries", "Errors", "Throughput (responses / s)", "User Actions / s", "Avg. Response Time (ms)",
"Max. Response Time (ms)", "95th Percentile of the Response Times (ms)", "99th Percentile of the Response Times (ms)", "Avg. Top 5 Response Times (ms)", "Avg. Delay (ms)");

            ConcurrentDictionary<string, DataTable> data = GetData(databaseActions, cancellationToken, stresstestIds);
            if (cancellationToken.IsCancellationRequested) return null;

            ConcurrentDictionary<ConcurrencyResult, string> results = GetResults(data, cancellationToken);
            data = null;
            if (cancellationToken.IsCancellationRequested) return null;

            ConcurrentDictionary<StresstestMetrics, string> metricsDic = GetMetrics(results, cancellationToken);
            results = null;
            if (cancellationToken.IsCancellationRequested) return null;

            foreach (StresstestMetrics metrics in metricsDic.Keys) {
                if (cancellationToken.IsCancellationRequested) return null;

                averageConcurrencyResults.Rows.Add(metricsDic[metrics], metrics.StartMeasuringTime, metrics.MeasuredTime.ToString("hh':'mm':'ss'.'fff"), Math.Round(metrics.MeasuredTime.TotalMilliseconds, MidpointRounding.AwayFromZero),
                    metrics.Concurrency, metrics.LogEntriesProcessed, metrics.LogEntries, metrics.Errors, Math.Round(metrics.ResponsesPerSecond, 2, MidpointRounding.AwayFromZero),
                    Math.Round(metrics.UserActionsPerSecond, 2, MidpointRounding.AwayFromZero), Math.Round(metrics.AverageResponseTime.TotalMilliseconds, MidpointRounding.AwayFromZero), 
                    Math.Round(metrics.MaxResponseTime.TotalMilliseconds, MidpointRounding.AwayFromZero), Math.Round(metrics.Percentile95thResponseTimes.TotalMilliseconds, MidpointRounding.AwayFromZero),
                    Math.Round(metrics.Percentile99thResponseTimes.TotalMilliseconds, MidpointRounding.AwayFromZero), Math.Round(metrics.AverageTop5ResponseTimes.TotalMilliseconds, MidpointRounding.AwayFromZero), 
                    Math.Round(metrics.AverageDelay.TotalMilliseconds, MidpointRounding.AwayFromZero));
            }

            DataView dv = averageConcurrencyResults.DefaultView;
            dv.Sort = "Started At";
            averageConcurrencyResults = dv.ToTable();
            return averageConcurrencyResults;
        }

        //Get all data from the database to be processed later.
        protected override ConcurrentDictionary<string, DataTable> GetData(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stresstestIds) {
            var data = new ConcurrentDictionary<string, DataTable>();

            data.TryAdd("stresstests", ReaderAndCombiner.GetStresstests(cancellationToken, databaseActions, stresstestIds, "Id", "Stresstest", "Connection", "Runs"));
            if (cancellationToken.IsCancellationRequested) return null;

            DataTable stresstestResults = ReaderAndCombiner.GetStresstestResults(cancellationToken, databaseActions, stresstestIds, "Id", "StresstestId");
            if (cancellationToken.IsCancellationRequested) return null;

            data.TryAdd("stresstestresults", stresstestResults);

            int[] stresstestResultIds = new int[stresstestResults.Rows.Count];
            for (int i = 0; i != stresstestResultIds.Length; i++)
                stresstestResultIds[i] = (int)stresstestResults.Rows[i][0];

            DataTable concurrencyResults = ReaderAndCombiner.GetConcurrencyResults(cancellationToken, databaseActions, stresstestResultIds, new string[] { "Id", "StartedAt", "StoppedAt", "Concurrency", "StresstestResultId" });
            if (cancellationToken.IsCancellationRequested) return null;

            data.TryAdd("concurrencyresults", concurrencyResults);

            int[] concurrencyResultIds = new int[concurrencyResults.Rows.Count];
            for (int i = 0; i != concurrencyResultIds.Length; i++)
                concurrencyResultIds[i] = (int)concurrencyResults.Rows[i][0];

            DataTable runResults = ReaderAndCombiner.GetRunResults(cancellationToken, databaseActions, concurrencyResultIds, "Id", "Run", "TotalLogEntryCount", "ConcurrencyResultId");
            if (cancellationToken.IsCancellationRequested) return null;

            data.TryAdd("runresults", runResults);

            DataTable[] parts = GetLogEntryResultsThreaded(databaseActions, cancellationToken, runResults, 4, "VirtualUser", "UserAction", "LogEntryIndex", "TimeToLastByteInTicks", "DelayInMilliseconds", "Error", "RunResultId");
            //A merge is way to slow. Needed rows will be extracted when getting results.
            for (int i = 0; i != parts.Length; i++)
                data.TryAdd("logentryresults" + i, parts[i]);
            parts = null;

            //int[] runResultIds = new int[runResults.Rows.Count];
            //for (int i = 0; i != runResultIds.Length; i++)
            //    runResultIds[i] = (int)runResults.Rows[i][0];

            //DataTable logEntryResults = ReaderAndCombiner.GetLogEntryResults(cancellationToken, databaseActions, runResultIds, "VirtualUser", "UserAction", "LogEntryIndex", "TimeToLastByteInTicks", "DelayInMilliseconds", "Error", "RunResultId");
            //if (cancellationToken.IsCancellationRequested) return null;

            //data.Add("logentryresults", logEntryResults);

            return data;
        }

        private ConcurrentDictionary<ConcurrencyResult, string> GetResults(ConcurrentDictionary<string, DataTable> data, CancellationToken cancellationToken) {
            DataRow[] stresstests = data["stresstests"].Select();
            if (stresstests == null || stresstests.Length == 0) return null;

            var concurrencyResultsDic = new ConcurrentDictionary<ConcurrencyResult, string>();

            //Get all the log entry result datatables.
            var logEntryResultsDataList = new List<DataTable>();
            foreach (string key in data.Keys)
                if (key.StartsWith("logentryresults"))
                    logEntryResultsDataList.Add(data[key]);
            DataTable[] logEntryResultsData = logEntryResultsDataList.ToArray();

            for (int stresstestRowIndex = 0; stresstestRowIndex != stresstests.Length; stresstestRowIndex++) {
                DataRow stresstestsRow = stresstests[stresstestRowIndex];
                if (cancellationToken.IsCancellationRequested) return null;

                int stresstestId = (int)stresstestsRow.ItemArray[0];
                string stresstest = stresstestsRow.ItemArray[1] as string;

                DataRow[] stresstestResults = data["stresstestresults"].Select(string.Format("StresstestId={0}", stresstestId));
                if (stresstestResults != null && stresstestResults.Length != 0) {
                    int stresstestResultId = (int)stresstestResults[0].ItemArray[0];

                    int runs = (int)stresstestsRow.ItemArray[3];

                    //Extract all concurrency results for the given stresstest result id and put them in the dictionary.
                    DataRow[] concurrencyResults = data["concurrencyresults"].Select(string.Format("StresstestResultId={0}", stresstestResultId));
                    if (concurrencyResults != null && concurrencyResults.Length != 0) {
                        for (int crRowIndex = 0; crRowIndex != concurrencyResults.Length; crRowIndex++) {
                            DataRow crRow = concurrencyResults[crRowIndex];
                            if (cancellationToken.IsCancellationRequested) return null;

                            int concurrencyResultId = (int)crRow.ItemArray[0];
                            int concurrency = (int)crRow.ItemArray[3];
                            ConcurrencyResult concurrencyResult = new ConcurrencyResult(concurrency, runs);
                            concurrencyResultsDic.TryAdd(concurrencyResult, stresstest);
                            concurrencyResult.StartedAt = (DateTime)crRow.ItemArray[1];
                            concurrencyResult.StoppedAt = (DateTime)crRow.ItemArray[2];

                            //Extract all the run results and put them in an array , runResultsArr,for later processing.
                            DataRow[] runResults = data["runresults"].Select(string.Format("ConcurrencyResultId={0}", concurrencyResultId));
                            if (runResults == null || runResults.Length == 0) continue;

                            var runResultIds = new int[runResults.Length];
                            var totalLogEntryCountsPerUser = new ulong[runResults.Length];

                            var runResultsArr = new RunResult[runResults.Length];
                            Parallel.For(0, runResults.Length, (rrRowIndex, loopState3) => {
                                if (cancellationToken.IsCancellationRequested) loopState3.Break();
                                DataRow rrRow = runResults[rrRowIndex];

                                int runResultId = (int)rrRow.ItemArray[0];
                                runResultIds[rrRowIndex] = runResultId;
                                runResultsArr[rrRowIndex] = new RunResult((int)rrRow.ItemArray[1], concurrencyResult.Concurrency);

                                totalLogEntryCountsPerUser[rrRowIndex] = (ulong)rrRow.ItemArray[2] / (ulong)concurrencyResult.Concurrency; //Used for adding ampty log entry results. Needed for correct calcullating metrics for a cancelled or broken test.
                            }
                            );
                            runResults = null; //Making it easy for the GC by cleaning up ourselves.
                            concurrencyResult.RunResults.AddRange(runResultsArr);

                            //Extract and add virtual user results and log entry results and add those to the runs in the array runResultsArr.
                            Parallel.For(0, runResultsArr.Length, (i, loopState) => {
                                if (cancellationToken.IsCancellationRequested) loopState.Break();

                                var runResult = runResultsArr[i];
                                int runResultId = runResultIds[i];

                                //Get the log entry results containing log entries with the given run result id.
                                var ler = new DataRow[0];
                                for (int lerDataIndex = 0; lerDataIndex != logEntryResultsData.Length; lerDataIndex++) {
                                    ler = logEntryResultsData[lerDataIndex].Select(string.Format("RunResultId={0}", runResultId));
                                    if (ler.Length != 0)
                                        break;
                                }

                                if (ler != null && ler.Length != 0) {
                                    var logEntryResults = new ConcurrentDictionary<string, SynchronizedCollection<LogEntryResult>>(); //Key == virtual user.

                                    //foreach (var lerRow in ler) {
                                    Parallel.ForEach(ler, (lerRow, loopState2) => {
                                        if (cancellationToken.IsCancellationRequested) loopState2.Break();

                                        string virtualUser = lerRow["VirtualUser"] as string;
                                        logEntryResults.TryAdd(virtualUser, new SynchronizedCollection<LogEntryResult>());

                                        logEntryResults[virtualUser].Add(new LogEntryResult() {
                                            VirtualUser = virtualUser, UserAction = lerRow["UserAction"] as string, LogEntryIndex = lerRow["LogEntryIndex"] as string,
                                            TimeToLastByteInTicks = (long)lerRow["TimeToLastByteInTicks"], DelayInMilliseconds = (int)lerRow["DelayInMilliseconds"], Error = lerRow["Error"] as string
                                        });
                                    }
                                    );
                                    ler = null;

                                    var virtualUserResults = new ConcurrentDictionary<string, VirtualUserResult>();

                                    //Add empty ones for broken runs.
                                    //foreach (var item in logEntryResults) { 
                                    Parallel.ForEach(logEntryResults, (item, loopState2) => {
                                        if (cancellationToken.IsCancellationRequested) loopState2.Break();

                                        while ((ulong)item.Value.Count < totalLogEntryCountsPerUser[i])
                                            item.Value.Add(new LogEntryResult());

                                        if (cancellationToken.IsCancellationRequested) loopState2.Break();

                                        virtualUserResults.TryAdd(item.Key, new VirtualUserResult(logEntryResults[item.Key].Count) { VirtualUser = item.Key });
                                        VirtualUserResult virtualUserResult = virtualUserResults[item.Key];

                                        for (int k = 0; k != item.Value.Count; k++) {
                                            if (cancellationToken.IsCancellationRequested) loopState2.Break();
                                            virtualUserResult.LogEntryResults[k] = item.Value[k];
                                        }
                                    }
                                    );
                                    runResult.VirtualUserResults = virtualUserResults.Values.ToArray();

                                    logEntryResults = null;
                                    virtualUserResults = null;
                                }
                            }
                            );
                            runResultsArr = null;

                        }
                        concurrencyResults = null;
                    }
                }
                stresstestResults = null;
            }
            stresstests = null;

            return concurrencyResultsDic;
        }

        private ConcurrentDictionary<StresstestMetrics, string> GetMetrics(ConcurrentDictionary<ConcurrencyResult, string> results, CancellationToken cancellationToken) {
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
