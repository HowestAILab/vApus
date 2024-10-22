﻿/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 *    
 * Used for regular and distributed testing.
 */
using System;
using System.ComponentModel;

namespace vApus.StressTest {
    /// <summary></summary>
    [Serializable]
    public enum StressTestStatus {
        Busy = 0,
        Ok = 1,
        Cancelled = 2,
        Error = 3
    }

    [Serializable]
    public enum RunSynchronization {
        None = 0,
        [Description("Break on first finished")]
        BreakOnFirstFinished,
        [Description("Break on last finished")]
        BreakOnLastFinished
    }

    [Serializable]
    public enum RunStateChange {
        None = 0,
        ToRunInitializedFirstTime = 1,
        ToRunDoneOnce = 2,
        ToRerunDone = 3
    }
}