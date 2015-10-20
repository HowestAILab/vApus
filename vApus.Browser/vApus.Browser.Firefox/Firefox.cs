/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using OpenQA.Selenium.Firefox;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Web;

namespace vApus.Browser.Firefox {
    /// <summary>
    /// Navigate a url with Firefox. Always call InitializeBrowser first.
    /// </summary>
    public class Firefox : BrowserBase {
        /// <summary>
        /// Always call this first.
        /// </summary>
        /// <param name="trustedUrls">For basic authentication. Should be hostname (+ : + port) + relative url, e.g. www.google.be/</param>
        public override void InitializeBrowser(params string[] trustedUrls) {
            ExitBrowser();

            var profile = new FirefoxProfile();

            profile.AddExtension(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebug-2.0.12b1.xpi"));
            profile.AddExtension(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "netExport-0.9b7.xpi"));


            profile.SetPreference("extensions.firebug.currentVersion", "2.0.12b1"); //Avoid Firebug start page
            profile.SetPreference("extensions.firebug.DBG_NETEXPORT", false);
            profile.SetPreference("extensions.firebug.allPagesActivation", "on");
            profile.SetPreference("extensions.firebug.defaultPanelName", "net");
            profile.SetPreference("extensions.firebug.net.enableSites", true);

            profile.SetPreference("extensions.firebug.netexport.showPreview", false);
            profile.SetPreference("extensions.firebug.netexport.alwaysEnableAutoExport", true);
            profile.SetPreference("extensions.firebug.netexport.defaultLogDir", CreateAndReturnLogDirectory());

            //Clear cache on shutdown.
            profile.SetPreference("privacy.clearOnShutdown.cache", true);
            profile.SetPreference("privacy.clearOnShutdown.cookies", true);
            profile.SetPreference("privacy.clearOnShutdown.downloads", true);
            profile.SetPreference("privacy.clearOnShutdown.formdata", true);
            profile.SetPreference("privacy.clearOnShutdown.history", true);
            profile.SetPreference("privacy.clearOnShutdown.offlineApps", true);
            profile.SetPreference("privacy.clearOnShutdown.openWindows", true);
            profile.SetPreference("privacy.clearOnShutdown.passwords", true);
            profile.SetPreference("privacy.clearOnShutdown.sessions", true);
            profile.SetPreference("privacy.clearOnShutdown.siteSettings", true);

            if (trustedUrls.Length != 0)
                profile.SetPreference("network.automatic-ntlm-auth.trusted-uris", CombineTrustedUrls(trustedUrls));

            _driver = new FirefoxDriver(profile);

            StartSelfHostWebPage();
            Navigate(Protocol.http, "localhost:" + Properties.Settings.Default.SelfHostWebPagePort , "/");
        }
        //Stupid init extensions work-around.
        private void StartSelfHostWebPage() {
            if (Process.GetProcessesByName("vApus.Browser.SelfHost").Length == 0) {
                var info = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vApus.Browser.SelfHost.exe"));
                info.Verb = "runas";
                info.Arguments = Properties.Settings.Default.SelfHostWebPagePort.ToString();
                Process.Start(info);
                Thread.Sleep(2000); //Should be enough.
            }
        }

        /// <summary>
        /// Navigate to a given url and return the log: a HTTPArchive (HAR).
        /// </summary>
        /// <param name="protocol">http or https</param>
        /// <param name="hostname">(+ : + port). For instance: www.google.com</param>
        /// <param name="relativeUrl">For instance: /</param>
        /// <param name="cookies">You can add your own cookies, e.g. authentication.</param>
        /// <param name="username">For basic authentication</param>
        /// <param name="password">For basic authentication</param>
        /// <returns>The network log file.</returns>
        public override string Navigate(Protocol protocol, string hostname, string relativeUrl, HttpCookieCollection cookies = null, string username = "", string password = "") {
            string url = protocol + "://";
            if (!string.IsNullOrWhiteSpace(username)) url += username + ":" + password + "@";

            url += hostname + relativeUrl;

            if (cookies != null)
                AddOrUpdateCookies(cookies);

            _driver.Navigate().GoToUrl(url);
            return FetchLog();
        }

        private string FetchLog() {
            string har = null;
            if (_driver != null) {
                var files = new string[0];
                while (files.Length < 1) {
                    files = Directory.GetFiles(CreateAndReturnLogDirectory());
                    Thread.Sleep(200);
                }

                using (var sr = new StreamReader(files[0]))
                    har = sr.ReadToEnd();

                foreach (string file in files)
                    File.Delete(file);
            }
            return har;
        }
    }
}
