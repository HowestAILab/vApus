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

namespace vApus.Stresstest
{
    [ContextMenu(new string[] { "Import_Click", "Add_Click", "Import_Prerequisites_Click", "SortItemsByLabel_Click", "Clear_Click", "Paste_Click" }, new string[] { "Add Connection Proxy", "Import One or More Connection Proxies", "Import Connection Proxy Prerequisites", "Sort", "Clear", "Paste" })]
    [Hotkeys(new string[] { "Add_Click", "Paste_Click" }, new Keys[] { Keys.Enter, (Keys.Control | Keys.V) })]
    [DisplayName("Connection Proxies"), Serializable]
    public class ConnectionProxies : BaseItem
    {
        public ConnectionProxies()
        { }
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
