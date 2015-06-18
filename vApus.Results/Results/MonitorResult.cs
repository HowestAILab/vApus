/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Collections.Generic;

namespace vApus.Results {
    public class MonitorResult {
        /// <summary>
        ///     Identifier for the metrics (the tostring of the monitor)
        /// </summary>
        public string Monitor { get; set; }
        /// <summary>
        ///     For linking the right results to the right configuration.
        /// </summary>
        public ulong MonitorConfigurationId { get; set; }
        /// <summary>
        ///     Set this when stress testing.
        /// </summary>
        public List<object[]> Rows { get; private set; }
        public string[] Headers { get; set; }

        public MonitorResult() {
            Rows = new List<object[]>();
            Headers = new string[0];
        }
    }
}