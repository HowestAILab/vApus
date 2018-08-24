/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.ArgumentsAnalyzer;
using vApus.Gui.Properties;
using vApus.Link;
using vApus.Results;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Gui {
    public partial class Main : Form {

        #region Fields
        private readonly string[] _args;
        private Win32WindowMessageHandler _msgHandler;
        private bool _saveAndCloseOnUpdate = false; // To set the buttons of the messagebox.

        private readonly FirstStepsView _firstStepsView = new FirstStepsView();
        private readonly AboutDialog _aboutDialog = new AboutDialog();
        private LogErrorToolTip _logErrorToolTip;

        private OptionsDialog _optionsDialog;
        private UpdateNotifierPanel _updateNotifierPanel;
        private LogPanel _logPanel;
        private LocalizationPanel _localizationPanel;
        private TestProgressNotifierPanel _progressNotifierPannel;
        private WindowsFirewallAutoUpdatePanel _disableFirewallAutoUpdatePanel;
        private CleanTempDataPanel _cleanTempDataPanel;
        private Publish.PublishPanel _publishPanel;
        #endregion

        #region Constructor
        public Main(string[] args = null) {
            LicenseChecker.LicenseCheckFinished += LicenseChecker_LicenseCheckFinished;
            _args = args;
            Init();
        }
        #endregion

        #region Init
        private void Init() {
            InitializeComponent();
            mainMenu.ImageList = new ImageList { ColorDepth = ColorDepth.Depth24Bit };
            _msgHandler = new Win32WindowMessageHandler();
            if (IsHandleCreated)
                SetGui();
            else
                HandleCreated += MainWindow_HandleCreated;
        }

        private void MainWindow_HandleCreated(object sender, EventArgs e) {
            HandleCreated -= MainWindow_HandleCreated;
            SetGui();
        }

        async private void SetGui() {
            try {
                HandleLicenseCheck();

                SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;
                Solution.RegisterDockPanel(dockPanel);
                Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
                if (Solution.ShowStressTestingSolutionExplorer() && Settings.Default.GreetWithFirstStepsView)
                    _firstStepsView.Show(dockPanel);
                OnActiveSolutionChanged(null);

                string error = Analyzer.AnalyzeAndExecute(_args);
                if (error.Length != 0)
                    Loggers.Log(Level.Error, "Argument Analyzer " + error);

                _updateNotifierPanel = new UpdateNotifierPanel();
                _logPanel = new LogPanel();
                Loggers.GetLogger<FileLogger>().LogEntryWritten += Main_LogEntryWritten;
                _logErrorToolTip = new LogErrorToolTip { AutoPopDelay = 10000 };
                _logErrorToolTip.Click += _logErrorToolTip_Click;

                _localizationPanel = new LocalizationPanel();
                _cleanTempDataPanel = new CleanTempDataPanel();
                _disableFirewallAutoUpdatePanel = new WindowsFirewallAutoUpdatePanel();

                //When this vApus is used for a slave, the title bar will change.
                SocketListenerLinker.NewTest += SocketListenerLinker_NewTest;

                string host, username, privateRSAKeyPath;
                int port, channel;
                bool smartUpdate;
                UpdateNotifier.GetCredentials(out host, out port, out username, out privateRSAKeyPath, out channel, out smartUpdate);

                UpdateNotifier.Refresh();

                if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.NewUpdateFound &&
                    UpdateNotifier.GetUpdateNotifierDialog().ShowDialog() == DialogResult.OK)
                    //Doing stuff automatically
                    if (Update(host, port, username, privateRSAKeyPath, channel))
                        await Task.Run(() => SynchronizationContextWrapper.SynchronizationContext.Send((state) => { Close(); }, null));

                _progressNotifierPannel = new TestProgressNotifierPanel();

                _publishPanel = new Publish.PublishPanel();

                _firstStepsView.LinkClicked += _firstStepsView_LinkClicked;

                SetStatusStrip();
            }
            catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed initializing GUI.", ex);
            }
        }

        private void LicenseChecker_LicenseCheckFinished(object sender, LicenseChecker.LicenseCheckEventArgs e) {
            HandleLicenseCheck();
        }
        private void HandleLicenseCheck() {
            return; //Not used ATM.

            for (int i = 1; ; i++) //Modal form managemant can be a bit buggy in winforms.
                try {
                    if (LicenseChecker.Status == LicenseChecker.__Status.NotLicensed && !_aboutDialog.Visible) {
                        if (!this.Visible) {
                            _aboutDialog.ShowInTaskbar = true;
                            _aboutDialog.StartPosition = FormStartPosition.CenterScreen;
                        }
                        _aboutDialog.ShowDialog();
                        if (LicenseChecker.Status == LicenseChecker.__Status.NotLicensed) Application.Exit();

                        _aboutDialog.ShowInTaskbar = false;
                        _aboutDialog.StartPosition = FormStartPosition.CenterParent;
                    }
                    break;
                }
                catch {
                    if (i == 3) throw;
                }
        }
        #endregion

        #region Misc
        private void SocketListenerLinker_NewTest(object sender, EventArgs e) { SynchronizationContextWrapper.SynchronizationContext.Send(delegate { Text = sender.ToString(); }, null); }

        private void MainWindow_LocationChanged(object sender, EventArgs e) { RelocateLogErrorToolTip(); }

        private void MainWindow_SizeChanged(object sender, EventArgs e) { RelocateLogErrorToolTip(); }

        private void RelocateLogErrorToolTip() {
            try {
                if (_logErrorToolTip != null && _logErrorToolTip.Visible) {
                    int x = statusStrip.Location.X + 9;
                    int y = statusStrip.Location.Y - 30;

                    _logErrorToolTip.Location = new Point(x, y);
                }
            }
            catch {
                //Not important. Ignore.
            }
        }
        #endregion

        #region Menu
        private void SetToolStripMenuItemImage(ToolStripMenuItem item) {
            var component = item.Tag as SolutionComponent;
            string componentTypeName = component.GetType().Name;
            if (!mainMenu.ImageList.Images.Keys.Contains(componentTypeName)) {
                Image image = component.GetImage();
                if (image != null)
                    mainMenu.ImageList.Images.Add(componentTypeName, image);
            }
            item.Image = mainMenu.ImageList.Images[componentTypeName];
        }

        /// <summary>
        ///     Params are for autoconnect (using auto update)
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="privateRSAKeyPath"></param>
        /// <returns>true if a new updater is launched.</returns>
        private bool Update(string host, int port, string username, string privateRSAKeyPath, int channel) {
            bool launchedNewUpdater = false;

            if (host != null) {
                Cursor = Cursors.WaitCursor;
                string path = Path.Combine(Application.StartupPath, "vApus.UpdateToolLoader.exe");
                if (File.Exists(path)) {
                    Enabled = false;
                    var process = new Process();
                    process.EnableRaisingEvents = true;
                    string solution = Solution.ActiveSolution == null ? string.Empty : " \"" + Solution.ActiveSolution.FileName + "\"";
                    string arguments = "{A84E447C-3734-4afd-B383-149A7CC68A32} " + host + " " +
                                                             port + " " + username + " " + privateRSAKeyPath.Replace(' ', '_') + " " + channel +
                                                             " " + false + " " + false + solution;
                    process.StartInfo = new ProcessStartInfo(path, arguments);

                    launchedNewUpdater = process.Start();
                    if (launchedNewUpdater)
                        process.Exited += updateProcess_Exited;
                }
                else {
                    MessageBox.Show("vApus could not be updated because the update tool was not found!", string.Empty,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }

                if (!launchedNewUpdater)
                    Enabled = true;

                Cursor = Cursors.Default;
            }

            return launchedNewUpdater;
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowOptionsDialog();
        }

        /// <summary>
        /// </summary>
        /// <param name="panelIndex">The panel to show.</param>
        private void ShowOptionsDialog(int panelIndex = 0) {
            Cursor = Cursors.WaitCursor;

            if (_optionsDialog == null) {
                _optionsDialog = new OptionsDialog();
                _optionsDialog.FormClosed += _optionsDialog_FormClosed;
                _optionsDialog.AddOptionsPanel(_logPanel);
                _optionsDialog.AddOptionsPanel(_cleanTempDataPanel);
                _optionsDialog.AddOptionsPanel(_localizationPanel);
                _optionsDialog.AddOptionsPanel(_publishPanel);
                SocketListenerLinker.AddSocketListenerManagerPanel(_optionsDialog);
                _optionsDialog.AddOptionsPanel(_progressNotifierPannel);
                _optionsDialog.AddOptionsPanel(_updateNotifierPanel);
                _optionsDialog.AddOptionsPanel(_disableFirewallAutoUpdatePanel);
            }
            _optionsDialog.SelectedPanel = panelIndex;
            if (!_optionsDialog.Visible) {
                _optionsDialog.Hide(); //Strange VB6 bug: Form that is already displayed modally cannot be displayed as a modal dialog box. work-around.
                _optionsDialog.ShowDialog(this);
            }
            SetStatusStrip();
            Cursor = Cursors.Default;
        }

        private void _optionsDialog_FormClosed(object sender, FormClosedEventArgs e) {
            try {
                Settings.Default.LogLevel = (int)Loggers.GetLogger<FileLogger>().CurrentLevel;
                Settings.Default.Save();
            }
            catch {
                //Dont't care.
            }
        }

        private void lupusTitaniumHTTPsProxyToolStripMenuItem_Click(object sender, EventArgs e) {
            string path = Path.Combine(Application.StartupPath, "lupus-titanium", "lupus-titanium_gui.exe");
            if (File.Exists(path)) {
                Process.Start(path);
            }
            else {
                string ex = "Lupus-Titanium was not found!";
                MessageBox.Show(ex, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Loggers.Log(Level.Error, ex, null, new object[] { sender, e });
            }

        }

        private void detailedResultsViewerToolStripMenuItem_Click(object sender, EventArgs e) {
            string path = Path.Combine(Application.StartupPath, "DetailedResultsViewer", "vApus.DetailedResultsViewer.exe");
            if (File.Exists(path)) {
                Process.Start(path);
            }
            else {
                string ex = "Detailed results viewer was not found!";
                MessageBox.Show(ex, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Loggers.Log(Level.Error, ex, null, new object[] { sender, e });
            }
        }
        private void publishItemsHandlerToolStripMenuItem_Click(object sender, EventArgs e) {
            string path = Path.Combine(Application.StartupPath, "PublishItemsHandler", "vApus.PublishItemsHandler.exe");
            if (File.Exists(path)) {
                Process.Start(path);
            }
            else {
                string ex = "Publish items handler was not found!";
                MessageBox.Show(ex, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Loggers.Log(Level.Error, ex, null, new object[] { sender, e });
            }
        }


        private void updateProcess_Exited(object sender, EventArgs e) {
            try {
                if (!IsDisposed)
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                        try {
                            Enabled = true;
                        }
                        catch (Exception ex) {
                            Loggers.Log(Level.Error, "Failed enabeling the GUI.", ex, new object[] { sender, e });
                        }
                        try {
                            _msgHandler.PostMessage();
                        }
                        catch (Exception ex) {
                            Loggers.Log(Level.Error, "Failed notifying update complete.", ex, new object[] { sender, e });
                        }
                    }, null);
            }
            catch (Exception exc) {
                Loggers.Log(Level.Error, "Failed notifying update complete.", exc, new object[] { sender, e });
            }
        }

        protected override void WndProc(ref Message m) {
            try {
                if (_msgHandler != null && m.Msg == _msgHandler.WINDOW_MSG) {
                    TopMost = true;
                    Show();
                    TopMost = false;
                }
                //WM_CLOSE
                if (m.Msg == 16) {
                    // for updater
                    if (m.LParam == (IntPtr)1) {
                        _saveAndCloseOnUpdate = true;

                        TopMost = true;
                        Show();
                        TopMost = false;

                        if (_optionsDialog != null && !_optionsDialog.IsDisposed)
                            _optionsDialog.Close();

                        _firstStepsView.DisableFormClosingEventHandling();
                    }
                    else {
                        _firstStepsView.CancelFormClosing(); //Let the parentform decide.
                    }
                }
            }
            catch {
                //Don't care
            }
            base.WndProc(ref m);
        }

        #region File

        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e) {
            OnActiveSolutionChanged(e);
        }

        //Append the text if needed (show the solution name, show a * if something has changed).
        private void OnActiveSolutionChanged(ActiveSolutionChangedEventArgs e) {
            if (Solution.ActiveSolution == null) {
                Text = "vApus";
            }
            else {
                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                distributedToolStripMenuItem.Visible = true;
                singleTestToolStripMenuItem.Visible = true;

                StringBuilder sb;
                if (e.ToBeSaved) {
                    sb = new StringBuilder("*");
                    sb.Append(Solution.ActiveSolution.Name);
                }
                else {
                    sb = new StringBuilder(Solution.ActiveSolution.Name);
                }

                sb.Append(" - vApus");
                Text = sb.ToString();
            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e) {
            if (Solution.ExplicitCancelFormClosing) {
                e.Cancel = true;
                Solution.ExplicitCancelFormClosing = false;
                return;
            }
            if (Solution.ActiveSolution != null &&
                (!Solution.ActiveSolution.IsSaved || Solution.ActiveSolution.FileName == null)) {
                DialogResult result =
                    MessageBox.Show(string.Format("Do you want to save '{0}' before exiting the application?",
                        Solution.ActiveSolution.Name), string.Empty, _saveAndCloseOnUpdate ? MessageBoxButtons.YesNo : MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes || result == DialogResult.No) {
                    if (result == DialogResult.Yes)
                        Solution.SaveActiveSolution();
                    tmrSetStatusStrip.Stop();

                    _firstStepsView.DisableFormClosingEventHandling();
                    //For the DockablePanels that are shown as dockstate document, otherwise the form won't close.
                    _firstStepsView.Hide();
                    _firstStepsView.Close();
                    SolutionComponentViewManager.DisposeViews();
                    e.Cancel = false;
                }
                else if (result == DialogResult.Cancel) {
                    e.Cancel = true;
                    return;
                }
            }
            else {
                tmrSetStatusStrip.Stop();

                _firstStepsView.DisableFormClosingEventHandling();
                //For the DockablePanels that are shown as dockstate document, otherwise the form won't close.
                _firstStepsView.Hide();
                _firstStepsView.Close();
                SolutionComponentViewManager.DisposeViews();
                e.Cancel = false;
            }
            //Look if there is nowhere a process busy (like stress testing) that can leed to cancelling the closing of the form.
            foreach (Form mdiChild in Solution.RegisteredForCancelFormClosing) {
                if (Solution.ExplicitCancelFormClosing) break;
                mdiChild.Close();
            }
            if (_firstStepsView != null)
                try {
                    _firstStepsView.Close();
                }
                catch { }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            Solution.CreateNew();
            Cursor = Cursors.Default;
        }

        async private void newFromTemplateToolStripMenuItem_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            string text = this.Text;
            this.Text = "Loading solution, please be patient... - vApus";
            if (!(await Solution.CreateNewFromTemplateAsync()))
                this.Text = text;
            Cursor = Cursors.Default;
        }

        async private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            string text = this.Text;
            this.Text = "Loading solution, please be patient... - vApus";
            if (!(await Solution.LoadNewActiveSolutionAsync()))
                this.Text = text;
            Cursor = Cursors.Default;
        }

        async private void reOpenToolStripMenuItem_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            string text = this.Text;
            this.Text = "Loading solution, please be patient... - vApus";
            if (!(await Solution.ReloadSolutionAsync()))
                this.Text = text;
            Cursor = Cursors.Default;
        }

        private void openRecentToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            List<ToolStripMenuItem> recentSolutions = Solution.GetRecentSolutionsMenuItems(this);
            var defaultItems = new ToolStripItem[2];
            for (int i = 0; i < 2; i++)
                defaultItems[i] = openRecentToolStripMenuItem.DropDownItems[i];
            openRecentToolStripMenuItem.DropDownItems.Clear();
            openRecentToolStripMenuItem.DropDownItems.AddRange(defaultItems);
            if (recentSolutions.Count > 0) {
                clearToolStripMenuItem.Enabled = true;
                openRecentToolStripMenuItem.DropDownItems.AddRange(recentSolutions.ToArray());
            }
            else {
                clearToolStripMenuItem.Enabled = false;
                var emptyItem = new ToolStripMenuItem("<empty>");
                emptyItem.Enabled = false;
                openRecentToolStripMenuItem.DropDownItems.Add(emptyItem);
            }
            Cursor = Cursors.Default;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            if (
                MessageBox.Show("Are you sure you want to clear this?", string.Empty, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
                clearToolStripMenuItem.Enabled = false;
                List<ToolStripMenuItem> recentSolutions = Solution.GetRecentSolutionsMenuItems(this);
                if (recentSolutions.Count > 0) {
                    Solution.ClearRecentSolutions();
                    while (openRecentToolStripMenuItem.DropDownItems.Count > 2)
                        openRecentToolStripMenuItem.DropDownItems.RemoveAt(2);
                    var emptyItem = new ToolStripMenuItem("<empty>");
                    emptyItem.Enabled = false;
                    openRecentToolStripMenuItem.DropDownItems.Add(emptyItem);
                }
            }
            Cursor = Cursors.Default;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            Solution.SaveActiveSolution();
            Cursor = Cursors.Default;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            Solution.SaveActiveSolutionAs();
            Cursor = Cursors.Default;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
        }

        #endregion

        #region View
        private void firstStepsToolStripMenuItem_Click(object sender, EventArgs e) {
            _firstStepsView.Show(dockPanel);
            //Show it again the next time.
            Settings.Default.GreetWithFirstStepsView = true;
            Settings.Default.Save();
        }

        private void stressTestingSolutionExplorerToolStripMenuItem_Click(object sender, EventArgs e) {
            Solution.ShowStressTestingSolutionExplorer();
        }
        #endregion

        #region Monitors and stress tests
        private void monitorToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            monitorToolStripMenuItem.DropDownItems.Clear();
            if (Solution.ActiveSolution != null)
                foreach (BaseItem monitor in Solution.ActiveSolution.GetProject("MonitorProject")) {
                    var item = new ToolStripMenuItem(monitor.ToString());
                    item.Tag = monitor;
                    SetToolStripMenuItemImage(item);
                    item.Click += item_Click;
                    monitorToolStripMenuItem.DropDownItems.Add(item);
                }
        }

        private void stressTestToolStripMenuItem_DropDownOpened(object sender, EventArgs e) {
            distributedToolStripMenuItem.DropDownItems.Clear();
            singleTestToolStripMenuItem.DropDownItems.Clear();
            if (Solution.ActiveSolution != null) {
                BaseProject distributedTestProject = Solution.ActiveSolution.GetProject("DistributedTestProject");
                distributedToolStripMenuItem.Tag = distributedTestProject;
                SetToolStripMenuItemImage(distributedToolStripMenuItem);
                foreach (BaseItem distributedTest in distributedTestProject) {
                    var item = new ToolStripMenuItem(distributedTest.ToString());
                    item.Tag = distributedTest;
                    SetToolStripMenuItemImage(item);
                    item.Click += item_Click;
                    distributedToolStripMenuItem.DropDownItems.Add(item);
                }
                BaseProject stressTestProject = Solution.ActiveSolution.GetProject("StressTestProject");
                singleTestToolStripMenuItem.Tag = stressTestProject;
                SetToolStripMenuItemImage(singleTestToolStripMenuItem);
                foreach (BaseItem stressTestCandidate in stressTestProject)
                    if (stressTestCandidate.GetType().Name.ToLowerInvariant() == "stresstest") {
                        var item = new ToolStripMenuItem(stressTestCandidate.ToString());
                        item.Tag = stressTestCandidate;
                        SetToolStripMenuItemImage(item);
                        item.Click += item_Click;
                        singleTestToolStripMenuItem.DropDownItems.Add(item);
                    }
            }
        }

        private void item_Click(object sender, EventArgs e) {
            ((sender as ToolStripMenuItem).Tag as LabeledBaseItem).Activate();
        }
        #endregion

        #region Help
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            _aboutDialog.ShowDialog();
        }
        private void helpToolStripMenuItem1_Click(object sender, EventArgs e) {
            try {
                Process.Start(Path.Combine(Application.StartupPath, "Help\\Help.htm"));
            }
            catch {
                Loggers.Log(Level.Error, "Help file not found.");
            }
        }
        #endregion

        #endregion

        #region Status Strip
        private void Main_LogEntryWritten(object sender, WriteLogEntryEventArgs e) {
            try {
                if (e.Entry.Level > Level.Warning)
                    SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                        try {
                            //Show the error messages in a tooltip.
                            _logErrorToolTip.IncrementNumberOfErrorsOrFatals();

                            if (!_logErrorToolTip.Visible) {
                                int x = statusStrip.Location.X + 9;
                                int y = statusStrip.Location.Y - 30;

                                _logErrorToolTip.Show(this, x, y);
                            }

                        }
                        catch (Exception ex) {
                            Loggers.Log(Level.Error, "Failed displaying the error log tooltip.", ex, new object[] { sender, e });
                        }
                    }, null);

            }
            catch (Exception exc) {
                Loggers.Log(Level.Error, "Failed displaying the error log tooltip.", exc, new object[] { sender, e });
            }
        }

        private void tmrSetStatusStrip_Tick(object sender, EventArgs e) {
            if (IsHandleCreated && Visible) try {
                    reopenToolStripMenuItem.Enabled =
                        (Solution.ActiveSolution != null && Solution.ActiveSolution.FileName != null && !Solution.ActiveSolution.IsSaved);
                    SetStatusStrip();
                }
                catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed setting the status strip.", ex, new object[] { sender, e });
                }
        }

        async private void SetStatusStrip() {
            var attr = typeof(UpdateNotifierState).GetField(UpdateNotifier.UpdateNotifierState.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            lblUpdateNotifier.Text = attr[0].Description;

            if (Publish.Publisher.Settings.PublisherEnabled) {
                _publishPanel.AutoLaunchvApusPublishItemsHandler();

                bool connected = await Task.Run(() => _publishPanel.Connected);

                lblPublisher.Text = connected ? "Publisher connected" : "Could not connect publisher";
            }
            else {
                lblPublisher.Text = "Publisher disabled";
            }


            SetWarningLabel();

            if (_cleanTempDataPanel != null) {
                double tempDataSizeInMB = _cleanTempDataPanel.TempDataSizeInMB;
                lblTempDataSize.Text = tempDataSizeInMB + " MB";

                if (tempDataSizeInMB < 1.0)
                    lblCleanTempData.Visible = lblTempDataSize.Visible = lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = false;
                else
                    lblCleanTempData.Visible = lblTempDataSize.Visible = true;
            }

            _updateNotifierPanel.CurrentSolutionFileName = Solution.ActiveSolution == null ? string.Empty : Solution.ActiveSolution.FileName;
        }

        private void SetWarningLabel() {
            WindowsFirewallAutoUpdatePanel.Status status = _disableFirewallAutoUpdatePanel.CheckStatus();
            switch (status) {
                case WindowsFirewallAutoUpdatePanel.Status.AllDisabled:
                    lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = lblWarning.Visible = false;
                    break;
                case WindowsFirewallAutoUpdatePanel.Status.WindowsFirewallEnabled:
                    lblWarning.Text = "Windows firewall enabled!";
                    lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = lblWarning.Visible = true;
                    break;
                case WindowsFirewallAutoUpdatePanel.Status.WindowsAutoUpdateEnabled:
                    lblWarning.Text = "Windows auto update enabled!";
                    lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = lblWarning.Visible = true;
                    break;
                case WindowsFirewallAutoUpdatePanel.Status.AllEnabled:
                    lblWarning.Text = "Windows firewall and auto update enabled!";
                    lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = lblWarning.Visible = true;
                    break;
            }
            //if (!lblWarning.Visible && !_savingResultsPanel.Connected) {
            //    lblWarning.Text = "Test results cannot be saved!";
            //    lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = lblWarning.Visible = true;
            //}
        }

        private void _firstStepsView_LinkClicked(object sender, FirstStepsView.LinkClickedEventArgs e) { ShowOptionsDialog(e.OptionsIndex); }

        private void lblUpdateNotifier_Click(object sender, EventArgs e) { ShowOptionsDialog(6); }

        private void lblPublisher_Click(object sender, EventArgs e) { ShowOptionsDialog(3); }

        private void _logErrorToolTip_Click(object sender, EventArgs e) { ShowOptionsDialog(0); }

        private void lblCleanTempData_Click(object sender, EventArgs e) { ShowOptionsDialog(1); }

        private void lblWarning_Click(object sender, EventArgs e) { ShowOptionsDialog(7); }
        #endregion

    }
}