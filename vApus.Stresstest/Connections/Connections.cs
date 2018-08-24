/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.StressTest {
    /// <summary>
    /// Groups all connections and proxies.
    /// </summary>
    [ContextMenu(new[] { "Activate_Click", "Add_Click", "Import_Click", "SortItemsByLabel_Click", "Clear_Click", "Paste_Click" },
        new[] { "Table view / Test all connections", "Add connection", "Import connection(s)", "Sort", "Clear", "Paste" })]
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