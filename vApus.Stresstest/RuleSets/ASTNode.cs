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

namespace vApus.Stresstest
{
    [Serializable]
    public class ASTNode : BaseItem
    {
        #region Fields
        private static object _lock = new object();

        private static Parameters _parameters;

        //Parameters
        public const string LOG_PARAMETER_SCOPE = "L.";
        public const string USER_ACTION_PARAMETER_SCOPE = "UA.";
        public const string LOG_ENTRY_PARAMETER_SCOPE = "LE.";
        public const string LEAF_NODE_PARAMETER_SCOPE = "LN.";
        public const string ALWAYS_PARAMETER_SCOPE = "";

        private BaseItem _ruleSetOrRuleSetItem;

        private string _childDelimiter = string.Empty,
                       _value = string.Empty;

        private string _error = string.Empty;

        #endregion

        #region Properties
        [SavableCloneable, PropertyControl]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        [SavableCloneable]
        public string ChildDelimiter
        {
            get { return _childDelimiter; }
            set { _childDelimiter = value; }
        }

        public string Error
        {
            get { return _error; }
            set { _error = value; }
        }
        #endregion

        #region Constructor
        public ASTNode(Parameters parameters = null)
        {
            ShowInGui = false;
            if (parameters != null && parameters != _parameters)
                _parameters = parameters;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruleSetOrRuleSetItem">The rule set item that defined the ast node.</param>
        /// <param name="childDelimiter"></param>
        /// <param name="childDelimiter"></param>
        public ASTNode(BaseItem ruleSetOrRuleSetItem, string childDelimiter, Parameters parameters = null)
            : this(parameters)
        {
            _childDelimiter = childDelimiter;
            _ruleSetOrRuleSetItem = ruleSetOrRuleSetItem;
        }
        #endregion

        #region Functions
        public string ToString(bool showNamesAndIndices, bool showLabels)
        {
            string toString = string.Empty;
            if (_ruleSetOrRuleSetItem != null)
            {
                if (showNamesAndIndices && showLabels)
                    toString = string.Format("{0} {1}: {2}", _ruleSetOrRuleSetItem.Name, (_ruleSetOrRuleSetItem is LabeledBaseItem ? (_ruleSetOrRuleSetItem as LabeledBaseItem).Index.ToString() : string.Empty), (_ruleSetOrRuleSetItem is LabeledBaseItem ? (_ruleSetOrRuleSetItem as LabeledBaseItem).Label : string.Empty));
                else if (showNamesAndIndices)
                    toString = string.Format("{0} {1}", _ruleSetOrRuleSetItem.Name, (_ruleSetOrRuleSetItem is LabeledBaseItem ? (_ruleSetOrRuleSetItem as LabeledBaseItem).Index.ToString() : string.Empty));
                else if (showLabels && _ruleSetOrRuleSetItem is LabeledBaseItem)
                    toString = (_ruleSetOrRuleSetItem as LabeledBaseItem).Label;
            }

            string v = _value.Length > 0 ? " = " + _value : ((Count == 0) ? " <empty>" : string.Empty);
            toString = (toString.Length == 0) ? v : toString + v;

            return toString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="beginTokenDelimiter">Must be determined for a collection of ast nodes (a log) (GetUniqueParameterTokenDelimiters()).</param>
        /// <param name="endTokenDelimiter">Must be determined for a collection of ast nodes (a log) (GetUniqueParameterTokenDelimiters()).</param>
        public StringTree GetParameterizedStructure(string beginTokenDelimiter, string endTokenDelimiter,
            HashSet<BaseParameter> chosenNextValueParametersForLScope,
            HashSet<BaseParameter> chosenNextValueParametersForUAScope,
            HashSet<BaseParameter> chosenNextValueParametersForLEScope)
        {
            return GetParameterizedStructure(GetParameterTokens(beginTokenDelimiter, endTokenDelimiter),
                chosenNextValueParametersForLScope,
                chosenNextValueParametersForUAScope,
                chosenNextValueParametersForLEScope);
        }
        private StringTree GetParameterizedStructure(Dictionary<string, BaseParameter> parameterTokens,
            HashSet<BaseParameter> chosenNextValueParametersForLScope,
            HashSet<BaseParameter> chosenNextValueParametersForUAScope,
            HashSet<BaseParameter> chosenNextValueParametersForLEScope)
        {
            StringTree st = new StringTree(string.Empty, _childDelimiter);

            if (this.Count == 0)
                st.Value = ParameterizeValue(parameterTokens,
                                             chosenNextValueParametersForLScope,
                                             chosenNextValueParametersForUAScope,
                                             chosenNextValueParametersForLEScope);
            else
                foreach (ASTNode node in this)
                    st.Add(node.GetParameterizedStructure(parameterTokens,
                                                          chosenNextValueParametersForLScope,
                                                          chosenNextValueParametersForUAScope,
                                                          chosenNextValueParametersForLEScope));

            return st;
        }
        public Dictionary<string, BaseParameter> GetParameterTokens(string beginTokenDelimiter, string endTokenDelimiter)
        {
            string[] scopeIdentifiers = new string[] 
            {
                LOG_PARAMETER_SCOPE,
                USER_ACTION_PARAMETER_SCOPE,
                LOG_ENTRY_PARAMETER_SCOPE,
                LEAF_NODE_PARAMETER_SCOPE,
                ALWAYS_PARAMETER_SCOPE
            };

            var parameterTokens = new Dictionary<string, BaseParameter>();

            int i;
            foreach (string scopeIdentifier in scopeIdentifiers)
            {
                i = 1;
                foreach (var parameter in _parameters.GetAllParameters())
                    parameterTokens.Add(beginTokenDelimiter + scopeIdentifier + (i++) + endTokenDelimiter, parameter);
            }

            return parameterTokens;
        }
        private string ParameterizeValue(Dictionary<string, BaseParameter> parameterTokens,
            HashSet<BaseParameter> chosenNextValueParametersForLScope,
            HashSet<BaseParameter> chosenNextValueParametersForUAScope,
            HashSet<BaseParameter> chosenNextValueParametersForLEScope)
        {
            string parameterizedValue = _value;
            /*
                = Always           
            LN. = leaf node
            LE. = log entry
            UA. = user action
            L. = Log
            */

            HashSet<BaseParameter> chosenNextValueParametersForLNScope = new HashSet<BaseParameter>();

            foreach (string token in parameterTokens.Keys)
                if (parameterizedValue.Contains(token))
                {
                    var parameter = parameterTokens[token];
                    bool next = false;

                    if (token.Contains(LOG_PARAMETER_SCOPE))
                    {
                        next = chosenNextValueParametersForLScope.Add(parameter);
                        chosenNextValueParametersForUAScope.Add(parameter);
                        chosenNextValueParametersForLEScope.Add(parameter);
                        chosenNextValueParametersForLNScope.Add(parameter);
                    }
                    else if (token.Contains(USER_ACTION_PARAMETER_SCOPE))
                    {
                        next = chosenNextValueParametersForUAScope.Add(parameter);
                        chosenNextValueParametersForLEScope.Add(parameter);
                        chosenNextValueParametersForLNScope.Add(parameter);
                    }
                    else if (token.Contains(LOG_ENTRY_PARAMETER_SCOPE))
                    {
                        next = chosenNextValueParametersForLEScope.Add(parameter);
                        chosenNextValueParametersForLNScope.Add(parameter);
                    }
                    else if (token.Contains(LEAF_NODE_PARAMETER_SCOPE))
                    {
                        next = chosenNextValueParametersForLNScope.Add(parameter);
                    }
                    else
                    {
                        string[] split = parameterizedValue.Split(new string[] { token }, StringSplitOptions.None);

                        parameterizedValue = string.Empty;
                        for (int i = 0; i != split.Length - 1; i++)
                        {
                            parameter.Next();
                            parameterizedValue += split[i] + parameter.Value;
                        }
                        parameterizedValue += split[split.Length - 1];

                        continue;
                    }

                    if (next)
                        parameter.Next();
                    parameterizedValue = parameterizedValue.Replace(token, parameter.Value);
                }

            return parameterizedValue;
        }
        /// <summary>
        /// Combine the child values (if any) using the delimiter or returns Value.
        /// SHould only be used for building a log entry string!
        /// </summary>
        /// <returns></returns>
        public string CombineValues()
        {
            lock (this)
            {
                StringBuilder sb = new StringBuilder(_value);
                if (Count != 0)
                {
                    if (_childDelimiter.Length == 0)
                    {
                        sb.Append((this[0] as ASTNode).CombineValues());
                    }
                    else
                    {
                        for (int i = 0; i < Count - 1; i++)
                        {
                            sb.Append((this[i] as ASTNode).CombineValues());
                            sb.Append(_childDelimiter);
                        }
                        sb.Append((this[Count - 1] as ASTNode).CombineValues());
                    }
                }
                return sb.ToString();
            }
        }

        public override string ToString()
        {
            return "ASTNode: " + _value;
        }
        #endregion
    }
}