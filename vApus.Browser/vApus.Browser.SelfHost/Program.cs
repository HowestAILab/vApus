/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace vApus.Browser.SelfHost {
    class Program {
        private static Mutex _namedMutex = new Mutex(true, Assembly.GetExecutingAssembly().FullName);

        static void Main(string[] args) {
            if (_namedMutex.WaitOne(0, true)) {

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Self-hosted empty web page.");
                Console.WriteLine("----------");
                Console.WriteLine("This can be used to initialize browser extensions when browsed to http://localhost:<port>. [default: " + Properties.Settings.Default.DefaultPort + " ]");
                Console.WriteLine("This must be run with elevated rights. Only one instance at a time can run.");
                Console.WriteLine("Usage: vApus.Browser.SelfHost.exe [<port>]");
                Console.WriteLine();

                HttpSelfHostServer server = null;

                try {
                    string address = "http://localhost:" + (args.Length == 0 ? Properties.Settings.Default.DefaultPort.ToString() : args[0]);
                    var config = new HttpSelfHostConfiguration(address);
                    config.MessageHandlers.Add(new EmptyPageHandler());

                    config.Routes.MapHttpRoute("API Default", "");

                    server = new HttpSelfHostServer(config);
                    server.OpenAsync().Wait();

                    Console.WriteLine("Empty web page hosted at " + address);
                } catch (Exception ex) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex);
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Press <any key> to quit.");
                Console.ReadLine();

                if (server != null) {
                    try {
                        server.Dispose();
                    } catch {
                        // Don't care
                    }
                }
                _namedMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Does what it says.
        /// </summary>
        public class EmptyPageHandler : DelegatingHandler {
            private Task<HttpResponseMessage> _task;

            public EmptyPageHandler() {
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(new HttpResponseMessage(HttpStatusCode.OK));
                _task = tsc.Task;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) { return _task; }
        }
    }
}
