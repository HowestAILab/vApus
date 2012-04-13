/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Stresstest
{
    public partial class CodeBlock : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        public event EventHandler CodeTextChanged;
        /// <summary>
        /// TextChangedDelayed event. It occurs after insert, delete, clear, undo and redo operations. This event occurs with a delay relative to TextChanged, and fires only once.
        /// </summary>
        public event EventHandler CodeTextChangedDelayed;
        /// <summary>
        /// sender is this or a child code block.
        /// Use this event to decide when to set the line numbers.
        /// </summary>
        public event EventHandler<CodeLineCountChangedEventArgs> CodeLineCountChanged;
        /// <summary>
        /// sender is this or a child code block.
        /// </summary>
        public event EventHandler<CaretPositionChangedEventArgs> CaretPositionChangedUsingKeyboard;

        #region Fields
        private Graphics _g;
        private CodeBlockTextBox _codeBlockTextBox;
        private int _lineNumberOffset = 1;

        private bool _parentLevelControl;
        #endregion

        #region Properties
        [Description("Set to true for the code block containing all other codeblocks.")]
        public bool ParentLevelControl
        {
            get { return _parentLevelControl; }
            set { _parentLevelControl = value; }
        }
        /// <summary>
        /// get: The resampled code
        /// set: Divides the input  over the different child code blocks if any based on the footer text.
        /// If a line of text equals a footer it will be used as a delimiter.
        /// 
        /// If the header is not provided like "header\n" in the beginning of the string this will return doing nothing.
        /// 
        /// '\n' is the line delimiter.
        /// </summary>
        [ReadOnly(true)]
        public string Code
        {
            get
            {
                string code = lblHeader.Text + '\n';

                if (_codeBlockTextBox == null)
                    foreach (CodeBlock codeBlock in splitContainer.Panel2.Controls)
                        code += codeBlock.Code + '\n';
                else
                    code += _codeBlockTextBox.Text + '\n';

                code += lblFooter.Text;
                return code;
            }
            set
            {
                value = value.Replace("\r\n", "\n").Replace('\r', '\n');
                List<string> lines = new List<string>(value.Split('\n'));
                SetCode(ref lines);
            }
        }

        public bool HeaderVisible
        {
            get { return !splitContainer.Panel1Collapsed; }
            set { splitContainer.Panel1Collapsed = !value; }
        }
        public string Header
        {
            get { return lblHeader.Text; }
            set { lblHeader.Text = value == null ? string.Empty : value.Trim(); }
        }

        [ReadOnly(true)]
        public string Body
        {
            get
            {
                if (_codeBlockTextBox == null)
                    return string.Empty;
                return _codeBlockTextBox.Text;
            }
        }

        public bool FooterVisible
        {
            get { return statusStrip.Visible; }
            set { statusStrip.Visible = value; }
        }
        public string Footer
        {
            get { return lblFooter.Text; }
            set { lblFooter.Text = value == null ? string.Empty : value.Trim(); }
        }

        public bool ReadOnly
        {
            get
            {
                if (_codeBlockTextBox == null)
                    return false;
                return _codeBlockTextBox.ReadOnly;
            }
            set
            {
                if (_codeBlockTextBox == null || _codeBlockTextBox.ReadOnly == value)
                    return;
                _codeBlockTextBox.ReadOnly = value;
                if (_codeBlockTextBox.ReadOnly)
                {
                    lblHeader.ForeColor = Color.DarkSlateGray;
                    lblFooter.ForeColor = Color.DarkSlateGray;

                    _codeBlockTextBox.EnterTextBox -= _codeBlockTextBox_EnterTextBox;
                    _codeBlockTextBox.LeaveTextBox -= _codeBlockTextBox_LeaveTextBox;
                }
                else
                {
                    lblHeader.ForeColor = Color.Blue;
                    lblFooter.ForeColor = Color.Blue;

                    _codeBlockTextBox.EnterTextBox += _codeBlockTextBox_EnterTextBox;
                    _codeBlockTextBox.LeaveTextBox += _codeBlockTextBox_LeaveTextBox;
                }
            }
        }

        public bool CanCollapse
        {
            get { return btnCollapseExpand.Visible; }
            set { btnCollapseExpand.Visible = value; }
        }
        public bool Collapsed
        {
            get { return splitContainer.Panel2Collapsed; }
            set
            {
                foreach (Control control in splitContainer.Panel2.Controls)
                    control.SizeChanged -= codeBlock_SizeChanged;

                if (value && btnCollapseExpand.Text == "-")
                {
                    btnCollapseExpand.Text = "+";
                    this.Tag = this.Height;
                    this.Height = splitContainer.Panel1.Height + statusStrip.Height;
                }
                else if (!value && btnCollapseExpand.Text == "+")
                {
                    btnCollapseExpand.Text = "-";
                    if (this.Tag != null)
                        this.Height = (int)this.Tag;
                }
                splitContainer.Panel2Collapsed = value;
                foreach (Control control in splitContainer.Panel2.Controls)
                    control.SizeChanged += codeBlock_SizeChanged;
            }
        }

        public bool ShowLineNumbers
        {
            get
            {
                if (_codeBlockTextBox != null)
                    return _codeBlockTextBox.ShowLineNumbers;

                bool showLineNumbers = true;
                foreach (CodeBlock codeBlock in splitContainer.Panel2.Controls)
                    if (!codeBlock.ShowLineNumbers)
                    {
                        showLineNumbers = false;
                        break;
                    }

                return showLineNumbers;
            }
            set
            {
                if (_codeBlockTextBox == null)
                    foreach (CodeBlock codeBlock in splitContainer.Panel2.Controls)
                        codeBlock.ShowLineNumbers = value;
                else
                    _codeBlockTextBox.ShowLineNumbers = value;
            }
        }
        /// <summary>
        /// For setting the line numbers
        /// </summary>

        public int LineNumberOffset
        {
            get
            {
                if (_codeBlockTextBox == null)
                    return _lineNumberOffset;
                return _codeBlockTextBox.LineNumberOffset;
            }
            set
            {
                if (_lineNumberOffset != value)
                {
                    _lineNumberOffset = value;
                    if (_codeBlockTextBox != null)
                        _codeBlockTextBox.SetLineNumbers(value);
                }
            }
        }
        [Description("The header + the footer + the line count of the code text box if any.")]
        public int CodeLineCount
        {
            get
            {
                if (_codeBlockTextBox == null)
                    return 2;
                return _codeBlockTextBox.CodeLineCount + 2;
            }
        }
        [Description("The count of child code blocks.")]
        public int CodeBlockCount
        {
            get
            {
                if (_codeBlockTextBox != null)
                    return 0;
                return splitContainer.Panel2.Controls.Count;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Design time constructor.
        /// </summary>
        public CodeBlock()
            : this(false)
        { }
        public CodeBlock(bool showLineNumbers)
        {
            InitializeComponent();
            InitBodyForText(showLineNumbers);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <param name="footer"></param>
        /// <param name="codeBlocks">If none, a code rich textbox is added.</param>
        public CodeBlock(string header, string footer, bool showLineNumbers, params CodeBlock[] codeBlocks)
            : this(showLineNumbers)
        {
            Header = header;
            Footer = footer;

            if (codeBlocks.Length != 0)
                AddRange(codeBlocks);
        }
        #endregion

        #region Functions

        #region Code Block Text Box
        public void InitBodyForText(bool showLineNumbers)
        {
            bool readOnly = false;
            if (_codeBlockTextBox != null)
                readOnly = _codeBlockTextBox.ReadOnly;

            _g = this.CreateGraphics();
            _codeBlockTextBox = new CodeBlockTextBox();
            _codeBlockTextBox.ReadOnly = readOnly;
            _codeBlockTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            _codeBlockTextBox.Left = 4;
            _codeBlockTextBox.Width = splitContainer.Panel2.Width - _codeBlockTextBox.Left;
            splitContainer.Panel2.Controls.Clear();
            splitContainer.Panel2.Controls.Add(_codeBlockTextBox);
            SetSize();
            _codeBlockTextBox.TextChanged += new EventHandler(_codeBlockTextBox_TextChanged);
            _codeBlockTextBox.TextChangedDelayed += new EventHandler(_codeBlockTextBox_TextChangedDelayed);
            _codeBlockTextBox.LineCountChanged += new EventHandler(_codeBlockTextBox_LineCountChanged);
            _codeBlockTextBox.CaretPositionChangedUsingKeyboard += new EventHandler(_codeBlockTextBox_CaretPositionChangedUsingKeyboard);

            if (!_codeBlockTextBox.ReadOnly)
            {
                _codeBlockTextBox.EnterTextBox += new EventHandler(_codeBlockTextBox_EnterTextBox);
                _codeBlockTextBox.LeaveTextBox += new EventHandler(_codeBlockTextBox_LeaveTextBox);
            }
            _codeBlockTextBox.ShowLineNumbers = showLineNumbers;
        }
        private void RemoveCodeBlockTextBox()
        {
            if (_codeBlockTextBox != null)
            {
                splitContainer.Panel2.Controls.Remove(_codeBlockTextBox);
                _codeBlockTextBox = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines">by ref explicitely because stuff need to happen after the recursion.</param>
        private void SetCode(ref List<string> lines)
        {
            //Remove the header, use this as a check
            lines[0] = lines[0].Trim();
            if (lines[0] == lblHeader.Text)
                lines.RemoveAt(0);
            else
                return;

            if (_codeBlockTextBox == null)
            {
                foreach (CodeBlock codeBlock in splitContainer.Panel2.Controls)
                    codeBlock.SetCode(ref lines);
                //Remove the footer
                if (lines.Count > 0)
                    lines.RemoveAt(0);
            }
            else
            {
                List<string> newLines = new List<string>(lines.Count);
                string body = string.Empty;
                bool gotBody = false;
                foreach (string line in lines)
                    if (gotBody)
                        newLines.Add(line.Trim());
                    else if (line == lblFooter.Text)
                        gotBody = true;
                    else
                        body += line.Trim() + '\n';

                _codeBlockTextBox.Text = body.Trim();
                lines = newLines;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineNumberOffset">You normally don't need this.</param>
        public void RefreshLineNumbers(int lineNumberOffset = -1)
        {
            ResetLineNumbers(lineNumberOffset);
            SetLineNumbers();
        }
        private void ResetLineNumbers(int lineNumberOffset = -1)
        {
            if (lineNumberOffset == -1)
                LineNumberOffset = (_parentLevelControl) ? 2 : 1;
            else
                LineNumberOffset = lineNumberOffset;

            if (_codeBlockTextBox == null)
            {
                foreach (Control control in splitContainer.Panel2.Controls)
                    (control as CodeBlock).ResetLineNumbers();
            }
        }
        private void SetLineNumbers()
        {
            if (_codeBlockTextBox == null)
            {
                CodeBlock previousCodeBlock = null;
                foreach (Control control in splitContainer.Panel2.Controls)
                {
                    CodeBlock codeBlock = control as CodeBlock;
                    if (previousCodeBlock != null)
                        codeBlock.LineNumberOffset = previousCodeBlock.LineNumberOffset + previousCodeBlock.Code.Split('\n').Length;
                    else
                        codeBlock.LineNumberOffset += LineNumberOffset;
                    previousCodeBlock = codeBlock;
                    codeBlock.SetLineNumbers();
                }
            }
            else
            {
                _codeBlockTextBox.SetLineNumbers(LineNumberOffset);
            }
        }

        /// <summary>
        /// Returns the code block that contains the given line number if any.
        /// </summary>
        /// <param name="absoluteLineNumber"></param>
        /// <returns></returns>
        public CodeBlock ContainsLine(int absoluteLineNumber)
        {
            if (_codeBlockTextBox == null)
                foreach (CodeBlock codeBlock in splitContainer.Panel2.Controls)
                {
                    CodeBlock childCodeBlock = codeBlock.ContainsLine(absoluteLineNumber);
                    if (childCodeBlock != null)
                        return childCodeBlock;
                }
            else if (_codeBlockTextBox.ContainsLine(absoluteLineNumber))
                return this;
            return null;
        }
        public void SelectLine(int absoluteLineNumber)
        {
            Collapsed = false;
            if (_codeBlockTextBox == null)
                foreach (CodeBlock codeBlock in splitContainer.Panel2.Controls)
                    codeBlock.SelectLine(absoluteLineNumber);
            else
                _codeBlockTextBox.SelectLine(absoluteLineNumber);
        }
        public void ClearSelection()
        {
            if (_codeBlockTextBox == null)
                foreach (CodeBlock codeBlock in splitContainer.Panel2.Controls)
                    codeBlock.ClearSelection();
            else
                _codeBlockTextBox.ClearSelection();
        }

        /// <summary>
        /// Returns the lines of text where the given string was found and the absolute line numbers.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="wholeWords"></param>
        /// <param name="matchCase"></param>
        /// <returns></returns>
        public Dictionary<int, string> Find(string text, bool wholeWords = false, bool matchCase = false)
        {
            var found = new Dictionary<int, string>();
            if (_codeBlockTextBox == null)
                foreach (CodeBlock codeBlock in splitContainer.Panel2.Controls)
                {
                    var dict = codeBlock.Find(text, wholeWords, matchCase);
                    foreach (int absoluteLineNumber in dict.Keys)
                        found.Add(absoluteLineNumber, dict[absoluteLineNumber]);
                }
            else
            {
                var dict = _codeBlockTextBox.Find(text, wholeWords, matchCase);
                foreach (int absoluteLineNumber in dict.Keys)
                    found.Add(absoluteLineNumber, dict[absoluteLineNumber]);
            }
            if (found.Count > 0)
                Collapsed = false;
            return found;
        }
        /// <summary>
        /// Returns the replaced lines (formatted) and the absolute line numbers.
        /// The found lines are also returned.
        /// </summary>
        /// <param name="oldText"></param>
        /// <param name="newText"></param>
        /// <param name="atLine">-1 for all (absolute line number)</param>
        /// <param name="wholeWords"></param>
        /// <param name="matchCase"></param>
        /// <returns></returns>
        public Dictionary<int, string> Replace(string oldText, string newText, int atLine, bool wholeWords = false, bool matchCase = false)
        {
            var replacedOrFound = new Dictionary<int, string>();
            if (_codeBlockTextBox == null)
                foreach (CodeBlock codeBlock in splitContainer.Panel2.Controls)
                {
                    var dict = codeBlock.Replace(oldText, newText, atLine, wholeWords, matchCase);
                    foreach (int absoluteLineNumber in dict.Keys)
                        replacedOrFound.Add(absoluteLineNumber, dict[absoluteLineNumber]);
                }
            else
            {
                var dict = _codeBlockTextBox.Replace(oldText, newText, atLine, wholeWords, matchCase);
                foreach (int absoluteLineNumber in dict.Keys)
                    replacedOrFound.Add(absoluteLineNumber, dict[absoluteLineNumber]);
            }
            if (replacedOrFound.Count > 0)
                Collapsed = false;
            return replacedOrFound;
        }

        #region Events
        private void _codeBlockTextBox_TextChanged(object sender, EventArgs e)
        {
            if (CodeTextChanged != null)
                CodeTextChanged(this, e);
        }
        private void _codeBlockTextBox_TextChangedDelayed(object sender, EventArgs e)
        {
            if (CodeTextChangedDelayed != null)
                CodeTextChangedDelayed(this, e);
        }
        private void _codeBlockTextBox_CaretPositionChangedUsingKeyboard(object sender, EventArgs e)
        {
            if (CaretPositionChangedUsingKeyboard != null)
                CaretPositionChangedUsingKeyboard(this, new CaretPositionChangedEventArgs(CaretPosition.Get()));
        }
        private void _codeBlockTextBox_LineCountChanged(object sender, EventArgs e)
        {
            if (CodeLineCountChanged != null)
                CodeLineCountChanged(this, new CodeLineCountChangedEventArgs(this));
            SetSize();
        }

        private void _codeBlockTextBox_EnterTextBox(object sender, EventArgs e)
        {
            splitContainer.Panel1.BackColor = Color.LightBlue;
            statusStrip.BackColor = Color.LightBlue;
        }
        private void _codeBlockTextBox_LeaveTextBox(object sender, EventArgs e)
        {
            splitContainer.Panel1.BackColor = Color.WhiteSmoke;
            statusStrip.BackColor = Color.WhiteSmoke;
        }

        private void SetSize()
        {
            LockWindowUpdate(this.Handle.ToInt32());

            Size size = CalculateRrtxtSize();
            if (size.Height != _codeBlockTextBox.Height)
            {
                _codeBlockTextBox.Height = size.Height;
                splitContainer.Height = splitContainer.Panel1.Height + _codeBlockTextBox.Height;
                this.Height = splitContainer.Height + statusStrip.Height;
            }
            //if (size.Width != _codeBlockTextBox.Width)
            //{
            //    _codeBlockTextBox.Width = size.Width;
            //    splitContainer.Width = _codeBlockTextBox.Width;
            //    this.Width = splitContainer.Width;
            //}

            LockWindowUpdate(0);
        }
        private Size CalculateRrtxtSize()
        {
            Size size = new Size();
            //12 == the height of the vertical scrollbar
            SizeF minimumSizeF = _g.MeasureString("A", Font);
            if (_codeBlockTextBox.Text.Length == 0 | !this.IsHandleCreated)
            {
                size.Height = (int)(minimumSizeF.Height * 2) + 12;
                size.Width = (int)minimumSizeF.Width;
            }
            else
            {
                SizeF sizeF = _g.MeasureString(_codeBlockTextBox.Text, Font);

                size.Height = (int)sizeF.Height + (int)minimumSizeF.Height * 2 + 12;
                size.Width = (int)sizeF.Width;
            }
            return size;
        }
        #endregion

        #endregion

        #region Code Blocks
        /// <summary>
        /// Will set the parent. Will remove the code block text box if any.
        /// </summary>
        /// <param name="codeBlock"></param>
        public void Add(CodeBlock codeBlock)
        {
            RemoveCodeBlockTextBox();

            codeBlock.Parent = this;

            codeBlock.Left = 18;
            codeBlock.Width = splitContainer.Panel2.Width - codeBlock.Left;
            codeBlock.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            codeBlock.CodeTextChanged += new EventHandler(codeBlock_CodeTextChanged);
            codeBlock.CodeTextChangedDelayed += new EventHandler(codeBlock_CodeTextChangedDelayed);
            codeBlock.CodeLineCountChanged += new EventHandler<CodeLineCountChangedEventArgs>(codeBlock_CodeLineCountChanged);
            codeBlock.CaretPositionChangedUsingKeyboard += new EventHandler<CaretPositionChangedEventArgs>(codeBlock_CaretPositionChangedUsingKeyboard);
            codeBlock.SizeChanged += new EventHandler(codeBlock_SizeChanged);

            if (splitContainer.Panel2.Controls.Count > 0)
            {
                CodeBlock previousCodeBlock = splitContainer.Panel2.Controls[splitContainer.Panel2.Controls.Count - 1] as CodeBlock;
                codeBlock.Top = previousCodeBlock.Bottom + previousCodeBlock.Margin.Bottom + codeBlock.Margin.Top;
            }
            else if (codeBlock.Top == 0)
            {
                codeBlock.Top = splitContainer.Panel2.Padding.Top;
            }
            splitContainer.Panel2.Controls.Add(codeBlock);
            splitContainer.Height = codeBlock.Bottom + codeBlock.Margin.Bottom;
            this.Height = splitContainer.Height + splitContainer.Panel2.Padding.Bottom + statusStrip.Height;
        }
        /// <summary>
        /// Will set the parent. Will remove the code block text box if any.
        /// </summary>
        /// <param name="codeBlock"></param>
        public void AddRange(params CodeBlock[] codeBlock)
        {
            foreach (CodeBlock codeBock in codeBlock)
                Add(codeBock);
        }

        public int IndexOf(CodeBlock codeBlock)
        {
            return splitContainer.Panel2.Controls.IndexOf(codeBlock);
        }
        public CodeBlock GetAt(int index)
        {
            return splitContainer.Panel2.Controls[index] as CodeBlock;
        }
        /// <summary>
        /// Get code blocks if any
        /// </summary>
        /// <returns></returns>
        public CodeBlock[] GetCodeBlocks()
        {
            var codeBlocks = new List<CodeBlock>();
            if (_codeBlockTextBox == null)
                foreach (CodeBlock codeBlock in splitContainer.Panel2.Controls)
                    codeBlocks.Add(codeBlock);

            return codeBlocks.ToArray();
        }

        #region Events
        private void codeBlock_CaretPositionChangedUsingKeyboard(object sender, CaretPositionChangedEventArgs e)
        {
            if (CaretPositionChangedUsingKeyboard != null)
                CaretPositionChangedUsingKeyboard(sender, e);
        }
        private void codeBlock_CodeLineCountChanged(object sender, CodeLineCountChangedEventArgs e)
        {
            if (CodeLineCountChanged != null)
                CodeLineCountChanged(sender, e);
        }
        private void codeBlock_CodeTextChanged(object sender, EventArgs e)
        {
            if (CodeTextChanged != null)
                CodeTextChanged.Invoke(sender, e);
        }
        private void codeBlock_CodeTextChangedDelayed(object sender, EventArgs e)
        {
            if (CodeTextChangedDelayed != null)
                CodeTextChangedDelayed.Invoke(sender, e);
        }
        private void codeBlock_SizeChanged(object sender, EventArgs e)
        {
            this.SuspendLayout();
            int combinedHeight = 0;
            // int greatestWidth = 0;
            int index = splitContainer.Panel2.Controls.IndexOf(sender as Control);
            Control previousControl = null;
            for (int i = index; i < splitContainer.Panel2.Controls.Count; i++)
            {
                Control control = splitContainer.Panel2.Controls[i];
                if (previousControl != null)
                    control.Top = previousControl.Bottom + previousControl.Margin.Bottom + control.Margin.Top;
                else if (control.Top == 0)
                    control.Top = splitContainer.Panel2.Padding.Top;
                combinedHeight = control.Bottom + control.Margin.Bottom;

                //if (control.Width > greatestWidth)
                //    greatestWidth = control.Width;

                previousControl = control;
            }
            splitContainer.Height = splitContainer.Panel1.Height + combinedHeight;
            this.Height = splitContainer.Height + splitContainer.Panel2.Padding.Bottom + statusStrip.Height;

            //this.Width = greatestWidth + 18;
            this.ResumeLayout();
        }
        #endregion

        #endregion

        private void CodeBlock_SizeChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Strip the header and the footer. 
        /// </summary>
        /// <param name="alsoChilds"></param>
        public void Strip(bool alsoChilds = false)
        {
            HeaderVisible = false;
            FooterVisible = false;

            if (alsoChilds && _codeBlockTextBox == null)
                foreach (CodeBlock codeBlock in GetCodeBlocks())
                    codeBlock.Strip(alsoChilds);
        }
        private void btnCollapseExpand_Click(object sender, EventArgs e)
        {
            Collapsed = btnCollapseExpand.Text == "-";
        }

        #endregion

        public class CodeLineCountChangedEventArgs : EventArgs
        {
            public readonly CodeBlock CodeBlock;
            public CodeLineCountChangedEventArgs(CodeBlock codeBlock)
            {
                CodeBlock = codeBlock;
            }
        }
        public class CaretPositionChangedEventArgs : EventArgs
        {
            public readonly Point CaretPosition;
            public CaretPositionChangedEventArgs(Point caretPosition)
            {
                CaretPosition = caretPosition;
            }
        }
    }
}
