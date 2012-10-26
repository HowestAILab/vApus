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

namespace vApus.Stresstest
{
    //[ContextMenu(new string[] { "Add_Click", "SortItemsByLabel_Click", "Clear_Click", "Paste_Click" }, new string[] { "Add Stresstest", "Sort", "Clear", "Paste" })]
    //[Hotkeys(new string[] { "Add_Click", "Paste_Click" }, new Keys[] { Keys.Insert, (Keys.Control | Keys.V) })]
    [DisplayName("Stresstests")]
    public class StresstestProject : BaseProject
    {
        public StresstestProject()
        {
            AddAsDefaultItem(new Parameters());
            AddAsDefaultItem(new Connections());
            AddAsDefaultItem(new Logs());
        }
        private void Add_Click(object sender, EventArgs e)
        {
            Add(new Stresstest());
        }
        public override void Clear()
        {
            List<BaseItem> itemsCopy = new List<BaseItem>();
            foreach (BaseItem item in this)
                if (!(item is Stresstest))
                    itemsCopy.Add(item);
            base.Clear();
            AddRange(itemsCopy);
        }
    }
}
