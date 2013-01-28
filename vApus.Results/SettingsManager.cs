using System.Collections.Specialized;
using vApus.Results.Properties;
using vApus.Util;

namespace vApus.Results {
    public static class SettingsManager {

        #region Fields

        private static string _passwordGUID = "{51E6A7AC-06C2-466F-B7E8-4B0A00F6A21F}";

        private static readonly byte[] _salt =
            {
                0x49, 0x16, 0x49, 0x2e, 0x11, 0x1e, 0x45, 0x24, 0x86, 0x05, 0x01, 0x03,
                0x62
            };

        #endregion

        public static StringCollection GetConnectionStrings() {
            StringCollection connectionStrings = Settings.Default.ConnectionStrings;
            if (connectionStrings == null) connectionStrings = new StringCollection();

            return connectionStrings;
        }

        public static StringCollection GetPasswords() {
            StringCollection passwords = Settings.Default.Passwords;
            if (passwords == null) passwords = new StringCollection();

            return passwords;
        }

        public static void AddCredentials(string user, string host, int port, string password) {
            EditCredentials(-1, user, host, port, password);
        }

        public static void EditCredentials(int connectionStringIndex, string user, string host, int port,
                                           string password) {
            string connectionString = user + "@" + host + ":" + port;
            password = password.Encrypt(_passwordGUID, _salt);

            StringCollection connectionStrings = GetConnectionStrings();
            StringCollection passwords = GetPasswords();

            if (connectionStringIndex == -1) {
                connectionStrings.Add(connectionString);
                passwords.Add(password);
                Settings.Default.ConnectionStringIndex = connectionStrings.IndexOf(connectionString);
            } else {
                connectionStrings[connectionStringIndex] = connectionString;
                passwords[connectionStringIndex] = password;
            }

            Settings.Default.ConnectionStrings = connectionStrings;
            Settings.Default.Passwords = passwords;
            Settings.Default.Save();
        }

        public static void GetCurrentCredentials(out string user, out string host, out int port, out string password) {
            GetCredentials(Settings.Default.ConnectionStringIndex, out user, out host, out port, out password);
        }

        public static void GetCredentials(int connectionStringIndex, out string user, out string host, out int port,
                                          out string password) {
            string connectionString = GetConnectionStrings()[connectionStringIndex];
            user = connectionString.Split('@')[0];
            connectionString = connectionString.Substring(user.Length + 1);
            host = connectionString.Split(':')[0];
            port = int.Parse(connectionString.Substring(host.Length + 1));

            password = GetPasswords()[connectionStringIndex].Decrypt(_passwordGUID, _salt);
        }

        public static void DeleteCredentials(int connectionStringIndex) {
            StringCollection connectionStrings = GetConnectionStrings();
            connectionStrings.RemoveAt(connectionStringIndex);
            Settings.Default.ConnectionStrings = connectionStrings;

            StringCollection passwords = GetPasswords();
            passwords.RemoveAt(connectionStringIndex);
            Settings.Default.Passwords = passwords;

            Settings.Default.Save();
        }
    }
}