/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Glenn Desmadryl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using vApus.Util.Properties;

namespace vApus.Util
{
    public delegate void BeforeLoggingEventHandler(object source, BeforeLoggingEventArgs e);

    public delegate void AfterLoggingEventHandler(object source, LogEventArgs e);

    public class LogEventArgs : EventArgs
    {
        public readonly object Data;
        public readonly LogLevel LogLevel;
        public readonly string Timestamp;

        /// <summary>
        /// </summary>
        /// <param name="timestamp">As a string for the rigt formatting.</param>
        /// <param name="logLevel"></param>
        /// <param name="data"></param>
        public LogEventArgs(string timestamp, LogLevel logLevel, object data)
        {
            Timestamp = timestamp;
            LogLevel = logLevel;
            Data = data;
        }
    }

    public class BeforeLoggingEventArgs
    {
        public BeforeLoggingEventArgs(bool cancel = false, object data = null)
        {
            Cancel = cancel;
            Data = data;
        }

        public bool Cancel { get; set; }

        public object Data { get; set; }
    }

    /// <summary>
    ///     Different LogLevels which can be used for tagging a log.
    /// </summary>
    public enum LogLevel
    {
        Info = 0,
        Warning = 1,
        Error = 2,

        /// <summary>
        ///     Use when the application crashes (in a try...catch around Application.Run(...)).
        /// </summary>
        Fatal = 3
    }

    /// <summary>
    ///     This is a wrapper class for logging. It is built as a combination of a static implementation (the default logger) and possibly other loggers.
    /// </summary>
    public class LogWrapper
    {
        #region Fields

        private static LogWrapper _default;

        private static LogLevel _logLevel;

        private static Dictionary<string, LogWrapper> _logwrappers;

        //keeps track whether we copy it to console or not, default false;
        private static bool _consoleEnabled;
        private readonly Logger _logger;

        #endregion

        public Logger Logger
        {
            get { return _logger; }
        }

        #region Events

        public event BeforeLoggingEventHandler BeforeLogging;
        public event AfterLoggingEventHandler AfterLogging;

        #endregion

        #region Properties

        public static LogWrapper Default
        {
            get
            {
                if (_default == null)
                    _default = new LogWrapper();
                return _default;
            }
            //set { LogWrapper._current = value; }
        }

        public static Dictionary<string, LogWrapper> Logwrappers
        {
            get
            {
                if (_logwrappers == null)
                    _logwrappers = new Dictionary<string, LogWrapper>();

                return _logwrappers;
            }
            //set { LogWrapper._logwrappers = value; }
        }

        /// <summary>
        ///     Logs with the same or higher level as this minimumLogLevel will be logged, lower will be discarded. This value is identical for all the loggers.
        /// </summary>
        public static LogLevel LogLevel
        {
            get { return _logLevel; }
            set
            {
                _logLevel = value;
                Settings.Default.LLogLevel = ((int) _logLevel).ToString();
                Settings.Default.Save();
            }
        }

        /// <summary>
        ///     Keeps track whether we copy it to console or not, default false;
        /// </summary>
        public static bool ConsoleEnabled
        {
            get { return _consoleEnabled; }
            set { _consoleEnabled = value; }
        }

        #endregion

        #region Constructors

        private LogWrapper()
            : this("PID_" + Process.GetCurrentProcess().Id)
        {
        }

        private LogWrapper(string loggerName)
        {
            _logger = new Logger(loggerName);
            Logwrappers.Add(loggerName, this);
            _logLevel = (LogLevel) int.Parse(Settings.Default.LLogLevel);
        }

        private LogWrapper(string loggerName, LogLevel minimumLogLevel)
            : this(loggerName)
        {
            LogLevel = minimumLogLevel;
        }

        #endregion

        #region Logic

        #region Static Methods

        public static void AddLog(string loggerName, LogLevel minimumLogLevel = LogLevel.Info)
        {
            if (!Logwrappers.ContainsKey(loggerName))
                new LogWrapper(loggerName, minimumLogLevel);
        }

        /// <summary>
        ///     Logs the given object with the default logger. The LogLevel used for this is LogLevel.Info.
        /// </summary>
        public static void Log(object input)
        {
            //Looses the overall stack trace
            //if (input is Exception)
            //    input = ExceptionHelper.ParseExceptionToString(input as Exception);

            Default.Log(input, LogLevel.Info);
        }

        /// <summary>
        ///     Logs the given object with the default logger and specified LogLevel.
        /// </summary>
        public static void LogByLevel(object input, LogLevel level)
        {
            Default.Log(input, level);
        }

        public static void RemoveEmptyLogs()
        {
            if (_logwrappers != null)
                foreach (LogWrapper logWrapper in _logwrappers.Values)
                    logWrapper.RemoveLogIfEmptyLog();

            if (_default != null)
                _default.RemoveLogIfEmptyLog();
        }

        #endregion

        /// <summary>
        ///     Use this method to log to a specific logger.
        /// </summary>
        public void Log(object input, LogLevel level)
        {
            if (LogLevel > level)
                return;

            if (BeforeLogging != null)
            {
                var ea = new BeforeLoggingEventArgs(false, input);

                //Invoke and let the user change the data or cancel. Invoke because the data can be changed
                BeforeLogging.Invoke(this, ea);
                input = ea.Data;

                if (ea.Cancel)
                {
                    if (ConsoleEnabled)
                        Console.WriteLine("Logging aborted by user.");
                    return;
                }
            }

            //Create to the following string
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff");
            object data = input;
            input = timeStamp + ";" + level.ToString() + ";" + input;

            //log the data
            _logger.Log(input);

            //raise the event that there has been logged, BeginInvoke because we dont want to wait.
            if (AfterLogging != null)
                AfterLogging.BeginInvoke(this, new LogEventArgs(timeStamp, level, data), null, null);

            if (_consoleEnabled)
                Console.WriteLine(input.ToString());
        }

        public void RemoveLogIfEmptyLog()
        {
            _logger.RemoveLogIfEmptyLog();
        }

        #endregion
    }
}