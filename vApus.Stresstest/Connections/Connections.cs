/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.Stresstest {
    /// <summary>
    /// Groups all connections and proxies.
    /// </summary>
    [ContextMenu(new[] { "Activate_Click", "Add_Click", "Import_Click", "SortItemsByLabel_Click", "Clear_Click", "Paste_Click" },
        new[] { "Table View / Test All Connections", "Add Connection", "Import Connection(s)", "Sort", "Clear", "Paste" })]
    [Hotkeys(new[] { "Activate_Click", "Add_Click", "Paste_Click" }, new[] { Keys.Enter, Keys.Insert, (Keys.Control | Keys.V) })]
    public class Connections : BaseItem {

        #region Constructor
        /// <summary>
        /// Groups all connections and proxies.
        /// </summary>
        public Connections() { AddAsDefaultItem(new ConnectionProxies()); }
        #endregion

        #region Functions
        private void Add_Click(object sender, EventArgs e) { Add(new Connection()); }

        /// <summary>
        /// Only clears connections, other items are not cleared.
        /// </summary>
        public override void Clear() {
            var itemsCopy = new List<BaseItem>();
            foreach (BaseItem item in this)
                if (!(item is Connection)) itemsCopy.Add(item);
            base.Clear();
            AddRange(itemsCopy);
        }

        public override BaseSolutionComponentView Activate() { return SolutionComponentViewManager.Show(this); }
        #endregion
    }
}