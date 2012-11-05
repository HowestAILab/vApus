using System;
using System.Collections.Generic;

namespace vApus.Results.Model
{
    public class ConcurrencyResult
    {
        public virtual int Id { get; set; }
        public virtual int ConcurrentUsers { get; set; }
        public virtual StresstestConfiguration StresstestConfiguration { get; set; }
        /// <summary>
        /// Set to DateTime.Now by the contructor.
        /// </summary>
        public virtual DateTime ConcurrencyStartedAt { get; set; }

        public virtual List<RunResult> RunResults { get; set; }

        public ConcurrencyResult()
        {
            ConcurrencyStartedAt = DateTime.Now;
            RunResults = new List<RunResult>();
        }
    }
}
