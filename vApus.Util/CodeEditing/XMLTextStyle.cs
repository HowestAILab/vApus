/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using FastColoredTextBoxNS;
using System.Drawing;
using System.Text.RegularExpressions;

namespace vApus.Util {
    public class XMLTextStyle {
        //styles
        private readonly TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        public readonly Style GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        private readonly TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        public readonly Style RedStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        private readonly MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));
        private readonly FastColoredTextBox _fastColoredTextBox;

        private Regex _HTMLAttrRegex, _HTMLAttrValRegex, _HTMLCommentRegex1, _HTMLCommentRegex2, _HTMLEndTagRegex, _HTMLTagNameRegex, _HTMLTagRegex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applyTo"></param>
        public XMLTextStyle(FastColoredTextBox applyTo) {
            _fastColoredTextBox = applyTo;

            _fastColoredTextBox.ClearStylesBuffer();

            //add this style explicitly for drawing under other styles
            _fastColoredTextBox.AddStyle(SameWordsStyle);

            InitHTMLRegex();

            _fastColoredTextBox.TextChanged += _fastColoredTextBox_TextChanged;
        }

        private void InitHTMLRegex() {
            _HTMLCommentRegex1 = new Regex(@"(<!--.*?-->)|(<!--.*)", RegexOptions.Singleline | RegexOptions.Compiled);
            _HTMLCommentRegex2 = new Regex(@"(<!--.*?-->)|(.*-->)",
                                           RegexOptions.Singleline | RegexOptions.RightToLeft | RegexOptions.Compiled);
            _HTMLTagRegex = new Regex(@"<|/>|</|>", RegexOptions.Compiled);
            _HTMLTagNameRegex = new Regex(@"<(?<range>[!\w:]+)", RegexOptions.Compiled);
            _HTMLEndTagRegex = new Regex(@"</(?<range>[\w:]+)>", RegexOptions.Compiled);
            _HTMLAttrRegex = new Regex(@"(?<range>\S+?)='[^']*'|(?<range>\S+)=""[^""]*""|(?<range>\S+)=\S+", RegexOptions.Compiled);
            _HTMLAttrValRegex = new Regex(@"\S+?=(?<range>'[^']*')|\S+=(?<range>""[^""]*"")|\S+=(?<range>\S+)", RegexOptions.Compiled);
        }

        /// <summary>
        ///     No html specifics taken into account.
        /// </summary>
        private void _fastColoredTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            Range range = e.ChangedRange;
            range.tb.CommentPrefix = null;
            range.tb.LeftBracket = '<';
            range.tb.RightBracket = '>';
            range.tb.LeftBracket2 = '(';
            range.tb.RightBracket2 = ')';
            //clear style of changed range
            range.ClearStyle(GreenStyle, BlueStyle, MaroonStyle, RedStyle);
            //
            if (_HTMLTagRegex == null)
                InitHTMLRegex();
            //comment highlighting
            range.SetStyle(GreenStyle, _HTMLCommentRegex1);
            range.SetStyle(GreenStyle, _HTMLCommentRegex2);
            //tag brackets highlighting
            range.SetStyle(BlueStyle, _HTMLTagRegex);
            //tag name
            range.SetStyle(MaroonStyle, _HTMLTagNameRegex);
            //end of tag
            range.SetStyle(MaroonStyle, _HTMLEndTagRegex);
            //attributes
            range.SetStyle(RedStyle, _HTMLAttrRegex);
            //attribute values
            range.SetStyle(BlueStyle, _HTMLAttrValRegex);
        }
    }
}