using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using vApus.SocketListenerLink;
using vApus.Link;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Gui
{
    internal static class ArgumentsAnalyzer
    {
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
        public static string[] PossibleArguments
        {
            get { return new List<string>(_argumentsWithDelegate.Keys).ToArray(); }
        }
        #endregion

        #region Constructor
        static ArgumentsAnalyzer()
        {
            Init();
        }
        #endregion

        #region Functions

        #region Public
        /// <summary>
        /// Initializes the possible arguments.
        /// This will be done automatically when first using 'AnalyzeAndExecute'.
        /// </summary>
        public static void Init()
        {
            _argumentsWithDelegate = new Dictionary<string, Delegate>();
            _argumentsWithDescription = new Dictionary<string, string>();

            _argumentsWithDelegate.Add("-a", new ArgumentsAnalyzerDelegate(About));
            _argumentsWithDescription.Add("-a", "About.");

            _argumentsWithDelegate.Add("-h", new ArgumentsAnalyzerDelegate(Help));
            _argumentsWithDescription.Add("-h", "Help.");

            _argumentsWithDelegate.Add("-ll", new ArgumentsAnalyzerParametersDelegate(LogLevel));
            _argumentsWithDescription.Add("-ll", "Sets the log level, if no parameters are given it just returns the current log level.\n\tParameters:\n\t0 (= info), 1 (= warning), 2 (= error) or 3 (= fatal)\n\t(example: -ll 0)");

            _argumentsWithDelegate.Add("-pa", new ArgumentsAnalyzerParametersDelegate(ProcessorAffinity));
            _argumentsWithDescription.Add("-pa", "Sets the processor affinity, if no parameters are given it just returns the current processor affinity.\n\tProcessor indices can be given space seperated.\n\t(example: -pa 0 1)");

            _argumentsWithDelegate.Add("-ipp", new ArgumentsAnalyzerParametersDelegate(SocketListenerIPP));
            _argumentsWithDescription.Add("-ipp", "Sets the socket listener IP and port, if no parameters are given it just returns the current socket listener IP and port.\n\t(example: -ipp 127.0.0.1:1337)");
        }
        /// <summary>
        /// Used for manual input.\nThis will analyze the args, execute the right functions sequentialy and return an error message if any.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string AnalyzeAndExecute(string args)
        {
            return AnalyzeAndExecute(args.Trim().Split(new char[] { ' ' }));
        }
        /// <summary>
        /// This will analyze the args, execute the right functions sequentialy and return an error message if any.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string AnalyzeAndExecute(string[] args)
        {
            //If not initialized do this.
            if (_argumentsWithDelegate == null || _argumentsWithDelegate.Count == 0)
                Init();

            //No need to do anything when no arguments are given.
            if (args != null && args.Length > 0)
            {
                List<string> argsCorrectSentenced = new List<string>();
                //First check if the array does not contain '"', if so make new sentences.
                bool quote = false;
                foreach (string s in args)
                {
                    if (s.StartsWith("\"") && !quote)
                    {
                        if (s.Length > 1)
                            argsCorrectSentenced.Add(s.Substring(1));
                        else
                            argsCorrectSentenced.Add("");
                        quote = true;
                    }
                    else if (s.EndsWith("\"") && quote)
                    {
                        if (s.Length > 1)
                            argsCorrectSentenced[argsCorrectSentenced.Count - 1] += " " + s.Substring(0, s.Length - 1);
                        else
                            argsCorrectSentenced[argsCorrectSentenced.Count - 1] += " ";
                        quote = false;
                    }
                    else
                    {
                        if (quote)
                            argsCorrectSentenced[argsCorrectSentenced.Count - 1] += " " + s;
                        else
                            argsCorrectSentenced.Add(s);
                    }
                }

                //Check which sentences are arguments and assemble them.
                List<List<string>> argsWithParams = new List<List<string>>();
                string toLower = string.Empty;
                for (int i = 0; i < argsCorrectSentenced.Count; i++)
                {
                    string s = argsCorrectSentenced[i];
                    toLower = s.ToLower();
                    //For other arguments
                    if (_argumentsWithDelegate.ContainsKey(toLower))
                    {
                        argsWithParams.Add(new List<string>());
                    }
                    else if (toLower.StartsWith("-"))
                    {
                        //Check if the argument is valid.
                        if (!_argumentsWithDelegate.ContainsKey(toLower))
                            return "ERROR\n'" + s + "' is not a valid argument!\nType '-h' for help.\n_____";
                        argsWithParams.Add(new List<string>());
                    }

                    //If the first word did not start with "-s" or it isn't one of the other arguments it is not a valid argument.
                    if (argsWithParams.Count == 0)
                    {
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

                foreach (List<string> argWithParams in argsWithParams)
                {
                    string ss = argWithParams[0];
                    toLower = ss.ToLower();
                    if (_argumentsWithDelegate.ContainsKey(toLower) && _argumentsWithDelegate[toLower] is ArgumentsAnalyzerParametersDelegate)
                    {
                        List<string> parameters = new List<string>();
                        for (int j = 1; j < argWithParams.Count; j++)
                            parameters.Add(argWithParams[j]);

                        message = (_argumentsWithDelegate[toLower] as ArgumentsAnalyzerParametersDelegate).Invoke(parameters);
                    }
                    else if (_argumentsWithDelegate.ContainsKey(toLower) && _argumentsWithDelegate[toLower] is ArgumentsAnalyzerDelegate)
                    {
                        message = (_argumentsWithDelegate[toLower] as ArgumentsAnalyzerDelegate).Invoke();
                    }
                    else
                    {
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
        /// 
        /// </summary>
        /// <returns></returns>
        private static string About()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("ABOUT");
            Console.WriteLine("Developed by Dieter Vandroemme aka Didjeeh.");
            Console.WriteLine("(mail: dieter.vandroemme@gmail.com)");
            Console.WriteLine("_____");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            return "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string Help()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            StringBuilder sb = new StringBuilder();
            Console.WriteLine("HELP");
            Console.WriteLine("Arguments can be combined any way you want to, but a solution filename must always come first (no argument key needed).");
            Console.WriteLine("You can run vApus from a script and feed it directly arguments\nor you can type them in the console.");
            Console.WriteLine();
            Console.WriteLine("Keep in mind that they are sequentialy handled, so if there is an error\nthe remaining arguments will not be interpreted.");
            Console.WriteLine();
            Console.WriteLine("Arguments can have parameters, not all of them are required\n(those between '(' and ')').");
            Console.WriteLine("The typing of more parameters than needed will not have any effect\non the process of execution.");
            Console.WriteLine("If you want to use parameters with spaces, like a filename, encapsulate them\nwith '" + "\"" + "'.");
            Console.WriteLine();
            Console.WriteLine();

            bool otherAlreadyWritten = false;
            foreach (string s in _argumentsWithDescription.Keys)
            {
                if (!s.StartsWith("-") && !otherAlreadyWritten)
                {
                    sb.AppendLine();
                    sb.AppendLine("Other:");
                    otherAlreadyWritten = true;
                }
                sb.AppendLine(s + "\t" + _argumentsWithDescription[s] + "\n");
            }
            Console.WriteLine(sb.ToString().Trim());
            Console.WriteLine("_____");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            return "";
        }
        private static string LogLevel(List<string> parameters)
        {
            try
            {
                if (parameters.Count != 0)
                    LogWrapper.LogLevel = (Util.LogLevel)int.Parse(parameters[0]);
            }
            catch (Exception ex)
            {
                return "ERROR\nCould not set the log level!\n" + ex;
            }
            return ((int)LogWrapper.LogLevel) + " (= " + LogWrapper.LogLevel + ")";
        }
        private static string ProcessorAffinity(List<string> parameters)
        {
            try
            {
                if (parameters.Count != 0)
                {
                    int[] cpus = new int[parameters.Count];
                    for (int i = 0; i < parameters.Count; i++)
                        cpus[i] = int.Parse(parameters[i]);
                    Process.GetCurrentProcess().ProcessorAffinity = ProcessorAffinityCalculator.FromArrayToBitmask(cpus);
                }
            }
            catch (Exception ex)
            {
                return "ERROR\nCould not set the processor affinity!\n" + ex;
            }
            string s = string.Empty;
            foreach (int i in ProcessorAffinityCalculator.FromBitmaskToArray(Process.GetCurrentProcess().ProcessorAffinity))
                s += i + " ";
            s = s.Trim();
            return s;
        }
        private static string SocketListenerIPP(List<string> parameters)
        {
            for (int i = 1; i != 4; i++)
                try
                {
                    if (parameters.Count != 0)
                    {
                        string[] split = parameters[0].Split(':');
                        SocketListenerLinker.SetIPAndPort(split[0], int.Parse(split[1]));
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Thread.Sleep(i * 500);
                    return "ERROR\nCould not set the socket listener IP and port!\n" + ex;
                }

            try
            {
                return SocketListenerLinker.SocketListenerIP + ':' + SocketListenerLinker.SocketListenerPort;
            }
            catch (Exception ex)
            {
                return "ERROR\nCould not return the socket listener IP and port!\n" + ex;
            }
        }
        private static string LoadNewActiveSolution(string fileName)
        {
            if (Solution.LoadNewActiveSolution(fileName))
                return fileName;
            return "ERROR\n'" + fileName + "' could not be loaded!";
        }
        #endregion

        #endregion
    }
}
