/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Stresstest {
    public partial class AddReferencesDialog : Form {
        public AddReferencesDialog() {
            InitializeComponent();
        }

        public IEnumerable<string> References {
            get {
                foreach (ListViewItem item in lvwGac.CheckedItems)
                    yield return item.Tag as string;
            }
        }

        private void LoadReferences() {
            Cursor = Cursors.WaitCursor;
            lvwGac.Items.Clear();
            // ConcurrentBag<object> cb = new ConcurrentBag<object>();
            string gac = Path.Combine(SpecialFolder.GetPath(SpecialFolder.Folder.Windows), "assembly");
            foreach (string processorArchitecture in Directory.GetDirectories(gac)) {
                string trimmedProcessorArchitecture = Path.GetFileName(processorArchitecture);
                if (!trimmedProcessorArchitecture.StartsWith("GAC", StringComparison.OrdinalIgnoreCase))
                    continue;
                trimmedProcessorArchitecture = trimmedProcessorArchitecture.Length > 4
                                                   ? trimmedProcessorArchitecture.Substring(4)
                                                   : string.Empty;
                foreach (string assemblyName in Directory.GetDirectories(processorArchitecture)) {
                    string trimmedAssemblyName = Path.GetFileName(assemblyName);
                    foreach (string version_Culture_PublicKeyToken in Directory.GetDirectories(assemblyName)) {
                        string[] properties = Path.GetFileName(version_Culture_PublicKeyToken).Split('_');
                        //Parallel.ForEach(Directory.GetFiles(version_Culture_PublicKeyToken, "*.dll"), delegate(string file)
                        foreach (string file in Directory.GetFiles(version_Culture_PublicKeyToken, "*.dll")) {
                            ListViewItem item = lvwGac.Items[trimmedAssemblyName];
                            if (item == null) {
                                item = lvwGac.Items.Add(new ListViewItem(trimmedAssemblyName));
                                for (int i = 0; i < 2; i++)
                                    item.SubItems.Add(properties[i]);
                                item.SubItems.Add(trimmedProcessorArchitecture);

                                item.Tag = Path.GetFileName(file);
                                item.Name = item.Text;
                            } else {
                                if (!item.SubItems[1].Text.Contains(properties[0]))
                                    item.SubItems[1].Text += ", " + properties[0];
                                if (!item.SubItems[2].Text.Contains(properties[1]))
                                    item.SubItems[2].Text += ", " + properties[1];
                                if (!item.SubItems[3].Text.Contains(trimmedProcessorArchitecture))
                                    item.SubItems[3].Text += ", " + trimmedProcessorArchitecture;
                            }
                        }
                        //);
                    }
                }
            }
            foreach (ColumnHeader clm in lvwGac.Columns) {
                int width = clm.Width;
                clm.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                if (clm.Width < width)
                    clm.Width = width;
            }
            Cursor = Cursors.Default;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void lvwGac_VisibleChanged(object sender, EventArgs e) {
            if (Visible)
                LoadReferences();
        }

        private void btnRefresh_Click(object sender, EventArgs e) {
            LoadReferences();
        }
    }
}