using System;
using System.Collections.Generic;

namespace vApus.Results.Model
{
    public class StresstestResult
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// Don't forget to set this.
        /// </summary>
        public virtual StresstestConfiguration StresstestConfiguration { get; set; }
        /// <summary>
        /// If this is not set, it is set to DateTime.Now in the constructor.
        /// </summary>
        public virtual DateTime StartedAt { get; set; }
        /// <summary>
        /// If this is not set, it is set to DateTime.MinValue in the constructor.
        /// </summary>
        public virtual DateTime StoppedAt { get; set; }
        /// <summary>
        /// OK (default), Cancelled, Failed
        /// </summary>
        public virtual string Status { get; set; }
        /// <summary>
        /// Exception on failure for example
        /// </summary>
        public virtual string StatusMessage { get; set; }

        public List<ConcurrencyResult> ConcurrencyResults { get; set; }

        public StresstestResult()
        {
            StartedAt = DateTime.Now;
            StoppedAt = DateTime.MinValue;
            Status = "OK";
            ConcurrencyResults = new List<ConcurrencyResult>();
        }
    }
}
