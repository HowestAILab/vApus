/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace vApus.Util
{
    /// <summary>
    /// Only for Arrays or ILists (BaseTypes are fine) ; or objects (no primitives!) that have an array or IList as parent 
    /// </summary>
    [ToolboxItem(false)]
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

            if (value.__Value.GetParent() == null)
                SetUndefinedCollectionControl();
            else
                SetDefinedCollectionControl();
        }
        public void SetUndefinedCollectionControl()
        {
            //Only take the value into account, the other properties are taken care off.
            //Keep control recycling in mind.
            UndefinedCollectionControl ucc = null;

            var ienumerable = base.__Value.__Value as IEnumerable;
            if (base.ValueControl == null || !(base.ValueControl is UndefinedCollectionControl))
            {
                Type elementType = ienumerable.AsQueryable().ElementType;

                ucc = new UndefinedCollectionControl(elementType);

                //Hard coded for the purpose of simplicity.
                ucc.Height = 170;
                ucc.Dock = DockStyle.Top;

                ucc.ValueChanged += new EventHandler(ucc_ValueChanged);
                ucc.Failed += new EventHandler(ucc_Failed);
            }
            else
            {
                ucc = base.ValueControl as UndefinedCollectionControl;
            }

            ucc.ValueChanged -= ucc_ValueChanged;
            ucc.SetValue(ienumerable);
            ucc.ValueChanged += ucc_ValueChanged;

            base.ValueControl = ucc;
        }
        public void SetDefinedCollectionControl()
        {
            //Only take the value into account, the other properties are taken care off.
            //Keep control recycling in mind.
            DefinedCollectionControl dcc = null;

            var ienumerable = base.__Value.__Value as IEnumerable;
            if (base.ValueControl == null || !(base.ValueControl is DefinedCollectionControl))
            {
                dcc = new DefinedCollectionControl();

                //Hard coded for the purpose of simplicity.
                dcc.Height = 170;
                dcc.Dock = DockStyle.Top;

                dcc.ValueChanged += new EventHandler(dcc_ValueChanged);
            }
            else
            {
                dcc = base.ValueControl as DefinedCollectionControl;
            }

            dcc.ValueChanged -= dcc_ValueChanged;
            dcc.SetValue(ienumerable);
            dcc.ValueChanged += dcc_ValueChanged;

            base.ValueControl = dcc;
        }

        private void ucc_ValueChanged(object sender, EventArgs e)
        {
            UndefinedCollectionControl cc = sender as UndefinedCollectionControl;
            base.HandleValueChanged(cc.Value);
        }
        private void dcc_ValueChanged(object sender, EventArgs e)
        {
            DefinedCollectionControl cc = sender as DefinedCollectionControl;
            base.HandleValueChanged(cc.Value);
        }
        private void ucc_Failed(object sender, EventArgs e)
        {
            MessageBox.Show("The new value is not of the right data type.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            UndefinedCollectionControl cc = sender as UndefinedCollectionControl;
            var ienumerable = base.__Value.__Value as IEnumerable;

            cc.ValueChanged -= ucc_ValueChanged;
            cc.SetValue(ienumerable);
            cc.ValueChanged += ucc_ValueChanged;
        }
    }
}
