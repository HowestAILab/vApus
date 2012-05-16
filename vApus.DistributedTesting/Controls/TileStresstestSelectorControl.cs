/*
 * Copyright 2010 (c) Sizing Servers Lab
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
    public partial class TileStresstestSelectorControl : UserControl
    {
        public new event EventHandler Click;
        /// <summary>
        /// If the monitor is initialized (if any) this is fired so that the stresstest can begin.
        /// </summary>
        public event EventHandler<MonitorView.MonitorInitializedEventArgs> MonitorInitialized;
        public event EventHandler<ErrorEventArgs> MonitorOnHandledException, MonitorOnUnhandledException;


        #region Fields
        private OldTileStresstest _tileStresstest;

        /// <summary>
        /// The monitor if any.
        /// </summary>
        private List<MonitorView> _monitorViews = new List<MonitorView>();

        private PushMessage _pushMessage;
        private Stresstest.StresstestResult _stresstestResult = Stresstest.StresstestResult.Busy;


        private TorrentClient _torrentClient;
        private DistributedTestCore _distributedTestCore;

        /// <summary>
        /// The stresstest report filename.
        /// </summary>
        private string RFilename;
        //To make sure that the same report cannot be loading while it is still busy.
        private static object _lock = new object();
        private volatile int _loadingStresstestReport = -1;
        private StresstestResults _stresstestResults;

        private StresstestReportControl _stresstestReportControl = null;
        #endregion

        #region Properties
        public OldTileStresstest TileStresstest
        {
            get { return _tileStresstest; }
        }
        //All the monitor views linked to this stresstest.
        public List<MonitorView> MonitorViews
        {
            get { return _monitorViews; }
            //This must be set for a report tile stresstest progress control.
            set { _monitorViews = value; }
        }
        /// <summary>
        /// This is needed to be able to visualize the stresstest progress and for the stresstest report.
        /// </summary>
        public PushMessage MostRecentPushMessage
        {
            get { return _pushMessage; }
            set
            {
                _pushMessage = value;

                if (pb.BeginOfTimeFrame == DateTime.MinValue)
                    pb.BeginOfTimeFrame = DateTime.Now;

                if (pic.Image == global::vApus.DistributedTesting.Properties.Resources.Error)
                    StresstestResult = Stresstest.StresstestResult.Error;
                else
                    StresstestResult = _pushMessage.StresstestResult;
            }
        }
        /// <summary>
        /// This is needed to be able to make the stresstest report.
        /// </summary>
        public Stresstest.StresstestResult StresstestResult
        {
            get { return _stresstestResult; }
            set
            {
                _stresstestResult = value;
                if (_stresstestResult == Stresstest.StresstestResult.Ok)
                {
                    pic.Image = global::vApus.DistributedTesting.Properties.Resources.OK;

                    pb.EndOfTimeFrame = DateTime.Now;
                    pb.SetProgressBarToNow();
                }
                else
                {
                    if (_stresstestResult == Stresstest.StresstestResult.Busy)
                        if (pic.Tag == null || (int)pic.Tag == 1)
                        {
                            pic.Image = global::vApus.DistributedTesting.Properties.Resources.Busy;
                            pic.Tag = 0;
                        }
                        else
                        {
                            pic.Image = global::vApus.DistributedTesting.Properties.Resources.Busy2;
                            pic.Tag = 1;
                        }
                    else if (_stresstestResult == Stresstest.StresstestResult.Cancelled)
                        pic.Image = global::vApus.DistributedTesting.Properties.Resources.Cancelled;
                    else
                        pic.Image = global::vApus.DistributedTesting.Properties.Resources.Error;

                    if (_pushMessage.TileStresstestProgressResults != null)
                    {
                        pb.EndOfTimeFrame = DateTime.Now + _pushMessage.TileStresstestProgressResults.EstimatedRuntimeLeft;
                        pb.SetProgressBarToNow();
                    }
                }
            }
        }
        /// <summary>
        /// This is needed to be able to make the stresstest report.
        /// </summary>
        public StresstestResults StresstestResults
        {
            get { return _stresstestResults; }
        }
        /// <summary>
        /// This is needed to be able to visualize the stresstest progress.
        /// </summary>
        public DateTime BeginOfTimeFrame
        {
            get { return pb.BeginOfTimeFrame; }
        }
        /// <summary>
        /// This is needed to be able to visualize the stresstest progress.
        /// </summary>
        public DateTime EndOfTimeFrame
        {
            get { return pb.EndOfTimeFrame; }
        }
        #endregion

        #region Constructors
        public TileStresstestSelectorControl()
        {
            InitializeComponent();
            pic.Image = global::vApus.DistributedTesting.Properties.Resources.Busy;
        }
        /// <summary>
        /// This will initialize a new monitor if the property is set to true.
        /// </summary>
        /// <param name="tileStresstest"></param>
        public TileStresstestSelectorControl(OldTileStresstest tileStresstest)
            : this()
        {
            _tileStresstest = tileStresstest;
            lblTileStresstest.Text = string.Format("{0} {1}", _tileStresstest.Parent, _tileStresstest);
        }
        #endregion

        #region Functions
        public void ShowMonitorView(vApus.Monitor.Monitor monitor)
        {
            //show the monitorview
            var view = SolutionComponentViewManager.Show(monitor) as Monitor.MonitorView;
            view.Tag = new MonitorReportControl();
            //For each view initialized, DistributedStresstestControl takes care of starting the test.
            view.MonitorInitialized += new EventHandler<MonitorView.MonitorInitializedEventArgs>(view_MonitorInitialized);
            view.OnHandledException += new EventHandler<ErrorEventArgs>(view_OnHandledException);
            view.OnUnhandledException += new EventHandler<ErrorEventArgs>(view_OnUnhandledException);
            view.InitializeForStresstest();

            _monitorViews.Add(view);
        }

        private void view_MonitorInitialized(object sender, MonitorView.MonitorInitializedEventArgs e)
        {
            MonitorView view = sender as MonitorView;
            view.MonitorInitialized -= view_MonitorInitialized;
            if (MonitorInitialized != null)
                MonitorInitialized(sender, e);
        }
        private void view_OnHandledException(object sender, ErrorEventArgs e)
        {
            MonitorView view = sender as MonitorView;
            if (MonitorOnUnhandledException != null)
                foreach (EventHandler<ErrorEventArgs> del in MonitorOnHandledException.GetInvocationList())
                    del.BeginInvoke(sender, e, null, null);
        }
        private void view_OnUnhandledException(object sender, ErrorEventArgs e)
        {
            MonitorView view = sender as MonitorView;
            if (MonitorOnUnhandledException != null)
                foreach (EventHandler<ErrorEventArgs> del in MonitorOnUnhandledException.GetInvocationList())
                    del.BeginInvoke(sender, e, null, null);
        }
        /// <summary>
        /// Used in stresstest started eventhandling.
        /// </summary>
        public void StartMonitorIfAny()
        {
            if (_monitorViews != null)
                foreach (MonitorView view in _monitorViews)
                    if (view != null && !view.IsDisposed)
                        try
                        {
                            view.Start();
                        }
                        catch 
                        {
                            try { view.Stop(); }
                            catch { }
                        }
        }
        /// <summary>
        /// Only used in stop
        /// </summary>
        public void StopMonitorIfAny()
        {
            if (_monitorViews != null)
                foreach (MonitorView view in _monitorViews)
                    if (view != null && !view.IsDisposed)
                        view.Stop();
        }
        private void lbl_SizeChanged(object sender, EventArgs e)
        {
            Width = lblTileStresstest.Right + 3;
        }
        private void TileStresstsControl_Enter(object sender, EventArgs e)
        {
            Focus();
        }
        private void lblTileStresstest_Click(object sender, EventArgs e)
        {
            Focus();
            if (Click != null)
                Click(this, null);
        }
        protected override void OnGotFocus(EventArgs e)
        {
            Select();
            base.OnGotFocus(e);
        }
        protected new void Select()
        {
            foreach (TileStresstestSelectorControl control in Parent.Controls)
                if (control != this)
                    control.Deselect();

            this.Font = new Font(this.Font, FontStyle.Bold);
            this.BackColor = Color.LightBlue;

            base.Select();
        }
        protected void Deselect()
        {
            this.Font = new Font(this.Font, FontStyle.Regular);
            this.BackColor = Color.FromArgb(240, 240, 240);
        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (Click != null)
                Click(this, null);
        }
        internal TorrentClient DownloadTorrent(byte[] torrentInfo, string path, DistributedTestCore distributedTestCore)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                lblTileStresstest.Text = string.Format("{0} {1} [...]", _tileStresstest.Parent, _tileStresstest);
            });

            _distributedTestCore = distributedTestCore;
            if (_torrentClient != null)
            {
                try { _torrentClient.StopTorrent(); }
                catch { }
                _torrentClient = null;
            }
            _torrentClient = new TorrentClient();
            _torrentClient.ProgressUpdated += new ProgressUpdatedEventHandler(_torrentClient_ProgressUpdated);
            _torrentClient.DownloadCompleted += new DownloadCompletedEventHandler(_torrentClient_DownloadCompleted);

            RFilename = Path.Combine(path, _torrentClient.DownloadTorrentFromBytes(torrentInfo, path));

            return _torrentClient;
        }

        private void _torrentClient_ProgressUpdated(TorrentClient source, TorrentEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                lblTileStresstest.Text = string.Format("{0} {1} [{2} %]", _tileStresstest.Parent, _tileStresstest, (int)e.PercentCompleted);
            });
        }
        private void _torrentClient_DownloadCompleted(object source, TorrentEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                lblTileStresstest.Text = string.Format("{0} {1} [Results Fetched!]", _tileStresstest.Parent, _tileStresstest);
            });

            _torrentClient.StopTorrent();
            _distributedTestCore.StopSeedingResults(_tileStresstest, _torrentClient.Name);
            _torrentClient = null;

            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                TileStresstestSelectorControl reportTileStresstestSelectorControl = Tag as TileStresstestSelectorControl;
                reportTileStresstestSelectorControl.RFilename = RFilename;
                reportTileStresstestSelectorControl.Enabled = true;
            });
        }
        /// <summary>
        /// Load the stresstest report for this tile stresstest selector control.
        /// </summary>
        /// <param name="stresstestReportControl"></param>
        /// <returns>All the monitor views of wich the tags are monitor report controls.</returns>
        public IEnumerable<MonitorView> LoadStresstestAndMonitorReport(StresstestReportControl stresstestReportControl)
        {
            if (Interlocked.Equals(_loadingStresstestReport, -1))
            {
                _loadingStresstestReport = 0;
                _stresstestReportControl = stresstestReportControl;
                if (_stresstestResults == null)
                {
                    _stresstestReportControl.ReportMade += new EventHandler(reportControl_ReportMade);
                    _stresstestReportControl.LoadRFile(RFilename);
                }
                else
                {
                    _stresstestReportControl.StresstestResults = _stresstestResults;
                    _stresstestReportControl.SetConfigurationLabels();
                    _stresstestReportControl.MakeReport();
                    _loadingStresstestReport = -1;
                }
                
                if (_monitorViews != null)
                    foreach (var view in _monitorViews)
                        if (view != null && view.Tag != null)
                        {
                            var monitorReportControl = view.Tag as MonitorReportControl;
                            //Only do this when loaded offcorse.
                            if (_stresstestResults != null)
                                monitorReportControl.SetHeaders_MonitorValuesAndStresstestResults(view.GetHeaders(), view.GetMonitorValues(), _stresstestResults);
                            yield return view;
                        }
            }
        }
        private void reportControl_ReportMade(object sender, EventArgs e)
        {
            StresstestReportControl reportControl = sender as StresstestReportControl;
            if (reportControl.RFileName == RFilename)
            {
                reportControl.ReportMade -= reportControl_ReportMade;
                _stresstestResults = reportControl.StresstestResults;
            }

            if (_monitorViews != null)
                foreach (var view in _monitorViews)
                    if (view != null && view.Tag != null)
                    {
                        var monitorReportControl = view.Tag as MonitorReportControl;
                        monitorReportControl.SetHeaders_MonitorValuesAndStresstestResults(view.GetHeaders(), view.GetMonitorValues(), _stresstestResults);
                    }

            _loadingStresstestReport = -1;
        }
        #endregion
    }
}
