using System;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Gui
{
    public partial class TestValueControlPanel : Form
    {
        public enum TestEnum
        {
            AEntry,
            BEntry,
            CEntry
        }

        private object[] _definedCollection;
        private object _empty = new object();

        public TestValueControlPanel()
        {
            InitializeComponent();
        }

        private void TestValueControlPanel_Load(object sender, EventArgs e)
        {
            object o1 = new object();
            object o2 = new object();

            _definedCollection = new object[] { o1, o2 };
            o1.SetParent(_definedCollection);
            o2.SetParent(_definedCollection);
            _empty.SetParent(_definedCollection);

            SetValues();
            SetValues();
        }

        private void valueControlPanel1_ValueChanged(object sender, ValueControlPanel.ValueChangedEventArgs e)
        {
            lblValueChanged.Text = "Index: " + e.Index + " New Value: " + e.NewValue + " Old Value: " + e.OldValue;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetValues();
        }

        private void SetValues()
        {
            object[] derivedFromDefinedCollection = new object[] { _definedCollection[0] };
            derivedFromDefinedCollection.SetParent(_definedCollection);
            valueControlPanel1.SetValues
            (
                new BaseValueControl.Value { Label = "1", Description = "Description", __Value = true, IsEncrypted = false, IsReadOnly = false },
                new BaseValueControl.Value { Label = "2", Description = null, __Value = 'a', IsEncrypted = false, IsReadOnly = false },
                new BaseValueControl.Value { Label = "3", Description = "Description", __Value = "Test", IsEncrypted = true, IsReadOnly = false },
                new BaseValueControl.Value { Label = "4", Description = "Description", __Value = false, IsEncrypted = false, IsReadOnly = true },
                new BaseValueControl.Value { Label = "5", Description = "Description", __Value = 1, IsEncrypted = false, IsReadOnly = false },
                new BaseValueControl.Value { Label = "6", Description = "Description", __Value = 1.2, IsEncrypted = false, IsReadOnly = false },
                new BaseValueControl.Value { Label = "7", Description = null, __Value = TestEnum.BEntry, IsEncrypted = false, IsReadOnly = false },
                new BaseValueControl.Value { Label = "8", Description = null, __Value = new string[] { "a", "b" }, IsEncrypted = false, IsReadOnly = false },
                new BaseValueControl.Value { Label = "9", Description = null, __Value = new int[] { 1, 2 }, IsEncrypted = false, IsReadOnly = false },
                new BaseValueControl.Value { Label = "10", Description = null, __Value = derivedFromDefinedCollection, IsEncrypted = false, IsReadOnly = false },
                new BaseValueControl.Value { Label = "11", Description = null, __Value = _definedCollection[0], IsEncrypted = false, IsReadOnly = false },
                new BaseValueControl.Value { Label = "12", Description = null, __Value = _empty, IsEncrypted = false, IsReadOnly = false }
            );
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (valueControlPanel1.Locked)
                valueControlPanel1.Unlock();
            else
                valueControlPanel1.Lock();
        }
    }
}