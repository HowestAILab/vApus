/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.IO;
using System.Windows.Forms;

namespace vApus.Report
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Otherwise probing privatePath will not work --> ConnectionProxyPrerequisites sub folder.
            Directory.SetCurrentDirectory(Application.StartupPath);

            if (args != null && args.Length != 0)
                Application.Run(new Report(args[0]));
            else
                Application.Run(new Report());
        }
    }
}