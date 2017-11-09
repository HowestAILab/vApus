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
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace vApus.CommitTool {
    public class Commit {
        private static Commit _commit;

        private static string _updateTempFilesPath;

        private Commit() {
            _updateTempFilesPath = Path.Combine(Application.StartupPath, "UpdateTempFiles");
        }

        public static Commit GetInstance() {
            return _commit ?? (_commit = new Commit());
        }

        /// <summary>
        /// All files are stored in a folder UpdateTempFiles, together with a version.ini for version checking. Those files should be copied over to a remote location by hand or using a CI service. 
        /// </summary>
        /// <param name="localGitRepository">The folder containing all the source files (and the .git folder).</param>
        /// <param name="gitCmd">eg C:\Program Files (x86)\Git\cmd\git.cmd</param>
        /// <param name="historyXml">The path to the history.xml</param>
        /// <param name="timeStamp">Give the timestamp that is appended to the package name that all files are put into, if Any. For example, if you zip the files and put them elsewhere, or make an installer. Then the time stamp is the same everywhere.</param>
        /// <param name="excludedFilesOrFolders"></param>
        public Exception Do(string historyXml, string localGitRepository, string gitCmd, string timeStamp, params string[] excludedFilesOrFolders) {
            Exception exception = null;

            try {
                string version = GetVersion(localGitRepository, gitCmd);

                if (timeStamp.Length != 0)
                    version += " " + timeStamp;

                string channel = GetChannel(localGitRepository, gitCmd);

                string history = GetHistory(historyXml);

                string files = StoreAndGetFiles(excludedFilesOrFolders);

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
            catch (Exception ex) {
                exception = ex;
            }

            return exception;
        }

        /// <summary>
        ///     Gets a version from the git tags
        /// </summary>
        /// <param name="localGitRepository"></param>
        /// <param name="gitCmd"></param>
        /// <returns>git tag (latest tag) </returns>
        private string GetVersion(string localGitRepository, string gitCmd) {
            List<string> output = GetCMDOutput(localGitRepository,
                                               "\"" + gitCmd + "\" describe \"--abbrev=0\" --tags");
            string version = output[output.Count - 1];
            version = version.Replace('_', ' ');

            return version;
        }

        /// <summary>
        /// </summary>
        /// <param name="localGitRepository"></param>
        /// <param name="gitCmd"></param>
        /// <returns>If the branch equals "* stable2" "Stable" is returned, otherwise "Nightly".</returns>
        private string GetChannel(string localGitRepository, string gitCmd) {
            string channel = null;
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
                if (branch == "* stable2") {
                    channel = "Stable";
                    break;
                }

            return channel;
        }

        /// <summary>
        /// </summary>
        /// <returns>The history from the history.xml in the start up folder of the commit tool.</returns>
        private string GetHistory(string historyXml) {
            string history = null;
            using (var sr = new StreamReader(historyXml))
                history = sr.ReadToEnd();
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

        private bool AppendToOutput_DetermineInputFound(List<string> output, string line, string input, bool killProcess, Process p) {
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
        /// Stores files in a folder UpdateTempFiles
        /// </summary>
        /// <param name="excludedFilesOrFolders"></param>
        /// <returns>The files section of the version.ini: short filename + md5 hash.</returns>
        private string StoreAndGetFiles(string[] excludedFilesOrFolders) {
            Directory.CreateDirectory(_updateTempFilesPath);

            var sb = new StringBuilder();
            using (var md5 = new MD5CryptoServiceProvider()) {
                foreach (string line in StoreAndGetFilesFromFolderFormatted(Application.StartupPath, Application.StartupPath, excludedFilesOrFolders, md5))
                    sb.AppendLine(line);

                foreach (string folder in Directory.GetDirectories(Application.StartupPath, "*", SearchOption.AllDirectories))
                    if (!IsFileOrFolderExcluded(folder, excludedFilesOrFolders))                        
                        foreach (string line in StoreAndGetFilesFromFolderFormatted(Application.StartupPath, folder, excludedFilesOrFolders, md5))
                            sb.AppendLine(line);
            }

            return sb.ToString();
        }

        /// <summary>
        /// </summary>
        /// <param name="absolutePartFolder">The startup path of the commit tool</param>
        /// <param name="folder">Where to get the files from</param>
        /// <param name="excludedFilesOrFolders"></param>
        /// <param name="md5"></param>
        /// <param name="currentDirectory"></param>
        /// <returns>
        /// The files section of the version.ini. List of relativefilename:md5hash,
        /// version.ini, history.xml and vApus.CommitTool.Xml are automatically excluded in the version.ini text,
        /// however version.ini will always be sent to the server.
        /// </returns>
        private List<string> StoreAndGetFilesFromFolderFormatted(string absolutePartFolder, string folder, string[] excludedFilesOrFolders, MD5CryptoServiceProvider md5) {
            var formatted = new List<string>();

            int absolutePartDirectoryLength = absolutePartFolder.Length;

            //Create a directory in the temp folder.
            Directory.CreateDirectory(Path.Combine(_updateTempFilesPath, folder.Substring(absolutePartDirectoryLength)));

            foreach (string fileName in Directory.GetFiles(folder, "*", SearchOption.TopDirectoryOnly)) {
                string shortFileName = fileName.Substring(absolutePartDirectoryLength);
                if (!IsFileOrFolderExcluded(shortFileName, excludedFilesOrFolders)) {
                    switch (shortFileName.ToLowerInvariant()) {
                        case @"\version.ini":
                            File.Copy(fileName, Path.Combine(_updateTempFilesPath, shortFileName));
                            break;
                        case @"\history.xml":
                        case @"\vApus.CommitTool.exe":
                            break;
                        default:
                            File.Copy(fileName, Path.Combine(_updateTempFilesPath, shortFileName));
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

    }
}