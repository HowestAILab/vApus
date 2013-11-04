/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vApus.Util {
    public struct StringTree {

        #region Fields
        private string _value, _childDelimiter;
        private StringTree[] _childs;
        #endregion

        #region Properties
        public string Value { get { return _value; } }

        public string ChildDelimiter { get { return _childDelimiter; } }

        public StringTree this[int index] {
            get { return _childs[index]; }
            set { _childs[index] = value; }
        }

        public int Count { get { return _childs.Length; } }
        #endregion

        #region Constructors

        /// <summary>
        ///     A simple structure for representing a tree of strings (Value).
        ///     New string trees can be added as nodes of this one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="childDelimiter">Use combine values to return a string representation of the elements in this structure.</param>
        public StringTree(string value = "", string childDelimiter = "", int count = 0) {
            if (value == null)
                throw new ArgumentNullException("value");
            if (childDelimiter == null)
                throw new ArgumentNullException("childDelimiter");

            _childs = new StringTree[count];

            _value = value;
            _childDelimiter = childDelimiter;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Not thread safe
        /// </summary>
        /// <returns>If this has no childs the value is returned.</returns>
        public string CombineValues() {
            if (_childs.Length == 0)
                return _value;

            var sb = new StringBuilder();
            if (_childDelimiter.Length == 0) {
                sb.Append(this[0].CombineValues());
            } else {
                for (int i = 0; i < _childs.Length - 1; i++) {
                    sb.Append(this[i].CombineValues());
                    sb.Append(_childDelimiter);
                }
                sb.Append((this[_childs.Length - 1]).CombineValues());
            }
            return sb.ToString();
        }

        public override string ToString() {
            return base.ToString() + (_childs.Length == 0 ? " value: " + _value : " count: " + _childs.Length);
        }

        #endregion
    }
}