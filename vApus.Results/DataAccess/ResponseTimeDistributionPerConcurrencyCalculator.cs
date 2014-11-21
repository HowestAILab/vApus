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
    internal sealed class ResponseTimeDistributionForLogEntriesPerConcurrencyCalculator : BaseResultSetCalculator {
        private static ResponseTimeDistributionForLogEntriesPerConcurrencyCalculator _instance = new ResponseTimeDistributionForLogEntriesPerConcurrencyCalculator();
        public static ResponseTimeDistributionForLogEntriesPerConcurrencyCalculator GetInstance() { return _instance; }

        private readonly object _lock = new object();

        private ResponseTimeDistributionForLogEntriesPerConcurrencyCalculator() { }

        public override DataTable Get(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stresstestIds) {
            ConcurrentDictionary<string, DataTable> data = GetData(databaseActions, cancellationToken, stresstestIds);
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

            DataTable runResults = ReaderAndCombiner.GetRunResults(cancellationToken, databaseActions, concurrencyResultIds, "Id", "ConcurrencyResultId");
            if (cancellationToken.IsCancellationRequested) return null;

            data.TryAdd("runresults", runResults);

            DataTable[] parts = GetLogEntryResultsThreaded(databaseActions, cancellationToken, runResults, 4, "TimeToLastByteInTicks", "RunResultId");
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

            DataTable responseTimeDistributionPerConcurrency = CreateEmptyDataTable("DistributionPerConcurrency", "Stresstest", "ConcurrencyResultId", "Concurrency", "Time to last byte (s)", "Count");

            foreach (DataRow stresstestsRow in stresstests) {
                if (cancellationToken.IsCancellationRequested) return null;

                int stresstestId = (int)stresstestsRow.ItemArray[0];

                DataRow[] stresstestResults = data["stresstestresults"].Select(string.Format("StresstestId={0}", stresstestId));
                if (stresstestResults == null || stresstestResults.Length == 0) continue;
                int stresstestResultId = (int)stresstestResults[0].ItemArray[0];

                string stresstest = string.Format("{0} {1}", stresstestsRow.ItemArray[1], stresstestsRow.ItemArray[2]);

                DataRow[] concurrencyResults = data["concurrencyresults"].Select(string.Format("StresstestResultId={0}", stresstestResultId));
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

                        //Get the log entry results containing log entries with the given run result id.
                        var logEntryResults = new DataRow[0];
                        for (int lerDataIndex = 0; lerDataIndex != logEntryResultsData.Length; lerDataIndex++) {
                            logEntryResults = logEntryResultsData[lerDataIndex].Select(string.Format("RunResultId In({0})", runResultIds.Combine(", ")));
                            if (logEntryResults.Length != 0)
                                break;
                        }

                        if (logEntryResults != null)
                            foreach (DataRow lerRow in logEntryResults) {
                                if (cancellationToken.IsCancellationRequested) loopState.Break();

                                double ttlb = Convert.ToDouble((long)lerRow["TimeToLastByteInTicks"]) / (TimeSpan.TicksPerSecond);
                                ttlb = Math.Round(ttlb, 1, MidpointRounding.AwayFromZero);

                                //Round to 500 ms.
                                //ttlb *= 2;
                                //ttlb = Math.Round(ttlb, MidpointRounding.AwayFromZero);
                                //ttlb /= 2;

                                ttlbInS.Add(ttlb);
                            }
                    }

                    ConcurrentDictionary<double, long> responseTimeDistribution = DistributionCalculator<double>.GetEntriesAndCounts(ttlbInS);
                    lock (_lock)
                        foreach (var kvp in responseTimeDistribution)
                            responseTimeDistributionPerConcurrency.Rows.Add(stresstest, concurrencyResultId, concurrency, kvp.Key, kvp.Value);
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

        public override DataTable Get(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stresstestIds) {
            ConcurrentDictionary<string, DataTable> data = GetData(databaseActions, cancellationToken, stresstestIds);
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

            DataTable runResults = ReaderAndCombiner.GetRunResults(cancellationToken, databaseActions, concurrencyResultIds, "Id", "ConcurrencyResultId");
            if (cancellationToken.IsCancellationRequested) return null;

            data.TryAdd("runresults", runResults);

            DataTable[] parts = GetLogEntryResultsThreaded(databaseActions, cancellationToken, runResults, 4, "Rerun", "VirtualUser", "UserAction", "TimeToLastByteInTicks", "DelayInMilliseconds", "RunResultId");
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

            DataTable responseTimeDistributionPerConcurrency = CreateEmptyDataTable("DistributionPerConcurrency", "Stresstest", "ConcurrencyResultId", "Concurrency", "Time to last byte (s)", "Count");

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

                    var ttlbInS = new List<double>();

                    //Place the log entry results under the right virtual user and the right user action
                    var userActions = new SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<LogEntryResult>>>>>(); // <VirtualUser,<UserAction, LogEntryResult

                    DataRow[] runResults = data["runresults"].Select(string.Format("ConcurrencyResultId={0}", concurrencyResultId));
                    if (runResults == null || runResults.Length == 0) continue;
                    foreach (DataRow rrRow in runResults) {
                        if (cancellationToken.IsCancellationRequested) return null;

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
                            //var userActionsMap = new ConcurrentDictionary<string, string>(); //Map duplicate user actions to the original ones, if need be.
                            for (int reRun = 0; reRun != runs; reRun++) {

                                var uas = new ConcurrentDictionary<string, ConcurrentDictionary<string, SynchronizedCollection<LogEntryResult>>>(); // <VirtualUser,<UserAction, LogEntryResult

                                Parallel.ForEach(logEntryResults.AsEnumerable(), (lerRow, loopState2) => {
                                    //foreach (DataRow lerRow in logEntryResults) {
                                    if (cancellationToken.IsCancellationRequested) loopState2.Break();
                                    //if (cancellationToken.IsCancellationRequested) return null;
                                    if ((int)lerRow["Rerun"] == reRun) {
                                        string virtualUser = lerRow["VirtualUser"] + "-" + reRun; //Make "virtual" virtual users :), handy way to make a correct calculation doing it like this.
                                        string userAction = lerRow["UserAction"] as string;

                                        var logEntryResult = new LogEntryResult() {
                                            TimeToLastByteInTicks = (long)lerRow["TimeToLastByteInTicks"], DelayInMilliseconds = (int)lerRow["DelayInMilliseconds"]
                                        };

                                        if (!uas.ContainsKey(virtualUser)) uas.TryAdd(virtualUser, new ConcurrentDictionary<string, SynchronizedCollection<LogEntryResult>>());
                                        if (!uas[virtualUser].ContainsKey(virtualUser)) uas[virtualUser].TryAdd(userAction, new SynchronizedCollection<LogEntryResult>());

                                        uas[virtualUser][userAction].Add(logEntryResult);
                                    }
                                }
                                );

                                //Flatten / export collection.
                                foreach (var item in uas) {
                                    if (cancellationToken.IsCancellationRequested) return null;

                                    var kvp = new KeyValuePair<string, SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<LogEntryResult>>>>(item.Key, new SynchronizedCollection<KeyValuePair<string, SynchronizedCollection<LogEntryResult>>>());
                                    foreach (string userAction in uas[item.Key].Keys) {
                                        var lers = uas[item.Key][userAction];
                                        kvp.Value.Add(new KeyValuePair<string, SynchronizedCollection<LogEntryResult>>(userAction, lers));
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
                            int delay = -1;

                            var lers = kvp.Value;

                            for (int j = lers.Count - 1; j != -1; j--) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                var ler = lers[j];
                                if (delay == -1) {
                                    delay = ler.DelayInMilliseconds;
                                    ttlb = ler.TimeToLastByteInTicks;
                                } else {
                                    ttlb += ler.TimeToLastByteInTicks + ler.DelayInMilliseconds;
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


                    ConcurrentDictionary<double, long> responseTimeDistribution = DistributionCalculator<double>.GetEntriesAndCounts(ttlbInS);
                    lock (_lock)
                        foreach (var kvp in responseTimeDistribution)
                            responseTimeDistributionPerConcurrency.Rows.Add(stresstest, concurrencyResultId, concurrency, kvp.Key, kvp.Value);
                }
            }

            return responseTimeDistributionPerConcurrency;
        }
    }
}
