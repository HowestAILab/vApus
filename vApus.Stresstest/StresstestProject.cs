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
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.StressTest {
    [ContextMenu(new[] { "Add_Click", "SortItemsByLabel_Click", "Clear_Click", "Paste_Click" },
        new[] { "Add stress test", "Sort", "Clear", "Paste" })]
    [Hotkeys(new[] { "Add_Click", "Paste_Click" }, new[] { Keys.Insert, (Keys.Control | Keys.V) })]
    [DisplayName("Stress tests")]
    public class StressTestProject : BaseProject {

        #region Constructor
        public StressTestProject() {
            AddAsDefaultItem(new Parameters());
            AddAsDefaultItem(new Connections());
            AddAsDefaultItem(new Scenarios());
        }
        #endregion

        #region Functions
        private void Add_Click(object sender, EventArgs e) { Add(new StressTest()); }
        /// <summary>
        /// Remove all stress tests, other items will not be removed.
        /// </summary>
        public override void Clear() {
            var itemsCopy = new List<BaseItem>();
            foreach (BaseItem item in this)
                if (!(item is StressTest))
                    itemsCopy.Add(item);
            base.Clear();
            AddRange(itemsCopy);
        }
        #endregion
    }
}