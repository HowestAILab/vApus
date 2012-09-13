/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using vApus.Stresstest;

namespace vApus.LogFixer
{
    [ToolboxItem(true)]
    public partial class FixTab : TabPage
    {
        #region Fields
        private Log _log;
        private Lines _lines = new Lines();
        private static string _multilineComment = string.Empty;
        #endregion

        public FixTab()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Filenames
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="logRuleSetFileName"></param>
        public void Init(string logFileName, string logRuleSetFileName)
        {
            string logText = null;
            LogRuleSet logRuleSet = null;
            if (File.Exists(logFileName))
            {
                using (var sw = new StreamReader(logFileName))
                    logText = sw.ReadToEnd();

                if (File.Exists(logRuleSetFileName))
                {
                    LogRuleSets logRuleSets = new LogRuleSets();
                    logRuleSet = new LogRuleSet();

                    string err;
                    XmlDocument doc = new XmlDocument();
                    doc.Load(logRuleSetFileName);

                    logRuleSets.LoadFromXml(doc.FirstChild, out err);
                    logRuleSet = logRuleSets[0] as LogRuleSet;
                }
                else
                {
                    logText = "<< File not found! >>";
                }
            }
            else
            {
                logText = "<< File not found! >>";
            }

            SetLog(logText, logRuleSet);

            Text = logFileName;

            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(FixTab_HandleCreated);
        }

        private void FixTab_HandleCreated(object sender, EventArgs e)
        {
            this.HandleCreated -= FixTab_HandleCreated;
            SetGui();
        }

        private void SetLog(string logText, LogRuleSet logRuleSet)
        {
            _log = new Log();
            _log.LogRuleSet = logRuleSet;

            logText = logText.Replace(Environment.NewLine, "\n").Replace('\r', '\n');
            foreach (string line in logText.Split('\n'))
            {
                if (line.Trim().Length == 0)
                    continue;

                string output;
                if (_log.LogRuleSet == null)
                {
                    _lines.Add(new Line(_lines, line, true));
                }
                else if (DetermineComment(line, out output))
                {
                    _lines.Add(new Line(_lines, line));
                }
                else
                {
                    LogEntry logEntry = new LogEntry(output);
                    _log.Add(logEntry);

                    _lines.Add(new Line(_lines, logEntry));
                }
            }
            if (_log.LogRuleSet != null)
                _log.ApplyLogRuleSet();
        }
        private bool DetermineComment(string input, out string output)
        {
            int singleline = -1;
            int multiline = -1;

            input = input.TrimStart().Trim();
            output = input;

            if (_multilineComment.Length > 0)
            {
                multiline = input.IndexOf(_log.LogRuleSet.EndCommentString);
                if (multiline > -1)
                {
                    output = input.Substring(multiline + _log.LogRuleSet.EndCommentString.Length);
                    if (output.Length == 0)
                    {
                        if (input.Length > _log.LogRuleSet.EndCommentString.Length + 1)
                            output = FormatComment(_multilineComment.TrimStart() + ' ' + input.Substring(0, input.Length - _log.LogRuleSet.EndCommentString.Length));
                        else
                            output = FormatComment(_multilineComment.TrimStart());
                        _multilineComment = string.Empty;
                        return true;
                    }
                    _multilineComment = string.Empty;
                    output = input.Substring(multiline + _log.LogRuleSet.EndCommentString.Length);
                    return true;
                }
                if (input.Length > 0)
                    _multilineComment = _multilineComment + ' ' + input;
                output = string.Empty;
                return true;
            }

            if (_log.LogRuleSet.SingleLineCommentString.Length > 0)
                singleline = input.IndexOf(_log.LogRuleSet.SingleLineCommentString);
            if (_log.LogRuleSet.BeginCommentString.Length > 0)
                multiline = input.IndexOf(_log.LogRuleSet.BeginCommentString);

            if (singleline > -1 && multiline == -1)
                multiline = int.MaxValue;
            else if (multiline > -1 && singleline == -1)
                singleline = int.MaxValue;

            if (singleline > -1 && singleline < multiline)
            {
                _multilineComment = string.Empty;
                if (singleline == 0)
                    output = FormatComment(input.Substring(_log.LogRuleSet.SingleLineCommentString.Length));
                return true;
            }
            else if (multiline > -1 && multiline < singleline)
            {
                int multilineCopy = input.IndexOf(_log.LogRuleSet.EndCommentString);
                if (multilineCopy > -1 && multilineCopy > multiline)
                {
                    _multilineComment = string.Empty;
                    output = input.TrimStart().Substring(0, multiline) + input.Substring(multilineCopy + _log.LogRuleSet.EndCommentString.Length);
                    if (output.Length == 0)
                        output = FormatComment(input.Substring(multiline + _log.LogRuleSet.BeginCommentString.Length, input.Length - _log.LogRuleSet.EndCommentString.Length - _log.LogRuleSet.BeginCommentString.Length));
                    return true;
                }
                else
                {
                    _multilineComment = input.Substring(multiline + _log.LogRuleSet.BeginCommentString.Length);
                    if (_multilineComment.Length == 0)
                        _multilineComment = " ";
                    output = input.Substring(0, multiline);
                    return true;
                }
            }
            return false;
        }
        private string FormatComment(string input)
        {
            int i = 0;
            input = input.TrimStart();
            StringBuilder sb = new StringBuilder(255);
            foreach (char c in input)
            {
                sb.Append(c);
                if (++i == 255)
                    break;
            }
            return sb.ToString();
        }
        private void SetGui()
        {
            int ok, error;
            SetGuiLL(out ok, out error);

            tbtnOK.Text = "     " + ok;
            tbtnOK.Visible = ok != 0;

            tbtnError.Text = "     " + error;
            tbtnError.Visible = error != 0;

            btnRestoreAll.Visible = false;
        }
        private void SetGuiLL(out int ok, out int error)
        {
            largeList.RemoveAll();

            ok = 0;
            error = 0;

            foreach (Line line in _lines)
            {
                var elc = new EditLineControl(line);
                elc.ButtonClicked += new EventHandler(elc_ButtonClicked);
                largeList.Add(elc);
                if (line.LexicalResult == LexicalResult.OK)
                {
                    if (line.Comment == null)
                        ++ok;
                }
                else
                {
                    ++error;
                }
            }
        }

        private void elc_ButtonClicked(object sender, EventArgs e)
        {
            ElcChanged();
        }
        private void ElcChanged()
        {
            int ok = 0;
            int error = 0;
            int changed = 0;

            foreach (Line line in _lines)
            {
                if (line.LexicalResult == LexicalResult.OK)
                    ++ok;
                else
                    ++error;
                if (line.Comment == null && line.LogEntry.LogEntryString != line.LogEntry.LogEntryStringAsImported)
                    ++changed;
            }

            tbtnOK.Text = "     " + ok;
            tbtnOK.Visible = ok != 0;

            tbtnError.Text = "     " + error;
            tbtnError.Visible = error != 0;

            btnRestoreAll.Text = "Restore All (" + changed + ")";
            btnRestoreAll.Visible = changed != 0;
        }

        #region Buttons
        private void tbtnOK_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var elc in EditLineControls())
                if (elc.Line.LexicalResult == LexicalResult.OK)
                    elc.Visible = tbtnOK.Checked;
        }
        private void tbtnError_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var elc in EditLineControls())
                if (elc.Line.LexicalResult == LexicalResult.Error)
                    elc.Visible = tbtnError.Checked;
        }
        private IEnumerable<EditLineControl> EditLineControls()
        {
            for (int i = 0; i != largeList.ViewCount; i++)
                foreach (EditLineControl elc in largeList[i])
                    yield return elc;
        }
        private void btnApplyFix_Click(object sender, EventArgs e)
        {
            ApplyFix applyFix = new ApplyFix(_lines);
            if (applyFix.ShowDialog() == DialogResult.OK)
            {
                List<EditLineControl> l = new List<EditLineControl>();
                foreach (EditLineControl elc in EditLineControls())
                    foreach(FixLineControl flc in applyFix.FixLineControls())
                        if (elc.Line == flc.Line)
                        {
                            if (flc.Checked)
                            {
                                elc.SetEdittedText(elc.Line.ToString());
                                l.Add(elc);
                            }
                            break;
                        }

                _log.ApplyLogRuleSet();

                foreach (EditLineControl elc in l)
                    elc.SetEdittedGui();

                ElcChanged();
            }
        }
        private void btnRestoreAll_Click(object sender, System.EventArgs e)
        {
            foreach (Line l in _lines)
                if (l.Comment == null)
                    l.LogEntry.LogEntryString = l.LogEntry.LogEntryStringAsImported;

            _log.ApplyLogRuleSet();

            SetGui();

        }
        private void btnSave_Click(object sender, System.EventArgs e)
        {
            Save();
        }
        private void Save()
        {
            if (sfd.ShowDialog() == DialogResult.OK && _lines.Count != 0)
                using (var sw = new StreamWriter(sfd.FileName))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Line line in _lines)
                        sb.AppendLine(line.ToString());

                    string s = sb.ToString();
                    if (s.Length > 2)
                        s = s.Substring(0, s.Length - 2);

                    sw.Write(s);
                    sw.Flush();
                }
        }
        #endregion
    }
}