using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vApus.SolutionTree;
using System.Windows.Forms;
using System.ComponentModel;

namespace vApus.Stresstest
{
    [ContextMenu(new string[] { "Import_Click", "Add_Click", "SortItemsByLabel_Click", "Clear_Click", "Paste_Click" }, new string[] { "Import Log Rule Set(s)", "Add Log Rule Set", "Sort", "Clear", "Paste" })]
    [Hotkeys(new string[] { "Paste_Click" }, new Keys[] { (Keys.Control | Keys.V) })]
    [DisplayName("Log Rule Sets"), Serializable]
    public class LogRuleSets : BaseRuleSets
    {
        public LogRuleSets()
        { }
        private void Add_Click(object sender, EventArgs e)
        {
            Add(new LogRuleSet());
        }
    }
}
