/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest
{
    [ContextMenu(new string[] { "Activate_Click", "Remove_Click", "Export_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new string[] { "Edit", "Remove", "Export", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new string[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new Keys[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    [Serializable]
    public abstract class BaseParameter : LabeledBaseItem
    {
        public enum Fixed
        {
            Prefix = 0,
            Suffix = 1,
            [Description("Pre- and Suffix")]
            PrefixAndSuffix = 2
        }

        #region Fields
        internal Random _r = new Random();
        internal HashSet<object> _chosenValues = new HashSet<object>();
        internal string _value;
        internal string _description = string.Empty;

        /// <summary>
        /// Will be determined if equals -1.
        /// </summary>
        internal int _tokenNumericIdentifier = -1;
        #endregion

        #region Properties
        
        public string Value
        {
            get { return _value; }
        }
        /// <summary>
        /// To synchronize the numeric portion of the parameter token in the log entries when a parameter is added or removed. 
        /// 
        /// (One-based)
        /// </summary>
        
        public int TokenNumericIdentifier
        {
            get { return _tokenNumericIdentifier; }
            internal set { _tokenNumericIdentifier = value; }
        }
        [PropertyControl(int.MaxValue), DisplayName("Read Me")]
        public string ReadMe
        {
            get { return "All parameter values are determined before the stresstest starts."; }
        }
        #endregion

        public BaseParameter()
        {
            //Initialize the tokenindex when a parent has been assigned to this.
            ObjectExtension.ParentChanged += new ObjectExtension.ParentChangedEventHandler(ObjectExtension_ParentChanged);
        }

        #region Functions
        private void ObjectExtension_ParentChanged(ObjectExtension.ParentOrTagChangedEventArgs parentOrTagChangedEventArgs)
        {
            if (_tokenNumericIdentifier == -1 && parentOrTagChangedEventArgs.New != null)
            {
                Parameters parameters = null;

                //Get the parameters collection.
                if (parentOrTagChangedEventArgs.New is Parameters)
                {
                    parameters = parentOrTagChangedEventArgs.New as Parameters;

                }
                else
                {
                    var o = parentOrTagChangedEventArgs.New.GetParent();
                    if (o != null)
                        parameters = o as Parameters;
                }

                //Determine the token using the 'index' of this.
                if (parameters != null)
                {
                    _tokenNumericIdentifier = 0;

                    //One based
                    foreach (BaseParameter parameter in parameters.GetAllParameters())
                    {
                        ++_tokenNumericIdentifier;
                        if (parameter == this)
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Calculates a new value.
        /// When not random a unique value will always be calculated.
        /// Step is not applicable for Random.
        /// </summary>
        public abstract void Next();
        public abstract void ResetValue();
        public override void Activate()
        {
            SolutionComponentViewManager.Show(this, typeof(ParameterView));
        }
        #endregion
    }
}
