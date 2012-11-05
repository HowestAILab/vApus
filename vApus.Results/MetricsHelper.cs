using System;
using vApus.Results.Model;

namespace vApus.Results
{
    public static class MetricsHelper
    {
        private static string[] _metricsHeadersConcurrency = { "Started At", "Time Left", "Measured Time", "Concurrency", "Throughput (responses / s)", "Response Time (ms)", "Max. Response Time (ms)", "Delay (ms)", "Errors" };
        private static string[] _metricsHeadersRun = { "Started At", "Time Left", "Measured Time", "Concurrency", "Run", "Throughput (responses / s)", "Response Time (ms)", "Max. Response Time (ms)", "Delay (ms)", "Errors" };

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
        public static Metrics GetMetrics(ConcurrencyResult concurrencyResult)
        {
            Metrics metrics = new Metrics();
            metrics.StartMeasuringRuntime = concurrencyResult.StartedAt;
            metrics.MeasuredRunTime = DateTime.Now - metrics.StartMeasuringRuntime;
            metrics.AverageResponseTime = new TimeSpan();
            metrics.MaxResponseTime = TimeSpan.MinValue;
            metrics.AverageDelay = new TimeSpan();

            long totalAndExtraLogEntriesProcessed = 0; //For break on last run sync.

            foreach (var result in concurrencyResult.RunResults)
            {
                Metrics runResultMetrics = GetMetrics(result);

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

            metrics.Throughput /= concurrencyResult.RunResults.Count;
            metrics.AverageResponseTime = new TimeSpan(metrics.AverageResponseTime.Ticks / concurrencyResult.RunResults.Count);
            metrics.AverageDelay = new TimeSpan(metrics.AverageDelay.Ticks / concurrencyResult.RunResults.Count);

            metrics.EstimatedTimeLeft = GetEstimatedRuntimeLeft(metrics);
            return metrics;
        }
        /// <summary>
        /// Get metrics for a run result.
        /// </summary>
        /// <param name="runResult"></param>
        /// <returns></returns>
        public static Metrics GetMetrics(RunResult runResult)
        {
            Metrics metrics = new Metrics();
            metrics.StartMeasuringRuntime = runResult.StartedAt;
            metrics.MeasuredRunTime = DateTime.Now - metrics.StartMeasuringRuntime;
            metrics.AverageResponseTime = new TimeSpan();
            metrics.MaxResponseTime = TimeSpan.MinValue;
            metrics.AverageDelay = new TimeSpan();

            int enteredUserResultsCount = 0;
            foreach (var result in runResult.VirtualUserResults)
            {
                if (result.VirtualUser != null)
                    ++enteredUserResultsCount;

                Metrics virtualuserMetrics = GetMetrics(result);

                metrics.AverageResponseTime = metrics.AverageResponseTime.Add(virtualuserMetrics.AverageResponseTime);
                if (virtualuserMetrics.MaxResponseTime > metrics.MaxResponseTime)
                    metrics.MaxResponseTime = virtualuserMetrics.MaxResponseTime;
                metrics.AverageDelay = metrics.AverageDelay.Add(virtualuserMetrics.AverageDelay);
                metrics.LogEntriesProcessed += virtualuserMetrics.LogEntriesProcessed;
                metrics.Throughput += virtualuserMetrics.Throughput;
                metrics.Errors += virtualuserMetrics.Errors;
            }

            if (enteredUserResultsCount != 0)
            {
                metrics.AverageResponseTime = new TimeSpan(metrics.AverageResponseTime.Ticks / enteredUserResultsCount);
                metrics.AverageDelay = new TimeSpan(metrics.AverageDelay.Ticks / enteredUserResultsCount);
            }

            metrics.EstimatedTimeLeft = GetEstimatedRuntimeLeft(metrics);
            return metrics;
        }
        private static Metrics GetMetrics(VirtualUserResult virtualUserResult)
        {
            Metrics metrics = new Metrics();
            //_metrics.MeasuredRunTime = _sw.Elapsed;
            metrics.MaxResponseTime = TimeSpan.MinValue;
            metrics.LogEntriesProcessed = 0;
            metrics.Errors = 0;

            TimeSpan totalTimeToLastByte = new TimeSpan(), totalDelay = new TimeSpan();
            for (int i = 0; i != virtualUserResult.LogEntryResults.Length; i++)
            {
                var result = virtualUserResult.LogEntryResults[i];
                if (!result.Empty)
                {
                    ++metrics.LogEntriesProcessed;
                    totalTimeToLastByte = totalTimeToLastByte.Add(result.TimeToLastByte);
                    if (result.TimeToLastByte > metrics.MaxResponseTime)
                        metrics.MaxResponseTime = result.TimeToLastByte;
                    totalDelay = totalDelay.Add(new TimeSpan(result.DelayInMilliseconds * TimeSpan.TicksPerMillisecond));
                    if (result.Exception != null)
                        ++metrics.Errors;
                }
            }

            metrics.AverageResponseTime = new TimeSpan(totalTimeToLastByte.Ticks / metrics.LogEntriesProcessed);
            metrics.AverageDelay = new TimeSpan(totalDelay.Ticks / metrics.LogEntriesProcessed);
            metrics.Throughput = (double)metrics.LogEntriesProcessed / ((double)(totalTimeToLastByte.Ticks + totalDelay.Ticks) / TimeSpan.TicksPerSecond);

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
                return new object[]{
                    metrics.StartMeasuringRuntime,
                    metrics.EstimatedTimeLeft,
                    metrics.MeasuredRunTime,
                    metrics.ConcurrentUsers,
                    metrics.Run,
                    metrics.LogEntriesProcessed,
                    metrics.LogEntries,
                    metrics.Throughput,
                    metrics.AverageResponseTime,
                    metrics.MaxResponseTime,
                    metrics.AverageDelay,
                    metrics.Errors
                };
            return new object[]{
                metrics.StartMeasuringRuntime,
                metrics.EstimatedTimeLeft,
                metrics.MeasuredRunTime,
                metrics.ConcurrentUsers,
                metrics.LogEntriesProcessed,
                metrics.LogEntries,
                metrics.Throughput,
                metrics.AverageResponseTime,
                metrics.MaxResponseTime,
                metrics.AverageDelay,
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