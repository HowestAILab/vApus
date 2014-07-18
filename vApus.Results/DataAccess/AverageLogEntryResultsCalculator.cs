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
    internal sealed class AverageLogEntryResultsCalculator : BaseResultSetCalculator {
        private static AverageLogEntryResultsCalculator _instance = new AverageLogEntryResultsCalculator();
        public static AverageLogEntryResultsCalculator GetInstance() { return _instance; }

        private readonly object _lock = new object();

        private AverageLogEntryResultsCalculator() { }

        public override DataTable Get(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stresstestIds) {
            ConcurrentDictionary<string, DataTable> data = GetData(databaseActions, cancellationToken, stresstestIds);
            if (cancellationToken.IsCancellationRequested) return null;

            DataTable results = GetResults(data, cancellationToken);
            data = null;
            if (cancellationToken.IsCancellationRequested) return null;

            DataView dv = results.DefaultView;
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

            DataTable[] parts = GetLogEntryResultsThreaded(databaseActions, cancellationToken, runResults, 4, "SameAsLogEntryIndex", "LogEntryIndex", "UserAction", "LogEntry", "TimeToLastByteInTicks", "DelayInMilliseconds", "Error", "RunResultId");
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

            var averageLogEntryResults = CreateEmptyDataTable("AverageLogEntryResults", "Stresstest", "Concurrency", "User Action", "Log Entry", "Avg. Response Time (ms)",
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

                        //Get the log entry results containing log entries with the given run result id.
                        var logEntryResults = new DataRow[0];
                        for (int lerDataIndex = 0; lerDataIndex != logEntryResultsData.Length; lerDataIndex++) {
                            logEntryResults = logEntryResultsData[lerDataIndex].Select(string.Format("RunResultId In({0})", runResultIds.Combine(", ")));
                            if (logEntryResults.Length != 0)
                                break;
                        }

                        if (logEntryResults != null && logEntryResults.Length != 0) {
                            //We don't need to keep the run ids for this one, it's much faster and simpler like this.
                            var uniqueLogEntryCounts = new ConcurrentDictionary<string, int>(); //To make a correct average.
                            var userActions = new ConcurrentDictionary<string, string>(); //log entry index, User Action

                            foreach (DataRow lerRow in logEntryResults) {
                                if (cancellationToken.IsCancellationRequested) loopState.Break();

                                string logEntryIndex = lerRow["SameAsLogEntryIndex"] as string; //Combine results when using distribution like this.
                                if (logEntryIndex == string.Empty) {
                                    logEntryIndex = lerRow["LogEntryIndex"] as string;

                                    //Make sure we have all the user actions before averages are calcullated, otherwise the duplicated user action names can be used. 
                                    if (!userActions.ContainsKey(logEntryIndex)) {
                                        string userAction = lerRow["UserAction"] as string;
                                        userActions.TryAdd(logEntryIndex, userAction);
                                    }
                                }

                                if (uniqueLogEntryCounts.ContainsKey(logEntryIndex)) ++uniqueLogEntryCounts[logEntryIndex];
                                else uniqueLogEntryCounts.TryAdd(logEntryIndex, 1);
                            }

                            var logEntries = new Dictionary<string, string>(); //log entry index, log entry

                            var avgTimeToLastByteInTicks = new Dictionary<string, double>();
                            var maxTimeToLastByteInTicks = new Dictionary<string, long>();
                            var timeToLastBytesInTicks = new Dictionary<string, List<long>>();
                            var percTimeToLastBytesInTicks = new ConcurrentDictionary<string, long>();

                            var avgDelay = new Dictionary<string, double>();
                            var errors = new Dictionary<string, long>();

                            foreach (DataRow lerRow in logEntryResults) {
                                if (cancellationToken.IsCancellationRequested) loopState.Break();

                                string logEntryIndex = lerRow["SameAsLogEntryIndex"] as string; //Combine results when using distribution like this.
                                if (logEntryIndex == string.Empty) logEntryIndex = lerRow["LogEntryIndex"] as string;

                                string userAction = lerRow["UserAction"] as string;
                                string logEntry = lerRow["LogEntry"] as string;
                                long ttlb = (long)lerRow["TimeToLastByteInTicks"];
                                int delay = (int)lerRow["DelayInMilliseconds"];
                                string error = lerRow["Error"] as string;

                                int uniqueLogEntryCount = uniqueLogEntryCounts[logEntryIndex];

                                if (!userActions.ContainsKey(logEntryIndex)) userActions.TryAdd(logEntryIndex, userAction);
                                if (!logEntries.ContainsKey(logEntryIndex)) logEntries.Add(logEntryIndex, logEntry);

                                if (avgTimeToLastByteInTicks.ContainsKey(logEntryIndex)) avgTimeToLastByteInTicks[logEntryIndex] += (((double)ttlb) / uniqueLogEntryCount);
                                else avgTimeToLastByteInTicks.Add(logEntryIndex, (((double)ttlb) / uniqueLogEntryCount));

                                if (maxTimeToLastByteInTicks.ContainsKey(logEntryIndex)) { if (maxTimeToLastByteInTicks[logEntryIndex] < ttlb) maxTimeToLastByteInTicks[logEntryIndex] = ttlb; } else maxTimeToLastByteInTicks.Add(logEntryIndex, ttlb);

                                if (!timeToLastBytesInTicks.ContainsKey(logEntryIndex)) timeToLastBytesInTicks.Add(logEntryIndex, new List<long>(uniqueLogEntryCount));
                                timeToLastBytesInTicks[logEntryIndex].Add(ttlb);

                                if (avgDelay.ContainsKey(logEntryIndex)) avgDelay[logEntryIndex] += (((double)delay) / uniqueLogEntryCount);
                                else avgDelay.Add(logEntryIndex, ((double)delay) / uniqueLogEntryCount);


                                if (!errors.ContainsKey(logEntryIndex)) errors.Add(logEntryIndex, 0);
                                if (!string.IsNullOrEmpty(error)) ++errors[logEntryIndex];
                            }

                            //95th percentile
                            //foreach (var item in timeToLastBytesInTicks) {
                            Parallel.ForEach(timeToLastBytesInTicks, (item, loopState2) => {
                                if (cancellationToken.IsCancellationRequested) loopState2.Break();

                                percTimeToLastBytesInTicks.TryAdd(item.Key, PercentileCalculator<long>.Get(timeToLastBytesInTicks[item.Key], 95));
                            }
                            );
                            if (cancellationToken.IsCancellationRequested) loopState.Break();

                            List<string> sortedLogEntryIndices = logEntries.Keys.ToList();
                            sortedLogEntryIndices.Sort(LogEntryIndexComparer.GetInstance());

                            lock (_lock)
                                foreach (string s in sortedLogEntryIndices) {
                                    if (cancellationToken.IsCancellationRequested) loopState.Break();

                                    averageLogEntryResults.Rows.Add(stresstest, concurrency, userActions[s], logEntries[s],
                                        Math.Round(avgTimeToLastByteInTicks[s] / TimeSpan.TicksPerMillisecond, MidpointRounding.AwayFromZero),
                                        maxTimeToLastByteInTicks[s] / TimeSpan.TicksPerMillisecond,
                                        percTimeToLastBytesInTicks[s] / TimeSpan.TicksPerMillisecond,
                                        Math.Round(avgDelay[s], MidpointRounding.AwayFromZero),
                                        errors[s]);
                                }
                        }
                    }
                }
                );
            }
            return averageLogEntryResults;
        }

        private class LogEntryIndexComparer : IComparer<string> {
            private static readonly LogEntryIndexComparer _logEntryIndexComparer = new LogEntryIndexComparer();
            public static LogEntryIndexComparer GetInstance() { return _logEntryIndexComparer; }

            private const char dot = '.';


            private LogEntryIndexComparer() { }

            public int Compare(string x, string y) {
                string[] split1 = x.Split(dot);
                string[] split2 = y.Split(dot);

                int i = 0, j = 0;
                for (int index = 0; index != split1.Length; index++) {
                    i = int.Parse(split1[index]);
                    j = int.Parse(split2[index]);
                    if (i > j) return 1;
                    if (i < j) return -1;
                }
                return 0;
            }
        }
    }
}
