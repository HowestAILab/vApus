/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;

namespace vApus.JumpStart {
    internal class Program {
        private static SocketListener _socketListener;

        private static void Main(string[] args) {
            try {
                _socketListener = SocketListener.GetInstance();
                _socketListener.Start();

                Application.ApplicationExit += Application_ApplicationExit;
                Application.Run();
            } catch {
            } finally {
                StopSocketListener();
            }
        }

        private static void StopSocketListener() {
            if (_socketListener != null) {
                try {
                    if (_socketListener.IsRunning)
                        _socketListener.Stop();
                } catch {
                }
                _socketListener = null;
            }
        }

        private static void Application_ApplicationExit(object sender, EventArgs e) { StopSocketListener(); }
    }
}