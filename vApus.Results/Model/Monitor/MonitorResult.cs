/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
namespace vApus.Results
{
    public class MonitorResult
    {
        /// <summary>
        /// PK
        /// </summary>
        public long Id { set; get; }
        /// <summary>
        /// FK
        /// </summary>
        public int MonitorId { get; set; }
        public string Value { get; set; }
    }
}
