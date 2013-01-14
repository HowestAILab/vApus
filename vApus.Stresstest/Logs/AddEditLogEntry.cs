/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using vApus.SolutionTree;
using vApus.Stresstest.Properties;
using vApus.Util;

namespace vApus.Stresstest
{
    public partial class AddEditLogEntry : Form
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        #region Fields

        private readonly string _beginTokenDelimiter;
        private readonly string _endTokenDelimiter;
        private readonly Log _log;
        private readonly LogEntry _logEntry;

        private readonly Parameters _parameters;

        private readonly List<int> _selectedTreeNodeIndex = new List<int>(new[] {0});
        private ParameterTokenTextStyle _asImportedParameterTokenTextStyle;

        private bool _canEnableBtnOK;

        private ParameterTokenTextStyle _editParameterTokenTextStyle;

        #endregion

        #region Properties

        public LogEntry LogEntry
        {
            get { return _logEntry; }
        }

        #endregion

        #region Constructors

        public AddEditLogEntry()
        {
            InitializeComponent();
            Text = "Add Log Entry";

            cboParameterScope.SelectedIndex = 5;

            fastColoredTextBoxEdit.AllowDrop = true;
        }

        public AddEditLogEntry(LogEntry logEntry)
        {
            InitializeComponent();
            Text = "Edit Log Entry";
            _logEntry = logEntry.Clone();

            fastColoredTextBoxEdit.TextChangedDelayed -= fastColoredTextBoxEdit_TextChangedDelayed;
            fastColoredTextBoxEdit.Text = _logEntry.LogEntryString;
            fastColoredTextBoxEdit.TextChangedDelayed += fastColoredTextBoxEdit_TextChangedDelayed;

            fastColoredTextBoxAsImported.Text = _logEntry.LogEntryStringAsImported;


            if (_logEntry.Parent is UserAction)
                _log = _logEntry.Parent.GetParent() as Log;
            else
                _log = _logEntry.Parent as Log;

            bool warning, error;
            _log.GetUniqueParameterTokenDelimiters(out _beginTokenDelimiter, out _endTokenDelimiter, out warning,
                                                   out error);

            _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof (Parameters)) as Parameters;

            SetValidation();
            SetCodeStyle();

            cboParameterScope.SelectedIndex = 5;

            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }

        #endregion

        #region Functions

        private void SetValidation()
        {
            switch (_logEntry.LexicalResult)
            {
                case LexicalResult.OK:
                    Icon = Icon.FromHandle(Resources.LogEntryOK.GetHicon());
                    tc.TabPages.Remove(tpError);
                    if (!tc.TabPages.Contains(tpEdit))
                        tc.TabPages.Add(tpEdit);
                    break;
                case LexicalResult.Error:
                    Icon = Icon.FromHandle(Resources.LogEntryError.GetHicon());
                    if (!tc.TabPages.Contains(tpError))
                        tc.TabPages.Add(tpError);
                    break;
            }
            FillTreeView();
        }

        private void FillTreeView()
        {
            LockWindowUpdate(Handle.ToInt32());

            btnOK.Enabled = false;

            TreeNode root = null;
            if (tvwValidation.Nodes.Count == 0)
            {
                root = new TreeNode();
                tvwValidation.Nodes.Add(root);
            }
            else
            {
                root = tvwValidation.Nodes[0];
            }

            root.Tag = _logEntry.LexedLogEntry;
            string lexedLogEntry_ToString = "Validated against: " +
                                            _logEntry.LexedLogEntry.ToString(false, chkShowLabels.Checked) +
                                            " [ > Select to Show/Edit Full Log Entry String]";


            if (_logEntry.LexedLogEntry.Error.Length == 0)
            {
                root.SelectedImageIndex = 0;
                root.ImageIndex = 0;
            }
            else
            {
                root.SelectedImageIndex = 1;
                root.ImageIndex = 1;
            }

            if (root.Text != lexedLogEntry_ToString)
            {
                root.Text = lexedLogEntry_ToString;
                root.ExpandAll();
                if (_canEnableBtnOK)
                    btnOK.Enabled = true;
            }

            AddOrRecycleChildNodesToTreeView(_logEntry.LexedLogEntry, _logEntry.LogRuleSet, root);

            tvwValidation.AfterSelect -= tvwValidation_AfterSelect;
            TreeNodeCollection nodeCollection = tvwValidation.Nodes;
            foreach (int index in _selectedTreeNodeIndex)
                if (nodeCollection.Count > index)
                {
                    tvwValidation.SelectedNode = nodeCollection[index];
                    nodeCollection = tvwValidation.SelectedNode.Nodes;
                }
                else
                {
                    tvwValidation.SelectedNode = tvwValidation.Nodes[0];
                    break;
                }
            tvwValidation.AfterSelect += tvwValidation_AfterSelect;

            LockWindowUpdate(0);
        }

        private void SetCodeStyle()
        {
            BaseItem customListParameters = _parameters[0];
            BaseItem numericParameters = _parameters[1];
            BaseItem textParameters = _parameters[2];
            BaseItem customRandomParameters = _parameters[3];

            var scopeIdentifiers = new[]
                {
                    ASTNode.ALWAYS_PARAMETER_SCOPE,
                    ASTNode.LEAF_NODE_PARAMETER_SCOPE,
                    ASTNode.LOG_ENTRY_PARAMETER_SCOPE,
                    ASTNode.USER_ACTION_PARAMETER_SCOPE,
                    ASTNode.LOG_PARAMETER_SCOPE
                };


            int index;
            List<string> clp = new List<string>(),
                         np = new List<string>(),
                         tp = new List<string>(),
                         crp = new List<string>();
            foreach (string scopeIdentifier in scopeIdentifiers)
            {
                index = 1;
                for (int i = 0; i < customListParameters.Count; i++)
                {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    clp.Add(token);
                }
                for (int i = 0; i < numericParameters.Count; i++)
                {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    np.Add(token);
                }
                for (int i = 0; i < textParameters.Count; i++)
                {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    tp.Add(token);
                }
                for (int i = 0; i < customRandomParameters.Count; i++)
                {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    crp.Add(token);
                }
            }
            _editParameterTokenTextStyle = new ParameterTokenTextStyle(fastColoredTextBoxEdit,
                                                                       GetDelimiters(_logEntry.LogRuleSet), clp, np, tp,
                                                                       crp, chkVisualizeWhitespace.Checked);
            _asImportedParameterTokenTextStyle = new ParameterTokenTextStyle(fastColoredTextBoxAsImported,
                                                                             GetDelimiters(_logEntry.LogRuleSet), clp,
                                                                             np, tp, crp, chkVisualizeWhitespace.Checked);
        }

        private string[] GetDelimiters(LogRuleSet logRuleSet)
        {
            var hs = new HashSet<string>();
            if (logRuleSet.ChildDelimiter.Length != 0)
                hs.Add(logRuleSet.ChildDelimiter);

            foreach (BaseItem item in logRuleSet)
                if (item is LogSyntaxItem)
                    foreach (string delimiter in GetDelimiters(item as LogSyntaxItem))
                        hs.Add(delimiter);

            var delimiters = new string[hs.Count];
            hs.CopyTo(delimiters);
            hs = null;

            return delimiters;
        }

        private IEnumerable<string> GetDelimiters(LogSyntaxItem logSyntaxItem)
        {
            if (logSyntaxItem.ChildDelimiter.Length != 0)
                yield return logSyntaxItem.ChildDelimiter;
            foreach (BaseItem item in logSyntaxItem)
                if (item is LogSyntaxItem)
                    foreach (string delimiter in GetDelimiters(item as LogSyntaxItem))
                        yield return delimiter;
        }

        private void AddOrRecycleChildNodesToTreeView(ASTNode astNode, LabeledBaseItem ruleSetItem,
                                                      TreeNode parentTreeNode)
        {
            int astNodeCount = astNode.Count;
            //Remove not needed nodes
            int parentTreeNode_NodeCount = parentTreeNode.GetNodeCount(false);
            for (int i = astNodeCount; i < parentTreeNode_NodeCount; i++)
                parentTreeNode.Nodes.RemoveAt(astNodeCount);

            //To select the right rule set item
            int astNodeIndex = 0;
            uint occuranceCheck = 0;
            for (int i = 0; i < astNodeCount; i++)
            {
                var childPart = astNode[i] as ASTNode;
                var childItem = ruleSetItem[astNodeIndex] as LabeledBaseItem;

                //Keep re-occuring items in mind.
                if (childItem is SyntaxItem)
                {
                    var syntaxItem = childItem as SyntaxItem;
                    if (syntaxItem.Occurance > 0 && occuranceCheck == 0)
                        occuranceCheck = syntaxItem.Occurance;
                }
                else
                {
                    ++astNodeIndex;
                }
                if (occuranceCheck > 0 && --occuranceCheck == 0)
                    ++astNodeIndex;

                TreeNode childTreeNode = null;
                if (i < parentTreeNode_NodeCount)
                {
                    childTreeNode = parentTreeNode.Nodes[i];
                }
                else
                {
                    childTreeNode = new TreeNode();
                    parentTreeNode.Nodes.Add(childTreeNode);
                }
                childTreeNode.Tag = childPart;

                if (childPart.Error.Length == 0)
                {
                    childTreeNode.SelectedImageIndex = 0;
                    childTreeNode.ImageIndex = 0;
                }
                else
                {
                    childTreeNode.SelectedImageIndex = 1;
                    childTreeNode.ImageIndex = 1;
                }

                string childPartToString = childPart.ToString(false, chkShowLabels.Checked);
                if (childTreeNode.Text != childPartToString)
                {
                    childTreeNode.Text = childPartToString;
                    parentTreeNode.ExpandAll();
                    if (_canEnableBtnOK)
                        btnOK.Enabled = true;
                }

                AddOrRecycleChildNodesToTreeView(childPart, childItem, childTreeNode);
            }
        }

        private void tvwValidation_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvwValidation.SelectedNode != null)
            {
                var astNode = tvwValidation.SelectedNode.Tag as ASTNode;
                rtxtError.Text = astNode.Error;

                fastColoredTextBoxEdit.TextChangedDelayed -= fastColoredTextBoxEdit_TextChangedDelayed;
                fastColoredTextBoxEdit.Text = astNode.CombineValues();
                fastColoredTextBoxEdit.TextChangedDelayed += fastColoredTextBoxEdit_TextChangedDelayed;

                _selectedTreeNodeIndex.Clear();

                TreeNode node = tvwValidation.SelectedNode;
                _selectedTreeNodeIndex.Add(node.Index);
                while (node.Parent != null)
                {
                    node = node.Parent;
                    _selectedTreeNodeIndex.Add(node.Index);
                }
                _selectedTreeNodeIndex.Reverse();
            }
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            if (Visible)
            {
                _logEntry.ApplyLogRuleSet();
                _canEnableBtnOK = _logEntry.LexicalResult == LexicalResult.OK;
                SetValidation();
                if (tvwValidation.SelectedNode != null)
                {
                    var lexedPart = tvwValidation.SelectedNode.Tag as ASTNode;
                    rtxtError.Text = lexedPart.Error;
                }
            }
        }

        private void chk_CheckedChanged(object sender, EventArgs e)
        {
            _canEnableBtnOK = btnOK.Enabled;
            FillTreeView();
        }

        private void chkVisualizeWhitespace_CheckedChanged(object sender, EventArgs e)
        {
            if (_editParameterTokenTextStyle != null)
                _editParameterTokenTextStyle.VisualizeWhiteSpace = chkVisualizeWhitespace.Checked;
            if (_asImportedParameterTokenTextStyle != null)
                _asImportedParameterTokenTextStyle.VisualizeWhiteSpace = chkVisualizeWhitespace.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void cboParameterScope_SelectedIndexChanged(object sender, EventArgs e)
        {
            string scopeIdentifier = null;

            flpTokens.Controls.Clear();

            switch (cboParameterScope.SelectedIndex)
            {
                case 1:
                    scopeIdentifier = ASTNode.LOG_PARAMETER_SCOPE;
                    break;
                case 2:
                    scopeIdentifier = ASTNode.USER_ACTION_PARAMETER_SCOPE;
                    break;
                case 3:
                    scopeIdentifier = ASTNode.LOG_ENTRY_PARAMETER_SCOPE;
                    break;
                case 4:
                    scopeIdentifier = ASTNode.LEAF_NODE_PARAMETER_SCOPE;
                    break;
                case 5:
                    scopeIdentifier = ASTNode.ALWAYS_PARAMETER_SCOPE;
                    break;
            }

            if (scopeIdentifier == null)
            {
                AddKvpsToFlps(ASTNode.LOG_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.USER_ACTION_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.LOG_ENTRY_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.LEAF_NODE_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.ALWAYS_PARAMETER_SCOPE);
            }
            else
            {
                AddKvpsToFlps(scopeIdentifier);
            }
        }

        private void AddKvpsToFlps(string scopeIdentifier)
        {
            BaseItem customListParameters = _parameters[0];
            BaseItem numericParameters = _parameters[1];
            BaseItem textParameters = _parameters[2];
            BaseItem customRandomParameters = _parameters[3];

            int j = 1;
            for (int i = 0; i < customListParameters.Count; i++)
                AddKvpToFlps(_beginTokenDelimiter + scopeIdentifier + (j++) + _endTokenDelimiter,
                             customListParameters[i].ToString(), Color.LightPink);
            for (int i = 0; i < numericParameters.Count; i++)
                AddKvpToFlps(_beginTokenDelimiter + scopeIdentifier + (j++) + _endTokenDelimiter,
                             numericParameters[i].ToString(), Color.LightGreen);
            for (int i = 0; i < textParameters.Count; i++)
                AddKvpToFlps(_beginTokenDelimiter + scopeIdentifier + (j++) + _endTokenDelimiter,
                             textParameters[i].ToString(), Color.LightBlue);
            for (int i = 0; i < customRandomParameters.Count; i++)
                AddKvpToFlps(_beginTokenDelimiter + scopeIdentifier + (j++) + _endTokenDelimiter,
                             customRandomParameters[i].ToString(), Color.Yellow);
        }

        private void AddKvpToFlps(string key, string value, Color backColor)
        {
            var kvp = new KeyValuePairControl(key, value);
            kvp.BackColor = backColor;
            flpTokens.Controls.Add(kvp);
        }

        private void fastColoredTextBoxEdit_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            if (tvwValidation.SelectedNode != null)
            {
                var astNode = tvwValidation.SelectedNode.Tag as ASTNode;
                astNode.ClearWithoutInvokingEvent();
                astNode.Value = fastColoredTextBoxEdit.Text;

                astNode = tvwValidation.Nodes[0].Tag as ASTNode;
                _logEntry.LogEntryString = astNode.CombineValues();

                _logEntry.ApplyLogRuleSet();

                _canEnableBtnOK = _logEntry.LexicalResult == LexicalResult.OK;

                SetValidation();
            }
        }

        #endregion
    }
}