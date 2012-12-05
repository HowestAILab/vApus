/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using vApus.Util;

namespace vApus.Results
{
    public static class MonitorMetricsHelper
    {
        #region Fields
        //private static string[] _metricsHeadersConcurrency, _metricsHeadersRun;
        #endregion

        #region Functions
        public static string[] GetMetricsHeadersConcurrency(string[] monitorHeaders, bool readable)
        {
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
        public static string[] GetMetricsHeadersRun(string[] monitorHeaders, bool readable)
        {
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
        private static List<string> GetFormattedHeaders(string[] headers)
        {
            string avg = "Avg. ";
            var l = new List<string>(headers.Length);
            foreach (string header in headers)
                if (header.Length != 0) l.Add(avg + header);
            return l;
        }

        public static MonitorMetrics GetMetrics(ConcurrencyResult result, MonitorResultCache monitorResultCache)
        {
            var metrics = new MonitorMetrics();
            metrics.StartMeasuringRuntime = result.StartedAt;
            metrics.MeasuredRunTime = (result.StoppedAt == DateTime.MinValue ? DateTime.Now : result.StoppedAt) - metrics.StartMeasuringRuntime;
            metrics.ConcurrentUsers = result.ConcurrentUsers;
            metrics.Headers = monitorResultCache.Headers;

            SetAverageMonitorResults(metrics, GetMonitorValues(result.StartedAt, result.StoppedAt == DateTime.MinValue ? DateTime.MaxValue : result.StoppedAt, monitorResultCache));

            return metrics;
        }
        public static MonitorMetrics GetMetrics(RunResult result, MonitorResultCache monitorResultCache)
        {
            var metrics = new MonitorMetrics();
            metrics.StartMeasuringRuntime = result.StartedAt;
            metrics.MeasuredRunTime = (result.StoppedAt == DateTime.MinValue ? DateTime.Now : result.StoppedAt) - metrics.StartMeasuringRuntime;
            metrics.ConcurrentUsers = result.VirtualUserResults.Length;
            metrics.Run = result.Run;
            metrics.Headers = monitorResultCache.Headers;

            SetAverageMonitorResults(metrics, GetMonitorValues(result.StartedAt, result.StoppedAt == DateTime.MinValue ? DateTime.MaxValue : result.StoppedAt, monitorResultCache));

            return metrics;
        }
        /// <summary>
        ///     Returns monitor values filtered.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private static Dictionary<DateTime, float[]> GetMonitorValues(DateTime from, DateTime to, MonitorResultCache monitorResultCache)
        {
            var monitorValues = new Dictionary<DateTime, float[]>();
            foreach (var row in monitorResultCache.Rows)
            {
                var timestamp = (DateTime)row[0];
                if (timestamp >= from && timestamp <= to)
                {
                    var values = new float[row.Length - 1];
                    for (int i = 0; i != values.Length; i++)
                        values[i] = (float)row[i + 1];

                    if (!monitorValues.ContainsKey(timestamp))
                        monitorValues.Add(timestamp, values);
                }
            }
            return monitorValues;
        }
        private static void SetAverageMonitorResults(MonitorMetrics metrics, Dictionary<DateTime, float[]> values)
        {
            metrics.AverageMonitorResults = new float[0];
            if (values.Count != 0)
            {
                //Average divider
                int valueCount = values.Count;
                metrics.AverageMonitorResults = new float[valueCount];

                foreach (var key in values.Keys)
                {
                    var floats = values[key];

                    // The averages length must be the same as the floats length.
                    if (metrics.AverageMonitorResults.Length != floats.Length) metrics.AverageMonitorResults = new float[floats.Length];

                    for (int i = 0; i != floats.Length; i++)
                    {
                        float value = floats[i], average = metrics.AverageMonitorResults[i];

                        if (value == -1) //Detect invalid values.
                            metrics.AverageMonitorResults[i] = -1;
                        else if (average != -1) //Add the value to the averages at the same index (i), divide it first (no overflow).
                            metrics.AverageMonitorResults[i] = average + (value / valueCount);
                    }
                }
            }
        }

        public static List<object[]> MetricsToRows(List<MonitorMetrics> metrics, bool readable)
        {
            var rows = new List<object[]>(metrics.Count);
            foreach (var m in metrics) rows.Add(readable ? ReadableMetricsToRow(m) : CalculatableMetricsToRow(m));
            return rows;
        }
        private static object[] ReadableMetricsToRow(MonitorMetrics metrics)
        {
            var row = new List<object>(metrics.AverageMonitorResults.Length + 4);
            row.Add(metrics.StartMeasuringRuntime.ToString());
            row.Add(metrics.MeasuredRunTime.ToShortFormattedString());
            row.Add(metrics.ConcurrentUsers);

            if (metrics.Run != 0) row.Add(metrics.Run);
            foreach (float avg in metrics.AverageMonitorResults) row.Add(avg);

            return row.ToArray();
        }
        private static object[] CalculatableMetricsToRow(MonitorMetrics metrics)
        {
            var row = new List<object>(metrics.AverageMonitorResults.Length + 4);
            row.Add(metrics.StartMeasuringRuntime.ToString());
            row.Add(Math.Round(metrics.MeasuredRunTime.TotalMilliseconds, 2));
            row.Add(metrics.ConcurrentUsers);

            if (metrics.Run != 0) row.Add(metrics.Run);
            foreach (float avg in metrics.AverageMonitorResults) row.Add(avg);

            return row.ToArray();
        }
        #endregion
    }
}