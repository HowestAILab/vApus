/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Tamir.SharpSsh;
using vApus.Util;

namespace vApus.CommitTool
{
    public partial class Commit : Form
    {
        #region Fields
        private Sftp _sftp;
        private int _previousCaretPosition;
        private bool _commiting;
        private List<string> _folders;
        #endregion

        #region Init
        public Commit()
        {
            InitializeComponent();
            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(Commit_HandleCreated);
        }
        private void Commit_HandleCreated(object sender, EventArgs e)
        {
            SetGui();
        }
        private void SetGui()
        {
            SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;

            string path = Path.Combine(Application.StartupPath, "vApus.CommitTool.config");
            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path);
                while (true)
                {
                    string line, line2;
                    if (sr.Peek() != -1)
                        line = sr.ReadLine().Trim();
                    else
                        break;
                    if (sr.Peek() != -1)
                        line2 = sr.ReadLine().Trim();
                    else
                        break;
                    switch (line)
                    {
                        case "Host:":
                            txtHost.Text = line2;
                            break;
                        case "Port:":
                            txtPort.Text = line2;
                            break;
                        case "Username:":
                            txtUsername.Text = line2;
                            break;
                        case "Exclude:":
                            rtxtExclude.Text = line2.Replace(";", Environment.NewLine).Trim();
                            break;
                    }
                }
            }
            path = Path.Combine(Application.StartupPath, "version.ini");
            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path);
                while (true)
                {
                    string line, line2;
                    if (sr.Peek() != -1)
                        line = sr.ReadLine().Trim();
                    else
                        break;
                    if (sr.Peek() != -1)
                        line2 = sr.ReadLine().Trim();
                    else
                        break;
                    switch (line)
                    {
                        case "Version:":
                            int i;
                            int.TryParse(line2, out i);
                            nudVersion.Value = i;
                            break;
                        case "HistoryOfChanges:":
                            rtxtHistoryOfChanges.Text = line2.Substring(9, line2.Length - 19);
                            break;
                    }
                }
            }
        }
        #endregion

        #region History management
        private void btnAddNewTitle_Click(object sender, EventArgs e)
        {
            InputDialog titleDialog = new InputDialog("Title:", "Add new title");
            titleDialog.SetInputLength(1, titleDialog.MaximumInputLength);
            if (titleDialog.ShowDialog() == DialogResult.OK)
            {
                InputDialog dateDialog = new InputDialog("Date:", "Set the date", DateTime.Now.ToString("dd-MM-yyyy"));
                dateDialog.SetInputLength(1, dateDialog.MaximumInputLength);
                if (dateDialog.ShowDialog() == DialogResult.OK)
                {
                    string s = string.Format("<t>{0}<d>{1}</d></t>", titleDialog.Input, dateDialog.Input);
                    int caretPosition = rtxtHistoryOfChanges.SelectionStart + s.Length;
                    rtxtHistoryOfChanges.Text = rtxtHistoryOfChanges.Text.Substring(0, rtxtHistoryOfChanges.SelectionStart) + s + rtxtHistoryOfChanges.Text.Substring(rtxtHistoryOfChanges.SelectionStart);
                    rtxtHistoryOfChanges.Select(caretPosition, 0);
                }
            }
        }
        private void btnAddNewItem_Click(object sender, EventArgs e)
        {
            InputDialog dialog = new InputDialog("Item:", "Add new item");
            dialog.SetInputLength(1, dialog.MaximumInputLength);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string s = string.Format("<i>{0}</i>", dialog.Input);
                int caretPosition = rtxtHistoryOfChanges.SelectionStart + s.Length;
                rtxtHistoryOfChanges.Text = rtxtHistoryOfChanges.Text.Substring(0, rtxtHistoryOfChanges.SelectionStart) + s + rtxtHistoryOfChanges.Text.Substring(rtxtHistoryOfChanges.SelectionStart);
                rtxtHistoryOfChanges.Select(caretPosition, 0);
            }
        }
        #endregion

        #region Connect
        private void _TextChanged(object sender, EventArgs e)
        {
            btnConnect.Enabled = txtHost.Text.Length > 0 && txtUsername.Text.Length > 0 && txtPassword.Text.Length > 0;
        }
        private void txtPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(e.KeyChar == '\b' || e.KeyChar.IsDigit());
        }
        private void txtPort_Leave(object sender, EventArgs e)
        {
            if (txtPort.Text.Length == 0)
                txtPort.Text = "5222";
        }
        private void _KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && btnConnect.Enabled == true && btnConnect.Text == "Connect")
                btnConnect_Click(this, null);
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            switch (btnConnect.Text)
            {
                case "Connect":
                    try
                    {
                        AppendLogLine("Connecting to \"" + txtUsername.Text + '@' + txtHost.Text + '\"' + txtPort.Text + '.', Color.Black);
                        _sftp = new Sftp(txtHost.Text, txtUsername.Text, txtPassword.Text);
                        _sftp.Connect(int.Parse(txtPort.Text));
                        pnlConnectTo.Enabled = false;
                        btnConnect.Text = "Disconnect";
                        tcCommit.SelectedIndex = 1;
                        AppendLogLine("Connected.", Color.Green);

                        GetFilesToCommit();
                    }
                    catch (Exception ex)
                    {
                        AppendLogLine("Failed to connect: " + ex.Message, Color.Red);
                    }

                    break;
                default:
                    if (_commiting && MessageBox.Show("Are you sure you want to disconnect while comitting?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                        return;
                    _commiting = false;
                    try
                    {
                        _sftp.Close();
                    }
                    catch { }
                    _sftp = null;
                    btnCommit.Enabled = false;
                    lvwCommit.ClearEmbeddedControls();
                    lvwCommit.Items.Clear();
                    pbTotal.Value = 0;
                    pbTotal.Tag = null;
                    pnlConnectTo.Enabled = true;

                    string tempFolder = Path.Combine(Application.StartupPath, "temp");
                    string tempVersion = Path.Combine(tempFolder, "tempversion.ini");
                    string currentVersion = Path.Combine(tempFolder, "version.ini");
                    try
                    {
                        if (File.Exists(tempVersion))
                            File.Delete(tempVersion);
                    }
                    catch { }


                    btnConnect.Text = "Connect";
                    AppendLogLine("Disconnected.", Color.Green);
                    break;
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Versioning
        private void GetFilesToCommit()
        {
            try
            {
                AppendLogLine("Getting the list of files needed to be commited.", Color.Green);

                //Get the version file from the server
                int startupPathLength = Application.StartupPath.Length + 1;
                string tempVersion = Path.Combine(Application.StartupPath, "tempversion.ini");
                List<string[]> serverVersions = new List<string[]>();
                List<string[]> currentVersions = new List<string[]>();
                bool versionINIAdded = false;
                _folders = new List<string>();
                //Get all the files from the directory to commit, exclude the ones in the exclude list.
                string[] exclude = rtxtExclude.Text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    if (File.Exists(tempVersion))
                        File.Delete(tempVersion);
                }
                catch { }
                try
                {
                    if (_sftp.GetFileList("vapus").Contains("version.ini"))
                        _sftp.Get("vapus/version.ini", tempVersion);
                }
                catch { }

                //All entries if any will be added to serverVersions.
                if (File.Exists(tempVersion))
                {
                    StreamReader srTVC = new StreamReader(tempVersion);
                    while (srTVC.Peek() != -1)
                    {
                        string[] line = srTVC.ReadLine().Split(',');
                        if (line.Length == 3)
                            serverVersions.Add(line);
                    }
                    try { srTVC.Close(); }
                    catch { }
                    try { srTVC.Dispose(); }
                    catch { }
                    srTVC = null;

                    try { File.Delete(tempVersion); }
                    catch { }
                }

                //Get all the files to be commited and the folders.
                foreach (string file in Directory.GetFiles(Application.StartupPath, "*", SearchOption.TopDirectoryOnly))
                {
                    string fileName = file.Substring(startupPathLength);
                    if (!IsFileOrFolderExcluded(fileName, exclude))
                    {
                        switch (fileName.ToLower())
                        {
                            case "version.ini":
                                currentVersions.Add(new string[] { fileName, string.Empty, string.Empty });
                                versionINIAdded = true;
                                break;
                            default:
                                currentVersions.Add(new string[] { fileName, File.GetLastWriteTime(file).ToString(), "1" });
                                break;
                        }
                    }
                }
                foreach (string folder in Directory.GetDirectories(Application.StartupPath, "*", SearchOption.AllDirectories))
                {
                    if (!IsFileOrFolderExcluded(folder, exclude))
                    {
                        foreach (string file in Directory.GetFiles(folder, "*", SearchOption.TopDirectoryOnly))
                        {
                            string fileName = file.Substring(startupPathLength);
                            if (!IsFileOrFolderExcluded(fileName, exclude))
                            {
                                switch (fileName.ToLower())
                                {
                                    case "version.ini":
                                        currentVersions.Add(new string[] { fileName, string.Empty, string.Empty });
                                        versionINIAdded = true;
                                        break;
                                    default:
                                        currentVersions.Add(new string[] { fileName, File.GetLastWriteTime(file).ToString(), "1" });
                                        break;
                                }
                            }
                        }
                        _folders.Add(folder.Substring(startupPathLength).Replace('\\', '/'));
                    }
                }


                foreach (string[] currentEntry in currentVersions)
                {
                    ListViewItem lvwi = new ListViewItem(currentEntry);
                    foreach (string[] serverEntry in serverVersions)
                        if (currentEntry[0] == serverEntry[0])
                        {
                            if (currentEntry[1] == serverEntry[1])
                                lvwi.SubItems[2].Text = serverEntry[2];
                            else
                                lvwi.SubItems[2].Text = (int.Parse(serverEntry[2]) + 1).ToString();
                            lvwi.SubItems.Add(serverEntry[2]);
                            break;
                        }
                    if (lvwi.SubItems.Count == 3)
                        lvwi.SubItems.Add("-1");
                    lvwCommit.Items.Add(lvwi);
                    lvwCommit.AddEmbeddedControl(new ProgressBar(), lvwCommit.Columns.Count - 1, lvwCommit.Items.Count - 1);
                }
                if (!versionINIAdded)
                {
                    lvwCommit.Items.Add(new ListViewItem("version.ini"));
                    lvwCommit.AddEmbeddedControl(new ProgressBar(), lvwCommit.Columns.Count - 1, lvwCommit.Items.Count - 1);
                }
                btnCommit.Enabled = lvwCommit.Items.Count > 0;

                AppendLogLine("Done.", Color.Green);
            }
            catch (Exception ex)
            {
                AppendLogLine("Failed to get the list of files needed to be commited: " + ex.Message, Color.Red);
            }
        }
        private bool IsFileOrFolderExcluded(string name, string[] exclude)
        {
            string[] splittedName = name.Split('\\');
            name = splittedName[splittedName.Length - 1];
            foreach (string s in exclude)
                if ((s.StartsWith("*") && s.EndsWith("*") && name.Contains(s.Substring(1, s.Length - 2)))
                    || (s.StartsWith("*") && name.EndsWith(s.Substring(1)))
                    || (s.EndsWith("*") && name.StartsWith(s.Substring(0, s.Length - 1)))
                    || name.EndsWith(s, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            return false;
        }
        #endregion

        #region Commit
        private void btnCommit_Click(object sender, EventArgs e)
        {
            _commiting = true;
            AppendLogLine("Commiting started.", Color.Black);
            tcCommit.SelectedIndex = 1;

            //Writing the config
            StreamWriter sw = new StreamWriter(Path.Combine(Application.StartupPath, "vApus.CommitTool.config"));
            sw.WriteLine("Host:");
            sw.WriteLine(txtHost.Text);
            sw.WriteLine("Port:");
            sw.WriteLine(txtPort.Text);
            sw.WriteLine("Username:");
            sw.WriteLine(txtUsername.Text);
            sw.WriteLine("Exclude:");
            foreach (string s in rtxtExclude.Text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                sw.Write(s);
                sw.Write(';');
            }
            try { sw.Close(); }
            catch { }
            try { sw.Dispose(); }
            catch { }

            foreach (string s in _folders)
                try
                {
                    _sftp.Mkdir("vapus/" + s);
                }
                catch { }

            _sftp.OnTransferProgress += new FileTransferEvent(_sftp_OnTransferProgress);
            _sftp.OnTransferEnd += new FileTransferEvent(_sftp_OnTransferEnd);

            try
            {
                btnCommit.Enabled = false;
                nudVersion.Enabled = false;
                rtxtHistoryOfChanges.ReadOnly = true;
                btnAddNewTitle.Enabled = false;
                btnAddNewItem.Enabled = false;

                Dictionary<string, string> toCommit = new Dictionary<string, string>(lvwCommit.Items.Count);
                foreach (ListViewItem lvwi in lvwCommit.Items)
                {
                    lvwi.Tag = Path.Combine(Application.StartupPath, lvwi.SubItems[0].Text);
                    if (chkCommitAll.Checked || lvwi.SubItems[2].Text != lvwi.SubItems[3].Text)
                        toCommit.Add(lvwi.Tag as string, "vapus/" + lvwi.SubItems[0].Text.Replace('\\', '/'));
                }

                Thread t = new Thread(delegate()
                {
                    try
                    {
                        foreach (string key in toCommit.Keys)
                            _sftp.Put(key, toCommit[key]);

                        SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                        {
                            string tempVersion = Path.Combine(Application.StartupPath, "tempversion.ini");
                            string currentVersion = Path.Combine(Application.StartupPath, "version.ini");

                            try
                            {
                                if (File.Exists(tempVersion))
                                    File.Delete(tempVersion);
                            }
                            catch { }
                            _sftp.OnTransferProgress -= _sftp_OnTransferProgress;
                            _sftp.OnTransferEnd -= _sftp_OnTransferEnd;

                            AppendLogLine("Completed!", Color.Green);
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
                        try
                        {
                            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                            {
                                nudVersion.Enabled = true;
                                rtxtHistoryOfChanges.ReadOnly = false;
                                btnAddNewTitle.Enabled = true;
                                btnAddNewItem.Enabled = true;
                            }, nudVersion);
                            _commiting = false;
                        }
                        catch { }
                    }
                });
                t.Start();
            }
            catch (Exception ex)
            {
                btnConnect_Click(this, null);
                AppendLogLine("Failed to update or reinstall: " + ex.Message, Color.Red);
            }
            //Writing the version ini
            sw = new StreamWriter(Path.Combine(Application.StartupPath, "version.ini"));
            sw.WriteLine("Version:");
            sw.WriteLine(nudVersion.Value);
            sw.WriteLine("HistoryOfChanges:");
            sw.WriteLine(string.Format("<history>{0}</history>", rtxtHistoryOfChanges.Text));
            sw.WriteLine("Files:");

            foreach (ListViewItem lvwi in lvwCommit.Items)
                if (!lvwi.SubItems[0].Text.Equals("version.ini", StringComparison.CurrentCultureIgnoreCase))
                    sw.WriteLine(lvwi.SubItems[0].Text + ',' + lvwi.SubItems[1].Text + ',' + lvwi.SubItems[2].Text);

            sw.Flush();
            try { sw.Close(); }
            catch { }
            try { sw.Dispose(); }
            catch { }
            sw = null;
        }

        private void _sftp_OnTransferEnd(string src, string dst, int transferredBytes, int totalBytes, string message)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                for (int i = 0; i < lvwCommit.Items.Count; i++)
                    if (lvwCommit.Items[i].Tag as string == src)
                    {
                        lvwCommit.Items[i].Selected = true;
                        lvwCommit.SelectedItems[0].EnsureVisible();
                        ProgressBar pb = lvwCommit.EmbeddedControls[i] as ProgressBar;
                        pb.Value = pb.Maximum;
                        break;
                    }
                float total;
                if (pbTotal.Tag == null)
                    total = (float)pbTotal.Maximum / lvwCommit.Items.Count;
                else
                    total = (float)pbTotal.Tag + (float)pbTotal.Maximum / lvwCommit.Items.Count;

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
                for (int i = 0; i < lvwCommit.Items.Count; i++)
                    if (lvwCommit.Items[i].Tag as string == src)
                    {
                        lvwCommit.Items[i].Selected = true;
                        lvwCommit.SelectedItems[0].EnsureVisible();
                        ProgressBar pb = lvwCommit.EmbeddedControls[i] as ProgressBar;
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
        private void Commit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_commiting)
            {
                if (MessageBox.Show("Are you sure you want to disconnect while commiting?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    _commiting = false;
                    btnConnect_Click(this, null);
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
            }
        }
        #endregion
    }
}
