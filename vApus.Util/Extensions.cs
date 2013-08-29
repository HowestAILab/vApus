/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
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
using System.Windows.Forms;

namespace vApus.Util {
    public static class AssemblyExtension {
        private static readonly object _lock = new object();
        /// <summary>
        /// Gets a type by its type name.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="typeName"></param>
        /// <returns>The type if found, otherwise null.</returns>
        public static Type GetTypeByName(this Assembly assembly, string typeName) {
            lock (_lock) {
                foreach (Type t in assembly.GetTypes())
                    if (t.Name == typeName)
                        return t;
                return null;
            }
        }
    }
    public static class TypeExtension {
        private static readonly object _lock = new object();
        /// <summary>
        /// To check if a type has an indirect base type (given)
        /// </summary>
        /// <param name="t"></param>
        /// <param name="BaseType"></param>
        /// <returns></returns>
        public static bool HasBaseType(this Type t, Type BaseType) {
            lock (_lock) {
                Type type = t;
                while (type.BaseType != null) {
                    if (type.BaseType == BaseType)
                        return true;
                    type = type.BaseType;
                }
                return false;
            }
        }
    }
    public static class TimeSpanExtension {
        private static readonly object _lock = new object();

        public static string ToLongFormattedString(this TimeSpan timeSpan) {
            lock (_lock) {
                StringBuilder sb = new StringBuilder();
                if (timeSpan.Days != 0) {
                    sb.Append(timeSpan.Days);
                    sb.Append(" days");
                }
                if (timeSpan.Hours != 0) {
                    if (sb.ToString().Length != 0)
                        sb.Append(", ");
                    sb.Append(timeSpan.Hours);
                    sb.Append(" hours");
                }
                if (timeSpan.Minutes != 0) {
                    if (sb.ToString().Length != 0)
                        sb.Append(", ");
                    sb.Append(timeSpan.Minutes);
                    sb.Append(" minutes");
                }
                if (timeSpan.Seconds != 0) {
                    if (sb.ToString().Length != 0)
                        sb.Append(", ");
                    sb.Append(timeSpan.Seconds);
                    sb.Append(" seconds");
                }
                if (timeSpan.Milliseconds != 0 || sb.ToString().Length == 0) {
                    if (sb.ToString().Length != 0)
                        sb.Append(", ");
                    sb.Append(timeSpan.Milliseconds);
                    sb.Append(" milliseconds");
                }
                return sb.ToString();
            }
        }
        public static string ToShortFormattedString(this TimeSpan timeSpan) {
            lock (_lock) {
                StringBuilder sb = new StringBuilder();
                if (timeSpan.Days != 0) {
                    sb.Append(timeSpan.Days);
                    sb.Append(" d");
                }
                if (timeSpan.Hours != 0) {
                    if (sb.ToString().Length != 0)
                        sb.Append(", ");
                    sb.Append(timeSpan.Hours);
                    sb.Append(" h");
                }
                if (timeSpan.Minutes != 0) {
                    if (sb.ToString().Length != 0)
                        sb.Append(", ");
                    sb.Append(timeSpan.Minutes);
                    sb.Append(" m");
                }
                if (timeSpan.Seconds != 0) {
                    if (sb.ToString().Length != 0)
                        sb.Append(", ");
                    sb.Append(timeSpan.Seconds);
                    sb.Append(" s");
                }
                if (timeSpan.Milliseconds != 0 || sb.ToString().Length == 0) {
                    if (sb.ToString().Length != 0)
                        sb.Append(", ");
                    sb.Append(timeSpan.Milliseconds);
                    sb.Append(" ms");
                }
                return sb.ToString();
            }
        }
    }
    public static class StringExtension {
        private static readonly object _lock = new object();

        public static bool ContainsChars(this string s, params char[] values) {
            lock (_lock) {
                foreach (var value in values)
                    if (!s.Contains(value))
                        return false;
                return true;
            }
        }
        public static string RemoveChars(this string s, params char[] values) {
            lock (_lock) {
                StringBuilder sb = new StringBuilder();
                foreach (char c in s)
                    if (!values.Contains(c))
                        sb.Append(c);
                return sb.ToString();
            }
        }
        public static bool ContainsStrings(this string s, params string[] values) {
            lock (_lock) {
                foreach (var value in values)
                    if (!s.Contains(value))
                        return false;
                return true;
            }
        }

        /// <summary>
        /// Determines if the string does or does not contain \,*,/,:,<,>,?,\ or |.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsValidWindowsFilenameString(this string s) {
            lock (_lock) {
                if (s == null || s.Length == 0)
                    return false;
                foreach (char c in s)
                    if (!c.IsValidWindowsFilenameChar())
                        return false;
                return true;
            }
        }
        /// <summary>
        /// Replaces \,*,/,:,<,>,?,\ and | with the given character in a new string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="newChar"></param>
        /// <returns></returns>
        public static string ReplaceInvalidWindowsFilenameChars(this string s, char newChar) {
            lock (_lock) {
                StringBuilder sb = new StringBuilder(s.Length);
                if (s == null)
                    throw new ArgumentNullException("s");
                foreach (char c in s)
                    if (c.IsValidWindowsFilenameChar())
                        sb.Append(c);
                    else
                        sb.Append(newChar);
                return sb.ToString();
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string s) {
            lock (_lock) {
                ulong ul;
                double d;
                return ulong.TryParse(s, out ul) || double.TryParse(s, out d);
            }
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
            lock (_lock) {
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, salt);
                byte[] encrypted = Encrypt(System.Text.Encoding.Unicode.GetBytes(s), pdb.GetBytes(32), pdb.GetBytes(16));
                return Convert.ToBase64String(encrypted);
            }
        }
        private static byte[] Encrypt(byte[] toEncrypt, byte[] key, byte[] IV) {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
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
            lock (_lock) {
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, salt);
                byte[] decrypted = Decrypt(Convert.FromBase64String(s), pdb.GetBytes(32), pdb.GetBytes(16));
                return System.Text.Encoding.Unicode.GetString(decrypted);
            }
        }
        private static byte[] Decrypt(byte[] toDecrypt, byte[] Key, byte[] IV) {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(toDecrypt, 0, toDecrypt.Length);
            try {
                cs.Close();
            } catch { }
            return ms.ToArray();
        }

        public static ListViewItem Parse(this string s, char delimiter) {
            lock (_lock) {
                ListViewItem item = new ListViewItem();
                string[] split = s.Split(delimiter);
                item.Text = split[0];
                for (int i = 1; i < split.Length; i++)
                    item.SubItems.Add(split[i]);
                return item;
            }
        }

        public static string Reverse(this string s) {
            lock (_lock) {
                StringBuilder sb = new StringBuilder(s.Length); ;
                for (int i = s.Length - 1; i != -1; i--)
                    sb.Append(s[i]);

                return sb.ToString();
            }
        }

        /// <summary>
        /// If the string is a binary representation of an object you can use this class to make an object out of it.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static object ToByteArrayToObject(this string s, string separator = ",") {
            lock (_lock) {
                string[] split = s.Split(new string[] { separator }, StringSplitOptions.None);
                byte[] buffer = new byte[split.Length];
                for (int i = 0; i != split.Length; i++)
                    buffer[i] = byte.Parse(split[i]);

                object o = null;
                using (var ms = new MemoryStream(buffer)) {
                    BinaryFormatter bf = new BinaryFormatter();
                    o = bf.UnsafeDeserialize(ms, null);
                    bf = null;
                }
                buffer = null;

                return o;
            }
        }
    }
    public static class CharExtension {
        private static readonly object _lock = new object();
        /// <summary>
        /// Determines if the char is or is not \,*,/,:,<,>,?,\ or |.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsValidWindowsFilenameChar(this char c) {
            lock (_lock) {
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
        /// <summary></summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsLetter(this char c) {
            lock (_lock) {
                int i = (int)c;
                return ((i > 64 && i < 91) || (i > 96 && i < 123));
            }
        }
        /// <summary></summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsDigit(this char c) {
            lock (_lock) {
                int i = (int)c;
                return (i > 47 && i < 58);
            }
        }
    }
    public static class ObjectExtension {
        public delegate void ParentChangedEventHandler(ParentOrTagChangedEventArgs parentOrTagChangedEventArgs);
        public delegate void TagChangedEventHandler(ParentOrTagChangedEventArgs parentOrTagChangedEventArgs);

        public static event ParentChangedEventHandler ParentChanged;

        //Nifty hack to make this work everywhere (also in derived types when shallow copying).
        //Having just a static field for tag and parent doesn't work, they will be the same for every object you assign them.
        //Do not use this for primary datatypes (strings included) except if you do something like this:
        //Object o = 1;
        [NonSerialized]
        private static Hashtable _tags = new Hashtable();
        [NonSerialized]
        private static Hashtable _parents = new Hashtable();
        [NonSerialized]
        private static Hashtable _descriptions = new Hashtable();
        /// <summary>
        ///Nifty hack to make this work everywhere (also in derived types when shallow copying).
        ///Having just a static field for tag and parent doesn't work, they will be the same for every object you assign them.
        ///Do not use this for primary datatypes (strings included) except if you do something like this:
        ///Object o = 1;
        /// </summary>
        /// <param name="o"></param>
        /// <param name="tag"></param>
        public static void SetTag(this object o, object tag) {
            lock (_tags.SyncRoot)
                if (o != null)
                    if (_tags.Contains(o))
                        if (tag == null) _tags.Remove(o); else _tags[o] = tag;
                    else if (tag != null) _tags.Add(o, tag);
        }
        public static object GetTag(this object o) {
            //Threadsafe for reader threads.
            if (o == null) return null;
            return _tags.Contains(o) ? _tags[o] : null;
        }
        /// <summary>
        ///Nifty hack to make this work everywhere (also in derived types when shallow copying).
        ///Having just a static field for tag and parent doesn't work, they will be the same for every object you assign them.
        ///Do not use this for primary datatypes (strings included) except if you do something like this:
        ///Object o = 1;
        /// </summary>
        /// <param name="o"></param>
        /// <param name="parent"></param>
        public static void SetParent(this object o, object parent, bool invokeParentChanged = true) {
            lock (_parents.SyncRoot)
                if (o != null) {
                    object previous = null;

                    if (_parents.Contains(o)) {
                        previous = _parents[o];
                        if (parent == null) _parents.Remove(o);
                        else if (previous != null && !previous.Equals(parent)) _parents[o] = parent; else return;
                    } else {
                        if (parent == null) return;
                        _parents.Add(o, parent);
                    }

                    if (invokeParentChanged && ParentChanged != null)
                        foreach (ParentChangedEventHandler del in ParentChanged.GetInvocationList())
                            del.BeginInvoke(new ParentOrTagChangedEventArgs(o, previous, parent), null, null);
                }
        }
        public static object GetParent(this object o) {
            //Threadsafe for reader threads.
            if (o == null) return null;
            return _parents.Contains(o) ? _parents[o] : null;
        }

        public static void SetDescription(this object o, string description) {
            lock (_descriptions.SyncRoot)
                if (o != null)
                    if (_descriptions.Contains(o))
                        if (description == null) _descriptions.Remove(o); else _descriptions[o] = description;
                    else if (description != null) _descriptions.Add(o, description);
        }
        public static string GetDescription(this object o) {
            //Threadsafe for reader threads.
            if (o == null) return null;
            return (_descriptions.Contains(o) ? _descriptions[o] : null) as string;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o">Child</param>
        /// <returns>True if the object was removed.</returns>
        public static bool RemoveParent(this object o, bool invokeParentChanged = true) {
            lock (_parents.SyncRoot) {
                bool removed = false;
                if (_parents.Contains(o)) {
                    object parent = _parents[o];

                    _parents.Remove(o);
                    removed = true;

                    if (invokeParentChanged && ParentChanged != null)
                        foreach (ParentChangedEventHandler del in ParentChanged.GetInvocationList())
                            del.BeginInvoke(new ParentOrTagChangedEventArgs(o, parent, null), null, null);
                }
                return removed;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns>True if the object was removed.</returns>
        public static bool RemoveTag(this object o) {
            lock (_tags.SyncRoot) {
                if (_tags.Contains(o)) {
                    _tags.Remove(o);
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns>True if the object was removed.</returns>
        public static bool RemoveDescription(this object o) {
            lock (_descriptions.SyncRoot) {
                if (_descriptions.Contains(o)) {
                    _descriptions.Remove(o);
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// This does not invoke parent changed.
        /// </summary>
        /// <returns>True if the cache was not empty.</returns>
        public static bool ClearCache() {
            lock (_tags.SyncRoot) {
                bool cleared = _tags.Count != 0 || _parents.Count != 0 || _descriptions.Count != 0;

                _tags.Clear();
                _parents.Clear();
                _descriptions.Clear();
                return cleared;
            }
        }

        public class ParentOrTagChangedEventArgs : EventArgs {
            public object Child, Previous, New;

            public ParentOrTagChangedEventArgs(object child, object previous, object __new) {
                Child = child;
                Previous = previous;
                New = __new;
            }
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
                BinaryFormatter bf = new BinaryFormatter();
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
        private static readonly object _lock = new object();
        public static void DoubleBuffered(this DataGridView dgv, bool doubleBuffered) {
            lock (_lock) {
                Type dgvType = dgv.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(dgv, doubleBuffered, null);
            }
        }
    }
    public static class DataGridViewRowExtension {
        private static readonly object _lock = new object();
        /// <summary>
        /// To CSV for example.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToSV(this DataGridViewRow row, string separator) {
            lock (_lock) {
                if (row.Cells.Count == 0) return string.Empty;
                if (row.Cells.Count == 1) return row.Cells[0].Value.ToString();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i != row.Cells.Count - 1; i++) {
                    sb.Append(row.Cells[i].Value);
                    sb.Append(separator);
                }
                sb.Append(row.Cells[row.Cells.Count - 1].Value);
                return sb.ToString();
            }
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
        private static readonly object _lock = new object();
        public static void AddRange<T>(this List<T> list, T item1, params T[] items) {
            lock (_lock) {
                list.Add(item1);
                foreach (T item in items) list.Add(item);
            }
        }
        public static string Combine<T>(this List<T> list, string separator, params object[] exclude) {
            lock (_lock) {
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
    }
    public static class DataColumnCollectionExtension {
        private static readonly object _lock = new object();
        public static void AddRange(this DataColumnCollection dataColumnCollection, string column1, params string[] columns) {
            lock (_lock) {
                dataColumnCollection.Add(column1);
                foreach (string column in columns)
                    dataColumnCollection.Add(column);
            }
        }
    }
}