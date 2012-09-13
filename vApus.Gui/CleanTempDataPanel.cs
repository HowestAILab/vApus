/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Gui
{
    public partial class CleanTempDataPanel : Panel
    {
        private static object _lock = new object();
        //Every 10 minutes
        private System.Timers.Timer _tmr = new System.Timers.Timer(600000);
        private Dictionary<string, double> _d = new Dictionary<string, double>(4);

        public double TempDataSizeInMB
        {
            get
            {
                lock (_lock)
                {
                    double total = 0;
                    foreach (double size in _d.Values)
                        total += size;
                    return total;
                }
            }
        }

        public CleanTempDataPanel()
        {
            InitializeComponent();

            _d.Add("SlaveSideResults", 0);
            _d.Add("ConnectionProxyTempFiles", 0);
            _d.Add("Logs", 0);
            _d.Add("UpdateTempFiles", 0);

            GetAndStoreAllSizes();

            _tmr.Elapsed += new System.Timers.ElapsedEventHandler(_tmr_Elapsed);
            _tmr.Start();

            this.HandleCreated += new EventHandler(CleanTempDataPanel_HandleCreated);
        }

        private void _tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    GetAndStoreAllSizes();
                }, null);
            }
            catch { }
        }
        private void CleanTempDataPanel_HandleCreated(object sender, EventArgs e)
        {
            this.HandleCreated -= CleanTempDataPanel_HandleCreated;
            GetAndStoreAllSizes();
        }
        private void GetAndStoreAllSizes()
        {
            string[] keys = new string[_d.Keys.Count];
            _d.Keys.CopyTo(keys, 0);
            foreach (string d in keys)
                GetAndStoreSize(d);

            if (this.IsHandleCreated)
            {
                btnOpenSlaveSideResults.Text = string.Format("     SlaveSideResults... [{0}MB]", _d["SlaveSideResults"]);
                btnOpenSlaveSideResults.Enabled = btnDeleteSlaveSideResults.Enabled = (_d["SlaveSideResults"] != 0);

                btnOpenConnectionProxyTempFiles.Text = string.Format("     ConnectionProxyTempFiles... [{0}MB]", _d["ConnectionProxyTempFiles"]);
                btnOpenConnectionProxyTempFiles.Enabled = btnDeleteConnectionProxyTempFiles.Enabled = (_d["ConnectionProxyTempFiles"] != 0);

                btnOpenLogs.Text = string.Format("     Logs... [{0}MB]", _d["Logs"]);
                btnOpenLogs.Enabled = btnDeleteLogs.Enabled = (_d["Logs"] != 0);

                btnOpenUpdateTempFiles.Text = string.Format("     UpdateTempFiles... [{0}MB]", _d["UpdateTempFiles"]);
                btnOpenUpdateTempFiles.Enabled = btnDeleteUpdateTempFiles.Enabled = (_d["UpdateTempFiles"] != 0);

                double tempDataSizeInMB = TempDataSizeInMB;
                btnDeleteAll.Text = string.Format("Delete All [{0}MB]", tempDataSizeInMB);
                btnDeleteAll.Enabled = (tempDataSizeInMB != 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        private void GetAndStoreSize(string d)
        {
            string d2 = Path.Combine(Application.StartupPath, d);
            double sizeInMB = 0;

            try
            {
                sizeInMB = Directory.Exists(d2) ? DirSize(new DirectoryInfo(d2)) : 0;
            }
            catch { }

            sizeInMB /= (1024 * 1024);
            _d[d] = Math.Round(sizeInMB, 0);
        }
        private double DirSize(DirectoryInfo d)
        {
            double size = 0;
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
                size += fi.Length;

            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
                size += DirSize(di);

            return size;
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            string s = Path.Combine(Application.StartupPath, btn.Tag.ToString());

            if (Directory.Exists(s))
                Process.Start(s);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Delete((sender as Button).Tag.ToString());
            GetAndStoreAllSizes();
        }
        private void Delete(string d)
        {
            d = Path.Combine(Application.StartupPath, d);

            if (Directory.Exists(d))
            {
                //Give a warning for 
                if (d.EndsWith("SlaveSideResults") &&
                    MessageBox.Show("Are you really sure you want to delete the slave-side results?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;

                try
                {
                    string[] files = Directory.GetFiles(d);
                    foreach (string f in files)
                        try
                        {
                            File.Delete(f);
                        }
                        catch { }

                    Directory.Delete(d, true);
                }
                catch { }

            }
        }
        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            foreach (string d in _d.Keys)
                Delete(d);
            GetAndStoreAllSizes();
        }

        public override string ToString()
        {
            return "Clean Temporary Data";
        }
    }
}
