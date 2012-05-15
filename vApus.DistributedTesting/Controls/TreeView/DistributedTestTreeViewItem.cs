using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace vApus.DistributedTesting
{
    [ToolboxItem(false)]
    public partial class DistributedTestTreeViewItem : UserControl, ITestTreeViewItem
    {
        private DistributedTest _distributedTest = new DistributedTest();

        public DistributedTest DistributedTest
        {
            get { return _distributedTest; }
        }
     
        public DistributedTestTreeViewItem()
        {
            InitializeComponent();
        }
        public DistributedTestTreeViewItem(DistributedTest distributedTest)
            :this()
        {
            _distributedTest = distributedTest;
            cboRunSync.SelectedIndex = (int)distributedTest.RunSynchronization;
        }

        public void SetVisibleControls()
        {
        }


        public void SetVisibleControls(bool visible)
        {
        }
        public void RefreshGui()
        {
        }
    }
}
