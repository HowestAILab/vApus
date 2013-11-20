/*
 * Copyright 2010 (c) Sizing Servers Lab
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

namespace vApus.Stresstest {
    /// <summary>
    /// Holds a list of strings, those can be generated from other parameter types.
    /// </summary>
    [DisplayName("Custom List Parameter"), Serializable]
    public class CustomListParameter : BaseParameter {

        #region Fields
        private string[] _customList = new string[] { };
        private bool _random;
        private int _currentValueIndex;
        private CustomListParameter _linkTo;
        #endregion

        #region Properties
        [PropertyControl(1), SavableCloneable]
        [Description("If unique, one (randomly picked) value can be given only once until none are left, then values will be reused."), DisplayName("Custom List")]
        public string[] CustomList {
            get { return _customList; }
            set { _customList = value; }
        }

        [PropertyControl(2), SavableCloneable]
        [Description("If false output values will be chosen in sequence.")]
        public bool Random {
            get { return _random; }
            set { _random = value; }
        }

#if EnableBetaFeature
        [PropertyControl(int.MaxValue), SavableCloneable]
#else
        [SavableCloneable]
#endif
        [Description("You can link this custom list parameter to another. This means that when a value is asked for this parameter in a stresstest, a value at the same index is asked for the other. Handy for instance when you need to link user names to passwords."), DisplayName("Link to")]
        public CustomListParameter LinkTo {
            get {
                if ((!this.IsEmpty && _linkTo == null) || _linkTo.Parent == null || _linkTo._linkTo != this) { //Links should work in both directions. if the other becomes empty or not this than this should be cleared.
                    _linkTo = GetEmpty(typeof(CustomListParameter), Parent as CustomListParameters) as CustomListParameter;
                    _linkTo.SetParent(Parent, false);
                    _linkTo.SetTag(this); //To not include 'this' in the selectabel values on the gui.
                }

                return _linkTo;
            }
            set {
                if (value == null)
                    return;
                if (value == this)
                    throw new Exception("Cannot link to self.");
                value.ParentIsNull -= _linkTo_ParentIsNull;
                _linkTo = value;
                _linkTo.SetTag(this);
                if (!_linkTo.IsEmpty) {
                    _linkTo._linkTo = this; //Make a link from the other side.
                    _linkTo._linkTo.SetTag(_linkTo);
                }
                _linkTo.ParentIsNull += _linkTo_ParentIsNull;
            }
        }
        public BaseParameter GenerateFromParameter {
            get {
                foreach (BaseParameter p in this)
                    if (!p.IsEmpty)
                        return p;
                return this[0] as BaseParameter;
            }
            set {
                if (value is NumericParameter) {
                    this[0].IsEmpty = false;
                    this[1].IsEmpty = true;
                    this[2].IsEmpty = true;
                } else if (value is TextParameter) {
                    this[0].IsEmpty = true;
                    this[1].IsEmpty = false;
                    this[2].IsEmpty = true;
                } else {
                    this[0].IsEmpty = true;
                    this[1].IsEmpty = true;
                    this[2].IsEmpty = false;
                }
            }
        }
        public int CurrentValueIndex {
            get { return _currentValueIndex; }
            private set { _currentValueIndex = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Holds a list of strings, those can be generated from other parameter types.
        /// </summary>
        public CustomListParameter() {
            BaseParameter p = new NumericParameter();
            p.ShowInGui = false;
            AddAsDefaultItem(p);

            p = new TextParameter();
            p.ShowInGui = false;
            AddAsDefaultItem(p);

            p = new CustomRandomParameter();
            p.ShowInGui = false;
            AddAsDefaultItem(p);
        }
        #endregion

        #region Functions
        private void _linkTo_ParentIsNull(object sender, EventArgs e) {
            if (_linkTo == sender) {
                LinkTo = GetEmpty(typeof(CustomListParameter), Parent as CustomListParameters) as CustomListParameter;
                _linkTo.SetParent(Parent, false);
                _linkTo.SetTag(this);
            }
        }
        public void Add(int count, BaseParameter baseParameterType) {
            var l = new List<string>(count + _customList.Length);
            l.AddRange(_customList);
            for (int i = 0; i < count; i++) {
                l.Add(baseParameterType.Value);
                baseParameterType.Next();
            }
            _customList = l.ToArray();
            InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        public override void Next() {
            if (_chosenValues.Count == _customList.Length)
                _chosenValues.Clear();

            Random random = null;
            if (_random) {
                random = new Random(Guid.NewGuid().GetHashCode());
                _currentValueIndex = random.Next(_customList.Length);
            } else {
                _currentValueIndex = _chosenValues.Count;
            }

            //Use the index here (lightweighter)
            while (!_chosenValues.Add(_currentValueIndex)) {
                _currentValueIndex = _random ? random.Next(_customList.Length) : _chosenValues.Count;

                if (_chosenValues.Count == _customList.Length)
                    _chosenValues.Clear();
            }

            Value = _customList[_currentValueIndex];
        }

        /// <summary>
        /// Takes overflows into account.
        /// </summary>
        /// <param name="index"></param>
        public void SetValueAt(int index) {
            _currentValueIndex = index;
            while (_currentValueIndex >= _customList.Length)
                _currentValueIndex -= _customList.Length;

            _chosenValues.Add(_currentValueIndex);
            if (_chosenValues.Count == _customList.Length)
                _chosenValues.Clear();
            Value = _customList[_currentValueIndex];
        }

        public override void ResetValue() {
            if (_customList.Length > 0)
                Value = _customList[0];
            _chosenValues.Clear();
        }
        #endregion
    }
}