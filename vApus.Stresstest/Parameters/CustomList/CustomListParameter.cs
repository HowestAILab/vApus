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
    [DisplayName("Custom List Parameter"), Serializable]
    public class CustomListParameter : BaseParameter {
        #region Fields

        private string[] _customList = new string[] { };
        private bool _random;

        #endregion

        #region Properties

        [PropertyControl(1), SavableCloneable]
        [DisplayName("Custom List"),
         Description(
             "If unique, one (randomly picked) value can be given only once until none are left, then values will be reused."
             )]
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

        #endregion

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

        #region Functions

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

            _value = _customList[index];
        }

        public override void ResetValue() {
            if (_customList.Length > 0)
                _value = _customList[0];
            _chosenValues.Clear();
        }

        #endregion
    }
}