/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Collections.Generic;

namespace vApus.Results.Model
{
    public class StresstestConfiguration
    {
        public virtual int Id { get; set; }
        public virtual string Stresstest { get; set; }
        public virtual string RunSynchronization { get; set; }
        public virtual string Connection { get; set; }
        public virtual string ConnectionProxy { get; set; }
        public virtual string ConnectionString { get; set; }        // Not a good idea to put credentials in the database
        public virtual string Log { get; set; }
        public virtual string LogRuleSet { get; set; }
        public virtual List<MonitorConfiguration> Monitors { get; set; }
        public virtual string Concurrency { get; set; }
        public virtual int Runs { get; set; }
        public virtual int MinimumDelay { get; set; }
        public virtual int MaximumDelay { get; set; }
        public virtual bool Shuffle { get; set; }
        public virtual string Distribute { get; set; }
        public virtual int MonitorBefore { get; set; }
        public virtual int MonitorAfter { get; set; }
        //public virtual bool UseParallelExecutionOfLogEntries { get; set; }

        public StresstestConfiguration()
        {
            Monitors = new List<MonitorConfiguration>();
        }
    }
}
