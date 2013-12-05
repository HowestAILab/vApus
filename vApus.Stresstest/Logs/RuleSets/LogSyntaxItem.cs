/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
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

namespace vApus.Stresstest {
    [ContextMenu(new[] { "Activate_Click", "AddSyntaxItem_Click", "AddRule_Click", "Clear_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" },
                 new[] { "Edit", "Add Syntax Item", "Add Rule", "Clear", "Remove", "Copy", "Cut", "Duplicate", "Paste" })]
    [Hotkeys(new[] { "Activate_Click", "AddSyntaxItem_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" },
             new[] { Keys.Enter, Keys.Insert, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D), (Keys.Control | Keys.V) })]
    [DisplayName("Syntax Item"), Serializable]
    public class LogSyntaxItem : SyntaxItem, ISerializable {

        public LogSyntaxItem() { }
        public LogSyntaxItem(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                _childDelimiter = sr.ReadString();
                _defaultValue = sr.ReadString();
                _occurance = sr.ReadUInt32();
                _optional = sr.ReadBoolean();

                AddRangeWithoutInvokingEvent(sr.ReadCollection<BaseItem>(new List<BaseItem>()));
            }
            sr = null;
        }

        protected new void AddSyntaxItem_Click(object sender, EventArgs e) {
            bool invalid = false;
            foreach (BaseItem item in this)
                if (!(item is LogSyntaxItem)) {
                    invalid = true;
                    break;
                }
            if (invalid) {
                if (
                    MessageBox.Show(
                        "If this Rules an Indexed Syntax Item cannot be added.\nDo You want to put these Rules in an optional Indexed Syntax Item?",
                        string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) ==
                    DialogResult.Yes) {
                    var syntaxItem = new LogSyntaxItem();
                    syntaxItem.Optional = true;
                    foreach (BaseItem item in this) {
                        item.Parent = syntaxItem;
                        syntaxItem.AddWithoutInvokingEvent(item);
                    }
                    Clear();
                    Add(syntaxItem);
                } else {
                    return;
                }
            }
            Add(new LogSyntaxItem());
        }
        protected new void AddRule_Click(object sender, EventArgs e) {
            bool invalid = false;
            foreach (BaseItem item in this)
                if (item is LogSyntaxItem) {
                    invalid = true;
                    break;
                }
            if (invalid) {
                if (
                    MessageBox.Show(
                        "If this contains Syntax Items a Rule cannot be added.\nDo You want to put it in an optional Indexed Syntax Item?",
                        string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) ==
                    DialogResult.Yes) {
                    var syntaxItem = new LogSyntaxItem();
                    syntaxItem.Parent = this;
                    syntaxItem.Optional = true;
                    syntaxItem.AddWithoutInvokingEvent(new Rule());
                    Add(syntaxItem);
                }
            } else {
                Add(new Rule());
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(_childDelimiter);
                sw.Write(_defaultValue);
                sw.Write(_occurance);
                sw.Write(_optional);

                sw.Write(this);
                sw.AddToInfo(info);
            }
            sw = null;
        }
    }
}