/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.Monitor;
using vApus.Stresstest;
using vApus.Util;
using System.Text;

namespace vApus.DistributedTesting
{
    public partial class DistributedStresstestControl : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        private DistributedTest _distributedTest;
        //The monitor views for the selected stresstest.
        private List<MonitorView> _monitorViews;

        //Caching the progress here.
        private Dictionary<TileStresstest, TileStresstestProgressResults> _progress = new Dictionary<TileStresstest, TileStresstestProgressResults>();
        #region Properties
        public DistributedTest DistributedTest
        {
            get { return _distributedTest; }
            set { _distributedTest = value; }
        }
        #endregion

        #region Constructor
        public DistributedStresstestControl()
        {
            InitializeComponent();

            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(DistributedStresstestControl_HandleCreated);
        }
        #endregion

        #region Functions
        private void DistributedStresstestControl_HandleCreated(object sender, EventArgs e)
        {
            this.HandleCreated -= DistributedStresstestControl_HandleCreated;
            SetGui();
        }
        private void SetGui()
        {
            cboDrillDown.SelectedIndex = 0;
        }
        public void Clear()
        {
            lvwFastResultsListing.Items.Clear();
            eventView.ClearEvents();
        }
        private void stresstestControl_MonitorClicked(object sender, EventArgs e)
        {
            if (_monitorViews != null)
                foreach (MonitorView view in _monitorViews)
                    if (!view.IsDisposed)
                        view.Show();
        }

        /// <summary>
        /// Leave all empty for the default values.
        /// </summary>
        /// <param name="runningTests"></param>
        /// <param name="ok"></param>
        /// <param name="cancelled"></param>
        /// <param name="failed"></param>
        /// <param name="cpuUsage"></param>
        /// <param name="contextSwitchesPerSecond"></param>
        /// <param name="memoryUsage"></param>
        /// <param name="totalVisibleMemory"></param>
        /// <param name="nicsSent"></param>
        /// <param name="nicsReceived"></param>
        public void SetMasterMonitoring(int runningTests = 0,
            int ok = 0,
            int cancelled = 0,
            int failed = 0,
            float cpuUsage = -1f,
            float contextSwitchesPerSecond = -1f,
            int memoryUsage = -1,
            int totalVisibleMemory = -1,
            float nicsSent = -1,
            float nicsReceived = -1)
        {
            kvmRunningTests.Visible = runningTests != 0;
            kvmRunningTests.Value = runningTests.ToString();

            kvmOK.Visible = ok != 0;
            kvmOK.Value = ok.ToString();

            kvmCancelled.Visible = cancelled != 0;
            kvmCancelled.Value = cancelled.ToString();

            kvmFailed.Visible = failed != 0;
            kvmFailed.Value = failed.ToString();

            if (cpuUsage == -1)
            {
                kvmMasterCPUUsage.Value = "N/A";
            }
            else
            {
                kvmMasterCPUUsage.Value = Math.Round(cpuUsage, 2).ToString() + " %";

                if (cpuUsage < 60)
                {
                    kvmMasterCPUUsage.BackColor = Color.GhostWhite;
                }
                else
                {
                    kvmMasterCPUUsage.BackColor = Color.Orange;
                    AppendMasterMessages(cpuUsage + " % CPU Usage", LogLevel.Warning);
                }
            }
            kvmMasterContextSwitchesPerSecond.Value = (contextSwitchesPerSecond == -1) ? "N/A" : contextSwitchesPerSecond.ToString();

            if (memoryUsage == -1 || totalVisibleMemory == -1)
            {
                kvmMasterMemoryUsage.Value = "N/A";
            }
            else
            {
                kvmMasterMemoryUsage.Value = memoryUsage.ToString() + " / " + totalVisibleMemory + " MB";
                if (memoryUsage < 0.9 * totalVisibleMemory)
                {
                    kvmMasterMemoryUsage.BackColor = Color.GhostWhite;
                }
                else if (memoryUsage != 0)
                {
                    kvmMasterMemoryUsage.BackColor = Color.Orange;
                    AppendMasterMessages(memoryUsage + " of " + totalVisibleMemory + " MB used", LogLevel.Warning);
                }
            }
            if (nicsSent == -1)
            {
                kvmMasterNicsSent.Value = "N/A";
            }
            else
            {
                kvmMasterNicsSent.Value = Math.Round((double)nicsSent, 2).ToString() + " %";
                if (nicsSent < 90)
                {
                    kvmMasterNicsSent.BackColor = Color.GhostWhite;
                }
                else
                {
                    kvmMasterNicsSent.BackColor = Color.Orange;
                    AppendMasterMessages(nicsSent + " % NIC Usage (Sent)", LogLevel.Warning);
                }
            }
            if (nicsReceived == -1)
            {
                kvmMasterNicsReceived.Value = "N/A";
            }
            else
            {
                kvmMasterNicsReceived.Value = Math.Round((double)nicsReceived, 2).ToString() + " %";
                if (nicsSent < 90)
                {
                    kvmMasterNicsReceived.BackColor = Color.GhostWhite;
                }
                else
                {
                    kvmMasterNicsReceived.BackColor = Color.Orange;
                    AppendMasterMessages(nicsReceived + " % NIC Usage (Received)", LogLevel.Warning);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">Distributed test or the tostring of the tile</param>
        /// <param name="progress"></param>
        public void SetOverallFastResults(string title, Dictionary<TileStresstest, TileStresstestProgressResults> progress)
        {
            _progress = progress;
            lblFastResultListing.Text = "Fast Results Listing [" + title + "]";
            SetOverallFastResults();
        }
        private void SetOverallFastResults()
        {
            try
            {
                if (!this.IsDisposed)
                {
                    lvwFastResultsListing.SuspendLayout();
                    lvwFastResultsListing.Items.Clear();

                    if (cboDrillDown.SelectedIndex == 0)
                        SetConcurrentUsersProgress();
                    else if (cboDrillDown.SelectedIndex == 1)
                        SetPrecisionProgress();
                    else
                        SetRunProgress();

                    lvwFastResultsListing.ResumeLayout();
                }
            }
            catch { }
        }
        private void SetConcurrentUsersProgress()
        {
            if (!lvwFastResultsListing.Columns.Contains(clmFRLConcurrentUsers))
            {
                lvwFastResultsListing.Columns.Insert(3, clmFRLConcurrentUsers);
                clmFRLConcurrentUsers.Width = -2;
            }
            if (lvwFastResultsListing.Columns.Contains(clmFRLPrecision))
                lvwFastResultsListing.Columns.Remove(clmFRLPrecision);
            if (lvwFastResultsListing.Columns.Contains(clmFRLRun))
                lvwFastResultsListing.Columns.Remove(clmFRLRun);

            lvwFastResultsListing.Items.AddRange(ConcurrentUsersLVWIs());
        }
        private ListViewItem[] ConcurrentUsersLVWIs()
        {
            List<ListViewItem> l = new List<ListViewItem>(_progress.Count);

            foreach (TileStresstest key in _progress.Keys)
                if (_progress[key] != null)
                    foreach (TileConcurrentUsersProgressResult result in _progress[key].TileConcurrentUsersProgressResults)
                    {
                        var metrics = result.Metrics;

                        ListViewItem lvwi = new ListViewItem(result.Metrics.StartMeasuringRuntime.ToString());
                        lvwi.UseItemStyleForSubItems = false;

                        Font font = new Font("Consolas", 10.25f);

                        lvwi.Font = font;
                        lvwi.Tag = result;


                        lvwi.SubItems.Add(result.EstimatedRuntimeLeft.ToShortFormattedString()).Font = font;
                        lvwi.SubItems.Add(result.Metrics.MeasuredRunTime.ToShortFormattedString()).Font = font;

                        lvwi.SubItems.Add(result.ConcurrentUsers.ToString()).Font = font;

                        ListViewItem.ListViewSubItem sub = lvwi.SubItems.Add(result.Metrics.TotalLogEntriesProcessed + " / " + result.Metrics.TotalLogEntries.ToString());
                        sub.Font = new Font(font, FontStyle.Bold);
                        sub.Name = "LogEntriesProcessed";
                        lvwi.SubItems.Add(Math.Round((result.Metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond), 4).ToString()).Font = font;
                        lvwi.SubItems.Add(result.Metrics.AverageTimeToLastByte.TotalMilliseconds.ToString()).Font = font;
                        lvwi.SubItems.Add(result.Metrics.MaxTimeToLastByte.TotalMilliseconds.ToString()).Font = font;
                        lvwi.SubItems.Add(result.Metrics.AverageDelay.TotalMilliseconds.ToString()).Font = font;

                        sub = lvwi.SubItems.Add(result.Metrics.Errors.ToString());
                        sub.Font = font;
                        sub.ForeColor = Color.Red;

                        for (int i = 0; i < lvwFastResultsListing.Items.Count - 1; i++)
                        {
                            sub = lvwFastResultsListing.Items[i].SubItems["LogEntriesProcessed"];
                            if (sub.Font.Bold)
                                sub.Font = new Font(sub.Font, FontStyle.Regular);
                        }

                        l.Add(lvwi);
                    }

            return l.ToArray();
        }
        private void SetPrecisionProgress()
        {
            if (!lvwFastResultsListing.Columns.Contains(clmFRLConcurrentUsers))
            {
                lvwFastResultsListing.Columns.Insert(3, clmFRLConcurrentUsers);
                clmFRLConcurrentUsers.Width = -2;
            }
            if (!lvwFastResultsListing.Columns.Contains(clmFRLPrecision))
            {
                lvwFastResultsListing.Columns.Insert(4, clmFRLPrecision);
                clmFRLPrecision.Width = -2;
            }
            if (lvwFastResultsListing.Columns.Contains(clmFRLRun))
                lvwFastResultsListing.Columns.Remove(clmFRLRun);

            lvwFastResultsListing.Items.AddRange(PrecisionLVWIs());
        }
        private ListViewItem[] PrecisionLVWIs()
        {
            List<ListViewItem> l = new List<ListViewItem>(_progress.Count);
            foreach (TileStresstest key in _progress.Keys)
                if (_progress[key] != null)
                    foreach (TileConcurrentUsersProgressResult cu in _progress[key].TileConcurrentUsersProgressResults)
                        foreach (TilePrecisionProgressResult result in cu.TilePrecisionProgressResults)
                        {
                            var metrics = result.Metrics;
                            ListViewItem lvwi = new ListViewItem(result.Metrics.StartMeasuringRuntime.ToString());
                            lvwi.UseItemStyleForSubItems = false;

                            Font font = new Font("Consolas", 10.25f);

                            lvwi.Font = font;
                            lvwi.Tag = result;

                            lvwi.SubItems.Add(result.EstimatedRuntimeLeft.ToShortFormattedString()).Font = font;
                            lvwi.SubItems.Add(result.Metrics.MeasuredRunTime.ToShortFormattedString()).Font = font;


                            lvwi.SubItems.Add(cu.ConcurrentUsers.ToString()).Font = font;
                            lvwi.SubItems.Add((result.Precision + 1).ToString()).Font = font;

                            ListViewItem.ListViewSubItem sub = lvwi.SubItems.Add(result.Metrics.TotalLogEntriesProcessed + " / " + result.Metrics.TotalLogEntries.ToString());
                            sub.Font = new Font(font, FontStyle.Bold);
                            sub.Name = "LogEntriesProcessed";
                            lvwi.SubItems.Add(Math.Round((result.Metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond), 4).ToString()).Font = font;
                            lvwi.SubItems.Add(result.Metrics.AverageTimeToLastByte.TotalMilliseconds.ToString()).Font = font;
                            lvwi.SubItems.Add(result.Metrics.MaxTimeToLastByte.TotalMilliseconds.ToString()).Font = font;
                            lvwi.SubItems.Add(result.Metrics.AverageDelay.TotalMilliseconds.ToString()).Font = font;

                            sub = lvwi.SubItems.Add(result.Metrics.Errors.ToString());
                            sub.Font = font;
                            sub.ForeColor = Color.Red;


                            for (int i = 0; i < lvwFastResultsListing.Items.Count - 1; i++)
                            {
                                sub = lvwFastResultsListing.Items[i].SubItems["LogEntriesProcessed"];
                                if (sub.Font.Bold)
                                    sub.Font = new Font(sub.Font, FontStyle.Regular);
                            }

                            l.Add(lvwi);
                        }

            return l.ToArray();
        }
        private void SetRunProgress()
        {
            if (!lvwFastResultsListing.Columns.Contains(clmFRLConcurrentUsers))
            {
                lvwFastResultsListing.Columns.Insert(3, clmFRLConcurrentUsers);
                clmFRLConcurrentUsers.Width = -2;
            }
            if (!lvwFastResultsListing.Columns.Contains(clmFRLPrecision))
            {
                lvwFastResultsListing.Columns.Insert(4, clmFRLPrecision);
                clmFRLPrecision.Width = -2;
            }
            if (!lvwFastResultsListing.Columns.Contains(clmFRLRun))
            {
                lvwFastResultsListing.Columns.Insert(5, clmFRLRun);
                clmFRLRun.Width = -2;
            }

            lvwFastResultsListing.Items.AddRange(RunLVWIs());
        }

        private ListViewItem[] RunLVWIs()
        {
            List<ListViewItem> l = new List<ListViewItem>(_progress.Count);

            foreach (TileStresstest key in _progress.Keys)
                if (_progress[key] != null)
                    foreach (TileConcurrentUsersProgressResult cu in _progress[key].TileConcurrentUsersProgressResults)
                        foreach (TilePrecisionProgressResult p in cu.TilePrecisionProgressResults)
                            foreach (TileRunProgressResult result in p.TileRunProgressResults)
                            {
                                var metrics = result.Metrics;

                                ListViewItem lvwi = new ListViewItem(result.Metrics.StartMeasuringRuntime.ToString());
                                lvwi.UseItemStyleForSubItems = false;
                                Font font = new Font("Consolas", 10.25f);

                                lvwi.Font = font;
                                lvwi.Tag = result;

                                lvwi.SubItems.Add(result.EstimatedRuntimeLeft.ToShortFormattedString()).Font = font;
                                lvwi.SubItems.Add(result.Metrics.MeasuredRunTime.ToShortFormattedString()).Font = font;


                                lvwi.SubItems.Add(cu.ConcurrentUsers.ToString()).Font = font;
                                lvwi.SubItems.Add((p.Precision + 1).ToString()).Font = font;
                                lvwi.SubItems.Add((result.Run + 1).ToString()).Font = font;

                                ListViewItem.ListViewSubItem sub = lvwi.SubItems.Add(result.Metrics.TotalLogEntriesProcessed + " / " + result.Metrics.TotalLogEntries.ToString());
                                sub.Font = new Font(font, FontStyle.Bold);
                                sub.Name = "LogEntriesProcessed";
                                lvwi.SubItems.Add(Math.Round((result.Metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond), 4).ToString()).Font = font;
                                lvwi.SubItems.Add(result.Metrics.AverageTimeToLastByte.TotalMilliseconds.ToString()).Font = font;
                                lvwi.SubItems.Add(result.Metrics.MaxTimeToLastByte.TotalMilliseconds.ToString()).Font = font;
                                lvwi.SubItems.Add(result.Metrics.AverageDelay.TotalMilliseconds.ToString()).Font = font;

                                sub = lvwi.SubItems.Add(result.Metrics.Errors.ToString());
                                sub.Font = font;
                                sub.ForeColor = Color.Red;

                                for (int i = 0; i < lvwFastResultsListing.Items.Count - 1; i++)
                                {
                                    sub = lvwFastResultsListing.Items[i].SubItems["LogEntriesProcessed"];
                                    if (sub.Font.Bold)
                                        sub.Font = new Font(sub.Font, FontStyle.Regular);
                                }

                                l.Add(lvwi);
                            }

            return l.ToArray();
        }
        public void AppendMasterMessages(string message, LogLevel logLevel = LogLevel.Info)
        {
            try
            {
                eventView.AddEvent((EventViewEventType)logLevel, message);
            }
            catch { }
        }
        /// <summary>
        /// label updates in visibility
        /// </summary>
        public void SetStresstestStopped()
        {
            //stresstestControl.SetStresstestStopped();
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            eventView.Export();
        }
        private void btnSaveDisplayedResults_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Where do you want to save the displayed results?";
            sfd.FileName = _distributedTest.ToString().ReplaceInvalidWindowsFilenameChars('_');
            sfd.Filter = "TXT|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        sw.Write(GetDisplayedResults());
                        sw.Flush();
                    }
                }
                catch
                {
                    MessageBox.Show("Could not access file: " + sfd.FileName, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }
        /// <summary>
        /// Get the displayed results.
        /// </summary>
        /// <param name="appendHeaders"></param>
        /// <param name="addStresstestColumn"></param>
        /// <returns></returns>
        private string GetDisplayedResults()
        {
            StringBuilder sb = new StringBuilder();
            foreach (ColumnHeader clm in lvwFastResultsListing.Columns)
            {
                sb.Append(clm.Text);
                sb.Append("\t");
            }
            sb.AppendLine();

            foreach (ListViewItem lvwi in lvwFastResultsListing.Items)
            {
                foreach (ListViewItem.ListViewSubItem subItem in lvwi.SubItems)
                {
                    sb.Append(subItem.Text);
                    sb.Append("\t");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        private void cboDrillDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetOverallFastResults();
        }
        #endregion
    }
}