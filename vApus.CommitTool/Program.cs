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
            Console.WriteLine("* Copyright 2009 - Present (c) Sizing Servers Lab");
            Console.WriteLine("* University College of West-Flanders, Department GKG");
            Console.WriteLine("*");
            Console.WriteLine(" * Author(s):");
            Console.WriteLine("*    Dieter Vandroemme");
            Console.WriteLine("*/");
            Console.WriteLine("Committing a new vApus update to the update server!");

            var commit = Commit.GetInstance();

            Exception exception;
            if (args.Length >= 8) {
                try {
                    var excludedFilesOrFolders = new string[args.Length - 8];
                    int j = 0;
                    for (int i = 8; i < args.Length; i++)
                        excludedFilesOrFolders[j++] = args[i];

                    exception = commit.Do(args[0], int.Parse(args[1]), args[2], args[3], args[4], args[5], args[6],
                               args[7], excludedFilesOrFolders);
                }
                catch (Exception ex) {
                    exception = ex;
                }
            }
            else {
                exception = new ArgumentException("Not enough arguments, 8 or more needed.");
            }
            if (exception != null) {
                Console.WriteLine(
                    "Usage: vApus.CommitTool.exe host port username password historyXml localGitRepository gitCmd timeStamp excludeFilesOrFolders 1 ... n");
                Console.WriteLine(
                    @"Example: vApus.CommitTool.exe vApusUpdateServer 5222 root pass c:\vapus\history.xml c:\vapus C:\Program Files\Git\cmd\git.cmd 20130816090800 *pdb temp*");
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