/*
 * Copyright 2014 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using Newtonsoft.Json;
using vApus.Monitor.Sources.Base;
namespace vApus.Monitor {
    internal static class DefaultWIWs {
        private static string _allAvailable = "{\"timestamp\":0,\"subs\":[{\"name\":\"*\",\"isAvailable\":true}]}";

        private static string _dstat = "{\"timestamp\":0,\"subs\":[{\"name\":\"FOO\",\"isAvailable\":true,\"subs\":[{\"name\":\"procs\",\"subs\":[{\"name\":\"*\"}]},{\"name\":\"memory usage\",\"subs\":[{\"name\":\"*\"}]},{\"name\":\"paging\",\"subs\":[{\"name\":\"*\"}]},{\"name\":\"dsk/total\",\"subs\":[{\"name\":\"*\"}]},{\"name\":\"system\",\"subs\":[{\"name\":\"*\"}]},{\"name\":\"total cpu usage\",\"subs\":[{\"name\":\"*\"}]},{\"name\":\"net/total\",\"subs\":[{\"name\":\"*\"}]}]}]}";
        private static string _wmi = "{\"timestamp\":0,\"subs\":[{\"name\":\"FOO\",\"isAvailable\":true,\"subs\":[{\"name\":\"Memory.Available Bytes\",\"counter\":\"System.Collections.Generic.List`1[System.String]\",\"subs\":[{\"name\":\"__Total__\"}]},{\"name\":\"Memory.Cache Bytes\",\"counter\":\"System.Collections.Generic.List`1[System.String]\",\"subs\":[{\"name\":\"__Total__\"}]},{\"name\":\"PhysicalDisk.Avg. Disk Queue Length\",\"counter\":\"System.Collections.Generic.List`1[System.String]\",\"subs\":[{\"name\":\"_Total\"}]},{\"name\":\"Processor Information.% Idle Time\",\"counter\":\"System.Collections.Generic.List`1[System.String]\",\"subs\":[{\"name\":\"_Total\"}]},{\"name\":\"Processor Information.% Privileged Time\",\"counter\":\"System.Collections.Generic.List`1[System.String]\",\"subs\":[{\"name\":\"_Total\"}]},{\"name\":\"Processor Information.% User Time\",\"counter\":\"System.Collections.Generic.List`1[System.String]\",\"subs\":[{\"name\":\"_Total\"}]},{\"name\":\"PhysicalDisk.Disk Read Bytes/sec\",\"subs\":[{\"name\":\"_Total\"}]},{\"name\":\"PhysicalDisk.Disk Write Bytes/sec\",\"subs\":[{\"name\":\"_Total\"}]},{\"name\":\"Network Interface.Bytes Received/sec\",\"subs\":[{\"name\":\"*\"}]},{\"name\":\"Network Interface.Bytes Sent/sec\",\"subs\":[{\"name\":\"*\"}]}]}]}";
        private static string _esxi = "{\"timestamp\":0,\"subs\":[{\"name\":\"FOO\",\"isAvailable\":true,\"subs\":[{\"name\":\"cpu.idle.summation (millisecond)\",\"subs\":[{\"name\":\"*\"}]},{\"name\":\"cpu.usage.average (percent)\",\"subs\":[{\"name\":\"*\"}]},{\"name\":\"cpu.wait.summation (millisecond)\",\"subs\":[{\"name\":\"*\"}]},{\"name\":\"disk.deviceLatency.average (millisecond)\",\"subs\":[{\"name\":\"*\"}]},{\"name\":\"disk.queueLatency.average (millisecond)\",\"subs\":[{\"name\":\"*\"}]},{\"name\":\"mem.consumed.average (kiloBytes)\",\"subs\":[{\"name\":\"0\"}]},{\"name\":\"mem.usage.average (percent)\",\"subs\":[{\"name\":\"0\"}]},{\"name\":\"power.power.average (watt)\",\"subs\":[{\"name\":\"0\"}]},{\"name\":\"mem.active.average (kiloBytes)\",\"subs\":[{\"name\":\"0\"}]}]}]}";
        private static string _sigar = "{\"timestamp\":0,\"subs\":[{\"name\":\"FOO\",\"isAvailable\":true,\"subs\":[{\"name\":\"cpu\",\"subs\":[{\"name\":\"_user (%)\"},{\"name\":\"_system (%)\"},{\"name\":\"_idle (%)\"},{\"name\":\"_wait (%)\"}]},{\"name\":\"memory\",\"subs\":[{\"name\":\"used (MB)\"},{\"name\":\"free (MB)\"},{\"name\":\"cache + buffer (MB)\"}]},{\"name\":\"swap\",\"subs\":[{\"name\":\"used (MB)\"},{\"name\":\"free (MB)\"}]},{\"name\":\"disk\",\"subs\":[{\"name\":\"*\"}]},{\"name\":\"nic\",\"subs\":[{\"name\":\"*\"}]}]}]}";

        /// <summary>
        /// Returns the default to monitor counters for a given monitor source.
        /// </summary>
        /// <param name="monitorSource"></param>
        /// <returns></returns>
        private static Entities Get(string monitorSource) {
            string defaultWIW = null;

            switch (monitorSource) {
                case "Dstat Agent":
                    defaultWIW = _dstat;
                    break;
                case "Local WMI":
                case "WMI Agent":
                    defaultWIW = _wmi;
                    break;
                case "ESXi":
                    defaultWIW = _esxi;
                    break;
                case "Sigar Agent":
                    defaultWIW = _sigar;
                    break;
                default:
                    defaultWIW = _allAvailable;
                    break;
            }

            if (defaultWIW == null) return null;

            return JsonConvert.DeserializeObject<Entities>(defaultWIW);
        }

        /// <summary>
        /// Set the default counters to the wiw of the given monito, if available. Only applicable for available entities.
        /// </summary>
        /// <param name="monitor"></param>
        /// <param name="wdyh"></param>
        public static void Set(Monitor monitor, Entities wdyh) {
            Entities defaultWIW = Get(monitor.MonitorSource.ToString());
            if (defaultWIW != null) {
                var wiw = new Entities();

                //Add all available entities from wdyh.
                if (defaultWIW.GetSubs()[0].GetName() == "*" && defaultWIW.GetSubs()[0].GetSubs().Count == 0) {
                    foreach (Entity wdyhEntity in wdyh.GetSubs())
                        if (wdyhEntity.IsAvailable()) {
                            var entity = new Entity(wdyhEntity.GetName(), true);
                            wiw.GetSubs().Add(entity);

                            foreach (CounterInfo wdyhCounterInfo in wdyhEntity.GetSubs()) {
                                var counterInfo = new CounterInfo(wdyhCounterInfo.GetName());
                                entity.GetSubs().Add(counterInfo);

                                foreach (CounterInfo wdyhInstance in wdyhCounterInfo.GetSubs())
                                    counterInfo.GetSubs().Add(new CounterInfo(wdyhInstance.GetName()));
                            }
                        }
                } else { //Or add specific counter infos.
                    int entitiesLength = defaultWIW.GetSubs()[0].GetName() == "*" ? wdyh.GetSubs().Count : 1;
                    Entity defaultWiwEntity = defaultWIW.GetSubs()[0];

                    //Set the entities to Wiw...
                    for (int entityIndex = 0; entityIndex != entitiesLength; entityIndex++) {
                        Entity wdyhEntity = wdyh.GetSubs()[entityIndex];
                        if (!wdyhEntity.IsAvailable()) continue;

                        var entity = new Entity(wdyhEntity.GetName(), wdyhEntity.IsAvailable());
                        wiw.GetSubs().Add(entity);

                        //And the counter infos...
                        foreach (CounterInfo defaultWiwCounterInfo in defaultWiwEntity.GetSubs()) {
                            CounterInfo wdyhCounterInfo = GetCounterInfo(wdyhEntity, defaultWiwCounterInfo.GetName());
                            if (wdyhCounterInfo != null) {
                                var counterInfo = new CounterInfo(defaultWiwCounterInfo.GetName());
                                entity.GetSubs().Add(counterInfo);

                                //And the instances.
                                if (defaultWiwCounterInfo.GetSubs().Count != 0) {
                                    int instancesLength = defaultWiwCounterInfo.GetSubs()[0].GetName() == "*" ? wdyhCounterInfo.GetSubs().Count : defaultWiwCounterInfo.GetSubs().Count;

                                    for (int instanceIndex = 0; instanceIndex != instancesLength; instanceIndex++)
                                        counterInfo.GetSubs().Add(new CounterInfo(wdyhCounterInfo.GetSubs()[instanceIndex].GetName()));
                                }
                            }
                        }
                    }
                }
                monitor.Wiw = wiw;
            }
        }

        private static CounterInfo GetCounterInfo(Entity entity, string counterInfoName) {
            foreach (CounterInfo counterInfo in entity.GetSubs())
                if (counterInfo.GetName() == counterInfoName)
                    return counterInfo;
            return null;
        }
    }
}
