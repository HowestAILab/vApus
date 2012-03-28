/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vApus.Util
{
    public static class ClipboardWrapper
    {
        public static IDataObject GetDataObject()
        {
            // retry 2 times should be enough for read access
            try
            {
                return Clipboard.GetDataObject();
            }
            catch (ExternalException)
            {
                try
                {
                    return Clipboard.GetDataObject();
                }
                catch (ExternalException)
                {
                    return new DataObject();
                }
            }
        }
        public static void Clear()
        {
            Clipboard.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">Must be serializable.</param>
        public static void SetDataObject(object data)
        {
            try
            {
                Clipboard.SetDataObject(data, true, 10, 50);
            }
            catch (ExternalException)
            {
                Application.DoEvents();
                try
                {
                    Clipboard.SetDataObject(data, true, 10, 50);
                }
                catch (ExternalException) { }
            }
        }
    }
}
