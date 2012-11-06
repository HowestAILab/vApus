using System;
using vApus.Results.Model;
using vApus.Util;

namespace vApus.Results
{
    public static class MetricsHelper
    {
        private static string[] _metricsHeadersConcurrency = { "Started At", "Time Left", "Measured Time", "Concurrency", "Log Entries Processed", "Log Entries", "Throughput (responses / s)", "Response Time (ms)", "Max. Response Time (ms)", "Delay (ms)", "Errors" };
        private static string[] _metricsHeadersRun = { "Started At", "Time Left", "Measured Time", "Concurrency", "Run", "Log Entries Processed", "Log Entries", "Throughput (responses / s)", "Response Time (ms)", "Max. Response Time (ms)", "Delay (ms)", "Errors" };

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

                foreach (var runResult in result.RunResults)
                {
                    Metrics runResultMetrics = GetMetrics(runResult);

                    metrics.AverageResponseTime = metrics.AverageResponseTime.Add(runResultMetrics.AverageResponseTime);
                    if (runResultMetrics.MaxResponseTime > metrics.MaxResponseTime)
                        metrics.MaxResponseTime = runResultMetrics.MaxResponseTime;
                    metrics.AverageDelay = metrics.AverageDelay.Add(runResultMetrics.AverageDelay);
                    totalAndExtraLogEntriesProcessed += runResultMetrics.LogEntriesProcessed;
                    metrics.Throughput += runResultMetrics.Throughput;
                    metrics.Errors += runResultMetrics.Errors;
                }
                if (metrics.LogEntries < totalAndExtraLogEntriesProcessed)
                    metrics.LogEntries = totalAndExtraLogEntriesProcessed;

                metrics.LogEntriesProcessed = totalAndExtraLogEntriesProcessed;

                metrics.Throughput /= result.RunResults.Count;
                metrics.AverageResponseTime = new TimeSpan(metrics.AverageResponseTime.Ticks / result.RunResults.Count);
                metrics.AverageDelay = new TimeSpan(metrics.AverageDelay.Ticks / result.RunResults.Count);

                metrics.EstimatedTimeLeft = GetEstimatedRuntimeLeft(metrics);
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
            metrics.ConcurrentUsers = result.Run;
            metrics.AverageResponseTime = new TimeSpan();
            metrics.MaxResponseTime = new TimeSpan();
            metrics.AverageDelay = new TimeSpan();

            int enteredUserResultsCount = 0;
            foreach (var virtualUserResult in result.VirtualUserResults)
            {
                if (virtualUserResult.VirtualUser != null)
                    ++enteredUserResultsCount;

                Metrics virtualuserMetrics = GetMetrics(virtualUserResult);

                metrics.AverageResponseTime = metrics.AverageResponseTime.Add(virtualuserMetrics.AverageResponseTime);
                if (virtualuserMetrics.MaxResponseTime > metrics.MaxResponseTime)
                    metrics.MaxResponseTime = virtualuserMetrics.MaxResponseTime;
                metrics.AverageDelay = metrics.AverageDelay.Add(virtualuserMetrics.AverageDelay);
                metrics.LogEntries += virtualuserMetrics.LogEntries;
                metrics.LogEntriesProcessed += virtualuserMetrics.LogEntriesProcessed;
                metrics.Throughput += virtualuserMetrics.Throughput;
                metrics.Errors += virtualuserMetrics.Errors;
            }

            if (enteredUserResultsCount != 0)
            {
                metrics.AverageResponseTime = new TimeSpan(metrics.AverageResponseTime.Ticks / enteredUserResultsCount);
                metrics.AverageDelay = new TimeSpan(metrics.AverageDelay.Ticks / enteredUserResultsCount);
                metrics.EstimatedTimeLeft = GetEstimatedRuntimeLeft(metrics);
            }
            return metrics;
        }
        private static Metrics GetMetrics(VirtualUserResult result)
        {
            Metrics metrics = new Metrics();
            metrics.MaxResponseTime = new TimeSpan();
            metrics.LogEntries = result.LogEntryResults.LongLength;

            TimeSpan totalTimeToLastByte = new TimeSpan(), totalDelay = new TimeSpan();
            foreach (var logEntryResult in result.LogEntryResults)
                if (logEntryResult != null && logEntryResult.LogEntryIndex != null)
                {
                    ++metrics.LogEntriesProcessed;
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
                metrics.Throughput = (double)metrics.LogEntriesProcessed / ((double)(totalTimeToLastByte.Ticks + totalDelay.Ticks) / TimeSpan.TicksPerSecond);
            }

            return metrics;
        }
        public static string[] Headers()
        {
            string[] headers = new string[11];
            return headers;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metrics"></param>
        /// <param name="addRunMetric">Fale if on concurrency level.</param>
        /// <returns></returns>
        public static object[] MetricsToRow(Metrics metrics, bool addRunMetric)
        {
            if (addRunMetric)
                return new object[]
                {
                    metrics.StartMeasuringRuntime.ToShortDateString(),
                    metrics.EstimatedTimeLeft.ToShortFormattedString(),
                    metrics.MeasuredRunTime.ToShortFormattedString(),
                    metrics.ConcurrentUsers,
                    metrics.Run,
                    metrics.LogEntriesProcessed,
                    metrics.LogEntries,
                    metrics.Throughput,
                    metrics.AverageResponseTime.TotalMilliseconds,
                    metrics.MaxResponseTime.TotalMilliseconds,
                    metrics.AverageDelay.TotalMilliseconds,
                    metrics.Errors
                };
            return new object[]
            {
                metrics.StartMeasuringRuntime.ToShortDateString(),
                metrics.EstimatedTimeLeft.ToShortFormattedString(),
                metrics.MeasuredRunTime.ToShortFormattedString(),
                metrics.ConcurrentUsers,
                metrics.LogEntriesProcessed,
                metrics.LogEntries,
                metrics.Throughput,
                metrics.AverageResponseTime.TotalMilliseconds,
                metrics.MaxResponseTime.TotalMilliseconds,
                metrics.AverageDelay.TotalMilliseconds,
                metrics.Errors
            };
        }


        private static TimeSpan GetEstimatedRuntimeLeft(Metrics metrics)
        {
            long estimatedRuntimeLeft = 0;
            //if (IsMeasuringTime) //For run sync first this must be 0.
            //{
            estimatedRuntimeLeft = (long)(((DateTime.Now - metrics.StartMeasuringRuntime).TotalMilliseconds / metrics.LogEntriesProcessed) * (metrics.LogEntries - metrics.LogEntriesProcessed) * 10000);
            if (estimatedRuntimeLeft < 0)
                estimatedRuntimeLeft = 0;
            //}
            return new TimeSpan(estimatedRuntimeLeft);
        }

    }
}