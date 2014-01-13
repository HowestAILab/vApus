/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    ///     Only for Arrays or ILists (BaseTypes are fine) ; or objects (no primitives!) that have an array or IList as parent
    /// </summary>
    [ToolboxItem(false)]
    public partial class CollectionValueControl : BaseValueControl, IValueControl {
        /// <summary>
        ///     Only for Arrays or ILists (BaseTypes are fine) ; or objects (no primitives!) that have an array or IList as parent
        /// </summary>
        public CollectionValueControl() {
            InitializeComponent();
        }

        public void Init(Value value) {
            base.__Value = value;

            if (value.__Value.GetParent() == null)
                SetUndefinedCollectionControl();
            else
                SetDefinedCollectionControl();
        }

        private void SetUndefinedCollectionControl() {
            //Only take the value into account, the other properties are taken care off.
            //Keep control recycling in mind.
            UndefinedCollectionControl ucc = null;

            var ienumerable = base.__Value.__Value as IEnumerable;
            if (base.ValueControl == null || !(base.ValueControl is UndefinedCollectionControl)) {
                Type elementType = ienumerable.AsQueryable().ElementType;

                ucc = new UndefinedCollectionControl(elementType);

                //Hard coded for the purpose of simplicity.
                ucc.Height = 170;
                ucc.Dock = DockStyle.Top;

                ucc.ValueChanged += ucc_ValueChanged;
                ucc.Failed += ucc_Failed;
            } else {
                ucc = base.ValueControl as UndefinedCollectionControl;
            }

            ucc.ValueChanged -= ucc_ValueChanged;
            ucc.SetValue(ienumerable);
            ucc.ValueChanged += ucc_ValueChanged;

            base.ValueControl = ucc;
        }

        private void SetDefinedCollectionControl() {
            //Only take the value into account, the other properties are taken care off.
            //Keep control recycling in mind.
            UserControl dcc = null;

            var ienumerable = base.__Value.__Value as IEnumerable;
            Type elementBaseType = null;
            try {
                elementBaseType = ienumerable.AsQueryable().ElementType.GetGenericTypeDefinition();
            } catch {
                //Will fail if no generic type def.
            }
            bool isKVP = elementBaseType == typeof(KeyValuePair<,>);
            if (base.ValueControl == null || !(base.ValueControl is DefinedCollectionControl)) {

                if (isKVP) {
                    dcc = new DefinedKVPCollectionControl();
                    (dcc as DefinedKVPCollectionControl).ValueChanged += dkvpcc_ValueChanged;
                } else {
                    dcc = new DefinedCollectionControl();
                    (dcc as DefinedCollectionControl).ValueChanged += dcc_ValueChanged;
                }

                //Hard coded for the purpose of simplicity.
                dcc.Height = 170;
                dcc.Dock = DockStyle.Top;

            } else {
                if (isKVP)
                    dcc = base.ValueControl as DefinedKVPCollectionControl;
                else
                    dcc = base.ValueControl as DefinedCollectionControl;
            }

            if (isKVP) {
                (dcc as DefinedKVPCollectionControl).ValueChanged -= dkvpcc_ValueChanged;
                (dcc as DefinedKVPCollectionControl).SetValue(ienumerable);
                (dcc as DefinedKVPCollectionControl).ValueChanged += dkvpcc_ValueChanged;
            } else {
                (dcc as DefinedCollectionControl).ValueChanged -= dcc_ValueChanged;
                (dcc as DefinedCollectionControl).SetValue(ienumerable);
                (dcc as DefinedCollectionControl).ValueChanged += dcc_ValueChanged;
            }

            base.ValueControl = dcc;
        }

        private void ucc_ValueChanged(object sender, EventArgs e) {
            var cc = sender as UndefinedCollectionControl;
            base.HandleValueChanged(cc.Value);
        }

        private void dcc_ValueChanged(object sender, EventArgs e) {
            var cc = sender as DefinedCollectionControl;
            base.HandleValueChanged(cc.Value);
        }
        private void dkvpcc_ValueChanged(object sender, EventArgs e) {
            var cc = sender as DefinedKVPCollectionControl;
            base.HandleValueChanged(cc.Value);
        }

        private void ucc_Failed(object sender, EventArgs e) {
            MessageBox.Show("The new value is not of the right data type.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            var cc = sender as UndefinedCollectionControl;
            var ienumerable = base.__Value.__Value as IEnumerable;

            cc.ValueChanged -= ucc_ValueChanged;
            cc.SetValue(ienumerable);
            cc.ValueChanged += ucc_ValueChanged;
        }
    }
}