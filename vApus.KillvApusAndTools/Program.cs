/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace vApus.KillvApusAndTools {
    /// <summary>
    /// Used to Kill vApus and Tools when uninstalling or updating vApus. Can be used manually if you want.
    /// </summary>
    internal static class Program {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main() {
            Parallel.ForEach(Process.GetProcesses(), (p) => {
                try {
                    if (p != null && (p.ProcessName == "vApus" || p.ProcessName == "vApus.UpdateTool" || p.ProcessName == "vApus.UpdateToolLoader" || 
                        p.ProcessName == "vApus.JumpStart" || p.ProcessName == "Lupus-Titanium_GUI" || p.ProcessName == "vApus.PublishItemsHandler" || 
                        p.ProcessName == "vApus.DetailedResultsViewer"))
                        p.Kill();
                } catch {
                    //Don't care
                }
            });

        }
    }
}