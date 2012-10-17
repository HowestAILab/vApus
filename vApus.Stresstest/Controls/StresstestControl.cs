/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Stresstest
{
    public partial class StresstestControl : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        /// <summary>
        /// To show the monitor view if any.
        /// </summary>
        public event EventHandler MonitorClicked;

        #region Fields
        private StresstestResults _stresstestResults;
        private List<ListViewItem> _fastResults = new List<ListViewItem>();
        private RFileHandler _slaveSideStresstestResultSaver = new RFileHandler();
        private bool _slaveSideSaveResultsTouched;

        private bool _monitorConfigurationControlVisible;
        #endregion

        #region Properties
        public int FastResultsCount
        {
            get { return _fastResults.Count; }
        }
        /// <summary>
        /// Use the SetStresstestResults function to set (overloading).
        /// </summary>
        public StresstestResults StresstestResults
        {
            get { return _stresstestResults; }
        }

        [Description("The visibility of this control.")]
        public bool MonitorConfigurationControlVisible
        {
            get { return _monitorConfigurationControlVisible; }
            set
            {
                _monitorConfigurationControlVisible = value;
                btnMonitor.Visible = _monitorConfigurationControlVisible;
            }
        }
        #endregion

        #region Constructor
        public StresstestControl()
        {
            InitializeComponent();

            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(StresstestControl_HandleCreated);
        }
        #endregion

        #region Functions
        private void StresstestControl_HandleCreated(object sender, EventArgs e)
        {
            this.HandleCreated -= StresstestControl_HandleCreated;
            SetGui();
        }

        private void SetGui()
        {
            cboDrillDown.SelectedIndex = 0;
            epnlMessages.Collapsed = true;
        }
        /// <summary>
        /// Resets everything to the initial state.
        /// </summary>
        public void SetStresstestInitialized()
        {
            lblUpdatesIn.Visible = true;
            epnlMessages.ClearEvents();

            lblStarted.Text = string.Empty;
            lblMeasuredRuntime.Text = string.Empty;
            lblStopped.Text = string.Empty;

            ClearFastResults();

            _slaveSideSaveResultsTouched = false;

            epnlMessages.EndOfTimeFrame = DateTime.MaxValue;
        }
        public void SetConfigurationControls(Stresstest stresstest)
        {
            SetConfigurationControls(stresstest.ToString(), stresstest.Connection, stresstest.ConnectionProxy, stresstest.Log, stresstest.LogRuleSet,
                stresstest.Monitors, stresstest.ConcurrentUsers, stresstest.Precision, stresstest.DynamicRunMultiplier, stresstest.MinimumDelay,
                stresstest.MaximumDelay, stresstest.Shuffle, stresstest.Distribute, stresstest.MonitorBefore, stresstest.MonitorAfter);
        }
        public void SetConfigurationControls(string stresstest, Connection connection, string connectionProxy,
                                              Log log, string logRuleSet, Monitor.Monitor[] monitors, int[] concurrentUsers,
                                              int precision, int dynamicRunMultiplier, int minimumDelay, int maximumDelay, bool shuffle,
                                              ActionAndLogEntryDistribution distribute, uint monitorBefore, uint monitorAfter)
        {
            kvpStresstest.Key = stresstest;
            kvpConnection.Key = connection.ToString();
            kvpConnectionProxy.Key = connectionProxy;

            kvpLog.Key = log.ToString();
            kvpLogRuleSet.Key = logRuleSet;

            if (monitors == null || monitors.Length == 0)
            {
                btnMonitor.Text = "No Monitor";
                btnMonitor.BackColor = Color.Orange;
            }
            else
            {
                btnMonitor.Text = monitors.Combine(", ") + "...";
                btnMonitor.BackColor = Color.LightBlue;
            }

            kvpConcurrentUsers.Value = concurrentUsers.Combine(", ");

            kvpPrecision.Value = precision.ToString();
            kvpDynamicRunMultiplier.Value = dynamicRunMultiplier.ToString();
            kvpDelay.Value = (minimumDelay == maximumDelay ? minimumDelay.ToString() : minimumDelay + " - " + maximumDelay) + " ms";
            kvpShuffle.Value = shuffle ? "Yes" : "No";
            kvpDistribute.Value = distribute.ToString();
            kvpMonitorBefore.Value = monitorBefore + (monitorBefore == 1 ? " minute" : " minutes");
            kvpMonitorAfter.Value = monitorAfter + (monitorAfter == 1 ? " minute" : " minutes");
        }
        private void btnMonitor_Click(object sender, EventArgs e)
        {
            if (btnMonitor.Text != "Monitor" && btnMonitor.Text != "No Monitor" && MonitorClicked != null)
                MonitorClicked(this, null);
        }
        /// <summary>
        /// </summary>
        /// <param name="countDown">smaller than 0 for paused or unknown</param>
        public void SetCountDownProgressDelay(int countDown)
        {
            if (countDown > -1)
            {
                lblUpdatesIn.ForeColor = Color.SteelBlue;
                lblUpdatesIn.Text = "updates in " + countDown;
                lblUpdatesIn.Image = null;
            }
            else
            {
                lblUpdatesIn.ForeColor = Color.DarkGray;
                lblUpdatesIn.Text = "updates in ..  ";
                lblUpdatesIn.Image = vApus.Stresstest.Properties.Resources.Wait;
            }
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
        public void SetClientMonitoring(int threadsInUse = 0,
            float cpuUsage = -1f,
            float contextSwitchesPerSecond = -1f,
            int memoryUsage = -1,
            int totalVisibleMemory = -1,
            float nicsSent = -1,
            float nicsReceived = -1)
        {
            kvmThreadsInUse.Value = threadsInUse.ToString();
            if (cpuUsage == -1)
            {
                kvmCPUUsage.Value = "N/A";
            }
            else
            {
                kvmCPUUsage.Value = Math.Round(cpuUsage, 2).ToString() + " %";

                if (cpuUsage < 60)
                {
                    kvmCPUUsage.BackColor = Color.GhostWhite;
                }
                else
                {
                    kvmCPUUsage.BackColor = Color.Orange;
                    AppendMessages(cpuUsage + " % CPU Usage", LogLevel.Warning);
                }
            }
            kvmContextSwitchesPerSecond.Value = (contextSwitchesPerSecond == -1) ? "N/A" : contextSwitchesPerSecond.ToString();

            if (memoryUsage == -1 || totalVisibleMemory == -1)
            {
                kvmMemoryUsage.Value = "N/A";
            }
            else
            {
                kvmMemoryUsage.Value = memoryUsage.ToString() + " / " + totalVisibleMemory + " MB";
                if (memoryUsage < 0.9 * totalVisibleMemory)
                {
                    kvmMemoryUsage.BackColor = Color.GhostWhite;
                }
                else if (memoryUsage != 0)
                {
                    kvmMemoryUsage.BackColor = Color.Orange;
                    AppendMessages(memoryUsage + " of " + totalVisibleMemory + " MB used", LogLevel.Warning);
                }
            }
            if (nicsSent == -1)
            {
                kvmNicsSent.Value = "N/A";
            }
            else
            {
                kvmNicsSent.Value = Math.Round((double)nicsSent, 2).ToString() + " %";
                if (nicsSent < 90)
                {
                    kvmNicsSent.BackColor = Color.GhostWhite;
                }
                else
                {
                    kvmNicsSent.BackColor = Color.Orange;
                    AppendMessages(nicsSent + " % NIC Usage (Sent)", LogLevel.Warning);
                }
            }
            if (nicsReceived == -1)
            {
                kvmNicsReceived.Value = "N/A";
            }
            else
            {
                kvmNicsReceived.Value = Math.Round((double)nicsReceived, 2).ToString() + " %";
                if (nicsReceived < 90)
                {
                    kvmNicsReceived.BackColor = Color.GhostWhite;
                }
                else
                {
                    kvmNicsReceived.BackColor = Color.Orange;
                    AppendMessages(nicsReceived + " % NIC Usage (Received)", LogLevel.Warning);
                }
            }
        }
        public void SlaveSideSaveResults()
        {
            if (_stresstestResults != null)
            {
                if (!_slaveSideSaveResultsTouched)
                {
                    _slaveSideStresstestResultSaver.SetStresstestResults(_stresstestResults);
                    _slaveSideSaveResultsTouched = true;
                }
                _slaveSideStresstestResultSaver.Save();

                AppendMessages("Slave-side saved results.");
            }
        }

        /// <summary>
        /// Calls SetStresstestStarted(DateTime start)
        /// </summary>
        /// <param name="stresstestResults"></param>
        public void SetStresstestResults(StresstestResults stresstestResults)
        {
            _stresstestResults = stresstestResults;
            SetStresstestStarted(_stresstestResults.Metrics.StartMeasuringRuntime);
        }
        /// <summary>
        /// Sets the label.
        /// </summary>
        /// <param name="start"></param>
        public void SetStresstestStarted(DateTime start)
        {
            lblStarted.Text = "Test started at " + start;
            epnlMessages.BeginOfTimeFrame = start;
        }
        public void UpdateFastResults()
        {
            if (_stresstestResults != null)
            {
            Retry:
                try
                {
                    _stresstestResults.RefreshLogEntryResultMetrics();
                }
                catch
                {
                    goto Retry;
                }
                SetRerunning(_stresstestResults.CurrentRunDoneOnce);
                RefreshFastResultsInGui();
            }
        }
        public void SetRerunning(bool rerun)
        {
            btnRerunning.Visible = rerun;
        }
        /// <summary>
        /// Sets the '; ran ...' label and the lvw entries.
        /// </summary>
        private void RefreshFastResultsInGui()
        {
            IResult result;
            Metrics metrics = _stresstestResults.Metrics;

            SetMeasuredRunTime(metrics.MeasuredRunTime, _stresstestResults.EstimatedRuntimeLeft);

            chkReadeble.Visible = true;
            foreach (ListViewItem lvwi in _fastResults)
            {
                result = lvwi.Tag as IResult;
                if (result != null)
                {
                    metrics = result.Metrics;
                    int startIndex = lvwi.SubItems.Count - 6;

                    string estimatedRuntimeLeft = null;
                    if (chkReadeble.Checked)
                    {
                        clmFRLRuntimeLeft.Text = "Time Left";
                        estimatedRuntimeLeft = result.EstimatedRuntimeLeft.ToShortFormattedString();
                    }
                    else
                    {
                        clmFRLRuntimeLeft.Text = "Time Left in ms";
                        estimatedRuntimeLeft = (((double)result.EstimatedRuntimeLeft.Ticks) / TimeSpan.TicksPerMillisecond).ToString();
                    }

                    //Only update the ones needed to be updated
                    if (lvwi.SubItems[startIndex].Text != metrics.TotalLogEntriesProcessed + " / " + metrics.TotalLogEntries ||
                        lvwi.SubItems[1].Text != estimatedRuntimeLeft ||
                        metrics.TotalLogEntriesProcessed < metrics.TotalLogEntries)
                    {
                        lvwi.SubItems[1].Text = estimatedRuntimeLeft;

                        string measuredRunTime = null;
                        if (chkReadeble.Checked)
                        {
                            clmFRLMeasuredRuntime.Text = "Measured Time";
                            measuredRunTime = metrics.MeasuredRunTime.ToShortFormattedString();
                        }
                        else
                        {
                            clmFRLMeasuredRuntime.Text = "Measured Time in ms";
                            measuredRunTime = (((double)metrics.MeasuredRunTime.Ticks) / TimeSpan.TicksPerMillisecond).ToString();
                        }
                        lvwi.SubItems[2].Text = measuredRunTime;

                        lvwi.SubItems[startIndex].Text = metrics.TotalLogEntriesProcessed + " / " + metrics.TotalLogEntries;
                        lvwi.SubItems[startIndex + 1].Text = Math.Round((metrics.TotalLogEntriesProcessedPerTick * TimeSpan.TicksPerSecond), 4).ToString();
                        lvwi.SubItems[startIndex + 2].Text = metrics.AverageTimeToLastByte.TotalMilliseconds.ToString();
                        lvwi.SubItems[startIndex + 3].Text = metrics.MaxTimeToLastByte.TotalMilliseconds.ToString();
                        lvwi.SubItems[startIndex + 4].Text = metrics.AverageDelay.TotalMilliseconds.ToString();
                        lvwi.SubItems[startIndex + 5].Text = metrics.Errors.ToString();
                    }
                }
            }
        }
        /// <summary>
        /// Sets the '; ran ...' label.
        /// </summary>
        /// <param name="metrics"></param>
        public void SetMeasuredRunTime(TimeSpan measuredRunTime, TimeSpan estimatedRuntimeLeft)
        {
            lblMeasuredRuntime.Text = "; ran " + measuredRunTime.ToShortFormattedString();

            if (lblStopped.Text.StartsWith("and finished at "))
                epnlMessages.EndOfTimeFrame = DateTime.Now;
            else
                epnlMessages.EndOfTimeFrame = DateTime.Now + estimatedRuntimeLeft;

            epnlMessages.SetProgressBarToNow();
        }
        public void ClearFastResults()
        {
            _fastResults.Clear();
            lvwFastResultsListing.Items.Clear();
        }
        public void AddFastResult(ConcurrentUsersResult result)
        {
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

            _fastResults.Add(lvwi);

            if (cboDrillDown.SelectedIndex == 0)
            {
                lvwFastResultsListing.Items.Add(lvwi);
                UpdateFastResults();
            }

            for (int i = 0; i < lvwFastResultsListing.Items.Count - 1; i++)
            {
                sub = lvwFastResultsListing.Items[i].SubItems["LogEntriesProcessed"];
                if (sub.Font.Bold)
                    sub.Font = new Font(sub.Font, FontStyle.Regular);
            }
        }
        public void AddFastResult(PrecisionResult result)
        {
            ListViewItem lvwi = new ListViewItem(result.Metrics.StartMeasuringRuntime.ToString());
            lvwi.UseItemStyleForSubItems = false;

            Font font = new Font("Consolas", 10.25f);

            lvwi.Font = font;
            lvwi.Tag = result;

            lvwi.SubItems.Add(result.EstimatedRuntimeLeft.ToShortFormattedString()).Font = font;
            lvwi.SubItems.Add(result.Metrics.MeasuredRunTime.ToShortFormattedString()).Font = font;


            lvwi.SubItems.Add((GetParent(lvwi).Tag as ConcurrentUsersResult).ConcurrentUsers.ToString()).Font = font;
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

            _fastResults.Add(lvwi);

            if (cboDrillDown.SelectedIndex == 1)
            {
                lvwFastResultsListing.Items.Add(lvwi);
                UpdateFastResults();
            }

            for (int i = 0; i < lvwFastResultsListing.Items.Count - 1; i++)
            {
                sub = lvwFastResultsListing.Items[i].SubItems["LogEntriesProcessed"];
                if (sub.Font.Bold)
                    sub.Font = new Font(sub.Font, FontStyle.Regular);
            }
        }
        private ListViewItem GetParent(ListViewItem child)
        {
            int index = _fastResults.IndexOf(child) - 1;
            if (index == -2)
                index = _fastResults.Count - 1;

            if (child.Tag is PrecisionResult)
                for (int i = index; i > -1; i--)
                {
                    ListViewItem item = _fastResults[i];
                    if (item.Tag is ConcurrentUsersResult)
                        return item;
                }
            else if (child.Tag is RunResult)
                for (int i = index; i > -1; i--)
                {
                    ListViewItem item = _fastResults[i];
                    if (item.Tag is PrecisionResult)
                        return item;
                }
            return null;
        }
        public void AddFastResult(RunResult result)
        {
            ListViewItem lvwi = new ListViewItem(result.Metrics.StartMeasuringRuntime.ToString());
            lvwi.UseItemStyleForSubItems = false;
            Font font = new Font("Consolas", 10.25f);

            lvwi.Font = font;
            lvwi.Tag = result;

            lvwi.SubItems.Add(result.EstimatedRuntimeLeft.ToShortFormattedString()).Font = font;
            lvwi.SubItems.Add(result.Metrics.MeasuredRunTime.ToShortFormattedString()).Font = font;

            ListViewItem parent = GetParent(lvwi);
            ListViewItem parentParent = GetParent(parent);

            lvwi.SubItems.Add((parentParent.Tag as ConcurrentUsersResult).ConcurrentUsers.ToString()).Font = font;
            lvwi.SubItems.Add((((parent.Tag as PrecisionResult)).Precision + 1).ToString()).Font = font;
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

            _fastResults.Add(lvwi);

            if (cboDrillDown.SelectedIndex == 2)
            {
                lvwFastResultsListing.Items.Add(lvwi);
                UpdateFastResults();
            }

            for (int i = 0; i < lvwFastResultsListing.Items.Count - 1; i++)
            {
                sub = lvwFastResultsListing.Items[i].SubItems["LogEntriesProcessed"];
                if (sub.Font.Bold)
                    sub.Font = new Font(sub.Font, FontStyle.Regular);
            }

            SetRerunning(result.RunDoneOnce);
        }

        /// <summary>
        /// 0 = Concurrent Users, 1 = precision, 2 = run
        /// </summary>
        /// <param name="level"></param>
        public void DrillDown(int level)
        {
            cboDrillDown.SelectedIndex = level;
        }
        private void btnRerunning_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The current run is rerunning until the slowest current run on another slave is finished (aka Break on Last Run Sync).", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// label updates in visibility
        /// </summary>
        public void SetStresstestStopped()
        {
            lblUpdatesIn.Visible = false;
            btnRerunning.Visible = false;
        }
        /// <summary>
        /// label updates in visibility + label stopped forecolor and text.
        /// </summary>
        /// <param name="stresstestResult">If == busy, the label text will be cleared.</param>
        /// <param name="message">If null, no message is appended.</param>
        public void SetStresstestStopped(StresstestResult stresstestResult, string message = null)
        {
            try
            {
                switch (stresstestResult)
                {
                    case StresstestResult.Ok:
                        lblStopped.ForeColor = Color.Green;
                        lblStopped.Text = "and finished at " + DateTime.Now;
                        if (message != null)
                        {
                            AppendMessages(message, Color.LightGreen);
                            LogWrapper.LogByLevel(message, LogLevel.Info);
                        }

                        epnlMessages.EndOfTimeFrame = DateTime.Now;
                        epnlMessages.SetProgressBarToNow();

                        SetStresstestStopped();
                        break;
                    case StresstestResult.Error:
                        lblStopped.ForeColor = Color.Red;
                        lblStopped.Text = " failed at " + DateTime.Now;
                        if (message != null)
                        {
                            AppendMessages(message, LogLevel.Error);
                            LogWrapper.LogByLevel(message, LogLevel.Error);
                        }
                        SetStresstestStopped();
                        break;
                    case StresstestResult.Cancelled:
                        lblStopped.ForeColor = Color.Orange;
                        lblStopped.Text = "and cancelled at " + DateTime.Now;
                        if (message != null)
                        {
                            AppendMessages(message, LogLevel.Warning);
                            LogWrapper.LogByLevel(message, LogLevel.Warning);
                        }

                        SetStresstestStopped();
                        break;
                    default:
                        lblStopped.Text = string.Empty;
                        lblUpdatesIn.Visible = true;
                        break;
                }
            }
            catch { }
        }
        public void AppendMessages(string message, LogLevel logLevel = LogLevel.Info)
        {
            Color[] c = new Color[] { Color.DarkGray, Color.Orange, Color.Red };
            AppendMessages(message, c[(int)logLevel], logLevel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventColor">a custom color if you need one</param>
        /// <param name="logLevel"></param>
        public void AppendMessages(string message, Color eventColor, LogLevel logLevel = LogLevel.Info)
        {
            try
            {
                epnlMessages.AddEvent((EventViewEventType)logLevel, eventColor, message);
            }
            catch { }
        }
        /// <summary>
        /// Get all events (messages).
        /// </summary>
        /// <returns></returns>
        public List<EventPanelEvent> GetEvents()
        {
            return epnlMessages.GetEvents();
        }
        /// <summary>
        /// Set events
        /// </summary>
        public void SetEvents(List<EventPanelEvent> events)
        {
            if (this.IsDisposed)
                return;
            LockWindowUpdate(this.Handle.ToInt32());
            epnlMessages.ClearEvents();
            foreach (var epe in events)
                epnlMessages.AddEvent(epe.EventType, epe.EventProgressBarEventColor, epe.Message, epe.At);
            LockWindowUpdate(0);
        }
        public void ClearEvents()
        {
            epnlMessages.ClearEvents();
        }
        /// <summary>
        /// Show event message at the right date time, use this if you have an external event progress bar.
        /// </summary>
        /// <param name="at"></param>
        public void ShowEvent(DateTime at)
        {
            epnlMessages.ShowEvent(at);
        }
        private void cboDrillDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvwFastResultsListing.SuspendLayout();
            lvwFastResultsListing.Items.Clear();
            switch (cboDrillDown.SelectedIndex)
            {
                case 0:
                    if (!lvwFastResultsListing.Columns.Contains(clmFRLConcurrentUsers))
                    {
                        lvwFastResultsListing.Columns.Insert(3, clmFRLConcurrentUsers);
                        clmFRLConcurrentUsers.Width = -2;
                    }
                    if (lvwFastResultsListing.Columns.Contains(clmFRLPrecision))
                        lvwFastResultsListing.Columns.Remove(clmFRLPrecision);
                    if (lvwFastResultsListing.Columns.Contains(clmFRLRun))
                        lvwFastResultsListing.Columns.Remove(clmFRLRun);

                    foreach (ListViewItem lvwi in _fastResults)
                        if (lvwi.Tag is ConcurrentUsersResult)
                            lvwFastResultsListing.Items.Add(lvwi);
                    break;
                case 1:
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

                    foreach (ListViewItem lvwi in _fastResults)
                        if (lvwi.Tag is PrecisionResult)
                            lvwFastResultsListing.Items.Add(lvwi);
                    break;
                case 2:
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

                    foreach (ListViewItem lvwi in _fastResults)
                        if (lvwi.Tag is RunResult)
                            lvwFastResultsListing.Items.Add(lvwi);
                    break;
            }
            for (int i = 0; i < lvwFastResultsListing.Items.Count - 1; i++)
            {
                ListViewItem.ListViewSubItem sub = lvwFastResultsListing.Items[i].SubItems["LogEntriesProcessed"];
                if (sub.Font.Bold)
                    sub.Font = new Font(sub.Font, FontStyle.Regular);
            }
            lvwFastResultsListing.ResumeLayout();
        }
        private void chkReadeble_CheckedChanged(object sender, EventArgs e)
        {
            if (_stresstestResults != null)
                RefreshFastResultsInGui();
        }
        private void btnSaveDisplayedResults_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Where do you want to save the displayed results?";
            sfd.FileName = kvpStresstest.Key.ReplaceInvalidWindowsFilenameChars('_');
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
        private void btnExport_Click(object sender, EventArgs e)
        {
            epnlMessages.Export();
        }
        // Gui correction, the epnl wil not be sized correctly otherwise.
        private void StresstestControl_SizeChanged(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());
            epnlMessages.Collapsed = !epnlMessages.Collapsed;
            epnlMessages.Collapsed = !epnlMessages.Collapsed;
            LockWindowUpdate(0);
        }
        // Set the splitter distance of the splitcontainer if collapsed has changed.
        private void epnlMessages_CollapsedChanged(object sender, EventArgs e)
        {
            epnlMessages.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            try
            {
                int distance = splitContainer.Panel2.Height - epnlMessages.Bottom;
                if (splitContainer.SplitterDistance + distance > 0)
                    splitContainer.SplitterDistance += distance;

                splitContainer.IsSplitterFixed = epnlMessages.Collapsed;
                this.BackColor = splitContainer.IsSplitterFixed ? Color.Transparent : SystemColors.Control;
            }
            catch { }

            epnlMessages.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        }
        #endregion
    }
}
