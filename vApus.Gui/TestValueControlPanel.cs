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
            MessageBox.Show("Index: " + e.Index + " New Value: " + e.NewValue + " Old Value: " + e.OldValue);
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
                new BaseValueControl.Value { Label = "2", Description = "Description", __Value = 'a', IsEncrypted = false, IsReadOnly = false },
                new BaseValueControl.Value { Label = "3", Description = "Description", __Value = "Test", IsEncrypted = false, IsReadOnly = false },
                new BaseValueControl.Value { Label = "4", Description = "Description", __Value = false, IsEncrypted = false, IsReadOnly = true }
            );
        }
    }
}