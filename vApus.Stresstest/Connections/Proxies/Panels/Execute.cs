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
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest
{
    public partial class Execute : UserControl
    {
        #region Fields
        public Compile Compile;
        private ConnectionProxyCode _connectionProxyCode;
        private ConnectionProxyPool _connectionProxyPool;
        private Dictionary<string, List<TreeNode>> _dict = new Dictionary<string, List<TreeNode>>();

        private int _processed;
        #endregion
        
        [ReadOnly(true)]
        public ConnectionProxyCode ConnectionProxyCode
        {
            get { return _connectionProxyCode; }
            set
            {
                ruleSetSyntaxItemPanel.InputChanged -= ruleSetSyntaxItemPanel_InputChanged;
                _connectionProxyCode = value;

                nudThreads.Value = _connectionProxyCode.Threads;
                nudThreads.ValueChanged += new EventHandler(nudThreads_ValueChanged);

                flp.Controls.Clear();
                flp.Controls.Add(new SolutionComponentCommonPropertyControl(_connectionProxyCode, _connectionProxyCode.GetType().GetProperty("TestLog")));
                flp.Controls.Add(new SolutionComponentCommonPropertyControl(_connectionProxyCode, _connectionProxyCode.GetType().GetProperty("TestLogEntryIndex")));

                ruleSetSyntaxItemPanel.SetRuleSetAndInput(_connectionProxyCode.ConnectionProxyRuleSet, _connectionProxyCode.TestConnectionString);
                ruleSetSyntaxItemPanel.InputChanged += ruleSetSyntaxItemPanel_InputChanged;
            }
        }
        private void ruleSetSyntaxItemPanel_InputChanged(object sender, EventArgs e)
        {
            _connectionProxyCode.TestConnectionString = ruleSetSyntaxItemPanel.Input;
        }
        public Execute()
        {
            InitializeComponent();

        }

        private void flp_Layout(object sender, LayoutEventArgs e)
        {
            foreach (Control control in flp.Controls)
                control.Width = flp.Width - 18;
        }
        private void btnPlay_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            _connectionProxyPool = Compile.TryCompile(true, false);
            this.Cursor = Cursors.Default;

            if (_connectionProxyPool != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(_connectionProxyCode.TestConnectionString))
                        throw new Exception("Please fill in the connection!");
                    else if (_connectionProxyCode.TestLog.IsEmpty)
                        throw new Exception("Please fill in the log!");
                    else
                    {
                        _connectionProxyPool.LogEntryTested += new EventHandler<ConnectionProxyPool.LogEntryTestedEventArgs>(connectionProxyPool_LogEntryTested);
                        _connectionProxyPool.TestWorkFinished += new EventHandler(connectionProxyPool_TestWorkFinished);
                        _connectionProxyPool.TestWorkException += new EventHandler<ConnectionProxyPool.TestWorkExceptionEventArgs>(_connectionProxyPool_TestWorkException);
                        _connectionProxyPool.TestCode(_connectionProxyCode.TestLog, _connectionProxyCode.TestLogEntryIndex, _connectionProxyCode.Threads);
                        SetGui(true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not test the code:\n" + ex.ToString(), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void SetGui(bool toPlayMode)
        {
            if (toPlayMode)
            {
                nudThreads.Enabled = false;
                btnPlay.Enabled = false;
                btnStop.Enabled = true;
                split.Enabled = false;
                tvw.Nodes.Clear();
                rtxtSelectedNode.Text = string.Empty;
                cboThread.Items.Clear();
                _dict.Clear();

                _processed = 0;
                pb.Value = 0;
                int count = 0;
                if (_connectionProxyCode.TestLogEntryIndex == -1)
                    foreach (BaseItem item in _connectionProxyCode.TestLog)
                        count += item is UserAction ? item.Count : 1;
                else
                    count = 1;

                pb.Maximum = count * _connectionProxyCode.Threads;

                lblProgress.Text = "0 / " + pb.Maximum;

                timer.Start();
            }
            else
            {
                nudThreads.Enabled = true;
                btnPlay.Enabled = true;
                btnStop.Enabled = false;
                split.Enabled = true;

                timer.Stop();

                RefreshGui();
            }
        }
        private void _connectionProxyPool_TestWorkException(object sender, ConnectionProxyPool.TestWorkExceptionEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                MessageBox.Show("Could not test the code:\n" + e.Exception.ToString(), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
            SetFinished();
        }
        private void connectionProxyPool_TestWorkFinished(object sender, EventArgs e)
        {
            SetFinished();
        }
        private void SetFinished()
        {
            _connectionProxyPool.TestWorkFinished -= connectionProxyPool_TestWorkFinished;
            _connectionProxyPool.LogEntryTested -= connectionProxyPool_LogEntryTested;
            _connectionProxyPool.TestWorkException -= _connectionProxyPool_TestWorkException;
            try
            {
                _connectionProxyPool.DeleteTempFiles();
            }
            finally
            {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    SetGui(false);
                });
            }
        }
        private void connectionProxyPool_LogEntryTested(object sender, ConnectionProxyPool.LogEntryTestedEventArgs e)
        {
            try
            {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    if (_connectionProxyPool == null)
                        return;
                    TreeNode node = new TreeNode(e.ParameterizedLogEntry.CombineValues());
                    node.Nodes.Add("Sent At: " + e.SentAt);
                    node.Nodes.Add("Time to Last Byte: " + e.TimeToLastByte);
                    if (e.Exception != null)
                        node.Nodes.Add("Exception: " + e.Exception.ToString());

                    if (e.Exception == null)
                    {
                        node.ImageIndex = 1;
                        node.SelectedImageIndex = 1;
                    }
                    else
                    {
                        node.ForeColor = Color.DarkRed;
                        node.ImageIndex = 2;
                        node.SelectedImageIndex = 2;
                    }

                    if (!_dict.ContainsKey(e.Thread))
                    {
                        _dict.Add(e.Thread, new List<TreeNode>());
                        cboThread.Items.Add(e.Thread);
                    }
                    _dict[e.Thread].Add(node);
                    if (cboThread.SelectedIndex == -1)
                        cboThread.SelectedIndex = 0;
                    ++_processed;
                });
            }
            catch { }
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            _connectionProxyPool.Dispose();
            _connectionProxyPool = null;
            SetGui(false);
        }
        private void cboThread_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshGui();
            if (tvw.SelectedNode == null && tvw.Nodes.Count != 0)
                tvw.SelectedNode = tvw.Nodes[0];
            else
                rtxtSelectedNode.Text = string.Empty;
        }
        private void nudThreads_ValueChanged(object sender, EventArgs e)
        {
            _connectionProxyCode.Threads = (int)nudThreads.Value;
        }
        private void tvw_AfterSelect(object sender, TreeViewEventArgs e)
        {
            rtxtSelectedNode.Text = (tvw.SelectedNode == null) ? string.Empty : tvw.SelectedNode.Text;
        }
        private void tvw_DoubleClick(object sender, EventArgs e)
        {
            if (tvw.SelectedNode != null)
                tabControl.SelectedIndex = 1;
        }
        private void RefreshGui()
        {
            if (cboThread.SelectedItem == null)
                return;
            if (tvw.Tag == null || tvw.Tag != cboThread.SelectedItem)
            {
                tvw.Tag = cboThread.SelectedItem;
                tvw.Nodes.Clear();
                tvw.Nodes.AddRange(_dict[cboThread.SelectedItem.ToString()].ToArray());
            }
            else
            {
                TreeNode[] arr = _dict[cboThread.SelectedItem.ToString()].ToArray();
                foreach (TreeNode node in arr)
                    if (!tvw.Nodes.Contains(node))
                        tvw.Nodes.Add(node);
            }
            pb.Value = _processed;
            lblProgress.Text = _processed + " / " + pb.Maximum;
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            RefreshGui();
        }
    }
}
