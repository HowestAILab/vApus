/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.IO;

namespace vApus.Util {
    public static class ExportingResultsManager {
        public static bool Enabled {
            get { return Properties.Settings.Default.EREnabled; }
            internal set { Properties.Settings.Default.EREnabled = value;
            Properties.Settings.Default.Save();
            }
        }
        public static string Folder {
            get {
                string folder = Properties.Settings.Default.ERFolder;
                if (!Directory.Exists(folder))
                    folder = SpecialFolder.GetPath(SpecialFolder.Folder.Desktop);

                return folder;
            }
            internal set {
                Properties.Settings.Default.ERFolder = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
