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
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using vApus.Monitor;
using vApus.Results;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting {
    public partial class TileStresstestView : BaseSolutionComponentView {

        #region Fields

        /// <summary>
        ///     Lock break, continue, push message
        /// </summary>
        private readonly object _lock = new object();

        private readonly Stresstest.Stresstest _stresstest;

        /// <summary>
        ///     Countdown for the update.
        /// </summary>
        private int _countDown;
        /// <summary>
        ///     In seconds how fast the stresstest progress will be updated.
        /// </summary>
        private const int PROGRESSUPDATEDELAY = 5;

        /// <summary>
        ///     Caching the results to visualize in the stresstestcontrol.
        /// </summary>
        private StresstestMetricsCache _stresstestMetricsCache;

        /// <summary>
        ///     Don't resend if it is finished (stop on form closing);
        /// </summary>
        private bool _finishedSent;

        private StresstestCore _stresstestCore;
        private StresstestResult _stresstestResult;
        private StresstestStatus _stresstestStatus;
        private string _tileStresstestIndex;

        private ResultsHelper _resultsHelper = new ResultsHelper();

        #endregion

        #region Properties

        /// <summary>
        ///     Store to identify the right stresstest.
        /// </summary>
        public string TileStresstestIndex {
            get { return _tileStresstestIndex; }
            set { _tileStresstestIndex = value; }
        }

        public RunSynchronization RunSynchronization { get; set; }
        public int MaxRerunsBreakOnLast { get; set; }
        public StresstestResult StresstestResult {
            get { return _stresstestResult; }
        }
        /// <summary>
        /// For adding results to the database.
        /// </summary>
        public int StresstestIdInDb {
            get { return _resultsHelper.StresstestId; }
            set { _resultsHelper.StresstestId = value; }
        }
        #endregion

        #region Constructor

        /// <summary>
        ///     Designer time constructor
        /// </summary>
        public TileStresstestView() {
            InitializeComponent();
        }

        public TileStresstestView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            Solution.RegisterForCancelFormClosing(this);
            _stresstest = SolutionComponent as Stresstest.Stresstest;

            InitializeComponent();
            if (IsHandleCreated)
                SetGui();
            else
                HandleCreated += StresstestProjectView_HandleCreated;
        }

        #endregion

        #region Functions

        #region Set the Gui

        private void StresstestProjectView_HandleCreated(object sender, EventArgs e) {
            SetGui();
        }

        private void SetGui() {
            Text = SolutionComponent.ToString();
            // fastResultsControl.ResultsHelper = _resultsHelper;
        }

        public override void Refresh() {
            base.Refresh();
            SetGui();
        }

        #endregion

        #region Start
        public void ConnectToExistingDatabase(string host, int port, string databaseName, string user, string password) {
            try {
                _resultsHelper.ConnectToExistingDatabase(host, port, databaseName, user, password);
            } catch {
                throw new Exception("MAKE SURE THAT YOU DO NOT TARGET 'localhost', '127.0.0.1', '0:0:0:0:0:0:0:1' or '::1' IN 'Options' > 'Saving Test Results'!\nA connection to the results server could not be made!");
            }
        }
        /// <summary>
        ///     Thread safe
        /// </summary>
        public void InitializeTest() {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                Cursor = Cursors.WaitCursor;
                btnStop.Enabled = true;
                try { LocalMonitor.StartMonitoring(PROGRESSUPDATEDELAY * 1000); } catch { fastResultsControl.AddEvent("Could not initialize the local monitor, something is wrong with your WMI.", LogLevel.Error); }
                tmrProgress.Interval = PROGRESSUPDATEDELAY * 1000;

                fastResultsControl.SetStresstestInitialized();
                _stresstestResult = null;
                _stresstestMetricsCache = new StresstestMetricsCache();
                fastResultsControl.SetConfigurationControls(_stresstest);

                _countDown = PROGRESSUPDATEDELAY - 1;
                try {
                    _stresstestCore = new StresstestCore(_stresstest);
                    _stresstestCore.ResultsHelper = _resultsHelper;
                    _stresstestCore.RunSynchronization = RunSynchronization;
                    _stresstestCore.MaxRerunsBreakOnLast = MaxRerunsBreakOnLast;
                    _stresstestCore.StresstestStarted += _stresstestCore_StresstestStarted;
                    _stresstestCore.ConcurrencyStarted += _stresstestCore_ConcurrentUsersStarted;
                    _stresstestCore.ConcurrencyStopped += _stresstestCore_ConcurrencyStopped;
                    _stresstestCore.RunInitializedFirstTime += _stresstestCore_RunInitializedFirstTime;
                    _stresstestCore.RunDoneOnce += _stresstestCore_RunDoneOnce;
                    _stresstestCore.RerunDone += _stresstestCore_RerunDone;
                    _stresstestCore.RunStopped += _stresstestCore_RunStopped;
                    _stresstestCore.Message += _stresstestCore_Message;
                    _stresstestCore.InitializeTest();

                    try {
                        fastResultsControl.SetClientMonitoring(_stresstestCore.BusyThreadCount, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond,
                                                              (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory,
                                                              LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
                    } catch { } //Exception on false WMI. 
                } catch (Exception ex) { Stop(ex); }
                Cursor = Cursors.Default;
            }, null);
        }

        /// <summary>
        ///     Thread safe
        /// </summary>
        public void StartTest() {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                if (_stresstestCore == null) return;

                _stresstestStatus = StresstestStatus.Busy;

                tmrProgress.Start();

                var stresstestThread = new Thread(StartStresstestInBackground);
                stresstestThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                stresstestThread.IsBackground = true;
                stresstestThread.Start();
            }, null);
        }

        private void StartStresstestInBackground() {
            _stresstestStatus = StresstestStatus.Busy;
            Exception ex = null;
            try {
                _stresstestStatus = _stresstestCore.ExecuteStresstest();
                _stresstestResult = _stresstestCore.StresstestResult;
            } catch (Exception e) { ex = e; } finally {
                if (_stresstestCore != null && !_stresstestCore.IsDisposed)
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                        Stop(ex);
                    }, null);
            }
        }

        #endregion

        /// <summary>
        ///     Thread safe
        /// </summary>
        public void Break() {
            lock (_lock)
                if (_stresstestStatus == StresstestStatus.Busy) {
                    tmrProgress.Stop();
                    _stresstestCore.Break();
                }
        }

        /// <summary>
        ///     Thread safe
        /// </summary>
        /// <param name="continueCounter">Every time the execution is paused the continue counter is incremented by one.</param>
        public void Continue(int continueCounter) {
            lock (_lock)
                if (_stresstestStatus == StresstestStatus.Busy) {
                    tmrProgress.Start();
                    _stresstestCore.Continue(continueCounter);
                }
        }

        #region Progress

        private void _stresstestCore_StresstestStarted(object sender, StresstestResultEventArgs e) {
            _stresstestResult = e.StresstestResult;
            fastResultsControl.SetStresstestStarted(e.StresstestResult.StartedAt);
        }

        private void _stresstestCore_ConcurrentUsersStarted(object sender, ConcurrencyResultEventArgs e) {
            _countDown = PROGRESSUPDATEDELAY;
            StopProgressDelayCountDown();
            tmrProgress.Stop();

            //Purge the previous concurrent user results from memory, we don't need it anymore.
            // Edit: For estimated runtime left we do need this.
            //foreach (var concurrencyResult in _stresstestResult.ConcurrencyResults)
            //    if (concurrencyResult.StoppedAt != DateTime.MinValue) {
            //        _stresstestResult.ConcurrencyResults.Remove(concurrencyResult);
            //        break;
            //    }

            //Update the metrics.
            fastResultsControl.UpdateFastConcurrencyResults(_stresstestMetricsCache.AddOrUpdate(e.Result));
            fastResultsControl.SetRerunning(false);
        }


        private void _stresstestCore_ConcurrencyStopped(object sender, ConcurrencyResultEventArgs e) { SendPushMessage(RunStateChange.None, false, true); }
        private void _stresstestCore_RunInitializedFirstTime(object sender, RunResultEventArgs e) {
            _countDown = PROGRESSUPDATEDELAY;
            StopProgressDelayCountDown();
            tmrProgress.Stop();

            fastResultsControl.UpdateFastRunResults(_stresstestMetricsCache.AddOrUpdate(e.Result));
            fastResultsControl.UpdateFastConcurrencyResults(_stresstestMetricsCache.GetConcurrencyMetrics(), false);

            SendPushMessage(RunStateChange.ToRunInitializedFirstTime, false, false);

            fastResultsControl.SetRerunning(false);

            fastResultsControl.SetCountDownProgressDelay(_countDown--);
            tmrProgressDelayCountDown.Start();

            tmrProgress.Start();
        }

        private void _stresstestCore_RunDoneOnce(object sender, EventArgs e) { SendPushMessage(RunStateChange.ToRunDoneOnce, false, false); }
        private void _stresstestCore_RerunDone(object sender, EventArgs e) { SendPushMessage(RunStateChange.ToRerunDone, false, false); }
        private void _stresstestCore_RunStopped(object sender, RunResultEventArgs e) { SendPushMessage(RunStateChange.None, true, false); }

        private void tmrProgressDelayCountDown_Tick(object sender, EventArgs e) { fastResultsControl.SetCountDownProgressDelay(_countDown--); }

        private void tmrProgress_Tick(object sender, ElapsedEventArgs e) {
            try {
                fastResultsControl.SetClientMonitoring(
                    _stresstestCore == null ? 0 : _stresstestCore.BusyThreadCount, LocalMonitor.CPUUsage,
                    LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage,
                    (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
            } catch { } //Exception on false WMI. 

            fastResultsControl.UpdateFastConcurrencyResults(_stresstestMetricsCache.GetConcurrencyMetrics());
            List<StresstestMetrics> runMetrics = _stresstestMetricsCache.GetRunMetrics();
            fastResultsControl.UpdateFastRunResults(runMetrics);

            //Set rerunning
            fastResultsControl.SetRerunning(runMetrics.Count == 0 ? false : runMetrics[runMetrics.Count - 1].RerunCount != 0);

            _countDown = PROGRESSUPDATEDELAY;

            SendPushMessage(RunStateChange.None, false, false);
        }

        /// <summary>
        /// </summary>
        /// <param name="runStateChange"></param>
        private void SendPushMessage(RunStateChange runStateChange, bool runFinished, bool concurrencyFinished) {
            if (!_finishedSent) {
                var estimatedRuntimeLeft = StresstestMetricsHelper.GetEstimatedRuntimeLeft(_stresstestResult, _stresstest.Concurrencies.Length, _stresstest.Runs);
                var events = new List<EventPanelEvent>();
                try { events = fastResultsControl.GetEvents(); } catch { }
                SlaveSideCommunicationHandler.SendPushMessage(_tileStresstestIndex, _stresstestMetricsCache, _stresstestStatus, fastResultsControl.StresstestStartedAt,
                    fastResultsControl.MeasuredRuntime, estimatedRuntimeLeft, _stresstestCore, events, runStateChange, runFinished, concurrencyFinished);
                if (_stresstestStatus != StresstestStatus.Busy) _finishedSent = true;
            }
        }

        /// <summary>
        ///     Refreshes the results for a selected node and refreshes the listed results.
        /// </summary>
        private void _stresstestCore_Message(object sender, MessageEventArgs e) {
            if (e.Color == Color.Empty) fastResultsControl.AddEvent(e.Message, e.LogLevel);
            else fastResultsControl.AddEvent(e.Message, e.Color, e.LogLevel);
        }

        #endregion

        #region Stop

        private void TileStresstestView_FormClosing(object sender, FormClosingEventArgs e) {
            if (!btnStop.Enabled || MessageBox.Show("Are you sure you want to close a running test?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                StopStresstest();

                tmrProgress.Stop();
                StopProgressDelayCountDown();


                if (_stresstestCore != null && !_stresstestCore.IsDisposed) {
                    _stresstestCore.Dispose();
                    _stresstestCore = null;
                }

                SendPushMessage(RunStateChange.None, false, false);
            } else {
                Solution.ExplicitCancelFormClosing = true;
                e.Cancel = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e) {
            PerformStopClick();
        }

        /// <summary>
        ///     To stop the test from the slave side communication handler.
        /// </summary>
        public void PerformStopClick() {
            if (btnStop.Enabled) {
                int busyThreadCount = -1;
                if (_stresstestCore != null) {
                    busyThreadCount = _stresstestCore.BusyThreadCount;
                    _stresstestCore.Cancel(); // Can only be cancelled once, calling multiple times is not a problem.
                }

                //Nasty, but last resort.
                ThreadPool.QueueUserWorkItem((state) => {
                    for (int i = 0; i != 30; i++) {
                        if (_stresstestCore == null)
                            break;
                        Thread.Sleep(1000);
                    }
                    if (_stresstestCore != null || busyThreadCount == 0)
                        SynchronizationContextWrapper.SynchronizationContext.Send((x) => {
                            Stop();
                        }, null);
                });
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="ex">The exception if failed.</param>
        private void Stop(Exception ex = null) {
            Cursor = Cursors.WaitCursor;
            try {
                StopStresstest();

                tmrProgress.Stop();
                StopProgressDelayCountDown();

                btnStop.Enabled = false;
                if (ex == null)
                    fastResultsControl.SetStresstestStopped(_stresstestStatus);
                else {
                    _stresstestStatus = StresstestStatus.Error;
                    fastResultsControl.SetStresstestStopped(_stresstestStatus, ex);
                }

                if (_stresstestCore != null && !_stresstestCore.IsDisposed) {
                    _stresstestCore.Dispose();
                    _stresstestCore = null;
                }

                SendPushMessage(RunStateChange.None, false, false);
            } catch (Exception eeee) {
                MessageBox.Show(eeee.ToString());
                LogWrapper.LogByLevel("Failed stopping the test.\n" + ex.Message + "\n" + ex.StackTrace, LogLevel.Error);
            }
            Cursor = Cursors.Default;
        }

        /// <summary>
        ///     Only used in stop
        /// </summary>
        private void StopStresstest() {
            if (_stresstestCore != null && !_stresstestCore.IsDisposed) {
                try {
                    fastResultsControl.SetClientMonitoring(_stresstestCore.BusyThreadCount, LocalMonitor.CPUUsage,
                                                          LocalMonitor.ContextSwitchesPerSecond,
                                                          (int)LocalMonitor.MemoryUsage,
                                                          (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent,
                                                          LocalMonitor.NicsReceived);
                } catch { } //Exception on false WMI. 

                fastResultsControl.UpdateFastConcurrencyResults(_stresstestMetricsCache.GetConcurrencyMetrics());
                fastResultsControl.UpdateFastRunResults(_stresstestMetricsCache.GetRunMetrics());

                fastResultsControl.SetRerunning(false);

                // Can only be cancelled once, calling multiple times is not a problem.
                if (_stresstestCore != null && !_stresstestCore.IsDisposed) try { _stresstestCore.Cancel(); } catch { }
            }

            fastResultsControl.SetStresstestStopped();
            _stresstestResult = null;

        }

        private void StopProgressDelayCountDown() {
            try {
                tmrProgressDelayCountDown.Stop();
                if (fastResultsControl != null && !fastResultsControl.IsDisposed)
                    fastResultsControl.SetCountDownProgressDelay(-1);
            } catch {
            }
        }

        #endregion

        #endregion
    }
}