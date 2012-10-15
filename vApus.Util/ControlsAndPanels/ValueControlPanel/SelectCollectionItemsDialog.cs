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

namespace vApus.Util
{
    public partial class SelectCollectionItemsDialog : Form
    {
        #region Fields
        private IEnumerable _value, _newValue;
        #endregion

        #region Properties
        public IEnumerable NewValue
        {
            get { return _newValue; }
        }
        #endregion

        #region Constructor
        public SelectCollectionItemsDialog()
        {
            InitializeComponent();
        }
        #endregion

        #region Functions
        public void SetValue(IEnumerable value)
        {
            _value = value;
            var parent = value.GetParent() as IEnumerable;
            foreach (object item in parent)
            {
                var lvwi = new ListViewItem(item.ToString());
                lvwi.Tag = item;
                lvw.Items.Add(lvwi);
            }

            IEnumerator enumerator = _value.GetEnumerator();
            enumerator.Reset();
            while (enumerator.MoveNext())
            {
                ListViewItem lvwi = ListViewItemHasTag(enumerator.Current);
                if (lvwi != null)
                    lvwi.Checked = true;
            }
            enumerator.Reset();
        }
        private ListViewItem ListViewItemHasTag(object tag)
        {
            foreach (ListViewItem item in lvw.Items)
                if (item.Tag == tag)
                    return item;
            return null;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            ArrayList arrayList = new ArrayList(lvw.CheckedItems.Count);
            Type elementType = _value.AsQueryable().ElementType;

            foreach (ListViewItem item in lvw.CheckedItems)
                arrayList.Add(item.Tag);

            if (_value is Array)
            {
                _newValue = arrayList.ToArray(elementType);
            }
            else if (_value is IList)
            {
                _newValue = Activator.CreateInstance(_value.GetType()) as IEnumerable;
                IList list = _newValue as IList;
                for (int i = 0; i < arrayList.Count; i++)
                    list.Add(arrayList[i]);
            }
            _newValue.SetParent(_value.GetParent());

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion
    }
}
