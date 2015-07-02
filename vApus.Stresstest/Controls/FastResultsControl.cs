using RandomUtils;
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
using System.Runtime.InteropServices;
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

        private Dictionary<string, List<MonitorMetrics>> _concurrencyMonitorMetrics = new Dictionary<string, List<MonitorMetrics>>();
        private Dictionary<string, List<object[]>> _concurrencyMonitorMetricsRows = new Dictionary<string, List<object[]>>();
        private Dictionary<string, List<MonitorMetrics>> _runMonitorMetrics = new Dictionary<string, List<MonitorMetrics>>();
        private Dictionary<string, List<object[]>> _runMonitorMetricsRows = new Dictionary<string, List<object[]>>();

        /// <summary>
        /// To switch the stress test fast results to monitor fast results.
        /// </summary>
        private List<LinkButton> _monitorLinkButtons = new List<LinkButton>();

        /// <summary>
        ///     Enables Auto scroll to end of fast results when appropriate.
        /// </summary>
        private bool _keepFastResultsAtEnd = true;
        /// <summary>
        /// Not visible on a slave in a distributed test.
        /// </summary>
        private bool _monitorConfigurationControlAndKeyValuePairControlVisible;
        /// <summary>
        /// Normally columns are only added to the datagrid view on gui interaction. For monitors this fails if the monitors are not yet initialized.
        /// </summary>
        private bool _trySettingDataGridViewColumnsOnNextMonitorUpdate;

        //For distributed test
        private DateTime _stressTestStartedAt = DateTime.Now;
        private TimeSpan _measuredRuntime = new TimeSpan();

        private bool _simplifiedMetricsReturned = false; //Only send a warning to the user once.
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
            SetConfigurationControlsAndMonitorLinkButtons(stressTest.ToString(), stressTest.Connection, stressTest.ConnectionProxy,
                                     stressTest.Scenarios, stressTest.ScenarioRuleSet,
                                     stressTest.Monitors, stressTest.Concurrencies, stressTest.Runs,
                                     stressTest.InitialMinimumDelay, stressTest.InitialMaximumDelay,
                                     stressTest.MinimumDelay, stressTest.MaximumDelay, stressTest.Shuffle,
                                     stressTest.ActionDistribution, stressTest.MaximumNumberOfUserActions,
                                     stressTest.MonitorBefore, stressTest.MonitorAfter);
        }
        public void SetConfigurationControlsAndMonitorLinkButtons(string stressTest, Connection connection, string connectionProxy, KeyValuePair<Scenario, uint>[] scenarios, string scenarioRuleSet, Monitor.Monitor[] monitors, int[] concurrencies,
                                             int runs, int initialMinimumDelay, int initialMaximumDelay, int minimumDelay, int maximumDelay, bool shuffle, bool actionDistribution, int maximumNumberOfUserActions, int monitorBefore, int monitorAfter) {
            kvpStressTest.Key = stressTest;
            kvpConnection.Key = connection.ToString();
            kvpConnectionProxy.Key = connectionProxy;

            var scenarioKeys = new List<Scenario>(scenarios.Length);
            foreach (var kvp in scenarios)
                scenarioKeys.Add(kvp.Key);

            kvpScenario.Key = scenarioKeys.Combine(", ");
            kvpScenarioRuleSet.Key = scenarioRuleSet;

            lbtnStressTest.Visible = false;
            lbtnStressTest.Active = true;
            foreach (var lbtnMonitor in _monitorLinkButtons) flpFastResultsHeader.Controls.Remove(lbtnMonitor);
            _monitorLinkButtons.Clear();

            if (_monitorConfigurationControlAndKeyValuePairControlVisible) {
                if (monitors == null || monitors.Length == 0) {
                    kvpMonitor.Key = "No monitor(s)";
                    kvpMonitor.BackColor = Color.Orange;
                } else {
                    lbtnStressTest.Visible = true;
                    kvpMonitor.Key = monitors.Combine(", ");
                    kvpMonitor.BackColor = Color.LightBlue;

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

            _concurrencyMonitorMetrics = new Dictionary<string, List<MonitorMetrics>>();
            _concurrencyMonitorMetricsRows = new Dictionary<string, List<object[]>>();
            _runMonitorMetrics = new Dictionary<string, List<MonitorMetrics>>();
            _runMonitorMetricsRows = new Dictionary<string, List<object[]>>();

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
        /// <returns>The last warning if any.</returns>
        public string SetClientMonitoring(int threadsInUse = 0, float cpuUsage = -1f, int memoryUsage = -1, int totalVisibleMemory = -1,
            string nic = "NIC", float nicBandwidth = -1, float nicsSent = -1, float nicsReceived = -1) {

            string lastWarning = string.Empty;

            kvmThreadsInUse.Value = threadsInUse.ToString();
            if (cpuUsage == -1) {
                kvmCPUUsage.Value = "N/A";
            } else {
                kvmCPUUsage.Value = Math.Round(cpuUsage, 2).ToString() + " %";

                if (cpuUsage < 60) {
                    kvmCPUUsage.BackColor = Color.GhostWhite;
                } else {
                    kvmCPUUsage.BackColor = Color.Orange;

                    lastWarning = cpuUsage + " % CPU Usage";
                    AddEvent(lastWarning, Level.Warning);
                }
            }

            if (memoryUsage == -1 || totalVisibleMemory == -1) {
                kvmMemoryUsage.Value = "N/A";
            } else {
                kvmMemoryUsage.Value = memoryUsage.ToString() + " / " + totalVisibleMemory + " MB";
                if (memoryUsage < 0.9 * totalVisibleMemory) {
                    kvmMemoryUsage.BackColor = Color.GhostWhite;
                } else if (memoryUsage != 0) {
                    kvmMemoryUsage.BackColor = Color.Orange;

                    lastWarning = memoryUsage + " of " + totalVisibleMemory + " MB used";
                    AddEvent(lastWarning, Level.Warning);
                }
            }
            kvmNic.Key = nic;
            if (nicBandwidth == -1)
                kvmNic.Value = "N/A";
            else
                kvmNic.Value = nicBandwidth + " Mbps";
            if (nicsSent == -1) {
                kvmNicsSent.Value = "N/A";
            } else {
                kvmNicsSent.Value = Math.Round(nicsSent, 2).ToString() + " %";
                if (nicsSent < 90) {
                    kvmNicsSent.BackColor = Color.GhostWhite;
                } else if (!float.IsPositiveInfinity(nicsSent) && !float.IsNegativeInfinity(nicsSent)) {
                    kvmNicsSent.BackColor = Color.Orange;

                    lastWarning = nicsSent + " % NIC Usage (Sent)";
                    AddEvent(lastWarning, Level.Warning);
                }
            }
            if (nicsReceived == -1) {
                kvmNicsReceived.Value = "N/A";
            } else {
                kvmNicsReceived.Value = Math.Round(nicsReceived, 2).ToString() + " %";
                if (nicsReceived < 90) {
                    kvmNicsReceived.BackColor = Color.GhostWhite;
                } else if (!float.IsPositiveInfinity(nicsReceived) && !float.IsNegativeInfinity(nicsReceived)) {
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
        public void UpdateFastConcurrencyResults(List<StressTestMetrics> metrics, bool setMeasuredRunTime = true, bool simplified = false) {
            _concurrencyStressTestMetrics = metrics;
            _simplifiedMetricsReturned = simplified;
            _concurrencyStressTestMetricsRows = FastStressTestMetricsHelper.MetricsToRows(metrics, chkReadable.Checked, _simplifiedMetricsReturned);
            if (cboDrillDown.SelectedIndex == 0 && lbtnStressTest.Active) {
                int count = _concurrencyStressTestMetricsRows.Count;
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

            if (_trySettingDataGridViewColumnsOnNextMonitorUpdate) RefreshFastResults();
        }

        /// <summary>
        /// </summary>
        /// <param name="metrics"></param>
        /// <param name="setMeasuredRunTime">Should only be true if the test is running or just done.</param>
        public void UpdateFastRunResults(List<StressTestMetrics> metrics, bool setMeasuredRunTime = true, bool simplified = false) {
            _runStressTestMetrics = metrics;
            _simplifiedMetricsReturned = simplified;
            _runStressTestMetricsRows = FastStressTestMetricsHelper.MetricsToRows(metrics, chkReadable.Checked, _simplifiedMetricsReturned);
            if (cboDrillDown.SelectedIndex == 1) {
                int count = _runStressTestMetricsRows.Count;
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
            if (_trySettingDataGridViewColumnsOnNextMonitorUpdate) RefreshFastResults();
        }
        private void dgvFastResults_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            try {
                if (lbtnStressTest.Active) {
                    List<object[]> rows = (cboDrillDown.SelectedIndex == 0 ? _concurrencyStressTestMetricsRows : _runStressTestMetricsRows);
                    if (rows.Count > e.RowIndex && rows[0].Length > e.ColumnIndex)
                        e.Value = rows[e.RowIndex][e.ColumnIndex];
                } else {
                    string monitorToString = null;
                    foreach (var lbtnMonitor in _monitorLinkButtons) if (lbtnMonitor.Active) { monitorToString = lbtnMonitor.Text; break; }
                    if (monitorToString != null) {
                        List<object[]> rows = (cboDrillDown.SelectedIndex == 0 ? _concurrencyMonitorMetricsRows : _runMonitorMetricsRows)[monitorToString];
                        if (rows.Count > e.RowIndex) {
                            object[] row = rows[e.RowIndex];
                            if (row.Length > e.ColumnIndex) {
                                e.Value = (e.ColumnIndex < row.Length) ? row[e.ColumnIndex] : "--";

                                string valueString = e.Value.ToString();
                                if (valueString == "0" || valueString == "-1")
                                    e.Value = "--";
                                else if (e.Value is float)
                                    e.Value = StringUtil.FloatToLongString((float)e.Value);
                                else if (e.Value is double)
                                    e.Value = StringUtil.DoubleToLongString((double)e.Value);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
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
        ///     Sets the '; ran ...' label.
        /// </summary>
        /// <param name="metrics"></param>
        private void SetMeasuredRuntime() {
            epnlMessages.SetEndOfTimeFrameToNow();
            _measuredRuntime = epnlMessages.EndOfTimeFrame - epnlMessages.BeginOfTimeFrame;
            _measuredRuntime = _measuredRuntime.Subtract(new TimeSpan(_measuredRuntime.Ticks % TimeSpan.TicksPerSecond));
            if (_measuredRuntime.TotalSeconds > 1)
                lblMeasuredRuntime.Text = "; ran " + _measuredRuntime.ToShortFormattedString();
        }
        public void SetMeasuredRuntime(TimeSpan measuredRuntime) {
            epnlMessages.SetEndOfTimeFrameTo(epnlMessages.BeginOfTimeFrame + measuredRuntime);
            measuredRuntime = _measuredRuntime.Subtract(new TimeSpan(measuredRuntime.Ticks % TimeSpan.TicksPerSecond));
            if (measuredRuntime.TotalSeconds > 1) {
                string s = "; ran " + measuredRuntime.ToShortFormattedString();
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
        }

        /// <summary>
        ///     label updates in visibility + label stopped forecolor and text.
        /// </summary>
        /// <param name="stressTestResult">If == busy, the label text will be cleared.</param>
        /// <param name="message">If null, no message is appended.</param>
        public void SetStressTestStopped(StressTestStatus stressTestResult, TimeSpan measuredRuntime) {
            SetMeasuredRuntime(measuredRuntime);
            __SetStressTestStopped(stressTestResult);
        }

        /// <summary>
        ///     label updates in visibility + label stopped forecolor and text.
        /// </summary>
        /// <param name="stressTestResult">If == busy, the label text will be cleared.</param>
        /// <param name="message">If null, no message is appended.</param>
        public void SetStressTestStopped(StressTestStatus stressTestResult) {
            SetMeasuredRuntime();
            __SetStressTestStopped(stressTestResult);
        }

        /// <summary>
        ///     label updates in visibility + label stopped forecolor and text.
        /// </summary>
        /// <param name="stressTestResult">If == busy, the label text will be cleared.</param>
        /// <param name="message">If null, no message is appended.</param>
        private void __SetStressTestStopped(StressTestStatus stressTestResult, Exception exception = null) {
            try {
                string message = null;
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

                            message = string.Format("The test completed succesfully in {0}.", (epnlMessages.EndOfTimeFrame - epnlMessages.BeginOfTimeFrame).ToShortFormattedString());
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
            } catch (Exception ex) {
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
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed adding event.", ex, new object[] { message, eventColor, logLevel });
            }
        }

        /// <summary>
        ///     Get all events (messages).
        /// </summary>
        /// <returns></returns>
        public List<EventPanelEvent> GetEvents() { return epnlMessages.GetEvents(); }

        /// <summary>
        ///     Set events
        /// </summary>
        public void SetEvents(List<EventPanelEvent> events) {
            if (IsDisposed) return;
            int toSetCount = events.Count;
            int currentEventCount = epnlMessages.EventCount;

            if (currentEventCount == 0 || toSetCount < currentEventCount) {
                epnlMessages.SetEvents(events);
            } else if (toSetCount > currentEventCount) {
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

        private void lbtnStressTest_ActiveChanged(object sender, EventArgs e) {
            if (lbtnStressTest.Active) RefreshFastResults();
        }
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

            string monitorToString = null;
            string[] columnHeaders = null;
            if (lbtnStressTest.Active)
                columnHeaders = cboDrillDown.SelectedIndex == 0 ? FastStressTestMetricsHelper.GetMetricsHeadersConcurrency(chkReadable.Checked) : FastStressTestMetricsHelper.GetMetricsHeadersRun(chkReadable.Checked);
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
            if (lbtnStressTest.Active)
                if (cboDrillDown.SelectedIndex == 0)
                    UpdateFastConcurrencyResults(_concurrencyStressTestMetrics, false, _simplifiedMetricsReturned);
                else
                    UpdateFastRunResults(_runStressTestMetrics, false, _simplifiedMetricsReturned);
            else if (monitorToString != null)
                if (cboDrillDown.SelectedIndex == 0)
                    UpdateFastConcurrencyResults(monitorToString, _concurrencyMonitorMetrics[monitorToString]);
                else
                    UpdateFastRunResults(monitorToString, _runMonitorMetrics[monitorToString]);
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
                } catch {
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
            List<object[]> rows = new List<object[]>();
            if (lbtnStressTest.Active)
                rows = cboDrillDown.SelectedIndex == 0 ? _concurrencyStressTestMetricsRows : _runStressTestMetricsRows;
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

        private void btnExport_Click(object sender, EventArgs e) { epnlMessages.Export(); }

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
            } catch (Exception ex) {
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