/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.Monitor {
    [ContextMenu(new[] { "Add_Click", "SortItemsByLabel_Click", "Clear_Click", "Paste_Click" },
        new[] { "Add Monitor", "Sort", "Clear", "Paste" })]
    [Hotkeys(new[] { "Add_Click", "Paste_Click" }, new[] { Keys.Insert, (Keys.Control | Keys.V) })]
    [DisplayName("Monitors")]
    public class MonitorProject : BaseProject {
        private void Add_Click(object sender, EventArgs e) {
            Add(new Monitor());
        }
    }
}