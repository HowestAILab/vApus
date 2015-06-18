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

namespace vApus.StressTest {
    /// <summary>
    /// Describes how a scenario should look like to be usable in a stress test.
    /// </summary>
    [ContextMenu(new[] { "Activate_Click", "Add_Click", "Export_Click", "Clear_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" },
        new[] { "Edit", "Add syntax item", "Export", "Clear", "Remove", "Copy", "Cut", "Duplicate", "Paste" })]
    [Hotkeys(new[] { "Activate_Click", "Add_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" },
        new[] { Keys.Enter, Keys.Insert, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D), (Keys.Control | Keys.V) })]
    [DisplayName("Scenario rule set"), Serializable]
    public class ScenarioRuleSet : BaseRuleSet, ISerializable {
        [field: NonSerialized]
        public event EventHandler ScenarioRuleSetChanged;

        #region Fields
        private bool _actionizeOnComment = true;
        private string _beginCommentString = "<!--";
        private string _endCommentString = "-->";
        private string _singleLineCommentString = string.Empty;
        private uint _clientConnectedTimestampIndex, _sentRequestTimestampIndex;
        #endregion

        #region Properties
        [SavableCloneable, PropertyControl(1)]
        [Description("If no delimiter is given, the request will not be splitted into parts (space = valid). Please use <16 0C 02 12$> as it is the default for the scenario recorder."),
        DisplayName("Child delimiter")]
        public override string ChildDelimiter {
            get { return base.ChildDelimiter; }
            set { base.ChildDelimiter = value; }
        }

        [SavableCloneable, PropertyControl(3)]
        [Description("A string that specifies single line comments, for example: \"//\"."),
         DisplayName("Single line comment string")]
        public string SingleLineCommentString {
            get { return _singleLineCommentString; }
            set { _singleLineCommentString = value; }
        }

        [SavableCloneable, PropertyControl(4)]
        [Description("The begin delimiter of comments, will be ignored if an end delimiter is not supplied."),
         DisplayName("Begin comment string")]
        public string BeginCommentString {
            get { return _beginCommentString; }
            set { _beginCommentString = value; }
        }

        [SavableCloneable, PropertyControl(5)]
        [Description("The end delimiter of comments, will be ignored if a begin delimiter is not supplied."),
         DisplayName("End comment string")]
        public string EndCommentString {
            get { return _endCommentString; }
            set { _endCommentString = value; }
        }

        [SavableCloneable, PropertyControl(6)]
        [Description("The entries of a scenario between a comment can be grouped into a user action."),
         DisplayName("Actionize on comment")]
        public bool ActionizeOnComment {
            get { return _actionizeOnComment; }
            set { _actionizeOnComment = value; }
        }

        [SavableCloneable, PropertyControl(7)]
        [Description("The ONE-BASED index of the syntax item defining the logged timestamp for when a client (browser) was connected. Set to 0 if you do not want to use it."),
         DisplayName("Client connected timestamp index")]
        public uint ClientConnectedTimestampIndex {
            get { return Count < _clientConnectedTimestampIndex ? 0 : _clientConnectedTimestampIndex; }
            set { _clientConnectedTimestampIndex = value; }
        }

        [SavableCloneable, PropertyControl(8)]
        [Description("The ONE-BASED index of the syntax item defining the logged timestamp for when a request was sent. Set to 0 if you do not want to use it."),
         DisplayName("Sent request timestamp index")]
        public uint SentRequestTimestampIndex {
            get { return Count < _sentRequestTimestampIndex ? 0 : _sentRequestTimestampIndex; }
            set { _sentRequestTimestampIndex = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Describes how a scenario should look like to be usable in a stress test.
        /// </summary>
        public ScenarioRuleSet() { 
            SolutionComponentChanged += BaseItem_SolutionComponentChanged; 
        }
        /// <summary>
        ///     Only for sending from master to slave.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public ScenarioRuleSet(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                Label = sr.ReadString();
                ChildDelimiter = sr.ReadString();
                _sentRequestTimestampIndex = sr.ReadUInt32();

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
                sw.Write(_sentRequestTimestampIndex);

                sw.Write(this);
                sw.AddToInfo(info);
            }
            sw = null;
        }

        private void BaseItem_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (ScenarioRuleSetChanged != null && (sender == this || DeepContains(this, sender as BaseItem)))
                ScenarioRuleSetChanged(this, null);
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

        protected new void Add_Click(object sender, EventArgs e) { Add(new ScenarioSyntaxItem()); }

        #endregion
    }
}