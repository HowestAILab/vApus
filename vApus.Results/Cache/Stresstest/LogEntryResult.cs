/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;

namespace vApus.Results {
    public class LogEntryResult {
        public string VirtualUser { get; set; }
        public string UserAction { get; set; }

        /// <summary>
        ///     Index in Log
        /// </summary>
        public string LogEntryIndex { get; set; }
        /// <summary>
        ///     To be able to calcullate averages when using distribute.
        /// </summary>
        public string SameAsLogEntryIndex { get; set; }

        public string LogEntry { get; set; }
        public DateTime SentAt { get; set; }
        public long TimeToLastByteInTicks { get; set; }
        public int DelayInMilliseconds { get; set; }
        public string Error { get; set; }

        /// <summary>
        /// 0 for all but break on last runs.
        /// </summary>
        public int Rerun { get; set; }
    }
}