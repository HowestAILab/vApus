/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 *    
 * 
 * All following stuff in in a different project (JumpStartStructures instead of JumpStart) to avoid circular dependencies.
 */
using System;

namespace vApus.JumpStartStructures {
    [Serializable]
    public enum Key {
        JumpStart,
        Kill,
        SmartUpdate
    }

    [Serializable]
    public struct JumpStartMessage {
        /// <summary>
        ///     All ports comma separated.
        /// </summary>
        public string Port;
        /// <summary>
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port">can be multiple ports divided by a comma.</param>
        public JumpStartMessage(string port) {
            Port = port;
        }
    }

    [Serializable]
    public struct KillMessage {
        //The master processID for example
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
    public struct SmartUpdateMessage {
        public string Version, Host, Username, PrivateRSAKeyPath; //Credentials vApus update server.
        public int Port, Channel;
    }
}