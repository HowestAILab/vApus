/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System.Drawing;
using FastColoredTextBoxNS;

namespace vApus.LogFixer
{
    public class VisualizeWhiteSpaceTextStyle
    {
        private readonly MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));
        private readonly FastColoredTextBox _fastColoredTextBox;

        private readonly TextStyle _whiteSpaceStyle = new TextStyle(Brushes.Black, Brushes.DarkGray, FontStyle.Regular);
        private bool _visualizeWhiteSpace;

        public VisualizeWhiteSpaceTextStyle(FastColoredTextBox fastColoredTextBox,
                                            bool visualizeWhiteSpace)
        {
            _fastColoredTextBox = fastColoredTextBox;
            _visualizeWhiteSpace = visualizeWhiteSpace;

            _fastColoredTextBox.ClearStylesBuffer();

            //add this style explicitly for drawing under other styles
            _fastColoredTextBox.AddStyle(SameWordsStyle);

            _fastColoredTextBox.TextChanged += _fastColoredTextBox_TextChanged;
        }

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
            if (_visualizeWhiteSpace)
                changedRange.SetStyle(_whiteSpaceStyle, @"\s");
        }
    }
}