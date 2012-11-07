using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vApus.Results.Model;
using vApus.Util;

namespace vApus.Results
{
    /// <summary>
    /// Should be kept where the results are visualized (rows in a datagridview) and used together with MetricsHelper.
    /// </summary>
    public class MetricsCache
    {
        public EventHandler<MetricAddedOrUpdatedEventArgs> MetricAddedOrUpdated;

        private List<MetricsCacheObject> _cache = new List<MetricsCacheObject>();

        /// <summary>
        /// Caching metrics for results that are not known (master-slave vApus setup).
        /// </summary>
        /// <param name="metrics"></param>
        public void Add(Metrics metrics)
        {
            __AddOrUpdate(metrics);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metrics">eg MetricsHelper.GetMetrics(result)</param>
        /// <param name="result">eg result</param>
        public void AddOrUpdate(Metrics metrics, ConcurrencyResult result)
        {
            __AddOrUpdate(metrics, result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metrics">eg MetricsHelper.GetMetrics(result)</param>
        /// <param name="result">eg result</param>
        public void AddOrUpdate(Metrics metrics, RunResult result)
        {
            __AddOrUpdate(metrics, result);
        }
        private void __AddOrUpdate(Metrics metrics, object result = null)
        {
            bool add = true;
            var cacheObject = new MetricsCacheObject() { Metrics = metrics, Row = MetricsToRow(metrics), Result = result };
            if (result != null)
                for (int i = 0; i != _cache.Count; i++)
                {
                    var o = _cache[i];
                    if (o.Result == result)
                    {
                        _cache[i] = cacheObject;
                        add = false;
                        break;
                    }
                }

            if (add)
                _cache.Add(cacheObject);

            if (MetricAddedOrUpdated != null)
                MetricAddedOrUpdated(this, new MetricAddedOrUpdatedEventArgs(metrics, add, cacheObject.Row, cacheObject.Result));
        }
        private object[] MetricsToRow(Metrics metrics)
        {
            if (metrics.Run != 0)
                return new object[]
                {
                    metrics.StartMeasuringRuntime.ToShortDateString() + " " + metrics.StartMeasuringRuntime.ToShortTimeString(),
                    metrics.EstimatedTimeLeft.ToShortFormattedString(),
                    metrics.MeasuredRunTime.ToShortFormattedString(),
                    metrics.ConcurrentUsers,
                    metrics.Run,
                    metrics.LogEntriesProcessed + " / " + (metrics.LogEntries == 0 ?  "--" : metrics.LogEntries.ToString()),
                    Math.Round(metrics.LogEntriesPerSecond, 2),
                    Math.Round(metrics.UserActionsPerSecond, 2),
                    Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2),
                    Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2),
                    Math.Round(metrics.AverageDelay.TotalMilliseconds, 2),
                    metrics.Errors
                };
            return new object[]
            {
                metrics.StartMeasuringRuntime.ToShortDateString() + " " + metrics.StartMeasuringRuntime.ToShortTimeString(),
                metrics.EstimatedTimeLeft.ToShortFormattedString(),
                metrics.MeasuredRunTime.ToShortFormattedString(),
                metrics.ConcurrentUsers,
                metrics.LogEntriesProcessed + " / " + (metrics.LogEntries == 0 ?  "--" : metrics.LogEntries.ToString()),
                Math.Round(metrics.LogEntriesPerSecond, 2),
                Math.Round(metrics.UserActionsPerSecond, 2),
                Math.Round(metrics.AverageResponseTime.TotalMilliseconds, 2),
                Math.Round(metrics.MaxResponseTime.TotalMilliseconds, 2),
                Math.Round(metrics.AverageDelay.TotalMilliseconds, 2),
                metrics.Errors
            };
        }

        /// <summary>
        /// This will update the metrics if possible, otherwise it will just return them.
        /// </summary>
        /// <returns></returns>
        public List<Metrics> GetConcurrencyMetrics()
        {
            var metrics = new List<Metrics>();
            foreach (var o in _cache)
                if (o.Metrics.Run == 0)
                {
                    if (o.Result != null)
                    {
                        o.Metrics = MetricsHelper.GetMetrics(o.Result as ConcurrencyResult);
                        o.Row = MetricsToRow(o.Metrics);
                    }
                    metrics.Add(o.Metrics);
                }
            return metrics;
        }
        /// <summary>
        /// This will update the metrics if possible, otherwise it will just return them.
        /// </summary>
        /// <returns></returns>
        public List<Metrics> GetRunMetrics()
        {
            var metrics = new List<Metrics>();
            foreach (var o in _cache)
                if (o.Metrics.Run != 0)
                {
                    if (o.Result != null)
                    {
                        o.Metrics = MetricsHelper.GetMetrics(o.Result as RunResult);
                        o.Row = MetricsToRow(o.Metrics);
                    }
                    metrics.Add(o.Metrics);
                }
            return metrics;
        }

        /// <summary>
        /// Will only work if you input the output of a Get...Metrics function.
        /// This is because the rows are cached too.
        /// </summary>
        /// <param name="metrics"></param>
        /// <returns></returns>
        public List<object[]> MetricsToRows(List<Metrics> metrics)
        {
            var rows = new List<object[]>(metrics.Count);
            foreach (var m in metrics)
                foreach (var o in _cache)
                    if (o.Metrics == m)
                    {
                        rows.Add(o.Row);
                        break;
                    }
            return rows;
        }
        public class MetricAddedOrUpdatedEventArgs : EventArgs
        {
            public Metrics Metrics { private set; get; }
            /// <summary>
            /// False for updated.
            /// </summary>
            public bool Added { private set; get; }
            public object[] Row { private set; get; }
            /// <summary>
            /// ConcurrencyResult or RunResult.
            /// </summary>
            public object Result { private set; get; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="metrics"></param>
            /// <param name="added">False for updated.</param>
            /// <param name="row"></param>
            /// <param name="result">ConcurrencyResult or RunResult.</param>
            public MetricAddedOrUpdatedEventArgs(Metrics metrics, bool added, object[] row, object result)
            {
                Metrics = metrics;
                Added = added;
                Row = row;
                Result = result;
            }
        }
        private class MetricsCacheObject
        {
            public Metrics Metrics { get; set; }
            public object[] Row { get; set; }
            /// <summary>
            /// ConcurrencyResult or RunResult.
            /// </summary>
            public object Result { get; set; }
        }
    }
}
