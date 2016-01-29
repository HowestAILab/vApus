using System;

namespace vApus.ResultsHandler {
    class Program {
        static void Main(string[] args) {
            QueuedListener.Start();
            Console.ReadLine();
        }       
    }
}
