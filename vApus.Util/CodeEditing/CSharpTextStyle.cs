/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System.Drawing;
using System.Text.RegularExpressions;
using FastColoredTextBoxNS;

namespace vApus.Util
{
    public class CSharpTextStyle
    {
        //styles
        private readonly TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        private readonly TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        private readonly TextStyle BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
        private readonly TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        private readonly TextStyle GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        private readonly TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        private readonly MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));
        private readonly FastColoredTextBox _fastColoredTextBox;
        private TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);

        public CSharpTextStyle(FastColoredTextBox fastColoredTextBox)
        {
            _fastColoredTextBox = fastColoredTextBox;

            _fastColoredTextBox.ClearStylesBuffer();

            //add this style explicitly for drawing under other styles
            _fastColoredTextBox.AddStyle(SameWordsStyle);

            _fastColoredTextBox.TextChanged += _fastColoredTextBox_TextChanged;
        }

        private void _fastColoredTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _fastColoredTextBox.LeftBracket = '(';
            _fastColoredTextBox.RightBracket = ')';
            _fastColoredTextBox.LeftBracket2 = '\x0';
            _fastColoredTextBox.RightBracket2 = '\x0';
            //clear style of changed range
            e.ChangedRange.ClearStyle(BlueStyle, BoldStyle, GrayStyle, MagentaStyle, GreenStyle, BrownStyle);
            //string highlighting
            e.ChangedRange.SetStyle(BrownStyle, @"""""|@""""|''|@"".*?""|[^@](?<range>"".*?[^\\]"")|'.*?[^\\]'");
            //comment highlighting
            e.ChangedRange.SetStyle(GreenStyle, @"//.*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(GreenStyle, @"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline);
            e.ChangedRange.SetStyle(GreenStyle, @"(/\*.*?\*/)|(.*\*/)",
                                    RegexOptions.Singleline | RegexOptions.RightToLeft);
            //number highlighting
            e.ChangedRange.SetStyle(MagentaStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");
            //attribute highlighting
            e.ChangedRange.SetStyle(GrayStyle, @"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline);
            //class name highlighting
            e.ChangedRange.SetStyle(BoldStyle, @"\b(class|struct|enum|interface)\s+(?<range>\w+?)\b");
            //keyword highlighting
            e.ChangedRange.SetStyle(BlueStyle,
                                    @"\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while|add|alias|ascending|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|value|var|where|yield)\b|#region\b|#endregion\b");

            //clear folding markers
            e.ChangedRange.ClearFoldingMarkers();
            //set folding markers
            e.ChangedRange.SetFoldingMarkers("{", "}"); //allow to collapse brackets block
            e.ChangedRange.SetFoldingMarkers(@"#region\b", @"#endregion\b"); //allow to collapse #region blocks
            e.ChangedRange.SetFoldingMarkers(@"/\*", @"\*/"); //allow to collapse comment block

            //Preprocessor stuff
            e.ChangedRange.SetStyle(BlueStyle, @"(#define|#undef|#if|#else|#elif|#endif)\b");
        }
    }
}