/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vApus.Util {
    public static class ClipboardWrapper {
        public static IDataObject GetDataObject() {
            // retry 2 times should be enough for read access
            try {
                return Clipboard.GetDataObject();
            } catch (ExternalException) {
                try {
                    return Clipboard.GetDataObject();
                } catch (ExternalException) {
                    return new DataObject();
                }
            }
        }

        public static void Clear() { Clipboard.Clear(); }

        /// <summary>
        /// </summary>
        /// <param name="data">Must be serializable.</param>
        public static void SetDataObject(object data) {
            try {
                Clipboard.SetDataObject(data, true, 10, 50);
            } catch (ExternalException) {
                Application.DoEvents();
                try {
                    Clipboard.SetDataObject(data, true, 10, 50);
                } catch (ExternalException) {
                }
            }
        }
    }
}