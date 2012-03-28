/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.SolutionTree
{
    /// <summary>
    /// A base item with a label and the index (only LabeledBaseItemIndex) in the tostring.
    /// </summary>
    [Serializable]
    public abstract class LabeledBaseItem : BaseItem
    {
        #region Fields
        private string _label = string.Empty;
        #endregion

        #region Properties
        [SavableCloneable, PropertyControl]
        public virtual string Label
        {
            get { return _label; }
            set
            {
                if (!IsEmpty)
                    _label = value == null ? string.Empty : value;
            }
        }
        [Description("The one-based index of this item in the collection of its parent.")]
        public int Index
        {
            get
            {
                int index = -1;
                if (!IsEmpty)
                {
                    index = 1;
                    if (Parent != null)
                        for (int i = 0; i != Parent.Count; i++)
                            if (Parent[i] == this)
                                break;
                            else if (Parent[i] is LabeledBaseItem)
                                ++index;
                }
                return index;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// A base item with a label and the index in the tostring.
        /// </summary>
        public LabeledBaseItem()
            : base()
        { }
        #endregion

        #region Functions
        internal void Export_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Xml Files (*.xml) | *.xml";
            sfd.Title = "Export to...";
            sfd.FileName = Label.ReplaceInvalidWindowsFilenameChars('_');
            if (sfd.ShowDialog() == DialogResult.OK)
                GetXmlStructure().Save(sfd.FileName);
        }
        public override string ToString()
        {
            if (IsEmpty)
                return null;
            return _label == string.Empty ? Name + ' ' + Index : Name + ' ' + Index + ": " + _label;
        }
        #endregion
    }
    public class LabeledBaseItemComparer : IComparer<LabeledBaseItem>
    {
        private static LabeledBaseItemComparer _labeledBaseItemComparer = new LabeledBaseItemComparer();
        private LabeledBaseItemComparer()
        { }
        public static LabeledBaseItemComparer GetInstance()
        {
            return _labeledBaseItemComparer;
        }
        public int Compare(LabeledBaseItem x, LabeledBaseItem y)
        {
            return x.Label.CompareTo(y.Label);
        }
    }
}
