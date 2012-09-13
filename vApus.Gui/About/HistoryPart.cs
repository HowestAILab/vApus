/*
 * Copyright 2008 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

namespace vApus.Gui
{
    public struct HistoryPart
    {
        public string Type;
        public int SelectionStart;
        public int Length;
        public HistoryPart(string type, int selectionStart, int length)
        {
            Type = type;
            SelectionStart = selectionStart;
            Length = length;
        }
    }
}
