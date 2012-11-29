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
    ///     Should be kept where the results are visualized (rows in a datagridview) and used together with MetricsHelper.
    /// </summary>
    public class StresstestMetricsCache
    {
        private readonly List<MetricsCacheObject> _cache = new List<MetricsCacheObject>();
        public EventHandler<MetricAddedOrUpdatedEventArgs> MetricAddedOrUpdated;

        public StresstestResult StresstestResult { get; set; }

        /// <summary>
        ///     Caching metrics for results that are not known (master-slave vApus setup).
        /// </summary>
        /// <param name="metrics"></param>
        public void Add(StresstestMetrics metrics)
        {
            __AddOrUpdate(metrics);
        }

        /// <summary>
        /// </summary>
        /// <param name="metrics">eg MetricsHelper.GetMetrics(result)</param>
        /// <param name="result">eg result</param>
        public void AddOrUpdate(StresstestMetrics metrics, ConcurrencyResult result)
        {
            __AddOrUpdate(metrics, result);
        }

        /// <summary>
        /// </summary>
        /// <param name="metrics">eg MetricsHelper.GetMetrics(result)</param>
        /// <param name="result">eg result</param>
        public void AddOrUpdate(StresstestMetrics metrics, RunResult result)
        {
            __AddOrUpdate(metrics, result);
        }

        private void __AddOrUpdate(StresstestMetrics metrics, object result = null)
        {
            bool add = true;
            var cacheObject = new MetricsCacheObject {Metrics = metrics, Result = result};
            if (result != null)
                for (int i = 0; i != _cache.Count; i++)
                {
                    MetricsCacheObject o = _cache[i];
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
        ///     This will update the metrics if possible, otherwise it will just return them.
        ///     It will also remove the concurrency results from the stresstest result if they are not needed anymore (stopped).
        ///     This is if you did set the StresstestResult property. The concurrency results are removed from cache anyways.
        /// </summary>
        /// <returns></returns>
        public List<StresstestMetrics> GetConcurrencyMetrics()
        {
            var removeResults = new List<MetricsCacheObject>();
            var metrics = new List<StresstestMetrics>();
            foreach (MetricsCacheObject o in _cache)
                if (o.Metrics.Run == 0)
                {
                    if (o.Result != null)
                    {
                        var cr = o.Result as ConcurrencyResult;
                        o.Metrics = StresstestMetricsHelper.GetMetrics(cr);
                        if (cr.StoppedAt != DateTime.MinValue)
                            removeResults.Add(o);
                    }
                    metrics.Add(o.Metrics);
                }

            foreach (MetricsCacheObject removeResult in removeResults)
            {
                var cr = removeResult.Result as ConcurrencyResult;
                if (StresstestResult != null && StresstestResult.ConcurrencyResults.Contains(cr))
                    StresstestResult.ConcurrencyResults.Remove(cr);
                removeResult.Result = null;

                GC.Collect();
            }
            return metrics;
        }

        /// <summary>
        ///     This will update the metrics if possible, otherwise it will just return them.
        /// </summary>
        /// <returns></returns>
        public List<StresstestMetrics> GetRunMetrics()
        {
            var metrics = new List<StresstestMetrics>();
            foreach (MetricsCacheObject o in _cache)
                if (o.Metrics.Run != 0)
                {
                    if (o.Result != null)
                        o.Metrics = StresstestMetricsHelper.GetMetrics(o.Result as RunResult);

                    metrics.Add(o.Metrics);
                }
            return metrics;
        }

        public class MetricAddedOrUpdatedEventArgs : EventArgs
        {
            /// <summary>
            /// </summary>
            /// <param name="metrics"></param>
            /// <param name="added">False for updated.</param>
            /// <param name="row"></param>
            /// <param name="result">ConcurrencyResult or RunResult.</param>
            public MetricAddedOrUpdatedEventArgs(StresstestMetrics metrics, bool added, object result)
            {
                Metrics = metrics;
                Added = added;
                Result = result;
            }

            public StresstestMetrics Metrics { private set; get; }

            /// <summary>
            ///     False for updated.
            /// </summary>
            public bool Added { private set; get; }

            /// <summary>
            ///     ConcurrencyResult or RunResult.
            /// </summary>
            public object Result { private set; get; }
        }

        private class MetricsCacheObject
        {
            public StresstestMetrics Metrics { get; set; }

            /// <summary>
            ///     ConcurrencyResult or RunResult.
            /// </summary>
            public object Result { get; set; }
        }
    }
}