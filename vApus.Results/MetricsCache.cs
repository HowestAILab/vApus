/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;

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
            var cacheObject = new MetricsCacheObject() { Metrics = metrics, Result = result };
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
                MetricAddedOrUpdated(this, new MetricAddedOrUpdatedEventArgs(metrics, add, cacheObject.Result));
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
                        o.Metrics = MetricsHelper.GetMetrics(o.Result as ConcurrencyResult);

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
                        o.Metrics = MetricsHelper.GetMetrics(o.Result as RunResult);

                    metrics.Add(o.Metrics);
                }
            return metrics;
        }
        public class MetricAddedOrUpdatedEventArgs : EventArgs
        {
            public Metrics Metrics { private set; get; }
            /// <summary>
            /// False for updated.
            /// </summary>
            public bool Added { private set; get; }
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
            public MetricAddedOrUpdatedEventArgs(Metrics metrics, bool added, object result)
            {
                Metrics = metrics;
                Added = added;
                Result = result;
            }
        }
        private class MetricsCacheObject
        {
            public Metrics Metrics { get; set; }
            /// <summary>
            /// ConcurrencyResult or RunResult.
            /// </summary>
            public object Result { get; set; }
        }
    }
}
