/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    [ContextMenu(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { "Edit", "Remove", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    [Serializable]
    public class Rule : LabeledBaseItem, ISerializable {

        #region Enum

        [Serializable]
        public enum ValueTypes {
            [Description("String")]
            stringType,
            [Description("Single character")]
            charType,
            [Description("16-bit integer")]
            shortType,
            [Description("32-bit integer")]
            intType,
            [Description("64-bit integer")]
            longType,
            [Description("Unsigned 16-bit integer")]
            ushortType,
            [Description("Unsigned 32-bit integer")]
            uintType,
            [Description("Unsigned 64-bit integer")]
            ulongType,
            [Description("32-bit floating point")]
            floatType,
            [Description("Numeric")]
            doubleType,
            [Description("Decimal")]
            decimalType,
            [Description("Boolean")]
            boolType
        }

        #endregion

        #region Fields

        private string _description = string.Empty;
        private bool _ignoreCase;
        private string _regExp = string.Empty;
        private bool _displayAsPassword;
        private ValueTypes _valueType;

        #endregion

        #region Properties

        [SavableCloneable, PropertyControl(0)]
        [Description("The actual rule. [http://www.google.com/search?q=regular+expressions]"),
         DisplayName("Regular expression")]
        public string RegExp {
            get { return _regExp; }
            set { _regExp = value; }
        }

        [SavableCloneable, PropertyControl(1)]
        [Description("Ignore case for the regular expression."), DisplayName("Ignore case")]
        public bool IgnoreCase {
            get { return _ignoreCase; }
            set { _ignoreCase = value; }
        }

        [SavableCloneable, PropertyControl(2)]
        [Description("The input must equal or castable/parsable to the selected type."), DisplayName("Value type")]
        public ValueTypes ValueType {
            get { return _valueType; }
            set { _valueType = value; }
        }

        [SavableCloneable, PropertyControl(3)]
        [Description(
            "If implemented, textual input will be displayed using the password character '*'. Where this is implemented encryption is always used."
            ), DisplayName("Display as password")]
        public bool DisplayAsPassword {
            get { return _displayAsPassword; }
            set { _displayAsPassword = value; }
        }

        [SavableCloneable, PropertyControl(4)]
        [Description("Describes this rule.")]
        public virtual string Description {
            get { return _description; }
            set { _description = value; }
        }

        #endregion

        #region Constructors
        public Rule() { }
        public Rule(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                _ignoreCase = sr.ReadBoolean();
                _regExp = sr.ReadString();
                _valueType = (ValueTypes)sr.ReadInt32();
            }
            sr = null;
        }
        #endregion

        #region Functions

        internal LexicalResult TryLexicalAnaysis(string input, out ASTNode output) {
            output = new ASTNode();
            output.Value = input;

            bool succes;
            CastOrParseInputToType(input, _valueType, out succes);

            if (!succes) {
                output.Error = string.Format("The input could not be parsed to {0}.", _valueType);
                return LexicalResult.Error;
            }

            if (_regExp.Length == 0 || Regex.IsMatch(input, _regExp, (_ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None)))
                return LexicalResult.OK;

            output.Error = string.Format("The value modifier is not valid for this input ({0} \"{1}\", Value Type {2} ).", input, _regExp, _valueType);
            return LexicalResult.Error;
        }

        private object CastOrParseInputToType(object input, ValueTypes valueType, out bool succes) {
            succes = true;
            switch (valueType) {
                case ValueTypes.boolType:
                    bool outputBool = false;
                    if (input is bool) outputBool = (bool)input; else succes = bool.TryParse(input.ToString(), out outputBool);
                    return outputBool;
                case ValueTypes.charType:
                    char outputChar = new char();
                    if (input is char) outputChar = (char)input; else succes = char.TryParse(input.ToString(), out outputChar);
                    return outputChar;
                case ValueTypes.decimalType:
                    decimal outputDecimal = 0;
                    if (input is decimal) outputDecimal = (decimal)input; else succes = decimal.TryParse(input.ToString(), out outputDecimal);
                    return outputDecimal;
                case ValueTypes.doubleType:
                    double outputDouble = 0;
                    if (input is double) outputDouble = (double)input; else succes = double.TryParse(input.ToString(), out outputDouble);
                    return outputDouble;
                case ValueTypes.floatType:
                    float outputFloat = 0;
                    if (input is float) outputFloat = (float)input; else succes = float.TryParse(input.ToString(), out outputFloat);
                    return outputFloat;
                case ValueTypes.intType:
                    int outputInt = 0;
                    if (input is int) outputInt = (int)input; else succes = int.TryParse(input.ToString(), out outputInt);
                    return outputInt;
                case ValueTypes.longType:
                    long outputLong = 0;
                    if (input is long) outputDecimal = (long)input; else succes = long.TryParse(input.ToString(), out outputLong);
                    return outputLong;
                case ValueTypes.shortType:
                    short outputShort = 0;
                    if (input is short) outputShort = (short)input; else succes = short.TryParse(input.ToString(), out outputShort);
                    return outputShort;
                case ValueTypes.stringType:
                    return input.ToString();
                case ValueTypes.uintType:
                    uint outputUint = 0;
                    if (input is uint) outputUint = (uint)input; else succes = uint.TryParse(input.ToString(), out outputUint);
                    return outputUint;
                case ValueTypes.ulongType:
                    ulong outputUlong = 0;
                    if (input is ulong) outputUlong = (ulong)input; else succes = ulong.TryParse(input.ToString(), out outputUlong);
                    return outputUlong;
                case ValueTypes.ushortType:
                    ushort outputUshort = 0;
                    if (input is ushort) outputUshort = (ushort)input; else succes = ushort.TryParse(input.ToString(), out outputUshort);
                    return outputUshort;
                default:
                    return input;
            }
        }

        public static Type GetType(ValueTypes valueType) {
            switch (valueType) {
                case ValueTypes.stringType:
                    return typeof(string);
                case ValueTypes.charType:
                    return typeof(char);
                case ValueTypes.shortType:
                    return typeof(short);
                case ValueTypes.intType:
                    return typeof(int);
                case ValueTypes.longType:
                    return typeof(long);
                case ValueTypes.ushortType:
                    return typeof(ushort);
                case ValueTypes.uintType:
                    return typeof(uint);
                case ValueTypes.ulongType:
                    return typeof(ulong);
                case ValueTypes.floatType:
                    return typeof(float);
                case ValueTypes.doubleType:
                    return typeof(double);
                case ValueTypes.decimalType:
                    return typeof(decimal);
                case ValueTypes.boolType:
                    return typeof(bool);
                default:
                    return null;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(_ignoreCase);
                sw.Write(_regExp);
                sw.Write((int)_valueType);
                sw.AddToInfo(info);
            }
            sw = null;
        }
        #endregion
    }
}