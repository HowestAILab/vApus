/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;

namespace vApus.Monitor {
    /// <summary>
    /// For instance WMI, ESXi, Windows agent, Dstat agent, racktivity power meter, raritan power meter.
    /// </summary>
    [Serializable]
    public class MonitorSource {
        private readonly string _source = "<None>";

        public string Source { get { return _source; } }

        public MonitorSource(string source) { _source = source; }

        public override string ToString() { return _source; }
    }
}