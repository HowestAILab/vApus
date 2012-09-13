/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;

namespace vApus.JumpStart
{
    /// <summary>
    /// </summary>
    public class IPChangedEventArgs : EventArgs
    {
        public readonly string IP;
        /// <summary>
        /// </summary>
        /// <param name="ip"></param>
        public IPChangedEventArgs(string ip)
        {
            IP = ip;
        }
    }
    public class ListeningErrorEventArgs : EventArgs
    {
        public readonly Exception Exception;
        public readonly string IP;
        public readonly int Port;

        public ListeningErrorEventArgs(string ip, int port, Exception exception)
        {
            Exception = exception;
            IP = ip;
            Port = port;
        }

        public override string ToString()
        {
            return "Listening error occured for slave " + IP + ":" + Port + " threw following exception: " + Exception;
        }
    }
}