using System;
using System.Windows.Forms;

namespace vApus.Util
{
    public partial class OptionsDialog : Form
    {
        private int _selectedPanel;

        public int SelectedPanel
        {
            get { return _selectedPanel; }
            set
            {
                _selectedPanel = value;
                if (IsHandleCreated && tvw.Nodes.Count >= _selectedPanel)
                    tvw.SelectedNode = tvw.Nodes[_selectedPanel];
            }
        }
        public OptionsDialog()
        {
            InitializeComponent();
            this.HandleCreated += new EventHandler(OptionsDialog_HandleCreated);
        }
        private void OptionsDialog_HandleCreated(object sender, EventArgs e)
        {
            tvw.AfterSelect += new TreeViewEventHandler(tvw_AfterSelect);
            if (tvw.Nodes.Count >= _selectedPanel)
                tvw.SelectedNode = tvw.Nodes[_selectedPanel];
        }
        private void tvw_AfterSelect(object sender, TreeViewEventArgs e)
        {
            split.Panel2.Controls.Clear();
            Panel panel = tvw.SelectedNode.Tag as Panel;
            panel.Dock = DockStyle.Fill;
            split.Panel2.Controls.Add(panel);
        }
        /// <summary>
        /// Do this before a handle is created.
        /// The ToString() of the panel is the text in the treeview.
        /// </summary>
        /// <param name="optionsPanel"></param>
        public void AddOptionsPanel(Panel optionsPanel)
        {
            TreeNode node = new TreeNode(optionsPanel.ToString());
            node.Tag = optionsPanel;
            tvw.Nodes.Add(node);
        }
    }
}