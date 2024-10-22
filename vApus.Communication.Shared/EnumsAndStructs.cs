﻿/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using vApus.Results;
using vApus.StressTest;
using vApus.Util;

namespace vApus.Communication.Shared {
    [Serializable]
    public enum Key {
        /// <summary>
        ///     To poll if the slave is online.
        ///     If so, this slave will connect back to the master.
        /// </summary>
        Poll,

        SynchronizeBuffers,

        /// <summary>
        ///     Use this key sending a stress test project from the master, initiate the stress test at the slave whereupon the slave must send a message with this key in it back.
        /// </summary>
        InitializeTest,
        StartTest,
        Break,
        Continue,
        ContinueDivided,
        StopTest,

        /// <summary>
        ///     To push progress to the master (also finished and failed and such).
        ///     Pushing progress will be at minimum, just the metrics will be send, getting the results will happen afterwards.
        /// </summary>
        Push
    }

    [Serializable]
    public struct PollMessage { public int ProcessID; }

    [Serializable]
    public struct SynchronizeBuffersMessage {
        public int BufferSize;
    }

    [Serializable]
    public struct InitializeTestMessage {
        public string Exception;

        /// <summary>
        ///     To push data back to the master.
        /// </summary>
        public string[] PushIPs;

        /// <summary>
        ///     To push data back to the master.
        /// </summary>
        public int PushPort;

        public StressTestWrapper StressTestWrapper;
    }

    [Serializable]
    public class StressTestWrapper {
        public string PublishResultSetId;
        public bool Publish;
        public string PublishHost;
        public ushort PublishPort;

        public RunSynchronization RunSynchronization;
        public int MaxRerunsBreakOnLast;
        public StressTest.StressTest StressTest;
        public ValueStore ValueStore;

        /// <summary>
        ///     To be able to link the stress test to the right tile stress test.
        ///     #.# (TileIndex.TileStress testIndex eg 0.0);
        /// </summary>
        public string TileStressTestIndex;
        public string TileStressTest;
        public string DistributedTest;

        public string[] Monitors;
    }

    [Serializable]
    public struct TestProgressMessage {
        public float CPUUsage;
        public EventPanelEvent[] Events;
        public string Exception;

        /// <summary>
        ///     in MB
        /// </summary>
        public uint MemoryUsage;

        public string Nic;
        public float NicReceived;
        public float NicSent;
        public int NicBandwidth;

        public RunStateChange RunStateChange;
        public bool RunFinished;
        public bool ConcurrencyFinished;
        public StressTestStatus StressTestStatus;
        public DateTime StartedAt;
        public TimeSpan MeasuredRuntime;
        public TimeSpan EstimatedRuntimeLeft;

        public int ThreadsInUse;
        public string TileStressTestIndex;
        public FastStressTestMetricsCache StressTestMetricsCache;
        public bool SimplifiedMetrics;

        /// <summary>
        ///     in MB
        /// </summary>
        public uint TotalVisibleMemory;
    }

    [Serializable]
    public struct StartAndStopMessage {
        public string Exception;
        public string TileStressTestIndex;
    }

    [Serializable]
    public struct ContinueMessage { public int ContinueCounter; }
}