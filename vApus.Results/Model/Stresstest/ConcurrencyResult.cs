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
        public int Id { get; set; }
        public int ConcurrentUsers { get; set; }
        public int RunCount { get; set; }
        /// <summary>
        /// If this is not set, it is set to DateTime.Now in the constructor.
        /// </summary>
        public DateTime StartedAt { get; set; }
        /// <summary>
        /// If this is not set, it is set to DateTime.MinValue in the constructor.
        /// </summary>
        public DateTime StoppedAt { get; set; }

        public List<RunResult> RunResults { get; set; }

        public ConcurrencyResult()
        {
            RunCount = 1;
            StartedAt = DateTime.Now;
            StoppedAt = DateTime.MinValue;
            RunResults = new List<RunResult>();
        }
    }
}
