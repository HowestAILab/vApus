/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Diagnostics;

namespace vApus.Util {
    /// <summary>
    /// Uses the default Windows RDP client.
    /// </summary>
    public static class RemoteDesktop {
        /// <summary>
        ///Not threadsafe --> Stupid credentials workaround. Do not forget to remove the credentials (RemoveCredentials(string ip)). Do not do this too fast, otherwise login will not work.
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        public static void Show(string ip, string username, string password, string domain = null) {
            StoreCredentials(ip, username, password, domain);

            var p = new Process();
            p.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe");
            p.StartInfo.Arguments = "/v " + ip;
            p.Start();
        }

        private static void StoreCredentials(string ip, string username, string password, string domain = null) {
            if (!string.IsNullOrWhiteSpace(domain)) username = domain + "\\" + username;

            var p = new Process();
            p.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe");
            p.StartInfo.Arguments = "/generic:TERMSRV/" + ip + " /user:" + username + " /pass:" + password;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
        }

        /// <summary>
        /// I know, this sucks. Why not can't credentials be passed to mstsc.exe as arguments?...
        /// </summary>
        /// <param name="ip"></param>
        public static void RemoveCredentials(string ip) {
            var p = new Process();
            p.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe");
            p.StartInfo.Arguments = "/delete:TERMSRV/" + ip;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

        }
    }
}
