/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Globalization;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    /// <summary>
    /// Generates numeric values with a pre- or suffix if you like.
    /// </summary>
    [DisplayName("Numeric Parameter"), Serializable]
    public class NumericParameter : BaseParameter {

        #region Fields
        private int _decimalPlaces;
        private string _decimalSeparator = ",";
        private double _doubleValue;
        private Fixed _fixed;
        private int _maxValue = int.MaxValue, _minValue = int.MinValue;

        private string _prefix = string.Empty;
        private bool _random;
        private double _step = 1;
        private string _suffix = string.Empty;
        #endregion

        #region Properties
        [PropertyControl(0), SavableCloneable]
        [DisplayName("Minimum Value"), Description("An inclusive minimum value.")]
        public int MinValue {
            get { return _minValue; }
            set {
                if (value > _maxValue)
                    value = _maxValue;

                _minValue = value;
                if (_doubleValue < _minValue) {
                    _doubleValue = _minValue;
                    Value = _minValue.ToString();
                }
            }
        }

        [PropertyControl(1), SavableCloneable]
        [DisplayName("Maximum Value"), Description("An exclusive maximum value.")]
        public int MaxValue {
            get { return _maxValue; }
            set {
                if (value < _minValue)
                    value = _minValue;

                _maxValue = value;
                if (_doubleValue >= _maxValue) {
                    _doubleValue = _maxValue;
                    Value = _maxValue.ToString();
                }
            }
        }

        [PropertyControl(2), SavableCloneable]
        [DisplayName("Decimal Places"),
         Description(
             "If this value is greater than 15 it will be ignored and no rounding of the output value will occur.")]
        public int DecimalPlaces {
            get { return _decimalPlaces; }
            set {
                if (_decimalPlaces < 0)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than 0.");
                _decimalPlaces = value;
            }
        }

        [PropertyControl(3), SavableCloneable]
        [DisplayName("Decimal Separator"),
         Description("Only . or , allowed.\nThe output of a parameter is a string, so this is important!")]
        public string DecimalSeparator {
            get { return _decimalSeparator; }
            set {
                if (value != "." && value != ",")
                    throw new ArgumentException("Only . or , allowed.");
                _decimalSeparator = value;
            }
        }

        [PropertyControl(4), SavableCloneable]
        [Description("Only applicable if random equals false.")]
        public double Step {
            get { return _step; }
            set {
                if (value < 0)
                    throw new Exception("The step cannot be smaller than zero.");
                _step = value;
            }
        }

        [PropertyControl(5), SavableCloneable]
        [Description("If false output values will be chosen in sequence using the step.")]
        public bool Random {
            get { return _random; }
            set { _random = value; }
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

        #region Constructors
        /// <summary>
        /// Generates numeric values with a pre- or suffix if you like.
        /// </summary>
        public NumericParameter() {
            Value = _minValue.ToString();
            _doubleValue = _minValue;

            _decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
        }
        /// <summary>
        /// Generates numeric values with a pre- or suffix if you like.
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public NumericParameter(int minValue, int maxValue)
            : this() {
            _minValue = minValue;
            _maxValue = maxValue;
            Value = minValue.ToString();
            _doubleValue = _minValue;
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

            if (_random) {
                _doubleValue = ((_maxValue - _minValue) * _r.NextDouble()) + _minValue;
            } else {
                _doubleValue += _step;
                if (_doubleValue >= _maxValue) {
                    _doubleValue = _minValue;
                    _chosenValues.Clear();
                }
            }

            if (_decimalPlaces < 15) //Can only round to max 15 digits
                _doubleValue = Math.Round(_doubleValue, _decimalPlaces);

            Value = GetFixedValue();
        }

        public override void ResetValue() {
            Value = _minValue.ToString();
            _doubleValue = _minValue;
            _chosenValues.Clear();
        }

        /// <summary>
        ///     Value with prefix and suffix if any.
        /// </summary>
        /// <returns></returns>
        private string GetFixedValue() {
            string pre = _prefix, suf = _suffix, value = StringUtil.DoubleToLongString(_doubleValue);
            if (_decimalSeparator != CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                value = value.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, _decimalSeparator);

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
        #endregion
    }
}