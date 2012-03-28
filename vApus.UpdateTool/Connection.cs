/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.IO;
using vApus.Util;

namespace vApus.UpdateTool
{
    /// <summary>
    /// For connecting with the OpenSSHD.
    /// </summary>
    internal struct Connection
    {
        #region Fields
        public string Host, Username, Password;
        public int Port;
        #endregion

        #region Constructors
        public Connection(string host)
            : this(host, 22)
        { }
        public Connection(string host, int port)
            : this(host, port, string.Empty)
        { }
        public Connection(string host, int port, string username)
            : this(host, port, username, string.Empty)
        { }
        public Connection(string host, int port, string username, string password)
        {
            host = host.Trim();
            if (host.Length == 0)
                throw new ArgumentException("host");
            Host = host;
            Port = port;
            if (username == null)
                throw new ArgumentNullException("username");
            Username = username;
            if (password == null)
                throw new ArgumentNullException("password");
            Password = password;
        }
        #endregion

        #region Functions
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public void SaveToCSV(StreamWriter sw)
        {
            if (Password.Length > 0)
                sw.WriteLine(Host + ',' + Port + ',' + Username + ',' + Password.Encrypt("{A84E447C-3734-4afd-B383-149A7CC68A32}", new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }));
            else
                sw.WriteLine(Host + ',' + Port + ',' + Username + ',');
        }
        /// <summary>
        /// </summary>
        /// <param name="entry"></param>
        public void LoadFromCSVEntry(string entry)
        {
            string[] splitted = entry.Split(',');
            if (splitted.Length != 4)
                throw new Exception("Not a valid entry: " + entry);
            Host = splitted[0];
            Port = int.Parse(splitted[1]);
            Username = splitted[2];
            if (splitted[3].Trim().Length > 0)
                Password = splitted[3].Decrypt("{A84E447C-3734-4afd-B383-149A7CC68A32}", new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            else
                Password = string.Empty;
        }
        public bool Equals(string host, int port, string username, string password)
        {
            return (Host.Equals(host, StringComparison.CurrentCultureIgnoreCase) && Port == port && Username.Equals(username, StringComparison.CurrentCultureIgnoreCase) && Password.Equals(password, StringComparison.CurrentCultureIgnoreCase));
        }
        public bool Equals(Connection connection)
        {
            return Equals(connection.Host, connection.Port, connection.Username, connection.Password);
        }
        public override string ToString()
        {
            return "\"" + Username + '@' + Host + '\"' + Port;
        }
        #endregion
    }
}
