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
using System.Text;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace vApus.Stresstest
{
    public partial class References : UserControl
    {
        private AddReferences _addReferences;

        #region Fields

        private CodeTextBox _codeTextBox;

        #endregion

        #region Properties

        public CodeTextBox CodeTextBox
        {
            get { return _codeTextBox; }
            set
            {
                _codeTextBox = value;
                SetGui();
                _codeTextBox.TextChangedDelayed += _codeTextBox_TextChangedDelayed;
            }
        }

        private List<string> Filenames
        {
            get
            {
                var filenames = new List<string>();

                string[] split = _codeTextBox.Text.Split(new[] {"// dllreferences:"}, StringSplitOptions.None);
                if (split.Length == 2)
                {
                    string references = split[1];
                    references = references.Split(new[] {"\n", "\r"}, StringSplitOptions.None)[0];
                    if (references.Length >= 2)
                        filenames.AddRange(references.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries));
                }

                return filenames;
            }
            set
            {
                string[] split = _codeTextBox.Text.Split(new[] {"// dllreferences:"}, StringSplitOptions.None);
                if (split.Length == 2)
                {
                    string references = split[1];
                    references = references.Split(new[] {"\n", "\r"}, StringSplitOptions.None)[0];

                    var sb = new StringBuilder();
                    foreach (string reference in value)
                    {
                        sb.Append(reference);
                        sb.Append(';');
                    }

                    int selectionStart = _codeTextBox.SelectionStart;
                    _codeTextBox.TextChangedDelayed -= _codeTextBox_TextChangedDelayed;
                    _codeTextBox.Text = split[0] + "// dllreferences:" + sb + split[1].Substring(references.Length);
                    _codeTextBox.TextChangedDelayed += _codeTextBox_TextChangedDelayed;

                    _codeTextBox.SelectionLength = 0;
                    _codeTextBox.SelectionStart = _codeTextBox.SelectionStart;

                    _codeTextBox.DoSelectionVisible();
                    _codeTextBox.Focus();
                }
            }
        }

        #endregion

        public References()
        {
            InitializeComponent();
        }

        #region Functions

        private void _codeTextBox_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            SetGui();
        }

        private void SetGui()
        {
            lvwReferences.Items.Clear();
            List<string> filenames = Filenames;
            foreach (string reference in filenames)
            {
                ListViewItem item = lvwReferences.Items.Add(reference);
                item.Name = reference;
            }

            if (lvwReferences.SelectedIndices.Count == 0 && lvwReferences.Items.Count != 0)
                lvwReferences.Items[0].Selected = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (_addReferences == null)
                _addReferences = new AddReferences();
            if (_addReferences.ShowDialog() == DialogResult.OK)
            {
                List<string> filenames = Filenames;
                foreach (string reference in _addReferences.References)
                    if (!filenames.Contains(reference))
                        filenames.Add(reference);

                Filenames = filenames;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<string> filenames = Filenames;
                foreach (string filename in openFileDialog.FileNames)
                {
                    string shortFilename = Path.GetFileName(filename);

                    if (!filenames.Contains(shortFilename))
                        filenames.Add(shortFilename);
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
                        string connectionProxyPrerequisitesDir = Path.Combine(Application.StartupPath,
                                                                              "ConnectionProxyPrerequisites");
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
                        MessageBox.Show(
                            "Could not copy '" + filename + "' to '" +
                            Path.Combine(Application.StartupPath, shortFilename) + "'.\n" + ex, string.Empty,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                Filenames = filenames;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            List<string> filenames = Filenames;
            foreach (ListViewItem item in lvwReferences.Items)
                if (lvwReferences.SelectedItems.Contains(item))
                    filenames.Remove(item.Text);

            Filenames = filenames;
        }

        private void References_Resize(object sender, EventArgs e)
        {
            clm.Width = lvwReferences.Width - 18;
        }

        private void References_VisibleChanged(object sender, EventArgs e)
        {
            if (lvwReferences.SelectedIndices.Count == 0 && lvwReferences.Items.Count != 0)
                lvwReferences.Items[0].Selected = true;
        }

        #endregion
    }
}