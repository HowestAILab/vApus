/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Threading;
using vApus.Util;
using System.Linq;

namespace vApus.Results {
    /// <summary>
    /// Used for average concurrant users results in the results helper.
    /// </summary>
    internal static class DetailedStresstestMetricsHelper {
        /// <summary>
        /// Metrics percentiles
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static StresstestMetrics GetMetrics(ConcurrencyResult result, CancellationToken cancellationToken) {
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

                var timesToLastByteInTicks = new List<long>();
                foreach (RunResult runResult in result.RunResults) {
                    StresstestMetrics runResultMetrics = GetMetrics(runResult, cancellationToken, false);
                    if (cancellationToken.IsCancellationRequested) return new StresstestMetrics();

                    metrics.StartsAndStopsRuns.Add(new KeyValuePair<DateTime, DateTime>(runResult.StartedAt, runResult.StoppedAt));

                    metrics.AverageResponseTime = metrics.AverageResponseTime.Add(runResultMetrics.AverageResponseTime);
                    if (runResultMetrics.MaxResponseTime > metrics.MaxResponseTime) metrics.MaxResponseTime = runResultMetrics.MaxResponseTime;

                    metrics.AverageDelay = metrics.AverageDelay.Add(runResultMetrics.AverageDelay);
                    metrics.LogEntries += runResultMetrics.LogEntries;
                    if (baseLogEntryCount == 0) baseLogEntryCount = metrics.LogEntries;

                    totalAndExtraLogEntriesProcessed += runResultMetrics.LogEntriesProcessed;
                    metrics.ResponsesPerSecond += runResultMetrics.ResponsesPerSecond;
                    metrics.UserActionsPerSecond += runResultMetrics.UserActionsPerSecond;
                    metrics.Errors += runResultMetrics.Errors;

                    //For the percentiles.
                    if (runResult.VirtualUserResults != null)
                        foreach (var vur in runResult.VirtualUserResults)
                            if (vur != null && vur.LogEntryResults != null)
                                foreach (var ler in vur.LogEntryResults)
                                    if (ler != null && ler.VirtualUser != null)
                                        timesToLastByteInTicks.Add(ler.TimeToLastByteInTicks);
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

                metrics.EstimatedTimeLeft = new TimeSpan(0L);

                long percentile95thResponseTimes = 0L, percentile99thResponseTimes = 0L, avgTop5ResponseTimes = metrics.MaxResponseTime.Ticks;
                if (timesToLastByteInTicks.Count != 0) {
                    IEnumerable<long> orderedValues;
                    percentile95thResponseTimes = PercentileCalculator<long>.Get(timesToLastByteInTicks, 95, out orderedValues);
                    percentile99thResponseTimes = PercentileCalculator<long>.Get(orderedValues, 99);

                    int top5Count = Convert.ToInt32(orderedValues.Count() * 0.05);
                    if (top5Count != 0)
                        avgTop5ResponseTimes = (long)orderedValues.Take(top5Count).Average();

                    //long mean = metrics.AverageResponseTime.Ticks;
                    //double variance = orderedValues.Sum(number => Math.Pow(number - mean, 2.0)) / orderedValues.Count();

                    //standardDeviation = (long)Math.Sqrt(variance);
                }

                metrics.Percentile95thResponseTimes = new TimeSpan(percentile95thResponseTimes);
                metrics.Percentile99thResponseTimes = new TimeSpan(percentile99thResponseTimes);
                metrics.AverageTop5ResponseTimes = new TimeSpan(avgTop5ResponseTimes);
            }
            return metrics;
        }

        /// <summary>
        /// Metrics + percentiles
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static StresstestMetrics GetMetrics(RunResult result, CancellationToken cancellationToken, bool includePercentiles = true) {
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
            var timesToLastByteInTicks = new List<long>();
            foreach (VirtualUserResult virtualUserResult in result.VirtualUserResults)
                if (virtualUserResult != null && virtualUserResult.LogEntryResults != null) {
                    ++enteredUserResultsCount;

                    StresstestMetrics virtualUserMetrics = GetMetrics(virtualUserResult, cancellationToken);
                    if (cancellationToken.IsCancellationRequested) return new StresstestMetrics();

                    metrics.LogEntries += virtualUserMetrics.LogEntries;

                    metrics.AverageResponseTime = metrics.AverageResponseTime.Add(virtualUserMetrics.AverageResponseTime);
                    if (virtualUserMetrics.MaxResponseTime > metrics.MaxResponseTime) metrics.MaxResponseTime = virtualUserMetrics.MaxResponseTime;
                    metrics.AverageDelay = metrics.AverageDelay.Add(virtualUserMetrics.AverageDelay);
                    metrics.LogEntriesProcessed += virtualUserMetrics.LogEntriesProcessed;
                    metrics.ResponsesPerSecond += virtualUserMetrics.ResponsesPerSecond;
                    metrics.UserActionsPerSecond += virtualUserMetrics.UserActionsPerSecond;
                    metrics.Errors += virtualUserMetrics.Errors;

                    //For the percentiles.
                    if (includePercentiles)
                        foreach (var ler in virtualUserResult.LogEntryResults)
                            if (ler != null && ler.VirtualUser != null)
                                timesToLastByteInTicks.Add(ler.TimeToLastByteInTicks);
                }

            if (enteredUserResultsCount != 0) {
                metrics.AverageResponseTime = new TimeSpan(metrics.AverageResponseTime.Ticks / enteredUserResultsCount);
                metrics.AverageDelay = new TimeSpan(metrics.AverageDelay.Ticks / enteredUserResultsCount);
                metrics.EstimatedTimeLeft = new TimeSpan(0L);

                long percentile95thResponseTimes = 0L, percentile99thResponseTimes = 0L, avgTop5ResponseTimes = metrics.MaxResponseTime.Ticks;
                if (includePercentiles && timesToLastByteInTicks.Count != 0) {
                    IEnumerable<long> orderedValues;
                    percentile95thResponseTimes = PercentileCalculator<long>.Get(timesToLastByteInTicks, 95, out orderedValues);
                    percentile99thResponseTimes = PercentileCalculator<long>.Get(orderedValues, 99);

                    int top5Count = Convert.ToInt32(orderedValues.Count() * 0.05);
                    if (top5Count != 0)
                        avgTop5ResponseTimes = (long)orderedValues.Take(top5Count).Average();

                    //long mean = metrics.AverageResponseTime.Ticks;
                    //double variance = orderedValues.Sum(number => Math.Pow(number - mean, 2.0)) / orderedValues.Count();

                    //standardDeviation = (long)Math.Sqrt(variance);
                }

                metrics.Percentile95thResponseTimes = new TimeSpan(percentile95thResponseTimes);
                metrics.Percentile99thResponseTimes = new TimeSpan(percentile99thResponseTimes);
                metrics.AverageTop5ResponseTimes = new TimeSpan(avgTop5ResponseTimes);
            }
            return metrics;
        }

        private static StresstestMetrics GetMetrics(VirtualUserResult result, CancellationToken cancellationToken) {
            var metrics = new StresstestMetrics();

            metrics.MaxResponseTime = new TimeSpan();
            metrics.LogEntries = result.LogEntryResults.LongLength;

            var uniqueUserActions = new HashSet<string>();
            TimeSpan totalTimeToLastByte = new TimeSpan(), totalDelay = new TimeSpan();
            foreach (LogEntryResult logEntryResult in result.LogEntryResults) {
                if (cancellationToken.IsCancellationRequested) return new StresstestMetrics();

                if (logEntryResult != null && logEntryResult.VirtualUser != null) {
                    ++metrics.LogEntriesProcessed;
                    uniqueUserActions.Add(logEntryResult.UserAction);

                    var ttlb = new TimeSpan(logEntryResult.TimeToLastByteInTicks);
                    totalTimeToLastByte = totalTimeToLastByte.Add(ttlb);
                    if (ttlb > metrics.MaxResponseTime) metrics.MaxResponseTime = ttlb;

                    totalDelay = totalDelay.Add(new TimeSpan(logEntryResult.DelayInMilliseconds * TimeSpan.TicksPerMillisecond));
                    if (!string.IsNullOrEmpty(logEntryResult.Error)) ++metrics.Errors;
                }
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
    }
}