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
        /// Keep this to create a update notifier dialog when needed.
        /// </summary>
        private static string _currentVersion, _newVersion, _currentChannel, _newChannel, _newHistory;

        private static bool _failedRefresh = false;
        private static bool _versionChanged = false;
        private static bool _refreshed = false;

        public static UpdateNotifierState UpdateNotifierState
        {
            get
            {
                if (_failedRefresh)
                    return Util.UpdateNotifierState.FailedConnectingToTheUpdateServer;

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
                        return Util.UpdateNotifierState.NewUpdateFound;
                    return UpdateNotifierState.UpToDate;
                }

                return UpdateNotifierState.PleaseRefresh;
            }
        }

        public static void SetCredentials(string host, int port, string username, string password, int channel)
        {
            vApus.Util.Properties.Settings.Default.Host = host;
            vApus.Util.Properties.Settings.Default.Port = port;
            vApus.Util.Properties.Settings.Default.Username = username;

            password = password.Encrypt("{A84E447C-3734-4afd-B383-149A7CC68A32}", new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            vApus.Util.Properties.Settings.Default.Password = password;
            vApus.Util.Properties.Settings.Default.Channel = channel;
            vApus.Util.Properties.Settings.Default.Save();

            _refreshed = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="channel">0 == Stable; 1 == Nightly</param>
        public static void GetCredentials(out string host, out int port, out string username, out string password, out int channel)
        {
            host = vApus.Util.Properties.Settings.Default.Host;
            port = vApus.Util.Properties.Settings.Default.Port;
            username = vApus.Util.Properties.Settings.Default.Username;
            password = vApus.Util.Properties.Settings.Default.Password;
            password = password.Decrypt("{A84E447C-3734-4afd-B383-149A7CC68A32}", new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            channel = vApus.Util.Properties.Settings.Default.Channel;

            //Trust the one in version.ini before the one in the settings.
            string versionIni = Path.Combine(Application.StartupPath, "version.ini");
            if (File.Exists(versionIni))
                channel = GetChannel(Path.Combine(Application.StartupPath, "version.ini")) == "Stable" ? 0 : 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if can update.</returns>
        public static void Refresh()
        {
            string tempFolder = Path.Combine(Application.StartupPath, "tempForUpdateNotifier");

            try
            {
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

                Sftp sftp = new Sftp(host, username, password);
                sftp.Connect(port);

                if (Directory.Exists(tempFolder) && Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories).Length == 0)
                    Directory.Delete(tempFolder, true);

                Directory.CreateDirectory(tempFolder);

                string tempVersion = Path.Combine(tempFolder, "version.ini");
                string currentVersion = Path.Combine(Application.StartupPath, "version.ini");

                try
                {
                    if (File.Exists(tempVersion))
                        File.Delete(tempVersion);
                }
                catch { }

                string channelDir = channel == 0 ? "stable" : "nightly";
                sftp.Get(channelDir + "/version.ini", tempVersion);

                try
                {
                    sftp.Close();
                }
                finally
                {
                    sftp = null;
                }

                _currentVersion = GetVersion(currentVersion);
                _newVersion = GetVersion(tempVersion);

                _currentChannel = GetChannel(currentVersion);
                _newChannel = GetChannel(tempVersion);

                _newHistory = GetHistory(tempVersion);

                _versionChanged = (_currentVersion != _newVersion) || (_currentChannel != _newChannel);

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
            using (StreamReader sr = new StreamReader(versionIniPath))
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
            return new UpdateNotifierDialog(_currentVersion, _newVersion, _currentChannel, _newChannel, _newHistory);
        }
    }
}
