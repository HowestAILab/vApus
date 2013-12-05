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

namespace vApus.Stresstest {
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
        private bool _usePasswordChar;
        private ValueTypes _valueType;

        #endregion

        #region Properties

        [SavableCloneable, PropertyControl(0)]
        [Description("The actual rule. [http://www.google.com/search?q=regular+expressions]"),
         DisplayName("Regular Expression")]
        public string RegExp {
            get { return _regExp; }
            set { _regExp = value; }
        }

        [SavableCloneable, PropertyControl(1)]
        [Description("Ignore case for the regular expression."), DisplayName("Ignore Case")]
        public bool IgnoreCase {
            get { return _ignoreCase; }
            set { _ignoreCase = value; }
        }

        [SavableCloneable, PropertyControl(2)]
        [Description("The input must equal or castable/parsable to the selected type."), DisplayName("Value Type")]
        public ValueTypes ValueType {
            get { return _valueType; }
            set { _valueType = value; }
        }

        [SavableCloneable, PropertyControl(3)]
        [Description(
            "If implemented, textual input will be displayed using the password character. Where this is implemented encryption is always used."
            ), DisplayName("Use Password Character")]
        public bool UsePasswordChar {
            get { return _usePasswordChar; }
            set { _usePasswordChar = value; }
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
            try {
                CastOrParseInputToType(input, _valueType);
            } catch {
                output.Error = string.Format("The input could not be parsed to {0}.", _valueType);
                return LexicalResult.Error;
            }

            if (_regExp.Length == 0 || Regex.IsMatch(input, _regExp, (_ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None)))
                return LexicalResult.OK;

            output.Error = string.Format("The value modifier is not valid for this input ({0} \"{1}\", Value Type {2} ).", input, _regExp, _valueType);
            return LexicalResult.Error;
        }

        private object CastOrParseInputToType(object input, ValueTypes valueType) {
            try {
                switch (valueType) {
                    case ValueTypes.boolType:
                        return (input is bool) ? (bool)input : bool.Parse(input.ToString());
                    case ValueTypes.charType:
                        return (input is char) ? (char)input : char.Parse(input.ToString());
                    case ValueTypes.decimalType:
                        return (input is decimal) ? (decimal)input : decimal.Parse(input.ToString());
                    case ValueTypes.doubleType:
                        return (input is double) ? (double)input : double.Parse(input.ToString());
                    case ValueTypes.floatType:
                        return (input is float) ? (float)input : float.Parse(input.ToString());
                    case ValueTypes.intType:
                        return (input is int) ? (int)input : int.Parse(input.ToString());
                    case ValueTypes.longType:
                        return (input is long) ? (long)input : long.Parse(input.ToString());
                    case ValueTypes.shortType:
                        return (input is short) ? (short)input : short.Parse(input.ToString());
                    case ValueTypes.stringType:
                        return input.ToString();
                    case ValueTypes.uintType:
                        return (input is uint) ? (uint)input : uint.Parse(input.ToString());
                    case ValueTypes.ulongType:
                        return (input is ulong) ? (ulong)input : ulong.Parse(input.ToString());
                    case ValueTypes.ushortType:
                        return (input is ushort) ? (ushort)input : ushort.Parse(input.ToString());
                    default:
                        return input;
                }
            } catch {
                throw new Exception(
                    "Please make sure the \"Value to Check Against\" is castable/parsable to the selected \"Value Type\".");
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