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
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public partial class TileStresstestView : BaseSolutionComponentView
    {
        #region Fields
        /// <summary>
        /// Lock break, continue, push message
        /// </summary>
        private object _lock = new object();

        // private System.Timers.Timer _messageTimer = new System.Timers.Timer();

        private string _tileStresstestIndex;
        private RunSynchronization _runSynchronization;

        //To report progress to the master.
        private TileStresstestProgressResults _tileStresstestProgressResults;
        private TileConcurrentUsersProgressResult _lastAddedTileConcurrentUsersProgressResult;
        private TilePrecisionProgressResult _lastTilePrecisionProgressResult;

        private Stresstest.Stresstest _stresstest;
        private Stresstest.StresstestCore _stresstestCore;

        private Stresstest.StresstestResult _stresstestResult;
        private Stresstest.StresstestResults _stresstestResults;
        private HashSet<ListViewItem> _resultListViewItems = new HashSet<ListViewItem>();

        /// <summary>
        /// Countdown for the update.
        /// </summary>
        private int _countDown;
        #endregion

        #region Properties
        /// <summary>
        /// Store to identify the right stresstest.
        /// </summary>
        public string TileStresstestIndex
        {
            get { return _tileStresstestIndex; }
            set { _tileStresstestIndex = value; }
        }
        public RunSynchronization RunSynchronization
        {
            get { return _runSynchronization; }
            set { _runSynchronization = value; }
        }
        public Stresstest.StresstestResults StresstestResults
        {
            get { return _stresstestResults; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Designer time constructor
        /// </summary>
        public TileStresstestView()
        {
            InitializeComponent();
        }
        public TileStresstestView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            _stresstest = SolutionComponent as Stresstest.Stresstest;

            InitializeComponent();
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
            //_messageTimer.Interval = tmrProgress.Interval;
            //_messageTimer.Elapsed += new System.Timers.ElapsedEventHandler(_messageTimer_Elapsed);
        }
        public override void Refresh()
        {
            base.Refresh();
            SetGui();
        }
        #endregion

        #region Start
        /// <summary>
        /// Thread safe
        /// </summary>
        public void InitializeTest()
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                Cursor = Cursors.WaitCursor;
                btnStop.Enabled = true;
                LocalMonitor.StartMonitoring(Stresstest.Stresstest.ProgressUpdateDelay * 1000);
                tmrProgress.Interval = Stresstest.Stresstest.ProgressUpdateDelay * 1000;
                // _messageTimer.Start();

                stresstestControl.SetStresstestInitialized();
                stresstestControl.SetConfigurationControls(_stresstest);

                _countDown = Stresstest.Stresstest.ProgressUpdateDelay - 1;
                try
                {
                    _stresstestCore = new Stresstest.StresstestCore(_stresstest, false);
                    _stresstestCore.RunSynchronization = this.RunSynchronization;
                    _stresstestCore.StresstestStarted += new EventHandler<Stresstest.StresstestStartedEventArgs>(_stresstestCore_StresstestStarted);
                    _stresstestCore.ConcurrentUsersStarted += new EventHandler<Stresstest.ConcurrentUsersStartedEventArgs>(_stresstestCore_ConcurrentUsersStarted);
                    _stresstestCore.PrecisionStarted += new EventHandler<Stresstest.PrecisionStartedEventArgs>(_stresstestCore_PrecisionStarted);
                    _stresstestCore.RunInitializedFirstTime += new EventHandler<Stresstest.RunInitializedFirstTimeEventArgs>(_stresstestCore_RunInitializedFirstTime);
                    _stresstestCore.RunDoneOnce += new EventHandler(_stresstestCore_RunDoneOnce);
                    _stresstestCore.Message += new EventHandler<Stresstest.MessageEventArgs>(_stresstestCore_Message);
                    _stresstestCore.InitializeTest();

                    stresstestControl.SetClientMonitoring(_stresstestCore.BusyThreadCount, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
                    stresstestControl.UpdateFastResults();
                }
                catch (Exception ex)
                {
                    Stop(ex);
                }
                Cursor = Cursors.Default;
            });
        }
        /// <summary>
        /// Thread safe
        /// </summary>
        public void StartTest()
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                if (_stresstestCore == null)
                    return;

                _stresstestResult = Stresstest.StresstestResult.Busy;

                tmrProgress.Start();
                tmrProgressDelayCountDown.Start();

                Thread stresstestThread = new Thread(StartStresstestInBackground);
                stresstestThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                stresstestThread.IsBackground = true;
                stresstestThread.Start();
            });
        }
        private void StartStresstestInBackground()
        {
            Exception ex = null;
            try
            {
                _stresstestResult = _stresstestCore.ExecuteStresstest();
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
                        string message = null;
                        switch (_stresstestResult)
                        {
                            case StresstestResult.Ok:
                                message = string.Format("The test completed succesfully in {0}.", _stresstestResults.Metrics.MeasuredRunTime.ToLongFormattedString());
                                stresstestControl.SetStresstestStopped(_stresstestResult, message);
                                break;
                            case StresstestResult.Cancelled:
                                message = "The stresstest was cancelled.";
                                stresstestControl.SetStresstestStopped(_stresstestResult, message);
                                break;
                            case StresstestResult.Error:
                                message = "The stresstest failed!";
                                stresstestControl.SetStresstestStopped(_stresstestResult, message);
                                break;
                        }
                        Stop(ex);
                    });
                }
            }
        }
        #endregion

        /// <summary>
        /// Thread safe
        /// </summary>
        public void Break()
        {
            lock (_lock)
                if (_stresstestResult == StresstestResult.Busy)
                {
                    tmrProgress.Stop();
                    _stresstestCore.Break();
                }
        }
        /// <summary>
        /// Thread safe
        /// </summary>
        /// <param name="continueCounter">Every time the execution is paused the continue counter is incremented by one.</param>
        public void Continue(int continueCounter)
        {
            lock (_lock)
                if (_stresstestResult == StresstestResult.Busy)
                {
                    tmrProgress.Start();
                    _stresstestCore.Continue(continueCounter);
                }
        }

        #region Progress
        private void _stresstestCore_StresstestStarted(object sender, Stresstest.StresstestStartedEventArgs e)
        {
            stresstestControl.SetStresstestResults(e.Result);

            if (_stresstestCore != null && !_stresstestCore.IsDisposed)
            {
                _stresstestResults = _stresstestCore.StresstestResults;
                _tileStresstestProgressResults = new TileStresstestProgressResults(_stresstestResults);
            }
        }
        private void _stresstestCore_ConcurrentUsersStarted(object sender, Stresstest.ConcurrentUsersStartedEventArgs e)
        {
            _countDown = Stresstest.Stresstest.ProgressUpdateDelay;
            tmrProgressDelayCountDown.Stop();
            tmrProgress.Stop();
            stresstestControl.AddFastResult(e.Result);
            stresstestControl.SetClientMonitoring(_stresstestCore.UsedThreadCount, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);

            _lastAddedTileConcurrentUsersProgressResult = new TileConcurrentUsersProgressResult(e.Result);
            _tileStresstestProgressResults.TileConcurrentUsersProgressResults.Add(_lastAddedTileConcurrentUsersProgressResult);
        }
        private void _stresstestCore_PrecisionStarted(object sender, Stresstest.PrecisionStartedEventArgs e)
        {
            _countDown = Stresstest.Stresstest.ProgressUpdateDelay;
            tmrProgressDelayCountDown.Stop();
            tmrProgress.Stop();
            stresstestControl.AddFastResult(e.Result);
            stresstestControl.SetClientMonitoring(_stresstestCore.UsedThreadCount, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);

            _lastTilePrecisionProgressResult = new TilePrecisionProgressResult(e.Result);
            _lastAddedTileConcurrentUsersProgressResult.TilePrecisionProgressResults.Add(_lastTilePrecisionProgressResult);
        }
        private void _stresstestCore_RunInitializedFirstTime(object sender, Stresstest.RunInitializedFirstTimeEventArgs e)
        {
            _countDown = Stresstest.Stresstest.ProgressUpdateDelay;
            tmrProgressDelayCountDown.Stop();
            tmrProgress.Stop();
            stresstestControl.AddFastResult(e.Result);
            stresstestControl.SetClientMonitoring(_stresstestCore.UsedThreadCount, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);

            _lastTilePrecisionProgressResult.TileRunProgressResults.Add(new TileRunProgressResult(e.Result));

            SendPushMessage(RunStateChange.ToRunInitializedFirstTime);

            stresstestControl.SetCountDownProgressDelay(_countDown--);
            tmrProgressDelayCountDown.Start();

            tmrProgress.Start();
        }
        private void _stresstestCore_RunDoneOnce(object sender, EventArgs e)
        {
            SendPushMessage(RunStateChange.ToRunDoneOnce);
        }
        private void tmrProgressDelayCountDown_Tick(object sender, EventArgs e)
        {
            stresstestControl.SetCountDownProgressDelay(_countDown);
            --_countDown;
        }
        private void tmrProgress_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                stresstestControl.SetClientMonitoring(_stresstestCore == null ? 0 : _stresstestCore.BusyThreadCount, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
                stresstestControl.UpdateFastResults();
            });

            _countDown = Stresstest.Stresstest.ProgressUpdateDelay;

            SendPushMessage(RunStateChange.None);
        }

        /// <summary>
        /// </summary>
        /// <param name="concurrentUsersStateChange"></param>
        private void SendPushMessage(RunStateChange concurrentUsersStateChange)
        {
            SlaveSideCommunicationHandler.SendPushMessage(_tileStresstestIndex,
                _tileStresstestProgressResults,
                _stresstestResult,
                _stresstestCore,
                stresstestControl.GetEvents(),
                concurrentUsersStateChange);
        }

        /// <summary>
        /// Refreshes the results for a selected node and refreshes the listed results.
        /// </summary>
        private void _stresstestCore_Message(object sender, Stresstest.MessageEventArgs e)
        {
            if (e.Color == Color.Empty)
                stresstestControl.AppendMessages(e.Message, e.LogLevel);
            else
                stresstestControl.AppendMessages(e.Message, e.Color, e.LogLevel);
        }

        #endregion

        #region Stop
        private void TileStresstestView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!btnStop.Enabled || MessageBox.Show("Are you sure you want to close a running test?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                Stop();
            else
                e.Cancel = true;
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            PerformStopClick();
        }
        /// <summary>
        /// To stop the test from the slave side communication handler.
        /// </summary>
        public void PerformStopClick()
        {
            if (_stresstestCore != null)
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

            //StopMonitor();
            StopStresstest();

            tmrProgress.Stop();
            StopProgressDelayCountDown();

            btnStop.Enabled = false;

            if (ex != null)
            {
                _stresstestResult = StresstestResult.Error;
                string message = "The stresstest threw an exception:\n" + ex.Message + "\n\nSee " + Path.Combine(Logger.DEFAULT_LOCATION, DateTime.Now.ToString("dd-MM-yyyy") + " " + LogWrapper.Default.Logger.Name + ".txt");
                stresstestControl.SetStresstestStopped(_stresstestResult, message);
            }

            if (_stresstestCore != null && !_stresstestCore.IsDisposed)
            {
                _stresstestCore.Dispose();
                _stresstestCore = null;
            }

            SendPushMessage(RunStateChange.None);

            Cursor = Cursors.Default;
        }
        /// <summary>
        /// Only used in stop
        /// </summary>
        private void StopStresstest()
        {
            if (_stresstestCore != null)
            {
                stresstestControl.SetClientMonitoring(_stresstestCore.BusyThreadCount, LocalMonitor.CPUUsage, LocalMonitor.ContextSwitchesPerSecond, (int)LocalMonitor.MemoryUsage, (int)LocalMonitor.TotalVisibleMemory, LocalMonitor.NicsSent, LocalMonitor.NicsReceived);
                stresstestControl.UpdateFastResults();
                stresstestControl.SlaveSideSaveResults();

                // Can only be cancelled once, calling multiple times is not a problem.
                _stresstestCore.Cancel();
            }

            stresstestControl.SetStresstestStopped();
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