/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using vApus.Util;

namespace vApus.Results {
    public static class StresstestMetricsHelper {

        #region Fields
        private static readonly string[] _readableMetricsHeadersConcurrency =
            {
                "Started At", "Time Left", "Measured Time", "Concurrency", "Log Entries Processed",
                "Throughput (responses / s)", "User Actions / s", "Avg. Response Time (ms)", "Max. Response Time (ms)",
                "Avg. Delay (ms)", "Errors"
            };
        //"95th Percentile of the Response Times (ms)", 
        private static readonly string[] _readableMetricsHeadersRun =
            {
                "Started At", "Time Left", "Measured Time", "Concurrency", "Run", "Log Entries Processed",
                "Throughput (responses / s)", "User Actions / s", "Avg. Response Time (ms)", "Max. Response Time (ms)",
                "Avg. Delay (ms)", "Errors"
            };
        private static readonly string[] _calculatableMetricsHeadersConcurrency =
            {
                "Started At", "Time Left (ms)", "Measured Time (ms)", "Concurrency", "Log Entries Processed",
                "Log Entries", "Throughput (responses / s)", "User Actions / s", "Avg. Response Time (ms)", "Max. Response Time (ms)",
                "Avg. Delay (ms)", "Errors"
            };
        private static readonly string[] _calculatableMetricsHeadersRun =
            {
                "Started At", "Time Left (ms)", "Measured Time (ms)", "Concurrency", "Run", "Log Entries Processed",
                "Log Entries", "Throughput (responses / s)", "User Actions / s", "Avg. Response Time (ms)", "Max. Response Time (ms)",
                "Avg. Delay (ms)", "Errors"
            };
        #endregion

        #region Functions
        public static string[] GetMetricsHeadersConcurrency(bool readable) { return readable ? _readableMetricsHeadersConcurrency : _calculatableMetricsHeadersConcurrency; }
        public static string[] GetMetricsHeadersRun(bool readable) { return readable ? _readableMetricsHeadersRun : _calculatableMetricsHeadersRun; }
        /// <summary>
        ///     Get metrics for a concurrency result.
        /// </summary>
        /// <param name="concurrencyResult"></param>
        /// <returns></returns>
        public static StresstestMetrics GetMetrics(ConcurrencyResult result, bool calculate95thPercentileResponseTimes = true) {
            var metrics = new StresstestMetrics();
            metrics.StartMeasuringTime = result.StartedAt;
            metrics.MeasuredTime = (result.StoppedAt == DateTime.MinValue ? DateTime.Now : result.StoppedAt) - metrics.StartMeasuringTime;
            metrics.Concurrency = result.Concurrency;
            metrics.AverageResponseTime = new TimeSpan();
            metrics.MaxResponseTime = new TimeSpan();
            metrics.AverageDelay = new TimeSpan();

            if (result.RunResults.Count != 0) {
                long totalAndExtraLogEntriesProcessed = 0; //For break on last run sync.
                long baseLogEntryCount = 0;

                var timesToLastByteInTicks = new List<long>(new long[] { 0 }); //For the 95th percentile of the response times.
                int percent5 = -1;
                foreach (RunResult runResult in result.RunResults) {
                    StresstestMetrics runResultMetrics = GetMetrics(runResult, false);

                    metrics.StartsAndStopsRuns.Add(new KeyValuePair<DateTime, DateTime>(runResult.StartedAt, runResult.StoppedAt));

                    if (calculate95thPercentileResponseTimes && percent5 == -1)
                        percent5 = (int)(result.RunResults.Count * runResultMetrics.LogEntries * 0.05) + 1;

                    metrics.AverageResponseTime = metrics.AverageResponseTime.Add(runResultMetrics.AverageResponseTime);
                    if (runResultMetrics.MaxResponseTime > metrics.MaxResponseTime) metrics.MaxResponseTime = runResultMetrics.MaxResponseTime;

                    metrics.AverageDelay = metrics.AverageDelay.Add(runResultMetrics.AverageDelay);
                    metrics.LogEntries += runResultMetrics.LogEntries;
                    if (baseLogEntryCount == 0) baseLogEntryCount = metrics.LogEntries;

                    totalAndExtraLogEntriesProcessed += runResultMetrics.LogEntriesProcessed;
                    metrics.ResponsesPerSecond += runResultMetrics.ResponsesPerSecond;
                    metrics.UserActionsPerSecond += runResultMetrics.UserActionsPerSecond;
                    metrics.Errors += runResultMetrics.Errors;

                    //For the 95th percentile.
                    if (calculate95thPercentileResponseTimes)
                        foreach (var vur in runResult.VirtualUserResults)
                            foreach (var ler in vur.LogEntryResults)
                                if (ler != null && ler.LogEntryIndex != null) {
                                    for (int i = 0; i != timesToLastByteInTicks.Count; i++)
                                        if (timesToLastByteInTicks[i] < ler.TimeToLastByteInTicks) {
                                            timesToLastByteInTicks.Insert(i, ler.TimeToLastByteInTicks);
                                            break;
                                        }
                                    while (timesToLastByteInTicks.Count > percent5) timesToLastByteInTicks.RemoveAt(percent5);
                                }
                }
                for (int i = result.RunResults.Count; i < result.RunCount; i++)
                    metrics.LogEntries += baseLogEntryCount;

                if (metrics.LogEntries < totalAndExtraLogEntriesProcessed)
                    metrics.LogEntries = totalAndExtraLogEntriesProcessed;

                metrics.LogEntriesProcessed = totalAndExtraLogEntriesProcessed;

                metrics.ResponsesPerSecond /= result.RunResults.Count;
                metrics.UserActionsPerSecond /= result.RunResults.Count;
                metrics.AverageResponseTime = new TimeSpan(metrics.AverageResponseTime.Ticks / result.RunResults.Count);
                metrics.AverageDelay = new TimeSpan(metrics.AverageDelay.Ticks / result.RunResults.Count);

                metrics.EstimatedTimeLeft = GetEstimatedTimeLeft(metrics, result.StoppedAt == DateTime.MinValue);

                long percentile95thResponseTimes = timesToLastByteInTicks[timesToLastByteInTicks.Count - 1];
                metrics.Percentile95thResponseTimes = percentile95thResponseTimes == 0 ? metrics.MaxResponseTime : new TimeSpan(percentile95thResponseTimes);
            }
            return metrics;
        }
        /// <summary>
        ///     Get metrics for a run result.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static StresstestMetrics GetMetrics(RunResult result, bool calculate95thPercentileResponseTimes = true) {
            var metrics = new StresstestMetrics();
            metrics.StartMeasuringTime = result.StartedAt;
            metrics.MeasuredTime = (result.StoppedAt == DateTime.MinValue ? DateTime.Now : result.StoppedAt) - metrics.StartMeasuringTime;
            metrics.Concurrency = result.VirtualUserResults.Length;
            metrics.Run = result.Run;
            metrics.RerunCount = result.RerunCount;
            metrics.AverageResponseTime = new TimeSpan();
            metrics.MaxResponseTime = new TimeSpan();
            metrics.AverageDelay = new TimeSpan();

            int enteredUserResultsCount = 0;
            var timesToLastByteInTicks = new List<long>(new long[] { 0 }); //For the 95th percentile of the response times.
            int percent5 = -1;
            foreach (VirtualUserResult virtualUserResult in result.VirtualUserResults) {
                if (virtualUserResult.VirtualUser != null) ++enteredUserResultsCount;

                StresstestMetrics virtualUserMetrics = GetMetrics(virtualUserResult);

                if (calculate95thPercentileResponseTimes && percent5 == -1)
                    percent5 = (int)(result.VirtualUserResults.Length * virtualUserMetrics.LogEntries * 0.05) + 1;

                metrics.AverageResponseTime = metrics.AverageResponseTime.Add(virtualUserMetrics.AverageResponseTime);
                if (virtualUserMetrics.MaxResponseTime > metrics.MaxResponseTime) metrics.MaxResponseTime = virtualUserMetrics.MaxResponseTime;
                metrics.AverageDelay = metrics.AverageDelay.Add(virtualUserMetrics.AverageDelay);
                metrics.LogEntries += virtualUserMetrics.LogEntries;
                metrics.LogEntriesProcessed += virtualUserMetrics.LogEntriesProcessed;
                metrics.ResponsesPerSecond += virtualUserMetrics.ResponsesPerSecond;
                metrics.UserActionsPerSecond += virtualUserMetrics.UserActionsPerSecond;
                metrics.Errors += virtualUserMetrics.Errors;

                if (calculate95thPercentileResponseTimes)
                    foreach (var ler in virtualUserResult.LogEntryResults)
                        if (ler != null && ler.LogEntryIndex != null) {
                            for (int i = 0; i != timesToLastByteInTicks.Count; i++)
                                if (timesToLastByteInTicks[i] < ler.TimeToLastByteInTicks) {
                                    timesToLastByteInTicks.Insert(i, ler.TimeToLastByteInTicks);
                                    break;
                                }
                            while (timesToLastByteInTicks.Count > percent5) timesToLastByteInTicks.RemoveAt(percent5);
                        }
            }

            if (enteredUserResultsCount != 0) {
                metrics.AverageResponseTime = new TimeSpan(metrics.AverageResponseTime.Ticks / enteredUserResultsCount);
                metrics.AverageDelay = new TimeSpan(metrics.AverageDelay.Ticks / enteredUserResultsCount);
                metrics.EstimatedTimeLeft = GetEstimatedTimeLeft(metrics, result.StoppedAt == DateTime.MinValue);
                long percentile95thResponseTimes = timesToLastByteInTicks[timesToLastByteInTicks.Count - 1];
                metrics.Percentile95thResponseTimes = percentile95thResponseTimes == 0 ? metrics.MaxResponseTime : new TimeSpan(percentile95thResponseTimes);
            }
            return metrics;
        }
        private static StresstestMetrics GetMetrics(VirtualUserResult result) {
            var metrics = new StresstestMetrics();
            metrics.MaxResponseTime = new TimeSpan();
            metrics.LogEntries = result.LogEntryResults.LongLength;

            var uniqueUserActions = new List<string>();
            TimeSpan totalTimeToLastByte = new TimeSpan(), totalDelay = new TimeSpan();
            foreach (LogEntryResult logEntryResult in result.LogEntryResults)
                if (logEntryResult != null && logEntryResult.LogEntryIndex != null) {
                    ++metrics.LogEntriesProcessed;
                    if (!uniqueUserActions.Contains(logEntryResult.UserAction)) uniqueUserActions.Add(logEntryResult.UserAction);

                    var ttlb = new TimeSpan(logEntryResult.TimeToLastByteInTicks);
                    totalTimeToLastByte = totalTimeToLastByte.Add(ttlb);
                    if (ttlb > metrics.MaxResponseTime) metrics.MaxResponseTime = ttlb;

                    totalDelay = totalDelay.Add(new TimeSpan(logEntryResult.DelayInMilliseconds * TimeSpan.TicksPerMillisecond));
                    if (!string.IsNullOrEmpty(logEntryResult.Error)) ++metrics.Errors;
                }

            if (metrics.LogEntriesProcessed != 0) {
                metrics.AverageResponseTime = new TimeSpan(totalTimeToLastByte.Ticks / metrics.LogEntriesProcessed);
                metrics.AverageDelay = new TimeSpan(totalDelay.Ticks / metrics.LogEntriesProcessed);

                double div = ((double)(totalTimeToLastByte.Ticks + totalDelay.Ticks) / TimeSpan.TicksPerSecond);
                metrics.ResponsesPerSecond = ((double)metrics.LogEntriesProcessed) / div;
                metrics.UserActionsPerSecond = ((double)uniqueUserActions.Count) / div;
            }
            return metrics;
        }
        private static TimeSpan GetEstimatedTimeLeft(StresstestMetrics metrics, bool running) {
            long estimatedTimeLeft = 0;
            if (running && metrics.LogEntriesProcessed != 0) {
                estimatedTimeLeft = (long)(((DateTime.Now - metrics.StartMeasuringTime).Ticks / metrics.LogEntriesProcessed) * (metrics.LogEntries - metrics.LogEntriesProcessed));
                if (estimatedTimeLeft < 0) estimatedTimeLeft = 0;
            }
            return new TimeSpan(estimatedTimeLeft);
        }
        /// <summary>
        /// Get estimated runtime left for the whole stresstest (this is not a precise estimation).
        /// </summary>
        /// <param name="result"></param>
        /// <param name="concurrencies"></param>
        /// <param name="runs"></param>
        /// <returns></returns>
        public static TimeSpan GetEstimatedRuntimeLeft(StresstestResult result, int concurrencies, int runs) {
            long estimatedRuntimeLeft = 0;
            try {
                if (result != null && result.StoppedAt == DateTime.MinValue) {
                    var now = DateTime.Now;
                    RunResult lastStoppedRun = null;

                    long logEntriesProcessed = 0, logEntries = 0;

                    runs *= concurrencies; //Get the total of runs
                    foreach (var cur in result.ConcurrencyResults) {
                        foreach (var rr in cur.RunResults) {
                            --runs; //Use to get the erl for the next, not commenced yet, runs
                            if (rr.StoppedAt == DateTime.MinValue) {
                                for (int vur = 0; vur != rr.VirtualUserResults.Length; vur++) {
                                    var virtualUserResult = rr.VirtualUserResults[vur];
                                    if (virtualUserResult != null) {
                                        var logEntryResults = virtualUserResult.LogEntryResults;
                                        for (int ler = 0; ler != logEntryResults.Length; ler++) {
                                            ++logEntries;
                                            if (logEntryResults[ler] != null) ++logEntriesProcessed;
                                        }
                                    }
                                }

                                if (logEntriesProcessed == 0) {
                                    ++runs;
                                }
                                else {
                                    var measuredTime = (now - rr.StartedAt).Ticks;
                                    estimatedRuntimeLeft = (long)((measuredTime / logEntriesProcessed) * (logEntries - logEntriesProcessed));
                                    if (estimatedRuntimeLeft < 0) estimatedRuntimeLeft = 0;

                                    //Get the estimated time left for the other runs, here we need the the already measured time.
                                    if (runs > 0) {
                                        var nextErl = estimatedRuntimeLeft + measuredTime;
                                        estimatedRuntimeLeft +=(nextErl * runs);
                                        runs = 0;
                                    }
                                }
                                break;
                            } else {
                                lastStoppedRun = rr;
                            }
                        }
                    }
                    //Calculate the erl when the current tun is stopped.
                    if (runs > 0 && lastStoppedRun != null)
                        estimatedRuntimeLeft = (lastStoppedRun.StoppedAt - lastStoppedRun.StartedAt).Ticks * runs;
                }
            } catch {
            }
            return new TimeSpan(estimatedRuntimeLeft);
        }

        /// <summary>
        /// </summary>
        /// <param name="metrics"></param>
        /// <returns></returns>
        public static List<object[]> MetricsToRows(List<StresstestMetrics> metrics, bool readable) {
            var rows = new List<object[]>(metrics.Count);
            foreach (var m in metrics) rows.Add(readable ? ReadableMetricsToRow(m) : CalculatableMetricsToRow(m));
            return rows;
        }
        private static object[] ReadableMetricsToRow(StresstestMetrics metrics) {
            if (metrics.Run == 0)
                return new object[]
                    {
                        metrics.StartMeasuringTime.ToString(),
                        metrics.EstimatedTimeLeft.ToShortFormattedString(),
                        metrics.MeasuredTime.ToShortFormattedString(),
                        metrics.Concurrency,
                        metrics.LogEntriesProcessed + " / " +
                        (metrics.LogEntries == 0 ? "--" : metrics.LogEntries.ToString()),
                        Math.Round(metrics.ResponsesPerSecond, 2),
                        Math.Round(metrics.UserActionsPerSecond, 2),
                        Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2),
                        Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2),
                        //Math.Round(metrics.Percentile95thResponseTimes.TotalMilliseconds, 2),
                        Math.Round(metrics.AverageDelay.TotalMilliseconds, 2),
                        metrics.Errors
                    };
            return new object[]
                {
                    metrics.StartMeasuringTime.ToString(),
                    metrics.EstimatedTimeLeft.ToShortFormattedString(),
                    metrics.MeasuredTime.ToShortFormattedString(),
                    metrics.Concurrency,
                    metrics.Run,
                    metrics.LogEntriesProcessed + " / " +
                    (metrics.LogEntries == 0 ? "--" : metrics.LogEntries.ToString()),
                    Math.Round(metrics.ResponsesPerSecond, 2),
                    Math.Round(metrics.UserActionsPerSecond, 2),
                    Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2),
                    Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2),
                    //Math.Round(metrics.Percentile95thResponseTimes.TotalMilliseconds, 2),
                    Math.Round(metrics.AverageDelay.TotalMilliseconds, 2),
                    metrics.Errors
                };
        }
        private static object[] CalculatableMetricsToRow(StresstestMetrics metrics) {
            if (metrics.Run == 0)
                return new object[]
                    {
                        metrics.StartMeasuringTime.ToString(),
                        Math.Round(metrics.EstimatedTimeLeft.TotalMilliseconds, 2),
                        Math.Round(metrics.MeasuredTime.TotalMilliseconds, 2),
                        metrics.Concurrency,
                        metrics.LogEntriesProcessed,
                        metrics.LogEntries == 0 ? "--" : metrics.LogEntries.ToString(),
                        Math.Round(metrics.ResponsesPerSecond, 2),
                        Math.Round(metrics.UserActionsPerSecond, 2),
                        Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2),
                        Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2),
                        //Math.Round(metrics.Percentile95thResponseTimes.TotalMilliseconds, 2),
                        Math.Round(metrics.AverageDelay.TotalMilliseconds, 2),
                        metrics.Errors
                    };
            return new object[]
                {
                    metrics.StartMeasuringTime.ToString(),
                    Math.Round(metrics.EstimatedTimeLeft.TotalMilliseconds, 2),
                    Math.Round(metrics.MeasuredTime.TotalMilliseconds, 2),
                    metrics.Concurrency,
                    metrics.Run,
                    metrics.LogEntriesProcessed,
                    metrics.LogEntries == 0 ? "--" : metrics.LogEntries.ToString(),
                    Math.Round(metrics.ResponsesPerSecond, 2),
                    Math.Round(metrics.UserActionsPerSecond, 2),
                    Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2),
                    Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2),
                    //Math.Round(metrics.Percentile95thResponseTimes.TotalMilliseconds, 2),
                    Math.Round(metrics.AverageDelay.TotalMilliseconds, 2),
                    metrics.Errors
                };
        }
        #endregion
    }
}