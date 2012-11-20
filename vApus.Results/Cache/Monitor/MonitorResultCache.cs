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
        /// Set this when stresstesting.
        /// </summary>
        public int MonitorConfigurationId { get; set; }
        public List<object[]> Rows { get; private set; }
        public MonitorResultCache()
        {
            MonitorConfigurationId = -1;
            Rows = new List<object[]>();
        }
    }
}
