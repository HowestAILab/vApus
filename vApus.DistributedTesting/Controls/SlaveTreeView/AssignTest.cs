/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Windows.Forms;

namespace vApus.DistributedTest {
    public partial class AssignTest : Form {
        private TileStressTest _assignedTest;
        private bool _goToAssignedTest;

        public AssignTest() {
            InitializeComponent();
        }

        public AssignTest(DistributedTest distributedTest, TileStressTest currentTileStressTest)
            : this() {
            _assignedTest = currentTileStressTest;
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

        public TileStressTest AssignedTest {
            get { return _assignedTest; }
        }

        private void tvwTiles_AfterSelect(object sender, TreeViewEventArgs e) {
            btnClear.Enabled = btnAssign.Enabled = btnAssignAndGoTo.Enabled = false;
            tvwTileStressTests.Nodes.Clear();

            var t = tvwTiles.SelectedNode.Tag as Tile;
            foreach (TileStressTest ts in t) {
                string label = ts.Index + ") " +
                               ((ts.BasicTileStressTest.Connection == null || ts.BasicTileStressTest.Connection.IsEmpty)
                                    ? string.Empty
                                    : ts.BasicTileStressTest.Connection.ToString());

                var node = new TreeNode(label);
                node.ToolTipText = ts.ToString();
                node.Tag = ts;
                tvwTileStressTests.Nodes.Add(node);
            }

            if (_assignedTest != null && t.Contains(_assignedTest)) {
                Focus();
                tvwTileStressTests.SelectedNode = tvwTileStressTests.Nodes[_assignedTest.Index - 1];
            } else if (tvwTileStressTests.Nodes.Count != 0) {
                Focus();
                tvwTileStressTests.SelectedNode = tvwTileStressTests.Nodes[0];
            }
        }

        private void tvwTileStressTests_AfterSelect(object sender, TreeViewEventArgs e) {
            _assignedTest = tvwTileStressTests.SelectedNode.Tag as TileStressTest;
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