/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using RandomUtils.Log;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    [ContextMenu(
        new[]
            {
                "Import_Click", "Add_Click", "Import_Prerequisites_Click", "SortItemsByLabel_Click", "Clear_Click",
                "Paste_Click"
            },
        new[]
            {
                "Import one or more connection proxies", "Add connection proxy", "Import connection proxy prerequisite(s)",
                "Sort", "Clear", "Paste"
            })]
    [Hotkeys(new[] { "Add_Click", "Paste_Click" }, new[] { Keys.Enter, (Keys.Control | Keys.V) })]
    [DisplayName("Connection proxies"), Serializable]
    public class ConnectionProxies : BaseItem {
        private void Import_Click(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();

            string path = Path.Combine(Application.StartupPath, "ConnectionProxies");
            if (Directory.Exists(path)) {
                ofd.InitialDirectory = path;
                MoveConnectionProxyPrerequisistes(path);
            }

            ofd.Multiselect = true;
            ofd.Filter = "Xml Files (*.xml) | *.xml";
            ofd.Title = (sender is ToolStripMenuItem)
                            ? (sender as ToolStripMenuItem).Text
                            : ofd.Title = "Import from...";

            if (ofd.ShowDialog() == DialogResult.OK) {
                var sb = new StringBuilder();
                foreach (string fileName in ofd.FileNames) {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(fileName);

                    try {
                        if (xmlDocument.FirstChild.Name == GetType().Name &&
                            xmlDocument.FirstChild.FirstChild.Name == "Items") {
                            string errorMessage;
                            CancellationToken cancellationToken = new CancellationToken(false);
                            LoadFromXml(xmlDocument.FirstChild, cancellationToken, out errorMessage);
                            sb.Append(errorMessage);
                            if (errorMessage.Length == 0) {
                                ResolveBranchedIndices();
                                InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, false);
                            }
                        } else {
                            sb.Append(fileName + " contains no valid structure to load;");
                        }
                    } catch {
                        sb.Append("Unknown or non-existing item found in " + fileName + ";");
                    }
                }
                if (sb.ToString().Length > 0) {
                    string s = "Failed loading: " + sb;
                    Loggers.Log(Level.Error, s, null, new object[] { sender, e });
                    MessageBox.Show(s, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
        }

        /// <summary>
        /// Only if in ConnectionProxies.
        /// </summary>
        /// <param name="sourceDir"></param>
        private void MoveConnectionProxyPrerequisistes(string sourceDir) {
            try {
                string path = Path.Combine(Application.StartupPath, "ConnectionProxyPrerequisites");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (!sourceDir.EndsWith("\\")) sourceDir += "\\";

                foreach (string file in Directory.GetFiles(sourceDir))
                    if (!file.EndsWith(".xml")) {
                        string destFile = Path.Combine(path, file.Substring(sourceDir.Length));
                        File.Copy(file, destFile, true);
                        File.Delete(file);
                    }
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed moving connection proxy prerequisites from the ConnectionProxies folder.", ex);
            }
        }

        private void Add_Click(object sender, EventArgs e) {
            Add(new ConnectionProxy());
        }

        private void Import_Prerequisites_Click(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter =
                "Dll Files (*.dll)|*.dll|Xml Files (*.xml)|*.xml|Config Files (*.config)|*.config|All Files|*.*";
            ofd.Title = "Import Connection Proxy Prerequisites...";
            if (ofd.ShowDialog() == DialogResult.OK) {
                foreach (string fileName in ofd.FileNames) {
                    string[] filenamePath = fileName.Split('\\');
                    string copyTo = Path.Combine(Application.StartupPath, filenamePath[filenamePath.Length - 1]);

                    try {
                        if (File.Exists(copyTo)) {
                            if (
                                MessageBox.Show("\"" + copyTo + "\" already exist, do you want to overwrite it?",
                                                string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                                MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                                File.Copy(fileName, Path.Combine(Application.StartupPath, copyTo), true);
                        } else {
                            File.Copy(fileName, Path.Combine(Application.StartupPath, copyTo));
                        }
                    } catch {
                        MessageBox.Show("Failed importing: \"" + fileName + ".\"", string.Empty, MessageBoxButtons.OK,
                                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                }
            }
        }
    }
}