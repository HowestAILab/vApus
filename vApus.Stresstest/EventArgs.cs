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
    public class StresstestResultEventArgs : EventArgs 
    {
        public readonly vApus.Results.Model.StresstestResult Result;
        public StresstestResultEventArgs(vApus.Results.Model.StresstestResult results)
        {
            Result = results;
        }

    }
    public class ConcurrencyResultEventArgs : EventArgs 
    {
        public readonly vApus.Results.Model.ConcurrencyResult Result;
        public ConcurrencyResultEventArgs(vApus.Results.Model.ConcurrencyResult result)
        {
            Result = result;
        }
    }
    public class RunResultEventArgs : EventArgs
    {
        public readonly vApus.Results.Model.RunResult Result;
        public RunResultEventArgs(vApus.Results.Model.RunResult result)
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
