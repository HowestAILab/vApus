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
using System.Text;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.DistributedTesting
{
    public partial class WizardConnectionUsage : Form
    {
        #region Fields
        private Tiles _tilesAlreadyInDistributedTest = new Tiles();
        private int _numberOfNewTiles, _numberOfTestsPerNewTile;
        private Stresstest.Connection[] _toAssignConnections;

        //All connections in the solution.
        private Stresstest.Connections _connections;
        #endregion

        /// <summary>
        /// Get this afer OK is clicked.
        /// </summary>
        public Stresstest.Connection[] ToAssignConnections
        {
            get { return _toAssignConnections; }
            set { _toAssignConnections = value; }
        }

        #region Constructor
        /// <summary>
        /// Please call Init(..) after making a new WizardConnectionUsage.
        /// </summary>
        public WizardConnectionUsage()
        {
            InitializeComponent();
        }
        #endregion

        #region Functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tilesAlreadyInDistributedTest"></param>
        /// <param name="numberOfNewTiles"></param>
        /// <param name="numberOfTestsPerNewTile"></param>
        /// <param name="toAssignConnections">Must be predetermined in Wizard (SmartAssignConnections)</param>
        public void Init(Tiles tilesAlreadyInDistributedTest, int numberOfNewTiles, int numberOfTestsPerNewTile, Stresstest.Connection[] toAssignConnections)
        {
            _tilesAlreadyInDistributedTest = tilesAlreadyInDistributedTest;
            _numberOfNewTiles = numberOfNewTiles;
            _numberOfTestsPerNewTile = numberOfTestsPerNewTile;
            _toAssignConnections = toAssignConnections;

            _connections = Solution.ActiveSolution.GetSolutionComponent(typeof(Stresstest.Connections)) as Stresstest.Connections;

            FillTreeView();
            FillConnections();
        }
        private void FillConnections()
        {
            flp.Controls.Clear();
            foreach (var item in _connections)
                if (item is Stresstest.Connection)
                {
                    var rdb = new RadioButton();
                    rdb.AutoSize = true;
                    rdb.Text = item.ToString();
                    rdb.Tag = item;
                    flp.Controls.Add(rdb);

                    //Style the rdbs if one is checked.
                    rdb.CheckedChanged += new EventHandler(rdb_CheckedChanged);
                }
        }

        private void rdb_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
            {
                StoreConnectionInTreeNode((sender as RadioButton).Tag as Stresstest.Connection);
                StyleUsedConnections();
            }
        }
        /// <summary>
        /// Store the connection in the tag of the tree node, this is used afterwards to build the distributed test.
        /// </summary>
        private void StoreConnectionInTreeNode(Stresstest.Connection connection)
        {
            if (tvw.SelectedNode != null && tvw.SelectedNode != null)
            {
                string label = (tvw.SelectedNode.Parent.Nodes.IndexOf(tvw.SelectedNode) + 1) + ") " +
                ((connection == null || connection.IsEmpty) ?
                    string.Empty : connection.ToString());
                tvw.SelectedNode.Text = label;

                var kvp = (KeyValuePair<Stresstest.Stresstest, Stresstest.Connection>)tvw.SelectedNode.Tag;
                tvw.SelectedNode.Tag = new KeyValuePair<Stresstest.Stresstest, Stresstest.Connection>(kvp.Key, connection);
            }
        }
        /// <summary>
        /// Give the radio button a back color and set a tooltip.
        /// The tooltip says in what tile stresstests the connection is used. 
        /// </summary>
        private void StyleUsedConnections()
        {
            foreach (RadioButton rdb in flp.Controls)
            {
                StringBuilder sb = new StringBuilder();
                foreach (TreeNode tileNode in tvw.Nodes)
                    foreach (TreeNode tileStresstestNode in tileNode.Nodes)
                    {
                        var kvp = (KeyValuePair<Stresstest.Stresstest, Stresstest.Connection>)tileStresstestNode.Tag;
                        if (kvp.Value == rdb.Tag)
                            sb.AppendLine(tileNode.Text + " -> " + tileStresstestNode.Text);
                    }

                rdb.Font = new Font(this.Font, rdb.Checked ? FontStyle.Bold : FontStyle.Regular);
                if (sb.Length == 0)
                {
                    rdb.BackColor = Color.Transparent;
                    toolTip.SetToolTip(rdb, null);
                }
                else
                {
                    rdb.BackColor = Color.FromArgb(192, 192, 255);
                    toolTip.SetToolTip(rdb, "Connection used in:\n" + sb.ToString());
                }
            }
        }
        private void FillTreeView()
        {
            tvw.Nodes.Clear();
            int connectionIndex = 0; //Distribute the connections over the tile stresstests.
            foreach (Tile tile in _tilesAlreadyInDistributedTest)
            {
                TreeNode tileNode = AddNewTileNode();
                foreach (TileStresstest tileStresstest in tile)
                {
                    TreeNode testNode = AddNewTestNode(tileNode, tileStresstest.DefaultAdvancedSettingsTo, _toAssignConnections[connectionIndex++]);
                }
            }

            Stresstest.Stresstest defaultTo = null;
            for (int t = 0; t != _numberOfNewTiles; t++)
            {
                TreeNode tileNode = AddNewTileNode();
                for (int ts = 0; ts != _numberOfTestsPerNewTile; ts++)
                {
                    defaultTo = GetNextDefaultToStresstest(defaultTo);
                    TreeNode testNode = AddNewTestNode(tileNode, defaultTo, _toAssignConnections[connectionIndex++]);
                }
            }
            tvw.ExpandAll();

            if (tvw.Nodes.Count != 0)
            {
                if (tvw.Nodes[0].Nodes.Count == 0)
                    tvw.SelectedNode = tvw.Nodes[0];
                else
                    tvw.SelectedNode = tvw.Nodes[0].Nodes[0];
            }
        }
        private TreeNode AddNewTileNode()
        {
            TreeNode tn = new TreeNode("Tile " + (tvw.Nodes.Count + 1));
            tvw.Nodes.Add(tn);
            return tn;
        }
        private TreeNode AddNewTestNode(TreeNode tileNode, Stresstest.Stresstest defaultSettingsTo, Stresstest.Connection connection)
        {
            string label = (tileNode.Nodes.Count + 1) + ") " +
            ((connection == null || connection.IsEmpty) ?
                string.Empty : connection.ToString());

            TreeNode tn = new TreeNode(label);
            var defaultSettingsToAndConnection =
                new KeyValuePair<Stresstest.Stresstest, Stresstest.Connection>(defaultSettingsTo, connection);

            tn.Tag = defaultSettingsToAndConnection;
            tn.ToolTipText =
                "The used connection defines the name of the tile stresstest.\nOther settings are defaulted to " +
                defaultSettingsTo.ToString();
            tileNode.Nodes.Add(tn);
            return tn;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="previous">Can be null</param>
        /// <returns></returns>
        private Stresstest.Stresstest GetNextDefaultToStresstest(Stresstest.Stresstest previous)
        {
            SolutionComponent stresstestProject = Solution.ActiveSolution.GetSolutionComponent(typeof(Stresstest.StresstestProject));
            if (previous != null)
            {
                bool previousFound = false;
                foreach (BaseItem item in stresstestProject)
                    if (item is Stresstest.Stresstest)
                        if (item == previous)
                            previousFound = true; //The next item of the correct type will be returned if any
                        else if (previousFound)
                            return item as Stresstest.Stresstest;
            }

            //If next was not found (starts with the first item again if any to use for a previous default to).
            return SolutionComponent.GetNextOrEmptyChild(typeof(Stresstest.Stresstest), stresstestProject) as Stresstest.Stresstest;
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure? All made changes will be rejeted!", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
            {
                //Store the index so the same node can be selected again.
                int level0Index, level1Index;
                GetIndexOfSelectedNode(out level0Index, out level1Index);

                FillTreeView();
                FillConnections();

                tvw.SelectedNode = GetNodeNodeByIndex(level0Index, level1Index);
            }
        }
        private void GetIndexOfSelectedNode(out int level0Index, out int level1Index)
        {
            level0Index = 0;
            level1Index = -1;
            if (tvw.SelectedNode != null)
                foreach (TreeNode tileNode in tvw.Nodes)
                {
                    level1Index = -1;

                    if (tileNode == tvw.SelectedNode)
                        return;
                    foreach (TreeNode tileStresstestNode in tileNode.Nodes)
                    {
                        ++level1Index;
                        if (tileStresstestNode == tvw.SelectedNode)
                            return;
                    }

                    ++level0Index;
                }
        }
        private TreeNode GetNodeNodeByIndex(int level0Index, int level1Index)
        {
            int i = 0;
            foreach (TreeNode tileNode in tvw.Nodes)
                if (i++ == level0Index)
                    if (level1Index == -1)
                    {
                        return tileNode;
                    }
                    else
                    {
                        int j = 0;
                        foreach (TreeNode tileStresstestNode in tileNode.Nodes)
                            if (j++ == level1Index)
                                return tileStresstestNode;
                    }

            return null;
        }
        private void tvw_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //The right panel should only be visible when a tile stresstest node is selected.
            if (tvw.SelectedNode != null)
                foreach (Control ctrl in split.Panel2.Controls)
                    ctrl.Visible = tvw.SelectedNode.Level != 0;

            //Set the default to and the used connection in the right panel.
            if (tvw.SelectedNode.Tag != null)
            {
                var defaultSettingsToAndConnection = (KeyValuePair<Stresstest.Stresstest, Stresstest.Connection>)tvw.SelectedNode.Tag;
                lblDefaultTo.Text =
                    "The used connection defines the name of the tile stresstest.\nOther settings are defaulted to " +
                    defaultSettingsToAndConnection.Key.ToString();

                foreach (RadioButton rdb in flp.Controls)
                    if (rdb.Tag == defaultSettingsToAndConnection.Value)
                    {
                        rdb.Checked = true;
                        break;
                    }
            }
        }

        /// <summary>
        /// Closes the dialog, sets the _toAssignConnections list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            int connectionIndex = 0;
            foreach (TreeNode tileNode in tvw.Nodes)
                foreach (TreeNode tileStresstestNode in tileNode.Nodes)
                {
                    var kvp = (KeyValuePair<Stresstest.Stresstest, Stresstest.Connection>)tileStresstestNode.Tag;
                    _toAssignConnections[connectionIndex++] = kvp.Value;
                }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion
    }
}
