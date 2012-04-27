using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace vApus.Util
{
    /// <summary>
    /// This is a panel to edit values of the types: string, char, bool, all numeric types and array or list of those.
    /// Furthermore, a single object having a parent of the type IEnumerable (GetParent() in vApus.Util.ObjectExtension), can be displayed also.
    /// The type of the property must be one of the above, or else an exception will be thrown. 
    /// The value may not be null or an exception will be thrown.
    /// </summary>
    public class ValueControlPanel : FlowLayoutPanel
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        public event EventHandler ValueChanged;

        #region Fields
        private object[] _values = { };
        //Filled with default controls --> these are encapsulated in ValueControls
        //key = value type, value = control type
        private Dictionary<Type, Type> _controls;
        #endregion

        #region Properties
        public object[] Values
        {
            get { return _values; }
        }
        #endregion

        public ValueControlPanel()
        {
            FillControls();
        }

        #region Functions
        public void SetValues(params object[] values)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            //ClearValues(); --> recycle controls!
            _values = values;
            foreach (object value in _values)
            { }

            LockWindowUpdate(0);
        }
        private void ClearValues()
        {
            _values = new object[0];
            this.Controls.Clear();
        }

        private void FillControls()
        {
            _controls = new Dictionary<Type, Type>();
            AddControl(typeof(bool), typeof(CheckBox));
            AddControl(typeof(char), typeof(TextBox));
            AddControl(typeof(string), typeof(TextBox));
            AddControl(typeof(Enum), typeof(ComboBox));

        }
        public void AddControl(Type valueType, Type controlType)
        {

        }
        #endregion

        public class ValueChangedEventArgs : EventArgs
        {
            public readonly int Index;
            public readonly object OldValue, NewValue;
            public ValueChangedEventArgs(int index, object oldValue, object newValue)
            {
                Index = index;
                OldValue = oldValue;
                NewValue = newValue;
            }
        }
    }
}
