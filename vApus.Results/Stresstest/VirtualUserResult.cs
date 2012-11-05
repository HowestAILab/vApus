using System.Collections.Generic;

namespace vApus.Results.Model
{
    public class VirtualUserResult
    {
        public virtual int Id { get; set; }
        public virtual string VirtualUser { get; set; }

        /// <summary>
        /// Use the SetLogEntryResultAt fx to add an item to this.
        /// Can contain null!
        /// </summary>
        public virtual LogEntryResult[] LogEntryResults { get; set; }

        public VirtualUserResult()
        {
            LogEntryResults = new LogEntryResult[0];
        }

        public void SetLogEntryResultAt(int index, LogEntryResult result)
        {
            //LogEntryResults[_runOffset + index] = result;
            LogEntryResults[index] = result;
        }
    }
}
