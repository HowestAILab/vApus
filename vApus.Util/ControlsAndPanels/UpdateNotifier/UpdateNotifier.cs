/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Renci.SshNet;
using vApus.Util.Properties;

namespace vApus.Util {
    public enum UpdateNotifierState {
        [Description("Update notifier disabled")]
        Disabled = 0,
        [Description("Failed Connecting to the update server!")]
        FailedConnectingToTheUpdateServer,
        [Description("Update notifier - please refresh...")]
        PleaseRefresh,
        [Description("New update found")]
        NewUpdateFound,
        [Description("vApus up to date")]
        UpToDate
    }

    /// <summary>
    /// Notifies if an update for vApus is available when given correct credentials.
    /// </summary>
    public static class UpdateNotifier {
        #region Fields

        private static bool _failedRefresh;
        private static bool _versionChanged;
        private static bool _refreshed;

        /// <summary>
        ///     Keep this to create a update notifier dialog when needed.
        /// </summary>
        private static string _currentVersion, _currentChannel, _newVersion, _newChannel, _newHistory;
        public static string CurrentVersion {
            get { return _currentVersion; }
            private set {
                if (_currentVersion != value) {
                    _currentVersion = value;
                    NamedObjectRegistrar.RegisterOrUpdate("Version", _currentVersion);
                }
            }
        }
        public static string CurrentChannel {
            get { return _currentChannel; }
            private set {
                if (_currentChannel != value) {
                    _currentChannel = value;
                    NamedObjectRegistrar.RegisterOrUpdate("Channel", _currentChannel);
                }
            }
        }
        #endregion

        #region Functions
        public static UpdateNotifierState UpdateNotifierState {
            get {
                if (_failedRefresh)
                    return UpdateNotifierState.FailedConnectingToTheUpdateServer;

                string host, username, privateRSAKeyPath;
                int port, channel;
                bool smartUpdate;
                GetCredentials(out host, out port, out username, out privateRSAKeyPath, out channel, out smartUpdate);

                if (host.Length == 0 || username.Length == 0 || privateRSAKeyPath.Length == 0) {
                    return UpdateNotifierState.Disabled;
                }
                else if (_refreshed) {
                    if (_versionChanged)
                        return UpdateNotifierState.NewUpdateFound;
                    return UpdateNotifierState.UpToDate;
                }

                return UpdateNotifierState.PleaseRefresh;
            }
        }

        public static void SetCredentials(string host, int port, string username, string privateRSAKeyPath, int channel, bool smartUpdate) {
            Settings.Default.UNHost = host;
            Settings.Default.UNPort = port;
            Settings.Default.UNUsername = username;
            Settings.Default.UNPrivateRSAKeyPath = privateRSAKeyPath;
            Settings.Default.UNChannel = channel;
            Settings.Default.UNSmartUpdate = smartUpdate;
            Settings.Default.Save();

            _refreshed = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="privateRSAKeyPath"></param>
        /// <param name="channel">0 == Stable; 1 == Nightly</param>
        public static void GetCredentials(out string host, out int port, out string username, out string privateRSAKeyPath, out int channel, out bool smartUpdate) {
            host = Settings.Default.UNHost;
            port = Settings.Default.UNPort;
            username = Settings.Default.UNUsername;
            privateRSAKeyPath = Settings.Default.UNPrivateRSAKeyPath;
            channel = Settings.Default.UNChannel;

            //If there is no channel set get it from the version.ini.
            if (channel == -1) {
                string versionIni = Path.Combine(Application.StartupPath, "version.ini");
                if (File.Exists(versionIni))
                    channel = GetChannel(Path.Combine(Application.StartupPath, "version.ini")) == "Stable" ? 0 : 1;
            }
            //If there is no version.ini set the channel to stable.
            if (channel == -1) channel = 0;

            smartUpdate = Settings.Default.UNSmartUpdate;
        }

        public static void Refresh() {
            string tempFolder = Path.Combine(Application.StartupPath, "tempForUpdateNotifier");

            _refreshed = false;

            bool mutexCreated;
            var canRefreshNamedMutex = new Mutex(true, "vApus_UpdateNotifier", out mutexCreated);
            if (mutexCreated || canRefreshNamedMutex.WaitOne(0))
                try {
                    string currentVersionIni = Path.Combine(Application.StartupPath, "version.ini");
                    CurrentVersion = GetVersion(currentVersionIni);
                    CurrentChannel = GetChannel(currentVersionIni);

                    string host, username, privateRSAKeyPath;
                    int port, channel;
                    bool smartUpdate;
                    GetCredentials(out host, out port, out username, out privateRSAKeyPath, out channel, out smartUpdate);

                    _failedRefresh = false;
                    if (host.Length == 0 || username.Length == 0 || privateRSAKeyPath.Length == 0) {
                        _versionChanged = false;
                        _refreshed = false;
                        return;
                    }
                    try {
                        if (Directory.Exists(tempFolder) &&
                        Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories).Length == 0)
                            Directory.Delete(tempFolder, true);
                    }
                    catch (Exception ex) {
                        Loggers.Log(Level.Warning, "Failed deleting the temp dir.", ex);
                    }

                    Directory.CreateDirectory(tempFolder);


                    string tempVersionPath = Path.Combine(tempFolder, "version.ini");
                    try {
                        if (File.Exists(tempVersionPath)) File.Delete(tempVersionPath);
                    }
                    catch (Exception ex) {
                        Loggers.Log(Level.Warning, "Failed deleting the temp version.", ex);
                    }

                    using (var sftp = new SftpClient(host, port, username, new PrivateKeyFile(privateRSAKeyPath))) {
                        sftp.Connect();           
                         
                        string channelDir = channel == 0 ? "stable" : "nightly";

                        using (var str = File.Create(tempVersionPath))
                            sftp.DownloadFile("vApusUpdate/" + channelDir + "/version.ini", str);
                    }

                    _newVersion = GetVersion(tempVersionPath);
                    _newChannel = GetChannel(tempVersionPath);

                    _newHistory = GetHistory(tempVersionPath);

                    if (_newVersion.Length == 0 || _newChannel.Length == 0 || _newHistory.Length == 0)
                        throw new Exception("Could not fetch the versioning data.");

                    _versionChanged = (CurrentVersion != _newVersion) || (CurrentChannel != _newChannel);

                    _refreshed = true;
                }
                catch (Exception ex) {
                    _failedRefresh = true;
                    _refreshed = false;
                }
                finally {
                    try {
                        if (Directory.Exists(tempFolder))
                            Directory.Delete(tempFolder, true);
                    }
                    finally {
                        canRefreshNamedMutex.ReleaseMutex();
                    }
                }
        }

        private static string GetVersion(string versionIniPath) {
            return Get(versionIniPath, "[VERSION]");
        }

        private static string GetChannel(string versionIniPath) {
            return Get(versionIniPath, "[CHANNEL]");
        }

        private static string GetHistory(string versionIniPath) {
            return Get(versionIniPath, "[HISTORY]");
        }

        private static string Get(string versionIniPath, string part) {
            int retry = 0;
            Retry:
            if (File.Exists(versionIniPath))
                try {
                    using (var sr = new StreamReader(versionIniPath)) {
                        bool found = false;
                        while (sr.Peek() != -1) {
                            string line = sr.ReadLine();
                            if (found)
                                return line;

                            if (line.Trim() == part)
                                found = true;
                        }
                    }
                }
                catch {
                    if (++retry != 101) {
                        Thread.Sleep(retry * 10);
                        goto Retry;
                    }
                    else throw;
                }
            return string.Empty;
        }

        public static UpdateNotifierDialog GetUpdateNotifierDialog() {
            return new UpdateNotifierDialog(CurrentVersion, _newVersion, CurrentChannel, _newChannel, _newHistory);
        }
        #endregion
    }
}