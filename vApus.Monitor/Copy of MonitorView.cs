/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;
using vApusSMT.Base;
using vApusSMT.Communication;
using System.Collections;

namespace vApus.Monitor
{
    public partial class MonitorViewCopy : BaseSolutionComponentView
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        public event EventHandler MonitorInitialized;

        #region Fields
        //Keep de dialog here.
        private LocalOrRemoteSMT _localOrRemoteSMT = new LocalOrRemoteSMT();

        //Point the solution component to here.
        private Monitor _monitor;
        //Check if this is changed --> matching the counters.
        private MonitorSource _previousMonitorSourceForParameters;
        //Filter the counters again if this changed. (combined using ";")
        private string _previousFilter;
        //Keep this here for enabling and disabling the Gui.
        private IMonitorProxy _monitorProxy;

        //Getting all the counters.
        private delegate void WDYHDel();
        private WDYHDel _wdyhDel;
        private Dictionary<Parameter, object> _parametersWithValues = new Dictionary<Parameter, object>();

        //
        private Stopwatch _swUpdatesIn;

        //Results are saved in this directory if you choose to.
        private string _batchSaveResultsDir = Path.Combine(Application.StartupPath, "BatchSavedResults");

        /// <summary>
        /// Show the label control in the first panel.
        /// </summary>
        private bool _showLabelControl = true;
        #endregion

        #region Properties
        /// <summary>
        /// Show the label control in the first panel.
        /// </summary>
        public bool ShowLabelControl
        {
            get { return _showLabelControl; }
            set
            {
                _showLabelControl = value;
                foreach (SolutionComponentCommonPropertyControl ctrl in propertyPanel.SolutionComponentPropertyControls)
                    if (ctrl.Label == "Label:")
                    {
                        ctrl.Visible = _showLabelControl;
                        break;
                    }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Designer time only
        /// </summary>
        public MonitorViewCopy()
        {
            InitializeComponent();
        }
        public MonitorViewCopy(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            InitializeComponent();

            _monitor = solutionComponent as Monitor;

            _wdyhDel = new WDYHDel(__WDYH);

            if (this.IsHandleCreated)
                SetGuiAndConnectToSMT();
            else
                this.HandleCreated += new System.EventHandler(MonitorView_HandleCreated);
        }
        #endregion

        #region Functions

        #region Private
        private void _monitorProxy_OnUnhandledException(object sender, ErrorEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                Stop();
                MessageBox.Show("An error has occured while monitoring:\n" + e.GetException(), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }
        private void _monitorProxy_OnMonitor(object sender, OnMonitorEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                try
                {
                    if (lblCountDown.Text.StartsWith("Determining Countdown: "))
                        lblCountDown.ForeColor = Color.SteelBlue;
                    lblCountDown.Tag = _swUpdatesIn.Elapsed.Seconds;
                    lblCountDown.Text = "Updates in " + lblCountDown.Tag;
                    _swUpdatesIn = Stopwatch.StartNew();

                    dgvLiveMonitoring.AddMonitorValues(e.MonitorValues);

                    ExtendedSchedule schedule = btnSchedule.Tag as ExtendedSchedule;
                    if (schedule != null && schedule.Duration.Ticks != 0)
                    {
                        DateTime endsAt = schedule.ScheduledAt + schedule.Duration;
                        if (endsAt <= DateTime.Now)
                            Stop();
                    }
                }
                catch { }
            });
        }

        #region Init
        private void MonitorView_HandleCreated(object sender, System.EventArgs e)
        {
            this.HandleCreated -= MonitorView_HandleCreated;
            SetGuiAndConnectToSMT();
            //Use this for filtering the counters.
            SolutionComponent.SolutionComponentChanged += new System.EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);
        }
        /// <summary>
        /// Sets the Gui and connects to smt.
        /// </summary>
        private void SetGuiAndConnectToSMT()
        {
            Text = SolutionComponent.ToString();
            string ip = _localOrRemoteSMT.IP;
            btnLocalOrRemoteSMT.Text = (ip == "127.0.0.1") ? "SMT: <local>" : "SMT: Remote at " + ip;
            ShowLabelControl = _showLabelControl;

            if (SynchronizationContextWrapper.SynchronizationContext == null)
                SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;

            Exception exception = ConnectToSMT(ip);
            propertyPanel.SolutionComponent = _monitor;
            SetFilterTextBox();

            Parameter[] parameters = _monitorProxy.GetParameters(_monitor.MonitorSource.Source, out exception);
            SetParameters(parameters);

            _previousMonitorSourceForParameters = _monitor.MonitorSource;
            _previousFilter = _monitor.Filter.Combine(";");

            if (exception == null)
                ConnectAndGetCounters();

            if (exception != null)
                MessageBox.Show("Could not connect to the monitor client.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>
        /// If _monitor changed then the Gui needs to be appended --> MonitorSource and Filter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private Exception ConnectToSMT(string ip)
        {
            Exception exception = null;

            try
            {
                if (_monitorProxy == null)
                {
                    _monitorProxy = CreateMonitorProxy(ip);
                    _monitorProxy.OnUnhandledException += new EventHandler<ErrorEventArgs>(_monitorProxy_OnUnhandledException);
                    _monitorProxy.OnMonitor += new EventHandler<OnMonitorEventArgs>(_monitorProxy_OnMonitor);
                }
                _monitorProxy.ConnectSMT(out exception, ip);
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
        /// <summary>
        /// Creates an instance using reflection.
        /// </summary>
        /// <param name="ip">If 127.0.0.1 the local one will be used, otherwise the remote one.</param>
        /// <returns></returns>
        private IMonitorProxy CreateMonitorProxy(string ip)
        {
            string assemblyName = (ip == "127.0.0.1") ? "vApusSMT.Proxy.Local" : "vApusSMT.Proxy.Remote";
            Assembly ass = Assembly.LoadFrom(Path.Combine(Application.StartupPath, assemblyName) + ".dll");
            Type t = ass.GetType(assemblyName + ".MonitorProxy");
            return Activator.CreateInstance(t) as IMonitorProxy;
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            if (sender == _monitor && this.IsHandleCreated)
            {
                if (_monitor.MonitorSource != _previousMonitorSourceForParameters)
                {
                    _previousMonitorSourceForParameters = _monitor.MonitorSource;

                    Exception exception;
                    Parameter[] parameters = _monitorProxy.GetParameters(_monitor.MonitorSource.Source, out exception);
                    SetParameters(parameters);
                }
                if (_monitor.MonitorSourceIndex == _monitor.PreviousMonitorSourceIndexForCounters || lvwEntities.Items.Count == 0)
                {
                    pnlEntitiesAndCounters.Enabled = true;
                    lblMonitorSourceMismatch.Visible = false;

                    btnMonitorReady.Enabled = btnStart.Enabled = btnSchedule.Enabled = _monitor.Wiw.Count != 0;
                }
                else
                {
                    pnlEntitiesAndCounters.Enabled = false;
                    lblMonitorSourceMismatch.Visible = true;

                    btnMonitorReady.Enabled = btnStart.Enabled = btnSchedule.Enabled = false;
                }
                //Filter the treenodes again if this is changed.
                string filter = _monitor.Filter.Combine(";");
                if (filter != _previousFilter)
                {
                    _previousFilter = filter;
                    FillCounters(lvwEntities, tvwCounters);
                }
            }
        }
        #endregion

        #region Fill tvwCounters
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lvw">The selected item from this list view is used</param>
        /// <param name="tvw">Counters are put in this treeview</param>
        private void FillCounters(ListView lvw, TreeView tvw)
        {
            if (lvw.SelectedItems.Count != 0)
            {
                LockWindowUpdate(this.Handle.ToInt32());
                tvw.SuspendLayout();

                tvw.Nodes.Clear();
                tvw.AfterCheck -= tvwCounter_AfterCheck;

                ListViewItem selected = lvw.SelectedItems[0];
                ParseTag(selected, tvw);

                tvw.Nodes.AddRange(FilterCounters(_monitor.Filter, selected.Tag as TreeNode[]));
                tvw.AfterCheck += tvwCounter_AfterCheck;

                tvw.ResumeLayout();
                LockWindowUpdate(0);
            }
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
        private void ParseTag(ListViewItem item, TreeView tvw)
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
                    for (int j = tvw.Nodes.Count - 1; j != -1; j--)
                    {
                        var node = tvw.Nodes[j];
                        if (node.Text == counter)
                        {
                            counterNode = node;
                            break;
                        }
                    }

                    if (counterNode == null)
                    {
                        counterNode = new TreeNode(counter);
                        newTag[i] = counterNode;
                    }

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

        private void btnFilter_Click(object sender, EventArgs e)
        {
            FromTextDialog ftd = new FromTextDialog();
            ftd.SetText(_monitor.Filter.Combine("\n"));
            if (ftd.ShowDialog() == DialogResult.OK)
            {
                _monitor.Filter = ftd.Entries;
                SetFilterTextBox();

                _monitor.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }
        private void SetFilterTextBox()
        {
            txtFilter.Text = _monitor.Filter.Combine(";");
        }
        #endregion

        private void btnGetCounters_Click(object sender, EventArgs e)
        {
            ConnectAndGetCounters();
        }
        private void ConnectAndGetCounters()
        {
            this.Cursor = Cursors.WaitCursor;

            _monitor.PreviousMonitorSourceIndexForCounters = _monitor.MonitorSourceIndex;

            btnGetCounters.Enabled = false;
            propertyPanel.Lock();
            parameterPanel.Lock();
            btnMonitorReady.Enabled = false;
            pnlEntitiesAndCounters.Enabled = false;

            _monitor.Wiw.Clear();
            tvwCounters.Nodes.Clear();
            tvwCountersInGui.Nodes.Clear();
            lvwEntities.Items.Clear();

            btnStart.Enabled = false;
            btnSchedule.Enabled = false;
            btnLocalOrRemoteSMT.Enabled = false;

            btnGetCounters.Text = "Getting Counters...";
            StaticActiveObjectWrapper.ActiveObject.Send(_wdyhDel);
        }
        private void __WDYH()
        {
            Exception exception = null;
            _monitorProxy.ConnectSMT(out exception, _localOrRemoteSMT.IP);

            Dictionary<Entity, List<CounterInfo>> entitiesAndCounters = null;
            string configuration = null;

            if (exception == null)
            {
                //Take encryption into account
                object[] parameters = new object[_parametersWithValues.Count];
                int i = 0;
                foreach (Parameter key in _parametersWithValues.Keys)
                {
                    object value = _parametersWithValues[key];
                    if (key.Encrypted && value is string)
                        value = (value as string).Encrypt("{A84E447C-3734-4afd-B383-149A7CC68A32}", new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    parameters[i++] = value;
                }
                entitiesAndCounters = _monitorProxy.ConnectToMonitorSource(_monitor.MonitorSource.Source, out exception, parameters);
            }
            if (exception == null)
                configuration = _monitorProxy.GetConfiguration(out exception);
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                if (exception == null)
                {
                    btnConfiguration.Enabled = (configuration != null);
                    btnConfiguration.Tag = configuration;
                    try
                    {
                        FillEntities(entitiesAndCounters);
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
                    MessageBox.Show("Entities and counters could not be retreived!\nHave you filled in the right credentials?", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                pnlEntitiesAndCounters.Enabled = true;
                btnGetCounters.Enabled = true;
                btnLocalOrRemoteSMT.Enabled = true;
                propertyPanel.Unlock();
                parameterPanel.Unlock();

                this.Cursor = Cursors.Default;
            });
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

                lvwEntities.Items.Add(lvwi);
                lvwi.Checked = false;
            }
            if (lvwEntities.Items.Count == 0)
                pnlEntitiesAndCounters.Enabled = false;
            else
                pnlEntitiesAndCounters.Enabled = true;

            if (lvwEntities.SelectedItems.Count == 0)
                lvwEntities.Items[0].Selected = true;
        }

        private void lvwEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            var lvw = sender as ListView;
            if (lvw.SelectedItems.Count != 0)
            {
                lvw.Tag = lvw.SelectedItems[0];
                if (sender == lvwEntities)
                    FillCounters(lvwEntities, tvwCounters);
                else
                    FillCounters(lvwEntitiesInGui, tvwCountersInGui);
            }
        }
        private void lvwEntities_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            e.Item.Selected = true;
            if (sender == lvwEntities)
            {
                ExtractWIWForListViewAction(tpCounters, lvwEntities, tvwCounters, _monitor.Wiw);
                SetCountersAndEntitiesInGUI();
            }
            else
            {
                ExtractWIWForListViewAction(tpCountersInGui, lvwEntitiesInGui, tvwCountersInGui, _monitor.WiwInGui);
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

                node.Nodes.AddRange(node.Tag as TreeNode[]);
                node.Tag = null;

                LockWindowUpdate(0);
            }
        }
        private void tvwCounter_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (sender == tvwCounters)
            {
                ExtractWIWForTreeViewAction(tpCounters, lvwEntities, tvwCounters, _monitor.Wiw, e.Node);
                SetCountersAndEntitiesInGUI();
            }
            else
            {
                ExtractWIWForTreeViewAction(tpCountersInGui, lvwEntitiesInGui, tvwCountersInGui, _monitor.WiwInGui, e.Node);
            }
        }
        private void ExtractWIWForListViewAction(TabPage tp, ListView lvw, TreeView tvw, Dictionary<string, List<CounterInfo>> wiw)
        {
            tvw.AfterCheck -= tvwCounter_AfterCheck;
            var selected = lvw.Tag as ListViewItem;
            string entityName = selected.SubItems[1].Text;
            if (selected.Checked)
                foreach (TreeNode counterNode in tvw.Nodes)
                {
                    if (counterNode.Tag != null)
                    {
                        LockWindowUpdate(this.Handle.ToInt32());

                        counterNode.Nodes.AddRange(counterNode.Tag as TreeNode[]);
                        counterNode.Tag = null;

                        LockWindowUpdate(0);
                    }

                    counterNode.Checked = true;
                    foreach (TreeNode node in counterNode.Nodes)
                        node.Checked = true;
                    ExtractWIW(lvw, tvw, wiw, counterNode);
                }
            else
                foreach (TreeNode counterNode in tvw.Nodes)
                {
                    counterNode.Checked = false;
                    foreach (TreeNode node in counterNode.Nodes)
                        node.Checked = false;
                    ExtractWIW(lvw, tvw, wiw, counterNode);
                }

            tvw.AfterCheck += tvwCounter_AfterCheck;

            SetChosenCountersInTabPage(tp, lvw, wiw);

            btnMonitorReady.Enabled = btnStart.Enabled = btnSchedule.Enabled = _monitor.Wiw.Count != 0;
        }
        private void SetChosenCountersInTabPage(TabPage tp, ListView lvw, Dictionary<string, List<CounterInfo>> wiw)
        {
            var selected = lvw.Tag as ListViewItem;
            if (selected != null)
            {
                string entityName = selected.SubItems[1].Text;

                //Set the counters tab page title
                int totalCount = 0;
                foreach (string key in wiw.Keys)
                {
                    var l = wiw[key];
                    int count = GetTotalCountOfCounters(l);
                    foreach (ListViewItem lvwi in lvw.Items)
                        if (lvwi.SubItems[1].Text == entityName)
                        {
                            lvwi.SubItems[2].Text = "[" + count + "]";
                            break;
                        }
                    totalCount += count;
                }
                if (_monitor.Wiw.Count == 0)
                    foreach (ListViewItem lvwi in lvw.Items)
                        if (lvwi.SubItems[1].Text == entityName)
                            lvwi.SubItems[2].Text = "[0]";

                tp.Text = tp.Text.Split('[')[0] + "[" + totalCount + "]";
            }
        }
        private void ExtractWIWForTreeViewAction(TabPage tp, ListView lvw, TreeView tvw, Dictionary<string, List<CounterInfo>> wiw, TreeNode counterNode)
        {
            tvw.AfterCheck -= tvwCounter_AfterCheck;
            if (counterNode.Level == 0)
            {
                if (counterNode.Tag != null)
                {
                    LockWindowUpdate(this.Handle.ToInt32());

                    counterNode.Nodes.AddRange(counterNode.Tag as TreeNode[]);
                    counterNode.Tag = null;

                    LockWindowUpdate(0);
                }
                foreach (TreeNode node in counterNode.Nodes)
                    node.Checked = counterNode.Checked;
                ExtractWIW(lvw, tvw, wiw, counterNode);
            }
            else
            {
                counterNode.Parent.Checked = true;
                ExtractWIW(lvw, tvw, wiw, counterNode.Parent);
            }
            tvw.AfterCheck += tvwCounter_AfterCheck;

            SetChosenCountersInTabPage(tp, lvw, wiw);

            btnMonitorReady.Enabled = btnStart.Enabled = btnSchedule.Enabled = _monitor.Wiw.Count != 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterNode">Add to or remove this from WiW depending on the checkstate of the node and its children</param>
        private void ExtractWIW(ListView lvw, TreeView tvw, Dictionary<string, List<CounterInfo>> wiw, TreeNode counterNode)
        {
            ListViewItem entity = lvw.Tag as ListViewItem;
            string entityName = entity.SubItems[1].Text;

            lvw.ItemChecked -= lvwEntities_ItemChecked;
            entity.Checked = false;
            foreach (TreeNode node in tvw.Nodes)
                if (node.Checked)
                {
                    entity.Checked = true;
                    break;
                }
            lvw.ItemChecked += lvwEntities_ItemChecked;

            if (entity.Checked)
                if (wiw.ContainsKey(entityName))
                {
                    foreach (CounterInfo info in wiw[entityName])
                        if (info.Counter == counterNode.Text)
                        {
                            wiw[entityName].Remove(info);
                            break;
                        }
                    if (counterNode.Checked)
                    {
                        CounterInfo newCounterInfo = new CounterInfo(counterNode.Text);
                        foreach (TreeNode node in counterNode.Nodes)
                            if (node.Checked)
                                newCounterInfo.Instances.Add(node.Text);

                        wiw[entityName].Add(newCounterInfo);
                    }
                }
                else
                {
                    List<CounterInfo> counters = new List<CounterInfo>();
                    CounterInfo newCounterInfo = new CounterInfo(counterNode.Text);
                    foreach (TreeNode node in counterNode.Nodes)
                        if (node.Checked)
                            newCounterInfo.Instances.Add(node.Text);

                    counters.Add(newCounterInfo);
                    wiw.Add(entityName, counters);
                }
            else
                wiw.Remove(entityName);
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

        /// <summary>
        /// Set what you want to see in the gui, this uses recycling of existing items.
        /// </summary>
        private void SetCountersAndEntitiesInGUI()
        {
            tpCountersInGui.Text = "Live Monitoring [0]";
            lvwEntitiesInGui.Items.Clear();
            lvwEntitiesInGui.Tag = null;
            tvwCountersInGui.Nodes.Clear();

            foreach (string entity in _monitor.Wiw.Keys)
            {
                ListViewItem lvwi = new ListViewItem(string.Empty);

                lvwi.SubItems.Add(entity);
                lvwi.SubItems.Add("[0]");
                lvwi.StateImageIndex = lvwi.ImageIndex;
                lvwi.Tag = _monitor.Wiw[entity];
                lvwi.Checked = false;

                lvwEntitiesInGui.Items.Add(lvwi);
            }

            if (lvwEntitiesInGui.Items.Count != 0)
                lvwEntitiesInGui.Items[0].Selected = true;
        }
        private ListViewItem GetItemWithText(string subItemText, int subItemIndex, ListView lvw)
        {
            foreach (ListViewItem item in lvw.Items)
                if (item.SubItems[subItemIndex].Text == subItemText)
                    return item;
            return null;
        }

        private void btnConfiguration_Click(object sender, EventArgs e)
        {
            if (btnConfiguration.Tag != null)
                (new ConfigurationDialog(btnConfiguration.Tag as string)).ShowDialog();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (dgvLiveMonitoring.RowCount == 0 ||
               (dgvLiveMonitoring.RowCount > 0 &&
               MessageBox.Show("Are you sure you want to start a new monitor?\nThis will clear the previous measured performance counters.", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes))
            {
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
            if (lblCountDown.Text.StartsWith("Determining Countdown: "))
            {
                lblCountDown.Tag = _swUpdatesIn.Elapsed.Seconds;
                lblCountDown.Text = "Determining Countdown: " + lblCountDown.Tag;
            }
            else
            {
                int countDown = (int)lblCountDown.Tag - 1;
                if (countDown > -1)
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
        private void btnMonitorReady_Click(object sender, EventArgs e)
        {
            btnMonitorReady.Visible = false;
            if (MonitorInitialized != null)
                MonitorInitialized(this, null);
        }

        private void btnLocalOrRemoteSMT_Click(object sender, EventArgs e)
        {
            //Set the Gui again, this will connect to smt.
            if (_localOrRemoteSMT.ShowDialog() == DialogResult.OK)
            {
                if (_monitorProxy != null)
                    _monitorProxy.Dispose();
                _monitorProxy = null;

                btnMonitorReady.Enabled = false;

                _monitor.Wiw.Clear();
                tvwCounters.Nodes.Clear();
                lvwEntities.Items.Clear();

                btnStart.Enabled = false;
                btnSchedule.Enabled = false;
                btnConfiguration.Enabled = false;

                int _monitorSourceIndex = _monitor.MonitorSourceIndex;
                SetGuiAndConnectToSMT();
                try
                {
                    _monitor.MonitorSource = _monitor._monitorSources[_monitorSourceIndex];
                }
                catch { }

                LockWindowUpdate(this.Handle.ToInt32());
                propertyPanel.SolutionComponent = null;
                propertyPanel.SolutionComponent = _monitor;

                ShowLabelControl = _showLabelControl;
                LockWindowUpdate(0);
            }
        }
        #endregion

        #region Public
        /// <summary>
        /// Will stop the previous one first.
        /// </summary>
        public void InitializeForStresstest()
        {
            LockWindowUpdate(this.Handle.ToInt32());

            Stop();

            btnStart.Visible = btnSchedule.Visible = btnStop.Visible = false;
            tc.SelectedIndex = 0;

            btnMonitorReady.Enabled = false;
            btnMonitorReady.Visible = true;
            foreach (TreeNode node in tvwCounters.Nodes)
                if (node.Checked)
                {
                    btnMonitorReady.Enabled = true;
                    break;
                }

            LockWindowUpdate(0);
        }
        public void Start()
        {
            pnlEntitiesAndCounters.Enabled = false;
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
            _monitorProxy.ConnectSMT(out exception, _localOrRemoteSMT.IP);
            _monitorProxy.StartMonitoring(_monitor.Wiw, out exception);

            if (exception == null)
            {
                dgvLiveMonitoring.ClearMonitorValues();
                _swUpdatesIn = Stopwatch.StartNew();

                lblCountDown.ForeColor = Color.DarkGray;
                lblCountDown.BackColor = Color.Transparent;
                lblCountDown.Text = "Determining Countdown: ";
                lblCountDown.Visible = true;

                tmrProgressDelayCountDown.Start();
                tmrBatchSaveResults.Start();

                tc.SelectedIndex = 1;
            }
            else
            {
                Stop();
                MessageBox.Show("Entities and counters could not be retreived!\nHave you filled in the right credentials?", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        private void tmrBatchSaveResults_Tick(object sender, EventArgs e)
        {
            if (_monitor.BatchResultSaving)
                BatchSaveResults();
        }
        private void BatchSaveResults()
        {
            string fileName = Path.Combine(_batchSaveResultsDir, "PID_" + Process.GetCurrentProcess().Id.ToString() + "_" + _monitor.ToString().Replace(' ', '_').ReplaceInvalidWindowsFilenameChars('_') + ".csv");
        RetryDir:
            if (!Directory.Exists(_batchSaveResultsDir))
                try
                {
                    Directory.CreateDirectory(_batchSaveResultsDir);
                }
                catch
                {
                    goto RetryDir;
                }

        RetryFile:
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);

                dgvLiveMonitoring.Save(fileName);
            }
            catch
            {
                goto RetryFile;
            }
        }
        public void Stop()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (_monitorProxy != null)
                    try
                    {
                        _monitorProxy.StopMonitoring();
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

                if (_swUpdatesIn != null)
                    _swUpdatesIn.Stop();

                _swUpdatesIn = null;
                tmrProgressDelayCountDown.Stop();
                tmrBatchSaveResults.Stop();

                pnlEntitiesAndCounters.Enabled = true;
                btnGetCounters.Enabled = true;
                propertyPanel.Unlock();
                parameterPanel.Unlock();

                btnStart.Enabled = true;
                btnSchedule.Enabled = true;
                btnStop.Enabled = false;
                btnLocalOrRemoteSMT.Enabled = true;

                lblCountDown.ForeColor = Color.DarkOrange;
                lblCountDown.BackColor = SystemColors.Control;
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
            return dgvLiveMonitoring.GetHeaders();
        }
        public Dictionary<DateTime, float[]> GetMonitorValues()
        {
            return dgvLiveMonitoring.GetMonitorValues();
        }
        #endregion

        #endregion
    }
}
