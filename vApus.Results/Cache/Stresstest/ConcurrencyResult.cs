/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;

namespace vApus.Results
{
    public class ConcurrencyResult
    {
        public int ConcurrentUsers { get; private set; }
        /// <summary>
        /// Not in the database, only for the metrics helper.
        /// </summary>
        public int RunCount { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime StoppedAt { get; internal set; }

        public List<RunResult> RunResults { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="concurrentUsers"></param>
        /// <param name="runCount">Not in the database, only for the metrics helper.</param>
        public ConcurrencyResult(int concurrentUsers, int runCount)
        {
            StartedAt = DateTime.Now;
            StoppedAt = DateTime.MinValue;
            ConcurrentUsers = concurrentUsers;
            RunCount = runCount;
            RunResults = new List<RunResult>();
        }
    }
}
