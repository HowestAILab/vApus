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
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest
{
    [DisplayName("User Action"), Serializable]
    public class UserAction : LabeledBaseItem
    {
        #region Fields

        private int _occurance = 1;
        private bool _pinned;

        #endregion

        #region Properties

        [ReadOnly(true)]
        [SavableCloneable]
        [Description(
            "How many times this user action occures in the log. Action and Log Entry Distribution in the stresstest determines how this value will be used."
            )]
        public int Occurance
        {
            get { return _occurance; }
            set
            {
                if (_occurance < 0)
                    throw new ArgumentOutOfRangeException("occurance");
                _occurance = value;
            }
        }

        [ReadOnly(true)]
        [SavableCloneable]
        [Description("To pin this user action in place.")]
        public bool Pinned
        {
            get { return _pinned; }
            set { _pinned = value; }
        }

        #endregion

        #region Constructors

        public UserAction()
        {
            ShowInGui = false;
        }

        public UserAction(string label)
            : this()
        {
            Label = label;
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="beginTokenDelimiter">Needed to dermine parameter tokens</param>
        /// <param name="endTokenDelimiter">Needed to dermine parameter tokens</param>
        /// <param name="chosenNextValueParametersForLScope">Can be an empty hash set but may not be null, used to store all these values for the right scope.</param>
        /// <returns></returns>
        internal List<StringTree> GetParameterizedStructure(string beginTokenDelimiter, string endTokenDelimiter,
                                                            HashSet<BaseParameter> chosenNextValueParametersForLScope)
        {
            var parameterizedStructure = new List<StringTree>();
            var chosenNextValueParametersForUAScope = new HashSet<BaseParameter>();

            foreach (LogEntry logEntry in this)
                parameterizedStructure.Add(logEntry.GetParameterizedStructure(beginTokenDelimiter, endTokenDelimiter,
                                                                              chosenNextValueParametersForLScope,
                                                                              chosenNextValueParametersForUAScope));
            return parameterizedStructure;
        }

        public UserAction Clone()
        {
            var userAction = new UserAction(Label);
            userAction.SetParent(Parent, false);
            userAction.Occurance = _occurance;
            userAction.Pinned = _pinned;

            foreach (LogEntry entry in this)
                userAction.AddWithoutInvokingEvent(entry.Clone(), false);

            return userAction;
        }
    }
}