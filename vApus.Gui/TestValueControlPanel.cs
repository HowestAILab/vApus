using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
        public TestValueControlPanel()
        {
            InitializeComponent();
        }

        private void TestValueControlPanel_Load(object sender, EventArgs e)
        {
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
                new BaseValueControl.Value { Label = "9", Description = null, __Value = new int[] { 1, 2 }, IsEncrypted = false, IsReadOnly = false }
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