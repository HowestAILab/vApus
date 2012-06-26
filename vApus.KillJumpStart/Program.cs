/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Diagnostics;

//Kill JumpStart when uninstalling vApus.
namespace vApus.KillJumpStart
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            foreach(Process p in Process.GetProcessesByName("vApus.JumpStart.exe"))
                if (p != null)
                {
                    p.Kill();
                    break;
                }
        }
    }
}
