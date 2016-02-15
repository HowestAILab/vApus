using System;
using System.Reflection;
using System.Threading;

namespace vApus.PublishItemsHandler {
    class Program {
        private static readonly Mutex _namedMutex = new Mutex(true, Assembly.GetExecutingAssembly().FullName);
        static void Main(string[] args) {
            if (_namedMutex.WaitOne(0)) {
                Console.WriteLine("vApus results handler");
                Console.WriteLine("-----");
                Console.WriteLine("This handler listens for messages published by vApus and puts them in a standardized vApus MySQL results db.");

                Console.WriteLine("Type MySQL <host>,<port>,<user> and press enter");

                string[] arr = Console.ReadLine().Split(',');

                Console.WriteLine("Type <password> and press enter");

                string password = ReadPassword();
                try {
                    PublishItemHandler.Init(arr[0], int.Parse(arr[1]), arr[2], password);
                    int port = 4337;
                    QueuedListener.Start(port);
                    Console.WriteLine("Listening on TCP port " + 4337 + " over IPv4.");
                }
                catch (Exception ex) {
                    Console.WriteLine("Failed connecting to MySQL\n" + ex.Message);
                }
                Console.WriteLine("Press <any key> to quit");
                Console.ReadLine();
            }
        }

        public static string ReadPassword() {
            string password = string.Empty;
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter) {
                if (info.Key != ConsoleKey.Backspace) {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace) {
                    if (!string.IsNullOrEmpty(password)) {
                        password = password.Substring(0, password.Length - 1);
                        int pos = Console.CursorLeft;
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            Console.WriteLine();
            return password;
        }
    }
}

