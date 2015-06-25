/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    /// <summary>
    /// Capture HTTP(S) traffic, import a scenario from text and extra tools.
    /// Call SetScenario before doing anything else.
    /// </summary>
    public partial class EditScenarioPanel : UserControl {
        public event EventHandler ScenarioImported, RevertedToAsImported, RedeterminedTokens;

        #region Fields
        private Scenario _scenario;
        private string _beginTokenDelimiter;
        private string _endTokenDelimiter;

        private Parameters _parameters;
        private ParameterTokenTextStyle _parameterTokenTextStyle;

        private const string VBLRn = "<16 0C 02 12n>";
        private const string VBLRr = "<16 0C 02 12r>";
        private static string _multilineComment = string.Empty;

        #endregion

        #region Constructors
        /// <summary>
        /// Capture HTTP(S) traffic, import a scenario from text and extra tools.
        /// Call SetScenario before doing anything else.
        /// </summary>
        public EditScenarioPanel() {
            InitializeComponent();
            try {
                _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
                SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;

                fctxtxImport.DefaultContextMenu(true);

            } catch {
                //Should / can never happen.
            }
        }
        #endregion

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender is CustomListParameters || sender is CustomListParameter || sender is CustomRandomParameters || sender is CustomRandomParameter
                || sender is NumericParameters || sender is NumericParameter || sender is TextParameters || sender is TextParameter) {
                SetCodeStyle();
            }
        }

        internal void SetScenario(Scenario scenario) {
            _scenario = scenario;

            bool requestContainsTokens;
            _scenario.GetParameterTokenDelimiters(out _beginTokenDelimiter, out _endTokenDelimiter, out requestContainsTokens, false);

            SetCodeStyle();

            captureControl.UseAllow = _scenario.UseAllow;
            captureControl.UseDeny = _scenario.UseDeny;
            captureControl.AllowIncludeReferer = _scenario.AllowIncludeReferer;

            captureControl.Allow = _scenario.Allow;
            captureControl.Deny = _scenario.Deny;
        }

        #region Capture HTTP(S)
        private void captureControl_StartClicked(object sender, EventArgs e) {
            if (chkClearScenarioBeforeCapture.Checked && _scenario.Count != 0)
                if (MessageBox.Show("Are you sure you want to clear the scenario?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                    _scenario.Clear();
                    if (ScenarioImported != null) ScenarioImported(this, null); //'Import' the empty scenario to correctly refresh the GUI.
                } else {
                    captureControl.CancelStart();
                    return;
                }
            SaveCaptureSettings();
        }
        private void captureControl_StopClicked(object sender, EventArgs e) {
            SaveCaptureSettings();
            Import(captureControl.ParsedScenario, false);
            try {
                ProxyHelper.UnsetProxy();
            } catch (Exception ex) {
                Loggers.Log(Level.Warning, "Failed to unset the proxy.", ex);
            }
        }
        private void SaveCaptureSettings() {
            try {
                if (_scenario != null) {
                    bool editted = false;
                    if (_scenario.UseAllow != captureControl.UseAllow) {
                        _scenario.UseAllow = captureControl.UseAllow;
                        editted = true;
                    }
                    if (_scenario.Allow.Length != captureControl.Allow.Length) {
                        _scenario.Allow = captureControl.Allow;
                        editted = true;
                    }
                    if (_scenario.AllowIncludeReferer != captureControl.AllowIncludeReferer) {
                        _scenario.AllowIncludeReferer = captureControl.AllowIncludeReferer;
                        editted = true;
                    }
                    if (_scenario.UseDeny != captureControl.UseDeny) {
                        _scenario.UseDeny = captureControl.UseDeny;
                        editted = true;
                    }
                    if (_scenario.Deny.Length != captureControl.Deny.Length) {
                        _scenario.Deny = captureControl.Deny;
                        editted = true;
                    }
                    if (editted)
                        _scenario.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                }
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed saving capture settings.", ex);
            }
        }
        #endregion

        #region Import from Text
        private void fctxtxImport_TextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) { btnImport.Enabled = fctxtxImport.Text.Trim().Length != 0; }

        private void btnBrowse_Click(object sender, EventArgs e) {
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                fctxtxImport.Clear();
                var sb = new StringBuilder();
                foreach (string fileName in openFileDialog.FileNames)
                    using (var sr = new StreamReader(fileName))
                        sb.AppendLine(sr.ReadToEnd());

                fctxtxImport.Text = sb.ToString().Trim();

                try {
                    bool requestContainsTokens;
                    _scenario.GetParameterTokenDelimiters(out _beginTokenDelimiter, out _endTokenDelimiter, out requestContainsTokens, false);
                    SetCodeStyle();
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed getting parameter token delimiters + setting code style.", ex, new object[] { sender, e });
                }
            }
        }
        private void SetCodeStyle() {
            BaseItem customListParameters = _parameters[0];
            BaseItem numericParameters = _parameters[1];
            BaseItem textParameters = _parameters[2];
            BaseItem customRandomParameters = _parameters[3];

            var scopeIdentifiers = new[] { ASTNode.ALWAYS_PARAMETER_SCOPE, ASTNode.LEAF_NODE_PARAMETER_SCOPE, ASTNode.REQUEST_PARAMETER_SCOPE, 
                ASTNode.USER_ACTION_PARAMETER_SCOPE, ASTNode.SCENARIO_PARAMETER_SCOPE };


            int index;
            List<string> clp = new List<string>(), np = new List<string>(), tp = new List<string>(), crp = new List<string>();
            foreach (string scopeIdentifier in scopeIdentifiers) {
                index = 1;
                for (int i = 0; i < customListParameters.Count; i++) {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    clp.Add(token);
                }
                for (int i = 0; i < numericParameters.Count; i++) {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    np.Add(token);
                }
                for (int i = 0; i < textParameters.Count; i++) {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    tp.Add(token);
                }
                for (int i = 0; i < customRandomParameters.Count; i++) {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    crp.Add(token);
                }
            }
            fctxtxImport.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);
            fctxtxImport.Range.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);

            if (_parameterTokenTextStyle != null) {
                _parameterTokenTextStyle.Dispose();
                _parameterTokenTextStyle = null;
            }
            _parameterTokenTextStyle = new ParameterTokenTextStyle(fctxtxImport, GetDelimiters(_scenario.ScenarioRuleSet), clp, np, tp, crp, true);
        }
        private string[] GetDelimiters(ScenarioRuleSet scenarioRuleSet) {
            var hs = new HashSet<string>();
            if (scenarioRuleSet.ChildDelimiter.Length != 0)
                hs.Add(scenarioRuleSet.ChildDelimiter);

            foreach (BaseItem item in scenarioRuleSet)
                if (item is ScenarioSyntaxItem)
                    foreach (string delimiter in GetDelimiters(item as ScenarioSyntaxItem))
                        hs.Add(delimiter);

            var delimiters = new string[hs.Count];
            hs.CopyTo(delimiters);
            hs = null;

            return delimiters;
        }
        private IEnumerable<string> GetDelimiters(ScenarioSyntaxItem scenarioSyntaxItem) {
            if (scenarioSyntaxItem.ChildDelimiter.Length != 0)
                yield return scenarioSyntaxItem.ChildDelimiter;
            foreach (BaseItem item in scenarioSyntaxItem)
                if (item is ScenarioSyntaxItem)
                    foreach (string delimiter in GetDelimiters(item as ScenarioSyntaxItem))
                        yield return delimiter;
        }

        private void btnImport_Click(object sender, EventArgs e) { Import(fctxtxImport.Text, chkClearScenarioBeforeImport.Checked); }
        private void Import(string text, bool clearScenario) {
            //Clone and add to the clone to redetermine the tokens if needed.
            Scenario toAdd = _scenario.Clone(false, false, false, false);

            if (clearScenario && _scenario.Count != 0)
                if (MessageBox.Show("Are you sure you want to clear the scenario?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    _scenario.ClearWithoutInvokingEvent();
                else
                    return;

            UserAction userAction = null;

            string[] splitter = new string[] { "\n", "\r" };
            foreach (string line in text.Split(splitter, StringSplitOptions.RemoveEmptyEntries)) {
                if (line.Trim().Length == 0)
                    continue;

                string output;
                if (DetermineComment(line, out output)) {
                    if (toAdd.ScenarioRuleSet.ActionizeOnComment) {
                        userAction = new UserAction(output);
                        toAdd.AddWithoutInvokingEvent(userAction);
                    }
                } else if (userAction == null) {
                    var request = new Request(output.Replace(VBLRn, "\n").Replace(VBLRr, "\r"));
                    var ua = new UserAction(request.RequestString.Length < 101 ? request.RequestString : request.RequestString.Substring(0, 100) + "...");
                    ua.RequestStringsAsImported.Add(request.RequestString);
                    ua.AddWithoutInvokingEvent(request);
                    toAdd.AddWithoutInvokingEvent(ua);
                } else {
                    var request = new Request(output.Replace(VBLRn, "\n").Replace(VBLRr, "\r"));
                    userAction.AddWithoutInvokingEvent(request);
                    userAction.RequestStringsAsImported.Add(request.RequestString);
                }
            }

            string beginTokenDelimiter, endTokenDelimiter;
            bool requestContainsTokens;
            toAdd.GetParameterTokenDelimiters(out beginTokenDelimiter, out endTokenDelimiter, out requestContainsTokens, false);

            if (requestContainsTokens) {
                var dialog = new RedetermineTokens(_scenario, toAdd);
                if (dialog.ShowDialog() == DialogResult.Cancel) return;
            }

            _scenario.AddRangeWithoutInvokingEvent(toAdd);
            toAdd = null;

            RemoveEmptyUserActions();

            _scenario.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

            if (ScenarioImported != null) ScenarioImported(this, null);
        }
        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns>True if is comment</returns>
        private bool DetermineComment(string input, out string output) {
            int singleline = -1;
            int multiline = -1;

            input = input.TrimStart().Trim();
            output = input;

            if (_multilineComment.Length > 0) {
                multiline = input.IndexOf(_scenario.ScenarioRuleSet.EndCommentString);
                if (multiline > -1) {
                    output = input.Substring(multiline + _scenario.ScenarioRuleSet.EndCommentString.Length);
                    if (output.Length == 0) {
                        if (input.Length > _scenario.ScenarioRuleSet.EndCommentString.Length + 1)
                            output =
                                FormatComment(_multilineComment.TrimStart() + ' ' +
                                              input.Substring(0, input.Length - _scenario.ScenarioRuleSet.EndCommentString.Length));
                        else
                            output = FormatComment(_multilineComment.TrimStart());
                        _multilineComment = string.Empty;
                        return true;
                    }
                    _multilineComment = string.Empty;
                    output = input.Substring(multiline + _scenario.ScenarioRuleSet.EndCommentString.Length);
                    return true;
                }
                if (input.Length > 0)
                    _multilineComment = _multilineComment + ' ' + input;
                output = string.Empty;
                return true;
            }

            if (_scenario.ScenarioRuleSet.SingleLineCommentString.Length > 0)
                singleline = input.IndexOf(_scenario.ScenarioRuleSet.SingleLineCommentString);
            if (_scenario.ScenarioRuleSet.BeginCommentString.Length > 0)
                multiline = input.IndexOf(_scenario.ScenarioRuleSet.BeginCommentString);

            if (singleline > -1 && multiline == -1)
                multiline = int.MaxValue;
            else if (multiline > -1 && singleline == -1)
                singleline = int.MaxValue;

            if (singleline > -1 && singleline < multiline) {
                _multilineComment = string.Empty;
                if (singleline == 0)
                    output = FormatComment(input.Substring(_scenario.ScenarioRuleSet.SingleLineCommentString.Length));
                return true;
            } else if (multiline > -1 && multiline < singleline) {
                int multilineCopy = input.IndexOf(_scenario.ScenarioRuleSet.EndCommentString);
                if (multilineCopy > -1 && multilineCopy > multiline) {
                    _multilineComment = string.Empty;
                    output = input.TrimStart().Substring(0, multiline) +
                             input.Substring(multilineCopy + _scenario.ScenarioRuleSet.EndCommentString.Length);
                    if (output.Length == 0)
                        output =
                            FormatComment(input.Substring(multiline + _scenario.ScenarioRuleSet.BeginCommentString.Length,
                                                          input.Length - _scenario.ScenarioRuleSet.EndCommentString.Length -
                                                          _scenario.ScenarioRuleSet.BeginCommentString.Length));
                    return true;
                } else {
                    _multilineComment = input.Substring(multiline + _scenario.ScenarioRuleSet.BeginCommentString.Length);
                    if (_multilineComment.Length == 0)
                        _multilineComment = " ";
                    output = input.Substring(0, multiline);
                    return true;
                }
            }
            return false;
        }
        private string FormatComment(string input) {
            int i = 0;
            input = input.TrimStart();
            var sb = new StringBuilder(255);
            foreach (char c in input) {
                sb.Append(c);
                if (++i == 255)
                    break;
            }
            return sb.ToString();
        }
        private void RemoveEmptyUserActions() {
            var emptyUserActions = new List<BaseItem>(_scenario.Count);
            foreach (BaseItem item in _scenario)
                if (item is UserAction && item.Count == 0)
                    emptyUserActions.Add(item);

            foreach (BaseItem item in emptyUserActions)
                _scenario.RemoveWithoutInvokingEvent(item);
        }
        #endregion

        #region Extra Tools
        private void btnExportToTextFile_Click(object sender, EventArgs e) {
            saveFileDialog.FileName = _scenario.ToString().ReplaceInvalidWindowsFilenameChars('_').Replace(' ', '_');
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                var sb = new StringBuilder();
                if (_scenario.ScenarioRuleSet.BeginCommentString.Length != 0 && _scenario.ScenarioRuleSet.EndCommentString.Length != 0)
                    foreach (UserAction ua in _scenario) {
                        sb.Append(_scenario.ScenarioRuleSet.BeginCommentString);
                        sb.Append(ua.Label);
                        sb.AppendLine(_scenario.ScenarioRuleSet.EndCommentString);
                        foreach (Request re in ua)
                            sb.AppendLine(re.RequestString);
                    } else
                    foreach (UserAction ua in _scenario) {
                        foreach (Request re in ua)
                            sb.AppendLine(re.RequestString);
                    }

                using (var sw = new StreamWriter(saveFileDialog.FileName))
                    sw.Write(sb.ToString().TrimEnd());
            }
        }
        private void btnRevertToImported_Click(object sender, EventArgs e) {
            if (_scenario != null && MessageBox.Show("Are you sure you want to do this?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                foreach (UserAction userAction in _scenario) {
                    userAction.ClearWithoutInvokingEvent();

                    foreach (string s in userAction.RequestStringsAsImported)
                        userAction.AddWithoutInvokingEvent(new Request(s));
                }
                _scenario.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

                _scenario.ApplyScenarioRuleSet();
                if (RevertedToAsImported != null)
                    RevertedToAsImported(this, null);
            }
        }
        private void btnRedetermineParameterTokens_Click(object sender, EventArgs e) {
            if ((new RedetermineTokens(_scenario)).ShowDialog() == DialogResult.OK && RedeterminedTokens != null)
                RedeterminedTokens(this, null);
        }
        #endregion

        #endregion
    }
}
