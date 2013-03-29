/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;

namespace vApus.JumpStartStructures {
    [Serializable]
    public enum Key {
        JumpStart,
        Kill,
        CpuCoreCount,
        SmartUpdate
    }

    [Serializable]
    public struct JumpStartMessage {
        /// <summary>
        ///     All ports comma separated.
        /// </summary>
        public string Port;

        /// <summary>
        ///     All cores space separated, comma separated per port.
        /// </summary>
        public string ProcessorAffinity;

        /// <summary>
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port">can be multiple ports divided by a comma.</param>
        /// <param name="processID"></param>
        public JumpStartMessage(string port, string processorAffinity) {
            Port = port;
            ProcessorAffinity = processorAffinity;
        }
    }

    [Serializable]
    public struct KillMessage {
        //The master port for example
        public int ExcludeProcessID;

        /// <summary>
        /// </summary>
        /// <param name="excludeIP"></param>
        /// <param name="excludeProcessID"></param>
        /// <param name="processID"></param>
        public KillMessage(int excludeProcessID) {
            ExcludeProcessID = excludeProcessID;
        }
    }

    [Serializable]
    public struct CpuCoreCountMessage {
        public int CpuCoreCount;

        public CpuCoreCountMessage(int cpuCoreCount) {
            CpuCoreCount = cpuCoreCount;
        }
    }

    [Serializable]
    public struct SmartUpdateMessage {
        public string Version, Host, Username, Password; //Credentials vApus update server.
        public int Port, Channel;
    }
}