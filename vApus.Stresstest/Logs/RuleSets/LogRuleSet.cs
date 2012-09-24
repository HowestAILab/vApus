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
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;
using System.Collections.Generic;

namespace vApus.Stresstest
{
    [ContextMenu(new string[] { "Activate_Click", "Add_Click", "Export_Click", "Clear_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" }, new string[] { "Edit", "Add Syntax Item", "Export", "Clear", "Remove", "Copy", "Cut", "Duplicate", "Paste" })]
    [Hotkeys(new string[] { "Activate_Click", "Add_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" }, new Keys[] { Keys.Enter, Keys.Insert, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D), (Keys.Control | Keys.V) })]
    [DisplayName("Log Rule Set"), Serializable]
    public class LogRuleSet : BaseRuleSet, ISerializable
    {
        [field: NonSerialized]
        public event EventHandler LogRuleSetChanged;

        #region Fields
        private string _singleLineCommentString = string.Empty, _beginCommentString = string.Empty, _endCommentString = string.Empty;
        private bool _actionizeOnComment = true, _actionizeOnFile = true;
        private uint _beginTimestampIndex, _endTimestampIndex;
        #endregion

        #region Properties
        [SavableCloneable, PropertyControl(3)]
        [Description("A string that specifies single line comments, for example: \"//\"."), DisplayName("Single Line Comment String")]
        public string SingleLineCommentString
        {
            get { return _singleLineCommentString; }
            set { _singleLineCommentString = value; }
        }
        [SavableCloneable, PropertyControl(4)]
        [Description("The begin delimiter of comments, will be ignored if an end delimiter is not supplied."), DisplayName("Begin Comment String")]
        public string BeginCommentString
        {
            get { return _beginCommentString; }
            set { _beginCommentString = value; }
        }
        [SavableCloneable, PropertyControl(5)]
        [Description("The end delimiter of comments, will be ignored if a begin delimiter is not supplied."), DisplayName("End Comment String")]
        public string EndCommentString
        {
            get { return _endCommentString; }
            set { _endCommentString = value; }
        }
        [SavableCloneable, PropertyControl(6)]
        [Description("The entries of a log between a comment can be grouped into a user action."), DisplayName("Actionize On Comment")]
        public bool ActionizeOnComment
        {
            get { return _actionizeOnComment; }
            set { _actionizeOnComment = value; }
        }
        [SavableCloneable, PropertyControl(7)]
        [Description("Different files can be grouped into different user actions."), DisplayName("Actionize On File")]
        public bool ActionizeOnFile
        {
            get { return _actionizeOnFile; }
            set { _actionizeOnFile = value; }
        }
        [SavableCloneable, PropertyControl(8)]
        [Description("The index of the syntax item defining the logged timestamp for when a request was send."), DisplayName("Begin Timestamp Index")]
        public uint BeginTimestampIndex
        {
            get
            {
                return this.Count < _beginTimestampIndex ? 0 : _beginTimestampIndex;
            }
            set { _beginTimestampIndex = value; }
        }
        [SavableCloneable, PropertyControl(9)]
        [Description("The index of the syntax item defining the logged timestamp for a request was answered."), DisplayName("End Timestamp Index")]
        public uint EndTimestampIndex
        {
            get
            {
                return this.Count < _endTimestampIndex ? 0 : _endTimestampIndex;
            }
            set { _endTimestampIndex = value; }
        }
        #endregion

        #region Constructors
        public LogRuleSet()
        {
            BaseItem.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(BaseItem_SolutionComponentChanged);
        }
               /// <summary>
        /// Only for sending from master to slave.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public LogRuleSet(SerializationInfo info, StreamingContext ctxt)
        {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info))
            {
                Label = sr.ReadString();
                _childDelimiter = sr.ReadString();
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
        private void BaseItem_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            if (LogRuleSetChanged != null && (sender == this || DeepContains(this, sender as BaseItem)))
                LogRuleSetChanged(this, null);
        }
        private bool DeepContains(BaseItem parent, BaseItem possibleChild)
        {
            if (possibleChild is SyntaxItem || possibleChild is Rule)
                if (parent.Contains(possibleChild))
                    return true;
                else
                    foreach (BaseItem newParent in parent)
                        if (DeepContains(newParent, possibleChild))
                            return true;
            return false;
        }
        protected new void Add_Click(object sender, EventArgs e)
        {
            Add(new LogSyntaxItem());
        }


        /// <summary>
        /// Only for sending from master to slave.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter())
            {
                sw.Write(Label);
                sw.Write(_childDelimiter);
                sw.Write(_beginTimestampIndex);
                sw.Write(_endTimestampIndex);

                sw.Write(this);
                sw.AddToInfo(info);
            }
            sw = null;
            //Not pretty, but helps against mem saturation.
            GC.Collect();
        }
        #endregion
    }
}
