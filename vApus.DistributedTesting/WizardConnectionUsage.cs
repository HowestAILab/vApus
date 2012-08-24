/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.DistributedTesting
{
    public partial class WizardConnectionUsage : Form
    {
        private Tiles _tilesAlreadyInDistributedTest = new Tiles();
        private int _numberOfNewTiles, _numberOfTestsPerNewTile;
        /// <summary>
        /// Please call Init(..) after making a new WizardConnectionUsage.
        /// </summary>
        public WizardConnectionUsage()
        {
            InitializeComponent();
        }
        public void Init(Tiles tilesAlreadyInDistributedTest, int numberOfNewTiles, int numberOfTestsPerNewTile)
        {
            _tilesAlreadyInDistributedTest = tilesAlreadyInDistributedTest;
            _numberOfNewTiles = numberOfNewTiles;
            _numberOfTestsPerNewTile = numberOfTestsPerNewTile;

            FillTreeView();
        }
        private void FillTreeView()
        {
            tvw.Nodes.Clear();
            foreach (Tile tile in _tilesAlreadyInDistributedTest)
            {
                TreeNode tileNode = AddNewTileNode();
                foreach (TileStresstest tileStresstest in tile)
                {
                    TreeNode testNode = AddNewTestNode(tileNode, tileStresstest.DefaultSettingsTo, tileStresstest.BasicTileStresstest.Connection);
                }
            }

            Stresstest.Stresstest defaultTo = null;
            for (int t = 0; t != _numberOfNewTiles; t++)
            {
                TreeNode tileNode = AddNewTileNode();
                for (int ts = 0; ts != _numberOfTestsPerNewTile; ts++)
                {
                    defaultTo = GetNextDefaultToStresstest(defaultTo);
                    TreeNode testNode = AddNewTestNode(tileNode, defaultTo, defaultTo.Connection);
                    //Connection must be determined different, all connections must be used.
                }
            }
            tvw.ExpandAll();
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
                string.Empty : defaultSettingsTo.Connection.ToString());

            TreeNode tn = new TreeNode(label);
            KeyValuePair<Stresstest.Stresstest, Stresstest.Connection> defaultSettingsToAndConnection
                = new KeyValuePair<Stresstest.Stresstest, Stresstest.Connection>(defaultSettingsTo, connection);

            tn.Tag = defaultSettingsToAndConnection;
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
                FillTreeView();
        }
    }
}
