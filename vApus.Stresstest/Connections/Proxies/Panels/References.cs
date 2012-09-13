/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace vApus.Stresstest
{
    public partial class References : UserControl
    {
        public event EventHandler ReferencesChanged;
        private AddReferences _addReferences;


        #region Fields
        private List<string> _defaultFilenames = new List<string>(new string[] { "System.dll", "System.Data.dll", "vApus.Util.dll", "vApus.Stresstest.dll" });
        private List<string> _filenames = new List<string>();
        #endregion

        #region Properties
        
        [ReadOnly(true)]
        public List<string> Filenames
        {
            get
            {
                List<string> filenames = new List<string>(_filenames.Count + _defaultFilenames.Count);
                filenames.AddRange(_defaultFilenames);
                filenames.AddRange(_filenames);
                filenames.Sort();
                return filenames;
            }
            set
            {
                _filenames.Clear();
                foreach (string filename in value)
                    if (!_defaultFilenames.Contains(filename))
                        _filenames.Add(filename);
                if (_filenames.Count == 0)
                { }
                lvwCustomReferences.Items.Clear();
                foreach (string filename in _filenames)
                {
                    string shortFilename = Path.GetFileName(filename);
                    ListViewItem item = lvwCustomReferences.Items.Add(shortFilename);
                    item.Name = shortFilename;
                    if (filename != shortFilename)
                        item.Tag = filename;
                }
            }
        }
        public IEnumerable<string> ShortFilenames
        {
            get
            {
                foreach (string filename in _defaultFilenames)
                    yield return filename;
                foreach (ListViewItem item in lvwCustomReferences.Items)
                    yield return item.Text;
            }
        }
        #endregion


        public References()
        {
            InitializeComponent();
        }

        #region Functions
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (_addReferences == null)
                _addReferences = new AddReferences();
            if (_addReferences.ShowDialog() == DialogResult.OK)
            {
                foreach (string reference in _addReferences.References)
                    if (!lvwCustomReferences.Items.ContainsKey(reference) && !_filenames.Contains(reference))
                    {
                        _filenames.Add(reference);
                        ListViewItem item = lvwCustomReferences.Items.Add(reference);
                        item.Name = reference;
                    }

                if (lvwCustomReferences.SelectedIndices.Count == 0 && lvwCustomReferences.Items.Count != 0)
                    lvwCustomReferences.Items[0].Selected = true;

                if (ReferencesChanged != null)
                    ReferencesChanged(this, null);
            }
        }
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in openFileDialog.FileNames)
                    if (!_filenames.Contains(filename))
                    {
                        _filenames.Add(filename);
                        string shortFilename = Path.GetFileName(filename);

                        if (_filenames.Contains(shortFilename))
                            _filenames.Remove(shortFilename);

                        ListViewItem item = lvwCustomReferences.Items.ContainsKey(shortFilename) ? lvwCustomReferences.Items[shortFilename] : lvwCustomReferences.Items.Add(shortFilename);
                        item.Name = shortFilename;
                        item.Tag = filename;

                        try
                        {
                            /*
                              <runtime>
                                <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
                                    <probing privatePath="ConnectionProxyPrerequisites"/>
                                </assemblyBinding>
                              </runtime>
                             
                             in the app.config for reference resolving.
                             */
                            string connectionProxyPrerequisitesDir = Path.Combine(Application.StartupPath, "ConnectionProxyPrerequisites");
                            string dest1 = Path.Combine(connectionProxyPrerequisitesDir, shortFilename);
                            string dest2 = Path.Combine(Application.StartupPath, shortFilename);
                            if (dest1 != filename || dest2 != filename)
                            {
                                if (!Directory.Exists(connectionProxyPrerequisitesDir))
                                    Directory.CreateDirectory(connectionProxyPrerequisitesDir);

                                File.Copy(filename, dest1, true);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Could not copy '" + filename + "' to '" + Path.Combine(Application.StartupPath, shortFilename) + "'.\n" + ex, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                if (ReferencesChanged != null)
                    ReferencesChanged(this, null);
            }
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            ListViewItem[] items = new ListViewItem[lvwCustomReferences.Items.Count - lvwCustomReferences.SelectedItems.Count];

            int i = 0;
            foreach (ListViewItem item in lvwCustomReferences.Items)
                if (!item.Selected)
                {
                    items[i++] = item;
                }
                else
                {
                    if (item.Tag != null)
                    {
                        _filenames.Remove(item.Tag as string);
                        string filename = Path.Combine(Application.StartupPath, Path.GetFileName(item.Tag as string));
                        if (File.Exists(filename))
                            try
                            {
                                File.Delete(filename);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Could not delete '" + filename + "'.\n" + ex, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                    }
                    else
                    {
                        _filenames.Remove(item.Text);
                    }
                }

            lvwCustomReferences.Items.Clear();
            lvwCustomReferences.Items.AddRange(items);

            if (ReferencesChanged != null)
                ReferencesChanged(this, null);
        }
        private void Added(ListViewItem item)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (item.Tag != null)
                    File.Copy(item.Tag as string, Path.Combine(Application.StartupPath, item.Text));

                if (ReferencesChanged != null)
                    ReferencesChanged(this, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not copy '" + item.Tag + "' to '" + Path.Combine(Application.StartupPath, item.Text) + "'.\n" + ex, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        private void References_Resize(object sender, EventArgs e)
        {
            clm.Width = lvwCustomReferences.Width - 18;
        }

        private void References_VisibleChanged(object sender, EventArgs e)
        {
            if (lvwCustomReferences.SelectedIndices.Count == 0 && lvwCustomReferences.Items.Count != 0)
                lvwCustomReferences.Items[0].Selected = true;
        }
        #endregion
    }
}
