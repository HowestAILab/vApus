/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;

namespace vApus.Util {
    [Serializable]
    public enum EventViewEventType {
        Info = 0,
        Warning,
        Error
    }
}