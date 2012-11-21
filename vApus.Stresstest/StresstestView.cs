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
using System.Threading;
using System.Windows.Forms;
using vApus.Monitor;
using vApus.Results;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest
{
    public partial class StresstestView : BaseSolutionComponentView
    {
        #region Fields
        private Stresstest _stresstest;
        private StresstestCore _stresstestCore;
        private StresstestResult _stresstestResult;
        /// <summary>
        /// Caching the results to visualize in the stresstestcontrol.
        /// </summary>
        private vApus.Results.MetricsCache _metricsCache;
        /// <summary>
        /// Countdown for the update.
        /// </summary>
        private int _countDown;
        private Countdown _monitorBeforeCountDown;
        private int _monitorsInitialized;
        private List<Monitor.MonitorView> _monitorViews = new List<MonitorView>();
        #endregion

        #region Constructor
        /// <summary>
        /// Designer time constructor.
        /// </summary>
        public StresstestView()
        {
            InitializeComponent();
        }
        public StresstestView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            Solution.ActiveSolution.RegisterForCancelFormClosing(this);
            _stresstest = SolutionComponent as Stresstest;

            InitializeComponent();
            //stresstestReportControl.Stresstest = _stresstest;

            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new System.EventHandler(StresstestProjectView_HandleCreated);
        }
        #endregion

        #region Functions

        #region Set the Gui
        private void StresstestProjectView_HandleCreated(object sender, System.EventArgs e)
        {
            SetGui();
        }
        private void SetGui()
        {
            Text = SolutionComponent.ToString();
            solutionComponentPropertyPanel.SolutionComponent = SolutionComponent;

            btnStart.Enabled = !btnStop.Enabled;

            if (_stresstest.Connection.IsEmpty ||
                _stresstest.Connection.ConnectionProxy.IsEmpty ||
                _stresstest.Log.IsEmpty ||
                _stresstest.Log.LogRuleSet.IsEmpty)
                btnStart.Enabled = false;
        }
        public override void Refresh()
        {
            base.Refresh();
            SetGui();
            solutionComponentPropertyPanel.Refresh();
        }
        #endregion

        private void stresstestControl_MonitorClicked(object sender, EventArgs e)
        {
            foreach (var view in _monitorViews)
                if (view != null && !view.IsDisposed)
                    view.Show();
        }

        #region Start
        /// <summary>
        /// Schedule the test at another time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSchedule_Click(object sender, EventArgs e)
        {
            Schedule schedule = (btnSchedule.Tag is DateTime) ? new Schedule((DateTime)btnSchedule.Tag) : new Schedule();
            if (schedule.ShowDialog() == DialogResult.OK)
            {
                if (schedule.ScheduledAt > DateTime.Now)
                {
                    btnSchedule.Text = "Scheduled at " + schedule.ScheduledAt;
                    btnSchedule.Tag = schedule.ScheduledAt;
                }
                else
                {
                    btnSchedule.Text = "Schedule...";
                    btnSchedule.Tag = null;
                }

                btnStart_Click(this, null);
            }
        }
        /// <summary>
        /// Start a test with or without monitoring it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, System.EventArgs e)
        {
            if (stresstestControl.FastResultsCount > 0 &&
                MessageBox.Show("Do you want to start the test and clear the previous results?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;
            if (InitDatabase())
            {
                SetGuiForStart();

                if (_stresstest.Monitors.Length != 0)
                    InitializeMonitors();
                else
                    Start();
            }
        }
        /// <summary>
        /// True on succes
        /// </summary>
        /// <returns></returns>
        private bool InitDatabase()
        {
            var ex = ResultsHelper.BuildSchemaAndConnect();
            if (ex == null)
            {
                var dialog = new DescriptionAndTagsInputDialog() { Description = _stresstest.Description, Tags = _stresstest.Tags };
                dialog.ShowDialog();

                bool edited = false;
                if (_stresstest.Description != dialog.Description)
                {
                    _stresstest.Description = dialog.Description;
                    edited = true;
                }
                if (_stresstest.Tags.Combine(",") != dialog.Tags.Combine(", "))
                {
                    _stresstest.Tags = dialog.Tags;
                    edited = true;
                }
                if (edited)
                    _stresstest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

                ResultsHelper.SetvApusInstance("foo", "bar", 1234, "haha", false);
                ResultsHelper.SetStresstest(_stresstest.ToString(), "None", _stresstest.Connection.ToString(),
                    _stresstest.ConnectionProxy.ToString(), _stresstest.Connection.ConnectionString, _stresstest.Log.ToString(),
                    _stresstest.LogRuleSet, _stresstest.Concurrencies, _stresstest.Runs, _stresstest.MinimumDelay, _stresstest.MaximumDelay,
                    _stresstest.Shuffle, _stresstest.Distribute.ToString(), _stresstest.MonitorBefore, _stresstest.MonitorAfter);

                return true;
            }
            else
            {
                LogWrapper.LogByLevel("Could not connect to MySQL.\n" + ex, LogLevel.Warning);
                if (MessageBox.Show(
                "Could not connect to MySQL!\nDo you want to proceed anyway? No report will be made.", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    return true;
            }
            return false;
        }
        private void SetGuiForStart()
        {
            btnStop.Enabled = true;
            btnStart.Enabled = false;
            btnSchedule.Enabled = false;
            btnSchedule.Text = "Schedule...";

            stresstestControl.SetStresstestInitialized();

            _stresstestResult = null;
            _metricsCache = new Results.MetricsCache();
            //stresstestReportControl.ClearReport();

            stresstestControl.SetConfigurationControls(_stresstest);

            solutionComponentPropertyPanel.Lock();

            tc.SelectedIndex = 1;

            _countDown = Stresstest.ProgressUpdateDelay - 1;
        }
        /// <summary>
        /// Start or schedule the test.
        /// </summary>
        private void Start()
        {
            if (btnSchedule.Tag != null && btnSchedule.Tag is DateTime && (DateTime)btnSchedule.Tag > DateTime.Now)
                ScheduleStresstest();
            else
                StartStresstest();
        }
        /// <summary>
        /// Set up al the Gui stuff (enable and disable buttons, clear previous results, ...).
        /// Initialize the test in this thread, start the test in another (otherwise waiting on the test finished blocks this thread)
        /// </summary>
        private void StartStresstest()
        {
            Cursor = Cursors.WaitCursor;

            try { LocalMonitor.StartMonitoring(Stresstest.ProgressUpdateDelay * 1000); }
            catch { stresstestControl.AppendMessages("Could not initialize the local monitor, something is wrong with your WMI.", LogLevel.Error); }
            tmrProgress.Interval = Stresstest.ProgressUpdateDelay * 1000;

            try
            {
                _stresstestCore = new StresstestCore(_stresstest, true);
                _stresstestCore.StresstestStarted += new EventHandler<StresstestResultEventArgs>(_stresstestCore_StresstestStarted);
                _stresstestCore.ConcurrentUsersStarted += new EventHandler<ConcurrencyResultEventArgs>(_stresstestCore_ConcurrentUsersStarted);
                _stresstestCore.RunInitializedFirstTime += new EventHandler<RunResultEventArgs>(_stresstestCore_RunInitializedFirstTime);
                _stresstestCore.Message += new EventHandler<MessageEventArgs>(_stresstestCore_Message);
                _stresstestCore.InitializeTest();

                StartMonitors();
            }
            catch (Exception ex)
            {
                //Only one test can run at the same time.
                if (ex is ArgumentOutOfRangeException)
                {
                    stresstestControl.AppendMessages("Cannot start this test because another one is still running.", LogLevel.Error);
                    ex = null;
                }
                Stop(ex);
            }
            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Allows the stresstest to start.
        /// </summary>
        private void MonitorBeforeDone()
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    Thread stresstestThread = new Thread(StartStresstestInBackground);
                    stresstestThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                    stresstestThread.IsBackground = true;
                    stresstestThread.Start();
                }
                catch (Exception ex)
                {
                    //Only one test can run at the same time.
                    if (ex is ArgumentOutOfRangeException)
                    {
                        stresstestControl.AppendMessages("Cannot start this test because another one is still running.", LogLevel.Error);
                        ex = null;
                    }
                    Stop(ex);
                }
                Cursor = Cursors.Default;
            }, null);
        }
        /// <summary>
        /// The stresstest is executed here, it handles also the results.
        /// </summary>
        private void StartStresstestInBackground()
        {
            StresstestStatus stresstestStatus = StresstestStatus.Busy;
            Exception ex = null;
            try
            {
                stresstestStatus = _stresstestCore.ExecuteStresstest();
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                if (_stresstestCore != null && !_stresstestCore.IsDisposed)
                {
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                    {
                        Stop(ex, stresstestStatus == StresstestStatus.Ok && ex == null);
                        try
                        {
                            //stresstestReportControl.StresstestResult = _stresstestResult;
                            //stresstestReportControl.SetConfigurationLabels();
                            //stresstestReportControl.MakeReport();
                        }
                        catch (Exception e)
                        {
                            LogWrapper.LogByLevel(this.Text + ": Failed making a stresstest report.\n" + e.ToString(), LogLevel.Error);
                        }

                        //if (_monitorViews != null)
                        //    foreach (var view in _monitorViews)
                        //        if (view != null && view.Tag != null)
                        //            try
                        //            {
                        //                var monitorReportControl = view.Tag as MonitorReportControl;
                        //                monitorReportControl.SetConfig_Headers_MonitorValuesAndStresstestResults(view.Configuration, view.GetHeaders(), view.GetMonitorValues(), _stresstestResult);
                        //            }
                        //            catch (Exception e)
                        //            {
                        //                LogWrapper.LogByLevel(view.Text + ": Failed making a monitor report.\n" + e.ToString(), LogLevel.Error);
                        //            }

                        stresstestControl.SetStresstestStopped(stresstestStatus);
                    }, null);
                }
            }
        }
        /// <summary>
        /// Will lock the Gui and start the stresstest at the scheduled time.
        /// </summary>
        private void ScheduleStresstest()
        {
            btnStop.Enabled = true;
            btnStart.Enabled = false;
            btnSchedule.Enabled = false;
            solutionComponentPropertyPanel.Lock();
            tmrSchedule.Start();
        }
        private void tmrSchedule_Tick(object sender, EventArgs e)
        {
            DateTime scheduledAt = (DateTime)btnSchedule.Tag;
            if (scheduledAt <= DateTime.Now)
            {
                btnSchedule.Text = "Scheduled at " + scheduledAt;
                tmrSchedule.Stop();
                StartStresstest();
            }
            else
            {
                TimeSpan dt = scheduledAt - DateTime.Now;
                if (dt.Milliseconds != 0)
                {
                    dt = new TimeSpan(dt.Ticks - (dt.Ticks % TimeSpan.TicksPerSecond));
                    dt += new TimeSpan(0, 0, 1);
                }
                btnSchedule.Text = "Scheduled in " + dt.ToLongFormattedString();
            }
        }
        /// <summary>
        /// Initialize the monitor (if chosen to monitor), if so the test can start.
        /// </summary>
        private void InitializeMonitors()
        {
            _monitorsInitialized = 0;
            foreach (var monitorView in _monitorViews)
            {
                try
                {
                    monitorView.MonitorInitialized -= monitorView_MonitorInitialized;
                    monitorView.OnHandledException -= monitorView_OnHandledException;
                    monitorView.OnUnhandledException -= monitorView_OnUnhandledException;
                }
                catch { }
            }
            _monitorViews.Clear();

            //Also remove the tab pages.
            while (tc.TabCount != 3)
                tc.TabPages.RemoveAt(3);

            //foreach (var monitor in _stresstest.Monitors)
            //{
            //    var monitorView = SolutionComponentViewManager.Show(monitor) as Monitor.MonitorView;
            //    this.Show();

            //    stresstestControl.AppendMessages("Initializing " + monitorView.Text + "...");
            //    _monitorViews.Add(monitorView);

            //    //Add a new tab page.
            //    var monitorTabPage = new TabPage("Report " + monitorView.Text);
            //    var monitorReportControl = new MonitorReportControl();
            //    monitorReportControl.Dock = DockStyle.Fill;

            //    monitorTabPage.Controls.Add(monitorReportControl);
            //    tc.TabPages.Add(monitorTabPage);

            //    //For easy reporting
            //    monitorView.Tag = monitorReportControl;

            //    //Initialize
            //    monitorView.MonitorInitialized += new EventHandler<MonitorView.MonitorInitializedEventArgs>(monitorView_MonitorInitialized);
            //    monitorView.OnHandledException += new EventHandler<ErrorEventArgs>(monitorView_OnHandledException);
            //    monitorView.OnUnhandledException += new EventHandler<ErrorEventArgs>(monitorView_OnUnhandledException);
            //    monitorView.InitializeForStresstest();
            //}
        }
        private void monitorView_MonitorInitialized(object sender, MonitorView.MonitorInitializedEventArgs e)
        {
            var monitorView = sender as MonitorView;
            monitorView.MonitorInitialized -= monitorView_MonitorInitialized;

            if (e.ErrorMessage != null && e.ErrorMessage.Length != 0)
            {
                Stop(new Exception(e.ErrorMessage));
            }
            else
            {
                stresstestControl.AppendMessages(monitorView.Text + " is initialized.");
                if (++_monitorsInitialized == _stresstest.Monitors.Length)
                    Start();
            }
        }
        private void monitorView_OnHandledException(object sender, ErrorEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                stresstestControl.AppendMessages((sender as MonitorView).Text + ": A counter became unavailable while monitoring:\n" + e.GetException(), LogLevel.Warning);
            }, null);
        }
        private void monitorView_OnUnhandledException(object sender, ErrorEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                stresstestControl.AppendMessages((sender as MonitorView).Text + ": An error has occured while monitoring, monitor stopped!\n" + e.GetException(), LogLevel.Error);
            }, null);
        }
        /// <summary>
        /// Used in stresstest started eventhandling.
        /// </summary>
        private void StartMonitors()
        {
            if (_monitorViews != null && _stresstest.Monitors.Length != 0)
            {
                int runningMonitors = 0;
                foreach (var view in _monitorViews)
                    if (view != null && !view.IsDisposed)
                        try
                        {
                            view.Start();
                            stresstestControl.AppendMessages(view.Text + " is started.");
                            ++runningMonitors;
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                LogWrapper.LogByLevel(view.Text + " is not started.\n" + e.ToString(), LogLevel.Error);
                                stresstestControl.AppendMessages(view.Text + " is not started.");

                                try { view.Stop(); }
                                catch { }
                            }
                            catch { }
                        }

                if (runningMonitors != 0 && _stresstest.MonitorBefore != 0)
                {
                    int countdownTime = _stresstest.MonitorBefore * 60000;
                    _monitorBeforeCountDown = new Countdown(countdownTime, 5000);
                    _monitorBeforeCountDown.Tick += monitorBeforeCountDown_Tick;
                    _monitorBeforeCountDown.Stopped += monitorBeforeCountDown_Stopped;
                    _monitorBeforeCountDown.Start();
                }
                else
                {
                    MonitorBeforeDone();
                }
            }
            else
            {
                MonitorBeforeDone();
            }
        }
        private void monitorBeforeCountDown_Tick(object sender, EventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                TimeSpan ts = new TimeSpan(_monitorBeforeCountDown.CountdownTime * TimeSpan.TicksPerMillisecond);
                stresstestControl.AppendMessages("The test will start in " + ts.ToShortFormattedString() + ", monitoring first.");

                int runningMonitors = 0;
                if (_monitorViews != null && _stresstest.Monitors.Length != 0)
                    foreach (var view in _monitorViews)
                        if (view != null && !view.IsDisposed)
                            runningMonitors++;

                if (runningMonitors == 0)
                {
                    _monitorBeforeCountDown.Stop();
                    stresstestControl.AppendMessages("All monitors were manually closed.");
                }

            }, null);
        }
        private void monitorBeforeCountDown_Stopped(object sender, EventArgs e)
        {
            if (_monitorBeforeCountDown != null)
            {
                _monitorBeforeCountDown.Dispose();
                _monitorBeforeCountDown = null;
            }
            MonitorBeforeDone();
        }
        #endregion

        #region Progress
        private void _stresstestCore_StresstestStarted(object sender, StresstestResultEventArgs e)
        {
            _stresstestResult = e.StresstestResult;
            stresstestControl.SetStresstestStarted(e.StresstestResult.StartedAt);
        }
        private void _stresstestCore_ConcurrentUsersStarted(object sender, ConcurrencyResultEventArgs e)
        {
            _countDown = Stresstest.ProgressUpdateDelay;
            StopProgressDelayCountDown();
            tmrProgress.Stop();

            _metricsCache.AddOrUpdate(vApus.Results.MetricsHelper.GetMetrics(e.Result), e.Result);
            stresstestControl.UpdateConcurrencyFastResults(_metricsCache.GetConcurrencyMetrics());
            stresstestControl.SetRerunning(false);
        }
        private void _stresstestCore_RunInitializedFirstTime(object sender, RunResultEventArgs e)
        {
            _countDown = Stresstest.ProgressUpdateDelay;
            StopProgressDelayCountDown();
            tmrProgress.Stop();

            _metricsCache.AddOrUpdate(vApus.Results.MetricsHelper.GetMetrics(e.Result), e.Result);
            stresstestControl.UpdateConcurrencyFastResults(_metricsCache.GetConcurrencyMetrics());
            stresstestControl.UpdateRunFastResults(_metricsCache.GetRunMetrics());
            stresstestControl.SetRerunning(false);

            stresstestControl.SetCountDownProgressDelay(_countDown--);
            tmrProgressDelayCountDown.Start();

            tmrProgress.Start();
        }
        private void tmrProgressDelayCountDown_Tick(object sender, EventArgs e)
        {
            stresstestControl.SetCountDownProgressDelay(_countDown--);
        }
        private void tmrProgress_Tick(object sender, EventArgs e)
        {
            if (_stresstestCore != null)
            {
                try
                {
                    stresstestControl.SetClientMonitoring(_stresstestCore.BusyThreadCount, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
                }
                catch { } //Exception on false WMI. 

                stresstestControl.UpdateConcurrencyFastResults(_metricsCache.GetConcurrencyMetrics());
                var runMetrics = _metricsCache.GetRunMetrics();
                stresstestControl.UpdateRunFastResults(runMetrics);

                //Set rerunning
                stresstestControl.SetRerunning(runMetrics.Count == 0 ? false : runMetrics[runMetrics.Count - 1].RerunCount != 0);

                _countDown = Stresstest.ProgressUpdateDelay;
            }
        }
        private void _stresstestCore_Message(object sender, MessageEventArgs e)
        {
            if (e.Color == Color.Empty)
                stresstestControl.AppendMessages(e.Message, e.LogLevel);
            else
                stresstestControl.AppendMessages(e.Message, e.Color, e.LogLevel);
        }
        #endregion

        #region Stop
        private void StresstestView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnStart.Enabled ||
                _stresstestCore == null ||
                _stresstestCore.IsDisposed ||
                MessageBox.Show("Are you sure you want to close a running test?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Stop();
            }
            else
            {
                Solution.ActiveSolution.ExplicitCancelFormClosing = true;
                e.Cancel = true;
            }
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_stresstestCore == null)
            {
                Stop();
            }
            else
            {
                if (_monitorBeforeCountDown != null)
                {
                    _monitorBeforeCountDown.Stop();
                    _monitorBeforeCountDown.Dispose();
                    _monitorBeforeCountDown = null;
                }
                // Can only be cancelled once, calling multiple times is not a problem.
                _stresstestCore.Cancel();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex">The exception if failed.</param>
        private void Stop(Exception ex = null, bool monitorAfter = false)
        {
            Cursor = Cursors.WaitCursor;

            if (!monitorAfter)
                StopMonitors();
            StopStresstest();

            tmrProgress.Stop();
            StopProgressDelayCountDown();

            solutionComponentPropertyPanel.Unlock();
            btnStop.Enabled = false;
            btnStart.Enabled = true;
            btnSchedule.Enabled = true;
            tmrSchedule.Stop();

            if (ex != null)
                stresstestControl.SetStresstestStopped(StresstestStatus.Error, ex);

            if (_stresstestCore != null && !_stresstestCore.IsDisposed)
            {
                _stresstestCore.Dispose();
                _stresstestCore = null;
            }

            Cursor = Cursors.Default;

            if (monitorAfter && _stresstest.MonitorAfter != 0)
            {
                int runningMonitors = 0;
                if (_monitorViews != null && _stresstest.Monitors.Length != 0)
                    foreach (var view in _monitorViews)
                        if (view != null && !view.IsDisposed)
                            runningMonitors++;

                if (runningMonitors != 0)
                {
                    int countdownTime = _stresstest.MonitorAfter * 60000;
                    Countdown monitorAfterCountdown = new Countdown(countdownTime, 5000);
                    monitorAfterCountdown.Tick += monitorAfterCountdown_Tick;
                    monitorAfterCountdown.Stopped += monitorAfterCountdown_Stopped;
                    monitorAfterCountdown.Start();
                }
            }
        }

        private void monitorAfterCountdown_Tick(object sender, EventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                Countdown monitorAfterCountDown = sender as Countdown;
                TimeSpan ts = new TimeSpan(monitorAfterCountDown.CountdownTime * TimeSpan.TicksPerMillisecond);
                stresstestControl.AppendMessages("Monitoring after the test is finished: " + ts.ToShortFormattedString() + ".");

                int runningMonitors = 0;
                if (_monitorViews != null && _stresstest.Monitors.Length != 0)
                    foreach (var view in _monitorViews)
                        if (view != null && !view.IsDisposed)
                            runningMonitors++;

                if (runningMonitors == 0)
                {
                    monitorAfterCountDown.Stop();
                    stresstestControl.AppendMessages("All monitors were manually closed.");
                }
            }, null);
        }
        private void monitorAfterCountdown_Stopped(object sender, EventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                StopMonitors();
            }, null);

            Countdown monitorAfterCountdown = sender as Countdown;
            monitorAfterCountdown.Dispose();
            monitorAfterCountdown = null;
        }

        /// <summary>
        /// Only used in stop
        /// </summary>
        private void StopStresstest()
        {
            btnSchedule.Tag = null;
            btnSchedule.Text = "Schedule...";
            tmrSchedule.Stop();

            if (_stresstestCore != null)
            {
                try
                {
                    stresstestControl.SetClientMonitoring(_stresstestCore.BusyThreadCount, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
                }
                catch { } //Exception on false WMI. 
                stresstestControl.UpdateConcurrencyFastResults(_metricsCache.GetConcurrencyMetrics());
                stresstestControl.UpdateRunFastResults(_metricsCache.GetRunMetrics());
                stresstestControl.SetRerunning(false);

                // Can only be cancelled once, calling multiple times is not a problem.
                _stresstestCore.Cancel();
            }

            stresstestControl.SetStresstestStopped();
        }
        /// <summary>
        /// Only used in stop
        /// </summary>
        private void StopMonitors()
        {
            if (_monitorViews != null && _stresstest.Monitors.Length != 0)
            {
                foreach (var view in _monitorViews)
                    if (view != null && !view.IsDisposed)
                    {
                        view.Stop();
                        stresstestControl.AppendMessages(view.Text + " is stopped.");
                    }
            }
        }
        private void StopProgressDelayCountDown()
        {
            try
            {
                tmrProgressDelayCountDown.Stop();
                if (stresstestControl != null && !stresstestControl.IsDisposed)
                    stresstestControl.SetCountDownProgressDelay(-1);
            }
            catch { }
        }
        #endregion

        #endregion
    }
}