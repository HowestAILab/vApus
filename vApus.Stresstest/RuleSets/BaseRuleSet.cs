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

namespace vApus.StressTest {
    [ContextMenu(new[] { "Activate_Click", "Add_Click", "Export_Click", "Clear_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" },
        new[] { "Edit", "Add syntax item", "Export", "Clear", "Remove", "Copy", "Cut", "Duplicate", "Paste" })]
    [Hotkeys(new[] { "Activate_Click", "Add_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" },
        new[] { Keys.Enter, Keys.Insert, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D), (Keys.Control | Keys.V) })]
    [Serializable]
    public abstract class BaseRuleSet : LabeledBaseItem {

        #region Fields
        private readonly object _lock = new object();
        private string _childDelimiter = "<16 0C 02 12$>", _description = string.Empty;
        #endregion

        #region Properties
        [SavableCloneable, PropertyControl(1)]
        [Description("If no delimiter is given, the string will not be splitted into parts (space = valid)."),
         DisplayName("Child delimiter")]
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

        #region Functions
        protected virtual void Add_Click(object sender, EventArgs e) { Add(new SyntaxItem()); }

        /// <summary>
        ///     Lexes the input if possible and builds an AST.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parameters">Can be null.</param>
        /// <param name="output"></param>
        /// <returns></returns>
        internal LexicalResult TryLexicalAnalysis(string input, Parameters parameters, out ASTNode output) {
            output = new ASTNode(_childDelimiter);
            if (input.Length == 0) {
                output.Error = "No input has been provided!";
                return LexicalResult.Error;
            }
            if (_childDelimiter.Length == 0) {
                if (Count == 0) {
                    output.Value = input;
                    return LexicalResult.OK;
                } else {
                    ASTNode syntaxItemOutput = null;
                    //When there is not split the input will be analysed with the child syntax items AND-wise.
                    for (int i = 0; i < Count; i++) {
                        var syntaxItem = this[i] as SyntaxItem;
                        LexicalResult lexicalResult = syntaxItem.TryLexicalAnaysis(input, out syntaxItemOutput);
                        if (lexicalResult != LexicalResult.OK) {
                            output.Value = input;
                            output.Error = syntaxItemOutput.Error;
                            return lexicalResult;
                        }
                    }
                    output.Add(syntaxItemOutput);
                }
            } else {
                string[] splitInput = input.Split(new[] { _childDelimiter }, StringSplitOptions.None);
                if (Count == 0) {
                    //Add AST items without validation.
                    var syntaxItem = new SyntaxItem();
                    syntaxItem.Parent = this;
                    for (int i = 0; i < splitInput.Length; i++) {
                        ASTNode syntaxItemOutput = null;
                        syntaxItem.TryLexicalAnaysis(splitInput[i], out syntaxItemOutput);
                        output.Add(syntaxItemOutput);
                    }
                } else {
                    int syntaxItemIndex = 0, loops = 0;
                    uint occuranceCheck = 0;
                    for (int i = 0; i != Count; i++) {
                        //Handle if not all parts can be checked.
                        if (syntaxItemIndex == Count) {
                            if (i < splitInput.Length && output.Count > 0) {
                                var last = output[output.Count - 1] as ASTNode;
                                for (int k = i; k < splitInput.Length; k++)
                                    last.Value = string.Format("{0}{1}{2}", last.Value, _childDelimiter, splitInput[k]);
                                break;
                            } else {
                                output.Error = "The input string could not be handled correctly due to not enough provided or right configured child syntax items.";
                                return LexicalResult.Error;
                            }
                        }

                        var syntaxItem = this[syntaxItemIndex] as SyntaxItem;
                        ASTNode syntaxItemOutput = null;

                        string syntaxItemInput = i < splitInput.Length ? splitInput[i] : string.Empty;
                        LexicalResult lexicalResult = syntaxItem.TryLexicalAnaysis(syntaxItemInput, out syntaxItemOutput);

                        //Skip invalid optional syntax items.
                        if (lexicalResult == LexicalResult.Error)
                            if (syntaxItem.Optional) {
                                occuranceCheck = 0;
                                ++syntaxItemIndex;
                                loops = i;
                                continue;
                            } else {
                                output.Add(syntaxItemOutput);
                                output.Error = syntaxItemOutput.Error;
                                return lexicalResult;
                            } else
                            output.Add(syntaxItemOutput);

                        //Apply the syntax item validation times the occurancy.
                        if (syntaxItem.Occurance > 0 && occuranceCheck == 0)
                            occuranceCheck = syntaxItem.Occurance;

                        if (occuranceCheck > 0 && --occuranceCheck == 0)
                            ++syntaxItemIndex;
                        loops = i;
                    }

                    //Handle if not all parts can be checked (assumption).
                    //Use 'loops' for optional syntaxItems.
                    if (loops < Count - 1) {
                        output.Error = "The input string could not be handled correctly either due to an infinite occuring syntax item that is not at the end of the collection where it should be.";
                        return LexicalResult.Error;
                    }
                }
            }
            return LexicalResult.OK;
        }
        #endregion
    }
}