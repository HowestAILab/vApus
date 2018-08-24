/*
 * 2016 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    /// <summary>
    /// Stores values that can be used in cps.
    /// </summary>
    [ContextMenu(new[] { "Add_Click", "Import_Click", "Clear_Click", "Paste_Click" },
     new[] { "Add value", "Import values(s)", "Clear", "Paste" })]
    [Hotkeys(new[] { "Add_Click", "Paste_Click" }, new[] { Keys.Insert, (Keys.Control | Keys.V) })]
    [DisplayName("Value store")]
    [Serializable]
    public class ValueStore : BaseItem, ISerializable {
        private readonly object _lock = new object();
        [NonSerialized]
        private static ValueStore _valueStore;
        /// <summary>
        /// Faster than a lock around this.
        /// </summary>
        [NonSerialized]
        private static ConcurrentDictionary<string, ValueStoreValue> _valuesForCPs = new ConcurrentDictionary<string, ValueStoreValue>();

        #region Constructors
        /// <summary>
        /// Stores values that can be used in cps.
        /// </summary>
        public ValueStore() {
            AddAsDefaultItem(new ValueStoreValue() { Label = "publish", Publish = true });
        }

        public ValueStore(SerializationInfo info, StreamingContext ctxt) {
            _valueStore = this;
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                AddRangeWithoutInvokingEvent(sr.ReadCollection<BaseItem>(new List<BaseItem>()));
            }
            sr = null;
        }
        #endregion

        #region Functions
        private void Add_Click(object sender, EventArgs e) { Add(new ValueStoreValue()); }
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(this);
                sw.AddToInfo(info);
            }
            sw = null;
        }

        /// <summary>
        /// Always do this before a test.
        /// </summary>
        public void InitForTest(string resultSetId, string test) {
            lock (_lock) {
                _valuesForCPs.Clear();

                foreach (ValueStoreValue v in this) {
                    v.ResultSetId = resultSetId;
                    v.Test = test;

                    if (v.ClearBeforeTestRun) v.ClearValues();

                    if (!string.IsNullOrWhiteSpace(v.Label))
                        _valuesForCPs.TryAdd(v.Label, v);
                }
            }
        }
        public void InitForTestRun() {
            foreach (ValueStoreValue v in _valuesForCPs.Values)
                if (v.ClearBeforeTestRun) v.ClearValues();
        }
        public void InitForTestConnection() {
            lock (_lock) {
                _valuesForCPs.Clear();

                foreach (ValueStoreValue v in this)
                    if (!string.IsNullOrWhiteSpace(v.Label))
                        _valuesForCPs.TryAdd(v.Label, v);
            }
        }

        /// <summary>
        /// Add or get a value store value from the connection proxy code.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="defaultValue"></param>
        /// <param name="publish"></param>
        /// <returns></returns>
        public static ValueStoreValue GetOrAdd(string label, object defaultValue = null, bool isUniqueForEachConnection = true, bool publish = false) {
            ValueStoreValue v;
            if (!_valuesForCPs.TryGetValue(label, out v)) {
                v = new ValueStoreValue() {
                    Label = label,
                    Type = ValueStoreValueTypes.objectType,
                    DefaultValue = defaultValue ?? string.Empty,
                    ClearBeforeTestRun = true,
                    IsUniqueForEachConnection = isUniqueForEachConnection,
                    Publish = publish,
                    ShowInGui = false
                };
                _valuesForCPs.TryAdd(label, v);
            }
            return v;
        }
        #endregion
    }
}