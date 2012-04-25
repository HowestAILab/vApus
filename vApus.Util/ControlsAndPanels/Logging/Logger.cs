/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Glenn Desmadryl
 */

using System;
using System.IO;
using System.Windows.Forms;

namespace vApus.Util
{
    /// <summary>
    /// The purpose of this class is to have a logger who writes to a file.
    /// Before this we used Log4Net but that was a bit overrated for the things we want to log (mainly exceptions).
    /// </summary>
    public class Logger
    {
        #region Fields
        public static readonly string DEFAULT_LOCATION;
        private string _location;
        private StreamWriter _sw;
        private string _name;
        private string _logFile;
        private static object _lock = new object();
        #endregion

        #region Properties
        public StreamWriter Writer
        {
            get
            {
                lock (_lock)
                {
                    if (_sw == null)
                    {
                        if (!System.IO.Directory.Exists(_location))
                            System.IO.Directory.CreateDirectory(_location);

                        //Only create a new log file if needed (if the app runs multiple days there will be only one file per run).
                        if (_logFile == null)
                            _logFile = Path.Combine(_location, DateTime.Now.ToString("yyyy-MM-dd") + " " + Name + ".txt");
                        _sw = new StreamWriter(_logFile, true);
                    }
                    return _sw;
                }
            }
            //set { _sw = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }
        public string LogFile
        {
            get { return _logFile; }
        }
        #endregion

        #region Constructors
        static Logger()
        {
            DEFAULT_LOCATION = Path.Combine(Application.StartupPath, "Logs");
        }
        /// <summary>
        /// Creates a new FileLogger with given name.
        /// </summary>
        /// <param name="name"></param>
        public Logger(string name)
            : this(name, DEFAULT_LOCATION)
        { }
        /// <summary>
        /// Creates a new FileLogger with given name and optionally a location.
        /// </summary>
        /// <param name="location">Use this this to a location different than the default location</param>
        public Logger(string name, string location)
        {
            Name = name;
            Location = location;

            OpenOrReOpenWriter();
        }
        #endregion

        public void Log(object input)
        {
            Writer.WriteLine(input);
            Writer.Flush();
        }
        /// <summary>
        /// If the log file must be accessible (temporary!), you can close the writer, don't forget to reopen it.
        /// </summary>
        public void CloseWriter()
        {
            if (_sw != null)
            {
                try { _sw.Close(); }
                catch { }
                try { _sw.Dispose(); }
                catch { }
                _sw = null;
            }
        }
        /// <summary>
        /// Opens the writer.
        /// </summary>
        public void OpenOrReOpenWriter()
        {
            var writer = Writer;
        }

        public void RemoveLogIfEmptyLog()
        {
            if (File.Exists(_logFile))
            {
                CloseWriter();
                string content = string.Empty;
                using (var sr = new StreamReader(_logFile))
                    content = sr.ReadToEnd();
                if (content.Trim().Length == 0)
                    File.Delete(_logFile);
            }
        }
    }
}
