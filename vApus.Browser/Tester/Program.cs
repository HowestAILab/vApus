/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using vApus.Browser;
using vApus.Browser.Chrome;
using vApus.Browser.Firefox;

namespace Tester {
    class Program {
        static void Main(string[] args) {
            var browser = new Firefox();
            browser.InitializeBrowser();

            string uncached = browser.Navigate(Protocol.https, "www.sizingservers.be", "/");
            string cached = browser.Navigate(Protocol.https, "www.sizingservers.be", "/");

            browser.ExitBrowser();
        }
    }
}
