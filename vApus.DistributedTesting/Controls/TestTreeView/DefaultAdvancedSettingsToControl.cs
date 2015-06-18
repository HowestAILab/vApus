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

namespace vApus.DistributedTest {
    /// <summary>
    ///     Merges a cbo and a checkbox. A bit dirty, but works well.
    /// </summary>
    public partial class DefaultAdvancedSettingsToControl : UserControl {
        private BaseProject _stressTestProject;
        private TileStressTest _tileStressTest;

        public DefaultAdvancedSettingsToControl() {
            InitializeComponent();
        }

        public bool DefaultToChecked {
            get { return chkDefaultTo.Checked; }
        }

        public event EventHandler CheckChanged;

        /// <summary>
        ///     Fill the cbo, selects the right stress test, checks the chekbox.
        /// </summary>
        /// <param name="tileStressTest"></param>
        public void Init(TileStressTest tileStressTest) {
            chkDefaultTo.CheckedChanged -= chkDefaultTo_CheckedChanged;
            cboStressTests.SelectedIndexChanged -= cboStressTests_SelectedIndexChanged;

            _stressTestProject = Solution.ActiveSolution.GetProject("StressTestProject");

            _tileStressTest = tileStressTest;

            cboStressTests.Items.Clear();
            foreach (BaseItem item in _stressTestProject)
                if (item is StressTest.StressTest)
                    cboStressTests.Items.Add(item);

            if (cboStressTests.Items.Count == 0) {
                cboStressTests.Enabled = false;
            } else {
                cboStressTests.Enabled = true;
                cboStressTests.SelectedItem = _tileStressTest.DefaultAdvancedSettingsTo;
            }
            chkDefaultTo.Checked = _tileStressTest.AutomaticDefaultAdvancedSettings;

            chkDefaultTo.CheckedChanged += chkDefaultTo_CheckedChanged;
            cboStressTests.SelectedIndexChanged += cboStressTests_SelectedIndexChanged;
        }

        private void chkDefaultTo_CheckedChanged(object sender, EventArgs e) {
            if (chkDefaultTo.Checked != _tileStressTest.AutomaticDefaultAdvancedSettings && CheckChanged != null)
                CheckChanged(this, null);
        }

        private void cboStressTests_SelectedIndexChanged(object sender, EventArgs e) {
            if (cboStressTests.SelectedItem != _tileStressTest.DefaultAdvancedSettingsTo) {
                _tileStressTest.DefaultAdvancedSettingsTo = cboStressTests.SelectedItem as StressTest.StressTest;
                _tileStressTest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }
    }
}