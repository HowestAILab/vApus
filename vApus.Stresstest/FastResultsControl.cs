/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using vApus.Results;
using vApus.StressTest.Properties;
using vApus.Util;

namespace vApus.StressTest {
    public partial class FastResultsControl : UserControl {

        #region Fields
        private List<StressTestMetrics> _concurrencyStressTestMetrics = new List<StressTestMetrics>();
        private List<object[]> _concurrencyStressTestMetricsRows = new List<object[]>();
        private List<StressTestMetrics> _runStressTestMetrics = new List<StressTestMetrics>();
        private List<object[]> _runStressTestMetricsRows = new List<object[]>();

        /// <summary>
        ///     Enables Auto scroll to end of fast results when appropriate.
        /// </summary>
        private bool _keepFastResultsAtEnd = true;
        /// <summary>
        /// Not visible on a slave in a distributed test.
        /// </summary>
        private bool _monitorConfigurationControlAndKeyValuePairControlVisible;

        //For distributed test
        private DateTime _stressTestStartedAt = DateTime.Now;
        private TimeSpan _measuredRuntime = new TimeSpan();
        #endregion

        #region Properties
        public bool HasResults { get { return _concurrencyStressTestMetricsRows.Count != 0; } }

        /// <summary>
        /// Should be false for a slave in a distributed test.
        /// </summary>
        public bool MonitorConfigurationControlAndKeyValuePairControlVisible {
            get { return _monitorConfigurationControlAndKeyValuePairControlVisible; }
            set {
                _monitorConfigurationControlAndKeyValuePairControlVisible = value;
                kvpMonitor.Visible = _monitorConfigurationControlAndKeyValuePairControlVisible;
            }
        }

        public DateTime StressTestStartedAt {
            get { return _stressTestStartedAt; }
        }
        /// <summary>
        /// For distributed test
        /// </summary>
        public TimeSpan MeasuredRuntime {
            get { return _measuredRuntime; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Used for displaying the fast results: stress test progress; Progress messages and local hardware monitoring.
        /// </summary>
        public FastResultsControl() {
            InitializeComponent();

            //Double buffer the datagridview.
            (dgvFastResults).GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(dgvFastResults, true);

            cboDrillDown.SelectedIndex = 0;
            btnCollapseExpand.PerformClick();
            epnlMessages.Collapsed = true;

            //Stupid workaround.
            dgvFastResults.ColumnHeadersDefaultCellStyle.Font = new Font(dgvFastResults.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
        }
        #endregion

        #region Functions
        /// <summary>
        ///     Resets everything to the initial state.
        /// </summary>
        public void SetStressTestInitialized() {
            _measuredRuntime = new TimeSpan();
            _stressTestStartedAt = DateTime.Now;

            epnlMessages.ClearEvents();

            lblStarted.Text = string.Empty;
            lblMeasuredRuntime.Text = string.Empty;
            lblStopped.Text = string.Empty;


            ClearFastResults();
        }

        public void SetConfigurationControls(StressTest stressTest) {
            SetConfigurationControls(stressTest.ToString(), stressTest.Connection, stressTest.ConnectionProxy,
                                     stressTest.Scenarios, stressTest.ScenarioRuleSet,
                                     stressTest.Monitors, stressTest.Concurrencies, stressTest.Runs,
                                     stressTest.InitialMinimumDelay, stressTest.InitialMaximumDelay,
                                     stressTest.MinimumDelay, stressTest.MaximumDelay, stressTest.Shuffle,
                                     stressTest.ActionDistribution, stressTest.MaximumNumberOfUserActions,
                                     stressTest.MonitorBefore, stressTest.MonitorAfter, stressTest.UseParallelExecutionOfRequests,
                                     stressTest.MaximumPersistentConnections, stressTest.PersistentConnectionsPerHostname);
        }
        public void SetConfigurationControls(string stressTest, Connection connection, string connectionProxy, KeyValuePair<Scenario, uint>[] scenarios, string scenarioRuleSet, Monitor.Monitor[] monitors, int[] concurrencies,
                                             int runs, int initialMinimumDelay, int initialMaximumDelay, int minimumDelay, int maximumDelay, bool shuffle, bool actionDistribution, int maximumNumberOfUserActions, int monitorBefore, int monitorAfter,
                                             bool useParallelExecutionOfRequests, int maximumPersistentConnections, int persistentConnectionsPerHostname) {
            kvpStressTest.Key = stressTest;
            kvpConnection.Key = connection.ToString();
            kvpConnectionProxy.Key = connectionProxy;

            var scenarioKeys = new List<Scenario>(scenarios.Length);
            foreach (var kvp in scenarios)
                scenarioKeys.Add(kvp.Key);

            kvpScenario.Key = scenarioKeys.Combine(", ");
            kvpScenarioRuleSet.Key = scenarioRuleSet;

            if (_monitorConfigurationControlAndKeyValuePairControlVisible) {
                if (monitors == null || monitors.Length == 0) {
                    kvpMonitor.Key = "No monitor(s)";
                    kvpMonitor.BackColor = Color.Orange;
                }
                else {
                    kvpMonitor.Key = monitors.Combine(", ");
                    kvpMonitor.BackColor = Color.LightBlue;

                    foreach (var monitor in monitors) {
                        var lbtnMonitor = new LinkButton();
                        lbtnMonitor.Text = monitor.ToString();
                        lbtnMonitor.RadioButtonBehavior = true;
                        lbtnMonitor.Margin = new Padding(3, 6, 0, 3);
                        lbtnMonitor.ActiveChanged += lbtnMonitor_ActiveChanged;

                        flpFastResultsHeader.Controls.Add(lbtnMonitor);
                    }
                }
            }

            kvpConcurrencies.Value = concurrencies.Combine(", ");

            kvpRuns.Value = runs.ToString();
            kvpInitialDelay.Value = (initialMinimumDelay == initialMaximumDelay
                                  ? initialMinimumDelay.ToString()
                                  : initialMinimumDelay + " - " + initialMaximumDelay) + " ms";
            kvpDelay.Value = (minimumDelay == maximumDelay
                                  ? minimumDelay.ToString()
                                  : minimumDelay + " - " + maximumDelay) + " ms";
            kvpShuffle.Value = shuffle ? "Yes" : "No";
            kvpActionDistribution.Value = actionDistribution ? "Yes" : "No";
            kvpMaximumNumberOfUserActions.Value = maximumNumberOfUserActions == 0 ? "N/A" : maximumNumberOfUserActions.ToString();
            kvpMonitorBefore.Value = monitorBefore + (monitorBefore == 1 ? " minute" : " minutes");
            kvpMonitorAfter.Value = monitorAfter + (monitorAfter == 1 ? " minute" : " minutes");

            if (useParallelExecutionOfRequests) {
                kvpParallelExecutions.Value = (maximumPersistentConnections == 0 ? "∞" : maximumPersistentConnections.ToString()) + " maximum persistent connections, ";
                kvpParallelExecutions.Value += (persistentConnectionsPerHostname == 0 ? "∞" : persistentConnectionsPerHostname.ToString()) + " persistent connections per hostname";

            }
            else {
                kvpParallelExecutions.Value = "No";
            }

            flpConfiguration.ScrollControlIntoView(pnlScrollConfigTo);

            //Reinit the datagridview.
            RefreshFastResults();
        }

        /// <summary>
        ///     Sets the label.
        /// </summary>
        /// <param name="start"></param>
        public void SetStressTestStarted(DateTime at) {
            _stressTestStartedAt = at;
            lblStarted.Text = "Test started at " + at.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
            epnlMessages.BeginOfTimeFrame = at;
        }

        public void SetRerunning(bool rerun) {
            btnRerunning.Visible = rerun;
        }

        public void ClearFastResults(bool clearLabels = true) {
            if (clearLabels) lblUpdatesIn.Text = lblStarted.Text = lblMeasuredRuntime.Text = lblStopped.Text = string.Empty;

            dgvFastResults.RowCount = 0;

            _concurrencyStressTestMetrics = new List<StressTestMetrics>();
            _concurrencyStressTestMetricsRows = new List<object[]>();
            _runStressTestMetrics = new List<StressTestMetrics>();
            _runStressTestMetricsRows = new List<object[]>();

            _keepFastResultsAtEnd = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="countDown">smaller than 0 for paused or unknown</param>
        public void SetCountDownProgressDelay(int countDown) {
            if (countDown > -1) {
                lblUpdatesIn.ForeColor = Color.SteelBlue;
                lblUpdatesIn.Text = "updates in " + countDown;
                lblUpdatesIn.Image = null;
            }
            else {
                lblUpdatesIn.ForeColor = Color.DarkGray;
                lblUpdatesIn.Text = "updates in  ";
                lblUpdatesIn.Image = Resources.Wait;
            }
        }

        /// <summary>
        ///     Leave all empty for the default values.
        /// </summary>
        /// <param name="threadsInUse"></param>
        /// <param name="cpuUsage"></param>
        /// <param name="contextSwitchesPerSecond"></param>
        /// <param name="threadQueueLengthPerSecond"></param>
        /// <param name="threadContentionsPerSecond"></param>
        /// <param name="memoryUsage"></param>
        /// <param name="totalVisibleMemory"></param>
        /// <returns>The last warning if any.</returns>
        public string SetClientMonitoring(int threadsInUse = 0, float cpuUsage = -1f, int memoryUsage = -1, int totalVisibleMemory = -1,
            string nic = "NIC", float nicBandwidth = -1, float nicsSent = -1, float nicsReceived = -1) {

            string lastWarning = string.Empty;

            kvmThreadsInUse.Value = threadsInUse.ToString();
            if (cpuUsage == -1) {
                kvmCPUUsage.Value = "N/A";
            }
            else {
                kvmCPUUsage.Value = Math.Round(cpuUsage, 2).ToString() + " %";

                if (cpuUsage < 60) {
                    kvmCPUUsage.BackColor = Color.GhostWhite;
                }
                else {
                    kvmCPUUsage.BackColor = Color.Orange;

                    lastWarning = cpuUsage + " % CPU Usage";
                    AddEvent(lastWarning, Level.Warning);
                }
            }

            if (memoryUsage == -1 || totalVisibleMemory == -1) {
                kvmMemoryUsage.Value = "N/A";
            }
            else {
                kvmMemoryUsage.Value = memoryUsage.ToString() + " / " + totalVisibleMemory + " MB";
                if (memoryUsage < 0.9 * totalVisibleMemory) {
                    kvmMemoryUsage.BackColor = Color.GhostWhite;
                }
                else if (memoryUsage != 0) {
                    kvmMemoryUsage.BackColor = Color.Orange;

                    lastWarning = memoryUsage + " of " + totalVisibleMemory + " MB used";
                    AddEvent(lastWarning, Level.Warning);
                }
            }
            if (string.IsNullOrEmpty(nic)) {
                kvmNic.Key = kvmNic.Value = string.Empty;
            }
            else {
                kvmNic.Key = nic;
                if (nicBandwidth == -1)
                    kvmNic.Value = "N/A";
                else
                    kvmNic.Value = nicBandwidth + " Mbps";
            }
            if (nicsSent == -1) {
                kvmNicsSent.Value = "N/A";
            }
            else {
                kvmNicsSent.Value = Math.Round(nicsSent, 2).ToString() + " %";
                if (nicsSent < 90) {
                    kvmNicsSent.BackColor = Color.GhostWhite;
                }
                else if (!float.IsPositiveInfinity(nicsSent) && !float.IsNegativeInfinity(nicsSent)) {
                    kvmNicsSent.BackColor = Color.Orange;

                    lastWarning = nicsSent + " % NIC Usage (Sent)";
                    AddEvent(lastWarning, Level.Warning);
                }
            }
            if (nicsReceived == -1) {
                kvmNicsReceived.Value = "N/A";
            }
            else {
                kvmNicsReceived.Value = Math.Round(nicsReceived, 2).ToString() + " %";
                if (nicsReceived < 90) {
                    kvmNicsReceived.BackColor = Color.GhostWhite;
                }
                else if (!float.IsPositiveInfinity(nicsReceived) && !float.IsNegativeInfinity(nicsReceived)) {
                    kvmNicsReceived.BackColor = Color.Orange;

                    lastWarning = nicsReceived + " % NIC Usage (Received)";
                    AddEvent(lastWarning, Level.Warning);
                }
            }

            return lastWarning;
        }

        /// <summary>
        /// </summary>
        /// <param name="metrics"></param>
        /// <param name="setMeasuredRunTime">Should only be true if the test is running or just done.</param>
        public void UpdateFastConcurrencyResults(List<StressTestMetrics> metrics, bool setMeasuredRunTime = true) {
            _concurrencyStressTestMetrics = metrics;
            _concurrencyStressTestMetricsRows = FastStressTestMetricsHelper.MetricsToRows(metrics, chkReadable.Checked);
            if (cboDrillDown.SelectedIndex == 0) {
                int count = _concurrencyStressTestMetricsRows.Count;
                if (dgvFastResults.RowCount == count && count != 0) dgvFastResults.InvalidateRow(count - 1); else dgvFastResults.RowCount = count;
                dgvFastResults.AutoResizeColumns();
                KeepFastResultsAtEnd();
            }
            if (setMeasuredRunTime) SetMeasuredRuntime();
        }

        /// <summary>
        /// </summary>
        /// <param name="metrics"></param>
        /// <param name="setMeasuredRunTime">Should only be true if the test is running or just done.</param>
        public void UpdateFastRunResults(List<StressTestMetrics> metrics, bool setMeasuredRunTime = true) {
            _runStressTestMetrics = metrics;
            _runStressTestMetricsRows = FastStressTestMetricsHelper.MetricsToRows(metrics, chkReadable.Checked);
            if (cboDrillDown.SelectedIndex == 1) {
                int count = _runStressTestMetricsRows.Count;
                if (dgvFastResults.RowCount == count && count != 0) dgvFastResults.InvalidateRow(count - 1); else dgvFastResults.RowCount = count;
                dgvFastResults.AutoResizeColumns();
                KeepFastResultsAtEnd();
            }
            if (setMeasuredRunTime) SetMeasuredRuntime();
        }

        private void dgvFastResults_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            try {
                List<object[]> rows = (cboDrillDown.SelectedIndex == 0 ? _concurrencyStressTestMetricsRows : _runStressTestMetricsRows);
                if (rows.Count > e.RowIndex && rows[0].Length > e.ColumnIndex)
                    e.Value = rows[e.RowIndex][e.ColumnIndex];
            }
            catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed displaying cell value.", ex, new object[] { sender, e });
            }
        }

        /// <summary>
        ///     When the row count changes.
        /// </summary>
        private void KeepFastResultsAtEnd() {
            if (_keepFastResultsAtEnd && dgvFastResults.RowCount != 0) {
                dgvFastResults.Scroll -= dgvFastResults_Scroll;
                dgvFastResults.FirstDisplayedScrollingRowIndex = dgvFastResults.RowCount - 1;
                dgvFastResults.Scroll += dgvFastResults_Scroll;
            }
        }

        private void dgvFastResults_Scroll(object sender, ScrollEventArgs e) {
            var verticalScrollBar = typeof(DataGridView).GetProperty("VerticalScrollBar", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                                                        .GetValue(dgvFastResults) as ScrollBar;
            _keepFastResultsAtEnd = (verticalScrollBar.Value + verticalScrollBar.LargeChange + 1) >= verticalScrollBar.Maximum;
        }

        /// <summary>
        ///     Sets the ', ran ...' label.
        /// </summary>
        /// <param name="metrics"></param>
        private void SetMeasuredRuntime() {
            epnlMessages.SetEndOfTimeFrameToNow();
            _measuredRuntime = epnlMessages.EndOfTimeFrame - epnlMessages.BeginOfTimeFrame;
            _measuredRuntime = _measuredRuntime.Subtract(new TimeSpan(_measuredRuntime.Ticks % TimeSpan.TicksPerSecond));
            if (_measuredRuntime.TotalSeconds > 1)
                lblMeasuredRuntime.Text = ", ran " + _measuredRuntime.ToShortFormattedString(true);
        }
        public void SetMeasuredRuntime(TimeSpan measuredRuntime) {
            epnlMessages.SetEndOfTimeFrameTo(epnlMessages.BeginOfTimeFrame + measuredRuntime);
            measuredRuntime = _measuredRuntime.Subtract(new TimeSpan(measuredRuntime.Ticks % TimeSpan.TicksPerSecond));
            if (measuredRuntime.TotalSeconds > 1) {
                string s = ", ran " + measuredRuntime.ToShortFormattedString(true);
                if (lblMeasuredRuntime.Text != s) lblMeasuredRuntime.Text = s;
            }
        }

        private void btnRerunning_Click(object sender, EventArgs e) {
            MessageBox.Show(
                "The current run is rerunning until the slowest current run on another slave is finished (aka Break on Last Run Sync).",
                string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        ///     label updates in visibility
        /// </summary>
        public void SetStressTestStopped() {
            lblUpdatesIn.Text = string.Empty;
            btnRerunning.Visible = false;
            CancelAddingStaticEventsToGui();
        }

        public void CancelAddingStaticEventsToGui() { epnlMessages.CancelAddingStaticEventsToGui(); }

        /// <summary>
        ///     label updates in visibility + label stopped forecolor and text.
        /// </summary>
        /// <param name="stressTestResult">If == busy, the label text will be cleared.</param>
        /// <param name="message">If null, no message is appended.</param>
        public void SetStressTestStopped(StressTestStatus stressTestResult, TimeSpan measuredRuntime, out string message) {
            SetMeasuredRuntime(measuredRuntime);
            __SetStressTestStopped(stressTestResult, out message);
        }

        /// <summary>
        ///     label updates in visibility + label stopped forecolor and text.
        /// </summary>
        /// <param name="stressTestResult">If == busy, the label text will be cleared.</param>
        /// <param name="message">If null, no message is appended.</param>
        public void SetStressTestStopped(StressTestStatus stressTestResult, out string message) {
            SetMeasuredRuntime();
            __SetStressTestStopped(stressTestResult, out message);
        }

        /// <summary>
        ///     label updates in visibility + label stopped forecolor and text.
        /// </summary>
        /// <param name="stressTestResult">If == busy, the label text will be cleared.</param>
        /// <param name="message">The appended message. level == info.</param>
        /// <param name="exception"></param>
        private void __SetStressTestStopped(StressTestStatus stressTestResult, out string message, Exception exception = null) {
            message = string.Empty;
            try {
                if (exception != null) {
                    var logger = Loggers.GetLogger<FileLogger>();
                    message = exception.Message + "\n" + exception.StackTrace + "\n\nSee " + logger.CurrentLogFile;
                    Loggers.Log(Level.Error, message);
                    AddEvent(message, Color.Red);
                }
                string lblStoppedText = null;
                switch (stressTestResult) {
                    case StressTestStatus.Ok:
                        lblStoppedText = "and finished at " + epnlMessages.EndOfTimeFrame;
                        if (lblStopped.Text != lblStoppedText) {
                            lblStopped.ForeColor = Color.Green;
                            lblStopped.Text = lblStoppedText;

                            message = string.Format("The test completed succesfully in {0}.", (epnlMessages.EndOfTimeFrame - epnlMessages.BeginOfTimeFrame).ToShortFormattedString(true));
                            Loggers.Log(message);
                            AddEvent(message, Color.GreenYellow);

                            SetStressTestStopped();
                        }
                        break;
                    case StressTestStatus.Error:
                        lblStoppedText = "failed at " + epnlMessages.EndOfTimeFrame;
                        if (lblStopped.Text != lblStoppedText) {
                            lblStopped.ForeColor = Color.Red;
                            lblStopped.Text = lblStoppedText;

                            message = "The stress test failed." + (exception == null ? "" : "\n" + exception.ToString());

                            SetStressTestStopped();
                        }
                        break;
                    case StressTestStatus.Cancelled:
                        lblStoppedText = "and cancelled at " + epnlMessages.EndOfTimeFrame;
                        if (lblStopped.Text != lblStoppedText) {
                            lblStopped.ForeColor = Color.Orange;
                            lblStopped.Text = lblStoppedText;

                            message = "The stress test was cancelled.";
                            Loggers.Log(message);
                            AddEvent(message, Color.Orange);

                            SetStressTestStopped();
                        }
                        break;
                    default:
                        lblStopped.Text = string.Empty;
                        break;
                }
                SetMeasuredRuntime();
            }
            catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed setting stress test stopped.", ex, new object[] { stressTestResult, exception });
            }
        }

        public void SetEventFilter(EventViewEventType filter) { epnlMessages.Filter = filter; }
        /// <summary>
        /// Append stress test progress and error messages.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        public void AddEvent(string message, Level logLevel = Level.Info) {
            var c = new[] { Color.DarkGray, Color.Orange, Color.Red };
            AddEvent(message, c[(int)logLevel], logLevel);
        }
        /// <summary>
        /// Append stress test progress and error messages.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventColor">a custom color if you need one</param>
        /// <param name="logLevel"></param>
        public void AddEvent(string message, Color eventColor, Level logLevel = Level.Info) {
            try {
                epnlMessages.AddEvent((EventViewEventType)logLevel, eventColor, message);
            }
            catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed adding event.", ex, new object[] { message, eventColor, logLevel });
            }
        }

        /// <summary>
        ///     Get all events (messages).
        /// </summary>
        /// <returns></returns>
        public EventPanelEvent[] GetEvents() { return epnlMessages.GetEvents(); }

        /// <summary>
        ///     Set events
        /// </summary>
        public void SetEvents(EventPanelEvent[] events) {
            if (IsDisposed) return;
            int toSetCount = events.Length;
            int currentEventCount = epnlMessages.EventCount;

            if (currentEventCount == 0 || toSetCount < currentEventCount) {
                epnlMessages.SetEvents(events);
            }
            else if (toSetCount > currentEventCount) {
                var eventSubset = new List<EventPanelEvent>(toSetCount - currentEventCount);
                for (int i = currentEventCount; i != toSetCount; i++)
                    eventSubset.Add(events[i]);

                epnlMessages.AddEvents(eventSubset);
            }
        }

        public void ClearEvents() { epnlMessages.ClearEvents(); }

        /// <summary>
        ///     Show / scroll to event message at the right date time, use this if you have an external event progress bar.
        /// </summary>
        /// <param name="at"></param>
        public void ShowEvent(DateTime at) { epnlMessages.ShowEvent(at); }

        private void lbtnMonitor_ActiveChanged(object sender, EventArgs e) {
            if ((sender as LinkButton).Active) RefreshFastResults();
        }
        private void cboDrillDown_SelectedIndexChanged(object sender, EventArgs e) {
            RefreshFastResults();
        }

        private void chkReadable_CheckedChanged(object sender, EventArgs e) {
            RefreshFastResults();
            toolTip.SetToolTip(chkReadable, chkReadable.Checked ? "Uncheck this if you want results you can calculate with." : "Check this if you want readable results.");
        }

        /// <summary>
        /// WHen a filter is applied (cboDrillDown_SelectedIndexChanged for instance).
        /// </summary>
        private void RefreshFastResults() {
            //Set the headers.
            dgvFastResults.Columns.Clear();

            string[] columnHeaders = cboDrillDown.SelectedIndex == 0 ? FastStressTestMetricsHelper.GetMetricsHeadersConcurrency(chkReadable.Checked) : FastStressTestMetricsHelper.GetMetricsHeadersRun(chkReadable.Checked);
            
            var clms = new DataGridViewColumn[columnHeaders.Length];
            string clmPrefix = ToString() + "clm";
            for (int headerIndex = 0; headerIndex != columnHeaders.Length; headerIndex++) {
                string header = columnHeaders[headerIndex];
                var clm = new DataGridViewTextBoxColumn();
                clm.Name = clmPrefix + header;
                clm.HeaderText = header;

                clm.SortMode = DataGridViewColumnSortMode.NotSortable;
                //To allow 2 power 32 columns.
                clm.FillWeight = 1;

                clms[headerIndex] = clm;
            }

            dgvFastResults.Columns.AddRange(clms);

            //Set the rows.
            if (cboDrillDown.SelectedIndex == 0)
                UpdateFastConcurrencyResults(_concurrencyStressTestMetrics, false);
            else
                UpdateFastRunResults(_runStressTestMetrics, false);
        }

        private void btnSaveDisplayedResults_Click(object sender, EventArgs e) {
            var sfd = new SaveFileDialog();
            sfd.Title = "Where do you want to save the displayed results?";
            sfd.FileName = kvpStressTest.Key.ReplaceInvalidWindowsFilenameChars('_');
            sfd.Filter = "TXT|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
                try {
                    using (var sw = new StreamWriter(sfd.FileName)) {
                        sw.Write(GetDisplayedResults());
                        sw.Flush();
                    }
                }
                catch {
                    MessageBox.Show("Could not access file: " + sfd.FileName, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        /// <summary>
        ///     Get the displayed results for saving to file.
        /// </summary>
        /// <returns></returns>
        private string GetDisplayedResults() {
            var sb = new StringBuilder();
            //Write column headers
            foreach (DataGridViewColumn clm in dgvFastResults.Columns) {
                sb.Append(clm.HeaderText);
                sb.Append("\t");
            }
            sb.AppendLine();

            //Select and write rows.
            List<object[]> rows =  cboDrillDown.SelectedIndex == 0 ? _concurrencyStressTestMetricsRows : _runStressTestMetricsRows;
            foreach (var row in rows) {
                sb.Append(row.Combine("\t"));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private void btnExport_Click(object sender, EventArgs e) { epnlMessages.Export(); }

        private void btnCollapseExpand_Click(object sender, EventArgs e) {
            if (btnCollapseExpand.Text == "-") {
                btnCollapseExpand.Text = "+";

                splitTop.SplitterDistance = splitTop.Panel1MinSize;
                splitTop.IsSplitterFixed = true;
                splitTop.BackColor = Color.White;
            }
            else {
                btnCollapseExpand.Text = "-";
                splitTop.SplitterDistance = 85;
                splitTop.IsSplitterFixed = false;
                splitTop.BackColor = SystemColors.Control;
            }
        }
        public void ExpandEventPanel() { epnlMessages.Collapsed = false; }
        public void CollapseEventPanel() { epnlMessages.Collapsed = true; }

        /// <summary>
        /// Set the splitter distance of the splitcontainer if collapsed has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void epnlMessages_CollapsedChanged(object sender, EventArgs e) {
            epnlMessages.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            try {
                int distance = splitContainer.SplitterDistance + splitContainer.Panel2.Height - epnlMessages.Bottom;
                if (distance >= splitContainer.Panel1MinSize && distance <= splitContainer.Height - splitContainer.Panel2MinSize)
                    splitContainer.SplitterDistance = distance;

                splitContainer.IsSplitterFixed = epnlMessages.Collapsed;
                BackColor = splitContainer.IsSplitterFixed ? Color.Transparent : SystemColors.Control;
            }
            catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed setting collapsed or expanded.", ex, new object[] { sender, e });
            }

            epnlMessages.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        }
        /// <summary>
        /// Gui correction, the epnl wil not be sized correctly otherwise.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FastResultsControl_SizeChanged(object sender, EventArgs e) {
            //SendMessageWrapper.SetWindowRedraw(Handle, false);

            epnlMessages.Collapsed = !epnlMessages.Collapsed;
            epnlMessages.Collapsed = !epnlMessages.Collapsed;

            //SendMessageWrapper.SetWindowRedraw(Handle, true);
        }
        #endregion
    }
}