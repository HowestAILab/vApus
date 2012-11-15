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
    /// <summary>
    /// The entry class for saving results, one should always start here.
    /// </summary>
    public class Test
    {
        /// <summary>
        /// PK
        /// The constructor sets this to DateTime.Now
        /// </summary>
        public DateTime TimeStamp { get; set; }

        public List<vApusInstance> vApusInstances { get; set; }

        public Test() 
        {
            TimeStamp = DateTime.Now;
            vApusInstances = new List<vApusInstance>();
        }
    }
}
