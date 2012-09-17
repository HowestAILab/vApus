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

namespace vApus.Stresstest
{
    public partial class RedetermineTokens : Form
    {
        private int _originalPreferredTokenDelimiterIndex;
        private Log _log;
        private bool _warning, _error;

        /// <summary>
        /// Design time constructor.
        /// </summary>
        public RedetermineTokens()
        {
            InitializeComponent();
        }

        public RedetermineTokens(Log log)
            : this()
        {
            _log = log;
            _log.ApplyLogRuleSet();

            _originalPreferredTokenDelimiterIndex = _log.PreferredTokenDelimiterIndex;

            string begin, end;
            bool warning, error;
            _log.GetUniqueParameterTokenDelimiters(out begin, out end, out warning, out error);

            lblCurrentBegin.Text = begin;
            lblCurrentEnd.Text = end;
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            int preferredTokenDelimiterIndex = _log.PreferredTokenDelimiterIndex;

            int i = 0;
            while (_log.PreferredTokenDelimiterIndex == preferredTokenDelimiterIndex)
                DetermineDelimiters(--i);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            DetermineDelimiters(1);
        }

        private void DetermineDelimiters(int step)
        {
            _log.PreferredTokenDelimiterIndex += step;

            string begin, end;
            _log.GetUniqueParameterTokenDelimiters(out begin, out end, out _warning, out _error, false);

            lblNewBegin.Text = begin;
            lblNewEnd.Text = end;

            if (begin == lblCurrentBegin.Text)
            {
                lblNewBegin.ForeColor = Color.DarkGray;
                lblNewEnd.ForeColor = Color.DarkGray;
                btnWarning.Visible = false;
                btnError.Visible = false;
            }
            else if (_error)
            {
                lblNewBegin.ForeColor = Color.Red;
                lblNewEnd.ForeColor = Color.Red;
                btnWarning.Visible = true;
                btnError.Visible = true;
            }
            else if (_warning)
            {
                lblNewBegin.ForeColor = Color.DarkOrange;
                lblNewEnd.ForeColor = Color.DarkOrange;
                btnWarning.Visible = true;
                btnError.Visible = false;
            }
            else
            {
                lblNewBegin.ForeColor = Color.Black;
                lblNewEnd.ForeColor = Color.Black;
                btnWarning.Visible = false;
                btnError.Visible = false;
            }

            btnOK.Enabled = begin != lblCurrentBegin.Text;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if ((_warning || _error) &&
                        MessageBox.Show("Are you sure you want to use these delimiters, this can make the log invalid!", string.Empty, MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }


            //Old indices are equal to the new ones.
            Dictionary<BaseParameter, KeyValuePair<int, int>> oldAndNewIndices = new Dictionary<BaseParameter, KeyValuePair<int, int>>();
            Parameters parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
            foreach (BaseParameter parameter in parameters.GetAllParameters())
                oldAndNewIndices.Add(parameter, new KeyValuePair<int, int>(parameter.TokenNumericIdentifier, parameter.TokenNumericIdentifier));

            _log.SynchronizeTokens(oldAndNewIndices, new KeyValuePair<string, string>(lblCurrentBegin.Text, lblNewBegin.Text), new KeyValuePair<string, string>(lblCurrentEnd.Text, lblNewEnd.Text));

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _log.PreferredTokenDelimiterIndex = _originalPreferredTokenDelimiterIndex;
        }

        private void btnWarning_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The chosen delimiters occur in the log entries as they were imported but not necessarely in the editted log entry strings.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void btnError_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The chosen delimiters occur in the editted log entry strings.\nAre you sure you want to use these, this can make the log invalid!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
