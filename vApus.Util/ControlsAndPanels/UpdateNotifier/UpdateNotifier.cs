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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Tamir.SharpSsh;
using vApus.Util.Properties;
using System.Linq;
using System.Reflection;

namespace vApus.Util {
    public enum UpdateNotifierState {
        [Description("Disabled")]
        Disabled = 0,
        [Description("Failed Connecting to the Update Server!")]
        FailedConnectingToTheUpdateServer,
        [Description("Please Refresh...")]
        PleaseRefresh,
        [Description("New Update Found")]
        NewUpdateFound,
        [Description("Up to Date")]
        UpToDate
    }

    /// <summary>
    /// Notifies if an update for vApus is available when given correct credentials.
    /// </summary>
    public static class UpdateNotifier {
        #region Fields
        private static readonly Mutex _canRefreshNamedMutex = new Mutex(true, Assembly.GetExecutingAssembly().FullName + "_UpdateNotifier");

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
                    NamedObjectRegistrar.RegisterOrUpdate("vApusVersion", _currentVersion);
                }
            }
        }
        public static string CurrentChannel {
            get { return _currentChannel; }
            private set {
                if (_currentChannel != value) {
                    _currentChannel = value;
                    NamedObjectRegistrar.RegisterOrUpdate("vApusChannel", _currentChannel);
                }
            }
        }
        #endregion

        #region Functions
        public static UpdateNotifierState UpdateNotifierState {
            get {
                if (_failedRefresh)
                    return UpdateNotifierState.FailedConnectingToTheUpdateServer;

                string host, username, password;
                int port, channel;
                bool smartUpdate;
                GetCredentials(out host, out port, out username, out password, out channel, out smartUpdate);

                if (host.Length == 0 || username.Length == 0 || password.Length == 0) {
                    return UpdateNotifierState.Disabled;
                } else if (_refreshed) {
                    if (_versionChanged)
                        return UpdateNotifierState.NewUpdateFound;
                    return UpdateNotifierState.UpToDate;
                }

                return UpdateNotifierState.PleaseRefresh;
            }
        }

        public static void SetCredentials(string host, int port, string username, string password, int channel, bool smartUpdate) {
            Settings.Default.UNHost = host;
            Settings.Default.UNPort = port;
            Settings.Default.UNUsername = username;

            password = password.Encrypt("{A84E447C-3734-4afd-B383-149A7CC68A32}",
                                        new byte[]
                                            {
                                                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65,
                                                0x76
                                            });
            Settings.Default.UNPassword = password;
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
        /// <param name="password"></param>
        /// <param name="channel">0 == Stable; 1 == Nightly</param>
        public static void GetCredentials(out string host, out int port, out string username, out string password, out int channel, out bool smartUpdate) {
            host = Settings.Default.UNHost;
            port = Settings.Default.UNPort;
            username = Settings.Default.UNUsername;
            password = Settings.Default.UNPassword;
            password = password.Decrypt("{A84E447C-3734-4afd-B383-149A7CC68A32}",
                                        new byte[]
                                            {
                                                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65,
                                                0x76
                                            });
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
            if (_canRefreshNamedMutex.WaitOne(0))
                try {
                    string currentVersionIni = Path.Combine(Application.StartupPath, "version.ini");
                    CurrentVersion = GetVersion(currentVersionIni);
                    CurrentChannel = GetChannel(currentVersionIni);

                    string host, username, password;
                    int port, channel;
                    bool smartUpdate;
                    GetCredentials(out host, out port, out username, out password, out channel, out smartUpdate);

                    _failedRefresh = false;
                    if (host.Length == 0 || username.Length == 0 || password.Length == 0) {
                        _versionChanged = false;
                        _refreshed = false;
                        return;
                    }

                    var sftp = new Sftp(host, username, password);
                    sftp.Connect(port);

                    try {
                        if (Directory.Exists(tempFolder) &&
                        Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories).Length == 0)
                            Directory.Delete(tempFolder, true);
                    } catch (Exception ex) {
                        Loggers.Log(Level.Warning, "Failed deleting the temp dir.", ex);
                    }

                    Directory.CreateDirectory(tempFolder);

                    string tempVersion = Path.Combine(tempFolder, "version.ini");

                    try {
                        if (File.Exists(tempVersion)) File.Delete(tempVersion);
                    } catch (Exception ex) {
                        Loggers.Log(Level.Warning, "Failed deleting the temp version.", ex);
                    }

                    string channelDir = channel == 0 ? "stable" : "nightly";
                    sftp.Get(channelDir + "/version.ini", tempVersion);

                    try { sftp.Close(); } finally { sftp = null; }

                    _newVersion = GetVersion(tempVersion);
                    _newChannel = GetChannel(tempVersion);

                    _newHistory = GetHistory(tempVersion);

                    if (_newVersion.Length == 0 || _newChannel.Length == 0 || _newHistory.Length == 0)
                        throw new Exception("Could not fetch the versioning data.");

                    _versionChanged = (CurrentVersion != _newVersion) || (CurrentChannel != _newChannel);

                    _refreshed = true;
                } catch {
                    _failedRefresh = true;
                    _refreshed = false;
                } finally {
                    if (Directory.Exists(tempFolder))
                        Directory.Delete(tempFolder, true);
                    _canRefreshNamedMutex.ReleaseMutex();
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
                } catch {
                    if (++retry != 101) {
                        Thread.Sleep(retry * 10);
                        goto Retry;
                    } else throw;
                }
            return string.Empty;
        }

        public static UpdateNotifierDialog GetUpdateNotifierDialog() {
            return new UpdateNotifierDialog(CurrentVersion, _newVersion, CurrentChannel, _newChannel, _newHistory);
        }
        #endregion
    }
}