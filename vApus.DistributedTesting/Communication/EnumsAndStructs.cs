/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using vApus.Results;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting {
    [Serializable]
    public enum Key {
        /// <summary>
        ///     To poll if the slave is online.
        ///     If so, this slave will connect back to the master.
        /// </summary>
        Poll,

        SynchronizeBuffers,

        /// <summary>
        ///     Use this key sending a stresstest project from the master, initiate the stresstest at the slave whereupon the slave must send a message with this key in it back.
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
        public string Exception;
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

        public StresstestWrapper StresstestWrapper;
    }

    [Serializable]
    public class StresstestWrapper {
        public int StresstestIdInDb;
        public string MySqlHost;
        public int MySqlPort;
        public string MySqlDatabaseName;
        public string MySqlUser;
        public string MySqlPassword;

        public RunSynchronization RunSynchronization;
        public int MaxRerunsBreakOnLast;
        public Stresstest.Stresstest Stresstest;

        /// <summary>
        ///     To be able to link the stresstest to the right tile stresstest.
        ///     #.# (TileIndex.TileStresstestIndex eg 0.0);
        /// </summary>
        public string TileStresstestIndex;
    }

    [Serializable]
    public struct TestProgressMessage {
        public float CPUUsage, ContextSwitchesPerSecond;
        public List<EventPanelEvent> Events;
        public string Exception;

        /// <summary>
        ///     in MB
        /// </summary>
        public uint MemoryUsage;

        public float NicsReceived;
        public float NicsSent;

        public RunStateChange RunStateChange;
        public bool RunFinished;
        public bool ConcurrencyFinished;
        public StresstestStatus StresstestStatus;
        public DateTime StartedAt;
        public TimeSpan MeasuredRuntime;
        public TimeSpan EstimatedRuntimeLeft;

        public int ThreadsInUse;
        public string TileStresstestIndex;
        public StresstestMetricsCache StresstestMetricsCache;
        public bool SimplifiedMetrics;

        /// <summary>
        ///     in MB
        /// </summary>
        public uint TotalVisibleMemory;
    }

    [Serializable]
    public struct StartAndStopMessage {
        public string Exception;
        public string TileStresstestIndex;
    }

    [Serializable]
    public struct ContinueMessage { public int ContinueCounter; }

    [Serializable]
    public struct ResultsMessage {
        public string Exception;
        public string TileStresstestIndex;
        public byte[] TorrentInfo;
    }
}