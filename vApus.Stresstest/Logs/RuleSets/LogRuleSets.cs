using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vApus.SolutionTree;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;
using vApus.Util;
using System.IO;

namespace vApus.Stresstest
{
    [ContextMenu(new string[] { "Import_Click", "Add_Click", "SortItemsByLabel_Click", "Clear_Click", "Paste_Click" }, new string[] { "Import Log Rule Set(s)", "Add Log Rule Set", "Sort", "Clear", "Paste" })]
    [Hotkeys(new string[] { "Paste_Click" }, new Keys[] { (Keys.Control | Keys.V) })]
    [DisplayName("Log Rule Sets"), Serializable]
    public class LogRuleSets : BaseRuleSets
    {
        public LogRuleSets()
        { }
        private void Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            string path = Path.Combine(Application.StartupPath, "LogRuleSets");
            if (Directory.Exists(path))
                ofd.InitialDirectory = path;

            ofd.Multiselect = true;
            ofd.Filter = "Xml Files (*.xml) | *.xml";
            ofd.Title = (sender is ToolStripMenuItem) ? (sender as ToolStripMenuItem).Text : ofd.Title = "Import from...";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string fileName in ofd.FileNames)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(fileName);

                    try
                    {
                        if (xmlDocument.FirstChild.Name == this.GetType().Name && xmlDocument.FirstChild.FirstChild.Name == "Items")
                        {
                            string errorMessage;
                            LoadFromXml(xmlDocument.FirstChild, out errorMessage);
                            sb.Append(errorMessage);
                            if (errorMessage.Length == 0)
                            {
                                ResolveBranchedIndices();
                                InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);
                            }
                        }
                        else
                        {
                            sb.Append(fileName + " contains no valid structure to load;");
                        }
                    }
                    catch
                    {
                        sb.Append("Unknown or non-existing item found in " + fileName + ";");
                    }
                }
                if (sb.ToString().Length > 0)
                {
                    string s = "Failed loading: " + sb.ToString();
                    LogWrapper.LogByLevel(s, LogLevel.Error);
                    MessageBox.Show(s, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
        }
        private void Add_Click(object sender, EventArgs e)
        {
            Add(new LogRuleSet());
        }
    }
}
