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

namespace vApus.StressTest {
    /// <summary>
    /// Used for visualizing parameters tokens in a FastColoredTextBox. 
    /// </summary>
    public class ParameterTokenTextStyle {

        #region Fields
        private readonly MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));

        private readonly TextStyle _customListParameterStyle = new TextStyle(Brushes.Black, Brushes.LightPink, FontStyle.Bold);
        private readonly IEnumerable<string> _customListParameters;

        private readonly TextStyle _customRandomParameterStyle = new TextStyle(Brushes.Black, Brushes.Yellow, FontStyle.Bold);
        private readonly IEnumerable<string> _customRandomParameters;

        private readonly TextStyle _delimiterStyle = new TextStyle(Brushes.Green, Brushes.GhostWhite, FontStyle.Italic | FontStyle.Underline);
        private readonly TextStyle _vblrStyle = new TextStyle(Brushes.Gray, null, FontStyle.Italic);

        private readonly IEnumerable<string> _delimiters;
        private readonly string[] _vblr = { "<16 0C 02 12n>", "<16 0C 02 12r>" };
        private readonly FastColoredTextBox _fastColoredTextBox;

        private readonly TextStyle _numericParameterStyle = new TextStyle(Brushes.Black, Brushes.LightGreen, FontStyle.Bold);

        private readonly IEnumerable<string> _numericParameters;

        private readonly TextStyle _textParameterStyle = new TextStyle(Brushes.Black, Brushes.LightBlue, FontStyle.Bold);
        private readonly IEnumerable<string> _textParameters;

        private readonly TextStyle _whiteSpaceStyle = new TextStyle(Brushes.Black, new SolidBrush(Color.FromArgb(255, 240, 240, 240)), FontStyle.Regular);
        private bool _visualizeWhiteSpace;
        #endregion

        #region Properties
        /// <summary>
        /// White space can be visualized on the fly by setting this.
        /// </summary>
        public bool VisualizeWhiteSpace {
            get { return _visualizeWhiteSpace; }
            set {
                if (_visualizeWhiteSpace != value) {
                    _visualizeWhiteSpace = value;
                    SetStyle(_fastColoredTextBox.Range);
                }
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Used for visualizing parameters tokens in a FastColoredTextBox. 
        /// </summary>
        /// <param name="fastColoredTextBox"></param>
        /// <param name="delimiters"></param>
        /// <param name="customListParameters"></param>
        /// <param name="numericParameters"></param>
        /// <param name="textParameters"></param>
        /// <param name="customRandomParameters"></param>
        /// <param name="visualizeWhiteSpace"></param>
        public ParameterTokenTextStyle(FastColoredTextBox fastColoredTextBox, IEnumerable<string> delimiters, IEnumerable<string> customListParameters, IEnumerable<string> numericParameters, IEnumerable<string> textParameters, IEnumerable<string> customRandomParameters, bool visualizeWhiteSpace) {
            if (delimiters == null || fastColoredTextBox == null || customListParameters == null ||
                numericParameters == null || textParameters == null || customRandomParameters == null)
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

            SetStyle(_fastColoredTextBox.Range);

            _fastColoredTextBox.TextChanged += _fastColoredTextBox_TextChanged;
        }
        #endregion

        #region Functions
        private void _fastColoredTextBox_TextChanged(object sender, TextChangedEventArgs e) { SetStyle(e.ChangedRange); }
        private void SetStyle(Range range) {
            _fastColoredTextBox.LeftBracket = '\x0';
            _fastColoredTextBox.RightBracket = '\x0';
            _fastColoredTextBox.LeftBracket2 = '\x0';
            _fastColoredTextBox.RightBracket2 = '\x0';

            //clear style of changed range
            range.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);

            string regex = ExtractRegex(_delimiters);
            if (regex != null) range.SetStyle(_delimiterStyle, regex);

            regex = ExtractRegex(_vblr);
            if (regex != null) range.SetStyle(_vblrStyle, regex);

            regex = ExtractRegex(_customListParameters);
            if (regex != null) range.SetStyle(_customListParameterStyle, regex);

            regex = ExtractRegex(_numericParameters);
            if (regex != null) range.SetStyle(_numericParameterStyle, regex);

            regex = ExtractRegex(_textParameters);
            if (regex != null) range.SetStyle(_textParameterStyle, regex);

            regex = ExtractRegex(_customRandomParameters);
            if (regex != null) range.SetStyle(_customRandomParameterStyle, regex);

            if (_visualizeWhiteSpace) range.SetStyle(_whiteSpaceStyle, @"\s");
        }
        /// <summary>
        /// Extract a regex from a string to find in the text (range.SetStyle)
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        private string ExtractRegex(IEnumerable<string> col) {
            var sb = new StringBuilder();
            foreach (string item in col) {
                sb.Append(Regex.Escape(item));
                sb.Append("|");
            }

            string s = sb.ToString();
            return (s.Length == 0) ? null : s.Substring(0, s.Length - 1);
        }

        public void Dispose() {
            _fastColoredTextBox.TextChanged -= _fastColoredTextBox_TextChanged;
        }
        #endregion
    }
}