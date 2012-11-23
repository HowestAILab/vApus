using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vApus.Results.Metrics.Monitor
{
    public static class MonitorMetricsHelper
    {
        public static string[] GetFormattedHeaders(string[] headers)
        {
            string avg = "Avg. ";
            List<string> l = new List<string>(headers.Length);
            foreach (string header in headers)
                if (header.Length != 0)
                    l.Add(avg + header);

            return l.ToArray();
        }
        /// <summary>
        /// Returns all monitor values.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<DateTime, float[]> GetMonitorValues(MonitorResultCache monitorResultCache)
        {
            return GetMonitorValues(DateTime.MinValue, DateTime.MaxValue, monitorResultCache);
        }
        /// <summary>
        /// Returns monitor values filtered.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, float[]> GetMonitorValues(DateTime from, DateTime to, MonitorResultCache monitorResultCache)
        {
            var monitorValues = new Dictionary<DateTime, float[]>();
            foreach (object[] row in monitorResultCache.Rows)
            {
                DateTime timestamp = (DateTime)row[0];
                if (timestamp >= from && timestamp <= to)
                {
                    float[] values = new float[row.Length - 1];
                    for (int i = 0; i != values.Length; i++)
                        values[i] = (float)row[i + 1];

                    if (!monitorValues.ContainsKey(timestamp))
                        monitorValues.Add(timestamp, values);
                }
            }
            return monitorValues;
        }
    }
}
