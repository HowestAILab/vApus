/*
 * Copyright 2012 (c) Vandroemme Dieter
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Vandroemme Dieter
 */

/*
 * Thanks to Andy Taylor and Trustin Lee of the Netty Project http://www.jboss.org/netty/
 * http://docs.jboss.org/netty/3.2/xref/org/jboss/netty/handler/codec/http/CookieDecoder.html
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace vApus.Util {
    public class CookieDecoder {
        private const string PATTERN =
            "(?:\\s|[;])*\\$*([^;=]+)(?:=(?:[\"']((?:\\\\.|[^\"])*)[\"']|([^;]*)))?(\\s*(?:[;]+\\s*|$))";

        /// <summary>
        ///     Parse cookies from a header starting with Set-Cookie.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public List<Cookie> Decode(string header) {
            var cookies = new List<Cookie>();
            foreach (string unparsedCookie in SplitHeaderInUnparsedCookies(header)) {
                List<string> names = new List<string>(8), values = new List<string>(8);
                ExtractKeyValuePairs(unparsedCookie, names, values);
                if (names.Count != 0) {
                    int i = 0, version = 0;
                    //Version is the only attribute that can appear before the name of the cookie.
                    if (names[0].ToLowerInvariant() == "version") {
                        int.TryParse(values[0], out version);
                        i = 1;
                    }
                    //Don't do anything if there is just a version attribute.
                    if (names.Count != 1) {
                        Cookie cookie = null;
                        for (; i != names.Count; i++) {
                            string name = names[i];
                            string lowerCaseName = name.ToLowerInvariant();
                            string value = values[i];
                            if (value == null)
                                value = string.Empty;

                            if (cookie == null)
                                cookie = new Cookie(name, value);

                            bool discard = false, secure = false, httpOnly = false;
                            string comment = null, domain = null, path = null;
                            Uri commentUri = null;
                            DateTime expires = DateTime.MaxValue;
                            var ports = new List<string>(2);

                            for (int j = i + 1; j < names.Count; j++, i++) {
                                name = names[j];
                                lowerCaseName = name.ToLowerInvariant();
                                value = values[j];

                                if (lowerCaseName == "discard") discard = true;
                                else if (lowerCaseName == "secure") secure = true;
                                else if (lowerCaseName == "httponly") httpOnly = true;
                                else if (lowerCaseName == "comment") comment = value;
                                else if (lowerCaseName == "commenturl")
                                    try {
                                        commentUri = new Uri(value);
                                    } catch {
                                    } else if (lowerCaseName == "domain")
                                    domain = value;
                                else if (lowerCaseName == "path") path = value;
                                else if (lowerCaseName == "expires") DateTime.TryParse(value, out expires);
                                else if (lowerCaseName == "version") int.TryParse(value, out version);
                                else if (lowerCaseName == "port") ports.AddRange(value.Split(','));
                                else break;
                            }

                            cookie.Version = version;
                            if (expires != DateTime.MaxValue)
                                cookie.Expires = expires;
                            cookie.Path = path;
                            cookie.Domain = domain;
                            cookie.Secure = secure;
                            cookie.HttpOnly = httpOnly;
                            if (version > 0)
                                cookie.Comment = comment;
                            if (version > 1) {
                                if (commentUri != null)
                                    cookie.CommentUri = commentUri;
                                if (ports.Count != 0)
                                    cookie.Port = ports[0];
                                cookie.Discard = discard;
                            }
                        }
                        if (cookie != null)
                            cookies.Add(cookie);
                    }
                }
            }
            return cookies;
        }

        /// <summary>
        ///     Takes values with comma's into account.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private List<string> SplitHeaderInUnparsedCookies(string header) {
            var l = new List<string>();
            int indexOfComma = header.IndexOf(",");
            string spacedSubSection = string.Empty;
            while (indexOfComma != -1) {
                int oneFurther = indexOfComma + 1;
                if (oneFurther != header.Length) {
                    if (header[oneFurther] == ' ' || header[oneFurther] == '\t') {
                        spacedSubSection += header.Substring(0, oneFurther);
                    } else {
                        l.Add(spacedSubSection + header.Substring(0, indexOfComma));
                        spacedSubSection = string.Empty;
                    }
                    header = header.Substring(oneFurther);
                }

                indexOfComma = header.IndexOf(",");
            }
            if ((spacedSubSection.Length != 0 && spacedSubSection != ",") || (header.Length != 0 && header != ","))
                l.Add(spacedSubSection + header);
            return l;
        }

        private void ExtractKeyValuePairs(string header, List<string> names, List<string> values) {
            string name = null, value = null, separator = null;
            foreach (Match m in Regex.Matches(header, PATTERN)) {
                string newName = m.Groups[1].Value;
                string newValue = m.Groups[3].Value;
                if (newValue == null)
                    newValue = DecodeValue(m.Groups[2].Value);
                string newSeparator = m.Groups[4].Value;

                if (name == null) {
                    name = newName;
                    value = newValue == null ? string.Empty : newValue;
                    separator = newSeparator;
                    continue;
                }

                if (newValue == null) {
                    string lowerCaseName = newName.ToLowerInvariant();
                    if (newName != "discard" && newName != "secure" && newName != "httponly") {
                        value += separator + newName;
                        separator = newSeparator;
                        continue;
                    }
                }

                names.Add(name);
                values.Add(value);

                name = newName;
                value = newValue;
                separator = newSeparator;
            }

            // The last entry
            if (name != null) {
                names.Add(name);
                values.Add(value);
            }
        }

        private string DecodeValue(string value) {
            if (value == null) return value;
            return value.Replace("\\\"", "\"").Replace("\\\\", "\\");
        }
    }
}