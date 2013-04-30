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
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    [DisplayName("User Action"), Serializable]
    public class UserAction : LabeledBaseItem {

        #region Fields
        private int _occurance = 1;
        private bool _pinned;
        private bool _useDelay = true;
        [field: NonSerialized]
        private List<string> _logEntryStringsAsImported = new List<string>();
        //These indices are stored here, this must be updated if something happens to a user action in the log.
        private List<int> _linkedToUserActionIndices = new List<int>();
        private int _linkColorARGB = -1;
        #endregion

        #region Properties

        [ReadOnly(true)]
        [SavableCloneable]
        [Description("How many times this user action occures in the log. Action and Log Entry Distribution in the stresstest determines how this value will be used.")]
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
        public bool Pinned {
            get { return _pinned; }
            set { _pinned = value; }
        }

        [ReadOnly(true)]
        [SavableCloneable]
        [Description("When true the determined delay (stresstest properties) will take place after this user action.")
        , DisplayName("Use Delay")]
        public bool UseDelay {
            get { return _useDelay; }
            set { _useDelay = value; }
        }

        [ReadOnly(true)]
        [SavableCloneable]
        [DisplayName("Log Entry Strings as Imported")]
        public List<string> LogEntryStringsAsImported {
            get { return _logEntryStringsAsImported; }
            set { _logEntryStringsAsImported = value; }
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
            get { return null; }
        }
        [ReadOnly(true)]
        [SavableCloneable]
        public int LinkColorARGB {
            get { return _linkColorARGB; }
            set { _linkColorARGB = value; }
        }
        public Color LinkColor {
            get { return Color.FromArgb(_linkColorARGB); }
            set { _linkColorARGB = value.ToArgb(); }
        }
        #endregion

        #region Constructors

        public UserAction() {
            ShowInGui = false;
        }

        public UserAction(string label)
            : this() {
            Label = label;
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="beginTokenDelimiter">Needed to dermine parameter tokens</param>
        /// <param name="endTokenDelimiter">Needed to dermine parameter tokens</param>
        /// <param name="chosenNextValueParametersForLScope">Can be an empty hash set but may not be null, used to store all these values for the right scope.</param>
        /// <returns></returns>
        internal List<StringTree> GetParameterizedStructure(string beginTokenDelimiter, string endTokenDelimiter, HashSet<BaseParameter> chosenNextValueParametersForLScope) {
            var parameterizedStructure = new List<StringTree>();
            var chosenNextValueParametersForUAScope = new HashSet<BaseParameter>();

            foreach (LogEntry logEntry in this)
                parameterizedStructure.Add(logEntry.GetParameterizedStructure(beginTokenDelimiter, endTokenDelimiter,
                                                                              chosenNextValueParametersForLScope,
                                                                              chosenNextValueParametersForUAScope));
            return parameterizedStructure;
        }

        public void AddToLink(UserAction userAction) {
            var log = this.Parent as Log;
            log.RemoveWithoutInvokingEvent(userAction);
            int index = this.Index;
            if (_linkedToUserActionIndices.Count != 0)
                index = _linkedToUserActionIndices[_linkedToUserActionIndices.Count - 1];
            if (index < log.Count)
                log.InsertWithoutInvokingEvent(index, userAction, false);
            else
                log.AddWithoutInvokingEvent(userAction, false);
            _linkedToUserActionIndices.Add(userAction.Index);

            log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        public void RemoveFromLink(UserAction userAction) {
            if (_linkedToUserActionIndices.Remove(userAction.Index))
                InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        public void MergeLinked() {
            var log = this.Parent as Log;
            var l = new List<int>(_linkedToUserActionIndices.Count + 1);
            l.AddRange(_linkedToUserActionIndices);
            l.Add(this.Index);
            l.Sort();

            var toMerge = new List<UserAction>(l.Count);
            foreach (int i in l) toMerge.Add(log[i - 1] as UserAction);

            var merged = toMerge[0];
            for (int i = 1; i != toMerge.Count; i++) {
                var ua = toMerge[i] as UserAction;
                merged.AddRangeWithoutInvokingEvent(ua, false);
                merged.LogEntryStringsAsImported.AddRange(ua.LogEntryStringsAsImported);
                log.RemoveWithoutInvokingEvent(ua);
            }
            merged.LinkedToUserActionIndices.Clear();
            log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        public void Split() {
            var log = this.Parent as Log;
            int index = this.Index - 1;
            var l = new List<UserAction>(this.Count);
            int i = 0;
            foreach (LogEntry logEntry in this) {
                var ua = new UserAction(logEntry.LogEntryString.Length < 101 ? logEntry.LogEntryString : logEntry.LogEntryString.Substring(0, 100) + "...");
                ua.AddWithoutInvokingEvent(logEntry, false);
                
                if (i < this.LogEntryStringsAsImported.Count)
                    ua.LogEntryStringsAsImported.Add(this.LogEntryStringsAsImported[i]);

                l.Add(ua);
                ++i;
            }
            log.InserRangeWithoutInvokingEvent(index, l, false);
            log.RemoveWithoutInvokingEvent(this);

            log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        public UserAction Clone() {
            UserAction userAction = new UserAction(Label);
            userAction.SetParent(Parent, false);
            userAction.Occurance = _occurance;
            userAction.Pinned = _pinned;
            userAction.UseDelay = _useDelay;
            userAction.LogEntryStringsAsImported = _logEntryStringsAsImported;

            foreach (LogEntry entry in this) userAction.AddWithoutInvokingEvent(entry.Clone(), false);

            return userAction;
        }
    }
}
