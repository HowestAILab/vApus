using System;
using System.Windows.Forms;
/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

namespace vApus.DistributedTesting
{
    public partial class AssignTest : Form
    {
        private bool _goToAssignedTest;
        private TileStresstest _assignedTest;

        public bool GoToAssignedTest
        {
            get { return _goToAssignedTest; }
        }
        public TileStresstest AssignedTest
        {
            get { return _assignedTest; }
        }

        public AssignTest()
        {
            InitializeComponent();
        }

        public AssignTest(DistributedTest distributedTest, TileStresstest currentTileStresstest)
            : this()
        {
            _assignedTest = currentTileStresstest;
            foreach (Tile t in distributedTest.Tiles)
            {
                TreeNode node = new TreeNode(t.ToString());
                node.Tag = t;
                tvwTiles.Nodes.Add(node);
            }

            if (_assignedTest != null)
                tvwTiles.SelectedNode = tvwTiles.Nodes[distributedTest.Tiles.IndexOf(_assignedTest.Parent as Tile)];
            else if (tvwTiles.Nodes.Count != 0)
                tvwTiles.SelectedNode = tvwTiles.Nodes[0];
        }

        private void tvwTiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            btnClear.Enabled = btnAssign.Enabled = btnAssignAndGoTo.Enabled = false;
            tvwTileStresstests.Nodes.Clear();

            Tile t = tvwTiles.SelectedNode.Tag as Tile;
            foreach (TileStresstest ts in t)
            {
                string label = ts.Index + ") " +
                ((ts.BasicTileStresstest.Connection == null || ts.BasicTileStresstest.Connection.IsEmpty) ?
                string.Empty : ts.BasicTileStresstest.Connection.ToString());

                TreeNode node = new TreeNode(label);
                node.ToolTipText = ts.ToString();
                node.Tag = ts;
                tvwTileStresstests.Nodes.Add(node);
            }

            if (_assignedTest != null && t.Contains(_assignedTest))
            {
                this.Focus();
                tvwTileStresstests.SelectedNode = tvwTileStresstests.Nodes[_assignedTest.Index - 1];
            }
            else if (tvwTileStresstests.Nodes.Count != 0)
            {
                this.Focus();
                tvwTileStresstests.SelectedNode = tvwTileStresstests.Nodes[0];
            }
        }

        private void tvwTileStresstests_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _assignedTest = tvwTileStresstests.SelectedNode.Tag as TileStresstest;
            btnClear.Enabled =btnAssign.Enabled = btnAssignAndGoTo.Enabled = true;
        }

        private void btnAssign_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnAssignAndGoTo_Click(object sender, EventArgs e)
        {
            _goToAssignedTest = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _assignedTest = null;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
