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
    internal sealed class AverageUserActionResultsCalculator : BaseResultSetCalculator {
        private static AverageUserActionResultsCalculator _instance = new AverageUserActionResultsCalculator();
        public static AverageUserActionResultsCalculator GetInstance() { return _instance; }
        private AverageUserActionResultsCalculator() { }

        public override DataTable Get(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stressTestIds) {
            ConcurrentDictionary<string, DataTable> data = GetData(databaseActions, cancellationToken, stressTestIds);
            if (cancellationToken.IsCancellationRequested) return null;

            DataTable results = GetResults(data, cancellationToken);
            data = null;
            if (cancellationToken.IsCancellationRequested) return null;

            //DataView dv = results.DefaultView;
            //dv.Sort = "Concurrency";
            //results = dv.ToTable();

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

            DataTable runResults = ReaderAndCombiner.GetRunResults(cancellationToken, databaseActions, concurrencyResultIds, "Id", "RerunCount", "ConcurrencyResultId");
            if (cancellationToken.IsCancellationRequested) return null;

            data.TryAdd("runresults", runResults);

            DataTable[] parts = GetRequestResultsThreaded(databaseActions, cancellationToken, runResults, 4, "Rerun", "VirtualUser", "UserAction", "SameAsRequestIndex", "RequestIndex", "TimeToLastByteInTicks", "DelayInMilliseconds", "Length(Error) As Error", "RunResultId");
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

            var averageUserActionResults = CreateEmptyDataTable("AverageUserActionResults", "Stress test", "ConcurrencyId", "Concurrency", "User action", "Avg. response time (ms)",
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

                foreach (DataRow crRow in concurrencyResults) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    int concurrencyResultId = (int)crRow.ItemArray[0];
                    int concurrency = (int)crRow.ItemArray[1];

                    DataRow[] runResults = data["runresults"].Select(string.Format("ConcurrencyResultId={0}", concurrencyResultId));
                    if (runResults == null || runResults.Length == 0) continue;

                    //Place the request results under the right virtual user and the right user action
                    var userActions = new SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<RequestResult>>>>>(); // <VirtualUser,<UserAction, RequestResult
                    //Parallel.ForEach(runResults, (rrRow, loopState) => {
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
                            var userActionsMap = new ConcurrentDictionary<string, string>(); //Map duplicate user actions to the original ones, if need be. Duplicate user names (Other names / indices for reruns) --> map to original names for correct avg calculation.
                            for (int reRun = 0; reRun != runs; reRun++) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                var uas = new ConcurrentDictionary<string, ConcurrentDictionary<string, SynchronizedCollection<RequestResult>>>(); // <VirtualUser,<UserAction, RequestResult

                                Parallel.ForEach(requestResults.AsEnumerable(), (rerRow, loopState2) => {
                                    //foreach (DataRow rerRow in requestResults) {
                                    if (cancellationToken.IsCancellationRequested) loopState2.Break();
                                    //if (cancellationToken.IsCancellationRequested) return null;
                                    if ((int)rerRow["Rerun"] == reRun) {
                                        string virtualUser = rerRow["VirtualUser"] + "-" + reRun; //Make "virtual" virtual users :), handy way to make a correct average doing it like this.
                                        string userAction = rerRow["UserAction"] as string;

                                        string requestIndex = rerRow["SameAsRequestIndex"] as string; //Combine results when using distribe like this.
                                        if (requestIndex == string.Empty) {
                                            requestIndex = rerRow["RequestIndex"] as string;

                                            //Make sure we have all the user actions before averages are calculated, otherwise the duplicated user action names can be used.
                                            //Map using the request index
                                            userActionsMap.TryAdd(requestIndex, userAction);
                                        }

                                        var requestResult = new RequestResult() {
                                            RequestIndex = requestIndex, TimeToLastByteInTicks = (long)rerRow["TimeToLastByteInTicks"], DelayInMilliseconds = (int)rerRow["DelayInMilliseconds"],
                                            Error = (long)rerRow["Error"] == 0 ? null : "-"
                                        };

                                        if (!uas.ContainsKey(virtualUser)) uas.TryAdd(virtualUser, new ConcurrentDictionary<string, SynchronizedCollection<RequestResult>>());
                                        if (!uas[virtualUser].ContainsKey(virtualUser)) uas[virtualUser].TryAdd(userAction, new SynchronizedCollection<RequestResult>());

                                        uas[virtualUser][userAction].Add(requestResult);
                                    }
                                }
                                );
                                if (cancellationToken.IsCancellationRequested) return null;

                                //Map duplicate user actions to the original ones, if need be.
                                //Parallel.ForEach(uas, (item, loopState2) => {
                                foreach (var item in uas) {
                                    //if (cancellationToken.IsCancellationRequested) loopState2.Break();
                                    //if (cancellationToken.IsCancellationRequested) return null;

                                    var kvp = new KeyValuePair<string, SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<RequestResult>>>>(item.Key, new SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<RequestResult>>>());
                                    foreach (string userAction in uas[item.Key].Keys) {
                                        //if (cancellationToken.IsCancellationRequested) loopState2.Break();

                                        string mappedUserAction = userAction;
                                        var rers = uas[item.Key][userAction];
                                        if (rers.Count != 0) {
                                            var reri = rers[0].RequestIndex;
                                            if (userActionsMap.ContainsKey(reri)) mappedUserAction = userActionsMap[reri];
                                        }
                                        kvp.Value.Add(new KeyValuePair<string, SynchronizedCollection<RequestResult>>(mappedUserAction, rers));
                                    }
                                    userActions.Add(kvp);
                                }
                                //);
                            }
                        }
                    }
                    //);
                    if (cancellationToken.IsCancellationRequested) return null;

                    //Determine following for each user action "UserAction", "TimeToLastByteInTicks", "DelayInMilliseconds", "Errors"
                    var userActionResultsList = new ConcurrentBag<object[]>();

                    //Parallel.For(0, userActions.Count, (i, loopState) => {
                    for (int i = 0; i != userActions.Count; i++) {
                        //if (cancellationToken.IsCancellationRequested) loopState.Break();
                        if (cancellationToken.IsCancellationRequested) return null;

                        foreach (var kvp in userActions[i].Value) {
                            //if (cancellationToken.IsCancellationRequested) loopState.Break();
                            if (cancellationToken.IsCancellationRequested) return null;

                            long ttlb = 0;
                            int delay = -1;
                            long ers = 0;

                            string userAction = kvp.Key;
                            var rers = kvp.Value;

                            for (int j = rers.Count - 1; j != -1; j--) {
                                //if (cancellationToken.IsCancellationRequested) loopState.Break();
                                if (cancellationToken.IsCancellationRequested) return null;

                                var rer = rers[j];
                                if (delay == -1) {
                                    delay = rer.DelayInMilliseconds;
                                    ttlb = rer.TimeToLastByteInTicks;
                                } else {
                                    ttlb += rer.TimeToLastByteInTicks + rer.DelayInMilliseconds;
                                }
                                if (!string.IsNullOrEmpty(rer.Error)) ++ers;
                            }
                            userActionResultsList.Add(new object[] { userAction, ttlb, delay, ers });
                        }
                    }
                    // );
                    if (cancellationToken.IsCancellationRequested) return null;

                    var userActionResults = userActionResultsList.ToArray();

                    var uniqueUserActionCounts = new ConcurrentDictionary<string, int>(); //To make a correct average.

                    //Parallel.ForEach(userActionResults, (uarRow, loopState) => {
                    foreach (object[] uarRow in userActionResults) {
                        //if (cancellationToken.IsCancellationRequested) loopState.Break();
                        if (cancellationToken.IsCancellationRequested) return null;

                        string userAction = uarRow[0] as string;

                        if (uniqueUserActionCounts.ContainsKey(userAction)) ++uniqueUserActionCounts[userAction];
                        else uniqueUserActionCounts.TryAdd(userAction, 1);
                    }
                    // );
                    if (cancellationToken.IsCancellationRequested) return null;

                    //Finally the averages
                    //The key of the entries for following collections are user actions.
                    var avgTimeToLastByteInTicks = new Dictionary<string, double>();
                    var maxTimeToLastByteInTicks = new Dictionary<string, long>();
                    var timeToLastBytesInTicks = new Dictionary<string, List<long>>();
                    var perc95TimeToLastBytesInTicks = new ConcurrentDictionary<string, long>();
                    var perc99TimeToLastBytesInTicks = new ConcurrentDictionary<string, long>();
                    var avgTop5TimeToLastBytesInTicks = new ConcurrentDictionary<string, long>();

                    var avgDelay = new Dictionary<string, double>();
                    var errors = new Dictionary<string, long>();

                    foreach (object[] row in userActionResults) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        string userAction = row[0] as string;
                        long ttlb = (long)row[1];
                        double delay = Convert.ToDouble((int)row[2]);
                        long ers = (long)row[3];

                        if (avgTimeToLastByteInTicks.ContainsKey(userAction)) avgTimeToLastByteInTicks[userAction] += (((double)ttlb) / uniqueUserActionCounts[userAction]);
                        else avgTimeToLastByteInTicks.Add(userAction, (((double)ttlb) / uniqueUserActionCounts[userAction]));

                        if (maxTimeToLastByteInTicks.ContainsKey(userAction)) { if (maxTimeToLastByteInTicks[userAction] < ttlb) maxTimeToLastByteInTicks[userAction] = ttlb; } else maxTimeToLastByteInTicks.Add(userAction, ttlb);

                        if (!timeToLastBytesInTicks.ContainsKey(userAction)) timeToLastBytesInTicks.Add(userAction, new List<long>(uniqueUserActionCounts[userAction]));
                        timeToLastBytesInTicks[userAction].Add(ttlb);

                        if (avgDelay.ContainsKey(userAction)) avgDelay[userAction] += (delay / uniqueUserActionCounts[userAction]);
                        else avgDelay.Add(userAction, delay / uniqueUserActionCounts[userAction]);

                        if (errors.ContainsKey(userAction)) errors[userAction] += ers;
                        else errors.Add(userAction, ers);
                    }

                    //Percentiles / avg top 5.
                    Parallel.ForEach(timeToLastBytesInTicks, (item, loopState) => {
                        if (cancellationToken.IsCancellationRequested) loopState.Break();
                        IEnumerable<long> orderedValues;
                        perc95TimeToLastBytesInTicks.TryAdd(item.Key, PercentileCalculator<long>.Get(timeToLastBytesInTicks[item.Key], 95, out orderedValues));
                        perc99TimeToLastBytesInTicks.TryAdd(item.Key, PercentileCalculator<long>.Get(orderedValues, 99));

                        int top5Count = Convert.ToInt32(orderedValues.Count() * 0.05);
                        if (top5Count == 0)
                            avgTop5TimeToLastBytesInTicks.TryAdd(item.Key, orderedValues.FirstOrDefault());
                        else
                            avgTop5TimeToLastBytesInTicks.TryAdd(item.Key, (long)orderedValues.Take(top5Count).Average());

                    });
                    if (cancellationToken.IsCancellationRequested) return null;

                    //Sort the user actions
                    List<string> sortedUserActions = avgTimeToLastByteInTicks.Keys.ToList();
                    sortedUserActions.Sort(UserActionComparer.GetInstance());

                    //Add the sorted user actions to the whole.
                    foreach (string userAction in sortedUserActions) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        averageUserActionResults.Rows.Add(stressTest, concurrencyResultId, concurrency, userAction,
                            Math.Round(avgTimeToLastByteInTicks[userAction] / TimeSpan.TicksPerMillisecond, MidpointRounding.AwayFromZero),
                            maxTimeToLastByteInTicks[userAction] / TimeSpan.TicksPerMillisecond,
                            perc95TimeToLastBytesInTicks[userAction] / TimeSpan.TicksPerMillisecond,
                            perc99TimeToLastBytesInTicks[userAction] / TimeSpan.TicksPerMillisecond,
                            avgTop5TimeToLastBytesInTicks[userAction] / TimeSpan.TicksPerMillisecond,
                            Math.Round(avgDelay[userAction], MidpointRounding.AwayFromZero),
                            errors[userAction]);
                    }
                }
            }

            return averageUserActionResults;
        }

        private class UserActionComparer : IComparer<string> {
            private static readonly UserActionComparer _userActionComparer = new UserActionComparer();
            public static UserActionComparer GetInstance() { return _userActionComparer; }

            private const string SCENARIO = "Scenario ";
            private const string UA = "User action ";
            private const char COLON = ':';

            private UserActionComparer() { }

            public int Compare(string x, string y) {
                if (x.StartsWith(SCENARIO)) { //Backwards compatible.

                    int xColonUa = x.IndexOf(COLON);
                    if (xColonUa == -1) xColonUa = x.IndexOf(UA) - 1;

                    int yColonUa = y.IndexOf(COLON);
                    if (yColonUa == -1) yColonUa = y.IndexOf(UA) - 1;

                    int scenarioX, scenarioY;
                    if (!int.TryParse(x.Substring(SCENARIO.Length, xColonUa - SCENARIO.Length), out scenarioX)) {
                        xColonUa = x.IndexOf(UA) - 1;
                        int.TryParse(x.Substring(SCENARIO.Length, xColonUa - SCENARIO.Length), out scenarioX);
                    }
                    if (!int.TryParse(x.Substring(SCENARIO.Length, yColonUa - SCENARIO.Length), out scenarioY)) {
                        yColonUa = y.IndexOf(UA) - 1;
                        int.TryParse(x.Substring(SCENARIO.Length, yColonUa - SCENARIO.Length), out scenarioY);
                    }

                    if (scenarioX > scenarioY) return 1;
                    if (scenarioY < scenarioX) return -1;

                    int xUA = x.IndexOf(UA);
                    int yUA = y.IndexOf(UA);

                    x = x.Substring(xUA);
                    y = y.Substring(yUA);
                }

                return UserActionCompare(x, y);
            }
            private int UserActionCompare(string x, string y) {
                x = x.Substring(UA.Length);
                y = y.Substring(UA.Length);

                x = x.Split(COLON)[0];
                y = y.Split(COLON)[0];

                int i = int.Parse(x);
                int j = int.Parse(y);

                return i.CompareTo(j);
            }
        }
    }
}
