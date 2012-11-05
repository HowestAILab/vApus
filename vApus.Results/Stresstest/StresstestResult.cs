using System;
using System.Collections.Generic;

namespace vApus.Results.Model
{
    public class StresstestResult
    {
        public virtual int Id { get; set; }
        public virtual StresstestConfiguration StresstestConfiguration { get; set; }
        /// <summary>
        /// Set to DateTime.Now by the contructor.
        /// </summary>
        public virtual DateTime StresstestStartedAt { get; set; }
        public virtual string Status { get; set; }                  // OK, Cancelled, Failed
        public virtual string StatusMessage { get; set; }           // Exception on failure for example

        public List<ConcurrencyResult> ConcurrencyResults { get; set; }

        public StresstestResult()
        {
            StresstestStartedAt = DateTime.Now;
            ConcurrencyResults = new List<ConcurrencyResult>();
        }
    }
}
