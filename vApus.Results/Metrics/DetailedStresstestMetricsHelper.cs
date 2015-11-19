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
    internal static class DetailedStressTestMetricsHelper {
        /// <summary>
        /// Metrics percentiles
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static StressTestMetrics GetMetrics(ConcurrencyResult result, CancellationToken cancellationToken) {
            var metrics = new StressTestMetrics();
            metrics.StartMeasuringTime = result.StartedAt;
            metrics.MeasuredTime = (result.StoppedAt == DateTime.MinValue ? DateTime.Now : result.StoppedAt) - metrics.StartMeasuringTime;
            metrics.Concurrency = result.Concurrency;
            metrics.AverageResponseTime = new TimeSpan();
            metrics.MaxResponseTime = new TimeSpan();
            metrics.AverageDelay = new TimeSpan();

            if (result.RunResults.Count != 0) {
                long totalAndExtraRequestsProcessed = 0; //For break on last run sync.
                long baseRequestCount = 0;

                var timesToLastByteInTicks = new List<long>();
                foreach (RunResult runResult in result.RunResults) {
                    StressTestMetrics runResultMetrics = GetMetrics(runResult, cancellationToken, false);
                    if (cancellationToken.IsCancellationRequested) return new StressTestMetrics();

                    metrics.StartsAndStopsRuns.Add(new KeyValuePair<DateTime, DateTime>(runResult.StartedAt, runResult.StoppedAt));

                    metrics.AverageResponseTime = metrics.AverageResponseTime.Add(runResultMetrics.AverageResponseTime);
                    if (runResultMetrics.MaxResponseTime > metrics.MaxResponseTime) metrics.MaxResponseTime = runResultMetrics.MaxResponseTime;

                    metrics.AverageDelay = metrics.AverageDelay.Add(runResultMetrics.AverageDelay);
                    metrics.Requests += runResultMetrics.Requests;
                    if (baseRequestCount == 0) baseRequestCount = metrics.Requests;

                    totalAndExtraRequestsProcessed += runResultMetrics.RequestsProcessed;
                    metrics.ResponsesPerSecond += runResultMetrics.ResponsesPerSecond;
                    metrics.UserActionsPerSecond += runResultMetrics.UserActionsPerSecond;
                    metrics.Errors += runResultMetrics.Errors;

                    //For the percentiles.
                    if (runResult.VirtualUserResults != null)
                        foreach (var vur in runResult.VirtualUserResults)
                            if (vur != null && vur.RequestResults != null)
                                foreach (var rer in vur.RequestResults)
                                    if (rer != null && rer.VirtualUser != null)
                                        timesToLastByteInTicks.Add(rer.TimeToLastByteInTicks);
                }
                for (int i = result.RunResults.Count; i < result.RunCount; i++)
                    metrics.Requests += baseRequestCount;

                if (metrics.Requests < totalAndExtraRequestsProcessed)
                    metrics.Requests = totalAndExtraRequestsProcessed;

                metrics.RequestsProcessed = totalAndExtraRequestsProcessed;

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
        public static StressTestMetrics GetMetrics(RunResult result, CancellationToken cancellationToken, bool includePercentiles = true) {
            var metrics = new StressTestMetrics();
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
                if (virtualUserResult != null && virtualUserResult.RequestResults != null) {
                    ++enteredUserResultsCount;

                    StressTestMetrics virtualUserMetrics = GetMetrics(virtualUserResult, cancellationToken);
                    if (cancellationToken.IsCancellationRequested) return new StressTestMetrics();

                    metrics.Requests += virtualUserMetrics.Requests;

                    metrics.AverageResponseTime = metrics.AverageResponseTime.Add(virtualUserMetrics.AverageResponseTime);
                    if (virtualUserMetrics.MaxResponseTime > metrics.MaxResponseTime) metrics.MaxResponseTime = virtualUserMetrics.MaxResponseTime;
                    metrics.AverageDelay = metrics.AverageDelay.Add(virtualUserMetrics.AverageDelay);
                    metrics.RequestsProcessed += virtualUserMetrics.RequestsProcessed;
                    metrics.ResponsesPerSecond += virtualUserMetrics.ResponsesPerSecond;
                    metrics.UserActionsPerSecond += virtualUserMetrics.UserActionsPerSecond;
                    metrics.Errors += virtualUserMetrics.Errors;

                    //For the percentiles.
                    if (includePercentiles)
                        foreach (var rer in virtualUserResult.RequestResults)
                            if (rer != null && rer.VirtualUser != null)
                                timesToLastByteInTicks.Add(rer.TimeToLastByteInTicks);
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

        /// <summary>
        /// Calculates max response time, # requests, average response time, average delay, responses / second and user actions per second for a single user.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static StressTestMetrics GetMetrics(VirtualUserResult result, CancellationToken cancellationToken) {
            var metrics = new StressTestMetrics();

            metrics.MaxResponseTime = new TimeSpan();
            metrics.Requests = result.RequestResults.LongLength;

            var uniqueUserActions = new HashSet<string>();
            TimeSpan totalTimeToLastByte = new TimeSpan(); 
            TimeSpan totalDelay = new TimeSpan(); //Delay after each user action.

            //Requests can happen in parallel (web tests). We need to take this into account to be able to calculate responses and user actions / second correctly.
            //The longest time counts for a parallel set. Only that time is added to the time span.
            TimeSpan parallelRequestsAwareTimeToLastByte = new TimeSpan(); 
            RequestResult prevRequestResult = null; //For parallelRequestsAwareTimeToLastByte calculations

            foreach (RequestResult requestResult in result.RequestResults) {
                if (cancellationToken.IsCancellationRequested) return new StressTestMetrics();

                if (requestResult != null && requestResult.VirtualUser != null) {
                    ++metrics.RequestsProcessed;
                    uniqueUserActions.Add(requestResult.UserAction);

                    var ttlb = new TimeSpan(requestResult.TimeToLastByteInTicks);
                    if (ttlb > metrics.MaxResponseTime) metrics.MaxResponseTime = ttlb;
                    totalTimeToLastByte = totalTimeToLastByte.Add(ttlb);

                    if (requestResult.InParallelWithPrevious && prevRequestResult != null) { //The longest time counts for a parallel set.
                        TimeSpan diffTtlb = requestResult.SentAt.Add(ttlb) - prevRequestResult.SentAt.AddTicks(prevRequestResult.TimeToLastByteInTicks);
                        ttlb = (diffTtlb.Ticks > 0L) ? diffTtlb : new TimeSpan(0);
                    }
                    parallelRequestsAwareTimeToLastByte = parallelRequestsAwareTimeToLastByte.Add(ttlb);

                    var delay = new TimeSpan(requestResult.DelayInMilliseconds * TimeSpan.TicksPerMillisecond);

                    totalDelay = totalDelay.Add(delay);
                    if (!string.IsNullOrEmpty(requestResult.Error)) ++metrics.Errors;

                    prevRequestResult = requestResult;
                }
            }

            if (metrics.RequestsProcessed != 0) {
                metrics.AverageResponseTime = new TimeSpan(totalTimeToLastByte.Ticks / metrics.RequestsProcessed);
                metrics.AverageDelay = new TimeSpan(totalDelay.Ticks / metrics.RequestsProcessed);

                double div = ((double)(parallelRequestsAwareTimeToLastByte.Ticks + totalDelay.Ticks) / TimeSpan.TicksPerSecond);
                metrics.ResponsesPerSecond = ((double)metrics.RequestsProcessed) / div;
                metrics.UserActionsPerSecond = ((double)uniqueUserActions.Count) / div;
            }
            return metrics;
        }
    }
}