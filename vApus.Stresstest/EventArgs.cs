/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Drawing;
using vApus.Util;

namespace vApus.Stresstest
{
    public class StresstestStartedEventArgs : EventArgs 
    {
        public readonly StresstestResults Result;
        public StresstestStartedEventArgs(StresstestResults results)
        {
            Result = results;
        }
    }
    public class ConcurrentUsersStartedEventArgs : EventArgs 
    {
        public readonly ConcurrentUsersResult Result;
        public ConcurrentUsersStartedEventArgs(ConcurrentUsersResult result)
        {
            Result = result;
        }
    }
    public class PrecisionStartedEventArgs : EventArgs
    {
        public readonly PrecisionResult Result;
        public PrecisionStartedEventArgs(PrecisionResult result)
        {
            Result = result;
        }
    }
    public class RunStartedEventArgs : EventArgs
    {
        public readonly DateTime At;
        public readonly RunResult Result;
        public RunStartedEventArgs(DateTime at, RunResult result)
        {
            At = at;
            Result = result;
        }
    }
    public class RunStoppedEventArgs : EventArgs
    {
        public readonly DateTime At;
        public RunStoppedEventArgs(DateTime at)
        {
            At = at;
        }
    }
    public class RunInitializedFirstTimeEventArgs : EventArgs 
    {
        public readonly RunResult Result;
        public RunInitializedFirstTimeEventArgs(RunResult result)
        {
            Result = result;
        }
    }
    public class IntValueEventArgs : EventArgs
    {
        public readonly int Value;

        public IntValueEventArgs(int value)
        {
            Value = value;
        }
    }
    public class MessageEventArgs : EventArgs
    {
        public readonly string Message;
        public readonly Color Color;
        public readonly LogLevel LogLevel;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        /// <param name="color">Can be null</param>
        public MessageEventArgs(string message,  Color color, LogLevel logLevel)
        {
            Message = message;
            Color = color;
            LogLevel = logLevel;
        }
    }
}
