/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.CodeDom.Compiler;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace vApus.StressTest {
    public partial class CompilePanel : UserControl {
        public event EventHandler CompileError;
        public event EventHandler<CompileErrorButtonClickedEventArgs> CompileErrorButtonClicked;

        #region Fields

        public ConnectionProxyCode ConnectionProxyCode;

        #endregion

        public CompilePanel() {
            InitializeComponent();
        }

        #region Functions

        /// <summary>
        /// </summary>
        /// <param name="deleteTempFiles"></param>
        /// <returns>null if fails</returns>
        public ConnectionProxyPool TryCompile(bool debug) {
            Cursor = Cursors.WaitCursor;

            var stub = new Connection();
            stub.ConnectionProxy = ConnectionProxyCode.Parent as ConnectionProxy;
            var connectionProxyPool = new ConnectionProxyPool(stub);
            CompilerResults results = connectionProxyPool.CompileConnectionProxyClass(debug);


            flpCompileLog.Controls.Clear();

            int errors = 0;
            int warnings = 0;

            lblCount.Text = string.Empty;


            foreach (CompilerError errorOrWarning in results.Errors) {
                bool can = false;

                if (errorOrWarning.IsWarning) {
                    if (!errorOrWarning.ErrorText.Contains("_connectionProxySyntaxItem")) {
                        can = true;
                        ++warnings;
                    }
                } else {
                    can = true;
                    ++errors;
                }

                if (can)
                    AddErrorOrWarningButton(errorOrWarning);
            }

            if (errors != 0 && warnings != 0) {
                connectionProxyPool.Dispose();
                connectionProxyPool = null;

                lblCount.Text = errors + " error(s), " + warnings + " warning(s)";
            } else if (errors != 0) {
                connectionProxyPool.Dispose();
                connectionProxyPool = null;

                lblCount.Text = errors + " error(s)";
            } else if (warnings != 0) {
                lblCount.Text = warnings + " warning(s)";
            }

            if (errors == 0)
                AddSuccessButton();
            else if (CompileError != null)
                CompileError(this, null);

            Thread.Sleep(100); //If trying to compile to fast it can go wrong.

            Cursor = Cursors.Default;

            return connectionProxyPool;
        }

        private void btnTryCompile_Click(object sender, EventArgs e) {
            TryCompile(true);
        }

        private void AddSuccessButton() {
            var btn = new Button();
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

        private void AddErrorOrWarningButton(CompilerError errorOrWarning) {
            var btn = new Button();
            btn.AutoSize = true;
            btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn.Font = flpCompileLog.Font;
            btn.TextAlign = ContentAlignment.TopLeft;

            btn.FlatStyle = FlatStyle.Flat;

            if (errorOrWarning.IsWarning) {
                btn.FlatAppearance.BorderColor = Color.DarkOrange;
                btn.Text = "Warning at line " + errorOrWarning.Line + " column " + errorOrWarning.Column + ":\n" +
                           errorOrWarning.ErrorText;
            } else {
                btn.FlatAppearance.BorderColor = Color.Red;
                btn.Text = "Error at line " + errorOrWarning.Line + " column " + errorOrWarning.Column + ":\n" +
                           errorOrWarning.ErrorText;
            }
            btn.Tag = errorOrWarning.Line - 1;
            btn.Click += btn_Click;
            flpCompileLog.Controls.Add(btn);

            btn.Width = flpCompileLog.ClientSize.Width - 18;
            int height = btn.Height;
            btn.AutoSize = false;
            btn.Height = height;
        }

        private void btn_Click(object sender, EventArgs e) {
            var lineNumber = (int)(sender as Button).Tag;
            if (CompileErrorButtonClicked != null)
                CompileErrorButtonClicked(this, new CompileErrorButtonClickedEventArgs(lineNumber));
        }

        private void flpCompileLog_SizeChanged(object sender, EventArgs e) {
            foreach (Control control in flpCompileLog.Controls)
                control.Width = flpCompileLog.ClientSize.Width - 18;
        }

        #endregion

        public class CompileErrorButtonClickedEventArgs : EventArgs {
            public readonly int LineNumber;

            public CompileErrorButtonClickedEventArgs(int lineNumber) {
                LineNumber = lineNumber;
            }
        }
    }
}