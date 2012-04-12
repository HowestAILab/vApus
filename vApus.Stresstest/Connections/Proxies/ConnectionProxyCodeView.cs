/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest
{
    public partial class ConnectionProxyCodeView : BaseSolutionComponentView
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        #region Fields
        private CodeBlock _dllreferences;
        private ConnectionProxyCode _connectionProxyCode;
        private Point _autoScrollPosition;
        private delegate void AutoScrollPositionDelegate();
        private bool _autoScrollPositionSet;
        private int _previousSplitterDistance;
        #endregion

        #region Constructor
        /// <summary>
        /// Designer time only constructor
        /// </summary>
        public ConnectionProxyCodeView()
        {
            InitializeComponent();
        }
        public ConnectionProxyCodeView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            InitializeComponent();

            _connectionProxyCode = solutionComponent as ConnectionProxyCode;
            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new System.EventHandler(ConnectionView_HandleCreated);
            this.TextChanged += new EventHandler(ConnectionProxyCodeView_TextChanged);
        }

        private void ConnectionProxyCodeView_TextChanged(object sender, EventArgs e)
        {
            this.TextChanged -= ConnectionProxyCodeView_TextChanged;
            Text = "Connection Proxy Code (" + (_connectionProxyCode.Parent as LabeledBaseItem).Label + ")";
            this.TextChanged += new EventHandler(ConnectionProxyCodeView_TextChanged);
        }
        #endregion

        #region Functions
        private void ConnectionView_HandleCreated(object sender, EventArgs e)
        {
            HandleCreated -= ConnectionView_HandleCreated;
            SetGui();
        }
        private void SetGui()
        {
            CodeBlock comments = new CodeBlock("/*", "*/", true);
            comments.ReadOnly = true;
            document.Add(comments);

            _dllreferences = new CodeBlock(string.Empty, string.Empty, true);
            _dllreferences.ReadOnly = true;
            document.Add(_dllreferences);

            CodeBlock defaultUsings = new CodeBlock("#region Default Usings", "#endregion //Default Usings", true);
            defaultUsings.ReadOnly = true;
            document.Add(defaultUsings);

            CodeBlock customUsings = new CodeBlock("#region Custom Usings", "#endregion //Custom Usings", true);
            document.Add(customUsings);

            CodeBlock defaultFields = new CodeBlock("#region Default Fields", "#endregion //Default Fields", true);
            defaultFields.ReadOnly = true;

            CodeBlock customFields = new CodeBlock("#region Custom Fields", "#endregion //Custom Fields", true);

            CodeBlock isConnectionOpen = new CodeBlock("public bool IsConnectionOpen { get {", "}} //IsConnectionOpen", true);
            CodeBlock isDisposed = new CodeBlock("public bool IsDisposed { get {", "}} //IsDisposed", true);
            isDisposed.ReadOnly = true;
            CodeBlock properties = new CodeBlock("#region Properties", "#endregion //Properties", false, isConnectionOpen, isDisposed);

            CodeBlock constructor = new CodeBlock("public ConnectionProxy() {", "} //ConnectionProxy", true);

            CodeBlock testConnection = new CodeBlock("public void TestConnection(out string error) {", "} //TestConnection", true);
            CodeBlock openConnection = new CodeBlock("public void OpenConnection() {", "} //OpenConnection", true);
            CodeBlock closeConnection = new CodeBlock("public void CloseConnection() {", "} //CloseConnection", true);
            CodeBlock sendAndReceive = new CodeBlock("public void SendAndReceive(StringTree parameterizedLogEntry, out DateTime sentAt, out TimeSpan timeToLastByte, out Exception exception) {", "} //SendAndReceive", true);
            CodeBlock testSendAndReceive = new CodeBlock("public void TestSendAndReceive(StringTree parameterizedLogEntry, out DateTime sentAt, out TimeSpan timeToLastByte, out Exception exception) {", "} //TestSendAndReceive", true);
            CodeBlock dispose = new CodeBlock("public void Dispose() {", "} //Dispose", true);
            CodeBlock functions = new CodeBlock("#region Functions", "#endregion //Functions", false,
                testConnection,
                openConnection,
                closeConnection,
                sendAndReceive,
                testSendAndReceive,
                dispose);

            CodeBlock freeCoding = new CodeBlock("#region Free Coding", "#endregion //Free Coding", true);
            CodeBlock __class = new CodeBlock("public class ConnectionProxy : IConnectionProxy {", "} //ConnectionProxy", false,
                defaultFields,
                customFields,
                properties,
                constructor,
                functions,
                freeCoding);

            CodeBlock nameSpace = new CodeBlock("namespace vApus.Stresstest {", "} //vApus.Stresstest", false, __class);

            document.Add(nameSpace);
            document.Code = (_connectionProxyCode.Parent as ConnectionProxy).BuildConnectionProxyClass();
            document.RefreshLineNumbers();

            isDisposed.Collapsed = true;
            defaultUsings.Collapsed = true;
            defaultFields.Collapsed = true;

            document.CodeTextChangedDelayed += new EventHandler(document_CodeTextChangedDelayed);
            document.CodeLineCountChanged += new EventHandler<CodeBlock.CodeLineCountChangedEventArgs>(document_CodeLineCountChanged);
            document.CaretPositionChangedUsingKeyboard += new EventHandler<CodeBlock.CaretPositionChangedEventArgs>(document_CaretPositionChangedUsingKeyboard);

            string body = _dllreferences.Code.Trim();
            List<string> filenames = new List<string>();
            filenames.AddRange(body.Split(':')[1].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            references.Filenames = filenames;

            find.Document = document;
            compile.Document = document;
            compile.ConnectionProxyCode = _connectionProxyCode;
            execute.Compile = compile;
            execute.ConnectionProxyCode = _connectionProxyCode;
        }

        private void document_SizeChanged(object sender, EventArgs e)
        {
            _autoScrollPositionSet = false;
            BeginInvoke(new AutoScrollPositionDelegate(SetAutoScrollPosition));
        }
        private void SetAutoScrollPosition()
        {
            scrollablePanel.AutoScrollPosition = _autoScrollPosition;
            _autoScrollPositionSet = true;
        }
        private void document_CodeTextChangedDelayed(object sender, EventArgs e)
        {
            _connectionProxyCode.Code = document.Code;
            _connectionProxyCode.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        private void document_CodeLineCountChanged(object sender, CodeBlock.CodeLineCountChangedEventArgs e)
        {
            document.RefreshLineNumbers();
            //Can only set one at a time (begin invoke!)
            if (_autoScrollPositionSet)
                _autoScrollPosition = new Point(-1 * scrollablePanel.AutoScrollPosition.X, -1 * scrollablePanel.AutoScrollPosition.Y);
        }
        private void document_CaretPositionChangedUsingKeyboard(object sender, CodeBlock.CaretPositionChangedEventArgs e)
        {
            //Can only set one at a time (begin invoke!)
            if (_autoScrollPositionSet)
                ScrollToCaret(e.CaretPosition);
        }
        private void ScrollToCaret(Point caretPosition)
        {
            Point location = PointToScreen(scrollablePanel.Location);
            if (caretPosition.Y < location.Y + 50)
            {
                _autoScrollPosition = new Point(-1 * scrollablePanel.AutoScrollPosition.X,
                    -1 * scrollablePanel.AutoScrollPosition.Y - (location.Y - caretPosition.Y) - 50);
                _autoScrollPositionSet = false;
                BeginInvoke(new AutoScrollPositionDelegate(SetAutoScrollPosition));
            }
            else if (caretPosition.Y > location.Y + scrollablePanel.Height - 120)
            {
                _autoScrollPosition = new Point(-1 * scrollablePanel.AutoScrollPosition.X,
                    -1 * scrollablePanel.AutoScrollPosition.Y + (caretPosition.Y - (location.Y + scrollablePanel.Height)) + 120);
                _autoScrollPositionSet = false;
                BeginInvoke(new AutoScrollPositionDelegate(SetAutoScrollPosition));
            }
        }

        private void scrollablePanel_Scroll(object sender, ScrollEventArgs e)
        {
            if (_autoScrollPositionSet)
                _autoScrollPosition = new Point(-1 * scrollablePanel.AutoScrollPosition.X, -1 * scrollablePanel.AutoScrollPosition.Y);

        }
        #region Tools
        private void references_ReferencesChanged(object sender, EventArgs e)
        {
            string code = _dllreferences.Header + "\n// dllreferences:";
            foreach (string filename in references.ShortFilenames)
                code += filename + ';';

            _dllreferences.Code = code;
            _connectionProxyCode.Code = document.Code;
        }
        private void find_FoundButtonClicked(object sender, FindAndReplace.FoundReplacedButtonClickedEventArgs e)
        {
            document.ClearSelection();
            e.CodeBlock.SelectLine(e.LineNumber);
            ScrollToCaret(CaretPosition.Get());
        }
        private void compile_CompileError(object sender, EventArgs e)
        {
            tcTools.SelectedIndex = 2;
        }
        private void compile_CompileErrorButtonClicked(object sender, Compile.CompileErrorButtonClickedEventArgs e)
        {
            document.ClearSelection();
            e.CodeBlock.SelectLine(e.LineNumber);
            ScrollToCaret(CaretPosition.Get());
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    sw.Write(document.Code.Replace("#region", "\n#region"));
                if (MessageBox.Show("Do you want to open the file?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    try
                    {
                        Process.Start(sfd.FileName);
                    }
                    catch
                    {
                        MessageBox.Show("Could not open the file!\nNo Application is associated with the 'cs' extension.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
            }
        }
        private void btnCollapseExpand_Click(object sender, EventArgs e)
        {
            if (btnCollapseExpand.Text == "-")
            {
                btnCollapseExpand.Text = "+";
                _previousSplitterDistance = splitCode.SplitterDistance;
                splitCode.SplitterDistance = splitCode.Height - 23;
                splitCode.IsSplitterFixed = true;

                tcTools.Hide();
            }
            else
            {
                btnCollapseExpand.Text = "-";
                splitCode.SplitterDistance = _previousSplitterDistance;
                splitCode.IsSplitterFixed = false;

                tcTools.Show();
            }
        }
        private void ConnectionProxyCodeView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)6)
                tcTools.SelectedIndex = 1;
        }
        private void btnFoldUnfold_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            LockWindowUpdate(this.Handle.ToInt32());

            bool fold = btnFoldUnfold.Text.StartsWith("Fold");
            foreach (CodeBlock codeBlock in document.GetCodeBlocks())
                FoldUnFold(codeBlock, fold);

            btnFoldUnfold.Text = (fold ? "Unfold" : "Fold") + " Code Text Blocks";

            LockWindowUpdate(0);
            this.Cursor = Cursors.Default;
        }
        private void FoldUnFold(CodeBlock codeBlock, bool fold)
        {
            CodeBlock[] codeBlocks = codeBlock.GetCodeBlocks();
            if (codeBlocks.Length == 0)
                codeBlock.Collapsed = fold;
            else foreach (CodeBlock cb in codeBlocks)
                    FoldUnFold(cb, fold);
        }
        #endregion

        #endregion

    }
}