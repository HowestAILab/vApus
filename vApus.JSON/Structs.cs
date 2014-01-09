/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;

namespace vApus.JSON {
    public struct MonitorConfig {
        public string MonitorSource;
        public object[] Parameters;
    }

    public struct MonitorMetrics {
        public string[] Headers;
        public Dictionary<DateTime, float[]> Values;
    }

    public struct TestConfig {
        public int[] Concurrency;
        public string Connection, ConnectionProxy;
        public string[] Logs;
        public string LogRuleSet;
        public int MaximumDelayInMS;
        public int MinimumDelayInMS;
        public int MonitorAfterInMinutes;
        public int MonitorBeforeInMinutes;
        public int MaximumNumberOfUserActions;
        public string[] Monitors;
        public int Run;
        public bool Shuffle, ActionDistribution;
        public string[] Slaves;
    }

    public struct TestFastConcurrencyResults {
        public TimeSpan AverageDelay;
        public TimeSpan AverageResponseTime;

        public long Errors;
        public TimeSpan EstimatedTimeLeft;

        public TimeSpan MaxResponseTime;

        public TimeSpan MeasuredTime;

        public string RunStateChange;
        public DateTime StartMeasuringTime;
        public string StresstestResult;

        public long LogEntries, LogEntriesProcessed;
        public double ResponsesPerSecond, UserActionsPerSecond;
    }

    public struct ClientMonitorMetrics {
        public int BusyThreadCount;
        public float CPUUsage;
        public float ContextSwitchesPerSecond;
        public uint MemoryUsage;
        public uint TotalVisibleMemory;
        public float NicsSent;
        public float NicsReceived;
    }

    public struct ClientMessages {
        public string[] Messages;
    }
}
