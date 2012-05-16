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
    public partial class NewDistributedTestView : BaseSolutionComponentView
    {
        public NewDistributedTestView()
        {
            InitializeComponent();
        }
        public NewDistributedTestView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            InitializeComponent();

            testTreeView.SetDistributedTest(solutionComponent as DistributedTest);

        }

        private void testTreeView_AfterSelect(object sender, EventArgs e)
        {
            if (sender is TileStresstestTreeViewItem)
            {
                TileStresstestTreeViewItem tstvi = sender as TileStresstestTreeViewItem;
                configureTileStresstest.SetTileStresstest(tstvi.TileStresstest);
            }
        }
    }
}
