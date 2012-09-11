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
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;
using vApusSMT.Base;
using vApusSMT.Proxy;

namespace vApus.Monitor
{
    public partial class MonitorView : BaseSolutionComponentView
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        public event EventHandler<MonitorInitializedEventArgs> MonitorInitialized;
        public event EventHandler<ErrorEventArgs> OnHandledException, OnUnhandledException;

        #region Fields
        //Keep de dialog here.
        private LocalOrRemoteSMT _localOrRemoteSMT = new LocalOrRemoteSMT();

        //Point the solution component to here.
        private Monitor _monitor;
        //Check if this is changed --> matching the counters.
        private MonitorSource _previousMonitorSourceForParameters;
        //Filter the counters again if this changed. (combined using ", ")
        private string _previousFilter;
        //Keep this here for enabling and disabling the Gui.
        private IMonitorProxy _monitorProxy;

        //Getting all the counters.
        private delegate void WDYHDel(bool suppressErrorMessageBox);
        private WDYHDel _wdyhDel;
        private ActiveObject _activeObject = new ActiveObject();

        private Dictionary<Parameter, object> _parametersWithValues = new Dictionary<Parameter, object>();

        //The refresht ime of the counter pushing In ms 
        private int _refreshTimeInMS;

        private bool _forStresstest = false;

        private string _configuration;
        #endregion

        #region Properties
        public Monitor Monitor
        {
            get { return _monitor; }
        }
        public string Configuration
        {
            get { return _configuration; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Designer time only
        /// </summary>
        public MonitorView()
        {
            InitializeComponent();
        }
        public MonitorView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            InitializeComponent();

            _monitor = solutionComponent as Monitor;

            _wdyhDel = new WDYHDel(__WDYH);

            if (this.IsHandleCreated)
                InitMonitorView();
            else
                this.HandleCreated += new System.EventHandler(MonitorView_HandleCreated);
        }
        #endregion

        #region Functions

        #region Private
        private void _monitorProxy_OnHandledException(object sender, ErrorEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                LogWrapper.LogByLevel(this.Text + ": A counter became unavailable while monitoring:\n" + e.GetException(), LogLevel.Warning);

                if (_forStresstest && OnHandledException != null)
                    foreach (EventHandler<ErrorEventArgs> del in OnHandledException.GetInvocationList())
                        del.BeginInvoke(this, e, null, null);
            }, null);
        }
        private void _monitorProxy_OnUnhandledException(object sender, ErrorEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                Stop();
                LogWrapper.LogByLevel(this.Text + ": An error has occured while monitoring, monitor stopped!\n" + e.GetException(), LogLevel.Error);

                if (_forStresstest && OnUnhandledException != null)
                    foreach (EventHandler<ErrorEventArgs> del in OnUnhandledException.GetInvocationList())
                        del.BeginInvoke(this, e, null, null);
            }, null);
        }
        private void _monitorProxy_OnMonitor(object sender, OnMonitorEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                try
                {
                    //Don't do this when stopped
                    if (tmrProgressDelayCountDown.Enabled)
                    {
                        int refreshInS = _refreshTimeInMS / 1000;
                        lblCountDown.Tag = refreshInS;

                        lblCountDown.Text = "Updates in " + refreshInS;
                    }

                    monitorControl.AddMonitorValues(e.MonitorValues);

                    btnSaveAllMonitorCounters.Enabled = monitorControl.ColumnCount != 0;
                    btnSaveFilteredMonitoredCounters.Enabled = monitorControl.ColumnCount != 0 && txtFilterMonitorControlColumns.Text.Length != 0;


                    ExtendedSchedule schedule = btnSchedule.Tag as ExtendedSchedule;
                    if (schedule != null && schedule.Duration.Ticks != 0)
                    {
                        DateTime endsAt = schedule.ScheduledAt + schedule.Duration;
                        if (endsAt <= DateTime.Now)
                            Stop();
                    }
                }
                catch { }
            }, null);
        }

        #region Init
        private void MonitorView_HandleCreated(object sender, System.EventArgs e)
        {
            this.HandleCreated -= MonitorView_HandleCreated;
            InitMonitorView();
        }
        /// <summary>
        /// Sets the Gui and connects to smt.
        /// </summary>
        private void InitMonitorView()
        {
            Text = SolutionComponent.ToString();
            string ip = _localOrRemoteSMT.IP;
            btnLocalOrRemoteSMT.Text = (ip == "127.0.0.1") ? "SMT: <local>" : "SMT: Remote at " + ip;

            if (SynchronizationContextWrapper.SynchronizationContext == null)
                SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;

            Exception exception = InitMonitorProxy();
            propertyPanel.SolutionComponent = _monitor;
            SetFilterTextBox();

            Parameter[] parameters = _monitorProxy.GetParameters(_monitor.MonitorSource.Source, out exception);
            SetParameters(parameters);

            _previousMonitorSourceForParameters = _monitor.MonitorSource;
            _previousFilter = _monitor.Filter.Combine(", ");

            if (exception != null)
            {
                string message = "Could not connect to the monitor client.";
                LogWrapper.LogByLevel(message + "\n" + exception, LogLevel.Error);
            }

            //Use this for filtering the counters.
            SolutionComponent.SolutionComponentChanged += new System.EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);
        }
        /// <summary>
        /// Destroys the previous one if any and returns a new one.
        /// </summary>
        /// <returns></returns>
        private Exception InitMonitorProxy()
        {
            Exception exception = null;

            try
            {
                if (_monitorProxy != null)
                {
                    try
                    {
                        Exception stopEx;
                        _monitorProxy.Stop(out stopEx);
                    }
                    catch { }
                    try { _monitorProxy.Dispose(); }
                    catch { }
                    _monitorProxy = null;
                }

                _monitorProxy = CreateMonitorProxy();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception == null)
            {
                //Otherwise probing privatePath will not work --> monitorsources and ConnectionProxyPrerequisites sub folder.
                System.IO.Directory.SetCurrentDirectory(Application.StartupPath);

                var sources = _monitorProxy.GetMonitorSources(out exception);
                //Ignore this exception
                _monitor.SetMonitorSources(sources);

                exception = null;
            }
            return exception;
        }
        /*
                 private Exception ConnectToSMT(string ip)
        {
            Exception exception = null;

            try
            {
                if (_monitorProxy != null)
                {
                    try {
                        Exception stopEx;
                        _monitorProxy.Stop(out stopEx); }
                    catch { }
                    try { _monitorProxy.Dispose(); }
                    catch { }
                    _monitorProxy = null;
                }

                _monitorProxy = CreateMonitorProxy(ip);
                _monitorProxy.OnHandledException += new EventHandler<ErrorEventArgs>(_monitorProxy_OnHandledException);
                _monitorProxy.OnUnhandledException += new EventHandler<ErrorEventArgs>(_monitorProxy_OnUnhandledException);
                _monitorProxy.OnMonitor += new EventHandler<OnMonitorEventArgs>(_monitorProxy_OnMonitor);
                _monitorProxy.Connect(out exception);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception == null)
            {
                //Otherwise probing privatePath will not work --> monitorsources and ConnectionProxyPrerequisites sub folder.
                System.IO.Directory.SetCurrentDirectory(Application.StartupPath);

                var sources = _monitorProxy.GetMonitorSources(out exception);
                //Ignore this exception
                _monitor.SetMonitorSources(sources);

                exception = null;
            }
            return exception;
        }
*/
        /// <summary>
        /// Creates an instance using reflection.
        /// </summary>
        /// <param name="ip">If 127.0.0.1 the local one will be used, otherwise the remote one.</param>
        /// <returns></returns>
        private IMonitorProxy CreateMonitorProxy()
        {
            var monitorProxy = new MonitorProxy();
            monitorProxy.OnHandledException += new EventHandler<ErrorEventArgs>(_monitorProxy_OnHandledException);
            monitorProxy.OnUnhandledException += new EventHandler<ErrorEventArgs>(_monitorProxy_OnUnhandledException);
            monitorProxy.OnMonitor += new EventHandler<OnMonitorEventArgs>(_monitorProxy_OnMonitor);

            return monitorProxy;
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            if (sender == _monitor && this.IsHandleCreated)
            {
                if (_monitor.MonitorSource != _previousMonitorSourceForParameters)
                {
                    _previousMonitorSourceForParameters = _monitor.MonitorSource;

                    Exception exception;
                    if (_monitorProxy == null)
                        _monitorProxy = CreateMonitorProxy();

                    Parameter[] parameters = _monitorProxy.GetParameters(_monitor.MonitorSource.Source, out exception);
                    SetParameters(parameters);
                }
                if (_monitor.MonitorSourceIndex == _monitor.PreviousMonitorSourceIndexForCounters || lvwEntities.Items.Count == 0)
                {
                    split.Panel2.Enabled = true;
                    lblMonitorSourceMismatch.Visible = false;

                    btnStart.Enabled = btnSchedule.Enabled = lvwEntities.Items.Count != 0 && _monitor.Wiw.Count != 0;
                }
                else
                {
                    split.Panel2.Enabled = false;
                    lblMonitorSourceMismatch.Visible = true;

                    btnStart.Enabled = btnSchedule.Enabled = false;
                }
                //Filter the treenodes again if this is changed.
                string filter = _monitor.Filter.Combine(", ");
                if (filter != _previousFilter)
                {
                    _previousFilter = filter;
                    FillCounters();
                }
            }
        }
        #endregion

        #region Fill & Filter tvwCounters, push the saved WIW tot the gui
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lvw">The selected item from this list view is used</param>
        /// <param name="tvw">Counters are put in this treeview</param>
        private void FillCounters()
        {
            if (lvwEntities.SelectedItems.Count != 0)
            {
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
                llblCheckAllVisible.Enabled = !llblUncheckAllVisible.Enabled;
            }
        }
        private bool HasCheckedNodes()
        {
            foreach (TreeNode node in tvwCounters.Nodes)
                if (node.Checked)
                    return true;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter">if the length == 0 the original 'nodes' is returned</param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private TreeNode[] FilterCounters(string[] filter, TreeNode[] nodes)
        {
            if (filter.Length == 0)
                return nodes;
            List<TreeNode> filtered = new List<TreeNode>();
            foreach (string s in filter)
                foreach (TreeNode node in Find(s, nodes))
                    if (!filtered.Contains(node))
                        filtered.Add(node);
            return filtered.ToArray();
        }
        private IEnumerable<TreeNode> Find(string text, TreeNode[] nodes)
        {
            text = Regex.Escape(text);
            text = text.Replace("\\*", ".*");
            text = "\\b" + text + "\\b";
            RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase;

            foreach (TreeNode node in nodes)
                if (Regex.IsMatch(node.Text, text, options))
                    yield return node;
        }
        /// <summary>
        /// Parse a List<CounterInfo> tag to a TreeNode[] tag search in the given counter tvw if it does not exists.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="tvw">searches for existing nodes in this tvw</param>
        private void ParseTag(ListViewItem item)
        {
            //Make a gui for the data store in the tag (this tag will be switched with the gui).
            if (item.Tag is List<CounterInfo>)
            {
                List<CounterInfo> counters = item.Tag as List<CounterInfo>;
                TreeNode[] newTag = new TreeNode[counters.Count];
                for (int i = 0; i != counters.Count; i++)
                {
                    CounterInfo counterInfo = counters[i];
                    string counter = counterInfo.Counter;
                    TreeNode counterNode = null;

                    //Search from end (faster) if the counter node already exists.
                    for (int j = tvwCounters.Nodes.Count - 1; j != -1; j--)
                    {
                        var node = tvwCounters.Nodes[j];
                        if (node.Text == counter)
                        {
                            counterNode = node;
                            break;
                        }
                    }

                    if (counterNode == null)
                        counterNode = new TreeNode(counter);
                    newTag[i] = counterNode;

                    if (counterInfo.Instances.Count != 0)
                    {
                        //Only add the first, the rest will be added when the node expands. (tvwCounter.BeforeExpand)
                        counterNode.Nodes.Add(counterInfo.Instances[0]);

                        //Keep the rest in the tag.
                        if (counterInfo.Instances.Count != 1)
                        {
                            TreeNode[] otherInstances = new TreeNode[counterInfo.Instances.Count - 1];
                            Parallel.For(1, counterInfo.Instances.Count, delegate(int k)
                            {
                                otherInstances[k - 1] = new TreeNode(counterInfo.Instances[k]);
                            });

                            counterNode.Tag = otherInstances;
                        }
                    }
                }
                item.Tag = newTag;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wiw"></param>
        /// <param name="entityName"></param>
        /// <returns>If not found a new entity with an empty name is returned</returns>
        private Entity GetEntity(Dictionary<Entity, List<CounterInfo>> wiw, string entityName)
        {
            foreach (Entity entity in wiw.Keys)
                if (entity.Name == entityName)
                    return entity;
            return new Entity(string.Empty, vApusSMT.Base.PowerState.Off);
        }
        private void PushSavedWiW()
        {
            lvwEntities.ItemChecked -= lvwEntities_ItemChecked;
            tvwCounters.AfterCheck -= tvwCounter_AfterCheck;

            //Make a new wiw to ensure that only valid counters remain in WiW (different machines can have different counters)
            var newWIW = new Dictionary<Entity, List<CounterInfo>>();
            foreach (ListViewItem lvwi in lvwEntities.Items)
            {
                string entityName = lvwi.SubItems[1].Text;
                Entity entity = GetEntity(_monitor.Wiw, entityName);
                lvwi.Checked = entity.Name.Length != 0;
                if (lvwi.Checked)
                {
                    ParseTag(lvwi);
                    newWIW.Add(entity, new List<CounterInfo>());
                }

                TreeNode[] nodes = lvwi.Tag as TreeNode[];
                if (nodes != null)
                    if (lvwi.Checked)
                    {
                        List<CounterInfo> l = _monitor.Wiw[entity];
                        foreach (TreeNode node in nodes)
                        {
                            CounterInfo info = GetCounterInfo(node.Text, l);
                            CounterInfo newInfo = null;
                            node.Checked = info != null;

                            if (node.Checked)
                            {
                                newInfo = new CounterInfo(info.Counter, node.Nodes.Count == 0 ? null : new List<string>());
                                foreach (TreeNode child in node.Nodes)
                                {
                                    child.Checked = info.Instances.Contains(child.Text);
                                    if (child.Checked)
                                        newInfo.Instances.Add(child.Text);
                                }
                            }
                            else
                                foreach (TreeNode child in node.Nodes)
                                    child.Checked = false;

                            TreeNode[] childNodes = node.Tag as TreeNode[];
                            if (childNodes != null)
                                if (node.Checked)
                                    foreach (TreeNode child in childNodes)
                                    {
                                        child.Checked = info.Instances.Contains(child.Text);
                                        if (child.Checked)
                                            newInfo.Instances.Add(child.Text);
                                    }
                                else
                                    foreach (TreeNode child in childNodes)
                                        child.Checked = false;

                            if (newInfo != null)
                                newWIW[entity].Add(newInfo);
                        }
                    }
                    else
                    {
                        foreach (TreeNode node in nodes)
                        {
                            node.Checked = false;
                            foreach (TreeNode child in node.Nodes)
                                child.Checked = false;

                            TreeNode[] childNodes = node.Tag as TreeNode[];
                            if (childNodes != null)
                                foreach (TreeNode child in childNodes)
                                    child.Checked = false;
                        }
                    }
            }

            //Set the new wiw
            _monitor.Wiw = newWIW;

            SetChosenCountersInListViewItems();

            btnStart.Enabled = btnSchedule.Enabled = (_monitor.MonitorSourceIndex == _monitor.PreviousMonitorSourceIndexForCounters || lvwEntities.Items.Count == 0) ?
                _monitor.Wiw.Count != 0 : false;

            lvwEntities.ItemChecked += lvwEntities_ItemChecked;
            tvwCounters.AfterCheck += tvwCounter_AfterCheck;
        }
        private CounterInfo GetCounterInfo(string counter, List<CounterInfo> l)
        {
            foreach (CounterInfo info in l)
                if (info.Counter == counter)
                    return info;
            return null;
        }
        private void txtFilter_Leave(object sender, EventArgs e)
        {
            SetFilter();
        }
        private void txtFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                SetFilter();
        }
        private void picFilter_Click(object sender, EventArgs e)
        {
            SetFilter();
        }
        private void SetFilter()
        {
            string[] filter = txtFilter.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> l = new List<string>();
            for (int i = 0; i != filter.Length; i++)
            {
                string entry = filter[i].Trim();
                if (entry.Length != 0 && !l.Contains(entry))
                    l.Add(entry);
            }

            _monitor.Filter = l.ToArray();

            SetFilterTextBox();
            _monitor.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        private void SetFilterTextBox()
        {
            try
            {
                txtFilter.Text = _monitor.Filter.Combine(", ");
                llblUncheckAllVisible.Enabled = HasCheckedNodes();
                llblCheckAllVisible.Enabled = !llblUncheckAllVisible.Enabled;
            }
            catch { }
        }
        #endregion

        private void btnGetCounters_Click(object sender, EventArgs e)
        {
            ConnectAndGetCounters();
        }
        private void ConnectAndGetCounters()
        {
            this.Cursor = Cursors.WaitCursor;

            if (_monitor.PreviousMonitorSourceIndexForCounters != _monitor.MonitorSourceIndex)
            {
                _monitor.PreviousMonitorSourceIndexForCounters = _monitor.MonitorSourceIndex;
                //Clear this when a new is selected.
                _monitor.Wiw.Clear();
            }
            lblMonitorSourceMismatch.Visible = false;

            btnGetCounters.Enabled = false;
            propertyPanel.Lock();
            parameterPanel.Lock();
            split.Panel2.Enabled = false;

            tvwCounters.Nodes.Clear();
            lvwEntities.Items.Clear();

            btnStart.Enabled = false;
            btnSchedule.Enabled = false;
            btnLocalOrRemoteSMT.Enabled = false;

            btnGetCounters.Text = "Getting Counters...";
            _activeObject.Send(_wdyhDel, _forStresstest);
        }
        private void __WDYH(bool forStresstest)
        {
            if (_monitorProxy == null)
                _monitorProxy = CreateMonitorProxy();

            Dictionary<Entity, List<CounterInfo>> wdyh = null;
            string configuration = null;

            //Set the parameters and the values in the gui and in the proxy
            Exception exception;
            Parameter[] parameters = _monitorProxy.GetParameters(_monitor.MonitorSource.Source, out exception);
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                SetParameters(parameters);
            }, null);
            if (exception == null)
                _monitorProxy.Connect(_monitor.MonitorSource.Source, out exception);

            if (exception == null)
                _refreshTimeInMS = _monitorProxy.GetRefreshRateInMs(_monitor.MonitorSource.Source, out exception);

            if (exception == null)
                configuration = _monitorProxy.GetConfigurationXML(out exception);

            if (exception == null)
                wdyh = _monitorProxy.GetWDYH(out exception);

            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                if (exception == null)
                {
                    btnConfiguration.Enabled = (configuration != null);
                    _configuration = configuration;
                    try
                    {
                        FillEntities(wdyh);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                }

                btnGetCounters.Text = "Get Counters";

                if (exception != null)
                {
                    btnStart.Enabled = false;
                    btnSchedule.Enabled = false;

                    string message = "Entities and counters could not be retrieved!\nHave you filled in the right credentials?";
                    if (!forStresstest)
                        MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogWrapper.LogByLevel(message + "\n" + exception, LogLevel.Error);
                }
                split.Panel2.Enabled = true;
                btnGetCounters.Enabled = true;
                btnLocalOrRemoteSMT.Enabled = true;
                propertyPanel.Unlock();
                parameterPanel.Unlock();

                this.Cursor = Cursors.Default;
            }, null);
        }

        private void SetParameters(Parameter[] parameters)
        {
            _parametersWithValues.Clear();
            if (parameters != null)
            {
                //Get parameter values and set the parameters
                object[] monitorParameters = _monitor.Parameters;
                for (int i = 0; i != parameters.Length; i++)
                {
                    Parameter parameter = parameters[i];
                    object value = parameter.DefaultValue;
                    if (i < monitorParameters.Length)
                    {
                        object candidate = monitorParameters[i];
                        if (candidate.GetType() == parameter.Type)
                            value = candidate;
                    }
                    _parametersWithValues.Add(parameter, value);
                }

                object[] parameterValues = new object[_parametersWithValues.Count];

                int valueIndex = 0;
                foreach (Parameter key in _parametersWithValues.Keys)
                {
                    object value = _parametersWithValues[key];
                    //Take encryption into account.
                    if (key.Encrypted && value is string)
                        value = (value as string).Encrypt("{A84E447C-3734-4afd-B383-149A7CC68A32}", new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    parameterValues[valueIndex++] = value;
                }
                Exception ex;
                _monitorProxy.SetParameterValues(parameterValues, out ex);
            }
            parameterPanel.ParametersWithValues = _parametersWithValues;
        }
        private void parameterPanel_ParameterValueChanged(object sender, EventArgs e)
        {
            StoreParameterValues();
        }
        /// <summary>
        /// Store from _parametersWithValues to the monitor object
        /// </summary>
        private void StoreParameterValues()
        {
            object[] monitorParameters = new object[_parametersWithValues.Count];
            int i = 0;
            foreach (object value in _parametersWithValues.Values)
                monitorParameters[i++] = value;
            _monitor.Parameters = monitorParameters;

            _monitor.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        private void FillEntities(Dictionary<Entity, List<CounterInfo>> entitiesAndCounters)
        {
            foreach (Entity entity in entitiesAndCounters.Keys)
            {
                ListViewItem lvwi = new ListViewItem(string.Empty);

                lvwi.SubItems.Add(entity.Name);
                lvwi.SubItems.Add("[0]");
                lvwi.ImageIndex = (int)entity.PowerState;
                lvwi.StateImageIndex = lvwi.ImageIndex;
                lvwi.Tag = entitiesAndCounters[entity];
                lvwi.Checked = false;

                lvwEntities.Items.Add(lvwi);
            }
            split.Panel2.Enabled = lvwEntities.Items.Count != 0;

            if (lvwEntities.Items.Count != 0)
                lvwEntities.Items[0].Selected = true;
        }

        private void lvwEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwEntities.SelectedItems.Count != 0)
            {
                lvwEntities.Tag = lvwEntities.SelectedItems[0];
                FillCounters();
            }
        }
        private void lvwEntities_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            bool itemChecked = e.Item.Checked;
            e.Item.Selected = true;

            //Push wiw clears this otherwise.
            lvwEntities.ItemChecked -= lvwEntities_ItemChecked;
            e.Item.Checked = itemChecked;
            lvwEntities.ItemChecked += lvwEntities_ItemChecked;
            
            ExtractWIWForListViewAction();
        }
        private void ExtractWIWForListViewAction()
        {
            LockWindowUpdate(this.Handle.ToInt32());
            try
            {
                tvwCounters.AfterCheck -= tvwCounter_AfterCheck;
                var selected = lvwEntities.Tag as ListViewItem;

                ParseTag(selected);
                TreeNode[] nodes = selected.Tag as TreeNode[];
                bool selectedChecked = selected.Checked;

                foreach (TreeNode counterNode in nodes)
                {
                    counterNode.Checked = selectedChecked;
                    foreach (TreeNode node in counterNode.Nodes)
                        node.Checked = selectedChecked;

                    if (counterNode.Tag != null)
                    {
                        TreeNode[] counterNodes = counterNode.Tag as TreeNode[];
                        foreach (TreeNode node in counterNodes)
                            node.Checked = selectedChecked;
                    }

                    if (selectedChecked)
                        ApplyToWIW(counterNode);
                }
                if (!selectedChecked)
                {
                    Entity entity = GetEntity(_monitor.Wiw, selected.SubItems[1].Text);
                    _monitor.Wiw.Remove(entity);
                }
                tvwCounters.AfterCheck += tvwCounter_AfterCheck;

                SetChosenCountersInListViewItems();
                btnStart.Enabled = btnSchedule.Enabled = lvwEntities.Items.Count != 0 && _monitor.Wiw.Count != 0;

                _monitor.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
            catch { throw; }
            finally
            {
                LockWindowUpdate(0);
            }
        }

        private void SetChosenCountersInListViewItems()
        {
            List<ListViewItem> applyCountTo = new List<ListViewItem>(lvwEntities.Items.Count);
            foreach (ListViewItem lvwi in lvwEntities.Items)
                applyCountTo.Add(lvwi);

            foreach (Entity entity in _monitor.Wiw.Keys)
            {
                var l = _monitor.Wiw[entity];
                int count = GetTotalCountOfCounters(l);
                foreach (ListViewItem lvwi in lvwEntities.Items)
                    if (lvwi.SubItems[1].Text == entity.Name)
                    {
                        lvwi.SubItems[2].Text = "[" + count + "]";
                        applyCountTo.Remove(lvwi);
                        break;
                    }
            }
            //For the ones that aren't in Wiw.
            foreach (ListViewItem lvwi in applyCountTo)
                lvwi.SubItems[2].Text = "[0]";
        }
        private int GetTotalCountOfCounters(List<CounterInfo> list)
        {
            int count = 0;
            foreach (CounterInfo info in list)
            {
                int c = info.Instances.Count;
                if (c == 0)
                    c = 1;
                count += c;
            }
            return count;
        }
        private void llblCheckAllVisible_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());
            try
            {
                tvwCounters.AfterCheck -= tvwCounter_AfterCheck;

                foreach (TreeNode counterNode in tvwCounters.Nodes)
                {
                    counterNode.Checked = true;
                    foreach (TreeNode node in counterNode.Nodes)
                        node.Checked = true;

                    if (counterNode.Tag != null)
                    {
                        TreeNode[] counterNodes = counterNode.Tag as TreeNode[];
                        foreach (TreeNode node in counterNodes)
                            node.Checked = true;
                    }
                    ApplyToWIW(counterNode);
                }

                tvwCounters.AfterCheck += tvwCounter_AfterCheck;
                SetChosenCountersInListViewItems();

                btnStart.Enabled = btnSchedule.Enabled = lvwEntities.Items.Count != 0 && _monitor.Wiw.Count != 0;
            }
            catch { throw; }
            finally
            {
                LockWindowUpdate(0);
            }
        }
        private void llblUncheckAllVisible_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());
            try
            {
                tvwCounters.AfterCheck -= tvwCounter_AfterCheck;

                foreach (TreeNode counterNode in tvwCounters.Nodes)
                {
                    counterNode.Checked = false;
                    foreach (TreeNode node in counterNode.Nodes)
                        node.Checked = false;

                    if (counterNode.Tag != null)
                    {
                        TreeNode[] counterNodes = counterNode.Tag as TreeNode[];
                        foreach (TreeNode node in counterNodes)
                            node.Checked = false;
                    }

                    ApplyToWIW(counterNode);
                }

                tvwCounters.AfterCheck += tvwCounter_AfterCheck;
                SetChosenCountersInListViewItems();

                btnStart.Enabled = btnSchedule.Enabled = lvwEntities.Items.Count != 0 && _monitor.Wiw.Count != 0;
            }
            catch { throw; }
            finally
            {
                LockWindowUpdate(0);
            }
        }

        private void tvwCounter_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            AddNodesFromTag(e.Node);
        }
        private void AddNodesFromTag(TreeNode node)
        {
            if (node.Tag != null)
            {
                LockWindowUpdate(this.Handle.ToInt32());
                try
                {
                    node.Nodes.AddRange(node.Tag as TreeNode[]);
                    node.Tag = null;
                }
                catch { throw; }
                finally
                {
                    LockWindowUpdate(0);
                }
            }
        }
        private void tvwCounter_AfterCheck(object sender, TreeViewEventArgs e)
        {
            ExtractWIWForTreeViewAction(e.Node);
        }

        private void ExtractWIWForTreeViewAction(TreeNode counterNode)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            tvwCounters.AfterCheck -= tvwCounter_AfterCheck;
            if (counterNode.Level == 0)
            {
                foreach (TreeNode node in counterNode.Nodes)
                    node.Checked = counterNode.Checked;

                if (counterNode.Tag != null)
                {
                    TreeNode[] counterNodes = counterNode.Tag as TreeNode[];
                    foreach (TreeNode node in counterNodes)
                        node.Checked = counterNode.Checked;
                }
                ApplyToWIW(counterNode);
            }
            else
            {
                counterNode.Parent.Checked = false;
                foreach (TreeNode node in counterNode.Parent.Nodes)
                    if (node.Checked)
                    {
                        counterNode.Parent.Checked = true;
                        break;
                    }
                ApplyToWIW(counterNode.Parent);
            }
            tvwCounters.AfterCheck += tvwCounter_AfterCheck;

            SetChosenCountersInListViewItems();

            btnStart.Enabled = btnSchedule.Enabled = lvwEntities.Items.Count != 0 && _monitor.Wiw.Count != 0;

            _monitor.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            LockWindowUpdate(0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterNode">Add to or remove this from WiW depending on the checkstate of the node and its children</param>
        private void ApplyToWIW(TreeNode counterNode)
        {
            lvwEntities.ItemChecked -= lvwEntities_ItemChecked;

            ListViewItem lvwiEntity = lvwEntities.Tag as ListViewItem;
            string entityName = lvwiEntity.SubItems[1].Text;
            Entity entity = GetEntity(_monitor.Wiw, entityName);

            lvwiEntity.Checked = false;
            foreach (TreeNode node in tvwCounters.Nodes)
                if (node.Checked)
                {
                    lvwiEntity.Checked = true;
                    break;
                }

            if (lvwiEntity.Checked)
            {
                if (_monitor.Wiw.ContainsKey(entity))
                {
                    foreach (CounterInfo info in _monitor.Wiw[entity])
                        if (info.Counter == counterNode.Text)
                        {
                            _monitor.Wiw[entity].Remove(info);
                            break;
                        }
                    if (counterNode.Checked)
                    {
                        CounterInfo newCounterInfo = new CounterInfo(counterNode.Text);
                        foreach (TreeNode node in counterNode.Nodes)
                            if (node.Checked)
                                newCounterInfo.Instances.Add(node.Text);

                        if (counterNode.Tag != null)
                        {
                            TreeNode[] counterNodes = counterNode.Tag as TreeNode[];
                            foreach (TreeNode node in counterNodes)
                                if (node.Checked)
                                    newCounterInfo.Instances.Add(node.Text);
                        }

                        _monitor.Wiw[entity].Add(newCounterInfo);
                    }
                }
                else
                {
                    List<CounterInfo> counters = new List<CounterInfo>();
                    CounterInfo newCounterInfo = new CounterInfo(counterNode.Text);
                    foreach (TreeNode node in counterNode.Nodes)
                        if (node.Checked)
                            newCounterInfo.Instances.Add(node.Text);

                    if (counterNode.Tag != null)
                    {
                        TreeNode[] counterNodes = counterNode.Tag as TreeNode[];
                        foreach (TreeNode node in counterNodes)
                            if (node.Checked)
                                newCounterInfo.Instances.Add(node.Text);
                    }
                    counters.Add(newCounterInfo);

                    //Random powerstate, doesn't matter
                    entity = new Entity(entityName, vApusSMT.Base.PowerState.On);
                    _monitor.Wiw.Add(entity, counters);
                }
            }
            else
                _monitor.Wiw.Remove(entity);

            lvwEntities.ItemChecked += lvwEntities_ItemChecked;
        }

        private void btnConfiguration_Click(object sender, EventArgs e)
        {
            if (_configuration != null)
                (new ConfigurationDialog(_configuration)).ShowDialog();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (monitorControl.RowCount == 0 ||
               (monitorControl.RowCount > 0 &&
               MessageBox.Show("Are you sure you want to start a new monitor?\nThis will clear the previous measured performance counters.", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes))
            {
                _forStresstest = false;
                Start();
            }
        }
        private void btnSchedule_Click(object sender, EventArgs e)
        {
            ExtendedSchedule schedule = btnSchedule.Tag as ExtendedSchedule;
            if (schedule == null)
                schedule = new ExtendedSchedule();

            if (schedule.ShowDialog() == DialogResult.OK)
            {
                if (schedule.ScheduledAt > DateTime.Now)
                {
                    btnSchedule.Text = "Scheduled at " + schedule.ScheduledAt;
                    btnSchedule.Text += GetEndsAtFormatted(schedule.ScheduledAt, schedule.Duration);
                    btnSchedule.Tag = schedule;
                }
                else
                {
                    btnSchedule.Text = "Not Scheduled" + GetEndsAtFormatted(schedule.ScheduledAt, schedule.Duration);
                    btnSchedule.Tag = schedule;
                }

                btnStart_Click(this, null);
            }
        }
        private string GetEndsAtFormatted(DateTime scheduledAt, TimeSpan duration)
        {
            if (duration.Ticks == 0)
                return string.Empty;

            if (duration.Milliseconds != 0)
            {
                duration = new TimeSpan(duration.Ticks - (duration.Ticks % TimeSpan.TicksPerSecond));
                duration += new TimeSpan(0, 0, 1);
            }
            return "; Ends At " + (scheduledAt + duration);
        }
        private void tmrProgressDelayCountDown_Tick(object sender, EventArgs e)
        {
            //Avoid a label update when after pushed stop
            if (tmrProgressDelayCountDown.Enabled)
            {
                int countDown = (int)lblCountDown.Tag - 1;
                if (countDown > 0)
                {
                    lblCountDown.Text = "Updates in " + countDown;
                    lblCountDown.Tag = countDown;
                }
            }
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

#warning btnLocalOrRemoteSMT_Click Uitgeschakeld atm
        private void btnLocalOrRemoteSMT_Click(object sender, EventArgs e)
        {
            //Set the Gui again, this will connect to smt.
            if (_localOrRemoteSMT.ShowDialog() == DialogResult.OK)
            {
                if (_monitorProxy != null)
                {
                    try
                    {
                        Exception stopEx;
                        _monitorProxy.Stop(out stopEx);
                    }
                    catch { }
                    try { _monitorProxy.Dispose(); }
                    catch { }
                    _monitorProxy = null;
                }
                _monitor.Wiw.Clear();
                tvwCounters.Nodes.Clear();
                lvwEntities.Items.Clear();

                btnStart.Enabled = false;
                btnSchedule.Enabled = false;
                btnConfiguration.Enabled = false;

                int _monitorSourceIndex = _monitor.MonitorSourceIndex;
                InitMonitorView();
                try
                {
                    _monitor.MonitorSource = _monitor._monitorSources[_monitorSourceIndex];
                }
                catch { }

                LockWindowUpdate(this.Handle.ToInt32());
                try
                {
                    propertyPanel.SolutionComponent = null;
                    propertyPanel.SolutionComponent = _monitor;
                }
                catch { throw; }
                finally
                {
                    LockWindowUpdate(0);
                }
            }
        }
        private void btnSaveAllMonitorCounters_Click(object sender, EventArgs e)
        {
            if (monitorControl.GetHeaders() != null && monitorControl.GetHeaders().Length != 0)
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;
                    monitorControl.Save(saveFileDialog.FileName);
                    this.Cursor = Cursors.Default;
                }

        }
        private void btnSaveFilteredMonitoredCounters_Click(object sender, EventArgs e)
        {
            if (monitorControl.GetHeaders() != null && monitorControl.GetHeaders().Length != 0)
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;
                    monitorControl.SaveFiltered(saveFileDialog.FileName);
                    this.Cursor = Cursors.Default;
                }
        }
        private void txtFilterMonitorControlColumns_TextChanged(object sender, EventArgs e)
        {
            try
            {
                btnSaveFilteredMonitoredCounters.Enabled = monitorControl.ColumnCount != 0 && txtFilterMonitorControlColumns.Text.Length != 0;
            }
            catch { }
        }
        private void txtFilterMonitorControlColumns_Leave(object sender, EventArgs e)
        {
            SetColumnFilter();
        }
        private void txtFilterMonitorControlColumns_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                SetColumnFilter();
        }
        private void picFilterMonitorControlColumns_Click(object sender, EventArgs e)
        {
            SetColumnFilter();
        }
        private void SetColumnFilter()
        {
            string[] filter = txtFilterMonitorControlColumns.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> l = new List<string>();
            for (int i = 0; i != filter.Length; i++)
            {
                string entry = filter[i].Trim();
                if (entry.Length != 0 && !l.Contains(entry))
                    l.Add(entry);
            }

            filter = l.ToArray();
            monitorControl.Filter(filter);
            txtFilterMonitorControlColumns.Text = filter.Combine(", ");
        }

        #endregion

        #region Public
        /// <summary>
        /// Will stop the previous one first.
        /// </summary>
        public void InitializeForStresstest()
        {
            _forStresstest = true;

            Stop();

            toolStrip.Visible = false;
            tc.Top = 0;
            tc.Height += toolStrip.Height;

            tc.SelectedIndex = 0;

            _activeObject.OnResult += new EventHandler<ActiveObject.OnResultEventArgs>(_activeObject_OnResult);

            ConnectAndGetCounters();
        }

        private void _activeObject_OnResult(object sender, ActiveObject.OnResultEventArgs e)
        {
            _activeObject.OnResult -= _activeObject_OnResult;

            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                string errorMessage = null;
                if (split.Panel2.Enabled && lvwEntities.Items.Count != 0 && tvwCounters.Nodes.Count != 0)
                {
                    errorMessage = this.Text + ": No counters were chosen.";
                    if (_monitor.Wiw.Count != 0)
                        foreach (Entity entity in _monitor.Wiw.Keys)
                        {
                            if (_monitor.Wiw[entity].Count != 0)
                                errorMessage = null;
                            break;
                        }
                }
                else
                {
                    errorMessage = this.Text + ": Entities and counters could not be retrieved!\nHave you filled in the right credentials?";
                }
                if (MonitorInitialized != null)
                    MonitorInitialized(this, new MonitorView.MonitorInitializedEventArgs(errorMessage));

            }, null);
        }
        public void Start()
        {
            split.Panel2.Enabled = false;
            btnGetCounters.Enabled = false;
            propertyPanel.Lock();
            parameterPanel.Lock();

            btnStart.Enabled = false;
            btnSchedule.Enabled = false;
            btnStop.Enabled = true;
            btnLocalOrRemoteSMT.Enabled = false;

            ExtendedSchedule schedule = btnSchedule.Tag as ExtendedSchedule;
            if (schedule != null && schedule.ScheduledAt > DateTime.Now)
                ScheduleMonitor();
            else
                StartMonitor();
        }
        private void ScheduleMonitor()
        {
            btnStop.Enabled = true;
            btnStart.Enabled = false;
            btnSchedule.Enabled = false;
            btnLocalOrRemoteSMT.Enabled = false;
            tmrSchedule.Start();
        }
        private void tmrSchedule_Tick(object sender, EventArgs e)
        {
            ExtendedSchedule schedule = btnSchedule.Tag as ExtendedSchedule;
            if (schedule.ScheduledAt <= DateTime.Now)
            {
                btnSchedule.Text = "Scheduled at " + schedule.ScheduledAt + GetEndsAtFormatted(schedule.ScheduledAt, schedule.Duration);
                tmrSchedule.Stop();
                StartMonitor();
            }
            else
            {
                if (btnSchedule.Text.StartsWith("Not Scheduled"))
                {
                    btnSchedule.Text = "Not Scheduled" + GetEndsAtFormatted(schedule.ScheduledAt, schedule.Duration);
                }
                else
                {
                    TimeSpan dt = schedule.ScheduledAt - DateTime.Now;
                    if (dt.Milliseconds != 0)
                    {
                        dt = new TimeSpan(dt.Ticks - (dt.Ticks % TimeSpan.TicksPerSecond));
                        dt += new TimeSpan(0, 0, 1);
                    }
                    btnSchedule.Text = "Scheduled in " + dt.ToLongFormattedString() + GetEndsAtFormatted(schedule.ScheduledAt, schedule.Duration);
                }
            }
        }
        private void StartMonitor()
        {
            this.Cursor = Cursors.WaitCursor;
            Exception exception;
            string[] units = { };

            //Set the parameters and the values in the gui and in the proxy
            Parameter[] parameters = _monitorProxy.GetParameters(_monitor.MonitorSource.Source, out exception);
            SetParameters(parameters);

            if (exception == null)
                _monitorProxy.SetWIW(_monitor.Wiw, out exception);
            if (exception == null)
                units = _monitorProxy.GetUnits(out exception);

            if (exception == null)
            {
                monitorControl.Init(_monitor.Wiw, units);
                btnSaveAllMonitorCounters.Enabled = btnSaveFilteredMonitoredCounters.Enabled = false;

                int refreshInS = _refreshTimeInMS / 1000;
                lblCountDown.Tag = refreshInS;
                lblCountDown.Text = "Updates in " + refreshInS;

                lblCountDown.ForeColor = Color.SteelBlue;
                lblCountDown.BackColor = Color.Transparent;
                lblCountDown.Visible = true;

                tmrProgressDelayCountDown.Start();

                tc.SelectedIndex = 1;
            }

            if (exception == null)
            {
                _monitorProxy.Start(out exception);
            }
            else
            {
                Stop();
                string message = "Could not connect to the monitor!";
                if (_forStresstest)
                    if (OnUnhandledException != null)
                    {
                        Exception e = new Exception(message);
                        foreach (EventHandler<ErrorEventArgs> del in OnUnhandledException.GetInvocationList())
                            del.BeginInvoke(this, new ErrorEventArgs(e), null, null);
                    }
                    else
                    {
                        MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                LogWrapper.LogByLevel(message + "\n" + exception, LogLevel.Error);
            }
            this.Cursor = Cursors.Default;
        }

        public void Stop()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                tmrProgressDelayCountDown.Stop();

                if (_monitorProxy != null)
                    try
                    {
                        Exception stopEx;
                        _monitorProxy.Stop(out stopEx);
                    }
                    catch { }

                ExtendedSchedule schedule = btnSchedule.Tag as ExtendedSchedule;
                if (btnSchedule.Text.StartsWith("Not Scheduled"))
                {
                    btnSchedule.Text = "Schedule...";
                    if (schedule != null)
                        btnSchedule.Text += GetEndsAtFormatted(schedule.ScheduledAt, schedule.Duration);
                }
                else if (btnSchedule.Text.StartsWith("Scheduled at"))
                {
                    btnSchedule.Text = "Scheduled at " + schedule.ScheduledAt + GetEndsAtFormatted(schedule.ScheduledAt, schedule.Duration);
                }
                tmrSchedule.Stop();

                split.Panel2.Enabled = true;
                btnGetCounters.Enabled = true;
                propertyPanel.Unlock();
                parameterPanel.Unlock();

                if (!toolStrip.Visible)
                {
                    //Releasing it from stresstest if any
                    _forStresstest = false;

                    toolStrip.Visible = true;
                    tc.Top = toolStrip.Bottom;
                    tc.Height -= toolStrip.Height;
                }

                btnStart.Enabled = true;
                btnSchedule.Enabled = true;
                btnStop.Enabled = false;
                btnLocalOrRemoteSMT.Enabled = true;

                lblCountDown.ForeColor = Color.Black;
                lblCountDown.BackColor = Color.Orange;
                lblCountDown.Text = "Stopped!";
            }
            catch { }

            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Gets the headers of all the counters
        /// </summary>
        /// <returns></returns>
        public string[] GetHeaders()
        {
            return monitorControl.GetHeaders();
        }
        public Dictionary<DateTime, float[]> GetMonitorValues()
        {
            return monitorControl.GetMonitorValues();
        }
        #endregion

        #endregion

        public class MonitorInitializedEventArgs : EventArgs
        {
            public readonly string ErrorMessage;
            public MonitorInitializedEventArgs(string errorMessage)
            { ErrorMessage = errorMessage; }
        }
    }
}
