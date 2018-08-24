/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    [ContextMenu(new[] { "Activate_Click", "AddRule_Click", "Clear_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" },
                  new[] { "Edit", "Add rule", "Clear", "Remove", "Copy", "Cut", "Duplicate", "Paste" })]
    [Hotkeys(new[] { "Activate_Click", "AddRule_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" },
              new[] { Keys.Enter, Keys.Insert, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D), (Keys.Control | Keys.V) })]
    [DisplayName("Syntax item"), Serializable]
    public class ConnectionProxySyntaxItem : SyntaxItem, ISerializable {
        public ConnectionProxySyntaxItem() {
            base._optional = true;
        }
        public ConnectionProxySyntaxItem(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                _defaultValue = sr.ReadString();

                AddRangeWithoutInvokingEvent(sr.ReadCollection<BaseItem>(new List<BaseItem>()));
            }
            sr = null;
        }

        public new string ChildDelimiter {
            get { return string.Empty; }
            set { }
        }
        public new uint Occurance {
            get { return 1; }
            set { }
        }
        public new bool Optional {
            get { return base.Optional; }
            set { }
        }
        [SavableCloneable, PropertyControl(0)]
        [Description("If the user did not fill in anything for this item in a connection, the given value will be used."), DisplayName("Default value")]
        public new string DefaultValue {
            get { return _defaultValue; }
            set { _defaultValue = value.Trim(); }
        }
        protected new void AddRule_Click(object sender, EventArgs e) {
            if (Count == 0)
                base.AddRule_Click(sender, e);
            else
                MessageBox.Show("Only one rule can be added.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(_defaultValue);

                sw.Write(this);
                sw.AddToInfo(info);
            }
            sw = null;
        }
    }
}