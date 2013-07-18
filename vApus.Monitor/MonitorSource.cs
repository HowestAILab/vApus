/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;

namespace vApus.Monitor {
    [Serializable]
    public class MonitorSource {
        private readonly string _source = "<None>";

        public MonitorSource(string source) {
            _source = source;
        }

        public string Source {
            get { return _source; }
        }

        public override string ToString() {
            return _source;
        }
    }
}