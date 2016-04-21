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
        public enum RuleValueTypes {
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
            [Description("64-bit floating point")]
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
        private RuleValueTypes _valueType;

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
        public RuleValueTypes ValueType {
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
                _valueType = (RuleValueTypes)sr.ReadInt32();
            }
            sr = null;
        }
        #endregion

        #region Functions

        internal LexicalResult TryLexicalAnaysis(string input, out ASTNode output) {
            output = new ASTNode();
            output.Value = input;

            bool success;
            CastOrParseInputToType(input, _valueType, out success);

            if (!success) {
                output.Error = string.Format("The input could not be parsed to {0}.", _valueType);
                return LexicalResult.Error;
            }

            if (_regExp.Length == 0 || Regex.IsMatch(input, _regExp, (_ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None)))
                return LexicalResult.OK;

            output.Error = string.Format("The value modifier is not valid for this input ({0} \"{1}\", Value Type {2} ).", input, _regExp, _valueType);
            return LexicalResult.Error;
        }

        private object CastOrParseInputToType(object input, RuleValueTypes valueType, out bool success) {
            success = true;
            switch (valueType) {
                case RuleValueTypes.boolType:
                    bool outputBool = false;
                    if (input is bool) outputBool = (bool)input; else success = bool.TryParse(input.ToString(), out outputBool);
                    return outputBool;
                case RuleValueTypes.charType:
                    char outputChar = new char();
                    if (input is char) outputChar = (char)input; else success = char.TryParse(input.ToString(), out outputChar);
                    return outputChar;
                case RuleValueTypes.decimalType:
                    decimal outputDecimal = 0;
                    if (input is decimal) outputDecimal = (decimal)input; else success = decimal.TryParse(input.ToString(), out outputDecimal);
                    return outputDecimal;
                case RuleValueTypes.doubleType:
                    double outputDouble = 0;
                    if (input is double) outputDouble = (double)input; else success = double.TryParse(input.ToString(), out outputDouble);
                    return outputDouble;
                case RuleValueTypes.floatType:
                    float outputFloat = 0;
                    if (input is float) outputFloat = (float)input; else success = float.TryParse(input.ToString(), out outputFloat);
                    return outputFloat;
                case RuleValueTypes.intType:
                    int outputInt = 0;
                    if (input is int) outputInt = (int)input; else success = int.TryParse(input.ToString(), out outputInt);
                    return outputInt;
                case RuleValueTypes.longType:
                    long outputLong = 0;
                    if (input is long) outputLong = (long)input; else success = long.TryParse(input.ToString(), out outputLong);
                    return outputLong;
                case RuleValueTypes.shortType:
                    short outputShort = 0;
                    if (input is short) outputShort = (short)input; else success = short.TryParse(input.ToString(), out outputShort);
                    return outputShort;
                case RuleValueTypes.stringType:
                    return input.ToString();
                case RuleValueTypes.uintType:
                    uint outputUint = 0;
                    if (input is uint) outputUint = (uint)input; else success = uint.TryParse(input.ToString(), out outputUint);
                    return outputUint;
                case RuleValueTypes.ulongType:
                    ulong outputUlong = 0;
                    if (input is ulong) outputUlong = (ulong)input; else success = ulong.TryParse(input.ToString(), out outputUlong);
                    return outputUlong;
                case RuleValueTypes.ushortType:
                    ushort outputUshort = 0;
                    if (input is ushort) outputUshort = (ushort)input; else success = ushort.TryParse(input.ToString(), out outputUshort);
                    return outputUshort;
                default:
                    return input;
            }
        }

        public static Type GetType(RuleValueTypes valueType) {
            switch (valueType) {
                case RuleValueTypes.stringType:
                    return typeof(string);
                case RuleValueTypes.charType:
                    return typeof(char);
                case RuleValueTypes.shortType:
                    return typeof(short);
                case RuleValueTypes.intType:
                    return typeof(int);
                case RuleValueTypes.longType:
                    return typeof(long);
                case RuleValueTypes.ushortType:
                    return typeof(ushort);
                case RuleValueTypes.uintType:
                    return typeof(uint);
                case RuleValueTypes.ulongType:
                    return typeof(ulong);
                case RuleValueTypes.floatType:
                    return typeof(float);
                case RuleValueTypes.doubleType:
                    return typeof(double);
                case RuleValueTypes.decimalType:
                    return typeof(decimal);
                case RuleValueTypes.boolType:
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