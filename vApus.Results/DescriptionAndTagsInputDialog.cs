/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Results
{
    /// <summary>
    /// Adds description and tags to the results database.
    /// </summary>
    public partial class DescriptionAndTagsInputDialog : Form
    {
        private string _description;
        private string[] _tags;

        public string Description
        {
            get { return _description; }
            set
            {
                if (value.Length != 0)
                {
                    _description = value;
                    txtDescription.Text = value;
                }
            }
        }
        public string[] Tags
        {
            get { return _tags; }
            set
            {
                if (value.Length != 0)
                {
                    _tags = value;
                    txtTags.Text = _tags.Combine(", ");
                }
            }
        }
        public DescriptionAndTagsInputDialog()
        {
            InitializeComponent();
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            _description = txtDescription.ForeColor == Color.DimGray ? string.Empty : txtDescription.Text.Trim();
            string tagstring = txtTags.ForeColor == Color.DimGray ? string.Empty : txtTags.Text.Trim();
            List<string> tags = new List<string>();
            foreach (string tag in tagstring.Split(','))
            {
                string t = tag.Trim();
                if (!tags.Contains(t)) tags.Add(t);
            }

            _tags = tags.ToArray();

            try { ResultsHelper.SetDescriptionAndTags(Description, Tags); }
            catch (Exception ex) { throw new Exception("The schema must be build first.", ex); }
            finally
            {
                try { this.Close(); }
                catch { }
            }
        }
    }
}
