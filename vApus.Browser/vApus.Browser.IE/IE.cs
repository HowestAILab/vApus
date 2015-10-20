using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace vApus.Browser.IE
{
    public class IE : BrowserBase {
        public override void InitializeBrowser(params string[] trustedUrls) {
            throw new NotImplementedException();
        }

        public override string Navigate(Protocol protocol, string hostname, string relativeUrl, HttpCookieCollection cookies = null, string username = "", string password = "") {
            throw new NotImplementedException();
        }
    }
}
