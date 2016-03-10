/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Drawing;

namespace vApus.Util {
    [Serializable]
    public struct EventPanelEvent {
        public DateTime At;
        public Color EventProgressBarEventColor;
        public EventViewEventType EventType;
        public string Message;

        public override string ToString() {
            return At.ToString("yyyy'-'MM'-'dd HH':'mm':'ss") + " " + EventType + " " + Message;
        }
    }
}