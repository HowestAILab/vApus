/*
 * Copyright 2012 (c) Sizing Servers Lab
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
    /// <summary>
    /// Only for Arrays or ILists (BaseTypes are fine) ; or objects (no primitives!) that have an array or IList as parent 
    /// </summary>
    public partial class CollectionValueControl : BaseValueControl, IValueControl
    {
        /// <summary>
        /// Only for Arrays or ILists (BaseTypes are fine) ; or objects (no primitives!) that have an array or IList as parent 
        /// </summary>
        public CollectionValueControl()
        {
            InitializeComponent();
        }

        public void Init(BaseValueControl.Value value)
        {
            base.__Value = value;

            //Only take the value into account, the other properties are taken care off.
            //Keep control recycling in mind.
            CollectionControl cc = null;

            var ienumerable = value.__Value as IEnumerable;
            if (base.ValueControl == null)
            {
                Type elementType = ienumerable.AsQueryable().ElementType;

                cc = new CollectionControl(elementType);

                //Hard coded for the purpose of simplicity.
                cc.Height = 170;
                cc.Dock = DockStyle.Top;

                cc.ValueChanged += new EventHandler(cc_ValueChanged);
                cc.Failed += new EventHandler(cc_Failed);
            }
            else
            {
                cc = base.ValueControl as CollectionControl;
            }

            cc.ValueChanged -= cc_ValueChanged;
            cc.SetValue(ienumerable);
            cc.ValueChanged += cc_ValueChanged;

            base.ValueControl = cc;
        }

        private void cc_ValueChanged(object sender, EventArgs e)
        {
            CollectionControl cc = sender as CollectionControl;
            base.HandleValueChanged(cc.Value);
        }
        private void cc_Failed(object sender, EventArgs e)
        {
            MessageBox.Show("The new value is not of the right data type.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            CollectionControl cc = sender as CollectionControl;
            var ienumerable = base.__Value.__Value as IEnumerable;
            
            cc.ValueChanged -= cc_ValueChanged;
            cc.SetValue(ienumerable);
            cc.ValueChanged += cc_ValueChanged;
        }
    }
}
