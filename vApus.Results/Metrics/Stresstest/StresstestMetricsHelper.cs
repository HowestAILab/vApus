/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using vApus.Util;

namespace vApus.Results
{
    public static class StresstestMetricsHelper
    {
        #region Fields
        private static readonly string[] _readableMetricsHeadersConcurrency =
            {
                "Started At", "Time Left", "Measured Time", "Concurrency", "Log Entries Processed",
                "Throughput (responses / s)", "User Actions / s", "Avg. Response Time (ms)", "Max. Response Time (ms)",
                "Avg. Delay (ms)", "Errors"
            };
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
                "Started At", "Time Left (ms)", "Measured Time (ms", "Concurrency", "Run", "Log Entries Processed",
                "Log Entries", "Throughput (responses / s)", "User Actions / s", "Avg. Response Time (ms)", "Max. Response Time (ms)",
                " Avg. Delay (ms)", "Errors"
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
        public static StresstestMetrics GetMetrics(ConcurrencyResult result)
        {
            var metrics = new StresstestMetrics();
            metrics.StartMeasuringRuntime = result.StartedAt;
            metrics.MeasuredRunTime = (result.StoppedAt == DateTime.MinValue ? DateTime.Now : result.StoppedAt) - metrics.StartMeasuringRuntime;
            metrics.ConcurrentUsers = result.ConcurrentUsers;
            metrics.AverageResponseTime = new TimeSpan();
            metrics.MaxResponseTime = new TimeSpan();
            metrics.AverageDelay = new TimeSpan();

            if (result.RunResults.Count != 0)
            {
                long totalAndExtraLogEntriesProcessed = 0; //For break on last run sync.
                long baseLogEntryCount = 0;
                foreach (RunResult runResult in result.RunResults)
                {
                    StresstestMetrics runResultMetrics = GetMetrics(runResult);

                    metrics.AverageResponseTime = metrics.AverageResponseTime.Add(runResultMetrics.AverageResponseTime);
                    if (runResultMetrics.MaxResponseTime > metrics.MaxResponseTime)
                        metrics.MaxResponseTime = runResultMetrics.MaxResponseTime;
                    metrics.AverageDelay = metrics.AverageDelay.Add(runResultMetrics.AverageDelay);
                    metrics.LogEntries += runResultMetrics.LogEntries;
                    if (baseLogEntryCount == 0)
                        baseLogEntryCount = metrics.LogEntries;

                    totalAndExtraLogEntriesProcessed += runResultMetrics.LogEntriesProcessed;
                    metrics.ResponsesPerSecond += runResultMetrics.ResponsesPerSecond;
                    metrics.UserActionsPerSecond += runResultMetrics.UserActionsPerSecond;
                    metrics.Errors += runResultMetrics.Errors;
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

                metrics.EstimatedTimeLeft = GetEstimatedRuntimeLeft(metrics, result.StoppedAt == DateTime.MinValue);
            }
            return metrics;
        }
        /// <summary>
        ///     Get metrics for a run result.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static StresstestMetrics GetMetrics(RunResult result)
        {
            var metrics = new StresstestMetrics();
            metrics.StartMeasuringRuntime = result.StartedAt;
            metrics.MeasuredRunTime = (result.StoppedAt == DateTime.MinValue ? DateTime.Now : result.StoppedAt) - metrics.StartMeasuringRuntime;
            metrics.ConcurrentUsers = result.VirtualUserResults.Length;
            metrics.Run = result.Run;
            metrics.RerunCount = result.RerunCount;
            metrics.AverageResponseTime = new TimeSpan();
            metrics.MaxResponseTime = new TimeSpan();
            metrics.AverageDelay = new TimeSpan();

            int enteredUserResultsCount = 0;
            foreach (VirtualUserResult virtualUserResult in result.VirtualUserResults)
            {
                if (virtualUserResult.VirtualUser != null)
                    ++enteredUserResultsCount;

                StresstestMetrics virtualUserMetrics = GetMetrics(virtualUserResult);

                metrics.AverageResponseTime = metrics.AverageResponseTime.Add(virtualUserMetrics.AverageResponseTime);
                if (virtualUserMetrics.MaxResponseTime > metrics.MaxResponseTime)
                    metrics.MaxResponseTime = virtualUserMetrics.MaxResponseTime;
                metrics.AverageDelay = metrics.AverageDelay.Add(virtualUserMetrics.AverageDelay);
                metrics.LogEntries += virtualUserMetrics.LogEntries;
                metrics.LogEntriesProcessed += virtualUserMetrics.LogEntriesProcessed;
                metrics.ResponsesPerSecond += virtualUserMetrics.ResponsesPerSecond;
                metrics.UserActionsPerSecond += virtualUserMetrics.UserActionsPerSecond;
                metrics.Errors += virtualUserMetrics.Errors;
            }

            if (enteredUserResultsCount != 0)
            {
                metrics.AverageResponseTime = new TimeSpan(metrics.AverageResponseTime.Ticks / enteredUserResultsCount);
                metrics.AverageDelay = new TimeSpan(metrics.AverageDelay.Ticks / enteredUserResultsCount);
                metrics.EstimatedTimeLeft = GetEstimatedRuntimeLeft(metrics, result.StoppedAt == DateTime.MinValue);
            }
            return metrics;
        }
        private static StresstestMetrics GetMetrics(VirtualUserResult result)
        {
            var metrics = new StresstestMetrics();
            metrics.MaxResponseTime = new TimeSpan();
            metrics.LogEntries = result.LogEntryResults.LongLength;

            var userActionIndices = new List<string>();
            TimeSpan totalTimeToLastByte = new TimeSpan(), totalDelay = new TimeSpan();
            foreach (LogEntryResult logEntryResult in result.LogEntryResults)
                if (logEntryResult != null && logEntryResult.LogEntryIndex != null)
                {
                    ++metrics.LogEntriesProcessed;
                    if (!userActionIndices.Contains(logEntryResult.UserAction))
                        userActionIndices.Add(logEntryResult.UserAction);
                    var ttlb = new TimeSpan(logEntryResult.TimeToLastByteInTicks);
                    totalTimeToLastByte = totalTimeToLastByte.Add(ttlb);
                    if (ttlb > metrics.MaxResponseTime)
                        metrics.MaxResponseTime = ttlb;
                    totalDelay =
                        totalDelay.Add(new TimeSpan(logEntryResult.DelayInMilliseconds * TimeSpan.TicksPerMillisecond));
                    if (!string.IsNullOrEmpty(logEntryResult.Exception))
                        ++metrics.Errors;
                }

            if (metrics.LogEntriesProcessed != 0)
            {
                metrics.AverageResponseTime = new TimeSpan(totalTimeToLastByte.Ticks / metrics.LogEntriesProcessed);
                metrics.AverageDelay = new TimeSpan(totalDelay.Ticks / metrics.LogEntriesProcessed);
                metrics.ResponsesPerSecond = metrics.LogEntriesProcessed /
                                             ((double)(totalTimeToLastByte.Ticks + totalDelay.Ticks) /
                                              TimeSpan.TicksPerSecond);
                metrics.UserActionsPerSecond = userActionIndices.Count /
                                               ((double)(totalTimeToLastByte.Ticks + totalDelay.Ticks) /
                                                TimeSpan.TicksPerSecond);
            }

            return metrics;
        }
        private static TimeSpan GetEstimatedRuntimeLeft(StresstestMetrics metrics, bool stopped)
        {
            long estimatedRuntimeLeft = 0;
            if (stopped) //For run sync first this must be 0.
            {
                estimatedRuntimeLeft =
                    (long)
                    (((DateTime.Now - metrics.StartMeasuringRuntime).TotalMilliseconds / metrics.LogEntriesProcessed) *
                     (metrics.LogEntries - metrics.LogEntriesProcessed) * 10000);
                if (estimatedRuntimeLeft < 0)
                    estimatedRuntimeLeft = 0;
            }
            return new TimeSpan(estimatedRuntimeLeft);
        }

        /// <summary>
        ///     Will only work if you input the output of a Get...Metrics function.
        ///     This is because the rows are cached too.
        /// </summary>
        /// <param name="metrics"></param>
        /// <returns></returns>
        public static List<object[]> MetricsToRows(List<StresstestMetrics> metrics, bool readable)
        {
            var rows = new List<object[]>(metrics.Count);
            foreach (var m in metrics) rows.Add(readable ? ReadableMetricsToRow(m) : CalculatableMetricsToRow(m));
            return rows;
        }
        private static object[] ReadableMetricsToRow(StresstestMetrics metrics)
        {
            if (metrics.Run == 0)
                return new object[]
                    {
                        metrics.StartMeasuringRuntime.ToString(),
                        metrics.EstimatedTimeLeft.ToShortFormattedString(),
                        metrics.MeasuredRunTime.ToShortFormattedString(),
                        metrics.ConcurrentUsers,
                        metrics.LogEntriesProcessed + " / " +
                        (metrics.LogEntries == 0 ? "--" : metrics.LogEntries.ToString()),
                        Math.Round(metrics.ResponsesPerSecond, 2),
                        Math.Round(metrics.UserActionsPerSecond, 2),
                        Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2),
                        Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2),
                        Math.Round(metrics.AverageDelay.TotalMilliseconds, 2),
                        metrics.Errors
                    };
            return new object[]
                {
                    metrics.StartMeasuringRuntime.ToString(),
                    metrics.EstimatedTimeLeft.ToShortFormattedString(),
                    metrics.MeasuredRunTime.ToShortFormattedString(),
                    metrics.ConcurrentUsers,
                    metrics.Run,
                    metrics.LogEntriesProcessed + " / " +
                    (metrics.LogEntries == 0 ? "--" : metrics.LogEntries.ToString()),
                    Math.Round(metrics.ResponsesPerSecond, 2),
                    Math.Round(metrics.UserActionsPerSecond, 2),
                    Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2),
                    Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2),
                    Math.Round(metrics.AverageDelay.TotalMilliseconds, 2),
                    metrics.Errors
                };
        }
        private static object[] CalculatableMetricsToRow(StresstestMetrics metrics)
        {
            if (metrics.Run == 0)
                return new object[]
                    {
                        metrics.StartMeasuringRuntime.ToString(),
                        Math.Round(metrics.EstimatedTimeLeft.TotalMilliseconds, 2),
                        Math.Round(metrics.MeasuredRunTime.TotalMilliseconds, 2),
                        metrics.ConcurrentUsers,
                        metrics.LogEntriesProcessed,
                        metrics.LogEntries == 0 ? "--" : metrics.LogEntries.ToString(),
                        Math.Round(metrics.ResponsesPerSecond, 2),
                        Math.Round(metrics.UserActionsPerSecond, 2),
                        Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2),
                        Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2),
                        Math.Round(metrics.AverageDelay.TotalMilliseconds, 2),
                        metrics.Errors
                    };
            return new object[]
                {
                    metrics.StartMeasuringRuntime.ToString(),
                    Math.Round(metrics.EstimatedTimeLeft.TotalMilliseconds, 2),
                    Math.Round(metrics.MeasuredRunTime.TotalMilliseconds, 2),
                    metrics.ConcurrentUsers,
                    metrics.Run,
                    metrics.LogEntriesProcessed,
                    metrics.LogEntries == 0 ? "--" : metrics.LogEntries.ToString(),
                    Math.Round(metrics.ResponsesPerSecond, 2),
                    Math.Round(metrics.UserActionsPerSecond, 2),
                    Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2),
                    Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2),
                    Math.Round(metrics.AverageDelay.TotalMilliseconds, 2),
                    metrics.Errors
                };
        }
        #endregion
    }
}