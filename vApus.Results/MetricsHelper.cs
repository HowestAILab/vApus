using System;
using System.Collections.Generic;
using vApus.Results.Model;

namespace vApus.Results
{
    public static class MetricsHelper
    {
        private static string[] _metricsHeadersConcurrency = { "Started At", "Time Left", "Measured Time", "Concurrency", "Log Entries Processed", "Throughput (log entries / s)", "Throughput (user actions / s)", "Response Time (ms)", "Max. Response Time (ms)", "Delay (ms)", "Errors" };
        private static string[] _metricsHeadersRun = { "Started At", "Time Left", "Measured Time", "Concurrency", "Run", "Log Entries Processed", "Throughput (log entries / s)", "Throughput (user actions / s)", "Response Time (ms)", "Max. Response Time (ms)", "Delay (ms)", "Errors" };

        public static string[] MetricsHeadersConcurrency
        {
            get { return _metricsHeadersConcurrency; }
        }
        public static string[] MetricsHeadersRun
        {
            get { return _metricsHeadersRun; }
        }

        /// Get metrics for a concurrency result.
        /// </summary>
        /// <param name="concurrencyResult"></param>
        /// <returns></returns>
        public static Metrics GetMetrics(ConcurrencyResult result)
        {
            Metrics metrics = new Metrics();
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
                foreach (var runResult in result.RunResults)
                {
                    Metrics runResultMetrics = GetMetrics(runResult);

                    metrics.AverageResponseTime = metrics.AverageResponseTime.Add(runResultMetrics.AverageResponseTime);
                    if (runResultMetrics.MaxResponseTime > metrics.MaxResponseTime)
                        metrics.MaxResponseTime = runResultMetrics.MaxResponseTime;
                    metrics.AverageDelay = metrics.AverageDelay.Add(runResultMetrics.AverageDelay);
                    metrics.LogEntries += runResultMetrics.LogEntries;
                    if (baseLogEntryCount == 0)
                        baseLogEntryCount = metrics.LogEntries;

                    totalAndExtraLogEntriesProcessed += runResultMetrics.LogEntriesProcessed;
                    metrics.LogEntriesPerSecond += runResultMetrics.LogEntriesPerSecond;
                    metrics.UserActionsPerSecond += runResultMetrics.UserActionsPerSecond;
                    metrics.Errors += runResultMetrics.Errors;
                }
                for (int i = result.RunResults.Count; i < result.RunCount; i++)
                    metrics.LogEntries += baseLogEntryCount;

                if (metrics.LogEntries < totalAndExtraLogEntriesProcessed)
                    metrics.LogEntries = totalAndExtraLogEntriesProcessed;

                metrics.LogEntriesProcessed = totalAndExtraLogEntriesProcessed;

                metrics.LogEntriesPerSecond /= result.RunResults.Count;
                metrics.UserActionsPerSecond /= result.RunResults.Count;
                metrics.AverageResponseTime = new TimeSpan(metrics.AverageResponseTime.Ticks / result.RunResults.Count);
                metrics.AverageDelay = new TimeSpan(metrics.AverageDelay.Ticks / result.RunResults.Count);

                metrics.EstimatedTimeLeft = GetEstimatedRuntimeLeft(metrics, result.StoppedAt == DateTime.MinValue);
            }
            return metrics;
        }
        /// <summary>
        /// Get metrics for a run result.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Metrics GetMetrics(RunResult result)
        {
            Metrics metrics = new Metrics();
            metrics.StartMeasuringRuntime = result.StartedAt;
            metrics.MeasuredRunTime = (result.StoppedAt == DateTime.MinValue ? DateTime.Now : result.StoppedAt) - metrics.StartMeasuringRuntime;
            metrics.ConcurrentUsers = result.VirtualUserResults.Length;
            metrics.Run = result.Run;
            metrics.RerunCount = result.RerunCount;
            metrics.AverageResponseTime = new TimeSpan();
            metrics.MaxResponseTime = new TimeSpan();
            metrics.AverageDelay = new TimeSpan();

            int enteredUserResultsCount = 0;
            foreach (var virtualUserResult in result.VirtualUserResults)
            {
                if (virtualUserResult.VirtualUser != null)
                    ++enteredUserResultsCount;

                Metrics virtualUserMetrics = GetMetrics(virtualUserResult);

                metrics.AverageResponseTime = metrics.AverageResponseTime.Add(virtualUserMetrics.AverageResponseTime);
                if (virtualUserMetrics.MaxResponseTime > metrics.MaxResponseTime)
                    metrics.MaxResponseTime = virtualUserMetrics.MaxResponseTime;
                metrics.AverageDelay = metrics.AverageDelay.Add(virtualUserMetrics.AverageDelay);
                metrics.LogEntries += virtualUserMetrics.LogEntries;
                metrics.LogEntriesProcessed += virtualUserMetrics.LogEntriesProcessed;
                metrics.LogEntriesPerSecond += virtualUserMetrics.LogEntriesPerSecond;
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
        private static Metrics GetMetrics(VirtualUserResult result)
        {
            Metrics metrics = new Metrics();
            metrics.MaxResponseTime = new TimeSpan();
            metrics.LogEntries = result.LogEntryResults.LongLength;

            List<string> userActionIndices = new List<string>();
            TimeSpan totalTimeToLastByte = new TimeSpan(), totalDelay = new TimeSpan();
            foreach (var logEntryResult in result.LogEntryResults)
                if (logEntryResult != null && logEntryResult.LogEntryIndex != null)
                {
                    ++metrics.LogEntriesProcessed;
                    if (!userActionIndices.Contains(logEntryResult.UserAction))
                        userActionIndices.Add(logEntryResult.UserAction);
                    totalTimeToLastByte = totalTimeToLastByte.Add(logEntryResult.TimeToLastByte);
                    if (logEntryResult.TimeToLastByte > metrics.MaxResponseTime)
                        metrics.MaxResponseTime = logEntryResult.TimeToLastByte;
                    totalDelay = totalDelay.Add(new TimeSpan(logEntryResult.DelayInMilliseconds * TimeSpan.TicksPerMillisecond));
                    if (logEntryResult.Exception != null)
                        ++metrics.Errors;
                }

            if (metrics.LogEntriesProcessed != 0)
            {
                metrics.AverageResponseTime = new TimeSpan(totalTimeToLastByte.Ticks / metrics.LogEntriesProcessed);
                metrics.AverageDelay = new TimeSpan(totalDelay.Ticks / metrics.LogEntriesProcessed);
                metrics.LogEntriesPerSecond = (double)metrics.LogEntriesProcessed / ((double)(totalTimeToLastByte.Ticks + totalDelay.Ticks) / TimeSpan.TicksPerSecond);
                metrics.UserActionsPerSecond = (double)userActionIndices.Count / ((double)(totalTimeToLastByte.Ticks + totalDelay.Ticks) / TimeSpan.TicksPerSecond);
            }

            return metrics;
        }

        private static TimeSpan GetEstimatedRuntimeLeft(Metrics metrics, bool stopped)
        {
            long estimatedRuntimeLeft = 0;
            if (stopped) //For run sync first this must be 0.
            {
                estimatedRuntimeLeft = (long)(((DateTime.Now - metrics.StartMeasuringRuntime).TotalMilliseconds / metrics.LogEntriesProcessed) * (metrics.LogEntries - metrics.LogEntriesProcessed) * 10000);
                if (estimatedRuntimeLeft < 0)
                    estimatedRuntimeLeft = 0;
            }
            return new TimeSpan(estimatedRuntimeLeft);
        }

    }
}