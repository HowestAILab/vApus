/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tamir.SharpSsh;

namespace vApus.CommitTool {
    public class Commit {
        private static Commit _commit;

        private Commit() {
        }

        public static Commit GetInstance() {
            return _commit ?? (_commit = new Commit());
        }

        /// <summary>
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port">Default = 5222 for extern users, 22 for intern users</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="localGitRepository">The folder containing all the source files (and the .git folder).</param>
        /// <param name="exception"></param>
        /// <param name="gitCmd">eg C:\Program Files (x86)\Git\cmd\git.cmd</param>
        /// <param name="historyXml">The path to the history.xml</param>
        /// <param name="timeStamp"></param>
        /// <param name="excludedFilesOrFolders"></param>
        public void Do(string host, int port, string username, string password, string historyXml,
                       string localGitRepository, out Exception exception,
                       string gitCmd = @"C:\Program Files (x86)\Git\cmd\git.cmd",
                       string timeStamp = "",
                       params string[] excludedFilesOrFolders) {
            Sftp sftp;
            SshStream ssh;
            Connect(host, port, username, password, out sftp, out ssh, out exception);
            if (exception == null) {
                string channel;
                List<string> fileList, folderList;

                MakeVersionIni(historyXml, localGitRepository, gitCmd, timeStamp, excludedFilesOrFolders, out channel,
                               out fileList, out folderList, out exception);
                if (exception == null)
                    CommitBinaries(sftp, ssh, channel, fileList, folderList, out exception);
            }

            //This does a null check also.
            Disconnect(sftp, ssh);
        }

        #region Communication

        /// <summary>
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="sftp">For putting binaries</param>
        /// <param name="ssh">For removing foldersthat are not empty</param>
        /// <param name="exception"></param>
        private void Connect(string host, int port, string username, string password, out Sftp sftp, out SshStream ssh,
                             out Exception exception) {
            exception = null;
            sftp = null;
            ssh = null;
            try {
                if (port == 0)
                    port = 22;

                sftp = new Sftp(host, username, password);
                sftp.Connect(port);

                ssh = new SshStream(host, username, password);
            } catch (Exception ex) {
                Disconnect(sftp, ssh);
                exception = ex;
            }
        }

        /// <summary>
        ///     Does a null check, closes the connection, disposes the object.
        /// </summary>
        /// <param name="sftp"></param>
        /// <param name="ssh"></param>
        private void Disconnect(Sftp sftp, SshStream ssh) {
            if (sftp != null) {
                try {
                    sftp.Close();
                } catch {
                }
                sftp = null;
            }
            if (ssh != null) {
                try {
                    ssh.Close();
                } catch {
                }
                ssh = null;
            }
        }

        private void CommitBinaries(Sftp sftp, SshStream ssh, string channel, List<string> fileList,
                                    List<string> folderList, out Exception exception) {
            string channelDir = channel.ToLower();
            exception = null;
            try {
                try {
                    //Remake the channel dir for a clean start
                    ssh.Write("rm -rf " + channelDir);
                    ssh.ReadResponse();
                    //A bit sad, but otherwise the following command doesn't work (10 seconds should be okay, just being sure)
                    Thread.Sleep(60000);
                } catch {
                }
                try {
                    sftp.Mkdir(channelDir);
                } catch {
                }

                int startupPathLength = Application.StartupPath.Length;
                foreach (string folder in folderList)
                    try {
                        sftp.Mkdir(channelDir + folder.Substring(startupPathLength).Replace('\\', '/'));
                    } catch {
                    }
                foreach (string file in fileList)
                    try {
                        sftp.Put(file, channelDir + file.Substring(startupPathLength).Replace('\\', '/'));
                    } catch {
                    }
            } catch (Exception ex) {
                exception = ex;
            }
        }

        #endregion

        #region Make the version.ini and get files and folder to commit to the server

        private void MakeVersionIni(string historyXml, string localGitRepository, string gitCmd, string timeStamp,
                                    string[] excludedFilesOrFolders, out string channel, out List<string> fileList,
                                    out List<string> folderList, out Exception exception) {
            exception = null;
            channel = null;
            fileList = null;
            folderList = null;

            string version = GetVersion(localGitRepository, gitCmd, out exception);
            if (exception != null) return;

            if (timeStamp.Length != 0)
                version += " " + timeStamp;

            channel = GetChannel(localGitRepository, gitCmd, out exception);
            if (exception != null) return;

            string history = GetHistory(historyXml, out exception);
            if (exception != null) return;

            string files = GetFiles(excludedFilesOrFolders, out fileList, out folderList, out exception);
            if (exception != null) return;

            using (var sw = new StreamWriter(Path.Combine(Application.StartupPath, "version.ini"))) {
                sw.WriteLine("[VERSION]");
                sw.WriteLine(version);
                sw.WriteLine("[CHANNEL]");
                sw.WriteLine(channel);
                sw.WriteLine("[HISTORY]");
                sw.WriteLine(history);
                sw.WriteLine("[FILES]");
                sw.WriteLine(files);

                sw.Flush();
            }
        }

        /// <summary>
        ///     Gets a version from the git tags
        /// </summary>
        /// <param name="localGitRepository"></param>
        /// <param name="gitCmd">Don't forget to escape this</param>
        /// <param name="exception"></param>
        /// <returns>git tag (latest tag) </returns>
        private string GetVersion(string localGitRepository, string gitCmd, out Exception exception) {
            string version = null;
            try {
                exception = null;
                List<string> output = GetCMDOutput(localGitRepository,
                                                   "\"" + gitCmd + "\" describe \"--abbrev=0\" --tags");
                version = output[output.Count - 1];
                version = version.Replace('_', ' ');
            } catch (Exception ex) {
                exception = ex;
            }
            return version;
        }

        /// <summary>
        /// </summary>
        /// <param name="localGitRepository"></param>
        /// <param name="gitCmd"></param>
        /// <param name="exception"></param>
        /// <returns>If the branch equals "* development" "Stable" is returned, otherwise "Nightly".</returns>
        private string GetChannel(string localGitRepository, string gitCmd, out Exception exception) {
            string channel = null;
            try {
                exception = null;
                string input = "\"" + gitCmd + "\" branch";
                List<string> output = GetCMDOutput(localGitRepository, input);
                var branches = new List<string>();

                bool canAddToBranches = false;
                foreach (string line in output) {
                    if (canAddToBranches)
                        branches.Add(line);

                    if (line.Contains(input))
                        canAddToBranches = true;
                }

                channel = "Nightly";
                foreach (string branch in branches)
                    if (branch == "* master") {
                        channel = "Stable";
                        break;
                    }
            } catch (Exception ex) {
                exception = ex;
            }
            return channel;
        }

        /// <summary>
        /// </summary>
        /// <param name="exception"></param>
        /// <returns>The history from the history.xml in the start up folder of the commit tool.</returns>
        private string GetHistory(string historyXml, out Exception exception) {
            string history = null;
            try {
                exception = null;
                using (var sr = new StreamReader(historyXml))
                    history = sr.ReadToEnd();
            } catch (Exception ex) {
                exception = ex;
            }
            return history;
        }

        /// <summary>
        ///     For reading out the cmd output of the git bash.
        /// </summary>
        /// <param name="localGitRepository"></param>
        /// <param name="gitbash">eg "git branch"</param>
        /// <returns></returns>
        private List<string> GetCMDOutput(string localGitRepository, string gitbash) {
            var p = new Process();
            var output = new List<string>();
            bool killProcess = false;

            p.EnableRaisingEvents = true;
            p.StartInfo = new ProcessStartInfo("cmd") {
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            //The magic code
            //Using an anonymous delegate to handle the event, the process will be killed after the line after the input is received
            p.OutputDataReceived +=
                (s, e) => killProcess = AppendToOutput_DetermineInputFound(output, e.Data, gitbash, killProcess, p);
            p.Start();

            p.BeginOutputReadLine();

            p.StandardInput.WriteLine(localGitRepository.Split('\\')[0]);
            p.StandardInput.WriteLine("cd \"" + localGitRepository + "\"");
            p.StandardInput.WriteLine(gitbash);


            p.WaitForExit();
            p = null;

            return output;
        }

        private bool AppendToOutput_DetermineInputFound(List<string> output, string line, string input, bool killProcess,
                                                        Process p) {
            if (line == null)
                return false;

            output.Add(line);
            if (killProcess) {
                p.Kill();
                p.Dispose();
            }
            return line.Contains(input);
        }

        /// <summary>
        /// </summary>
        /// <param name="excludedFilesOrFolders"></param>
        /// <param name="fileList">All the files needed to be commit (absolute paths)</param>
        /// <param name="folderList">All the folders needed to be made serverside (absolute paths)</param>
        /// <param name="exception"></param>
        /// <returns>The files section of the version.ini</returns>
        private string GetFiles(string[] excludedFilesOrFolders, out List<string> fileList, out List<string> folderList,
                                out Exception exception) {
            exception = null;
            folderList = new List<string>();
            fileList = new List<string>();

            string files = null;
            try {
                var sb = new StringBuilder();
                using (var md5 = new MD5CryptoServiceProvider()) {
                    foreach (
                        string line in
                            GetFilesFromFolderFormatted(Application.StartupPath, Application.StartupPath,
                                                        excludedFilesOrFolders, md5, out fileList))
                        sb.AppendLine(line);

                    foreach (
                        string folder in
                            Directory.GetDirectories(Application.StartupPath, "*", SearchOption.AllDirectories))
                        if (!IsFileOrFolderExcluded(folder, excludedFilesOrFolders)) {
                            List<string> tempFileList;
                            foreach (
                                string line in
                                    GetFilesFromFolderFormatted(Application.StartupPath, folder, excludedFilesOrFolders,
                                                                md5, out tempFileList))
                                sb.AppendLine(line);
                            fileList.AddRange(tempFileList);
                            folderList.Add(folder);
                        }
                }
                //No hash for this, but it doesn't matter, this always needs to be updated.
                fileList.Add(Path.Combine(Application.StartupPath, "version.ini"));
                files = sb.ToString();
            } catch (Exception ex) {
                exception = ex;
            }
            return files;
        }

        /// <summary>
        /// </summary>
        /// <param name="absolutePartFolder">The startup path of the commit tool</param>
        /// <param name="folder">Where to get the files from</param>
        /// <param name="excludedFilesOrFolders"></param>
        /// <param name="fileList"></param>
        /// <returns>
        ///     The files section of the version.ini. List of relativefilename:md5hash,
        ///     version.ini, history.xml and vApus.CommitTool.Xml are automatically excluded in the version.ini text,
        ///     however version.ini will always be sent to the server.
        /// </returns>
        private List<string> GetFilesFromFolderFormatted(string absolutePartFolder, string folder,
                                                         string[] excludedFilesOrFolders, MD5CryptoServiceProvider md5,
                                                         out List<string> fileList) {
            var formatted = new List<string>();
            fileList = new List<string>();
            int absolutePartDirectoryLength = absolutePartFolder.Length;
            foreach (string fileName in Directory.GetFiles(folder, "*", SearchOption.TopDirectoryOnly)) {
                string shortFileName = fileName.Substring(absolutePartDirectoryLength);
                if (!IsFileOrFolderExcluded(shortFileName, excludedFilesOrFolders)) {
                    switch (shortFileName.ToLower()) {
                        case @"\version.ini":
                        case @"\history.xml":
                        case @"\vApus.CommitTool.exe":
                            break;
                        default:
                            fileList.Add(fileName);
                            formatted.Add(shortFileName + ":" + GetMD5HashFromFile(fileName, md5));
                            break;
                    }
                }
            }
            return formatted;
        }

        /// <summary>
        ///     Checks if the given file or folder can be added to the version.ini and can be commit to the server.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="excludedFilesOrFolders"></param>
        /// <returns></returns>
        private bool IsFileOrFolderExcluded(string name, string[] excludedFilesOrFolders) {
            string[] splittedName = name.Split('\\');
            name = splittedName[splittedName.Length - 1];
            foreach (string s in excludedFilesOrFolders)
                if ((s.StartsWith("*") && s.EndsWith("*") && name.Contains(s.Substring(1, s.Length - 2)))
                    || (s.StartsWith("*") && name.EndsWith(s.Substring(1)))
                    || (s.EndsWith("*") && name.StartsWith(s.Substring(0, s.Length - 1)))
                    || name.EndsWith(s, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private string GetMD5HashFromFile(string fileName, MD5CryptoServiceProvider md5) {
            byte[] retVal = md5.ComputeHash(File.ReadAllBytes(fileName));

            var sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
                sb.Append(retVal[i].ToString("x2"));
            return sb.ToString();
        }

        #endregion
    }
}