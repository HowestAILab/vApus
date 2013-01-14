/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace vApus.LogFixer
{
    public class Scenario : List<ScenarioItem>
    {
        public Scenario()
        {
        }

        public Scenario(TrackedChanges trackedChanges)
        {
            foreach (TrackedChange tc in trackedChanges)
                Add(tc);
        }

        public void Add(TrackedChange trackedChange)
        {
            Add(new ScenarioItem(trackedChange));
        }

        public static bool TryParse(string text, out Scenario scenario)
        {
            scenario = new Scenario();

            text = text.Replace('\r', '\n');
            string[] lines = text.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                ScenarioItem scenarioItem;
                if (!ScenarioItem.TryParse(line, out scenarioItem))
                    return false;

                scenario.Add(scenarioItem);
            }

            return true;
        }

        public string Apply(string textToChange)
        {
            foreach (ScenarioItem si in this)
                textToChange = si.Apply(textToChange);

            return textToChange;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (ScenarioItem si in this)
                sb.AppendLine(si.ToString());

            return sb.ToString().Trim();
        }
    }

    public class ScenarioItem
    {
        public enum Action
        {
            Add = 0,
            Remove
        }

        public enum Where
        {
            AtIndex = 0,
            AsOccurance
        }

        public int AtOrOccurance;

        public string What;
        public Action __Action;
        public Where __Where;

        public ScenarioItem()
        {
        }

        public ScenarioItem(TrackedChange trackedChange)
        {
            __Action = trackedChange.Action == LogFixer.Action.Added ? Action.Add : Action.Remove;
            What = "\"" + trackedChange.What + "\"";
            AtOrOccurance = trackedChange.At;
        }

        public static bool TryParse(string line, out ScenarioItem scenarioItem)
        {
            scenarioItem = new ScenarioItem();

            string[] words = line.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

            //The max word count of a valid line
            if (words.Length == 4)
            {
                if (!Enum.TryParse(words[0], out scenarioItem.__Action))
                    return false;

                if (words[1].Length < 2)
                    return false;

                if (words[1].StartsWith("\"") && words[1].EndsWith("\""))
                    scenarioItem.What = words[1];
                else if (words[1] == "char" && scenarioItem.__Action == Action.Remove)
                    scenarioItem.What = words[1];
                else
                    return false;

                if (!Enum.TryParse(words[2], out scenarioItem.__Where))
                    return false;

                //This is not concrete, thus not valid.
                if (scenarioItem.__Where == Where.AsOccurance && scenarioItem.What == "char")
                    return false;

                if (!int.TryParse(words[3], out scenarioItem.AtOrOccurance))
                    return false;

                return true;
            }
            return false;
        }

        public string Apply(string textToChange)
        {
            if (__Action == Action.Add)
            {
                int index = (__Where == Where.AtIndex)
                                ? AtOrOccurance
                                : FindOccuranceIndex(AtOrOccurance, textToChange);

                if (__Where == Where.AsOccurance && index != int.MaxValue)
                    ++index;
                if (index < 0)
                    index = 0;

                string what = What.Substring(1, What.Length - 2);

                if (index < textToChange.Length)
                    textToChange = textToChange.Insert(index, what);
                else
                    textToChange += what;
            }
            else
            {
                if (What == "char")
                {
                    int index = (__Where == Where.AtIndex)
                                    ? AtOrOccurance
                                    : FindOccuranceIndex(AtOrOccurance, textToChange);
                    if (index < 0)
                        index = 0;
                    else if (index >= textToChange.Length)
                        index = textToChange.Length - 1;

                    textToChange = textToChange.Remove(index);
                }
                else
                {
                    string what = What.Substring(1, What.Length - 2);

                    int index = int.MaxValue;

                    index = (__Where == Where.AtIndex)
                                ? ClosestMatchWhat(textToChange)
                                : index = FindOccuranceIndex(AtOrOccurance, textToChange);

                    if (index < textToChange.Length)
                        textToChange = textToChange.Remove(index, what.Length);
                }
            }

            return textToChange;
        }

        /// <summary>
        ///     Find the index of What in the text to change of the given occurance.
        /// </summary>
        /// <param name="occurance"></param>
        /// <returns>If this equals int.MaxValue nothing was found.</returns>
        private int FindOccuranceIndex(int occurance, string textToChange)
        {
            if (occurance < 0)
                return -1;

            string what = What.Substring(1, What.Length - 2);

            int j = 0;
            for (int i = 0; i != textToChange.Length; i++)
            {
                if (i + what.Length > textToChange.Length)
                    break;

                if (textToChange.Substring(i, what.Length) == what &&
                    j++ == occurance)
                    return i;
            }
            return int.MaxValue;
        }

        private int ClosestMatchWhat(string textToChange)
        {
            int down = ClosestMatchWhatDown(textToChange);
            int up = ClosestMatchWhatUp(textToChange);

            if (AtOrOccurance - down < up - AtOrOccurance)
                return down;
            return up;
        }

        private int ClosestMatchWhatDown(string textToChange)
        {
            string what = What.Substring(1, What.Length - 2);

            for (int i = AtOrOccurance; i != -1; i--)
            {
                if (i + what.Length > textToChange.Length)
                    continue;

                if (textToChange.Substring(i, what.Length) == what)
                    return i;
            }

            return int.MaxValue;
        }

        private int ClosestMatchWhatUp(string textToChange)
        {
            string what = What.Substring(1, What.Length - 2);

            for (int i = AtOrOccurance; i != textToChange.Length; i++)
            {
                if (i + what.Length > textToChange.Length)
                    break;

                if (textToChange.Substring(i, what.Length) == what)
                    return i;
            }

            return int.MaxValue;
        }

        public override string ToString()
        {
            return __Action + " " + What + " " + __Where + " " + AtOrOccurance;
        }
    }
}