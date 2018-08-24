/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.SolutionTree {
    /// <summary>
    ///     A base item with a label and the index (only LabeledBaseItemIndex) in the tostring.
    /// </summary>
    [Serializable]
    public abstract class LabeledBaseItem : BaseItem {

        #region Fields
        private readonly object _lock = new object();
        protected string _label = string.Empty;
        #endregion

        #region Properties
        [SavableCloneable]
        public virtual string Label {
            get { return _label; }
            set {
                if (!IsEmpty)
                    _label = value == null ? string.Empty : value;
            }
        }

        /// <summary>
        ///     The one-based index of this item in the collection of its parent.
        /// </summary>
        [Description("The one-based index of this item in the collection of its parent.")]
        public int Index {
            get {
                lock (_lock) {
                    int index = -1;
                    if (!IsEmpty) {
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
        }
        #endregion

        #region Functions
        internal void Export_Click(object sender, EventArgs e) {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Xml Files (*.xml) | *.xml";
            sfd.Title = "Export to...";
            sfd.FileName = Label.ReplaceInvalidWindowsFilenameChars('_');
            if (sfd.ShowDialog() == DialogResult.OK)
                GetXmlStructure().Save(sfd.FileName);
        }

        public override string ToString() {
            if (IsEmpty)
                return "<none>";
            return _label == string.Empty ? string.Join(" ", Name, Index) : string.Join(": ", string.Join(" ", Name, Index), _label);
        }
        #endregion
    }

    public class LabeledBaseItemComparer : IComparer<LabeledBaseItem> {
        private static readonly LabeledBaseItemComparer _labeledBaseItemComparer = new LabeledBaseItemComparer();

        private LabeledBaseItemComparer() {
        }

        public int Compare(LabeledBaseItem x, LabeledBaseItem y) {
            return x.Label.CompareTo(y.Label);
        }

        public static LabeledBaseItemComparer GetInstance() {
            return _labeledBaseItemComparer;
        }
    }
}