/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    public partial class ConnectionsView : BaseSolutionComponentView {
        private readonly Connections _connections;
        private readonly DataTable _dataSource;
        private volatile int _preferedRowToSelect = -1;

        public ConnectionsView() {
            InitializeComponent();
        }

        public ConnectionsView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args) {
            InitializeComponent();
            _connections = solutionComponent as Connections;
            _dataSource = new DataTable("Connections");
            _dataSource.Columns.Add("Connection");
            _dataSource.Columns.Add("Connection Proxy");
            _dataSource.Columns.Add("Connection String");

            foreach (DataGridViewColumn clm in dgvConnections.Columns) clm.SortMode = DataGridViewColumnSortMode.NotSortable;

            FillConnectionsDatagrid();

            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }

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

            if (dgvConnections.RowCount == 0)
                btnEdit.Enabled = btnDuplicate.Enabled = btnRemove.Enabled = btnRemoveAll.Enabled = btnSort.Enabled = false;
            else {
                dgvConnections.ClearSelection();
                if (_preferedRowToSelect == -1)
                    dgvConnections.Rows[0].Selected = true;
                else
                    try {
                        dgvConnections.Rows[_preferedRowToSelect].Selected = true;
                    } catch { dgvConnections.Rows[0].Selected = true; }
            }
        }
        private void dgvConnections_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (!chkShowConnectionStrings.Checked && e.ColumnIndex == 2 && e.Value != null) {
                e.Value = new string('*', e.Value.ToString().Length);
            }
        }
        private void dgvConnections_CellEnter(object sender, DataGridViewCellEventArgs e) {
            btnEdit.Enabled = btnDuplicate.Enabled = btnRemove.Enabled = btnRemoveAll.Enabled = btnSort.Enabled = true;
        }

        private void btnAdd_Click(object sender, EventArgs e) {
            _preferedRowToSelect = _dataSource.Rows.Count;
            var connection = new Connection();
            _connections.Add(connection);
            //connection.Activate();
        }
        private void btnDuplicate_Click(object sender, EventArgs e) {
            SolutionComponent.SolutionComponentChanged -= SolutionComponent_SolutionComponentChanged;
            _preferedRowToSelect = _dataSource.Rows.Count;
            var connections = new List<Connection>();
            foreach (DataGridViewCell cell in dgvConnections.SelectedCells) {
                var connection = _dataSource.Rows[cell.RowIndex].GetTag() as Connection;
                if (connection != null && !connections.Contains(connection)) {
                    connections.Add(connection);
                    connection.Duplicate();
                }
            }
            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
            FillConnectionsDatagrid();
        }
        private void btnEdit_Click(object sender, EventArgs e) {
            foreach (DataGridViewCell cell in dgvConnections.SelectedCells) {
                dgvConnections.Rows[cell.RowIndex].Cells[0].Selected = true;
                var connection = _dataSource.Rows[cell.RowIndex].GetTag() as Connection;
                if (connection != null) connection.Activate();
                break;
            }
        }
        private void btnSort_Click(object sender, EventArgs e) {
            _preferedRowToSelect = -1;
            _connections.SortItemsByLabel();
        }
        private void btnRemove_Click(object sender, EventArgs e) {
            _preferedRowToSelect = -1;
            var connections = new List<Connection>();
            foreach (DataGridViewCell cell in dgvConnections.SelectedCells) {
                var connection = _dataSource.Rows[cell.RowIndex].GetTag() as Connection;
                if (connection != null && !connections.Contains(connection)) connections.Add(connection);
                if (_preferedRowToSelect == -1) _preferedRowToSelect = cell.RowIndex;
            }

            SolutionComponent.SolutionComponentChanged -= SolutionComponent_SolutionComponentChanged;
            bool removed = _connections.RemoveRange(connections);
            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
            if (removed) FillConnectionsDatagrid();
        }
        private void btnRemoveAll_Click(object sender, EventArgs e) {
            _preferedRowToSelect = -1;
            _connections.Clear();
        }

        private void chkShowConnectionStrings_CheckedChanged(object sender, EventArgs e) {
            dgvConnections.Refresh();
        }
    }
}
