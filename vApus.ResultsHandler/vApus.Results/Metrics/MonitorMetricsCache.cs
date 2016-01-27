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
    ///     Serves at caching monitor metrics so they do not need to be calculated from the monitor results every time if metrics are asked.
    ///     Should be kept where the results are visualized (rows in a datagridview) and used together with MonitorMetricsHelper.
    /// </summary>
    public class MonitorMetricsCache {

        #region Fields
        private readonly List<MetricsCacheObject> _cache = new List<MetricsCacheObject>();
        #endregion

        #region Functions
        /// <summary>
        ///     Caching metrics for results that are not known (master-slave vApus setup). Use AddOrUpdate if you do not need this.
        ///     Note: Before calling this fx you must clear the metrics. If you are unable to do that, use AddOrUpdate.
        /// </summary>
        /// <param name="metrics"></param>
        public void Add(MonitorMetrics metrics) { __AddOrUpdate(metrics); }
        /// <summary>
        /// This will auto add or update metrics for the given result.
        /// </summary>
        /// <param name="result"></param>
        /// <returns>The metrics for the complete resultset.</returns>
        public List<MonitorMetrics> AddOrUpdate(ConcurrencyResult result, MonitorResult monitorResultCache) {
            __AddOrUpdate(MonitorMetricsHelper.GetMetrics(result, monitorResultCache), monitorResultCache, result);
            return GetConcurrencyMetrics(monitorResultCache.Monitor);
        }
        /// <summary>
        /// This will auto add or update metrics for the given result.
        /// </summary>
        /// <param name="result"></param>
        /// <returns>The metrics for the complete resultset.</returns>
        public List<MonitorMetrics> AddOrUpdate(RunResult result, MonitorResult monitorResultCache) {
            __AddOrUpdate(MonitorMetricsHelper.GetMetrics(result, monitorResultCache), monitorResultCache, result);
            return GetRunMetrics(monitorResultCache.Monitor);
        }
        private void __AddOrUpdate(MonitorMetrics metrics, MonitorResult monitorResultCache = null, object result = null) {
            bool add = true; //False for update
            var cacheObject = new MetricsCacheObject { Metrics = metrics, MonitorResultCache = monitorResultCache, Result = result };
            if (result != null)
                for (int i = 0; i != _cache.Count; i++) {
                    MetricsCacheObject o = _cache[i];
                    if (o.Result == result && o.MonitorResultCache == monitorResultCache) {
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
        /// <monitorToString>The identifier of the monitor.</monitorToString>
        /// <returns></returns>
        public List<MonitorMetrics> GetConcurrencyMetrics(string monitorToString) {
            var removeResults = new List<MetricsCacheObject>();
            var metrics = new List<MonitorMetrics>();
            foreach (MetricsCacheObject o in _cache)
                if (o.Metrics.Monitor == monitorToString && o.Metrics.Run == 0) {
                    if (o.Result != null && o.MonitorResultCache != null) {
                        var cr = o.Result as ConcurrencyResult;
                        o.Metrics = MonitorMetricsHelper.GetMetrics(cr, o.MonitorResultCache);
                        if (cr.StoppedAt != DateTime.MinValue) removeResults.Add(o);
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
        /// <monitorToString>The identifier of the monitor.</monitorToString>
        /// <returns></returns>
        public List<MonitorMetrics> GetRunMetrics(string monitorToString) {
            var metrics = new List<MonitorMetrics>();
            foreach (MetricsCacheObject o in _cache)
                if (o.Metrics.Monitor == monitorToString && o.Metrics.Run != 0) {
                    if (o.Result != null && o.MonitorResultCache != null) o.Metrics = MonitorMetricsHelper.GetMetrics(o.Result as RunResult, o.MonitorResultCache);
                    metrics.Add(o.Metrics);
                }
            return metrics;
        }
        #endregion

        private class MetricsCacheObject {
            public MonitorMetrics Metrics { get; set; }
            public MonitorResult MonitorResultCache { get; set; }
            /// <summary>
            ///     ConcurrencyResult or RunResult.
            /// </summary>
            public object Result { get; set; }
        }
    }
}
