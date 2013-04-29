/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.Stresstest {
    [ContextMenu(
        new[]
            {
                "Activate_Click", "Add_Click", "Export_Click", "Clear_Click", "Remove_Click", "Copy_Click", "Cut_Click",
                "Duplicate_Click", "Paste_Click"
            },
        new[] { "Edit", "Add Syntax Item", "Export", "Clear", "Remove", "Copy", "Cut", "Duplicate", "Paste" })]
    [Hotkeys(
        new[] { "Activate_Click", "Add_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" }
        ,
        new[]
            {
                Keys.Enter, Keys.Insert, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X),
                (Keys.Control | Keys.D), (Keys.Control | Keys.V)
            })]
    [Serializable]
    public abstract class BaseRuleSet : LabeledBaseItem {
        #region Fields

        protected string _childDelimiter = string.Empty, _description = string.Empty;

        #endregion

        #region Properties

        [SavableCloneable, PropertyControl(1)]
        [Description("If no delimiter is given, the string will not be splitted into parts (space = valid)."),
         DisplayName("Child Delimiter")]
        public virtual string ChildDelimiter {
            get { return _childDelimiter; }
            set { _childDelimiter = value; }
        }

        [SavableCloneable, PropertyControl(int.MaxValue)]
        [Description("Describes this rule set.")]
        public virtual string Description {
            get { return _description; }
            set { _description = value; }
        }

        #endregion

        #region Constructors

        #endregion

        #region Functions

        protected virtual void Add_Click(object sender, EventArgs e) {
            Add(new SyntaxItem());
        }

        /// <summary>
        ///     Lexes the input if possible and builds an AST.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parameters">Can be null.</param>
        /// <param name="output"></param>
        /// <returns></returns>
        public LexicalResult TryLexicalAnalysis(string input, Parameters parameters, out ASTNode output) {
            return RuleSetLexer.TryLexicalAnalysis(input, this, _childDelimiter, parameters, out output);
        }

        #endregion
    }
}