/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;

namespace vApus.Results
{
    public class LogEntryResult
    {
        public string VirtualUser { get; set; }
        public string UserAction { get; set; }
        /// <summary>
        /// Index in Log
        /// </summary>
        public string LogEntryIndex { get; set; }
        public string LogEntry { get; set; }
        public DateTime SentAt { get; set; }
        public long TimeToLastByteInTicks { get; set; }
        public int DelayInMilliseconds { get; set; }
        public string Exception { get; set; }
    }
}
