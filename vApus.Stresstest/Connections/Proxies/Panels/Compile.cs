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
    public partial class Compile : UserControl
    {
        public event EventHandler CompileError;
        public event EventHandler<CompileErrorButtonClickedEventArgs> CompileErrorButtonClicked;

        #region Fields
        public CodeBlock Document;
        public ConnectionProxyCode ConnectionProxyCode;
        #endregion

        public Compile()
        {
            InitializeComponent();
        }

        #region Functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deleteTempFiles"></param>
        /// <returns>bull if fails</returns>
        public ConnectionProxyPool TryCompile(bool debug, bool deleteTempFiles)
        {
            this.Cursor = Cursors.WaitCursor;

            Connection connection = new Connection();
            Connection stub = new Connection();
            stub.ConnectionProxy = ConnectionProxyCode.Parent as ConnectionProxy;
            ConnectionProxyPool connectionProxyPool = new ConnectionProxyPool(stub);
            CompilerResults results = connectionProxyPool.CompileConnectionProxyClass(debug, deleteTempFiles);


            flpCompileLog.Controls.Clear();

            int errors = 0;
            int warnings = 0;

            lblCount.Text = string.Empty;


            foreach (CompilerError errorOrWarning in results.Errors)
            {
                bool can = false;

                if (errorOrWarning.IsWarning)
                {
                    if (!errorOrWarning.ErrorText.Contains("_connectionProxySyntaxItem"))
                    {
                        can = true;
                        ++warnings;
                    }
                }
                else
                {
                    can = true;
                    ++errors;
                }

                if (can)
                    AddErrorOrWarningButton(errorOrWarning);
            }

            if (errors != 0 && warnings != 0)
            {
                connectionProxyPool.Dispose();
                connectionProxyPool = null;

                lblCount.Text = errors + " error(s), " + warnings + " warning(s)";
            }
            else if (errors != 0)
            {
                connectionProxyPool.Dispose();
                connectionProxyPool = null;

                lblCount.Text = errors + " error(s)";
            }
            else if (warnings != 0)
            {
                lblCount.Text = warnings + " warning(s)";
            }

            if (errors == 0)
                AddSuccessButton();
            else if (CompileError != null)
                CompileError(this, null);

            this.Cursor = Cursors.Default;

            return connectionProxyPool;
        }

        private void btnTryCompile_Click(object sender, EventArgs e)
        {
            TryCompile(false, true);
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
        private void AddErrorOrWarningButton(CompilerError errorOrWarning)
        {
            Button btn = new Button();
            btn.AutoSize = true;
            btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn.Font = flpCompileLog.Font;
            btn.TextAlign = ContentAlignment.TopLeft;

            btn.FlatStyle = FlatStyle.Flat;

            if (errorOrWarning.IsWarning)
            {
                btn.FlatAppearance.BorderColor = Color.DarkOrange;
                btn.Text = "Warning at line " + errorOrWarning.Line + " column " + errorOrWarning.Column + ":\n" + errorOrWarning.ErrorText;
            }
            else
            {
                btn.FlatAppearance.BorderColor = Color.Red;
                btn.Text = "Error at line " + errorOrWarning.Line + " column " + errorOrWarning.Column + ":\n" + errorOrWarning.ErrorText;
            }
            btn.Tag = errorOrWarning.Line;
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
            public readonly CodeBlock CodeBlock;

            public CompileErrorButtonClickedEventArgs(int lineNumber, CodeBlock codePart)
            {
                LineNumber = lineNumber;
                CodeBlock = codePart;
            }
        }
    }
}
