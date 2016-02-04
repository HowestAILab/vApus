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
        public override DataTable Get(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stressTestIds) {
            DataTable averageConcurrencyResults = CreateEmptyDataTable("AverageConcurrencyResults", "Stress test", "Started at", "Measured time", "Measured time (ms)", "Concurrency",
"Requests processed", "Requests", "Errors", "Throughput (responses / s)", "User actions / s", "Avg. response time (ms)",
"Max. response time (ms)", "95th percentile of the response times (ms)", "99th percentile of the response times (ms)", "Avg. top 5 response Times (ms)", "Avg. delay (ms)");

            ConcurrentDictionary<string, DataTable> data = GetData(databaseActions, cancellationToken, stressTestIds);
            if (cancellationToken.IsCancellationRequested) return null;

            ConcurrentDictionary<ConcurrencyResult, string> results = GetResults(data, cancellationToken);
            data = null;
            if (cancellationToken.IsCancellationRequested) return null;

            ConcurrentDictionary<StressTestMetrics, string> metricsDic = GetMetrics(results, cancellationToken);
            results = null;
            if (cancellationToken.IsCancellationRequested) return null;

            foreach (StressTestMetrics metrics in metricsDic.Keys) {
                if (cancellationToken.IsCancellationRequested) return null;

                string measuredTime = metrics.MeasuredTime.TotalSeconds < 1d ? metrics.MeasuredTime.ToString("hh':'mm':'ss'.'fff") : metrics.MeasuredTime.ToString("hh':'mm':'ss");
                averageConcurrencyResults.Rows.Add(metricsDic[metrics], metrics.StartMeasuringTime, measuredTime, Math.Round(metrics.MeasuredTime.TotalMilliseconds, MidpointRounding.AwayFromZero),
                    metrics.Concurrency, metrics.RequestsProcessed, metrics.Requests, metrics.Errors, Math.Round(metrics.ResponsesPerSecond, 2, MidpointRounding.AwayFromZero),
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
        protected override ConcurrentDictionary<string, DataTable> GetData(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stressTestIds) {
            var data = new ConcurrentDictionary<string, DataTable>();

            data.TryAdd("stresstests", ReaderAndCombiner.GetStressTests(cancellationToken, databaseActions, stressTestIds, "Id", "StressTest", "Connection", "Runs"));
            if (cancellationToken.IsCancellationRequested) return null;

            DataTable stressTestResults = ReaderAndCombiner.GetStressTestResults(cancellationToken, databaseActions, stressTestIds, "Id", "StressTestId");
            if (cancellationToken.IsCancellationRequested) return null;

            data.TryAdd("stresstestresults", stressTestResults);

            int[] stressTestResultIds = new int[stressTestResults.Rows.Count];
            for (int i = 0; i != stressTestResultIds.Length; i++)
                stressTestResultIds[i] = (int)stressTestResults.Rows[i][0];

            DataTable concurrencyResults = ReaderAndCombiner.GetConcurrencyResults(cancellationToken, databaseActions, stressTestResultIds, new string[] { "Id", "StartedAt", "StoppedAt", "Concurrency", "StressTestResultId" });
            if (cancellationToken.IsCancellationRequested) return null;

            data.TryAdd("concurrencyresults", concurrencyResults);

            int[] concurrencyResultIds = new int[concurrencyResults.Rows.Count];
            for (int i = 0; i != concurrencyResultIds.Length; i++)
                concurrencyResultIds[i] = (int)concurrencyResults.Rows[i][0];

            DataTable runResults = ReaderAndCombiner.GetRunResults(cancellationToken, databaseActions, concurrencyResultIds, "Id", "Run", "TotalRequestCount", "ConcurrencyResultId");
            if (cancellationToken.IsCancellationRequested) return null;

            data.TryAdd("runresults", runResults);

            DataTable[] parts = GetRequestResultsPerRunThreaded(databaseActions, cancellationToken, runResults, 4, "Id", "VirtualUser", "UserAction", "RequestIndex", "InParallelWithPrevious", "TimeToLastByteInTicks", "DelayInMilliseconds", "SentAt", "Length(Error) As Error", "RunResultId");
            //A merge is way to slow. Needed rows will be extracted when getting results.
            for (int i = 0; i != parts.Length; i++)
                data.TryAdd("requestresults" + i, parts[i]);
            parts = null;

            //int[] runResultIds = new int[runResults.Rows.Count];
            //for (int i = 0; i != runResultIds.Length; i++)
            //    runResultIds[i] = (int)runResults.Rows[i][0];

            //DataTable requestResults = ReaderAndCombiner.GetRequestResults(cancellationToken, databaseActions, runResultIds, "VirtualUser", "UserAction", "RequestIndex", "TimeToLastByteInTicks", "DelayInMilliseconds", "Error", "RunResultId");
            //if (cancellationToken.IsCancellationRequested) return null;

            //data.Add("requestresults", requestResults);

            return data;
        }

        private ConcurrentDictionary<ConcurrencyResult, string> GetResults(ConcurrentDictionary<string, DataTable> data, CancellationToken cancellationToken) {
            DataRow[] stressTests = data["stresstests"].Select();
            if (stressTests == null || stressTests.Length == 0) return null;

            var concurrencyResultsDic = new ConcurrentDictionary<ConcurrencyResult, string>();

            //Get all the request result datatables.
            var requestResultsDataList = new List<DataTable>();
            foreach (string key in data.Keys)
                if (key.StartsWith("requestresults"))
                    requestResultsDataList.Add(data[key]);
            DataTable[] requestResultsData = requestResultsDataList.ToArray();

            for (int stressTestRowIndex = 0; stressTestRowIndex != stressTests.Length; stressTestRowIndex++) {
                DataRow stressTestsRow = stressTests[stressTestRowIndex];
                if (cancellationToken.IsCancellationRequested) return null;

                int stressTestId = (int)stressTestsRow.ItemArray[0];
                string stressTest = stressTestsRow.ItemArray[1] as string;

                DataRow[] stressTestResults = data["stresstestresults"].Select(string.Format("StressTestId={0}", stressTestId));
                if (stressTestResults != null && stressTestResults.Length != 0) {
                    int stressTestResultId = (int)stressTestResults[0].ItemArray[0];

                    int runs = (int)stressTestsRow.ItemArray[3];

                    //Extract all concurrency results for the given stress test result id and put them in the dictionary.
                    DataRow[] concurrencyResults = data["concurrencyresults"].Select(string.Format("StressTestResultId={0}", stressTestResultId));
                    if (concurrencyResults != null && concurrencyResults.Length != 0) {
                        for (int crRowIndex = 0; crRowIndex != concurrencyResults.Length; crRowIndex++) {
                            DataRow crRow = concurrencyResults[crRowIndex];
                            if (cancellationToken.IsCancellationRequested) return null;

                            int concurrencyResultId = (int)crRow.ItemArray[0];
                            int concurrency = (int)crRow.ItemArray[3];
                            ConcurrencyResult concurrencyResult = new ConcurrencyResult(concurrencyResultId, concurrency, runs);
                            concurrencyResultsDic.TryAdd(concurrencyResult, stressTest);
                            concurrencyResult.StartedAt = (DateTime)crRow.ItemArray[1];
                            concurrencyResult.StoppedAt = (DateTime)crRow.ItemArray[2];
                            if (concurrencyResult.StoppedAt == DateTime.MinValue) concurrencyResult.StoppedAt = concurrencyResult.StartedAt.Subtract(new TimeSpan(TimeSpan.TicksPerMillisecond));

                            //Extract all the run results and put them in an array , runResultsArr,for later processing.
                            DataRow[] runResults = data["runresults"].Select(string.Format("ConcurrencyResultId={0}", concurrencyResultId));
                            if (runResults == null || runResults.Length == 0) continue;

                            var runResultIds = new int[runResults.Length];
                            var totalRequestCountsPerUser = new ulong[runResults.Length];

                            var runResultsArr = new RunResult[runResults.Length];
                            Parallel.For(0, runResults.Length, (rrRowIndex, loopState3) => {
                                if (cancellationToken.IsCancellationRequested) loopState3.Break();
                                DataRow rrRow = runResults[rrRowIndex];

                                int runResultId = (int)rrRow.ItemArray[0];
                                runResultIds[rrRowIndex] = runResultId;
                                runResultsArr[rrRowIndex] = new RunResult(concurrencyResult.ConcurrencyId, (int)rrRow.ItemArray[1], concurrencyResult.Concurrency);

                                totalRequestCountsPerUser[rrRowIndex] = (ulong)rrRow.ItemArray[2] / (ulong)concurrencyResult.Concurrency; //Used for adding ampty request results. Needed for correct calcullating metrics for a cancelled or broken test.
                            }
                            );
                            runResults = null; //Making it easy for the GC by cleaning up ourselves.
                            concurrencyResult.RunResults.AddRange(runResultsArr);

                            //Extract and add virtual user results and request results and add those to the runs in the array runResultsArr.
                            Parallel.For(0, runResultsArr.Length, (i, loopState) => {
                                if (cancellationToken.IsCancellationRequested) loopState.Break();

                                var runResult = runResultsArr[i];
                                int runResultId = runResultIds[i];

                                //Get the request results containing requests with the given run result id.
                                var rer = new DataRow[0];
                                for (int rerDataIndex = 0; rerDataIndex != requestResultsData.Length; rerDataIndex++) {
                                    rer = requestResultsData[rerDataIndex].Select(string.Format("RunResultId={0}", runResultId));
                                    if (rer.Length != 0)
                                        break;
                                }

                                //The order of items is very important for parallel requests.
                                if (rer != null && rer.Length != 0) {
                                    var requestResults = new ConcurrentDictionary<string, ConcurrentDictionary<int, RequestResult>>(); //Key == virtual user.

                                    //foreach (var rerRow in rer) {
                                    Parallel.For(0, rer.Length, (rerRowIndex, loopState2) => {
                                        if (cancellationToken.IsCancellationRequested) loopState2.Break();

                                        DataRow rerRow = rer[rerRowIndex];

                                        string virtualUser = rerRow["VirtualUser"] as string;
                                        requestResults.TryAdd(virtualUser, new ConcurrentDictionary<int, RequestResult>());

                                        requestResults[virtualUser].TryAdd(rerRowIndex, new RequestResult() {
                                            VirtualUser = virtualUser, UserAction = rerRow["UserAction"] as string, RequestIndex = rerRow["RequestIndex"] as string,
                                            InParallelWithPrevious = (bool)rerRow["InParallelWithPrevious"], TimeToLastByteInTicks = (long)rerRow["TimeToLastByteInTicks"],
                                            DelayInMilliseconds = (int)rerRow["DelayInMilliseconds"], SentAt = (DateTime)rerRow["SentAt"], Error = (long)rerRow["Error"] == 0 ? null : "-"
                                        });
                                    }
                                    );
                                    rer = null;

                                    var virtualUserResults = new ConcurrentDictionary<string, VirtualUserResult>();

                                    //Add empty ones for broken runs.
                                    //foreach (var item in requestResults) { 
                                    Parallel.ForEach(requestResults, (item, loopState2) => {
                                        if (cancellationToken.IsCancellationRequested) loopState2.Break();

                                        ulong valueCount = (ulong)item.Value.Count;
                                        int maxValueKey = item.Value.Keys.Max();
                                        while (valueCount++ < totalRequestCountsPerUser[i])
                                            item.Value.TryAdd(++maxValueKey, new RequestResult());

                                        if (cancellationToken.IsCancellationRequested) loopState2.Break();

                                        virtualUserResults.TryAdd(item.Key, new VirtualUserResult(requestResults[item.Key].Count) { VirtualUser = item.Key });
                                        VirtualUserResult virtualUserResult = virtualUserResults[item.Key];

                                        virtualUserResult.RequestResults = item.Value.Values.ToArray();
                                    }
                                    );
                                    runResult.VirtualUserResults = virtualUserResults.Values.ToArray();

                                    requestResults = null;
                                    virtualUserResults = null;
                                }
                            }
                            );
                            runResultsArr = null;

                        }
                        concurrencyResults = null;
                    }
                }
                stressTestResults = null;
            }
            stressTests = null;

            return concurrencyResultsDic;
        }

        private ConcurrentDictionary<StressTestMetrics, string> GetMetrics(ConcurrentDictionary<ConcurrencyResult, string> results, CancellationToken cancellationToken) {
            var metricsDic = new ConcurrentDictionary<StressTestMetrics, string>();
            Parallel.ForEach(results, (item, loopState) => {
                if (cancellationToken.IsCancellationRequested) loopState.Break();
                metricsDic.TryAdd(DetailedStressTestMetricsHelper.GetMetrics(item.Key, cancellationToken), item.Value);
            });
            if (cancellationToken.IsCancellationRequested) return null;
            return metricsDic;
        }
    }
}
