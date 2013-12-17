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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.JumpStartStructures;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting {
    /// <summary>
    /// This (re)constructs a distributed test for you and is shown automaticaly when opening up an empty distributed test.
    /// </summary>
    public partial class Wizard : Form {

        #region Fields
        private object _lock = new object();

        //All connections in the solution.
        [ThreadStatic]
        private static TryConnect _tryConnectWorkItem;
        //This is done in parallel, therefore a threadstatic work item.

        private readonly List<Connection> _connections = new List<Connection>();
        private readonly StresstestProject _stresstestProject;
        private DistributedTest _distributedTest;

        //Kept here, easier and faster.
        internal Dictionary<string, string> _ipsAndHostNames = new Dictionary<string, string>();
        #endregion

        #region Constructors
        /// <summary>
        /// Design time constructor, use the other one.
        /// </summary>
        public Wizard() { InitializeComponent(); }
        /// <summary>
        /// This (re)constructs a distributed test for you and is shown automaticaly when opening up an empty distributed test.
        /// </summary>
        public Wizard(DistributedTest distributedTest)
            : this() {

            var connections = Solution.ActiveSolution.GetSolutionComponent(typeof(Connections)) as Connections;
            foreach (BaseItem item in connections)
                if (item is Connection)
                    _connections.Add(item as Connection);

            _stresstestProject = Solution.ActiveSolution.GetProject("StresstestProject") as StresstestProject;

            _distributedTest = distributedTest;
            SetDistributedTestToGui();
        }
        #endregion

        #region Functions
        private void SetDistributedTestToGui() {
            SetDefaultTestSettings();
            SetGenerateTiles();
            SetAddClientsAndSlaves();
        }
        private void SetDefaultTestSettings() {
            chkUseRDP.Checked = _distributedTest.UseRDP;
            cboRunSync.SelectedIndex = (int)_distributedTest.RunSynchronization;
        }
        private void SetGenerateTiles() {
            nudTiles.Value = 1;
        }
        private void SetAddClientsAndSlaves() {
            nudTests.Value = _stresstestProject.CountOf(typeof(Stresstest.Stresstest));

            foreach (Client client in _distributedTest.Clients) {
                string ipOrHostname = client.HostName.Length == 0 ? client.IP : client.HostName;
                ipOrHostname = ipOrHostname.Trim();
                if (ipOrHostname.Length != 0)
                    dgvClients.Rows.Add(ipOrHostname, client.UserName, client.Domain, client.Password, client.Count, 0);
            }

            RefreshDGV();

            nudTiles.ValueChanged += nudTilesAndTests_ValueChanged;
            nudTests.ValueChanged += nudTilesAndTests_ValueChanged;
            dgvClients.CellEndEdit += dgvClients_CellEndEdit;
            dgvClients.RowsRemoved += dgvClients_RowsRemoved;
        }

        private void chkUseRDP_CheckedChanged(object sender, EventArgs e) {
            clmUserName.HeaderText = chkUseRDP.Checked ? "* User Name (RDP)" : "User Name (RDP)";
            clmPassword.HeaderText = chkUseRDP.Checked ? "* Password" : "Password";
        }

        private void nudTilesAndTests_ValueChanged(object sender, EventArgs e) {
            SetCountsInGui();
        }
        private void rdbsGenerateAndtAddTiles_CheckedChanged(object sender, EventArgs e) {
            if ((sender as RadioButton).Checked) //3 rdbs handle this event here, this way it is only handled once.
            {
                nudTiles.Enabled = nudTests.Enabled = !rdbDoNotAddTiles.Checked;
                SetCountsInGui();
            }
        }

        /// <summary>
        ///     Display passwordchars in the password cell.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvClients_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e) {
            var txt = e.Control as TextBox;
            if (txt != null)
                if (dgvClients.CurrentCell.ColumnIndex == 3) {
                    txt.UseSystemPasswordChar = true;

                    if (dgvClients.CurrentRow.Tag != null)
                        txt.Text = dgvClients.CurrentRow.Tag.ToString();
                } else {
                    txt.UseSystemPasswordChar = false;
                }
        }
        private void dgvClients_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            foreach (DataGridViewRow row in dgvClients.Rows) {
                if (row.Cells.Count < 4 || row.Cells[3].Value == null) {
                    row.Tag = null;
                } else {
                    DataGridViewCell cell = row.Cells[3];
                    row.Tag = cell.Value;
                    cell.Value = new string('*', cell.Value.ToString().Length);
                }
            }
        }
        private void dgvClients_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            if (dgvClients.CurrentCell != null && dgvClients.CurrentCell.ColumnIndex == 3)
                if (dgvClients.CurrentCell.Value == null) {
                    dgvClients.CurrentRow.Tag = null;
                } else {
                    dgvClients.CurrentRow.Tag = dgvClients.CurrentCell.Value.ToString();
                    dgvClients.CurrentCell.Value = new string('*', dgvClients.CurrentCell.Value.ToString().Length);
                }

            if (dgvClients.CurrentRow != dgvClients.Rows[dgvClients.Rows.Count - 1] &&
                (dgvClients.CurrentRow.Cells[0].Value == null ||
                 dgvClients.CurrentRow.Cells[0].Value.ToString().Length == 0)) {
                dgvClients.Rows.Remove(dgvClients.CurrentRow);
                return;
            }

            foreach (DataGridViewRow row in dgvClients.Rows) {
                if (row.Cells[0].Value == row.Cells[0].DefaultNewRowValue)
                    break;
                if (row != dgvClients.CurrentRow && dgvClients.CurrentRow.Cells[0].Value != null
                    && row.Cells[0].Value.ToString() == dgvClients.CurrentRow.Cells[0].Value.ToString()) {
                    MessageBox.Show("This client was already added.", string.Empty, MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    dgvClients.Rows.Remove(dgvClients.CurrentRow);
                    return;
                }
            }
            RefreshDGV();
        }
        private void dgvClients_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) {
            SetCountsInGui();
        }

        private void btnRefresh_Click(object sender, EventArgs e) {
            RefreshDGV();
        }

        private void nudSlavesPerClient_ValueChanged(object sender, EventArgs e) {
            RefreshDGV();
        }

        async private void RefreshDGV() {
            Text = "Wizard - Connecting clients, please be patient...";
            pnl.Enabled = false;

            await Task.Run(() => SetSlaveCountsPerClients());

            pnl.Enabled = true;
            Text = "Wizard - Let vApus build your distributed test";
        }

        private void SetSlaveCountsPerClients() {
            var connected = TryConnectClients();
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                SetConnectedInDataGridView(connected);

                var slaveCounts = new List<int>(dgvClients.RowCount - 1);
                for (int i = 0; i != dgvClients.RowCount - 1; i++)
                    slaveCounts.Add(dgvClients.Rows[i].DefaultCellStyle.BackColor == Color.LightGreen ? (int)nudSlavesPerClient.Value : 0);

                SetSlaveCountInDataGridView(slaveCounts);
                SetCountsInGui();
            }, null);
        }
        /// <summary>
        ///     Get the count of the cores for each client (-1 if unknown)
        /// </summary>
        /// <returns></returns>
        private List<bool> TryConnectClients() {
            //Put all in an array for thread safety.
            var r = new List<DataGridViewRow>();
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                foreach (DataGridViewRow row in dgvClients.Rows)
                    if (row.Cells[0].Value != row.Cells[0].DefaultNewRowValue &&
                        row.Cells[0].Value != null && row.Cells[0].Value.ToString().Length != 0)
                        r.Add(row);
            }, null);

            var connected = new List<bool>(new bool[r.Count]);
            DataGridViewRow[] rows = r.ToArray();

            //Process each row in parallel, this way of doing stuff will maybe be changed to the new async stuff in .net.
            var waitHandle = new AutoResetEvent(false);
            int processed = 0;
            for (int j = 0; j != rows.Length; j++) {
                ThreadPool.QueueUserWorkItem((object state) => {
                    _tryConnectWorkItem = new TryConnect();
                    //A thread static field used to be able to process in parallel
                    bool b = _tryConnectWorkItem.Try(rows[(int)state].Cells[0].Value.ToString(), this);

                    lock (_lock) connected[(int)state] = b;

                    if (Interlocked.Increment(ref processed) == rows.Length)
                        waitHandle.Set();
                }, j);
            }
            if (rows.Length != 0)
                waitHandle.WaitOne();

            return connected;
        }
        private void SetSlaveCountInDataGridView(List<int> slaveCounts) {
            int i = 0;
            foreach (DataGridViewRow row in dgvClients.Rows) {
                if (row.Cells[0].Value == row.Cells[0].DefaultNewRowValue)
                    break;

                row.Cells[4].Value = slaveCounts[i++];
            }
        }
        private void SetConnectedInDataGridView(List<bool> connected) {
            int i = 0;
            foreach (DataGridViewRow row in dgvClients.Rows) {
                if (row.Cells[0].Value == row.Cells[0].DefaultNewRowValue)
                    break;

                bool b = connected[i++];
                if (b) {
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    foreach (DataGridViewCell cell in row.Cells)
                        cell.ToolTipText = string.Empty;
                } else {
                    row.DefaultCellStyle.BackColor = Color.Orange;
                    foreach (DataGridViewCell cell in row.Cells)
                        cell.ToolTipText =
                            "Unreacheable client or vApus.Jumpstart not running on the client. This client will not be added to the distributed test.";
                }
            }
        }

        /// <summary>
        ///     Also cleans the dictionary holding the ips and host names.
        /// </summary>
        private void SetCountsInGui() {
            int totalNewTestCount = rdbDoNotAddTiles.Checked ? 0 : (int)(nudTiles.Value * nudTests.Value);

            int totalTestCount = totalNewTestCount, totalUsedTestCount = totalNewTestCount;

            if (rdbAppendTiles.Checked) //In the other cases we do not add new tiles.
                foreach (Tile tile in _distributedTest.Tiles)
                    foreach (TileStresstest ts in tile) {
                        ++totalTestCount;
                        if (ts.Use)
                            ++totalUsedTestCount;
                    }

            int totalSlaveCount = 0;
            foreach (DataGridViewRow row in dgvClients.Rows) {
                if (row.Cells[4].Value == row.Cells[4].DefaultNewRowValue)
                    break;

                totalSlaveCount += int.Parse(row.Cells[4].Value.ToString());
            }
            int totalAssignedTestCount = totalSlaveCount <= totalUsedTestCount ? totalSlaveCount : totalUsedTestCount;

            clmSlaves.HeaderText = "Number of Slaves (" + totalSlaveCount + ")";
            clmTests.HeaderText = "Number of Tests (" + totalAssignedTestCount + "/" + totalUsedTestCount + ")";

            int yetToAssingTestCount = totalAssignedTestCount;
            foreach (DataGridViewRow row in dgvClients.Rows) {
                if (row.Cells[4].Value == row.Cells[4].DefaultNewRowValue)
                    break;

                if (yetToAssingTestCount == 0) {
                    row.Cells[5].Value = 0;
                } else {
                    int slaveCount = int.Parse(row.Cells[4].Value.ToString());

                    int tests = yetToAssingTestCount - slaveCount >= 0 ? slaveCount : yetToAssingTestCount;
                    row.Cells[5].Value = tests;
                    yetToAssingTestCount -= tests;
                }
            }

            int notAssigned = totalTestCount - totalAssignedTestCount;
            lblNotAssignedTests.Text = notAssigned + " Test" + (notAssigned == 1 ? " is" : "s are") + " not Assigned to a Slave.";

            int notUsedTestCount = totalTestCount - totalUsedTestCount;
            if (notUsedTestCount > 0)
                lblNotAssignedTests.Text += " whereof " + notUsedTestCount + " that " + (notUsedTestCount == 1 ? "is" : "are") + " not Used. (Checked)";

            CleanDictionary();
        }

        /// <summary>
        ///     Cleans the IPs and Hostnames Dictionary
        /// </summary>
        private void CleanDictionary() {
            var clean = new Dictionary<string, string>();
            foreach (string ip in _ipsAndHostNames.Keys)
                if (DGVContainsIpOrHostName(ip, _ipsAndHostNames[ip]))
                    clean.Add(ip, _ipsAndHostNames[ip]);
            _ipsAndHostNames = clean;
        }

        private bool DGVContainsIpOrHostName(string ip, string hostName) {
            foreach (DataGridViewRow row in dgvClients.Rows)
                if (row.Cells[0].Value != row.Cells[0].DefaultNewRowValue &&
                    (row.Cells[0].Value.ToString() == ip ||
                     row.Cells[0].Value.ToString().ToLower().Split('.')[0] == hostName.ToLower().Split('.')[0])
                    )
                    return true;
            return false;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            Connection[] toAssignConnections = SmartAssignConnections();
            //Review the connections and make tweaks.
            if (chkReview.Checked) {
                if (toAssignConnections.Length == 0 || toAssignConnections[0].IsEmpty) {
                    MessageBox.Show("You do not have connections in your solution!", string.Empty, MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                } else {
                    var conUsage = new WizardConnectionUsage();

                    Tiles alreadyInStresstest = rdbStartFromScratch.Checked ? new Tiles() : _distributedTest.Tiles;
                    int tiles = rdbDoNotAddTiles.Checked ? 0 : (int)nudTiles.Value;
                    var tests = (int)nudTests.Value;

                    conUsage.Init(alreadyInStresstest, tiles, tests, toAssignConnections);
                    if (conUsage.ShowDialog() == DialogResult.Cancel)
                        return;
                    else
                        toAssignConnections = conUsage.ToAssignConnections;
                }
            }
            //Set all changes to the distributed test.
            if (!SetGuiToDistributedTest(toAssignConnections))
                if (MessageBox.Show(
                    "One or more important(*) cells are not filled in under 'Add Clients and Slaves'.\nDo you want to proceed anyway?.",
                    string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;

            Close();
        }

        /// <summary>
        ///     Fills an array with connections. All available connections are used, the assign pattern thats used for stresstests is applied.
        /// </summary>
        private Connection[] SmartAssignConnections() {
            int totalTestCount = rdbDoNotAddTiles.Checked ? 0 : (int)(nudTiles.Value * nudTests.Value);

            if (!rdbStartFromScratch.Checked) //In the other cases we do not add new tiles.
                foreach (Tile tile in _distributedTest.Tiles)
                    foreach (TileStresstest ts in tile)
                        ++totalTestCount;


            var toAssignConnections = new Connection[totalTestCount];

            if (_connections.Count == 0) //Assign empty connections.
            {
                SolutionComponent connectionParent = Solution.ActiveSolution.GetSolutionComponent(typeof(Connections));
                for (int i = 0; i != totalTestCount; i++)
                    toAssignConnections[i] =
                        SolutionComponent.GetNextOrEmptyChild(typeof(Connection), connectionParent) as Connection;
            } else {
                int[] assignPattern = GetAssignPattern();
                for (int i = 0; i != totalTestCount; i++) {
                    //make sure assignPatternIndex can not go out of bounds.
                    int assignPatternIndex = i;
                    int connectionIndexOffset = 0; //Offset the connections so all can be used.
                    while (assignPatternIndex >= assignPattern.Length) {
                        assignPatternIndex -= assignPattern.Length;
                        connectionIndexOffset += assignPattern.Length;
                    }

                    int connectionIndex = assignPattern[assignPatternIndex] + connectionIndexOffset;
                    //Correct if it goes out of bounds off the connection collection.
                    while (connectionIndex >= _connections.Count)
                        connectionIndex -= _connections.Count;

                    toAssignConnections[i] = _connections[connectionIndex];
                }
            }
            return toAssignConnections;
        }

        private int[] GetAssignPattern() {
            var assignPattern = new List<int>(_stresstestProject.CountOf(typeof(Stresstest.Stresstest)));

            foreach (BaseItem item in _stresstestProject)
                if (item is Stresstest.Stresstest)
                    assignPattern.Add(_connections.IndexOf((item as Stresstest.Stresstest).Connection));

            return assignPattern.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <returns>If the gui was succesfully set to the distributed test</returns>
        private bool SetGuiToDistributedTest(Connection[] toAssignConnections) {
            //Validate if the gui can be set to the distributed test.
            var starredColumnIndices = new List<int>();
            for (int columnIndex = 0; columnIndex != dgvClients.Columns.Count; columnIndex++)
                if (dgvClients.Columns[columnIndex].HeaderText.Contains("*"))
                    starredColumnIndices.Add(columnIndex);

            foreach (DataGridViewRow row in dgvClients.Rows) {
                if (row.Cells[0].Value == row.Cells[0].DefaultNewRowValue)
                    break;

                foreach (int starredColumnIndex in starredColumnIndices)
                    if (row.Cells[starredColumnIndex].Value == row.Cells[starredColumnIndex].DefaultNewRowValue ||
                        row.Cells[starredColumnIndex].Value == null ||
                        row.Cells[starredColumnIndex].Value.ToString().Length == 0)
                        return false;
            }

            SetDefaultTestSettingToDistributedTest();
            List<Slave> toAssingTestsTo = AddClientsAndSlavesToDistributedTest();
            GenerateAndAddTilesToDistributedTest(toAssignConnections, toAssingTestsTo);

            //Notify the gui.
            _distributedTest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, nudTiles.Value > 1);

            return true;
        }

        private void SetDefaultTestSettingToDistributedTest() {
            _distributedTest.UseRDP = chkUseRDP.Checked;
            _distributedTest.RunSynchronization = (RunSynchronization)cboRunSync.SelectedIndex;
        }

        /// <summary></summary>
        /// <returns>The slaves added.</returns>
        private List<Slave> AddClientsAndSlavesToDistributedTest() {
            RefreshDGV();

            //Clear the clients in de distributed test, new ones will be added.
            _distributedTest.Clients.ClearWithoutInvokingEvent();

            var toAssingTestsTo = new List<Slave>();
            foreach (DataGridViewRow row in dgvClients.Rows) {
                if (row.Cells[0].Value == row.Cells[0].DefaultNewRowValue || row.DefaultCellStyle.BackColor == Color.Orange)
                    continue;

                var numberOfSlaves = (int)row.Cells[4].Value;
                if (numberOfSlaves == 0)
                    continue; //No use adding a client without slaves.

                //Add a new client.
                var client = new Client();
                _distributedTest.Clients.AddWithoutInvokingEvent(client);

                //Set the ip and host name.
                var ipOrHostname = row.Cells[0].Value as string;
                if (_ipsAndHostNames.ContainsKey(ipOrHostname)) {
                    client.IP = ipOrHostname;
                    client.HostName = _ipsAndHostNames[ipOrHostname];
                } else if (_ipsAndHostNames.ContainsValue(ipOrHostname.ToLower().Split('.')[0])) {
                    string ip;
                    if (_ipsAndHostNames.TryGetKey(ipOrHostname.ToLower().Split('.')[0], out ip)) {
                        client.IP = ip;
                        client.HostName = ipOrHostname;
                    }
                }

                client.UserName = row.Cells[1].Value as string;
                client.Domain = row.Cells[2].Value as string;
                if (row.Tag != null)
                    client.Password = row.Tag as string;

                //Add slaves to the client.
                bool clientIsMaster = false;
                foreach (var ipAddress in Dns.GetHostAddresses(Dns.GetHostName()))
                    if (ipAddress.ToString() == client.IP) {
                        clientIsMaster = true;
                        break;
                    }

                int startPort = 1347;
                if (clientIsMaster && startPort <= SocketListener.GetInstance().Port)
                    startPort = SocketListener.GetInstance().Port + 1;

                //var alreadyUsedPas = new List<int>();
                for (int i = 0; i != numberOfSlaves; i++) {
                    var slave = new Slave();
                    slave.Port = startPort++;
                    //slave.ProcessorAffinity = GetProcessorAffinity(int.Parse(numberOfSlaves.ToString()),
                    //                                               int.Parse(row.Cells[6].Value.ToString()),
                    //                                               alreadyUsedPas);
                    //alreadyUsedPas.AddRange(slave.ProcessorAffinity);
                    client.AddWithoutInvokingEvent(slave);
                    toAssingTestsTo.Add(slave);
                }
            }
            return toAssingTestsTo;
        }

        /// <summary></summary>
        /// <param name="slaves">To assign the tests to</param>
        private void GenerateAndAddTilesToDistributedTest(Connection[] toAssignConnections, List<Slave> slaves) {
            //Distribute connections.
            int connectionIndex = 0;
            if (rdbStartFromScratch.Checked)
                _distributedTest.Tiles.ClearWithoutInvokingEvent();
            else
                foreach (Tile tile in _distributedTest.Tiles)
                    foreach (TileStresstest tileStresstest in tile)
                        tileStresstest.BasicTileStresstest.Connection = toAssignConnections[connectionIndex++];

            //The existing stresstests will be reassigned to the slaves.
            if (!rdbDoNotAddTiles.Checked)
                //Add the tests to the distributed test.
                for (int i = 0; i != (int)nudTiles.Value; i++) {
                    var tile = new Tile();
                    _distributedTest.Tiles.AddWithoutInvokingEvent(tile);

                    Stresstest.Stresstest defaultToStresstest = null;
                    for (int j = 0; j != (int)nudTests.Value; j++) {
                        var tileStresstest = new TileStresstest();
                        defaultToStresstest = GetNextDefaultToStresstest(defaultToStresstest);
                        //Default to a stresstest in the solution.
                        tileStresstest.DefaultAdvancedSettingsTo = defaultToStresstest;

                        tile.AddWithoutInvokingEvent(tileStresstest);

                        tileStresstest.BasicTileStresstest.Connection = toAssignConnections[connectionIndex++];
                    }
                }

            int slaveIndex = 0;
            //Assign the tests to slaves
            foreach (Tile tile in _distributedTest.Tiles)
                foreach (TileStresstest tileStresstest in tile) {
                    tileStresstest.Use = true;
                    tileStresstest.BasicTileStresstest.FillAndGetSlavesParent();
                    tileStresstest.BasicTileStresstest.Slaves = slaveIndex == slaves.Count ? new Slave[] { } : new Slave[] { slaves[slaveIndex++] };
                }
        }

        /// <summary>
        /// </summary>
        /// <param name="previous">Can be null</param>
        /// <returns></returns>
        private Stresstest.Stresstest GetNextDefaultToStresstest(Stresstest.Stresstest previous) {
            SolutionComponent stresstestProject =
                Solution.ActiveSolution.GetSolutionComponent(typeof(StresstestProject));
            if (previous != null) {
                bool previousFound = false;
                foreach (BaseItem item in stresstestProject)
                    if (item is Stresstest.Stresstest)
                        if (item == previous)
                            previousFound = true; //The next item of the correct type will be returned if any
                        else if (previousFound)
                            return item as Stresstest.Stresstest;
            }

            //If next was not found (starts with the first item again if any to use for a previous default to).
            return
                SolutionComponent.GetNextOrEmptyChild(typeof(Stresstest.Stresstest), stresstestProject) as
                Stresstest.Stresstest;
        }
        #endregion

        private class TryConnect {
            private static readonly object _lock = new object();
            /// <summary>
            ///     stores the ip and hostname in the given list.
            /// </summary>
            /// <param name="ipOrHostName"></param>
            /// <param name="wizard"></param>
            /// <returns></returns>
            public bool Try(string ipOrHostName, Wizard wizard) {
                IPAddress address = null;
                string ip = null, hostName = null;
                try {
                    if (IPAddress.TryParse(ipOrHostName, out address)) {
                        ip = ipOrHostName;
                        hostName = Dns.GetHostEntry(ipOrHostName).HostName.ToLower();
                    } else {
                        ipOrHostName = ipOrHostName.ToLower().Split('.')[0];
                        IPHostEntry hostEntry = Dns.GetHostEntry(ipOrHostName);
                        foreach (IPAddress a in hostEntry.AddressList)
                            if (a.AddressFamily == AddressFamily.InterNetwork || a.AddressFamily == AddressFamily.InterNetworkV6) {
                                address = a;
                                break;
                            }

                        ip = address.ToString();
                        hostName = ipOrHostName != "localhost" ? hostEntry.HostName.ToLower() : "localhost";
                    }

                    lock (_lock)
                        if (wizard._ipsAndHostNames.ContainsKey(ip))
                            wizard._ipsAndHostNames[ip] = hostName.Split('.')[0];
                        else
                            wizard._ipsAndHostNames.Add(ip, hostName.Split('.')[0]);

                    return true;
                } catch {
                    address = null;
                }

                return false;
            }
        }
    }
}