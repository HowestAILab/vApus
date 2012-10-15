/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Drawing;
using FastColoredTextBoxNS;

namespace vApus.LogFixer
{
    public class VisualizeWhiteSpaceTextStyle
    {
        private FastColoredTextBox _fastColoredTextBox;

        private bool _visualizeWhiteSpace;

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

        public VisualizeWhiteSpaceTextStyle(FastColoredTextBox fastColoredTextBox,
            bool visualizeWhiteSpace)
        {
            _fastColoredTextBox = fastColoredTextBox;
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
            if (_visualizeWhiteSpace)
                changedRange.SetStyle(_whiteSpaceStyle, @"\s");
        }
    }
}
