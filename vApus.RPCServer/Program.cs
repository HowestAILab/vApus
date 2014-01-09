/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;

namespace vApusRPCServer {
    class Program {
        private static SocketListener _socketListener;
        static void Main(string[] args) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Howest Sizing Servers Lab's VAPUS RPC SERVER");
            Console.WriteLine("--------------------------------------------");

            _socketListener = SocketListener.GetInstance();
            _socketListener.Start();

            Console.WriteLine("Listening on all IPv4 and IPv6 addresses on port " + _socketListener.Port + ".");

            Console.ReadLine();
            _socketListener.Stop();
        }
    }
}
