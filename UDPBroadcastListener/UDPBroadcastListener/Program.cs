/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UDPBroadcastListener {
    class Program {
        static void Main(string[] args) {

            Console.WriteLine("Developed by Dieter Vandroemme.");
            Console.WriteLine("(mail: dieter@sizingservers.be)");
            Console.WriteLine("_____");
            Console.WriteLine();

            Console.WriteLine("This is a very simple UDP broadcast listener implementation.");
            Console.WriteLine("It listens to the port defined in the settings file and is used to test the broadcast publisher in vApus.");
            Console.WriteLine("<Press any key to quit>");
            Console.WriteLine();

            var listener = new UdpClient(Properties.Settings.Default.ListenToPort);

            bool running = true;
            ThreadPool.QueueUserWorkItem((state) => {
                while (running) {
                    var sender = new IPEndPoint(IPAddress.Any, 0);
                    Console.WriteLine(Encoding.UTF8.GetString(listener.Receive(ref sender)));
                }
            }, null);

            Console.ReadLine();
            running = false;
        }
    }
}
