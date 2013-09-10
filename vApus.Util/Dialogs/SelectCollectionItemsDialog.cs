/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.Linq;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    /// Used in DefinedCollectionControl and DefinedKVPCollectionControl.
    /// </summary>
    public partial class SelectCollectionItemsDialog : Form {

        #region Fields
        private IEnumerable _newValue, _value;
        #endregion

        #region Properties
        public IEnumerable NewValue { get { return _newValue; } }

        public bool MultipleValues {
            get { return lvw.CheckBoxes; }
            set { lvw.CheckBoxes = value; }
        }
        #endregion

        #region Constructor
        public SelectCollectionItemsDialog() { InitializeComponent(); }
        #endregion

        #region Functions
        public void SetValue(IEnumerable value) {
            _value = value;
            var parent = value.GetParent() as IEnumerable;
            foreach (object item in parent) {
                var lvwi = new ListViewItem(item.ToString());
                lvwi.Tag = item;
                lvw.Items.Add(lvwi);
            }

            IEnumerator enumerator = _value.GetEnumerator();
            enumerator.Reset();
            while (enumerator.MoveNext()) {
                ListViewItem lvwi = ListViewItemHasTag(enumerator.Current);
                if (lvwi != null)
                    lvwi.Checked = true;
            }
            enumerator.Reset();

            if (lvw.Items.Count != 0) lvw.Items[0].Selected = true;
        }

        private ListViewItem ListViewItemHasTag(object tag) {
            foreach (ListViewItem item in lvw.Items)
                if (item.Tag == tag)
                    return item;
            return null;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            ArrayList arrayList = null;
            Type elementType = _value.AsQueryable().ElementType;
            if (MultipleValues) {
                arrayList = new ArrayList(lvw.CheckedItems.Count);

                foreach (ListViewItem item in lvw.CheckedItems)
                    arrayList.Add(item.Tag);
            } else {
                arrayList = new ArrayList(1);

                foreach (ListViewItem item in lvw.SelectedItems) {
                    arrayList.Add(item.Tag);
                    break;
                }
            }
            if (_value is Array) {
                _newValue = arrayList.ToArray(elementType);
            } else if (_value is IList) {
                _newValue = Activator.CreateInstance(_value.GetType()) as IEnumerable;
                var list = _newValue as IList;
                for (int i = 0; i < arrayList.Count; i++)
                    list.Add(arrayList[i]);
            }
            _newValue.SetParent(_value.GetParent());

            DialogResult = DialogResult.OK;
            Close();
        }
        #endregion
    }
}