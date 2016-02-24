/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using RandomUtils;
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Runtime.Serialization;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    /// <summary>
    /// Contains UserActions that contain requests.
    /// </summary>
    [Serializable]
    [ContextMenu(new[] { "Activate_Click", "EditPlainText_Click", "Remove_Click", "Export_Click", "ExportScenarioAndUsedParameters_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { "Edit/Import", "Edit plain text", "Remove", "Export data structure", "Export scenario and used parameter data sstructures", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    public class Scenario : LabeledBaseItem, ISerializable {

        /// <summary>
        ///     This event is used in a control, this makes sure that trying to serialize the control where this event is used will not happen.
        ///     This is asynchronously invoked, invoke to the gui where needed.
        /// </summary>
        [field: NonSerialized] //This makes sure that trying to serialize the control where this event is used will not happen.
        internal event EventHandler<LexicalResultsChangedEventArgs> LexicalResultChanged;

        #region Fields
        private static readonly object _lock = new object();

        private LexicalResult _lexicalResult;
        private ScenarioRuleSet _scenarioRuleSet;

        private static Parameters _parameters;

        private int _preferredTokenDelimiterIndex;
        #endregion

        #region Properties
        [SavableCloneable, PropertyControl(1)]
        [DisplayName("Scenario rule set"), Description("You must define a rule set to validate if the scenario is correctly formated to be able to stress test.")]
        public ScenarioRuleSet ScenarioRuleSet {
            get {
                if (Solution.ActiveSolution != null && (_scenarioRuleSet.IsEmpty || _scenarioRuleSet.Parent == null))
                    _scenarioRuleSet = GetNextOrEmptyChild(typeof(ScenarioRuleSet), Solution.ActiveSolution.GetSolutionComponent(typeof(ScenarioRuleSets))) as ScenarioRuleSet;

                return _scenarioRuleSet;
            }
            set {
                if (value == null)
                    return;
                _scenarioRuleSet = value;
            }
        }

        /// <summary>
        ///     Used for getting the right token delimiters, this value is updated if the tokens are manually redetermined.
        ///     Set: if it is outside boundaries this will be corrected by going to the last or first possible index.
        /// </summary>
        [SavableCloneable]
        public int PreferredTokenDelimiterIndex {
            get { return _preferredTokenDelimiterIndex; }
            set {
                if (value < 0)
                    value = Request.MaxTokenDelimiterIndex;
                else if (value > Request.MaxTokenDelimiterIndex)
                    value = 0;

                _preferredTokenDelimiterIndex = value;
            }
        }

        public LexicalResult LexicalResult {
            get { return _lexicalResult; }
        }

        /// <summary>
        /// </summary>
        internal static Parameters Parameters {
            set { _parameters = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Contains UserActions that contain requests.
        /// </summary>
        public Scenario() {
            if (Solution.ActiveSolution == null) {
                Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
            }
            else {
                ScenarioRuleSet = GetNextOrEmptyChild(typeof(ScenarioRuleSet), Solution.ActiveSolution.GetSolutionComponent(typeof(ScenarioRuleSets))) as ScenarioRuleSet;
                _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
            }
        }

        /// <summary>
        ///     Only for sending from master to slave. (Synchronization)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public Scenario(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                Label = sr.ReadString();
                _scenarioRuleSet = sr.ReadObject() as ScenarioRuleSet;
                _preferredTokenDelimiterIndex = sr.ReadInt32();
                _parameters = sr.ReadObject() as Parameters;

                AddRangeWithoutInvokingEvent(sr.ReadCollection<BaseItem>(new List<BaseItem>()));
            }
            sr = null;
            //Not pretty, but helps against mem saturation.
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
        #endregion

        #region Functions
        /// <summary>
        ///     Only for sending from master to slave.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(Label);
                sw.WriteObject(ScenarioRuleSet);
                sw.Write(_preferredTokenDelimiterIndex);
                sw.WriteObject(_parameters);

                sw.Write(this);
                sw.AddToInfo(info);
            }
            sw = null;
            //Not pretty, but helps against mem saturation.
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e) {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            ScenarioRuleSet = GetNextOrEmptyChild(typeof(ScenarioRuleSet), Solution.ActiveSolution.GetSolutionComponent(typeof(ScenarioRuleSets))) as ScenarioRuleSet;
            Parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
        }

        /// <summary>
        ///     Threadsafe.
        /// </summary>
        /// <returns></returns>
        public Request[] GetAllRequests() {
            lock (_lock) {
                int count = GetTotalRequestCount();
                var arr = new Request[count];

                int index = 0;
                foreach (UserAction item in this)
                    foreach (Request childItem in item)
                        arr[index++] = childItem;

                return arr;
            }
        }

        public int GetTotalRequestCount() {
            int count = 0;
            foreach (UserAction item in this)
                count += item.Count;
            return count;
        }

        private void ExportScenarioAndUsedParameters_Click(object sender, EventArgs e) {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Zip Files (*.zip) | *.zip";
            sfd.Title = "Export scenario and used parameters to...";
            sfd.FileName = Label.ReplaceInvalidWindowsFilenameChars('_');
            if (sfd.ShowDialog() == DialogResult.OK) {
                Package package = null;

                try {
                    package = Package.Open(sfd.FileName, FileMode.Create, FileAccess.ReadWrite);

                    var uri = new Uri("/" + Name, UriKind.Relative);
                    var part = package.CreatePart(uri, string.Empty, CompressionOption.Maximum);
                    using (var sw = new StreamWriter(part.GetStream(FileMode.Create, FileAccess.Write)))
                        GetXmlStructure().Save(sw);

                    //Get the parameters used in the scenario.
                    string begin, end;
                    bool requestContainsTokens;
                    GetParameterTokenDelimiters(out begin, out end, out requestContainsTokens, false);

                    var usedParameters = new List<BaseParameter>();
                    var allParameterTokens = GetParameterTokens(begin, end);

                    foreach (UserAction userAction in this)
                        foreach (Request request in userAction)
                            foreach (string token in allParameterTokens.Keys) {
                                var parameter = allParameterTokens[token];
                                if (!usedParameters.Contains(parameter) && request.RequestString.Contains(token))
                                    usedParameters.Add(allParameterTokens[token]);
                            }

                    //Save thenm to the package.
                    foreach (var parameter in usedParameters) {
                        uri = new Uri("/" + parameter.Name.Replace(' ', '_') + "_0" + parameter.Index, UriKind.Relative);
                        part = package.CreatePart(uri, string.Empty, CompressionOption.Maximum);
                        using (var sw = new StreamWriter(part.GetStream(FileMode.Create, FileAccess.Write)))
                            parameter.GetXmlStructure().Save(sw);
                    }

                    package.Flush();
                }
                catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed to export the scenario + parameters", ex, new object[] { sender, e });
                }

                try {
                    if (package != null)
                        package.Close();
                }
                catch {
                    //Ignrore. Not important.
                }
            }
        }
        private void EditPlainText_Click(object sender, EventArgs e) { SolutionComponentViewManager.Show(this, typeof(PlaintTextScenarioView)); }

        /// <summary>
        ///     This will apply the ruleset (lexing).
        ///     The lexed request will be filled in for the requests.
        /// </summary>
        public void ApplyScenarioRuleSet() {
            var requestsWithErrors = new List<Request>();
            foreach (var request in GetAllRequests()) {
                try {
                    request.ApplyScenarioRuleSet(ScenarioRuleSet);
                    if (request.LexicalResult == LexicalResult.Error)
                        requestsWithErrors.Add(request);
                }
                catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed to apply scenario rule set.", ex);
                }
            }

            _lexicalResult = (requestsWithErrors.Count == 0) ? LexicalResult.OK : LexicalResult.Error;
            var requestsWithErrorsArr = requestsWithErrors.ToArray();

            if (_lexicalResult == LexicalResult.OK) {
                if (!SetParallelExecutions()) {
                    string message = this + ": Could not determine the parallel offset, hostname and redirect flag for one or more requests in the different user actions, are they correctly formatted?";
                    Loggers.Log(Level.Error, message);
                }
            }

            if (LexicalResultChanged != null)
                LexicalResultChanged(this, new LexicalResultsChangedEventArgs(requestsWithErrorsArr));
        }

        /// <summary>
        /// Webpage --> all statics in parrallel after analyzing the dom.
        /// </summary>
        /// <returns></returns>
        private bool SetParallelExecutions() {
            int offsetIndex = (int)ScenarioRuleSet.OffsetInMillisIndex - 1;//One-based
            if (offsetIndex == -1) return true;

            int hostnameIndex = (int)ScenarioRuleSet.HostnameIndex - 1;//One-based
            if (hostnameIndex == -1) return true;

            int redirectsIndex = (int)ScenarioRuleSet.RedirectsIndex - 1;//One-based
            if (redirectsIndex == -1) return true;


            bool succes = true;
            foreach (UserAction ua in this)
                foreach (Request re in ua) {
                    int count = re.LexedRequest.Count;
                    if (offsetIndex < count) {
                        int offset;
                        if (string.IsNullOrEmpty(re.LexedRequest[offsetIndex].Value)) {
                            offset = 0;
                        }
                        else if (!int.TryParse(re.LexedRequest[offsetIndex].Value, out offset)) {
                            succes = false;
                            break;
                        }
                        re.ParallelOffsetInMs = offset;
                    }
                    if (redirectsIndex < count) {
                        bool redirects;
                        if (!bool.TryParse(re.LexedRequest[redirectsIndex].Value, out redirects)) {
                            succes = false;
                            break;
                        }
                        re.Redirects = redirects;

                    }
                    if (hostnameIndex < count)
                        re.Hostname = re.LexedRequest[hostnameIndex].Value;
                }

            if (!succes) //rollback.
                foreach (UserAction ua in this)
                    foreach (Request re in ua) {
                        re.ParallelOffsetInMs = 0;
                        re.Redirects = false;
                        re.Hostname = null;
                    }


            return succes;
        }

        /// <summary>
        /// </summary>
        /// <param name="beginTokenDelimiter"></param>
        /// <param name="endTokenDelimiter"></param>
        /// <param name="requestContainsTokens">True if one of the delimiters is in a request string.</param>
        public void GetParameterTokenDelimiters(out string beginTokenDelimiter, out string endTokenDelimiter, out bool requestContainsTokens, bool autoNextOnRequestContainsTokens) {
            beginTokenDelimiter = string.Empty;
            endTokenDelimiter = string.Empty;
            requestContainsTokens = false;

            string b, e;
            bool bln;
            int tokenIndex = -1;

            foreach (Request request in GetAllRequests()) {
                tokenIndex = request.GetParameterTokenDelimiters(autoNextOnRequestContainsTokens, out b, out e, out bln, _preferredTokenDelimiterIndex);

                if (tokenIndex >= _preferredTokenDelimiterIndex) {
                    beginTokenDelimiter = b;
                    endTokenDelimiter = e;
                    if (bln) requestContainsTokens = true;

                    _preferredTokenDelimiterIndex = tokenIndex;
                }
            }
        }
        private Dictionary<string, BaseParameter> GetParameterTokens(string beginTokenDelimiter, string endTokenDelimiter) {

            var scopeIdentifiers = new[] { ASTNode.SCENARIO_PARAMETER_SCOPE, ASTNode.USER_ACTION_PARAMETER_SCOPE, ASTNode.REQUEST_PARAMETER_SCOPE, ASTNode.LEAF_NODE_PARAMETER_SCOPE, ASTNode.ALWAYS_PARAMETER_SCOPE };

            var parameterTokens = new Dictionary<string, BaseParameter>();

            int i;
            foreach (string scopeIdentifier in scopeIdentifiers) {
                i = 1;
                foreach (BaseParameter parameter in _parameters.GetAllParameters())
                    parameterTokens.Add(beginTokenDelimiter + scopeIdentifier + (i++) + endTokenDelimiter, parameter);
            }

            return parameterTokens;
        }

        /// <summary>
        ///     Get a list of string trees, these are used in the connection proxy code.
        /// </summary>
        /// <returns></returns>
        public Util.StringTree[] GetParameterizedStructure(out bool hasParameters) {
            var parameterizedStructure = new List<Util.StringTree>(Count);

            string b, e;
            bool requestContainsTokens;
            GetParameterTokenDelimiters(out b, out e, out requestContainsTokens, false);

            HashSet<BaseParameter> chosenNextValueParametersForSScope = requestContainsTokens ? new HashSet<BaseParameter>() : null;

            Dictionary<string, BaseParameter> parameterTokens = requestContainsTokens ? GetParameterTokens(b, e) : null;

            foreach (UserAction userAction in this)
                parameterizedStructure.AddRange(userAction.GetParameterizedStructure(parameterTokens, chosenNextValueParametersForSScope));

            hasParameters = parameterTokens != null;

            return parameterizedStructure.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="oldAndNewIndices"></param>
        /// <param name="oldAndNewBeginTokenDelimiter"></param>
        /// <param name="oldAndNewEndTokenDelimiter"></param>
        public void SynchronizeTokens(Dictionary<BaseParameter, KeyValuePair<int, int>> oldAndNewIndices, KeyValuePair<string, string> oldAndNewBeginTokenDelimiter,
            KeyValuePair<string, string> oldAndNewEndTokenDelimiter) {
            //Synchronize only if needed.
            if (oldAndNewIndices.Count == 0)
                return;

            var oldAndNewTokens = new Dictionary<string, string>();

            var scopeIdentifiers = new[] { ASTNode.SCENARIO_PARAMETER_SCOPE, ASTNode.USER_ACTION_PARAMETER_SCOPE,
                ASTNode.REQUEST_PARAMETER_SCOPE, ASTNode.LEAF_NODE_PARAMETER_SCOPE, ASTNode.ALWAYS_PARAMETER_SCOPE };

            foreach (string scopeIdentifier in scopeIdentifiers)
                foreach (BaseParameter parameter in oldAndNewIndices.Keys) {
                    KeyValuePair<int, int> kvp = oldAndNewIndices[parameter];
                    string oldToken = oldAndNewBeginTokenDelimiter.Key + scopeIdentifier + kvp.Key + oldAndNewEndTokenDelimiter.Key;
                    string newToken = oldAndNewBeginTokenDelimiter.Value + scopeIdentifier + kvp.Value + oldAndNewEndTokenDelimiter.Value;

                    oldAndNewTokens.Add(oldToken, newToken);
                }

            foreach (Request entry in GetAllRequests())
                foreach (string oldToken in oldAndNewTokens.Keys)
                    entry.RequestString = entry.RequestString.Replace(oldToken, oldAndNewTokens[oldToken]);

            InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cloneChildren"></param>
        /// <param name="applyRuleSet">Not needed in a distributed test</param>
        /// <param name="cloneLabelsAndRequestStringsByRef">Set to true to leverage memory usage, should only be used in a distributed test otherwise strange things will happen.</param>
        /// <param name="copyRequestsAsImported">Not needed in a distributed test</param>
        /// <returns></returns>
        public Scenario Clone(bool cloneChildren = true, bool applyRuleSet = true, bool cloneLabelsAndRequestStringsByRef = false, bool copyRequestsAsImported = true) {
            var scenario = new Scenario();
            scenario.Parent = Parent;
            scenario.Label = Label;
            scenario._scenarioRuleSet = ScenarioRuleSet;
            scenario._lexicalResult = _lexicalResult;
            scenario.PreferredTokenDelimiterIndex = _preferredTokenDelimiterIndex;

            if (cloneChildren)
                foreach (UserAction userAction in this)
                    scenario.AddWithoutInvokingEvent(userAction.Clone(scenario._scenarioRuleSet, applyRuleSet, cloneLabelsAndRequestStringsByRef, copyRequestsAsImported, true));

            return scenario;
        }

        public override BaseSolutionComponentView Activate() {
            if ((Count > 499 || GetTotalRequestCount() > 4999) &&
                MessageBox.Show("This is a large scenario! Do you want to use the plain text editor?\nYou will loose most functionality, but vApus will stay responsive and memory usage within boundaries.", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                return SolutionComponentViewManager.Show(this, typeof(PlaintTextScenarioView));
            }
            return SolutionComponentViewManager.Show(this);
        }

        public new void Dispose() {
            _parameters = null;
            base.Dispose();
        }
        #endregion

        public class LexicalResultsChangedEventArgs : EventArgs {
            public Request[] RequestsWithErrors { get; private set; }
            public LexicalResultsChangedEventArgs(Request[] requestsWithErrors) {
                RequestsWithErrors = requestsWithErrors;
            }
        }
    }
}