﻿/*
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
using vApus.Util;

namespace vApus.Stresstest
{
    public partial class TestCustomRandom : UserControl
    {
        public event EventHandler<CompileErrorButtonClickedEventArgs> CompileErrorButtonClicked;

        #region Fields
        public CodeBlock Document;
        public CustomRandomParameter Parameter;
        #endregion

        public TestCustomRandom()
        {
            InitializeComponent();
        }

        #region Functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deleteTempFiles"></param>
        /// <returns>bull if fails</returns>
        internal void TryCompileAndTestCode(out Exception exception)
        {
            this.Cursor = Cursors.WaitCursor;
            flpCompileLog.Controls.Clear();

            TryCompile(out exception);

            if (exception == null)
                TestCode(out exception);

            this.Cursor = Cursors.Default;
        }
        private void TryCompile(out Exception exception)
        {
            exception = null;
            var results = Parameter.CreateInstance();

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

            Parameter.ResetValue();
            if (errors != 0 && warnings != 0)
                lblCount.Text = errors + " error(s), " + warnings + " warning(s)";
            else if (errors != 0)
                lblCount.Text = errors + " error(s)";
            else if (warnings != 0)
                lblCount.Text = warnings + " warning(s)";

            if (errors != 0)
                exception = new Exception("The custom code does not compile!\nPlease check it for errors.");
        }
        private void TestCode(out Exception exception)
        {
            exception = null;

            string[] values = new string[3];
            int index = 0;
            try
            {
                for (int i = 0; i != values.Length; i++)
                {
                    Parameter.Next();
                    values[i] = Parameter.Value;
                
                    index = i;
                }
                AddSuccessButton(values);
            }
            catch (Exception ex)
            {
                exception = ex;

                for (int i = index; i != values.Length; i++)
                    values[i] = values[index];

                string[] lines = Parameter.BuildCode().Replace("\r\n", "\n").Replace("\n\r", "\n").Split('\r', '\n');

                var error = new CompilerError(string.Empty, lines.Length -1, 6, "-1", exception.Message + "\nGenerated three values: " + values.Combine(", "));
                error.IsWarning = false;
                AddErrorOrWarningButton(error);
            }
        }
        private void btnTryCompile_Click(object sender, EventArgs e)
        {
            Exception exception;
            TryCompileAndTestCode(out exception);
        }
        private void AddSuccessButton(string[] values)
        {
            Button btn = new Button();
            btn.AutoSize = true;
            btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn.Font = new Font(flpCompileLog.Font, FontStyle.Bold);

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.MouseOverBackColor = BackColor;
            btn.FlatAppearance.BorderColor = Color.DarkGreen;

            btn.Text = "Success!\nGenerated three values: " + values.Combine(", ");
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