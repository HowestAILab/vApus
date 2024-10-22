﻿/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    [ContextMenu(new[] { "Remove_Click", "Export_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { "Remove", "Export", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new[] { "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    [DisplayName("Connection proxy"), Serializable]
    public class ConnectionProxy : LabeledBaseItem, ISerializable {

        #region Properties

        public ConnectionProxyRuleSet ConnectionProxyRuleSet {
            get { return this[0] as ConnectionProxyRuleSet; }
        }

        public ConnectionProxyCode ConnectionProxyCode {
            get { return this[1] as ConnectionProxyCode; }
        }

        #endregion

        #region Constructor

        public ConnectionProxy() {
            AddAsDefaultItem(new ConnectionProxyRuleSet());
            AddAsDefaultItem(new ConnectionProxyCode());
        }

        /// <summary>
        ///     Only for sending from master to slave.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public ConnectionProxy(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                Label = sr.ReadString();
                ClearWithoutInvokingEvent();
                AddWithoutInvokingEvent(sr.ReadObject() as ConnectionProxyRuleSet);
                AddWithoutInvokingEvent(sr.ReadObject() as ConnectionProxyCode);
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
                sw.WriteObject(ConnectionProxyRuleSet);
                sw.WriteObject(ConnectionProxyCode);
                sw.AddToInfo(info);
            }
            sw = null;
        }

        /// <summary>
        ///     Build and returns a new connection proxy class.
        /// </summary>
        /// <returns></returns>
        public string BuildConnectionProxyClass(string connectionString = "") {
            return ConnectionProxyCode.BuildConnectionProxyClass(ConnectionProxyRuleSet, connectionString);
        }

        #endregion
    }
}