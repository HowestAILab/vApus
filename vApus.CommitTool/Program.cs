/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;

namespace vApus.CommitTool {
    internal static class Program {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args) {
            Console.WriteLine("/*");
            Console.WriteLine("* Copyright 2017 - Present (c) Sizing Servers Lab");
            Console.WriteLine("* University College of West-Flanders, Department GKG");
            Console.WriteLine("*");
            Console.WriteLine(" * Author(s):");
            Console.WriteLine("*    Dieter Vandroemme");
            Console.WriteLine("*/");
            Console.WriteLine("Storing a new vApus update to be committed to the update server!");
            Console.WriteLine("Do not forget to remove the folder UpdateTempFiles in your CI, if need be.");

            var commit = Commit.GetInstance();

            Exception exception;
            if (args.Length >= 8) {
                try {
                    var excludedFilesOrFolders = new string[args.Length - 4];
                    int j = 0;
                    for (int i = 4; i < args.Length; i++)
                        excludedFilesOrFolders[j++] = args[i];

                    exception = commit.Do(args[0], args[1], args[2], args[3], excludedFilesOrFolders);
                }
                catch (Exception ex) {
                    exception = ex;
                }
            }
            else {
                exception = new ArgumentException("Not enough arguments, 4 or more needed.");
            }
            if (exception != null) {
                Console.WriteLine(
                    "Usage: vApus.CommitTool.exe pathToHistoryXml localGitRepository gitCmd timeStamp excludeFilesOrFolders 1 ... n");
                Console.WriteLine(
                    @"Example: vApus.CommitTool.exe c:\vapus\history.xml c:\vapus C:\Program Files\Git\cmd\git.cmd 20130816090800 *pdb temp*");
                Console.WriteLine();
                Console.WriteLine("Exception: " + exception);
                Environment.ExitCode = -1;
            }
            else {
                Console.WriteLine("Done!");
            }
        }
    }
}