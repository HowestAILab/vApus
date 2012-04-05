/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;

namespace vApus.CommitTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Committing a new vApus update to the update server!");

            Commit commit = Commit.GetInstance();

            Exception exception;
            if (args.Length >= 7)
            {
                try
                {
                    string[] excludedFilesOrFolders = new string[args.Length - 7];
                    int j = 0;
                    for (int i = 7; i < args.Length; i++)
                        excludedFilesOrFolders[j++] = args[i];

                    commit.Do(args[0], int.Parse(args[1]), args[2], args[3], args[4], args[5], out exception, args[6], excludedFilesOrFolders);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }
            else
            {
                exception = new ArgumentException("Not enough arguments.");
            }
            if (exception != null)
            {
                Console.WriteLine("Usage: vApus.CommitTool.exe host port username password historyXml localGitRepository gitCmd excludeFilesOrFolders 1 ... n");
                Console.WriteLine(@"Example: vApus.CommitTool.exe vApusUpdateServer 5222 root pass c:\vapus\history.xml c:\vapus C:\Program Files\Git\cmd\git.cmd *pdb temp*");
                Console.WriteLine();
                Console.WriteLine("Exception: " + exception);
            }
            Console.WriteLine("Done!");
        }
    }
}
