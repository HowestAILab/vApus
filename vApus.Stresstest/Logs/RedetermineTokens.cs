/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.Stresstest {
    public partial class RedetermineTokens : Form {
        private readonly Log _originalLog, _toAdd;
        private bool _logEntryContainsTokens;

        /// <summary>
        ///     Design time constructor.
        /// </summary>
        public RedetermineTokens() {
            InitializeComponent();
        }

        /// <summary>
        /// </summary>
        /// <param name="originalLog"></param>
        /// <param name="toAdd"></param>
        public RedetermineTokens(Log originalLog, Log toAdd)
            : this() {
            _originalLog = originalLog;
            _originalLog.ApplyLogRuleSet();

            _toAdd = toAdd;
            _toAdd.ApplyLogRuleSet();

            string begin, end;
            bool logEntryContainsTokens;
            _toAdd.GetParameterTokenDelimiters(out begin, out end, out logEntryContainsTokens, false);

            lblCurrentBegin.Text = lblNewBegin.Text = begin;
            lblCurrentEnd.Text = lblNewEnd.Text = end;
        }

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
            _toAdd.GetParameterTokenDelimiters(out begin, out end, out _logEntryContainsTokens, false);

            lblNewBegin.Text = begin;
            lblNewEnd.Text = end;

            if (begin == lblCurrentBegin.Text) {
                lblNewBegin.ForeColor = Color.DarkGray;
                lblNewEnd.ForeColor = Color.DarkGray;
                btnError.Visible = false;
            } else if (_logEntryContainsTokens) {
                lblNewBegin.ForeColor = Color.DarkOrange;
                lblNewEnd.ForeColor = Color.DarkOrange;
                btnError.Visible = true;
            } else {
                lblNewBegin.ForeColor = Color.Black;
                lblNewEnd.ForeColor = Color.Black;
                btnError.Visible = false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e) {
            if (_logEntryContainsTokens && MessageBox.Show("Are you sure you want to use these delimiters, this can make the log invalid!",
                string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No) {
                return;
            }

            if (lblCurrentBegin.Text != lblNewBegin.Text) {
                _originalLog.PreferredTokenDelimiterIndex = _toAdd.PreferredTokenDelimiterIndex;

                //Old indices are equal to the new ones.
                var oldAndNewIndices = new Dictionary<BaseParameter, KeyValuePair<int, int>>();
                var parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
                foreach (BaseParameter parameter in parameters.GetAllParameters())
                    oldAndNewIndices.Add(parameter, new KeyValuePair<int, int>(parameter.TokenNumericIdentifier, parameter.TokenNumericIdentifier));

                _originalLog.SynchronizeTokens(oldAndNewIndices, new KeyValuePair<string, string>(lblCurrentBegin.Text, lblNewBegin.Text), new KeyValuePair<string, string>(lblCurrentEnd.Text, lblNewEnd.Text));
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnError_Click(object sender, EventArgs e) {
            MessageBox.Show(
                "The chosen delimiters occur in the log entry strings.\nAre you sure you want to use these, this can make the log invalid!",
                string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}