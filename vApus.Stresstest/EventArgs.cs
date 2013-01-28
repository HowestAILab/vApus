/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Drawing;
using vApus.Results;
using vApus.Util;

namespace vApus.Stresstest {
    public class StresstestResultEventArgs : EventArgs {
        public StresstestResultEventArgs(StresstestResult stresstestResult) {
            StresstestResult = stresstestResult;
        }

        public StresstestResult StresstestResult { private set; get; }
    }

    public class ConcurrencyResultEventArgs : EventArgs {
        public ConcurrencyResultEventArgs(ConcurrencyResult result) {
            Result = result;
        }

        public ConcurrencyResult Result { private set; get; }
    }

    public class RunResultEventArgs : EventArgs {
        public RunResultEventArgs(RunResult result) {
            Result = result;
        }

        public RunResult Result { private set; get; }
    }

    public class IntValueEventArgs : EventArgs {
        public IntValueEventArgs(int value) {
            Value = value;
        }

        public int Value { private set; get; }
    }

    public class MessageEventArgs : EventArgs {
        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        /// <param name="color">Can be null</param>
        public MessageEventArgs(string message, Color color, LogLevel logLevel) {
            Message = message;
            Color = color;
            LogLevel = logLevel;
        }

        public string Message { private set; get; }
        public Color Color { private set; get; }
        public LogLevel LogLevel { private set; get; }
    }
}