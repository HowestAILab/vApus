/*
 * 2011 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
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