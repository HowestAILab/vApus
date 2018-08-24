/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Threading.Tasks;
using vApus.DistributedTest;
using vApus.Communication;
using vApus.Util;

namespace vApus.Link {
    /// <summary>
    /// This could not be included in the linker due to circular dependencies. Linking works the other way around here, this class handles communication to the socket listener (Used in vApus.Gui.MainWindow).
    /// </summary>
    public static class SocketListenerLinker {
        /// <summary>
        ///     Use this for instance to show the test name in the title bar of the main window.
        ///     The title of the test is the sender.
        /// </summary>
        public static event EventHandler NewTest;

        #region Fields
        private static readonly SocketListener _socketListener;
        #endregion

        #region Properties
        public static bool SocketListenerIsRunning { get { return _socketListener.IsRunning; } }

        public static int SocketListenerPort { get { return _socketListener.Port; } }
        #endregion

        #region Constructor
        static SocketListenerLinker() {
            _socketListener = SocketListener.GetInstance();
            _socketListener.NewTest += _socketListener_NewTest;
        }
        #endregion

        #region Functions
        private static void _socketListener_NewTest(object sender, SlaveSideCommunicationHandler.NewTestEventArgs e) {
            if (NewTest != null) {
                var invocationList = NewTest.GetInvocationList();
                Parallel.For(0, invocationList.Length, (i) => {
                    (invocationList[i] as EventHandler).Invoke(e.Test, null);
                });
            }
        }

        public static void StartSocketListener() { _socketListener.Start(); }

        public static void SetPort(int port, bool preferred = false) { _socketListener.SetPort(port, preferred); }

        public static void AddSocketListenerManagerPanel(OptionsDialog optionsDialog) { optionsDialog.AddOptionsPanel(new SocketListenerManagerPanel()); }
        #endregion
    }
}