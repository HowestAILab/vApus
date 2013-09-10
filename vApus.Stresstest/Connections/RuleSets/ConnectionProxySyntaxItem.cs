/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.Stresstest {
    [ContextMenu( new[] { "Activate_Click", "AddRule_Click", "Clear_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" },
                  new[] { "Edit", "Add Rule", "Clear", "Remove", "Copy", "Cut", "Duplicate", "Paste" })]
    [Hotkeys( new[] { "Activate_Click", "AddRule_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click", "Paste_Click" },
              new[] { Keys.Enter, Keys.Insert, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D), (Keys.Control | Keys.V) })]
    [DisplayName("Syntax Item"), Serializable]
    public class ConnectionProxySyntaxItem : SyntaxItem {
        public ConnectionProxySyntaxItem() {
            base._optional = true;
        }
        public new string ChildDelimiter {
            get { return string.Empty; }
            set { }
        }
        public new uint Occurance {
            get { return 1; }
            set { }
        }
        public new bool Optional {
            get { return base.Optional; }
            set { }
        }
        protected new void AddRule_Click(object sender, EventArgs e) {
            if (Count == 0)
                base.AddRule_Click(sender, e);
            else
                MessageBox.Show("Only one rule can be added.", string.Empty, MessageBoxButtons.OK,  MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        }
    }
}