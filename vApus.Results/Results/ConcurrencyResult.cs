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
    public class ConcurrencyResult {
        #region Properties
        public int Concurrency { get; private set; }

        /// <summary>
        ///     Not in the database, only for the metrics helper.
        /// </summary>
        public int RunCount { get; private set; }

        public DateTime StartedAt { get; internal set; }
        /// <summary>
        /// Set to DateTime.MinValue in the constructor.
        /// </summary>
        public DateTime StoppedAt { get; set; }

        public List<RunResult> RunResults { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// </summary>
        /// <param name="concurrentUsers"></param>
        /// <param name="runCount">Not in the database, only for the metrics helper.</param>
        public ConcurrencyResult(int concurrentUsers, int runCount) {
            StartedAt = DateTime.Now;
            StoppedAt = DateTime.MinValue;
            Concurrency = concurrentUsers;
            RunCount = runCount;
            RunResults = new List<RunResult>();
        }
        #endregion
    }
}