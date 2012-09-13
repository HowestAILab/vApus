/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using vApus.DistributedTesting;
using vApus.Util;
using System;

namespace vApus.Link
{
    public static class SocketListenerLinker
    {
        /// <summary>
        /// Use this for instance to show the test name in the title bar of the main window.
        /// The title of the test is the sender.
        /// </summary>
        public static event EventHandler NewTest;

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
            _socketListener.NewTest += new EventHandler<SlaveSideCommunicationHandler.NewTestEventArgs>(_socketListener_NewTest);
        }
        #endregion

        #region Functions
        private static void _socketListener_NewTest(object sender, SlaveSideCommunicationHandler.NewTestEventArgs e)
        {
            if (NewTest != null)
                foreach (EventHandler del in NewTest.GetInvocationList())
                    del.BeginInvoke(e.Test, null, null, null);
        }
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
