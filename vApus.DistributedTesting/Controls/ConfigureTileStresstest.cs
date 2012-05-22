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
            lblUsage.Visible = false;

            solutionComponentPropertyPanelDefaultTo.Visible =
            solutionComponentPropertyPanelBasic.Visible =
            llblAdvancedSettings.Visible =
            chkAutomatic.Visible = true;

            if (_tileStresstest != tileStresstest)
            {
                _tileStresstest = tileStresstest;
                solutionComponentPropertyPanelDefaultTo.SolutionComponent = _tileStresstest;
                solutionComponentPropertyPanelBasic.SolutionComponent = _tileStresstest.BasicTileStresstest;
                solutionComponentPropertyPanelAdvanced.SolutionComponent = _tileStresstest.AdvancedTileStresstest;


                chkAutomatic.Checked = _tileStresstest.AutomaticDefaultAdvancedSettings;

                Handle_chkAutomatic_CheckedChanged();
            }
        }
        public void ClearTileStresstest()
        {
            solutionComponentPropertyPanelDefaultTo.Visible =
            solutionComponentPropertyPanelBasic.Visible =
            llblAdvancedSettings.Visible =
            chkAutomatic.Visible = false;

            lblUsage.Visible = true;

        }
        private void chkAutomatic_CheckedChanged(object sender, EventArgs e)
        {
            Handle_chkAutomatic_CheckedChanged();
        }

        private void Handle_chkAutomatic_CheckedChanged()
        {
            if (chkAutomatic.Checked)
                solutionComponentPropertyPanelAdvanced.Lock();
            else
                solutionComponentPropertyPanelAdvanced.Unlock();

            if (_tileStresstest.AutomaticDefaultAdvancedSettings != chkAutomatic.Checked)
            {
                solutionComponentPropertyPanelAdvanced.Visible = true;
                _tileStresstest.AutomaticDefaultAdvancedSettings = chkAutomatic.Checked;
                _tileStresstest.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }

        private void llblAdvancedSettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            solutionComponentPropertyPanelAdvanced.Visible = !solutionComponentPropertyPanelAdvanced.Visible;
        }
    }
}
