/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;

namespace vApus.Results {
    public class StresstestResult {
        public StresstestResult() {
            StartedAt = DateTime.Now;
            StoppedAt = DateTime.MinValue;
            ConcurrencyResults = new List<ConcurrencyResult>();
        }

        public DateTime StartedAt { get; private set; }
        public DateTime StoppedAt { get; internal set; }

        public List<ConcurrencyResult> ConcurrencyResults { get; private set; }

        //public TimeSpan EstimatedRuntimeLeft {
        //    get {
        //        long estimatedRuntimeLeft = 0;
        //        if (StoppedAt != DateTime.MinValue) //For run sync first this must be 0.
        //        {
        //            //If the run is broken it is visible in the run results, so we need to correct this here for calculating the runtime left.
        //            ulong totalLogEntriesProcessedWorkAround = 0;
        //            foreach (var cr in ConcurrencyResults)
        //                foreach (var rr in cr.RunResults) {
        //                }   // totalLogEntriesProcessedWorkAround += rr.StoppedAt == DateTime.MinValue ? rr. // rr.Metrics.TotalLogEntriesProcessed : rr.Metrics.TotalLogEntries;

        //            estimatedRuntimeLeft = (long)(((DateTime.Now - StartedAt).TotalMilliseconds / _metrics.TotalLogEntriesProcessed) *
        //                (_metrics.TotalLogEntries - totalLogEntriesProcessedWorkAround) * 10000);
        //            if (estimatedRuntimeLeft < 0) estimatedRuntimeLeft = 0;
        //        }
        //        return new TimeSpan(estimatedRuntimeLeft);
        //    }
        //}
    }
}