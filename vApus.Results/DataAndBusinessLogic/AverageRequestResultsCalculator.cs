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
    internal sealed class AverageRequestResultsCalculator : BaseResultSetCalculator {
        private static AverageRequestResultsCalculator _instance = new AverageRequestResultsCalculator();
        public static AverageRequestResultsCalculator GetInstance() { return _instance; }

        private readonly object _lock = new object();

        private AverageRequestResultsCalculator() { }

        public override DataTable Get(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stressTestIds) {
            ConcurrentDictionary<string, DataTable> data = GetData(databaseActions, cancellationToken, stressTestIds);
            if (cancellationToken.IsCancellationRequested) return null;

            DataTable results = GetResults(data, cancellationToken);
            data = null;
            if (cancellationToken.IsCancellationRequested) return null;

            DataView dv = results.DefaultView;
            dv.Sort = "Concurrency";
            results = dv.ToTable();

            return results;
        }

        protected override ConcurrentDictionary<string, DataTable> GetData(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stressTestIds) {
            var data = new ConcurrentDictionary<string, DataTable>();

            data.TryAdd("stresstests", ReaderAndCombiner.GetStressTests(cancellationToken, databaseActions, stressTestIds, "Id", "StressTest", "Connection"));
            if (cancellationToken.IsCancellationRequested) return null;

            DataTable stressTestResults = ReaderAndCombiner.GetStressTestResults(cancellationToken, databaseActions, stressTestIds, "Id", "StressTestId");
            if (cancellationToken.IsCancellationRequested) return null;

            data.TryAdd("stresstestresults", stressTestResults);

            int[] stressTestResultIds = new int[stressTestResults.Rows.Count];
            for (int i = 0; i != stressTestResultIds.Length; i++)
                stressTestResultIds[i] = (int)stressTestResults.Rows[i][0];

            DataTable concurrencyResults = ReaderAndCombiner.GetConcurrencyResults(cancellationToken, databaseActions, stressTestResultIds, new string[] { "Id", "Concurrency", "StressTestResultId" });
            if (cancellationToken.IsCancellationRequested) return null;

            data.TryAdd("concurrencyresults", concurrencyResults);

            int[] concurrencyResultIds = new int[concurrencyResults.Rows.Count];
            for (int i = 0; i != concurrencyResultIds.Length; i++)
                concurrencyResultIds[i] = (int)concurrencyResults.Rows[i][0];

            DataTable runResults = ReaderAndCombiner.GetRunResults(cancellationToken, databaseActions, concurrencyResultIds, "Id", "ConcurrencyResultId");
            if (cancellationToken.IsCancellationRequested) return null;

            data.TryAdd("runresults", runResults);

            DataTable[] parts = GetRequestResultsPerRunThreaded(databaseActions, cancellationToken, runResults, 4, "Id", "SameAsRequestIndex", "RequestIndex", "UserAction", "Request", "TimeToLastByteInTicks", "DelayInMilliseconds", "Length(Error) As Error", "RunResultId");
            //A merge is way to slow. Needed rows will be extracted when getting results.
            for (int i = 0; i != parts.Length; i++)
                data.TryAdd("requestresults" + i, parts[i]);
            parts = null;

            return data;
        }

        private DataTable GetResults(ConcurrentDictionary<string, DataTable> data, CancellationToken cancellationToken) {
            DataRow[] stressTests = data["stresstests"].Select();
            if (stressTests == null || stressTests.Length == 0) return null;

            //Get all the request result datatables.
            var requestResultsDataList = new List<DataTable>();
            foreach (string key in data.Keys)
                if (key.StartsWith("requestresults"))
                    requestResultsDataList.Add(data[key]);
            DataTable[] requestResultsData = requestResultsDataList.ToArray();

            DataTable averageRequestResults = CreateEmptyDataTable("AverageRequestResults", "Stress test", "Concurrency", "User action", "Request", "Avg. response time (ms)",
"Max. response time (ms)", "95th percentile of the response times (ms)", "99th percentile of the response times (ms)", "Avg. top 5 response times (ms)", "Avg. delay (ms)", "Errors");

            foreach (DataRow stressTestsRow in stressTests) {
                if (cancellationToken.IsCancellationRequested) return null;

                int stressTestId = (int)stressTestsRow.ItemArray[0];

                DataRow[] stressTestResults = data["stresstestresults"].Select(string.Format("StressTestId={0}", stressTestId));
                if (stressTestResults == null || stressTestResults.Length == 0) continue;
                int stressTestResultId = (int)stressTestResults[0].ItemArray[0];

                string stressTest = string.Format("{0} {1}", stressTestsRow.ItemArray[1], stressTestsRow.ItemArray[2]);

                DataRow[] concurrencyResults = data["concurrencyresults"].Select(string.Format("StressTestResultId={0}", stressTestResultId));
                if (concurrencyResults == null || concurrencyResults.Length == 0) continue;

                Parallel.ForEach(concurrencyResults, (crRow, loopState) => {
                    if (cancellationToken.IsCancellationRequested) loopState.Break();

                    int concurrencyResultId = (int)crRow.ItemArray[0];
                    int concurrency = (int)crRow.ItemArray[1];

                    DataRow[] runResults = data["runresults"].Select(string.Format("ConcurrencyResultId={0}", concurrencyResultId));
                    if (runResults != null && runResults.Length != 0) {
                        var runResultIds = new int[runResults.Length];
                        for (int rrRowIndex = 0; rrRowIndex != runResults.Length; rrRowIndex++) {
                            if (cancellationToken.IsCancellationRequested) loopState.Break();

                            runResultIds[rrRowIndex] = ((int)runResults[rrRowIndex].ItemArray[0]);
                        }

                        //Get the request results containing requests with the given run result id.
                        string selectClause = string.Format("RunResultId In({0})", runResultIds.Combine(", "));
                        var requestResults = new List<DataRow>();
                        for (int rerDataIndex = 0; rerDataIndex != requestResultsData.Length; rerDataIndex++) {
                            DataRow[] selectedRows = requestResultsData[rerDataIndex].Select(selectClause);
                            requestResults.AddRange(selectedRows);
                        }

                        if (requestResults.Count != 0) {
                            //We don't need to keep the run ids for this one, it's much faster and simpler like this.
                            var uniqueRequestCounts = new ConcurrentDictionary<string, int>(); //To make a correct average.
                            var userActions = new ConcurrentDictionary<string, string>(); //request index, User Action

                            foreach (DataRow rerRow in requestResults) {
                                if (cancellationToken.IsCancellationRequested) loopState.Break();

                                string rerEntryIndex = rerRow["SameAsRequestIndex"] as string; //Combine results when using distribution like this.
                                if (rerEntryIndex == string.Empty) {
                                    rerEntryIndex = rerRow["RequestIndex"] as string;

                                    //Make sure we have all the user actions before averages are calculated, otherwise the duplicated user action names can be used. 
                                    if (!userActions.ContainsKey(rerEntryIndex)) {
                                        string userAction = rerRow["UserAction"] as string;
                                        userActions.TryAdd(rerEntryIndex, userAction);
                                    }
                                }

                                if (uniqueRequestCounts.ContainsKey(rerEntryIndex)) ++uniqueRequestCounts[rerEntryIndex];
                                else uniqueRequestCounts.TryAdd(rerEntryIndex, 1);
                            }

                            var requests = new Dictionary<string, string>(); //Request index, request

                            var avgTimeToLastByteInTicks = new Dictionary<string, double>();
                            var maxTimeToLastByteInTicks = new Dictionary<string, double>();
                            var timeToLastBytesInTicks = new Dictionary<string, List<double>>();
                            var perc95TimeToLastBytesInTicks = new ConcurrentDictionary<string, double>();
                            var perc99TimeToLastBytesInTicks = new ConcurrentDictionary<string, double>();
                            var avgTop5TimeToLastBytesInTicks = new ConcurrentDictionary<string, double>();

                            var avgDelay = new Dictionary<string, double>();
                            var errors = new Dictionary<string, long>();

                            foreach (DataRow rerRow in requestResults) {
                                if (cancellationToken.IsCancellationRequested) loopState.Break();

                                string requestIndex = rerRow["SameAsRequestIndex"] as string; //Combine results when using distribution like this.
                                if (requestIndex == string.Empty) requestIndex = rerRow["RequestIndex"] as string;

                                string userAction = rerRow["UserAction"] as string;
                                string request = rerRow["Request"] as string;
                                double ttlb = Convert.ToDouble((long)rerRow["TimeToLastByteInTicks"]);
                                double delay = Convert.ToDouble((int)rerRow["DelayInMilliseconds"]);
                                string error = (long)rerRow["Error"] == 0 ? null : "-";

                                int uniqueRequestCount = uniqueRequestCounts[requestIndex];

                                if (!userActions.ContainsKey(requestIndex)) userActions.TryAdd(requestIndex, userAction);
                                if (!requests.ContainsKey(requestIndex)) requests.Add(requestIndex, request);

                                if (avgTimeToLastByteInTicks.ContainsKey(requestIndex)) avgTimeToLastByteInTicks[requestIndex] += (ttlb / uniqueRequestCount);
                                else avgTimeToLastByteInTicks.Add(requestIndex, ttlb / uniqueRequestCount);

                                if (maxTimeToLastByteInTicks.ContainsKey(requestIndex)) { if (maxTimeToLastByteInTicks[requestIndex] < ttlb) maxTimeToLastByteInTicks[requestIndex] = ttlb; } else maxTimeToLastByteInTicks.Add(requestIndex, ttlb);

                                if (!timeToLastBytesInTicks.ContainsKey(requestIndex)) timeToLastBytesInTicks.Add(requestIndex, new List<double>(uniqueRequestCount));
                                timeToLastBytesInTicks[requestIndex].Add(ttlb);

                                if (avgDelay.ContainsKey(requestIndex)) avgDelay[requestIndex] += (delay / uniqueRequestCount);
                                else avgDelay.Add(requestIndex, delay / uniqueRequestCount);


                                if (!errors.ContainsKey(requestIndex)) errors.Add(requestIndex, 0);
                                if (!string.IsNullOrEmpty(error)) ++errors[requestIndex];
                            }

                            //95th percentile
                            //foreach (var item in timeToLastBytesInTicks) {
                            Parallel.ForEach(timeToLastBytesInTicks, (item, loopState2) => {
                                if (cancellationToken.IsCancellationRequested) loopState2.Break();

                                IEnumerable<double> orderedValues;
                                perc95TimeToLastBytesInTicks.TryAdd(item.Key, PercentileCalculator<double>.Get(timeToLastBytesInTicks[item.Key], 95, out orderedValues));
                                perc99TimeToLastBytesInTicks.TryAdd(item.Key, PercentileCalculator<double>.Get(orderedValues, 99));

                                int top5Count = Convert.ToInt32(orderedValues.Count() * 0.05);
                                if (top5Count == 0)
                                    avgTop5TimeToLastBytesInTicks.TryAdd(item.Key, orderedValues.FirstOrDefault());
                                else
                                    avgTop5TimeToLastBytesInTicks.TryAdd(item.Key, orderedValues.Take(top5Count).Average());
                            });
                            if (cancellationToken.IsCancellationRequested) loopState.Break();

                            List<string> sortedRequestIndices = requests.Keys.ToList();
                            sortedRequestIndices.Sort(ResultsHelper.RequestIndexComparer.GetInstance());

                            lock (_lock)
                                foreach (string s in sortedRequestIndices) {
                                    if (cancellationToken.IsCancellationRequested) loopState.Break();

                                    averageRequestResults.Rows.Add(stressTest, concurrency, userActions[s], requests[s],
                                        Math.Round(avgTimeToLastByteInTicks[s] / TimeSpan.TicksPerMillisecond, MidpointRounding.AwayFromZero),
                                        Math.Round(maxTimeToLastByteInTicks[s] / TimeSpan.TicksPerMillisecond, MidpointRounding.AwayFromZero),
                                        Math.Round(perc95TimeToLastBytesInTicks[s] / TimeSpan.TicksPerMillisecond, MidpointRounding.AwayFromZero),
                                        Math.Round(perc99TimeToLastBytesInTicks[s] / TimeSpan.TicksPerMillisecond, MidpointRounding.AwayFromZero),
                                        Math.Round(avgTop5TimeToLastBytesInTicks[s] / TimeSpan.TicksPerMillisecond, MidpointRounding.AwayFromZero),
                                        Math.Round(avgDelay[s], MidpointRounding.AwayFromZero),
                                        errors[s]);
                                }
                        }
                    }
                }
                );
            }
            return averageRequestResults;
        }
    }
}
