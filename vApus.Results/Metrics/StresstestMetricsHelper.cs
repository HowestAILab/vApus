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
    public static class StresstestMetricsHelper {

        #region Fields
        private static readonly string[] _readableMetricsHeadersConcurrency =
            {
                "Started At", "Time Left", "Measured Time", "Concurrency", "Log Entries Processed", "Errors",
                "Throughput (responses / s)", "User Actions / s", "Avg. Response Time (ms)", "Max. Response Time (ms)",
                "Avg. Delay (ms)"
            };
        //"95th Percentile of the Response Times (ms)", 
        private static readonly string[] _readableMetricsHeadersRun =
            {
                "Started At", "Time Left", "Measured Time", "Concurrency", "Run", "Log Entries Processed", "Errors",
                "Throughput (responses / s)", "User Actions / s", "Avg. Response Time (ms)", "Max. Response Time (ms)",
                "Avg. Delay (ms)"
            };
        private static readonly string[] _calculatableMetricsHeadersConcurrency =
            {
                "Started At", "Time Left (ms)", "Measured Time (ms)", "Concurrency", "Log Entries Processed",
                "Log Entries", "Errors", "Throughput (responses / s)", "User Actions / s", "Avg. Response Time (ms)", "Max. Response Time (ms)",
                "Avg. Delay (ms)"
            };
        private static readonly string[] _calculatableMetricsHeadersRun =
            {
                "Started At", "Time Left (ms)", "Measured Time (ms)", "Concurrency", "Run", "Log Entries Processed",
                "Log Entries", "Errors", "Throughput (responses / s)", "User Actions / s", "Avg. Response Time (ms)", "Max. Response Time (ms)",
                "Avg. Delay (ms)"
            };

        /// <summary>
        /// In ms. If the calculate time is bigger than this and we do not want the 95th percentile of the response times a simplified metrics object is returned.
        /// You can also specify when calling a getmetrics function if you want the simplified output or not.
        /// </summary>
        private const int MAXGETMETRICSTIME = 3000;
        #endregion

        #region Functions
        public static string[] GetMetricsHeadersConcurrency(bool readable) { return readable ? _readableMetricsHeadersConcurrency : _calculatableMetricsHeadersConcurrency; }
        public static string[] GetMetricsHeadersRun(bool readable) { return readable ? _readableMetricsHeadersRun : _calculatableMetricsHeadersRun; }

        /// <summary>
        ///     Get metrics for a concurrency result. If the time to calculate this exceeds MAXGETMETRICSTIME a simplified metrics is returned. BUT only if you do not want the 95th percentile.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="timeToCalculate"></param>
        /// <param name="simplified">Only started at, measured time, estimate time left, concurrency, processed log entries, log entries, errors and StartsAndStopsRuns are put in the metrics object this is true and 
        /// if calculate95thPercentileResponseTimes == false. If the time to calculate exceeds 3 seconds this is set to true, if calculate95thPercentileResponseTimes == false.</param>
        /// <param name="calculate95thPercentileResponseTimes"></param>
        /// <returns></returns>
        public static StresstestMetrics GetMetrics(ConcurrencyResult result, ref bool simplified, bool calculate95thPercentileResponseTimes = true) {
            if (calculate95thPercentileResponseTimes == true) simplified = false;
            else if (simplified) return GetSimplifiedMetrics(result);

            var sw = Stopwatch.StartNew();
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
                    StresstestMetrics runResultMetrics = GetMetrics(runResult, ref simplified, false);
                    if (simplified) { //Should return simplified if calculating one of the runs takes too long.
                        sw.Stop();
                        return GetSimplifiedMetrics(result);
                    }

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
                    if (calculate95thPercentileResponseTimes) {
                        foreach (var vur in runResult.VirtualUserResults)
                            foreach (var ler in vur.LogEntryResults)
                                if (ler != null && ler.VirtualUser != null) {
                                    long lerTimeToLastByteInTicks = ler.TimeToLastByteInTicks;
                                    for (int i = 0; i != timesToLastByteInTicks.Count; i++)
                                        if (timesToLastByteInTicks[i] < lerTimeToLastByteInTicks) {
                                            timesToLastByteInTicks.Insert(i, lerTimeToLastByteInTicks);
                                            break;
                                        }
                                    while (timesToLastByteInTicks.Count > percent5) timesToLastByteInTicks.RemoveAt(percent5);
                                }
                    } else if (sw.ElapsedMilliseconds >= MAXGETMETRICSTIME) {
                        sw.Stop();
                        simplified = true;
                        return GetSimplifiedMetrics(result);
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
            sw.Stop();
            return metrics;
        }

        /// <summary>
        ///     Only started at, measured time, estimate time left, concurrency, processed log entries, log entries, errors and StartsAndStopsRuns
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static StresstestMetrics GetSimplifiedMetrics(ConcurrencyResult result) {
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

                foreach (RunResult runResult in result.RunResults) {
                    StresstestMetrics runResultMetrics = GetSimplifiedMetrics(runResult);

                    metrics.StartsAndStopsRuns.Add(new KeyValuePair<DateTime, DateTime>(runResult.StartedAt, runResult.StoppedAt));

                    metrics.LogEntries += runResultMetrics.LogEntries;
                    if (baseLogEntryCount == 0) baseLogEntryCount = metrics.LogEntries;

                    totalAndExtraLogEntriesProcessed += runResultMetrics.LogEntriesProcessed;
                    metrics.Errors += runResultMetrics.Errors;

                }
                for (int i = result.RunResults.Count; i < result.RunCount; i++)
                    metrics.LogEntries += baseLogEntryCount;

                if (metrics.LogEntries < totalAndExtraLogEntriesProcessed)
                    metrics.LogEntries = totalAndExtraLogEntriesProcessed;

                metrics.LogEntriesProcessed = totalAndExtraLogEntriesProcessed;

                metrics.EstimatedTimeLeft = GetEstimatedTimeLeft(metrics, result.StoppedAt == DateTime.MinValue);
            }
            return metrics;
        }

        /// <summary>
        ///     Get metrics for a run result.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="simplified">Only started at, measured time, estimate time left, concurrency, run, rerun count, processed log entries, log entries, errors if this is true and
        /// if calculate95thPercentileResponseTimes == false. If the time to calculate exceeds 3 seconds this is set to true, if calculate95thPercentileResponseTimes == false.</param>
        /// <param name="calculate95thPercentileResponseTimes"></param>
        /// <returns></returns>
        public static StresstestMetrics GetMetrics(RunResult result, ref bool simplified, bool calculate95thPercentileResponseTimes = true) {
            if (calculate95thPercentileResponseTimes == true) simplified = false;
            else if (simplified) return GetSimplifiedMetrics(result);

            var sw = Stopwatch.StartNew();
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
            foreach (VirtualUserResult virtualUserResult in result.VirtualUserResults)
                if (virtualUserResult != null) {
                    ++enteredUserResultsCount;

                    StresstestMetrics virtualUserMetrics = GetMetrics(virtualUserResult, ref simplified, !calculate95thPercentileResponseTimes);
                    if (simplified) {
                        sw.Stop();
                        return GetSimplifiedMetrics(result);
                    }

                    metrics.LogEntries += virtualUserMetrics.LogEntries;

                    if (calculate95thPercentileResponseTimes && percent5 == -1)
                        percent5 = (int)(result.VirtualUserResults.Length * virtualUserMetrics.LogEntries * 0.05) + 1;

                    metrics.AverageResponseTime = metrics.AverageResponseTime.Add(virtualUserMetrics.AverageResponseTime);
                    if (virtualUserMetrics.MaxResponseTime > metrics.MaxResponseTime) metrics.MaxResponseTime = virtualUserMetrics.MaxResponseTime;
                    metrics.AverageDelay = metrics.AverageDelay.Add(virtualUserMetrics.AverageDelay);
                    metrics.LogEntriesProcessed += virtualUserMetrics.LogEntriesProcessed;
                    metrics.ResponsesPerSecond += virtualUserMetrics.ResponsesPerSecond;
                    metrics.UserActionsPerSecond += virtualUserMetrics.UserActionsPerSecond;
                    metrics.Errors += virtualUserMetrics.Errors;

                    if (calculate95thPercentileResponseTimes) {
                        foreach (var ler in virtualUserResult.LogEntryResults)
                            if (ler != null && ler.VirtualUser != null) {
                                for (int i = 0; i != timesToLastByteInTicks.Count; i++)
                                    if (timesToLastByteInTicks[i] < ler.TimeToLastByteInTicks) {
                                        timesToLastByteInTicks.Insert(i, ler.TimeToLastByteInTicks);
                                        break;
                                    }
                                while (timesToLastByteInTicks.Count > percent5) timesToLastByteInTicks.RemoveAt(percent5);
                            }
                    } else if (sw.ElapsedMilliseconds >= MAXGETMETRICSTIME) {
                        sw.Stop();
                        simplified = true;
                        return GetSimplifiedMetrics(result);
                    }
                }

            if (enteredUserResultsCount != 0) {
                metrics.AverageResponseTime = new TimeSpan(metrics.AverageResponseTime.Ticks / enteredUserResultsCount);
                metrics.AverageDelay = new TimeSpan(metrics.AverageDelay.Ticks / enteredUserResultsCount);
                metrics.EstimatedTimeLeft = GetEstimatedTimeLeft(metrics, result.StoppedAt == DateTime.MinValue);
                long percentile95thResponseTimes = timesToLastByteInTicks[timesToLastByteInTicks.Count - 1];
                metrics.Percentile95thResponseTimes = percentile95thResponseTimes == 0 ? metrics.MaxResponseTime : new TimeSpan(percentile95thResponseTimes);
            }
            sw.Stop();
            return metrics;
        }
        /// <summary>
        ///     Only started at, measured time, estimate time left, concurrency, run, rerun count, processed log entries, log entries, errors.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static StresstestMetrics GetSimplifiedMetrics(RunResult result) {
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
            foreach (VirtualUserResult virtualUserResult in result.VirtualUserResults)
                if (virtualUserResult != null) {
                    ++enteredUserResultsCount;

                    StresstestMetrics virtualUserMetrics = GetSimplifiedMetrics(virtualUserResult);
                    metrics.LogEntries += virtualUserMetrics.LogEntries;

                    metrics.LogEntriesProcessed += virtualUserMetrics.LogEntriesProcessed;
                    metrics.Errors += virtualUserMetrics.Errors;
                }

            if (enteredUserResultsCount != 0)
                metrics.EstimatedTimeLeft = GetEstimatedTimeLeft(metrics, result.StoppedAt == DateTime.MinValue);

            return metrics;
        }


        private static StresstestMetrics GetMetrics(VirtualUserResult result, ref bool simplified, bool allowSimplifiedResults) {
            var metrics = new StresstestMetrics();

            var sw = Stopwatch.StartNew();

            metrics.MaxResponseTime = new TimeSpan();
            metrics.LogEntries = result.LogEntryResults.LongLength;

            var uniqueUserActions = new List<string>();
            TimeSpan totalTimeToLastByte = new TimeSpan(), totalDelay = new TimeSpan();
            foreach (LogEntryResult logEntryResult in result.LogEntryResults) {
                if (logEntryResult != null && logEntryResult.VirtualUser != null) {
                    ++metrics.LogEntriesProcessed;
                    if (!uniqueUserActions.Contains(logEntryResult.UserAction)) uniqueUserActions.Add(logEntryResult.UserAction);

                    var ttlb = new TimeSpan(logEntryResult.TimeToLastByteInTicks);
                    totalTimeToLastByte = totalTimeToLastByte.Add(ttlb);
                    if (ttlb > metrics.MaxResponseTime) metrics.MaxResponseTime = ttlb;

                    totalDelay = totalDelay.Add(new TimeSpan(logEntryResult.DelayInMilliseconds * TimeSpan.TicksPerMillisecond));
                    if (!string.IsNullOrEmpty(logEntryResult.Error)) ++metrics.Errors;
                }

                if (allowSimplifiedResults && sw.ElapsedMilliseconds >= MAXGETMETRICSTIME) {
                    sw.Stop();
                    simplified = true;
                    return GetSimplifiedMetrics(result);
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
        /// <summary>
        /// Only log entries processed, log entries and errors are returned.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static StresstestMetrics GetSimplifiedMetrics(VirtualUserResult result) {
            var metrics = new StresstestMetrics();
            metrics.LogEntries = result.LogEntryResults.LongLength;

            foreach (LogEntryResult logEntryResult in result.LogEntryResults)
                if (logEntryResult != null && logEntryResult.VirtualUser != null) {
                    ++metrics.LogEntriesProcessed;
                    if (!string.IsNullOrEmpty(logEntryResult.Error)) ++metrics.Errors;
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
                                    var logEntryResults = rr.VirtualUserResults[vur].LogEntryResults;
                                    if (logEntryResults != null) {
                                        for (int ler = 0; ler != logEntryResults.Length; ler++) {
                                            ++logEntries;
                                            var logEntryResult = logEntryResults[ler];
                                            if (logEntryResult != null && logEntryResult.VirtualUser != null) ++logEntriesProcessed;
                                        }
                                    }
                                }

                                if (logEntriesProcessed == 0) {
                                    ++runs;
                                } else {
                                    var measuredTime = (now - rr.StartedAt).Ticks;
                                    estimatedRuntimeLeft = (long)((measuredTime / logEntriesProcessed) * (logEntries - logEntriesProcessed));
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
            } catch {
            }
            return new TimeSpan(estimatedRuntimeLeft);
        }

        /// <summary>
        /// </summary>
        /// <param name="metrics"></param>
        /// <returns></returns>
        public static List<object[]> MetricsToRows(List<StresstestMetrics> metrics, bool readable, bool simplified) {
            var rows = new List<object[]>(metrics.Count);
            foreach (var m in metrics) rows.Add(readable ? ReadableMetricsToRow(m, simplified) : CalculatableMetricsToRow(m, simplified));
            return rows;
        }
        private static object[] ReadableMetricsToRow(StresstestMetrics metrics, bool simplified) {
            if (metrics.Run == 0)
                return new object[]
                    {
                        metrics.StartMeasuringTime.ToString(),
                        metrics.EstimatedTimeLeft.ToShortFormattedString(),
                        metrics.MeasuredTime.ToShortFormattedString(),
                        metrics.Concurrency,
                        metrics.LogEntriesProcessed + " / " +
                        (metrics.LogEntries == 0 ? "--" : metrics.LogEntries.ToString()),
                        metrics.Errors,
                        simplified ? "--" : Math.Round(metrics.ResponsesPerSecond, 2).ToString(),
                        simplified ? "--" : Math.Round(metrics.UserActionsPerSecond, 2).ToString(),
                        simplified ? "--" : Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2).ToString(),
                        simplified ? "--" : Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2).ToString(),
                        simplified ? "--" : Math.Round(metrics.AverageDelay.TotalMilliseconds, 2).ToString()
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
                    metrics.Errors,
                    simplified ? "--" : Math.Round(metrics.ResponsesPerSecond, 2).ToString(),
                    simplified ? "--" : Math.Round(metrics.UserActionsPerSecond, 2).ToString(),
                    simplified ? "--" : Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2).ToString(),
                    simplified ? "--" : Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2).ToString(),
                    simplified ? "--" : Math.Round(metrics.AverageDelay.TotalMilliseconds, 2).ToString()
                };
        }
        private static object[] CalculatableMetricsToRow(StresstestMetrics metrics, bool simplified) {
            if (metrics.Run == 0)
                return new object[]
                    {
                        metrics.StartMeasuringTime.ToString(),
                        Math.Round(metrics.EstimatedTimeLeft.TotalMilliseconds, 2),
                        Math.Round(metrics.MeasuredTime.TotalMilliseconds, 2),
                        metrics.Concurrency,
                        metrics.LogEntriesProcessed,
                        metrics.LogEntries == 0 ? "--" : metrics.LogEntries.ToString(),
                        metrics.Errors,
                        simplified ? "--" : Math.Round(metrics.ResponsesPerSecond, 2).ToString(),
                        simplified ? "--" : Math.Round(metrics.UserActionsPerSecond, 2).ToString(),
                        simplified ? "--" : Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2).ToString(),
                        simplified ? "--" : Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2).ToString(),
                        simplified ? "--" : Math.Round(metrics.AverageDelay.TotalMilliseconds, 2).ToString()
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
                    metrics.Errors,
                    simplified ? "--" : Math.Round(metrics.ResponsesPerSecond, 2).ToString(),
                    simplified ? "--" : Math.Round(metrics.UserActionsPerSecond, 2).ToString(),
                    simplified ? "--" : Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2).ToString(),
                    simplified ? "--" : Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2).ToString(),
                    simplified ? "--" : Math.Round(metrics.AverageDelay.TotalMilliseconds, 2).ToString()
                };
        }

        /// <summary>
        /// Used in many-to-one distributed tests (1 stresstests workload divided over multiple slaves), to be able to calcullate the correct metrics. 
        /// </summary>
        /// <param name="metricsCaches">If only one entry the list will be returned directly.</param>
        /// <returns></returns>
        public static StresstestMetricsCache MergeStresstestMetricsCaches(List<StresstestMetricsCache> metricsCaches) {
            int count = metricsCaches.Count;
            if (count == 0)
                throw new Exception("The given list must contain more than 0 metrics caches.");
            else if (count == 1)
                return metricsCaches[0];

            bool calculateSimplifiedMetrics = false;

            //First get all the metrics...
            int maxConcurrencyAndRunCount = 0;
            var allMetrics = new List<List<StresstestMetrics>>(count);
            foreach (var metricsCache in metricsCaches) {
                if (metricsCache.CalculatedSimplifiedMetrics) calculateSimplifiedMetrics = true;
                var m = metricsCache.GetAllMetrics();
                allMetrics.Add(m);
                if (m.Count > maxConcurrencyAndRunCount)
                    maxConcurrencyAndRunCount = m.Count;
            }

            //...then pivot them (We do not want to merge all the results for one stresstest, but the concurrencies and runs over the tests at the same index).
            var pivotedMetrics = new List<List<StresstestMetrics>>(maxConcurrencyAndRunCount);
            for (int i = 0; i < maxConcurrencyAndRunCount; i++) {
                var l = new List<StresstestMetrics>(count);
                for (int j = 0; j < count; j++) {
                    var part = allMetrics[j];
                    if (i < part.Count) l.Add(part[i]);
                }
                pivotedMetrics.Add(l);
            }

            //Now do the merge.
            var mergedMetricsCache = new StresstestMetricsCache() { CalculatedSimplifiedMetrics = calculateSimplifiedMetrics } ;
            foreach (var toBeMerged in pivotedMetrics)
                mergedMetricsCache.Add(MergeStresstestMetrics(toBeMerged));
            return mergedMetricsCache;
        }
        /// <summary>
        /// Used in many-to-one distributed tests (1 stresstests workload divided over multiple slaves), to be able to calcullate the correct metrics. 
        /// </summary>
        /// <param name="metrics">If only one entry the list will be returned directly..</param>
        /// <returns></returns>
        private static StresstestMetrics MergeStresstestMetrics(List<StresstestMetrics> metrics) {
            int count = metrics.Count;
            if (count == 0)
                throw new Exception("The given list must contain more than 0 metrics.");
            else if (count == 1)
                return metrics[0];

            var mergedMetrics = new StresstestMetrics();
            mergedMetrics.StartMeasuringTime = metrics[0].StartMeasuringTime;
            mergedMetrics.StartsAndStopsRuns = metrics[0].StartsAndStopsRuns;

            foreach (var m in metrics) {
                if (m.EstimatedTimeLeft > mergedMetrics.EstimatedTimeLeft) mergedMetrics.EstimatedTimeLeft = m.EstimatedTimeLeft;
                if (m.MeasuredTime > mergedMetrics.MeasuredTime) mergedMetrics.MeasuredTime = m.MeasuredTime;
                mergedMetrics.Concurrency += m.Concurrency;
                if (m.Run > mergedMetrics.Run) mergedMetrics.Run = m.Run;
                if (m.RerunCount > mergedMetrics.RerunCount) mergedMetrics.RerunCount = m.RerunCount;
                mergedMetrics.LogEntries += m.LogEntries;
                mergedMetrics.ResponsesPerSecond += m.ResponsesPerSecond;
                mergedMetrics.UserActionsPerSecond += m.UserActionsPerSecond;

                mergedMetrics.AverageResponseTime += new TimeSpan(m.AverageResponseTime.Ticks / count);

                if (m.MaxResponseTime > mergedMetrics.MaxResponseTime) mergedMetrics.MaxResponseTime = m.MaxResponseTime;

                mergedMetrics.AverageDelay += new TimeSpan(m.AverageDelay.Ticks / count);
                mergedMetrics.Errors += m.Errors;
                mergedMetrics.LogEntriesProcessed += m.LogEntriesProcessed;
            }

            return mergedMetrics;

        }
        #endregion
    }
}