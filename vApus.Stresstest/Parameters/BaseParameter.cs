/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    /// <summary>
    /// Values in requests can be parameterized when stress testing by the means of adding parameter tokens. This class is the base for all parameters.
    /// </summary>
    [ContextMenu(new[] { "Activate_Click", "Remove_Click", "Export_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { "Edit", "Remove", "Export", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    [Serializable]
    public abstract class BaseParameter : LabeledBaseItem {
        public enum Fixed {
            Prefix = 0,
            Suffix = 1,
            [Description("Pre- and Suffix")]
            PrefixAndSuffix = 2
        }

        #region Fields
        internal HashSet<object> _chosenValues = new HashSet<object>();

        protected object _lock = new object();
        protected int _tokenNumericIdentifier = -1;
        #endregion

        #region Properties
        public string Value { get; internal set; }

        /// <summary>
        ///     To synchronize the numeric portion of the parameter token in the requests when a parameter is added or removed.
        ///     (One-based)
        /// </summary>
        public int TokenNumericIdentifier {
            get {
                lock (_lock) {
                    if (_tokenNumericIdentifier == -1)
                        DetermineTokenNumericIdentifier();

                    return _tokenNumericIdentifier;
                }
            }
            set { Interlocked.Exchange(ref _tokenNumericIdentifier, value); }
        }

        [PropertyControl(int.MaxValue), DisplayName("Read me")]
        public string ReadMe { get { return "All parameter values are determined before the stress test starts."; } }

        private void DetermineTokenNumericIdentifier() {
            _tokenNumericIdentifier = 0;

            //One based
            if (Parent != null) {
                object grandParent = Parent.GetParent();
                if (grandParent != null && grandParent is Parameters) {
                    var parameters = grandParent as Parameters;
                    foreach (BaseParameter parameter in parameters.GetAllParameters()) {
                        ++_tokenNumericIdentifier;
                        if (parameter == this)
                            break;
                    }
                }
            }
        }
        #endregion

        #region Functions
        /// <summary>
        ///     Calculates a new value.
        /// </summary>
        public abstract void Next();

        public abstract void ResetValue();

        public override BaseSolutionComponentView Activate() {  return SolutionComponentViewManager.Show(this, typeof(ParameterView));   }
        #endregion
    }
}