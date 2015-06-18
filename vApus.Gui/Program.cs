/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 *
 * makecert.exe is in place to be able to auto generate an ssl certificate for capturing https trafic (vApus.StressTest.EditLog).  
 */
using RandomUtils.Log;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Threading;
using System.Windows.Forms;
using vApus.Gui.Properties;
using vApus.Link;

namespace vApus.Gui {
    internal static class Program {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args) {
            ProfileOptimization.SetProfileRoot(Application.StartupPath);
            ProfileOptimization.StartProfile("Startup.Profile");

            Loggers.Log("vApus Started!");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


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

            Application.Run(new Main(args));

            Loggers.Log("Bye");
        }
    }
}