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
    /// <summary>
    /// Describes how a log should look like to be usable in a stresstest.
    /// </summary>
    [ContextMenu(new[] { "Activate_Click", "Add_Click", "Export_Click", "Clear_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" },
        new[] { "Edit", "Add Syntax Item", "Export", "Clear", "Remove", "Copy", "Cut", "Duplicate", "Paste" })]
    [Hotkeys(new[] { "Activate_Click", "Add_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" },
        new[] { Keys.Enter, Keys.Insert, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D), (Keys.Control | Keys.V) })]
    [DisplayName("Log Rule Set"), Serializable]
    public class LogRuleSet : BaseRuleSet, ISerializable {
        [field: NonSerialized]
        public event EventHandler LogRuleSetChanged;

        #region Fields
        private bool _actionizeOnComment = true;
        private string _beginCommentString = string.Empty;
        private uint _beginTimestampIndex;
        private string _endCommentString = string.Empty;
        private uint _endTimestampIndex;
        private string _singleLineCommentString = string.Empty;
        #endregion

        #region Properties
        [SavableCloneable, PropertyControl(1)]
        [Description("If no delimiter is given, the log entry will not be splitted into parts (space = valid). Please use <16 0C 02 12$> as it is the default for the log recorder. Just like new lines this is replaced by \"◦\" in the labels of the log entry controls for readability."),
        DisplayName("Child Delimiter")]
        public override string ChildDelimiter {
            get { return base.ChildDelimiter; }
            set { base.ChildDelimiter = value; }
        }

        [SavableCloneable, PropertyControl(3)]
        [Description("A string that specifies single line comments, for example: \"//\"."),
         DisplayName("Single Line Comment String")]
        public string SingleLineCommentString {
            get { return _singleLineCommentString; }
            set { _singleLineCommentString = value; }
        }

        [SavableCloneable, PropertyControl(4)]
        [Description("The begin delimiter of comments, will be ignored if an end delimiter is not supplied."),
         DisplayName("Begin Comment String")]
        public string BeginCommentString {
            get { return _beginCommentString; }
            set { _beginCommentString = value; }
        }

        [SavableCloneable, PropertyControl(5)]
        [Description("The end delimiter of comments, will be ignored if a begin delimiter is not supplied."),
         DisplayName("End Comment String")]
        public string EndCommentString {
            get { return _endCommentString; }
            set { _endCommentString = value; }
        }

        [SavableCloneable, PropertyControl(6)]
        [Description("The entries of a log between a comment can be grouped into a user action."),
         DisplayName("Actionize On Comment")]
        public bool ActionizeOnComment {
            get { return _actionizeOnComment; }
            set { _actionizeOnComment = value; }
        }

        [SavableCloneable, PropertyControl(7)]
        [Description("The index of the syntax item defining the logged timestamp for when a request was send."),
         DisplayName("Begin Timestamp Index")]
        public uint BeginTimestampIndex {
            get { return Count < _beginTimestampIndex ? 0 : _beginTimestampIndex; }
            set { _beginTimestampIndex = value; }
        }

        [SavableCloneable, PropertyControl(8)]
        [Description("The index of the syntax item defining the logged timestamp for a request was answered."),
         DisplayName("End Timestamp Index")]
        public uint EndTimestampIndex {
            get { return Count < _endTimestampIndex ? 0 : _endTimestampIndex; }
            set { _endTimestampIndex = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Describes how a log should look like to be usable in a stresstest.
        /// </summary>
        public LogRuleSet() { SolutionComponentChanged += BaseItem_SolutionComponentChanged; }
        /// <summary>
        ///     Only for sending from master to slave.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public LogRuleSet(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                Label = sr.ReadString();
                ChildDelimiter = sr.ReadString();
                _beginTimestampIndex = sr.ReadUInt32();
                _endTimestampIndex = sr.ReadUInt32();

                AddRangeWithoutInvokingEvent(sr.ReadCollection<BaseItem>(new List<BaseItem>()), false);
            }
            sr = null;
            //Not pretty, but helps against mem saturation.
            GC.Collect();
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
                sw.Write(_beginTimestampIndex);
                sw.Write(_endTimestampIndex);

                sw.Write(this);
                sw.AddToInfo(info);
            }
            sw = null;
            //Not pretty, but helps against mem saturation.
            GC.Collect();
        }

        private void BaseItem_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (LogRuleSetChanged != null && (sender == this || DeepContains(this, sender as BaseItem)))
                LogRuleSetChanged(this, null);
        }

        private bool DeepContains(BaseItem parent, BaseItem possibleChild) {
            bool contains = false;
            if (possibleChild is SyntaxItem || possibleChild is Rule)
                if (parent.Contains(possibleChild))
                    contains = true;
                else
                    foreach (BaseItem newParent in parent)
                        if (DeepContains(newParent, possibleChild)) {
                            contains = true;
                            break;
                        }
            return contains;
        }

        protected new void Add_Click(object sender, EventArgs e) { Add(new LogSyntaxItem()); }

        #endregion
    }
}