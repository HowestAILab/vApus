/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    /// This panel is designed to group all available options/preferences for vApus.
    /// </summary>
    public partial class OptionsDialog : Form {
        private int _selectedPanel;

        public OptionsDialog() {
            InitializeComponent();
            HandleCreated += OptionsDialog_HandleCreated;
        }

        public int SelectedPanel {
            get { return _selectedPanel; }
            set {
                _selectedPanel = value;
                if (IsHandleCreated && tvw.Nodes.Count >= _selectedPanel)
                    tvw.SelectedNode = tvw.Nodes[_selectedPanel];
            }
        }

        private void OptionsDialog_HandleCreated(object sender, EventArgs e) {
            tvw.AfterSelect += tvw_AfterSelect;
            if (tvw.Nodes.Count >= _selectedPanel)
                tvw.SelectedNode = tvw.Nodes[_selectedPanel];
        }

        private void tvw_AfterSelect(object sender, TreeViewEventArgs e) {
            split.Panel2.Controls.Clear();
            var panel = tvw.SelectedNode.Tag as Panel;
            panel.Dock = DockStyle.Fill;
            split.Panel2.Controls.Add(panel);
            panel.Refresh();
        }

        /// <summary>
        ///     Do this before a handle is created.
        ///     The ToString() of the panel is the text in the treeview.
        /// </summary>
        /// <param name="optionsPanel"></param>
        public void AddOptionsPanel(Panel optionsPanel) {
            var node = new TreeNode(optionsPanel.ToString());
            node.Tag = optionsPanel;
            tvw.Nodes.Add(node);
        }
    }
}