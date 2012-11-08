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
using System.Reflection;
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
        private List<vApus.Results.Metrics> _concurrencyMetrics = new List<vApus.Results.Metrics>();
        private List<object[]> _concurrencyMetricsCache = new List<object[]>();
        private List<vApus.Results.Metrics> _runMetrics = new List<vApus.Results.Metrics>();
        private List<object[]> _runMetricsCache = new List<object[]>();

        private bool _monitorConfigurationControlVisible;

        /// <summary>
        /// Enables Auto scroll to end of fast results when appropriate.
        /// </summary>
        private bool _keepFastResultsAtEnd = true;

        #endregion

        #region Properties
        public int FastResultsCount
        {
            get { return dgvFastResults.RowCount; }
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
        }
        public void SetConfigurationControls(Stresstest stresstest)
        {
            SetConfigurationControls(stresstest.ToString(), stresstest.Connection, stresstest.ConnectionProxy, stresstest.Log, stresstest.LogRuleSet,
                stresstest.Monitors, stresstest.Concurrency, stresstest.Runs, stresstest.MinimumDelay,
                stresstest.MaximumDelay, stresstest.Shuffle, stresstest.Distribute, stresstest.MonitorBefore, stresstest.MonitorAfter);
        }
        public void SetConfigurationControls(string stresstest, Connection connection, string connectionProxy,
                                              Log log, string logRuleSet, Monitor.Monitor[] monitors, int[] concurrency,
                                              int runs, int minimumDelay, int maximumDelay, bool shuffle,
                                              ActionAndLogEntryDistribution distribute, int monitorBefore, int monitorAfter)
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

            kvpConcurrency.Value = concurrency.Combine(", ");

            kvpRuns.Value = runs.ToString();
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

        }


        /// <summary>
        /// Sets the label.
        /// </summary>
        /// <param name="start"></param>
        public void SetStresstestStarted(DateTime at)
        {
            lblStarted.Text = "Test started at " + at;
            epnlMessages.BeginOfTimeFrame = at;
        }
        public void SetRerunning(bool rerun)
        {
            btnRerunning.Visible = rerun;
        }
        public void ClearFastResults()
        {
            dgvFastResults.RowCount = 0;
            _concurrencyMetrics = new List<vApus.Results.Metrics>();
            _concurrencyMetricsCache = new List<object[]>();
            _runMetrics = new List<vApus.Results.Metrics>();
            _runMetricsCache = new List<object[]>();
            _keepFastResultsAtEnd = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metrics"></param>
        /// <param name="setMeasuredRunTime">Should only be true if the test is running or just done.</param>
        public void UpdateConcurrencyFastResults(List<vApus.Results.Metrics> metrics, bool setMeasuredRunTime = true)
        {
            _concurrencyMetrics = metrics;
            _concurrencyMetricsCache = vApus.Results.MetricsHelper.MetricsToRows(metrics, chkReadable.Checked);
            if (cboDrillDown.SelectedIndex == 0)
            {
                dgvFastResults.RowCount = 0;
                dgvFastResults.RowCount = _concurrencyMetricsCache.Count;
                KeepFastResultsAtEnd();
            }
            if (setMeasuredRunTime)
                SetMeasuredRunTime();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metrics"></param>
        /// <param name="setMeasuredRunTime">Should only be true if the test is running or just done.</param>
        public void UpdateRunFastResults(List<vApus.Results.Metrics> metrics, bool setMeasuredRunTime = true)
        {
            _runMetrics = metrics;
            _runMetricsCache = vApus.Results.MetricsHelper.MetricsToRows(metrics, chkReadable.Checked);
            if (cboDrillDown.SelectedIndex == 1)
            {
                dgvFastResults.RowCount = 0;
                dgvFastResults.RowCount = _runMetricsCache.Count;
                KeepFastResultsAtEnd();
            }
            if (setMeasuredRunTime)
                SetMeasuredRunTime();
        }
        private void dgvFastResults_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                List<object[]> cache = cboDrillDown.SelectedIndex == 0 ? _concurrencyMetricsCache : _runMetricsCache;
                e.Value = cache[e.RowIndex][e.ColumnIndex];
            }
            catch { }
        }
        /// <summary>
        /// When the row count changes.
        /// </summary>
        private void KeepFastResultsAtEnd()
        {
            if (_keepFastResultsAtEnd && dgvFastResults.RowCount != 0)
            {
                dgvFastResults.Scroll -= dgvFastResults_Scroll;
                dgvFastResults.FirstDisplayedScrollingRowIndex = dgvFastResults.RowCount - 1;
                dgvFastResults.Scroll += dgvFastResults_Scroll;
            }
        }
        /// <summary>
        /// Sets the '; ran ...' label.
        /// </summary>
        /// <param name="metrics"></param>
        private void SetMeasuredRunTime()
        {
            epnlMessages.SetEndOfTimeFrameToNow();

            lblMeasuredRuntime.Text = "; ran " + (epnlMessages.EndOfTimeFrame - epnlMessages.BeginOfTimeFrame).ToShortFormattedString();
        }
        /// <summary>
        /// 0 = Concurrency, 1 = runs
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
        public void SetStresstestStopped(StresstestStatus stresstestResult, Exception exception = null)
        {
            try
            {
                SetMeasuredRunTime();

                string message = null;
                if (exception != null)
                {
                    message = "The stresstest threw an exception:\n" + exception.Message + "\n\nSee " + Path.Combine(Logger.DEFAULT_LOCATION, DateTime.Now.ToString("dd-MM-yyyy") + " " + LogWrapper.Default.Logger.Name + ".txt");
                    LogWrapper.LogByLevel(message, LogLevel.Error);
                    AppendMessages(message, Color.Red);
                }
                switch (stresstestResult)
                {
                    case StresstestStatus.Ok:
                        lblStopped.ForeColor = Color.Green;
                        lblStopped.Text = "and finished at " + epnlMessages.EndOfTimeFrame;

                        message = string.Format("The test completed succesfully in {0}.", (epnlMessages.EndOfTimeFrame - epnlMessages.BeginOfTimeFrame).ToShortFormattedString());
                        LogWrapper.LogByLevel(message, LogLevel.Info);
                        AppendMessages(message, Color.GreenYellow);

                        SetStresstestStopped();
                        break;
                    case StresstestStatus.Error:
                        lblStopped.ForeColor = Color.Red;
                        lblStopped.Text = " failed at " + epnlMessages.EndOfTimeFrame;

                        SetStresstestStopped();
                        break;
                    case StresstestStatus.Cancelled:
                        lblStopped.ForeColor = Color.Orange;
                        lblStopped.Text = "and cancelled at " + epnlMessages.EndOfTimeFrame;

                        message = "The stresstest was cancelled.";
                        LogWrapper.LogByLevel(message, LogLevel.Info);
                        AppendMessages(message, Color.Orange);

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
            SetFastResultsOnGuiInteraction();
        }
        private void chkReadable_CheckedChanged(object sender, EventArgs e)
        {
            SetFastResultsOnGuiInteraction();
            toolTip.SetToolTip(chkReadable, chkReadable.Checked ? "Uncheck this if you want results you can calculate with." : "Check this if you want readable results.");
        }
        private void SetFastResultsOnGuiInteraction()
        {
            dgvFastResults.RowCount = 0;
            dgvFastResults.Columns.Clear();

            string[] columnHeaders = cboDrillDown.SelectedIndex == 0 ? (chkReadable.Checked ? vApus.Results.MetricsHelper.ReadableMetricsHeadersConcurrency : vApus.Results.MetricsHelper.CalculatableMetricsHeadersConcurrency)
                : (chkReadable.Checked ? vApus.Results.MetricsHelper.ReadableMetricsHeadersRun : vApus.Results.MetricsHelper.CalculatableMetricsHeadersRun);

            DataGridViewColumn[] clms = new DataGridViewColumn[columnHeaders.Length];
            string clmPrefix = this.ToString() + "clm";
            for (int headerIndex = 0; headerIndex != columnHeaders.Length; headerIndex++)
            {
                string header = columnHeaders[headerIndex];
                DataGridViewColumn clm = new DataGridViewTextBoxColumn();
                clm.Name = clmPrefix + header;
                clm.HeaderText = header;

                clm.SortMode = DataGridViewColumnSortMode.NotSortable;
                clm.FillWeight = 1;

                clms[headerIndex] = clm;
            }

            dgvFastResults.Columns.AddRange(clms);

            UpdateConcurrencyFastResults(_concurrencyMetrics, false);
            UpdateRunFastResults(_runMetrics, false);
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
            foreach (DataGridViewColumn clm in dgvFastResults.Columns)
            {
                sb.Append(clm.HeaderText);
                sb.Append("\t");
            }
            sb.AppendLine();
            List<object[]> cache = cboDrillDown.SelectedIndex == 0 ? _concurrencyMetricsCache : _runMetricsCache;
            foreach (var row in cache)
            {
                sb.Append(row.Combine("\t"));
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

        private void dgvFastResults_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollBar verticalScrollBar = typeof(DataGridView).GetProperty("VerticalScrollBar", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static ).GetValue(dgvFastResults) as ScrollBar;
            _keepFastResultsAtEnd = (verticalScrollBar.Value + verticalScrollBar.LargeChange + 1) >= verticalScrollBar.Maximum;
        }
    }
}