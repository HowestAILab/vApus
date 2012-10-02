/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace vApus.UpdateToolLoader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args != null && args.Length != 0)
            {
                string[] from = new string[] {Path.Combine(Application.StartupPath, "vApus.UpdateTool.exe"), 
                    Path.Combine(Application.StartupPath, "DiffieHellman.dll"),
                    Path.Combine(Application.StartupPath, "Org.Mentalis.Security.dll"),
                    Path.Combine(Application.StartupPath, "Tamir.SharpSSH.dll"),
                    Path.Combine(Application.StartupPath, "vApus.exe"),
                    Path.Combine(Application.StartupPath, "vApus.Util.dll")
                    };

                string cachePath = Path.Combine(Application.StartupPath, "UpdaterRuntimeCache");

                string[] to = new string[] {Path.Combine(cachePath, "vApus.UpdateTool.exe"),
                    Path.Combine(cachePath, "DiffieHellman.dll"),
                    Path.Combine(cachePath, "Org.Mentalis.Security.dll"),
                    Path.Combine(cachePath, "Tamir.SharpSSH.dll"),
                    Path.Combine(cachePath, "vApus.exe"),
                    Path.Combine(cachePath, "vApus.Util.dll")
                    };

                try
                {
                    if (!Directory.Exists(cachePath))
                        Directory.CreateDirectory(cachePath);

                    string readme = Path.Combine(cachePath, "README.TXT");
                    if (!File.Exists(readme))
                        using (var sw = new StreamWriter(readme))
                        {
                            sw.Write("This is a cache folder where the updater will run from (files in use cannot be overwritten aka updated therefore this workaround), this folder can be removed safely after the updater process exited.");
                            sw.Flush();
                        }

                    for (int i = 0; i < from.Length; i++)
                        File.Copy(from[i], to[i], true);

                    string arguments = string.Empty;
                    foreach (string arg in args)
                        arguments += arg + " ";
                    arguments = arguments.Trim();

                    Process p = Process.Start(to[0], arguments);
                    p.WaitForExit();
                }
                catch
                {
                    MessageBox.Show("The update tool could not be started because the previously cached files where locked!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try
                {
                    Directory.Delete(cachePath, true);
                }
                catch
                {
                    //Will break when multiple shadow copies are alive.
                }
            }
        }
    }
}