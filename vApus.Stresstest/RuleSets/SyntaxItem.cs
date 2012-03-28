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

namespace vApus.Stresstest
{
    [DisplayName("Syntax Item"), Serializable]
    public class SyntaxItem : LabeledBaseItem
    {
        #region Fields
        protected bool _optional;
        protected string _childDelimiter = string.Empty, _description = string.Empty;
        protected uint _occurance = 1;
        #endregion

        #region Properties
        [SavableCloneable, PropertyControl(1)]
        [Description("If optional and containing rules/syntax items cannot be applied, it will be ignored and the following Indexed Syntax Item will be applied if possible.")]
        public bool Optional
        {
            get { return _optional; }
            set
            {
                if (_occurance == 0 && value == false)
                    throw new Exception("Must be optional with an occurance of 0.");
                _optional = value; 
            
            }
        }
        [SavableCloneable, PropertyControl(2)]
        [Description("How many times this particular item occurs, for that count the containing rules/syntax items will be applied (zero = infinite).")]
        public uint Occurance
        {
            get { return _occurance; }
            set
            {
                if (_optional == false && value == 0)
                    throw new Exception("Must be optional with an occurance of 0.");
                _occurance = value; 
            }
        }
        [SavableCloneable, PropertyControl(3)]
        [Description("If the length of the delimiter is zero, the given string in this item will not be splitted into parts (space = valid). Caution, playing with this can malform the log entries."), DisplayName("Child Delimiter")]
        public virtual string ChildDelimiter
        {
            get { return _childDelimiter; }
            set { _childDelimiter = value; }
        }
        [SavableCloneable, PropertyControl(4)]
        [Description("Describes this syntax item.")]
        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// If the number of child items equals 0 the input string will be returned split on  ChildDelimiter if any.
        /// For tokenizing, Rules can be added, rules are in an “OR relationship” with their siblings. SyntaxItems can be added too, however both types cannot be contained in the same collection. If you want to keep to combine both you can put Rules in an optional SyntaxItem.
        /// The occurrence can be set to be able to recheck the same rules, this has no effect when Optional equals true. But if this value equals zero there will be checked against this SyntaxItem for every next part of the parent’s split input string (ignoring possible next Syntax Items).
        /// If the split input has more parts than there are SyntaxItems those are returned as meta-data.
        /// </summary>
        public SyntaxItem()
        {
        }
        #endregion

        #region Functions
        protected void AddSyntaxItem_Click(object sender, EventArgs e)
        {
            bool invalid = false;
            foreach (BaseItem item in this)
                if (!(item is SyntaxItem))
                {
                    invalid = true;
                    break;
                }
            if (invalid)
            {
                if (MessageBox.Show("If this Rules an Indexed Syntax Item cannot be added.\nDo You want to put these Rules in an optional Indexed Syntax Item?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    SyntaxItem syntaxItem = new SyntaxItem();
                    syntaxItem.Optional = true;
                    foreach (BaseItem item in this)
                    {
                        item.Parent = syntaxItem;
                        syntaxItem.AddWithoutInvokingEvent(item);
                    }
                    Clear();
                    Add(syntaxItem);
                }
                else
                {
                    return;
                }
            }
            Add(new SyntaxItem());
        }
        protected void AddRule_Click(object sender, EventArgs e)
        {
            bool invalid = false;
            foreach (BaseItem item in this)
                if (item is SyntaxItem)
                {
                    invalid = true;
                    break;
                }
            if (invalid)
            {
                if (MessageBox.Show("If this contains Syntax Items a Rule cannot be added.\nDo You want to put it in an optional Indexed Syntax Item?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    SyntaxItem syntaxItem = new SyntaxItem();
                    syntaxItem.Parent = this;
                    syntaxItem.Optional = true;
                    syntaxItem.AddWithoutInvokingEvent(new Rule());
                    this.Add(syntaxItem);
                }
            }
            else
            {
                Add(new Rule());
            }
        }
        internal LexicalResult TryLexicalAnaysis(string input, Parameters parameters, out ASTNode output)
        {
            output = new ASTNode(this, _childDelimiter, parameters);
            if (_childDelimiter.Length == 0)
            {
                if (Count == 0)
                {
                    if (input.Length == 0 && !Optional)
                    {
                        output.Error = "No input has been provided!";
                        return LexicalResult.Error;
                    }
                    output.Value = input;
                    return LexicalResult.OK;
                }
                else
                {
                    if (this[0] is SyntaxItem)
                    {
                        //When there is not split the input will be analysed with the child syntax items AND-wise.
                        for (int i = 0; i < Count; i++)
                        {
                            SyntaxItem syntaxItem = this[i] as SyntaxItem;
                            ASTNode syntaxItemOutput = null;
                            LexicalResult lexicalResult = syntaxItem.TryLexicalAnaysis(input, parameters , out syntaxItemOutput);
                            output.AddWithoutInvokingEvent(syntaxItemOutput);
                            if (lexicalResult != LexicalResult.OK)
                            {
                                output.Value = input;
                                output.Error = syntaxItemOutput.Error;
                                return lexicalResult;
                            }
                        }
                    }
                    else
                    {
                        //Check the rules OR-wise.
                        output.Value = input;
                        //Report all the problems.
                        ASTNode[] ruleOutputs = new ASTNode[Count];
                        for (int i = 0; i < Count; i++)
                        {
                            Rule rule = this[i] as Rule;
                            LexicalResult lexicalResult = rule.TryLexicalAnaysis(input, parameters, out ruleOutputs[i]);
                            if (lexicalResult == LexicalResult.OK)
                                return lexicalResult;
                        }
                        output.Error = "The input does not comply to one of the rules.";
                        foreach (ASTNode ruleoutput in ruleOutputs)
                            output.AddWithoutInvokingEvent(ruleoutput);
                        return LexicalResult.Error;
                    }

                }
            }
            else
            {
                string[] splitInput = input.Split(new string[] { _childDelimiter }, StringSplitOptions.None);
                if (Count == 0)
                {
                    if (input.Length == 0 && !Optional)
                    {
                        output.Error = "No input has been provided!";
                        return LexicalResult.Error;
                    }
                    SyntaxItem syntaxItem = new SyntaxItem();
                    syntaxItem.Parent = this;
                    for (int i = 0; i < splitInput.Length; i++)
                    {
                        ASTNode syntaxItemOutput = null;
                        syntaxItem.TryLexicalAnaysis(splitInput[i], parameters, out syntaxItemOutput);
                        output.AddWithoutInvokingEvent(syntaxItemOutput);
                    }
                }
                else
                {
                    if (this[0] is Rule)
                    {
                        for (int i = 0; i < splitInput.Length; i++)
                        {
                            for (int j = 0; j < Count; j++)
                            {
                                Rule rule = this[j] as Rule;
                                ASTNode ruleOutput = null;
                                LexicalResult lexicalResult = rule.TryLexicalAnaysis(splitInput[i], parameters, out ruleOutput);
                                if (lexicalResult == LexicalResult.OK)
                                {
                                    output.AddWithoutInvokingEvent(ruleOutput);
                                    break;
                                }
                            }
                            if (output.Count < i)
                            {
                                output.Error = "One of the rules could not be applied to the input.";
                                output.Value = splitInput[i];
                                return LexicalResult.Error;
                            }
                        }
                    }
                    else
                    {
                        int syntaxItemIndex = 0, loops = 0;
                        uint occuranceCheck = 0;
                        for (int i = 0; i < splitInput.Length; i++)
                        {
                            //Handle if not all parts can be checked.
                            if (syntaxItemIndex == Count)
                            {
                                if (i < splitInput.Length && output.Count > 0)
                                {
                                    ASTNode last = output[output.Count - 1] as ASTNode;
                                    for (int k = i; k < splitInput.Length; k++)
                                        last.Value = string.Format("{0}{1}{2}", last.Value, _childDelimiter, splitInput[k]);
                                    break;
                                }
                                else
                                {
                                    output.Error = "The input string could not be handled correctly due to not enough provided or right configured child syntax items.";
                                    return LexicalResult.Error;
                                }
                            }

                            SyntaxItem syntaxItem = this[syntaxItemIndex] as SyntaxItem;
                            ASTNode syntaxItemOutput = null;
                            LexicalResult lexicalResult = syntaxItem.TryLexicalAnaysis(splitInput[i], parameters, out syntaxItemOutput);

                            //Skip invalid optional syntax items.
                            if (lexicalResult == LexicalResult.Error)
                                if (syntaxItem.Optional)
                                {
                                    occuranceCheck = 0;
                                    ++syntaxItemIndex;
                                    loops = i;
                                    continue;
                                }
                                else
                                {
                                    output.AddWithoutInvokingEvent(syntaxItemOutput);
                                    output.Error = syntaxItemOutput.Error;
                                    return lexicalResult;
                                }
                            else
                                output.AddWithoutInvokingEvent(syntaxItemOutput);

                            //Apply the syntax item validation times the occurancy.
                            if (syntaxItem.Occurance > 0 && occuranceCheck == 0)
                                occuranceCheck = syntaxItem.Occurance;

                            if (occuranceCheck > 0 && --occuranceCheck == 0)
                                ++syntaxItemIndex;
                            loops = i;

                            //Handle if the collection ends with a optional syntax items that cannot be checked.
                            if (i == splitInput.Length - 1 && i < Count)
                            {
                                loops = Count - 1;
                                for (int j = i + 1; j < Count; j++)
                                {
                                    if (!(this[j] as SyntaxItem).Optional)
                                    {
                                        loops = 0;
                                        break;
                                    }
                                }
                            }
                        }
                        //Handle if not all parts can be checked (assumption).
                        //Use 'loops' for optional syntaxItems.
                        if (loops < Count - 1)
                        {
                            output.Error = "The input string could not be handled correctly either due to an infinite occuring syntax item that is not at the end of the collection where it should be.";
                            return LexicalResult.Error;
                        }
                    }
                }
            }
            return LexicalResult.OK;
        }
        /// <summary>
        /// Gets a control based on the first rule if any.
        /// </summary>
        /// <returns></returns>
        internal Control GetControl()
        {
            return null;
        }
        #endregion
    }
}
