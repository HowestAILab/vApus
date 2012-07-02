/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using vApus.Monitor;
using vApus.SolutionTree;
using vApus.Util;
using System.Collections.Generic;

namespace vApus.Stresstest
{
    public partial class StresstestView : BaseSolutionComponentView
    {
        #region Fields
        private Stresstest _stresstest;
        private StresstestCore _stresstestCore;
        private StresstestResults _stresstestResults;
        /// <summary>
        /// Countdown for the update.
        /// </summary>
        private int _countDown;

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
            stresstestReportControl.Stresstest = _stresstest;

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
            if (stresstestControl.FastResultsCount > 0 && MessageBox.Show("Do you want to start the test and clear the previous results?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            SetGuiForStart();

            if (_stresstest.Monitors.Length != 0)
                InitializeMonitors();
            else
                Start();
        }
        private void SetGuiForStart()
        {
            btnStop.Enabled = true;
            btnStart.Enabled = false;
            btnSchedule.Enabled = false;
            btnSchedule.Text = "Schedule...";

            stresstestControl.SetStresstestInitialized();

            _stresstestResults = null;
            stresstestReportControl.ClearReport();

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
            tmrProgress.Start();

            try
            {
                _stresstestCore = new StresstestCore(_stresstest, true);
                _stresstestCore.StresstestStarted += new EventHandler<StresstestStartedEventArgs>(_stresstestCore_StresstestStarted);
                _stresstestCore.ConcurrentUsersStarted += new EventHandler<ConcurrentUsersStartedEventArgs>(_stresstestCore_ConcurrentUsersStarted);
                _stresstestCore.PrecisionStarted += new EventHandler<PrecisionStartedEventArgs>(_stresstestCore_PrecisionStarted);
                _stresstestCore.RunInitializedFirstTime += new EventHandler<RunInitializedFirstTimeEventArgs>(_stresstestCore_RunInitializedFirstTime);
                _stresstestCore.Message += new EventHandler<MessageEventArgs>(_stresstestCore_Message);
                _stresstestCore.InitializeTest();

                try
                {
                    stresstestControl.SetClientMonitoring(_stresstestCore.BusyThreadCount, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
                }
                catch { } //Exception on false WMI. 

                StartMonitors();

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
        }
        /// <summary>
        /// The stresstest is executed here, it handles also the results.
        /// </summary>
        private void StartStresstestInBackground()
        {
            StresstestResult stresstestResult = StresstestResult.Busy;
            Exception ex = null;
            try
            {
                stresstestResult = _stresstestCore.ExecuteStresstest();
                _stresstestResults = _stresstestCore.StresstestResults;
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
                       Stop(ex);
                       try
                       {
                           stresstestReportControl.StresstestResults = _stresstestResults;
                           stresstestReportControl.SetConfigurationLabels();
                           stresstestReportControl.MakeReport();
                       }
                       catch (Exception e)
                       {
                           LogWrapper.LogByLevel(this.Text + ": Failed making a stresstest report.\n" + e.ToString(), LogLevel.Error);
                       }

                       if (_monitorViews != null)
                           foreach (var view in _monitorViews)
                               if (view != null && view.Tag != null)
                                   try
                                   {
                                       var monitorReportControl = view.Tag as MonitorReportControl;
                                       monitorReportControl.SetConfig_Headers_MonitorValuesAndStresstestResults(view.Configuration, view.GetHeaders(), view.GetMonitorValues(), _stresstestResults);
                                   }
                                   catch (Exception e)
                                   {
                                       LogWrapper.LogByLevel(view.Text + ": Failed making a monitor report.\n" + e.ToString(), LogLevel.Error);
                                   }

                       string message = null;
                       switch (stresstestResult)
                       {
                           case StresstestResult.Ok:
                               message = string.Format("The test completed succesfully in {0}.", _stresstestResults.Metrics.MeasuredRunTime.ToLongFormattedString());
                               stresstestControl.SetStresstestStopped(stresstestResult, message);
                               break;
                           case StresstestResult.Cancelled:
                               message = "The stresstest was cancelled.";
                               stresstestControl.SetStresstestStopped(stresstestResult, message);
                               break;
                           case StresstestResult.Error:
                               message = "The stresstest failed!";
                               stresstestControl.SetStresstestStopped(stresstestResult, message);
                               break;
                       }
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
            int i = 3;
            while (tc.TabCount != 3)
                tc.TabPages.RemoveAt(3);

            foreach (var monitor in _stresstest.Monitors)
            {
                var monitorView = SolutionComponentViewManager.Show(monitor) as Monitor.MonitorView;
                this.Show();

                stresstestControl.AppendMessages("Initializing " + monitorView.Text + "...");
                _monitorViews.Add(monitorView);

                //Add a new tab page.
                var monitorTabPage = new TabPage("Report " + monitorView.Text);
                var monitorReportControl = new MonitorReportControl();
                monitorReportControl.Dock = DockStyle.Fill;

                monitorTabPage.Controls.Add(monitorReportControl);
                tc.TabPages.Add(monitorTabPage);

                //For easy reporting
                monitorView.Tag = monitorReportControl;

                //Initialize
                monitorView.MonitorInitialized += new EventHandler<MonitorView.MonitorInitializedEventArgs>(monitorView_MonitorInitialized);
                monitorView.OnHandledException += new EventHandler<ErrorEventArgs>(monitorView_OnHandledException);
                monitorView.OnUnhandledException += new EventHandler<ErrorEventArgs>(monitorView_OnUnhandledException);
                monitorView.InitializeForStresstest();
            }
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
                foreach (var view in _monitorViews)
                    if (view != null && !view.IsDisposed)
                        try
                        {
                            view.Start();
                            stresstestControl.AppendMessages(view.Text + " is started.");
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                LogWrapper.LogByLevel(view.Text + " is not started.\n" + e.ToString(), LogLevel.Error);
                                stresstestControl.AppendMessages(view.Text + " is not started.");
                            }
                            catch { }
                        }
        }
        #endregion

        #region Progress
        private void _stresstestCore_StresstestStarted(object sender, StresstestStartedEventArgs e)
        {
            stresstestControl.SetStresstestResults(e.Result);
        }
        private void _stresstestCore_ConcurrentUsersStarted(object sender, ConcurrentUsersStartedEventArgs e)
        {
            _countDown = Stresstest.ProgressUpdateDelay;
            StopProgressDelayCountDown();
            tmrProgress.Stop();
            stresstestControl.AddFastResult(e.Result);
            try
            {
                stresstestControl.SetClientMonitoring(_stresstestCore.UsedThreadCount, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
            }
            catch { } //Exception on false WMI. 
        }
        private void _stresstestCore_PrecisionStarted(object sender, PrecisionStartedEventArgs e)
        {
            _countDown = Stresstest.ProgressUpdateDelay;
            StopProgressDelayCountDown();
            tmrProgress.Stop();
            stresstestControl.AddFastResult(e.Result);
            try
            {
                stresstestControl.SetClientMonitoring(_stresstestCore.UsedThreadCount, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
            }
            catch { } //Exception on false WMI. 
        }
        private void _stresstestCore_RunInitializedFirstTime(object sender, RunInitializedFirstTimeEventArgs e)
        {
            _countDown = Stresstest.ProgressUpdateDelay;
            StopProgressDelayCountDown();
            tmrProgress.Stop();
            stresstestControl.AddFastResult(e.Result);
            try
            {
                stresstestControl.SetClientMonitoring(_stresstestCore.UsedThreadCount, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
            }
            catch { } //Exception on false WMI. 

            stresstestControl.SetCountDownProgressDelay(_countDown--);
            tmrProgressDelayCountDown.Start();

            tmrProgress.Start();
        }
        private void tmrProgressDelayCountDown_Tick(object sender, EventArgs e)
        {
            stresstestControl.SetCountDownProgressDelay(_countDown);
            --_countDown;
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
                stresstestControl.UpdateFastResults();

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
                Stop();
            else
                // Can only be cancelled once, calling multiple times is not a problem.
                _stresstestCore.Cancel();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex">The exception if failed.</param>
        private void Stop(Exception ex = null)
        {
            Cursor = Cursors.WaitCursor;

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
            {
                string message = "The stresstest threw an exception:\n" + ex.Message + "\n\nSee " + Path.Combine(Logger.DEFAULT_LOCATION, DateTime.Now.ToString("dd-MM-yyyy") + " " + LogWrapper.Default.Logger.Name + ".txt");
                stresstestControl.SetStresstestStopped(StresstestResult.Error, message);
            }

            if (_stresstestCore != null && !_stresstestCore.IsDisposed)
            {
                _stresstestCore.Dispose();
                _stresstestCore = null;
            }

            Cursor = Cursors.Default;
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
                stresstestControl.UpdateFastResults();

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