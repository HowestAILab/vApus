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
    public class StresstestConfiguration
    {
        /// <summary>
        /// PK
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// FK
        /// </summary>
        public int vApusInstanceId { get; set; }
        public string Stresstest { get; set; }
        public string RunSynchronization { get; set; }
        public string Connection { get; set; }
        public string ConnectionProxy { get; set; }
        public string ConnectionString { get; set; }        // Not a good idea to put credentials in the database
        public string Log { get; set; }
        public string LogRuleSet { get; set; }
        public string Concurrency { get; set; }
        public int Runs { get; set; }
        public int MinimumDelay { get; set; }
        public int MaximumDelay { get; set; }
        public bool Shuffle { get; set; }
        public string Distribute { get; set; }
        public int MonitorBefore { get; set; }
        public int MonitorAfter { get; set; }
        //public bool UseParallelExecutionOfLogEntries { get; set; }

        public StresstestResult StresstestResult { get; set; }
    }
}
