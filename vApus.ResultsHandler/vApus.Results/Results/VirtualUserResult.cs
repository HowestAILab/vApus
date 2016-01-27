/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
namespace vApus.Results {
    public class VirtualUserResult {

        #region Fields
        private string _virtualUser;
        /// <summary>
        ///     For break on last runsync.
        /// </summary>
        private long _runOffset;
        /// <summary>
        ///     For break on last runsync.
        /// </summary>
        private long _baseRequestCount;

        private RequestResult[] _requestResults;
        #endregion

        #region Properties
        /// <summary>
        ///     When not entered in the test this remains null. This is set in the StressTestCore.
        /// </summary>
        public string VirtualUser {
            get { return _virtualUser; }
            set { _virtualUser = value; }
        }

        /// <summary>
        ///     Use the SetRequestResultAt function to add an item to this. (this fixes the index when using break on last run sync.)
        ///     Don't forget to initialize this the first time.
        ///     Can contain null!
        /// </summary>
        public RequestResult[] RequestResults {
            get { return _requestResults; }
            internal set { _requestResults = value; }
        }

        #endregion

        #region Constructor
        public VirtualUserResult(int logLength) {
            _virtualUser = null;
            _runOffset = 0;
            _requestResults = new RequestResult[logLength];
            _baseRequestCount = _requestResults.LongLength;
        }
        #endregion

        #region Functions
        public void SetRequestResultAt(int index, RequestResult result) {
            RequestResults[_runOffset + index] = result;
        }

        /// <summary>
        ///     For break on last run sync. should only be used in the RunResult class.
        /// </summary>
        internal void PrepareForRerun() {
            _runOffset += _baseRequestCount;

            var increasedRequestResults = new RequestResult[RequestResults.LongLength + _baseRequestCount];
            _requestResults.CopyTo(increasedRequestResults, 0);

            long previousLength = RequestResults.LongLength;
            _requestResults = increasedRequestResults;
            for (long l = previousLength; l != _requestResults.LongLength; l++)
                _requestResults[l] = new RequestResult();
        }
        #endregion
    }
}