/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;

namespace vApus.JumpStartStructures
{
    [Serializable]
    public enum Key
    {
        JumpStart,
        Kill,
    }
    [Serializable]
    public struct JumpStartMessage
    {
        public string IP;
        public string Port;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port">can be multiple ports divided by a comma.</param>
        /// <param name="processID"></param>
        public JumpStartMessage(string ip, string port)
        {
            IP = ip;
            Port = port;
        }
    }
    [Serializable]
    public struct KillMessage
    {
        //The master port for example
        public int ExcludeProcessID;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="excludeIP"></param>
        /// <param name="excludeProcessID"></param>
        /// <param name="processID"></param>
        public KillMessage(int excludeProcessID)
        {
            ExcludeProcessID = excludeProcessID;
        }
    }
}