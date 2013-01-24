/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Tamir.SharpSsh;
using vApus.Util.Properties;

namespace vApus.Util
{
    public enum UpdateNotifierState
    {
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

    public static class UpdateNotifier
    {
        /// <summary>
        ///     Keep this to create a update notifier dialog when needed.
        /// </summary>
        private static string _currentVersion, _currentChannel, _newVersion, _newChannel, _newHistory;
        private static string CurrentVersion
        {
            get { return _currentVersion; }
            set
            {
                if (_currentVersion != value)
                {
                    _currentVersion = value;
                    NamedObjectRegistrar.RegisterOrUpdate("vApusVersion", _currentVersion);
                }
            }
        }
        private static string CurrentChannel
        {
            get { return _currentChannel; }
            set
            {
                if (_currentChannel != value)
                {
                    _currentChannel = value;
                    NamedObjectRegistrar.RegisterOrUpdate("vApusChannel", _currentChannel);
                }
            }
        }
        private static bool _failedRefresh;
        private static bool _versionChanged;
        private static bool _refreshed;

        public static UpdateNotifierState UpdateNotifierState
        {
            get
            {
                if (_failedRefresh)
                    return UpdateNotifierState.FailedConnectingToTheUpdateServer;

                string host, username, password;
                int port, channel;
                GetCredentials(out host, out port, out username, out password, out channel);

                if (host.Length == 0 || username.Length == 0 || password.Length == 0)
                {
                    return UpdateNotifierState.Disabled;
                }
                else if (_refreshed)
                {
                    if (_versionChanged)
                        return UpdateNotifierState.NewUpdateFound;
                    return UpdateNotifierState.UpToDate;
                }

                return UpdateNotifierState.PleaseRefresh;
            }
        }

        public static void SetCredentials(string host, int port, string username, string password, int channel)
        {
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
        public static void GetCredentials(out string host, out int port, out string username, out string password,
                                          out int channel)
        {
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
            if (channel == -1)
            {
                string versionIni = Path.Combine(Application.StartupPath, "version.ini");
                if (File.Exists(versionIni))
                    channel = GetChannel(Path.Combine(Application.StartupPath, "version.ini")) == "Stable" ? 0 : 1;
            }
            //If there is no version.ini set the channel to stable.
            if (channel == -1)
                channel = 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>true if can update.</returns>
        public static void Refresh()
        {
            string tempFolder = Path.Combine(Application.StartupPath, "tempForUpdateNotifier");

            try
            {
                string currentVersionIni = Path.Combine(Application.StartupPath, "version.ini");
                CurrentVersion = GetVersion(currentVersionIni);
                CurrentChannel = GetChannel(currentVersionIni);

                string host, username, password;
                int port, channel;
                GetCredentials(out host, out port, out username, out password, out channel);

                _failedRefresh = false;
                if (host.Length == 0 || username.Length == 0 || password.Length == 0)
                {
                    _versionChanged = false;
                    _refreshed = false;
                    return;
                }

                var sftp = new Sftp(host, username, password);
                sftp.Connect(port);

                if (Directory.Exists(tempFolder) &&
                    Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories).Length == 0)
                    Directory.Delete(tempFolder, true);

                Directory.CreateDirectory(tempFolder);

                string tempVersion = Path.Combine(tempFolder, "version.ini");

                try { if (File.Exists(tempVersion)) File.Delete(tempVersion); }
                catch { }

                string channelDir = channel == 0 ? "stable" : "nightly";
                sftp.Get(channelDir + "/version.ini", tempVersion);

                try { sftp.Close(); }
                finally { sftp = null; }

                _newVersion = GetVersion(tempVersion);
                _newChannel = GetChannel(tempVersion);

                _newHistory = GetHistory(tempVersion);

                _versionChanged = (CurrentVersion != _newVersion) || (CurrentChannel != _newChannel);

                _refreshed = true;
            }
            catch
            {
                _failedRefresh = true;
                _refreshed = false;
            }
            finally
            {
                if (Directory.Exists(tempFolder))
                    Directory.Delete(tempFolder, true);
            }
        }

        private static string GetVersion(string versionIniPath)
        {
            return Get(versionIniPath, "[VERSION]");
        }

        private static string GetChannel(string versionIniPath)
        {
            return Get(versionIniPath, "[CHANNEL]");
        }

        private static string GetHistory(string versionIniPath)
        {
            return Get(versionIniPath, "[HISTORY]");
        }

        private static string Get(string versionIniPath, string part)
        {
            using (var sr = new StreamReader(versionIniPath))
            {
                bool found = false;
                while (sr.Peek() != -1)
                {
                    string line = sr.ReadLine();
                    if (found)
                        return line;

                    if (line.Trim() == part)
                        found = true;
                }
            }
            return string.Empty;
        }

        public static UpdateNotifierDialog GetUpdateNotifierDialog()
        {
            return new UpdateNotifierDialog(CurrentVersion, _newVersion, CurrentChannel, _newChannel, _newHistory);
        }
    }
}