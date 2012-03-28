/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using vApus.Util;

namespace vApus.Stresstest
{
    public interface IResult
    {
        #region Properties
        TimeSpan EstimatedRuntimeLeft { get; }
        Metrics Metrics { get; set; }
        #endregion

        #region Functions
        void RefreshLogEntryResultMetrics();
        #endregion
    }
    [Serializable]
    public class StresstestResults : IResult, ISerializable
    {
        #region Fields
        private object _lock = new object();
        private Stopwatch _sw;
        public string Solution;
        /// <summary>
        /// Please use the constructor.
        /// </summary>
        public string Stresstest, Log, LogRuleSet, Connection, ConnectionProxy, ConnectionString, Monitors;
        public int[] ConcurrentUsers;
        public int Precision, DynamicRunMultiplier, MinimumDelay, MaximumDelay;
        public bool Shuffle;
        public int ProgressUpdateDelay;
        public ActionAndLogEntryDistribution Distribute;
        public bool BatchResultSaving;
        private char _resultsDelimiter = '\0';
        private Metrics _metrics = new Metrics();

        public List<ConcurrentUsersResult> ConcurrentUsersResults = new List<ConcurrentUsersResult>();
        #endregion

        #region Properties
        public TimeSpan EstimatedRuntimeLeft
        {
            get
            {
                long estimatedRuntimeLeft = (long)(((DateTime.Now - _metrics.StartMeasuringRuntime).TotalMilliseconds / _metrics.TotalLogEntriesProcessed) * (_metrics.TotalLogEntries - _metrics.TotalLogEntriesProcessed) * 10000);
                if (estimatedRuntimeLeft < 0)
                    estimatedRuntimeLeft = 0;
                return new TimeSpan(estimatedRuntimeLeft);
            }
        }
        public Metrics Metrics
        {
            get { return _metrics; }
            //Manual override
            set { _metrics = value; }
        }
        /// <summary>
        /// Return if the current run is re-running or not (break on last run sync)
        /// </summary>
        public bool CurrentRunDoneOnce
        {
            get
            {
                if (ConcurrentUsersResults.Count != 0)
                {
                    var c = ConcurrentUsersResults[ConcurrentUsersResults.Count - 1];
                    if (c.PrecisionResults.Count != 0)
                    {
                        var p = c.PrecisionResults[c.PrecisionResults.Count - 1];
                        if (p.RunResults.Count != 0)
                        {
                            var r = p.RunResults[p.RunResults.Count - 1];
                            return r.RunDoneOnce;
                        }
                    }
                }
                return false;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This will start measuring time. Call StopTimeMeasurement() when the stresstest finished.
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="stresstest"></param>
        /// <param name="totalLogEntries"></param>
        /// <param name="startOfStresstest"></param>
        public StresstestResults(string solution, Stresstest stresstest, ulong totalLogEntries, DateTime startOfStresstest)
        {
            Solution = solution;
            Stresstest = stresstest.ToString();

            Connection = stresstest.Connection.ToString();
            ConnectionProxy = stresstest.ConnectionProxy;
            ConnectionString = stresstest.Connection.ConnectionString;
            Log = stresstest.Log.ToString();
            LogRuleSet = stresstest.LogRuleSet;

            if (stresstest.Monitors == null || stresstest.Monitors.Length == 0)
                Monitors = "No Monitor";
            else
                Monitors = stresstest.Monitors.Combine(", ");

            Distribute = stresstest.Distribute;
            ConcurrentUsers = stresstest.ConcurrentUsers;
            DynamicRunMultiplier = stresstest.DynamicRunMultiplier;
            MinimumDelay = stresstest.MinimumDelay;
            MaximumDelay = stresstest.MaximumDelay;
            Precision = stresstest.Precision;
            Shuffle = stresstest.Shuffle;
            ProgressUpdateDelay = vApus.Stresstest.Stresstest.ProgressUpdateDelay;
            BatchResultSaving = false;

            _metrics.TotalLogEntries = totalLogEntries;
            _metrics.StartMeasuringRuntime = startOfStresstest;
            _sw = Stopwatch.StartNew();
        }
        public StresstestResults(SerializationInfo info, StreamingContext ctxt)
        {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info))
            {
                Solution = sr.ReadString();
                Stresstest = sr.ReadString();
                Log = sr.ReadString();
                LogRuleSet = sr.ReadString();
                Connection = sr.ReadString();
                ConnectionProxy = sr.ReadString();
                ConnectionString = sr.ReadString();
                Monitors = sr.ReadString();
                ConcurrentUsers = sr.ReadArray(typeof(int)) as int[];
                Precision = sr.ReadInt32();
                DynamicRunMultiplier = sr.ReadInt32();
                MinimumDelay = sr.ReadInt32();
                MaximumDelay = sr.ReadInt32();
                Shuffle = sr.ReadBoolean();
                ProgressUpdateDelay = sr.ReadInt32();
                Distribute = (ActionAndLogEntryDistribution)sr.ReadInt32();
                BatchResultSaving = sr.ReadBoolean();
                _resultsDelimiter = sr.ReadChar();
                _metrics = (Metrics)sr.ReadObject();
                ConcurrentUsersResults = sr.ReadCollection<ConcurrentUsersResult>(ConcurrentUsersResults) as List<ConcurrentUsersResult>;
            }
            sr = null;
            //Not pretty, but helps against mem saturation.
            GC.Collect();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Used when redoing the same run for for instance break on last run synchronization.
        /// </summary>
        /// <param name="logEntries"></param>
        public void SetCurrentRunDoneOnce()
        {
            var c = ConcurrentUsersResults[ConcurrentUsersResults.Count - 1];
            var p = c.PrecisionResults[c.PrecisionResults.Count - 1];
            var r = p.RunResults[p.RunResults.Count - 1];

            r.SetRunDoneOnce();

            _metrics.TotalLogEntries += r.BaseLogEntryCount;
        }
        /// <summary>
        /// 
        /// </summary>
        public void StopTimeMeasurement()
        {
            _sw.Stop();
            foreach (ConcurrentUsersResult result in ConcurrentUsersResults)
                result.StopTimeMeasurement();
        }
        /// <summary>
        /// Thread safe
        /// </summary>
        public void RefreshLogEntryResultMetrics()
        {
            lock (_lock)
            {
                _metrics.MeasuredRunTime = _sw.Elapsed;
                if (ConcurrentUsersResults.Count == 0)
                    return;
                _metrics.AverageTimeToLastByte = new TimeSpan();
                _metrics.MaxTimeToLastByte = new TimeSpan();
                _metrics.AverageDelay = new TimeSpan();
                _metrics.TotalLogEntriesProcessed = 0;
                _metrics.TotalLogEntriesProcessedPerTick = 0;
                _metrics.Errors = 0;

                foreach (ConcurrentUsersResult result in ConcurrentUsersResults)
                {
                    result.RefreshLogEntryResultMetrics();
                    Metrics resultMetrics = result.Metrics;

                    _metrics.AverageTimeToLastByte = _metrics.AverageTimeToLastByte.Add(resultMetrics.AverageTimeToLastByte);
                    if (resultMetrics.MaxTimeToLastByte > _metrics.MaxTimeToLastByte)
                        _metrics.MaxTimeToLastByte = resultMetrics.MaxTimeToLastByte;
                    _metrics.AverageDelay = _metrics.AverageDelay.Add(resultMetrics.AverageDelay);
                    _metrics.TotalLogEntriesProcessed += resultMetrics.TotalLogEntriesProcessed;
                    _metrics.TotalLogEntriesProcessedPerTick += resultMetrics.TotalLogEntriesProcessedPerTick;
                    _metrics.Errors += resultMetrics.Errors;
                }
                _metrics.TotalLogEntriesProcessedPerTick /= ConcurrentUsersResults.Count;
                _metrics.AverageTimeToLastByte = new TimeSpan(_metrics.AverageTimeToLastByte.Ticks / ConcurrentUsersResults.Count);
                _metrics.AverageDelay = new TimeSpan(_metrics.AverageDelay.Ticks / ConcurrentUsersResults.Count);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public char UniqueResultsDelimiter()
        {
            if (_resultsDelimiter == '\0')
            {
                char[] candidates = new char[] { '\t', ';', ':', '#', '$', '*', '/', '\\', '+', '-', '%', '<', '|', '>', '(', ')', '\'', '\"', '&', '°', '¨', '?', '!', '§', '~', '²', '³' };
                //Choose the default, just in case.
                _resultsDelimiter = candidates[0];

                bool unique;
                foreach (char candidate in candidates)
                {
                    //unique until proven otherwise.
                    unique = true;
                    foreach (var cur in ConcurrentUsersResults)
                    {
                        foreach (var pr in cur.PrecisionResults)
                        {
                            foreach (var rr in pr.RunResults)
                            {
                                foreach (var ur in rr.UserResults)
                                {
                                    foreach (var uar in ur.UserActionResults.Values)
                                        if (uar.UserAction.Contains(candidate))
                                        {
                                            unique = false;
                                            break;
                                        }
                                    foreach (var ler in ur.LogEntryResults)
                                        if (!ler.Empty && ler.LogEntryString.Contains(candidate))
                                        {
                                            unique = false;
                                            break;
                                        }
                                    break;
                                }
                                break;
                            }
                            break;
                        }
                        break;
                    }
                    if (unique)
                    {
                        _resultsDelimiter = candidate;
                        break;
                    }
                }
            }
            return _resultsDelimiter;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter())
            {
                sw.Write(Solution);
                sw.Write(Stresstest);
                sw.Write(Log);
                sw.Write(LogRuleSet);
                sw.Write(Connection);
                sw.Write(ConnectionProxy);
                sw.Write(ConnectionString);
                sw.Write(Monitors);
                sw.Write(ConcurrentUsers);
                sw.Write(Precision);
                sw.Write(DynamicRunMultiplier);
                sw.Write(MinimumDelay);
                sw.Write(MaximumDelay);
                sw.Write(Shuffle);
                sw.Write(ProgressUpdateDelay);
                sw.Write((int)Distribute);
                sw.Write(BatchResultSaving);
                sw.Write(_resultsDelimiter);
                sw.WriteObject(_metrics);
                sw.Write<ConcurrentUsersResult>(ConcurrentUsersResults);
                sw.AddToInfo(info);
            }
            sw = null;
            //Not pretty, but helps against mem saturation.
            GC.Collect();
        }

        #endregion
    }
    [Serializable]
    public class ConcurrentUsersResult : IResult, ISerializable
    {
        #region Fields
        private Stopwatch _sw;
        private int _concurrentUsers;

        internal Metrics _metrics = new Metrics();
        public List<PrecisionResult> PrecisionResults = new List<PrecisionResult>();
        #endregion

        #region Properties
        public int ConcurrentUsers
        {
            get { return _concurrentUsers; }
        }
        public TimeSpan EstimatedRuntimeLeft
        {
            get
            {
                long estimatedRuntimeLeft = (long)(((DateTime.Now - _metrics.StartMeasuringRuntime).TotalMilliseconds / _metrics.TotalLogEntriesProcessed) * (_metrics.TotalLogEntries - _metrics.TotalLogEntriesProcessed) * 10000);
                if (estimatedRuntimeLeft < 0)
                    estimatedRuntimeLeft = 0;
                return new TimeSpan(estimatedRuntimeLeft);
            }
        }
        public Metrics Metrics
        {
            get { return _metrics; }
            //Manual override
            set { _metrics = value; }
        }
        private ulong LogEntryResultsCount
        {
            get
            {
                ulong count = 0;
                foreach (PrecisionResult pr in PrecisionResults)
                    foreach (RunResult rr in pr.RunResults)
                        foreach (UserResult ur in rr.UserResults)
                            count += ur.LogEntriesProcessed;
                return count;
            }
        }
        private int UserActionResultsCount
        {
            get
            {
                int count = 0;
                foreach (PrecisionResult pr in PrecisionResults)
                    foreach (RunResult rr in pr.RunResults)
                        foreach (UserResult ur in rr.UserResults)
                            count += ur.UserActionResults.Count;
                return count;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This will start measuring time. Call StopTimeMeasurement() when the concurrent users finished.
        /// </summary>
        /// <param name="concurrentUsers"></param>
        /// <param name="totalLogEntries"></param>
        /// <param name="startOfConcurrentUsers"></param>
        public ConcurrentUsersResult(int concurrentUsers, ulong totalLogEntries, DateTime startOfConcurrentUsers)
        {
            _concurrentUsers = concurrentUsers;
            _metrics.TotalLogEntries = totalLogEntries;
            _metrics.StartMeasuringRuntime = startOfConcurrentUsers;
            _sw = Stopwatch.StartNew();
        }
        public ConcurrentUsersResult(SerializationInfo info, StreamingContext ctxt)
        {
            SerializationReader sr = SerializationReader.GetReader(info);
            _concurrentUsers = sr.ReadInt32();
            _metrics = (Metrics)sr.ReadObject();
            PrecisionResults = sr.ReadCollection<PrecisionResult>(PrecisionResults) as List<PrecisionResult>;
        }
        #endregion

        #region Functions
        public void RefreshLogEntryResultMetrics()
        {
            _metrics.MeasuredRunTime = _sw.Elapsed;
            if (PrecisionResults.Count == 0)
                return;
            _metrics.AverageTimeToLastByte = new TimeSpan();
            _metrics.MaxTimeToLastByte = new TimeSpan();
            //_metrics.Percentile90MaxTimeToLastByte = new TimeSpan();
            _metrics.AverageDelay = new TimeSpan();
            _metrics.TotalLogEntriesProcessed = 0;
            _metrics.TotalLogEntriesProcessedPerTick = 0;
            _metrics.Errors = 0;

            foreach (PrecisionResult result in PrecisionResults)
            {
                result.RefreshLogEntryResultMetrics();
                Metrics resultMetrics = result.Metrics;

                _metrics.AverageTimeToLastByte = _metrics.AverageTimeToLastByte.Add(resultMetrics.AverageTimeToLastByte);
                if (resultMetrics.MaxTimeToLastByte > _metrics.MaxTimeToLastByte)
                    _metrics.MaxTimeToLastByte = resultMetrics.MaxTimeToLastByte;
                _metrics.AverageDelay = _metrics.AverageDelay.Add(resultMetrics.AverageDelay);
                _metrics.TotalLogEntriesProcessed += resultMetrics.TotalLogEntriesProcessed;
                _metrics.TotalLogEntriesProcessedPerTick += resultMetrics.TotalLogEntriesProcessedPerTick;
                _metrics.Errors += resultMetrics.Errors;
            }
            _metrics.TotalLogEntriesProcessedPerTick /= PrecisionResults.Count;
            _metrics.AverageTimeToLastByte = new TimeSpan(_metrics.AverageTimeToLastByte.Ticks / PrecisionResults.Count);
            //_metrics.Percentile90MaxTimeToLastByte = new TimeSpan((long)((double)_metrics.MaxTimeToLastByte.Ticks * 0.9));
            _metrics.AverageDelay = new TimeSpan(_metrics.AverageDelay.Ticks / PrecisionResults.Count);
        }
        public void StopTimeMeasurement()
        {
            _sw.Stop();
            foreach (PrecisionResult result in PrecisionResults)
                result.StopTimeMeasurement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lowestLevel">Only if it is the lowest visible level it will have the 5 last before last entries for avoiding confusion.</param>
        /// <returns></returns>
        public object[] DetailedLogEntryResultMetrics(bool lowestLevel)
        {
            RefreshPercentile90MaxTimeToLastByteForLogEntryResults();

            if (lowestLevel)
                return new object[] {_metrics.StartMeasuringRuntime
                      ,_metrics.MeasuredRunTime.ToShortFormattedString()
                      , ConcurrentUsers
                      , string.Empty //Precision
                      , string.Empty //Run
                      , string.Empty //User
                      , string.Empty //User Action
                      , string.Empty //Log Entry
                      , _metrics.TotalLogEntriesProcessed + " / " + _metrics.TotalLogEntries
                      , (_metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond)
                      , _metrics.AverageTimeToLastByte.TotalMilliseconds
                      , _metrics.MaxTimeToLastByte.TotalMilliseconds
                      , _metrics.Percentile95MaxTimeToLastByte.TotalMilliseconds
                      , _metrics.AverageDelay.TotalMilliseconds
                      , _metrics.Errors
                    };

            return new object[] {_metrics.StartMeasuringRuntime
                      ,_metrics.MeasuredRunTime.ToShortFormattedString()
                      , ConcurrentUsers
                      , string.Empty //Precision
                      , string.Empty //Run
                      , string.Empty //User
                      , string.Empty //User Action
                      , string.Empty //Log Entry
                      , _metrics.TotalLogEntriesProcessed + " / " + _metrics.TotalLogEntries
                      , (_metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond)
                      , string.Empty
                      , string.Empty
                      , string.Empty
                      , string.Empty
                      , _metrics.Errors
                    };
        }
        private void RefreshPercentile90MaxTimeToLastByteForLogEntryResults()
        {
            int percent5 = (int)(LogEntryResultsCount * 0.05);
            if (percent5 == 0)
            {
                if (_metrics.MaxTimeToLastByte == TimeSpan.MinValue)
                    RefreshLogEntryResultMetrics();
                _metrics.Percentile95MaxTimeToLastByte = _metrics.MaxTimeToLastByte;
            }
            else
            {
                List<TimeSpan> sorted = new List<TimeSpan>((int)LogEntryResultsCount);
                foreach (PrecisionResult pr in PrecisionResults)
                    foreach (RunResult rr in pr.RunResults)
                        foreach (UserResult ur in rr.UserResults)
                            foreach (var result in ur.LogEntryResults)
                                if (!result.Empty)
                                    sorted.Add(result.TimeToLastByte);

                sorted.Sort();

                _metrics.Percentile95MaxTimeToLastByte = sorted[sorted.Count - percent5 - 1];
            }
        }
        public Dictionary<LogEntryResult, Metrics> GetPivotedLogEntryResults(string userAction)
        {
            var combinedResults = new Dictionary<LogEntryResult, Metrics>();
            var pivotedPercentiles = GetPivotedPercentile95MaxTimeToLastByteForLogEntryResults(userAction);

            int precisionCount = PrecisionResults.Count;

            foreach (PrecisionResult precisionResult in PrecisionResults)
            {
                var combinedRunResults = precisionResult.GetPivotedLogEntryResults(userAction, false);
                foreach (LogEntryResult logEntryResult in combinedRunResults.Keys)
                    if (string.Equals(logEntryResult.UserAction, userAction, StringComparison.Ordinal))
                    {
                        bool found = false;
                        Metrics runMetrics, newMetrics;

                        if (precisionCount != 1)
                            foreach (var result in combinedResults.Keys)
                                if (string.Equals(result.LogEntryIndex, logEntryResult.LogEntryIndex, StringComparison.Ordinal))
                                {
                                    runMetrics = combinedRunResults[logEntryResult];
                                    newMetrics = combinedResults[result];

                                    newMetrics.AverageTimeToLastByte = newMetrics.AverageTimeToLastByte.Add(new TimeSpan(runMetrics.AverageTimeToLastByte.Ticks / PrecisionResults.Count));

                                    if (runMetrics.MaxTimeToLastByte > newMetrics.MaxTimeToLastByte)
                                        newMetrics.MaxTimeToLastByte = runMetrics.MaxTimeToLastByte;

                                    newMetrics.AverageDelay = newMetrics.AverageDelay.Add(new TimeSpan(runMetrics.AverageDelay.Ticks / PrecisionResults.Count));
                                    newMetrics.TotalLogEntries += runMetrics.TotalLogEntries;
                                    newMetrics.TotalLogEntriesProcessed += runMetrics.TotalLogEntriesProcessed;
                                    newMetrics.Errors += runMetrics.Errors;
                                    combinedResults[result] = newMetrics;
                                    found = true;
                                    break;
                                }

                        if (!found)
                        {
                            runMetrics = combinedRunResults[logEntryResult];
                            newMetrics = new Metrics();
                            newMetrics.AverageTimeToLastByte = new TimeSpan(runMetrics.AverageTimeToLastByte.Ticks / PrecisionResults.Count);
                            newMetrics.MaxTimeToLastByte = runMetrics.MaxTimeToLastByte;
                            newMetrics.AverageDelay = new TimeSpan(runMetrics.AverageDelay.Ticks / PrecisionResults.Count);
                            newMetrics.TotalLogEntries = runMetrics.TotalLogEntries;
                            newMetrics.TotalLogEntriesProcessed = runMetrics.TotalLogEntriesProcessed;
                            newMetrics.Errors = runMetrics.Errors;

                            foreach (var ler in pivotedPercentiles.Keys)
                                if (string.Equals(ler.LogEntryIndex, logEntryResult.LogEntryIndex, StringComparison.Ordinal))
                                {
                                    newMetrics.Percentile95MaxTimeToLastByte = pivotedPercentiles[ler];
                                    break;
                                }

                            combinedResults.Add(logEntryResult, newMetrics);
                        }
                    }
            }
            return combinedResults;
        }
        private Dictionary<LogEntryResult, TimeSpan> GetPivotedPercentile95MaxTimeToLastByteForLogEntryResults(string userAction)
        {
            var tempTimeToLastByte = new Dictionary<LogEntryResult, List<TimeSpan>>();
            foreach (var pr in PrecisionResults)
                foreach (var rr in pr.RunResults)
                    foreach (var ur in rr.UserResults)
                        foreach (var ler in ur.LogEntryResults)
                            if (!ler.Empty && string.Equals(ler.UserAction, userAction, StringComparison.Ordinal))
                            {
                                bool found = false;
                                foreach (var result in tempTimeToLastByte.Keys)
                                    if (string.Equals(result.LogEntryIndex, ler.LogEntryIndex, StringComparison.Ordinal))
                                    {
                                        tempTimeToLastByte[result].Add(ler.TimeToLastByte);
                                        found = true;
                                        break;
                                    }
                                if (!found)
                                {
                                    var l = new List<TimeSpan>();
                                    l.Add(ler.TimeToLastByte);
                                    tempTimeToLastByte.Add(ler, l);
                                }
                            }

            var pivoted = new Dictionary<LogEntryResult, TimeSpan>(tempTimeToLastByte.Count);
            foreach (var key in tempTimeToLastByte.Keys)
                pivoted.Add(key, GetPercentile95MaxTimeToLastByte(tempTimeToLastByte[key]));

            return pivoted;
        }

        public Dictionary<UserActionResult, Metrics> GetPivotedUserActionResults()
        {
            var combinedResults = new Dictionary<UserActionResult, Metrics>();
            var pivotedPercentiles = GetPivotedPercentile95MaxTimeToLastByteForUserActionResults();

            foreach (PrecisionResult precisionResult in PrecisionResults)
            {
                var combinedRunResults = precisionResult.GetPivotedUserActionResults();
                foreach (UserActionResult userActionResult in combinedRunResults.Keys)
                {
                    bool found = false;
                    Metrics precisionMetrics, newMetrics;
                    foreach (var result in combinedResults.Keys)
                    {
                        if (string.Equals(result.UserAction, userActionResult.UserAction, StringComparison.Ordinal))
                        {
                            precisionMetrics = combinedRunResults[userActionResult];
                            newMetrics = combinedResults[result];

                            newMetrics.AverageTimeToLastByte = newMetrics.AverageTimeToLastByte.Add(new TimeSpan(precisionMetrics.AverageTimeToLastByte.Ticks / PrecisionResults.Count));

                            if (precisionMetrics.MaxTimeToLastByte > newMetrics.MaxTimeToLastByte)
                                newMetrics.MaxTimeToLastByte = precisionMetrics.MaxTimeToLastByte;

                            newMetrics.AverageDelay = newMetrics.AverageDelay.Add(new TimeSpan(precisionMetrics.AverageDelay.Ticks / PrecisionResults.Count));
                            newMetrics.TotalLogEntries += precisionMetrics.TotalLogEntries;
                            newMetrics.TotalLogEntriesProcessed += precisionMetrics.TotalLogEntriesProcessed;
                            newMetrics.Errors += precisionMetrics.Errors;
                            combinedResults[result] = newMetrics;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        precisionMetrics = combinedRunResults[userActionResult];
                        newMetrics = new Metrics();
                        newMetrics.AverageTimeToLastByte = new TimeSpan(precisionMetrics.AverageTimeToLastByte.Ticks / PrecisionResults.Count);
                        newMetrics.MaxTimeToLastByte = precisionMetrics.MaxTimeToLastByte;
                        newMetrics.AverageDelay = new TimeSpan(precisionMetrics.AverageDelay.Ticks / PrecisionResults.Count);
                        newMetrics.TotalLogEntries = precisionMetrics.TotalLogEntries;
                        newMetrics.TotalLogEntriesProcessed = precisionMetrics.TotalLogEntriesProcessed;
                        newMetrics.Errors = precisionMetrics.Errors;

                        foreach (var uar in pivotedPercentiles.Keys)
                            if (string.Equals(uar.UserAction, userActionResult.UserAction, StringComparison.Ordinal))
                            {
                                newMetrics.Percentile95MaxTimeToLastByte = pivotedPercentiles[uar];
                                break;
                            }

                        combinedResults.Add(userActionResult, newMetrics);
                    }
                }
            }
            return combinedResults;
        }
        private Dictionary<UserActionResult, TimeSpan> GetPivotedPercentile95MaxTimeToLastByteForUserActionResults()
        {
            var tempTimeToLastByte = new Dictionary<UserActionResult, List<TimeSpan>>();

            foreach (var pr in PrecisionResults)
                foreach (var rr in pr.RunResults)
                    foreach (var ur in rr.UserResults)
                        foreach (var uar in ur.UserActionResults)
                        {
                            bool found = false;
                            uar.Value.RefreshMetrics();

                            foreach (var result in tempTimeToLastByte.Keys)
                                if (result.UserActionIndex == uar.Key)
                                {
                                    tempTimeToLastByte[result].Add(uar.Value.TimeToLastByte);
                                    found = true;
                                    break;
                                }
                            if (!found)
                            {
                                var l = new List<TimeSpan>();
                                l.Add(uar.Value.TimeToLastByte);
                                tempTimeToLastByte.Add(uar.Value, l);
                            }
                        }

            var pivoted = new Dictionary<UserActionResult, TimeSpan>(tempTimeToLastByte.Count);
            foreach (var key in tempTimeToLastByte.Keys)
                pivoted.Add(key, GetPercentile95MaxTimeToLastByte(tempTimeToLastByte[key]));

            return pivoted;
        }

        private TimeSpan GetPercentile95MaxTimeToLastByte(List<TimeSpan> timeToLastBytes)
        {
            int timeToLastBytesCount = timeToLastBytes.Count;
            int percent5 = (int)(timeToLastBytesCount * 0.05);

            timeToLastBytes.Sort();

            return timeToLastBytes[timeToLastBytesCount - percent5 - 1];
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter())
            {
                sw.Write(_concurrentUsers);
                sw.WriteObject(_metrics);
                sw.Write<PrecisionResult>(PrecisionResults);
                sw.AddToInfo(info);
            }
            sw = null;
            //Not pretty, but helps against mem saturation.
            GC.Collect();
        }
        #endregion
    }
    [Serializable]
    public class PrecisionResult : IResult, ISerializable
    {
        #region Fields
        private Stopwatch _sw;
        private int _precision;

        internal Metrics _metrics = new Metrics();
        public List<RunResult> RunResults = new List<RunResult>();
        #endregion

        #region Properties
        public int Precision
        {
            get { return _precision; }
        }
        public TimeSpan EstimatedRuntimeLeft
        {
            get
            {
                long estimatedRuntimeLeft = (long)(((DateTime.Now - _metrics.StartMeasuringRuntime).TotalMilliseconds / _metrics.TotalLogEntriesProcessed) * (_metrics.TotalLogEntries - _metrics.TotalLogEntriesProcessed) * 10000);
                if (estimatedRuntimeLeft < 0)
                    estimatedRuntimeLeft = 0;
                return new TimeSpan(estimatedRuntimeLeft);
            }
        }
        public Metrics Metrics
        {
            get { return _metrics; }
            //Manual override
            set { _metrics = value; }
        }

        private ulong LogEntryResultsCount
        {
            get
            {
                ulong count = 0;
                foreach (RunResult rr in RunResults)
                    foreach (UserResult ur in rr.UserResults)
                        count += ur.LogEntriesProcessed;
                return count;
            }
        }
        private int UserActionResultsCount
        {
            get
            {
                int count = 0;
                foreach (RunResult rr in RunResults)
                    foreach (UserResult ur in rr.UserResults)
                        count += ur.UserActionResults.Count;
                return count;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This will start measuring time. Call StopTimeMeasurement() when the precision finished.
        /// </summary>
        /// <param name="precision"></param>
        /// <param name="totalLogEntries"></param>
        /// <param name="startOfPrecision"></param>
        public PrecisionResult(int precision, ulong totalLogEntries, DateTime startOfPrecision)
        {
            _precision = precision;
            _metrics.TotalLogEntries = totalLogEntries;
            _metrics.StartMeasuringRuntime = startOfPrecision;
            _sw = Stopwatch.StartNew(); ;
        }
        public PrecisionResult(SerializationInfo info, StreamingContext ctxt)
        {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info))
            {
                _precision = sr.ReadInt32();
                _metrics = (Metrics)sr.ReadObject();
                RunResults = sr.ReadCollection<RunResult>(RunResults) as List<RunResult>;
            }
            sr = null;
            //Not pretty, but helps against mem saturation.
            GC.Collect();
        }
        #endregion

        #region Functions
        public void RefreshLogEntryResultMetrics()
        {
            _metrics.MeasuredRunTime = _sw.Elapsed;
            if (RunResults.Count == 0)
                return;
            _metrics.AverageTimeToLastByte = new TimeSpan();
            _metrics.MaxTimeToLastByte = new TimeSpan();
            _metrics.AverageDelay = new TimeSpan();
            _metrics.TotalLogEntriesProcessed = 0;
            _metrics.TotalLogEntriesProcessedPerTick = 0;
            _metrics.Errors = 0;

            foreach (RunResult result in RunResults)
            {
                result.RefreshLogEntryResultMetrics();
                Metrics resultMetrics = result.Metrics;

                _metrics.AverageTimeToLastByte = _metrics.AverageTimeToLastByte.Add(resultMetrics.AverageTimeToLastByte);
                if (resultMetrics.MaxTimeToLastByte > _metrics.MaxTimeToLastByte)
                    _metrics.MaxTimeToLastByte = resultMetrics.MaxTimeToLastByte;
                _metrics.AverageDelay = _metrics.AverageDelay.Add(resultMetrics.AverageDelay);
                _metrics.TotalLogEntriesProcessed += resultMetrics.TotalLogEntriesProcessed;
                _metrics.TotalLogEntriesProcessedPerTick += resultMetrics.TotalLogEntriesProcessedPerTick;
                _metrics.Errors += resultMetrics.Errors;
            }
            _metrics.TotalLogEntriesProcessedPerTick /= RunResults.Count;
            _metrics.AverageTimeToLastByte = new TimeSpan(_metrics.AverageTimeToLastByte.Ticks / RunResults.Count);
            //_metrics.Percentile90MaxTimeToLastByte = new TimeSpan((long)((double)_metrics.MaxTimeToLastByte.Ticks * 0.9));
            _metrics.AverageDelay = new TimeSpan(_metrics.AverageDelay.Ticks / RunResults.Count);
        }
        public void StopTimeMeasurement()
        {
            _sw.Stop();
            foreach (RunResult result in RunResults)
                result.StopTimeMeasurement();
        }

        /// <summary>
        /// Returns the results formatted, this will fill in Percentile90MaxTimeToLastByte.
        /// </summary>
        /// <param name="concurrentUsers"></param>
        /// <param name="lowestLevel">Only if it is the lowest visible level it will have the 5 last before last entries for avoiding confusion.</param>
        /// <returns></returns>
        public object[] DetailedLogEntryResultMetrics(string concurrentUsers, bool lowestLevel)
        {
            RefreshPercentile90MaxTimeToLastByteForLogEntryResults();

            if (lowestLevel)
                return new object[] {_metrics.StartMeasuringRuntime
                      ,_metrics.MeasuredRunTime.ToShortFormattedString()
                      ,  concurrentUsers
                      , (Precision + 1)
                      , string.Empty //Run
                      , string.Empty //User
                      , string.Empty //User Action
                      , string.Empty //Log Entry
                      , _metrics.TotalLogEntriesProcessed + " / " + _metrics.TotalLogEntries
                      , (_metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond)
                      , _metrics.AverageTimeToLastByte.TotalMilliseconds
                      , _metrics.MaxTimeToLastByte.TotalMilliseconds
                      , _metrics.Percentile95MaxTimeToLastByte.TotalMilliseconds
                      , _metrics.AverageDelay.TotalMilliseconds
                      , _metrics.Errors
                  };

            return new object[] {_metrics.StartMeasuringRuntime
                      ,_metrics.MeasuredRunTime.ToShortFormattedString()
                      ,  concurrentUsers
                      , (Precision + 1)
                      , string.Empty //Run
                      , string.Empty //User
                      , string.Empty //User Action
                      , string.Empty //Log Entry
                      , _metrics.TotalLogEntriesProcessed + " / " + _metrics.TotalLogEntries
                      , (_metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond)
                      , string.Empty
                      , string.Empty
                      , string.Empty
                      , string.Empty
                      , _metrics.Errors
                  };
        }
        private void RefreshPercentile90MaxTimeToLastByteForLogEntryResults()
        {
            int percent5 = (int)(LogEntryResultsCount * 0.05);
            if (percent5 == 0)
            {
                if (_metrics.MaxTimeToLastByte == TimeSpan.MinValue)
                    RefreshLogEntryResultMetrics();
                _metrics.Percentile95MaxTimeToLastByte = _metrics.MaxTimeToLastByte;
            }
            else
            {
                List<TimeSpan> sorted = new List<TimeSpan>((int)LogEntryResultsCount);
                foreach (RunResult rr in RunResults)
                    foreach (UserResult ur in rr.UserResults)
                        foreach (var result in ur.LogEntryResults)
                            if (!result.Empty)
                                sorted.Add(result.TimeToLastByte);

                sorted.Sort();

                _metrics.Percentile95MaxTimeToLastByte = sorted[sorted.Count - percent5 - 1];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userAction"></param>
        /// <param name="directCall">Direct called function --> set to true</param>
        /// <returns></returns>
        public Dictionary<LogEntryResult, Metrics> GetPivotedLogEntryResults(string userAction, bool directCall = true)
        {
            var pivotedResults = new Dictionary<LogEntryResult, Metrics>();
            var pivotedPercentiles = directCall ? GetPivotedPercentile95MaxTimeToLastByteForLogEntryResults(userAction) : new Dictionary<LogEntryResult, TimeSpan>();

            int runCount = RunResults.Count;

            foreach (RunResult runResult in RunResults)
            {
                var combinedRunResults = runResult.GetPivotedLogEntryResults(userAction, false);
                foreach (LogEntryResult logEntryResult in combinedRunResults.Keys)
                    if (string.Equals(logEntryResult.UserAction, userAction, StringComparison.Ordinal))
                    {
                        bool found = false;
                        Metrics runMetrics, newMetrics;

                        if (runCount != 1)
                            foreach (var result in pivotedResults.Keys)
                            {
                                if (string.Equals(result.LogEntryIndex, logEntryResult.LogEntryIndex, StringComparison.Ordinal))
                                {
                                    runMetrics = combinedRunResults[logEntryResult];
                                    newMetrics = pivotedResults[result];

                                    newMetrics.AverageTimeToLastByte = newMetrics.AverageTimeToLastByte.Add(new TimeSpan(runMetrics.AverageTimeToLastByte.Ticks / RunResults.Count));

                                    if (runMetrics.MaxTimeToLastByte > newMetrics.MaxTimeToLastByte)
                                        newMetrics.MaxTimeToLastByte = runMetrics.MaxTimeToLastByte;

                                    newMetrics.AverageDelay = newMetrics.AverageDelay.Add(new TimeSpan(runMetrics.AverageDelay.Ticks / RunResults.Count));
                                    newMetrics.TotalLogEntries += runMetrics.TotalLogEntries;
                                    newMetrics.TotalLogEntriesProcessed += runMetrics.TotalLogEntriesProcessed;
                                    newMetrics.Errors += runMetrics.Errors;
                                    pivotedResults[result] = newMetrics;

                                    found = true;
                                    break;
                                }
                            }
                        if (!found)
                        {
                            runMetrics = combinedRunResults[logEntryResult];
                            newMetrics = new Metrics();
                            newMetrics.AverageTimeToLastByte = new TimeSpan(runMetrics.AverageTimeToLastByte.Ticks / RunResults.Count);
                            newMetrics.MaxTimeToLastByte = runMetrics.MaxTimeToLastByte;
                            newMetrics.Percentile95MaxTimeToLastByte = runMetrics.Percentile95MaxTimeToLastByte;
                            newMetrics.AverageDelay = new TimeSpan(runMetrics.AverageDelay.Ticks / RunResults.Count);
                            newMetrics.TotalLogEntries = runMetrics.TotalLogEntries;
                            newMetrics.TotalLogEntriesProcessed = runMetrics.TotalLogEntriesProcessed;
                            newMetrics.Errors = runMetrics.Errors;

                            foreach (var ler in pivotedPercentiles.Keys)
                                if (string.Equals(ler.LogEntryIndex, logEntryResult.LogEntryIndex, StringComparison.Ordinal))
                                {
                                    newMetrics.Percentile95MaxTimeToLastByte = pivotedPercentiles[ler];
                                    break;
                                }

                            pivotedResults.Add(logEntryResult, newMetrics);
                        }
                    }
            }
            return pivotedResults;
        }
        private Dictionary<LogEntryResult, TimeSpan> GetPivotedPercentile95MaxTimeToLastByteForLogEntryResults(string userAction)
        {
            var tempTimeToLastByte = new Dictionary<LogEntryResult, List<TimeSpan>>();

            foreach (var rr in RunResults)
                foreach (var ur in rr.UserResults)
                    foreach (var ler in ur.LogEntryResults)
                        if (!ler.Empty && string.Equals(ler.UserAction, userAction, StringComparison.Ordinal))
                        {
                            bool found = false;
                            foreach (var result in tempTimeToLastByte.Keys)
                                if (string.Equals(result.LogEntryIndex, ler.LogEntryIndex, StringComparison.Ordinal))
                                {
                                    tempTimeToLastByte[result].Add(ler.TimeToLastByte);
                                    found = true;
                                    break;
                                }
                            if (!found)
                            {
                                var l = new List<TimeSpan>();
                                l.Add(ler.TimeToLastByte);
                                tempTimeToLastByte.Add(ler, l);
                            }
                        }

            var pivoted = new Dictionary<LogEntryResult, TimeSpan>(tempTimeToLastByte.Count);
            foreach (var key in tempTimeToLastByte.Keys)
                pivoted.Add(key, GetPercentile95MaxTimeToLastByte(tempTimeToLastByte[key]));

            return pivoted;
        }

        public Dictionary<UserActionResult, Metrics> GetPivotedUserActionResults()
        {
            var pivotedResults = new Dictionary<UserActionResult, Metrics>();
            var pivotedPercentiles = GetPivotedPercentile95MaxTimeToLastByteForUserActionResults();
            int runCount = RunResults.Count;

            foreach (RunResult runResult in RunResults)
            {
                var combinedRunResults = runResult.GetPivotedUserActionResults();
                foreach (UserActionResult userActionResult in combinedRunResults.Keys)
                {
                    bool found = false;
                    Metrics runMetrics, newMetrics;
                    if (runCount != 1)
                        foreach (var result in pivotedResults.Keys)
                        {
                            if (string.Equals(result.UserAction, userActionResult.UserAction, StringComparison.Ordinal))
                            {
                                runMetrics = combinedRunResults[userActionResult];
                                newMetrics = pivotedResults[result];

                                newMetrics.AverageTimeToLastByte = newMetrics.AverageTimeToLastByte.Add(new TimeSpan(runMetrics.AverageTimeToLastByte.Ticks / RunResults.Count));

                                if (runMetrics.MaxTimeToLastByte > newMetrics.MaxTimeToLastByte)
                                    newMetrics.MaxTimeToLastByte = runMetrics.MaxTimeToLastByte;

                                newMetrics.AverageDelay = newMetrics.AverageDelay.Add(new TimeSpan(runMetrics.AverageDelay.Ticks / RunResults.Count));
                                newMetrics.TotalLogEntries += runMetrics.TotalLogEntries;
                                newMetrics.TotalLogEntriesProcessed += runMetrics.TotalLogEntriesProcessed;
                                newMetrics.Errors += runMetrics.Errors;
                                pivotedResults[result] = newMetrics;

                                found = true;
                                break;
                            }
                        }
                    if (!found)
                    {
                        runMetrics = combinedRunResults[userActionResult];
                        newMetrics = new Metrics();
                        newMetrics.AverageTimeToLastByte = new TimeSpan(runMetrics.AverageTimeToLastByte.Ticks / RunResults.Count);
                        newMetrics.MaxTimeToLastByte = runMetrics.MaxTimeToLastByte;
                        newMetrics.AverageDelay = new TimeSpan(runMetrics.AverageDelay.Ticks / RunResults.Count);
                        newMetrics.TotalLogEntries = runMetrics.TotalLogEntries;
                        newMetrics.TotalLogEntriesProcessed = runMetrics.TotalLogEntriesProcessed;
                        newMetrics.Errors = runMetrics.Errors;

                        foreach (var uar in pivotedPercentiles.Keys)
                            if (string.Equals(uar.UserAction, userActionResult.UserAction, StringComparison.Ordinal))
                            {
                                newMetrics.Percentile95MaxTimeToLastByte = pivotedPercentiles[uar];
                                break;
                            }

                        pivotedResults.Add(userActionResult, newMetrics);
                    }
                }
            }
            return pivotedResults;
        }
        private Dictionary<UserActionResult, TimeSpan> GetPivotedPercentile95MaxTimeToLastByteForUserActionResults()
        {
            var tempTimeToLastByte = new Dictionary<UserActionResult, List<TimeSpan>>();

            foreach (var rr in RunResults)
                foreach (var ur in rr.UserResults)
                    foreach (var uar in ur.UserActionResults)
                    {
                        bool found = false;
                        uar.Value.RefreshMetrics();

                        foreach (var result in tempTimeToLastByte.Keys)
                            if (result.UserActionIndex == uar.Key)
                            {
                                tempTimeToLastByte[result].Add(uar.Value.TimeToLastByte);
                                found = true;
                                break;
                            }
                        if (!found)
                        {
                            var l = new List<TimeSpan>();
                            l.Add(uar.Value.TimeToLastByte);
                            tempTimeToLastByte.Add(uar.Value, l);
                        }
                    }

            var pivoted = new Dictionary<UserActionResult, TimeSpan>(tempTimeToLastByte.Count);
            foreach (var key in tempTimeToLastByte.Keys)
                pivoted.Add(key, GetPercentile95MaxTimeToLastByte(tempTimeToLastByte[key]));

            return pivoted;
        }

        private TimeSpan GetPercentile95MaxTimeToLastByte(List<TimeSpan> timeToLastBytes)
        {
            int timeToLastBytesCount = timeToLastBytes.Count;
            int percent5 = (int)(timeToLastBytesCount * 0.05);

            timeToLastBytes.Sort();

            return timeToLastBytes[timeToLastBytesCount - percent5 - 1];
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter())
            {
                sw.Write(_precision);
                sw.WriteObject(_metrics);
                sw.Write<RunResult>(RunResults);
                sw.AddToInfo(info);
            }
            sw = null;
            //Not pretty, but helps against mem saturation.
            GC.Collect();
        }

        #endregion
    }
    [Serializable]
    public class RunResult : IResult, ISerializable
    {
        #region Fields
        private Stopwatch _sw;
        private int _run;
        private int _singleUserLogEntryCount;
        private Metrics _metrics = new Metrics();
        private ulong _baseLogEntryCount;
        /// <summary>
        /// Set if the run was finished once. (Meaning the result set can not grow anymore (run sync break on last)).
        /// </summary>
        private bool _runDoneOnce;
        private Dictionary<DateTime, DateTime> _runStartedAndStopped = new Dictionary<DateTime, DateTime>();
        public UserResult[] UserResults;
        #endregion

        #region Properties
        public int Run
        {
            get { return _run; }
        }
        /// <summary>
        /// Only use this for setting results, for nothing else.
        /// Returns null if the run was done once (run sync break on last)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public UserResult this[int index]
        {
            get
            {
                if (_runDoneOnce)
                    return null;
                return UserResults[index];
            }
        }
        public TimeSpan EstimatedRuntimeLeft
        {
            get
            {
                long estimatedRuntimeLeft = (long)(((DateTime.Now - _metrics.StartMeasuringRuntime).TotalMilliseconds / _metrics.TotalLogEntriesProcessed) * (_metrics.TotalLogEntries - _metrics.TotalLogEntriesProcessed) * 10000);
                if (estimatedRuntimeLeft < 0)
                    estimatedRuntimeLeft = 0;
                return new TimeSpan(estimatedRuntimeLeft);
            }
        }
        public Metrics Metrics
        {
            get { return _metrics; }
            //Manual override
            set { _metrics = value; }
        }

        public ulong BaseLogEntryCount
        {
            get { return _baseLogEntryCount; }
        }

        private ulong LogEntryResultsCount
        {
            get
            {
                ulong count = 0;
                foreach (UserResult ur in UserResults)
                    count += ur.LogEntriesProcessed;
                return count;
            }
        }
        private int UserActionResultsCount
        {
            get
            {
                int count = 0;
                foreach (UserResult ur in UserResults)
                    count += ur.UserActionResults.Count;
                return count;
            }
        }

        /// <summary>
        /// When a run started or restarted (run sync break on last) and stopped.
        /// </summary>
        public Dictionary<DateTime, DateTime> RunStartedAndStopped
        {
            get { return _runStartedAndStopped; }
        }

        /// <summary>
        /// Return if this run is re-running or not (break on last run sync)
        /// </summary>
        public bool RunDoneOnce
        {
            get { return _runDoneOnce; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="run"></param>
        /// <param name="users"></param>
        /// <param name="totalLogEntries"></param>
        /// <param name="singleUserLogEntryCount"></param>
        /// <param name="initOfRun"></param>
        /// <param name="runStartedAndStopped">Optional, if none have been determined (started only after initialization of the thread and connectionpool)</param>
        /// <param name="runDoneOnce">If the run was done onse or not (break on last run sync)</param>
        public RunResult(int run, int users, ulong totalLogEntries, int singleUserLogEntryCount, DateTime initOfRun, Dictionary<DateTime, DateTime> runStartedAndStopped = null, bool runDoneOnce = false)
        {
            _run = run;
            _singleUserLogEntryCount = singleUserLogEntryCount;
            UserResults = new UserResult[users];
            for (int i = 0; i < users; i++)
                UserResults[i] = new UserResult(_singleUserLogEntryCount);

            _baseLogEntryCount = totalLogEntries;
            _metrics.TotalLogEntries = totalLogEntries;
            _metrics.StartMeasuringRuntime = initOfRun;

            _runStartedAndStopped = (runStartedAndStopped == null) ? new Dictionary<DateTime, DateTime>() : runStartedAndStopped;
            _runDoneOnce = runDoneOnce;

            _sw = Stopwatch.StartNew();
        }
        public RunResult(SerializationInfo info, StreamingContext ctxt)
        {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info))
            {
                _run = sr.ReadInt32();
                _metrics = (Metrics)sr.ReadObject();
                _runStartedAndStopped = sr.ReadDictionary<DateTime, DateTime>(_runStartedAndStopped) as Dictionary<DateTime, DateTime>;
                _runDoneOnce = sr.ReadBoolean();
                UserResults = sr.ReadArray(typeof(UserResult)) as UserResult[];
            }
            sr = null;
            //Not pretty, but helps against mem saturation.
            GC.Collect();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Set when a run started or restarted (run sync break on last) to the Dictionary.
        /// </summary>
        /// <param name="at"></param>
        public void SetRunStarted(DateTime at)
        {
            _runStartedAndStopped.Add(at, DateTime.MinValue);
        }
        /// <summary>
        /// Set when a run stopped to the Dictionary.
        /// </summary>
        /// <param name="at"></param>
        public void SetRunStopped(DateTime at)
        {
            _runStartedAndStopped[_runStartedAndStopped.GetKeyAt(_runStartedAndStopped.Count - 1)] = at;
        }
        /// <summary>
        /// Used when redoing the same run for for instance break on last run synchronization.
        /// </summary>
        public void SetRunDoneOnce()
        {
            _runDoneOnce = true;
        }
        public void RefreshLogEntryResultMetrics()
        {
            _metrics.MeasuredRunTime = _sw.Elapsed;
            _metrics.AverageTimeToLastByte = new TimeSpan();
            _metrics.MaxTimeToLastByte = TimeSpan.MinValue;
            _metrics.AverageDelay = new TimeSpan();
            _metrics.TotalLogEntriesProcessed = 0;
            _metrics.TotalLogEntriesProcessedPerTick = 0;
            _metrics.Errors = 0;

            int enteredUserResultsCount = 0;
            foreach (UserResult result in UserResults)
            {
                if (result.Entered)
                    ++enteredUserResultsCount;

                TimeSpan resultAverageTimeToLastByte, resultMaxTimeToLastByte,
                    resultTotalDelay, resultAverageDelay;
                ulong resultLogEntriesProcessed, resultErrors;
                double resultLogEntriesProcessedPerTick;
                result.RefreshLogEntryResultMetrics();
                result.GetLogEntryResultMetrics(out resultAverageTimeToLastByte, out resultMaxTimeToLastByte,
                    out resultTotalDelay, out resultAverageDelay, out resultLogEntriesProcessed, out resultLogEntriesProcessedPerTick,
                    out resultErrors);

                _metrics.AverageTimeToLastByte = _metrics.AverageTimeToLastByte.Add(resultAverageTimeToLastByte);
                if (resultMaxTimeToLastByte > _metrics.MaxTimeToLastByte)
                    _metrics.MaxTimeToLastByte = resultMaxTimeToLastByte;
                _metrics.AverageDelay = _metrics.AverageDelay.Add(resultAverageDelay);
                _metrics.TotalLogEntriesProcessed += resultLogEntriesProcessed;
                _metrics.TotalLogEntriesProcessedPerTick += resultLogEntriesProcessedPerTick;
                _metrics.Errors += resultErrors;
            }

            if (enteredUserResultsCount != 0)
            {
                _metrics.AverageTimeToLastByte = new TimeSpan(_metrics.AverageTimeToLastByte.Ticks / enteredUserResultsCount);
                _metrics.AverageDelay = new TimeSpan(_metrics.AverageDelay.Ticks / enteredUserResultsCount);
            }
        }
        public void StopTimeMeasurement()
        {
            _sw.Stop();
        }

        /// <summary>
        /// Returns the results formatted, this will fill in Percentile90MaxTimeToLastByte.
        /// </summary>
        /// <param name="concurrentUsers"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public object[] DetailedLogEntryResultMetrics(string concurrentUsers, string precision, bool lowestLevel)
        {
            RefreshPercentile95MaxTimeToLastByteForLogEntryResults();

            if (lowestLevel)
                return new object[] {_metrics.StartMeasuringRuntime
                      ,_metrics.MeasuredRunTime.ToShortFormattedString()
                      , concurrentUsers
                      , precision 
                      , (Run + 1)
                      , string.Empty //User
                      , string.Empty //User Action
                      , string.Empty //Log Entry
                      , _metrics.TotalLogEntriesProcessed + " / " + _metrics.TotalLogEntries 
                      , (_metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond)
                      , _metrics.AverageTimeToLastByte.TotalMilliseconds
                      , _metrics.MaxTimeToLastByte.TotalMilliseconds
                      , _metrics.Percentile95MaxTimeToLastByte.TotalMilliseconds
                      , _metrics.AverageDelay.TotalMilliseconds
                      , _metrics.Errors
                    };

            return new object[] {_metrics.StartMeasuringRuntime
                      ,_metrics.MeasuredRunTime.ToShortFormattedString()
                      , concurrentUsers
                      , precision 
                      , (Run + 1)
                      , string.Empty //User
                      , string.Empty //User Action
                      , string.Empty //Log Entry
                      , _metrics.TotalLogEntriesProcessed + " / " + _metrics.TotalLogEntries 
                      , (_metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond)
                      , string.Empty
                      , string.Empty
                      , string.Empty
                      , string.Empty
                      , _metrics.Errors
                    };
        }
        private void RefreshPercentile95MaxTimeToLastByteForLogEntryResults()
        {
            int percent5 = (int)(LogEntryResultsCount * 0.05);
            if (percent5 == 0)
            {
                if (_metrics.MaxTimeToLastByte == TimeSpan.MinValue)
                    RefreshLogEntryResultMetrics();
                _metrics.Percentile95MaxTimeToLastByte = _metrics.MaxTimeToLastByte;
            }
            else
            {
                List<TimeSpan> sorted = new List<TimeSpan>((int)LogEntryResultsCount);
                foreach (UserResult ur in UserResults)
                    foreach (var result in ur.LogEntryResults)
                        if (!result.Empty)
                            sorted.Add(result.TimeToLastByte);

                sorted.Sort();

                _metrics.Percentile95MaxTimeToLastByte = sorted[sorted.Count - percent5 - 1];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userAction"></param>
        /// <param name="directCall">Direct called function --> set to true</param>
        /// <returns></returns>
        public Dictionary<LogEntryResult, Metrics> GetPivotedLogEntryResults(string userAction, bool directCall = true)
        {
            var tempMetrics = new Dictionary<LogEntryResult, Metrics>();
            var tempTimeToLastByte = new Dictionary<LogEntryResult, List<TimeSpan>>();
            foreach (UserResult userResult in UserResults)
                foreach (LogEntryResult logEntryResult in userResult.LogEntryResults)
                    if (!logEntryResult.Empty && string.Equals(logEntryResult.UserAction, userAction, StringComparison.Ordinal))
                    {
                        bool found = false;
                        Metrics metrics;
                        foreach (var result in tempMetrics.Keys)
                            if (string.Equals(result.LogEntryIndex, logEntryResult.LogEntryIndex, StringComparison.Ordinal))
                            {
                                metrics = tempMetrics[result];

                                metrics.AverageTimeToLastByte = metrics.AverageTimeToLastByte.Add(logEntryResult.TimeToLastByte);
                                if (logEntryResult.TimeToLastByte > metrics.MaxTimeToLastByte)
                                    metrics.MaxTimeToLastByte = logEntryResult.TimeToLastByte;
                                metrics.AverageDelay = metrics.AverageDelay.Add(new TimeSpan(logEntryResult.DelayInMilliseconds * TimeSpan.TicksPerMillisecond));
                                ++metrics.TotalLogEntries;
                                ++metrics.TotalLogEntriesProcessed;
                                if (logEntryResult.Exception != null)
                                    ++metrics.Errors;

                                tempMetrics[result] = metrics;
                                tempTimeToLastByte[result].Add(logEntryResult.TimeToLastByte);

                                found = true;
                                break;
                            }
                        if (!found)
                        {
                            metrics = new Metrics();
                            metrics.AverageTimeToLastByte = logEntryResult.TimeToLastByte;
                            metrics.MaxTimeToLastByte = logEntryResult.TimeToLastByte;
                            metrics.AverageDelay = new TimeSpan(logEntryResult.DelayInMilliseconds * TimeSpan.TicksPerMillisecond);
                            metrics.TotalLogEntries = 1;
                            metrics.TotalLogEntriesProcessed = 1;
                            metrics.Errors = (ulong)(logEntryResult.Exception == null ? 0 : 1);
                            tempMetrics.Add(logEntryResult, metrics);

                            List<TimeSpan> l = new List<TimeSpan>();
                            l.Add(logEntryResult.TimeToLastByte);
                            tempTimeToLastByte.Add(logEntryResult, l);
                        }
                    }

            var combinedResults = new Dictionary<LogEntryResult, Metrics>(tempMetrics.Count);
            foreach (var result in tempMetrics.Keys)
            {
                Metrics metrics = tempMetrics[result];
                metrics.AverageTimeToLastByte = new TimeSpan(metrics.AverageTimeToLastByte.Ticks / (long)metrics.TotalLogEntriesProcessed);
                metrics.AverageDelay = new TimeSpan(metrics.AverageDelay.Ticks / (long)metrics.TotalLogEntriesProcessed);

                if (directCall)
                    metrics.Percentile95MaxTimeToLastByte = GetPercentile95MaxTimeToLastByte(tempTimeToLastByte[result]);

                combinedResults.Add(result, metrics);
            }
            return combinedResults;
        }
        public Dictionary<UserActionResult, Metrics> GetPivotedUserActionResults()
        {
            Dictionary<UserActionResult, Metrics> tempMetrics = new Dictionary<UserActionResult, Metrics>();
            Dictionary<UserActionResult, List<TimeSpan>> tempTimeToLastByte = new Dictionary<UserActionResult, List<TimeSpan>>();
            foreach (UserResult userResult in UserResults)
                foreach (var uar in userResult.UserActionResults)
                {
                    bool found = false;
                    Metrics metrics;

                    var userActionResult = uar.Value;
                    userActionResult.RefreshMetrics();

                    foreach (var result in tempMetrics.Keys)
                    {
                        if (result.UserActionIndex == uar.Key)
                        {
                            metrics = tempMetrics[result];

                            metrics.AverageTimeToLastByte = metrics.AverageTimeToLastByte.Add(userActionResult.TimeToLastByte);
                            if (userActionResult.TimeToLastByte > metrics.MaxTimeToLastByte)
                                metrics.MaxTimeToLastByte = userActionResult.TimeToLastByte;
                            metrics.AverageDelay = metrics.AverageDelay.Add(new TimeSpan(userActionResult.DelayInMilliseconds * TimeSpan.TicksPerMillisecond));
                            ++metrics.TotalLogEntries;
                            ++metrics.TotalLogEntriesProcessed;
                            metrics.Errors += userActionResult.Errors;
                            tempMetrics[result] = metrics;

                            tempMetrics[result] = metrics;
                            tempTimeToLastByte[result].Add(userActionResult.TimeToLastByte);

                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        metrics = new Metrics();

                        metrics.AverageTimeToLastByte = userActionResult.TimeToLastByte;
                        metrics.MaxTimeToLastByte = userActionResult.TimeToLastByte;
                        metrics.AverageDelay = new TimeSpan(userActionResult.DelayInMilliseconds * TimeSpan.TicksPerMillisecond);
                        metrics.TotalLogEntries = 1;
                        metrics.TotalLogEntriesProcessed = 1;
                        metrics.Errors = userActionResult.Errors;
                        tempMetrics.Add(userActionResult, metrics);

                        List<TimeSpan> l = new List<TimeSpan>();
                        l.Add(userActionResult.TimeToLastByte);
                        tempTimeToLastByte.Add(userActionResult, l);
                    }
                }
            var combinedResults = new Dictionary<UserActionResult, Metrics>(tempMetrics.Count);
            foreach (var result in tempMetrics.Keys)
            {
                Metrics metrics = tempMetrics[result];
                metrics.AverageTimeToLastByte = new TimeSpan(metrics.AverageTimeToLastByte.Ticks / (long)metrics.TotalLogEntriesProcessed);
                metrics.AverageDelay = new TimeSpan(metrics.AverageDelay.Ticks / (long)metrics.TotalLogEntriesProcessed);

                metrics.Percentile95MaxTimeToLastByte = GetPercentile95MaxTimeToLastByte(tempTimeToLastByte[result]);

                combinedResults.Add(result, metrics);
            }
            return combinedResults;
        }
        private TimeSpan GetPercentile95MaxTimeToLastByte(List<TimeSpan> timeToLastBytes)
        {
            int timeToLastBytesCount = timeToLastBytes.Count;
            int percent5 = (int)(timeToLastBytesCount * 0.05);

            timeToLastBytes.Sort();

            return timeToLastBytes[timeToLastBytesCount - percent5 - 1];
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter())
            {
                sw.Write(_run);
                sw.WriteObject(_metrics);
                sw.Write<DateTime, DateTime>(_runStartedAndStopped);
                sw.Write(_runDoneOnce);
                sw.Write<UserResult>(UserResults);
                sw.AddToInfo(info);
            }
            sw = null;
            //Not pretty, but helps against mem saturation.
            GC.Collect();
        }
        #endregion

    }
    /// <summary>
    /// Contains the results for all log entries for a certain simulated user.
    /// </summary>
    [Serializable]
    public class UserResult : ISerializable
    {
        #region Fields
        private TimeSpan _averageTimeToLastByte = new TimeSpan(),
            _maxTimeToLastByte = TimeSpan.MinValue,
            _totalDelay = new TimeSpan(),
            _averageDelay = new TimeSpan();

        private ulong _logEntriesProcessed;
        private ulong _errors;
        private double _logEntriesProcessedPerTick;

        public string User = string.Empty;

        // A dictionary for fast lookups
        private SortedDictionary<int, UserActionResult> _userActionResults;
        /// <summary>
        /// Use the SetLogEntryResultAt fx to add an item to this.
        /// Can contain null!
        /// </summary>
        public LogEntryResult[] LogEntryResults;
        #endregion

        /// <summary>
        /// To acknowledge that metrics can be calculated.
        /// </summary>
        public bool Entered
        {
            get { return _logEntriesProcessed != 0; }
        }
        public SortedDictionary<int, UserActionResult> UserActionResults
        {
            get
            {
                //Must only occur once.
                if (_userActionResults == null)
                {
                    _userActionResults = new SortedDictionary<int, UserActionResult>();
                    UserActionResult userActionResult;

                    foreach (var ler in LogEntryResults)
                        if (!ler.Empty)
                        {
                            userActionResult = null;
                            if (!UserActionResults.TryGetValue(ler.UserActionIndex, out userActionResult))
                            {
                                string userActionName = ler.UserAction;
                                int userActionIndex = ler.UserActionIndex;
                                userActionResult = new UserActionResult(userActionIndex, userActionName);
                                UserActionResults.Add(userActionIndex, userActionResult);
                            }
                            userActionResult.LogEntryResults.Add(ler);
                        }
                }
                return _userActionResults;
            }
        }
        public ulong LogEntriesProcessed
        {
            get { return _logEntriesProcessed; }
        }

        #region Constructors
        public UserResult(int logEntryCount)
        {
            LogEntryResults = new LogEntryResult[logEntryCount];
            for (int i = 0; i != LogEntryResults.Length; i++)
                LogEntryResults[i] = new LogEntryResult();
        }
        public UserResult(SerializationInfo info, StreamingContext ctxt)
        {
            using (SerializationReader sr = SerializationReader.GetReader(info))
            {
                _averageTimeToLastByte = sr.ReadTimeSpan();
                _maxTimeToLastByte = sr.ReadTimeSpan();
                _totalDelay = sr.ReadTimeSpan();
                _averageDelay = sr.ReadTimeSpan();
                _logEntriesProcessed = sr.ReadUInt64();
                _errors = sr.ReadUInt64();
                _logEntriesProcessedPerTick = sr.ReadDouble();

                User = sr.ReadString();

                LogEntryResults = sr.ReadArray(typeof(LogEntryResult)) as LogEntryResult[];
            }
        }
        #endregion

        #region Functions
        public void RefreshLogEntryResultMetrics()
        {
            CalculateLogEntryResultMetrics(out _averageTimeToLastByte,
                out _maxTimeToLastByte,
                out _totalDelay,
                out _averageDelay,
                out _logEntriesProcessedPerTick,
                out _errors);
        }
        /// <summary>
        /// Call RefreshMetrics() first.
        /// </summary>
        /// <param name="averageTimeToLastByte"></param>
        /// <param name="maxTimeToLastByte"></param>
        /// <param name="percentile90MaxTimeToLastByte"></param>
        /// <param name="totalDelay"></param>
        /// <param name="averageDelay"></param>
        /// <param name="logEntriesProcessed"></param>
        /// <param name="logEntriesProcessedPerTick"></param>
        /// <param name="errors"></param>
        public void GetLogEntryResultMetrics(out TimeSpan averageTimeToLastByte,
            out TimeSpan maxTimeToLastByte,
            out TimeSpan totalDelay,
            out TimeSpan averageDelay,
            out ulong logEntriesProcessed,
            out double logEntriesProcessedPerTick,
            out ulong errors)
        {
            averageTimeToLastByte = _averageTimeToLastByte;
            maxTimeToLastByte = _maxTimeToLastByte;
            totalDelay = _totalDelay;
            averageDelay = _averageDelay;
            logEntriesProcessed = _logEntriesProcessed;
            logEntriesProcessedPerTick = _logEntriesProcessedPerTick;
            errors = _errors;
        }
        private void CalculateLogEntryResultMetrics(out TimeSpan averageTimeToLastByte,
            out TimeSpan maxTimeToLastByte,
            out TimeSpan totalDelay,
            out TimeSpan averageDelay,
            out double logEntriesProcessedPerTick,
            out ulong errors)
        {
            totalDelay = new TimeSpan();
            averageTimeToLastByte = new TimeSpan();
            maxTimeToLastByte = new TimeSpan();
            averageDelay = new TimeSpan();
            logEntriesProcessedPerTick = 0;
            errors = 0;
            if (_logEntriesProcessed == 0)
                return;

            TimeSpan totalTimeToLastByte = new TimeSpan();
            int logEntriesProcessed = (int)_logEntriesProcessed;
            for (int i = 0; i != LogEntryResults.Length; i++)
            {
                var result = LogEntryResults[i];
                if (!result.Empty)
                {
                    totalTimeToLastByte = totalTimeToLastByte.Add(result.TimeToLastByte);
                    if (result.TimeToLastByte > maxTimeToLastByte)
                        maxTimeToLastByte = result.TimeToLastByte;
                    totalDelay = totalDelay.Add(new TimeSpan(result.DelayInMilliseconds * TimeSpan.TicksPerMillisecond));
                    if (result.Exception != null)
                        ++errors;
                }
            }

            averageTimeToLastByte = new TimeSpan(totalTimeToLastByte.Ticks / logEntriesProcessed);
            averageDelay = new TimeSpan(totalDelay.Ticks / logEntriesProcessed);
            logEntriesProcessedPerTick = (double)logEntriesProcessed / (totalTimeToLastByte.Ticks + totalDelay.Ticks);
        }
        public void SetLogEntryResultAt(int index, LogEntryResult result)
        {
            LogEntryResults[index] = result;
            ++_logEntriesProcessed;
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            using (SerializationWriter sw = SerializationWriter.GetWriter())
            {
                sw.Write(_averageTimeToLastByte);
                sw.Write(_maxTimeToLastByte);
                sw.Write(_totalDelay);
                sw.Write(_averageDelay);
                sw.Write(_logEntriesProcessed);
                sw.Write(_errors);
                sw.Write(_logEntriesProcessedPerTick);
                sw.Write(User);

                sw.Write<LogEntryResult>(LogEntryResults);
                sw.AddToInfo(info);
            }
        }
        #endregion
    }
    /// <summary>
    /// This is build using the information from the log entry results aka no need to be serialized.
    /// </summary>
    public class UserActionResult
    {
        #region Fields
        public readonly string UserAction;
        public readonly int UserActionIndex;
        public List<LogEntryResult> LogEntryResults = new List<LogEntryResult>();

        public DateTime SentAt;
        public TimeSpan TimeToLastByte;
        public int DelayInMilliseconds;
        public ulong Errors;
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userAction">The ToString() of the user action.</param>
        public UserActionResult(int userActionIndex, string userAction)
        {
            UserAction = userAction;
            UserActionIndex = userActionIndex;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Call this before reading the metrics.
        /// </summary>
        public void RefreshMetrics()
        {
            CalculateMetrics(out SentAt,
                out TimeToLastByte,
                out DelayInMilliseconds,
                out Errors);
        }
        private void CalculateMetrics(out DateTime sentAt,
                    out TimeSpan timeToLastByte,
                    out int delayInMilliseconds,
                    out ulong errors)
        {
            sentAt = DateTime.Now;
            timeToLastByte = new TimeSpan();
            delayInMilliseconds = 0;
            errors = 0;

            if (LogEntryResults.Count == 0)
                return;

            int i = 0;
            foreach (var result in LogEntryResults)
            {
                timeToLastByte = timeToLastByte.Add(result.TimeToLastByte);

                ++i;
                if (i == 1)
                    sentAt = result.SentAt;
                if (i == LogEntryResults.Count)
                    delayInMilliseconds += result.DelayInMilliseconds;
                else
                    timeToLastByte.Add(new TimeSpan(result.DelayInMilliseconds * TimeSpan.TicksPerMillisecond));

                if (result.Exception != null)
                    ++errors;
            }
        }
        #endregion
    }
    [Serializable]
    public class LogEntryResult : ISerializable
    {
        #region Fields
        public readonly bool Empty;

        /// <summary>
        /// Index in Log
        /// </summary>
        public string LogEntryIndex;
        public string LogEntryString;
        public string UserAction;
        public int UserActionIndex;
        public DateTime SentAt;
        public TimeSpan TimeToLastByte;
        public int DelayInMilliseconds;
        public Exception Exception;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an empty Log Entry Result (Property Empty == true)
        /// Only for checking, must be replaced with a real one.
        /// </summary>
        public LogEntryResult()
        {
            Empty = true;
            LogEntryIndex = "N/A";
            LogEntryString = "N/A: No stored result";
            UserAction = string.Empty;
            UserActionIndex = -1;
            SentAt = DateTime.MinValue;
            TimeToLastByte = new TimeSpan();
            DelayInMilliseconds = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logEntryIndex">Should be log.IndexOf(LogEntry) or log.IndexOf(UserAction) + "." + UserAction.IndexOf(LogEntry), this must be unique</param>
        /// <param name="logEntryString"></param>
        /// <param name="userAction">Cannot be null, string.empty is allowed</param>
        /// <param name="sentAt"></param>
        /// <param name="timeToLastByte"></param>
        /// <param name="delayInMilliseconds"></param>
        /// <param name="exception"></param>
        public LogEntryResult(string logEntryIndex, string logEntryString, int userActionIndex, string userAction, DateTime sentAt, TimeSpan timeToLastByte, int delayInMilliseconds, Exception exception)
        {
            LogEntryIndex = logEntryIndex;
            LogEntryString = logEntryString;
            UserActionIndex = userActionIndex;
            UserAction = userAction;
            SentAt = sentAt;
            TimeToLastByte = timeToLastByte;
            DelayInMilliseconds = delayInMilliseconds;
            Exception = exception;
        }
        public LogEntryResult(SerializationInfo info, StreamingContext ctxt)
        {
            using (SerializationReader sr = SerializationReader.GetReader(info))
            {
                Empty = sr.ReadBoolean();
                LogEntryIndex = sr.ReadString();
                LogEntryString = sr.ReadString();
                UserActionIndex = sr.ReadInt32();
                UserAction = sr.ReadString();
                SentAt = sr.ReadDateTime();
                TimeToLastByte = sr.ReadTimeSpan();
                DelayInMilliseconds = sr.ReadInt32();
                Exception = sr.ReadObject() as Exception;
            }
        }
        #endregion

        #region Functions
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            using (SerializationWriter sw = SerializationWriter.GetWriter())
            {
                sw.Write(Empty);
                sw.Write(LogEntryIndex);
                sw.Write(LogEntryString);
                sw.Write(UserActionIndex);
                sw.Write(UserAction);
                sw.Write(SentAt);
                sw.Write(TimeToLastByte);
                sw.Write(DelayInMilliseconds);
                sw.WriteObject(Exception);
                sw.AddToInfo(info);
            }
        }
        #endregion
    }
}