/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.DistributedTesting
{
    /// <summary>
    ///     Merges a cbo and a checkbox. A bit dirty but works well.
    /// </summary>
    public partial class DefaultAdvancedSettingsToControl : UserControl
    {
        private BaseProject _stresstestProject;
        private TileStresstest _tileStresstest;

        public DefaultAdvancedSettingsToControl()
        {
            InitializeComponent();
        }

        public bool DefaultToChecked
        {
            get { return chkDefaultTo.Checked; }
        }

        public event EventHandler CheckChanged;

        /// <summary>
        ///     Fill the cbo, selects the right stresstest, checks the chekbox.
        /// </summary>
        /// <param name="tileStresstest"></param>
        public void Init(TileStresstest tileStresstest)
        {
            chkDefaultTo.CheckedChanged -= chkDefaultTo_CheckedChanged;
            cboStresstests.SelectedIndexChanged -= cboStresstests_SelectedIndexChanged;

            _stresstestProject = Solution.ActiveSolution.GetProject("StresstestProject");

            _tileStresstest = tileStresstest;

            cboStresstests.Items.Clear();
            foreach (BaseItem item in _stresstestProject)
                if (item is Stresstest.Stresstest)
                    cboStresstests.Items.Add(item);

            if (cboStresstests.Items.Count == 0)
            {
                cboStresstests.Enabled = false;
            }
            else
            {
                cboStresstests.Enabled = true;
                cboStresstests.SelectedItem = _tileStresstest.DefaultAdvancedSettingsTo;
            }
            chkDefaultTo.Checked = _tileStresstest.AutomaticDefaultAdvancedSettings;

            chkDefaultTo.CheckedChanged += chkDefaultTo_CheckedChanged;
            cboStresstests.SelectedIndexChanged += cboStresstests_SelectedIndexChanged;
        }

        private void chkDefaultTo_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDefaultTo.Checked != _tileStresstest.AutomaticDefaultAdvancedSettings && CheckChanged != null)
                CheckChanged(this, null);
        }

        private void cboStresstests_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboStresstests.SelectedItem != _tileStresstest.DefaultAdvancedSettingsTo)
            {
                _tileStresstest.DefaultAdvancedSettingsTo = cboStresstests.SelectedItem as Stresstest.Stresstest;
                _tileStresstest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }
    }
}