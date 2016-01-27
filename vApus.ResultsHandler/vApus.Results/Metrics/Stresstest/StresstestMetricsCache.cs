/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;

namespace vApus.Results {
    /// <summary>
    ///     Should be kept where the results are visualized (rows in a datagridview) and used together with StresstestMetricsHelper.
    /// </summary>
    [Serializable]
    public class StresstestMetricsCache {
        #region Fields
        private readonly List<MetricsCacheObject> _cache = new List<MetricsCacheObject>();
        #endregion

        #region Functions
        /// <summary>
        ///     Caching metrics for results that are not known (master-slave vApus setup).
        /// </summary>
        /// <param name="metrics"></param>
        public void Add(StresstestMetrics metrics) { __AddOrUpdate(metrics); }
        /// <summary>
        /// This will auto add or update metrics for the given result.
        /// </summary>
        /// <param name="result"></param>
        /// <returns>The metrics for the complete resultset.</returns>
        public List<StresstestMetrics> AddOrUpdate(ConcurrencyResult result) {
            __AddOrUpdate(StresstestMetricsHelper.GetMetrics(result, false), result);
            return GetConcurrencyMetrics();
        }
        /// <summary>
        /// This will auto add or update metrics for the given result.
        /// </summary>
        /// <param name="result"></param>
        /// <returns>The metrics for the complete resultset.</returns>
        public List<StresstestMetrics> AddOrUpdate(RunResult result) {
            __AddOrUpdate(StresstestMetricsHelper.GetMetrics(result), result);
            return GetRunMetrics();
        }
        private void __AddOrUpdate(StresstestMetrics metrics, object result = null) {
            bool add = true;
            var cacheObject = new MetricsCacheObject { Metrics = metrics, Result = result };
            if (result != null)
                for (int i = 0; i != _cache.Count; i++) {
                    MetricsCacheObject o = _cache[i];
                    if (o.Result == result) {
                        _cache[i] = cacheObject;
                        add = false;
                        break;
                    }
                }

            if (add) _cache.Add(cacheObject);
        }

        /// <summary>
        ///     This will update the metrics if possible, otherwise it will just return them.
        ///     The concurrency results are removed from cache when not needed anymre.
        /// </summary>
        /// <returns></returns>
        public List<StresstestMetrics> GetConcurrencyMetrics() {
            var removeResults = new List<MetricsCacheObject>();
            var metrics = new List<StresstestMetrics>();
            foreach (MetricsCacheObject o in _cache)
                if (o.Metrics.Run == 0) {
                    if (o.Result != null) {
                        var cr = o.Result as ConcurrencyResult;
                        o.Metrics = StresstestMetricsHelper.GetMetrics(cr, false);
                        if (cr.StoppedAt != DateTime.MinValue)
                            removeResults.Add(o);
                    }
                    metrics.Add(o.Metrics);
                }

            foreach (MetricsCacheObject removeResult in removeResults) {
                var cr = removeResult.Result as ConcurrencyResult;
                removeResult.Result = null;

                GC.Collect();
            }
            return metrics;
        }
        /// <summary>
        ///     This will update the metrics if possible, otherwise it will just return them.
        /// </summary>
        /// <returns></returns>
        public List<StresstestMetrics> GetRunMetrics() {
            var metrics = new List<StresstestMetrics>();
            foreach (MetricsCacheObject o in _cache)
                if (o.Metrics.Run != 0) {
                    if (o.Result != null && (o.Metrics.LogEntries == 0 || o.Metrics.LogEntriesProcessed != o.Metrics.LogEntries)) o.Metrics = StresstestMetricsHelper.GetMetrics(o.Result as RunResult, false);
                    metrics.Add(o.Metrics);
                }
            return metrics;
        }
        #endregion

        [Serializable]
        private class MetricsCacheObject {
            public StresstestMetrics Metrics { get; set; }
            /// <summary>
            ///     ConcurrencyResult or RunResult.
            /// </summary>
            [NonSerialized]
            public object Result;
        }
    }
}