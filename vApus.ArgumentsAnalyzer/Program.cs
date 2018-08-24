/*
 * 2015 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Text;

namespace vApus.ArgumentsAnalyzer {
    class Program {
        static void Main(string[] args) {
            Console.Title = "vApus";

            Console.ForegroundColor = ConsoleColor.Gray;
            if (args[0] == "a") WriteAbout();
            else if (args[0] == "h") WriteHelp();

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press <any key> to quit.");
            Console.Read();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private static void WriteAbout() {
            Console.WriteLine("ABOUT");
            Console.WriteLine("Developed by Dieter Vandroemme.");
            Console.WriteLine("(mail: dieter@sizingservers.be)");
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private static void WriteHelp() {
            var sb = new StringBuilder();
            Console.WriteLine("HELP");
            Console.WriteLine("A solution filename must always come first (no argument key needed). -d, -m and -s must always come last.");
            Console.WriteLine("You can run vApus from a script and feed it directly arguments or you can type them in the console.");
            Console.WriteLine();
            Console.WriteLine("Keep in mind that they are sequentialy handled, so if there is an error the remaining arguments will not be interpreted.");
            Console.WriteLine();
            Console.WriteLine("Some arguments can have parameters.");
            Console.WriteLine("The typing of more parameters than needed will not have any effect\non the process of execution.");
            Console.WriteLine("If you want to use parameters containing spaces, like a filename, encapsulate them with ''.");
            Console.WriteLine();
            Console.WriteLine();

            bool otherAlreadyWritten = false;
            foreach (string s in Analyzer.PossibleArguments.Keys) {
                if (!s.StartsWith("-") && !otherAlreadyWritten) {
                    sb.AppendLine();
                    sb.AppendLine("Other:");
                    otherAlreadyWritten = true;
                }
                sb.AppendLine(s + "\t" + Analyzer.PossibleArguments[s]);
                sb.AppendLine();
            }
            Console.WriteLine(sb.ToString().Trim());
        }

    }
}
