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
using System.Threading;
using System.Threading.Tasks;
using vApus.Util;
using System.Linq;

namespace vApus.Results {
    internal sealed class ResponseTimeDistributionForRequestsPerConcurrencyCalculator : BaseResultSetCalculator {
        private static ResponseTimeDistributionForRequestsPerConcurrencyCalculator _instance = new ResponseTimeDistributionForRequestsPerConcurrencyCalculator();
        public static ResponseTimeDistributionForRequestsPerConcurrencyCalculator GetInstance() { return _instance; }

        private readonly object _lock = new object();

        private ResponseTimeDistributionForRequestsPerConcurrencyCalculator() { }

        public override DataTable Get(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stressTestIds) {
            ConcurrentDictionary<string, DataTable> data = GetData(databaseActions, cancellationToken, stressTestIds);
            if (cancellationToken.IsCancellationRequested) return null;

            DataTable results = GetResults(data, cancellationToken);
            data = null;
            if (cancellationToken.IsCancellationRequested) return null;

            DataView dv = results.DefaultView;
            dv.Sort = "Concurrency, Time to last byte (s)";
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

            DataTable[] parts = GetRequestResultsPerRunThreaded(databaseActions, cancellationToken, runResults, 4, "Id", "TimeToLastByteInTicks", "RunResultId");
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

            DataTable responseTimeDistributionPerConcurrency = CreateEmptyDataTable("DistributionPerConcurrency", "Stress test", "ConcurrencyResultId", "Concurrency", "Time to last byte (s)", "Count");

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

                    var ttlbInS = new List<double>();

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

                        if (requestResults != null)
                            foreach (DataRow rerRow in requestResults) {
                                if (cancellationToken.IsCancellationRequested) loopState.Break();

                                double ttlb = Convert.ToDouble((long)rerRow["TimeToLastByteInTicks"]) / (TimeSpan.TicksPerSecond);
                                ttlb = Math.Round(ttlb, 1, MidpointRounding.AwayFromZero);

                                //Round to 500 ms.
                                //ttlb *= 2;
                                //ttlb = Math.Round(ttlb, MidpointRounding.AwayFromZero);
                                //ttlb /= 2;

                                ttlbInS.Add(ttlb);
                            }
                    }

                    Dictionary<double, long> responseTimeDistribution = DistributionCalculator<double>.GetEntriesAndCounts(ttlbInS);
                    responseTimeDistribution = DistributionCalculator<double>.EvenOutRanges(responseTimeDistribution, 0.1d);
                    lock (_lock)
                        foreach (var kvp in responseTimeDistribution)
                            responseTimeDistributionPerConcurrency.Rows.Add(stressTest, concurrencyResultId, concurrency, kvp.Key, kvp.Value);
                });
            }

            return responseTimeDistributionPerConcurrency;
        }
    }

    internal sealed class ResponseTimeDistributionForUserActionsPerConcurrencyCalculator : BaseResultSetCalculator {
        private static ResponseTimeDistributionForUserActionsPerConcurrencyCalculator _instance = new ResponseTimeDistributionForUserActionsPerConcurrencyCalculator();
        public static ResponseTimeDistributionForUserActionsPerConcurrencyCalculator GetInstance() { return _instance; }

        private readonly object _lock = new object();

        private ResponseTimeDistributionForUserActionsPerConcurrencyCalculator() { }

        public override DataTable Get(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stressTestIds) {
            ConcurrentDictionary<string, DataTable> data = GetData(databaseActions, cancellationToken, stressTestIds);
            if (cancellationToken.IsCancellationRequested) return null;

            DataTable results = GetResults(data, cancellationToken);
            data = null;
            if (cancellationToken.IsCancellationRequested) return null;

            DataView dv = results.DefaultView;
            dv.Sort = "Time to last byte (s)";
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

            DataTable[] parts = GetRequestResultsPerRunThreaded(databaseActions, cancellationToken, runResults, 4, "Id", "Rerun", "VirtualUser", "UserAction", "InParallelWithPrevious", "TimeToLastByteInTicks", "DelayInMilliseconds", "RunResultId");
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

            DataTable responseTimeDistributionPerConcurrency = CreateEmptyDataTable("DistributionPerConcurrency", "Stress test", "ConcurrencyResultId", "Concurrency", "Time to last byte (s)", "Count");

            foreach (DataRow stressTestsRow in stressTests) {
                if (cancellationToken.IsCancellationRequested) return null;

                int stressTestId = (int)stressTestsRow.ItemArray[0];

                DataRow[] stressTestResults = data["stresstestresults"].Select(string.Format("StressTestId={0}", stressTestId));
                if (stressTestResults == null || stressTestResults.Length == 0) continue;
                int stressTestResultId = (int)stressTestResults[0].ItemArray[0];

                string stressTest = string.Format("{0} {1}", stressTestsRow.ItemArray[1], stressTestsRow.ItemArray[2]);

                DataRow[] concurrencyResults = data["concurrencyresults"].Select(string.Format("StressTestResultId={0}", stressTestResultId));
                if (concurrencyResults == null || concurrencyResults.Length == 0) continue;

                foreach (DataRow crRow in concurrencyResults) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    int concurrencyResultId = (int)crRow.ItemArray[0];
                    int concurrency = (int)crRow.ItemArray[1];

                    var ttlbInS = new List<double>();

                    //Place the request results under the right virtual user and the right user action
                    var userActions = new SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<RequestResult>>>>>(); // <VirtualUser,<UserAction, RequestResult

                    DataRow[] runResults = data["runresults"].Select(string.Format("ConcurrencyResultId={0}", concurrencyResultId));
                    if (runResults == null || runResults.Length == 0) continue;
                    foreach (DataRow rrRow in runResults) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        int runResultId = (int)rrRow.ItemArray[0];

                        //Get the request results containing requests with the given run result id.
                        var requestResults = new DataRow[0];
                        for (int rerDataIndex = 0; rerDataIndex != requestResultsData.Length; rerDataIndex++) {
                            requestResults = requestResultsData[rerDataIndex].Select(string.Format("RunResultId={0}", runResultId));
                            if (requestResults.Length != 0)
                                break;
                        }

                        if (requestResults != null && requestResults.Length != 0) {
                            //Keeping reruns in mind (break on last)
                            int runs = ((int)rrRow.ItemArray[1]) + 1;
                            //var userActionsMap = new ConcurrentDictionary<string, string>(); //Map duplicate user actions to the original ones, if need be.
                            for (int reRun = 0; reRun != runs; reRun++) {

                                var uas = new ConcurrentDictionary<string, ConcurrentDictionary<string, SynchronizedCollection<RequestResult>>>(); // <VirtualUser,<UserAction, RequestResult

                                Parallel.ForEach(requestResults.AsEnumerable(), (rerRow, loopState2) => {
                                    //foreach (DataRow rerRow in requestResults) {
                                    if (cancellationToken.IsCancellationRequested) loopState2.Break();
                                    //if (cancellationToken.IsCancellationRequested) return null;
                                    if ((int)rerRow["Rerun"] == reRun) {
                                        string virtualUser = rerRow["VirtualUser"] + "-" + reRun; //Make "virtual" virtual users :), handy way to make a correct calculation doing it like this.
                                        string userAction = rerRow["UserAction"] as string;

                                        var requestResult = new RequestResult() {
                                            TimeToLastByteInTicks = (long)rerRow["TimeToLastByteInTicks"], DelayInMilliseconds = (int)rerRow["DelayInMilliseconds"]
                                        };

                                        if (!uas.ContainsKey(virtualUser)) uas.TryAdd(virtualUser, new ConcurrentDictionary<string, SynchronizedCollection<RequestResult>>());
                                        if (!uas[virtualUser].ContainsKey(virtualUser)) uas[virtualUser].TryAdd(userAction, new SynchronizedCollection<RequestResult>());

                                        uas[virtualUser][userAction].Add(requestResult);
                                    }
                                }
                                );

                                //Flatten / export collection.
                                foreach (var item in uas) {
                                    if (cancellationToken.IsCancellationRequested) return null;

                                    var kvp = new KeyValuePair<string, SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<RequestResult>>>>(item.Key, new SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<RequestResult>>>());
                                    foreach (string userAction in uas[item.Key].Keys) {
                                        var rers = uas[item.Key][userAction];
                                        kvp.Value.Add(new KeyValuePair<string, SynchronizedCollection<RequestResult>>(userAction, rers));
                                    }
                                    userActions.Add(kvp);
                                }
                            }
                        }
                    }
                    if (cancellationToken.IsCancellationRequested) return null;

                    //Determine following for each user action "TimeToLastByteInTicks"
                    var userActionResultsList = new ConcurrentBag<long>();

                    for (int i = 0; i != userActions.Count; i++) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        foreach (var kvp in userActions[i].Value) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            long ttlb = 0;
                            int delay = 0;

                            var rers = kvp.Value;

                            RequestResult prevRequestResult = null; //For parallel request calculations
                            for (int j = 0; j != rers.Count; j++) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                var rer = rers[j];

                                if (rer.DelayInMilliseconds != 0) delay = rer.DelayInMilliseconds;

                                if (rer.InParallelWithPrevious && prevRequestResult != null) { //For parallel requests the total time to last byte in a virtual result is calculated differently. The longest time counts for a parallel set.
                                    long diffTtlb = (rer.SentAt.AddTicks(rer.TimeToLastByteInTicks) - prevRequestResult.SentAt.AddTicks(prevRequestResult.TimeToLastByteInTicks)).Ticks;
                                    if (diffTtlb > 0.0) ttlb += diffTtlb;
                                } else {
                                    ttlb += rer.TimeToLastByteInTicks;
                                }
                            }
                            userActionResultsList.Add(ttlb);
                        }
                    }

                    if (cancellationToken.IsCancellationRequested) return null;

                    foreach (long userActionResultsListEntry in userActionResultsList) {
                        double ttlb = Convert.ToDouble(userActionResultsListEntry) / (TimeSpan.TicksPerSecond);
                        ttlb = Math.Round(ttlb, 1, MidpointRounding.AwayFromZero);

                        //        //Round to 500 ms.
                        //        //ttlb *= 2;
                        //        //ttlb = Math.Round(ttlb, MidpointRounding.AwayFromZero);
                        //        //ttlb /= 2;

                        ttlbInS.Add(ttlb);
                    }


                    Dictionary<double, long> responseTimeDistribution = DistributionCalculator<double>.GetEntriesAndCounts(ttlbInS);
                    responseTimeDistribution = DistributionCalculator<double>.EvenOutRanges(responseTimeDistribution, 0.1d);
                    lock (_lock)
                        foreach (var kvp in responseTimeDistribution)
                            responseTimeDistributionPerConcurrency.Rows.Add(stressTest, concurrencyResultId, concurrency, kvp.Key, kvp.Value);
                }
            }

            return responseTimeDistributionPerConcurrency;
        }
    }
}
