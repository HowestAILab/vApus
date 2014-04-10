using RandomUtils;
using RandomUtils.Log;
/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
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
using vApus.Gui.Properties;
using vApus.Link;
using vApus.Results;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Gui {
    public partial class MainWindow : Form {

        #region Fields
        private readonly string[] _args;
        private Win32WindowMessageHandler _msgHandler;
        private bool _saveAndCloseOnUpdate = false; // To set the buttons of the messagebox.

        private readonly WelcomeView _welcomeView = new WelcomeView();
        private readonly AboutDialog _aboutDialog = new AboutDialog();
        private LogErrorToolTip _logErrorToolTip;

        private OptionsDialog _optionsDialog;
        private UpdateNotifierPanel _updateNotifierPanel;
        private LogPanel _logPanel;
        private LocalizationPanel _localizationPanel;
        //private ProcessorAffinityPanel _processorAffinityPanel;
        private TestProgressNotifierPanel _progressNotifierPannel;
        private SavingResultsPanel _savingResultsPanel;
        private WindowsFirewallAutoUpdatePanel _disableFirewallAutoUpdatePanel;
        private CleanTempDataPanel _cleanTempDataPanel;
        #endregion

        #region Constructor
        public MainWindow(string[] args = null) {
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
                SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;
                Solution.RegisterDockPanel(dockPanel);
                Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
                if (Solution.ShowStresstestingSolutionExplorer() && Settings.Default.GreetWithWelcomePage)
                    _welcomeView.Show(dockPanel);
                OnActiveSolutionChanged(null);

                string error = ArgumentsAnalyzer.AnalyzeAndExecute(_args);
                if (error.Length != 0)
                    Loggers.Log(Level.Error, "Argument Analyzer " + error);

                _updateNotifierPanel = new UpdateNotifierPanel();
                _logPanel = new LogPanel();
                _logPanel.LogErrorCountChanged += _logPanel_LogErrorCountChanged;
                _logErrorToolTip = new LogErrorToolTip { AutoPopDelay = 10000 };
                _logErrorToolTip.Click += lblLogLevel_Click;

                _localizationPanel = new LocalizationPanel();
                //_processorAffinityPanel = new ProcessorAffinityPanel();
                _cleanTempDataPanel = new CleanTempDataPanel();
                _disableFirewallAutoUpdatePanel = new WindowsFirewallAutoUpdatePanel();

                //When this vApus is used for a slave, the title bar will change.
                SocketListenerLinker.NewTest += SocketListenerLinker_NewTest;

                string host, username, password;
                int port, channel;
                bool smartUpdate;
                UpdateNotifier.GetCredentials(out host, out port, out username, out password, out channel, out smartUpdate);

                UpdateNotifier.Refresh();

                if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.NewUpdateFound &&
                    UpdateNotifier.GetUpdateNotifierDialog().ShowDialog() == DialogResult.OK)
                    //Doing stuff automatically
                    if (Update(host, port, username, password, channel))
                        await Task.Run(() => SynchronizationContextWrapper.SynchronizationContext.Send((state) => { Close(); }, null));

                _progressNotifierPannel = new TestProgressNotifierPanel();
                _savingResultsPanel = new SavingResultsPanel();
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed initializing GUI." , ex);
            }
        }
        #endregion

        #region Misc
        private void SocketListenerLinker_NewTest(object sender, EventArgs e) { SynchronizationContextWrapper.SynchronizationContext.Send(delegate { Text = sender.ToString(); }, null); }

        private void MainWindow_LocationChanged(object sender, EventArgs e) { RelocateLogErrorToolTip(); }

        private void MainWindow_SizeChanged(object sender, EventArgs e) { RelocateLogErrorToolTip(); }

        private void RelocateLogErrorToolTip() {
            try {
                if (_logErrorToolTip.Visible) {
                    int x = statusStrip.Location.X + lblLogLevel.Bounds.X;
                    int y = statusStrip.Location.Y - 30;

                    _logErrorToolTip.Location = new Point(x, y);
                }
            } catch {
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
        /// <param name="password"></param>
        /// <returns>true if a new updater is launched.</returns>
        private bool Update(string host, int port, string username, string password, int channel) {
            bool launchedNewUpdater = false;

            if (host != null) {
                Cursor = Cursors.WaitCursor;
                string path = Path.Combine(Application.StartupPath, "vApus.UpdateToolLoader.exe");
                if (File.Exists(path)) {
                    Enabled = false;
                    var process = new Process();
                    process.EnableRaisingEvents = true;
                    process.StartInfo = new ProcessStartInfo(path,
                                                             "{A84E447C-3734-4afd-B383-149A7CC68A32} " + host + " " +
                                                             port + " " + username + " " + password + " " + channel +
                                                             " " + false + " " + false);

                    launchedNewUpdater = process.Start();
                    if (launchedNewUpdater)
                        process.Exited += updateProcess_Exited;
                } else {
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
                _optionsDialog.AddOptionsPanel(_updateNotifierPanel);
                _optionsDialog.AddOptionsPanel(_logPanel);
                _optionsDialog.AddOptionsPanel(_localizationPanel);
                SocketListenerLinker.AddSocketListenerManagerPanel(_optionsDialog);
                //_optionsDialog.AddOptionsPanel(_processorAffinityPanel);
                _optionsDialog.AddOptionsPanel(_progressNotifierPannel);
                _optionsDialog.AddOptionsPanel(_savingResultsPanel);
                _optionsDialog.AddOptionsPanel(_disableFirewallAutoUpdatePanel);
                _optionsDialog.AddOptionsPanel(_cleanTempDataPanel);
            }
            _optionsDialog.SelectedPanel = panelIndex;
            _optionsDialog.ShowDialog(this);
            SetStatusStrip();
            Cursor = Cursors.Default;
        }

        private void detailedResultsViewerToolStripMenuItem_Click(object sender, EventArgs e) {
            string reportApp = Path.Combine(Application.StartupPath, "vApus.DetailedResultsViewer.exe");
            if (File.Exists(reportApp))
                Process.Start(reportApp);
            else
                MessageBox.Show(
                    "The report application could not be found!\nPlease re-install vApus or do an update with 'Get versioned and non-versioned' checked.",
                    string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void updateProcess_Exited(object sender, EventArgs e) {
            try {
                if (!IsDisposed)
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                        try {
                            Enabled = true;
                            _msgHandler.PostMessage();
                        } catch { }
                    }, null);
            } catch {
            }
        }

        protected override void WndProc(ref Message m) {
            if (_msgHandler != null && m.Msg == _msgHandler.WINDOW_MSG) {
                TopMost = true;
                Show();
                TopMost = false;
            }
            //WM_CLOSE
            if (m.Msg == 16) {
                _saveAndCloseOnUpdate = true;

                TopMost = true;
                Show();
                TopMost = false;

                if (_optionsDialog != null && !_optionsDialog.IsDisposed)
                    _optionsDialog.Close();

                _welcomeView.DisableFormClosingEventHandling();
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
            } else {
                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                distributedToolStripMenuItem.Visible = true;
                singleTestToolStripMenuItem.Visible = true;

                StringBuilder sb;
                if (e.ToBeSaved) {
                    sb = new StringBuilder("*");
                    sb.Append(Solution.ActiveSolution.Name);
                } else {
                    sb = new StringBuilder(Solution.ActiveSolution.Name);
                }

                sb.Append(" - vApus");
                Text = sb.ToString();
            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e) {
            //Look if there is nowhere a process busy (like stresstesting) that can leed to cancelling the closing of the form.
            foreach (Form mdiChild in Solution.RegisteredForCancelFormClosing) {
                mdiChild.Close();
                if (Solution.ExplicitCancelFormClosing) break;
            }

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

                    _welcomeView.DisableFormClosingEventHandling();
                    //For the DockablePanels that are shown as dockstate document, otherwise the form won't close.
                    _welcomeView.Hide();
                    _welcomeView.Close();
                    SolutionComponentViewManager.DisposeViews();
                    e.Cancel = false;
                } else if (result == DialogResult.Cancel) {
                    e.Cancel = true;
                }
            } else {
                tmrSetStatusStrip.Stop();

                _welcomeView.DisableFormClosingEventHandling();
                //For the DockablePanels that are shown as dockstate document, otherwise the form won't close.
                _welcomeView.Hide();
                _welcomeView.Close();
                SolutionComponentViewManager.DisposeViews();
                e.Cancel = false;
            }
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
            } else {
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
        private void welcomeToolStripMenuItem_Click(object sender, EventArgs e) {
            _welcomeView.Show(dockPanel);
            //Show it again the next time.
            Settings.Default.GreetWithWelcomePage = true;
            Settings.Default.Save();
        }

        private void stresstestingSolutionExplorerToolStripMenuItem_Click(object sender, EventArgs e) {
            Solution.ShowStresstestingSolutionExplorer();
        }
        #endregion

        #region Monitors and Stresstests
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

        private void stresstestToolStripMenuItem_DropDownOpened(object sender, EventArgs e) {
            distributedToolStripMenuItem.DropDownItems.Clear();
            singleTestToolStripMenuItem.DropDownItems.Clear();
            if (Solution.ActiveSolution != null) {
                BaseProject distributedTestingProject = Solution.ActiveSolution.GetProject("DistributedTestingProject");
                distributedToolStripMenuItem.Tag = distributedTestingProject;
                SetToolStripMenuItemImage(distributedToolStripMenuItem);
                foreach (BaseItem distributedTest in distributedTestingProject) {
                    var item = new ToolStripMenuItem(distributedTest.ToString());
                    item.Tag = distributedTest;
                    SetToolStripMenuItemImage(item);
                    item.Click += item_Click;
                    distributedToolStripMenuItem.DropDownItems.Add(item);
                }
                BaseProject stresstestProject = Solution.ActiveSolution.GetProject("StresstestProject");
                singleTestToolStripMenuItem.Tag = stresstestProject;
                SetToolStripMenuItemImage(singleTestToolStripMenuItem);
                foreach (BaseItem stresstest in stresstestProject)
                    if (stresstest.Name == "Stresstest") {
                        var item = new ToolStripMenuItem(stresstest.ToString());
                        item.Tag = stresstest;
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
        #endregion

        #endregion

        #region Status Strip
        private void _logPanel_LogErrorCountChanged(object sender, LogPanel.LogErrorCountChangedEventArgs e) {
            try {
                //Show the error messages in a tooltip.
                _logErrorToolTip.Hide();

                int x = statusStrip.Location.X + lblLogLevel.Bounds.X;
                int y = statusStrip.Location.Y - 30;

                _logErrorToolTip.NumberOfErrorsOrFatals = e.LogErrorCount;

                _logErrorToolTip.Show(this, x, y);
            } catch {
            }
        }

        private void tmrSetStatusStrip_Tick(object sender, EventArgs e) {
            if (IsHandleCreated && Visible) try {
                    reopenToolStripMenuItem.Enabled =
                        (Solution.ActiveSolution != null && Solution.ActiveSolution.FileName != null && !Solution.ActiveSolution.IsSaved);
                    SetStatusStrip();
                } catch {
                }
        }

        private void SetStatusStrip() {
            var attr =
                typeof(UpdateNotifierState).GetField(UpdateNotifier.UpdateNotifierState.ToString())
                                            .GetCustomAttributes(typeof(DescriptionAttribute), false) as
                DescriptionAttribute[];
            lblUpdateNotifier.Text = attr[0].Description;

            if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.Disabled ||
                UpdateNotifier.UpdateNotifierState == UpdateNotifierState.FailedConnectingToTheUpdateServer)
                lblUpdateNotifier.Image = Resources.Error;
            else if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.PleaseRefresh ||
                     UpdateNotifier.UpdateNotifierState == UpdateNotifierState.NewUpdateFound)
                lblUpdateNotifier.Image = Resources.Warning;
            else
                lblUpdateNotifier.Image = Resources.OK;

            lblLogLevel.Text = Loggers.GetLogger<FileLogger>().CurrentLevel.ToString();
            lblLocalization.Text = Thread.CurrentThread.CurrentCulture.DisplayName;
            //SetProcessorAffinityLabel();
            lblSocketListener.Text = Dns.GetHostName() + ":" + SocketListenerLinker.SocketListenerPort;
            if (!SocketListenerLinker.SocketListenerIsRunning)
                lblSocketListener.Text += " [Stopped]";

            SetWarningLabel();

            if (_cleanTempDataPanel != null) {
                double tempDataSizeInMB = _cleanTempDataPanel.TempDataSizeInMB;
                lblTempDataSize.Text = tempDataSizeInMB + "MB";

                if (tempDataSizeInMB == 0)
                    lblCleanTempData.Visible =
                        lblTempDataSize.Visible = lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = false;
                else
                    lblCleanTempData.Visible = lblTempDataSize.Visible = true;
            }
        }

        private void SetProcessorAffinityLabel() {
            int[] cpus = ProcessorAffinityHelper.FromBitmaskToArray(Process.GetCurrentProcess().ProcessorAffinity);
            //Make it one-based
            var oneBasedCPUs = new int[cpus.Length];
            for (int i = 0; i != cpus.Length; i++)
                oneBasedCPUs[i] = cpus[i] + 1;

            lblProcessorAffinity.Text = oneBasedCPUs.Combine(", ");
        }

        private void SetWarningLabel() {
            WindowsFirewallAutoUpdatePanel.Status status = _disableFirewallAutoUpdatePanel.CheckStatus();
            switch (status) {
                case WindowsFirewallAutoUpdatePanel.Status.AllDisabled:
                    lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = lblWarning.Visible = false;
                    break;
                case WindowsFirewallAutoUpdatePanel.Status.WindowsFirewallEnabled:
                    lblWarning.Text = "Windows Firewall enabled!";
                    lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = lblWarning.Visible = true;
                    break;
                case WindowsFirewallAutoUpdatePanel.Status.WindowsAutoUpdateEnabled:
                    lblWarning.Text = "Windows Auto Update enabled!";
                    lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = lblWarning.Visible = true;
                    break;
                case WindowsFirewallAutoUpdatePanel.Status.AllEnabled:
                    lblWarning.Text = "Windows Firewall and Auto Update enabled!";
                    lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = lblWarning.Visible = true;
                    break;
            }
            //if (!lblWarning.Visible && !_savingResultsPanel.Connected) {
            //    lblWarning.Text = "Test results cannot be saved!";
            //    lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = lblWarning.Visible = true;
            //}
        }

        private void lblUpdateNotifier_Click(object sender, EventArgs e) { ShowOptionsDialog(0); }

        private void lblLogLevel_Click(object sender, EventArgs e) { ShowOptionsDialog(1); }

        private void lblLocalization_Click(object sender, EventArgs e) { ShowOptionsDialog(2); }

        private void lblSocketListener_Click(object sender, EventArgs e) { ShowOptionsDialog(3); }

        //private void lblProcessorAffinity_Click(object sender, EventArgs e) { ShowOptionsDialog(4); }

        private void lblCleanTempData_Click(object sender, EventArgs e) { ShowOptionsDialog(7); }

        private void lblWarning_Click(object sender, EventArgs e) { if (lblWarning.Text.StartsWith("Windows")) ShowOptionsDialog(6); else ShowOptionsDialog(5); }
        #endregion
    }
}