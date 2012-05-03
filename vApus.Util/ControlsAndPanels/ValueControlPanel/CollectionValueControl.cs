/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace vApus.Util
{
    public partial class CollectionValueControl : BaseValueControl, IValueControl
    {
        public CollectionValueControl()
        {
            InitializeComponent();
        }

        public void Init(BaseValueControl.Value value)
        {
            //base.__Value = value;

            ////Only take the value into account, the other properties are taken care off.
            ////Keep control recycling in mind.
            //CollectionControl cc = null;

            //if (base.ValueControl == null)
            //{
            //    Type elementType = value.__Value.AsQueryable().ElementType;

            //    _commonPropertyControl = (elementType.HasBaseType(typeof(BaseItem))) ?
            //        GetDefinedCollectionControl(value) :
            //    GetUndefinedCollectionControl(elementType, value);


            //    //Hard coded for the purpose of simplicity.
            //    _commonPropertyControl.Height = 170;
            //    _commonPropertyControl.Dock = DockStyle.Top;
            //}
            //else
            //{
            //    cc = base.ValueControl as CollectionControl;
            //}

            //base.ValueControl = cc;
        }
    }
}
