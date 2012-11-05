using System;
using System.Collections.Generic;

namespace vApus.Results.Model
{
    public class RunResult
    {
        public virtual int Id { get; set; }
        public virtual int Run { get; set; }
        public virtual StresstestConfiguration StresstestConfiguration { get; set; }
        /// <summary>
        /// Set to DateTime.Now by the contructor 
        /// </summary>
        public virtual DateTime RunStartedAt { get; set; }

        public virtual List<VirtualUserResult> VirtualUserResults { get; set; }

        public RunResult()
        {
            RunStartedAt = DateTime.Now;
            VirtualUserResults = new List<VirtualUserResult>();
        }
    }
}
