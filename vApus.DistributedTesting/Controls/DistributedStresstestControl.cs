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

namespace vApus.DistributedTesting
{
    public partial class DistributedStresstestControl : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        public event EventHandler DrillDownChanged;

        private DistributedTest _distributedTest;
        //The monitor views for the selected stresstest.
        private List<MonitorView> _monitorViews;

        #region Properties
        public DistributedTest DistributedTest
        {
            get { return _distributedTest; }
            set { _distributedTest = value; }
        }
        public int FastResultsCount
        {
            get { return stresstestControl.FastResultsCount; }
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
            this.epnlMasterMessages.CollapsedChanged += new System.EventHandler(this.epnlMasterMessages_CollapsedChanged);
            this.SizeChanged += new System.EventHandler(this.DistributedStresstestControl_SizeChanged);
        }
        /// <summary>
        /// </summary>
        /// <param name="countDown">smaller than 0 for paused or unknown</param>
        public void SetCountDownProgressDelay(int countDown)
        {
            stresstestControl.SetCountDownProgressDelay(countDown);
        }
        public void Clear()
        {
            dgvFastResults.Rows.Clear();
            epnlMasterMessages.ClearEvents();
        }
        /// <summary>
        /// Resets everything to the initial state.
        /// </summary>
        public void SetSlaveInitialized()
        {
            stresstestControl.SetStresstestInitialized();
        }
        /// <summary>
        /// Switch between the master and slave tab
        /// </summary>
        /// <param name="index"></param>
        public void SetSelectedTabIndex(int index)
        {
            tc.SelectedIndex = index;
        }
        /// <summary>
        /// Set the configuration for the chosen tile stresstest.
        /// </summary>
        /// <param name="tileStresstest"></param>
        public void SetSlaveConfigurationControls(OldTileStresstest tileStresstest, List<MonitorView> monitorViews)
        {
            stresstestControl.SetConfigurationControls(tileStresstest.GetNewStresstest());
            _monitorViews = monitorViews;
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
        /// <param name="threadsInUse"></param>
        /// <param name="cpuUsage"></param>
        /// <param name="contextSwitchesPerSecond"></param>
        /// <param name="threadQueueLengthPerSecond"></param>
        /// <param name="threadContentionsPerSecond"></param>
        /// <param name="memoryUsage"></param>
        /// <param name="totalVisibleMemory"></param>
        public void SetSlaveMonitoring(int threadsInUse = 0,
            float cpuUsage = -1f,
            float contextSwitchesPerSecond = -1f,
            int memoryUsage = -1,
            int totalVisibleMemory = -1,
            float nicsSent = -1,
            float nicsReceived = -1)
        {
            stresstestControl.SetClientMonitoring(threadsInUse, cpuUsage, contextSwitchesPerSecond, memoryUsage,
                totalVisibleMemory, nicsSent, nicsReceived);
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

            if (runningTests == 0 && ok != 0 && cancelled == 0 && failed == 0)
            {
                epnlMasterMessages.EndOfTimeFrame = DateTime.Now;
                epnlMasterMessages.SetProgressBarToNow();
            }

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
        /// <param name="tileStresstest"></param>
        /// <param name="pushMessage"></param>
        /// <param name="overalEndOfTimeFrame">The end of time frame for the full test.</param>
        public void SetSlaveProgress(OldTileStresstest tileStresstest, PushMessage pushMessage, DateTime overalBeginOfTimeFrame, DateTime overalEndOfTimeFrame)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            epnlMasterMessages.BeginOfTimeFrame = overalBeginOfTimeFrame;
            epnlMasterMessages.EndOfTimeFrame = overalEndOfTimeFrame;
            epnlMasterMessages.SetProgressBarToNow();

            //Build and add fast results.
            stresstestControl.ClearFastResults();
            if (pushMessage.TileStresstestProgressResults != null)
            {
                foreach (TileConcurrentUsersProgressResult tcr in pushMessage.TileStresstestProgressResults.TileConcurrentUsersProgressResults)
                {
                    ConcurrentUsersResult cr = new ConcurrentUsersResult(tcr.ConcurrentUsers, tcr.Metrics.TotalLogEntries, tcr.Metrics.StartMeasuringRuntime);
                    cr.Metrics = tcr.Metrics;
                    stresstestControl.AddFastResult(cr);

                    foreach (TilePrecisionProgressResult tpr in tcr.TilePrecisionProgressResults)
                    {
                        PrecisionResult pr = new PrecisionResult(tpr.Precision, tpr.Metrics.TotalLogEntries, tpr.Metrics.StartMeasuringRuntime);
                        pr.Metrics = tpr.Metrics;
                        stresstestControl.AddFastResult(pr);

                        foreach (TileRunProgressResult trr in tpr.TileRunProgressResults)
                        {
                            RunResult rr = new RunResult(trr.Run, 1, trr.Metrics.TotalLogEntries, 1, trr.Metrics.StartMeasuringRuntime, trr.RunStartedAndStopped, trr.RunDoneOnce);
                            rr.Metrics = trr.Metrics;
                            stresstestControl.AddFastResult(rr);
                        }
                    }
                }
                stresstestControl.SetStresstestStarted(pushMessage.TileStresstestProgressResults.Metrics.StartMeasuringRuntime);
                stresstestControl.SetMeasuredRunTime(pushMessage.TileStresstestProgressResults.Metrics.MeasuredRunTime, pushMessage.TileStresstestProgressResults.EstimatedRuntimeLeft);
            }

            if (pushMessage.Events == null)
                stresstestControl.ClearEvents();
            else
                stresstestControl.SetEvents(pushMessage.Events);

            stresstestControl.SetStresstestStopped(pushMessage.StresstestResult);

            LockWindowUpdate(0);
        }
        public void SetOverallFastResults(Dictionary<OldTileStresstest, TileStresstestProgressResults> progress)
        {
            if (cboDrillDown.SelectedIndex == 0)
                SetConcurrentUsersProgress(progress);
            else if (cboDrillDown.SelectedIndex == 1)
                SetPrecisionProgress(progress);
            else
                SetRunProgress(progress);

        }
        private void SetConcurrentUsersProgress(Dictionary<OldTileStresstest, TileStresstestProgressResults> progress)
        {
            dgvFastResults.Rows.Clear();

            //Add the header.
            dgvFastResults.Rows.Add("Tile Stresstest", "Started At", "Time Left", "Measured Time", "Concurrent Users",
                "Log Entries Processed", "Throughput / s", "Response Time in ms", "Max. Response Time", "Delay in ms", "Errors");

            //Add the rows
            foreach (OldTileStresstest key in progress.Keys)
                if (progress[key] != null)
                    foreach (TileConcurrentUsersProgressResult result in progress[key].TileConcurrentUsersProgressResults)
                    {
                        var metrics = result.Metrics;
                        dgvFastResults.Rows.Add(key,
                            metrics.StartMeasuringRuntime,
                            result.EstimatedRuntimeLeft.ToShortFormattedString(),
                            metrics.MeasuredRunTime.ToShortFormattedString(),
                            result.ConcurrentUsers,
                            metrics.TotalLogEntriesProcessed + " / " + metrics.TotalLogEntries,
                            Math.Round((result.Metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond), 4),
                            metrics.AverageTimeToLastByte.TotalMilliseconds,
                            metrics.MaxTimeToLastByte.TotalMilliseconds,
                            metrics.AverageDelay.TotalMilliseconds, metrics.Errors);
                    }
        }
        private void SetPrecisionProgress(Dictionary<OldTileStresstest, TileStresstestProgressResults> progress)
        {
            dgvFastResults.Rows.Clear();

            //Add the header.
            dgvFastResults.Rows.Add("Tile Stresstest", "Started At", "Time Left", "Measured Time", "Concurrent Users", "Precision",
                "Log Entries Processed", "Throughput / s", "Response Time in ms", "Max. Response Time", "Delay in ms", "Errors");

            //Add the rows.
            foreach (OldTileStresstest key in progress.Keys)
                if (progress[key] != null)
                    foreach (TileConcurrentUsersProgressResult cu in progress[key].TileConcurrentUsersProgressResults)
                        foreach (TilePrecisionProgressResult result in cu.TilePrecisionProgressResults)
                        {
                            var metrics = result.Metrics;
                            dgvFastResults.Rows.Add(key,
                                metrics.StartMeasuringRuntime,
                                result.EstimatedRuntimeLeft.ToShortFormattedString(),
                                metrics.MeasuredRunTime.ToShortFormattedString(),
                                cu.ConcurrentUsers,
                                result.Precision,
                                metrics.TotalLogEntriesProcessed + " / " + metrics.TotalLogEntries,
                                Math.Round((result.Metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond), 4),
                                metrics.AverageTimeToLastByte.TotalMilliseconds,
                                metrics.MaxTimeToLastByte.TotalMilliseconds,
                                metrics.AverageDelay.TotalMilliseconds, metrics.Errors);
                        }
        }
        private void SetRunProgress(Dictionary<OldTileStresstest, TileStresstestProgressResults> progress)
        {
            dgvFastResults.Rows.Clear();

            //Add the header.
            dgvFastResults.Rows.Add("Tile Stresstest", "Started At", "Time Left", "Measured Time", "Concurrent Users", "Precision", "Run",
                "Log Entries Processed", "Throughput / s", "Response Time in ms", "Max. Response Time", "Delay in ms", "Errors");

            //Add the rows.
            foreach (OldTileStresstest key in progress.Keys)
                if (progress[key] != null)
                    foreach (TileConcurrentUsersProgressResult cu in progress[key].TileConcurrentUsersProgressResults)
                        foreach (TilePrecisionProgressResult p in cu.TilePrecisionProgressResults)
                            foreach (TileRunProgressResult result in p.TileRunProgressResults)
                            {
                                var metrics = result.Metrics;
                                dgvFastResults.Rows.Add(key,
                                    metrics.StartMeasuringRuntime,
                                    result.EstimatedRuntimeLeft.ToShortFormattedString(),
                                    metrics.MeasuredRunTime.ToShortFormattedString(),
                                    cu.ConcurrentUsers,
                                    p.Precision,
                                    result.Run,
                                    metrics.TotalLogEntriesProcessed + " / " + metrics.TotalLogEntries,
                                    Math.Round((result.Metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond), 4),
                                    metrics.AverageTimeToLastByte.TotalMilliseconds,
                                    metrics.MaxTimeToLastByte.TotalMilliseconds,
                                    metrics.AverageDelay.TotalMilliseconds, metrics.Errors);
                            }
        }

        public void AppendMasterMessages(string message, LogLevel logLevel = LogLevel.Info)
        {
            Color[] c = new Color[] { Color.DarkGray, Color.Orange, Color.Red };
            AppendMasterMessages(message, c[(int)logLevel], logLevel);
        }
        public void AppendMasterMessages(string message, Color eventColor, LogLevel logLevel = LogLevel.Info)
        {
            try
            {
                epnlMasterMessages.AddEvent((EventViewEventType)logLevel, eventColor, message);
            }
            catch { }
        }
        // Gui correction, the epnl wil not be sized correctly otherwise.
        private void DistributedStresstestControl_SizeChanged(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());
            epnlMasterMessages.Collapsed = !epnlMasterMessages.Collapsed;
            epnlMasterMessages.Collapsed = !epnlMasterMessages.Collapsed;
            LockWindowUpdate(0);
        }
        // Set the splitter distance of the splitcontainer if collapsed has changed.
        private void epnlMasterMessages_CollapsedChanged(object sender, EventArgs e)
        {
            epnlMasterMessages.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            splitContainer.IsSplitterFixed = epnlMasterMessages.Collapsed;

            int distance = splitContainer.Panel2.Height - epnlMasterMessages.Bottom;
            if (splitContainer.SplitterDistance + distance > 0)
            {
                splitContainer.Panel2MinSize = 25;
                splitContainer.SplitterDistance += distance;
            }

            epnlMasterMessages.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        }
        /// <summary>
        /// label updates in visibility
        /// </summary>
        public void SetStresstestStopped()
        {
            stresstestControl.SetStresstestStopped();
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            epnlMasterMessages.Export();
        }
        private void btnSaveDisplayedResults_Click(object sender, EventArgs e)
        {
            if (dgvFastResults.RowCount == 0)
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Where do you want to save the displayed results?";
            sfd.FileName = _distributedTest.ToString().ReplaceInvalidWindowsFilenameChars('_');
            sfd.Filter = "TXT|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        foreach (DataGridViewRow row in dgvFastResults.Rows)
                            sw.WriteLine(row.ToSV("\t"));

                        sw.Flush();
                    }
                }
                catch
                {
                    MessageBox.Show("Could not access file: " + sfd.FileName, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }
        private void cboDrillDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DrillDownChanged != null)
                DrillDownChanged(this, null);
        }
        #endregion
    }
}
