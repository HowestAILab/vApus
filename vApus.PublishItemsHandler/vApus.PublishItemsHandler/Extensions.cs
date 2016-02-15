/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace vApus.Util {
    public static class StringExtension {
        /// <summary>
        /// A simple way to encrypt a string.
        /// Example (don't use this): s.Encrypt("password", new byte[] { 0x59, 0x06, 0x59, 0x3e, 0x21, 0x4e, 0x55, 0x34, 0x96, 0x15, 0x11, 0x13, 0x72 });
        /// </summary>
        /// <param name="s"></param>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns>The encrypted string.</returns>
        public static string Encrypt(this string s, string password, byte[] salt) {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, salt);
            byte[] encrypted = Encrypt(System.Text.Encoding.Unicode.GetBytes(s), pdb.GetBytes(32), pdb.GetBytes(16));
            return Convert.ToBase64String(encrypted);
        }
        private static byte[] Encrypt(byte[] toEncrypt, byte[] key, byte[] IV) {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = key;
            alg.IV = IV;
            //alg.Padding = PaddingMode.None;

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
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, salt);
            byte[] decrypted = Decrypt(Convert.FromBase64String(s), pdb.GetBytes(32), pdb.GetBytes(16));
            return System.Text.Encoding.Unicode.GetString(decrypted);
        }
        private static byte[] Decrypt(byte[] toDecrypt, byte[] Key, byte[] IV) {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            //alg.Padding = PaddingMode.None;

            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(toDecrypt, 0, toDecrypt.Length);
            try {
                cs.Close();
            } catch {
                //Don't care.
            }
            return ms.ToArray();
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

}