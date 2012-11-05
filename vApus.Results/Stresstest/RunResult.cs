using System;
using System.Collections.Generic;

namespace vApus.Results.Model
{
    public class RunResult
    {
        public virtual int Id { get; set; }
        public virtual int Run { get; set; }
        /// <summary>
        /// If this is not set, it is set to DateTime.Now in the constructor.
        /// </summary>
        public virtual DateTime StartedAt { get; set; }
        /// <summary>
        /// If this is not set, it is set to DateTime.MinValue in the constructor.
        /// </summary>
        public virtual DateTime StoppedAt { get; set; }

        public virtual VirtualUserResult[] VirtualUserResults { get; set; }

        public RunResult()
        {
            StartedAt = DateTime.Now;
            StoppedAt = DateTime.MinValue;
            VirtualUserResults = new VirtualUserResult[0];
        }
    }
}
