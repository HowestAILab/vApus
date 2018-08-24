/*
 * 2008 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
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