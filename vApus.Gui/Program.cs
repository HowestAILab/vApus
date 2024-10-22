﻿/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 *
 * makecert.exe is in place to be able to auto generate an ssl certificate for capturing https trafic.  
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
using vApus.Publish;
using vApus.Util;

namespace vApus.Gui {
    internal static class Program {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args) {
          //  if (Environment.OSVersion.Version.Major > 5) SetProcessDPIAware();

            ProfileOptimization.SetProfileRoot(Application.StartupPath);
            ProfileOptimization.StartProfile("Startup.Profile");

            NamedObjectRegistrar.RegisterOrUpdate("IsMaster", true);

            Publisher.Init();
            Loggers.GetLogger<FileLogger>().CurrentLevel = (Level)Settings.Default.LogLevel;
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

            //Prerequisite for the workshop competition, 20150925
            //System.Diagnostics.Process udpBroadcastListener = null;
            //string udpBroadcastListenerPath = Path.Combine(Application.StartupPath, @"UdpBroadcastListener\UdpBroadcastListener.exe");
            //if (File.Exists(udpBroadcastListenerPath))
            //    udpBroadcastListener = System.Diagnostics.Process.Start(udpBroadcastListenerPath);


            Application.Run(new Main(args));


            //if (udpBroadcastListener != null)
            //    try { udpBroadcastListener.Kill(); } catch { }

            Loggers.Log("Bye");
            Loggers.Flush();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}