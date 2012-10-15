/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Windows.Forms;
using vApus.SolutionTree;
using System.IO;
using System.Text;
using System.Xml;
using vApus.Util;

namespace vApus.Stresstest
{
    [ContextMenu(new string[] { "Import_Click", "Add_Click", "Import_Prerequisites_Click", "SortItemsByLabel_Click", "Clear_Click", "Paste_Click" }, new string[] { "Import One or More Connection Proxies", "Add Connection Proxy", "Import Connection Proxy Prerequisite(s)", "Sort", "Clear", "Paste" })]
    [Hotkeys(new string[] { "Add_Click", "Paste_Click" }, new Keys[] { Keys.Enter, (Keys.Control | Keys.V) })]
    [DisplayName("Connection Proxies"), Serializable]
    public class ConnectionProxies : BaseItem
    {
        public ConnectionProxies()
        { }
        private void Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            string path = Path.Combine(Application.StartupPath, "ConnectionProxies");
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
            Add(new ConnectionProxy());
        }
        private void Import_Prerequisites_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Dll Files (*.dll)|*.dll|Xml Files (*.xml)|*.xml|Config Files (*.config)|*.config|All Files|*.*";
            ofd.Title = "Import Connection Proxy Prerequisites...";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in ofd.FileNames)
                {
                    string[] filenamePath = fileName.Split('\\');
                    string copyTo = Path.Combine(Application.StartupPath, filenamePath[filenamePath.Length - 1]);

                    try
                    {
                        if (File.Exists(copyTo))
                        {
                            if (MessageBox.Show("\"" + copyTo + "\" already exist, do you want to overwrite it?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                                File.Copy(fileName, Path.Combine(Application.StartupPath, copyTo), true);
                        }
                        else
                        {
                            File.Copy(fileName, Path.Combine(Application.StartupPath, copyTo));
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Failed importing: \"" + fileName + ".\"", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                }
            }
        }
    }
}
