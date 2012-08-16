/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public partial class Wizard : Form
    {
        #region Fields
        private DistributedTest _distributedTest;
        private Bitmap _transparentImage = new Bitmap(16, 16);

        [ThreadStatic]
        private static CoreCount _coreCountWorkItem;

        protected Dictionary<string, string> _ipsAndHostNames = new Dictionary<string, string>();
        #endregion

        /// <summary>
        /// Don't forget to call SetDistributedTest(DistributedTest).
        /// </summary>
        public Wizard()
        {
            InitializeComponent();

            Graphics g = Graphics.FromImage(_transparentImage);
            g.FillRectangle(Brushes.Transparent, 0, 0, 16, 16);
            g.Dispose();
        }

        #region Functions
        /// <summary>
        /// Set this, otherwise nothing will happen.
        /// </summary>
        /// <param name="distributedTest"></param>
        public void SetDistributedTest(DistributedTest distributedTest)
        {
            if (_distributedTest != distributedTest)
            {
                _distributedTest = distributedTest;
                SetDistributedTestToGui();
            }
        }
        private void SetDistributedTestToGui()
        {
            SetDefaultTestSettings();
            SetGenerateTiles();
            SetAddClientsAndSlaves();
        }
        private void SetDefaultTestSettings()
        {
            chkUseRDP.Checked = _distributedTest.UseRDP;
            cboRunSync.SelectedIndex = (int)_distributedTest.RunSynchronization;
            lblResultPath.Text = _distributedTest.ResultPath;
        }
        private void SetGenerateTiles()
        {
            nudTiles.Value = _distributedTest.Tiles.Count == 0 ? 1 : 0;
        }
        private void SetAddClientsAndSlaves()
        {
            StresstestProject stresstestProject = SolutionTree.Solution.ActiveSolution.GetProject("StresstestProject") as StresstestProject;
            nudTests.Value = stresstestProject.CountOf(typeof(Stresstest.Stresstest));

            foreach (Client client in _distributedTest.Clients)
                dgvClients.Rows.Add(client.HostName.Length == 0 ? client.IP : client.HostName,
                     client.UserName, client.Domain, client.Password, client.Count, 0);

            RefreshDGV();

            nudTiles.ValueChanged += new EventHandler(this.nudTiles_ValueChanged);
            nudTests.ValueChanged += new EventHandler(this.nudTests_ValueChanged);
            dgvClients.CellEndEdit += new DataGridViewCellEventHandler(dgvClients_CellEndEdit);
            dgvClients.RowsRemoved += new DataGridViewRowsRemovedEventHandler(this.dgvClients_RowsRemoved);
        }

        private void SetCountsInGui()
        {
            int totalNewTestCount = (int)(nudTiles.Value * nudTests.Value);

            int totalTestCount = totalNewTestCount, totalUsedTestCount = totalNewTestCount;

            foreach (Tile tile in _distributedTest.Tiles)
                foreach (TileStresstest ts in tile)
                {
                    ++totalTestCount;
                    if (ts.Use)
                        ++totalUsedTestCount;
                }

            int totalSlaveCount = 0;
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                if (row.Cells[4].Value == row.Cells[4].DefaultNewRowValue)
                    break;

                totalSlaveCount += int.Parse(row.Cells[4].Value.ToString());
            }
            int totalAssignedTestCount = totalSlaveCount <= totalUsedTestCount ? totalSlaveCount : totalUsedTestCount;

            clmSlaves.HeaderText = "Number of Slaves (" + totalSlaveCount + ")";
            clmTests.HeaderText = "Number of Tests (" + totalAssignedTestCount + "/" + totalUsedTestCount + ")";

            int yetToAssingTestCount = totalAssignedTestCount;
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                if (row.Cells[4].Value == row.Cells[4].DefaultNewRowValue)
                    break;

                if (yetToAssingTestCount == 0)
                {
                    row.Cells[5].Value = 0;
                }
                else
                {
                    int slaveCount = int.Parse(row.Cells[4].Value.ToString());

                    int tests = yetToAssingTestCount - slaveCount >= 0 ? slaveCount : yetToAssingTestCount;
                    row.Cells[5].Value = tests;
                    yetToAssingTestCount -= tests;

                }
            }


            lblNotAssignedTests.Text = (totalTestCount - totalAssignedTestCount) + " Tests are not Assigned to a Slave";
            if (totalUsedTestCount != 0)
                lblNotAssignedTests.Text += " whereof " + (totalTestCount - totalUsedTestCount) + " that are not Used (Checked)";

            CleanDictionary();
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            SetGuiToDistributedTest();
            this.Close();
        }
        private void SetGuiToDistributedTest()
        {
            List<Slave> toAssingTestsTo = new List<Slave>(); //easier
            //Add slaves to the distributed test.
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                if (row.Cells[0].Value == row.Cells[0].DefaultNewRowValue)
                    break;

                Client client = new Client();
                _distributedTest.Clients.AddWithoutInvokingEvent(client, false);

                string ipOrHostname = row.Cells[0].Value as string;
                if (_ipsAndHostNames.ContainsKey(ipOrHostname))
                {
                    client.IP = ipOrHostname;
                    client.HostName = _ipsAndHostNames[ipOrHostname];
                }
                else if (_ipsAndHostNames.ContainsValue(ipOrHostname.ToLower()))
                {
                    string ip;
                    if (_ipsAndHostNames.TryGetKey(ipOrHostname, out ip))
                    {
                        client.IP = ip;
                        client.HostName = ipOrHostname;
                    }
                }

                client.UserName = row.Cells[1].Value as string;
                client.Domain = row.Cells[2].Value as string;
                if (row.Tag != null)
                    client.Password = row.Tag as string;

                for (int i = 0; i != ((int)row.Cells[4].Value); i++)
                {
                    Slave slave = new Slave();
                    client.AddWithoutInvokingEvent(slave, false);
                    toAssingTestsTo.Add(slave);
                }
            }

            int k = 0;
            //Add the tests to the distributed test.
            for (int i = 0; i != (int)nudTiles.Value; i++)
            {
                Tile tile = new Tile();
                _distributedTest.Tiles.AddWithoutInvokingEvent(tile, false);

                Stresstest.Stresstest defaultToStresstest = null;
                for (int j = 0; j != (int)nudTests.Value; j++)
                {
                    TileStresstest tileStresstest = new TileStresstest();
                    defaultToStresstest = GetNextDefaultToStresstest(defaultToStresstest);
                    tileStresstest.DefaultSettingsTo = defaultToStresstest;

                    tile.AddWithoutInvokingEvent(tileStresstest, false);

                    if (k < toAssingTestsTo.Count)
                        tileStresstest.BasicTileStresstest.Slaves = new Slave[] { toAssingTestsTo[k] };

                    ++k;
                }
            }
            _distributedTest.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Added, nudTiles.Value > 1);
        }
        private Stresstest.Stresstest GetNextDefaultToStresstest(Stresstest.Stresstest previous)
        {
            SolutionComponent stresstestProject = Solution.ActiveSolution.GetSolutionComponent(typeof(Stresstest.StresstestProject));
            if (previous != null)
            {
                bool previousFound = false;
                foreach (BaseItem item in stresstestProject)
                    if (item is Stresstest.Stresstest)
                        if (item == previous)
                            previousFound = true;
                        else if (previousFound)
                            return item as Stresstest.Stresstest;
            }

            //If next was not found (starts with the first item again if any to use for a previous default to).
            return SolutionComponent.GetNextOrEmptyChild(typeof(Stresstest.Stresstest), stresstestProject) as Stresstest.Stresstest;
        }

        private void chkUseRDP_CheckedChanged(object sender, EventArgs e)
        {
            clmUserName.HeaderText = chkUseRDP.Checked ? "* User Name" : "User Name";
            clmPassword.HeaderText = chkUseRDP.Checked ? "* Password" : "Password";
        }

        private void llblResultPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Directory.Exists(_distributedTest.ResultPath))
                folderBrowserDialog.SelectedPath = _distributedTest.ResultPath;
            else
                folderBrowserDialog.SelectedPath = Application.StartupPath;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK && lblResultPath.Text != folderBrowserDialog.SelectedPath)
                lblResultPath.Text = folderBrowserDialog.SelectedPath;
        }

        private void dgvClients_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvClients.Columns[e.ColumnIndex].Name == "clmPassword" && e.Value != null)
            {
                dgvClients.Rows[e.RowIndex].Tag = e.Value;
                e.Value = new String('*', e.Value.ToString().Length);
            }
        }

        private void dgvClients_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvClients.CurrentCell.ColumnIndex == 3 && dgvClients.CurrentRow.Tag != null)
                e.Control.Text = dgvClients.CurrentRow.Tag.ToString();
        }
        #endregion

        private void nudTiles_ValueChanged(object sender, EventArgs e)
        {
            SetCountsInGui();
        }

        private void nudTests_ValueChanged(object sender, EventArgs e)
        {
            SetCountsInGui();
        }

        private void dgvClients_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                if (row.Cells[0].Value == row.Cells[0].DefaultNewRowValue)
                    break;
                if (row != dgvClients.CurrentRow && row.Cells[0].Value.ToString() == dgvClients.CurrentRow.Cells[0].Value.ToString())
                {
                    MessageBox.Show("This client was already added.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvClients.Rows.Remove(dgvClients.CurrentRow);
                    return;
                }
            }
            RefreshDGV();
        }

        private void dgvClients_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            SetCountsInGui();
        }

        private void rdbSlavesPerCores_CheckedChanged(object sender, EventArgs e)
        {
            RefreshDGV();
        }
        private void nudSlavesPerCores_ValueChanged(object sender, EventArgs e)
        {
            if (rdbSlavesPerCores.Checked)
                SetSaveCountsPerCores();
        }
        private void nudSlavesPerClient_ValueChanged(object sender, EventArgs e)
        {
            if (rdbSlavesPerClient.Checked)
                SetSlaveCountsPerClients();
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDGV();
        }
        private void RefreshDGV()
        {
            if (rdbSlavesPerCores.Checked)
                SetSaveCountsPerCores();
            else
                SetSlaveCountsPerClients();
        }
        private void SetSaveCountsPerCores()
        {
            List<int> coreCounts = GetCoreCounts();
            List<int> slaveCounts = new List<int>(dgvClients.RowCount - 1);
            for (int i = 0; i != dgvClients.RowCount - 1; i++)
            {
                int slaveCount = (int)(coreCounts[i] / nudSlavesPerCores.Value);
                slaveCounts.Add(slaveCount);
            }
            SetSlaveCountInDataGridView(slaveCounts);
            SetCoreCountInDataGridView(coreCounts);
            SetCountsInGui();
        }
        private void SetSlaveCountsPerClients()
        {
            List<int> slaveCounts = new List<int>(dgvClients.RowCount - 1);
            for (int i = 0; i != dgvClients.RowCount - 1; i++)
                slaveCounts.Add((int)nudSlavesPerClient.Value);
            SetSlaveCountInDataGridView(slaveCounts);
            SetCountsInGui();
        }
        /// <summary>
        /// Get the count of the cores for each client (-1 if unknown)
        /// </summary>
        /// <returns></returns>
        private List<int> GetCoreCounts()
        {
            this.Cursor = Cursors.WaitCursor;
            //Put all in an arry for thread safety.
            DataGridViewRow[] rows = new DataGridViewRow[dgvClients.RowCount - 1];

            int i = 0;
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                if (row.Cells[0].Value == row.Cells[0].DefaultNewRowValue)
                    break;
                rows[i++] = row;
            }

            AutoResetEvent waitHandle = new AutoResetEvent(false);
            int[] coreCounts = new int[rows.Length];
            int processed = 0;
            for (int j = 0; j != rows.Length; j++)
            {
                Thread t = new Thread(delegate(object arg)
                {
                    _coreCountWorkItem = new CoreCount();
                    string ip, hostName;
                    coreCounts[(int)arg] = _coreCountWorkItem.Get(rows[(int)arg].Cells[0].Value.ToString(), out ip, out hostName);
                    if (_ipsAndHostNames.ContainsKey(ip))
                        _ipsAndHostNames[ip] = hostName;
                    else
                        _ipsAndHostNames.Add(ip, hostName);
                    if (Interlocked.Increment(ref processed) == rows.Length)
                        waitHandle.Set();
                });
                t.IsBackground = true;
                t.Start(j);
            }
            if (rows.Length != 0)
                waitHandle.WaitOne();

            this.Cursor = Cursors.Default;

            return new List<int>(coreCounts);
        }
        private void SetSlaveCountInDataGridView(List<int> slaveCounts)
        {
            int i = 0;
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                if (row.Cells[0].Value == row.Cells[0].DefaultNewRowValue)
                    break;

                row.Cells[4].Value = slaveCounts[i++];
            }
        }
        private void SetCoreCountInDataGridView(List<int> coreCounts)
        {
            int i = 0;
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                if (row.Cells[0].Value == row.Cells[0].DefaultNewRowValue)
                    break;

                row.Cells[6].Value = coreCounts[i++];
            }
        }

        /// <summary>
        /// Cleans the IPs and Hostnames Dictionary
        /// </summary>
        private void CleanDictionary()
        {
            Dictionary<string, string> clean = new Dictionary<string, string>();
            foreach (string ip in _ipsAndHostNames.Keys)
                if (DGVContainsIpOrHostName(ip, _ipsAndHostNames[ip]))
                    clean.Add(ip, _ipsAndHostNames[ip]);
            _ipsAndHostNames = clean;
        }
        private bool DGVContainsIpOrHostName(string ip, string hostName)
        {
            foreach (DataGridViewRow row in dgvClients.Rows)
                if (row.Cells[0].Value as string == ip || row.Cells[0].Value as string == hostName)
                    return true;
            return false;
        }

        private class CoreCount
        {
            /// <summary>
            /// Outputs the ip and hostname (to lower).
            /// </summary>
            /// <param name="ipOrHostName"></param>
            /// <param name="ip"></param>
            /// <param name="hostName"></param>
            /// <returns></returns>
            public int Get(string ipOrHostName, out string ip, out string hostName)
            {
                IPAddress address = null;
                ip = hostName = null;
                if (IPAddress.TryParse(ipOrHostName, out address))
                {
                    ip = ipOrHostName;
                    hostName = Dns.GetHostEntry(ipOrHostName).HostName.ToLower();
                }
                else
                {
                    try
                    {
                        IPHostEntry hostEntry = Dns.GetHostEntry(ipOrHostName);
                        foreach (var a in hostEntry.AddressList)
                            if (a.AddressFamily == AddressFamily.InterNetwork)
                            {
                                address = a;
                                break;
                            }

                        ip = address.ToString();
                        hostName = hostEntry.HostName.ToLower();
                    }
                    catch { }
                }

                if (address != null)
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    SocketWrapper sw = new SocketWrapper(address, 1314, socket);
                    try
                    {
                        sw.Connect();
                        sw.Send(new Message<vApus.JumpStartStructures.Key>(vApus.JumpStartStructures.Key.CpuCoreCount, null), SendType.Binary);
                        Message<vApus.JumpStartStructures.Key> message = (Message<vApus.JumpStartStructures.Key>)sw.Receive(SendType.Binary);
                        return ((vApus.JumpStartStructures.CpuCoreCountMessage)message.Content).CpuCoreCount;
                    }
                    catch
                    {
                        try { sw.Close(); }
                        catch { }
                        sw = null;
                        socket = null;
                    }
                }
                return 0;
            }
        }

    }
}
