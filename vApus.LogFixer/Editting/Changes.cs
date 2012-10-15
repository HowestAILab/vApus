/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Collections.Generic;

namespace vApus.LogFixer
{
    public enum Action
    {
        Added = 0,
        Removed
    }
    /// <summary>
    /// Use Track to track changes between the original and the new text.
    /// </summary>
    public class Changes
    {
        /// <summary>
        /// Use Track to track changes between the original and the new text.
        /// </summary>
        public Changes()
        { }
        /// <summary>
        /// Find the changes from the original to the new text. For this the TrackedChanges property is not used.
        /// </summary>
        /// <param name="originalText"></param>
        /// <param name="newText"></param>
        /// <returns></returns>
        public TrackedChanges Track (string originalText, string newText)
        {
            string match;
            string difference;
            string missing;

            DetermineMatchDifferenceAndMissing(originalText, newText, out match, out difference, out missing);
            return TranslateMatchDifferenceAndMissingToChanges(match, difference, missing, newText.Length);
        }

        #region DetermineMatchDifferenceAndMissing
        private void DetermineMatchDifferenceAndMissing(string originalText, string newText, out string match, out string difference, out string missing)
        {
            match = null;
            difference = null;
            missing = null;

            //Find the best match and start from there.
            int bestMatchIndex = 0, bestMatchLength;
            BestMatchOfOriginalInNew(originalText, newText, out bestMatchIndex, out bestMatchLength);

            //Add '\0' chars to match the starting indices of the best match with the new text.
            //Make the lengths the same
            string preptOriginalText, preptNewText;
            PreProcessOriginalAndNew(originalText, newText, bestMatchIndex, out preptOriginalText, out preptNewText);

            SequentialDetermineMDaM(preptOriginalText, preptNewText, out match, out difference, out missing);

            TrimLeadingAndTrailingNULChars(match, difference, missing, out match, out difference, out missing);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalText"></param>
        /// <param name="newText"></param>
        /// <param name="bestMatchIndex">Will be 0 even if there is no match.</param>
        /// <param name="bestMatchLength">Will be 0 if there is no match, not really usable.</param>
        private void BestMatchOfOriginalInNew(string originalText, string newText, out int bestMatchIndex, out int bestMatchLength)
        {
            bestMatchIndex = 0;
            bestMatchLength = 0;

            int originalLength = originalText.Length;
            int newLength = newText.Length;
            int matchingChars = 0;
            //i moves the position of the window in the new text
            for (int i = -originalLength + 1; i != newLength; i++)
            {
                string possibleMatch = string.Empty;
                int possibleMatchingChars = 0;

                int windowsModifier = 0;
                //Don't go out of bounds
                if (i < 0)
                    windowsModifier += (i * -1);

                //Add \0 chars if left out of bounds
                possibleMatch = new string('\0', windowsModifier);

                //j scans the window and k searches for matching chars of the original text in the new text
                for (int j = windowsModifier; j != originalLength; j++)
                {
                    int k = i + j;
                    //Don't go out of bounds
                    if (k == newLength)
                        break;

                    possibleMatch += newText[k];
                    if (newText[k] == originalText[j])
                        ++possibleMatchingChars;
                }

                if (possibleMatchingChars > matchingChars)
                {
                    bestMatchLength = possibleMatch.Length;
                    matchingChars = possibleMatchingChars;
                    bestMatchIndex = i;
                }

                //Break when no better match can be found.
                if (matchingChars >= newLength - i)
                    break;
            }
        }
        /// <summary>
        /// Add '\0' chars to match the starting indices of the best match with the new text.
        /// </summary>
        /// <param name="originalText"></param>
        /// <param name="newText"></param>
        /// <param name="bestMatchIndex"></param>
        /// <param name="preptOriginalText"></param>
        /// <param name="preptNewText"></param>
        private void PreProcessOriginalAndNew(string originalText, string newText, int bestMatchIndex, out string preptOriginalText, out string preptNewText)
        {
            preptOriginalText = originalText;
            preptNewText = newText;

            if (bestMatchIndex > 0)
                preptOriginalText = new string('\0', bestMatchIndex) + originalText;
            else if (bestMatchIndex < 0)
                preptNewText = new string('\0', (bestMatchIndex * -1)) + newText;

            if (preptOriginalText.Length < preptNewText.Length)
                preptOriginalText += new string('\0', preptNewText.Length - preptOriginalText.Length);
            else if (preptOriginalText.Length > newText.Length)
                preptNewText += new string('\0', preptOriginalText.Length - preptNewText.Length);
        }
        /// <summary>
        /// Sequential Determine Matches, Differences and Missing
        /// </summary>
        /// <param name="preptOriginalText"></param>
        /// <param name="preptNewText"></param>
        /// <param name="match"></param>
        /// <param name="difference"></param>
        /// <param name="missing"></param>
        private void SequentialDetermineMDaM(string preptOriginalText, string preptNewText, out string match, out string difference, out string missing)
        {
            match = difference = missing = null;

            //Determine to compare the original text to the new text and the other way around
            //Go for the biggest chunks in the most missing
            string s1 = preptOriginalText, s2 = preptNewText;
            string match1, difference1, missing1;

            DetermineMDAMLogic(preptOriginalText, preptNewText, s1, s2, out match1, out difference1, out missing1);


            s1 = preptNewText;
            s2 = preptOriginalText;

            string match2, difference2, missing2;
            DetermineMDAMLogic(preptOriginalText, preptNewText, s1, s2, out match2, out difference2, out missing2);

            string missingChunk1 = BiggestChunk('\0', missing1);

            //If the new text contains the chunk but it is in the difference it is a valid analyses, otherwise choose the other one.
            if (!preptNewText.Contains(missingChunk1) || difference1.Contains(missingChunk1))
            {
                match = match1;
                difference = difference1;
                missing = missing1;
            }
            else
            {
                match = match2;
                difference = difference2;
                missing = missing2;
            }
        }
        private void DetermineMDAMLogic(string preptOriginalText, string preptNewText, string s1, string s2, out string match, out string difference, out string missing)
        {
            match = missing = difference = string.Empty;

            //Found match index.
            int j = 0;
            //Start of the difference window.
            int startJ = 0;

            for (int i = 0; i != preptOriginalText.Length; i++)
            {
                //Start from the previous J to find the index (index) of a char in the new text sitting at the current index in the old text
                j = FindNext(s1[i], startJ, s2);

                if (j == -1)
                {
                    //Break if we are on the end of our search.
                    if (startJ >= preptNewText.Length && preptOriginalText[i] == '\0')
                        break;
                    //Match and difference combined must equal the new text, therefore '\0' chars are added to fill the holes.
                    match += '\0';
                    //Store the missing text
                    missing += preptOriginalText[i];

                    //Add the new text to the difference and don't go out of bounds
                    difference += (startJ < preptNewText.Length) ? preptNewText[startJ] : '\0';

                    ++startJ;
                }
                else
                {
                    //Add all the differences from the start j to the current.
                    for (int k = startJ; k != j; k++)
                    {
                        match += '\0';
                        difference += preptNewText[k];
                        missing += '\0';
                    }
                    //Add the found match.
                    match += preptNewText[j];
                    difference += '\0';
                    //To keep the missing parts apart.
                    missing += '\0';

                    startJ = j + 1;
                }
            }
        }
        private int FindNext(char c, int fromIndex, string inText)
        {
            if (c != '\0')
                if (fromIndex < inText.Length)
                    for (int i = fromIndex; i != inText.Length; i++)
                        if (inText[i] == c)
                            return i;

            return -1;
        }
        private string BiggestChunk(char delimiter, string inText)
        {
            string chunk = string.Empty;
            string[] arr = inText.Split('\0');
            foreach (string s in arr)
                if (s.Length > chunk.Length)
                    chunk = s;
            return chunk;
        }
        /// <summary>
        /// Trim leading and trailing '\0' but keeping the lenth the same
        /// </summary>
        /// <param name="match"></param>
        /// <param name="difference"></param>
        /// <param name="missing"></param>
        /// <param name="match2"></param>
        /// <param name="difference2"></param>
        /// <param name="missing2"></param>
        private void TrimLeadingAndTrailingNULChars(string match, string difference, string missing, out string match2, out string difference2, out string missing2)
        {
            //Any length will do, they are all the same
            int length = match.Length;
            int leading = 0;
            for (leading = 0; leading != length; leading++)
                if (match[leading] != '\0' || difference[leading] != '\0' || missing[leading] != '\0')
                    break;

            match2 = match.Substring(leading);
            difference2 = difference.Substring(leading);
            missing2 = missing.Substring(leading);

            length = match2.Length;
            int trailing = 0;
            for (trailing = length - 1; trailing != -1; trailing--)
                if (match2[trailing] != '\0' || difference2[trailing] != '\0' || missing2[trailing] != '\0')
                    break;

            trailing = length - trailing - 1;

            match2 = match2.Substring(0, length - trailing);
            difference2 = difference2.Substring(0, length - trailing);
            missing2 = missing2.Substring(0, length - trailing);
        }
        #endregion

        #region TranslateMatchDifferenceAndMissingToChanges
        /// <summary>
        /// 
        /// </summary>
        /// <param name="match"></param>
        /// <param name="difference"></param>
        /// <param name="missing"></param>
        /// <param name="newTextLength">The right boundary of the scan.</param>
        /// <returns></returns>
        private TrackedChanges TranslateMatchDifferenceAndMissingToChanges(string match, string difference, string missing, int newTextLength)
        {
            /* EXAMPLE
             * -----------------------------
             * ORIGINAL:    AF$GET$$$$FOO
             * NEW:         F$GET$B$C$D$GH
             * MATCH:        F$GET$ $ $ $  
             * DIFFERENCE:         B C D GH
             * MISSING:     A            FOO
             * 
             * Added     "B"     at  7
             * Added     "C"     at  9
             * Added     "D"     at  11
             * Removed   "FOO"   at  13
             * Added     "GH"    at  13
             * REMOVED   "A"     at  0 |--> Always at the end, so the indices stay correct.
             * ----------------------------- */

            var trackedChanges = new TrackedChanges();
            //When there is a remove without an add at the beginning of the text, keep this at the end of the tracked changes collection, so the indices will be correct.
            var keepAtEnd = new TrackedChanges();


            //All the lengths are the same.
            int i = 0;
            int length = difference.Length;
            while (i < length)
            {
                //Scan for stuff to remove at the current i.
                string remove = string.Empty;
                char missingChar = missing[i];
                TrackedChange removedTracked = new TrackedChange();

                //check if there are further differences, otherwise there are only missings.
                bool hasDifferences = difference.Substring(i).Trim('\0').Length != 0;

                if (missingChar == '\0')
                {
                    //Only missings, so continue
                    if (!hasDifferences)
                    {
                        ++i;
                        continue;
                    }
                }
                else
                {
                    while (missingChar != '\0')
                    {
                        remove += missingChar;
                        if (++i == missing.Length)
                            break;
                        missingChar = missing[i];
                    }

                    //Reset i to start scanning for a part to add at the original position.
                    i -= remove.Length;

                    removedTracked = new TrackedChange(Action.Removed, remove, i);
                    trackedChanges.Add(removedTracked);

                    //Only missing, make sure it continues from here
                    if (!hasDifferences)
                    {
                        i += remove.Length;
                        continue;
                    }
                }


                //Scan for stuff to add at the current i.
                string add = string.Empty;
                char differenceChar = difference[i];

                //Keep this to scan past the part to remove and to know where to add.
                int startI = i;

                if (differenceChar == '\0')
                {
                    if (remove.Length != 0)
                    {
                        trackedChanges.Remove(removedTracked);
                        keepAtEnd.Add(removedTracked);
                    }
                    ++i;
                }
                else
                {
                    //Scan for the whole length of what was removed.
                    while (differenceChar != '\0' || (remove.Length != 0 && i != startI + remove.Length))
                    {
                        if (differenceChar != '\0')
                            add += differenceChar;

                        //Skip ahead while scanning for a next diff char and don't go out of bounds.
                        if (++i == difference.Length)
                            break;
                        differenceChar = difference[i];
                    }
                    trackedChanges.Add(Action.Added, add, startI);
                }
            }
            trackedChanges.AddRange(keepAtEnd);
            return trackedChanges;
        }
        #endregion
    }
    public class TrackedChanges : List<TrackedChange>
    {
        public string OriginalText, NewText;
        public void Add(Action action, string text, int at)
        {
            Add(new TrackedChange(action, text, at));
        }
    }
    public struct TrackedChange
    {
        public Action Action;
        public string What;
        public int At;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="text"></param>
        /// <param name="at"></param>
        public TrackedChange(Action action, string text, int at)
        {
            Action = action;
            What = text;
            At = at;
        }

        public override string ToString()
        {
            return base.ToString() + ": \"" + What + "; Action: " + Action.ToString() + "; At: " + At;
        }
    }
}