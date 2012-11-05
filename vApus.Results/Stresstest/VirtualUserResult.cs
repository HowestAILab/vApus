using System.Collections.Generic;

namespace vApus.Results.Model
{
    public class VirtualUserResult
    {
        public virtual int Id { get; set; }
        public virtual string VirtualUser { get; set; }

        public virtual LogEntryResult[] LogEntryResults { get; set; }

        public VirtualUserResult()
        {
            LogEntryResults = new LogEntryResult[0];
        }
    }
}
