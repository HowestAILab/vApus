/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using OpenQA.Selenium.Remote;
using System;
using System.Diagnostics;
using System.IO;
using System.Web;

namespace vApus.Browser.Chrome {
    public class Chrome : BrowserBase {
        /// <summary>
        /// Always call this first.
        /// </summary>
        /// <param name="trustedUrls">For basic authentication. Not supported.</param>
        public override void InitializeBrowser(params string[] trustedUrls) {
            ExitBrowser();
            if (Process.GetProcessesByName("chromedriver").Length == 0) //This will work for simultanious users. There will be one process that must be manually closed.
                Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chromedriver.exe --port=" + Properties.Settings.Default.ChromeDriverPort));
            _driver = new RemoteWebDriver(new Uri("http://localhost:" + Properties.Settings.Default.ChromeDriverPort), DesiredCapabilities.Chrome());

            //profile....
            //export har...

        }
        /// <summary>
        /// Navigate to a given url and return the log: a HTTPArchive (HAR).
        /// </summary>
        /// <param name="protocol">http or https</param>
        /// <param name="hostname">(+ : + port). For instance: www.google.com</param>
        /// <param name="relativeUrl">For instance: /</param>
        /// <param name="cookies">You can add your own cookies, e.g. authentication.</param>
        /// <param name="username">For basic authentication. Not supported.</param>
        /// <param name="password">For basic authentication. Not supported.</param>
        /// <returns>The network log file.</returns>
        public override string Navigate(Protocol protocol, string hostname, string relativeUrl, HttpCookieCollection cookies = null, string username = "", string password = "") {
            if (cookies != null)
                AddOrUpdateCookies(cookies);

            _driver.Navigate().GoToUrl(protocol + "://" + hostname + relativeUrl);
            return "";
        }
    }
}
