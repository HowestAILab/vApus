/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using vApus.DistributedTesting;
using vApus.Util;

namespace vApus.Link
{
    public static class SocketListenerLinker
    {
        #region Fields
        private static SocketListener _socketListener;
        #endregion

        #region Properties
        public static bool SocketListenerIsRunning
        {
            get
            {
                return _socketListener.IsRunning;
            }
        }
        public static string SocketListenerIP
        {
            get { return _socketListener.IP; }
        }

        public static int SocketListenerPort
        {
            get { return _socketListener.Port; }
        }
        #endregion

        #region Constructor
        static SocketListenerLinker()
        {
            _socketListener = SocketListener.GetInstance();
        }
        #endregion

        #region Functions
        public static void StartSocketListener()
        {
            _socketListener.Start();
        }
        public static void SetIPAndPort(string ip, int port, bool preferred = false)
        {
            _socketListener.SetIPAndPort(ip, port, preferred);
        }
        public static void AddSocketListenerManagerPanel(OptionsDialog optionsDialog)
        {
            optionsDialog.AddOptionsPanel(new SocketListenerManagerPanel());
        }
        #endregion
    }
}
