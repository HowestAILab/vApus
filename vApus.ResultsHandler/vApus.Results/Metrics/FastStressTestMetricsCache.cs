/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Runtime;

namespace vApus.Results {
    /// <summary>
    ///     Serves at caching stress test metrics so they do not need to be calculated from the stresstest results every time if metrics are asked.
    ///     Should be kept where the results are visualized (rows in a datagridview) and used together with StressTestMetricsHelper.
    /// </summary>
    [Serializable]
    public class FastStressTestMetricsCache {

        private readonly List<MetricsCacheObject> _cache = new List<MetricsCacheObject>();

        /// <summary>
        ///     Serves at caching stresstest metrics so they do not need to be calculated from the stress test results every time if metrics are asked.
        ///     Should be kept where the results are visualized (rows in a datagridview) and used together with StressTestMetricsHelper.
        /// </summary>
        public FastStressTestMetricsCache() { }

        #region Functions
        /// <summary>
        ///     Caching metrics for results that are not known (master-slave vApus setup).
        /// </summary>
        /// <param name="metrics"></param>
        internal void Add(StressTestMetrics metrics) { __AddOrUpdate(metrics); }
        /// <summary>
        /// This will auto add or update metrics for the given result.
        /// </summary>
        /// <param name="result"></param>
        /// <returns>The metrics for the complete resultset.</returns>
        public List<StressTestMetrics> AddOrUpdate(ConcurrencyResult result, bool allowSimplified) {
            bool simplified = allowSimplified && result.StoppedAt == DateTime.MinValue;
            __AddOrUpdate(simplified ? FastStressTestMetricsHelper.GetSimplifiedMetrics(result) : FastStressTestMetricsHelper.GetMetrics(result), result);
            return GetConcurrencyMetrics(allowSimplified);
        }
        /// <summary>
        /// This will auto add or update metrics for the given result.
        /// </summary>
        /// <param name="result"></param>
        /// <returns>The metrics for the complete resultset.</returns>
        public List<StressTestMetrics> AddOrUpdate(RunResult result, bool allowSimplified) {
            bool simplified = allowSimplified && result.StoppedAt == DateTime.MinValue;
            __AddOrUpdate(simplified ? FastStressTestMetricsHelper.GetSimplifiedMetrics(result) : FastStressTestMetricsHelper.GetMetrics(result), result);
            return GetRunMetrics(allowSimplified);
        }
        private void __AddOrUpdate(StressTestMetrics metrics, object result = null) {
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
        ///     The concurrency results are removed from cache when not needed anymore.
        /// </summary>
        /// <returns></returns>
        public List<StressTestMetrics> GetConcurrencyMetrics(bool allowSimplified) {
            var removeResults = new List<MetricsCacheObject>();
            var metrics = new List<StressTestMetrics>();
            foreach (MetricsCacheObject o in _cache)
                if (o.Metrics.Run == 0) {
                    if (o.Result != null) {
                        var r = o.Result as ConcurrencyResult;
                        if (!o.Finished) { //Do not recalculate if finished. Finished results schould never be simplified.
                            o.Finished = r.StoppedAt != DateTime.MinValue;
                            o.Metrics = allowSimplified && !o.Finished ? FastStressTestMetricsHelper.GetSimplifiedMetrics(r) : FastStressTestMetricsHelper.GetMetrics(r);
                        }
                        if (o.Finished)
                            removeResults.Add(o);
                    }
                    metrics.Add(o.Metrics);
                }

            bool resultsRemoved = false;
            foreach (MetricsCacheObject removeResult in removeResults) {
                var r = removeResult.Result as ConcurrencyResult;
                removeResult.Result = null;

                resultsRemoved = true;
            }
            if (resultsRemoved) {
                GC.WaitForPendingFinalizers();
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect();
            }

            return metrics;
        }
        /// <summary>
        ///     This will update the metrics if possible, otherwise it will just return them.
        /// </summary>
        /// <returns></returns>
        public List<StressTestMetrics> GetRunMetrics(bool allowSimplified) {
            var metrics = new List<StressTestMetrics>();
            foreach (MetricsCacheObject o in _cache)
                if (o.Metrics.Run != 0) {
                    if (o.Result != null) {
                        var r = o.Result as RunResult;
                        if (!o.Finished) { //Do not recalculate if finished. Finished results schould never be simplified.
                            o.Finished = r.StoppedAt != DateTime.MinValue;
                            o.Metrics = allowSimplified && !o.Finished ? FastStressTestMetricsHelper.GetSimplifiedMetrics(r) : FastStressTestMetricsHelper.GetMetrics(r);
                        }
                    }
                    metrics.Add(o.Metrics);
                }
            return metrics;
        }

        internal List<StressTestMetrics> GetAllMetrics(bool allowSimplified) {
            var metrics = new List<StressTestMetrics>();
            foreach (MetricsCacheObject o in _cache) {
                if (o.Result != null)
                    if (o.Metrics.Run == 0) {
                        var r = o.Result as ConcurrencyResult;
                        if (!o.Finished) { //Do not recalculate if finished. Finished results schould never be simplified.
                            o.Finished = r.StoppedAt != DateTime.MinValue;
                            o.Metrics = allowSimplified && !o.Finished ? FastStressTestMetricsHelper.GetSimplifiedMetrics(r) : FastStressTestMetricsHelper.GetMetrics(r);
                        }
                    } else {
                        var r = o.Result as RunResult;
                        if (!o.Finished) { //Do not recalculate if finished. Finished results schould never be simplified.
                            o.Finished = r.StoppedAt != DateTime.MinValue;
                            o.Metrics = allowSimplified && !o.Finished ? FastStressTestMetricsHelper.GetSimplifiedMetrics(r) : FastStressTestMetricsHelper.GetMetrics(r);
                        }
                    }

                metrics.Add(o.Metrics);
            }
            return metrics;
        }
        #endregion

        [Serializable]
        private class MetricsCacheObject {
            public StressTestMetrics Metrics { get; set; }
            public bool Finished { get; set; }
            /// <summary>
            ///     ConcurrencyResult or RunResult.
            /// </summary>
            [NonSerialized]
            public object Result;
        }
    }
}