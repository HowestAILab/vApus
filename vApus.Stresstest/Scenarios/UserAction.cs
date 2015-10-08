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
using System.Drawing;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using vApus.SolutionTree;
using vApus.Util;
using RandomUtils;
using RandomUtils.Log;


namespace vApus.StressTest {
    /// <summary>
    /// Contains requests.
    /// </summary>
    [DisplayName("User action"), Serializable]
    public class UserAction : LabeledBaseItem, ISerializable {

        #region Fields
        private int _occurance = 1;
        private bool _useDelay = true;
        [field: NonSerialized]
        private List<string> _requestStringsAsImported = new List<string>();
        //These indices are stored here, this must be updated if something happens to a user action in the scenario.
        private List<int> _linkedToUserActionIndices = new List<int>();
        private int _linkColorRGB = -1;
        #endregion

        #region Properties
        [ReadOnly(true)]
        [SavableCloneable]
        [Description("How many times this user action occures in the scenario. Action Distribution in the stress test determines how this value will be used.")]
        public int Occurance {
            get { return _occurance; }
            set {
                if (_occurance < 0)
                    throw new ArgumentOutOfRangeException("occurance");
                _occurance = value;
            }
        }

        [ReadOnly(true)]
        [SavableCloneable]
        [Description("To pin this user action in place.")]
        public bool Pinned { get; set; }

        [ReadOnly(true)]
        [SavableCloneable]
        [Description("When true the determined delay (stress test properties) will take place after this user action."), DisplayName("Use delay")]
        public bool UseDelay {
            get { return _useDelay; }
            set { _useDelay = value; }
        }

        [ReadOnly(true)]
        [SavableCloneable]
        [DisplayName("Request strings as imported")]
        public List<string> RequestStringsAsImported {
            get { return _requestStringsAsImported; }
            set { _requestStringsAsImported = value; }
        }

        /// <summary>
        /// One-based indices
        /// </summary>
        [ReadOnly(true)]
        [SavableCloneable]
        public List<int> LinkedToUserActionIndices {
            get { return _linkedToUserActionIndices; }
            set { _linkedToUserActionIndices = value; }
        }
        public UserAction[] LinkedToUserActions {
            get {
                var l = new List<UserAction>(_linkedToUserActionIndices.Count);
                try {
                    var scenario = this.Parent as Scenario;

                    foreach (int i in _linkedToUserActionIndices)
                        l.Add(scenario[i - 1] as UserAction);

                } catch (Exception ex) {
                    Loggers.Log(Level.Warning, "Failed linking user actions", ex);
                }
                return l.ToArray();
            }
        }

        [ReadOnly(true)]
        [SavableCloneable]
        public int LinkColorRGB {
            get { return _linkColorRGB; }
            set { _linkColorRGB = value; }
        }
        public Color LinkColor {
            get {
                if (_linkColorRGB != -1) {
                    int red = _linkColorRGB >> 16;
                    int green = (_linkColorRGB >> 8) & 0x00FF;
                    int blue = _linkColorRGB & 0x0000FF;
                    return Color.FromArgb(red, green, blue);
                }
                return Color.Transparent;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Contains requests.
        /// </summary>
        public UserAction() {
            ShowInGui = false;
        }
        /// <summary>
        /// Contains requests.
        /// </summary>
        /// <param name="label"></param>
        public UserAction(string label)
            : this() {
            Label = label;
        }
        public UserAction(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                Label = sr.ReadString();
                _occurance = sr.ReadInt32();
                Pinned = sr.ReadBoolean();
                _useDelay = sr.ReadBoolean();
                _linkedToUserActionIndices = sr.ReadCollection<int>(_linkedToUserActionIndices) as List<int>;

                AddRangeWithoutInvokingEvent(sr.ReadCollection<BaseItem>(new List<BaseItem>()));
            }
            sr = null;
        }
        #endregion

        #region Functions
        /// <summary>
        /// </summary>
        /// <param name="beginTokenDelimiter">Needed to dermine parameter tokens</param>
        /// <param name="endTokenDelimiter">Needed to dermine parameter tokens</param>
        /// <param name="containsTokens">Does the scenario contain the tokens? If not getting the structure can be simpler/faster.</param>
        /// <param name="chosenNextValueParametersForSScope">Can be an empty hash set but may not be null, used to store all these values for the right scope.</param>
        /// <returns></returns>
        internal Util.StringTree[] GetParameterizedStructure(Dictionary<string, BaseParameter> parameterTokens, HashSet<BaseParameter> chosenNextValueParametersForSScope) {
            var parameterizedStructure = new Util.StringTree[Count];

            HashSet<BaseParameter> chosenNextValueParametersForUAScope = parameterTokens == null ? null : new HashSet<BaseParameter>();

            if (parameterTokens == null && Count > 1)
                Parallel.For(0, parameterizedStructure.Length, (i) => {
                    parameterizedStructure[i] = (this[i] as Request).GetParameterizedStructure(parameterTokens, chosenNextValueParametersForSScope, chosenNextValueParametersForUAScope);
                });
            else
                for (int i = 0; i != parameterizedStructure.Length; i++)
                    parameterizedStructure[i] = (this[i] as Request).GetParameterizedStructure(parameterTokens, chosenNextValueParametersForSScope, chosenNextValueParametersForUAScope);
            return parameterizedStructure;
        }

        public void AddToLink(UserAction userAction, int[] linkColorToChooseFrom, bool invokeSolutionComponentChanched = true) {
            var scenario = this.Parent as Scenario;

            bool canDetermineColor = _linkedToUserActionIndices.Count == 0;

            //Add linked user actions to this link, because indices can change, re-add the already linked user actions
            var toLink = new List<UserAction>();
            toLink.AddRange(LinkedToUserActions);
            toLink.Add(userAction);
            toLink.AddRange(userAction.LinkedToUserActions);

            scenario.RemoveRangeWithoutInvokingEvent(toLink);

            _linkedToUserActionIndices.Clear();
            userAction.LinkedToUserActionIndices.Clear();

            //Determine the link color.
            if (canDetermineColor) {
                var exclude = new List<int>();
                foreach (UserAction ua in scenario)
                    if (ua != this && ua.LinkedToUserActionIndices.Count != 0)
                        exclude.Add(ua.LinkColorRGB);

                foreach (int c in linkColorToChooseFrom)
                    if (!exclude.Contains(c)) {
                        _linkColorRGB = c;
                        break;
                    }
            }

            //Re-add the useractions the scenario.
            int index = this.Index;
            if (_linkedToUserActionIndices.Count != 0)
                index = _linkedToUserActionIndices[_linkedToUserActionIndices.Count - 1];

            foreach (var ua in toLink) {
                if (index < scenario.Count)
                    scenario.InsertWithoutInvokingEvent(index, ua);
                else
                    scenario.AddWithoutInvokingEvent(ua);
                ua.Pinned = Pinned;
                _linkedToUserActionIndices.Add(ua.Index);

                ua.LinkColorRGB = _linkColorRGB;
                ++index;
            }

            if (invokeSolutionComponentChanched)
                scenario.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        public void RemoveFromLink(UserAction userAction) {
            if (_linkedToUserActionIndices.Remove(userAction.Index)) {
                userAction.LinkColorRGB = -1;
                //Put the unlinked useraction behind this linked group if need be.
                if (_linkedToUserActionIndices.Count != 0) {
                    var scenario = this.Parent as Scenario;

                    var arr = LinkedToUserActions;

                    scenario.RemoveWithoutInvokingEvent(userAction);

                    _linkedToUserActionIndices.Clear();
                    foreach (var ua in arr)
                        _linkedToUserActionIndices.Add(ua.Index);

                    int lastIndex = _linkedToUserActionIndices[_linkedToUserActionIndices.Count - 1];
                    if (lastIndex < scenario.Count)
                        scenario.InsertWithoutInvokingEvent(lastIndex, userAction);
                    else
                        scenario.AddWithoutInvokingEvent(userAction);
                }

                InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }
        public void MergeLinked() {
            var scenario = this.Parent as Scenario;
            var l = new List<int>(_linkedToUserActionIndices.Count + 1);
            l.AddRange(_linkedToUserActionIndices);
            l.Add(this.Index);
            l.Sort();

            var toMerge = new List<UserAction>(l.Count);
            foreach (int i in l) toMerge.Add(scenario[i - 1] as UserAction);

            var merged = toMerge[0];
            for (int i = 1; i != toMerge.Count; i++) {
                var ua = toMerge[i] as UserAction;
                merged.AddRangeWithoutInvokingEvent(ua);
                merged.RequestStringsAsImported.AddRange(ua.RequestStringsAsImported);
                scenario.RemoveWithoutInvokingEvent(ua);
            }

            //Update the linked indices for the other user actions.
            for (int i = merged.Index; i != scenario.Count; i++) {
                var ua = scenario[i] as UserAction;
                var linkedIndices = ua.LinkedToUserActionIndices.ToArray();
                for (int j = 0; j != linkedIndices.Length; j++)
                    ua.LinkedToUserActionIndices[j] = linkedIndices[j] - merged.LinkedToUserActionIndices.Count;
            }

            merged.LinkedToUserActionIndices.Clear();
            scenario.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        /// <summary>
        /// This user action will find its own parent scenario, more convinient but not so fast.
        /// </summary>
        /// <param name="linkUserAction"></param>
        /// <returns></returns>
        public bool IsLinked(out UserAction linkUserAction) {
            return IsLinked(Parent as Scenario, out linkUserAction);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scenario">The scenario where this user action resides in.</param>
        /// <param name="linkUserAction"></param>
        /// <returns></returns>
        public bool IsLinked(Scenario scenario, out UserAction linkUserAction) {
            linkUserAction = null;
            if (LinkedToUserActionIndices.Count == 0) {
                int index = Index;
                for (int i = index - 2; i != -1; i--) {
                    var ua = scenario[i] as UserAction;
                    if (ua.LinkedToUserActionIndices.Contains(index)) {
                        linkUserAction = ua;
                        return true;
                    }
                }
            } else {
                linkUserAction = this;
                return true;
            }
            return false;
        }
        public void Split() {
            var scenario = this.Parent as Scenario;
            int index = this.Index - 1;
            var l = new List<UserAction>(this.Count);
            int i = 0;
            foreach (Request request in this) {
                var ua = new UserAction(request.RequestString.Length < 101 ? request.RequestString : request.RequestString.Substring(0, 100) + "...");
                ua.AddWithoutInvokingEvent(request);

                if (i < this.RequestStringsAsImported.Count)
                    ua.RequestStringsAsImported.Add(this.RequestStringsAsImported[i]);

                l.Add(ua);
                ++i;
            }

            //Update the linked indices for the other user actions.
            int add = l.Count - 1;
            for (int j = index + 1; j != scenario.Count; j++) {
                var ua = scenario[j] as UserAction;
                var linkedIndices = ua.LinkedToUserActionIndices.ToArray();
                for (int k = 0; k != linkedIndices.Length; k++)
                    ua.LinkedToUserActionIndices[k] = linkedIndices[k] + add;
            }

            scenario.InserRangeWithoutInvokingEvent(index, l);
            scenario.RemoveWithoutInvokingEvent(this);

            scenario.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scenarioRuleSet"></param>
        /// <param name="applyRuleSet">Not needed in a distributed test</param>
        /// <param name="cloneLabelAndRequestStringByRef">Set to true to leverage memory usage, should only be used in a distributed test otherwise strange things will happen.</param>
        /// <param name="copyRequestAsImported">Not needed in a distributed test</param>
        /// <param name="copyLinkedUserActionIndices">Needed in a distributed test</param>
        /// <returns></returns>
        public UserAction Clone(ScenarioRuleSet scenarioRuleSet, bool applyRuleSet, bool cloneLabelAndRequestStringByRef, bool copyRequestAsImported, bool copyLinkedUserActionIndices) {
            UserAction userAction = new UserAction();
            if (cloneLabelAndRequestStringByRef)
                SetRequestStringByRef(userAction, ref _label);
            else
                userAction.Label = _label;

            userAction.SetParent(Parent);
            userAction.Occurance = _occurance;
            userAction.Pinned = Pinned;
            userAction.UseDelay = _useDelay;

            if (copyRequestAsImported) {
                var arr = new string[_requestStringsAsImported.Count];
                _requestStringsAsImported.CopyTo(arr);
                userAction.RequestStringsAsImported = new List<string>(arr);
            }

            if (copyLinkedUserActionIndices) {
                foreach (int i in _linkedToUserActionIndices) userAction._linkedToUserActionIndices.Add(i);
                userAction._linkColorRGB = _linkColorRGB;
            }

            foreach (Request request in this)
                userAction.AddWithoutInvokingEvent(request.Clone(scenarioRuleSet, applyRuleSet, cloneLabelAndRequestStringByRef));

            return userAction;
        }
        private void SetRequestStringByRef(UserAction userAction, ref string label) { userAction._label = label; }
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(Label);
                sw.Write(_occurance);
                sw.Write(Pinned);
                sw.Write(_useDelay);
                sw.Write(_linkedToUserActionIndices);

                sw.Write(this);
                sw.AddToInfo(info);
            }
            sw = null;
        }
        #endregion
    }
}
