/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using vApus.SocketListenerLink;
using vApus.Link;
using vApus.Util;

namespace vApus.Gui
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            LogWrapper.Log("vApus Started!");

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

                //Mainly for the ToString() of floating point numbers and of DateTime().
                string culture = global::vApus.Gui.Properties.Settings.Default.Culture;
                if (culture != null && culture != string.Empty)
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                Linker.Link();

                //Work around for the jump start to work.
                if (!args.Contains("-ipp"))
                    SocketListenerLinker.StartSocketListener();

                //Otherwise probing privatePath will not work --> monitorsources and ConnectionProxyPrerequisites sub folder.
                System.IO.Directory.SetCurrentDirectory(Application.StartupPath);

                Application.Run(new MainWindow(args));

            }
            catch (Exception ex)
            {
                Application.ThreadException -= Application_ThreadException;
                LogWrapper.LogByLevel(ex, LogLevel.Fatal);
                throw;
            }
            finally
            {
                LogWrapper.Log("Bye");
                LogWrapper.RemoveEmptyLogs();
            }
        }
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            //LogWrapper.LogByLevel(e.Exception, LogLevel.Fatal);
            System.Diagnostics.Debug.WriteLine(e.Exception, "Application_ThreadException");
        }
    }
}
