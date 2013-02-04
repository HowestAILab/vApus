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
        ///     For break on last runsync.
        /// </summary>
        private readonly long _baseLogEntryCount;

        public long BaseLogEntryCount {
            get { return _baseLogEntryCount; }
        } 

        /// <summary>
        ///     For break on last runsync.
        /// </summary>
        private long _runOffset;

        public VirtualUserResult(int logLength)
        {
            LogEntryResults = new LogEntryResult[logLength];
            _baseLogEntryCount = LogEntryResults.LongLength;
        }

        /// <summary>
        ///     When not entered in the test this remains null.
        /// </summary>
        public string VirtualUser { get; set; }

        /// <summary>
        ///     Use the SetLogEntryResultAt function to add an item to this. (this fixes the index when using break on last run sync.)
        ///     Don't forget to initialize this the first time.
        ///     Can contain null!
        /// </summary>
        public LogEntryResult[] LogEntryResults { get; internal set; }

        public void SetLogEntryResultAt(int index, LogEntryResult result)
        {
            LogEntryResults[_runOffset + index] = result;
        }

        /// <summary>
        ///     For break on last run sync. should only be used in the RunResult class.
        /// </summary>
        public void PrepareForRerun()
        {
            _runOffset += _baseLogEntryCount;

            var increasedLogEntryResults = new LogEntryResult[LogEntryResults.LongLength + _baseLogEntryCount];
            for (long l = 0; l != LogEntryResults.LongLength; l++)
                increasedLogEntryResults[l] = LogEntryResults[l];

            for (long l = LogEntryResults.LongLength; l != increasedLogEntryResults.LongLength; l++)
                increasedLogEntryResults[l] = new LogEntryResult();

            LogEntryResults = increasedLogEntryResults;
        }
    }
}