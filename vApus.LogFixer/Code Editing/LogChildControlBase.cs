/*
 * Copyright 2006-2007 (c) Blancke Karen, Cavaliere Leandro, Kets Brecht, Vandroemme Dieter
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */

using System;
using System.Windows.Forms;
using vApus.SolutionTree;
using System.ComponentModel;

namespace vApus.Stresstest
{
    [ToolboxItem(false)]
    public partial class LogChildControlBase : UserControl
    {
        #region Events
        public event EventHandler CheckedChanged;
        #endregion

        #region Fields
        public const int MINIMUMLEFTMARGIN = 3;
        #endregion

        #region Properties
        /// <summary>
        /// Use this in a derived class.
        /// </summary>
        public virtual BaseItem LogChild
        {
            get { throw new Exception("Use this in a derived class"); }
        }
        /// <summary>
        /// The standard indentationLevel, can be overridden though.
        /// </summary>
        public virtual uint IndentationLevel
        {
            get { return 0; }
        }
        public virtual bool Checked
        {
            get { return false; }
            set { }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public LogChildControlBase()
        {
        }
        #endregion

        #region Functions
        /// <summary>
        /// Use this in a derived class.
        /// </summary>
        public virtual void ClearLogChild()
        {
            throw new Exception("Use this in a derived class");
        }
        protected void InvokeCheckedChanged()
        {
            if (CheckedChanged != null)
                CheckedChanged(this, null);
        }
        #endregion
    }
}
