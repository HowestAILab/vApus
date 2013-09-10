/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace vApus.DetailedResultsViewer {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Use ISO 8601 for DateTime formatting.
            var cultureInfo = new CultureInfo(Thread.CurrentThread.CurrentCulture.Name);
            cultureInfo.DateTimeFormat.ShortDatePattern = "yyyy'-'MM'-'dd";
            cultureInfo.DateTimeFormat.LongTimePattern = "HH':'mm':'ss'.'fff";
            Thread.CurrentThread.CurrentCulture = cultureInfo;

            Application.Run(new Viewer());
        }
    }
}
