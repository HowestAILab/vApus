/*
 * 2011 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using vApus.Monitor.Sources.Base;

namespace vApus.Monitor {
    /// <summary>
    /// For instance WMI, ESXi, Windows agent, Dstat agent, racktivity power meter, raritan power meter.
    /// </summary>
    [Serializable]
    public class MonitorSourceClient {
        private readonly string _name = "<None>";
        private readonly Type _type;

        public string Source { get { return _name; } }

        public Type Type { get { return _type; } }

        public MonitorSourceClient() { }
        public MonitorSourceClient(string name, Type type) {
            _name = name;
            if (_name.EndsWith(" Client")) _name = _name.Substring(0, _name.Length - " Client".Length);
            _type = type;
        }

        public override string ToString() { return _name; }
    }
}