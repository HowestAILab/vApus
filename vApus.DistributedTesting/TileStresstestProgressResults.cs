/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using vApus.Stresstest;

namespace vApus.DistributedTesting
{
    public interface ITileProgressResult
    {
        TimeSpan EstimatedRuntimeLeft { get; }
        Metrics Metrics { get; }
    }
    [Serializable]
    public class TileStresstestProgressResults : ITileProgressResult
    {
        #region Fields
        private Metrics _metrics = new Metrics();
        private TimeSpan _estimatedRuntimeLeft;
        [NonSerialized]
        private StresstestResults _stresstestResults;
        private List<TileConcurrentUsersProgressResult> _tileConcurrentUsersProgressResults = new List<TileConcurrentUsersProgressResult>();
        #endregion

        #region Properties
        public Metrics Metrics
        {
            get { return _metrics; }
        }
        public TimeSpan EstimatedRuntimeLeft
        {
            get { return _estimatedRuntimeLeft; }
        }
        public List<TileConcurrentUsersProgressResult> TileConcurrentUsersProgressResults
        {
            get { return _tileConcurrentUsersProgressResults; }
        }
        #endregion

        #region Constructors
        public TileStresstestProgressResults()
        { }
        /// <summary>
        /// This will start measuring time.
        /// </summary>
        /// <param name="tileStresstest"></param>
        /// <param name="startOfStresstest"></param>
        public TileStresstestProgressResults(StresstestResults stresstestResults)
        {
            _stresstestResults = stresstestResults;
            Refresh();
        }
        #endregion

        public void Refresh()
        {
            _metrics = _stresstestResults.Metrics;
            _estimatedRuntimeLeft = _stresstestResults.EstimatedRuntimeLeft;
            foreach (TileConcurrentUsersProgressResult result in _tileConcurrentUsersProgressResults)
                result.Refresh();
        }
    }
    [Serializable]
    public class TileConcurrentUsersProgressResult : ITileProgressResult
    {
        #region Fields
        private int _concurrentUsers;
        private Metrics _metrics = new Metrics();
        private TimeSpan _estimatedRuntimeLeft;
        [NonSerialized]
        private ConcurrentUsersResult _concurrentUsersResult;
        private List<TilePrecisionProgressResult> _tilePrecisionProgressResults = new List<TilePrecisionProgressResult>();
        #endregion

        #region Properties
        public int ConcurrentUsers
        {
            get { return _concurrentUsers; }
        }
        public Metrics Metrics
        {
            get { return _metrics; }
        }
        public TimeSpan EstimatedRuntimeLeft
        {
            get { return _estimatedRuntimeLeft; }
        }
        public List<TilePrecisionProgressResult> TilePrecisionProgressResults
        {
            get { return _tilePrecisionProgressResults; }
        }
        #endregion

        #region Constructors
        public TileConcurrentUsersProgressResult() { }
        public TileConcurrentUsersProgressResult(ConcurrentUsersResult concurrentUsersResult)
        {
            _concurrentUsersResult = concurrentUsersResult;
            _concurrentUsers = _concurrentUsersResult.ConcurrentUsers;
            Refresh();
        }
        #endregion

        public void Refresh()
        {
            _metrics = _concurrentUsersResult.Metrics;
            _estimatedRuntimeLeft = _concurrentUsersResult.EstimatedRuntimeLeft;
            foreach (TilePrecisionProgressResult result in _tilePrecisionProgressResults)
                result.Refresh();
        }
    }
    [Serializable]
    public class TilePrecisionProgressResult : ITileProgressResult
    {
        #region Fields
        private int _precision;
        private Metrics _metrics = new Metrics();
        private TimeSpan _estimatedRuntimeLeft;
        [NonSerialized]
        private PrecisionResult _precisionResult;
        private List<TileRunProgressResult> _tileRunProgressResults = new List<TileRunProgressResult>();
        #endregion

        #region Properties
        public int Precision
        {
            get { return _precision; }
        }
        public Metrics Metrics
        {
            get { return _metrics; }
        }
        public TimeSpan EstimatedRuntimeLeft
        {
            get { return _estimatedRuntimeLeft; }
        }
        public List<TileRunProgressResult> TileRunProgressResults
        {
            get { return _tileRunProgressResults; }
        }
        #endregion

        #region Constructors
        public TilePrecisionProgressResult() { }
        public TilePrecisionProgressResult(PrecisionResult precisionResult)
        {
            _precisionResult = precisionResult;
            _precision = _precisionResult.Precision;
        }
        #endregion

        public void Refresh()
        {
            _metrics = _precisionResult.Metrics;
            _estimatedRuntimeLeft = _precisionResult.EstimatedRuntimeLeft;
            foreach (TileRunProgressResult result in _tileRunProgressResults)
                result.Refresh();
        }
    }
    [Serializable]
    public class TileRunProgressResult : ITileProgressResult
    {
        #region Fields
        private int _run;
        private Metrics _metrics = new Metrics();
        private TimeSpan _estimatedRuntimeLeft;
        private Dictionary<DateTime, DateTime> _runStartedAndStopped;
        /// <summary>
        /// Set if the run was finished once. (Meaning the result set can not grow anymore (run sync break on last)).
        /// </summary>
        private bool _runDoneOnce;
        [NonSerialized]
        private RunResult _runResult;
        #endregion

        #region Properties
        public int Run
        {
            get { return _run; }
        }
        public Metrics Metrics
        {
            get { return _metrics; }
        }
        public TimeSpan EstimatedRuntimeLeft
        {
            get { return _estimatedRuntimeLeft; }
        }
        /// <summary>
        /// When a run started or restarted (run sync break on last) and stopped.
        /// </summary>
        public Dictionary<DateTime, DateTime> RunStartedAndStopped
        {
            get { return _runStartedAndStopped; }
        }
        /// <summary>
        /// Set if the run was finished once. (Meaning the result set can not grow anymore (run sync break on last)).
        /// </summary>
        public bool RunDoneOnce
        {
            get { return _runDoneOnce; }
        }
        #endregion

        #region Constructors
        public TileRunProgressResult() { }
        public TileRunProgressResult(RunResult runResult)
        {
            _runResult = runResult;
            _run = _runResult.Run;
        }
        #endregion

        public void Refresh()
        {
            _metrics = _runResult.Metrics;
            _estimatedRuntimeLeft = _runResult.EstimatedRuntimeLeft;
            _runStartedAndStopped = _runResult.RunStartedAndStopped;
            _runDoneOnce = _runResult.RunDoneOnce;
        }
    }
}