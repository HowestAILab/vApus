/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using vApus.SolutionTree;

namespace vApus.Stresstest
{
    public static class RuleSetLexer
    {
        private static readonly object _lock = new object();

        /// <summary>
        ///     Lexes the input if possible and builds an AST.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="ruleSet"></param>
        /// <param name="childDelimiter"></param>
        /// <param name="parameters">Can be null</param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static LexicalResult TryLexicalAnalysis(string input, BaseItem ruleSet, string childDelimiter,
                                                       Parameters parameters, out ASTNode output)
        {
            lock (_lock)
            {
                output = new ASTNode(ruleSet, childDelimiter, parameters);
                if (input.Length == 0)
                {
                    output.Error = "No input has been provided!";
                    return LexicalResult.Error;
                }
                if (childDelimiter.Length == 0)
                {
                    if (ruleSet.Count == 0)
                    {
                        output.Value = input;
                        return LexicalResult.OK;
                    }
                    else
                    {
                        ASTNode syntaxItemOutput = null;
                        //When there is not split the input will be analysed with the child syntax items AND-wise.
                        for (int i = 0; i < ruleSet.Count; i++)
                        {
                            var syntaxItem = ruleSet[i] as SyntaxItem;
                            LexicalResult lexicalResult = syntaxItem.TryLexicalAnaysis(input, parameters,
                                                                                       out syntaxItemOutput);
                            if (lexicalResult != LexicalResult.OK)
                            {
                                output.Value = input;
                                output.Error = syntaxItemOutput.Error;
                                return lexicalResult;
                            }
                        }
                        output.AddWithoutInvokingEvent(syntaxItemOutput, false);
                    }
                }
                else
                {
                    string[] splitInput = input.Split(new[] {childDelimiter}, StringSplitOptions.None);
                    if (ruleSet.Count == 0)
                    {
                        //Add AST items without validation.
                        var syntaxItem = new SyntaxItem();
                        syntaxItem.Parent = ruleSet;
                        for (int i = 0; i < splitInput.Length; i++)
                        {
                            ASTNode syntaxItemOutput = null;
                            syntaxItem.TryLexicalAnaysis(splitInput[i], parameters, out syntaxItemOutput);
                            output.AddWithoutInvokingEvent(syntaxItemOutput, false);
                        }
                    }
                    else
                    {
                        int syntaxItemIndex = 0, loops = 0;
                        uint occuranceCheck = 0;
                        for (int i = 0; i < splitInput.Length; i++)
                        {
                            //Handle if not all parts can be checked.
                            if (syntaxItemIndex == ruleSet.Count)
                            {
                                if (i < splitInput.Length && output.Count > 0)
                                {
                                    var last = output[output.Count - 1] as ASTNode;
                                    for (int k = i; k < splitInput.Length; k++)
                                        last.Value = string.Format("{0}{1}{2}", last.Value, childDelimiter,
                                                                   splitInput[k]);
                                    break;
                                }
                                else
                                {
                                    output.Error =
                                        "The input string could not be handled correctly due to not enough provided or right configured child syntax items.";
                                    return LexicalResult.Error;
                                }
                            }

                            var syntaxItem = ruleSet[syntaxItemIndex] as SyntaxItem;
                            ASTNode syntaxItemOutput = null;
                            LexicalResult lexicalResult = syntaxItem.TryLexicalAnaysis(splitInput[i], parameters,
                                                                                       out syntaxItemOutput);

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
                                    output.AddWithoutInvokingEvent(syntaxItemOutput, false);
                                    output.Error = syntaxItemOutput.Error;
                                    return lexicalResult;
                                }
                            else
                                output.AddWithoutInvokingEvent(syntaxItemOutput, false);

                            //Apply the syntax item validation times the occurancy.
                            if (syntaxItem.Occurance > 0 && occuranceCheck == 0)
                                occuranceCheck = syntaxItem.Occurance;

                            if (occuranceCheck > 0 && --occuranceCheck == 0)
                                ++syntaxItemIndex;
                            loops = i;

                            //Handle if the collection ends with a optional syntax items that cannot be checked.
                            if (i == splitInput.Length - 1 && i < ruleSet.Count)
                            {
                                loops = ruleSet.Count - 1;
                                for (int j = i + 1; j < ruleSet.Count; j++)
                                {
                                    if (!(ruleSet[j] as SyntaxItem).Optional)
                                    {
                                        loops = 0;
                                        break;
                                    }
                                }
                            }
                        }
                        //Handle if not all parts can be checked (assumption).
                        //Use 'loops' for optional syntaxItems.
                        if (loops < ruleSet.Count - 1)
                        {
                            output.Error =
                                "The input string could not be handled correctly either due to an infinite occuring syntax item that is not at the end of the collection where it should be.";
                            return LexicalResult.Error;
                        }
                    }
                }
                return LexicalResult.OK;
            }
        }
    }
}