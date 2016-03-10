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
using System.ComponentModel;
using System.Runtime.Serialization;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    /// <summary>
    /// Contains a captured request to a server app.
    /// </summary>
    [Serializable]
    public class Request : LabeledBaseItem, ISerializable {

        #region Fields
        private static readonly char[] _beginParameterTokenDelimiterCanditates = new[] { '{', '§', '<', '[', '(', '\\', '#', '$', '£', '€', '%', '*', '²', '³', '°' };
        private static readonly char[] _endParameterTokenDelimiterCanditates = new[] { '}', '§', '>', ']', ')', '/', '#', '$', '£', '€', '%', '*', '²', '³', '°' };

        private string _requestString = string.Empty;

        private bool _useDelay = false;

        private int _parallelOffsetInMs;
        private string _hostname;
        private bool _redirects;

        private Request _sameAs;

        private ASTNode _lexedRequest;
        private LexicalResult _lexicalResult = LexicalResult.Error;
        #endregion

        #region Properties
        /// <summary>
        ///     Call ApplyScenarioRuleSet after setting this.
        /// </summary>
        [ReadOnly(true)]
        [SavableCloneable]
        [DisplayName("Request string")]
        public string RequestString {
            get { return _requestString; }
            set { _requestString = value; }
        }

        /// <summary>
        ///     Is valid after calling ApplyScenarioRuleSet.
        /// </summary>
        [Description("Specifies if the request is valid or not."), DisplayName("Lexical result")]
        public LexicalResult LexicalResult {
            get { return _lexicalResult; }
        }

        /// <summary>
        ///     Is valid after calling ApplyScenarioRuleSet.
        /// </summary>
        internal ASTNode LexedRequest { get { return _lexedRequest; } }

        [ReadOnly(true)]
        [Description("When true the determined delay (stress test properties) will take place after this request."), DisplayName("Use delay")]
        public bool UseDelay {
            get { return _useDelay; }
            set { _useDelay = value; }
        }

        /// <summary>
        /// For parallel executions.
        /// </summary>
        [ReadOnly(true)]
        [SavableCloneable]
        public int ParallelOffsetInMs {
            get { return _parallelOffsetInMs; }
            set { _parallelOffsetInMs = value; }
        }

        /// <summary>
        /// For parallel executions.
        /// </summary>
        [ReadOnly(true)]
        [SavableCloneable]
        public string Hostname {
            get { return _hostname; }
            set { _hostname = value; }
        }

        /// <summary>
        /// For parallel executions.
        /// </summary>
        [ReadOnly(true)]
        [SavableCloneable]
        public bool Redirects {
            get { return _redirects; }
            set { _redirects = value; }
        }


        /// <summary>
        ///     The maximum index of the token delimiters that can be chosen.
        /// </summary>
        public static int MaxTokenDelimiterIndex {
            get { return (_beginParameterTokenDelimiterCanditates.Length * 3) - 1; }
        }

        /// <summary>
        /// Only happens in stress test core when determining test patterns.
        /// </summary>
        public Request SameAs {
            get { return _sameAs; }
            set { _sameAs = value; }
        }

        internal bool ExecuteInParallelWithPrevious { get; set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Contains a captured request to a server app.
        /// </summary>
        public Request() { ShowInGui = false; }
        /// <summary>
        /// Contains a captured request to a server app.
        /// </summary>
        /// <param name="requestString">Request string as imported will get this value also.</param>
        public Request(string requestString)
            : this() {
            RequestString = requestString;
        }

        public Request(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                Label = sr.ReadString();
                _requestString = sr.ReadString();
                _useDelay = sr.ReadBoolean();
                _parallelOffsetInMs = sr.ReadInt32();
                _hostname = sr.ReadString();
                _redirects = sr.ReadBoolean();
            }
            sr = null;
        }
        #endregion

        #region Functions
        /// <summary>
        ///     This will apply the ruleset (lexing).
        ///     The lexed request will be filled in.
        /// </summary>
        public void ApplyScenarioRuleSet(ScenarioRuleSet scenarioRuleSet) {
            if (_lexedRequest != null) {
                _lexedRequest.Dispose();
                _lexedRequest = null;
            }

            _lexicalResult = (scenarioRuleSet == null) ? LexicalResult.Error : scenarioRuleSet.TryLexicalAnalysis(_requestString, out _lexedRequest);
        }

        /// <summary>
        ///     (Re)determines and gets a unique begin and end parameter token delimiter for the combined value (CombineValues()) of this node.
        /// </summary>
        /// <param name="beginTokenDelimiter"></param>
        /// <param name="endTokenDelimiter"></param>
        /// <param name="requestStringContainsTokens">if not unique in the request.</param>
        /// <param name="offset">The offset</param>
        /// <returns>
        ///     A unique index of the chosen strings to cross check with other ast nodes.
        ///     If is is greater than for a previous node they are also unique for that node, if it is smalller they are not unique for the previous node
        /// </returns>
        internal int GetParameterTokenDelimiters(bool autoNextOnRequestContainsTokens, out string beginTokenDelimiter, out string endTokenDelimiter, out bool requestStringContainsTokens, int offset = 0) {
            beginTokenDelimiter = string.Empty;
            endTokenDelimiter = string.Empty;
            requestStringContainsTokens = false;

            var parameterTokenDelimiterIndices = new[] { 0, -1, -1 };

            int uniqueCombinedIndex = 0;
            for (int i = 0; i < parameterTokenDelimiterIndices.Length; i++)
                for (int j = 0; j < _beginParameterTokenDelimiterCanditates.Length; j++) {
                    if (j > parameterTokenDelimiterIndices[i])
                        parameterTokenDelimiterIndices[i] = j;

                    if (uniqueCombinedIndex >= offset) {
                        BuildParameterTokenDelimiter(parameterTokenDelimiterIndices, out beginTokenDelimiter, out endTokenDelimiter);

                        requestStringContainsTokens = (_requestString.Contains(beginTokenDelimiter) || _requestString.Contains(endTokenDelimiter));
                        if (!(requestStringContainsTokens && autoNextOnRequestContainsTokens))
                            return uniqueCombinedIndex;
                    }
                    ++uniqueCombinedIndex;
                }

            throw new Exception("No unique delimiters could be chosen!");
        }

        /// <summary>
        ///     Just builds.
        /// </summary>
        /// <param name="beginDelimiter"></param>
        /// <param name="endDelimiter"></param>
        private void BuildParameterTokenDelimiter(int[] parameterTokenDelimiterIndices, out string beginDelimiter, out string endDelimiter) {
            beginDelimiter = string.Empty;
            endDelimiter = string.Empty;
            foreach (int i in parameterTokenDelimiterIndices) {
                if (i == -1)
                    break;

                beginDelimiter += _beginParameterTokenDelimiterCanditates[i];
                endDelimiter += _endParameterTokenDelimiterCanditates[i];
            }

            endDelimiter = endDelimiter.Reverse();
        }

        /// <summary>
        /// Apply the scenario rule set before doing this.
        /// </summary>
        /// <param name="beginTokenDelimiter">Needed to dermine parameter tokens</param>
        /// <param name="endTokenDelimiter">Needed to dermine parameter tokens</param>
        /// <param name="chosenNextValueParametersForSScope">Can be an empty hash set but may not be null, used to store all these values for the right scope.</param>
        /// <param name="chosenNextValueParametersForUAScope">Can be an empty hash set but may not be null, used to store all these values for the right scope. If the request is not in a user action this should be an empty hash set.</param>
        /// <returns></returns>
        internal Util.StringTree GetParameterizedStructure(Dictionary<string, BaseParameter> parameterTokens, HashSet<BaseParameter> chosenNextValueParametersForSScope, HashSet<BaseParameter> chosenNextValueParametersForUAScope) {

            HashSet<BaseParameter> chosenNextValueParametersForREScope = parameterTokens == null ? null : new HashSet<BaseParameter>();

            return _lexedRequest.GetParameterizedStructure(parameterTokens, chosenNextValueParametersForSScope, chosenNextValueParametersForUAScope, chosenNextValueParametersForREScope);
        }

        /// <summary>
        /// Clones and applies the scenario rule set.
        /// </summary>
        /// <param name="scenarioRuleSet"></param>
        /// <param name="applyRuleSet">Not needed in a distributed test.</param>
        /// <param name="cloneRequestStringByRef">Set to true to leverage memory usage, should only be used in a distributed test otherwise strange things will happen.</param>
        /// <returns></returns>
        public Request Clone(ScenarioRuleSet scenarioRuleSet, bool applyRuleSet, bool cloneRequestStringByRef) {
            var request = new Request();
            request.SetParent(Parent);

            if (cloneRequestStringByRef)
                SetRequestStringByRef(request, ref _requestString);
            else
                request._requestString = _requestString;

            request._hostname = _hostname;
            request._parallelOffsetInMs = _parallelOffsetInMs;
            request._redirects = _redirects;

            if (applyRuleSet)
                request.ApplyScenarioRuleSet(scenarioRuleSet);

            return request;
        }
        private void SetRequestStringByRef(Request request, ref string requestString) {
            request._requestString = requestString;
        }

        /// <summary>
        /// Request #: RequestString.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return (base.ToString() == null ? string.Empty : base.ToString() + ": ") + _requestString;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(Label);
                sw.Write(_requestString);
                sw.Write(_useDelay);
                sw.Write(_parallelOffsetInMs);
                sw.Write(_hostname);
                sw.Write(_redirects);
                sw.AddToInfo(info);
            }
            sw = null;
        }


        public new void Dispose() {
            if (_lexedRequest != null) {
                _lexedRequest.Dispose();
                _lexedRequest = null;
            }
            base.Dispose();
        }
        #endregion
    }
}
