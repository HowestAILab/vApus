/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using System.Timers;

namespace vApus.Monitor {
    public static class LocalMonitor {
        private static object _lock = new object();
        private static readonly Timer _tmr = new Timer();

        private static readonly PerformanceCounter _cpuUsage = new PerformanceCounter("Processor", "% Processor Time",
                                                                                      "_Total", true);

        private static readonly PerformanceCounter _contextSwitchesPerSecond = new PerformanceCounter("System",
                                                                                                      "Context Switches/sec",
                                                                                                      true);

        private static readonly ManagementObjectSearcher _availableMemory =
            new ManagementObjectSearcher("SELECT AvailableMBytes FROM Win32_PerfFormattedData_PerfOS_Memory");

        private static readonly PerformanceCounterCategory _nicsCat = new PerformanceCounterCategory("Network Interface");

        private static int _nicsDifference;
        private static readonly List<object> _nics = new List<object>();

        public static float CPUUsage, ContextSwitchesPerSecond;
        public static uint MemoryUsage, TotalVisibleMemory;
        public static float NicsSent, NicsReceived;

        static LocalMonitor() {
            _tmr.Elapsed += _tmr_Elapsed;

            var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
            foreach (ManagementObject queryObj in searcher.Get())
                TotalVisibleMemory = Convert.ToUInt32(queryObj.Properties["TotalVisibleMemorySize"].Value) / 1024;
        }

        public static void StartMonitoring(int refreshInterval) {
            _tmr.Stop();
            _tmr.Interval = refreshInterval;
            _tmr.Start();
        }

        private static void _tmr_Elapsed(object sender, ElapsedEventArgs e) {
            try {
                CPUUsage = _cpuUsage.NextValue();
                ContextSwitchesPerSecond = _contextSwitchesPerSecond.NextValue();

                uint availableMemory = 0;
                foreach (ManagementObject queryObj in _availableMemory.Get())
                    availableMemory = Convert.ToUInt32(queryObj.Properties["AvailableMBytes"].Value);

                MemoryUsage = TotalVisibleMemory - availableMemory;

                //Nic usage (largest)
                string[] instances = _nicsCat.GetInstanceNames();
                PerformanceCounter sent, received;
                float bandWidth;
                int instancesCount = instances.Length - _nicsDifference;

                if (_nics.Count != instancesCount * 3) {
                    _nics.Clear();
                    _nicsDifference = 0;
                    NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                    foreach (string instance in instances) {
                        if (instance == "MS TCP Loopback interface") {
                            ++_nicsDifference;
                            continue;
                        }

                        bool down = false;
                        foreach (NetworkInterface nic in nics)
                            if (nic.Name == instance && nic.OperationalStatus != OperationalStatus.Up) {
                                down = true;
                                ++_nicsDifference;
                                break;
                            }

                        if (down)
                            continue;

                        sent = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instance);
                        received = new PerformanceCounter("Network Interface", "Bytes Received/sec", instance);
                        bandWidth =
                            (new PerformanceCounter("Network Interface", "Current Bandwidth", instance)).NextValue() / 800;

                        if (bandWidth == 0f) {
                            ++_nicsDifference;
                            continue;
                        }

                        _nics.Add(sent);
                        _nics.Add(received);
                        _nics.Add(bandWidth);
                    }

                    instancesCount = instances.Length - _nicsDifference;
                }

                NicsSent = 0f;
                NicsReceived = 0f;

                for (int i = 0; i != _nics.Count; i++) {
                    sent = _nics[i] as PerformanceCounter;
                    ++i;
                    received = _nics[i] as PerformanceCounter;
                    ++i;
                    bandWidth = (float)_nics[i];

                    float s = ((sent.NextValue() / bandWidth) / instancesCount);
                    float r = ((received.NextValue() / bandWidth) / instancesCount);

                    if (s > NicsSent || r > NicsReceived) {
                        NicsSent = s;
                        NicsReceived = r;
                    }
                }
            } catch {
            }
        }
    }
}