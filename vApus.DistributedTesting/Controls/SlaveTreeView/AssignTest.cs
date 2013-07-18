/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Windows.Forms;

namespace vApus.DistributedTesting {
    public partial class AssignTest : Form {
        private TileStresstest _assignedTest;
        private bool _goToAssignedTest;

        public AssignTest() {
            InitializeComponent();
        }

        public AssignTest(DistributedTest distributedTest, TileStresstest currentTileStresstest)
            : this() {
            _assignedTest = currentTileStresstest;
            foreach (Tile t in distributedTest.Tiles) {
                var node = new TreeNode(t.ToString());
                node.Tag = t;
                tvwTiles.Nodes.Add(node);
            }

            if (_assignedTest != null)
                tvwTiles.SelectedNode = tvwTiles.Nodes[distributedTest.Tiles.IndexOf(_assignedTest.Parent as Tile)];
            else if (tvwTiles.Nodes.Count != 0)
                tvwTiles.SelectedNode = tvwTiles.Nodes[0];
        }

        public bool GoToAssignedTest {
            get { return _goToAssignedTest; }
        }

        public TileStresstest AssignedTest {
            get { return _assignedTest; }
        }

        private void tvwTiles_AfterSelect(object sender, TreeViewEventArgs e) {
            btnClear.Enabled = btnAssign.Enabled = btnAssignAndGoTo.Enabled = false;
            tvwTileStresstests.Nodes.Clear();

            var t = tvwTiles.SelectedNode.Tag as Tile;
            foreach (TileStresstest ts in t) {
                string label = ts.Index + ") " +
                               ((ts.BasicTileStresstest.Connection == null || ts.BasicTileStresstest.Connection.IsEmpty)
                                    ? string.Empty
                                    : ts.BasicTileStresstest.Connection.ToString());

                var node = new TreeNode(label);
                node.ToolTipText = ts.ToString();
                node.Tag = ts;
                tvwTileStresstests.Nodes.Add(node);
            }

            if (_assignedTest != null && t.Contains(_assignedTest)) {
                Focus();
                tvwTileStresstests.SelectedNode = tvwTileStresstests.Nodes[_assignedTest.Index - 1];
            } else if (tvwTileStresstests.Nodes.Count != 0) {
                Focus();
                tvwTileStresstests.SelectedNode = tvwTileStresstests.Nodes[0];
            }
        }

        private void tvwTileStresstests_AfterSelect(object sender, TreeViewEventArgs e) {
            _assignedTest = tvwTileStresstests.SelectedNode.Tag as TileStresstest;
            btnClear.Enabled = btnAssign.Enabled = btnAssignAndGoTo.Enabled = true;
        }

        private void btnAssign_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnAssignAndGoTo_Click(object sender, EventArgs e) {
            _goToAssignedTest = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnClear_Click(object sender, EventArgs e) {
            _assignedTest = null;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}