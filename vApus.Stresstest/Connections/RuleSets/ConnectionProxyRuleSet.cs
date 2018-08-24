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
    [ContextMenu(new[] { "Activate_Click", "Add_Click", "Clear_Click", "Paste_Click" },
        new[] { "Edit", "Add syntax item", "Clear", "Paste" })]
    [Hotkeys(new[] { "Activate_Click", "Add_Click", "Paste_Click" },
        new[] { Keys.Enter, Keys.Insert, (Keys.Control | Keys.V) })]
    [DisplayName("Connection proxy rule set"), Serializable]
    public class ConnectionProxyRuleSet : BaseRuleSet, ISerializable {
   
        #region Fields
        private bool _connected = true;
        private uint _tracertField = 1;
        #endregion

        #region Properties

        public new string Label {
            get { return string.Empty; }
            set { }
        }

        [SavableCloneable, PropertyControl(3)]
        [Description("Is it a connected or connectionless protocol that is used?")]
        public bool Connected {
            get { return _connected; }
            set { _connected = value; }
        }

        [SavableCloneable, PropertyControl(3)]
        [Description("The one-base index of the syntax item that is used for tracing the route of communication."),
         DisplayName("Trace route field")]
        public uint TracertField {
            get { return _tracertField; }
            set { _tracertField = value; }
        }

        #endregion

        #region Constructors
        public ConnectionProxyRuleSet() { }
        public ConnectionProxyRuleSet(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                Label = sr.ReadString();
                ChildDelimiter = sr.ReadString();
                _connected = sr.ReadBoolean();

                AddRangeWithoutInvokingEvent(sr.ReadCollection<BaseItem>(new List<BaseItem>()));
            }
            sr = null;
        }
        #endregion

        #region Functions
        /// <summary>
        ///     Only for sending from master to slave.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(Label);
                sw.Write(ChildDelimiter);
                sw.Write(_connected);

                sw.Write(this);
                sw.AddToInfo(info);
            }
            sw = null;
        }
        protected new void Add_Click(object sender, EventArgs e) {
            Add(new ConnectionProxySyntaxItem());
        }

        public override string ToString() {
            return Name;
        }
        #endregion
    }
}