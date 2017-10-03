/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using RandomUtils.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using vApus.Monitor.Sources.Base;
using vApus.Publish;
using vApus.Results;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Monitor {
    /// <summary>
    /// Communicates to a number of monitor sources provided using the vApusSMT binaries (See monitorsources in the vApus build folder).
    /// Hardware configs can be retrieved if any; various hardware devices can be monitored.
    /// </summary>
    public partial class MonitorView : BaseSolutionComponentView {
        public event EventHandler<MonitorInitializedEventArgs> MonitorInitialized;
        public event EventHandler<ErrorEventArgs> OnHandledException, OnUnhandledException;

        #region Fields
        private readonly MonitorProject _monitorProject;
        private readonly Monitor _monitor;
        private string _hardwareConfiguration;

        private int _refreshTimeInMS;

        private string _test;
        private string _previousFilter;
        private MonitorSourceClient _previousMonitorSourceForParameters;

        private System.Timers.Timer _invokeChangedTmr = new System.Timers.Timer(1000);

        private IClient _monitorSourceClient;

        private Entities _wdyh = null;
        private string _decimalSeparator;

        #endregion

        #region Properties
        public Monitor Monitor {
            get { return _monitor; }
        }

        public string HardwareConfiguration {
            get { return _hardwareConfiguration; }
            private set { _hardwareConfiguration = value; }
        }
        public bool IsRunning {
            get {
                bool isRunning = false;
                try {
                    SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                        isRunning = btnStop.Enabled;
                    }, null);
                }
                catch {
                    //Ignore.
                }
                return isRunning;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        ///     Designer time only
        /// </summary>
        public MonitorView() {
            InitializeComponent();
        }
        /// <summary>
        /// Communicates to a number of monitor sources provided using the vApusSMT binaries (See monitorsources in the vApus build folder).
        /// Hardware configs can be retrieved if any; various hardware devices can be monitored.
        /// </summary>
        /// <param name="solutionComponent"></param>
        /// <param name="args"></param>
        public MonitorView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            InitializeComponent();

            _monitor = solutionComponent as Monitor;
            _monitorProject = Solution.ActiveSolution.GetProject("MonitorProject") as MonitorProject;

            _invokeChangedTmr.Elapsed += _invokeChangedTmr_Elapsed;

            if (IsHandleCreated)
                InitMonitorView();
            else
                HandleCreated += MonitorView_HandleCreated;

            //Stupid workaround.
            monitorControl.ColumnHeadersDefaultCellStyle.Font = new Font(monitorControl.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
        }
        #endregion

        #region Functions

        #region Private
        #region Init

        private void MonitorView_HandleCreated(object sender, EventArgs e) {
            HandleCreated -= MonitorView_HandleCreated;
            InitMonitorView();
        }

        /// <summary>
        ///     Sets the Gui and connects to smt.
        /// </summary>
        private void InitMonitorView() {
            Text = SolutionComponent.ToString();
            if (SynchronizationContextWrapper.SynchronizationContext == null)
                SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;

            Exception exception = InitMonitorSourceClient();
            propertyPanel.SolutionComponent = _monitor;
            SetFilterTextBox();

            SetValuesToParameters();

            _previousMonitorSourceForParameters = _monitor.MonitorSource;
            _previousFilter = _monitor.Filter.Combine(", ");

            if (exception != null) {
                string message = "Could not connect to the monitor client.";
                Loggers.Log(Level.Error, message, exception);
            }

            //Use this for filtering the counters.
            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }

        /// <summary>
        ///     Destroys the previous one if any and returns a new one.
        /// </summary>
        /// <returns></returns>
        private Exception InitMonitorSourceClient() {
            Exception exception = null;

            try {
                if (_monitorSourceClient != null) {
                    try {
                        _monitorSourceClient.Stop();
                    }
                    catch (Exception exc) {
                        Loggers.Log(Level.Warning, "Failed stopping the monitor source client.", exc);
                    }
                    _monitorSourceClient.OnMonitor -= _monitorSourceClient_OnMonitor;
                    _monitorSourceClient.Dispose();
                    _monitorSourceClient = null;
                }

                _monitor.InitMonitorSourceClients();

                _monitorSourceClient = ClientFactory.Create(_monitor.MonitorSource.Type);
            }
            catch (Exception ex) {
                exception = ex;
            }

            return exception;
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender == _monitor && IsHandleCreated) {
                if (_monitor.MonitorSource != _previousMonitorSourceForParameters) {
                    _previousMonitorSourceForParameters = _monitor.MonitorSource;

                    InitMonitorSourceClient();

                    SetValuesToParameters();
                }
                if (_monitor.MonitorSourceIndex == _monitor.PreviousMonitorSourceIndexForCounters ||
                    lvwEntities.Items.Count == 0) {
                    split.Panel2.Enabled = btnSetDefaultWiw.Enabled = true;
                    lblMonitorSourceMismatch.Visible = false;

                    btnStart.Enabled = btnSchedule.Enabled = lvwEntities.Items.Count != 0 && _monitor.Wiw.GetSubs().Count != 0;
                }
                else {
                    split.Panel2.Enabled = btnSetDefaultWiw.Enabled = false;
                    lblMonitorSourceMismatch.Visible = true;

                    btnStart.Enabled = btnSchedule.Enabled = false;
                }
                //Filter the treenodes again if this is changed.
                string filter = _monitor.Filter.Combine(", ");
                if (filter != _previousFilter) {
                    _previousFilter = filter;
                    FillCounters();
                }
            }
        }
        #endregion

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        //private void _monitorProxy_OnHandledException(object sender, ErrorEventArgs e) {
        //    SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
        //        Loggers.Log(Level.Warning, Text + ": A counter became unavailable while monitoring.", e.GetException(), new object[] { sender, e });

        //        if (_forStressTest && OnHandledException != null) {
        //            var invocationList = OnHandledException.GetInvocationList();
        //            Parallel.For(0, invocationList.Length, (i) => {
        //                (invocationList[i] as EventHandler<ErrorEventArgs>).Invoke(this, e);
        //            });
        //        }
        //    }, null);
        //}

        //private void _monitorProxy_OnUnhandledException(object sender, ErrorEventArgs e) {
        //    SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
        //        bool forStressTest = _forStressTest;
        //        Stop();
        //        Loggers.Log(Level.Error, Text + ": An error has occured while monitoring, monitor stopped.", e.GetException(), new object[] { sender, e });

        //        if (forStressTest && OnUnhandledException != null) {
        //            var invocationList = OnHandledException.GetInvocationList();
        //            Parallel.For(0, invocationList.Length, (i) => {
        //                (invocationList[i] as EventHandler<ErrorEventArgs>).Invoke(this, e);
        //            });

        //        }
        //    }, null);
        //}

        private void _monitorSourceClient_OnMonitor(object sender, OnMonitorEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                try {
                    monitorControl.AddCounters(e.Counters, _decimalSeparator);

                    //Don't do this when stopped
                    if (tmrProgressDelayCountDown.Enabled) {
                        int refreshInS = _refreshTimeInMS / 1000;
                        lblCountDown.Tag = refreshInS;

                        lblCountDown.Text = "Updates in " + refreshInS;
                    }

                    PublishProgress();

                    btnSaveAllMonitorCounters.Enabled = monitorControl.ColumnCount != 0;
                    btnSaveFilteredMonitoredCounters.Enabled = monitorControl.ColumnCount != 0 &&
                                                               txtFilterMonitorControlColumns.Text.Length != 0;

                    var schedule = btnSchedule.Tag as ExtendedSchedule;
                    if (schedule != null && schedule.Duration.Ticks != 0) {
                        DateTime endsAt = schedule.ScheduledAt + schedule.Duration;
                        if (endsAt <= DateTime.Now)
                            Stop();
                    }
                }
                catch (Exception ex) {
                    Loggers.Log(Level.Error, "Monitor proxy on monitor failed.", ex, new object[] { _monitor.ToString() });
                }
            }, null);
        }

        private void btnGetCounters_Click(object sender, EventArgs e) {
            ConnectAndGetCounters();

            //Ugly tww sort workaround.
            tvwCounters.TreeViewNodeSorter = CountersReverseTreeNodeTextComparer.GetInstance(); //Sorting in tvws is strange.
            tvwCounters.TreeViewNodeSorter = CountersTreeNodeCheckedComparer.GetInstance();
        }

        async private void ConnectAndGetCounters() {
            Cursor = Cursors.WaitCursor;

            if (_monitor.PreviousMonitorSourceIndexForCounters != _monitor.MonitorSourceIndex) {
                _monitor.PreviousMonitorSourceIndexForCounters = _monitor.MonitorSourceIndex;
                //Clear this when a new is selected.
                _monitor.Wiw.GetSubs().Clear();
            }
            lblMonitorSourceMismatch.Visible = false;

            btnGetCounters.Enabled = false;
            propertyPanel.Lock();
            parameterPanel.Enabled = false;
            split.Panel2.Enabled = btnSetDefaultWiw.Enabled = false;

            _monitorProject.Locked = true;

            tvwCounters.Nodes.Clear();
            lvwEntities.Items.Clear();

            btnStart.Enabled = false;
            btnSchedule.Enabled = false;

            btnGetCounters.Text = "Getting counters...";

            await Task.Run(() => __WDYH());

            GroupChecked = chkGroupChecked.Checked;

            string errorMessage = null;
            if (split.Panel2.Enabled && lvwEntities.Items.Count != 0 && tvwCounters.Nodes.Count != 0) {

                if (_monitor.Wiw.GetDeepCount() == 0)
                    errorMessage = Text + ": No counters were chosen.";

            }
            else {
                errorMessage = Text + ": Entities and counters could not be retrieved!\nHave you filled in the right credentials?";
            }

            if (MonitorInitialized != null)
                MonitorInitialized(this, new MonitorInitializedEventArgs(errorMessage));
        }

        private void __WDYH() {
            InitMonitorSourceClient();

            string config = null;
            //Set the parameters and the values in the gui and in the proxy
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate { SetValuesToParameters(); }, null);

            bool isConnected = false;
            Exception exception = null;
            try {
                if (_monitorSourceClient.Connect()) {
                    _refreshTimeInMS = _monitorSourceClient.RefreshCountersInterval;

                    config = _monitorSourceClient.Config;
                    _decimalSeparator = _monitorSourceClient.DecimalSeparator;
                    _wdyh = _monitorSourceClient.WDYH;

                    if (!string.IsNullOrEmpty(config) && config.StartsWith("<lines>") && config.EndsWith("</lines>") && config.Contains("<line>") && config.Contains("</line>")) {
                        try {
                            var sb = new StringBuilder();
                            var doc = new XmlDocument();
                            doc.LoadXml(config);

                            foreach (XmlNode node in doc.FirstChild.ChildNodes)
                                sb.AppendLine(node.InnerText);

                            config = sb.ToString().Trim();
                        }
                        catch {
                            //No xml after all.
                        }
                    }
                    isConnected = true;
                }
                else {
                    exception = new Exception("Failed to connect to " + _monitorSourceClient.Name + ".");
                }
            }
            catch (Exception ex) {
                exception = new Exception("Failed to connect to " + _monitorSourceClient.Name + ".", ex);
            }

            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                if (isConnected) {
                    btnConfiguration.Enabled = !string.IsNullOrEmpty(config);
                    HardwareConfiguration = config;
                    try { FillEntities(_wdyh); } catch (Exception ex) { exception = ex; }
                }

                btnGetCounters.Text = "Get counters";

                if (exception != null) {
                    btnStart.Enabled = btnSchedule.Enabled = false;

                    string message = "Entities and counters could not be retrieved!\nHave you filled in the right credentials?";
                    Loggers.Log(Level.Error, message, exception);

                    if (_test == null) MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    btnSetDefaultWiw.Enabled = false;
                }
                else {
                    btnSetDefaultWiw.Enabled = true;
                }
                split.Panel2.Enabled = btnGetCounters.Enabled = true;
                propertyPanel.Unlock();
                parameterPanel.Enabled = true;

                _monitorProject.Locked = false;

                Cursor = Cursors.Default;
            }, null);
        }

        private void SetValuesToParameters() {
            //Get parameter values and set the parameters
            for (int i = 0; i != _monitorSourceClient.Parameters.Length; i++) {
                if (i >= _monitor.ParameterValues.Length) break;

                Parameter parameter = _monitorSourceClient.Parameters[i];

                object candidate = _monitor.ParameterValues[i];
                if (candidate.GetType() == parameter.DefaultValue.GetType())
                    parameter.Value = candidate;
            }

            parameterPanel.Parameters = _monitorSourceClient.Parameters;

            lblMonitorSourceParameters.Visible = _monitorSourceClient.Parameters.Length != 0;
        }

        private void parameterPanel_ParameterValueChanged(object sender, EventArgs e) {
            StoreParameterValues();
        }

        /// <summary>
        ///     Store from _parametersWithValues to the monitor object
        /// </summary>
        private void StoreParameterValues() {
            var parameterValues = new object[_monitorSourceClient.Parameters.Length];
            int i = 0;
            foreach (Parameter parameter in _monitorSourceClient.Parameters)
                parameterValues[i++] = parameter.Value;
            _monitor.ParameterValues = parameterValues;

            InvokeChanged();
        }

        private void FillEntities(Entities entitiesAndCounters) {
            foreach (Entity entity in entitiesAndCounters.GetSubs()) {
                var lvwi = new ListViewItem(string.Empty);

                lvwi.SubItems.Add(entity.GetName());
                lvwi.SubItems.Add("[0]");
                lvwi.ImageIndex = entity.IsAvailable() ? 0 : 1;
                lvwi.StateImageIndex = lvwi.ImageIndex;
                lvwi.Tag = entity;
                lvwi.Checked = false;

                lvwEntities.Items.Add(lvwi);
            }
            split.Panel2.Enabled = btnSetDefaultWiw.Enabled = lvwEntities.Items.Count != 0;

            if (lvwEntities.Items.Count != 0)
                lvwEntities.Items[0].Selected = true;
        }

        private void lvwEntities_SelectedIndexChanged(object sender, EventArgs e) {
            if (lvwEntities.SelectedItems.Count != 0) {
                lvwEntities.Tag = lvwEntities.SelectedItems[0];
                FillCounters();
            }
        }

        private void lvwEntities_ItemChecked(object sender, ItemCheckedEventArgs e) {
            bool itemChecked = e.Item.Checked;
            e.Item.Selected = true;

            //Push wiw clears this otherwise.
            lvwEntities.ItemChecked -= lvwEntities_ItemChecked;
            e.Item.Checked = itemChecked;
            lvwEntities.ItemChecked += lvwEntities_ItemChecked;

            ExtractWIWForListViewAction();

            llblUncheckAllVisible.Enabled = HasCheckedNodes();
            llblCheckAllVisible.Enabled = HasUncheckedNodes();
        }

        private void ExtractWIWForListViewAction() {
            LockWindowUpdate(Handle);
            try {
                tvwCounters.AfterCheck -= tvwCounter_AfterCheck;
                var selected = lvwEntities.Tag as ListViewItem;

                ParseTag(selected);
                var nodes = selected.Tag as TreeNode[];
                bool selectedChecked = selected.Checked;

                foreach (TreeNode counterNode in nodes) {
                    counterNode.Checked = selectedChecked;
                    foreach (TreeNode node in counterNode.Nodes)
                        node.Checked = selectedChecked;

                    if (counterNode.Tag != null) {
                        var counterNodes = counterNode.Tag as TreeNode[];
                        foreach (TreeNode node in counterNodes)
                            node.Checked = selectedChecked;
                    }

                    if (selectedChecked)
                        ApplyToWIW(counterNode);
                }
                if (!selectedChecked) {
                    Entity entity = _monitor.Wiw.GetEntity(selected.SubItems[1].Text);
                    _monitor.Wiw.GetSubs().Remove(entity);
                }
                tvwCounters.AfterCheck += tvwCounter_AfterCheck;

                SetChosenCountersInListViewItems();
                btnStart.Enabled = btnSchedule.Enabled = lvwEntities.Items.Count != 0 && _monitor.Wiw.GetSubs().Count != 0;

                InvokeChanged();
            }
            catch {
                throw;
            }
            finally {
                LockWindowUpdate(IntPtr.Zero);
            }
        }

        private void SetChosenCountersInListViewItems() {
            var applyCountTo = new List<ListViewItem>(lvwEntities.Items.Count);
            foreach (ListViewItem lvwi in lvwEntities.Items)
                applyCountTo.Add(lvwi);

            foreach (Entity entity in _monitor.Wiw.GetSubs()) {
                List<CounterInfo> l = entity.GetSubs();
                int count = GetTotalCountOfCounters(l);
                foreach (ListViewItem lvwi in lvwEntities.Items)
                    if (lvwi.SubItems[1].Text == entity.GetName()) {
                        lvwi.SubItems[2].Text = "[" + count + "]";
                        applyCountTo.Remove(lvwi);
                        break;
                    }
            }
            //For the ones that aren't in Wiw.
            foreach (ListViewItem lvwi in applyCountTo)
                lvwi.SubItems[2].Text = "[0]";
        }

        private int GetTotalCountOfCounters(List<CounterInfo> list) {
            int count = 0;
            foreach (CounterInfo info in list) {
                int c = info.GetSubs().Count;
                if (c == 0) c = 1;
                count += c;
            }
            return count;
        }

        private void llblCheckAllVisible_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            LockWindowUpdate(Handle);
            try {
                tvwCounters.AfterCheck -= tvwCounter_AfterCheck;

                foreach (TreeNode counterNode in tvwCounters.Nodes) {
                    counterNode.Checked = true;
                    foreach (TreeNode node in counterNode.Nodes) node.Checked = true;

                    if (counterNode.Tag != null) {
                        var counterNodes = counterNode.Tag as TreeNode[];
                        foreach (TreeNode node in counterNodes) node.Checked = true;
                    }
                    ApplyToWIW(counterNode);
                }

                tvwCounters.AfterCheck += tvwCounter_AfterCheck;
                SetChosenCountersInListViewItems();

                btnStart.Enabled = btnSchedule.Enabled = lvwEntities.Items.Count != 0 && _monitor.Wiw.GetSubs().Count != 0;

                llblUncheckAllVisible.Enabled = HasCheckedNodes();
                llblCheckAllVisible.Enabled = HasUncheckedNodes();
            }
            catch { throw; }
            finally { LockWindowUpdate(IntPtr.Zero); }
        }

        private void llblUncheckAllVisible_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            LockWindowUpdate(Handle);
            try {
                tvwCounters.AfterCheck -= tvwCounter_AfterCheck;

                foreach (TreeNode counterNode in tvwCounters.Nodes) {
                    counterNode.Checked = false;
                    foreach (TreeNode node in counterNode.Nodes)
                        node.Checked = false;

                    if (counterNode.Tag != null) {
                        var counterNodes = counterNode.Tag as TreeNode[];
                        foreach (TreeNode node in counterNodes)
                            node.Checked = false;
                    }

                    ApplyToWIW(counterNode);
                }

                tvwCounters.AfterCheck += tvwCounter_AfterCheck;
                SetChosenCountersInListViewItems();

                btnStart.Enabled = btnSchedule.Enabled = lvwEntities.Items.Count != 0 && _monitor.Wiw.GetSubs().Count != 0;

                llblUncheckAllVisible.Enabled = HasCheckedNodes();
                llblCheckAllVisible.Enabled = HasUncheckedNodes();
            }
            catch {
                throw;
            }
            finally {
                LockWindowUpdate(IntPtr.Zero);
            }
        }

        private void tvwCounter_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
            AddNodesFromTag(e.Node);
        }

        private void AddNodesFromTag(TreeNode node) {
            if (node.Tag != null) {
                LockWindowUpdate(Handle);
                try {
                    node.Nodes.AddRange(node.Tag as TreeNode[]);
                    node.Tag = null;

                    tvwCounters.TreeViewNodeSorter = CountersReverseTreeNodeTextComparer.GetInstance(); //Sorting in tvws is strange.

                    if (GroupChecked)
                        tvwCounters.TreeViewNodeSorter = CountersTreeNodeCheckedComparer.GetInstance();
                    else
                        tvwCounters.TreeViewNodeSorter = CountersTreeNodeTextComparer.GetInstance();

                    node.Expand();
                }
                catch {
                    throw;
                }
                finally {
                    LockWindowUpdate(IntPtr.Zero);
                }
            }
        }

        private void tvwCounter_AfterCheck(object sender, TreeViewEventArgs e) {
            ExtractWIWForTreeViewAction(e.Node);
            llblUncheckAllVisible.Enabled = HasCheckedNodes();
            llblCheckAllVisible.Enabled = HasUncheckedNodes();

            if (chkGroupChecked.Checked)
                GroupChecked = true;
        }

        private void ExtractWIWForTreeViewAction(TreeNode counterNode) {
            LockWindowUpdate(Handle);

            tvwCounters.AfterCheck -= tvwCounter_AfterCheck;
            if (counterNode.Level == 0) {
                foreach (TreeNode node in counterNode.Nodes)
                    node.Checked = counterNode.Checked;

                if (counterNode.Tag != null) {
                    var counterNodes = counterNode.Tag as TreeNode[];
                    foreach (TreeNode node in counterNodes)
                        node.Checked = counterNode.Checked;
                }
                ApplyToWIW(counterNode);
            }
            else {
                counterNode.Parent.Checked = false;
                foreach (TreeNode node in counterNode.Parent.Nodes)
                    if (node.Checked) {
                        counterNode.Parent.Checked = true;
                        break;
                    }
                ApplyToWIW(counterNode.Parent);
            }
            tvwCounters.AfterCheck += tvwCounter_AfterCheck;

            SetChosenCountersInListViewItems();

            btnStart.Enabled = btnSchedule.Enabled = lvwEntities.Items.Count != 0 && _monitor.Wiw.GetSubs().Count != 0;

            LockWindowUpdate(IntPtr.Zero);

            InvokeChanged();
        }
        private void InvokeChanged() {
            if (_invokeChangedTmr != null) {
                _invokeChangedTmr.Stop();
                _invokeChangedTmr.Start();
            }
        }
        private void _invokeChangedTmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if (_invokeChangedTmr != null) {
                _invokeChangedTmr.Stop();
                _monitor.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="counterNode">Add to or remove this from WiW depending on the checkstate of the node and its children</param>
        private void ApplyToWIW(TreeNode counterNode) {
            lvwEntities.ItemChecked -= lvwEntities_ItemChecked;

            var lvwiEntity = lvwEntities.Tag as ListViewItem;
            string entityName = lvwiEntity.SubItems[1].Text;
            Entity entity = _monitor.Wiw.GetEntity(entityName);
            if (entity == null) {
                entity = new Entity(entityName, true);
                _monitor.Wiw.GetSubs().Add(entity);
            }

            lvwiEntity.Checked = false;
            var nodes = lvwiEntity.Tag as TreeNode[];
            foreach (TreeNode node in nodes)
                if (node.Checked) {
                    lvwiEntity.Checked = true;
                    break;
                }

            if (lvwiEntity.Checked) {
                if (_monitor.Wiw.GetSubs().Contains(entity)) {
                    foreach (CounterInfo info in entity.GetSubs())
                        if (info.GetName() == counterNode.Text) {
                            entity.GetSubs().Remove(info);
                            break;
                        }
                    if (counterNode.Checked) {
                        var newCounterInfo = new CounterInfo(counterNode.Text);

                        foreach (TreeNode node in counterNode.Nodes)
                            if (node.Checked)
                                newCounterInfo.GetSubs().Add(new CounterInfo(node.Text));

                        if (counterNode.Tag != null) {
                            var counterNodes = counterNode.Tag as TreeNode[];
                            foreach (TreeNode node in counterNodes)
                                if (node.Checked)
                                    newCounterInfo.GetSubs().Add(new CounterInfo(node.Text));
                        }

                        entity.GetSubs().Add(newCounterInfo);
                    }
                }
                else {
                    var newCounterInfo = new CounterInfo(counterNode.Text);

                    foreach (TreeNode node in counterNode.Nodes)
                        if (node.Checked)
                            newCounterInfo.GetSubs().Add(new CounterInfo(node.Text));

                    if (counterNode.Tag != null) {
                        var counterNodes = counterNode.Tag as TreeNode[];
                        foreach (TreeNode node in counterNodes)
                            if (node.Checked)
                                newCounterInfo.GetSubs().Add(new CounterInfo(node.Text));
                    }
                    entity.GetSubs().Add(newCounterInfo);

                    //Random powerstate, doesn't matter
                    entity = new Entity(entityName, true);
                    _monitor.Wiw.GetSubs().Add(entity);
                }
            }
            else {
                _monitor.Wiw.GetSubs().Remove(entity);
            }

            _monitor.Wiw.GetSubs().Sort(EntityComparer.GetInstance());
            foreach (var e in _monitor.Wiw.GetSubs()) {
                e.GetSubs().Sort(CounterInfoComparer.GetInstance());

                foreach (var ci in e.GetSubs()) ci.GetSubs().Sort(CounterInfoComparer.GetInstance());
            }

            lvwEntities.ItemChecked += lvwEntities_ItemChecked;
        }

        private void btnConfiguration_Click(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(HardwareConfiguration))
                (new HardwareConfigurationDialog(HardwareConfiguration)).ShowDialog();
        }

        private void btnStart_Click(object sender, EventArgs e) {
            if (monitorControl.RowCount == 0 ||
                (monitorControl.RowCount > 0 &&
                 MessageBox.Show(
                     "Are you sure you want to start a new monitor?\nThis will clear the previous measured performance counters.",
                     string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) ==
                 DialogResult.Yes)) {
                _test = null;
                Start();
            }
        }

        private void btnSchedule_Click(object sender, EventArgs e) {
            var schedule = btnSchedule.Tag as ExtendedSchedule;
            if (schedule == null)
                schedule = new ExtendedSchedule();

            if (schedule.ShowDialog() == DialogResult.OK) {
                if (schedule.ScheduledAt > DateTime.Now) {
                    btnSchedule.Text = "Scheduled at " + schedule.ScheduledAt;
                    btnSchedule.Text += GetEndsAtFormatted(schedule.ScheduledAt, schedule.Duration);
                    btnSchedule.Tag = schedule;
                }
                else {
                    btnSchedule.Text = "Not scheduled" + GetEndsAtFormatted(schedule.ScheduledAt, schedule.Duration);
                    btnSchedule.Tag = schedule;
                }

                btnStart_Click(this, null);
            }
        }

        private string GetEndsAtFormatted(DateTime scheduledAt, TimeSpan duration) {
            if (duration.Ticks == 0)
                return string.Empty;

            if (duration.Milliseconds != 0) {
                duration = new TimeSpan(duration.Ticks - (duration.Ticks % TimeSpan.TicksPerSecond));
                duration += new TimeSpan(0, 0, 1);
            }
            return "; Ends At " + (scheduledAt + duration);
        }

        private void tmrProgressDelayCountDown_Tick(object sender, EventArgs e) {
            //Avoid a label update when after pushed stop
            if (tmrProgressDelayCountDown.Enabled) {
                int countDown = (int)lblCountDown.Tag - 1;
                if (countDown > 0) {
                    lblCountDown.Text = "Updates in " + countDown;
                    lblCountDown.Tag = countDown;
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e) {
            Stop();
        }

        private void btnSaveAllMonitorCounters_Click(object sender, EventArgs e) {
            if (monitorControl.MonitorResultCache.Headers != null && monitorControl.MonitorResultCache.Headers.Length != 0)
                if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                    Cursor = Cursors.WaitCursor;
                    monitorControl.Save(saveFileDialog.FileName);
                    Cursor = Cursors.Default;
                }
        }

        private void btnSaveFilteredMonitoredCounters_Click(object sender, EventArgs e) {
            if (monitorControl.MonitorResultCache.Headers != null && monitorControl.MonitorResultCache.Headers.Length != 0)
                if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                    Cursor = Cursors.WaitCursor;
                    monitorControl.SaveFiltered(saveFileDialog.FileName);
                    Cursor = Cursors.Default;
                }
        }

        private void txtFilterMonitorControlColumns_TextChanged(object sender, EventArgs e) {
            try {
                btnSaveFilteredMonitoredCounters.Enabled = monitorControl.ColumnCount != 0 &&
                                                           txtFilterMonitorControlColumns.Text.Length != 0;
            }
            catch {
                //Ignore. Only happens on gui disposed.
            }
        }

        private void txtFilterMonitorControlColumns_Leave(object sender, EventArgs e) {
            SetColumnFilter();
        }

        private void txtFilterMonitorControlColumns_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Return)
                SetColumnFilter();
        }

        private void picFilterMonitorControlColumns_Click(object sender, EventArgs e) {
            SetColumnFilter();
        }

        private void SetColumnFilter() {
            string[] filter = txtFilterMonitorControlColumns.Text.Split(new[] { ',' },
                                                                        StringSplitOptions.RemoveEmptyEntries);
            var l = new List<string>();
            for (int i = 0; i != filter.Length; i++) {
                string entry = filter[i].Trim();
                if (entry.Length != 0 && !l.Contains(entry))
                    l.Add(entry);
            }

            filter = l.ToArray();
            monitorControl.Filter(filter);
            txtFilterMonitorControlColumns.Text = filter.Combine(", ");
        }

        #region Fill & Filter tvwCounters, push the saved WIW tot the gui

        /// <summary>
        /// </summary>
        /// <param name="lvw">The selected item from this list view is used</param>
        /// <param name="tvw">Counters are put in this treeview</param>
        private void FillCounters() {
            if (lvwEntities.SelectedItems.Count != 0) {
                lvwEntities.ItemChecked -= lvwEntities_ItemChecked;
                tvwCounters.AfterCheck -= tvwCounter_AfterCheck;
                tvwCounters.Nodes.Clear();

                ListViewItem selected = lvwEntities.SelectedItems[0];
                ParseTag(selected);

                tvwCounters.Nodes.AddRange(FilterCounters(_monitor.Filter, selected.Tag as TreeNode[]));
                tvwCounters.AfterCheck += tvwCounter_AfterCheck;
                lvwEntities.ItemChecked += lvwEntities_ItemChecked;

                PushSavedWiW();

                llblUncheckAllVisible.Enabled = HasCheckedNodes();
                llblCheckAllVisible.Enabled = HasUncheckedNodes();

                GroupChecked = chkGroupChecked.Checked;
            }
        }

        private bool HasCheckedNodes() {
            foreach (TreeNode node in tvwCounters.Nodes)
                if (node.Checked)
                    return true;
            return false;
        }

        private bool HasUncheckedNodes() {
            foreach (TreeNode node in tvwCounters.Nodes)
                if (!node.Checked)
                    return true;
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="filter">if the length == 0 the original 'nodes' is returned</param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private TreeNode[] FilterCounters(string[] filter, TreeNode[] nodes) {
            if (filter.Length == 0)
                return nodes;
            var filtered = new List<TreeNode>();
            foreach (string s in filter)
                foreach (TreeNode node in Find(s, nodes))
                    if (!filtered.Contains(node))
                        filtered.Add(node);
            return filtered.ToArray();
        }

        private IEnumerable<TreeNode> Find(string text, TreeNode[] nodes) {
            text = Regex.Escape(text);
            text = text.Replace("\\*", ".*");
            text = "\\b" + text + "\\b";
            RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase;

            foreach (TreeNode node in nodes)
                if (Regex.IsMatch(node.Text, text, options))
                    yield return node;
        }

        /// <summary>
        ///     Parse a List<CounterInfo> tag to a TreeNode[] tag search in the given counter tvw if it does not exists.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="tvw">searches for existing nodes in this tvw</param>
        private void ParseTag(ListViewItem item) {
            //Make a gui for the data store in the tag (this tag will be switched with the gui).
            if (item.Tag is Entity) {
                var counterInfos = (item.Tag as Entity).GetSubs();
                var newTag = new TreeNode[counterInfos.Count];
                for (int i = 0; i != counterInfos.Count; i++) {
                    CounterInfo counterInfo = counterInfos[i];
                    string counter = counterInfo.GetName();
                    TreeNode counterNode = null;

                    //Search from end (faster) if the counter node already exists.
                    //The number of instances is important, it is possible that not each instance has the same entities for a counter.
                    for (int j = tvwCounters.Nodes.Count - 1; j != -1; j--) {
                        TreeNode node = tvwCounters.Nodes[j];
                        if (node.Text == counter && node.Nodes.Count == counterInfo.GetSubs().Count) {
                            counterNode = node;
                            break;
                        }
                    }

                    if (counterNode == null)
                        counterNode = new TreeNode(counter);
                    newTag[i] = counterNode;

                    if (counterInfo.GetSubs().Count != 0) {
                        //Only add the first, the rest will be added when the node expands. (tvwCounter.BeforeExpand)
                        counterNode.Nodes.Add(counterInfo.GetSubs()[0].GetName());

                        //Keep the rest in the tag.
                        if (counterInfo.GetSubs().Count != 1) {
                            var otherInstances = new TreeNode[counterInfo.GetSubs().Count - 1];
                            Parallel.For(1, counterInfo.GetSubs().Count,
                                         delegate (int k) { otherInstances[k - 1] = new TreeNode(counterInfo.GetSubs()[k].GetName()); });

                            counterNode.Tag = otherInstances;
                        }
                    }
                }
                item.Tag = newTag;
            }
        }

        private void PushSavedWiW() {
            lvwEntities.ItemChecked -= lvwEntities_ItemChecked;
            tvwCounters.AfterCheck -= tvwCounter_AfterCheck;

            //Autoscroll to the first selected/checked counter.
            TreeNode firstVisible = null;

            //Default WIW when needed and if available.
            //if (_monitor.Wiw.Count == 0) {
            //    DefaultWIWs.Set(_monitor, _wdyh);
            //} 
            //else { //Funky functionality, maybe implemented later on.
            //    //Correct entity names, if needed 
            //    for (int entityIndex = 0; entityIndex != _monitor.Wiw.Count; entityIndex++) {
            //        if (entityIndex >= _wdyh.Count)
            //            break;

            //        var wiwEntity = _monitor.Wiw[entityIndex];
            //        var wihEntity = _wdyh[entityIndex];
            //        wiwEntity.name = wihEntity.GetName();
            //        wiwEntity.isAvailable = wihEntity.IsAvailable();
            //    }
            //}

            //Make a new wiw to ensure that only valid counters remain in WiW (different machines can have different counters)
            var newWIW = new Entities();
            foreach (ListViewItem lvwi in lvwEntities.Items) {
                string entityName = lvwi.SubItems[1].Text;
                Entity entity = _monitor.Wiw.GetEntity(entityName);
                lvwi.Checked = entity != null;

                Entity newEntity = null;
                if (lvwi.Checked) {
                    ParseTag(lvwi);
                    newEntity = new Entity(entity.GetName(), entity.IsAvailable());
                    newWIW.GetSubs().Add(newEntity);
                }

                var nodes = lvwi.Tag as TreeNode[];
                if (nodes != null)
                    if (lvwi.Checked) {
                        List<CounterInfo> l = entity.GetSubs();
                        foreach (TreeNode node in nodes) {
                            CounterInfo info = GetCounterInfo(node.Text, l);
                            CounterInfo newInfo = null;
                            node.Checked = info != null;

                            if (node.Checked) {
                                if (firstVisible == null) firstVisible = node;

                                newInfo = new CounterInfo(info.GetName(), node.Nodes.Count == 0 ? null : new List<string>());

                                foreach (TreeNode child in node.Nodes) {
                                    CounterInfo instanceCandidate = GetCounterInfo(child.Text, info.GetSubs());
                                    child.Checked = instanceCandidate != null;
                                    if (child.Checked)
                                        newInfo.GetSubs().Add(new CounterInfo(instanceCandidate.GetName()));
                                }

                                var childNodes = node.Tag as TreeNode[];
                                if (childNodes != null) { //Only if the node was not expanded.
                                    foreach (TreeNode child in childNodes) {
                                        CounterInfo instanceCandidate = GetCounterInfo(child.Text, info.GetSubs());
                                        child.Checked = instanceCandidate != null;
                                        if (child.Checked)
                                            newInfo.GetSubs().Add(new CounterInfo(instanceCandidate.GetName()));
                                    }
                                }

                                newEntity.GetSubs().Add(newInfo);

                            }
                            else {
                                foreach (TreeNode child in node.Nodes)
                                    child.Checked = false;
                            }
                        }
                    }
                    else {
                        foreach (TreeNode node in nodes) {
                            node.Checked = false;
                            foreach (TreeNode child in node.Nodes)
                                child.Checked = false;

                            var childNodes = node.Tag as TreeNode[];
                            if (childNodes != null)
                                foreach (TreeNode child in childNodes)
                                    child.Checked = false;
                        }
                    }
            }

            //Set the new wiw
            _monitor.Wiw = newWIW;

            SetChosenCountersInListViewItems();

            btnStart.Enabled =
                btnSchedule.Enabled =
                (_monitor.MonitorSourceIndex == _monitor.PreviousMonitorSourceIndexForCounters ||
                 lvwEntities.Items.Count == 0)
                    ? _monitor.Wiw.GetSubs().Count != 0
                    : false;

            lvwEntities.ItemChecked += lvwEntities_ItemChecked;
            tvwCounters.AfterCheck += tvwCounter_AfterCheck;

            //Scroll the first checked into view if any.
            if (tvwCounters.Nodes.Count != 0) {
                if (firstVisible == null)
                    firstVisible = tvwCounters.Nodes[0];
                else
                    //Scroll down first so firstVisible will be at the top of the list.
                    tvwCounters.Nodes[tvwCounters.Nodes.Count - 1].EnsureVisible();
                firstVisible.EnsureVisible();
            }

            //Select the first visible counter, stupid but only way to do this.
            var tmr = new System.Timers.Timer(200);
            tmr.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => {
                tmr.Stop();
                SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                    tvwCounters.Focus();
                    if (firstVisible != null) tvwCounters.SelectedNode = firstVisible;
                }, null);
            };
            tmr.Start();
        }
        private void btnSetDefaultWiw_Click(object sender, EventArgs e) {
            try {
                _monitor.Wiw.GetSubs().Clear();
                DefaultWIWs.Set(_monitor, _wdyh);
                PushSavedWiW();
                if (GroupChecked)
                    GroupChecked = true;
            }
            catch {
                //UI error can enable this button too soon. Clicking this when loading counters will result in a crash.
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterName"></param>
        /// <param name="l"></param>
        /// <returns>Null if not found</returns>
        private CounterInfo GetCounterInfo(string counterName, List<CounterInfo> l) {
            foreach (CounterInfo info in l)
                if (info.GetName() == counterName)
                    return info;
            return null;
        }

        private void txtFilter_TextChanged(object sender, EventArgs e) { txtFilter.BackColor = (txtFilter.Text.Length == 0) ? SystemColors.Window : Color.LightBlue; }
        private void txtFilter_Leave(object sender, EventArgs e) { SetFilter(); }
        private void txtFilter_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Return)
                SetFilter();
        }
        private void picFilter_Click(object sender, EventArgs e) { SetFilter(); }
        private void SetFilter() {
            string[] filter = txtFilter.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var l = new List<string>();
            for (int i = 0; i != filter.Length; i++) {
                string entry = filter[i].Trim();
                if (entry.Length != 0 && !l.Contains(entry))
                    l.Add(entry);
            }

            _monitor.Filter = l.ToArray();

            SetFilterTextBox();
            InvokeChanged();
        }
        private void SetFilterTextBox() {
            try {
                txtFilter.Text = _monitor.Filter.Combine(", ");
                llblUncheckAllVisible.Enabled = HasCheckedNodes();
                llblCheckAllVisible.Enabled = HasUncheckedNodes();
            }
            catch {
                //Ignore. Only on gui disposed.
            }
        }

        private void chkGroupChecked_CheckedChanged(object sender, EventArgs e) { GroupChecked = chkGroupChecked.Checked; }
        private bool GroupChecked {
            get { return chkGroupChecked.Checked; }
            set {
                chkGroupChecked.CheckedChanged -= chkGroupChecked_CheckedChanged;

                chkGroupChecked.Checked = value;
                if (chkGroupChecked.Checked) {
                    tvwCounters.TreeViewNodeSorter = CountersReverseTreeNodeTextComparer.GetInstance(); //Sorting in tvws is strange.
                    tvwCounters.TreeViewNodeSorter = CountersTreeNodeCheckedComparer.GetInstance();
                }
                else {
                    tvwCounters.TreeViewNodeSorter = CountersTreeNodeTextComparer.GetInstance();

                    foreach (TreeNode node in tvwCounters.Nodes)
                        if (node.Checked) {
                            node.EnsureVisible();
                            break;
                        }
                }

                chkGroupChecked.CheckedChanged += chkGroupChecked_CheckedChanged;
            }
        }
        #endregion

        #region Publish
        private string _resultSetId;
        private bool CanPublish() { return Publisher.Settings.PublisherEnabled && _monitorSourceClient != null; }

        private void PublishConfiguration() {
            if (CanPublish()) {
                var publishItem = new MonitorConfiguration();
                publishItem.Test = _test ?? string.Empty;
                publishItem.Monitor = Monitor.ToString();
                publishItem.MonitorSource = _monitor.MonitorSourceName;

                var parameters = new List<KeyValuePair<string, string>>();
                foreach (Parameter parameter in _monitorSourceClient.Parameters)
                    if (parameter.Name.ToLowerInvariant() != "password")
                        parameters.Add(new KeyValuePair<string, string>(parameter.Name, parameter.Value.ToString()));

                publishItem.Parameters = parameters.ToArray();
                publishItem.HardwareConfiguration = _hardwareConfiguration;

                //Do not generate if a parent test generated one already.
                _resultSetId = _test == null ? Publisher.GenerateResultSetId() : Publisher.LastGeneratedResultSetId;
                Publisher.Send(publishItem, _resultSetId);
            }
        }

        private void PublishProgress() {
            if (CanPublish()) {
                MonitorResult monitorResult = GetMonitorResultCache();

                if (monitorResult.Rows.Count != 0) {
                    var publishItem = new Publish.MonitorMetrics();
                    publishItem.Monitor = _monitor.ToString();

                    publishItem.Headers = new string[monitorResult.Headers.Length - 1];
                    Array.Copy(monitorResult.Headers, 1, publishItem.Headers, 0, publishItem.Headers.Length);

                    object[] candidate = monitorResult.Rows[monitorResult.Rows.Count - 1];

                    var row = new object[candidate.Length - 1];
                    Array.Copy(candidate, 1, row, 0, row.Length);

                    DateTime timestamp = (DateTime)candidate[0];
                    publishItem.AtInMillisecondsSinceEpochUtc = (long)(timestamp.ToUniversalTime() - PublishItem.EpochUtc).TotalMilliseconds;

                    publishItem.Values = row;

                    Publisher.Send(publishItem, _resultSetId);
                }
            }
        }


        #endregion

        #endregion

        #region Public

        /// <summary>
        ///     Will stop the previous one first.
        /// </summary>
        /// <param name="test">StressTest- or TileStressTest.ToString()</param>
        public void InitializeForStressTest(string test) {
            _test = test;

            Stop();

            toolStrip.Visible = false;
            tc.Top = 0;
            tc.Height += toolStrip.Height;

            tc.SelectedIndex = 0;

            ConnectAndGetCounters();
        }

        public bool Start() {
            split.Panel2.Enabled = btnGetCounters.Enabled = false;
            propertyPanel.Lock();
            parameterPanel.Enabled = false;
            btnSetDefaultWiw.Enabled = false;

            _monitorProject.Locked = true;

            btnStart.Enabled = false;
            btnSchedule.Enabled = false;
            btnStop.Enabled = true;

            var schedule = btnSchedule.Tag as ExtendedSchedule;
            if (schedule != null && schedule.ScheduledAt > DateTime.Now)
                ScheduleMonitor();
            else
                return StartMonitor();
            return true;
        }

        private void ScheduleMonitor() {
            btnStop.Enabled = true;
            btnStart.Enabled = false;
            btnSchedule.Enabled = false;
            tmrSchedule.Start();
        }

        private void tmrSchedule_Tick(object sender, EventArgs e) {
            var schedule = btnSchedule.Tag as ExtendedSchedule;
            if (schedule.ScheduledAt <= DateTime.Now) {
                btnSchedule.Text = "Scheduled at " + schedule.ScheduledAt +
                                   GetEndsAtFormatted(schedule.ScheduledAt, schedule.Duration);
                tmrSchedule.Stop();
                StartMonitor();
            }
            else {
                if (btnSchedule.Text.StartsWith("Not scheduled")) {
                    btnSchedule.Text = "Not scheduled" + GetEndsAtFormatted(schedule.ScheduledAt, schedule.Duration);
                }
                else {
                    TimeSpan dt = schedule.ScheduledAt - DateTime.Now;
                    if (dt.Milliseconds != 0) {
                        dt = new TimeSpan(dt.Ticks - (dt.Ticks % TimeSpan.TicksPerSecond));
                        dt += new TimeSpan(0, 0, 1);
                    }
                    btnSchedule.Text = "Scheduled in " + dt.ToLongFormattedString(true) +
                                       GetEndsAtFormatted(schedule.ScheduledAt, schedule.Duration);
                }
            }
        }

        private bool StartMonitor() {
            Cursor = Cursors.WaitCursor;

            try {
                //Set the parameters and the values in the gui and in the proxy
                SetValuesToParameters();

                bool isConnected = false;
                //Re-establish the connection.
                if (_monitorSourceClient.Connect()) {
                    _monitorSourceClient.OnMonitor += _monitorSourceClient_OnMonitor;

                    _monitorSourceClient.WIW = _monitor.Wiw;

                    monitorControl.Init(_monitor, _wdyh);
                    btnSaveAllMonitorCounters.Enabled = btnSaveFilteredMonitoredCounters.Enabled = false;

                    int refreshInS = _refreshTimeInMS / 1000;
                    lblCountDown.Tag = refreshInS;
                    lblCountDown.Text = "Updates in " + refreshInS;

                    lblCountDown.ForeColor = Color.SteelBlue;
                    lblCountDown.BackColor = Color.Transparent;
                    lblCountDown.Visible = true;

                    tmrProgressDelayCountDown.Start();

                    tc.SelectedIndex = 1;

                    isConnected = true;
                }

                if (!isConnected || !_monitorSourceClient.Start()) {
                    Stop();
                    throw new Exception("The monitor did not start.");
                }

            }
            catch (Exception ex) {
                string message = "Could not connect to the monitor!";
                if (_test != null)
                    if (OnUnhandledException != null) {
                        var e = new Exception(message);
                        var invocationList = OnHandledException.GetInvocationList();
                        Parallel.For(0, invocationList.Length, (i) => {
                            (invocationList[i] as EventHandler<ErrorEventArgs>).Invoke(this, new ErrorEventArgs(e));
                        });
                    }
                    else {
                        MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                Loggers.Log(Level.Error, message, ex, new object[] { _monitor.ToString() });

                return false;
            }
            finally {
                Cursor = Cursors.Default;
            }

            PublishConfiguration();

            return true;
        }

        public void Stop() {
            Cursor = Cursors.WaitCursor;
            try {
                tmrProgressDelayCountDown.Stop();

                if (_monitorSourceClient != null)
                    try {
                        _monitorSourceClient.Stop();
                        _monitorSourceClient.OnMonitor -= _monitorSourceClient_OnMonitor;
                    }
                    catch (Exception ex) {
                        Loggers.Log(Level.Error, "Failed stopping the monitor.", ex);
                    }

                var schedule = btnSchedule.Tag as ExtendedSchedule;
                if (btnSchedule.Text.StartsWith("Not Scheduled")) {
                    btnSchedule.Text = "Schedule...";
                    if (schedule != null)
                        btnSchedule.Text += GetEndsAtFormatted(schedule.ScheduledAt, schedule.Duration);
                }
                else if (btnSchedule.Text.StartsWith("Scheduled at")) {
                    btnSchedule.Text = "Scheduled at " + schedule.ScheduledAt + GetEndsAtFormatted(schedule.ScheduledAt, schedule.Duration);
                }
                tmrSchedule.Stop();

                split.Panel2.Enabled = true;
                btnGetCounters.Enabled = true;

                propertyPanel.Unlock();
                parameterPanel.Enabled = true;
                btnSetDefaultWiw.Enabled = true;

                _monitorProject.Locked = false;

                if (!toolStrip.Visible) {
                    //Releasing it from stressTest if any
                    _test = null;

                    toolStrip.Visible = true;
                    tc.Top = toolStrip.Bottom;
                    tc.Height -= toolStrip.Height;
                }

                btnStart.Enabled = true;
                btnSchedule.Enabled = true;
                btnStop.Enabled = false;

                lblCountDown.ForeColor = Color.Black;
                lblCountDown.BackColor = Color.Orange;
                lblCountDown.Text = "Stopped!";
            }
            catch (Exception exc) {
                Loggers.Log(Level.Error, "Failed stopping the monitor.", exc);
            }

            Cursor = Cursors.Default;
        }

        public MonitorResult GetMonitorResultCache() { return monitorControl.MonitorResultCache; }


        /// <summary>
        ///     Get the connection parameters comma-separated.
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString() {
            var connectionString = new List<string>();
            foreach (Parameter parameter in _monitorSourceClient.Parameters)
                connectionString.Add(parameter.Name + "=" + parameter.Value);
            return connectionString.Combine(", ");
        }

        #endregion

        #endregion

        public class MonitorInitializedEventArgs : EventArgs {
            public readonly string ErrorMessage;
            public MonitorInitializedEventArgs(string errorMessage) {
                ErrorMessage = errorMessage;
            }
        }

        private class CountersTreeNodeTextComparer : IComparer {
            private static CountersTreeNodeTextComparer _instance = new CountersTreeNodeTextComparer();
            public int Compare(object x, object y) {
                return (x as TreeNode).Text.CompareTo((y as TreeNode).Text);
            }
            private CountersTreeNodeTextComparer() { }
            public static CountersTreeNodeTextComparer GetInstance() { return _instance; }
        }

        private class CountersReverseTreeNodeTextComparer : IComparer {
            private static CountersReverseTreeNodeTextComparer _instance = new CountersReverseTreeNodeTextComparer();
            public int Compare(object x, object y) {
                return (y as TreeNode).Text.CompareTo((x as TreeNode).Text);
            }
            private CountersReverseTreeNodeTextComparer() { }
            public static CountersReverseTreeNodeTextComparer GetInstance() { return _instance; }
        }

        private class CountersTreeNodeCheckedComparer : IComparer {
            private static CountersTreeNodeCheckedComparer _instance = new CountersTreeNodeCheckedComparer();
            public int Compare(object x, object y) {
                return (y as TreeNode).Checked.CompareTo((x as TreeNode).Checked);
            }
            private CountersTreeNodeCheckedComparer() { }
            public static CountersTreeNodeCheckedComparer GetInstance() { return _instance; }
        }

        private class EntityComparer : Comparer<Entity> {
            private static EntityComparer _instance = new EntityComparer();
            public static EntityComparer GetInstance() { return _instance; }
            public override int Compare(Entity x, Entity y) { return x.GetName().CompareTo(y.GetName()); }
        }

        private class CounterInfoComparer : Comparer<CounterInfo> {
            private static CounterInfoComparer _instance = new CounterInfoComparer();
            public static CounterInfoComparer GetInstance() { return _instance; }
            public override int Compare(CounterInfo x, CounterInfo y) { return x.GetName().CompareTo(y.GetName()); }
        }
    }
}