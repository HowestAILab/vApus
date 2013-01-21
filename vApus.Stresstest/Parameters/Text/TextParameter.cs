/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest
{
    [DisplayName("Text Parameter"), Serializable]
    public class TextParameter : BaseParameter
    {
        #region Fields

        private Fixed _fixed;
        private int _maxLength = 100;
        private int _minLength;
        private string _pattern = string.Empty;

        private string _prefix = string.Empty, _suffix = string.Empty;

        #endregion

        #region Properties

        [PropertyControl(0), SavableCloneable]
        [DisplayName("Minimum Length"), Description("Only applicable if no pattern is given.")]
        public int MinLength
        {
            get { return _minLength; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Cannot be smaller then zero.");

                if (value > _maxLength)
                    value = _maxLength;

                _minLength = value;
            }
        }

        [PropertyControl(1), SavableCloneable]
        [DisplayName("Maximum Length"), Description("Only applicable if no pattern is given.")]
        public int MaxLength
        {
            get { return _maxLength; }
            set
            {
                if (value < _minLength)
                    value = _minLength;

                _maxLength = value;
            }
        }

        [PropertyControl(2), SavableCloneable]
        [Description(
            "0 = numeric char, obligatory; 9 = optional; A = capital, obligatory; a = non-capital; B = capital, optional; b = non-capital; # = random char, obligatory; ? = optional."
            )]
        public string Pattern
        {
            get { return _pattern; }
            set { _pattern = value; }
        }

        [PropertyControl(100), SavableCloneable]
        [Description("Prefix the output value.")]
        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        [PropertyControl(101), SavableCloneable]
        [Description("Suffix the output value.")]
        public string Suffix
        {
            get { return _suffix; }
            set { _suffix = value; }
        }

        [PropertyControl(102), SavableCloneable]
        [DisplayName("Fixed"),
         Description(
             "If a pre- or suffix is not fixed their length will be adepted to the output value (try generate custom list)."
             )]
        public Fixed _Fixed
        {
            get { return _fixed; }
            set { _fixed = value; }
        }

        #endregion

        #region Constructor

        public TextParameter()
        {
            Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
        }

        public TextParameter(string pattern)
            : this()
        {
            _pattern = pattern;
        }

        #endregion

        #region Functions

        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e)
        {
            if (Parent != null && Parent is CustomListParameter)
                ShowInGui = false;
        }

        public override void Next()
        {
            SetValue();
            while (!_chosenValues.Add(_value))
                SetValue();
        }

        private void SetValue()
        {
            if (_chosenValues.Count == int.MaxValue)
                _chosenValues.Clear();

            _value = (_pattern.Length > 0)
                         ? StringUtil.GenerateRandomPattern(_pattern)
                         : StringUtil.GenerateRandomName(_minLength, _maxLength);
            _value = GetFixedValue();
        }

        public override void ResetValue()
        {
            _value = string.Empty;
            _chosenValues.Clear();
        }

        /// <summary>
        ///     Value with prefix and suffix if any.
        /// </summary>
        /// <returns></returns>
        private string GetFixedValue()
        {
            string pre = _prefix, suf = _suffix, value = Value;
            int length;
            if (_fixed == Fixed.Suffix)
            {
                length = pre.Length - value.Length + 1;
                pre = (length > 0) ? pre.Substring(0, length) : string.Empty;
            }
            else if (_fixed == Fixed.Prefix)
            {
                length = suf.Length - value.Length + 1;
                suf = (length > 0) ? suf.Substring(suf.Length - length) : string.Empty;
            }
            return pre + value + suf;
        }

        #endregion
    }
}