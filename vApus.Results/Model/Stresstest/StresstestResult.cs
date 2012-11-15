/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;

namespace vApus.Results
{
    public class StresstestResult
    {
        /// <summary>
        /// PK
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// FK
        /// </summary>
        public int StresstestID { get; set; }
        /// <summary>
        /// If this is not set, it is set to DateTime.Now in the constructor.
        /// </summary>
        public DateTime StartedAt { get; set; }
        /// <summary>
        /// If this is not set, it is set to DateTime.MinValue in the constructor.
        /// </summary>
        public DateTime StoppedAt { get; set; }
        /// <summary>
        /// OK (default), Cancelled, Failed
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Exception on failure for example
        /// </summary>
        public string StatusMessage { get; set; }

        public List<ConcurrencyResult> ConcurrencyResults { get; set; }

        public StresstestResult()
        {
            StartedAt = DateTime.Now;
            StoppedAt = DateTime.MinValue;
            Status = "OK";
            ConcurrencyResults = new List<ConcurrencyResult>();
        }
    }
}
