/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 *    
 * Events for updating the GUI.
 */
using System;
using System.Drawing;
using vApus.Results;
using vApus.Util;

namespace vApus.Stresstest {
    public class TestInitializedEventArgs : EventArgs {
        public Exception Exception { private set; get; }
        public TestInitializedEventArgs(Exception exception) { Exception = exception; }
    }
    public class StresstestResultEventArgs : EventArgs {
        public StresstestResult StresstestResult { private set; get; }
        public StresstestResultEventArgs(StresstestResult stresstestResult) { StresstestResult = stresstestResult; }
    }

    public class ConcurrencyResultEventArgs : EventArgs {
        public ConcurrencyResult Result { private set; get; }
        public ConcurrencyResultEventArgs(ConcurrencyResult result) { Result = result; }
    }

    public class RunResultEventArgs : EventArgs {
        public RunResult Result { private set; get; }
        public RunResultEventArgs(RunResult result) { Result = result; }
    }

    /// <summary>
    /// To let the user know what is happening while stresstesting (Run started, error occured).
    /// </summary>
    public class MessageEventArgs : EventArgs {
        public string Message { private set; get; }
        public Color Color { private set; get; }
        public LogLevel LogLevel { private set; get; }

        /// <summary>
        /// To let the user know what is happening while stresstesting (Run started, error occured).
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color">Can be null</param>
        /// <param name="logLevel"></param>
        public MessageEventArgs(string message, Color color, LogLevel logLevel) {
            Message = message;
            Color = color;
            LogLevel = logLevel;
        }
    }
}