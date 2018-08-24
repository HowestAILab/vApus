/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace vApus.UpdateToolLoader {
    internal static class Program {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args) {
            if (args != null && args.Length != 0) {
                var from = new[]
                    {
                        Path.Combine(Application.StartupPath, "vApus.UpdateTool.exe"),
                        Path.Combine(Application.StartupPath, "vApus.Util.dll"),
                        Path.Combine(Application.StartupPath, "Renci.SshNet.dll"),
                        Path.Combine(Application.StartupPath, "RandomUtils.dll")
                    };

                string cachePath = Path.Combine(Application.StartupPath, "UpdaterRuntimeCache");

                var to = new[]
                    {
                        Path.Combine(cachePath, "vApus.UpdateTool.exe"),
                        Path.Combine(cachePath, "vApus.Util.dll"),
                        Path.Combine(cachePath, "Renci.SshNet.dll"),
                        Path.Combine(cachePath, "RandomUtils.dll")
                    };

                try {
                    if (!Directory.Exists(cachePath))
                        Directory.CreateDirectory(cachePath);

                    string readme = Path.Combine(cachePath, "README.TXT");
                    if (!File.Exists(readme))
                        using (var sw = new StreamWriter(readme)) {
                            sw.Write(
                                "This is a cache folder where the updater will run from (files in use cannot be overwritten aka updated therefore this workaround), this folder can be removed safely after the updater process exited.");
                            sw.Flush();
                        }

                    for (int i = 0; i < from.Length; i++)
                        File.Copy(from[i], to[i], true);

                    string arguments = string.Empty;
                    for (int i = 0; i != args.Length; i++) {
                        string arg = args[i];
                        if (arg.Contains(" ") || arg.Contains("\t")) arg = "\"" + arg + "\"";
                        arguments += arg + " ";
                    }

                    arguments = arguments.Trim();

                    Process p = Process.Start(to[0], arguments);
                    p.WaitForExit();
                } catch (FileNotFoundException ex) {
                    MessageBox.Show("Failed at copying files.\n" + ex.Message + " " + ex.StackTrace, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);

                } catch {
                    MessageBox.Show("The update tool could not be started because the previously cached files where locked!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try {
                    Directory.Delete(cachePath, true);
                } catch {
                    //Will break when update instances are alive, this can normally not be the case.
                }
            }
        }
    }
}