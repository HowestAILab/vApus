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
    public partial class ConfigureTileStresstest : UserControl
    {
        private TileStresstest _tileStresstest;

        public TileStresstest TileStresstest
        {
            get { return _tileStresstest; }
        }

        public ConfigureTileStresstest()
        {
            InitializeComponent();
        }

        public void SetTileStresstest(TileStresstest tileStresstest)
        {
            if (_tileStresstest != tileStresstest)
            {
                _tileStresstest = tileStresstest;
                solutionComponentPropertyPanelDefaultTo.SolutionComponent = _tileStresstest;
                //solutionComponentPropertyPanelBasic.SolutionComponent = _tileStresstest.BasicTileStresstest;
                //solutionComponentPropertyPanelAdvanced.SolutionComponent = _tileStresstest.AdvancedTileStresstest;
            }
        }
    }
}
