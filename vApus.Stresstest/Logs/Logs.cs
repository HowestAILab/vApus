/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    /// <summary>
    /// Contains logs and LogRuleSets.
    /// </summary>
    [ContextMenu(new[] { "Add_Click", "Import_Click", "ImportLogAndUsedParameters_Click", "SortItemsByLabel_Click", "Clear_Click", "Paste_Click" },
        new[] { "Add Log", "Import Log Data Structure(s)", "Import Log and Used Parameter Data Structures", "Sort", "Clear", "Paste" })]
    [Hotkeys(new[] { "Add_Click", "Paste_Click" }, new[] { Keys.Insert, (Keys.Control | Keys.V) })]
    public class Logs : BaseItem {

        #region Fields
        //Do not synchronize the tokens of a log that was imported together with parameter data structures. (ImportLogAndUsedParameters_Click)
        //The value of a KVP is a parameters collection.
        private List<KeyValuePair<Log, object>> _excludeFromSynchronizeTokens = new List<KeyValuePair<Log, object>>();
        #endregion

        #region Constructor
        public Logs() {
            AddAsDefaultItem(new LogRuleSets());

            //To synchronize the tokens with the changed tokens in all log entries.
            SolutionComponentChanged += Parameters_SolutionComponentChanged;
        }
        #endregion

        #region Functions
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

                            var excludeKvp = new KeyValuePair<Log, object>(null, null);
                            foreach (var kvp in _excludeFromSynchronizeTokens)
                                if (kvp.Key == log && kvp.Value == sender) {
                                    excludeKvp = kvp;
                                    break;
                                }

                            if (excludeKvp.Key == null) {
                                string beginTokenDelimiter = null, endTokenDelimiter = null;
                                bool logEntryContainsTokens;
                                log.GetParameterTokenDelimiters(out beginTokenDelimiter, out endTokenDelimiter, out logEntryContainsTokens, false);

                                log.SynchronizeTokens(oldAndNewIndices, new KeyValuePair<string, string>(beginTokenDelimiter, beginTokenDelimiter), new KeyValuePair<string, string>(endTokenDelimiter, endTokenDelimiter));

                            } else if (sender is CustomListParameters || sender is CustomRandomParameters || sender is NumericParameters || sender is TextParameters) {
                                _excludeFromSynchronizeTokens.Remove(excludeKvp);
                            }
                        }
                }
            }
        }

        private void Add_Click(object sender, EventArgs e) {
            Add(new Log());
        }

        private void ImportLogAndUsedParameters_Click(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Zip Files (*.zip) | *.zip";
            ofd.Title = "Import log and used parameters from... [Parameter indices will be editted automatically; Expensive operation]";

            if (ofd.ShowDialog() == DialogResult.OK) {
                var parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
                var customListParameters = parameters[0] as CustomListParameters;
                var numericParameters = parameters[1] as NumericParameters;
                var textParameters = parameters[2] as TextParameters;
                var customRandomParameters = parameters[3] as CustomRandomParameters;

                foreach (string fileName in ofd.FileNames) {
                    //First get all the parts, it is important that things happen in order.
                    PackagePart logPart = null;
                    var customListParameterParts = new List<PackagePart>();
                    var numericParameterParts = new List<PackagePart>();
                    var textParameterParts = new List<PackagePart>();
                    var customRandomParameterParts = new List<PackagePart>();

                    var package = Package.Open(fileName, FileMode.Open, FileAccess.Read);
                    foreach (var part in package.GetParts()) {
                        string name = part.Uri.ToString();
                        if (name.StartsWith("/Custom_List_Parameter_")) customListParameterParts.Add(part);
                        else if (name.StartsWith("/Numeric_Parameter_")) numericParameterParts.Add(part);
                        else if (name.StartsWith("/Text_Parameter_")) textParameterParts.Add(part);
                        else if (name.StartsWith("/Custom_Random_Parameter_")) customRandomParameterParts.Add(part);
                        else if (name == "/Log") logPart = part;
                    }


                    //Import all the parameters in order and put a map here so indices can be changed correctly in the added log.
                    var indexMapLeft = new List<int>();
                    var indexMapRight = new List<int>();

                    if (customListParameterParts.Count != 0) {
                        foreach (var part in customListParameterParts) {
                            int index = int.Parse(part.Uri.ToString().Substring(("/Custom_List_Parameter_").Length));
                            customListParameters.Import(false, part.GetStream());
                            indexMapLeft.Add(index);
                            indexMapRight.Add(customListParameters.Count);
                        }
                        customListParameters.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, customListParameterParts.Count == 1);
                    }

                    if (numericParameterParts.Count != 0) {
                        foreach (var part in numericParameterParts) {
                            int index = int.Parse(part.Uri.ToString().Substring(("/Numeric_Parameter_").Length));
                            numericParameters.Import(false, part.GetStream());
                            indexMapLeft.Add(customListParameterParts.Count + index);
                            indexMapRight.Add(customListParameters.Count + numericParameters.Count);
                        }
                        numericParameters.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, numericParameterParts.Count == 1);
                    }

                    if (textParameterParts.Count != 0) {
                        foreach (var part in textParameterParts) {
                            int index = int.Parse(part.Uri.ToString().Substring(("/Text_Parameter_").Length));
                            textParameters.Import(false, part.GetStream());
                            indexMapLeft.Add(customListParameterParts.Count + numericParameterParts.Count + index);
                            indexMapRight.Add(customListParameters.Count + numericParameters.Count + textParameters.Count);
                        }
                        textParameters.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, textParameterParts.Count == 1);
                    }

                    if (customRandomParameterParts.Count != 0) {
                        foreach (var part in customRandomParameterParts) {
                            int index = int.Parse(part.Uri.ToString().Substring(("/Custom_Random_Parameter_").Length));
                            customRandomParameters.Import(false, part.GetStream());
                            indexMapLeft.Add(customListParameterParts.Count + numericParameterParts.Count + textParameterParts.Count + index);
                            indexMapRight.Add(customListParameters.Count + numericParameters.Count + textParameters.Count + customRandomParameters.Count);
                        }
                        customRandomParameters.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, customRandomParameterParts.Count == 1);
                    }

                    //Finally import the log, complete the map and replace the parameter indices in the log entries.
                    Import(false, logPart.GetStream());

                    try { package.Close(); } catch { }

                    var log = this[Count - 1] as Log;

                    //Exclude this for the triggered delayed solution component changed events. We do not want the indices to be synchronized for this log, it will screw everything.
                    if (customListParameterParts.Count != 0)
                        _excludeFromSynchronizeTokens.Add(new KeyValuePair<Log, object>(log, customListParameters));
                    if (numericParameterParts.Count != 0)
                        _excludeFromSynchronizeTokens.Add(new KeyValuePair<Log, object>(log, numericParameters));
                    if (textParameterParts.Count != 0)
                        _excludeFromSynchronizeTokens.Add(new KeyValuePair<Log, object>(log, textParameters));
                    if (customRandomParameterParts.Count != 0)
                        _excludeFromSynchronizeTokens.Add(new KeyValuePair<Log, object>(log, customRandomParameters));


                    string begin, end;
                    bool logEntryContainsTokens;
                    log.GetParameterTokenDelimiters(out begin, out end, out logEntryContainsTokens, false);

                    var indexMapLeft1 = new string[indexMapLeft.Count];
                    var indexMapRight1 = new string[indexMapLeft.Count];
                    var indexMapLeft2 = new string[indexMapLeft.Count];
                    var indexMapRight2 = new string[indexMapLeft.Count];

                    for (int i = 0; i != indexMapLeft.Count; i++) {
                        indexMapLeft1[i] = begin + indexMapLeft[i] + end;
                        indexMapRight1[i] = begin + indexMapRight[i] + end;

                        indexMapLeft2[i] = "." + indexMapLeft[i] + end;
                        indexMapRight2[i] = "." + indexMapRight[i] + end;
                    }


                    foreach (UserAction userAction in log)
                        Parallel.ForEach(userAction as ICollection<BaseItem>, (x) => {
                            var logEntry = x as LogEntry;
                            for (int i = indexMapLeft1.Length - 1; i != -1; i--) {
                                List<int> rows, columns, matchLengths;
                                FindAndReplace.Find(indexMapLeft1[i], logEntry.LogEntryString, out rows, out columns, out matchLengths, false, true);
                                if (matchLengths.Count != 0)
                                    logEntry.LogEntryString = vApus.Util.FindAndReplace.Replace(rows, columns, matchLengths, logEntry.LogEntryString, indexMapRight1[i]);

                                FindAndReplace.Find(indexMapLeft2[i], logEntry.LogEntryString, out rows, out columns, out matchLengths, false, true);
                                if (matchLengths.Count != 0)
                                    logEntry.LogEntryString = vApus.Util.FindAndReplace.Replace(rows, columns, matchLengths, logEntry.LogEntryString, indexMapRight2[i]);
                            }
                        });


                    InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, false);
                }
            }
        }

        public override void Clear() {
            var itemsCopy = new List<BaseItem>();
            foreach (BaseItem item in this)
                if (!(item is Log))
                    itemsCopy.Add(item);
            base.Clear();
            AddRange(itemsCopy);
        }
        #endregion
    }
}