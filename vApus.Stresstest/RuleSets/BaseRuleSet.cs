/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.Stresstest
{
    [ContextMenu(new string[] { "Activate_Click", "Add_Click", "Export_Click", "Clear_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" }, new string[] { "Edit", "Add Syntax Item", "Export", "Clear", "Remove", "Copy", "Cut", "Duplicate", "Paste" })]
    [Hotkeys(new string[] { "Activate_Click", "Add_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" }, new Keys[] { Keys.Enter, Keys.Insert, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D), (Keys.Control | Keys.V) })]
    [Serializable]
    public abstract class BaseRuleSet : LabeledBaseItem
    {
        #region Fields
        protected string _childDelimiter = string.Empty, _description = string.Empty;
        #endregion

        #region Properties
        [SavableCloneable, PropertyControl(1)]
        [Description("If no delimiter is given, the log entry will not be splitted into parts (space = valid). Please use <16 0C 02 12$> as it is the default for the log recorder. Just like new lines this is replaced by \"◦\" in the labels of the log entry controls for readability."), DisplayName("Child Delimiter")]
        public virtual string ChildDelimiter
        {
            get { return _childDelimiter; }
            set { _childDelimiter = value; }
        }
        [SavableCloneable, PropertyControl(int.MaxValue)]
        [Description("Describes this rule set.")]
        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This is an abstract class from which there must be derived.
        /// If the number of SyntaxItems equals 0 the input string will be returned split on ChildDelimiter if any.
        /// Otherwise the string will be split and each part will be checked against a SyntaxItem having the same index. The array of strings must have a length that at least equals the number of SyntaxItems.
        /// However those items can be optional, so if a rule deeper in the structure does not comply with the matching index in the array, that part of the array will be matched against the next non-optional SyntaxItem.
        /// If the split input has more parts than there are SyntaxItems those are returned as meta-data.
        /// </summary>
        public BaseRuleSet()
        { }
        #endregion

        #region Functions
        protected virtual void Add_Click(object sender, EventArgs e)
        {
            Add(new SyntaxItem());
        }
        /// <summary>
        /// Lexes the input if possible and builds an AST.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parameters">Can be null.</param>
        /// <param name="output"></param>
        /// <returns></returns>
        public LexicalResult TryLexicalAnalysis(string input, Parameters parameters, out ASTNode output)
        {
            return RuleSetLexer.TryLexicalAnalysis(input, this, _childDelimiter, parameters, out output);
        }
        #endregion
    }
}
