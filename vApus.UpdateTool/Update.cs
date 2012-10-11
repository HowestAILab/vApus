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
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Tamir.SharpSsh;
using vApus.Util;

namespace vApus.UpdateTool
{
    public partial class Update : Form
    {
        #region Fields
        private Sftp _sftp;
        //This exe is run as a copy, so set the startup path to the parent directory.
        private string _startupPath;
        private List<string[]> _currentVersions;
        private bool _updating;

        private Win32WindowMessageHandler _msgHandler;

        private const int MAXRETRY = 3;

        /// <summary>
        /// To auto connect.
        /// </summary>
        private string _host, _userName, _password;
        private int _port = 22; //External port 5222
        private int _channel;
        private bool _force; //Update all files regardesly that they must or must not be updated

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
                _userName = args[3];
                _password = args[4];
                _channel = int.Parse(args[5]);
                _force = bool.Parse(args[6]);
            }

            this.HandleCreated += new EventHandler(Update_HandleCreated);

            if (_host != null)
                this.Shown += new EventHandler(Update_Shown);
        }
        private void Update_HandleCreated(object sender, EventArgs e)
        {
            try
            {
                _msgHandler = new Win32WindowMessageHandler();

                SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;

                _currentVersions = LoadVersion(Path.Combine(_startupPath, "version.ini"));
            }
            catch { }

        }
        private void Update_Shown(object sender, EventArgs e)
        {
            this.Shown -= Update_Shown;

            //Tries connecting first, if that works then update.
            if (Connect())
                DoUpdate();
        }
        #endregion

        #region Connect
        private bool Connect()
        {
            bool connected = false;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                _sftp = new Sftp(_host, _userName, _password);
                _sftp.Connect(_port);

                GetFilesToUpdate();

                connected = true;
            }
            catch
            {
                MessageBox.Show("Failed to connect!\nAre your credentials correct?", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            this.Cursor = Cursors.Default;
            return connected;
        }
        private void Disconnect()
        {
            this.Cursor = Cursors.WaitCursor;

            if (!_updating || MessageBox.Show("Are you sure you want to disconnect while updating?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                _updating = false;
                try
                {
                    _sftp.Close();
                }
                catch { }
                _sftp = null;
                lvwUpdate.ClearEmbeddedControls();
                lvwUpdate.Items.Clear();

                string tempFolder = Path.Combine(_startupPath, "UpdateTempFiles");
                string tempVersion = Path.Combine(tempFolder, "version.ini");
                try
                {
                    if (File.Exists(tempVersion))
                        File.Delete(tempVersion);
                }
                catch { }
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Versioning
        /// <summary>
        /// Returns the versions of the files.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private List<string[]> LoadVersion(string versionControl)
        {
            bool filesFound = false;
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

                    if (line == "[FILES]")
                    {
                        filesFound = true;
                        continue;
                    }

                    if (filesFound)
                    {
                        string[] splittedLine = line.Split(':');
                        if (splittedLine.Length == 2)
                            fileVersions.Add(splittedLine);
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
        private void GetFilesToUpdate()
        {
            try
            {                
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

                string channelDir = _channel == 0 ? "stable" : "nightly";
                _sftp.Get(channelDir + "/version.ini", tempVersion);

                List<string[]> serverVersions = LoadVersion(tempVersion);

                foreach (string[] line in serverVersions)
                {
                    string localMD5Hash;
                    if (_force | !AlreadyVersioned(line, _currentVersions, out localMD5Hash))
                        {
                            ListViewItem lvwi = new ListViewItem(line[0]);
                            lvwi.SubItems.Add(localMD5Hash);
                            lvwi.SubItems.Add(line[1]);
                            lvwUpdate.Items.Add(lvwi);
                            lvwUpdate.AddEmbeddedControl(new ProgressBar(), lvwUpdate.Columns.Count - 1, lvwUpdate.Items.Count - 1);
                        }
                }
            }
            catch
            {
                MessageBox.Show("Failed to get the list of files needed to be versioned.\nThe update server is probably down.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool AlreadyVersioned(string[] entry, List<string[]> versioned, out string localMD5Hash)
        {
            localMD5Hash = string.Empty;
            foreach (string[] line in versioned)
            {
                List<bool> equals = new List<bool>(line.Length);
                for (int i = 0; i < line.Length; i++)
                    equals.Add(line[i] == entry[i]);

                if (equals[0] == true)
                    localMD5Hash = line[1];
                if (!equals.Contains(false))
                    return true;
            }

            return false;
        }
        #endregion

        #region Update or reinstall
        private void DoUpdate()
        {
            string tempFolder = Path.Combine(_startupPath, "UpdateTempFiles");
            _sftp.OnTransferProgress += new FileTransferEvent(_sftp_OnTransferProgress);
            _sftp.OnTransferEnd += new FileTransferEvent(_sftp_OnTransferEnd);
            try
            {
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

                    lvwi.Tag = tempFolder + lvwi.SubItems[0].Text;
                    toUpdate.Add(channelDir + lvwi.SubItems[0].Text.Replace('\\', '/'), lvwi.Tag as string);
                }

                Thread t = new Thread(delegate()
                {
                    try
                    {
                        foreach (string key in toUpdate.Keys)
                            _sftp.Get(key, toUpdate[key]);

                        SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                        {
                            _sftp.OnTransferProgress -= _sftp_OnTransferProgress;
                            _sftp.OnTransferEnd -= _sftp_OnTransferEnd;

                            OverwriteFiles();
                        }, null);
                    }
                    catch
                    {
                        try
                        {
                            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                            {
                                MessageBox.Show("Failed to update or reinstall.\nThe connection to the server was broken or the existing vApus files could not be overwritten.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }, null);
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
            catch
            {
                Disconnect();
                MessageBox.Show("Failed to update or reinstall.\nThe connection to the server was broken or the existing vApus files could not be overwritten.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        this.Close();
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
                    filename = _startupPath + lvwi.Text;
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
            }, null);
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
            }, null);
        }
        #endregion

        #region Other
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
                    try
                    {
                        _updating = false;
                        Disconnect();
                    }
                    catch { }
                else
                    e.Cancel = true;
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