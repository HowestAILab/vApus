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

namespace vApus.DistributedTesting
{
    [ContextMenu(new string[] { "Add_Click", "SortItemsByLabel_Click", "Clear_Click", "Paste_Click" }, new string[] { "Add Distributed Test", "Sort", "Clear", "Paste" })]
    [Hotkeys(new string[] { "Add_Click", "Paste_Click" }, new Keys[] { Keys.Insert, (Keys.Control | Keys.V) })]
    [DisplayName("Distributed Testing")]
    public class DistributedTestingProject : BaseProject
    {
        private void Add_Click(object sender, EventArgs e)
        {
            Add(new DistributedTest());
        }
    }
}
