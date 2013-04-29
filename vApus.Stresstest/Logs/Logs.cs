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
    [ContextMenu(new[] { "Add_Click", "Import_Click", "SortItemsByLabel_Click", "Clear_Click", "Paste_Click" },
        new[] { "Add Log", "Import Log Data Structure(s)", "Sort", "Clear", "Paste" })]
    [Hotkeys(new[] { "Add_Click", "Paste_Click" }, new[] { Keys.Insert, (Keys.Control | Keys.V) })]
    public class Logs : BaseItem {
        public Logs() {
            AddAsDefaultItem(new LogRuleSets());

            //To synchronize the tokens with the changed tokens in all log entries.
            SolutionComponentChanged += Parameters_SolutionComponentChanged;
        }

        private void Parameters_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            //Added while loading, so check if it has a parent
            if (Solution.ActiveSolution != null && Parent != null &&
                (sender is BaseParameter || sender is CustomListParameters ||
                 sender is CustomRandomParameters || sender is NumericParameters || sender is TextParameters)) {
                var parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
                if (parameters != null && parameters.Contains(sender as BaseItem)) {
                    Dictionary<BaseParameter, KeyValuePair<int, int>> oldAndNewIndices;
                    parameters.SynchronizeTokenNumericIdentifierToIndices(out oldAndNewIndices);
                    //We must loop over the collection in reverse order if a parameter was removed, otherwise replacing the parameter tokens will go wrong.
                    if (e.__DoneAction == SolutionComponentChangedEventArgs.DoneAction.Removed) {
                        var keys = new List<BaseParameter>(oldAndNewIndices.Count);
                        foreach (BaseParameter key in oldAndNewIndices.Keys)
                            keys.Add(key);

                        keys.Reverse();

                        var reversed = new Dictionary<BaseParameter, KeyValuePair<int, int>>(oldAndNewIndices.Count);
                        foreach (BaseParameter key in keys)
                            reversed.Add(key, oldAndNewIndices[key]);

                        oldAndNewIndices = reversed;
                    }

                    foreach (BaseItem item in this)
                        if (item is Log) {
                            var log = item as Log;
                            log.ApplyLogRuleSet();
                            string beginTokenDelimiter = null, endTokenDelimiter = null;
                            bool logEntryContainsTokens;
                            log.GetParameterTokenDelimiters(out beginTokenDelimiter, out endTokenDelimiter, out logEntryContainsTokens, false);

                            log.SynchronizeTokens(oldAndNewIndices, new KeyValuePair<string, string>(beginTokenDelimiter, beginTokenDelimiter), new KeyValuePair<string, string>(endTokenDelimiter, endTokenDelimiter));
                        }
                }
            }
        }

        private void Add_Click(object sender, EventArgs e) {
            Add(new Log());
        }

        public override void Clear() {
            var itemsCopy = new List<BaseItem>();
            foreach (BaseItem item in this)
                if (!(item is Log))
                    itemsCopy.Add(item);
            base.Clear();
            AddRange(itemsCopy);
        }
    }
}