//http://www.gotdotnet.ru/Forums/Common/130375.aspx

/*
 * Copyright 2008 (c) Vandroemme Dieter 
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Runtime.InteropServices;

namespace vApus.Util
{
    public class SpecialFolder
    {
        private SpecialFolder() { }
        /// <summary>
        /// Takes the SpecialFolder identifier of a folder and returns the pathname.
        /// </summary>
        /// <param name="folder">Identifier of a special folder</param>
        /// <returns>Full path of the special folder</returns>
        public static string GetPath(Folder folder)
        {
            string sPath = new String(' ', 255); // prepare buffer for result
            SHGetFolderPath(IntPtr.Zero, folder, IntPtr.Zero, 0, sPath);
            return sPath.Trim().Substring(0, sPath.Trim().Length - 1);
        }

        /// <summary>
        /// Takes the unique system-independent identifier for special folders and returns the pathname.
        /// </summary>
        /// <param name="hwnd">Handle to an owner window. This parameter is typically set to NULL. If it is not NULL, and a dial-up connection needs to be made to access the folder, a user interface (UI) prompt will appear in this window.</param>
        /// <param name="folder">A value that identifies the folder whose path is to be retrieved.</param>
        /// <param name="accessToken">An access token that can be used to represent a particular user. </param>
        /// <param name="flags">Flags to specify which path is to be returned. 
        /// Use 0 for current value for user, verify it exists. 
        /// Use 1 for default value, may not exist
        /// </param>
        /// <param name="path">Pointer to a null-terminated string of length MAX_PATH which will receive the path. If an error occurs or S_FALSE is returned, this string will be empty. </param>
        /// <returns>Returns standard HRESULT codes</returns>
        [System.Runtime.InteropServices.DllImport("SHFolder", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private extern static int SHGetFolderPath(IntPtr hwnd, Folder folder, IntPtr accessToken, int flags, [MarshalAs(UnmanagedType.LPTStr)] string path);

        /// <summary>
        /// Describes available special folders.
        /// </summary>
        public enum Folder : int
        {
            /// <summary>
            /// The file system directory that is used to store administrative tools for an individual user.
            /// </summary>
            AdminTools = 0x0030,
            /// <summary>
            /// The file system directory that corresponds to the user's nonlocalized Startup program group.
            /// </summary>
            AltStartup = 0x001d,
            /// <summary>
            /// File system directory that serves as a common repository for application-specific data.
            /// </summary>
            ApplicationData = 0x001a,
            /// <summary>
            /// The virtual folder containing the objects in the user's Recycle Bin.
            /// </summary>
            BitBucket = 0x000a,
            /// <summary>
            /// The file system directory acting as a staging area for files waiting to be written to CD. 
            /// </summary>
            CDBurnArea = 0x003b,
            /// <summary>
            /// The file system directory containing administrative tools for all users of the computer.
            /// </summary>
            CommonAdminTools = 0x002f,
            /// <summary>
            /// The file system directory that corresponds to the nonlocalized Startup program group for all users.
            /// </summary>
            CommonAltStartup = 0x001e,
            /// <summary>
            /// The file system directory containing application data for all users.
            /// </summary>
            CommonApplicationData = 0x0023,
            /// <summary>
            /// The file system directory that contains files and folders that appear on the desktop for all users.
            /// </summary>
            CommonDesktopFolder = 0x0019,
            /// <summary>
            /// The file system directory that contains documents that are common to all users. 
            /// </summary>
            CommonDocuments = 0x002e,
            /// <summary>
            /// The file system directory that serves as a common repository for favorite items common to all users. 
            /// </summary>
            CommonFavorites = 0x001f,
            /// <summary>
            /// The file system directory that serves as a repository for music files common to all users. 
            /// </summary>
            CommonMusic = 0x0035,
            /// <summary>
            /// The file system directory that contains the directories for the common program groups that appear on the Start menu for all users. 
            /// </summary>
            CommonPrograms = 0x0017,
            /// <summary>
            /// The file system directory that contains the programs and folders that appear on the Start menu for all users.
            /// </summary>
            CommonStartMenu = 0x0016,
            /// <summary>
            /// The file system directory that contains the programs that appear in the Startup folder for all users.
            /// </summary>
            CommonStartup = 0x0018,
            /// <summary>
            /// The file system directory that contains the templates that are available to all users. 
            /// </summary>
            CommonTemplates = 0x002d,
            /// <summary>
            /// The file system directory that serves as a repository for video files common to all users.
            /// </summary>
            CommonVideo = 0x0037,
            /// <summary>
            /// The virtual folder containing icons for the Control Panel applications.
            /// </summary>
            ControlPanelIcons = 0x0003,
            /// <summary>
            /// The file system directory that serves as a common repository for Internet cookies. 
            /// </summary>
            Cookies = 0x0021,
            /// <summary>
            /// The virtual folder representing the Windows desktop, the root of the namespace.
            /// </summary>
            Desktop = 0x0,
            /// <summary>
            /// The file system directory used to physically store file objects on the desktop (not to be confused with the desktop folder itself).
            /// </summary>
            DesktopFolder = 0x0010,
            /// <summary>
            /// The file system directory that serves as a common repository for the user's favorite items. 
            /// </summary>
            Favorites = 0x6,
            /// <summary>
            /// A virtual folder containing fonts.
            /// </summary>
            Fonts = 0x0014,
            /// <summary>
            /// The file system directory that serves as a common repository for Internet history items.
            /// </summary>
            History = 0x0022,
            /// <summary>
            /// A virtual folder representing the Internet.
            /// </summary>
            Internet = 0x0001,
            /// <summary>
            /// The file system directory that serves as a common repository for temporary Internet files. 
            /// </summary>
            InternetCache = 0x0020,
            /// <summary>
            /// The file system directory that serves as a data repository for local (nonroaming) applications.
            /// </summary>
            LocalApplicationData = 0x001c,
            /// <summary>
            /// The virtual folder representing My Computer, containing everything on the local computer: storage devices, printers, and Control Panel. The folder may also contain mapped network drives.
            /// </summary>
            MyComputer = 0x0011,
            /// <summary>
            /// The virtual folder representing the My Documents desktop item.
            /// </summary>
            MyDocuments = 0x5,
            /// <summary>
            /// The file system directory that serves as a common repository for music files.
            /// </summary>
            MyMusic = 0xd,
            /// <summary>
            /// The file system directory that serves as a common repository for image files. 
            /// </summary>
            MyPictures = 0x27,
            /// <summary>
            /// The file system directory that serves as a common repository for video files. 
            /// </summary>
            MyVideo = 0xe,
            /// <summary>
            /// A file system directory containing the link objects that may exist in the My Network Places virtual folder. It is not the same as Network, which represents the network namespace root. 
            /// </summary>
            Nethood = 0x0013,
            /// <summary>
            /// A virtual folder representing Network Neighborhood, the root of the network namespace hierarchy.
            /// </summary>
            Network = 0x0012,
            /// <summary>
            /// The virtual folder containing installed printers.
            /// </summary>
            Printers = 0x0004,
            /// <summary>
            /// The file system directory that contains the link objects that can exist in the Printers virtual folder.
            /// </summary>
            Printhood = 0x001b,
            /// <summary>
            /// The user's profile folder. Applications should not create files or folders at this level.
            /// </summary>
            Profile = 0x0028,
            /// <summary>
            /// The file system directory containing user profile folders. 
            /// </summary>
            Profiles = 0x0026,
            /// <summary>
            /// The Program Files folder.
            ProgramFiles = 0x26,
            /// <summary>
            /// A folder for components that are shared across applications.
            /// </summary>
            ProgramFilesCommon = 0x002b,
            /// <summary>
            /// <summary>
            /// The file system directory that contains the user's program groups (which are themselves file system directories).
            /// </summary>
            Programs = 0x0002,
            /// <summary>
            /// The file system directory that contains shortcuts to the user's most recently used documents. 
            /// To create a shortcut in this folder, use SHAddToRecentDocs. In addition to creating the shortcut, this function updates the Shell's list of recent documents and adds the shortcut to the My Recent Documents submenu of the Start menu.
            /// </summary>
            Recent = 0x8,
            /// <summary>
            /// The file system directory that contains Send To menu items. 
            /// </summary>
            SendTo = 0x0009,
            /// <summary>
            /// The file system directory that corresponds to the user's Startup program group. The system starts these programs whenever any user logs onto Windows NT or starts Windows 95
            /// </summary>
            StartMenu = 0x0007,
            /// <summary>
            /// The Windows System folder. 
            /// </summary>
            System = 0x25,
            /// <summary>
            /// The file system directory that serves as a common repository for document templates. 
            /// </summary>
            Templates = 0x0015,
            /// <summary>
            /// The Windows directory or SYSROOT. This corresponds to the %windir% or %SYSTEMROOT% environment variables. 
            /// </summary>
            Windows = 0x24,
        }
    }
}
