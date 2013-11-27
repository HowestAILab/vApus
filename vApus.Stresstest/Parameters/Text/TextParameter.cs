/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    /// <summary>
    /// To generate a text parameter, can be pre- or suffixed, have lenght boundaries or generated using a pattern.
    /// </summary>
    [DisplayName("Text Parameter"), Serializable]
    public class TextParameter : BaseParameter, ISerializable {

        #region Fields
        private Fixed _fixed;
        private int _maxLength = 100, _minLength;
        private string _pattern = string.Empty, _prefix = string.Empty, _suffix = string.Empty;
        #endregion

        #region Properties
        [PropertyControl(0), SavableCloneable]
        [DisplayName("Minimum Length"), Description("Only applicable if no pattern is given.")]
        public int MinLength {
            get { return _minLength; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Cannot be smaller then zero.");

                if (value > _maxLength)
                    value = _maxLength;

                _minLength = value;
            }
        }

        [PropertyControl(1), SavableCloneable]
        [DisplayName("Maximum Length"), Description("Only applicable if no pattern is given.")]
        public int MaxLength {
            get { return _maxLength; }
            set {
                if (value < _minLength)
                    value = _minLength;

                _maxLength = value;
            }
        }

        [PropertyControl(2), SavableCloneable]
        [Description("0 = numeric char, obligatory; 9 = optional; A = capital, obligatory; a = non-capital; B = capital, optional; b = non-capital; # = random char, obligatory; ? = optional.")]
        public string Pattern {
            get { return _pattern; }
            set { _pattern = value; }
        }

        [PropertyControl(100), SavableCloneable]
        [Description("Prefix the output value.")]
        public string Prefix {
            get { return _prefix; }
            set { _prefix = value; }
        }

        [PropertyControl(101), SavableCloneable]
        [Description("Suffix the output value.")]
        public string Suffix {
            get { return _suffix; }
            set { _suffix = value; }
        }

        [PropertyControl(102), SavableCloneable]
        [DisplayName("Fixed"),
         Description("If a pre- or suffix is not fixed their length will be adepted to the output value (try generate custom list).")]
        public Fixed _Fixed {
            get { return _fixed; }
            set { _fixed = value; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// To generate a text parameter, can be pre- or suffixed, have lenght boundaries or generated using a pattern.
        /// </summary>
        public TextParameter() { Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged; }
        /// <summary>
        /// To generate a text parameter, can be pre- or suffixed, have lenght boundaries or generated using a pattern.
        /// </summary>
        /// <param name="pattern"></param>
        public TextParameter(string pattern)
            : this() {
            _pattern = pattern;
        }
        public TextParameter(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                Label = sr.ReadString();
                _fixed = (Fixed)sr.ReadInt32();
                _maxLength = sr.ReadInt32();
                _minLength = sr.ReadInt32();
                _pattern = sr.ReadString();
                _prefix = sr.ReadString();
                _suffix = sr.ReadString();
                _tokenNumericIdentifier = sr.ReadInt32();
            }
            sr = null;
        }
        #endregion

        #region Functions
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e) {
            if (Parent != null && Parent is CustomListParameter)
                ShowInGui = false;
        }

        public override void Next() {
            SetValue();
            while (!_chosenValues.Add(Value))
                SetValue();
        }

        private void SetValue() {
            if (_chosenValues.Count == int.MaxValue)
                _chosenValues.Clear();

            Value = (_pattern.Length > 0)
                         ? StringUtil.GenerateRandomPattern(_pattern)
                         : StringUtil.GenerateRandomName(_minLength, _maxLength);
            Value = GetFixedValue();
        }

        public override void ResetValue() {
            Value = string.Empty;
            _chosenValues.Clear();
        }

        /// <summary>
        ///     Value with prefix and suffix if any.
        /// </summary>
        /// <returns></returns>
        private string GetFixedValue() {
            string pre = _prefix, suf = _suffix, value = Value;
            int length;
            if (_fixed == Fixed.Suffix) {
                length = pre.Length - value.Length + 1;
                pre = (length > 0) ? pre.Substring(0, length) : string.Empty;
            } else if (_fixed == Fixed.Prefix) {
                length = suf.Length - value.Length + 1;
                suf = (length > 0) ? suf.Substring(suf.Length - length) : string.Empty;
            }
            return pre + value + suf;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(Label);
                sw.Write((int)_fixed);
                sw.Write(_maxLength);
                sw.Write(_minLength);
                sw.Write(_pattern);
                sw.Write(_prefix);
                sw.Write(_suffix);
                sw.Write(_tokenNumericIdentifier);
                sw.AddToInfo(info);
            }
            sw = null;
        }
        #endregion
    }
}