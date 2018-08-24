/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using FastColoredTextBoxNS;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vApus.Util {
    public static class DictionaryExtension {
        public static TKey GetKeyAt<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, int index) {
            if (index < 0)
                throw new IndexOutOfRangeException("index < 0");
            else if (index >= dictionary.Count)
                throw new IndexOutOfRangeException("Index is larger or equals the count of dictionary kvps.");

            IEnumerator<TKey> enumerator = dictionary.Keys.GetEnumerator();
            enumerator.Reset();
            int currentIndex = -1;
            while (currentIndex++ < index)
                enumerator.MoveNext();
            return enumerator.Current;
        }

        public static bool TryGetKey<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue value, out TKey key) {
            foreach (TKey k in dictionary.Keys)
                if (dictionary[k].Equals(value)) {
                    key = k;
                    return true;
                }
            key = default(TKey);
            return false;
        }
    }
    public static class AssemblyExtension {
        /// <summary>
        /// Gets a type by its non fully qualified name.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="typeName"></param>
        /// <returns>The type if found, otherwise null.</returns>
        public static Type GetTypeByName(this Assembly assembly, string typeName) {
            var cacheEntry = FunctionOutputCacheWrapper.FunctionOutputCache.GetOrAdd(MethodBase.GetCurrentMethod(), assembly, typeName);
            if (cacheEntry.ReturnValue == null) {
                Parallel.ForEach(assembly.GetTypes(), (t, loopState) => {
                    if (t.Name == typeName) {
                        cacheEntry.ReturnValue = t;
                        loopState.Break();
                    }
                });
            }
            return cacheEntry.ReturnValue as Type;
        }
    }
    public static class TimeSpanExtension {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="roundMillisToNearestSecond"></param>
        /// <param name="returnOnZero"></param>
        /// <returns></returns>
        public static string ToLongFormattedString(this TimeSpan timeSpan, bool roundMillisToNearestSecond, string returnOnZero = "--") {
            if (roundMillisToNearestSecond) timeSpan = RoundMillisToNearestSecond(timeSpan);

            if (timeSpan.TotalMilliseconds == 0d) return returnOnZero;

            var sb = new StringBuilder();
            if (timeSpan.Days != 0) {
                sb.Append(timeSpan.Days);
                sb.Append(" days");
            }
            if (timeSpan.Hours != 0) {
                if (sb.ToString().Length != 0) sb.Append(", ");
                sb.Append(timeSpan.Hours);
                sb.Append(" hours");
            }
            if (timeSpan.Minutes != 0) {
                if (sb.ToString().Length != 0) sb.Append(", ");
                sb.Append(timeSpan.Minutes);
                sb.Append(" minutes");
            }

            if (timeSpan.Seconds != 0) {
                if (sb.ToString().Length != 0) sb.Append(", ");
                sb.Append(timeSpan.Seconds);
                sb.Append(" seconds");
            }
            if (timeSpan.Milliseconds != 0) {
                if (sb.ToString().Length != 0) sb.Append(", ");
                sb.Append(timeSpan.Milliseconds);
                sb.Append(" milliseconds");
            }
            if (sb.ToString().Length == 0)
                return returnOnZero;

            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="roundMillisToNearestSecond"></param>
        /// <param name="returnOnZero"></param>
        /// <returns></returns>
        public static string ToShortFormattedString(this TimeSpan timeSpan, bool roundMillisToNearestSecond, string returnOnZero = "--") {
            if (roundMillisToNearestSecond) timeSpan = RoundMillisToNearestSecond(timeSpan);

            if (timeSpan.TotalMilliseconds == 0d) return returnOnZero;

            var sb = new StringBuilder();
            if (timeSpan.Days != 0) {
                sb.Append(timeSpan.Days);
                sb.Append(" d");
            }
            if (timeSpan.Hours != 0) {
                if (sb.ToString().Length != 0) sb.Append(", ");
                sb.Append(timeSpan.Hours);
                sb.Append(" h");
            }
            if (timeSpan.Minutes != 0) {
                if (sb.ToString().Length != 0) sb.Append(", ");
                sb.Append(timeSpan.Minutes);
                sb.Append(" m");
            }
            if (timeSpan.Seconds != 0) {
                if (sb.ToString().Length != 0) sb.Append(", ");
                sb.Append(timeSpan.Seconds);
                sb.Append(" s");
            }
            if (timeSpan.Milliseconds != 0) {
                if (sb.ToString().Length != 0) sb.Append(", ");
                sb.Append(timeSpan.Milliseconds);
                sb.Append(" ms");
            }
            if (sb.ToString().Length == 0)
                return returnOnZero;

            return sb.ToString();
        }

        private static TimeSpan RoundMillisToNearestSecond(TimeSpan timeSpan) {
            if (timeSpan.Milliseconds > 499)
                return timeSpan.Subtract(new TimeSpan(0, 0, 0, 0, timeSpan.Milliseconds))
                    .Add(new TimeSpan(0, 0, 1));
            return timeSpan.Subtract(new TimeSpan(0, 0, 0, 0, timeSpan.Milliseconds));
        }
    }

    public static class StringExtension {
        public static bool ContainsChars(this string s, params char[] values) {
            foreach (var value in values)
                if (!s.Contains(value))
                    return false;
            return true;
        }

        /// <summary>
        /// Determines if the string does or does not contain \,*,/,:,<,>,?,\ or |.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsValidWindowsFilenameString(this string s) {
            if (s == null || s.Length == 0)
                return false;
            foreach (char c in s)
                if (!c.IsValidWindowsFilenameChar())
                    return false;
            return true;
        }
        /// <summary>
        /// Replaces \,*,/,:,<,>,?,\ and | with the given character in a new string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="newChar"></param>
        /// <returns></returns>
        public static string ReplaceInvalidWindowsFilenameChars(this string s, char newChar) {
            var sb = new StringBuilder(s.Length);
            if (s == null)
                throw new ArgumentNullException("s");
            foreach (char c in s)
                if (c.IsValidWindowsFilenameChar())
                    sb.Append(c);
                else
                    sb.Append(newChar);
            return sb.ToString();
        }
        /// <summary>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string s) {
            ulong ul;
            double d;
            return ulong.TryParse(s, out ul) || double.TryParse(s, out d);
        }
        /// <summary>
        /// A simple way to encrypt a string.
        /// Example (don't use this): s.Encrypt("password", new byte[] { 0x59, 0x06, 0x59, 0x3e, 0x21, 0x4e, 0x55, 0x34, 0x96, 0x15, 0x11, 0x13, 0x72 });
        /// </summary>
        /// <param name="s"></param>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns>The encrypted string.</returns>
        public static string Encrypt(this string s, string password, byte[] salt) {
            var pdb = new PasswordDeriveBytes(password, salt);
            byte[] encrypted = Encrypt(System.Text.Encoding.Unicode.GetBytes(s), pdb.GetBytes(32), pdb.GetBytes(16));
            return Convert.ToBase64String(encrypted);
        }
        private static byte[] Encrypt(byte[] toEncrypt, byte[] key, byte[] IV) {
            var ms = new MemoryStream();
            var alg = Rijndael.Create();
            alg.Key = key;
            alg.IV = IV;
            //alg.Padding = PaddingMode.None;

            var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(toEncrypt, 0, toEncrypt.Length);
            cs.Close();
            return ms.ToArray();
        }
        /// <summary>
        /// A simple way to decrypt a string.
        /// Example (don't use this): s.Decrypt("password", new byte[] { 0x59, 0x06, 0x59, 0x3e, 0x21, 0x4e, 0x55, 0x34, 0x96, 0x15, 0x11, 0x13, 0x72 });
        /// </summary>
        /// <param name="s"></param>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns>The decrypted string.</returns>
        public static string Decrypt(this string s, string password, byte[] salt) {
            var pdb = new PasswordDeriveBytes(password, salt);
            byte[] decrypted = Decrypt(Convert.FromBase64String(s), pdb.GetBytes(32), pdb.GetBytes(16));
            return System.Text.Encoding.Unicode.GetString(decrypted);
        }
        private static byte[] Decrypt(byte[] toDecrypt, byte[] Key, byte[] IV) {
            var ms = new MemoryStream();
            var alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            //alg.Padding = PaddingMode.None;

            var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(toDecrypt, 0, toDecrypt.Length);
            try {
                cs.Close();
            }
            catch {
                //Don't care.
            }
            return ms.ToArray();
        }
        public static string Reverse(this string s) {
            var sb = new StringBuilder(s.Length); ;
            for (int i = s.Length - 1; i != -1; i--)
                sb.Append(s[i]);

            return sb.ToString();
        }

        /// <summary>
        /// If the string is a binary representation of an object you can use this class to make an object out of it.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static object ToByteArrayToObject(this string s, string separator = ",") {
            string[] split = s.Split(new string[] { separator }, StringSplitOptions.None);
            byte[] buffer = new byte[split.Length];
            for (int i = 0; i != split.Length; i++)
                buffer[i] = byte.Parse(split[i]);

            object o = null;
            using (var ms = new MemoryStream(buffer)) {
                var bf = new BinaryFormatter();
                o = bf.UnsafeDeserialize(ms, null);
                bf = null;
            }
            buffer = null;

            return o;
        }
    }
    public static class CharExtension {
        /// <summary>
        /// Determines if the char is or is not \,*,/,:,<,>,?,\ or |.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsValidWindowsFilenameChar(this char c) {
            switch ((int)c) {
                case 34:  // '\"'
                case 42:  // '*'
                case 47:  // '/'
                case 58:  // ':'
                case 60:  // '<'
                case 62:  // '>'
                case 63:  // '?'
                case 92:  // '\\'
                case 124: // '|'
                    return false;
            }
            return true;
        }
    }
    public static class ObjectExtension {
        //Nifty hack to make this work everywhere (also in derived types when shallow copying).
        //Having just a static field for tag and parent doesn't work, they will be the same for every object you assign them.
        //Do not use this for primary datatypes (strings included) except if you do something like this:
        //Object o = 1;
        [NonSerialized]
        private static ConcurrentDictionary<object, object> _tags = new ConcurrentDictionary<object, object>();
        [NonSerialized]
        private static ConcurrentDictionary<object, object> _parents = new ConcurrentDictionary<object, object>();
        [NonSerialized]
        private static ConcurrentDictionary<object, object> _descriptions = new ConcurrentDictionary<object, object>();
        /// <summary>
        ///Nifty hack to make this work everywhere (also in derived types when shallow copying).
        ///Having just a static field for tag and parent doesn't work, they will be the same for every object you assign them.
        ///Do not use this for primary datatypes (strings included) except if you do something like this:
        ///Object o = 1;
        /// </summary>
        /// <param name="o"></param>
        /// <param name="tag"></param>
        public static void SetTag(this object o, object tag) { Set(_tags, o, tag); }
        public static object GetTag(this object o) { return Get(_tags, o); }

        /// <summary>
        ///Nifty hack to make this work everywhere (also in derived types when shallow copying).
        ///Having just a static field for tag and parent doesn't work, they will be the same for every object you assign them.
        ///Do not use this for primary datatypes (strings included) except if you do something like this:
        ///Object o = 1;
        /// </summary>
        /// <param name="o"></param>
        /// <param name="parent"></param>
        public static void SetParent(this object o, object parent) {
            Set(_parents, o, parent);
        }
        public static object GetParent(this object o) { return Get(_parents, o); }

        public static void SetDescription(this object o, string description) { Set(_descriptions, o, description); }
        public static string GetDescription(this object o) { return Get(_descriptions, o) as string; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o">Child</param>
        /// <returns>True if the object was removed.</returns>
        public static bool RemoveParent(this object o) { return Remove(_parents, o); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns>True if the object was removed.</returns>
        public static bool RemoveTag(this object o) { return Remove(_tags, o); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns>True if the object was removed.</returns>
        public static bool RemoveDescription(this object o) { return Remove(_descriptions, o); }
        /// <summary>
        /// This does not invoke parent changed.
        /// </summary>
        /// <returns>True if the cache was not empty.</returns>
        public static bool ClearCache() {
            bool cleared = _tags.Count != 0 || _parents.Count != 0 || _descriptions.Count != 0;

            _tags.Clear();
            _parents.Clear();
            _descriptions.Clear();

            return cleared;
        }

        private static void Set(ConcurrentDictionary<object, object> dict, object key, object value) {
            if (key != null)
                if (value == null) {
                    object val;
                    dict.TryRemove(key, out val);
                }
                else {
                    dict.AddOrUpdate(key, value, (k, oldvalue) => value);
                }
        }

        private static object Get(ConcurrentDictionary<object, object> dict, object key) {
            if (key == null) return null;

            object value;
            dict.TryGetValue(key, out value);
            return value;
        }
        private static bool Remove(ConcurrentDictionary<object, object> dict, object key) {
            if (key == null) return false;
            object val;
            return dict.TryRemove(key, out val);
        }

        /// <summary>
        /// Returns the string representation of the serialized object --> Must be serializable!
        /// </summary>
        /// <param name="o"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToBinaryToString(this object o, string separator = ",") {
            byte[] buffer = null;
            using (var ms = new MemoryStream(1)) {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, o);
                bf = null;

                buffer = ms.GetBuffer();
            }
            string s = buffer.Combine(separator);
            buffer = null;

            return s;
        }
    }
    public static class DataGridViewExtension {
        public static void DoubleBuffered(this DataGridView dgv, bool doubleBuffered) {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, doubleBuffered, null);
        }
    }
    public static class DataGridViewRowExtension {
        /// <summary>
        /// To CSV for example.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToSV(this DataGridViewRow row, string separator) {
            if (row.Cells.Count == 0) return string.Empty;
            if (row.Cells.Count == 1) return row.Cells[0].Value.ToString();

            var sb = new StringBuilder();
            for (int i = 0; i != row.Cells.Count - 1; i++) {
                sb.Append(row.Cells[i].Value);
                sb.Append(separator);
            }
            sb.Append(row.Cells[row.Cells.Count - 1].Value);
            return sb.ToString();
        }
    }
    public static class ArrayExtension {
        /// <summary>
        /// Combine a one-dimensional array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Combine(this Array array, string separator, params object[] exclude) {
            lock (array.SyncRoot) {
                if (array.Length == 0) return string.Empty;

                var sb = new StringBuilder();
                object value;
                for (int i = 0; i != array.Length - 1; i++) {
                    value = array.GetValue(i);
                    if (exclude == null || !exclude.Contains(value)) {
                        sb.Append(value);
                        sb.Append(separator);
                    }
                }
                value = array.GetValue(array.Length - 1);
                if (exclude == null || !exclude.Contains(value)) sb.Append(value);

                return sb.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="item">Case sensitive in case of strings.</param>
        /// <returns>-1 if not found</returns>
        public static int IndexOf(this Array array, object item) {
            int i = 0;
            foreach (var o in array) {
                if (o.Equals(item))
                    return i;
                ++i;
            }
            return -1;
        }
    }
    public static class ConcurrentBagExtension {
        /// <summary>
        /// A thread safe implementation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="concurrentBag"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool Contains<T>(this ConcurrentBag<T> concurrentBag, T item) {
            if (concurrentBag.GetTag() == null)
                concurrentBag.SetTag(new object());
            lock (concurrentBag.GetTag()) {
                foreach (T i in concurrentBag)
                    if (i.Equals(item))
                        return true;
                return false;
            }
        }
    }
    public static class ListExtension {
        public static string Combine<T>(this List<T> list, string separator, params object[] exclude) {
            if (list.Count == 0) return string.Empty;

            var sb = new StringBuilder();
            object value;
            for (int i = 0; i != list.Count - 1; i++) {
                value = list[i];
                if (exclude == null || !exclude.Contains(value)) {
                    sb.Append(value);
                    sb.Append(separator);
                }
            }
            value = list[list.Count - 1];
            if (exclude == null || !exclude.Contains(value)) sb.Append(value);

            return sb.ToString();
        }
    }
    public static class DataTableExtension {
        /// <summary>
        /// Convert a data table to json using Newtonsoft.Json.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string ToJson(this DataTable table) {
            Type type = table.GetType();

            var json = new JsonSerializer();

            json.NullValueHandling = NullValueHandling.Ignore;

            json.ObjectCreationHandling = ObjectCreationHandling.Replace;
            json.MissingMemberHandling = MissingMemberHandling.Ignore;
            json.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;


            json.Converters.Add(new DataTableConverter());

            var sw = new StringWriter();
            var writer = new JsonTextWriter(sw);
            writer.Formatting = Formatting.None;

            writer.QuoteChar = '"';
            json.Serialize(writer, table);

            string output = sw.ToString();
            writer.Close();
            sw.Close();

            return output;
        }
        /// <summary>
        /// Convert json text to a data table using Newtonsoft.Json.
        /// </summary>
        /// <param name="jsonText"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this string jsonText) {
            var json = new JsonSerializer();

            json.NullValueHandling = NullValueHandling.Ignore;
            json.ObjectCreationHandling = ObjectCreationHandling.Replace;
            json.MissingMemberHandling = MissingMemberHandling.Ignore;
            json.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            var sr = new StringReader(jsonText);
            JsonTextReader reader = new JsonTextReader(sr);
            DataTable table = json.Deserialize<DataTable>(reader);
            reader.Close();

            return table;
        }
    }
    public static class RichTextBoxExtension {
        /// <summary>
        /// Select all, Cut, Copy, Paste
        /// </summary>
        /// <param name="rtxt"></param>
        /// <param name="enable">Do enable only once</param>
        public static void DefaultContextMenu(this RichTextBox rtxt, bool enable) {
            if (enable) {
                rtxt.MouseUp += rtxt_MouseUp;
                rtxt.Disposed += rtxt_Disposed;
            }
            else {
                rtxt.MouseUp -= rtxt_MouseUp;
                rtxt.Disposed -= rtxt_Disposed;
            }
        }

        static void rtxt_Disposed(object sender, EventArgs e) {
            var rtxt = sender as RichTextBox;
            rtxt.MouseUp -= rtxt_MouseUp;
        }

        private static void rtxt_MouseUp(object sender, MouseEventArgs e) {
            var rtxt = sender as RichTextBox;

            var contextMenu = new ContextMenu();
            var menuItem = new MenuItem("Select all");
            menuItem.Click += new EventHandler((s, a) => SelectAll(rtxt));
            contextMenu.MenuItems.Add(menuItem);

            if (rtxt.Enabled && !rtxt.ReadOnly) {
                menuItem = new MenuItem("Cut");
                menuItem.Click += new EventHandler((s, a) => Cut(rtxt));
                contextMenu.MenuItems.Add(menuItem);
            }

            menuItem = new MenuItem("Copy");
            menuItem.Click += new EventHandler((s, a) => Copy(rtxt));
            contextMenu.MenuItems.Add(menuItem);

            if (rtxt.Enabled && !rtxt.ReadOnly) {
                menuItem = new MenuItem("Paste");
                menuItem.Click += new EventHandler((s, a) => Paste(rtxt));
                contextMenu.MenuItems.Add(menuItem);
            }

            rtxt.ContextMenu = contextMenu;
        }
        private static void SelectAll(RichTextBox rtxt) { rtxt.SelectAll(); }
        private static void Cut(RichTextBox rtxt) {
            Copy(rtxt);
            rtxt.SelectedText = string.Empty;
        }
        private static void Copy(RichTextBox rtxt) {
            Clipboard.SetData(DataFormats.UnicodeText, rtxt.SelectedText);
        }
        private static void Paste(RichTextBox rtxt) {
            if (Clipboard.ContainsText(TextDataFormat.Rtf))
                rtxt.SelectedRtf = Clipboard.GetData(DataFormats.Rtf).ToString();
            else if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
                rtxt.SelectedText = Clipboard.GetData(DataFormats.UnicodeText).ToString();
        }

    }

    public static class FastColoredTextBoxExtension {
        /// <summary>
        /// Select all, Cut, Copy, Paste
        /// </summary>
        /// <param name="fctxt"></param>
        /// <param name="enable">Do enable only once</param>
        public static void DefaultContextMenu(this FastColoredTextBox fctxt, bool enable) {
            if (enable) {
                fctxt.MouseUp += fctxt_MouseUp;
                fctxt.Disposed += fctxt_Disposed;
            }
            else {
                fctxt.MouseUp -= fctxt_MouseUp;
                fctxt.Disposed -= fctxt_Disposed;
            }
        }

        static void fctxt_Disposed(object sender, EventArgs e) {
            var fctxt = sender as FastColoredTextBox;
            fctxt.MouseUp -= fctxt_MouseUp;
        }

        private static void fctxt_MouseUp(object sender, MouseEventArgs e) {
            var fctxt = sender as FastColoredTextBox;

            var contextMenu = new ContextMenu();
            var menuItem = new MenuItem("Select all");
            menuItem.Click += new EventHandler((s, a) => SelectAll(fctxt));
            contextMenu.MenuItems.Add(menuItem);

            if (fctxt.Enabled && !fctxt.ReadOnly) {
                menuItem = new MenuItem("Cut");
                menuItem.Click += new EventHandler((s, a) => Cut(fctxt));
                contextMenu.MenuItems.Add(menuItem);
            }

            menuItem = new MenuItem("Copy");
            menuItem.Click += new EventHandler((s, a) => Copy(fctxt));
            contextMenu.MenuItems.Add(menuItem);

            if (fctxt.Enabled && !fctxt.ReadOnly) {
                menuItem = new MenuItem("Paste");
                menuItem.Click += new EventHandler((s, a) => Paste(fctxt));
                contextMenu.MenuItems.Add(menuItem);
            }

            menuItem = new MenuItem("Toggle word wrap");
            menuItem.Click += new EventHandler((s, a) => ToggleWordWrap(fctxt));
            contextMenu.MenuItems.Add(menuItem);

            fctxt.ContextMenu = contextMenu;
        }
        private static void SelectAll(FastColoredTextBox fctxt) { fctxt.SelectAll(); }
        private static void Cut(FastColoredTextBox fctxt) {
            Copy(fctxt);
            fctxt.SelectedText = string.Empty;
        }
        private static void Copy(FastColoredTextBox fctxt) { Clipboard.SetData(DataFormats.UnicodeText, fctxt.SelectedText); }
        private static void Paste(FastColoredTextBox fctxt) {
            if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
                fctxt.SelectedText = Clipboard.GetData(DataFormats.UnicodeText).ToString();
        }
        private static void ToggleWordWrap(FastColoredTextBox fctxt) { fctxt.WordWrap = !fctxt.WordWrap; }

    }
}