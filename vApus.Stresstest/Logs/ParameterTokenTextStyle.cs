/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using FastColoredTextBoxNS;

namespace vApus.Stresstest
{
    public class ParameterTokenTextStyle
    {
        private FastColoredTextBox _fastColoredTextBox;
        private IEnumerable<string> _delimiters, _customListParameters, _numericParameters, _textParameters, _customRandomParameters;

        private bool _visualizeWhiteSpace;

        //styles
        private TextStyle _delimiterStyle = new TextStyle(Brushes.Black, null, FontStyle.Bold);
        private TextStyle _customListParameterStyle = new TextStyle(Brushes.Black, Brushes.LightPink, FontStyle.Bold);
        private TextStyle _numericParameterStyle = new TextStyle(Brushes.Black, Brushes.LightGreen, FontStyle.Bold);
        private TextStyle _textParameterStyle = new TextStyle(Brushes.Black, Brushes.LightBlue, FontStyle.Bold);
        private TextStyle _customRandomParameterStyle = new TextStyle(Brushes.Black, Brushes.Yellow, FontStyle.Bold);
        private TextStyle _whiteSpaceStyle = new TextStyle(Brushes.Black, Brushes.DarkGray, FontStyle.Regular);

        private MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));


        public bool VisualizeWhiteSpace
        {
            get { return _visualizeWhiteSpace; }
            set
            {
                if (_visualizeWhiteSpace != value)
                {
                    _visualizeWhiteSpace = value;
                    SetStyle(_fastColoredTextBox.Range);
                }
            }
        }

        public ParameterTokenTextStyle(FastColoredTextBox fastColoredTextBox,
            IEnumerable<string> delimiters,
            IEnumerable<string> customListParameters, 
            IEnumerable<string> numericParameters, 
            IEnumerable<string> textParameters, 
            IEnumerable<string> customRandomParameters,
            bool visualizeWhiteSpace)
        {
            if (delimiters == null || fastColoredTextBox == null || customListParameters == null || numericParameters == null || textParameters == null || customRandomParameters == null)
                throw new ArgumentNullException();

            _delimiters = delimiters;
            _fastColoredTextBox = fastColoredTextBox;
            _customListParameters = customListParameters;
            _numericParameters = numericParameters;
            _textParameters = textParameters;
            _customRandomParameters = customRandomParameters;

            _visualizeWhiteSpace = visualizeWhiteSpace;

            _fastColoredTextBox.ClearStylesBuffer();

            //add this style explicitly for drawing under other styles
            _fastColoredTextBox.AddStyle(SameWordsStyle);

            _fastColoredTextBox.TextChanged += new EventHandler<TextChangedEventArgs>(_fastColoredTextBox_TextChanged);
        }

        private void _fastColoredTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetStyle(e.ChangedRange);
        }
        private void SetStyle(Range changedRange)
        {
            _fastColoredTextBox.LeftBracket = '\x0';
            _fastColoredTextBox.RightBracket = '\x0';
            _fastColoredTextBox.LeftBracket2 = '\x0';
            _fastColoredTextBox.RightBracket2 = '\x0';

            //clear style of changed range
            changedRange.ClearStyle(_customListParameterStyle, _numericParameterStyle, _textParameterStyle, _customRandomParameterStyle, _whiteSpaceStyle);
            string regex = ExtractRegex(_delimiters);
            if (regex != null)
                changedRange.SetStyle(_delimiterStyle, regex);

            regex = ExtractRegex(_customListParameters);
            if (regex != null)
                changedRange.SetStyle(_customListParameterStyle, regex);

            regex = ExtractRegex(_numericParameters);
            if (regex != null)
                changedRange.SetStyle(_numericParameterStyle, regex);

            regex = ExtractRegex(_textParameters);
            if (regex != null)
                changedRange.SetStyle(_textParameterStyle, regex);

            regex = ExtractRegex(_customRandomParameters);
            if (regex != null)
                changedRange.SetStyle(_customRandomParameterStyle, regex);

            if (_visualizeWhiteSpace)
                changedRange.SetStyle(_whiteSpaceStyle, @"\s");
        }
        private string ExtractRegex(IEnumerable<string> col)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string item in col)
            {
                sb.Append(Regex.Escape(item));
                sb.Append("|");
            }

            string s = sb.ToString();
            if (s.Length == 0)
                return null;
            return s.Substring(0, s.Length - 1);
        }
    }
}
