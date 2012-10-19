/*
 * Copyright 2010 (c) Sizing Servers Lab
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
using System.Threading;
using System.Windows.Forms;
using vApus.Util;
using System.Text;

namespace vApus.Stresstest
{
    public partial class StresstestReportControl : UserControl
    {
        public event EventHandler ReportMade;
        [Description("Should only occur when loading old reports")]
        public event EventHandler<ErrorEventArgs> LoadingError;

        delegate void LoadRFileDel();

        #region Fields
        private const string VBLRn = "<16 0C 02 12n>";
        private const string VBLRr = "<16 0C 02 12r>";

        private RFileHandler _rFileHandler = new RFileHandler();
        private LoadRFileDel _loadRFile;
        private ActiveObject _activeObject = new ActiveObject();

        private string _rFileName;
        private List<object[]> _cache = new List<object[]>();
        private Thread _reportThread;
        private bool _cancel = false;

        //Can be null, but then StresstestResults needs to be filled in.
        public Stresstest Stresstest;
        public StresstestResults StresstestResults;

        private bool _canSaveRFile = true;

        #endregion

        /// <summary>
        /// Set the gui for this.
        /// </summary>
        public bool CanSaveRFile
        {
            get { return _canSaveRFile; }
            set
            {
                if (_canSaveRFile != value)
                {
                    _canSaveRFile = value;
                    btnSaveTheRFile.Visible = _canSaveRFile;
                }
            }
        }
        public string RFileName
        {
            get { return _rFileName; }
        }

        public StresstestReportControl()
        {
            InitializeComponent();

            _loadRFile = new LoadRFileDel(LoadRFile_Callback);
            _activeObject.OnResult += new EventHandler<ActiveObject.OnResultEventArgs>(ActiveObject_OnResult);

            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(DetailedResultsListing_HandleCreated);
        }

        private void DetailedResultsListing_HandleCreated(object sender, EventArgs e)
        {
            this.HandleCreated -= DetailedResultsListing_HandleCreated;
            SetGui();
        }
        private void SetGui()
        {
            SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;
            this.dgvDetailedResultsListing.DoubleBuffered(true);

            cboPivotLevel.SelectedIndex = 0;
            clmDRLUser.Visible = false;

            this.cboPivotLevel.SelectedIndexChanged += new System.EventHandler(this.cbo_SelectedIndexChanged);

            MakeReport();
        }

        #region Report
        public void LoadRFile(string rFileName)
        {
            if (File.Exists(rFileName))
            {
                if (Path.GetExtension(rFileName) == ".r")
                {
                    FileInfo f = new FileInfo(rFileName);

                    _rFileName = rFileName;

                    flpDetailedResultsListing.Enabled = false;
                    dgvDetailedResultsListing.Enabled = false;
                    btnSaveTheConfigurationAndTheChosenResultSet.Enabled = false;
                    btnSaveTheRFile.Enabled = false;

                    lblWait.Text = "[Please wait, loading file (can take a while)]";
                    lblWait.Visible = true;

                    _activeObject.Send(_loadRFile);

                }
                else
                {
                    throw new ArgumentException(rFileName + " is not a .r file");
                }
            }
            else
            {
                throw new ArgumentException(rFileName + " does not exist");
            }
        }
        private void LoadRFile_Callback()
        {
            int i = 0;
        Retry:
            try
            {
                StresstestResults = _rFileHandler.Load(_rFileName);
            }
            catch
            {
                Thread.Sleep(500);
                if (++i == 3)
                    throw;
                else
                    goto Retry;
            }
        }
        private void ActiveObject_OnResult(object sender, ActiveObject.OnResultEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                flpDetailedResultsListing.Enabled = true;
                dgvDetailedResultsListing.Enabled = true;

            }, null);

            if (e.Exception == null)
            {
                SetConfigurationLabels();
                MakeReport();
            }
            else if (LoadingError != null)
            {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    Exception ex = new Exception("Could not load the r-file because of a version mismatch:\n" + e.Exception);
                    LoadingError(this, new ErrorEventArgs(ex));
                }, null);
            }

            if (ReportMade != null)
                foreach (EventHandler del in ReportMade.GetInvocationList())
                    del.BeginInvoke(this, null, null, null);
        }

        /// <summary>
        /// The configuration of the stresstest.
        /// </summary>
        public void SetConfigurationLabels()
        {
            Exception exception = null;
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                try
                {
                    kvpSolution.Value = StresstestResults.Solution;
                    kvpStresstest.Key = StresstestResults.Stresstest;
                    kvpConnection.Key = StresstestResults.Connection;
                    kvpConnectionProxy.Key = StresstestResults.ConnectionProxy;

                    kvpLog.Key = StresstestResults.Log;
                    kvpLogRuleSet.Key = StresstestResults.LogRuleSet;

                    kvpMonitor.Key = StresstestResults.Monitors;

                    kvpConcurrency.Value = StresstestResults.Concurrency.Combine(", ");

                    kvpRuns.Value = StresstestResults.Runs.ToString();
                    kvpDelay.Value = (StresstestResults.MinimumDelay == StresstestResults.MaximumDelay ? StresstestResults.MinimumDelay.ToString() : StresstestResults.MinimumDelay + " - " + StresstestResults.MaximumDelay) + " ms";
                    kvpShuffle.Value = StresstestResults.Shuffle ? "Yes" : "No";
                    kvpDistribute.Value = StresstestResults.Distribute.ToString();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }, null);

            if (exception != null)
                throw (exception);
        }

        private void btnSaveTheConfigurationAndTheChosenResultSet_Click(object sender, EventArgs e)
        {
            sfd.Filter = "TXT|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = null;
                try
                {
                    using (sw = new StreamWriter(sfd.FileName))
                    {
                        //The config
                        foreach (Control ctrl in flpSaveConfiguration.Controls)
                            if (ctrl is KeyValuePairControl)
                            {
                                var kvp = ctrl as KeyValuePairControl;
                                if (kvp.Value.Length == 0)
                                    sw.WriteLine(kvp.Key);
                                else
                                    sw.WriteLine(kvp.Key + ": " + kvp.Value);
                            }
                        sw.WriteLine();

                        //The result set
                        List<int> l = new List<int>();
                        string headers = string.Empty;

                        for (int clmIndex = 0; clmIndex != dgvDetailedResultsListing.ColumnCount; clmIndex++)
                        {
                            DataGridViewColumn clm = dgvDetailedResultsListing.Columns[clmIndex];
                            if (clm.Visible)
                            {
                                l.Add(clmIndex);
                                headers += clm.HeaderText + StresstestResults.UniqueResultsDelimiter();
                            }
                        }
                        int[] columnIndices = l.ToArray();

                        if (columnIndices.Length == 0)
                            return;

                        sw.WriteLine(headers.Substring(0, headers.Length - 1));

                        foreach (object[] row in _cache)
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i != columnIndices.Length - 1; i++)
                            {
                                sb.Append(row[columnIndices[i]]);
                                sb.Append(StresstestResults.UniqueResultsDelimiter());
                            }

                            sb.Append(row[columnIndices[columnIndices.Length - 1]]);

                            sw.WriteLine(sb.ToString());
                        }
                        sw.Flush();
                    }
                }
                catch
                {
                    MessageBox.Show("The file could not be replaced because it is opened in another program.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Dispose();
                        sw = null;
                    }
                }
            }
        }
        private void btnSaveTheRFile_Click(object sender, EventArgs e)
        {
            sfd.Filter = "R-File|*.r";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                _rFileHandler.SetStresstestResults(StresstestResults, sfd.FileName);
                string fileName = _rFileHandler.Save();
                if (fileName != sfd.FileName)
                    MessageBox.Show("The R-file could not be saved to '" + sfd.FileName + "' therefore it is saved to '" + fileName + "'.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Cursor = Cursors.Default;
            }
        }
        private void filter_CheckedChanged(object sender, EventArgs e)
        {
            MakeReport();
        }
        private void filterRDB_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
                MakeReport();
        }
        private void cbo_SelectedIndexChanged(object sender, EventArgs e)
        {
            MakeReport();
        }

        private void dgvDetailedResultsListing_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                object[] row = _cache[e.RowIndex];
                if (row != null)
                {
                    object value = row[e.ColumnIndex];
                    if (value != null)
                        e.Value = value;
                }
            }
            catch { }
        }
        /// <summary>
        /// </summary>
        /// <param name="line">row cells</param>
        private void AddToCache(object[] row)
        {
            if (!_cancel)
                try
                {
                    _cache.Add(row);
                    int cacheCount = _cache.Count;
                    if (cacheCount == dgvDetailedResultsListing.RowCount + 5000)
                        SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                        {
                            dgvDetailedResultsListing.RowCount = cacheCount;
                        }, null);
                }
                catch { }
        }

        private void CancelMakeReport()
        {
            _cancel = true;
            while (_reportThread != null && _reportThread.ThreadState == ThreadState.Running)
            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    _reportThread.Join(5000);

                    if (_reportThread.ThreadState == ThreadState.Running)
                        _reportThread.Abort();
                }
                finally
                {
                    _reportThread = null;
                }
                this.Cursor = Cursors.Default;
            }
            _cancel = false;
        }
        /// <summary>
        /// Be carefull, this is to be used in a threadsafe context.
        /// </summary>
        public void ClearReport()
        {
            CancelMakeReport();

            StresstestResults = null;

            dgvDetailedResultsListing.Rows.Clear();
            _cache.Clear();
            dgvDetailedResultsListing.RowCount = 0;
        }
        /// <summary>
        /// Be carefull, this is to be used in a threadsafe context.
        /// </summary>
        public void MakeReport()
        {
            int pivotLevel = 0;
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                try
                {
                    CancelMakeReport();

                    btnSaveTheConfigurationAndTheChosenResultSet.Enabled = false;
                    btnSaveTheRFile.Enabled = false;
                    lblWait.Text = "[Please wait, calculating]";
                    if (StresstestResults != null)
                    {
                        lblWait.Visible = true;
                        btnRefresh.Enabled = false;
                    }

                    dgvDetailedResultsListing.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;

                    dgvDetailedResultsListing.Rows.Clear();
                    _cache.Clear();
                    dgvDetailedResultsListing.RowCount = 0;

                    dgvDetailedResultsListing.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                    pivotLevel = cboPivotLevel.SelectedIndex;
                }
                catch { }
            }, null);

            _reportThread = new Thread(delegate()
            {
                try
                {
                    if (rdbAverages.Checked)
                        MakeReportForAverages(pivotLevel);
                    else if (rdbUsers.Checked)
                        MakeReportForUsers();
                    else
                        MakeReportForErrors();

                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                    {
                        if (_cancel)
                            return;

                        int cacheCount = _cache.Count;
                        if (dgvDetailedResultsListing.RowCount != cacheCount)
                            dgvDetailedResultsListing.RowCount = cacheCount;

                        try
                        {
                            Color c = Color.FromArgb(250, 250, 250);
                            foreach (DataGridViewRow row in dgvDetailedResultsListing.Rows)
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    string cellValue = cell.Value.ToString();
                                    if (cellValue.Length == 0)
                                        cell.Style.BackColor = c;
                                }
                        }
                        catch { }

                        if (StresstestResults != null)
                        {
                            btnSaveTheConfigurationAndTheChosenResultSet.Enabled = true;
                            btnSaveTheRFile.Enabled = true;
                        }
                        lblWait.Visible = false;
                        btnRefresh.Enabled = true;
                    }, null);
                }
                catch (Exception e)
                {
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                    {
                        if (_cancel)
                            return;

                        if (LoadingError != null)
                        {
                            Exception ex = new Exception("Could not make the report:\n" + e);
                            LoadingError(this, new ErrorEventArgs(ex));
                        }
                    }, null);
                }
            });
            _reportThread.IsBackground = true;
            _reportThread.Start();
        }

        #region Averages
        private void MakeReportForAverages(int pivotLevel)
        {
            InitMakeReportForAverages();
            if (_cancel) return;

            switch (pivotLevel)
            {
                case 0:
                    MakeReportForAveragesPivotConcurrentUsers();
                    break;
                case 1:
                    MakeReportForAveragesPivotRun();
                    break;
            }
        }
        private void InitMakeReportForAverages()
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                if (_cancel) return;

                clmDRLStartedAtSentAt.HeaderText = "Started At";
                clmDRLStartedAtSentAt.Visible = true;
                clmDRLMeasuredTime.Visible = true;

                tbtnConcurrency.Visible = true;
                tbtnUser.Visible = false;

                clmDRLConcurrency.Visible = true;

                clmDRLUser.Visible = false;
                clmDRLUserAction.Visible = true;
                clmDRLLogEntry.Visible = true;
                clmDRLLogEntriesProcessed.Visible = true;
                clmDRLThroughput.Visible = true;
                clmDRLResponseTime.Visible = true;
                clmDRLMaxResponseTime.Visible = true;
                clmDRLPercentileMaxResponseTime.Visible = true;
                clmDRLDelay.Visible = true;
                clmDRLErrors.Visible = true;

                pnlBorderPivotLevel.Visible = true;
                lblPivotLevel.Visible = true;
            }, null);
        }

        private void MakeReportForAveragesPivotConcurrentUsers()
        {
            InitMakeReportForAveragesPivotConcurrentUsers();
            if (StresstestResults == null || _cancel) return;

            foreach (ConcurrencyResult cr in StresstestResults.ConcurrencyResults)
            {
                if (_cancel) return;
                if (tbtnConcurrency.Checked)
                    MakeReportForAveragesPivotConcurrentUsersCR(cr, false);

                Dictionary<UserActionResult, Metrics> combinedUserActionResults = cr.GetPivotedUserActionResults();
                foreach (UserActionResult uar in combinedUserActionResults.Keys)
                {
                    if (_cancel) return;
                    if (tbtnUserAction.Checked)
                        MakeReportForAveragesPivotConcurrentUsersUAR(cr, uar, combinedUserActionResults[uar]);

                    if (tbtnLogEntry.Checked)
                    {
                        Dictionary<LogEntryResult, Metrics> combinedLogEntryResults = cr.GetPivotedLogEntryResults(uar.UserAction);
                        foreach (LogEntryResult ler in combinedLogEntryResults.Keys)
                        {
                            if (_cancel) return;
                            MakeReportForAveragesPivotConcurrentUsersLER(cr, ler, combinedLogEntryResults[ler]);
                        }
                    }
                }
            }
        }
        private void InitMakeReportForAveragesPivotConcurrentUsers()
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                if (_cancel) return;
                tbtnRun.Visible = false;

                clmDRLRun.Visible = false;
            }, null);
        }
        private void MakeReportForAveragesPivotConcurrentUsersCR(ConcurrencyResult cr, bool runChecked)
        {
            bool lowestLevel = !runChecked && !tbtnUserAction.Checked && !tbtnLogEntry.Checked;
            object[] row = cr.DetailedLogEntryResultMetrics(lowestLevel);

            AddToCache(row);
        }
        private void MakeReportForAveragesPivotConcurrentUsersUAR(ConcurrencyResult cr, UserActionResult uar, Metrics metrics)
        {
            object[] row = null;
            if (tbtnLogEntry.Checked)
                row = new object[] { string.Empty, 
                             string.Empty, 
                             (tbtnConcurrency.Checked ? string.Empty : cr.ConcurrentUsers.ToString()),
                             string.Empty,
                             string.Empty,
                             string.Empty,
                             uar.UserAction,
                             string.Empty,
                             metrics.TotalLogEntriesProcessed,
                             string.Empty,
                             string.Empty,
                             string.Empty,
                             string.Empty,
                             string.Empty,
                             metrics.Errors
                           };
            else
                row = new object[] { string.Empty, 
                             string.Empty, 
                             (tbtnConcurrency.Checked ? string.Empty : cr.ConcurrentUsers.ToString()),
                             string.Empty,
                             string.Empty,
                             string.Empty,
                             uar.UserAction,
                             string.Empty,
                             metrics.TotalLogEntriesProcessed,
                             string.Empty,
                             metrics.AverageTimeToLastByte.TotalMilliseconds,
                             metrics.MaxTimeToLastByte.TotalMilliseconds,
                             metrics.Percentile95MaxTimeToLastByte.TotalMilliseconds, 
                             metrics.AverageDelay.TotalMilliseconds,
                             metrics.Errors
                           };
            AddToCache(row);
        }
        private void MakeReportForAveragesPivotConcurrentUsersLER(ConcurrencyResult cr, LogEntryResult ler, Metrics metrics)
        {
            if (!ler.Empty)
            {
                object[] row = {
                           string.Empty,
                           string.Empty,
                           (tbtnConcurrency.Checked || tbtnUserAction.Checked ? string.Empty : cr.ConcurrentUsers.ToString()),
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           (tbtnUserAction.Checked ? string.Empty : ler.UserAction),
                           ler.LogEntryString.Replace("\n", VBLRn).Replace("\r", VBLRr),
                           metrics.TotalLogEntriesProcessed,
                           string.Empty,
                           metrics.AverageTimeToLastByte.TotalMilliseconds,
                           metrics.MaxTimeToLastByte.TotalMilliseconds,
                           metrics.Percentile95MaxTimeToLastByte.TotalMilliseconds,
                           metrics.AverageDelay.TotalMilliseconds,
                           metrics.Errors
                           };

                AddToCache(row);
            }
        }

        private void MakeReportForAveragesPivotRun()
        {
            InitMakeReportForAveragesPivotRun();
            if (StresstestResults == null || _cancel) return;

            foreach (ConcurrencyResult cr in StresstestResults.ConcurrencyResults)
            {
                if (_cancel) return;
                if (tbtnConcurrency.Checked)
                    MakeReportForAveragesPivotConcurrentUsersCR(cr, tbtnRun.Checked);

                foreach (RunResult rr in cr.RunResults)
                {
                    if (_cancel) return;
                    if (tbtnRun.Checked)
                        MakeReportForAveragesPivotRunRR(cr, rr, tbtnConcurrency.Checked);

                    Dictionary<UserActionResult, Metrics> combinedUserActionResults = rr.GetPivotedUserActionResults();
                    foreach (UserActionResult uar in combinedUserActionResults.Keys)
                    {
                        if (_cancel) return;
                        if (tbtnUserAction.Checked)
                            MakeReportForAveragesPivotRunUAR(cr, rr, uar, combinedUserActionResults[uar]);

                        if (tbtnLogEntry.Checked)
                        {
                            Dictionary<LogEntryResult, Metrics> combinedLogEntryResults = rr.GetPivotedLogEntryResults(uar.UserAction);
                            foreach (LogEntryResult ler in combinedLogEntryResults.Keys)
                            {
                                if (_cancel) return;
                                MakeReportForAveragesPivotRunLER(cr, rr, ler, combinedLogEntryResults[ler]);
                            }
                        }
                    }
                }
            }
        }
        private void InitMakeReportForAveragesPivotRun()
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                if (_cancel) return;
                tbtnRun.Visible = true;

                clmDRLRun.Visible = true;
            }, null);
        }
        private void MakeReportForAveragesPivotRunRR(ConcurrencyResult cr, RunResult rr, bool concurrentUsersChecked)
        {
            object[] row = rr.DetailedLogEntryResultMetrics(concurrentUsersChecked ? string.Empty : cr.ConcurrentUsers.ToString(),
                 !tbtnUserAction.Checked && !tbtnLogEntry.Checked);

            //Show on gui
            AddToCache(row);
        }
        private void MakeReportForAveragesPivotRunUAR(ConcurrencyResult cr, RunResult rr, UserActionResult uar, Metrics metrics)
        {
            object[] row = null;
            if (tbtnLogEntry.Checked)
                row = new object[] { 
                                   string.Empty,
                                   string.Empty,
                                   (tbtnConcurrency.Checked || tbtnRun.Checked ? string.Empty : cr.ConcurrentUsers.ToString()),
                                   (tbtnRun.Checked ? string.Empty : (rr.Run + 1).ToString()),
                                   string.Empty,
                                   uar.UserAction,
                                   string.Empty,
                                   metrics.TotalLogEntriesProcessed,
                                   string.Empty,
                                   string.Empty,
                                   string.Empty,
                                   string.Empty,
                                   string.Empty,
                                   metrics.Errors
                                   };
            else
                row = new object[] { 
                                   string.Empty,
                                   string.Empty,
                                   (tbtnConcurrency.Checked || tbtnRun.Checked ? string.Empty : cr.ConcurrentUsers.ToString()),
                                   (tbtnRun.Checked ? string.Empty : (rr.Run + 1).ToString()),
                                   string.Empty,
                                   uar.UserAction,
                                   string.Empty,
                                   metrics.TotalLogEntriesProcessed,
                                   string.Empty,
                                   metrics.AverageTimeToLastByte.TotalMilliseconds,
                                   metrics.MaxTimeToLastByte.TotalMilliseconds,
                                   metrics.Percentile95MaxTimeToLastByte.TotalMilliseconds,
                                   metrics.AverageDelay.TotalMilliseconds,
                                   metrics.Errors
                                   };

            AddToCache(row);
        }
        private void MakeReportForAveragesPivotRunLER(ConcurrencyResult cr, RunResult rr, LogEntryResult ler, Metrics metrics)
        {
            if (!ler.Empty)
            {
                object[] row = { 
                           string.Empty,
                           string.Empty,
                           (tbtnConcurrency.Checked || tbtnRun.Checked || tbtnUserAction.Checked ? string.Empty : cr.ConcurrentUsers.ToString()),
                           (tbtnRun.Checked || tbtnUserAction.Checked ? string.Empty : (rr.Run + 1).ToString()),
                           string.Empty,
                           (tbtnUserAction.Checked ? string.Empty : ler.UserAction),
                           ler.LogEntryString.Replace("\n", VBLRn).Replace("\r", VBLRr),
                           metrics.TotalLogEntriesProcessed,
                           string.Empty,
                           metrics.AverageTimeToLastByte.TotalMilliseconds,
                           metrics.MaxTimeToLastByte.TotalMilliseconds,
                           metrics.Percentile95MaxTimeToLastByte.TotalMilliseconds,
                           metrics.AverageDelay.TotalMilliseconds,
                           metrics.Errors
                           };

                AddToCache(row);
            }
        }
        #endregion

        #region Users
        private void MakeReportForUsers()
        {
            InitMakeReportForUsers();
            if (StresstestResults == null || _cancel) return;

            foreach (ConcurrencyResult cr in StresstestResults.ConcurrencyResults)
            {
                if (_cancel) return;
                foreach (RunResult rr in cr.RunResults)
                {
                    if (_cancel) return;

                    foreach (UserResult userResult in rr.UserResults)
                    {
                        if (_cancel) return;
                        if (tbtnUser.Checked)
                            MakeReportForUsersUR(cr, rr, userResult);

                        if (tbtnUserAction.Checked)
                        {
                            foreach (UserActionResult userActionResult in userResult.UserActionResults.Values)
                            {
                                if (_cancel) return;
                                MakeReportForUsersUAR(cr, rr, userResult, userActionResult);

                                if (tbtnLogEntry.Checked)
                                {
                                    var logEntryResults = userActionResult.LogEntryResults;
                                    foreach (LogEntryResult logEntryResult in logEntryResults)
                                    {
                                        if (_cancel) return;
                                        MakeReportForUsersUARLER(cr, rr, userResult, userActionResult, logEntryResult);
                                    }
                                }
                            }
                        }
                        else if (tbtnLogEntry.Checked)
                        {
                            var logEntryResults = userResult.LogEntryResults;
                            foreach (LogEntryResult logEntryResult in logEntryResults)
                            {
                                if (_cancel) return;
                                MakeReportForUsersLER(cr, rr, userResult, logEntryResult);
                            }

                        }
                    }
                }
            }
        }
        private void InitMakeReportForUsers()
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                if (_cancel) return;

                clmDRLStartedAtSentAt.HeaderText = "Sent At";
                clmDRLStartedAtSentAt.Visible = true;
                clmDRLMeasuredTime.Visible = false;

                tbtnConcurrency.Visible = false;
                tbtnRun.Visible = false;
                tbtnUser.Visible = true;

                clmDRLConcurrency.Visible = true;
                clmDRLRun.Visible = true;

                clmDRLUser.Visible = true;
                clmDRLUserAction.Visible = true;
                clmDRLLogEntry.Visible = true;
                clmDRLLogEntriesProcessed.Visible = true;
                clmDRLThroughput.Visible = false;
                clmDRLResponseTime.Visible = true;
                clmDRLMaxResponseTime.Visible = false;
                clmDRLPercentileMaxResponseTime.Visible = false;
                clmDRLDelay.Visible = true;
                clmDRLErrors.Visible = true;

                pnlBorderPivotLevel.Visible = false;
                lblPivotLevel.Visible = false;
            }, null);
        }
        private void MakeReportForUsersUR(ConcurrencyResult cr, RunResult rr, UserResult ur)
        {
            TimeSpan averageTimeToLastByte, maxTimeToLastByte, totalDelay, averageDelay;
            ulong logEntriesProcessed, errors;
            double logEntriesProcessedPerTick;

            ur.GetLogEntryResultMetrics(out averageTimeToLastByte,
                out maxTimeToLastByte,
                out totalDelay,
                out averageDelay,
                out logEntriesProcessed,
                out logEntriesProcessedPerTick,
                out errors);

            object[] row = { 
                           string.Empty,
                           string.Empty,
                           cr.ConcurrentUsers,
                           (rr.Run + 1),
                           ur.User,
                           string.Empty,
                           string.Empty,
                           ur.LogEntriesProcessed,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           errors
                           };

            AddToCache(row);
        }
        private void MakeReportForUsersUAR(ConcurrencyResult cr, RunResult rr, UserResult ur, UserActionResult userActionResult)
        {
            userActionResult.RefreshMetrics();
            object[] row = null;
            if (tbtnUser.Checked)
                row = new object[] {
                                   userActionResult.SentAt.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                   string.Empty,
                                   string.Empty,
                                   string.Empty,
                                   string.Empty,
                                   string.Empty,
                                   userActionResult.UserAction,
                                   string.Empty,
                                   userActionResult.LogEntryResults.Count,
                                   string.Empty,
                                   (tbtnLogEntry.Checked ? string.Empty : userActionResult.TimeToLastByte.TotalMilliseconds.ToString()),
                                   string.Empty,
                                   string.Empty,
                                   (tbtnLogEntry.Checked ? string.Empty : userActionResult.DelayInMilliseconds.ToString()),
                                   userActionResult.Errors
                                   };
            else
                row = new object[] {
                                   userActionResult.SentAt.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                   string.Empty,
                                   cr.ConcurrentUsers,
                                   (rr.Run + 1),
                                   ur.User,
                                   userActionResult.UserAction,
                                   string.Empty,
                                   userActionResult.LogEntryResults.Count,
                                   string.Empty,
                                   (tbtnLogEntry.Checked ? string.Empty : userActionResult.TimeToLastByte.TotalMilliseconds.ToString()),
                                   string.Empty,
                                   string.Empty,
                                   (tbtnLogEntry.Checked ? string.Empty : userActionResult.DelayInMilliseconds.ToString()),
                                   userActionResult.Errors
                                   };
            AddToCache(row);
        }
        private void MakeReportForUsersUARLER(ConcurrencyResult cr, RunResult rr, UserResult ur, UserActionResult userActionResult, LogEntryResult logEntryResult)
        {
            if (!logEntryResult.Empty)
            {
                object[] row = { 
                           logEntryResult.SentAt.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           logEntryResult.LogEntryString.Replace("\n", VBLRn).Replace("\r", VBLRr),
                           1,
                           string.Empty,
                           logEntryResult.TimeToLastByte.TotalMilliseconds,
                           string.Empty,
                           string.Empty,
                           logEntryResult.DelayInMilliseconds,
                           ((logEntryResult.Exception == null) ? string.Empty : logEntryResult.Exception.ToString().Replace("\n", VBLRn).Replace("\r", VBLRr))
                           };

                AddToCache(row);
            }
        }
        private void MakeReportForUsersLER(ConcurrencyResult cr, RunResult rr, UserResult ur, LogEntryResult logEntryResult)
        {
            if (!logEntryResult.Empty)
            {
                object[] row = null;
                if (tbtnUser.Checked)
                    row = new object[] {
                                   logEntryResult.SentAt.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                   string.Empty,
                                   cr.ConcurrentUsers,
                                   (rr.Run + 1),
                                   ur.User,
                                   logEntryResult.UserAction,
                                   logEntryResult.LogEntryString.Replace("\n", VBLRn).Replace("\r", VBLRr),
                                   string.Empty,
                                   string.Empty,
                                   logEntryResult.TimeToLastByte.TotalMilliseconds,
                                   string.Empty,
                                   string.Empty,
                                   logEntryResult.DelayInMilliseconds,
                                   ((logEntryResult.Exception == null) ? string.Empty : logEntryResult.Exception.ToString().Replace("\n", VBLRn).Replace("\r", VBLRr))
                                   };
                else
                    row = new object[] {
                                   logEntryResult.SentAt.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                   string.Empty,
                                   cr.ConcurrentUsers,
                                   (rr.Run + 1),
                                   ur.User,
                                   logEntryResult.UserAction,
                                   logEntryResult.LogEntryString.Replace("\n", VBLRn).Replace("\r", VBLRr),
                                   string.Empty,
                                   string.Empty,
                                   logEntryResult.TimeToLastByte.TotalMilliseconds,
                                   string.Empty,
                                   string.Empty,
                                   logEntryResult.DelayInMilliseconds,
                                   ((logEntryResult.Exception == null) ? string.Empty : logEntryResult.Exception.ToString().Replace("\n", VBLRn).Replace("\r", VBLRr))
                                   };
                AddToCache(row);
            }
        }
        #endregion

        #region Errors
        private void MakeReportForErrors()
        {
            InitMakeReportForErrors();
            if (StresstestResults == null || _cancel) return;

            foreach (ConcurrencyResult cr in StresstestResults.ConcurrencyResults)
            {
                if (_cancel) return;
                if (!MakeReportForErrorsCU(cr)) continue;

                foreach (RunResult rr in cr.RunResults)
                {
                    if (_cancel) return;
                    if (tbtnRun.Checked)
                        if (!MakeReportForErrorsRR(cr, rr)) continue;

                    foreach (UserResult userResult in rr.UserResults)
                    {
                        if (_cancel) return;
                        if (tbtnUser.Checked)
                            if (!MakeReportForErrorsUR(cr, rr, userResult)) continue;

                        if (tbtnUserAction.Checked)
                        {
                            foreach (UserActionResult userActionResult in userResult.UserActionResults.Values)
                            {
                                if (_cancel) return;
                                if (!MakeReportForErrorsUAR(cr, rr, userResult, userActionResult)) continue;
                                if (tbtnLogEntry.Checked)
                                {
                                    var logEntryResults = userActionResult.LogEntryResults;
                                    foreach (LogEntryResult logEntryResult in logEntryResults)
                                    {
                                        if (_cancel) return;
                                        if (!MakeReportForErrorsUARLER(cr, rr, userResult, userActionResult, logEntryResult)) continue;
                                    }
                                }
                            }
                        }
                        else if (tbtnLogEntry.Checked)
                        {
                            var logEntryResults = userResult.LogEntryResults;
                            foreach (LogEntryResult logEntryResult in logEntryResults)
                            {
                                if (_cancel) return;
                                if (!MakeReportForErrorsLER(cr, rr, userResult, logEntryResult)) continue;
                            }
                        }
                    }
                }
            }
        }
        private void InitMakeReportForErrors()
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                if (_cancel) return;

                clmDRLStartedAtSentAt.HeaderText = "Started At";
                clmDRLStartedAtSentAt.Visible = true;
                clmDRLMeasuredTime.Visible = true;

                tbtnConcurrency.Visible = true;
                tbtnRun.Visible = true;
                tbtnUser.Visible = true;

                clmDRLConcurrency.Visible = true;
                clmDRLRun.Visible = true;

                clmDRLUser.Visible = true;
                clmDRLUserAction.Visible = true;
                clmDRLLogEntry.Visible = true;
                clmDRLLogEntriesProcessed.Visible = true;
                clmDRLThroughput.Visible = false;
                clmDRLResponseTime.Visible = false;
                clmDRLMaxResponseTime.Visible = false;
                clmDRLPercentileMaxResponseTime.Visible = false;
                clmDRLDelay.Visible = false;
                clmDRLErrors.Visible = true;

                pnlBorderPivotLevel.Visible = false;
                lblPivotLevel.Visible = false;
            }, null);
        }
        /// <summary>
        /// Returns true for errors
        /// </summary>
        /// <param name="cr"></param>
        /// <returns>true for errors</returns>
        private bool MakeReportForErrorsCU(ConcurrencyResult cr)
        {
            if (cr.Metrics.Errors == 0)
                return false;

            if (tbtnConcurrency.Checked)
            {
                object[] row = { 
                               string.Empty, 
                               string.Empty, 
                               cr.ConcurrentUsers,
                               string.Empty, 
                               string.Empty, 
                               string.Empty, 
                               string.Empty, 
                               string.Empty, 
                               string.Empty, 
                               string.Empty, 
                               string.Empty, 
                               string.Empty, 
                               string.Empty, 
                               string.Empty, 
                               cr.Metrics.Errors
                               };
                //Show on gui
                AddToCache(row);
            }
            return true;
        }
        /// <summary>
        /// Returns true for errors
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="pr"></param>
        /// <param name="rr"></param>
        /// <returns>true for errors</returns>
        private bool MakeReportForErrorsRR(ConcurrencyResult cr, RunResult rr)
        {
            if (rr.Metrics.Errors == 0)
                return false;

            object[] row = { 
                           string.Empty,
                           string.Empty,
                           (tbtnConcurrency.Checked ? string.Empty : cr.ConcurrentUsers.ToString()),
                           (rr.Run + 1),
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           rr.Metrics.Errors
                           };
            //Show on gui
            AddToCache(row);
            return true;
        }
        /// <summary>
        /// Returns true for errors
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="pr"></param>
        /// <param name="rr"></param>
        /// <param name="ur"></param>
        /// <param name="showOnGui"></param>
        /// <returns>true for errors</returns>
        private bool MakeReportForErrorsUR(ConcurrencyResult cr, RunResult rr, UserResult ur)
        {
            TimeSpan averageTimeToLastByte, maxTimeToLastByte, totalDelay, averageDelay;
            ulong logEntriesProcessed, errors;
            double logEntriesProcessedPerTick;

            ur.GetLogEntryResultMetrics(out averageTimeToLastByte,
                out maxTimeToLastByte,
                out totalDelay,
                out averageDelay,
                out logEntriesProcessed,
                out logEntriesProcessedPerTick,
                out errors);

            if (errors == 0)
                return false;

            object[] row = { 
                           string.Empty,
                           string.Empty,
                           ((tbtnConcurrency.Checked || tbtnRun.Checked) ? string.Empty : cr.ConcurrentUsers.ToString()),
                           (tbtnRun.Checked ? string.Empty : (rr.Run + 1).ToString()),
                           ur.User,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           errors
                           };

            //Show on gui
            AddToCache(row);
            return true;
        }
        /// <summary>
        /// Returns true on error
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="pr"></param>
        /// <param name="rr"></param>
        /// <param name="ur"></param>
        /// <param name="userActionResult"></param>
        /// <param name="showOnGui"></param>
        /// <returns>true on error</returns>
        private bool MakeReportForErrorsUAR(ConcurrencyResult cr, RunResult rr, UserResult ur, UserActionResult userActionResult)
        {
            userActionResult.RefreshMetrics();
            if (userActionResult.Errors == 0)
                return false;

            object[] row = { 
                           userActionResult.SentAt.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                           string.Empty,
                           ((tbtnConcurrency.Checked || tbtnRun.Checked || tbtnUser.Checked) ? string.Empty : cr.ConcurrentUsers.ToString()),
                           ((tbtnRun.Checked || tbtnUser.Checked) ? string.Empty : (rr.Run + 1).ToString()).ToString(),
                           (tbtnUser.Checked ? string.Empty : ur.User),
                           userActionResult.UserAction,
                           string.Empty,
                           userActionResult.LogEntryResults.Count + StresstestResults.UniqueResultsDelimiter(),
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           userActionResult.Errors
                           };
            //Show on gui
            AddToCache(row);
            return true;
        }
        /// <summary>
        /// Returns true on error
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="pr"></param>
        /// <param name="rr"></param>
        /// <param name="ur"></param>
        /// <param name="userActionResult"></param>
        /// <param name="showOnGui"></param>
        /// <returns>true on error</returns>
        private bool MakeReportForErrorsUARLER(ConcurrencyResult cr, RunResult rr, UserResult ur, UserActionResult userActionResult, LogEntryResult logEntryResult)
        {
            if (logEntryResult.Exception == null) return false;

            object[] row = { 
                           logEntryResult.SentAt.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                           string.Empty,
                           ((tbtnConcurrency.Checked || tbtnRun.Checked || tbtnUser.Checked || tbtnUserAction.Checked) ? string.Empty : cr.ConcurrentUsers.ToString()),
                           ((tbtnRun.Checked || tbtnUser.Checked || tbtnUserAction.Checked) ? string.Empty : (rr.Run + 1).ToString()),
                           ((tbtnUser.Checked || tbtnUserAction.Checked) ? string.Empty : ur.User),
                           (tbtnUserAction.Checked ? string.Empty : userActionResult.UserAction),
                           logEntryResult.LogEntryString.Replace("\n", VBLRn).Replace("\r", VBLRr),
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           logEntryResult.Exception == null ? string.Empty : logEntryResult.Exception.ToString().Replace("\n", VBLRn).Replace("\r", VBLRr)
                           };
            //Show on gui
            AddToCache(row);
            return true;
        }
        /// <summary>
        /// Returns true on error
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="pr"></param>
        /// <param name="rr"></param>
        /// <param name="ur"></param>
        /// <param name="logEntryResult"></param>
        /// <param name="showOnGui"></param>
        /// <returns>true on error</returns>
        private bool MakeReportForErrorsLER(ConcurrencyResult cr, RunResult rr, UserResult ur, LogEntryResult logEntryResult)
        {
            if (logEntryResult.Exception == null) return false;

            object[] row = { 
                           logEntryResult.SentAt.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                           string.Empty,
                           ((tbtnConcurrency.Checked || tbtnRun.Checked || tbtnUser.Checked) ? string.Empty : cr.ConcurrentUsers.ToString()),
                           ((tbtnRun.Checked || tbtnUser.Checked) ? string.Empty : (rr.Run + 1).ToString()),
                           (tbtnUser.Checked ? string.Empty : ur.User),
                           logEntryResult.UserAction,
                           logEntryResult.LogEntryString.Replace("\n", VBLRn).Replace("\r", VBLRr),
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           string.Empty,
                           ((logEntryResult.Exception == null) ? string.Empty : logEntryResult.Exception.ToString().Replace("\n", VBLRn).Replace("\r", VBLRr))
                           };
            //Show on gui
            AddToCache(row);
            return true;
        }
        #endregion

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            MakeReport();
        }
        #endregion

        public class ReportMadeEventArgs : EventArgs
        {
            public readonly Exception Exception;
            public bool Succes
            {
                get { return Exception == null; }
            }

            public ReportMadeEventArgs(Exception exception)
            {
                Exception = exception;
            }
        }
    }
}