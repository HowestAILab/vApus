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

namespace vApus.Stresstest {
    /// <summary>
    /// Holds a list of strings, those can be generated from other parameter types.
    /// </summary>
    [DisplayName("Custom List Parameter"), Serializable]
    public class CustomListParameter : BaseParameter {

        #region Fields
        private string[] _customList = new string[] { };
        private bool _random;
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
            int index;
            if (_chosenValues.Count == _customList.Length)
                _chosenValues.Clear();

            index = _random ? _r.Next(_customList.Length) : _chosenValues.Count;

            //Use the index here (lightweighter)
            while (!_chosenValues.Add(index)) {
                index = _random ? _r.Next(_customList.Length) : _chosenValues.Count;

                if (_chosenValues.Count == _customList.Length)
                    _chosenValues.Clear();
            }

            Value = _customList[index];
        }

        public override void ResetValue() {
            if (_customList.Length > 0)
                Value = _customList[0];
            _chosenValues.Clear();
        }
        #endregion
    }
}