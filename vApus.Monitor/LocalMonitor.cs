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
using vApus.Util;

namespace vApus.Monitor {
    /// <summary>
    /// Monitors local hardware usage.
    /// </summary>
    public static class LocalMonitor {

        #region Fields
        //private static object _lock = new object();
        private static readonly Timer _tmr = new Timer();

        private static readonly PerformanceCounter _cpuUsage = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
        private static readonly ManagementObjectSearcher _availableMemory = new ManagementObjectSearcher("SELECT AvailableMBytes FROM Win32_PerfFormattedData_PerfOS_Memory");
        private static readonly PerformanceCounterCategory _nicsCat = new PerformanceCounterCategory("Network Interface");

        private static int _nicsDifference;
        private static readonly Dictionary<string, List<object>> _nicSentReceivedAndBandwidth = new Dictionary<string, List<object>>();
        #endregion

        #region Properties
        public static float CPUUsage { get; private set; }
        public static uint MemoryUsage { get; private set; }
        public static uint TotalVisibleMemory { get; private set; }
        public static string Nic { get; private set; }
        /// <summary>
        /// In %
        /// </summary>
        public static float NicSent { get; private set; }
        /// <summary>
        /// In %
        /// </summary>
        public static float NicReceived { get; private set; }
        /// <summary>
        /// In Mbps
        /// </summary>
        public static int NicBandwidth { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Monitors local hardware usage.
        /// </summary>
        static LocalMonitor() {
            _tmr.Elapsed += _tmr_Elapsed;

            var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
            foreach (ManagementObject queryObj in searcher.Get())
                TotalVisibleMemory = Convert.ToUInt32(queryObj.Properties["TotalVisibleMemorySize"].Value) / 1024;

            //Refresh first.
            Get();
        }
        #endregion

        #region Functions
        public static void StartMonitoring(int refreshInterval) {
            _tmr.Stop();
            _tmr.Interval = refreshInterval;
            _tmr.Start();
        }
        private static void _tmr_Elapsed(object sender, ElapsedEventArgs e) { Get(); }
        private static void Get() {
            //lock (_lock)
            try {
                CPUUsage = _cpuUsage.NextValue();

                uint availableMemory = 0;
                foreach (ManagementObject queryObj in _availableMemory.Get())
                    availableMemory = Convert.ToUInt32(queryObj.Properties["AvailableMBytes"].Value);

                MemoryUsage = TotalVisibleMemory - availableMemory;

                //Nic usage (largest)
                string[] instances = _nicsCat.GetInstanceNames();
                PerformanceCounter sent, received;
                float bandWidth;
                int instancesCount = instances.Length - _nicsDifference;

                if (_nicSentReceivedAndBandwidth.Count != instancesCount) {
                    _nicSentReceivedAndBandwidth.Clear();
                    _nicsDifference = 0;
                    NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                    foreach (string instance in instances) {
                        if (instance == "MS TCP Loopback interface") {
                            ++_nicsDifference;
                            continue;
                        }

                        string nicName = instance;
                        bool down = false;
                        foreach (NetworkInterface nic in nics) {
                            string description = nic.Description;
                            description = description.Replace("\\", "_");
                            description = description.Replace("/", "_");
                            description = description.Replace("(", "[");
                            description = description.Replace(")", "]");
                            description = description.Replace("#", "_");
                            if (description == instance) {
                                nicName = nic.Name;
                                if (nic.OperationalStatus != OperationalStatus.Up) {
                                    down = true;
                                    ++_nicsDifference;
                                }
                                break;
                            }
                        }

                        if (down)
                            continue;

                        sent = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instance);
                        received = new PerformanceCounter("Network Interface", "Bytes Received/sec", instance);
                        bandWidth = (new PerformanceCounter("Network Interface", "Current Bandwidth", instance)).NextValue();

                        if (bandWidth == 0f) {
                            ++_nicsDifference;
                            continue;
                        }

                        var l = new List<object>();
                        l.Add(sent);
                        l.Add(received);
                        l.Add(bandWidth);
                        _nicSentReceivedAndBandwidth.Add(nicName, l);
                    }
                }

                NicSent = 0f;
                NicReceived = 0f;

                foreach (string name in _nicSentReceivedAndBandwidth.Keys) {
                    List<object> l = _nicSentReceivedAndBandwidth[name];
                    sent = l[0] as PerformanceCounter;
                    received = l[1] as PerformanceCounter;
                    bandWidth = (float)l[2];

                    float b = bandWidth / 800;
                    float s = sent.NextValue() / b;
                    float r = received.NextValue() / b;

                    if (s > NicSent) {
                        Nic = name;
                        NicSent = s;
                        NicReceived = r;
                        NicBandwidth = Convert.ToInt32(bandWidth / 1000000);
                    }
                }
                if (NicBandwidth == 0) {
                    Nic = "Nic";
                    NicBandwidth = -1;
                    NicReceived = NicSent = -1;
                }

            } catch {
            }
        }
        #endregion
    }
}