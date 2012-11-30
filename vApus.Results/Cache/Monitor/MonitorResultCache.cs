/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System.Collections.Generic;

namespace vApus.Results
{
    public class MonitorResultCache
    {
        /// <summary>
        ///     Identifier for the metrics (the tostring of the monitor)
        /// </summary>
        public string Monitor { get; set; }
        /// <summary>
        ///     For linking the right results to the right configuration.
        /// </summary>
        public int MonitorConfigurationId { get; set; }

        /// <summary>
        ///     Set this when stresstesting.
        /// </summary>
        public List<object[]> Rows { get; private set; }
        public string[] Headers { get; set; }

        public MonitorResultCache()
        {
            MonitorConfigurationId = -1;
            Rows = new List<object[]>();
            Headers = new string[0];
        }
    }
}