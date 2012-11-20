using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vApus.Util;

namespace vApus.Results
{
    public static class SettingsManager
    {
        #region Fields
        private static string _passwordGUID = "{51E6A7AC-06C2-466F-B7E8-4B0A00F6A21F}";
        private static byte[] _salt = { 0x49, 0x16, 0x49, 0x2e, 0x11, 0x1e, 0x45, 0x24, 0x86, 0x05, 0x01, 0x03, 0x62 };
        #endregion

        public static StringCollection GetConnectionStrings()
        {
            var connectionStrings = vApus.Results.Properties.Settings.Default.ConnectionStrings;
            if (connectionStrings == null)
                connectionStrings = new StringCollection();

            return connectionStrings;
        }
        public static StringCollection GetPasswords()
        {
            var passwords = vApus.Results.Properties.Settings.Default.Passwords;
            if (passwords == null)
                passwords = new StringCollection();

            return passwords;
        }
        public static void AddCredentials(string user, string host, int port, string password)
        {
            EditCredentials(-1, user, host, port, password);
        }
        public static void EditCredentials(int connectionStringIndex, string user, string host, int port, string password)
        {
            string connectionString = user + "@" + host + ":" + port;
            password = password.Encrypt(_passwordGUID, _salt);

            var connectionStrings = GetConnectionStrings();
            var passwords = GetPasswords();

            if (connectionStringIndex == -1)
            {
                connectionStrings.Add(connectionString);
                passwords.Add(password);
                vApus.Results.Properties.Settings.Default.ConnectionStringIndex = connectionStrings.IndexOf(connectionString);
            }
            else
            {
                connectionStrings[connectionStringIndex] = connectionString;
                passwords[connectionStringIndex] = password;
            }

            vApus.Results.Properties.Settings.Default.ConnectionStrings = connectionStrings;
            vApus.Results.Properties.Settings.Default.Passwords = passwords;
            vApus.Results.Properties.Settings.Default.Save();
        }
        public static void GetCurrentCredentials(out string user, out string host, out int port, out string password)
        {
            GetCredentials(vApus.Results.Properties.Settings.Default.ConnectionStringIndex, out user, out host, out port, out password);
        }
        public static void GetCredentials(int connectionStringIndex, out string user, out string host, out int port, out string password)
        {
            string connectionString = GetConnectionStrings()[connectionStringIndex];
            user = connectionString.Split('@')[0];
            connectionString = connectionString.Substring(user.Length + 1);
            host = connectionString.Split(':')[0];
            port = int.Parse(connectionString.Substring(host.Length + 1));

            password = GetPasswords()[connectionStringIndex].Decrypt(_passwordGUID, _salt);
        }
        public static void DeleteCredentials(int connectionStringIndex)
        {
            var connectionStrings = GetConnectionStrings();
            connectionStrings.RemoveAt(connectionStringIndex);
            vApus.Results.Properties.Settings.Default.ConnectionStrings = connectionStrings;

            var passwords = GetPasswords();
            passwords.RemoveAt(connectionStringIndex);
            vApus.Results.Properties.Settings.Default.Passwords = passwords;

            vApus.Results.Properties.Settings.Default.Save();
        }

    }
}
