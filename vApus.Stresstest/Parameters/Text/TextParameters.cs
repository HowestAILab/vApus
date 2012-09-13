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

namespace vApus.Stresstest
{
    [ContextMenu(new string[] { "Add_Click", "Import_Click", "Clear_Click", "Paste_Click" }, new string[] { "Add Text Parameter", "Import Parameter(s)", "Clear", "Paste" })]
    [Hotkeys(new string[] { "Add_Click", "Paste_Click" }, new Keys[] { Keys.Insert, (Keys.Control | Keys.V) })]
    [DisplayName("Text Parameters")]
    [Serializable]
    public class TextParameters : BaseItem
    {
        private void Add_Click(object sender, EventArgs e)
        {
            Add(new TextParameter());
        }
    }
}
