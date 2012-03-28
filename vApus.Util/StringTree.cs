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

namespace vApus.Util
{
    public class StringTree : ICollection<StringTree>
    {
        #region Fields
        public bool _isReadOnly = false;
        private string _value, _childDelimiter;

        private StringTree[] _childs = new StringTree[] { };
        #endregion

        #region Properties
        public string Value
        {
            get { return _value; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Value");
                _value = value;
            }
        }
        public string ChildDelimiter
        {
            get { return _childDelimiter; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("ChildDelimiter");
                _childDelimiter = value;
            }
        }
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { _isReadOnly = value; }
        }
        public StringTree this[int index]
        {
            get { return _childs[index]; }
            set { _childs[index] = value; }

        }
        public int Count
        {
            get { return _childs.Length; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// A simple structure for representing a tree of strings (Value).
        /// New string trees can be added as nodes of this one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="childDelimiter">Use combine values to return a string representation of the elements in this structure.</param>
        public StringTree(string value = "", string childDelimiter = "")
        {
            Value = value;
            ChildDelimiter = childDelimiter;
        }
        #endregion

        #region Functions
        public void Add(StringTree item)
        {
            if (_isReadOnly)
                throw new Exception("Is read only");

            List<StringTree> childs = new List<StringTree>(_childs);
            childs.Add(item);
            _childs = childs.ToArray();
        }
        public void Clear()
        {
            if (_isReadOnly)
                throw new Exception("Is read only");

            _childs = new StringTree[] { };
        }
        public bool Contains(StringTree item)
        {
            return _childs.Contains(item);
        }
        public void CopyTo(StringTree[] array, int arrayIndex)
        {
            _childs.CopyTo(array, arrayIndex);
        }
        public bool Remove(StringTree item)
        {
            if (_isReadOnly)
                throw new Exception("Is read only");

            bool removed = false;

            var childs = new List<StringTree>();
            for (int i = 0; i < _childs.Length; i++)
            {
                if (_childs[i] == item)
                    removed = true;
                else
                    childs.Add(_childs[i]);
            }

            if (removed)
                _childs = childs.ToArray();

            return removed;
        }

        public string CombineValues()
        {
            lock (this)
            {
                StringBuilder sb = new StringBuilder(_value);
                if (Count != 0)
                {
                    if (_childDelimiter.Length == 0)
                    {
                        sb.Append(this[0].CombineValues());
                    }
                    else
                    {
                        for (int i = 0; i < Count - 1; i++)
                        {
                            sb.Append(this[i].CombineValues());
                            sb.Append(_childDelimiter);
                        }
                        sb.Append((this[Count - 1]).CombineValues());
                    }
                }
                return sb.ToString();
            }
        }
        public IEnumerator<StringTree> GetEnumerator()
        {
            return _childs.Cast<StringTree>().GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _childs.GetEnumerator();
        }
        public override string ToString()
        {
            return base.ToString() + (Count == 0 ?  " value: " + Value : " count: " + Count);
        }
        #endregion
    }
}
