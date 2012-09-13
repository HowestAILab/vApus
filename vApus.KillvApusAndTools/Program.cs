/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Diagnostics;

//Kill vApus and Tools when uninstalling vApus.
namespace vApus.KillvApusAndTools
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            foreach (Process p in Process.GetProcessesByName("vApus"))
                if (p != null)
                    p.Kill();

            foreach (Process p in Process.GetProcessesByName("vApus.UpdateTool"))
                if (p != null)
                {
                    p.Kill();
                    break;
                }
            foreach (Process p in Process.GetProcessesByName("vApus.UpdateToolLoader"))
                if (p != null)
                {
                    p.Kill();
                    break;
                }
            foreach (Process p in Process.GetProcessesByName("vApusSMT_GUI"))
                if (p != null)
                    p.Kill();

            foreach (Process p in Process.GetProcessesByName("vApus.LogFixer"))
                if (p != null)
                    p.Kill();

            foreach (Process p in Process.GetProcessesByName("vApus.JumpStart"))
                if (p != null)
                {
                    p.Kill();
                    break;
                }
        }
    }
}
