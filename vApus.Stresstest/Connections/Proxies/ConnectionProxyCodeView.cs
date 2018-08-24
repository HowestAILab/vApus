/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using FastColoredTextBoxNS;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    public partial class ConnectionProxyCodeView : BaseSolutionComponentView {

        #region Fields

        private readonly ConnectionProxyCode _connectionProxyCode;

        //private bool _codeInitialized;
        private string _prevCode; //Track if changed.
        private CSharpTextStyle _csharpTextStyle;
        private int _previousSplitterDistance;

        #endregion

        #region Constructor

        /// <summary>
        ///     Designer time only constructor
        /// </summary>
        public ConnectionProxyCodeView() {
            InitializeComponent();
        }

        public ConnectionProxyCodeView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            InitializeComponent();

            _connectionProxyCode = solutionComponent as ConnectionProxyCode;
            if (IsHandleCreated)
                SetGui();
            else
                HandleCreated += ConnectionView_HandleCreated;
            TextChanged += ConnectionProxyCodeView_TextChanged;

            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }

        private void ConnectionProxyCodeView_TextChanged(object sender, EventArgs e) {
            TextChanged -= ConnectionProxyCodeView_TextChanged;
            Text = "Connection Proxy Code (" + (_connectionProxyCode.Parent as LabeledBaseItem).Label + ")";
            TextChanged += ConnectionProxyCodeView_TextChanged;
        }

        #endregion

        #region Functions

        private void ConnectionView_HandleCreated(object sender, EventArgs e) {
            HandleCreated -= ConnectionView_HandleCreated;
            SetGui();
        }

        private void SetGui() {
            _csharpTextStyle = new CSharpTextStyle(codeTextBox);
            codeTextBox.Text = (_connectionProxyCode.Parent as ConnectionProxy).BuildConnectionProxyClass();
            _prevCode = codeTextBox.Text;
            codeTextBox.ClearUndo();

            referencesPanel.CodeTextBox = codeTextBox;
            findAndReplacePanel.CodeTextBox = codeTextBox;
            compilePanel.ConnectionProxyCode = _connectionProxyCode;
            codeTextBox.DelayedTextChangedInterval = 1000;
            codeTextBox.TextChanged += codeTextBox_TextChanged;

            _previousSplitterDistance = splitCode.SplitterDistance;

            codeTextBox.DisableFindAndReplace();
            codeTextBox.DisableGotoLine();

            codeTextBox.Leave += codeTextBox_Leave;
            FormClosing += ConnectionProxyCodeView_FormClosing;
            Leave += ConnectionProxyCodeView_Leave;
            Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
        }

        private void codeTextBox_Leave(object sender, EventArgs e) { SetCodeChanged(); }
        private void ConnectionProxyCodeView_Leave(object sender, EventArgs e) { SetCodeChanged(); }
        private void ConnectionProxyCodeView_FormClosing(object sender, FormClosingEventArgs e) {
            if (ParentForm != null)
                try {
                    codeTextBox.Leave -= codeTextBox_Leave;
                    FormClosing -= ConnectionProxyCodeView_FormClosing;
                    Leave -= ConnectionProxyCodeView_Leave;
                    Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;

                    SetCodeChanged();
                }
                catch {
                    //Ignore. Can happen on mdi parent close.
                }
        }

        private void codeTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            _connectionProxyCode.Code = codeTextBox.Text;
        }

        private void SetCodeChanged() {
            try {
                if (_connectionProxyCode.Code != _prevCode) {
                    _connectionProxyCode.Code = _prevCode = codeTextBox.Text;
                    _connectionProxyCode.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                }
            }
            catch { }
        }
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e) {
            if (Solution.ActiveSolution.IsSaved) _prevCode = _connectionProxyCode.Code;
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender == _connectionProxyCode && e.__DoneAction == SolutionComponentChangedEventArgs.DoneAction.Edited) {
                if (!this.IsActivated) this.Activate();
                codeTextBox.Focus();
            }
        }

        #region Tools
        private void find_FoundButtonClicked(object sender, FindAndReplacePanel.FoundReplacedButtonClickedEventArgs e) {
            codeTextBox.ClearSelection();
            codeTextBox.SelectLine(e.LineNumber);
        }

        private void compile_CompileError(object sender, EventArgs e) {
            tcTools.SelectedIndex = 2;
        }

        private void compile_CompileErrorButtonClicked(object sender, CompilePanel.CompileErrorButtonClickedEventArgs e) {
            codeTextBox.ClearSelection();
            codeTextBox.SelectLine(e.LineNumber);
        }

        private void btnExport_Click(object sender, EventArgs e) {
            if (sfd.ShowDialog() == DialogResult.OK) {
                using (var sw = new StreamWriter(sfd.FileName))
                    sw.Write(codeTextBox.Text);
#pragma warning disable 0168
                if (
                    MessageBox.Show("Do you want to open the file?", string.Empty, MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                    try {
                        Process.Start(sfd.FileName);
                    }
                    catch (FileNotFoundException fnfe) {
                        MessageBox.Show("Could not open the file!\nThe file could not be found.", string.Empty,
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch {
                        MessageBox.Show(
                            "Could not open the file!\nNo Application is associated with the 'cs' extension.",
                            string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
            }
        }

        private void btnCollapseExpand_Click(object sender, EventArgs e) {
            if (btnCollapseExpand.Text == "-") Collapse();
            else Uncollapse();
        }
        private void Collapse() {
            if (btnCollapseExpand.Text == "-") {
                btnCollapseExpand.Text = "+";
                _previousSplitterDistance = splitCode.SplitterDistance;
                splitCode.SplitterDistance = splitCode.Height - 23;
                splitCode.IsSplitterFixed = true;

                tcTools.Hide();
            }
        }
        private void Uncollapse() {
            if (btnCollapseExpand.Text == "+") {
                btnCollapseExpand.Text = "-";
                splitCode.SplitterDistance = _previousSplitterDistance;
                splitCode.IsSplitterFixed = false;

                tcTools.Show();
            }
        }

        private void ConnectionProxyCodeView_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)2) { //ctrl+b
                Uncollapse();
                tcTools.SelectedIndex = 2;

                compilePanel.TryCompile(true);
            }
            else if (e.KeyChar == (char)6) { //ctrl+f
                Uncollapse();
                tcTools.SelectedIndex = 1;

                if (codeTextBox.SelectedText.Length != 0 && !codeTextBox.SelectedText.ContainsChars('\r', '\n'))
                    findAndReplacePanel.Find(codeTextBox.SelectedText);
            }
            else if (e.KeyChar == (char)7) { //ctrl+g
                txtGoToLine.Select();
            }
        }

        private bool _handledTxtKeyDown = false;
        private void txtGoToLine_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Back ||
                e.KeyCode == Keys.Delete ||
                e.KeyCode == Keys.Escape ||
                e.KeyCode == Keys.Enter) {
                _handledTxtKeyDown = false;
            }
            else {
                _handledTxtKeyDown = true;
            }
        }

        private void txtGoToLine_KeyPress(object sender, KeyPressEventArgs e) {
            e.Handled = _handledTxtKeyDown && !char.IsDigit(e.KeyChar);
        }
        private void txtGoToLine_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && codeTextBox != null && txtGoToLine.Text.IsNumeric()) {
                int line = int.Parse(txtGoToLine.Text) - 1;
                if (line < 0) line = 0;
                if (line >= codeTextBox.LinesCount) line = codeTextBox.LinesCount - 1;

                txtGoToLine.Text = string.Empty;

                codeTextBox.ClearSelection();
                codeTextBox.SelectLine(line);
            }
        }
        #endregion

        #endregion
    }
}