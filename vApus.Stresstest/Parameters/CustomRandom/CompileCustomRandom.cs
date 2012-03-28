/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.CodeDom.Compiler;
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Stresstest
{
    public partial class CompileCustomRandom : UserControl
    {
        public event EventHandler<CompileErrorButtonClickedEventArgs> CompileErrorButtonClicked;

        #region Fields
        public CodeBlock Document;
        public CustomRandomParameter Parameter;
        #endregion

        public CompileCustomRandom()
        {
            InitializeComponent();
        }

        #region Functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deleteTempFiles"></param>
        /// <returns>bull if fails</returns>
        private void TryCompile()
        {
            this.Cursor = Cursors.WaitCursor;

            var results = Parameter.CreateInstance();


            flpCompileLog.Controls.Clear();

            int errors = 0;
            int warnings = 0;

            lblCount.Text = string.Empty;


            foreach (CompilerError errorOrWarning in results.Errors)
            {
                if (errorOrWarning.IsWarning)
                    ++warnings;
                else
                    ++errors;

                AddErrorOrWarningButton(errorOrWarning);
            }

            if (errors != 0 && warnings != 0)
            {
                Parameter.ResetValue();

                lblCount.Text = errors + " error(s), " + warnings + " warning(s)";
            }
            else if (errors != 0)
            {
                Parameter.ResetValue();

                lblCount.Text = errors + " error(s)";
            }
            else if (warnings != 0)
            {
                lblCount.Text = warnings + " warning(s)";
            }

            if (errors == 0)
                AddSuccessButton();

            this.Cursor = Cursors.Default;
        }

        private void btnTryCompile_Click(object sender, EventArgs e)
        {
            TryCompile();
        }
        private void AddSuccessButton()
        {
            Button btn = new Button();
            btn.AutoSize = true;
            btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn.Font = new Font(flpCompileLog.Font, FontStyle.Bold);

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.MouseOverBackColor = BackColor;
            btn.FlatAppearance.BorderColor = Color.DarkGreen;

            btn.Text = "Success!";
            flpCompileLog.Controls.Add(btn);

            btn.Width = flpCompileLog.ClientSize.Width - 18;
            int height = btn.Height;
            btn.AutoSize = false;
            btn.Height = height;
        }
        private void AddErrorOrWarningButton(CompilerError error)
        {
            Button btn = new Button();
            btn.AutoSize = true;
            btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn.Font = flpCompileLog.Font;
            btn.TextAlign = ContentAlignment.TopLeft;

            btn.FlatStyle = FlatStyle.Flat;

            int line = error.Line - 6;
            if (error.IsWarning)
            {
                btn.FlatAppearance.BorderColor = Color.DarkOrange;
                btn.Text = "Warning at line " + line + " column " + error.Column + ":\n" + error.ErrorText;
            }
            else
            {
                btn.FlatAppearance.BorderColor = Color.Red;
                btn.Text = "Error at line " + line + " column " + error.Column + ":\n" + error.ErrorText;
            }
            btn.Tag = line;
            btn.Click += new EventHandler(btn_Click);
            flpCompileLog.Controls.Add(btn);

            btn.Width = flpCompileLog.ClientSize.Width - 18;
            int height = btn.Height;
            btn.AutoSize = false;
            btn.Height = height;
        }
        private void btn_Click(object sender, EventArgs e)
        {
            int lineNumber = (int)(sender as Button).Tag;
            CodeBlock codeBlock = Document.ContainsLine(lineNumber);
            while (codeBlock == null)
                codeBlock = Document.ContainsLine(++lineNumber);

            if (CompileErrorButtonClicked != null)
                CompileErrorButtonClicked(this, new CompileErrorButtonClickedEventArgs(lineNumber, codeBlock));
        }
        private void flpCompileLog_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control control in flpCompileLog.Controls)
                control.Width = flpCompileLog.ClientSize.Width - 18;
        }
        #endregion

        public class CompileErrorButtonClickedEventArgs : EventArgs
        {
            public readonly int LineNumber;
            public readonly CodeBlock CodePart;

            public CompileErrorButtonClickedEventArgs(int lineNumber, CodeBlock codePart)
            {
                LineNumber = lineNumber;
                CodePart = codePart;
            }
        }
    }
}
