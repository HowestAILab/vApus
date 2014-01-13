/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    /// <summary>
    /// Shows, edit, test all connections in one view.
    /// </summary>
    public partial class ConnectionsView : BaseSolutionComponentView {

        #region Fields
        private readonly object _lock = new object();

        //Handier than SoltuionComponent.
        private readonly Connections _connections;
        //Holds all connection data.
        private readonly DataTable _dataSource;
        private volatile int _preferedRowToSelect = -1;

        private ConcurrentBag<TestConnections.TestWorkItem.MessageEventArgs> _testConnectionMessages;
        #endregion

        #region Constructors
        /// <summary>
        /// Design time constructor.
        /// </summary>
        public ConnectionsView() { InitializeComponent(); }
        /// <summary>
        /// Shows, edit, test all connections in one view.
        /// </summary>
        /// <param name="solutionComponent"></param>
        public ConnectionsView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            InitializeComponent();

            _connections = solutionComponent as Connections;
            _dataSource = new DataTable("Connections");
            _dataSource.Columns.Add("Connection");
            _dataSource.Columns.Add("Connection Proxy");
            _dataSource.Columns.Add("Connection String");

            FillConnectionsDatagrid();

            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }
        #endregion

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender == _connections || sender is Connection) FillConnectionsDatagrid();
        }

        public void FillConnectionsDatagrid() {
            dgvConnections.DataSource = null;
            _dataSource.Clear();
            foreach (var item in _connections)
                if (item is Connection) {
                    var connection = item as Connection;
                    var row = _dataSource.Rows.Add(connection.ToString(), connection.ConnectionProxy.ToString(), connection.ConnectionString);
                    row.SetTag(connection);
                }
            dgvConnections.DataSource = _dataSource;

            if (dgvConnections.RowCount != 0) {
                dgvConnections.ClearSelection();
                if (_preferedRowToSelect == -1)
                    dgvConnections.Rows[0].Selected = true;
                else
                    try {
                        dgvConnections.Rows[_preferedRowToSelect].Selected = true;
                    } catch { dgvConnections.Rows[0].Selected = true; }

                foreach (DataGridViewColumn clm in dgvConnections.Columns) clm.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        private void dgvConnections_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (!chkShowConnectionStrings.Checked && e.ColumnIndex == 2 && e.Value != null) {
                e.Value = new string('*', e.Value.ToString().Length);
            }
        }

        private void chkShowConnectionStrings_CheckedChanged(object sender, EventArgs e) {
            dgvConnections.Refresh();
        }

        private void btnTest_Click(object sender, EventArgs e) {
            btnTest.Text = "Busy...";
            btnTest.Enabled = btnEdit.Enabled = btnUndo.Enabled = false;

            var testConnections = new TestConnections();
            testConnections.Message += testConnections_Message;

            var l = new List<Connection>();
            foreach (var item in _connections)
                if (item is Connection) l.Add(item as Connection);
            _testConnectionMessages = new ConcurrentBag<TestConnections.TestWorkItem.MessageEventArgs>();

            testConnections.Test(l);
        }
        private void testConnections_Message(object sender, TestConnections.TestWorkItem.MessageEventArgs e) {
            _testConnectionMessages.Add(e);

            lock (_lock) {
                dgvConnections.Refresh();
                if (_testConnectionMessages.Count == _connections.CountOf(typeof(Connection)))
                    SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                        btnTest.Text = "Test";
                        btnTest.Enabled = btnEdit.Enabled = btnUndo.Enabled = true;
                    }, null);
            }
        }
        private void dgvConnections_CellPainting(object sender, DataGridViewCellPaintingEventArgs e) {
            if (_testConnectionMessages != null)
                foreach (var message in _testConnectionMessages)
                    if (message.Connection.Index == e.RowIndex + 1) {
                        var br = new SolidBrush(message.Succes ? Color.LightGreen : Color.Red);
                        e.Graphics.FillRectangle(br, e.CellBounds);
                        e.PaintContent(e.ClipBounds);
                        e.Handled = true;
                        break;
                    }
        }

        private void btnEdit_Click(object sender, EventArgs e) {
            var dialog = new FromTextDialog();
            dialog.Height = 800;
            dialog.Width = 800;
            dialog.WarnForEndingWithNewLine = false;

            var sb = new StringBuilder();
            int cpIndex = 0;
            ConnectionProxies cps = null;
            foreach (var item in _connections) {
                if (item is ConnectionProxies) {
                    cps = item as ConnectionProxies;
                    foreach (ConnectionProxy cp in cps) {
                        sb.Append("    My ");
                        sb.Append(cp.Label);
                        sb.Append(" Connection; ");
                        sb.Append((++cpIndex));
                        sb.Append(";");

                        var ruleSet = cp.ConnectionProxyRuleSet;
                        if (ruleSet.Count == 0) {
                            sb.AppendLine("Text");
                        } else {
                            for (int i = 0; i != ruleSet.Count - 1; i++) {
                                sb.Append((ruleSet[i] as SyntaxItem).Label);
                                sb.Append(ruleSet.ChildDelimiter);
                            }
                            sb.AppendLine((ruleSet[ruleSet.Count - 1] as SyntaxItem).Label);
                        }
                    }
                    break;
                }
            }
            dialog.Description = "Specify the label, the one-based connection proxy index and the connection string.\nThe delimiters are important. Invalid entries are discarded automatically.\n\nExample(s):\n\n" + sb.ToString().TrimEnd();

            sb = new StringBuilder();

            foreach (var item in _connections)
                if (item is Connection) {
                    var connection = item as Connection;
                    sb.Append(connection.Label);
                    sb.Append(";");
                    sb.Append((connection.ConnectionProxy.Index));
                    sb.Append(";");
                    sb.AppendLine(connection.ConnectionString);
                }

            //Remove the trailing \r\n.
            string text = sb.ToString();
            if (text.Length > 1)
                text = text.Substring(0, text.Length - 2);
            dialog.SetText(text);

            //Make new connections.
            if (dialog.ShowDialog() == DialogResult.OK) {
                //Set the undo data.
                var connectionsUndo = new List<Connection>(_connections.Count);
                foreach (var item in _connections)
                    if (item is Connection)
                        connectionsUndo.Add(item as Connection);
                btnUndo.Tag = connectionsUndo;
                btnUndo.Enabled = true;

                //Build new connections.
                _connections.Clear();
                foreach (string entry in dialog.Entries)
                    try {
                        string s = entry;

                        string label = s.Substring(0, s.IndexOf(';'));
                        s = s.Substring(label.Length + 1);
                        label = label.Trim();

                        string cpIndexString = s.Substring(0, s.IndexOf(';'));
                        s = s.Substring(cpIndexString.Length + 1);
                        cpIndex = int.Parse(cpIndexString.TrimStart()) - 1;

                        var connection = new Connection();
                        connection.Label = label;
                        connection.ConnectionProxy = cps[cpIndex] as ConnectionProxy;
                        connection.ConnectionString = s;

                        _connections.AddWithoutInvokingEvent(connection);

                    } catch {
                    }

                _connections.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }
        private void btnUndo_Click(object sender, EventArgs e) {
            if (btnUndo.Tag != null) {
                var connectionsUndo = btnUndo.Tag as List<Connection>;
                _connections.Clear();
                _connections.AddRangeWithoutInvokingEvent(connectionsUndo);

                _connections.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                btnUndo.Tag = null;
            }
            btnUndo.Enabled = false;
        }
        #endregion
    }
}
