/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using vApus.Stresstest;

namespace vApus.LogFixer
{
    public partial class ApplyFix : Form
    {
        #region Fields

        private readonly Line _line;
        private readonly Lines _lines;
        private readonly TrackedChanges _trackedChanges;

        private Help _help;
        private Scenario _scenario;

        #endregion

        public ApplyFix()
        {
            InitializeComponent();
        }

        public ApplyFix(Lines lines)
            : this()
        {
            _lines = lines;

            SetPreview();

            largeList.HandleCreated += largeList_HandleCreated;
        }

        public ApplyFix(Line line, TrackedChanges trackedChanges)
            : this()
        {
            _line = line;
            _lines = _line.Parent;
            _trackedChanges = trackedChanges;

            SetPreview();

            largeList.HandleCreated += largeList_HandleCreated;
        }

        private void SetPreview()
        {
            int okCount = 0, errorCount = 0;
            for (int i = 0; i != _lines.Count; i++)
            {
                Line l = _lines[i];
                if ((_line != null && l == _line) || l.LogEntry == null)
                    continue;

                var flc = new FixLineControl(l);

                if (l.LexicalResult == LexicalResult.OK)
                {
                    flc.Visible = false;
                    ++okCount;
                }
                else
                {
                    flc.Visible = true;
                    flc.Checked = true;
                    ++errorCount;
                }

                flc.CheckedChanged += flc_CheckedChanged;

                largeList.Add(flc);
            }

            tbtnPreviewOK.Text = "     " + okCount;
            tbtnPreviewError.Text = "     " + errorCount;
            tbtnPreviewError.Tag = errorCount;

            chkAll.Text = errorCount.ToString();
            chkAll.CheckedChanged -= chkAll_CheckedChanged;
            chkAll.CheckState = CheckState.Checked;
            chkAll.CheckedChanged += chkAll_CheckedChanged;
        }

        private void flc_CheckedChanged(object sender, EventArgs e)
        {
            int check = 0, visible = 0;
            foreach (FixLineControl flc in FixLineControls())
            {
                if (flc.Checked)
                    ++check;
                if (flc.Visible)
                    ++visible;
            }

            SetChkAll(check, visible);
            btnApply.Enabled = chkAll.CheckState != CheckState.Unchecked;
        }

        private void SetChkAll(int check, int visible)
        {
            chkAll.Text = check.ToString();

            chkAll.CheckedChanged -= chkAll_CheckedChanged;
            if (check == 0)
                chkAll.CheckState = CheckState.Unchecked;
            else if (check == visible)
                chkAll.CheckState = CheckState.Checked;
            else
                chkAll.CheckState = CheckState.Indeterminate;
            chkAll.CheckedChanged += chkAll_CheckedChanged;
        }

        private void largeList_HandleCreated(object sender, EventArgs e)
        {
            largeList.HandleCreated -= largeList_HandleCreated;
            SetScenarios(_trackedChanges);
        }

        private void SetScenarios(TrackedChanges trackedChanges = null)
        {
            if (trackedChanges == null)
                _scenario = new Scenario();
            else
                _scenario = new Scenario(trackedChanges);

            rtxtScenario.Text = _scenario.ToString();
            PreviewScenario();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Scenario scenario;
            Scenario.TryParse(rtxtScenario.Text, out scenario);
            foreach (FixLineControl flc in FixLineControls())
                if (flc.Checked)
                    flc.Line.LogEntry.LogEntryString = scenario.Apply(flc.Line.LogEntry.LogEntryString);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void tbtnPreviewOK_CheckedChanged(object sender, EventArgs e)
        {
            int check = 0, visible = 0;
            foreach (FixLineControl flc in FixLineControls())
            {
                if (flc.Line.LexicalResult == LexicalResult.OK)
                    flc.Visible = tbtnPreviewOK.Checked;

                if (flc.Visible)
                    ++visible;
                else
                    flc.Checked = false;

                if (flc.Checked)
                    ++check;
            }
            SetChkAll(check, visible);
            btnApply.Enabled = chkAll.CheckState != CheckState.Unchecked;
        }

        private void tbtnPreviewError_CheckedChanged(object sender, EventArgs e)
        {
            int check = 0, visible = 0;
            foreach (FixLineControl flc in FixLineControls())
            {
                if (flc.Line.LexicalResult == LexicalResult.Error)
                    flc.Visible = tbtnPreviewError.Checked;

                if (flc.Visible)
                    ++visible;
                else
                    flc.Checked = false;

                if (flc.Checked)
                    ++check;
            }
            SetChkAll(check, visible);
            btnApply.Enabled = chkAll.CheckState != CheckState.Unchecked;
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAll.CheckState != CheckState.Indeterminate)
            {
                int check = 0;
                foreach (FixLineControl flc in FixLineControls())
                    if (flc.Visible)
                    {
                        flc.CheckedChanged -= flc_CheckedChanged;
                        flc.Checked = chkAll.Checked;
                        if (flc.Checked)
                            ++check;
                        flc.CheckedChanged += flc_CheckedChanged;
                    }

                chkAll.Text = check.ToString();
                btnApply.Enabled = chkAll.CheckState == CheckState.Checked;
            }
        }

        public IEnumerable<FixLineControl> FixLineControls()
        {
            for (int i = 0; i != largeList.ViewCount; i++)
                foreach (FixLineControl elc in largeList[i])
                    yield return elc;
        }

        private void rtxtScenario_TextChanged(object sender, EventArgs e)
        {
            PreviewScenario();
        }

        private void PreviewScenario()
        {
            Scenario scenario;
            Scenario.TryParse(rtxtScenario.Text, out scenario);

            int previewedErrorCount = 0;
            foreach (FixLineControl flc in FixLineControls())
                if (flc.SetPreview(scenario.Apply(flc.Line.LogEntry.LogEntryString)) && flc.Checked)
                    ++previewedErrorCount;

            var errorCount = (int) tbtnPreviewError.Tag;
            if (previewedErrorCount == errorCount)
                tbtnPreviewError.Text = "     " + errorCount;
            else
                tbtnPreviewError.Text = "     " + errorCount + " --> " + previewedErrorCount;
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            if (_help == null)
                _help = new Help();

            _help.Show(this);
        }
    }
}