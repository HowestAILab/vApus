/*
 * Copyright 2014 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

namespace vApus.Monitor {
    internal static class DefaultWIWs {
        private static string _dstat = "[{\"name\":\"FOO\",\"isAvailable\":true,\"subs\":[{\"name\":\"procs\",\"subs\":[{\"name\":\"run\"},{\"name\":\"blk\"},{\"name\":\"new\"}]},{\"name\":\"memory usage\",\"subs\":[{\"name\":\"used\"},{\"name\":\"buff\"},{\"name\":\"cach\"},{\"name\":\"free\"}]},{\"name\":\"paging\",\"subs\":[{\"name\":\"in\"},{\"name\":\"out\"}]},{\"name\":\"dsk/total\",\"subs\":[{\"name\":\"read\"},{\"name\":\"writ\"}]},{\"name\":\"system\",\"subs\":[{\"name\":\"int\"},{\"name\":\"csw\"}]},{\"name\":\"total cpu usage\",\"subs\":[{\"name\":\"usr\"},{\"name\":\"sys\"},{\"name\":\"idl\"},{\"name\":\"wai\"},{\"name\":\"hiq\"},{\"name\":\"siq\"}]},{\"name\":\"net/total\",\"subs\":[{\"name\":\"recv\"},{\"name\":\"send\"}]}]}]";
        private static string _wmi = "[{\"name\":\"FOO\",\"isAvailable\":true,\"subs\":[{\"name\":\"Memory.Available Bytes\",\"counter\":\"System.Collections.Generic.List`1[System.String]\",\"subs\":[{\"name\":\"__Total__\"}]},{\"name\":\"Memory.Cache Bytes\",\"counter\":\"System.Collections.Generic.List`1[System.String]\",\"subs\":[{\"name\":\"__Total__\"}]},{\"name\":\"PhysicalDisk.Avg. Disk Queue Length\",\"counter\":\"System.Collections.Generic.List`1[System.String]\",\"subs\":[{\"name\":\"_Total\"}]},{\"name\":\"Processor Information.% Idle Time\",\"counter\":\"System.Collections.Generic.List`1[System.String]\",\"subs\":[{\"name\":\"_Total\"}]},{\"name\":\"Processor Information.% Interrupt Time\",\"counter\":\"System.Collections.Generic.List`1[System.String]\",\"subs\":[{\"name\":\"_Total\"}]},{\"name\":\"Processor Information.% Privileged Time\",\"counter\":\"System.Collections.Generic.List`1[System.String]\",\"subs\":[{\"name\":\"_Total\"}]},{\"name\":\"Processor Information.% User Time\",\"counter\":\"System.Collections.Generic.List`1[System.String]\",\"subs\":[{\"name\":\"_Total\"}]},{\"name\":\"PhysicalDisk.Disk Read Bytes/sec\",\"subs\":[{\"name\":\"_Total\"}]},{\"name\":\"PhysicalDisk.Disk Write Bytes/sec\",\"subs\":[{\"name\":\"_Total\"}]}]}]";
        /// <summary>
        /// Returns the default to monitor counters for a given monitor source.
        /// </summary>
        /// <param name="monitorSource"></param>
        /// <returns></returns>
        public static string Get(string monitorSource) {
            if (monitorSource == "Dstat Agent")
                return _dstat;
            if (monitorSource == "WMI Agent" || monitorSource == "Local WMI")
                return _wmi;
            return null;
        }
    }
}
