/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    /// Used in DefinedCollectionControl and DefinedKVPCollectionControl.
    /// </summary>
    public partial class SelectCollectionItemsDialog : Form {

        #region Fields
        private IEnumerable _newValue, _value;
        private bool _isKVP;
        private Type _elementType;
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
            _elementType = _value.AsQueryable().ElementType;

            Type elementBaseType = null, keyType = _elementType;
            try {
                elementBaseType = _elementType.GetGenericTypeDefinition();
                keyType = _elementType.GetGenericArguments()[0];
            } catch {
                //Will fail if no generic type def.
            }
            _isKVP = elementBaseType == typeof(KeyValuePair<,>);

            foreach (object item in parent)
                if (item.GetType() == keyType) {
                    var lvwi = new ListViewItem(item.ToString());
                    lvwi.Tag = item;
                    lvw.Items.Add(lvwi);
                }

            IEnumerator enumerator = _value.GetEnumerator();
            enumerator.Reset();
            while (enumerator.MoveNext()) {
                var tag = enumerator.Current;
                if (_isKVP)
                    tag = _elementType.GetProperty("Key").GetValue(tag, null);

                ListViewItem lvwi = ListViewItemHasTag(tag);
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

            if (_isKVP) {
                var kvpArrayList = new ArrayList(arrayList.Count);
                var valueType = _elementType.GetGenericArguments()[1];
                for (int i = 0; i < arrayList.Count; i++) {
                    var key = arrayList[i];

                    //Get the editable value if any from the previous set.
                    var value = Activator.CreateInstance(valueType);
                    IEnumerator enumerator = _value.GetEnumerator();
                    enumerator.Reset();
                    while (enumerator.MoveNext()) {
                        var oldKey = elementType.GetProperty("Key").GetValue(enumerator.Current, null);
                        if (oldKey == key) {
                            value = elementType.GetProperty("Value").GetValue(enumerator.Current, null);
                            break;
                        }
                    }
                    enumerator.Reset();

                    var kvp = Activator.CreateInstance(_elementType, new object[] { key, value });
                    kvpArrayList.Add(kvp);
                }

                arrayList = kvpArrayList;
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