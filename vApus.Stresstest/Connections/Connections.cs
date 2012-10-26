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

namespace vApus.Stresstest
{
    //[ContextMenu(new string[] { "Add_Click", "Import_Click", "Test_Click", "SortItemsByLabel_Click", "Clear_Click", "Paste_Click" }, new string[] { "Add Connection", "Import Connection(s)", "Test All Connections", "Sort", "Clear", "Paste" })]
    //[Hotkeys(new string[] { "Add_Click", "Paste_Click" }, new Keys[] { Keys.Insert, (Keys.Control | Keys.V) })]
    public class Connections : BaseItem
    {
        public Connections()
        {
            AddAsDefaultItem(new ConnectionProxies());
        }
        private void Add_Click(object sender, EventArgs e)
        {
            Add(new Connection());
        }
        private void Test_Click(object sender, EventArgs e)
        {
            SolutionComponentViewManager.Show(this, typeof(TestAllConnections));
        }
        public override void Clear()
        {
            List<BaseItem> itemsCopy = new List<BaseItem>();
            foreach (BaseItem item in this)
                if (!(item is Connection))
                    itemsCopy.Add(item);
            base.Clear();
            AddRange(itemsCopy);
        }
    }
}
