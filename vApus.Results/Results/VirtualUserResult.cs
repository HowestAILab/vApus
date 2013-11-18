/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
namespace vApus.Results {
    public struct VirtualUserResult {

        #region Fields
        private string _virtualUser;
        /// <summary>
        ///     For break on last runsync.
        /// </summary>
        private long _runOffset;
        /// <summary>
        ///     For break on last runsync.
        /// </summary>
        private long _baseLogEntryCount;

        private LogEntryResult[] _logEntryResults;
        #endregion

        #region Properties
        /// <summary>
        ///     When not entered in the test this remains null. This is set in the StresstestCore.
        /// </summary>
        public string VirtualUser {
            get { return _virtualUser; }
            set { _virtualUser = value; }
        }

        /// <summary>
        ///     Use the SetLogEntryResultAt function to add an item to this. (this fixes the index when using break on last run sync.)
        ///     Don't forget to initialize this the first time.
        ///     Can contain null!
        /// </summary>
        public LogEntryResult[] LogEntryResults {
            get { return _logEntryResults; }
            internal set { _logEntryResults = value; }
        }

        #endregion

        #region Constructor
        public VirtualUserResult(int logLength) {
            _virtualUser = null;
            _runOffset = 0;
            _logEntryResults = new LogEntryResult[logLength];
            _baseLogEntryCount = _logEntryResults.LongLength;
        }
        #endregion

        #region Functions
        public void SetLogEntryResultAt(int index, LogEntryResult result) {
            LogEntryResults[_runOffset + index] = result;
        }

        /// <summary>
        ///     For break on last run sync. should only be used in the RunResult class.
        /// </summary>
        internal VirtualUserResult CloneAndPrepareForRerun() {
            var clone = new VirtualUserResult();
            clone._virtualUser = _virtualUser;
            clone._baseLogEntryCount = _baseLogEntryCount;
            clone._runOffset = _runOffset + _baseLogEntryCount;

            var increasedLogEntryResults = new LogEntryResult[LogEntryResults.LongLength + _baseLogEntryCount];
            _logEntryResults.CopyTo(increasedLogEntryResults, 0);

            clone._logEntryResults = increasedLogEntryResults;

            return clone;
        }
        #endregion
    }
}