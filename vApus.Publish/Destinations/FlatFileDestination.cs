/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.IO;

namespace vApus.Publish {
    /// <summary>
    /// Writes to a file containing maximum 1 message.
    /// </summary>
    internal class FlatFileDestination : BaseDestination {
        private string _fileName, _directory;

        private FlatFileDestination() { }
        public FlatFileDestination(string fileName) {
            _fileName = fileName;
            _directory = Path.GetDirectoryName(_fileName);
        }

        public override void Post(object message) {
            if (!Directory.Exists(_directory)) Directory.CreateDirectory(_directory);

            using (var sw = new StreamWriter(_fileName, false)) {
                sw.WriteLine(FormatMessage(message));
                sw.Flush();
            }
        }
    }
}
