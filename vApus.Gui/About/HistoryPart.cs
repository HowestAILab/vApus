/*
 * Copyright 2008 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

namespace vApus.Gui {
    public struct HistoryPart {
        public int Length;
        public int SelectionStart;
        public string Type;

        public HistoryPart(string type, int selectionStart, int length) {
            Type = type;
            SelectionStart = selectionStart;
            Length = length;
        }
    }
}