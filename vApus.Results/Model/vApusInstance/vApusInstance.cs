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
    public class vApusInstance
    {
        /// <summary>
        /// PK
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// FK
        /// </summary>
        public DateTime TestTimeStamp { get; set; }
        public string HostName { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public string Version { get; set; } 
        public bool IsMaster { get; set; }

        public List<StresstestConfiguration> Stresstests { get; set; }
        public List<MonitorConfiguration> Monitors { get; set; }

        public vApusInstance()
        {
            Stresstests = new List<StresstestConfiguration>();
            Monitors = new List<MonitorConfiguration>();
        }
    }
}
