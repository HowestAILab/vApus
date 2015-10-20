/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Web;

namespace vApus.Browser {
    /// <summary>
    /// An interface that should only be implemented in 'BaseBrowse' for a navigate-url-with-a browser solution.
    /// </summary>
    public interface IBrowser : IDisposable {
        /// <summary>
        /// Always call this first.
        /// </summary>
        /// <param name="trustedUrls">For basic authentication. Should be hostname (+ : + port) + relative url, e.g. www.google.be/</param>
        void InitializeBrowser(params string[] trustedUrls);

        /// <summary>
        /// Navigate to a given url and return the log, e.g. HTTPArchive, IE's F12 NetXML.
        /// </summary>
        /// <param name="protocol">HTTP or HTTPS</param>
        /// <param name="hostname">(+ : + port). For instance: www.google.com</param>
        /// <param name="relativeUrl">For instance: /</param>
        /// <param name="cookies">You can add your own cookies, e.g. authentication.</param>
        /// <param name="username">For basic authentication</param>
        /// <param name="password">For basic authentication</param>
        /// <returns>The network log file.</returns>
        string Navigate(Protocol protocol, string hostname, string relativeUrl, HttpCookieCollection cookies = null, string username = "", string password = "");

        /// <summary>
        /// Deletes the log directory and closes the driver.
        /// </summary>
        void ExitBrowser();
    }
}
