/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Collections.Specialized;
using vApus.Results.Properties;
using vApus.Util;

namespace vApus.Results {
    /// <summary>
    /// Serves at saving and loading connection strings. A connection string is needed to be able to save and load stress test results.
    /// </summary>
    public static class ConnectionStringManager {

        #region Fields
        /// <summary>
        /// Currently ony used for getting the credentials.
        /// </summary>
        private static readonly object _lock = new object();

        private static string _passwordGUID = "{51E6A7AC-06C2-466F-B7E8-4B0A00F6A21F}";

        private static readonly byte[] _salt = { 0x49, 0x16, 0x49, 0x2e, 0x11, 0x1e, 0x45, 0x24, 0x86, 0x05, 0x01, 0x03, 0x62 };
        #endregion

        #region Properties
        /// <summary>
        /// The other properties and functions will work if this is set to false. However, GetCredentials and GetCurrentCredentials will output null values.
        /// This check can be used in the gui and to disable result saving.
        /// </summary>
        public static bool Enabled {
            get {
                return Settings.Default.Enabled;
            }
            set {
                Settings.Default.Enabled = value;
                Settings.Default.Save();
            }
        }
        public static string CurrentDatabaseName { get; private set; }
        #endregion

        #region Functions
        /// <summary>
        /// Adds the different parts of a connection string. To get a usable connection string use the function GetCurrentConnectionString.
        /// The newly added connection string will be the current one.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="password"></param>
        public static void AddConnectionString(string user, string host, int port, string password) {
            EditConnectionString(-1, user, host, port, password);
        }
        /// <summary>
        /// Edits the different parts of a connection string. To get a usable connection string use the function GetCurrentConnectionString.
        /// The editted connection string will be the current one.
        /// </summary>
        /// <param name="connectionStringIndex"></param>
        /// <param name="user"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="password"></param>
        public static void EditConnectionString(int connectionStringIndex, string user, string host, int port, string password) {
            string connectionString = user + "@" + host + ":" + port;
            password = password.Encrypt(_passwordGUID, _salt);

            StringCollection connectionStrings = GetFormattedConnectionStrings();
            StringCollection passwords = GetPasswords();

            if (connectionStringIndex == -1) {
                connectionStrings.Add(connectionString);
                passwords.Add(password);
            } else {
                connectionStrings[connectionStringIndex] = connectionString;
                passwords[connectionStringIndex] = password;
            }

            Settings.Default.ConnectionStrings = connectionStrings;
            Settings.Default.ConnectionStringIndex = connectionStrings.IndexOf(connectionString);
            Settings.Default.Passwords = passwords;
            Settings.Default.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A connection string to be set in the ConnectionString property of DatabaseActions. DatabaseActions handles the communication to the db. If Enabled is set to false null will be returned.</returns>
        public static string GetCurrentConnectionString() {
            string user, host, password;
            int port;
            GetCurrentConnectionString(out user, out host, out port, out password);

            return user == null ? null : string.Format("Server={0};Port={1};Uid={2};Pwd={3};table cache = true;", host, port, user, password);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName">Connect to a specific database.</param>
        /// <returns>A connection string to be set in the ConnectionString property of DatabaseActions. DatabaseActions handles the communication to the db. If Enabled is set to false null will be returned.</returns>
        public static string GetCurrentConnectionString(string databaseName) {
            if (string.IsNullOrEmpty(databaseName)) return GetCurrentConnectionString();

            string user, host, password;
            int port;
            GetCurrentConnectionString(out user, out host, out port, out password);

            CurrentDatabaseName = databaseName;
            return user == null ? null : string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};table cache = true", host, port, databaseName, user, password);
        }

        /// <summary>
        /// Outputs the different parts of the current connection string. If Enabled is set to false null values will be outputted.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="password"></param>
        public static void GetCurrentConnectionString(out string user, out string host, out int port, out string password) {
            GetConnectionString(Settings.Default.ConnectionStringIndex, out user, out host, out port, out password);
        }
        /// <summary>
        /// Outputs the different parts of a connection string. If Enabled is set to false null values will be outputted.
        /// </summary>
        /// <param name="connectionStringIndex"></param>
        /// <param name="user"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="password"></param>
        /// <param name="ignoreDisabled"></param>
        public static void GetConnectionString(int connectionStringIndex, out string user, out string host, out int port, out string password, bool ignoreDisabled = false) {
            lock (_lock) {
                user = null;
                host = null;
                port = 0;
                password = null;

                if (!Enabled && !ignoreDisabled) return;

                var connectionStrings = GetFormattedConnectionStrings();
                if (connectionStrings.Count != 0) {
                    string connectionString = connectionStrings[connectionStringIndex];
                    user = connectionString.Split('@')[0];
                    connectionString = connectionString.Substring(user.Length + 1);
                    host = connectionString.Split(':')[0];
                    port = int.Parse(connectionString.Substring(host.Length + 1));

                    password = GetPasswords()[connectionStringIndex].Decrypt(_passwordGUID, _salt);
                }
            }
        }
       
        /// <summary>
        /// For listing in a combobox for instance.
        /// </summary>
        /// <returns></returns>
        public static StringCollection GetFormattedConnectionStrings() {
            StringCollection connectionStrings = Settings.Default.ConnectionStrings;
            if (connectionStrings == null) connectionStrings = new StringCollection();

            return connectionStrings;
        }
        private static StringCollection GetPasswords() {
            StringCollection passwords = Settings.Default.Passwords;
            if (passwords == null) passwords = new StringCollection();

            return passwords;
        }
        
        public static void DeleteConnectionString(int connectionStringIndex) {
            StringCollection connectionStrings = GetFormattedConnectionStrings();
            connectionStrings.RemoveAt(connectionStringIndex);
            Settings.Default.ConnectionStrings = connectionStrings;

            StringCollection passwords = GetPasswords();
            passwords.RemoveAt(connectionStringIndex);
            Settings.Default.Passwords = passwords;

            Settings.Default.Save();
        }
        #endregion
    }
}