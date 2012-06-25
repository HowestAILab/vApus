/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Stresstest
{
    /// <summary>
    /// For saving the stresstest results as bytes on file.
    /// </summary>
    public class RFileHandler
    {
        #region Fields
        private object _lock = new object();
        private string _SlaveSideResultsDir = Path.Combine(Application.StartupPath, "SlaveSideResults");
        private string _fileName;
        private StresstestResults _stresstestResults;
        #endregion

        #region Properties
        public string FileName
        {
            get { return _fileName; }
        }
        public StresstestResults StresstestResults
        {
            get { return _stresstestResults; }
        }
        #endregion

        #region Functions

        #region Save
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stresstestResults"></param>
        /// <param name="fileName">If null, a unique one will be chosen.</param>
        public void SetStresstestResults(StresstestResults stresstestResults, string fileName = null)
        {
            _stresstestResults = stresstestResults;
            if (fileName == null)
            {
                _fileName = Path.Combine(_SlaveSideResultsDir, _stresstestResults.Stresstest.Replace(' ', '_').ReplaceInvalidWindowsFilenameChars('_') + ".r");
                if (!Directory.Exists(_SlaveSideResultsDir))
                    return;

                int i = 0;
                while (File.Exists(_fileName))
                    _fileName = Path.Combine(_SlaveSideResultsDir,  _stresstestResults.Stresstest.Replace(' ', '_').ReplaceInvalidWindowsFilenameChars('_') + new string('_', ++i) + ".r");
            }
            else
            {
                _fileName = fileName;
            }
        }
        /// <summary>
        /// Call SetStresstestResults() before this.
        /// </summary>
        /// <returns>The filename</returns>
        public string Save()
        {
            lock (_lock)
            {
            RetryDir:
                if (!Directory.Exists(_SlaveSideResultsDir))
                    try
                    {
                        Directory.CreateDirectory(_SlaveSideResultsDir);
                    }
                    catch
                    {
                        goto RetryDir;
                    }

                if (_fileName == null)
                    SetStresstestResults(_stresstestResults);

            RetryFile:
                try
                {
                    if (File.Exists(_fileName))
                        File.Delete(_fileName);
                    WriteObjectToFile(_stresstestResults, _fileName);
                }
                catch
                {
                    SetStresstestResults(_stresstestResults, _fileName);
                    goto RetryFile;
                }
                return _fileName;
            }
        }
        private void WriteObjectToFile(object obj, string fileName)
        {
            try
            {
                //Write to a memory stream first to be able to compress in memory,
                //then write to file.
                byte[] buffer = null;
                using (var ms = new MemoryStream(1))
                {
                    var bf = new BinaryFormatter();
                    bf.Serialize(ms, obj);
                    bf = null;
                    GC.Collect();

                    //Unused bytes can reside at the end of the byte array (solved setting the initial capacity I think), but this does not use up more memory, ToArray() and Read(...) make a copy of the buffer.
                    buffer = ms.GetBuffer();
                }
                GC.Collect();

                var qlz = new QuickLZ.Net.QuickLZ();
                var toWrite = qlz.Compress(buffer);
                buffer = null;
                qlz = null;
                GC.Collect();

                using (var fs = File.Open(fileName, FileMode.CreateNew))
                    fs.Write(toWrite, 0, toWrite.Length);
            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion

        #region Load
        public StresstestResults Load(string fileName)
        {
            lock (_lock)
            {
                _fileName = fileName;
                return ReadObjectFromFile(_fileName) as StresstestResults;
            }
        }
        private object ReadObjectFromFile(string fileName)
        {
            object o = null;
            try
            {
                //Read to a buffer to be able to decompress in memory.
                byte[] buffer = File.ReadAllBytes(fileName);
                GC.Collect();

                var qlz = new QuickLZ.Net.QuickLZ();
                byte[] toRead = qlz.Decompress(buffer);
                buffer = null;
                qlz = null;
                GC.Collect();

                //Make a new memory stream with the decompressed buffer to be able to deserialize.
                using (var ms = new MemoryStream(toRead))
                {
                    var bf = new BinaryFormatter();
                    o = bf.UnsafeDeserialize(ms, null);
                    bf = null;
                }
                toRead = null;
            }
            finally
            {
                GC.Collect();
            }
            return o;
        }
        #endregion

        #endregion
    }
}
