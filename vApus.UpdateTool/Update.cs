/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Renci.SshNet;
using vApus.Util;

namespace vApus.UpdateTool {
    public partial class Update : Form {

        #region Fields
        private const string UPDATETEMPFILESDIR = "__UpdateTempFiles__";

        private const int MAXRETRY = 3;
        private readonly int _channel;
        private readonly bool _force; //Update all files regardesly that they must or must not be updated
        private readonly bool _silent; //if no errors occur, For smart update
        private readonly string _solution;

        /// <summary>
        /// To auto connect.
        /// </summary>
        private readonly string _host;

        /// <summary>
        /// To auto connect.
        /// </summary>
        private readonly string _privateRSAKeyPath;

        private readonly int _port = 22; //External port 5222
        private readonly string _startupPath;

        /// <summary>
        /// To auto connect.
        /// </summary>
        private readonly string _userName;

        private List<string[]> _currentVersions;
        private Win32WindowMessageHandler _msgHandler;
        private SftpClient _sftp;
        private bool _updating;

        #endregion

        #region Init

        /// <summary>
        /// </summary>
        /// <param name="args">If contains GUID, host, port, username, privateRSAKeyPath in that order it will auto connect.</param>
        public Update(string[] args) {
            InitializeComponent();

            _startupPath = Directory.GetParent(Application.StartupPath).FullName;

            if (args.Length > 7) {
                _host = args[1];
                _port = int.Parse(args[2]);
                _userName = args[3];
                _privateRSAKeyPath = args[4].Replace('_', ' ');
                _channel = int.Parse(args[5]);
                _force = bool.Parse(args[6]);
                _silent = bool.Parse(args[7]);

                if (args.Length == 9)
                    _solution = args[8].Trim();
            }

            HandleCreated += Update_HandleCreated;

            if (_host != null)
                Shown += Update_Shown;
        }

        private void Update_HandleCreated(object sender, EventArgs e) {
            try {
                _msgHandler = new Win32WindowMessageHandler();

                SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;

                _currentVersions = LoadVersion(Path.Combine(_startupPath, "version.ini"));
            }
            catch {
            }
        }

        private void Update_Shown(object sender, EventArgs e) {
            Shown -= Update_Shown;

            //Tries connecting first, if that works then update.
            if (Connect())
                DoUpdate();
        }

        #endregion

        #region Connect

        private bool Connect() {
            bool connected = false;
            Cursor = Cursors.WaitCursor;

            try {
                _sftp = new SftpClient(_host, _port, _userName, new PrivateKeyFile(_privateRSAKeyPath));
                _sftp.Connect();

                GetFilesToUpdate();

                connected = true;
            }
            catch (Exception ex) {
                MessageBox.Show("Failed to connect!\nAre your credentials correct?", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Cursor = Cursors.Default;
            return connected;
        }

        private void Disconnect() {
            Cursor = Cursors.WaitCursor;

            if (!_updating ||
                MessageBox.Show("Are you sure you want to disconnect while updating?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
                _updating = false;
                try { _sftp.Dispose(); } catch { }
                _sftp = null;
                lvwUpdate.ClearEmbeddedControls();
                lvwUpdate.Items.Clear();

                string tempFolder = Path.Combine(_startupPath, UPDATETEMPFILESDIR);
                string tempVersion = Path.Combine(tempFolder, "version.ini");
                try {
                    if (File.Exists(tempVersion))
                        File.Delete(tempVersion);
                }
                catch {
                }
            }
            Cursor = Cursors.Default;
        }

        #endregion

        #region Versioning

        /// <summary>
        ///     Returns the versions of the files.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private List<string[]> LoadVersion(string versionControl) {
            bool filesFound = false;
            var fileVersions = new List<string[]>();
            string line = string.Empty;

            if (File.Exists(versionControl)) {
                var sr = new StreamReader(versionControl);
                while (sr.Peek() != -1) {
                    line = sr.ReadLine().Trim();

                    if (line.Length == 0)
                        continue;

                    if (line == "[FILES]") {
                        filesFound = true;
                        continue;
                    }

                    if (filesFound) {
                        string[] splittedLine = line.Split(':');
                        if (splittedLine.Length == 2)
                            fileVersions.Add(splittedLine);
                    }
                }

                try { sr.Close(); } catch { }
                try { sr.Dispose(); } catch { }
                sr = null;
            }
            return fileVersions;
        }

        private void GetFilesToUpdate() {
            try {
                string tempFolder = Path.Combine(_startupPath, UPDATETEMPFILESDIR);
                if (Directory.Exists(tempFolder) &&
                    Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories).Length == 0)
                    Directory.Delete(tempFolder, true);

                Directory.CreateDirectory(tempFolder);
                
                string tempVersionPath = Path.Combine(tempFolder, "version.ini");

                string channelDir = _channel == 0 ? "stable" : "nightly";

                using (var str = File.Create(tempVersionPath))
                    _sftp.DownloadFile("vApusUpdate/" + channelDir + "/version.ini", str);

                List<string[]> serverVersions = LoadVersion(tempVersionPath);

                foreach (var line in serverVersions) {
                    string localMD5Hash;
                    if (_force | !AlreadyVersioned(line, _currentVersions, out localMD5Hash)) {
                        var lvwi = new ListViewItem(line[0]);
                        lvwi.SubItems.Add(localMD5Hash);
                        lvwi.SubItems.Add(line[1]);
                        lvwUpdate.Items.Add(lvwi);
                        lvwUpdate.AddEmbeddedControl(new ProgressBar(), lvwUpdate.Columns.Count - 1, lvwUpdate.Items.Count - 1);
                    }
                }
            }
            catch {
                MessageBox.Show("Failed to get the list of files needed to be versioned.\nThe update server is probably down.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool AlreadyVersioned(string[] entry, List<string[]> versioned, out string localMD5Hash) {
            localMD5Hash = string.Empty;
            foreach (var line in versioned) {
                var equals = new List<bool>(line.Length);
                for (int i = 0; i < line.Length; i++)
                    equals.Add(line[i] == entry[i]);

                if (@equals[0])
                    localMD5Hash = line[1];
                if (!equals.Contains(false))
                    return true;
            }

            return false;
        }

        #endregion

        #region Update or reinstall

        private void DoUpdate() {
            string exception = "Failed to update or reinstall.\nThe connection to the server was broken or the existing vApus files could not be overwritten.";
            string tempFolder = Path.Combine(_startupPath, UPDATETEMPFILESDIR);

            try {
                string possibleNonExistingFolder;

                string channelDir = _channel == 0 ? "stable" : "nightly";

                var toUpdate = new Dictionary<string, string>(lvwUpdate.Items.Count);
                foreach (ListViewItem lvwi in lvwUpdate.Items) {
                    possibleNonExistingFolder = tempFolder;
                    string[] splittedPath = lvwi.SubItems[0].Text.Split('\\');
                    for (int i = 0; i < splittedPath.Length - 1; i++)
                        possibleNonExistingFolder = Path.Combine(possibleNonExistingFolder, splittedPath[i]);
                    if (!Directory.Exists(possibleNonExistingFolder))
                        Directory.CreateDirectory(possibleNonExistingFolder);

                    lvwi.Tag = tempFolder + lvwi.SubItems[0].Text;
                    toUpdate.Add("vApusUpdate/" + channelDir + lvwi.SubItems[0].Text.Replace('\\', '/'), lvwi.Tag as string);
                }

                var t = new Thread(delegate () {
                    try {
                        foreach (string source in toUpdate.Keys) //Download from source to destination and show it on the GUI.
                            using (var str = File.Create(toUpdate[source])) {
                                string destination = toUpdate[source];
                                FileDownLoadStarted(destination);
                                _sftp.DownloadFile(source, str);
                                FileDownLoadFinished(destination);
                            }

                        SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                            OverwriteFiles();
                        }, null);
                    }
                    catch {
                        try {
                            SynchronizationContextWrapper.SynchronizationContext.Send((state) => { MessageBox.Show(exception, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error); }, null);
                        }
                        catch {
                        }
                    }
                    finally {
                        _updating = false;
                    }
                });
                t.Start();
            }
            catch {
                Disconnect();
                MessageBox.Show(exception, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        async private void OverwriteFiles() {
            Enabled = false;
            try {
                if (Process.GetProcessesByName("vApus").Length != 0) {
                    string message = "vApus will now be updated, click 'Yes' to close all running instances.";
                    if (_silent || MessageBox.Show(message, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
                        int retry = 0;
                        TopMost = false;

                        Retry:
                        foreach (Process p in Process.GetProcessesByName("vApus"))
                            await Task.Run(() => {
                                try {
                                    if (retry == 0)
                                        //Send a close message to the process, that way a solution can be saved before vApus is closed.
                                        Win32WindowMessageHandler.PostMessage(p.MainWindowHandle, 16, IntPtr.Zero, (IntPtr)1); //WM_CLOSE
                                    else
                                        p.Kill();
                                    p.WaitForExit(120000);
                                }
                                catch {
                                }
                            });
                        if (Process.GetProcessesByName("vApus").Length != 0)
                            if (++retry <= MAXRETRY) {
                                Thread.Sleep(1000 * retry);
                                goto Retry;
                            }
                            else {
                                if (MessageBox.Show("Something went wrong when trying to close one or more instances of vApus.\nClick 'Yes' to try again.",
                                    string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
                                    retry = 0;
                                    goto Retry;
                                }
                            }

                        TopMost = true;
                    }
                    else {
                        Close();
                        return;
                    }
                }

                //Kill other tools
                Parallel.ForEach(Process.GetProcesses(), (p) => {
                    try {
                        if (p != null && (p.ProcessName == "vApus.JumpStart" || p.ProcessName == "vApus.PublishItemsHandler" || p.ProcessName == "vApus.DetailedResultsViewer" || p.ProcessName == "Lupus-Titanium_GUI"))
                            p.Kill();
                    }
                    catch { }
                });

                //Delete all the files that must be replaced by updated ones.
                string filename;
                foreach (ListViewItem lvwi in lvwUpdate.Items) {
                    filename = _startupPath + lvwi.Text;
                    if (File.Exists(filename)) {
                        int sleepTime = 0;
                        while (File.Exists(filename) && File.GetAttributes(filename) == FileAttributes.ReadOnly && sleepTime++ < 3000)
                            Thread.Sleep(1);
                        Exception ex = null;
                        while (File.Exists(filename) && sleepTime++ < 6000) {
                            try {
                                File.Delete(filename);
                            }
                            catch (Exception e) {
                                ex = e;
                                Thread.Sleep(1);
                            }
                        }
                        if (File.Exists(filename)) {
                            // MessageBox.Show(filename + "\n\n" + ex);
                            throw ex;
                        }
                    }
                }

                //Place files from the temp folder in the vApus folder.
                string tempFolder = Path.Combine(_startupPath, UPDATETEMPFILESDIR);
                TryOverwriteDirectoriesAndFiles(tempFolder, _startupPath);

                foreach (string d in Directory.GetDirectories(_startupPath, "*", SearchOption.AllDirectories))
                    try {
                        if (Directory.GetFiles(d).Length == 0 && Directory.GetDirectories(d).Length == 0)
                            Directory.Delete(d);
                    }
                    catch { }

                if (!Directory.Exists(tempFolder)) {
                    if (_silent || MessageBox.Show("Do you want to start vApus now?", "Updated!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        if (string.IsNullOrWhiteSpace(_solution) || !File.Exists(_solution))
                            Process.Start(Path.Combine(_startupPath, "vApus.exe"));
                        else
                            Process.Start(Path.Combine(_startupPath, "vApus.exe"), "\"" + _solution + "\"");
                }
                else if (Directory.Exists(tempFolder) &&
                         Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories).Length == 0) {
                    Directory.Delete(tempFolder, true);
                    if (_silent || MessageBox.Show("Do you want to start vApus now?", "Updated!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        if (string.IsNullOrWhiteSpace(_solution) || !File.Exists(_solution))
                            Process.Start(Path.Combine(_startupPath, "vApus.exe"));
                        else
                            Process.Start(Path.Combine(_startupPath, "vApus.exe"), "\"" + _solution + "\"");
                }
                else {
                    MessageBox.Show(
                        "Not all files where updated due to access or authorization errors!\nThose files are stored in the '__UpdateTempFiles__' folder located in the top directory of vApus, so you can put them at the right place manually.",
                        "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
            catch {
                throw;
            }
            finally {
                //Start JumpStart
                try {
                    string jumpStartPath = Path.Combine(_startupPath, "vApus.JumpStart.exe");
                    if (File.Exists(jumpStartPath))
                        Process.Start(jumpStartPath);
                }
                catch {
                }
                Enabled = true;
                _updating = false;
            }
            Close();
        }

        private void TryOverwriteDirectoriesAndFiles(string from, string to) {
            string[] splittedFilename;
            foreach (string f in Directory.GetFiles(from)) {
                try {
                    splittedFilename = f.Split('\\');
                    File.Copy(f, Path.Combine(to, splittedFilename[splittedFilename.Length - 1]), true);
                    File.Delete(f);
                }
                catch {
                }
            }
            string[] splittedDirectoryName;
            string directoryName, newFrom, newTo;
            foreach (string d in Directory.GetDirectories(from)) {
                splittedDirectoryName = d.Split('\\');
                directoryName = splittedDirectoryName[splittedDirectoryName.Length - 1];
                newFrom = Path.Combine(from, directoryName);
                newTo = Path.Combine(to, directoryName);

                if (!Directory.Exists(newTo))
                    Directory.CreateDirectory(newTo);
                TryOverwriteDirectoriesAndFiles(newFrom, newTo);
            }
        }

        private void FileDownLoadStarted(string destination) {
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                for (int i = 0; i < lvwUpdate.Items.Count; i++)
                    if (lvwUpdate.Items[i].Tag as string == destination) {
                        lvwUpdate.Items[i].Selected = true;
                        lvwUpdate.SelectedItems[0].EnsureVisible();
                        var pb = lvwUpdate.EmbeddedControls[i] as ProgressBar;
                        pb.Style = ProgressBarStyle.Marquee;
                        break;
                    }
            }, null);
        }
        private void FileDownLoadFinished(string destination) {
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                for (int i = 0; i < lvwUpdate.Items.Count; i++)
                    if (lvwUpdate.Items[i].Tag as string == destination) {
                        lvwUpdate.Items[i].Selected = true;
                        lvwUpdate.SelectedItems[0].EnsureVisible();
                        var pb = lvwUpdate.EmbeddedControls[i] as ProgressBar;
                        pb.Style = ProgressBarStyle.Continuous;
                        pb.Value = pb.Maximum;
                        break;
                    }
            }, null);
        }
        #endregion

        #region Other

        protected override void WndProc(ref Message m) {
            if (_msgHandler != null && m.Msg == _msgHandler.WINDOW_MSG) {
                TopMost = true;
                TopMost = false;
                Activate();
            }
            base.WndProc(ref m);
        }

        private void Update_FormClosing(object sender, FormClosingEventArgs e) {
            if (_updating) {
                if (MessageBox.Show("Are you sure you want to disconnect while updating?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
                    try {
                        _updating = false;
                        Disconnect();
                    }
                    catch { }
                }
                else {
                    e.Cancel = true;
                }
            }
            else if (_sftp != null) {
                try { _sftp.Dispose(); } catch { }
                _sftp = null;
                string tempFolder = Path.Combine(_startupPath, UPDATETEMPFILESDIR);
                if (Directory.Exists(tempFolder) && Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories).Length == 0)
                    Directory.Delete(tempFolder, true);
            }
        }

        #endregion
    }
}