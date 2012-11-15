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
    public class MonitorConfiguration
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }
        public string Monitor { get; set; }
        public string ConnectionString { get; set; } // Not a good idea to put credentials in the database
        public string MachineConfiguration { get; set; } // XML in plain text
        public string ResultHeaders { get; set; }

        public List<MonitorResult> MonitorResults { get; set; }

        public MonitorConfiguration()
        {
            MonitorResults = new List<MonitorResult>();
        }
    }
}
