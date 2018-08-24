/*
 * 2011 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.StressTest {
    public partial class RedetermineTokens : Form {
        #region Fields
        private readonly Scenario _originalScenario, _toAdd;
        private bool _requestContainsTokens;
        private int _preferredTokenDelimiterIndexBak;
        #endregion

        #region Constructors
        /// <summary>
        ///     Design time constructor.
        /// </summary>
        public RedetermineTokens() { InitializeComponent(); }

        public RedetermineTokens(Scenario scenario)
            : this(scenario, scenario) {
            lblDescription.Visible = btnOK.Enabled = false;
        }

        /// <summary>
        /// Compare the delimiters from the requests to add with the scenario that is already there and redetermine accordingly.
        /// </summary>
        /// <param name="originalScenario"></param>
        /// <param name="toAdd"></param>
        public RedetermineTokens(Scenario originalScenario, Scenario toAdd)
            : this() {
            _originalScenario = originalScenario;
            _originalScenario.ApplyScenarioRuleSet();

            _toAdd = toAdd;
            if (_originalScenario != _toAdd)
                _toAdd.ApplyScenarioRuleSet();

            _preferredTokenDelimiterIndexBak = toAdd.PreferredTokenDelimiterIndex;

            string begin, end;
            bool requestContainsTokens;
            _toAdd.GetParameterTokenDelimiters(out begin, out end, out requestContainsTokens, false);

            lblCurrentBegin.Text = lblNewBegin.Text = begin;
            lblCurrentEnd.Text = lblNewEnd.Text = end;
        }
        #endregion

        #region Functions
        private void btnPrevious_Click(object sender, EventArgs e) {
            int preferredTokenDelimiterIndex = _toAdd.PreferredTokenDelimiterIndex;

            int i = 0;
            while (_toAdd.PreferredTokenDelimiterIndex == preferredTokenDelimiterIndex)
                DetermineDelimiters(--i);
        }
        private void btnNext_Click(object sender, EventArgs e) {
            DetermineDelimiters(1);
        }
        private void DetermineDelimiters(int step) {
            _toAdd.PreferredTokenDelimiterIndex += step;

            string begin, end;
            _toAdd.GetParameterTokenDelimiters(out begin, out end, out _requestContainsTokens, false);

            lblNewBegin.Text = begin;
            lblNewEnd.Text = end;

            if (_requestContainsTokens) {
                lblNewBegin.ForeColor = Color.DarkOrange;
                lblNewEnd.ForeColor = Color.DarkOrange;
                btnError.Visible = true;
                btnOK.Enabled = true;
            }  else if (begin == lblCurrentBegin.Text) {
                lblNewBegin.ForeColor = Color.DarkGray;
                lblNewEnd.ForeColor = Color.DarkGray;
                btnError.Visible = false;
                btnOK.Enabled = _originalScenario != _toAdd;
            } else {
                lblNewBegin.ForeColor = Color.Black;
                lblNewEnd.ForeColor = Color.Black;
                btnError.Visible = false;
                btnOK.Enabled = true;
            }
        }

        private void btnOK_Click(object sender, EventArgs e) {
            if (_requestContainsTokens && MessageBox.Show("Are you sure you want to use these delimiters, this can make the scenario invalid!",
                string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No) {
                return;
            }

            if (lblCurrentBegin.Text != lblNewBegin.Text) {
                _originalScenario.PreferredTokenDelimiterIndex = _toAdd.PreferredTokenDelimiterIndex;

                //Old indices are equal to the new ones.
                var oldAndNewIndices = new Dictionary<BaseParameter, KeyValuePair<int, int>>();
                var parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
                foreach (BaseParameter parameter in parameters.GetAllParameters())
                    oldAndNewIndices.Add(parameter, new KeyValuePair<int, int>(parameter.TokenNumericIdentifier, parameter.TokenNumericIdentifier));

                _originalScenario.SynchronizeTokens(oldAndNewIndices, new KeyValuePair<string, string>(lblCurrentBegin.Text, lblNewBegin.Text), new KeyValuePair<string, string>(lblCurrentEnd.Text, lblNewEnd.Text));
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnError_Click(object sender, EventArgs e) {
            MessageBox.Show(
                "The chosen delimiters occur in the request strings.\nAre you sure you want to use these, this can make the scenario invalid!",
                string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void RedetermineTokens_FormClosing(object sender, FormClosingEventArgs e) {
            if (DialogResult == DialogResult.Cancel)
                _toAdd.PreferredTokenDelimiterIndex = _preferredTokenDelimiterIndexBak;
        }
        #endregion
    }
}