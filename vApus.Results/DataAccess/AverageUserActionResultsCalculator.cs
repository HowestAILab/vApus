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

        public override DataTable Get(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stresstestIds) {
            ConcurrentDictionary<string, DataTable> data = GetData(databaseActions, cancellationToken, stresstestIds);
            if (cancellationToken.IsCancellationRequested) return null;

            DataTable results = GetResults(data, cancellationToken);
            data = null;
            if (cancellationToken.IsCancellationRequested) return null;

            //DataView dv = results.DefaultView;
            //dv.Sort = "Concurrency";
            //results = dv.ToTable();

            return results;
        }
        protected override ConcurrentDictionary<string, DataTable> GetData(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stresstestIds) {
            var data = new ConcurrentDictionary<string, DataTable>();

            data.TryAdd("stresstests", ReaderAndCombiner.GetStresstests(cancellationToken, databaseActions, stresstestIds, "Id", "Stresstest", "Connection"));
            if (cancellationToken.IsCancellationRequested) return null;

            DataTable stresstestResults = ReaderAndCombiner.GetStresstestResults(cancellationToken, databaseActions, stresstestIds, "Id", "StresstestId");
            if (cancellationToken.IsCancellationRequested) return null;

            data.TryAdd("stresstestresults", stresstestResults);

            int[] stresstestResultIds = new int[stresstestResults.Rows.Count];
            for (int i = 0; i != stresstestResultIds.Length; i++)
                stresstestResultIds[i] = (int)stresstestResults.Rows[i][0];

            DataTable concurrencyResults = ReaderAndCombiner.GetConcurrencyResults(cancellationToken, databaseActions, stresstestResultIds, new string[] { "Id", "Concurrency", "StresstestResultId" });
            if (cancellationToken.IsCancellationRequested) return null;

            data.TryAdd("concurrencyresults", concurrencyResults);

            int[] concurrencyResultIds = new int[concurrencyResults.Rows.Count];
            for (int i = 0; i != concurrencyResultIds.Length; i++)
                concurrencyResultIds[i] = (int)concurrencyResults.Rows[i][0];

            DataTable runResults = ReaderAndCombiner.GetRunResults(cancellationToken, databaseActions, concurrencyResultIds, "Id", "RerunCount", "ConcurrencyResultId");
            if (cancellationToken.IsCancellationRequested) return null;

            data.TryAdd("runresults", runResults);

            DataTable[] parts = GetLogEntryResultsThreaded(databaseActions, cancellationToken, runResults, 4, "Rerun", "VirtualUser", "UserAction", "SameAsLogEntryIndex", "LogEntryIndex", "TimeToLastByteInTicks", "DelayInMilliseconds", "Error", "RunResultId");
            //A merge is way to slow. Needed rows will be extracted when getting results.
            for (int i = 0; i != parts.Length; i++)
                data.TryAdd("logentryresults" + i, parts[i]);
            parts = null;

            return data;
        }

        private DataTable GetResults(ConcurrentDictionary<string, DataTable> data, CancellationToken cancellationToken) {
            DataRow[] stresstests = data["stresstests"].Select();
            if (stresstests == null || stresstests.Length == 0) return null;

            //Get all the log entry result datatables.
            var logEntryResultsDataList = new List<DataTable>();
            foreach (string key in data.Keys)
                if (key.StartsWith("logentryresults"))
                    logEntryResultsDataList.Add(data[key]);
            DataTable[] logEntryResultsData = logEntryResultsDataList.ToArray();

            var averageUserActionResults = CreateEmptyDataTable("AverageUserActionResults", "Stresstest", "ConcurrencyId", "Concurrency", "User Action", "Avg. Response Time (ms)",
                           "Max. Response Time (ms)", "95th Percentile of the Response Times (ms)", "Avg. Delay (ms)", "Errors");

            foreach (DataRow stresstestsRow in stresstests) {
                if (cancellationToken.IsCancellationRequested) return null;

                int stresstestId = (int)stresstestsRow.ItemArray[0];

                DataRow[] stresstestResults = data["stresstestresults"].Select(string.Format("StresstestId={0}", stresstestId));
                if (stresstestResults == null || stresstestResults.Length == 0) continue;
                int stresstestResultId = (int)stresstestResults[0].ItemArray[0];

                string stresstest = string.Format("{0} {1}", stresstestsRow.ItemArray[1], stresstestsRow.ItemArray[2]);

                DataRow[] concurrencyResults = data["concurrencyresults"].Select(string.Format("StresstestResultId={0}", stresstestResultId));
                if (concurrencyResults == null || concurrencyResults.Length == 0) continue;

                foreach (DataRow crRow in concurrencyResults) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    int concurrencyResultId = (int)crRow.ItemArray[0];
                    int concurrency = (int)crRow.ItemArray[1];

                    DataRow[] runResults = data["runresults"].Select(string.Format("ConcurrencyResultId={0}", concurrencyResultId));
                    if (runResults == null || runResults.Length == 0) continue;

                    //Place the log entry results under the right virtual user and the right user action
                    var userActions = new SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<LogEntryResult>>>>>(); // <VirtualUser,<UserAction, LogEntryResult
                    Parallel.ForEach(runResults, (rrRow, loopState) => {
                        //foreach (DataRow rrRow in runResults) {
                        if (cancellationToken.IsCancellationRequested) loopState.Break();

                        int runResultId = (int)rrRow.ItemArray[0];

                        //Get the log entry results containing log entries with the given run result id.
                        var logEntryResults = new DataRow[0];
                        for (int lerDataIndex = 0; lerDataIndex != logEntryResultsData.Length; lerDataIndex++) {
                            logEntryResults = logEntryResultsData[lerDataIndex].Select(string.Format("RunResultId={0}", runResultId));
                            if (logEntryResults.Length != 0)
                                break;
                        }

                        if (logEntryResults != null && logEntryResults.Length != 0) {
                            //Keeping reruns in mind (break on last)
                            int runs = ((int)rrRow.ItemArray[1]) + 1;
                            var userActionsMap = new ConcurrentDictionary<string, string>(); //Map duplicate user actions to the original ones, if need be.
                            for (int reRun = 0; reRun != runs; reRun++) {
                                if (cancellationToken.IsCancellationRequested) loopState.Break();

                                var uas = new ConcurrentDictionary<string, ConcurrentDictionary<string, SynchronizedCollection<LogEntryResult>>>(); // <VirtualUser,<UserAction, LogEntryResult

                                Parallel.ForEach(logEntryResults.AsEnumerable(), (lerRow, loopState2) => {
                                    //foreach (DataRow lerRow in logEntryResults) {
                                    if (cancellationToken.IsCancellationRequested) loopState2.Break();
                                    //if (cancellationToken.IsCancellationRequested) return null;
                                    if ((int)lerRow["Rerun"] == reRun) {
                                        string virtualUser = lerRow["VirtualUser"] + "-" + reRun; //Make "virtual" virtual users :), handy way to make a correct average doing it like this.
                                        string userAction = lerRow["UserAction"] as string;

                                        string logEntryIndex = lerRow["SameAsLogEntryIndex"] as string; //Combine results when using distribe like this.
                                        if (logEntryIndex == string.Empty) {
                                            logEntryIndex = lerRow["LogEntryIndex"] as string;

                                            //Make sure we have all the user actions before averages are calcullated, otherwise the duplicated user action names can be used.
                                            //Map using the log entry index
                                            userActionsMap.TryAdd(logEntryIndex, userAction);
                                        }

                                        var logEntryResult = new LogEntryResult() {
                                            LogEntryIndex = logEntryIndex, TimeToLastByteInTicks = (long)lerRow["TimeToLastByteInTicks"], DelayInMilliseconds = (int)lerRow["DelayInMilliseconds"],
                                            Error = lerRow["Error"] as string
                                        };

                                        if (!uas.ContainsKey(virtualUser)) uas.TryAdd(virtualUser, new ConcurrentDictionary<string, SynchronizedCollection<LogEntryResult>>());
                                        if (!uas[virtualUser].ContainsKey(virtualUser)) uas[virtualUser].TryAdd(userAction, new SynchronizedCollection<LogEntryResult>());

                                        uas[virtualUser][userAction].Add(logEntryResult);
                                    }
                                }
                                );
                                if (cancellationToken.IsCancellationRequested) loopState.Break();

                                //Map duplicate user actions to the original ones, if need be.
                                Parallel.ForEach(uas, (item, loopState2) => {
                                    //foreach (string virtualUser in uas.Keys) {
                                    if (cancellationToken.IsCancellationRequested) loopState2.Break();
                                    //if (cancellationToken.IsCancellationRequested) return null;

                                    var kvp = new KeyValuePair<string, SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<LogEntryResult>>>>(item.Key, new SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<LogEntryResult>>>());
                                    foreach (string userAction in uas[item.Key].Keys) {
                                        if (cancellationToken.IsCancellationRequested) loopState2.Break();

                                        string mappedUserAction = userAction;
                                        var lers = uas[item.Key][userAction];
                                        if (lers.Count != 0) {
                                            var leri = lers[0].LogEntryIndex;
                                            if (userActionsMap.ContainsKey(leri)) mappedUserAction = userActionsMap[leri];
                                        }
                                        kvp.Value.Add(new KeyValuePair<string, SynchronizedCollection<LogEntryResult>>(mappedUserAction, lers));
                                    }
                                    userActions.Add(kvp);
                                }
                                );
                            }
                        }
                    }
                    );
                    if (cancellationToken.IsCancellationRequested) return null;

                    //Determine following for each user action "UserAction", "TimeToLastByteInTicks", "DelayInMilliseconds", "Errors"
                    var userActionResultsList = new ConcurrentBag<object[]>();

                    Parallel.For(0, userActions.Count, (i, loopState) => {
                        //for (int i = 0; i != userActions.Count; i++) {
                        if (cancellationToken.IsCancellationRequested) loopState.Break();

                        foreach (var kvp in userActions[i].Value) {
                            if (cancellationToken.IsCancellationRequested) loopState.Break();

                            long ttlb = 0;
                            int delay = -1;
                            long ers = 0;

                            string userAction = kvp.Key;
                            var lers = kvp.Value;

                            for (int j = lers.Count - 1; j != -1; j--) {
                                if (cancellationToken.IsCancellationRequested) loopState.Break();

                                var ler = lers[j];
                                if (delay == -1) {
                                    delay = ler.DelayInMilliseconds;
                                    ttlb = ler.TimeToLastByteInTicks;
                                } else {
                                    ttlb += ler.TimeToLastByteInTicks + ler.DelayInMilliseconds;
                                }
                                if (!string.IsNullOrEmpty(ler.Error)) ++ers;
                            }
                            userActionResultsList.Add(new object[] { userAction, ttlb, delay, ers });
                        }
                    }
                    );
                    if (cancellationToken.IsCancellationRequested) return null;

                    var userActionResults = userActionResultsList.ToArray();

                    var uniqueUserActionCounts = new ConcurrentDictionary<string, int>(); //To make a correct average.

                    Parallel.ForEach(userActionResults, (uarRow, loopState) => {
                        //foreach (object[] uarRow in userActionResults) {
                        if (cancellationToken.IsCancellationRequested) loopState.Break();

                        string userAction = uarRow[0] as string;

                        if (uniqueUserActionCounts.ContainsKey(userAction)) ++uniqueUserActionCounts[userAction];
                        else uniqueUserActionCounts.TryAdd(userAction, 1);
                    }
                    );
                    if (cancellationToken.IsCancellationRequested) return null;

                    //Finally the averages
                    //The key of the entries for following collections are user actions.
                    var avgTimeToLastByteInTicks = new Dictionary<string, double>();
                    var maxTimeToLastByteInTicks = new Dictionary<string, long>();
                    var timeToLastBytesInTicks = new Dictionary<string, List<long>>();
                    var percTimeToLastBytesInTicks = new ConcurrentDictionary<string, long>();

                    var avgDelay = new Dictionary<string, double>();
                    var errors = new Dictionary<string, long>();

                    foreach (object[] row in userActionResults) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        string userAction = row[0] as string;
                        long ttlb = (long)row[1];
                        int delay = (int)row[2];
                        long ers = (long)row[3];

                        if (avgTimeToLastByteInTicks.ContainsKey(userAction)) avgTimeToLastByteInTicks[userAction] += (((double)ttlb) / uniqueUserActionCounts[userAction]);
                        else avgTimeToLastByteInTicks.Add(userAction, (((double)ttlb) / uniqueUserActionCounts[userAction]));

                        if (maxTimeToLastByteInTicks.ContainsKey(userAction)) { if (maxTimeToLastByteInTicks[userAction] < ttlb) maxTimeToLastByteInTicks[userAction] = ttlb; } else maxTimeToLastByteInTicks.Add(userAction, ttlb);

                        if (!timeToLastBytesInTicks.ContainsKey(userAction)) timeToLastBytesInTicks.Add(userAction, new List<long>(uniqueUserActionCounts[userAction]));
                        timeToLastBytesInTicks[userAction].Add(ttlb);

                        if (avgDelay.ContainsKey(userAction)) avgDelay[userAction] += (((double)delay) / uniqueUserActionCounts[userAction]);
                        else avgDelay.Add(userAction, ((double)delay) / uniqueUserActionCounts[userAction]);

                        if (errors.ContainsKey(userAction)) errors[userAction] += ers;
                        else errors.Add(userAction, ers);
                    }

                    //95th percentile
                    Parallel.ForEach(timeToLastBytesInTicks, (item, loopState) => {
                        if (cancellationToken.IsCancellationRequested) loopState.Break();

                        percTimeToLastBytesInTicks.TryAdd(item.Key, PercentileCalculator<long>.Get(timeToLastBytesInTicks[item.Key], 95));
                    });
                    if (cancellationToken.IsCancellationRequested) return null;

                    //Sort the user actions
                    List<string> sortedUserActions = avgTimeToLastByteInTicks.Keys.ToList();
                    sortedUserActions.Sort(UserActionComparer.GetInstance());

                    //Add the sorted user actions to the whole.
                    foreach (string userAction in sortedUserActions) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        averageUserActionResults.Rows.Add(stresstest, concurrencyResultId, concurrency, userAction,
                            Math.Round(avgTimeToLastByteInTicks[userAction] / TimeSpan.TicksPerMillisecond, MidpointRounding.AwayFromZero),
                            maxTimeToLastByteInTicks[userAction] / TimeSpan.TicksPerMillisecond,
                            percTimeToLastBytesInTicks[userAction] / TimeSpan.TicksPerMillisecond,
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

            private const string LOG = "Log ";
            private const string UA = "User Action ";
            private const char COLON = ':';

            private UserActionComparer() { }

            public int Compare(string x, string y) {
                if (x.StartsWith(LOG)) { //Backwards compatible.

                    int xColonUa = x.IndexOf(COLON);
                    if (xColonUa == -1) xColonUa = x.IndexOf(UA) - 1;

                    int yColonUa = y.IndexOf(COLON);
                    if (yColonUa == -1) yColonUa = y.IndexOf(UA) - 1;

                    int logX, logY;
                    if (!int.TryParse(x.Substring(LOG.Length, xColonUa - LOG.Length), out logX)) {
                        xColonUa = x.IndexOf(UA) - 1;
                        int.TryParse(x.Substring(LOG.Length, xColonUa - LOG.Length), out logX);
                    }
                    if (!int.TryParse(x.Substring(LOG.Length, yColonUa - LOG.Length), out logY)) {
                        yColonUa = y.IndexOf(UA) - 1;
                        int.TryParse(x.Substring(LOG.Length, yColonUa - LOG.Length), out logY);
                    }

                    if (logX > logY) return 1;
                    if (logY < logX) return -1;

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
