/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

namespace vApus.Results
{
    public class VirtualUserResult
    {
        /// <summary>
        /// For break on last runsync.
        /// </summary>
        private long _runOffset, _baseLogEntryCount;

        public int Id { get; set; }
        /// <summary>
        /// When not entered in the test this remains null.
        /// </summary>
        public string VirtualUser { get; set; }

        public int UserActionCount { get; set; }

        /// <summary>
        /// Use the SetLogEntryResultAt function to add an item to this. (this fixes the index when using break on last run sync.)
        /// Don't forget to initialize this the first time.
        /// Can contain null!
        /// </summary>
        public LogEntryResult[] LogEntryResults { get; set; }

        public VirtualUserResult()
        {
            LogEntryResults = new LogEntryResult[0];
        }

        public void SetLogEntryResultAt(int index, LogEntryResult result)
        {
            LogEntryResults[_runOffset + index] = result;
        }
        /// <summary>
        /// For break on last run sync. should only be used in the RunResult class.
        /// </summary>
        public void PrepareForRerun()
        {
            if (_baseLogEntryCount == 0)
                _baseLogEntryCount = LogEntryResults.LongLength;
            _runOffset += _baseLogEntryCount;

            LogEntryResult[] increasedLogEntryResults = new LogEntryResult[LogEntryResults.LongLength + _baseLogEntryCount];
            for (long l = 0; l != LogEntryResults.LongLength; l++)
                increasedLogEntryResults[l] = LogEntryResults[l];

            for (long l = LogEntryResults.LongLength; l != increasedLogEntryResults.LongLength; l++)
                increasedLogEntryResults[l] = new LogEntryResult();

            LogEntryResults = increasedLogEntryResults;
        }
    }
}
