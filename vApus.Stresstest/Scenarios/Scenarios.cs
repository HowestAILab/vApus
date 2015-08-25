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

namespace vApus.StressTest {
    /// <summary>
    /// Contains scenarios and ScenarioRuleSets.
    /// </summary>
    [ContextMenu(new[] { "Add_Click", "Import_Click", "ImportScenarioAndUsedParameters_Click", "SortItemsByLabel_Click", "Clear_Click", "Paste_Click" },
        new[] { "Add scenario", "Import scenario data structure(s)", "Import scenario and used parameter data structures", "Sort", "Clear", "Paste" })]
    [Hotkeys(new[] { "Add_Click", "Paste_Click" }, new[] { Keys.Insert, (Keys.Control | Keys.V) })]
    public class Scenarios : BaseItem {

        #region Fields
        //Do not synchronize the tokens of a scenario that was imported together with parameter data structures. (ImportScenarioAndUsedParameters_Click)
        //The value of a KVP is a parameters collection.
        private List<KeyValuePair<Scenario, object>> _excludeFromSynchronizeTokens = new List<KeyValuePair<Scenario, object>>();

        private const string VBLRn = "<16 0C 02 12n>";
        private const string VBLRr = "<16 0C 02 12r>";
        #endregion

        #region Constructor
        public Scenarios() {
            AddAsDefaultItem(new ScenarioRuleSets());

            //To synchronize the tokens with the changed tokens in all requests.
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
                        if (item is Scenario) {
                            var scenario = item as Scenario;
                            scenario.ApplyScenarioRuleSet();

                            var excludeKvp = new KeyValuePair<Scenario, object>(null, null);
                            foreach (var kvp in _excludeFromSynchronizeTokens)
                                if (kvp.Key == scenario && kvp.Value == sender) {
                                    excludeKvp = kvp;
                                    break;
                                }

                            if (excludeKvp.Key == null) {
                                string beginTokenDelimiter = null, endTokenDelimiter = null;
                                bool requestContainsTokens;
                                scenario.GetParameterTokenDelimiters(out beginTokenDelimiter, out endTokenDelimiter, out requestContainsTokens, false);

                                scenario.SynchronizeTokens(oldAndNewIndices, new KeyValuePair<string, string>(beginTokenDelimiter, beginTokenDelimiter), new KeyValuePair<string, string>(endTokenDelimiter, endTokenDelimiter));

                            } else if (sender is CustomListParameters || sender is CustomRandomParameters || sender is NumericParameters || sender is TextParameters) {
                                _excludeFromSynchronizeTokens.Remove(excludeKvp);
                            }
                        }
                }
            }
        }

        private void Add_Click(object sender, EventArgs e) {
            Add(new Scenario());
        }

        private void ImportScenarioAndUsedParameters_Click(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Zip Files (*.zip) | *.zip";
            ofd.Title = "Import scenario and used parameters from... [Parameter indices will be editted automatically; Expensive operation]";

            if (ofd.ShowDialog() == DialogResult.OK) {
                var parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
                var customListParameters = parameters[0] as CustomListParameters;
                var numericParameters = parameters[1] as NumericParameters;
                var textParameters = parameters[2] as TextParameters;
                var customRandomParameters = parameters[3] as CustomRandomParameters;

                foreach (string fileName in ofd.FileNames) {
                    //First get all the parts, it is important that things happen in order.
                    PackagePart scenarioPart = null;
                    var customListParameterParts = new List<PackagePart>();
                    var numericParameterParts = new List<PackagePart>();
                    var textParameterParts = new List<PackagePart>();
                    var customRandomParameterParts = new List<PackagePart>();

                    var package = Package.Open(fileName, FileMode.Open, FileAccess.Read);
                    foreach (var part in package.GetParts()) {
                        string name = part.Uri.ToString();
                        if (name.StartsWith("/Custom_list_parameter_")) customListParameterParts.Add(part);
                        else if (name.StartsWith("/Numeric_parameter_")) numericParameterParts.Add(part);
                        else if (name.StartsWith("/Text_parameter_")) textParameterParts.Add(part);
                        else if (name.StartsWith("/Custom_random_parameter_")) customRandomParameterParts.Add(part);
                        else if (name == "/Scenario") scenarioPart = part;
                    }


                    //Import all the parameters in order and put a map here so indices can be changed correctly in the added scenario.
                    var indexMapLeft = new List<int>();
                    var indexMapRight = new List<int>();

                    if (customListParameterParts.Count != 0) {
                        foreach (var part in customListParameterParts) {
                            int index = int.Parse(part.Uri.ToString().Substring(("/Custom_list_parameter_").Length));
                            customListParameters.Import(false, part.GetStream());
                            indexMapLeft.Add(index);
                            indexMapRight.Add(customListParameters.Count);
                        }
                        customListParameters.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, customListParameterParts.Count == 1);
                    }

                    if (numericParameterParts.Count != 0) {
                        foreach (var part in numericParameterParts) {
                            int index = int.Parse(part.Uri.ToString().Substring(("/Numeric_parameter_").Length));
                            numericParameters.Import(false, part.GetStream());
                            indexMapLeft.Add(customListParameterParts.Count + index);
                            indexMapRight.Add(customListParameters.Count + numericParameters.Count);
                        }
                        numericParameters.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, numericParameterParts.Count == 1);
                    }

                    if (textParameterParts.Count != 0) {
                        foreach (var part in textParameterParts) {
                            int index = int.Parse(part.Uri.ToString().Substring(("/Text_parameter_").Length));
                            textParameters.Import(false, part.GetStream());
                            indexMapLeft.Add(customListParameterParts.Count + numericParameterParts.Count + index);
                            indexMapRight.Add(customListParameters.Count + numericParameters.Count + textParameters.Count);
                        }
                        textParameters.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, textParameterParts.Count == 1);
                    }

                    if (customRandomParameterParts.Count != 0) {
                        foreach (var part in customRandomParameterParts) {
                            int index = int.Parse(part.Uri.ToString().Substring(("/Custom_random_parameter_").Length));
                            customRandomParameters.Import(false, part.GetStream());
                            indexMapLeft.Add(customListParameterParts.Count + numericParameterParts.Count + textParameterParts.Count + index);
                            indexMapRight.Add(customListParameters.Count + numericParameters.Count + textParameters.Count + customRandomParameters.Count);
                        }
                        customRandomParameters.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, customRandomParameterParts.Count == 1);
                    }

                    //Finally import the scenario, complete the map and replace the parameter indices in the requests.
                    Import(false, scenarioPart.GetStream());

                    try {
                        package.Close();
                    } catch {
                        //Ignore. Not important.
                    }

                    var scenario = this[Count - 1] as Scenario;

                    //Exclude this for the triggered delayed solution component changed events. We do not want the indices to be synchronized for this scenario, it will screw everything.
                    if (customListParameterParts.Count != 0)
                        _excludeFromSynchronizeTokens.Add(new KeyValuePair<Scenario, object>(scenario, customListParameters));
                    if (numericParameterParts.Count != 0)
                        _excludeFromSynchronizeTokens.Add(new KeyValuePair<Scenario, object>(scenario, numericParameters));
                    if (textParameterParts.Count != 0)
                        _excludeFromSynchronizeTokens.Add(new KeyValuePair<Scenario, object>(scenario, textParameters));
                    if (customRandomParameterParts.Count != 0)
                        _excludeFromSynchronizeTokens.Add(new KeyValuePair<Scenario, object>(scenario, customRandomParameters));


                    string begin, end;
                    bool requestContainsTokens;
                    scenario.GetParameterTokenDelimiters(out begin, out end, out requestContainsTokens, false);

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


                    foreach (UserAction userAction in scenario)
                        Parallel.ForEach(userAction as ICollection<BaseItem>, (x) => {
                            var request = x as Request;
                            for (int i = indexMapLeft1.Length - 1; i != -1; i--) {
                                List<int> rows, columns, matchLengths;
                                string preppedRequestString = request.RequestString.Replace("\n", VBLRn).Replace("\r", VBLRr);
                                FindAndReplace.Find(indexMapLeft1[i], preppedRequestString, out rows, out columns, out matchLengths, false, true);
                                if (matchLengths.Count != 0)
                                    preppedRequestString = vApus.Util.FindAndReplace.Replace(rows, columns, matchLengths, preppedRequestString, indexMapRight1[i]);

                                FindAndReplace.Find(indexMapLeft2[i], preppedRequestString, out rows, out columns, out matchLengths, false, true);
                                if (matchLengths.Count != 0)
                                    preppedRequestString = vApus.Util.FindAndReplace.Replace(rows, columns, matchLengths, preppedRequestString, indexMapRight2[i]);

                                request.RequestString = preppedRequestString.Replace(VBLRn, "\n").Replace(VBLRr, "\r");
                            }
                        });


                    InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, false);
                }
            }
        }

        public override void Clear() {
            var itemsCopy = new List<BaseItem>();
            foreach (BaseItem item in this)
                if (!(item is Scenario))
                    itemsCopy.Add(item);
            base.Clear();
            AddRange(itemsCopy);
        }
        #endregion
    }
}