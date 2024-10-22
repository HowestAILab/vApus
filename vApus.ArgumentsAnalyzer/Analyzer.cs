﻿/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Link;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.ArgumentsAnalyzer {
    /// <summary>
    /// Basic vApus CLI stuff.
    /// </summary>
    public static class Analyzer {

        #region Delegates
        //Two types each returning a string, if the string equals "" that means there is no error
        //else this is the error message written to the console and analyzing/executing will abort.
        private delegate string ArgumentsAnalyzerDelegate();

        private delegate string ArgumentsAnalyzerParametersDelegate(List<string> parameters);
        #endregion

        #region Fields

        //Two dictionaries, one holding the matching delegate and the other for the help.
        private static Dictionary<string, Delegate> _argumentsWithDelegate = new Dictionary<string, Delegate>();
        private static Dictionary<string, string> _argumentsWithDescription = new Dictionary<string, string>();

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public static Dictionary<string, string> PossibleArguments {
            get { return _argumentsWithDescription; }
        }

        public static bool StartApplication { get; private set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Basic vApus CLI stuff.
        /// </summary>
        static Analyzer() {
            Init();
        }
        #endregion

        #region Functions

        #region Public
        /// <summary>
        ///     Initializes the possible arguments.
        ///     This will be done automatically when first using 'AnalyzeAndExecute'.
        /// </summary>
        public static void Init() {
            _argumentsWithDelegate = new Dictionary<string, Delegate>();
            _argumentsWithDescription = new Dictionary<string, string>();

            _argumentsWithDelegate.Add("-a", new ArgumentsAnalyzerDelegate(About));
            _argumentsWithDescription.Add("-a", "About.");

            _argumentsWithDelegate.Add("-h", new ArgumentsAnalyzerDelegate(Help));
            _argumentsWithDescription.Add("-h", "Help.");

            _argumentsWithDelegate.Add("-ll", new ArgumentsAnalyzerParametersDelegate(LogLevel));
            _argumentsWithDescription.Add("-ll", "Sets the log level, if no parameters are given it just returns the current log level. Parameters: 0 (= info), 1 (= warning), 2 (= error) or 3 (= fatal) (example: -ll 0)");

            _argumentsWithDelegate.Add("-p", new ArgumentsAnalyzerParametersDelegate(SocketListenerPort));
            _argumentsWithDescription.Add("-p", "Sets the socket listener port, if no parameters are given it just returns the current socket port. (example: -p 1337)");

            _argumentsWithDelegate.Add("-d", new ArgumentsAnalyzerParametersDelegate(StartDistributedTest));
            _argumentsWithDescription.Add("-d", "Starts a distributed test defined by its one-based index. You must check yourself if the test works. (example: test.vass -d 1)");

            _argumentsWithDelegate.Add("-m", new ArgumentsAnalyzerParametersDelegate(StartMonitors));
            _argumentsWithDescription.Add("-m", "Starts one or more monitors defined by their one-based indices separated by spaces. You must check yourself if the test works. (example: test.vass -m 1 2)");

            _argumentsWithDelegate.Add("-s", new ArgumentsAnalyzerParametersDelegate(StartStressTest));
            _argumentsWithDescription.Add("-s", "Starts a stress test defined by its one-based index. You must check yourself if the monitor works. (example: test.vass -s 1)");
        }

        /// <summary>
        ///     Used for manual input.\nThis will analyze the args, execute the right functions sequentialy and return an error message if any.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string AnalyzeAndExecute(string args) {
            return AnalyzeAndExecute(args.Trim().Split(new[] { ' ' }));
        }

        /// <summary>
        ///     This will analyze the args, execute the right functions sequentialy and return an error message if any.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string AnalyzeAndExecute(string[] args) {
            StartApplication = true;

            //If not initialized do this.
            if (_argumentsWithDelegate == null || _argumentsWithDelegate.Count == 0)
                Init();

            //No need to do anything when no arguments are given.
            if (args != null && args.Length > 0) {
                var argsCorrectSentenced = new List<string>();
                //First check if the array does not contain '"', if so make new sentences.
                bool quote = false;
                foreach (string s in args) {
                    if (s.StartsWith("\"") && !quote) {
                        if (s.Length > 1)
                            argsCorrectSentenced.Add(s.Substring(1));
                        else
                            argsCorrectSentenced.Add("");
                        quote = true;
                    } else if (s.EndsWith("\"") && quote) {
                        if (s.Length > 1)
                            argsCorrectSentenced[argsCorrectSentenced.Count - 1] += " " + s.Substring(0, s.Length - 1);
                        else
                            argsCorrectSentenced[argsCorrectSentenced.Count - 1] += " ";
                        quote = false;
                    } else {
                        if (quote)
                            argsCorrectSentenced[argsCorrectSentenced.Count - 1] += " " + s;
                        else
                            argsCorrectSentenced.Add(s);
                    }
                }

                //Check which sentences are arguments and assemble them.
                var argsWithParams = new List<List<string>>();
                string toLower = string.Empty;
                for (int i = 0; i < argsCorrectSentenced.Count; i++) {
                    string s = argsCorrectSentenced[i];
                    toLower = s.ToLowerInvariant();
                    //For other arguments
                    if (_argumentsWithDelegate.ContainsKey(toLower)) {
                        argsWithParams.Add(new List<string>());
                    } else if (toLower.StartsWith("-")) {
                        //Check if the argument is valid.
                        if (!_argumentsWithDelegate.ContainsKey(toLower))
                            return "ERROR\n'" + s + "' is not a valid argument!\nType '-h' for help.\n_____";
                        argsWithParams.Add(new List<string>());
                    }

                    //If the first word did not start with "-s" or it isn't one of the other arguments it is not a valid argument.
                    if (argsWithParams.Count == 0) {
                        if (i == 0)
                            argsWithParams.Add(new List<string>());
                        else
                            return "ERROR\n'" + s + "' is not a valid argument!\nType '-h' for help.\n_____";
                    }
                    //
                    argsWithParams[argsWithParams.Count - 1].Add(s);
                }
                //Execute the right function for the right argument.
                string message = "";

                foreach (var argWithParams in argsWithParams) {
                    string ss = argWithParams[0];
                    toLower = ss.ToLowerInvariant();
                    if (_argumentsWithDelegate.ContainsKey(toLower) &&
                        _argumentsWithDelegate[toLower] is ArgumentsAnalyzerParametersDelegate) {
                        var parameters = new List<string>();
                        for (int j = 1; j < argWithParams.Count; j++)
                            parameters.Add(argWithParams[j]);

                        message =
                            (_argumentsWithDelegate[toLower] as ArgumentsAnalyzerParametersDelegate).Invoke(parameters);
                    } else if (_argumentsWithDelegate.ContainsKey(toLower) &&
                               _argumentsWithDelegate[toLower] is ArgumentsAnalyzerDelegate) {
                        message = (_argumentsWithDelegate[toLower] as ArgumentsAnalyzerDelegate).Invoke();
                    } else {
                        message = LoadNewActiveSolution(ss);
                    }
                    if (message.StartsWith("ERROR"))
                        return message;
                }
            }
            return "";
        }

        #endregion

        #region Private

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private static string About() {
            Process.Start(Path.Combine(Application.StartupPath, "vApus.ArgumentsAnalyzer.exe"), "a");
            Application.Exit();
            return "";
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private static string Help() {
            Process.Start(Path.Combine(Application.StartupPath, "vApus.ArgumentsAnalyzer.exe"), "h");
            Application.Exit();
            return "";
        }

        private static string LogLevel(List<string> parameters) {
            var logger = Loggers.GetLogger<FileLogger>();
            try {
                if (parameters.Count != 0)
                    logger.CurrentLevel = (Level)int.Parse(parameters[0]);
            } catch (Exception ex) {
                return "ERROR\nCould not set the log level!\n" + ex;
            }
            return ((int)logger.CurrentLevel) + " (= " + logger.CurrentLevel + ")";
        }

        private static string SocketListenerPort(List<string> parameters) {
            for (int i = 1; i != 4; i++)
                try {
                    if (parameters.Count != 0) {
                        SocketListenerLinker.SetPort(int.Parse(parameters[0]));
                        break;
                    }
                } catch (Exception ex) {
                    Thread.Sleep(i * 500);
                    return "ERROR\nCould not set the socket listener IP and port!\n" + ex;
                }

            try {
                return SocketListenerLinker.SocketListenerPort.ToString();
            } catch (Exception ex) {
                return "ERROR\nCould not return the socket listener IP and port!\n" + ex;
            }
        }
        private static string LoadNewActiveSolution(string fileName) {
            fileName = Path.GetFullPath(fileName);
            if (Solution.LoadNewActiveSolution(fileName))
                return fileName;
            return "ERROR\n'" + fileName + "' could not be loaded!";
        }

        private static string StartDistributedTest(List<string> parameters) {
            try {
                CommunicateToVApus("/startdistributedtest/" + parameters[0]);
            } catch (Exception ex) {
                return "ERROR\nDistributed test could not be started!\n" + ex;
            }
            return "";
        }

        private static string StartMonitors(List<string> parameters) {
            var sb = new StringBuilder();
            foreach (string index in parameters)
                sb.AppendLine(StartMonitor(index));
            return sb.ToString();
        }

        private static string StartMonitor(string index) {
            try {
                CommunicateToVApus("/startmonitor/" + index);
            } catch (Exception ex) {
                return "ERROR\nMonitor could not be started!\n" + ex;
            }
            return "";
        }

        private static string StartStressTest(List<string> parameters) {
            try {
                CommunicateToVApus("/startstresstest/" + parameters[0]);
            } catch (Exception ex) {
                return "ERROR\nStress test could not be started!\n" + ex;
            }
            return "";
        }

        private static void CommunicateToVApus(string message) {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var socketWrapper = new SocketWrapper("127.0.0.1", SocketListenerLinker.SocketListenerPort, socket);

            socketWrapper.Connect(20000, 2);

            socketWrapper.Send(message, SendType.Text, Util.Encoding.UTF8);
            //Don't care about the answer. Just close the socket, otherwise the main thread will deadlock.

            socketWrapper.Close();
        }

        #endregion

        #endregion
    }
}