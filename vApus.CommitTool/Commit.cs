using System;
using System.Collections.Generic;
using System.Text;
using Tamir.SharpSsh;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Windows.Forms;

namespace vApus.CommitTool
{
    public class Commit
    {
        #region Fields
        private List<string> _folders;
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port">Default = 5222 for extern users, 22 for intern users</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="localGitRepository">The folder containing all the source files (and the .git folder).</param>
        /// <param name="gitCmd">eg C:\Program Files (x86)\Git\cmd\git.cmd</param>
        /// <param name="excludedFilesOrFolders"></param>
        public Commit(string host, int port, string username, string password,
            string localGitRepository, string gitCmd = @"C:\Program Files (x86)\Git\cmd\git.cmd",
            params string[] excludedFilesOrFolders)
        {
            Exception exception;
            Sftp sftp = Connect(host, port, username, password, out exception);
            if (exception == null)
            {
                MakeVersionIni(localGitRepository, gitCmd, excludedFilesOrFolders, out exception);
                if (exception == null)
                {
                    CommitBinaries(out exception);
                }
            }

            if (sftp != null)
            {
                try { sftp.Close(); }
                catch { }
                sftp = null;
            }
        }

        private Sftp Connect(string host, int port, string username, string password, out Exception exception)
        {
            exception = null;
            Sftp sftp = null;
            try
            {
                if (port == 0)
                    port = 22;

                sftp = new Sftp(host, username, password);
                sftp.Connect(port);
            }
            catch (Exception ex)
            {
                try { sftp.Close(); }
                catch { }
                sftp = null;
                exception = ex;
            }
            return sftp;
        }

        private void MakeVersionIni(string localGitRepository, string gitCmd,
            string[] excludedFilesOrFolders, out Exception exception)
        {
            exception = null;
            string version = GetVersion(localGitRepository, gitCmd, out exception);
            if (exception != null) return;

            string channel = GetChannel(localGitRepository, gitCmd, out exception);
            if (exception != null) return;

            string history = GetHistory(localGitRepository, gitCmd, out exception);
            if (exception != null) return;

            List<string> fileList, folderList;
            string files = GetFiles(excludedFilesOrFolders, out fileList, out folderList, out exception);
            if (exception != null) return;

            using (var sw = new StreamWriter(Path.Combine(Application.StartupPath, "version.ini")))
            {
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
        /// Gets a version from the git tags
        /// </summary>
        /// <param name="localGitRepository"></param>
        /// <param name="gitCmd"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private string GetVersion(string localGitRepository, string gitCmd, out Exception exception)
        {
            string version = null;
            try
            {
                exception = null;
                version = GetCMDOutput(localGitRepository, gitCmd + "describe");
            }
            catch (Exception ex) { exception = ex; }
            return version;
        }
        private string GetChannel(string localGitRepository, string gitCmd, out Exception exception)
        {
            string channel = null;
            try
            {
                exception = null;
                string branches = GetCMDOutput(localGitRepository, gitCmd + "branch");

                channel = "NIGHTLY";
                string[] split = branches.Split('\n', '\r');
                foreach (string branch in split)
                    if (branch == "* master")
                    {
                        channel = "STABLE";
                        break;
                    }
            }
            catch (Exception ex) { exception = ex; }
            return channel;
        }
        private string GetHistory(string localGitRepository, string gitCmd, out Exception exception)
        {
            string history = null;
            try
            {
                exception = null;
                history = GetCMDOutput(localGitRepository, gitCmd + "log");
            }
            catch (Exception ex) { exception = ex; }
            return history;
        }
        private string GetCMDOutput(string localGitRepository, string gitbash)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("cmd", "/K cd \"" + localGitRepository + "\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            p.Start();

            string foo = p.StandardOutput.ReadToEnd();

            p.StandardInput.WriteLine(gitbash);
            string output = p.StandardOutput.ReadToEnd();

            p.Close();
            p = null;

            return output;
        }
        private string GetFiles(string[] excludedFilesOrFolders, out List<string> fileList, out List<string> folderList, out Exception exception)
        {
            exception = null;
            folderList = new List<string>();
            fileList = new List<string>();

            string files = null;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[FILES]");

                foreach (string line in GetFilesFromFolderFormatted(Application.StartupPath, Application.StartupPath, excludedFilesOrFolders, out fileList))
                    sb.AppendLine(line);

                foreach (string folder in Directory.GetDirectories(Application.StartupPath, "*", SearchOption.AllDirectories))
                    if (!IsFileOrFolderExcluded(folder, excludedFilesOrFolders))
                    {
                        List<string> tempFileList;
                        foreach (string line in GetFilesFromFolderFormatted(Application.StartupPath, folder, excludedFilesOrFolders, out tempFileList))
                            sb.AppendLine(line);
                        fileList.AddRange(tempFileList);
                        folderList.Add(folder);
                    }

                sb.AppendLine("version.ini:\\");
                files = sb.ToString();
            }
            catch (Exception ex) { exception = ex; }
            return files;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="absolutePartFolder"></param>
        /// <param name="folder"></param>
        /// <param name="excludedFilesOrFolders"></param>
        /// <param name="fileList"></param>
        /// <returns> List of relativefilename:md5hash</returns>
        private List<string> GetFilesFromFolderFormatted(string absolutePartFolder, string folder, string[] excludedFilesOrFolders, out List<string> fileList)
        {
            List<string> formatted = new List<string>();
            fileList = new List<string>();
            int absolutePartDirectoryLength = absolutePartFolder.Length;
            foreach (string fileName in Directory.GetFiles(folder, "*", SearchOption.TopDirectoryOnly))
            {
                fileList.Add(fileName);
                string shortFileName = fileName.Substring(absolutePartDirectoryLength);
                if (!IsFileOrFolderExcluded(shortFileName, excludedFilesOrFolders))
                {
                    switch (shortFileName.ToLower())
                    {
                        case "version.ini":
                        case "vApus.CommitTool.exe":
                            break;
                        default:
                            formatted.Add(shortFileName + ":" + GetMD5HashFromFile(fileName));
                            break;
                    }
                }
            }
            return formatted;
        }
        private bool IsFileOrFolderExcluded(string name, string[] excludedFilesOrFolders)
        {
            string[] splittedName = name.Split('\\');
            name = splittedName[splittedName.Length - 1];
            foreach (string s in excludedFilesOrFolders)
                if ((s.StartsWith("*") && s.EndsWith("*") && name.Contains(s.Substring(1, s.Length - 2)))
                    || (s.StartsWith("*") && name.EndsWith(s.Substring(1)))
                    || (s.EndsWith("*") && name.StartsWith(s.Substring(0, s.Length - 1)))
                    || name.EndsWith(s, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            return false;
        }
        private string GetMD5HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
                sb.Append(retVal[i].ToString("x2"));
            return sb.ToString();
        }

        private void CommitBinaries(List<string> fileList, List<string> folderList, out Exception exception)
        {
            exception = null;
            try { }
            catch (Exception ex) { exception = ex; }
        }

    }
}
