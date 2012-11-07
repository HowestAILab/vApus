using System;
using System.Collections.Generic;

namespace vApus.Results.Model
{
    public class ConcurrencyResult
    {
        public virtual int Id { get; set; }
        public virtual int ConcurrentUsers { get; set; }
        public virtual int RunCount { get; set; }
        /// <summary>
        /// If this is not set, it is set to DateTime.Now in the constructor.
        /// </summary>
        public virtual DateTime StartedAt { get; set; }
        /// <summary>
        /// If this is not set, it is set to DateTime.MinValue in the constructor.
        /// </summary>
        public virtual DateTime StoppedAt { get; set; }

        public virtual List<RunResult> RunResults { get; set; }

        public ConcurrencyResult()
        {
            RunCount = 1;
            StartedAt = DateTime.Now;
            StoppedAt = DateTime.MinValue;
            RunResults = new List<RunResult>();
        }
    }
}
