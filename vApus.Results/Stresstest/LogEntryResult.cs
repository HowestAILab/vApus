using System;

namespace vApus.Results.Model
{
    public class LogEntryResult
    {
        public bool Empty;

        public virtual int Id { get; set; }
        /// <summary>
        /// Index in Log
        /// </summary>
        public virtual string LogEntryIndex { get; set; }
        public virtual string LogEntryString { get; set; }
        public virtual string UserAction { get; set; }
        public virtual int UserActionIndex { get; set; }
        public virtual DateTime SentAt { get; set; }
        public virtual TimeSpan TimeToLastByte { get; set; }
        public virtual int DelayInMilliseconds { get; set; }
        public virtual Exception Exception { get; set; }

    }
}
