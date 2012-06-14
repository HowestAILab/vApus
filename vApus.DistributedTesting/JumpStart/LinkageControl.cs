/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public partial class LinkageControl : UserControl
    {
        #region events
        public event EventHandler CollapsedChanged;
        public event EventHandler StateChanged;
        #endregion

        public enum State
        {
            Offline = 0,
            OnlineComputer,
            OnlineSlave
        }

        #region Fields
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinkageControl));

        private object _lock = new object();

        private Color _offline = Color.Red;
        private Color _onlineComputer = Color.Blue;
        private Color _onlineSlave = Color.Green;

        private State _state;
        private string _slaveIP;
        private int _slavePort;

        /// <summary>
        /// To be able to kill the process. 
        /// You can set this manually if you like, be carefull.
        /// </summary>
        public int SlaveProcessID = -1;

        private int _expandedHeight = -1;

        #endregion

        #region Properties
        public string Title
        {
            get { return lbl.Text; }
        }
        public string Label
        {
            get { return rtxt.Text; }
            set { rtxt.Text = value; }
        }
        public bool Collapsed
        {
            get { return split.Panel2Collapsed; }
            set
            {
                if (split.Panel2Collapsed != value)
                {
                    split.Panel2Collapsed = value;
                    if (split.Panel2Collapsed)
                    {
                        btnCollapseExpand.Text = "+";
                        _expandedHeight = this.Height;
                        this.Height = 65;
                    }
                    else
                    {
                        btnCollapseExpand.Text = "-";
                        if (_expandedHeight != -1)
                            this.Height = _expandedHeight;
                    }

                    if (CollapsedChanged != null)
                        CollapsedChanged(this, null);
                }
            }

        }
        public State __State
        {
            get { return _state; }
        }
        #endregion

        public LinkageControl(string slaveIP = "", int slavePort = -1)
        {
            InitializeComponent();

            lbl.Text = slaveIP + ':' + slavePort;
            _slaveIP = slaveIP;
            _slavePort = slavePort;
        }

        private void JumpStartOrKill_Done(object sender, EventArgs e)
        {
            JumpStartOrKill.Done -= JumpStartOrKill_Done;
            CheckForSlaves();
        }
        /// <summary>
        /// Re-enable if refresh needed
        /// </summary>
        public void Unlock()
        {
            btnJumpStart.Enabled = true;
        }
        private void SetState(State state)
        {
            _state = state;
            switch (_state)
            {
                case State.Offline:
                    lbl.ForeColor = _offline;
                    btnJumpStart.Text = "Jump Start";
                    btnJumpStart.Image = ((System.Drawing.Image)(resources.GetObject("btnJumpStart.Image")));
                    btnJumpStart.Enabled = false;
                    break;
                case State.OnlineComputer:
                    lbl.ForeColor = _onlineComputer;
                    btnJumpStart.Text = "Jump Start";
                    btnJumpStart.Image = ((System.Drawing.Image)(resources.GetObject("btnJumpStart.Image")));
                    btnJumpStart.Enabled = true;
                    break;
                case State.OnlineSlave:
                    lbl.ForeColor = _onlineSlave;
                    if (SlaveProcessID == -1)
                    {
                        btnJumpStart.Text = "Jump Start";
                        btnJumpStart.Image = ((System.Drawing.Image)(resources.GetObject("btnJumpStart.Image")));
                        btnJumpStart.Enabled = false;
                    }
                    else
                    {
                        btnJumpStart.Text = "Kill Slave";
                        btnJumpStart.Image = ((System.Drawing.Image)(resources.GetObject("btnKill.Image")));
                        btnJumpStart.Enabled = true;
                    }
                    break;
            }
            if (StateChanged != null)
                StateChanged(this, null);
        }
        /// <summary>
        /// Checks for slaves on another thread.
        /// </summary>
        public void CheckForSlaves()
        {
            lblMessage.Visible = false;
            ThreadPool.QueueUserWorkItem(PingComputer, null);
        }
        private void PingComputer(object state)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                btnJumpStart.Enabled = false;
                lblProgress.ForeColor = _onlineComputer;
                lblProgress.Text = "Pinging...";
                lblProgress.Visible = true;
            });

            Ping p = new Ping();
            p.PingCompleted += new PingCompletedEventHandler(p_PingCompleted);

            try
            {
                p.SendAsync(_slaveIP, null);
            }
            catch
            {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    SetState(LinkageControl.State.Offline);
                    lblProgress.Visible = false;
                });
            }
        }

        private void p_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                PollvApus();
            }
            else
            {
                SlaveProcessID = -1;
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    SetState(LinkageControl.State.Offline);
                    lblProgress.Visible = false;
                });
            }
        }

        private void PollvApus()
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                btnJumpStart.Enabled = false;
                lblProgress.ForeColor = _onlineSlave;
                lblProgress.Text = "Polling vApus...";
                lblProgress.Visible = true;
            });

            ThreadPool.QueueUserWorkItem(delegate
            {
                if (!this.IsDisposed)
                {
                    //Exception exception;
                    //int id;

                    //MasterSideCommunicationHandler masterCommunication = new MasterSideCommunicationHandler();
                    //masterCommunication.ConnectSlave(_slaveIP, _slavePort, out id, out exception);
                    //SlaveProcessID = id;

                    //SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                    //{
                    //    if (exception == null)
                    //        SetState(LinkageControl.State.OnlineSlave);
                    //    else
                    //        SetState(LinkageControl.State.OnlineComputer);

                    //    lblProgress.Visible = false;
                    //});
                }
            });
        }

        private void btnJumpStartKill_Click(object sender, System.EventArgs e)
        {
            JumpStartOrKill.Done += new EventHandler(JumpStartOrKill_Done);

            if (SlaveProcessID == -1)
                RegisterForJumpStart();
            else
                RegisterForKill();

            JumpStartOrKill.Do();
        }
        /// <summary>
        /// Jump starts via a socket connection with vApus.JumpStart.exe.
        /// </summary>
        public void RegisterForJumpStart()
        {
            lock (_lock)
            {
                btnJumpStart.Enabled = false;

                lblProgress.ForeColor = _onlineSlave;
                lblProgress.Text = "Busy...";
                lblProgress.Visible = true;

                lblMessage.Visible = false;

                JumpStartOrKill.RegisterForJumpStart(_slaveIP, _slavePort);
            }
        }
        /// <summary>
        /// Kills via a socket connection with vApus.JumpStart.exe.
        /// </summary>
        /// <param name="useProcessID">If false, all instances on the machine will be killed if possible.</param>
        /// <returns></returns>
        public void RegisterForKill(bool useProcessID = true)
        {
            lock (_lock)
            {
                btnJumpStart.Enabled = false;

                if (useProcessID && SlaveProcessID == -1)
                {
                    btnJumpStart.Enabled = true;
                    return;
                }

                lblProgress.ForeColor = _onlineComputer;
                lblProgress.Text = "Busy...";
                lblProgress.Visible = true;


                lblMessage.Visible = false;

                JumpStartOrKill.RegisterForKill(_slaveIP, useProcessID ? SlaveProcessID : -1);
            }
        }

        private void btnCollapseExpand_Click(object sender, EventArgs e)
        {
            Collapsed = btnCollapseExpand.Text == "-";
        }
    }
}
