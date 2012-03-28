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

namespace vApus.Stresstest
{
    [DisplayName("Numeric Parameter"), Serializable]
    public class NumericParameter : BaseParameter
    {
        #region Fields
        private double _doubleValue;

        private int _minValue = int.MinValue, _maxValue = int.MaxValue, _decimalPlaces;
        private double _step = 1;
        private bool _random;


        private string _prefix = string.Empty, _suffix = string.Empty;
        private Fixed _fixed;
        #endregion

        #region Properties
        [PropertyControl(0), SavableCloneable]
        [DisplayName("Minimum Value"), Description("An inclusive minimum value.")]
        public int MinValue
        {
            get { return _minValue; }
            set
            {
                if (value > _maxValue)
                    value = _maxValue;

                _minValue = value;
                if (_doubleValue < _minValue)
                {
                    _doubleValue = _minValue;
                    _value = _minValue.ToString();
                }
            }
        }
        [PropertyControl(1), SavableCloneable]
        [DisplayName("Maximum Value"), Description("An exclusive maximum value.")]
        public int MaxValue
        {
            get { return _maxValue; }
            set
            {
                if (value < _minValue)
                    value = _minValue;

                _maxValue = value;
                if (_doubleValue >= _maxValue)
                {
                    _doubleValue = _maxValue;
                    _value = _maxValue.ToString();
                }
            }
        }
        [PropertyControl(2), SavableCloneable]
        [DisplayName("Decimal Places")]
        public int DecimalPlaces
        {
            get { return _decimalPlaces; }
            set { _decimalPlaces = value; }
        }
        [PropertyControl(3), SavableCloneable]
        [Description("Only applicable if random equals false.")]
        public double Step
        {
            get { return _step; }
            set
            {
                if (value < 0)
                    throw new Exception("The step cannot be smaller than zero.");
                _step = value;
            }
        }
        [PropertyControl(4), SavableCloneable]
        [Description("If false output values will be chosen in sequence using the step.")]
        public bool Random
        {
            get { return _random; }
            set { _random = value; }
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
        [DisplayName("Fixed"), Description("If a pre- or suffix is not fixed their length will be adepted to the output value (try generate custom list).")]
        public Fixed _Fixed
        {
            get { return _fixed; }
            set { _fixed = value; }
        }
        #endregion

        #region Constructors
        public NumericParameter()
        {
            _value = _minValue.ToString();
            _doubleValue = _minValue;

            Solution.ActiveSolutionChanged += new EventHandler<ActiveSolutionChangedEventArgs>(Solution_ActiveSolutionChanged);
        }
        public NumericParameter(int minValue, int maxValue)
            : this()
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _value = minValue.ToString();
            _doubleValue = _minValue;
        }
        #endregion

        #region Functions
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e)
        {
            if (Parent != null && Parent is CustomListParameter)
                this.ShowInGui = false;
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

            if (_random)
            {
                _doubleValue = Math.Round((_maxValue * _r.NextDouble()) - _minValue, _decimalPlaces);
                _value = GetFixedValue();
            }
            else
            {
                _doubleValue = Math.Round(_doubleValue + _step, _decimalPlaces);
                if (_doubleValue >= _maxValue)
                {
                    _doubleValue = _minValue;
                    _chosenValues.Clear();
                }
                _value = GetFixedValue();
            }
        }
        public override void ResetValue()
        {
            _value = _minValue.ToString();
            _doubleValue = _minValue;
            _chosenValues.Clear();
        }
        /// <summary>
        /// Value with prefix and suffix if any.
        /// </summary>
        /// <returns></returns>
        private string GetFixedValue()
        {
            string pre = _prefix, suf = _suffix, value = _doubleValue.ToString();
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
