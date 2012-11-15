/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
namespace vApus.Results
{
    public class vApusInstancesMonitors
    {
        /// <summary>
        /// PK
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// FK
        /// </summary>
        public int vApusInstanceId { get; set; }
        public int MonitorId { get; set; }
    }
}
