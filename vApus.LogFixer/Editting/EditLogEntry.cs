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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using vApus.LogFixer.Properties;
using vApus.SolutionTree;
using vApus.Stresstest;

namespace vApus.LogFixer
{
    public partial class EditLogEntry : Form
    {
        private readonly LogEntry _logEntry;

        private readonly List<int> _selectedTreeNodeIndex = new List<int>(new[] {0});
        private readonly Changes _textChangeTracker;
        private readonly string _untouchedLogEntry;

        private readonly VisualizeWhiteSpaceTextStyle _visualizeWhiteSpaceTextStyle;
        private bool _canEnableBtnOK;
        private TrackedChanges _trackedChanges;

        public EditLogEntry()
        {
            InitializeComponent();
            fastColoredTextBoxEdit.AllowDrop = true;
        }

        public EditLogEntry(int line, LogEntry logEntry)
            : this()
        {
            Text = "Edit Log Entry  at Line" + line;

            _logEntry = logEntry.Clone();
            _untouchedLogEntry = _logEntry.LogEntryString;
            fastColoredTextBoxAsImported.Text = _logEntry.LogEntryStringAsImported;
            _textChangeTracker = new Changes();

            SetValidation();

            fastColoredTextBoxEdit.TextChangedDelayed -= fastColoredTextBoxEdit_TextChangedDelayed;
            fastColoredTextBoxEdit.Text = logEntry.LogEntryString;
            fastColoredTextBoxEdit.TextChangedDelayed += fastColoredTextBoxEdit_TextChangedDelayed;

            _visualizeWhiteSpaceTextStyle = new VisualizeWhiteSpaceTextStyle(fastColoredTextBoxEdit,
                                                                             chkVisualizeWhitespace.Checked);
        }

        public LogEntry LogEntry
        {
            get { return _logEntry; }
        }

        public TrackedChanges TrackedChanges
        {
            get { return _trackedChanges; }
        }

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

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
                                            _logEntry.LexedLogEntry.ToString(chkShowNamesAndIndices.Checked,
                                                                             chkShowLabels.Checked) +
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

                string childPartToString = childPart.ToString(chkShowNamesAndIndices.Checked, chkShowLabels.Checked);
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

        private void chk_CheckedChanged(object sender, EventArgs e)
        {
            _canEnableBtnOK = btnOK.Enabled;
            FillTreeView();
        }

        private void chkVisualizeWhitespace_CheckedChanged(object sender, EventArgs e)
        {
            _visualizeWhiteSpaceTextStyle.VisualizeWhiteSpace = chkVisualizeWhitespace.Checked;
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

                _visualizeWhiteSpaceTextStyle.VisualizeWhiteSpace = chkVisualizeWhitespace.Checked;

                _trackedChanges = _textChangeTracker.Track(_untouchedLogEntry, _logEntry.LogEntryString);
            }
        }
    }
}