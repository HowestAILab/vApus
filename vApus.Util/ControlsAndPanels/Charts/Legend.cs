/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 *    Glenn Desmadryl
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Util
{
    [ToolboxItem(false)]
    public partial class Legend : UserControl
    {
        #region Fields
        //Serves at using InvokeAllSeriesPropertiesChanged.
        private Series _lastSeries;
        #endregion

        #region Properties
        /// <summary>
        /// The number of series.
        /// </summary>
        public int Count
        {
            get
            {
                int counter = 0;
                foreach (TreeNode node in tvw.Nodes)
                    counter += node.Nodes.Count == 0 ? 1 : node.Nodes.Count;
                return counter;
            }
        }
        #endregion

        #region Constructor
        public Legend()
        {
            InitializeComponent();
        }
        #endregion

        #region Functions
        //This resizes the clm correctly so a horizontal scrollbar is never displayed.
        private void Legend_SizeChanged(object sender, EventArgs e)
        {
            //clmLegendItems.Width = lvw.Width - 21;
        }
        internal void AddSeries(Series series)
        {
            tvw.AfterCheck -= tvw_AfterCheck;
            _lastSeries = series;
            AddNode(series);

            chkToggle.Enabled = true;
            tvw.AfterCheck += tvw_AfterCheck;
        }

        private void AddNode(Series series)
        {
            bool found = false;
            TreeNode node = new TreeNode();

            foreach (TreeNode parent in tvw.Nodes)
                if (parent.Text.Equals(series.Label))
                {
                    node = new TreeNode(series.Instance);
                    parent.Nodes.Add(node);
                    found = true;
                }

            if (!found)
            {

                node = new TreeNode(series.Label);
                node.Checked = true;
                tvw.Nodes.Add(node);
                tvw.SelectedNode = node;

                if (series.Instance != String.Empty)
                {
                    node = new TreeNode(series.Instance);
                    tvw.Nodes[tvw.Nodes.Count - 1].Nodes.Add(node);
                }
            }

            try
            {
                node.Tag = series;

                node.Checked = true;
                node.BackColor = series.Color;
                if (node.BackColor.ToArgb() > int.MaxValue / 2)
                    node.ForeColor = Color.White;

                if (tvw.Nodes[0].GetNodeCount(false) > 0)
                    tvw.SelectedNode = tvw.Nodes[0].Nodes[0];
                else
                    tvw.SelectedNode = tvw.Nodes[0];
            }
            catch { }

        }

        private void chkToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (tvw.Nodes.Count != 0)
            {
                tvw.AfterCheck -= tvw_AfterCheck;
                switch (chkToggle.CheckState)
                {
                    case CheckState.Checked:

                        foreach (TreeNode node in tvw.Nodes)
                        {
                            node.Checked = true;
                            if (node.GetNodeCount(false) > 0)
                            {
                                foreach (TreeNode child in node.Nodes)
                                {
                                    child.Checked = true;
                                    (child.Tag as Series).Visible = true;
                                }
                            }
                            else
                                (node.Tag as Series).Visible = true;
                        }
                        tvw.SelectedNode = tvw.Nodes[0];

                        break;
                    case CheckState.Unchecked:
                        foreach (TreeNode node in tvw.Nodes)
                        {
                            node.Checked = false;
                            if (node.GetNodeCount(false) > 0)
                            {
                                foreach (TreeNode child in node.Nodes)
                                {
                                    child.Checked = false;
                                    (child.Tag as Series).Visible = false;
                                }
                            }
                            else
                                (node.Tag as Series).Visible = false;
                        }
                        tvw.SelectedNode = null;
                        break;
                }
                tvw.AfterCheck += tvw_AfterCheck;
            }
        }
        private void chooseColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = tvw.SelectedNode;
            colorDialog.Color = node.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                Series series = node.Tag as Series;
                series.Color = colorDialog.Color;
                node.BackColor = series.Color;
                if (node.BackColor.ToArgb() > int.MaxValue / 2)
                    node.ForeColor = Color.White;
                series.InvokeSeriesPropertiesChanged();
            }
        }
        private void cmnu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tvw.SelectedNode == null)
                e.Cancel = true;
        }
        private void tvw_AfterCheck(object sender, TreeViewEventArgs e)
        {
            chkToggle.CheckedChanged -= chkToggle_CheckedChanged;
            tvw.AfterCheck -= tvw_AfterCheck;
            if (e.Node.Nodes.Count != 0)
                foreach (TreeNode child in e.Node.Nodes)
                {
                    child.Checked = e.Node.Checked;
                    (child.Tag as Series).Visible = e.Node.Checked;
                }
            else
                (e.Node.Tag as Series).Visible = e.Node.Checked;

            if (!e.Node.Checked && tvw.SelectedNode != null)
                foreach (TreeNode child in e.Node.Nodes)
                    if (child == tvw.SelectedNode)
                    {
                        tvw.SelectedNode = null;
                        break;
                    }

            if (tvw.SelectedNode != null)
            {
                if (tvw.SelectedNode.Nodes.Count > 0)
                    foreach (TreeNode child in tvw.SelectedNode.Nodes)
                    {
                        (child.Tag as Series).InvokeSeriesPropertiesChanged();
                        tvw.SelectedNode = child;
                    }
                else
                {
                    (tvw.SelectedNode.Tag as Series).InvokeSeriesPropertiesChanged();
                    tvw.AfterSelect -= tvw_AfterSelect;
                    tvw.SelectedNode = e.Node;
                    tvw.AfterSelect += tvw_AfterSelect;
                }
            }
            //check if the parent has to be checked or not
            CheckIfParentHasToBeChecked(e.Node.Parent);

            //Set the toggle checkbox.
            int count = tvw.GetNodeCount(true), checkedCount = 0;
            foreach (TreeNode parent in tvw.Nodes)
            {
                if (parent.Checked) checkedCount++;
                foreach (TreeNode node in parent.Nodes)
                    if (node.Checked) checkedCount++;
            }

            if (checkedCount == 0)
                chkToggle.CheckState = CheckState.Unchecked;
            else if (checkedCount == count)
                chkToggle.CheckState = CheckState.Checked;
            else
                chkToggle.CheckState = CheckState.Indeterminate;

            chkToggle.CheckedChanged += chkToggle_CheckedChanged;
            tvw.AfterCheck += tvw_AfterCheck;
        }

        /// <summary>
        /// Returns the first checked child (null if not found)
        /// </summary>
        /// <param name="parent">Node to search through</param>
        /// <returns>the node itself it there are no children, null if no child has been checked, the child if found</returns>
        private TreeNode getFirstCheckedChild(TreeNode parent)
        {
            if (parent.GetNodeCount(false) == 0)
                return parent;
            else
            {
                foreach (TreeNode child in parent.Nodes)
                    if (child.Checked)
                        return child;
                return null;
            }
        }
        /// <summary>
        /// Checks if the parent when it exists has to be checked or not
        /// </summary>
        /// <param name="parent">The parent node to check for</param>
        private void CheckIfParentHasToBeChecked(TreeNode parent)
        {
            //check if the parent has to be checked or not
            if (parent != null)
            {
                int checkedChilds = 0;
                foreach (TreeNode child in parent.Nodes)
                    if (child.Checked)
                        checkedChilds++;
                parent.Checked = checkedChilds.Equals(parent.GetNodeCount(false)) ? true : false;

            }
        }

        private void tvw_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvw.SelectedNode != null)
            {
                try
                {
                    //if we select Host and Host has instances under it, we select the first instance
                    if (tvw.SelectedNode.GetNodeCount(false) != 0)
                    {
                        //if we have to select a child we make sure that the child is checked
                        tvw.AfterSelect -= tvw_AfterSelect;
                        tvw.SelectedNode = getFirstCheckedChild(tvw.SelectedNode);
                        tvw.AfterSelect += tvw_AfterSelect;
                    }


                    if (tvw.SelectedNode.Tag == null)
                        return;

                    //Unhighlight all.
                    foreach (TreeNode parent in tvw.Nodes)
                        if (parent.Tag == null)
                        {
                            foreach (TreeNode node in parent.Nodes)
                                if (node.Tag != null)
                                    (node.Tag as Series).IsHighlighted = false;
                        }
                        else
                        {
                            (parent.Tag as Series).IsHighlighted = false;
                        }

                    Series series = tvw.SelectedNode.Tag as Series;
                    series.IsHighlighted = true;
                    //tvw.AfterCheck -= tvw_AfterCheck;
                    tvw.SelectedNode.Checked = true;
                    //tvw.AfterCheck += tvw_AfterCheck;
                    series.InvokeSeriesPropertiesChanged();

                }
                catch { }

            }
            else
                _lastSeries.InvokeAllSeriesPropertiesChanged();
        }
        #endregion
    }
}
