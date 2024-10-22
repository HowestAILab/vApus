﻿/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using vApus.Util;

namespace vApus.Results {
#warning OBSOLETE
    /// <summary>
    /// Gets metrics from results or cache. Those are added to the cache if not already present.
    /// </summary>
    public static class MonitorMetricsHelper {

        #region Fields
        //private static string[] _metricsHeadersConcurrency, _metricsHeadersRun;
        #endregion

        #region Functions
        public static string[] GetMetricsHeadersConcurrency(string[] monitorHeaders, bool readable) {
            //if (_metricsHeadersConcurrency == null)
            //{
            var l = new List<string>(monitorHeaders.Length + 3);
            l.Add("Started At");
            l.Add(readable ? "Measured Time" : "Measured Time (ms)");
            l.Add("Concurrency");
            l.AddRange(GetFormattedHeaders(monitorHeaders));
            return l.ToArray();
            //      _metricsHeadersConcurrency = l.ToArray();
            //}
            //return _metricsHeadersConcurrency;
        }
        public static string[] GetMetricsHeadersRun(string[] monitorHeaders, bool readable) {
            //if (_metricsHeadersRun == null)
            //{
            var l = new List<string>(monitorHeaders.Length + 4);
            l.Add("Started At");
            l.Add(readable ? "Measured Time" : "Measured Time (ms)");
            l.Add("Concurrency");
            l.Add("Run");
            l.AddRange(GetFormattedHeaders(monitorHeaders));
            return l.ToArray();
            //    _metricsHeadersRun = l.ToArray();
            //}
            //return _metricsHeadersRun;
        }
        private static List<string> GetFormattedHeaders(string[] headers) {
            string avg = "Avg. ";
            var l = new List<string>(headers.Length);
            foreach (string header in headers)
                if (header.Length != 0) l.Add(avg + header);
            return l;
        }

        public static MonitorMetrics GetMetrics(ConcurrencyResult result, MonitorResult monitorResultCache) {
            var metrics = new MonitorMetrics();
            metrics.Monitor = monitorResultCache.Monitor;
            metrics.StartMeasuringRuntime = result.StartedAt;
            metrics.MeasuredRunTime = (result.StoppedAt == DateTime.MinValue ? DateTime.Now : result.StoppedAt) - metrics.StartMeasuringRuntime;
            metrics.ConcurrentUsers = result.Concurrency;
            metrics.Headers = monitorResultCache.Headers;

            //Done this way to strip the monitor values during vApus think times between the runs.
            var monitorValues = new Dictionary<DateTime, double[]>();
            foreach (var runResult in result.RunResults) {
                var part = GetMonitorValues(runResult.StartedAt, runResult.StoppedAt == DateTime.MinValue ? DateTime.MaxValue : runResult.StoppedAt, monitorResultCache);
                foreach (var key in part.Keys) if (!monitorValues.ContainsKey(key)) monitorValues.Add(key, part[key]);
            }

            SetAverageMonitorResultsToMetrics(metrics, monitorValues);

            return metrics;
        }
        /// <summary>
        /// This is an override function, use GetMetrics if possible.
        /// </summary>
        /// <param name="monitor"></param>
        /// <param name="concurrencyMetrics"></param>
        /// <param name="monitorResultCache"></param>
        /// <returns></returns>
        public static MonitorMetrics GetConcurrencyMetrics(string monitor, StressTestMetrics concurrencyMetrics, MonitorResult monitorResultCache) {
            var metrics = new MonitorMetrics();
            metrics.Monitor = monitor;
            metrics.StartMeasuringRuntime = concurrencyMetrics.StartMeasuringTime;
            metrics.MeasuredRunTime = concurrencyMetrics.MeasuredTime;
            metrics.ConcurrentUsers = concurrencyMetrics.Concurrency;
            metrics.Headers = monitorResultCache.Headers;

            //Done this way to strip the monitor values during vApus think times between the runs.
            var monitorValues = new Dictionary<DateTime, double[]>();
            foreach (var kvp in concurrencyMetrics.StartsAndStopsRuns) {
                var part = GetMonitorValues(kvp.Key, kvp.Value == DateTime.MinValue ? DateTime.MaxValue : kvp.Value, monitorResultCache);
                foreach (var key in part.Keys) if (!monitorValues.ContainsKey(key)) monitorValues.Add(key, part[key]);
            }

            SetAverageMonitorResultsToMetrics(metrics, monitorValues);

            return metrics;
        }

        public static MonitorMetrics GetMetrics(RunResult result, MonitorResult monitorResultCache) {
            var metrics = new MonitorMetrics();
            metrics.Monitor = monitorResultCache.Monitor;
            metrics.StartMeasuringRuntime = result.StartedAt;
            metrics.MeasuredRunTime = (result.StoppedAt == DateTime.MinValue ? DateTime.Now : result.StoppedAt) - metrics.StartMeasuringRuntime;
            metrics.ConcurrentUsers = result.VirtualUserResults.Length;
            metrics.Run = result.Run;
            metrics.Headers = monitorResultCache.Headers;

            SetAverageMonitorResultsToMetrics(metrics, GetMonitorValues(result.StartedAt, result.StoppedAt == DateTime.MinValue ? DateTime.MaxValue : result.StoppedAt, monitorResultCache));

            return metrics;
        }
        /// <summary>
        /// This is an override function, use GetMetrics if possible.
        /// </summary>
        /// <param name="monitor"></param>
        /// <param name="runMetrics"></param>
        /// <param name="monitorResultCache"></param>
        /// <returns></returns>
        public static MonitorMetrics GetRunMetrics(string monitor, StressTestMetrics runMetrics, MonitorResult monitorResultCache) {
            var metrics = new MonitorMetrics();
            metrics.Monitor = monitor;
            metrics.StartMeasuringRuntime = runMetrics.StartMeasuringTime;
            metrics.MeasuredRunTime = runMetrics.MeasuredTime;
            metrics.ConcurrentUsers = runMetrics.Concurrency;
            metrics.Run = runMetrics.Run;
            metrics.Headers = monitorResultCache.Headers;

            var stoppedAt = runMetrics.StartMeasuringTime + runMetrics.MeasuredTime;
            SetAverageMonitorResultsToMetrics(metrics, GetMonitorValues(runMetrics.StartMeasuringTime, stoppedAt == runMetrics.StartMeasuringTime ? DateTime.MaxValue : stoppedAt, monitorResultCache));

            return metrics;
        }
        /// <summary>
        ///     Returns monitor values filtered.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private static Dictionary<DateTime, double[]> GetMonitorValues(DateTime from, DateTime to, MonitorResult monitorResultCache) {
            var monitorValues = new Dictionary<DateTime, double[]>();
            foreach (var row in monitorResultCache.Rows) {
                var timestamp = (DateTime)row[0];
                if (timestamp >= from && timestamp <= to) {
                    var values = new double[row.Length - 1];
                    for (int i = 0; i != values.Length; i++) {
                        object cellValue = row[i + 1];
                        values[i] = cellValue is double ? (double)cellValue : -1;
                    }

                    if (!monitorValues.ContainsKey(timestamp))
                        monitorValues.Add(timestamp, values);
                }
            }
            return monitorValues;
        }
        private static void SetAverageMonitorResultsToMetrics(MonitorMetrics metrics, Dictionary<DateTime, double[]> monitorValues) {
            metrics.AverageMonitorResults = new double[0];
            if (monitorValues.Count != 0) {
                //Average divider
                int valueCount = monitorValues.Count;
                metrics.AverageMonitorResults = new double[valueCount];

                foreach (var key in monitorValues.Keys) {
                    var doubles = monitorValues[key];

                    // The averages length must be the same as the doubles length.
                    if (metrics.AverageMonitorResults.Length != doubles.Length) metrics.AverageMonitorResults = new double[doubles.Length];

                    for (int i = 0; i != doubles.Length; i++) {
                        double value = doubles[i], average = metrics.AverageMonitorResults[i];

                        if (value == -1) //Detect invalid values.
                            metrics.AverageMonitorResults[i] = -1;
                        else if (average != -1) //Add the value to the averages at the same index (i), divide it first (no overflow).
                            metrics.AverageMonitorResults[i] = average + (value / valueCount);
                    }
                }
            }
        }

        public static List<object[]> MetricsToRows(List<MonitorMetrics> metrics, bool readable) {
            var rows = new List<object[]>(metrics.Count);
            foreach (var m in metrics) rows.Add(readable ? ReadableMetricsToRow(m) : CalculatableMetricsToRow(m));
            return rows;
        }
        private static object[] ReadableMetricsToRow(MonitorMetrics metrics) {
            var row = new List<object>(metrics.AverageMonitorResults.Length + 4);
            row.Add(metrics.StartMeasuringRuntime.ToString());
            row.Add(metrics.MeasuredRunTime.ToShortFormattedString(true));
            row.Add(metrics.ConcurrentUsers);

            if (metrics.Run != 0) row.Add(metrics.Run);
            foreach (double avg in metrics.AverageMonitorResults) row.Add(avg);

            return row.ToArray();
        }
        private static object[] CalculatableMetricsToRow(MonitorMetrics metrics) {
            var row = new List<object>(metrics.AverageMonitorResults.Length + 4);
            row.Add(metrics.StartMeasuringRuntime.ToString());
            row.Add(Math.Round(metrics.MeasuredRunTime.TotalMilliseconds, MidpointRounding.AwayFromZero));
            row.Add(metrics.ConcurrentUsers);

            if (metrics.Run != 0) row.Add(metrics.Run);
            foreach (double avg in metrics.AverageMonitorResults) row.Add(avg);

            return row.ToArray();
        }
        #endregion
    }
}