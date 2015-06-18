/*
 * Copyright 2009 (c) Sizing Servers Lab
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
    /// <summary>
    /// Holds instances of ScenarioRuleSet.
    /// </summary>
    [ContextMenu(new[] { "Import_Click", "Add_Click", "SortItemsByLabel_Click", "Clear_Click", "Paste_Click" },
        new[] { "Import scenario rule set(s)", "Add scenario rule set", "Sort", "Clear", "Paste" })]
    [Hotkeys(new[] { "Paste_Click" }, new[] { (Keys.Control | Keys.V) })]
    [DisplayName("Scenario rule sets")]
    public class ScenarioRuleSets : BaseRuleSets {
        private void Import_Click(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();

            string path = Path.Combine(Application.StartupPath, "ScenarioRuleSets");
            if (Directory.Exists(path))
                ofd.InitialDirectory = path;

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
                    MessageBox.Show(s, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error,
                                    MessageBoxDefaultButton.Button1);
                }
            }
        }
        private void Add_Click(object sender, EventArgs e) {
            Add(new ScenarioRuleSet());
        }
    }
}