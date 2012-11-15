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
        public int Id { get; set; }
        /// <summary>
        /// Index in Log
        /// </summary>
        public string LogEntryIndex { get; set; }
        public string LogEntryString { get; set; }
        public string UserAction { get; set; }
        public int UserActionIndex { get; set; }
        public DateTime SentAt { get; set; }
        public TimeSpan TimeToLastByte { get; set; }
        public int DelayInMilliseconds { get; set; }
        public Exception Exception { get; set; }
    }
}
