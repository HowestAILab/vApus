/*
 * Copyright 2016 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;
using vApus.Publish;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    [ContextMenu(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
     new[] { "Edit", "Remove", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
     new[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    [DisplayName("Value")]
    [Serializable]
    public class ValueStoreValue : LabeledBaseItem, ISerializable {

        #region Enum

        [Serializable]
        public enum ValueStoreValueTypes {
            [Description("Object")]
            objectType = 0,
            [Description("String")]
            stringType,
            [Description("32-bit integer")]
            intType,
            [Description("64-bit integer")]
            longType,
            [Description("32-bit floating point")]
            floatType,
            [Description("64-bit floating point")]
            doubleType,
            [Description("Boolean")]
            boolType
        }

        #endregion

        #region Fields
        private readonly object _lock = new object();

        private ValueStoreValueTypes _valueType = ValueStoreValueTypes.objectType;
        private ConcurrentDictionary<string, object> _values = new ConcurrentDictionary<string, object>();

        private object _defaultValue;
        private string _savableDefaultValue = string.Empty;

        private volatile bool _isUniqueForEachConnection = true, _clearBeforeTest = true, _publish;

        #endregion

        #region Properties

        [DisplayName("Label"), SavableCloneable, PropertyControl(0)]
        [Description("To be able to use this value in a connection proxy, this must be filled in. In the case of duplicates, the first found will be used when called in a connection proxy.")]
        public new string Label {
            set {
                value = value.Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "").ReplaceInvalidWindowsFilenameChars('_');
                if (base.Label != value) {
                    base.Label = value;
                    ClearValues();
                }
            }
            get { return base.Label; }
        }

        [DisplayName("Type"), SavableCloneable, PropertyControl(2)]
        public ValueStoreValueTypes Type {
            get { return _valueType; }
            set {
                if (_valueType != value) {
                    _valueType = value;
                    ClearValues();
                    if (_savableDefaultValue.Length != 0) {
                        if (_valueType == ValueStoreValueTypes.objectType) {
                            _defaultValue = _savableDefaultValue;
                        }
                        else {
                            bool success;
                            _defaultValue = CastOrParseInputToType(_savableDefaultValue, _valueType, out success);
                        }
                    }
                }
            }
        }

        [DisplayName("Default value"), SavableCloneable, PropertyControl(3)]
        [Description("All available types are valid for a default value ('Object' maps to 'String').")]
        public string SavableDefaultValue {
            get {
                DefaultValue = _savableDefaultValue;
                return _savableDefaultValue;
            }
            set { DefaultValue = value; }
        }
        public object DefaultValue {
            get {
                lock (_lock) {
                    bool success;
                    _defaultValue = CastOrParseInputToType(_defaultValue == null ? _savableDefaultValue : _defaultValue, _valueType, out success);

                    return _defaultValue;
                }
            }
            internal set {
                lock (_lock) {
                    bool success;
                    _defaultValue = CastOrParseInputToType(value, _valueType, out success);
                    _savableDefaultValue = _defaultValue.ToString();
                }
            }
        }

        [DisplayName("Unique for each connection"), SavableCloneable, PropertyControl(4)]
        [Description("When true, a value is unique for a connection. This is determined by the executing thread name. When false, a value is shared for all connections in a stress test.")]
        public bool IsUniqueForEachConnection {
            get { return _isUniqueForEachConnection; }
            set {
                if (_isUniqueForEachConnection != value) {
                    _isUniqueForEachConnection = value;
                    ClearValues();
                }
            }
        }

        [DisplayName("Clear value before test"), SavableCloneable, PropertyControl(4)]
        public bool ClearBeforeTest {
            get { return _clearBeforeTest; }
            set { _clearBeforeTest = value; }
        }

        [SavableCloneable, PropertyControl(5)]
        [Description("When a value is set you can publish it, if publishing is enabled (see Tools > Options... > Publish values). This is handy for external tracing / debugging and, unlike EventPanel.AddEvent(...), without straining the vApus GUI.")]
        public bool Publish {
            get { return _publish; }
            set { _publish = value; }
        }

        public ConcurrentDictionary<string, object> Values { get { return _values; } }

        public string Test { get; set; }
        public string ResultSetId { get; set; }

        #endregion

        #region Constructors
        public ValueStoreValue() { }

        public ValueStoreValue(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                Label = sr.ReadString();
                _valueType = (ValueStoreValueTypes)sr.ReadInt32();
                SavableDefaultValue = sr.ReadString();
                _isUniqueForEachConnection = sr.ReadBoolean();
                _clearBeforeTest = sr.ReadBoolean();
                _publish = sr.ReadBoolean();
            }
            sr = null;
        }
        #endregion

        #region Functions
        public T Get<T>() { return Get<T>(Thread.CurrentThread.Name); }

        public T Get<T>(string ownerName) {
            if (typeof(T) != typeof(object) || typeof(T) != GetType(_valueType)) throw new Exception("The given type is not " + _valueType + " or object ( value store value " + Label + ").");

            object castedValue = null;
            try {
                object value;
                if (_isUniqueForEachConnection) {
                    if (!_values.TryGetValue(ownerName, out value))
                        value = _defaultValue;
                }
                else {
                    value = _values.Count == 0 ? _defaultValue : _values.First().Value;
                }

                bool success;
                castedValue = CastOrParseInputToType(value, _valueType, out success);
                if (!success) throw new Exception();
            }
            catch {
                throw new Exception("Failed getting value store value " + Label  + " for " + ownerName + ".");
            }
            return (T)castedValue;
        }

        public string[] GetOwners() {
            string[] owners = new string[_values.Count];
            _values.Keys.CopyTo(owners, 0);
            return owners;
        }

        public void Set(object value) { SetValue(Thread.CurrentThread.Name, value); }

        private void SetValue(string threadName, object value) {
            bool success;
            object castedValue = CastOrParseInputToType(value, _valueType, out success);
            if (!success) throw new Exception("Value not of type " + _valueType + ".");

            string key = _isUniqueForEachConnection ? Thread.CurrentThread.Name : "shared";
            if (key != null) {
                _values.AddOrUpdate(key, castedValue, (k, oldValue) => castedValue);

                if (Publish) PublishValue(key, castedValue.ToString());
            }
        }

        public void ClearValues() { _values.Clear(); }

        private object CastOrParseInputToType(object input, ValueStoreValueTypes valueType, out bool success) {
            success = true;
            switch (valueType) {
                case ValueStoreValueTypes.boolType:
                    bool outputBool = false;
                    if (input is bool) outputBool = (bool)input; else success = bool.TryParse(input.ToString(), out outputBool);
                    return outputBool;
                case ValueStoreValueTypes.doubleType:
                    double outputDouble = 0;
                    if (input is double) outputDouble = (double)input; else success = double.TryParse(input.ToString(), out outputDouble);
                    return outputDouble;
                case ValueStoreValueTypes.floatType:
                    float outputFloat = 0;
                    if (input is float) outputFloat = (float)input; else success = float.TryParse(input.ToString(), out outputFloat);
                    return outputFloat;
                case ValueStoreValueTypes.intType:
                    int outputInt = 0;
                    if (input is int) outputInt = (int)input; else success = int.TryParse(input.ToString(), out outputInt);
                    return outputInt;
                case ValueStoreValueTypes.longType:
                    long outputLong = 0;
                    if (input is long) outputLong = (long)input; else success = long.TryParse(input.ToString(), out outputLong);
                    return outputLong;
                case ValueStoreValueTypes.stringType:
                    return input.ToString();
                default:
                    return input;
            }
        }

        public static Type GetType(ValueStoreValueTypes valueType) {
            switch (valueType) {
                case ValueStoreValueTypes.stringType:
                    return typeof(string);
                case ValueStoreValueTypes.intType:
                    return typeof(int);
                case ValueStoreValueTypes.longType:
                    return typeof(long);
                case ValueStoreValueTypes.floatType:
                    return typeof(float);
                case ValueStoreValueTypes.doubleType:
                    return typeof(double);
                case ValueStoreValueTypes.boolType:
                    return typeof(bool);
                case ValueStoreValueTypes.objectType:
                    return typeof(object);
                default:
                    return null;
            }
        }

        public override BaseSolutionComponentView Activate() { return SolutionComponentViewManager.Show(this); }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(Label);
                sw.Write((int)_valueType);
                sw.Write(_savableDefaultValue);
                sw.Write(_isUniqueForEachConnection);
                sw.Write(_clearBeforeTest);
                sw.Write(_publish);
                sw.AddToInfo(info);
            }
            sw = null;
        }

        private void PublishValue(string owner, string value) {
            if (Publisher.Settings.PublisherEnabled && Test != null && ResultSetId != null) {
                var publishItem = new TestEvent();
                publishItem.Test = Test;
                publishItem.TestEventType = (int)TestEventType.TestValue;
                publishItem.Parameters = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("Label", Label),
                    new KeyValuePair<string, string>("Owner", owner),
                    new KeyValuePair<string, string>("Value", value)
                };

                Publisher.Send(publishItem, ResultSetId);
            }
        }
        #endregion
    }
}
