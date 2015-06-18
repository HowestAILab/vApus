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

        #region Fields
        private readonly List<MetricsCacheObject> _cache = new List<MetricsCacheObject>();
        private bool _simplifiedMetrics = false;
        private bool _allowSimplifiedMetrics = true;
        #endregion

        #region Properties
        /// <summary>
        /// This value is set to false when AddOrUpdate is called. It will be changed to true if calculating the metrics takes longer than 5 seconds AND the 95th percentile of response times is not calculated.
        /// This means than only basic stuff is returned, test with a big scenario and see (100k entries for instance).
        /// </summary>
        public bool SimplifiedMetrics { get { return _simplifiedMetrics && _allowSimplifiedMetrics; } }
        /// <summary>
        /// Set to false to ignore CalculatedSimplifiedMetrics even if calculating the metrics takes longer than 5 seconds.
        /// </summary>
        public bool AllowSimplifiedMetrics {
            get { return _allowSimplifiedMetrics; }
            set { _allowSimplifiedMetrics = value; }
        }
        #endregion

        /// <summary>
        ///     Serves at caching stresstest metrics so they do not need to be calculated from the stress test results every time if metrics are asked.
        ///     Should be kept where the results are visualized (rows in a datagridview) and used together with StressTestMetricsHelper.
        ///     
        /// Default: metrics are not simplified.
        /// </summary>
        public FastStressTestMetricsCache() :this(false) { }

        /// <summary>
        ///     Serves at caching stresstest metrics so they do not need to be calculated from the stress test results every time if metrics are asked.
        ///     Should be kept where the results are visualized (rows in a datagridview) and used together with StressTestMetricsHelper.
        /// </summary>
        /// <param name="simplifiedMetrics">True for calculating simplified results. Special usecase if you want to do this right away. The code self-determines if simplification is needed (time-bound 5 sec)</param>
        public FastStressTestMetricsCache(bool simplifiedMetrics) { _simplifiedMetrics = simplifiedMetrics; }

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
        public List<StressTestMetrics> AddOrUpdate(ConcurrencyResult result) {
            __AddOrUpdate(FastStressTestMetricsHelper.GetMetrics(result, ref _simplifiedMetrics, _allowSimplifiedMetrics), result);
            return GetConcurrencyMetrics();
        }
        /// <summary>
        /// This will auto add or update metrics for the given result.
        /// </summary>
        /// <param name="result"></param>
        /// <returns>The metrics for the complete resultset.</returns>
        public List<StressTestMetrics> AddOrUpdate(RunResult result) {
            __AddOrUpdate(FastStressTestMetricsHelper.GetMetrics(result, ref _simplifiedMetrics, _allowSimplifiedMetrics), result);
            return GetRunMetrics();
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
        public List<StressTestMetrics> GetConcurrencyMetrics() {
            var removeResults = new List<MetricsCacheObject>();
            var metrics = new List<StressTestMetrics>();
            foreach (MetricsCacheObject o in _cache)
                if (o.Metrics.Run == 0) {
                    if (o.Result != null) {
                        var cr = o.Result as ConcurrencyResult;
                        o.Metrics = FastStressTestMetricsHelper.GetMetrics(cr, ref _simplifiedMetrics, _allowSimplifiedMetrics);
                        if (!_simplifiedMetrics && cr.StoppedAt != DateTime.MinValue)
                            removeResults.Add(o);
                    }
                    metrics.Add(o.Metrics);
                }

            bool resultsRemoved = false;
            foreach (MetricsCacheObject removeResult in removeResults) {
                var cr = removeResult.Result as ConcurrencyResult;
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
        public List<StressTestMetrics> GetRunMetrics() {
            var metrics = new List<StressTestMetrics>();
            foreach (MetricsCacheObject o in _cache)
                if (o.Metrics.Run != 0) {
                    if (o.Result != null)
                        if (!_allowSimplifiedMetrics || o.Metrics.Requests == 0 || o.Metrics.RequestsProcessed != o.Metrics.Requests)
                            o.Metrics = FastStressTestMetricsHelper.GetMetrics(o.Result as RunResult, ref _simplifiedMetrics, _allowSimplifiedMetrics);
                    metrics.Add(o.Metrics);
                }
            return metrics;
        }

        internal List<StressTestMetrics> GetAllMetrics() {
            var metrics = new List<StressTestMetrics>();
            foreach (MetricsCacheObject o in _cache) {
                if (o.Result != null) {
                    o.Metrics = (o.Metrics.Run == 0) ?
                        FastStressTestMetricsHelper.GetMetrics(o.Result as ConcurrencyResult, ref _simplifiedMetrics, _allowSimplifiedMetrics) :
                        FastStressTestMetricsHelper.GetMetrics(o.Result as RunResult, ref _simplifiedMetrics, _allowSimplifiedMetrics);
                }
                metrics.Add(o.Metrics);
            }
            return metrics;
        }
        #endregion

        [Serializable]
        private class MetricsCacheObject {
            public StressTestMetrics Metrics { get; set; }
            /// <summary>
            ///     ConcurrencyResult or RunResult.
            /// </summary>
            [NonSerialized]
            public object Result;
        }
    }
}