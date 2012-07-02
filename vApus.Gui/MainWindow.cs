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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using vApus.Link;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Gui
{
    public partial class MainWindow : Form
    {
        #region Fields
        public const UInt32 FLASHW_TRAY = 2;
        private Win32WindowMessageHandler _msgHandler;
        private string[] _args;

        private Welcome _welcome = new Welcome();
        private About _about = new About();
        private OptionsDialog _optionsDialog;
        private UpdateNotifierPanel _updateNotifierPanel;
        private LogPanel _logPanel;
        private LogErrorToolTip _logErrorToolTip;
        private LocalizationPanel _localizationPanel;
        private ProcessorAffinityPanel _processorAffinityPanel;
        private CleanTempDataPanel _cleanTempDataPanel;
        private DisableFirewallAutoUpdatePanel _disableFirewallAutoUpdatePanel;
        #endregion

        public MainWindow(string[] args = null)
        {
            _args = args;
            Init();
        }

        #region Init
        private void Init()
        {
            InitializeComponent();
            mainMenu.ImageList = new ImageList();
            mainMenu.ImageList.ColorDepth = ColorDepth.Depth24Bit;
            _msgHandler = new Win32WindowMessageHandler();

            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(MainWindow_HandleCreated);
        }
        private void MainWindow_HandleCreated(object sender, EventArgs e)
        {
            this.HandleCreated -= MainWindow_HandleCreated;
            SetGui();
        }
        private void SetGui()
        {
            SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;
            Solution.RegisterDockPanel(dockPanel);
            Solution.ActiveSolutionChanged += new EventHandler<ActiveSolutionChangedEventArgs>(Solution_ActiveSolutionChanged);
            Solution.ShowStresstestingSolutionExplorer();
            _welcome.Show(dockPanel);
            OnActiveSolutionChanged(null);

            string error = ArgumentsAnalyzer.AnalyzeAndExecute(_args);
            if (error.Length != 0)
                LogWrapper.LogByLevel("Argument Analyzer " + error, LogLevel.Error);

            _updateNotifierPanel = new UpdateNotifierPanel();
            _logPanel = new LogPanel();
            _logPanel.LogErrorCountChanged += new EventHandler<LogPanel.LogErrorCountChangedEventArgs>(_logPanel_LogErrorCountChanged);
            _logErrorToolTip = new LogErrorToolTip();
            _logErrorToolTip.AutoPopDelay = 10000;

            _localizationPanel = new LocalizationPanel();
            _processorAffinityPanel = new ProcessorAffinityPanel();
            _cleanTempDataPanel = new CleanTempDataPanel();
            _disableFirewallAutoUpdatePanel = new DisableFirewallAutoUpdatePanel();

            string host, username, password;
            int port, channel;
            UpdateNotifier.GetCredentials(out host, out port, out username, out password, out channel);

            UpdateNotifier.Refresh();

            if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.NewUpdateFound &&
                UpdateNotifier.GetUpdateNotifierDialog().ShowDialog() == DialogResult.OK)
                //Doing stuff automatically
                if (Update(host, port, username, password, channel))
                    this.Close();
        }
        #endregion

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            RelocateLogErrorToolTip();
        }
        private void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            RelocateLogErrorToolTip();
        }

        private void RelocateLogErrorToolTip()
        {
            try
            {
                if (_logErrorToolTip.Visible)
                {
                    int x = statusStrip.Location.X + lblLogLevel.Bounds.X;
                    int y = statusStrip.Location.Y - 30;

                    _logErrorToolTip.Location = new Point(x, y);
                }
            }
            catch { }
        }

        #region Menu

        private void SetToolStripMenuItemImage(ToolStripMenuItem item)
        {
            SolutionComponent component = item.Tag as SolutionComponent;
            string componentTypeName = component.GetType().Name;
            if (!mainMenu.ImageList.Images.Keys.Contains(componentTypeName))
            {
                Image image = component.GetImage();
                if (image != null)
                    mainMenu.ImageList.Images.Add(componentTypeName, image);
            }
            item.Image = mainMenu.ImageList.Images[componentTypeName];
        }

        #region File
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e)
        {
            OnActiveSolutionChanged(e);
        }
        //Append the text if needed (show the solution name, show a * if something has changed).
        private void OnActiveSolutionChanged(ActiveSolutionChangedEventArgs e)
        {
            if (Solution.ActiveSolution == null)
            {
                this.Text = "vApus";
            }
            else
            {
                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                distributedToolStripMenuItem.Visible = true;
                singleTestToolStripMenuItem.Visible = true;

                StringBuilder sb;
                if (e.ToBeSaved)
                {
                    sb = new StringBuilder("*");
                    sb.Append(Solution.ActiveSolution.Name);
                }
                else
                {
                    sb = new StringBuilder(Solution.ActiveSolution.Name);
                }

                sb.Append(" - vApus");
                this.Text = sb.ToString();
            }
        }
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Solution.ActiveSolution != null)
            {
                //Look if there is nowhere a process busy (like stresstesting) that can leed to cancelling the closing of the form.
                foreach (Form mdiChild in Solution.ActiveSolution.RegisteredForCancelFormClosing)
                {
                    mdiChild.Close();
                    if (Solution.ActiveSolution.ExplicitCancelFormClosing)
                        break;
                }

                if (Solution.ActiveSolution.ExplicitCancelFormClosing)
                {
                    e.Cancel = true;
                    Solution.ActiveSolution.ExplicitCancelFormClosing = false;
                    return;
                }
            }
            if (Solution.ActiveSolution != null && (!Solution.ActiveSolution.IsSaved || Solution.ActiveSolution.FileName == null))
            {
                DialogResult result = MessageBox.Show(string.Format("Do you want to save '{0}' before exiting the application?", Solution.ActiveSolution.Name), string.Empty, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes || result == DialogResult.No)
                {
                    if (result == DialogResult.Yes)
                        Solution.SaveActiveSolution();
                    tmrSetStatusStrip.Stop();

                    //For the DockablePanels that are shown as dockstate document, otherwise the form won't close.
                    _welcome.Hide();
                    _welcome.Close();
                    SolutionComponentViewManager.DisposeViews();
                    e.Cancel = false;
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                tmrSetStatusStrip.Stop();

                //For the DockablePanels that are shown as dockstate document, otherwise the form won't close.
                _welcome.Hide();
                _welcome.Close();
                SolutionComponentViewManager.DisposeViews();
                e.Cancel = false;
            }
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            Solution.CreateNew();
            this.Cursor = Cursors.Default;
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            Solution.LoadNewActiveSolution();
            this.Cursor = Cursors.Default;
        }
        private void openRecentToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            List<ToolStripMenuItem> recentSolutions = Solution.GetRecentSolutionsMenuItems();
            ToolStripItem[] defaultItems = new ToolStripItem[2];
            for (int i = 0; i < 2; i++)
                defaultItems[i] = openRecentToolStripMenuItem.DropDownItems[i];
            openRecentToolStripMenuItem.DropDownItems.Clear();
            openRecentToolStripMenuItem.DropDownItems.AddRange(defaultItems);
            if (recentSolutions.Count > 0)
            {
                clearToolStripMenuItem.Enabled = true;
                openRecentToolStripMenuItem.DropDownItems.AddRange(recentSolutions.ToArray());
            }
            else
            {
                clearToolStripMenuItem.Enabled = false;
                ToolStripMenuItem emptyItem = new ToolStripMenuItem("<empty>");
                emptyItem.Enabled = false;
                openRecentToolStripMenuItem.DropDownItems.Add(emptyItem);
            }
            this.Cursor = Cursors.Default;
        }
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (MessageBox.Show("Are you sure you want to clear this?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                clearToolStripMenuItem.Enabled = false;
                List<ToolStripMenuItem> recentSolutions = Solution.GetRecentSolutionsMenuItems();
                if (recentSolutions.Count > 0)
                {
                    Solution.ClearRecentSolutions();
                    while (openRecentToolStripMenuItem.DropDownItems.Count > 2)
                        openRecentToolStripMenuItem.DropDownItems.RemoveAt(2);
                    ToolStripMenuItem emptyItem = new ToolStripMenuItem("<empty>");
                    emptyItem.Enabled = false;
                    openRecentToolStripMenuItem.DropDownItems.Add(emptyItem);
                }
            }
            this.Cursor = Cursors.Default;
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            Solution.SaveActiveSolution();
            this.Cursor = Cursors.Default;
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            Solution.SaveActiveSolutionAs();
            this.Cursor = Cursors.Default;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region View
        private void welcomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _welcome.Show(dockPanel);
        }
        private void stresstestingSolutionExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Solution.ShowStresstestingSolutionExplorer();
        }
        #endregion

        #region Monitors and Stresstests
        private void monitorToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            monitorToolStripMenuItem.DropDownItems.Clear();
            if (Solution.ActiveSolution != null)
                foreach (BaseItem monitor in Solution.ActiveSolution.GetProject("MonitorProject"))
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(monitor.ToString());
                    item.Tag = monitor;
                    SetToolStripMenuItemImage(item);
                    item.Click += new EventHandler(item_Click);
                    monitorToolStripMenuItem.DropDownItems.Add(item);
                }
        }
        private void stresstestToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            distributedToolStripMenuItem.DropDownItems.Clear();
            singleTestToolStripMenuItem.DropDownItems.Clear();
            if (Solution.ActiveSolution != null)
            {
                BaseProject distributedTestingProject = Solution.ActiveSolution.GetProject("DistributedTestingProject");
                distributedToolStripMenuItem.Tag = distributedTestingProject;
                SetToolStripMenuItemImage(distributedToolStripMenuItem);
                foreach (BaseItem distributedTest in distributedTestingProject)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(distributedTest.ToString());
                    item.Tag = distributedTest;
                    SetToolStripMenuItemImage(item);
                    item.Click += new EventHandler(item_Click);
                    distributedToolStripMenuItem.DropDownItems.Add(item);
                }
                BaseProject stresstestProject = Solution.ActiveSolution.GetProject("StresstestProject");
                singleTestToolStripMenuItem.Tag = stresstestProject;
                SetToolStripMenuItemImage(singleTestToolStripMenuItem);
                foreach (BaseItem stresstest in stresstestProject)
                    if (stresstest.Name == "Stresstest")
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem(stresstest.ToString());
                        item.Tag = stresstest;
                        SetToolStripMenuItemImage(item);
                        item.Click += new EventHandler(item_Click);
                        singleTestToolStripMenuItem.DropDownItems.Add(item);
                    }
            }
        }
        private void item_Click(object sender, EventArgs e)
        {
            ((sender as ToolStripMenuItem).Tag as LabeledBaseItem).Activate();
        }
        #endregion

        #region Help
        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _about.ShowDialog();
        }
        #endregion

        /// <summary>
        /// Params are for autoconnect (using auto update)
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>true if a new updater is launched.</returns>
        private bool Update(string host, int port, string username, string password, int channel)
        {
            bool launchedNewUpdater = false;

            if (host != null)
            {
                this.Cursor = Cursors.WaitCursor;
                string path = Path.Combine(Application.StartupPath, "vApus.UpdateToolLoader.exe");
                if (File.Exists(path))
                {
                    this.Enabled = false;
                    Process process = new Process();
                    process.EnableRaisingEvents = true;
                    process.StartInfo = new ProcessStartInfo(path, "{A84E447C-3734-4afd-B383-149A7CC68A32} " + host + " " +
                            port + " " + username + " " + password + " " + channel + " " + true);

                    launchedNewUpdater = process.Start();
                    if (launchedNewUpdater)
                        process.Exited += new EventHandler(updateProcess_Exited);
                }
                else
                {
                    MessageBox.Show("vApus could not be updated because the update tool was not found!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }

                if (!launchedNewUpdater)
                    this.Enabled = true;

                this.Cursor = Cursors.Default;
            }

            return launchedNewUpdater;
        }
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowOptionsDialog();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="panelIndex">The panel to show.</param>
        private void ShowOptionsDialog(int panelIndex = 0)
        {
            this.Cursor = Cursors.WaitCursor;
            tmrSetStatusStrip.Stop();
            if (_optionsDialog == null)
            {
                _optionsDialog = new OptionsDialog();
                _optionsDialog.AddOptionsPanel(_updateNotifierPanel);
                _optionsDialog.AddOptionsPanel(_logPanel);
                _optionsDialog.AddOptionsPanel(_localizationPanel);
                _optionsDialog.AddOptionsPanel(_processorAffinityPanel);
                SocketListenerLinker.AddSocketListenerManagerPanel(_optionsDialog);
                _optionsDialog.AddOptionsPanel(_cleanTempDataPanel);
                _optionsDialog.AddOptionsPanel(_disableFirewallAutoUpdatePanel);
            }
            _optionsDialog.SelectedPanel = panelIndex;
            _optionsDialog.ShowDialog(this);
            SetStatusStrip();
            tmrSetStatusStrip.Start();
            this.Cursor = Cursors.Default;
        }
        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string reportApp = Path.Combine(Application.StartupPath, "vApus.Report.exe");
            if (File.Exists(reportApp))
                Process.Start(reportApp);
            else
                MessageBox.Show("The report application could not be found!\nPlease re-install vApus or do an update with 'Get versioned and non-versioned' checked.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void updateProcess_Exited(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsDisposed)
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                    {
                        try
                        {
                            this.Enabled = true;
                            _msgHandler.PostMessage();
                        }
                        catch { }
                    }, null);
            }
            catch { }
        }
        protected override void WndProc(ref Message m)
        {
            if (_msgHandler != null && m.Msg == _msgHandler.WINDOW_MSG)
            {
                this.TopMost = true;
                this.Show();
                this.TopMost = false;
            }
            base.WndProc(ref m);
        }
        #endregion

        #region Status Strip
        private void _logPanel_LogErrorCountChanged(object sender, LogPanel.LogErrorCountChangedEventArgs e)
        {
            try
            {
                //Show the error messages in a tooltip.
                _logErrorToolTip.Hide();

                int x = statusStrip.Location.X + lblLogLevel.Bounds.X;
                int y = statusStrip.Location.Y - 30;

                _logErrorToolTip.NumberOfErrorsOrFatals = e.LogErrorCount;

                _logErrorToolTip.Show(this, x, y);
            }
            catch { }
        }
        private void tmrSetStatusStrip_Tick(object sender, EventArgs e)
        {
            if (this.IsHandleCreated)
                SetStatusStrip();
        }
        private void SetStatusStrip()
        {
            DescriptionAttribute[] attr = typeof(UpdateNotifierState).GetField(UpdateNotifier.UpdateNotifierState.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            lblUpdateNotifier.Text = attr[0].Description;

            if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.Disabled ||
                UpdateNotifier.UpdateNotifierState == UpdateNotifierState.FailedConnectingToTheUpdateServer)
                lblUpdateNotifier.Image = vApus.Gui.Properties.Resources.Error;
            else if (UpdateNotifier.UpdateNotifierState == UpdateNotifierState.PleaseRefresh ||
                UpdateNotifier.UpdateNotifierState == UpdateNotifierState.NewUpdateFound)
                lblUpdateNotifier.Image = vApus.Gui.Properties.Resources.Warning;
            else
                lblUpdateNotifier.Image = vApus.Gui.Properties.Resources.OK;

            lblLogLevel.Text = LogWrapper.LogLevel.ToString();
            lblLocalization.Text = Thread.CurrentThread.CurrentCulture.DisplayName;
            SetProcessorAffinityLabel();
            lblSocketListener.Text = SocketListenerLinker.SocketListenerIP + ":" + SocketListenerLinker.SocketListenerPort;
            if (!SocketListenerLinker.SocketListenerIsRunning)
                lblSocketListener.Text += " [Stopped]";

            if (_cleanTempDataPanel != null)
            {
                double tempDataSizeInMB = _cleanTempDataPanel.TempDataSizeInMB;
                lblCleanTempData.Text = tempDataSizeInMB + "MB";
                lblCleanTempData.Font = new Font(lblCleanTempData.Font, tempDataSizeInMB == 0 ? FontStyle.Regular : FontStyle.Bold);
            }
            SetWindowsFirewallAutoUpdateLabel();
        }
        private void SetProcessorAffinityLabel()
        {
            int[] cpus = ProcessorAffinityCalculator.FromBitmaskToArray(Process.GetCurrentProcess().ProcessorAffinity);
            //Make it one-based
            int[] oneBasedCPUs = new int[cpus.Length];
            for (int i = 0; i != cpus.Length; i++)
                oneBasedCPUs[i] = cpus[i] + 1;

            lblProcessorAffinity.Text = oneBasedCPUs.Combine(", ");
        }
        private void SetWindowsFirewallAutoUpdateLabel()
        {
            var status = _disableFirewallAutoUpdatePanel.CheckStatus();
            switch (status)
            {
                case DisableFirewallAutoUpdatePanel.Status.AllDisabled:
                    lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = lblMicrosoftFirewallAutoUpdateEnabled.Visible = false;
                    break;
                case DisableFirewallAutoUpdatePanel.Status.WindowsFirewallEnabled:
                    lblMicrosoftFirewallAutoUpdateEnabled.Text = "Windows Firewall Enabled!";
                    lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = lblMicrosoftFirewallAutoUpdateEnabled.Visible = true;
                    break;
                case DisableFirewallAutoUpdatePanel.Status.WindowsAutoUpdateEnabled:
                    lblMicrosoftFirewallAutoUpdateEnabled.Text = "Windows Auto Update Enabled!";
                    lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = lblMicrosoftFirewallAutoUpdateEnabled.Visible = true;
                    break;
                case DisableFirewallAutoUpdatePanel.Status.AllEnabled:
                    lblMicrosoftFirewallAutoUpdateEnabled.Text = "Windows Firewall and Auto Update Enabled!";
                    lblPipeMicrosoftFirewallAutoUpdateEnabled.Visible = lblMicrosoftFirewallAutoUpdateEnabled.Visible = true;
                    break;
            }

        }
        private void lblUpdateNotifier_Click(object sender, EventArgs e)
        {
            ShowOptionsDialog(0);
        }
        private void lblLogLevel_Click(object sender, EventArgs e)
        {
            ShowOptionsDialog(1);
        }
        private void lblLocalization_Click(object sender, EventArgs e)
        {
            ShowOptionsDialog(2);
        }
        private void lblProcessorAffinity_Click(object sender, EventArgs e)
        {
            ShowOptionsDialog(3);
        }
        private void lblSocketListener_Click(object sender, EventArgs e)
        {
            ShowOptionsDialog(4);
        }
        private void lblCleanTempData_Click(object sender, EventArgs e)
        {
            ShowOptionsDialog(5);
        }
        private void lblMicrosoftFirewallAutoUpdateEnabled_Click(object sender, EventArgs e)
        {
            ShowOptionsDialog(6);
        }
        #endregion
    }
}