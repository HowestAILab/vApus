/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;

namespace vApus.JumpStart
{
    class Program
    {
        private static SocketListener _socketListener;

        static void Main(string[] args)
        {
            try
            {
                _socketListener = SocketListener.GetInstance();
                _socketListener.Start();

                Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
                Application.Run();
            }
            catch { }
            finally
            {
                StopSocketListener();
            }
        }
        private static void StopSocketListener()
        {
            if (_socketListener != null)
            {
                try
                {
                    if (_socketListener.IsRunning)
                        _socketListener.Stop();
                }
                catch { }
                _socketListener = null;
            }
        }
        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            StopSocketListener();
        }
    }
}
