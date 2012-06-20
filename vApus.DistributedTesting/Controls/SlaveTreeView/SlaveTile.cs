/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public partial class SlaveTile : UserControl
    {
        #region Events
        public event EventHandler DuplicateClicked;
        public event EventHandler DeleteClicked;
        #endregion

        #region Fields
        private Slave _slave;
        private bool _clientOnline = false;
        private DistributedTestMode _distributedTestMode;
        #endregion

        #region Properties
        public Slave Slave
        {
            get { return _slave; }
        }
        public bool ClientOnline
        {
            get { return _clientOnline; }
        }
        #endregion

        #region Constructor
        public SlaveTile()
        {
            InitializeComponent();
        }
        #endregion

        #region Functions
        public void SetSlave(Slave slave)
        {
            if (_slave != slave)
            {
                _slave = slave;
                //SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);

            }
            SetGui();
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            if (sender == _slave)
                SetGui();
        }
        private void SetGui()
        {
            nudPort.ValueChanged -= nudPort_ValueChanged;
            nudPort.Value = _slave.Port;
            nudPort.ValueChanged += nudPort_ValueChanged;

            string pa = _slave.ProcessorAffinity.Combine(", ");
            llblPA.Text = pa.Length == 0 ? "..." : pa;

            TileStresstest ts = Slave.TileStresstest;
            llblTest.Text = ts == null ? "..." : ts.ToString();
        }

        private void nudPort_ValueChanged(object sender, EventArgs e)
        {
            //Check if the port is not already used. Don't allow duplicates.
            int port = (int)nudPort.Value;
            foreach (Slave slave in _slave.Parent)
                if (slave.Port == port && slave != _slave)
                {
                    MessageBox.Show(this, "Cannot use the same port more than once.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    nudPort.ValueChanged -= nudPort_ValueChanged;
                    nudPort.Value = _slave.Port;
                    nudPort.ValueChanged += nudPort_ValueChanged;

                    return;
                }

            _slave.Port = port;
            _slave.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        private void llblPA_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FromTextDialog ftd = new FromTextDialog();
            ftd.Text = "The one-based indices of the CPU cores, each on a new line.";
            ftd.SetText(_slave.ProcessorAffinity.Combine("\n"));

            if (ftd.ShowDialog() == DialogResult.OK)
            {
                string paToString = ftd.Entries.Combine(", ");
                if (llblPA.Text != paToString)
                {
                    int[] pa = new int[ftd.Entries.Length];
                    if (paToString.Length != 0)
                        for (int i = 0; i != pa.Length; i++)
                        {
                            int index;
                            if (!int.TryParse(ftd.Entries[i], out index))
                                return;

                            if (index == 0)
                                return;

                            pa[i] = index;
                        }

                    llblPA.Text = paToString.Length == 0 ? "..." : paToString;

                    _slave.ProcessorAffinity = pa;
                    _slave.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                }
            }
        }

        private void llblTest_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void picDuplicate_Click(object sender, EventArgs e)
        {
            if (DuplicateClicked != null)
                DuplicateClicked(this, null);
        }

        private void picDelete_Click(object sender, EventArgs e)
        {
            if (DeleteClicked != null)
                DeleteClicked(this, null);
        }
        public void SetClientStatus(bool online)
        {
            _clientOnline = online;
            if (_clientOnline)
            {
                toolTip.SetToolTip(picStatus, "Client Online");
                picStatus.Image = vApus.DistributedTesting.Properties.Resources.OK;
            }
            else
            {
                toolTip.SetToolTip(picStatus, "Client Offline");
                picStatus.Image = vApus.DistributedTesting.Properties.Resources.Cancelled;
            }
        }

        public void SetMode(DistributedTestMode distributedTestMode)
        {
            _distributedTestMode = distributedTestMode;
#warning Flag it or check if it is used in the tests.
            if (_distributedTestMode == DistributedTestMode.Edit)
                if (_slave.TileStresstest == null)
                {
                    this.Visible = true;
                }
                else
                {
                    picDuplicate.Visible =
                    picDelete.Visible =
                    nudPort.Enabled =
                    llblPA.Enabled =
                    llblTest.Enabled = true;
                }
            else
                if (_slave.TileStresstest == null)
                {
                    this.Visible = false;
                }
                else
                {
                    picDuplicate.Visible =
                    picDelete.Visible =
                    nudPort.Enabled =
                    llblPA.Enabled =
                    llblTest.Enabled = false;
                }
        }
        #endregion
    }
}
