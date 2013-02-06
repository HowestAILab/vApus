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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using vApus.Results;
using vApus.Stresstest.Properties;
using vApus.Util;

namespace vApus.Stresstest {
    public partial class FastResultsControl : UserControl {
        /// <summary>
        /// For smooth updating the gui.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        /// <summary>
        ///     To show the monitor view if any.
        /// </summary>
        public event EventHandler MonitorClicked;

        #region Fields
        private List<StresstestMetrics> _concurrencyStresstestMetrics = new List<StresstestMetrics>();
        private List<object[]> _concurrencyStresstestMetricsRows = new List<object[]>();
        private List<StresstestMetrics> _runStresstestMetrics = new List<StresstestMetrics>();
        private List<object[]> _runStresstestMetricsRows = new List<object[]>();

        private Dictionary<string, List<MonitorMetrics>> _concurrencyMonitorMetrics = new Dictionary<string, List<MonitorMetrics>>();
        private Dictionary<string, List<object[]>> _concurrencyMonitorMetricsRows = new Dictionary<string, List<object[]>>();
        private Dictionary<string, List<MonitorMetrics>> _runMonitorMetrics = new Dictionary<string, List<MonitorMetrics>>();
        private Dictionary<string, List<object[]>> _runMonitorMetricsRows = new Dictionary<string, List<object[]>>();

        /// <summary>
        /// To switch the stresstest fast results to monitor fast results.
        /// </summary>
        private List<LinkButton> _monitorLinkButtons = new List<LinkButton>();

        /// <summary>
        ///     Enables Auto scroll to end of fast results when appropriate.
        /// </summary>
        private bool _keepFastResultsAtEnd = true;
        /// <summary>
        /// Not visible on a slave in a distributed test.
        /// </summary>
        private bool _monitorConfigurationControlAndLinkButtonsVisible;
        /// <summary>
        /// Normally columns are only added to the datagrid view on gui interaction. For monitors this fails if the monitors are not yet initialized.
        /// </summary>
        private bool _trySettingDataGridViewColumnsOnNextMonitorUpdate;

        //For distributed test
        private DateTime _stresstestStartedAt = DateTime.Now;
        private TimeSpan _measuredRuntime = new TimeSpan();

        private ResultsHelper _resultsHelper;

        #endregion

        #region Properties

        public bool HasResults { get { return _concurrencyStresstestMetricsRows.Count != 0; } }

        /// <summary>
        /// Should be false for a slave in a distributed test.
        /// </summary>
        public bool MonitorConfigurationControlAndLinkButtonsVisible {
            get { return _monitorConfigurationControlAndLinkButtonsVisible; }
            set {
                _monitorConfigurationControlAndLinkButtonsVisible = value;
                btnMonitor.Visible = _monitorConfigurationControlAndLinkButtonsVisible;
            }
        }

        public DateTime StresstestStartedAt {
            get { return _stresstestStartedAt; }
        }
        /// <summary>
        /// For distributed test
        /// </summary>
        public TimeSpan MeasuredRuntime {
            get { return _measuredRuntime; }
        }

        public ResultsHelper ResultsHelper {
            get { return _resultsHelper; }
            set { _resultsHelper = value; }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Dont't forget to set resultshelper
        /// </summary>
        public FastResultsControl() {
            InitializeComponent();

            //Double buffer the datagridview.
            (dgvFastResults).GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(dgvFastResults, true);

            btnCollapseExpand.PerformClick();
            cboDrillDown.SelectedIndex = 0;
            epnlMessages.Collapsed = true;
        }

        #endregion

        #region Functions

        /// <summary>
        ///     Resets everything to the initial state.
        /// </summary>
        public void SetStresstestInitialized() {
            _measuredRuntime = new TimeSpan();
            _stresstestStartedAt = DateTime.Now;

            epnlMessages.ClearEvents();

            lblStarted.Text = string.Empty;
            lblMeasuredRuntime.Text = string.Empty;
            lblStopped.Text = string.Empty;


            ClearFastResults(true);
        }

        public void SetConfigurationControls(Stresstest stresstest) {
            SetConfigurationControlsAndMonitorLinkButtons(stresstest.ToString(), stresstest.Connection, stresstest.ConnectionProxy,
                                     stresstest.Log, stresstest.LogRuleSet,
                                     stresstest.Monitors, stresstest.Concurrencies, stresstest.Runs,
                                     stresstest.MinimumDelay,
                                     stresstest.MaximumDelay, stresstest.Shuffle, stresstest.Distribute,
                                     stresstest.MonitorBefore, stresstest.MonitorAfter);
        }

        public void SetConfigurationControlsAndMonitorLinkButtons(string stresstest, Connection connection, string connectionProxy, Log log, string logRuleSet, Monitor.Monitor[] monitors, int[] concurrencies,
                                             int runs, int minimumDelay, int maximumDelay, bool shuffle, ActionAndLogEntryDistribution distribute, int monitorBefore, int monitorAfter) {
            kvpStresstest.Key = stresstest;
            kvpConnection.Key = connection.ToString();
            kvpConnectionProxy.Key = connectionProxy;

            kvpLog.Key = log.ToString();
            kvpLogRuleSet.Key = logRuleSet;

            lbtnStresstest.Visible = false;
            lbtnStresstest.Active = true;
            foreach (var lbtnMonitor in _monitorLinkButtons) flpFastResultsHeader.Controls.Remove(lbtnMonitor);
            _monitorLinkButtons.Clear();

            if (_monitorConfigurationControlAndLinkButtonsVisible) {
                if (monitors == null || monitors.Length == 0) {
                    btnMonitor.Text = "No Monitor(s)";
                    btnMonitor.BackColor = Color.Orange;
                } else {
                    lbtnStresstest.Visible = true;
                    btnMonitor.Text = monitors.Combine(", ") + "...";
                    btnMonitor.BackColor = Color.LightBlue;

                    foreach (var monitor in monitors) {
                        var lbtnMonitor = new LinkButton();
                        lbtnMonitor.Text = monitor.ToString();
                        lbtnMonitor.RadioButtonBehavior = true;
                        lbtnMonitor.Margin = new Padding(3, 6, 0, 3);
                        lbtnMonitor.ActiveChanged += lbtnMonitor_ActiveChanged;

                        _monitorLinkButtons.Add(lbtnMonitor);
                        flpFastResultsHeader.Controls.Add(lbtnMonitor);
                    }
                }
            }

            kvpConcurrencies.Value = concurrencies.Combine(", ");

            kvpRuns.Value = runs.ToString();
            kvpDelay.Value = (minimumDelay == maximumDelay
                                  ? minimumDelay.ToString()
                                  : minimumDelay + " - " + maximumDelay) + " ms";
            kvpShuffle.Value = shuffle ? "Yes" : "No";
            kvpDistribute.Value = distribute.ToString();
            kvpMonitorBefore.Value = monitorBefore + (monitorBefore == 1 ? " minute" : " minutes");
            kvpMonitorAfter.Value = monitorAfter + (monitorAfter == 1 ? " minute" : " minutes");

            flpConfiguration.ScrollControlIntoView(pnlScrollConfigTo);

            //Reinit the datagridview.
            SetFastResultsOnGuiInteraction();
        }

        private void btnMonitor_Click(object sender, EventArgs e) {
            if (btnMonitor.Text != "Monitor" && btnMonitor.Text != "No Monitor" && MonitorClicked != null)
                MonitorClicked(this, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="countDown">smaller than 0 for paused or unknown</param>
        public void SetCountDownProgressDelay(int countDown) {
            if (countDown > -1) {
                lblUpdatesIn.ForeColor = Color.SteelBlue;
                lblUpdatesIn.Text = "updates in " + countDown;
                lblUpdatesIn.Image = null;
            } else {
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
        public void SetClientMonitoring(int threadsInUse = 0, float cpuUsage = -1f, float contextSwitchesPerSecond = -1f, int memoryUsage = -1,
                                        int totalVisibleMemory = -1, float nicsSent = -1, float nicsReceived = -1) {
            kvmThreadsInUse.Value = threadsInUse.ToString();
            if (cpuUsage == -1) {
                kvmCPUUsage.Value = "N/A";
            } else {
                kvmCPUUsage.Value = Math.Round(cpuUsage, 2).ToString() + " %";

                if (cpuUsage < 60) {
                    kvmCPUUsage.BackColor = Color.GhostWhite;
                } else {
                    kvmCPUUsage.BackColor = Color.Orange;
                    AppendMessages(cpuUsage + " % CPU Usage", LogLevel.Warning);
                }
            }
            kvmContextSwitchesPerSecond.Value = (contextSwitchesPerSecond == -1)
                                                    ? "N/A"
                                                    : contextSwitchesPerSecond.ToString();

            if (memoryUsage == -1 || totalVisibleMemory == -1) {
                kvmMemoryUsage.Value = "N/A";
            } else {
                kvmMemoryUsage.Value = memoryUsage.ToString() + " / " + totalVisibleMemory + " MB";
                if (memoryUsage < 0.9 * totalVisibleMemory) {
                    kvmMemoryUsage.BackColor = Color.GhostWhite;
                } else if (memoryUsage != 0) {
                    kvmMemoryUsage.BackColor = Color.Orange;
                    AppendMessages(memoryUsage + " of " + totalVisibleMemory + " MB used", LogLevel.Warning);
                }
            }
            if (nicsSent == -1) {
                kvmNicsSent.Value = "N/A";
            } else {
                kvmNicsSent.Value = Math.Round(nicsSent, 2).ToString() + " %";
                if (nicsSent < 90) {
                    kvmNicsSent.BackColor = Color.GhostWhite;
                } else {
                    kvmNicsSent.BackColor = Color.Orange;
                    AppendMessages(nicsSent + " % NIC Usage (Sent)", LogLevel.Warning);
                }
            }
            if (nicsReceived == -1) {
                kvmNicsReceived.Value = "N/A";
            } else {
                kvmNicsReceived.Value = Math.Round(nicsReceived, 2).ToString() + " %";
                if (nicsReceived < 90) {
                    kvmNicsReceived.BackColor = Color.GhostWhite;
                } else {
                    kvmNicsReceived.BackColor = Color.Orange;
                    AppendMessages(nicsReceived + " % NIC Usage (Received)", LogLevel.Warning);
                }
            }
        }


        /// <summary>
        ///     Sets the label.
        /// </summary>
        /// <param name="start"></param>
        public void SetStresstestStarted(DateTime at) {
            _stresstestStartedAt = at;
            lblStarted.Text = "Test started at " + at;
            epnlMessages.BeginOfTimeFrame = at;
        }

        public void SetRerunning(bool rerun) {
            btnRerunning.Visible = rerun;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clearLabels">Don,t clear those when updating results.</param>
        public void ClearFastResults(bool clearLabels) {
            if (clearLabels) lblUpdatesIn.Text = lblStarted.Text = lblMeasuredRuntime.Text = lblStopped.Text = string.Empty;

            dgvFastResults.RowCount = 0;

            _concurrencyStresstestMetrics = new List<StresstestMetrics>();
            _concurrencyStresstestMetricsRows = new List<object[]>();
            _runStresstestMetrics = new List<StresstestMetrics>();
            _runStresstestMetricsRows = new List<object[]>();

            _concurrencyMonitorMetrics = new Dictionary<string, List<MonitorMetrics>>();
            _concurrencyMonitorMetricsRows = new Dictionary<string, List<object[]>>();
            _runMonitorMetrics = new Dictionary<string, List<MonitorMetrics>>();
            _runMonitorMetricsRows = new Dictionary<string, List<object[]>>();

            _keepFastResultsAtEnd = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="metrics"></param>
        /// <param name="setMeasuredRunTime">Should only be true if the test is running or just done.</param>
        public void UpdateFastConcurrencyResults(List<StresstestMetrics> metrics, bool setMeasuredRunTime = true) {
            _concurrencyStresstestMetrics = metrics;
            _concurrencyStresstestMetricsRows = StresstestMetricsHelper.MetricsToRows(metrics, chkReadable.Checked);
            if (cboDrillDown.SelectedIndex == 0 && lbtnStresstest.Active) {
                int count = _concurrencyStresstestMetricsRows.Count;
                if (dgvFastResults.RowCount == count && count != 0) dgvFastResults.InvalidateRow(count - 1); else dgvFastResults.RowCount = count;
                dgvFastResults.AutoResizeColumns();
                KeepFastResultsAtEnd();
            }
            if (setMeasuredRunTime) SetMeasuredRuntime();
        }
        /// <summary>
        /// Uses the monitor.ToString as an identifier.
        /// </summary>
        /// <param name="monitorToString"></param>
        /// <param name="metrics"></param>
        public void UpdateFastConcurrencyResults(string monitorToString, List<MonitorMetrics> metrics) {
            if (monitorToString == null) return;
            if (_concurrencyMonitorMetrics.ContainsKey(monitorToString)) _concurrencyMonitorMetrics[monitorToString] = metrics;
            else _concurrencyMonitorMetrics.Add(monitorToString, metrics);

            var concurrencyMonitorMetricsRows = MonitorMetricsHelper.MetricsToRows(metrics, chkReadable.Checked);
            if (_concurrencyMonitorMetricsRows.ContainsKey(monitorToString)) _concurrencyMonitorMetricsRows[monitorToString] = concurrencyMonitorMetricsRows;
            else _concurrencyMonitorMetricsRows.Add(monitorToString, concurrencyMonitorMetricsRows);

            LinkButton lbtnMonitor = null;
            foreach (var lbtn in _monitorLinkButtons) if (lbtn.Text == monitorToString) { lbtnMonitor = lbtn; break; }

            if (cboDrillDown.SelectedIndex == 0 && lbtnMonitor != null && lbtnMonitor.Active) {
                int count = concurrencyMonitorMetricsRows.Count;
                if (dgvFastResults.RowCount == count && count != 0) dgvFastResults.InvalidateRow(count - 1); else dgvFastResults.RowCount = count;
                dgvFastResults.AutoResizeColumns();
                KeepFastResultsAtEnd();
            }

            if (_trySettingDataGridViewColumnsOnNextMonitorUpdate) SetFastResultsOnGuiInteraction();
        }

        /// <summary>
        /// </summary>
        /// <param name="metrics"></param>
        /// <param name="setMeasuredRunTime">Should only be true if the test is running or just done.</param>
        public void UpdateFastRunResults(List<StresstestMetrics> metrics, bool setMeasuredRunTime = true) {
            _runStresstestMetrics = metrics;
            _runStresstestMetricsRows = StresstestMetricsHelper.MetricsToRows(metrics, chkReadable.Checked);
            if (cboDrillDown.SelectedIndex == 1) {
                int count = _runStresstestMetricsRows.Count;
                if (dgvFastResults.RowCount == count && count != 0) dgvFastResults.InvalidateRow(count - 1); else dgvFastResults.RowCount = count;
                dgvFastResults.AutoResizeColumns();
                KeepFastResultsAtEnd();
            }
            if (setMeasuredRunTime) SetMeasuredRuntime();
        }
        /// <summary>
        /// Uses the monitor.ToString as an identifier.
        /// </summary>
        /// <param name="monitorToString"></param>
        /// <param name="metrics"></param>
        public void UpdateFastRunResults(string monitorToString, List<MonitorMetrics> metrics) {
            if (monitorToString == null) return;
            if (_runMonitorMetrics.ContainsKey(monitorToString)) _runMonitorMetrics[monitorToString] = metrics;
            else _runMonitorMetrics.Add(monitorToString, metrics);

            var runMonitorMetricsRows = MonitorMetricsHelper.MetricsToRows(metrics, chkReadable.Checked);
            if (_runMonitorMetricsRows.ContainsKey(monitorToString)) _runMonitorMetricsRows[monitorToString] = runMonitorMetricsRows;
            else _runMonitorMetricsRows.Add(monitorToString, runMonitorMetricsRows);

            LinkButton lbtnMonitor = null;
            foreach (var lbtn in _monitorLinkButtons) if (lbtn.Text == monitorToString) { lbtnMonitor = lbtn; break; }

            if (cboDrillDown.SelectedIndex == 1 && lbtnMonitor != null && lbtnMonitor.Active) {
                int count = runMonitorMetricsRows.Count;
                if (dgvFastResults.RowCount == count && count != 0) dgvFastResults.InvalidateRow(count - 1); else dgvFastResults.RowCount = count;
                dgvFastResults.AutoResizeColumns();
                KeepFastResultsAtEnd();
            }
            if (_trySettingDataGridViewColumnsOnNextMonitorUpdate) SetFastResultsOnGuiInteraction();
        }
        private void dgvFastResults_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            try {
                if (lbtnStresstest.Active)
                    e.Value = (cboDrillDown.SelectedIndex == 0 ? _concurrencyStresstestMetricsRows : _runStresstestMetricsRows)[e.RowIndex][e.ColumnIndex];
                else {
                    string monitorToString = null;
                    foreach (var lbtnMonitor in _monitorLinkButtons) if (lbtnMonitor.Active) { monitorToString = lbtnMonitor.Text; break; }
                    if (monitorToString != null) {
                        var row = (cboDrillDown.SelectedIndex == 0 ? _concurrencyMonitorMetricsRows : _runMonitorMetricsRows)[monitorToString][e.RowIndex];
                        e.Value = (e.ColumnIndex < row.Length) ? row[e.ColumnIndex] : "--";
                    }
                }
            } catch { }
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
        ///     Sets the '; ran ...' label.
        /// </summary>
        /// <param name="metrics"></param>
        private void SetMeasuredRuntime() {
            epnlMessages.SetEndOfTimeFrameToNow();
            _measuredRuntime = epnlMessages.EndOfTimeFrame - epnlMessages.BeginOfTimeFrame;
            lblMeasuredRuntime.Text = "; ran " + _measuredRuntime.ToShortFormattedString();
        }
        public void SetMeasuredRuntime(TimeSpan measuredRuntime) {
            epnlMessages.SetEndOfTimeFrameTo(epnlMessages.BeginOfTimeFrame + measuredRuntime);
            string s = "; ran " + measuredRuntime.ToShortFormattedString();
            if (lblMeasuredRuntime.Text != s) lblMeasuredRuntime.Text = s;
        }

        private void btnRerunning_Click(object sender, EventArgs e) {
            MessageBox.Show(
                "The current run is rerunning until the slowest current run on another slave is finished (aka Break on Last Run Sync).",
                string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        ///     label updates in visibility
        /// </summary>
        public void SetStresstestStopped() {
            lblUpdatesIn.Text = string.Empty;
            btnRerunning.Visible = false;
        }

        /// <summary>
        ///     label updates in visibility + label stopped forecolor and text.
        /// </summary>
        /// <param name="stresstestResult">If == busy, the label text will be cleared.</param>
        /// <param name="message">If null, no message is appended.</param>
        public void SetStresstestStopped(StresstestStatus stresstestResult, TimeSpan measuredRuntime, Exception exception = null) {
            SetMeasuredRuntime(measuredRuntime);
            __SetStresstestStopped(stresstestResult);
        }

        /// <summary>
        ///     label updates in visibility + label stopped forecolor and text.
        /// </summary>
        /// <param name="stresstestResult">If == busy, the label text will be cleared.</param>
        /// <param name="message">If null, no message is appended.</param>
        public void SetStresstestStopped(StresstestStatus stresstestResult, Exception exception = null) {
            SetMeasuredRuntime();
            __SetStresstestStopped(stresstestResult);
        }

        /// <summary>
        ///     label updates in visibility + label stopped forecolor and text.
        /// </summary>
        /// <param name="stresstestResult">If == busy, the label text will be cleared.</param>
        /// <param name="message">If null, no message is appended.</param>
        private void __SetStresstestStopped(StresstestStatus stresstestResult, Exception exception = null) {
            try {
                string message = null;
                if (exception != null) {
                    message = "The stresstest threw an exception:\n" + exception + "\n\nSee " +
                              Path.Combine(Logger.DEFAULT_LOCATION, DateTime.Now.ToString("dd-MM-yyyy") + " " + LogWrapper.Default.Logger.Name + ".txt");
                    LogWrapper.LogByLevel(message, LogLevel.Error);
                    AppendMessages(message, Color.Red);
                }
                string lblStoppedText = null;
                switch (stresstestResult) {
                    case StresstestStatus.Ok:
                        lblStoppedText = "and finished at " + epnlMessages.EndOfTimeFrame;
                        if (lblStopped.Text != lblStoppedText) {
                            lblStopped.ForeColor = Color.Green;
                            lblStopped.Text = lblStoppedText;

                            message = string.Format("The test completed succesfully in {0}.", (epnlMessages.EndOfTimeFrame - epnlMessages.BeginOfTimeFrame).ToShortFormattedString());
                            LogWrapper.LogByLevel(message, LogLevel.Info);
                            AppendMessages(message, Color.GreenYellow);

                            SetStresstestStopped();
                        }
                        break;
                    case StresstestStatus.Error:
                        lblStoppedText = "failed at " + epnlMessages.EndOfTimeFrame;
                        if (lblStopped.Text != lblStoppedText) {
                            lblStopped.ForeColor = Color.Red;
                            lblStopped.Text = lblStoppedText;

                            SetStresstestStopped();
                        }
                        break;
                    case StresstestStatus.Cancelled:
                        lblStoppedText = "and cancelled at " + epnlMessages.EndOfTimeFrame;
                        if (lblStopped.Text != lblStoppedText) {
                            lblStopped.ForeColor = Color.Orange;
                            lblStopped.Text = lblStoppedText;

                            message = "The stresstest was cancelled.";
                            LogWrapper.LogByLevel(message, LogLevel.Info);
                            AppendMessages(message, Color.Orange);

                            SetStresstestStopped();
                        }
                        break;
                    default:
                        lblStopped.Text = string.Empty;
                        break;
                }
            } catch { }
        }
        public void AppendMessages(string message, LogLevel logLevel = LogLevel.Info) {
            var c = new[] { Color.DarkGray, Color.Orange, Color.Red };
            AppendMessages(message, c[(int)logLevel], logLevel);
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventColor">a custom color if you need one</param>
        /// <param name="logLevel"></param>
        public void AppendMessages(string message, Color eventColor, LogLevel logLevel = LogLevel.Info) {
            try { epnlMessages.AddEvent((EventViewEventType)logLevel, eventColor, message); } catch { }
        }

        /// <summary>
        ///     Get all events (messages).
        /// </summary>
        /// <returns></returns>
        public List<EventPanelEvent> GetEvents() {
            return epnlMessages.GetEvents();
        }

        /// <summary>
        ///     Set events
        /// </summary>
        public void SetEvents(List<EventPanelEvent> events) {
            if (IsDisposed) return;

            LockWindowUpdate(Handle.ToInt32());
            epnlMessages.ClearEvents();
            foreach (EventPanelEvent epe in events)
                epnlMessages.AddEvent(epe.EventType, epe.EventProgressBarEventColor, epe.Message, epe.At);
            LockWindowUpdate(0);
        }

        public void ClearEvents() { epnlMessages.ClearEvents(); }

        /// <summary>
        ///     Show event message at the right date time, use this if you have an external event progress bar.
        /// </summary>
        /// <param name="at"></param>
        public void ShowEvent(DateTime at) { epnlMessages.ShowEvent(at); }

        private void lbtnStresstest_ActiveChanged(object sender, EventArgs e) {
            if (lbtnStresstest.Active) SetFastResultsOnGuiInteraction();
        }
        private void lbtnMonitor_ActiveChanged(object sender, EventArgs e) {
            if ((sender as LinkButton).Active) SetFastResultsOnGuiInteraction();
        }
        private void cboDrillDown_SelectedIndexChanged(object sender, EventArgs e) {
            SetFastResultsOnGuiInteraction();
        }

        private void chkReadable_CheckedChanged(object sender, EventArgs e) {
            SetFastResultsOnGuiInteraction();
            toolTip.SetToolTip(chkReadable, chkReadable.Checked ? "Uncheck this if you want results you can calculate with." : "Check this if you want readable results.");
        }

        private void SetFastResultsOnGuiInteraction() {
            //Set the headers.
            dgvFastResults.Columns.Clear();

            string monitorToString = null;
            string[] columnHeaders = null;
            if (lbtnStresstest.Active)
                columnHeaders = cboDrillDown.SelectedIndex == 0 ? StresstestMetricsHelper.GetMetricsHeadersConcurrency(chkReadable.Checked) : StresstestMetricsHelper.GetMetricsHeadersRun(chkReadable.Checked);
            else {
                foreach (var lbtnMonitor in _monitorLinkButtons) if (lbtnMonitor.Active) { monitorToString = lbtnMonitor.Text; break; }
                if (monitorToString != null)
                    if (cboDrillDown.SelectedIndex == 0 && _concurrencyMonitorMetrics.ContainsKey(monitorToString) && _concurrencyMonitorMetrics[monitorToString].Count != 0)
                        columnHeaders = MonitorMetricsHelper.GetMetricsHeadersConcurrency(_concurrencyMonitorMetrics[monitorToString][0].Headers, chkReadable.Checked);
                    else if (cboDrillDown.SelectedIndex == 1 && _runMonitorMetrics.ContainsKey(monitorToString) && _runMonitorMetrics[monitorToString].Count != 0)
                        columnHeaders = MonitorMetricsHelper.GetMetricsHeadersRun(_runMonitorMetrics[monitorToString][0].Headers, chkReadable.Checked);
            }
            //For when the selected monitor is not initialized yet.
            if (columnHeaders == null) {
                _trySettingDataGridViewColumnsOnNextMonitorUpdate = true;
                return;
            }
            _trySettingDataGridViewColumnsOnNextMonitorUpdate = false;

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
            if (lbtnStresstest.Active)
                if (cboDrillDown.SelectedIndex == 0) UpdateFastConcurrencyResults(_concurrencyStresstestMetrics, false); else UpdateFastRunResults(_runStresstestMetrics, false);
            else if (monitorToString != null)
                if (cboDrillDown.SelectedIndex == 0) UpdateFastConcurrencyResults(monitorToString, _concurrencyMonitorMetrics[monitorToString]); else UpdateFastRunResults(monitorToString, _runMonitorMetrics[monitorToString]);
        }

        private void btnSaveDisplayedResults_Click(object sender, EventArgs e) {
            var sfd = new SaveFileDialog();
            sfd.Title = "Where do you want to save the displayed results?";
            sfd.FileName = kvpStresstest.Key.ReplaceInvalidWindowsFilenameChars('_');
            sfd.Filter = "TXT|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
                try {
                    using (var sw = new StreamWriter(sfd.FileName)) {
                        sw.Write(GetDisplayedResults());
                        sw.Flush();
                    }
                } catch {
                    MessageBox.Show("Could not access file: " + sfd.FileName, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        /// <summary>
        ///     Get the displayed results.
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
            List<object[]> rows = new List<object[]>();
            if (lbtnStresstest.Active)
                rows = cboDrillDown.SelectedIndex == 0 ? _concurrencyStresstestMetricsRows : _runStresstestMetricsRows;
            else {
                string monitorToString = null;
                foreach (var lbtnMonitor in _monitorLinkButtons) if (lbtnMonitor.Active) { monitorToString = lbtnMonitor.Text; break; }
                if (monitorToString != null)
                    rows = (cboDrillDown.SelectedIndex == 0 ? _concurrencyMonitorMetricsRows : _runMonitorMetricsRows)[monitorToString];
            }
            foreach (var row in rows) {
                sb.Append(row.Combine("\t"));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private void btnExport_Click(object sender, EventArgs e) {
            epnlMessages.Export();
        }

        // Gui correction, the epnl wil not be sized correctly otherwise.
        private void StresstestControl_SizeChanged(object sender, EventArgs e) {
            LockWindowUpdate(Handle.ToInt32());
            epnlMessages.Collapsed = !epnlMessages.Collapsed;
            epnlMessages.Collapsed = !epnlMessages.Collapsed;
            LockWindowUpdate(0);
        }

        // Set the splitter distance of the splitcontainer if collapsed has changed.
        private void epnlMessages_CollapsedChanged(object sender, EventArgs e) {
            epnlMessages.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            try {
                int distance = splitContainer.Panel2.Height - epnlMessages.Bottom;
                if (splitContainer.SplitterDistance + distance > 0)
                    splitContainer.SplitterDistance += distance;

                splitContainer.IsSplitterFixed = epnlMessages.Collapsed;
                BackColor = splitContainer.IsSplitterFixed ? Color.Transparent : SystemColors.Control;
            } catch { }

            epnlMessages.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        }

        private void btnCollapseExpand_Click(object sender, EventArgs e) {
            if (btnCollapseExpand.Text == "-") {
                btnCollapseExpand.Text = "+";

                splitTop.SplitterDistance = splitTop.Panel1MinSize;
                splitTop.IsSplitterFixed = true;
                splitTop.BackColor = Color.White;
            } else {
                btnCollapseExpand.Text = "-";
                splitTop.SplitterDistance = 85;
                splitTop.IsSplitterFixed = false;
                splitTop.BackColor = SystemColors.Control;
            }
        }
        #endregion
    }
}