/*
 * Copyright 2009 (c) Sizing Servers Lab
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
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Tamir.SharpSsh;
using vApus.Gui;
using vApus.Util;

namespace vApus.UpdateTool
{
    public partial class Update : Form
    {
        #region Fields
        private Sftp _sftp;
        private int _previousCaretPosition;
        //This exe is run as a copy, so set the startup path to the parent directory.
        private string _startupPath;
        private List<string[]> _currentVersions;
        private Font _titleFont;
        private Font _dateFont;
        private Font _itemFont;
        private bool _updating;

        private Win32WindowMessageHandler _msgHandler;

        private const int MAXRETRY = 3;

        /// <summary>
        /// To auto connect.
        /// </summary>
        private string _host, _username, _password;
        private int _port = 5222, _channel;
        private bool _autoUpdate;

        #endregion

        #region Init
        /// <summary>
        /// </summary>
        /// <param name="args">If contains GUID, host, port, username, password in that order it will auto connect.</param>
        public Update(string[] args)
        {
            InitializeComponent();
            _startupPath = Directory.GetParent(Application.StartupPath).FullName;

            if (args.Length == 7)
            {
                _host = args[1];
                _port = int.Parse(args[2]);
                _username = args[3];
                _password = args[4];
                _channel = int.Parse(args[5]);
                _autoUpdate = bool.Parse(args[6]);
            }

            this.HandleCreated += new EventHandler(Update_HandleCreated);

            if (_host != null && _autoUpdate)
                this.Shown += new EventHandler(Update_Shown);
        }
        private void Update_HandleCreated(object sender, EventArgs e)
        {
            try
            {
                _titleFont = new Font(rtxtHistoryOfChanges.Font, FontStyle.Bold);
                _dateFont = new Font(rtxtHistoryOfChanges.Font, FontStyle.Italic);
                _itemFont = new Font(rtxtHistoryOfChanges.Font, FontStyle.Regular);

                _msgHandler = new Win32WindowMessageHandler();

                SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;

                LoadConnection();

                _currentVersions = LoadVersion(Path.Combine(_startupPath, "version.ini"));
            }
            catch { }

        }
        private void LoadConnection()
        {
            txtHost.Text = _host;
            nudPort.Value = _port;
            txtUsername.Text = _username;
            txtPassword.Text = _password;
        }
        private void Update_Shown(object sender, EventArgs e)
        {
            this.Shown -= Update_Shown;

            LoadConnection();

            //Tries connecting first, if that works then update.
            if (PerformConnectClick())
                UpdateOrReinstall();
        }
        #endregion

        #region Connect
        private void txtHost_TextChanged(object sender, EventArgs e)
        {
            btnConnect.Enabled = txtHost.Text.Length != 0;
        }
        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            txtPassword.Enabled = txtUsername.Text.Length != 0;
            if (!txtPassword.Enabled)
                txtPassword.Text = string.Empty;
        }
        private void txtPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(e.KeyChar == '\b' || e.KeyChar.IsDigit());
        }

        private void _KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && btnConnect.Enabled == true && btnConnect.Text == "Connect")
                PerformConnectClick();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            PerformConnectClick();
        }
        /// <summary>
        /// Returns true on connected.
        /// </summary>
        /// <returns></returns>
        private bool PerformConnectClick()
        {
            bool connected = false;

            this.Cursor = Cursors.WaitCursor;
            switch (btnConnect.Text)
            {
                case "Connect":
                    try
                    {
                        int port = (int)nudPort.Value;

                        AppendLogLine("Connecting to \"" + txtUsername.Text + '@' + txtHost.Text + '\"' + port + '.', Color.Black);
                        _sftp = new Sftp(txtHost.Text, txtUsername.Text, txtPassword.Text);
                        _sftp.Connect(port);
                        flpConnectTo.Enabled = false;
                        btnRefresh.Enabled = true;
                        btnUpdateOrReinstall.Text = chkGetAll.Checked ? "Reinstall" : "Update";
                        btnConnect.Text = "Disconnect";
                        tcCommit.SelectedIndex = 1;
                        AppendLogLine("Connected.", Color.Green);

                        GetFilesToUpdate();

                        connected = true;
                    }
                    catch (Exception ex)
                    {
                        AppendLogLine("Failed to connect: " + ex.Message, Color.Red);
                    }
                    break;

                default:
                    if (_updating && MessageBox.Show("Are you sure you want to disconnect while updating?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                    {
                        connected = true;
                    }
                    else
                    {
                        _updating = false;
                        try
                        {
                            _sftp.Close();
                        }
                        catch { }
                        _sftp = null;
                        btnUpdateOrReinstall.Enabled = false;
                        lvwUpdate.ClearEmbeddedControls();
                        lvwUpdate.Items.Clear();
                        pbTotal.Value = 0;
                        pbTotal.Tag = null;
                        flpConnectTo.Enabled = true;
                        btnRefresh.Enabled = false;

                        string tempFolder = Path.Combine(_startupPath, "UpdateTempFiles");
                        string tempVersion = Path.Combine(tempFolder, "version.ini");
                        try
                        {
                            if (File.Exists(tempVersion))
                                File.Delete(tempVersion);
                        }
                        catch { }

                        btnConnect.Text = "Connect";
                        AppendLogLine("Disconnected.", Color.Green);
                    }
                    break;
            }
            this.Cursor = Cursors.Default;

            return connected;
        }
        #endregion

        #region Versioning
        /// <summary>
        /// Updates the gui for version and history of changes, and returns the versions of the files.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private List<string[]> LoadVersion(string versionControl)
        {
            bool versionFound = false, channelFound = false, historyFound = false, filesFound = false;
            List<string[]> fileVersions = new List<string[]>();
            string line = string.Empty;

            if (File.Exists(versionControl))
            {
                StreamReader sr = new StreamReader(versionControl);
                while (sr.Peek() != -1)
                {
                    line = sr.ReadLine().Trim();

                    if (line.Length == 0)
                        continue;

                    switch (line)
                    {
                        case "[VERSION]":
                            versionFound = true;
                            continue;
                        case "[CHANNEL]":
                            channelFound = true;
                            continue;
                        case "[HISTORY]":
                            historyFound = true;
                            continue;
                        case "[FILES]":
                            filesFound = true;
                            continue;
                    }

                    if (filesFound)
                    {
                        string[] splittedLine = line.Split(':');
                        if (splittedLine.Length == 2)
                            fileVersions.Add(splittedLine);
                    }
                    else if (historyFound)
                    {
                        FillHistoryOfChanges(line);
                        historyFound = false;
                    }
                    else if (channelFound)
                    {
                        lblChannel.Text = "Channel: " + line;
                        channelFound = false;
                    }
                    else if (versionFound)
                    {
                        lblVersion.Text = "Version: " + line;
                        versionFound = false;
                    }
                }
                try { sr.Close(); }
                catch { }
                try { sr.Dispose(); }
                catch { }
                sr = null;
            }
            return fileVersions;
        }
        private void FillHistoryOfChanges(string historyOfChanges)
        {
            rtxtHistoryOfChanges.Text = string.Empty;

            List<HistoryPart> parts = new List<HistoryPart>();

            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(historyOfChanges));
            XmlReader reader = XmlReader.Create(ms);
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.ReadNode(reader);

            //First filling the rtxt and then applying the style
            int previousCaretPosition = 0;
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "t":
                        foreach (XmlNode nn in n.ChildNodes)
                        {
                            switch (nn.Name)
                            {
                                case "d":
                                    rtxtHistoryOfChanges.Text = rtxtHistoryOfChanges.Text + " (" + nn.InnerText + ")" + Environment.NewLine;
                                    parts.Add(new HistoryPart("d", previousCaretPosition, rtxtHistoryOfChanges.Text.Length - previousCaretPosition));
                                    break;
                                default:
                                    if (previousCaretPosition > 0)
                                        rtxtHistoryOfChanges.Text = rtxtHistoryOfChanges.Text + Environment.NewLine + nn.InnerText;
                                    else
                                        rtxtHistoryOfChanges.Text = rtxtHistoryOfChanges.Text + nn.InnerText;
                                    parts.Add(new HistoryPart("t", previousCaretPosition, rtxtHistoryOfChanges.Text.Length - previousCaretPosition));
                                    break;
                            }
                            previousCaretPosition = rtxtHistoryOfChanges.Text.Length;
                            rtxtHistoryOfChanges.Select(rtxtHistoryOfChanges.Text.Length, 0);
                        }
                        break;
                    case "i":
                        rtxtHistoryOfChanges.Text = rtxtHistoryOfChanges.Text + n.InnerText + Environment.NewLine;
                        parts.Add(new HistoryPart("i", previousCaretPosition, rtxtHistoryOfChanges.Text.Length - previousCaretPosition));
                        break;
                }
                previousCaretPosition = rtxtHistoryOfChanges.Text.Length;
            }
            foreach (HistoryPart part in parts)
            {
                rtxtHistoryOfChanges.Select(part.SelectionStart, part.Length);
                switch (part.Type)
                {
                    case "d":
                        rtxtHistoryOfChanges.SelectionFont = _dateFont;
                        rtxtHistoryOfChanges.SelectionColor = Color.Blue;
                        break;
                    case "i":
                        rtxtHistoryOfChanges.SelectionFont = _itemFont;
                        rtxtHistoryOfChanges.SelectionBullet = true;
                        break;
                    default:
                        rtxtHistoryOfChanges.SelectionFont = _titleFont;
                        rtxtHistoryOfChanges.SelectionColor = Color.Green;
                        break;
                }
            }
            rtxtHistoryOfChanges.Select(0, 0);
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            lvwUpdate.ClearEmbeddedControls();
            lvwUpdate.Items.Clear();
            pbTotal.Value = 0;
            pbTotal.Tag = null;
            btnUpdateOrReinstall.Text = chkGetAll.Checked ? "Reinstall" : "Update";
            tcCommit.SelectedIndex = 1;
            GetFilesToUpdate();
            this.Cursor = Cursors.Default;
        }
        private void GetFilesToUpdate()
        {
            try
            {
                AppendLogLine("Getting the list of files needed to be versioned.", Color.Green);

                string tempFolder = Path.Combine(_startupPath, "UpdateTempFiles");
                if (Directory.Exists(tempFolder) && Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories).Length == 0)
                    Directory.Delete(tempFolder, true);

                Directory.CreateDirectory(tempFolder);
                string readme = Path.Combine(tempFolder, "README.TXT");
                if (!File.Exists(readme))
                    using (var sw = new StreamWriter(readme))
                    {
                        sw.Write("All new files are found here (except this readme which is generated), for a manual update you have to overwrite the files in the upper folder with these.\nThis folder has no use when empty so it can be removed safely.");
                        sw.Flush();
                    }

                string tempVersion = Path.Combine(tempFolder, "version.ini");

                try
                {
                    if (File.Exists(tempVersion))
                        File.Delete(tempVersion);
                }
                catch { }

                string channelDir = _channel == 0 ? "stable" : "nightly";
                _sftp.Get(channelDir + "/version.ini", tempVersion);

                List<string[]> serverVersions = LoadVersion(tempVersion);

                foreach (string[] line in serverVersions)
                {
                    string md5Hash = string.Empty;
                    if (chkGetAll.Checked | !AlreadyVersioned(line, _currentVersions, out md5Hash))
                    {
                        ListViewItem lvwi = new ListViewItem(line);
                        lvwi.SubItems.Add(md5Hash);
                        lvwUpdate.Items.Add(lvwi);
                        lvwUpdate.AddEmbeddedControl(new ProgressBar(), lvwUpdate.Columns.Count - 1, lvwUpdate.Items.Count - 1);
                    }
                }

                btnUpdateOrReinstall.Enabled = lvwUpdate.Items.Count != 0;
                if (!btnUpdateOrReinstall.Enabled && Directory.Exists(tempFolder))
                    Directory.Delete(tempFolder, true);

                AppendLogLine("Done.", Color.Green);
            }
            catch (Exception ex)
            {
                AppendLogLine("Failed to get the list of files needed to be versioned: " + ex.Message, Color.Red);
            }
        }
        private bool AlreadyVersioned(string[] entry, List<string[]> versioned, out string md5Hash)
        {
            md5Hash = string.Empty;
            foreach (string[] line in versioned)
            {
                List<bool> equals = new List<bool>(line.Length);
                for (int i = 0; i < line.Length; i++)
                    equals.Add(line[i] == entry[i]);

                if (equals[0] == true)
                    md5Hash = line[1];
                if (!equals.Contains(false))
                    return true;
            }

            return false;
        }
        #endregion

        #region Update or reinstall
        private void btnUpdateOrReinstall_Click(object sender, EventArgs e)
        {
            UpdateOrReinstall();
        }
        private void UpdateOrReinstall()
        {
            AppendLogLine("Updating/re-installing started.", Color.Black);
            tcCommit.SelectedIndex = 1;
            string tempFolder = Path.Combine(_startupPath, "UpdateTempFiles");
            _sftp.OnTransferProgress += new FileTransferEvent(_sftp_OnTransferProgress);
            _sftp.OnTransferEnd += new FileTransferEvent(_sftp_OnTransferEnd);
            try
            {
                btnRefresh.Enabled = false;
                chkGetAll.Enabled = false;
                btnUpdateOrReinstall.Enabled = false;
                string possibleNonExistingFolder;

                string channelDir = _channel == 0 ? "stable" : "nightly";

                Dictionary<string, string> toUpdate = new Dictionary<string, string>(lvwUpdate.Items.Count);
                foreach (ListViewItem lvwi in lvwUpdate.Items)
                {
                    possibleNonExistingFolder = tempFolder;
                    string[] splittedPath = lvwi.SubItems[0].Text.Split('\\');
                    for (int i = 0; i < splittedPath.Length - 1; i++)
                        possibleNonExistingFolder = Path.Combine(possibleNonExistingFolder, splittedPath[i]);
                    if (!Directory.Exists(possibleNonExistingFolder))
                        Directory.CreateDirectory(possibleNonExistingFolder);

                    lvwi.Tag = Path.Combine(tempFolder, lvwi.SubItems[0].Text);
                    toUpdate.Add(channelDir + "/" + lvwi.SubItems[0].Text.Replace('\\', '/'), lvwi.Tag as string);
                }

                Thread t = new Thread(delegate()
                {
                    try
                    {
                        foreach (string key in toUpdate.Keys)
                            _sftp.Get(key, toUpdate[key]);

                        SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                        {
                            btnRefresh.Enabled = true;
                            chkGetAll.Enabled = true;

                            _sftp.OnTransferProgress -= _sftp_OnTransferProgress;
                            _sftp.OnTransferEnd -= _sftp_OnTransferEnd;

                            AppendLogLine("Completed!", Color.Green);

                            OverwriteFiles();

                        });
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                            {
                                AppendLogLine("Failed to update or reinstall: " + ex.Message, Color.Red);
                            });
                        }
                        catch { }
                    }
                    finally
                    {
                        _updating = false;
                    }
                });
                t.Start();
            }
            catch (Exception ex)
            {
                PerformConnectClick();
                AppendLogLine("Failed to update or reinstall: " + ex.Message, Color.Red);
            }
        }
        private void OverwriteFiles()
        {
            this.Enabled = false;
            try
            {
                if (Process.GetProcessesByName("vApus").Length != 0)
                {
                    string message = "vApus will now be updated, click 'Yes' to close all running instances.";
                    if (MessageBox.Show(message, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        int retry = 0;
                    Retry:
                        foreach (Process p in Process.GetProcessesByName("vApus"))
                            try
                            {
                                p.Kill();
                                p.WaitForExit(10000);
                            }
                            catch { }
                        if (Process.GetProcessesByName("vApus").Length != 0)
                            if (++retry <= MAXRETRY)
                            {
                                Thread.Sleep(1000 * retry);
                                goto Retry;
                            }
                            else
                            {
                                message = "Something went wrong when trying to close one or more instances of vApus.\nPlease close it manually and click 'Yes' or 'No', or click 'Yes' to try again.";
                                if (MessageBox.Show(message, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                                {
                                    retry = 0;
                                    goto Retry;
                                }
                            }
                    }
                    else
                    {
                        this.Enabled = true;
                        return;
                    }
                }

                //Kill vApus JumpStart
                int retryKillJumpStart = 0;
            RetryKillJumpStart:
                foreach (Process p in Process.GetProcessesByName("vApus.JumpStart"))
                    try
                    {
                        p.Kill();
                        p.WaitForExit(10000);
                    }
                    catch { }
                if (Process.GetProcessesByName("vApus.JumpStart").Length != 0)
                    if (++retryKillJumpStart <= MAXRETRY)
                    {
                        Thread.Sleep(1000 * retryKillJumpStart);
                        goto RetryKillJumpStart;
                    }

                string filename;
                foreach (ListViewItem lvwi in lvwUpdate.Items)
                {
                    filename = Path.Combine(_startupPath, lvwi.Text);
                    if (File.Exists(filename))
                    {
                        int sleepTime = 0;
                        while (File.Exists(filename) && File.GetAttributes(filename) == FileAttributes.ReadOnly && sleepTime++ < 3000)
                            Thread.Sleep(1);
                        Exception ex = null;
                        while (File.Exists(filename) && sleepTime++ < 6000)
                        {
                            try
                            {
                                File.Delete(filename);
                            }
                            catch (Exception e)
                            {
                                ex = e;
                                Thread.Sleep(1);
                            }
                        }
                        if (File.Exists(filename))
                        {
                            MessageBox.Show(filename + "\n\n" + ex.ToString());

                            //MessageBox.Show("Not all files where update due to access or authorization errors!\nThose files are stored in the 'UpdateTempFiles' folder located in the top directory of vApus, so you can put them at the right place manually.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            throw ex;
                        }
                    }
                }

                string tempFolder = Path.Combine(_startupPath, "UpdateTempFiles");
                TryOverwriteDirectoriesAndFiles(tempFolder, _startupPath);

                foreach (string d in Directory.GetDirectories(_startupPath, "*", SearchOption.AllDirectories))
                    try
                    {
                        if (Directory.GetFiles(d).Length == 0 && Directory.GetDirectories(d).Length == 0)
                            Directory.Delete(d);
                    }
                    catch { }

                if (!Directory.Exists(tempFolder))
                {
                    if (MessageBox.Show("Do you want to start vApus now?", "Updated!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        Process.Start(Path.Combine(_startupPath, "vApus.exe"));
                }
                else if (Directory.Exists(tempFolder) && Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories).Length == 0)
                {
                    Directory.Delete(tempFolder, true);
                    if (MessageBox.Show("Do you want to start vApus now?", "Updated!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        Process.Start(Path.Combine(_startupPath, "vApus.exe"));
                }
                else
                {
                    MessageBox.Show("Not all files where updated due to access or authorization errors!\nThose files are stored in the 'UpdateTempFiles' folder located in the top directory of vApus, so you can put them at the right place manually.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                //Start JumpStart
                try
                {
                    string jumpStartPath = Path.Combine(_startupPath, "vApus.JumpStart.exe");
                    if (File.Exists(jumpStartPath))
                        Process.Start(jumpStartPath);
                }
                catch { }
                this.Enabled = true;
                _updating = false;
            }
            this.Close();
        }
        private void TryOverwriteDirectoriesAndFiles(string from, string to)
        {
            string[] splittedFilename;
            foreach (string f in Directory.GetFiles(from))
            {
                try
                {
                    splittedFilename = f.Split('\\');
                    File.Copy(f, Path.Combine(to, splittedFilename[splittedFilename.Length - 1]), true);
                    File.Delete(f);
                }
                catch { }
            }
            string[] splittedDirectoryName;
            string directoryName, newFrom, newTo;
            foreach (string d in Directory.GetDirectories(from))
            {
                splittedDirectoryName = d.Split('\\');
                directoryName = splittedDirectoryName[splittedDirectoryName.Length - 1];
                newFrom = Path.Combine(from, directoryName);
                newTo = Path.Combine(to, directoryName);

                if (!Directory.Exists(newTo))
                    Directory.CreateDirectory(newTo);
                TryOverwriteDirectoriesAndFiles(newFrom, newTo);
            }
        }
        private void _sftp_OnTransferEnd(string src, string dst, int transferredBytes, int totalBytes, string message)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                for (int i = 0; i < lvwUpdate.Items.Count; i++)
                    if (lvwUpdate.Items[i].Tag as string == dst)
                    {
                        lvwUpdate.Items[i].Selected = true;
                        lvwUpdate.SelectedItems[0].EnsureVisible();
                        ProgressBar pb = lvwUpdate.EmbeddedControls[i] as ProgressBar;
                        pb.Value = pb.Maximum;
                        break;
                    }
                float total;
                if (pbTotal.Tag == null)
                    total = (float)pbTotal.Maximum / lvwUpdate.Items.Count;
                else
                    total = (float)pbTotal.Tag + (float)pbTotal.Maximum / lvwUpdate.Items.Count;

                pbTotal.Tag = total;
                if (total == pbTotal.Maximum)
                    pbTotal.Value = pbTotal.Maximum;
                else
                    pbTotal.Value = (int)total;
            });
        }

        private void _sftp_OnTransferProgress(string src, string dst, int transferredBytes, int totalBytes, string message)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                for (int i = 0; i < lvwUpdate.Items.Count; i++)
                    if (lvwUpdate.Items[i].Tag as string == dst)
                    {
                        lvwUpdate.Items[i].Selected = true;
                        lvwUpdate.SelectedItems[0].EnsureVisible();
                        ProgressBar pb = lvwUpdate.EmbeddedControls[i] as ProgressBar;
                        pb.Maximum = totalBytes;
                        pb.Value = transferredBytes;
                        break;
                    }
            });
        }
        #endregion

        #region Other
        private void AppendLogLine(string line, Color color)
        {
            switch (rtxtLog.Text.Length)
            {
                case 0:
                    rtxtLog.Text = line;
                    break;
                default:
                    rtxtLog.Text = rtxtLog.Text + Environment.NewLine + line;
                    break;
            }
            rtxtLog.Select(_previousCaretPosition, rtxtLog.Text.Length - _previousCaretPosition);
            rtxtLog.SelectionColor = color;
            rtxtLog.Select(rtxtLog.Text.Length, 0);
            _previousCaretPosition = rtxtLog.SelectionStart;
            rtxtLog.ScrollToCaret();
        }
        protected override void WndProc(ref Message m)
        {
            if (_msgHandler != null && m.Msg == _msgHandler.WINDOW_MSG)
            {
                this.TopMost = true;
                this.TopMost = false;
                this.Activate();
            }
            base.WndProc(ref m);
        }
        private void Update_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_updating)
            {
                if (MessageBox.Show("Are you sure you want to disconnect while updating?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    _updating = false;
                    PerformConnectClick();
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else if (_sftp != null)
            {
                try
                {
                    if (_sftp.Connected)
                        _sftp.Close();
                }
                catch { }
                _sftp = null;
                string tempFolder = Path.Combine(_startupPath, "UpdateTempFiles");
                if (Directory.Exists(tempFolder) && Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories).Length == 0)
                    Directory.Delete(tempFolder, true);
            }
        }
        #endregion
    }
}