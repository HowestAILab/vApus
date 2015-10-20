/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;

namespace vApus.Browser {
    /// <summary>
    /// This class should be inherited in a navigate-url-with-a browser solution.
    /// </summary>
    public abstract class BrowserBase : IBrowser {
        private string _logDirectory;
        protected IWebDriver _driver;

        /// <summary>
        /// <para>Create a log directory if it does not already exists and return the path.</para>
        /// <para>This path is stored in _logDirectory.</para>
        /// </summary>
        /// <returns></returns>
        protected string CreateAndReturnLogDirectory() {
            if (_logDirectory == null)
                _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Process.GetCurrentProcess().Id + "-" + DateTime.UtcNow.Ticks);

            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
            return _logDirectory;
        }
        /// <summary>
        /// </summary>
        /// <param name="trustedUrls"></param>
        /// <returns>trusted urls, comma-delimited.</returns>
        protected string CombineTrustedUrls(string[] trustedUrls) {
            if (trustedUrls == null) throw new NullReferenceException("trustedUrls");

            var sb = new StringBuilder();
            for (int i = 0; i < trustedUrls.Length - 1; i++) {
                sb.Append(trustedUrls[i]);
                sb.Append(", ");
            }

            if (trustedUrls.Length != 0)
                sb.Append(trustedUrls[trustedUrls.Length - 1]);

            return sb.ToString();
        }
        /// <summary>
        /// Add or update cookies to be send with browser requests.
        /// </summary>
        /// <param name="cookies"></param>
        protected void AddOrUpdateCookies(HttpCookieCollection cookies) {
            foreach (HttpCookie cookie in cookies) {
                _driver.Manage().Cookies.DeleteCookieNamed(cookie.Name);
                _driver.Manage().Cookies.AddCookie(new Cookie(cookie.Name, cookie.Value, cookie.Domain, cookie.Path, cookie.Expires));
            }
        }

        public void Dispose() {
            ExitBrowser();
        }

        /// <summary>
        /// Always call this first.
        /// </summary>
        /// <param name="trustedUrls">For basic authentication. Should be hostname (+ : + port) + relative url, e.g. www.google.be/</param>
        public abstract void InitializeBrowser(params string[] trustedUrls);

        /// <summary>
        /// Navigate to a given url and return the log, e.g. HTTPArchive, IE's F12 NetXML.
        /// </summary>
        /// <param name="protocol">http or https</param>
        /// <param name="hostname">(+ : + port). For instance: www.google.com</param>
        /// <param name="relativeUrl">For instance: /</param>
        /// <param name="cookies">You can add your own cookies, e.g. authentication.</param>
        /// <param name="username">For basic authentication</param>
        /// <param name="password">For basic authentication</param>
        /// <returns>The network log file.</returns>
        public abstract string Navigate(Protocol protocol, string hostname, string relativeUrl, HttpCookieCollection cookies = null, string username = "", string password = "");

        /// <summary>
        /// Deletes the log directory and closes the driver.
        /// </summary>
        public virtual void ExitBrowser() {
            if (_driver != null)
                try {
                    _driver.Close();
                } catch {
                    // Don't care
                }
            if (_logDirectory != null && Directory.Exists(_logDirectory)) {
                try {
                    Directory.Delete(_logDirectory, true);
                } catch {
                    // Don't care
                }
                _logDirectory = null;
            }
        }
    }
}
