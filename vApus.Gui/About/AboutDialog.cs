/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using vApus.Gui.Properties;
using vApus.Util;

namespace vApus.Gui {
    public partial class AboutDialog : Form {

        #region Fields

        private readonly Font _dateFont;
        private readonly Font _itemFont;
        private readonly Font _titleFont;

        #endregion

        #region Properties

        /// <summary></summary>
        private string AssemblyVersion {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        /// <summary></summary>
        private string AssemblyDescription {
            get {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                    return "";
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        /// <summary></summary>
        private string AssemblyCopyright {
            get {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                    return "";
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        private string Licenses {
            get { return Resources.Licenses; }
        }

        #endregion

        public AboutDialog() {
            InitializeComponent();

            lblLicense.Text = "Payed licensing replaced with this one:\n\n" +
@"MIT License

Copyright (c) 2018 Sizing Servers Lab, affiliated with IT bachelor degree NMCT 
at University College of West-Flanders, Department GKG
(www.sizingservers.be, www.nmct.be, www.howest.be/en)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the " + "\"Software\"" + @"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/ or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

            The above copyright notice and this permission notice shall be included in all
            copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED " + "\"AS IS\"" + @", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.";
            LicenseChecker.LicenseCheckFinished += LicenseChecker_LicenseCheckFinished;

            lblDescription.Text = AssemblyDescription;

            _titleFont = new Font(rtxtHistory.Font, FontStyle.Bold);
            _dateFont = new Font(rtxtHistory.Font, FontStyle.Italic);
            _itemFont = new Font(rtxtHistory.Font, FontStyle.Regular);

            ReadVersionIni();

            rtxtLicenses.Text = Licenses;

            rtxtHistory.DefaultContextMenu(true);
            rtxtLicenses.DefaultContextMenu(true);
        }

        #region Functions
        private void LicenseChecker_LicenseCheckFinished(object sender, LicenseChecker.LicenseCheckEventArgs e) {
            lblLicense.Text = LicenseChecker.StatusMessage;
        }
        private void btnActivateLicense_Click(object sender, EventArgs e) {
            if (ofd.ShowDialog() == DialogResult.OK) {
                try {
                    if (Path.Combine(Application.StartupPath, "license.license") != ofd.FileName)
                        File.Copy(ofd.FileName, Path.Combine(Application.StartupPath, "license.license"), true);
                    Application.Restart();
                }
                catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed to activate the license. The original file at '" + Application.StartupPath + "' is in use.", ex);
                }
            }
        }
        private void btnRequestLicense_Click(object sender, EventArgs e) {
            (new RequestLicense()).ShowDialog();
        }

        private void ReadVersionIni() {
            string ini = Path.Combine(Application.StartupPath, "version.ini");
            string line = string.Empty;
            bool versionFound = false, channelFound = false, historyFound = false;

            if (File.Exists(ini)) {
                var sr = new StreamReader(ini);
                while (sr.Peek() != -1) {
                    line = sr.ReadLine().Trim();

                    if (line.Length == 0)
                        continue;

                    switch (line) {
                        case "[VERSION]":
                            versionFound = true;
                            continue;
                        case "[CHANNEL]":
                            channelFound = true;
                            continue;
                        case "[HISTORY]":
                            historyFound = true;
                            continue;
                    }

                    if (historyFound) {
                        FillHistory(line);
                        break;
                    }
                    else if (channelFound) {
                        txtChannel.Text = "Channel: " + line;
                        channelFound = false;
                    }
                    else if (versionFound) {
                        txtVersion.Text = "Version: " + line;
                        versionFound = false;
                    }
                }
                try {
                    sr.Close();
                }
                catch {
                    //Failed closing the stream reader. Ignore.
                }
                try {
                    sr.Dispose();
                }
                catch {
                    //Failed disposing the stream reader. Ignore.
                }
                sr = null;
            }
        }

        private void FillHistory(string historyOfChanges) {
            var parts = new List<HistoryPart>();
            var doc = new XmlDocument();
            XmlNode node = doc.ReadNode(XmlReader.Create(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(historyOfChanges))));

            //First filling the rtxt and then applying the style
            int previousCaretPosition = 0;
            foreach (XmlNode n in node.ChildNodes) {
                switch (n.Name) {
                    case "t":
                        foreach (XmlNode nn in n.ChildNodes) {
                            switch (nn.Name) {
                                case "d":
                                    rtxtHistory.Text = rtxtHistory.Text + " (" + nn.InnerText + ")" +
                                                       Environment.NewLine;
                                    parts.Add(new HistoryPart("d", previousCaretPosition,
                                                              rtxtHistory.Text.Length - previousCaretPosition));
                                    break;
                                default:
                                    if (previousCaretPosition > 0)
                                        rtxtHistory.Text = rtxtHistory.Text + Environment.NewLine + nn.InnerText;
                                    else
                                        rtxtHistory.Text = rtxtHistory.Text + nn.InnerText;
                                    parts.Add(new HistoryPart("t", previousCaretPosition,
                                                              rtxtHistory.Text.Length - previousCaretPosition));
                                    break;
                            }
                            previousCaretPosition = rtxtHistory.Text.Length;
                            rtxtHistory.Select(rtxtHistory.Text.Length, 0);
                        }
                        break;
                    case "i":
                        rtxtHistory.Text = rtxtHistory.Text + n.InnerText + Environment.NewLine;
                        parts.Add(new HistoryPart("i", previousCaretPosition,
                                                  rtxtHistory.Text.Length - previousCaretPosition));
                        break;
                }
                previousCaretPosition = rtxtHistory.Text.Length;
            }
            foreach (HistoryPart part in parts) {
                rtxtHistory.Select(part.SelectionStart, part.Length);
                switch (part.Type) {
                    case "d":
                        rtxtHistory.SelectionFont = _dateFont;
                        rtxtHistory.SelectionColor = Color.Blue;
                        break;
                    case "i":
                        rtxtHistory.SelectionFont = _itemFont;
                        rtxtHistory.SelectionBullet = true;
                        break;
                    default:
                        rtxtHistory.SelectionFont = _titleFont;
                        rtxtHistory.SelectionColor = Color.Green;
                        break;
                }
            }
            rtxtHistory.Select(0, 0);
        }

        private void lblWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                Process.Start("http://www.sizingservers.be");
            }
            catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed browsing sizingservers.be.", ex, new object[] { sender, e });
            }
        }
        #endregion

        private void picNMCT_Click(object sender, EventArgs e) {
            Process.Start("https://www.nmct.be");
        }

        private void picSSL_Click(object sender, EventArgs e) {
            Process.Start("https://www.sizingservers.be");
        }

        private void picHowest_Click(object sender, EventArgs e) {
            Process.Start("https://www.howest.be/en");
        }
    }
}