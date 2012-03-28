/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting
{
    [Serializable]
    public enum Key
    {
        SynchronizeBuffers,
        /// <summary>
        /// To poll if the slave is online.
        /// If so, this slave will connect back to the master.
        /// </summary>
        Poll,
        /// <summary>
        /// Use this key sending a stresstest project from the master, initiate the stresstest at the slave whereupon the slave must send a message with this key in it back.
        /// </summary>
        InitializeTest,
        StartTest,
        Break,
        Continue,
        StopTest,
        /// <summary>
        /// To push progress to the master (also finished and failed and such).
        /// Pushing progress will be at minimum, just the metrics will be send, getting the results will happen afterwards.
        /// </summary>
        Push,
        /// <summary>
        /// This will return a torrent file in bytes that the torrent client will use on the master-side.
        /// </summary>
        Results,
        StopSeedingResults
    }
    [Serializable]
    public struct PollMessage
    {
        public int ProcessID;
        public PollMessage(int processID)
        {
            ProcessID = processID;
        }
    }
    [Serializable]
    public struct InitializeTestMessage
    {
        public string PushIP;
        public int PushPort;
        public string Exception;
        public TileStresstest TileStresstest;
        public InitializeTestMessage(string pushIP, int pushPort, string exception, TileStresstest tileStresstest)
        {
            PushIP = pushIP;
            PushPort = pushPort;
            Exception = exception;
            TileStresstest = tileStresstest;
        }
    }
    [Serializable]
    public struct PushMessage
    {
        public int TileStresstestOriginalHashCode;
        public StresstestResult StresstestResult;
        public TileStresstestProgressResults TileStresstestProgressResults;
        public List<EventPanelEvent> Events;

        public int ThreadsInUse;
        /// <summary>
        /// in MB
        /// </summary>
        public uint MemoryUsage, TotalVisibleMemory;
        public float CPUUsage, ContextSwitchesPerSecond, NicsSent, NicsReceived;

        public RunStateChange RunStateChange;

        public string Exception;
    }
    [Serializable]
    public struct SynchronizeBuffersMessage
    {
        public int BufferSize;
        public string Exception;
        public SynchronizeBuffersMessage(int bufferSize, string exception)
        {
            BufferSize = bufferSize;
            Exception = exception;
        }
    }
    [Serializable]
    public struct StartAndStopMessage
    {
        public List<int> TileStresstestHashCodes;
        public string Exception;
        public StartAndStopMessage(List<int> tileStresstestHashCodes, string exception)
        {
            TileStresstestHashCodes = tileStresstestHashCodes;
            Exception = exception;
        }
    }
    [Serializable]
    public struct ContinueMessage
    {
        public int ContinueCounter;
        public ContinueMessage(int continueCounter)
        {
            ContinueCounter = continueCounter;
        }
    }
    [Serializable]
    public struct ResultsMessage
    {
        public List<int> TileStresstestHashCodes;
        public List<byte[]> TorrentInfo;
        public string Exception;
        public ResultsMessage(List<int> tileStresstestHashCodes, List<byte[]> torrentInfo, string exception)
        {
            TileStresstestHashCodes = tileStresstestHashCodes;
            TorrentInfo = torrentInfo;
            Exception = exception;
        }
    }
    [Serializable]
    public struct StopSeedingResultsMessage
    {
        public string TorrentName;
        public string Exception;
        public StopSeedingResultsMessage(string torrentName, string exception)
        {
            TorrentName = torrentName;
            Exception = exception;
        }
    }
}
