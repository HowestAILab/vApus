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
    public class MonitorConfiguration
    {
        public virtual int Id { get; set; }
        public virtual string Monitor { get; set; }
        public virtual string ConnectionString { get; set; } // Not a good idea to put credentials in the database
        public virtual string MachineConfiguration { get; set; } // XML in plain text
        public virtual string ResultHeaders { get; set; }

        public virtual List<MonitorResult> MonitorResults { get; set; }

        public MonitorConfiguration()
        {
            MonitorResults = new List<MonitorResult>();
        }
    }
}
