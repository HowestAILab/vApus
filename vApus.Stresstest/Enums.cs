/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;

namespace vApus.Stresstest
{
    /// <summary></summary>
    [Serializable]
    public enum StresstestStatus
    {
        Busy = 0,
        Ok,
        Error,
        Cancelled
    }

    [Serializable]
    public enum UserActionDistribution
    {
        None = 0,
        Fast = 1,
        Full = 2
    }

    [Serializable]
    public enum RunSynchronization
    {
        None = 0,
        [Description("Break on first finished")] BreakOnFirstFinished,
        [Description("Break on last finished")] BreakOnLastFinished
    }

    [Serializable]
    public enum RunStateChange
    {
        None = 0,
        ToRunInitializedFirstTime,
        ToRunDoneOnce
    }
}