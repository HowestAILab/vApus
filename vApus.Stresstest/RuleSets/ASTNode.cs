/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Text;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    internal class ASTNode {

        #region Fields
        //Parameters
        public const string LOG_PARAMETER_SCOPE = "L.";
        public const string USER_ACTION_PARAMETER_SCOPE = "UA.";
        public const string LOG_ENTRY_PARAMETER_SCOPE = "LE.";
        public const string LEAF_NODE_PARAMETER_SCOPE = "LN.";
        public const string ALWAYS_PARAMETER_SCOPE = "";

        private static object _lock = new object();

        private static Parameters _parameters;

        private readonly BaseItem _ruleSetOrRuleSetItem;

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

        public ASTNode(Parameters parameters = null) {
            if (parameters != null && parameters != _parameters)
                _parameters = parameters;
        }

        /// <summary>
        /// </summary>
        /// <param name="ruleSetOrRuleSetItem">The rule set item that defined the ast node.</param>
        /// <param name="childDelimiter"></param>
        /// <param name="childDelimiter"></param>
        public ASTNode(BaseItem ruleSetOrRuleSetItem, string childDelimiter, Parameters parameters = null)
            : this(parameters) {
            _childDelimiter = childDelimiter;
            _ruleSetOrRuleSetItem = ruleSetOrRuleSetItem;
        }

        #endregion

        #region Functions
        public void Add(ASTNode child) {
            if (_children == null) _children = new List<ASTNode>();
            _children.Add(child);
        }

        /// <summary>
        /// </summary>
        /// <param name="beginTokenDelimiter">Must be determined for a collection of ast nodes (a log) (GetUniqueParameterTokenDelimiters()).</param>
        /// <param name="endTokenDelimiter">Must be determined for a collection of ast nodes (a log) (GetUniqueParameterTokenDelimiters()).</param>
        public StringTree GetParameterizedStructure(Dictionary<string, BaseParameter> parameterTokens, HashSet<BaseParameter> chosenNextValueParametersForLScope,
                                                     HashSet<BaseParameter> chosenNextValueParametersForUAScope, HashSet<BaseParameter> chosenNextValueParametersForLEScope) {
            StringTree st;
            int count = _children == null ? 0 : _children.Count;
            if (count == 0) {
                st = new StringTree(ParameterizeValue(parameterTokens, chosenNextValueParametersForLScope, chosenNextValueParametersForUAScope, chosenNextValueParametersForLEScope), _childDelimiter, count);
            } else {
                st = new StringTree(string.Empty, _childDelimiter, count);
                for (int i = 0; i != count; i++)
                    st[i] = _children[i].GetParameterizedStructure(parameterTokens, chosenNextValueParametersForLScope, chosenNextValueParametersForUAScope, chosenNextValueParametersForLEScope);
            }

            return st;
        }


        private string ParameterizeValue(Dictionary<string, BaseParameter> parameterTokens, HashSet<BaseParameter> chosenNextValueParametersForLScope, HashSet<BaseParameter> chosenNextValueParametersForUAScope, HashSet<BaseParameter> chosenNextValueParametersForLEScope) {
            if (_value.Length == 0) return _defaultValue;

            HashSet<BaseParameter> chosenNextValueParametersForLNScope = parameterTokens == null ? null : new HashSet<BaseParameter>();

            if (parameterTokens == null) return _value;

            string parameterizedValue = _value;
            /*
                = Always           
            LN. = leaf node
            LE. = log entry
            UA. = user action
            L. = Log
            */

            foreach (string token in parameterTokens.Keys)
                if (parameterizedValue.Contains(token)) {
                    string currentScope = string.Empty;
                    BaseParameter parameter = parameterTokens[token];
                    bool next = false;
                    //Can a next value be determined, this is based on if it could be added to the right hash set or not.

                    if (token.Contains(LOG_PARAMETER_SCOPE)) {
                        currentScope = LOG_PARAMETER_SCOPE;
                        next = chosenNextValueParametersForLScope.Add(parameter);
                        chosenNextValueParametersForUAScope.Add(parameter);
                        chosenNextValueParametersForLEScope.Add(parameter);
                        chosenNextValueParametersForLNScope.Add(parameter);
                    } else if (token.Contains(USER_ACTION_PARAMETER_SCOPE)) {
                        currentScope = USER_ACTION_PARAMETER_SCOPE;
                        next = chosenNextValueParametersForUAScope.Add(parameter);
                        chosenNextValueParametersForLEScope.Add(parameter);
                        chosenNextValueParametersForLNScope.Add(parameter);
                    } else if (token.Contains(LOG_ENTRY_PARAMETER_SCOPE)) {
                        currentScope = LOG_ENTRY_PARAMETER_SCOPE;
                        next = chosenNextValueParametersForLEScope.Add(parameter);
                        chosenNextValueParametersForLNScope.Add(parameter);
                    } else if (token.Contains(LEAF_NODE_PARAMETER_SCOPE)) {
                        currentScope = LEAF_NODE_PARAMETER_SCOPE;
                        next = chosenNextValueParametersForLNScope.Add(parameter);
                    } else {
                        string[] split = parameterizedValue.Split(new[] { token }, StringSplitOptions.None);

                        var customListParameter = parameter as CustomListParameter;
                        if (customListParameter != null && customListParameter.LinkTo.IsEmpty)
                            customListParameter = null;

                        List<int> valueIndices = null;
                        if (customListParameter != null) valueIndices = new List<int>();

                        parameterizedValue = string.Empty;
                        for (int i = 0; i != split.Length - 1; i++) {
                            parameter.Next();
                            if (customListParameter != null) valueIndices.Add(parameter.Index);
                            parameterizedValue += split[i] + parameter.Value;
                        }
                        parameterizedValue += split[split.Length - 1];

                        if (customListParameter != null)
                            parameterizedValue = GetParameterizeValueUsingLinkedToParameter(parameterTokens, null, null, null, null,
                                                            parameterizedValue, parameter as CustomListParameter, currentScope, valueIndices);

                        continue;
                    }

                    if (next)
                        parameter.Next();
                    parameterizedValue = parameterizedValue.Replace(token, parameter.Value);

                    if (parameter is CustomListParameter)
                        parameterizedValue = GetParameterizeValueUsingLinkedToParameter(parameterTokens, chosenNextValueParametersForLScope, chosenNextValueParametersForUAScope, chosenNextValueParametersForLEScope, chosenNextValueParametersForLNScope,
                            parameterizedValue, parameter as CustomListParameter, currentScope, null);
                }

            return parameterizedValue;
        }
        /// <summary>
        /// Specialized functionality to link one custom list parameter output to another.
        /// </summary>
        /// <param name="parameterTokens"></param>
        /// <param name="chosenNextValueParametersForLScope"></param>
        /// <param name="chosenNextValueParametersForUAScope"></param>
        /// <param name="chosenNextValueParametersForLEScope"></param>
        /// <param name="chosenNextValueParametersForLNScope"></param>
        /// <param name="parameterizedValue"></param>
        /// <param name="parameter"></param>
        /// <param name="currentScope"></param>
        /// <param name="valueIndicesForEmptyScope"></param>
        /// <returns></returns>
        private string GetParameterizeValueUsingLinkedToParameter(Dictionary<string, BaseParameter> parameterTokens, HashSet<BaseParameter> chosenNextValueParametersForLScope, HashSet<BaseParameter> chosenNextValueParametersForUAScope, HashSet<BaseParameter> chosenNextValueParametersForLEScope, HashSet<BaseParameter> chosenNextValueParametersForLNScope,
            string parameterizedValue, CustomListParameter parameter, string currentScope, List<int> valueIndicesForEmptyScope) {
            if (!parameter.LinkTo.IsEmpty) {
                foreach (string token in parameterTokens.Keys)
                    if (parameterizedValue.Contains(token) && parameterTokens[token] == parameter && token.Contains(currentScope)) {
                        if (currentScope == LOG_PARAMETER_SCOPE) {
                            chosenNextValueParametersForLScope.Add(parameter);
                            chosenNextValueParametersForUAScope.Add(parameter);
                            chosenNextValueParametersForLEScope.Add(parameter);
                            chosenNextValueParametersForLNScope.Add(parameter);
                        } else if (currentScope == USER_ACTION_PARAMETER_SCOPE) {
                            chosenNextValueParametersForUAScope.Add(parameter);
                            chosenNextValueParametersForLEScope.Add(parameter);
                            chosenNextValueParametersForLNScope.Add(parameter);
                        } else if (currentScope == LOG_ENTRY_PARAMETER_SCOPE) {
                            chosenNextValueParametersForLEScope.Add(parameter);
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
        public string CombineValues() {
            lock (this) {
                var sb = new StringBuilder(_value);
                if (_children != null && _children.Count != 0) {
                    if (_childDelimiter.Length == 0) {
                        sb.Append((_children[0] as ASTNode).CombineValues());
                    } else {
                        for (int i = 0; i < _children.Count - 1; i++) {
                            sb.Append((_children[i] as ASTNode).CombineValues());
                            sb.Append(_childDelimiter);
                        }
                        sb.Append((_children[_children.Count - 1] as ASTNode).CombineValues());
                    }
                }
                return sb.ToString();
            }
        }

        public override string ToString() {
            return "ASTNode: " + _value;
        }

        #endregion
    }
}