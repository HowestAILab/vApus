/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;

namespace vApus.Results
{
    public class RunResult
    {
        public int Id { get; set; }
        public int Run { get; set; }
        /// <summary>
        /// For break on last run sync.
        /// </summary>
        public int RerunCount { get; set; }
        /// <summary>
        /// If this is not set, it is set to DateTime.Now in the constructor.
        /// </summary>
        public DateTime StartedAt { get; set; }
        /// <summary>
        /// If this is not set, it is set to DateTime.MinValue in the constructor.
        /// </summary>
        public DateTime StoppedAt { get; set; }

        /// <summary>
        /// Dont forget to set this.
        /// </summary>
        public VirtualUserResult[] VirtualUserResults { get; set; }

        public RunResult()
        {
            StartedAt = DateTime.Now;
            StoppedAt = DateTime.MinValue;
            VirtualUserResults = new VirtualUserResult[0];
        }
        /// <summary>
        /// Only used for break on last run ync.
        /// </summary>
        public void PrepareForRerun()
        {
            ++RerunCount;

            foreach (VirtualUserResult result in VirtualUserResults)
                result.PrepareForRerun();
        }
    }
}
