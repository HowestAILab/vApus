using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using vApus.Stresstest;

namespace vApus.REST.Convert
{
    public static class Converter
    {
        private static string _writeDir = Path.Combine(Application.StartupPath, "REST");

        public static string WriteDir
        {
            get { return _writeDir; }
        }

        private static void CreateOutputDir()
        {
            if (!Directory.Exists(_writeDir)) Directory.CreateDirectory(_writeDir);
        }
        public static void SetTestProgress(Hashtable testProgressCache, string distributedTest, string tileStresstest, int concurrentUsers, int precision, int run,
             Metrics metrics, TimeSpan estimatedRuntimeLeft, RunStateChange runStateChange, StresstestResult stresstestResult)
        {
            CreateOutputDir();

            var precisionCache = AddCache(precision, AddCache(concurrentUsers, AddCache(tileStresstest, AddCache(distributedTest, testProgressCache))));
            var testProgress = new TestProgress
            {
                StartMeasuringRuntime = metrics.StartMeasuringRuntime,
                MeasuredRunTime = metrics.MeasuredRunTime,
                EstimatedRuntimeLeft = estimatedRuntimeLeft,
                AverageTimeToLastByte = metrics.AverageTimeToLastByte,
                MaxTimeToLastByte = metrics.MaxTimeToLastByte,
                Percentile95MaxTimeToLastByte = metrics.Percentile95MaxTimeToLastByte,
                AverageDelay = metrics.AverageDelay,
                TotalLogEntries = metrics.TotalLogEntries,
                TotalLogEntriesProcessed = metrics.TotalLogEntriesProcessed,
                TotalLogEntriesProcessedPerTick = metrics.TotalLogEntriesProcessedPerTick,
                Errors = metrics.Errors,
                RunStateChange = runStateChange.ToString(),
                StresstestResult = stresstestResult.ToString()
            };

            if (precisionCache.Contains(run)) precisionCache[run] = testProgress; else precisionCache.Add(run, testProgress);
        }
        public static void WriteToFile(Hashtable cache, string fileName)
        {
            using (var sw = new StreamWriter(Path.Combine(_writeDir, fileName)))
                sw.Write(JsonConvert.SerializeObject(cache));

        }
        private static Hashtable AddCache(object key, Hashtable parent)
        {
            if (!parent.Contains(key)) parent.Add(key, new Hashtable());
            return parent[key] as Hashtable;
        }
        public struct TestProgress
        {
            public DateTime StartMeasuringRuntime;
            public TimeSpan MeasuredRunTime, EstimatedRuntimeLeft, AverageTimeToLastByte, MaxTimeToLastByte, Percentile95MaxTimeToLastByte, AverageDelay;
            public ulong TotalLogEntries;
            public ulong TotalLogEntriesProcessed;
            public double TotalLogEntriesProcessedPerTick;
            public ulong Errors;

            public string RunStateChange;
            public string StresstestResult;
        }
    }
}
