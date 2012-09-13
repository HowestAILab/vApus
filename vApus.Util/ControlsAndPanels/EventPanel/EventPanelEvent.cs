/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Drawing;

namespace vApus.Util
{
    [Serializable]
    public struct EventPanelEvent
    {
        public EventViewEventType EventType;
        public Color EventProgressBarEventColor;
        public string Message;
        public DateTime At;

        public EventPanelEvent(EventViewEventType eventType, Color eventProgressBarEventColor, string message, DateTime at)
        {
            EventType = eventType;
            EventProgressBarEventColor = eventProgressBarEventColor;
            Message = message;
            At = at;
        }
    }
}
