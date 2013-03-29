/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using vApus.Gui.Properties;
using vApus.Link;
using vApus.Util;

namespace vApus.Gui {
    internal static class Program {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args) {
            LogWrapper.Log("vApus Started!");
            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ThreadException += Application_ThreadException;

                //Mainly for the ToString() of floating point numbers and of DateTime().
                string culture = Settings.Default.Culture;
                if (!string.IsNullOrEmpty(culture)) {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                }
                //Use ISO 8601 for DateTime formatting.
                var cultureInfo = new CultureInfo(Thread.CurrentThread.CurrentCulture.Name);
                cultureInfo.DateTimeFormat.ShortDatePattern = "yyyy'-'MM'-'dd";
                cultureInfo.DateTimeFormat.LongTimePattern = "HH':'mm':'ss'.'fff";
                Thread.CurrentThread.CurrentCulture = cultureInfo;

                Linker.Link();

                //Work around for the jump start to work.
                if (!args.Contains("-p"))
                    SocketListenerLinker.StartSocketListener();

                //Otherwise probing privatePath will not work --> monitorsources and ConnectionProxyPrerequisites sub folder.
                Directory.SetCurrentDirectory(Application.StartupPath);

                Application.Run(new MainWindow(args));
            } catch (Exception ex) {
                Application.ThreadException -= Application_ThreadException;
                LogWrapper.LogByLevel(ex, LogLevel.Fatal);
                throw;
            } finally {
                LogWrapper.Log("Bye");
                LogWrapper.RemoveEmptyLogs();
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e) {
            //LogWrapper.LogByLevel(e.Exception, LogLevel.Fatal);
            Debug.WriteLine(e.Exception, "Application_ThreadException");
        }
    }
}