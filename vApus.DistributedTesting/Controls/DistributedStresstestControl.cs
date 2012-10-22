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
using System.Text;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public partial class DistributedStresstestControl : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        private DistributedTest _distributedTest;

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
                    AppendMessages(cpuUsage + " % CPU Usage", LogLevel.Warning);
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
                    AppendMessages(memoryUsage + " of " + totalVisibleMemory + " MB used", LogLevel.Warning);
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
                    AppendMessages(nicsSent + " % NIC Usage (Sent)", LogLevel.Warning);
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
                    AppendMessages(nicsReceived + " % NIC Usage (Received)", LogLevel.Warning);
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
            LockWindowUpdate(lvwFastResultsListing.Handle.ToInt32());
            try
            {
                if (!this.IsDisposed && _progress != null)
                {
                    if (cboDrillDown.SelectedIndex == 0)
                        SetConcurrentUsersProgress();
                    else
                        SetRunProgress();
                }
            }
            catch { }
            LockWindowUpdate(0);
        }

        private void SetConcurrentUsersProgress()
        {
            if (!lvwFastResultsListing.Columns.Contains(clmFRLConcurrency))
            {
                lvwFastResultsListing.Columns.Insert(3, clmFRLConcurrency);
                clmFRLConcurrency.Width = -2;
            }
            if (lvwFastResultsListing.Columns.Contains(clmFRLRuns))
                lvwFastResultsListing.Columns.Remove(clmFRLRuns);

            //Remove the ListViewItems we don't need.
            int count = 0;
            foreach (TileStresstest key in _progress.Keys)
                if (_progress[key] != null)
                    foreach (TileConcurrencyProgressResult result in _progress[key].TileConcurrencyProgressResults)
                        ++count;

            while (lvwFastResultsListing.Items.Count > count)
                lvwFastResultsListing.Items.RemoveAt(count);

            //Add new or recycle.
            int index = 0;
            foreach (TileStresstest key in _progress.Keys)
                if (_progress[key] != null)
                    foreach (TileConcurrencyProgressResult result in _progress[key].TileConcurrencyProgressResults)
                    {
                        if (index < lvwFastResultsListing.Items.Count)
                            RefreshConcurrentUsersLVWI(key, result, lvwFastResultsListing.Items[index]);
                        else
                            lvwFastResultsListing.Items.Add(NewConcurrentUsersLVWI(key, result));

                        ++index;
                    }
        }
        /// <summary>
        /// Recycles list view items.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="lvwi"></param>
        private void RefreshConcurrentUsersLVWI(TileStresstest ts, TileConcurrencyProgressResult result, ListViewItem lvwi)
        {
            lvwi.Tag = result;
            var metrics = result.Metrics;

            string tilestresstest = ts.ToString();
            string estimatedRuntimeLeft = result.EstimatedRuntimeLeft.ToShortFormattedString();

            //Only update the ones needed to be updated
            if (lvwi.Text != tilestresstest || lvwi.SubItems[2].Text != estimatedRuntimeLeft ||
                lvwi.SubItems[5].Text != metrics.TotalLogEntriesProcessed + " / " + metrics.TotalLogEntries ||
                metrics.TotalLogEntriesProcessed < metrics.TotalLogEntries)
            {
                lvwi.SubItems[0].Text = tilestresstest;
                lvwi.SubItems[1].Text = result.Metrics.StartMeasuringRuntime.ToString();
                lvwi.SubItems[2].Text = estimatedRuntimeLeft;
                lvwi.SubItems[3].Text = metrics.MeasuredRunTime.ToShortFormattedString();
                lvwi.SubItems[4].Text = result.ConcurrentUsers.ToString();
                lvwi.SubItems[5].Text = metrics.TotalLogEntriesProcessed + " / " + metrics.TotalLogEntries;
                lvwi.SubItems[6].Text = Math.Round((metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond), 4).ToString();
                lvwi.SubItems[7].Text = metrics.AverageTimeToLastByte.TotalMilliseconds.ToString();
                lvwi.SubItems[8].Text = metrics.MaxTimeToLastByte.TotalMilliseconds.ToString();
                lvwi.SubItems[9].Text = metrics.AverageDelay.TotalMilliseconds.ToString();
                lvwi.SubItems[10].Text = metrics.Errors.ToString();
                lvwi.SubItems[10].ForeColor = Color.Red;

                while (lvwi.SubItems.Count > 11) //Remove the ones we do not need.
                    lvwi.SubItems.RemoveAt(11);
            }
        }
        private ListViewItem NewConcurrentUsersLVWI(TileStresstest ts, TileConcurrencyProgressResult result)
        {
            var metrics = result.Metrics;

            string tilestresstest = ts.ToString();

            ListViewItem lvwi = new ListViewItem(tilestresstest);
            lvwi.UseItemStyleForSubItems = false;

            Font font = new Font("Consolas", 10.25f);

            lvwi.Font = font;
            lvwi.Tag = result;

            lvwi.SubItems.Add(result.Metrics.StartMeasuringRuntime.ToString()).Font = font;
            lvwi.SubItems.Add(result.EstimatedRuntimeLeft.ToShortFormattedString()).Font = font;
            lvwi.SubItems.Add(result.Metrics.MeasuredRunTime.ToShortFormattedString()).Font = font;

            lvwi.SubItems.Add(result.ConcurrentUsers.ToString()).Font = font;

            ListViewItem.ListViewSubItem sub = lvwi.SubItems.Add(result.Metrics.TotalLogEntriesProcessed + " / " + result.Metrics.TotalLogEntries.ToString());
            sub.Font = font;
            sub.Name = "LogEntriesProcessed";
            lvwi.SubItems.Add(Math.Round((result.Metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond), 4).ToString()).Font = font;
            lvwi.SubItems.Add(result.Metrics.AverageTimeToLastByte.TotalMilliseconds.ToString()).Font = font;
            lvwi.SubItems.Add(result.Metrics.MaxTimeToLastByte.TotalMilliseconds.ToString()).Font = font;
            lvwi.SubItems.Add(result.Metrics.AverageDelay.TotalMilliseconds.ToString()).Font = font;

            sub = lvwi.SubItems.Add(result.Metrics.Errors.ToString());
            sub.Font = font;
            sub.ForeColor = Color.Red;

            return lvwi;
        }
        private void SetRunProgress()
        {
            if (!lvwFastResultsListing.Columns.Contains(clmFRLConcurrency))
            {
                lvwFastResultsListing.Columns.Insert(3, clmFRLConcurrency);
                clmFRLConcurrency.Width = -2;
            }
            if (!lvwFastResultsListing.Columns.Contains(clmFRLRuns))
            {
                lvwFastResultsListing.Columns.Insert(4, clmFRLRuns);
                clmFRLRuns.Width = -2;
            }

            int count = 0;
            foreach (TileStresstest key in _progress.Keys)
                if (_progress[key] != null)
                    foreach (TileConcurrencyProgressResult cu in _progress[key].TileConcurrencyProgressResults)
                        foreach (TileRunProgressResult result in cu.TileRunProgressResults)
                            ++count;

            while (lvwFastResultsListing.Items.Count > count)
                lvwFastResultsListing.Items.RemoveAt(count);

            int index = 0;
            foreach (TileStresstest key in _progress.Keys)
                if (_progress[key] != null)
                    foreach (TileConcurrencyProgressResult cu in _progress[key].TileConcurrencyProgressResults)
                            foreach (TileRunProgressResult result in cu.TileRunProgressResults)
                            {
                                if (index < lvwFastResultsListing.Items.Count)
                                    RefreshRunLVWI(key, cu, result, lvwFastResultsListing.Items[index]);
                                else
                                    lvwFastResultsListing.Items.Add(NewRunLVWI(key, cu, result));

                                ++index;
                            }

        }
        /// <summary>
        /// Recycles list view items.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="lvwi"></param>
        private void RefreshRunLVWI(TileStresstest ts, TileConcurrencyProgressResult cu, TileRunProgressResult result, ListViewItem lvwi)
        {
            lvwi.Tag = result;
            var metrics = result.Metrics;

            string tilestresstest = ts.ToString();
            string estimatedRuntimeLeft = result.EstimatedRuntimeLeft.ToShortFormattedString();

            //Only update the ones needed to be updated
            if (lvwi.Text != tilestresstest || lvwi.SubItems[2].Text != estimatedRuntimeLeft ||
                lvwi.SubItems[7].Text != metrics.TotalLogEntriesProcessed + " / " + metrics.TotalLogEntries ||
                metrics.TotalLogEntriesProcessed < metrics.TotalLogEntries)
            {
                lvwi.SubItems[0].Text = tilestresstest;
                lvwi.SubItems[1].Text = result.Metrics.StartMeasuringRuntime.ToString();
                lvwi.SubItems[2].Text = estimatedRuntimeLeft;
                lvwi.SubItems[3].Text = metrics.MeasuredRunTime.ToShortFormattedString();
                lvwi.SubItems[4].Text = cu.ConcurrentUsers.ToString();
                lvwi.SubItems[5].Text = (result.Run + 1).ToString();
                lvwi.SubItems[6].Text = metrics.TotalLogEntriesProcessed + " / " + metrics.TotalLogEntries;
                lvwi.SubItems[7].Text = Math.Round((metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond), 4).ToString();
                lvwi.SubItems[8].Text = metrics.AverageTimeToLastByte.TotalMilliseconds.ToString();
                lvwi.SubItems[9].Text = metrics.MaxTimeToLastByte.TotalMilliseconds.ToString();
                lvwi.SubItems[9].ForeColor = Color.Black;

                if (lvwi.SubItems.Count == 10) //Add if not available.
                {
                    lvwi.SubItems.Add(metrics.AverageDelay.TotalMilliseconds.ToString()).Font = lvwi.Font;

                    var sub = lvwi.SubItems.Add(metrics.Errors.ToString());
                    sub.Font = lvwi.Font;
                    sub.ForeColor = Color.Red;
                }
                else if (lvwi.SubItems.Count == 11)
                {
                    lvwi.SubItems[10].Text = metrics.AverageDelay.TotalMilliseconds.ToString();

                    var sub = lvwi.SubItems.Add(metrics.Errors.ToString());
                    sub.Font = lvwi.Font;
                    sub.ForeColor = Color.Red;
                }
                else if (lvwi.SubItems.Count == 12)
                {
                    lvwi.SubItems[10].Text = metrics.AverageDelay.TotalMilliseconds.ToString();
                    lvwi.SubItems[10].ForeColor = Color.Black;

                    lvwi.SubItems[11].Text = metrics.Errors.ToString();
                    lvwi.SubItems[11].ForeColor = Color.Red;
                }
            }
        }
        private ListViewItem NewRunLVWI(TileStresstest ts, TileConcurrencyProgressResult cu, TileRunProgressResult result)
        {
            string tilestresstest = ts.ToString();
            var metrics = result.Metrics;

            ListViewItem lvwi = new ListViewItem(tilestresstest);
            lvwi.UseItemStyleForSubItems = false;
            Font font = new Font("Consolas", 10.25f);

            lvwi.Font = font;
            lvwi.Tag = result;

            lvwi.SubItems.Add(result.Metrics.StartMeasuringRuntime.ToString()).Font = font;
            lvwi.SubItems.Add(result.EstimatedRuntimeLeft.ToShortFormattedString()).Font = font;
            lvwi.SubItems.Add(result.Metrics.MeasuredRunTime.ToShortFormattedString()).Font = font;


            lvwi.SubItems.Add(cu.ConcurrentUsers.ToString()).Font = font;
            lvwi.SubItems.Add((result.Run + 1).ToString()).Font = font;

            ListViewItem.ListViewSubItem sub = lvwi.SubItems.Add(result.Metrics.TotalLogEntriesProcessed + " / " + result.Metrics.TotalLogEntries.ToString());
            sub.Font = font;
            sub.Name = "LogEntriesProcessed";
            lvwi.SubItems.Add(Math.Round((result.Metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond), 4).ToString()).Font = font;
            lvwi.SubItems.Add(result.Metrics.AverageTimeToLastByte.TotalMilliseconds.ToString()).Font = font;
            lvwi.SubItems.Add(result.Metrics.MaxTimeToLastByte.TotalMilliseconds.ToString()).Font = font;
            lvwi.SubItems.Add(result.Metrics.AverageDelay.TotalMilliseconds.ToString()).Font = font;

            sub = lvwi.SubItems.Add(result.Metrics.Errors.ToString());
            sub.Font = font;
            sub.ForeColor = Color.Red;

            return lvwi;
        }
        public void AppendMessages(string message, LogLevel logLevel = LogLevel.Info)
        {
            try { eventView.AddEvent((EventViewEventType)logLevel, message); }
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
            if (_distributedTest != null)
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