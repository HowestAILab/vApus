/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;

namespace vApus.Results {
    public class ConcurrencyResult {
        #region Properties
        /// <summary>
        /// Only for publisher
        /// </summary>
        public int ConcurrencyId { get; private set; }
        public int Concurrency { get; private set; }

        /// <summary>
        ///     Not in the database, only for the metrics helper.
        /// </summary>
        public int RunCount { get; private set; }

        public DateTime StartedAt { get; internal set; }
        /// <summary>
        /// Set to DateTime.MinValue in the constructor.
        /// </summary>
        public DateTime StoppedAt { get; set; }

        public List<RunResult> RunResults { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// </summary>
        /// <param name="concurrencyId"></param>
        /// <param name="concurrency"></param>
        /// <param name="runCount">Not in the database, only for the metrics helper.</param>
        public ConcurrencyResult(int concurrencyId, int concurrency, int runCount) {
            StartedAt = DateTime.Now;
            StoppedAt = DateTime.MinValue;
            ConcurrencyId = concurrencyId;
            Concurrency = concurrency;
            RunCount = runCount;
            RunResults = new List<RunResult>();
        }
        #endregion
    }
}