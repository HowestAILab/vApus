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
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public partial class Resources : Form
    {
        #region Fields
        private SocketListener _socketListener;
        private DistributedTest _distributedTest;
        private int _scanCounter;
        private int _slaveCount;
        #endregion

        public Resources(DistributedTest distributedTest = null)
        {
            InitializeComponent();
            _distributedTest = distributedTest;
            resourceSolutionComponentPropertyPanel.SolutionComponent = _distributedTest;
            _socketListener = SocketListener.GetInstance();
            _socketListener.IPChanged += new EventHandler<IPChangedEventArgs>(SocketListener_IPChanged);
        }

        #region Functions
        private void SocketListener_IPChanged(object sender, IPChangedEventArgs e)
        {
            resourceSolutionComponentPropertyPanel.Refresh();
        }
        private void btnRefreshResourcePool_Click(object sender, EventArgs e)
        {
            RefreshResourcePool();
        }
        private void RefreshResourcePool()
        {
            Cursor = Cursors.WaitCursor;
            btnRefreshResourcePool.Enabled = false;
            pbRefreshResourcePool.Style = ProgressBarStyle.Marquee;
            Text = "Resources";
            _slaveCount = 0;
            tvwResourcePool.Nodes.Clear();
            if (_distributedTest.Network == "127.0.0.")
            {
                _scanCounter = 253;
                MakeComputerNode("127.0.0.1");
            }
            else
            {
                _scanCounter = 0;
                for (int i = 1; i < 255; i++)
                    MakeComputerNode(_distributedTest.Network + i);
            }
        }
        private void MakeComputerNode(string ip)
        {
            TreeNode computerNode = new TreeNode(ip);
            tvwResourcePool.Nodes.Add(computerNode);

            Ping p = new Ping();
            p.PingCompleted += p_PingCompleted;
            p.SendAsync(ip, computerNode);
        }
        private void p_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            if (!this.IsDisposed)
                try
                {
                    TreeNode computerNode = e.UserState as TreeNode;

                    if (e.Reply.Status == IPStatus.Success)
                    {
                        computerNode.ImageIndex = 1;
                        computerNode.SelectedImageIndex = 1;

                        List<object[]> args = new List<object[]>();
                        for (int port = _distributedTest.BeginPort; port <= _distributedTest.EndPort; port++)
                            ThreadPool.QueueUserWorkItem(CheckForSlave, new object[] { computerNode.Text, port, computerNode });
                    }
                    if (++_scanCounter == 254)
                    {
                        Cursor = Cursors.Default;
                        btnRefreshResourcePool.Enabled = true;
                        pbRefreshResourcePool.Style = ProgressBarStyle.Blocks;
                    }
                }
                catch { }
        }

        private void CheckForSlave(object state)
        {
            if (!this.IsDisposed)
            {
                SynchronizationContextWrapper.SynchronizationContext.Post(delegate
                {
                    Cursor = Cursors.WaitCursor;
                    btnRefreshResourcePool.Enabled = false;
                    pbRefreshResourcePool.Style = ProgressBarStyle.Marquee;
                }, null);

                object[] arg = state as object[];
                string ip = arg[0] as string;
                int port = (int)arg[1];
                TreeNode computerNode = arg[2] as TreeNode;

                Exception exception;
                MasterSideCommunicationHandler masterCommunication = new MasterSideCommunicationHandler();
                masterCommunication.ConnectSlave(ip, port, out exception);
                if (exception == null)
                {
                    SynchronizationContextWrapper.SynchronizationContext.Post(delegate
                    {
                        computerNode.NodeFont = new System.Drawing.Font(tvwResourcePool.Font, FontStyle.Bold);
                        //Force reset drawing box.
                        computerNode.Text = computerNode.Text;
                        computerNode.ForeColor = Color.Black;
                        computerNode.ImageIndex = 2;
                        computerNode.SelectedImageIndex = 2;

                        TreeNode slaveNode = new TreeNode(port.ToString());
                        computerNode.Nodes.Add(slaveNode);
                        slaveNode.ForeColor = Color.Black;
                        slaveNode.ImageIndex = 2;
                        slaveNode.SelectedImageIndex = 2;

                        Text = string.Format("Resources ({0} Slave(s))", ++_slaveCount);

                        Cursor = Cursors.Default;
                        btnRefreshResourcePool.Enabled = true;
                        pbRefreshResourcePool.Style = ProgressBarStyle.Blocks;
                    }, null);
                }
                else
                {
                    SynchronizationContextWrapper.SynchronizationContext.Post(delegate
                    {
                        Cursor = Cursors.Default;
                        btnRefreshResourcePool.Enabled = true;
                        pbRefreshResourcePool.Style = ProgressBarStyle.Blocks;
                    }, null);
                }
                masterCommunication.Dispose();
            }
        }
        #endregion
    }
}