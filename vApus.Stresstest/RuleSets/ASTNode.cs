/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    internal class ASTNode : IDisposable {

        #region Fields
        //Parameters
        public const string SCENARIO_PARAMETER_SCOPE = "S.";
        public const string USER_ACTION_PARAMETER_SCOPE = "UA.";
        public const string REQUEST_PARAMETER_SCOPE = "RE.";
        public const string LEAF_NODE_PARAMETER_SCOPE = "LN.";
        public const string ALWAYS_PARAMETER_SCOPE = "";

        private string _childDelimiter = string.Empty;

        private string _error = string.Empty;
        private string _value = string.Empty;
        private string _defaultValue = string.Empty;

        private List<ASTNode> _children = null;
        #endregion

        #region Properties

        public string Value {
            get { return _value; }
            set { _value = value; }
        }
        public string DefaultValue {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }
        public string ChildDelimiter {
            get { return _childDelimiter; }
            set { _childDelimiter = value; }
        }

        public string Error {
            get { return _error; }
            set { _error = value; }
        }

        public int Count { get { return _children == null ? 0 : _children.Count; } }

        public ASTNode this[int index] { get { return _children == null ? null : _children[index]; } }
        #endregion

        #region Constructor

        /// <summary>
        /// Do not forget to set the static property Parameters!
        /// </summary>
        /// <param name="parameters"></param>
        public ASTNode() { }

        /// <summary>
        /// Do not forget to set the static property Parameters!
        /// </summary>
        /// <param name="ruleSetOrRuleSetItem">The rule set item that defined the ast node.</param>
        /// <param name="childDelimiter"></param>
        /// <param name="childDelimiter"></param>
        public ASTNode(string childDelimiter) {
            _childDelimiter = childDelimiter;
        }

        #endregion

        #region Functions
        public void Add(ASTNode child) {
            if (_children == null) _children = new List<ASTNode>();
            _children.Add(child);
        }

        /// <summary>
        /// </summary>
        /// <param name="beginTokenDelimiter">Must be determined for a collection of ast nodes (a scenario) (GetUniqueParameterTokenDelimiters()).</param>
        /// <param name="endTokenDelimiter">Must be determined for a collection of ast nodes (a scenario) (GetUniqueParameterTokenDelimiters()).</param>
        public Util.StringTree GetParameterizedStructure(Dictionary<string, BaseParameter> parameterTokens, HashSet<BaseParameter> chosenNextValueParametersForSScope,
                                                     HashSet<BaseParameter> chosenNextValueParametersForUAScope, HashSet<BaseParameter> chosenNextValueParametersForREScope) {
            Util.StringTree st;
            int count = _children == null ? 0 : _children.Count;
            if (count == 0) {
                st = new Util.StringTree(ParameterizeValue(parameterTokens, chosenNextValueParametersForSScope, chosenNextValueParametersForUAScope, chosenNextValueParametersForREScope), _childDelimiter, count);
            } else {
                st = new Util.StringTree(string.Empty, _childDelimiter, count);
                for (int i = 0; i != count; i++)
                    st[i] = _children[i].GetParameterizedStructure(parameterTokens, chosenNextValueParametersForSScope, chosenNextValueParametersForUAScope, chosenNextValueParametersForREScope);
            }

            return st;
        }


        private string ParameterizeValue(Dictionary<string, BaseParameter> parameterTokens, HashSet<BaseParameter> chosenNextValueParametersForSScope, HashSet<BaseParameter> chosenNextValueParametersForUAScope, HashSet<BaseParameter> chosenNextValueParametersForREScope) {
            if (_value.Length == 0) return _defaultValue;

            HashSet<BaseParameter> chosenNextValueParametersForLNScope = parameterTokens == null ? null : new HashSet<BaseParameter>();

            if (parameterTokens == null) return _value;

            string parameterizedValue = _value;
            /*
                = Always           
            LN. = leaf node
            RE. = request
            UA. = user action
            S. = Scenario
            */

            foreach (string token in parameterTokens.Keys)
                if (parameterizedValue.Contains(token)) {
                    string currentScope = string.Empty;
                    BaseParameter parameter = parameterTokens[token];
                    bool next = false;
                    //Can a next value be determined, this is based on if it could be added to the right hash set or not.

                    if (token.Contains(SCENARIO_PARAMETER_SCOPE)) {
                        currentScope = SCENARIO_PARAMETER_SCOPE;
                        next = chosenNextValueParametersForSScope.Add(parameter);
                        chosenNextValueParametersForUAScope.Add(parameter);
                        chosenNextValueParametersForREScope.Add(parameter);
                        chosenNextValueParametersForLNScope.Add(parameter);
                    } else if (token.Contains(USER_ACTION_PARAMETER_SCOPE)) {
                        currentScope = USER_ACTION_PARAMETER_SCOPE;
                        next = chosenNextValueParametersForUAScope.Add(parameter);
                        chosenNextValueParametersForREScope.Add(parameter);
                        chosenNextValueParametersForLNScope.Add(parameter);
                    } else if (token.Contains(REQUEST_PARAMETER_SCOPE)) {
                        currentScope = REQUEST_PARAMETER_SCOPE;
                        next = chosenNextValueParametersForREScope.Add(parameter);
                        chosenNextValueParametersForLNScope.Add(parameter);
                    } else if (token.Contains(LEAF_NODE_PARAMETER_SCOPE)) {
                        currentScope = LEAF_NODE_PARAMETER_SCOPE;
                        next = chosenNextValueParametersForLNScope.Add(parameter);
                    } else {
                        string[] split = parameterizedValue.Split(new[] { token }, StringSplitOptions.None);

                        var customListParameter = parameter as CustomListParameter;

                        parameterizedValue = string.Empty;
                        for (int i = 0; i != split.Length - 1; i++) {
                            parameter.Next();
                            parameterizedValue += split[i] + parameter.Value;
                        }
                        parameterizedValue += split[split.Length - 1];

                        if (customListParameter != null && !customListParameter.LinkTo.IsEmpty) {
                            var valueIndices = new List<int>();

                            valueIndices.Add(customListParameter.CurrentValueIndex);
                            parameterizedValue = GetParameterizeValueUsingLinkedToParameter(parameterTokens, null, null, null, null,
                                                            parameterizedValue, parameter as CustomListParameter, currentScope, valueIndices);
                            customListParameter.LinkTo.SetValueAt(customListParameter.CurrentValueIndex);
                        }

                        continue;
                    }

                    if (next)
                        parameter.Next();
                    parameterizedValue = parameterizedValue.Replace(token, parameter.Value);

                    if (parameter is CustomListParameter) {
                        var customListParameter = parameter as CustomListParameter;
                        if (!customListParameter.LinkTo.IsEmpty) {
                            parameterizedValue = GetParameterizeValueUsingLinkedToParameter(parameterTokens, chosenNextValueParametersForSScope, chosenNextValueParametersForUAScope, chosenNextValueParametersForREScope, chosenNextValueParametersForLNScope,
                                parameterizedValue, customListParameter, currentScope, null);

                            customListParameter.LinkTo.SetValueAt(customListParameter.CurrentValueIndex);
                        }
                    }
                }

            return parameterizedValue;
        }
        /// <summary>
        /// Specialized functionality to link one custom list parameter output to another.
        /// </summary>
        /// <param name="parameterTokens"></param>
        /// <param name="chosenNextValueParametersForSScope"></param>
        /// <param name="chosenNextValueParametersForUAScope"></param>
        /// <param name="chosenNextValueParametersForREScope"></param>
        /// <param name="chosenNextValueParametersForLNScope"></param>
        /// <param name="parameterizedValue"></param>
        /// <param name="parameter"></param>
        /// <param name="currentScope"></param>
        /// <param name="valueIndicesForEmptyScope"></param>
        /// <returns></returns>
        private string GetParameterizeValueUsingLinkedToParameter(Dictionary<string, BaseParameter> parameterTokens, HashSet<BaseParameter> chosenNextValueParametersForSScope, HashSet<BaseParameter> chosenNextValueParametersForUAScope, HashSet<BaseParameter> chosenNextValueParametersForREScope, HashSet<BaseParameter> chosenNextValueParametersForLNScope,
            string parameterizedValue, CustomListParameter parameter, string currentScope, List<int> valueIndicesForEmptyScope) {
            if (!parameter.LinkTo.IsEmpty) {
                foreach (string token in parameterTokens.Keys)
                    if (_value.Contains(token) && parameterTokens[token] == parameter.LinkTo && token.Contains(currentScope)) {
                        if (currentScope == SCENARIO_PARAMETER_SCOPE) {
                            chosenNextValueParametersForSScope.Add(parameter);
                            chosenNextValueParametersForUAScope.Add(parameter);
                            chosenNextValueParametersForREScope.Add(parameter);
                            chosenNextValueParametersForLNScope.Add(parameter);
                        } else if (currentScope == USER_ACTION_PARAMETER_SCOPE) {
                            chosenNextValueParametersForUAScope.Add(parameter);
                            chosenNextValueParametersForREScope.Add(parameter);
                            chosenNextValueParametersForLNScope.Add(parameter);
                        } else if (currentScope == REQUEST_PARAMETER_SCOPE) {
                            chosenNextValueParametersForREScope.Add(parameter);
                            chosenNextValueParametersForLNScope.Add(parameter);
                        } else if (currentScope == LEAF_NODE_PARAMETER_SCOPE) {
                            chosenNextValueParametersForLNScope.Add(parameter);
                        } else {
                            string[] split = parameterizedValue.Split(new[] { token }, StringSplitOptions.None);
                            parameterizedValue = string.Empty;
                            for (int i = 0; i != split.Length - 1; i++) {
                                if (i < valueIndicesForEmptyScope.Count) {
                                    parameter.LinkTo.SetValueAt(valueIndicesForEmptyScope[i]);
                                    parameterizedValue += split[i] + parameter.LinkTo.Value;
                                } else {
                                    parameterizedValue += split[i] + token;
                                }
                            }
                            parameterizedValue += split[split.Length - 1];

                            return parameterizedValue;
                        }

                        parameter.LinkTo.SetValueAt(parameter.CurrentValueIndex);
                        parameterizedValue = parameterizedValue.Replace(token, parameter.LinkTo.Value);

                        return parameterizedValue; //Return immediatly, only one hit.
                    }
            }
            return parameterizedValue;
        }

        /// <summary>
        ///     Combine the child values (if any) using the delimiter or returns Value.
        ///     Should only be used for testing.
        /// </summary>
        /// <returns></returns>
        //public string CombineValues() {
        //    lock (this) {
        //        var sb = new StringBuilder(_value);
        //        if (_children != null && _children.Count != 0) {
        //            if (_childDelimiter.Length == 0) {
        //                sb.Append((_children[0] as ASTNode).CombineValues());
        //            } else {
        //                for (int i = 0; i < _children.Count - 1; i++) {
        //                    sb.Append((_children[i] as ASTNode).CombineValues());
        //                    sb.Append(_childDelimiter);
        //                }
        //                sb.Append((_children[_children.Count - 1] as ASTNode).CombineValues());
        //            }
        //        }
        //        return sb.ToString();
        //    }
        //}

        public override string ToString() {
            return "ASTNode: " + _value;
        }

        public void Dispose() {
            _childDelimiter = null;
            _value = null;
            _defaultValue = null;

            if (_children != null && _children.Count != 0)
                Parallel.ForEach(_children, (childNode) => {
                    childNode.Dispose();
                });
        }
        #endregion
    }
}