/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;

namespace vApus.Results.Model
{
    public class RunResult
    {
        public virtual int Id { get; set; }
        public virtual int Run { get; set; }
        /// <summary>
        /// For break on last run sync.
        /// </summary>
        public virtual int RerunCount { get; set; }
        /// <summary>
        /// If this is not set, it is set to DateTime.Now in the constructor.
        /// </summary>
        public virtual DateTime StartedAt { get; set; }
        /// <summary>
        /// If this is not set, it is set to DateTime.MinValue in the constructor.
        /// </summary>
        public virtual DateTime StoppedAt { get; set; }

        /// <summary>
        /// Dont forget to set this.
        /// </summary>
        public virtual VirtualUserResult[] VirtualUserResults { get; set; }

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
