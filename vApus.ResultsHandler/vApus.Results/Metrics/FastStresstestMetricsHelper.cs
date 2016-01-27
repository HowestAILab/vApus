using RandomUtils.Log;
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
    /// <summary>
    /// Gets metrics from results or cache. Those are added to the cache if not already present.
    /// </summary>
    public static class FastStressTestMetricsHelper {

        #region Fields
        private static readonly string[] _readableMetricsHeadersConcurrency =
            {
                "Started at", "Time left", "Measured time", "Concurrency", "Requests processed", "Errors",
                "Throughput (responses / s)", "User actions / s", "Avg. response time (ms)", "Max. response time (ms)",
                "Avg. delay (ms)"
            };
        private static readonly string[] _readableMetricsHeadersRun =
            {
                "Started at", "Time left", "Measured time", "Concurrency", "Run", "Requests processed", "Errors",
                "Throughput (responses / s)", "User actions / s", "Avg. response time (ms)", "Max. response time (ms)",
                "Avg. delay (ms)"
            };
        private static readonly string[] _calculatableMetricsHeadersConcurrency =
            {
                "Started at", "Time left (ms)", "Measured time (ms)", "Concurrency", "Requests processed",
                "Requests", "Errors", "Throughput (responses / s)", "User actions / s", "Avg. response time (ms)", "Max. response time (ms)",
                "Avg. delay (ms)"
            };
        private static readonly string[] _calculatableMetricsHeadersRun =
            {
                "Started at", "Time left (ms)", "Measured time (ms)", "Concurrency", "Run", "Requests processed",
                "Requests", "Errors", "Throughput (responses / s)", "User actions / s", "Avg. response time (ms)", "Max. response time (ms)",
                "Avg. delay (ms)"
            };
        #endregion

        #region Functions
        public static string[] GetMetricsHeadersConcurrency(bool readable) { return readable ? _readableMetricsHeadersConcurrency : _calculatableMetricsHeadersConcurrency; }
        public static string[] GetMetricsHeadersRun(bool readable) { return readable ? _readableMetricsHeadersRun : _calculatableMetricsHeadersRun; }

        /// <summary>
        ///     Get metrics for a concurrency result. If the time to calculate this exceeds MAXGETMETRICSTIME a simplified metrics is returned. BUT only if you do not want the 95th percentile.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="timeToCalculate"></param>
        /// <param name="simplified">Only started at, measured time, estimate time left, concurrency, processed requests, requests, errors and StartsAndStopsRuns are put in the metrics object this is true and 
        /// <returns></returns>
        public static StressTestMetrics GetMetrics(ConcurrencyResult result) {
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

                foreach (RunResult runResult in result.RunResults) {
                    StressTestMetrics runResultMetrics = GetMetrics(runResult);
                    
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
                }
                baseRequestCount *= result.RunCount;
                if (metrics.Requests < baseRequestCount)
                    metrics.Requests = baseRequestCount;

                if (metrics.Requests < totalAndExtraRequestsProcessed)
                    metrics.Requests = totalAndExtraRequestsProcessed;

                metrics.RequestsProcessed = totalAndExtraRequestsProcessed;

                metrics.ResponsesPerSecond /= result.RunResults.Count;
                metrics.UserActionsPerSecond /= result.RunResults.Count;
                metrics.AverageResponseTime = new TimeSpan(metrics.AverageResponseTime.Ticks / result.RunResults.Count);
                metrics.AverageDelay = new TimeSpan(metrics.AverageDelay.Ticks / result.RunResults.Count);

                metrics.EstimatedTimeLeft = GetEstimatedTimeLeft(metrics, result.StoppedAt == DateTime.MinValue);
            }
            return metrics;
        }

        /// <summary>
        ///     Only started at, measured time, estimate time left, concurrency, processed requests, requests, errors and StartsAndStopsRuns
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static StressTestMetrics GetSimplifiedMetrics(ConcurrencyResult result) {
            var metrics = new StressTestMetrics();
            metrics.Simplified = true;
            metrics.StartMeasuringTime = result.StartedAt;
            metrics.MeasuredTime = (result.StoppedAt == DateTime.MinValue ? DateTime.Now : result.StoppedAt) - metrics.StartMeasuringTime;
            metrics.Concurrency = result.Concurrency;
            metrics.AverageResponseTime = new TimeSpan();
            metrics.MaxResponseTime = new TimeSpan();
            metrics.AverageDelay = new TimeSpan();

            if (result.RunResults.Count != 0) {
                long totalAndExtraRequestsProcessed = 0; //For break on last run sync.
                long baseRequestCount = 0;

                foreach (RunResult runResult in result.RunResults) {
                    StressTestMetrics runResultMetrics = GetSimplifiedMetrics(runResult);

                    metrics.StartsAndStopsRuns.Add(new KeyValuePair<DateTime, DateTime>(runResult.StartedAt, runResult.StoppedAt));

                    metrics.Requests += runResultMetrics.Requests;
                    if (baseRequestCount == 0) baseRequestCount = metrics.Requests;

                    totalAndExtraRequestsProcessed += runResultMetrics.RequestsProcessed;
                    metrics.Errors += runResultMetrics.Errors;

                }
                baseRequestCount *= result.RunCount;
                if (metrics.Requests < baseRequestCount)
                    metrics.Requests = baseRequestCount;

                if (metrics.Requests < totalAndExtraRequestsProcessed)
                    metrics.Requests = totalAndExtraRequestsProcessed;

                metrics.RequestsProcessed = totalAndExtraRequestsProcessed;

                metrics.EstimatedTimeLeft = GetEstimatedTimeLeft(metrics, result.StoppedAt == DateTime.MinValue);
            }
            return metrics;
        }

        /// <summary>
        ///     Get metrics for a run result.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="simplified">Only started at, measured time, estimate time left, concurrency, run, rerun count, processed requests, requests, errors if this is true and
        /// <returns></returns>
        public static StressTestMetrics GetMetrics(RunResult result) {
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
            foreach (VirtualUserResult virtualUserResult in result.VirtualUserResults)
                if (virtualUserResult != null && virtualUserResult.RequestResults != null) {
                    ++enteredUserResultsCount;

                    StressTestMetrics virtualUserMetrics = GetMetrics(virtualUserResult);

                    metrics.Requests += virtualUserMetrics.Requests;

                    metrics.AverageResponseTime = metrics.AverageResponseTime.Add(virtualUserMetrics.AverageResponseTime);
                    if (virtualUserMetrics.MaxResponseTime > metrics.MaxResponseTime) metrics.MaxResponseTime = virtualUserMetrics.MaxResponseTime;
                    metrics.AverageDelay = metrics.AverageDelay.Add(virtualUserMetrics.AverageDelay);
                    metrics.RequestsProcessed += virtualUserMetrics.RequestsProcessed;
                    metrics.ResponsesPerSecond += virtualUserMetrics.ResponsesPerSecond;
                    metrics.UserActionsPerSecond += virtualUserMetrics.UserActionsPerSecond;
                    metrics.Errors += virtualUserMetrics.Errors;
                }

            if (enteredUserResultsCount != 0) {
                metrics.AverageResponseTime = new TimeSpan(metrics.AverageResponseTime.Ticks / enteredUserResultsCount);
                metrics.AverageDelay = new TimeSpan(metrics.AverageDelay.Ticks / enteredUserResultsCount);
                metrics.EstimatedTimeLeft = GetEstimatedTimeLeft(metrics, result.StoppedAt == DateTime.MinValue);
            }
            return metrics;
        }
        /// <summary>
        ///     Only started at, measured time, estimate time left, concurrency, run, rerun count, processed requests, requests, errors.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static StressTestMetrics GetSimplifiedMetrics(RunResult result) {
            var metrics = new StressTestMetrics();
            metrics.Simplified = true;
            metrics.StartMeasuringTime = result.StartedAt;
            metrics.MeasuredTime = (result.StoppedAt == DateTime.MinValue ? DateTime.Now : result.StoppedAt) - metrics.StartMeasuringTime;
            metrics.Concurrency = result.VirtualUserResults.Length;
            metrics.Run = result.Run;
            metrics.RerunCount = result.RerunCount;
            metrics.AverageResponseTime = new TimeSpan();
            metrics.MaxResponseTime = new TimeSpan();
            metrics.AverageDelay = new TimeSpan();

            int enteredUserResultsCount = 0;
            foreach (VirtualUserResult virtualUserResult in result.VirtualUserResults)
                if (virtualUserResult != null && virtualUserResult.RequestResults != null) {
                    ++enteredUserResultsCount;

                    StressTestMetrics virtualUserMetrics = GetSimplifiedMetrics(virtualUserResult);
                    metrics.Requests += virtualUserMetrics.Requests;

                    metrics.RequestsProcessed += virtualUserMetrics.RequestsProcessed;
                    metrics.Errors += virtualUserMetrics.Errors;
                }

            if (enteredUserResultsCount != 0)
                metrics.EstimatedTimeLeft = GetEstimatedTimeLeft(metrics, result.StoppedAt == DateTime.MinValue);

            return metrics;
        }

        private static StressTestMetrics GetMetrics(VirtualUserResult result) {
            var metrics = new StressTestMetrics();

            metrics.MaxResponseTime = new TimeSpan();
            metrics.Requests = result.RequestResults.LongLength;

            var uniqueUserActions = new HashSet<string>();
            TimeSpan totalTimeToLastByte = new TimeSpan(), parallelRequestsAwareTimeToLastByte = new TimeSpan(), totalDelay = new TimeSpan();
            RequestResult prevRequestResult = null; //For parallel request calculations

            foreach (RequestResult requestResult in result.RequestResults) {
                if (requestResult != null && requestResult.VirtualUser != null) {
                    ++metrics.RequestsProcessed;
                    uniqueUserActions.Add(requestResult.UserAction);

                    var ttlb = new TimeSpan(requestResult.TimeToLastByteInTicks);
                    if (ttlb > metrics.MaxResponseTime) metrics.MaxResponseTime = ttlb;
                    totalTimeToLastByte = totalTimeToLastByte.Add(ttlb);

                    if (requestResult.InParallelWithPrevious && prevRequestResult != null) { //For parallel requests the total time to last byte in a virtual result is calculated differently. The longest time counts for a parallel set.
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
        /// <summary>
        /// Only requests processed, requests and errors are returned.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static StressTestMetrics GetSimplifiedMetrics(VirtualUserResult result) {
            var metrics = new StressTestMetrics();
            metrics.Simplified = true;
            metrics.Requests = result.RequestResults.LongLength;

            foreach (RequestResult requestResult in result.RequestResults)
                if (requestResult != null && requestResult.VirtualUser != null) {
                    ++metrics.RequestsProcessed;
                    if (!string.IsNullOrEmpty(requestResult.Error)) ++metrics.Errors;
                }
            return metrics;
        }
        private static TimeSpan GetEstimatedTimeLeft(StressTestMetrics metrics, bool running) {
            long estimatedTimeLeft = 0;
            if (running && metrics.RequestsProcessed != 0) {
                estimatedTimeLeft = (long)(((DateTime.Now - metrics.StartMeasuringTime).Ticks / metrics.RequestsProcessed) * (metrics.Requests - metrics.RequestsProcessed));
                if (estimatedTimeLeft < 0) estimatedTimeLeft = 0;
            }
            return new TimeSpan(estimatedTimeLeft);
        }
        /// <summary>
        /// Get estimated runtime left for the whole stress test (this is not a precise estimation).
        /// </summary>
        /// <param name="result"></param>
        /// <param name="concurrencies"></param>
        /// <param name="runs"></param>
        /// <returns></returns>
        public static TimeSpan GetEstimatedRuntimeLeft(StressTestResult result, int concurrencies, int runs) {
            long estimatedRuntimeLeft = 0;
            try {
                if (result != null && result.StoppedAt == DateTime.MinValue) {
                    var now = DateTime.Now;
                    RunResult lastStoppedRun = null;

                    long requestsProcessed = 0, requests = 0;

                    runs *= concurrencies; //Get the total of runs
                    foreach (var cur in result.ConcurrencyResults) {
                        foreach (var rr in cur.RunResults) {
                            --runs; //Use to get the erl for the next, not commenced yet, runs
                            if (rr.StoppedAt == DateTime.MinValue) {
                                for (int vur = 0; vur != rr.VirtualUserResults.Length; vur++) {
                                    var requestResults = rr.VirtualUserResults[vur].RequestResults;
                                    if (requestResults != null) {
                                        for (int rer = 0; rer != requestResults.Length; rer++) {
                                            ++requests;
                                            var requestResult = requestResults[rer];
                                            if (requestResult != null && requestResult.VirtualUser != null) ++requestsProcessed;
                                        }
                                    }
                                }

                                if (requestsProcessed == 0) {
                                    ++runs;
                                } else {
                                    var measuredTime = (now - rr.StartedAt).Ticks;
                                    estimatedRuntimeLeft = (long)((measuredTime / requestsProcessed) * (requests - requestsProcessed));
                                    if (estimatedRuntimeLeft < 0) estimatedRuntimeLeft = 0;

                                    //Get the estimated time left for the other runs, here we need the the already measured time.
                                    if (runs > 0) {
                                        var nextErl = estimatedRuntimeLeft + measuredTime;
                                        estimatedRuntimeLeft += (nextErl * runs);
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
            } catch (Exception ex) {
                Loggers.Log(Level.Warning, "Failed calculating estimated runtime left.", ex);
            }
            return new TimeSpan(estimatedRuntimeLeft);
        }

        /// <summary>
        /// </summary>
        /// <param name="metrics"></param>
        /// <returns></returns>
        public static List<object[]> MetricsToRows(List<StressTestMetrics> metrics, bool readable) {
            var rows = new List<object[]>(metrics.Count);
            foreach (var m in metrics) rows.Add(readable ? ReadableMetricsToRow(m) : CalculatableMetricsToRow(m));
            return rows;
        }
        private static object[] ReadableMetricsToRow(StressTestMetrics metrics) {
            if (metrics.Run == 0)
                return new object[]
                    {
                        metrics.StartMeasuringTime.ToString(),
                        metrics.EstimatedTimeLeft.ToShortFormattedString(true),
                        metrics.MeasuredTime.ToShortFormattedString(true),
                        metrics.Concurrency,
                        metrics.RequestsProcessed + " / " +
                        (metrics.Requests == 0 ? "--" : metrics.Requests.ToString()),
                        metrics.Errors,
                        metrics.Simplified ? "--" : Math.Round(metrics.ResponsesPerSecond, 2, MidpointRounding.AwayFromZero).ToString(),
                        metrics.Simplified ? "--" : Math.Round(metrics.UserActionsPerSecond, 2, MidpointRounding.AwayFromZero).ToString(),
                        metrics.Simplified ? "--" : Math.Round(metrics.AverageResponseTime.TotalMilliseconds, MidpointRounding.AwayFromZero).ToString(),
                        metrics.Simplified ? "--" : Math.Round(metrics.MaxResponseTime.TotalMilliseconds, MidpointRounding.AwayFromZero).ToString(),
                        metrics.Simplified ? "--" : Math.Round(metrics.AverageDelay.TotalMilliseconds, MidpointRounding.AwayFromZero).ToString()
                    };
            return new object[]
                {
                    metrics.StartMeasuringTime.ToString(),
                    metrics.EstimatedTimeLeft.ToShortFormattedString(true),
                    metrics.MeasuredTime.ToShortFormattedString(true),
                    metrics.Concurrency,
                    metrics.Run,
                    metrics.RequestsProcessed + " / " +
                    (metrics.Requests == 0 ? "--" : metrics.Requests.ToString()),
                    metrics.Errors,
                    metrics.Simplified ? "--" : Math.Round(metrics.ResponsesPerSecond, 2, MidpointRounding.AwayFromZero).ToString(),
                    metrics.Simplified ? "--" : Math.Round(metrics.UserActionsPerSecond, 2, MidpointRounding.AwayFromZero).ToString(),
                    metrics.Simplified ? "--" : Math.Round(metrics.AverageResponseTime.TotalMilliseconds, MidpointRounding.AwayFromZero).ToString(),
                    metrics.Simplified ? "--" : Math.Round(metrics.MaxResponseTime.TotalMilliseconds, MidpointRounding.AwayFromZero).ToString(),
                    metrics.Simplified ? "--" : Math.Round(metrics.AverageDelay.TotalMilliseconds, MidpointRounding.AwayFromZero).ToString()
                };
        }
        private static object[] CalculatableMetricsToRow(StressTestMetrics metrics) {
            if (metrics.Run == 0)
                return new object[]
                    {
                        metrics.StartMeasuringTime.ToString(),
                        Math.Round(metrics.EstimatedTimeLeft.TotalMilliseconds, MidpointRounding.AwayFromZero),
                        Math.Round(metrics.MeasuredTime.TotalMilliseconds, MidpointRounding.AwayFromZero),
                        metrics.Concurrency,
                        metrics.RequestsProcessed,
                        metrics.Requests == 0 ? "--" : metrics.Requests.ToString(),
                        metrics.Errors,
                        metrics.Simplified ? "--" : Math.Round(metrics.ResponsesPerSecond, 2, MidpointRounding.AwayFromZero).ToString(),
                        metrics.Simplified ? "--" : Math.Round(metrics.UserActionsPerSecond, 2, MidpointRounding.AwayFromZero).ToString(),
                        metrics.Simplified ? "--" : Math.Round(metrics.AverageResponseTime.TotalMilliseconds, MidpointRounding.AwayFromZero).ToString(),
                        metrics.Simplified ? "--" : Math.Round(metrics.MaxResponseTime.TotalMilliseconds, MidpointRounding.AwayFromZero).ToString(),
                        metrics.Simplified ? "--" : Math.Round(metrics.AverageDelay.TotalMilliseconds, MidpointRounding.AwayFromZero).ToString()
                    };
            return new object[]
                {
                    metrics.StartMeasuringTime.ToString(),
                    Math.Round(metrics.EstimatedTimeLeft.TotalMilliseconds, MidpointRounding.AwayFromZero),
                    Math.Round(metrics.MeasuredTime.TotalMilliseconds, MidpointRounding.AwayFromZero),
                    metrics.Concurrency,
                    metrics.Run,
                    metrics.RequestsProcessed,
                    metrics.Requests == 0 ? "--" : metrics.Requests.ToString(),
                    metrics.Errors,
                    metrics.Simplified ? "--" : Math.Round(metrics.ResponsesPerSecond, 2, MidpointRounding.AwayFromZero).ToString(),
                    metrics.Simplified ? "--" : Math.Round(metrics.UserActionsPerSecond, 2, MidpointRounding.AwayFromZero).ToString(),
                    metrics.Simplified ? "--" : Math.Round(metrics.AverageResponseTime.TotalMilliseconds, MidpointRounding.AwayFromZero).ToString(),
                    metrics.Simplified ? "--" : Math.Round(metrics.MaxResponseTime.TotalMilliseconds, MidpointRounding.AwayFromZero).ToString(),
                    metrics.Simplified ? "--" : Math.Round(metrics.AverageDelay.TotalMilliseconds, MidpointRounding.AwayFromZero).ToString()
                };
        }

        /// <summary>
        /// Used in many-to-one distributed tests (1 stress tests workload divided over multiple slaves), to be able to calculate the correct metrics. 
        /// </summary>
        /// <param name="metricsCaches">If only one entry the list will be returned directly.</param>
        /// <returns></returns>
        public static FastStressTestMetricsCache MergeStressTestMetricsCaches(List<FastStressTestMetricsCache> metricsCaches, bool allowSimplified) {
            int count = metricsCaches.Count;
            if (count == 0)
                throw new Exception("The given list must contain more than 0 metrics caches.");
            else if (count == 1)
                return metricsCaches[0];

            //First get all the metrics...
            int maxConcurrencyAndRunCount = 0;
            var allMetrics = new List<List<StressTestMetrics>>(count);
            foreach (var metricsCache in metricsCaches) {
                var m = metricsCache.GetAllMetrics(allowSimplified);
                allMetrics.Add(m);
                if (m.Count > maxConcurrencyAndRunCount)
                    maxConcurrencyAndRunCount = m.Count;
            }

            //...then pivot them (We do not want to merge all the results for one stress test, but the concurrencies and runs over the tests at the same index).
            var pivotedMetrics = new List<List<StressTestMetrics>>(maxConcurrencyAndRunCount);
            for (int i = 0; i < maxConcurrencyAndRunCount; i++) {
                var l = new List<StressTestMetrics>(count);
                for (int j = 0; j < count; j++) {
                    var part = allMetrics[j];
                    if (i < part.Count) l.Add(part[i]);
                }
                pivotedMetrics.Add(l);
            }

            //Now do the merge.
            var mergedMetricsCache = new FastStressTestMetricsCache();
            foreach (var toBeMerged in pivotedMetrics)
                mergedMetricsCache.Add(MergeStressTestMetrics(toBeMerged));
            return mergedMetricsCache;
        }
        /// <summary>
        /// Used in many-to-one distributed tests (1 stress tests workload divided over multiple slaves), to be able to calculate the correct metrics. 
        /// </summary>
        /// <param name="metrics">If only one entry the list will be returned directly..</param>
        /// <returns></returns>
        private static StressTestMetrics MergeStressTestMetrics(List<StressTestMetrics> metrics) {
            int count = metrics.Count;
            if (count == 0)
                throw new Exception("The given list must contain more than 0 metrics.");
            else if (count == 1)
                return metrics[0];

            var mergedMetrics = new StressTestMetrics();
            mergedMetrics.StartMeasuringTime = metrics[0].StartMeasuringTime;
            mergedMetrics.StartsAndStopsRuns = metrics[0].StartsAndStopsRuns;

            foreach (var m in metrics) {
                if (m.EstimatedTimeLeft > mergedMetrics.EstimatedTimeLeft) mergedMetrics.EstimatedTimeLeft = m.EstimatedTimeLeft;
                if (m.MeasuredTime > mergedMetrics.MeasuredTime) mergedMetrics.MeasuredTime = m.MeasuredTime;
                mergedMetrics.Concurrency += m.Concurrency;
                if (m.Run > mergedMetrics.Run) mergedMetrics.Run = m.Run;
                if (m.RerunCount > mergedMetrics.RerunCount) mergedMetrics.RerunCount = m.RerunCount;
                mergedMetrics.Requests += m.Requests;
                mergedMetrics.ResponsesPerSecond += m.ResponsesPerSecond;
                mergedMetrics.UserActionsPerSecond += m.UserActionsPerSecond;

                mergedMetrics.AverageResponseTime += new TimeSpan(m.AverageResponseTime.Ticks / count);

                if (m.MaxResponseTime > mergedMetrics.MaxResponseTime) mergedMetrics.MaxResponseTime = m.MaxResponseTime;

                mergedMetrics.AverageDelay += new TimeSpan(m.AverageDelay.Ticks / count);
                mergedMetrics.Errors += m.Errors;
                mergedMetrics.RequestsProcessed += m.RequestsProcessed;

                if (m.Simplified) mergedMetrics.Simplified = true;
            }

            return mergedMetrics;

        }
        #endregion
    }
}