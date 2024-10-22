﻿/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using RandomUtils;
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using vApus.Util;
using Timer = System.Timers.Timer;

namespace vApus.Util {
    public partial class CleanTempDataPanel : Panel {
        private static readonly object _lock = new object();
        //Every 10 minutes
        private readonly Dictionary<string, double> _d = new Dictionary<string, double>(4);
        private readonly Timer _tmr = new Timer(600000);

        public CleanTempDataPanel() {
            InitializeComponent();

            _d.Add("CompilerUnitTempFiles", 0);
            _d.Add("Logs", 0);
            _d.Add("UpdateTempFiles", 0);

            GetAndStoreAllSizes();

            _tmr.Elapsed += _tmr_Elapsed;
            _tmr.Start();

            HandleCreated += CleanTempDataPanel_HandleCreated;
        }

        public double TempDataSizeInMB {
            get {
                lock (_lock) {
                    double total = 0;
                    foreach (double size in _d.Values)
                        total += size;
                    return total;
                }
            }
        }

        private void _tmr_Elapsed(object sender, ElapsedEventArgs e) {
            try {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { GetAndStoreAllSizes(); }, null);
            }
            catch (Exception ex) {
                Loggers.Log(Level.Warning, "Failed getting temp files.", ex);
            }
        }

        private void CleanTempDataPanel_HandleCreated(object sender, EventArgs e) {
            HandleCreated -= CleanTempDataPanel_HandleCreated;
            GetAndStoreAllSizes();
        }

        private void GetAndStoreAllSizes() {
            var keys = new string[_d.Keys.Count];
            _d.Keys.CopyTo(keys, 0);
            foreach (string d in keys)
                GetAndStoreSize(d);

            if (IsHandleCreated) {
                btnOpenConnectionProxyTempFiles.Text = string.Format("     ConnectionProxyTempFiles... [{0} MB]",
                                                                     _d["CompilerUnitTempFiles"]);
                btnOpenConnectionProxyTempFiles.Enabled =
                    btnDeleteConnectionProxyTempFiles.Enabled = (_d["CompilerUnitTempFiles"] != 0);

                btnOpenLogs.Text = string.Format("     Logs... [{0} MB]", _d["Logs"]);
                btnOpenLogs.Enabled = btnDeleteLogs.Enabled = (_d["Logs"] != 0);

                btnOpenUpdateTempFiles.Text = string.Format("     UpdateTempFiles... [{0} MB]", _d["UpdateTempFiles"]);
                btnOpenUpdateTempFiles.Enabled = btnDeleteUpdateTempFiles.Enabled = (_d["UpdateTempFiles"] != 0);

                double tempDataSizeInMB = TempDataSizeInMB;
                btnDeleteAll.Text = string.Format("Delete all [{0} MB]", tempDataSizeInMB);
                btnDeleteAll.Enabled = (tempDataSizeInMB != 0);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="d"></param>
        private void GetAndStoreSize(string d) {
            string d2 = Path.Combine(Application.StartupPath, d);
            double sizeInMB = 0.0;

            try {
                sizeInMB = Directory.Exists(d2) ? DirSize(new DirectoryInfo(d2)) : 0;
            }
            catch (Exception ex) {
                Loggers.Log(Level.Warning, "Failed getting dir size.", ex);
            }

            sizeInMB /= (1024 * 1024);
            _d[d] = Math.Round(sizeInMB, 1);
        }

        private double DirSize(DirectoryInfo d) {
            double size = 0;
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
                size += fi.Length;

            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
                size += DirSize(di);

            return size;
        }

        private void btnOpen_Click(object sender, EventArgs e) {
            var btn = sender as Button;
            string s = Path.Combine(Application.StartupPath, btn.Tag.ToString());

            if (Directory.Exists(s))
                Process.Start(s);
        }

        private void btnDelete_Click(object sender, EventArgs e) {
            Delete((sender as Button).Tag.ToString());
            GetAndStoreAllSizes();
        }

        private void Delete(string d) {
            bool error = false;
            d = Path.Combine(Application.StartupPath, d);
            if (Directory.Exists(d))
                try {
                    string[] files = Directory.GetFiles(d);
                    foreach (string f in files)
                        try {
                            File.Delete(f);
                        }
                        catch {
                            error = false;
                        }
                    Directory.Delete(d, true);
                }
                catch {
                }
            if (error)
                Loggers.Log(Level.Warning, "Failed to delete one or more files that are in use. Probably these are files generated for a stress test.");
        }

        private void btnDeleteAll_Click(object sender, EventArgs e) {
            foreach (string d in _d.Keys)
                Delete(d);
            GetAndStoreAllSizes();
        }

        public override string ToString() {
            return "Clean temporary data";
        }
    }
}