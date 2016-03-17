/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace vApus.SolutionTree {
    /// <summary>
    ///     Properties with this attributes will be saved (GetXmlToSave) or cloned.
    ///     This will work for all primary datatypes and arrays/generic lists containing primary datatypes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SavableCloneableAttribute : Attribute {

        #region Properties
        public bool Encrypt { get; private set; }
        #endregion

        #region Constructors
        public SavableCloneableAttribute()
            : this(false) { }

        public SavableCloneableAttribute(bool encrypt) { Encrypt = encrypt; }
        #endregion
    }

    /// <summary>
    ///     To define that a common property control can be created for the property.
    ///     This will work for all primary datatypes and arrays/generic lists containing primary datatypes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyControlAttribute : Attribute {

        #region Fields
        private readonly int _displayIndex = -1, _allowedMinimum, _allowedMaximum;
        #endregion

        #region Properties
        public int DisplayIndex { get { return _displayIndex; } }
        public int AllowedMinimum { get { return _allowedMinimum; } }
        public int AllowedMaximum { get { return _allowedMaximum; } }
        public bool AdvancedProperty { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        ///     To define that a common property control can be created for the property.
        ///     This will work for all primary datatypes and arrays/generic lists containing primary datatypes.
        ///     Use ValueControlPanel.AddControlType(...) if you want a custom property control.
        /// </summary>
        /// <param name="allowedMinimum">Only for integer values.</param>
        /// <param name="allowedMaximum">Only for integer values.</param>
        public PropertyControlAttribute(int allowedMinimum = int.MinValue, int allowedMaximum = int.MaxValue) {
            _allowedMinimum = allowedMinimum;
            _allowedMaximum = allowedMaximum;
        }
        /// <summary>
        ///     To define that a common property control can be created for the property.
        ///     This will work for all primary datatypes and arrays/generic lists containing primary datatypes.
        ///     Use ValueControlPanel.AddControlType(...) if you want a custom property control.
        /// </summary>
        /// <param name="displayIndex">
        ///     A number greater than -1, it doesn't matter if it isn't directly following the display indices of other property control attributes.
        ///     If this is not spedified, alphabetical sorting will take place.
        /// </param>
        /// <param name="allowedMinimum">Only for integer values.</param>
        /// <param name="allowedMaximum">Only for integer values.</param>
        public PropertyControlAttribute(int displayIndex, int allowedMinimum, int allowedMaximum)
            : this(allowedMinimum, allowedMaximum) {
            _displayIndex = displayIndex;
        }
        /// <summary>
        ///     To define that a common property control can be created for the property.
        ///     This will work for all primary datatypes and arrays/generic lists containing primary datatypes.
        ///     Use ValueControlPanel.AddControlType(...) if you want a custom property control.
        /// </summary>
        /// <param name="displayIndex">
        ///     A number greater than -1, it doesn't matter if it isn't directly following the display indices of other property control attributes.
        ///     If this is not spedified, alphabetical sorting will take place.
        /// </param>
        public PropertyControlAttribute(int displayIndex)
            : this(displayIndex, int.MinValue, int.MaxValue) {
            _displayIndex = displayIndex;
        }
        /// <summary>
        ///     To define that a common property control can be created for the property.
        ///     This will work for all primary datatypes and arrays/generic lists containing primary datatypes.
        ///     Use ValueControlPanel.AddControlType(...) if you want a custom property control.
        /// </summary>
        /// <param name="displayIndex">
        ///     A number greater than -1, it doesn't matter if it isn't directly following the display indices of other property control attributes.
        ///     If this is not spedified, alphabetical sorting will take place.
        /// </param>
        /// <param name="advancedProperty">
        ///     An advanced property will be hidden. The user can choose to see it. This value is default false.
        /// </param>
        /// <param name="allowedMinimum">Only for integer values.</param>
        /// <param name="allowedMaximum">Only for integer values.</param>
        public PropertyControlAttribute(int displayIndex, bool advancedProperty, int allowedMinimum = int.MinValue, int allowedMaximum = int.MaxValue)
            : this(displayIndex, allowedMinimum, allowedMaximum) {
            AdvancedProperty = advancedProperty;
        }
        #endregion
    }

    /// <summary>
    ///     When this "class-attribute" is provided with a "SolutionComponent" a context menu is automatically created (usage: for the auto generated treenode), the class must contain methods(s) (void(object sender, EventArgs e) target) with the same name as the methodname(s) given with (case insensitive).
    ///     Standard methods for "SolutionComponent": "Clear_Click" and "SortItemsByLabel_Click", and one for "BaseItem": "Remove_Click".
    ///     It will try to add the same key as shortcut key as the provided hotkey, if any. Otherwise just the key desplay name is altered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ContextMenuAttribute : Attribute {

        #region Fields
        private readonly string[] _labels;
        private readonly string[] _methodNames;
        #endregion

        #region Constructors
        /// <summary>
        ///     When this "class-attribute" is provided with a "SolutionComponent" a context menu is automatically created (usage: for the auto generated treenode), the class must contain methods(s) (void(object sender, EventArgs e) target) with the same name as the methodname(s) given with (case insensitive).
        ///     Standard methods for "SolutionComponent": "Clear_Click" and "SortItemsByLabel_Click", and one for "BaseItem": "Remove_Click".
        ///     It will try to add the same key as shortcut key as the provided hotkey, if any. Otherwise just the key desplay name is altered.
        /// </summary>
        public ContextMenuAttribute(string[] methodNames, string[] labels) {
            if (methodNames == null || labels == null)
                throw new ArgumentNullException("methodNames || labels");
            if (methodNames.Length == 0 || labels.Length != methodNames.Length)
                throw new ArgumentOutOfRangeException(
                    "methodNames, labels (make sure the length of the labels array equals the length of the method names array)");
            foreach (string methodName in methodNames)
                if (methodName == null)
                    throw new ArgumentNullException("methodNames");
            foreach (string label in labels)
                if (label == null)
                    throw new ArgumentNullException("labels");

            _methodNames = methodNames;
            _labels = labels;
        }
        /// <summary>
        ///     When this "class-attribute" is provided with a "SolutionComponent" a context menu is automatically created (usage: for the auto generated treenode), the class must contain methods(s) (void(object sender, EventArgs e) target) with the same name as the methodname(s) given with (case insensitive).
        ///     Standard methods for "SolutionComponent": "Clear_Click" and "SortItemsByLabel_Click", and one for "BaseItem": "Remove_Click".
        ///     It will try to add the same key as shortcut key as the provided hotkey, if any. Otherwise just the key desplay name is altered.
        /// </summary>
        public ContextMenuAttribute(string[] methodNames)
            : this(methodNames, methodNames) { }
        /// <summary>
        ///     When this "class-attribute" is provided with a "SolutionComponent" a context menu is automatically created (usage: for the auto generated treenode), the class must contain methods(s) (void(object sender, EventArgs e) target) with the same name as the methodname(s) given with (case insensitive).
        ///     Standard methods for "SolutionComponent": "Clear_Click" and "SortItemsByLabel_Click", and one for "BaseItem": "Remove_Click".
        ///     It will try to add the same key as shortcut key as the provided hotkey, if any. Otherwise just the key desplay name is altered.
        /// </summary>
        public ContextMenuAttribute(string methodName, string label)
            : this(new[] { methodName }, new[] { label }) { }
        /// <summary>
        ///     When this "class-attribute" is provided with a "SolutionComponent" a context menu is automatically created (usage: for the auto generated treenode), the class must contain methods(s) (void(object sender, EventArgs e) target) with the same name as the methodname(s) given with (case insensitive).
        ///     Standard methods for "SolutionComponent": "Clear_Click" and "SortItemsByLabel_Click", and one for "BaseItem": "Remove_Click".
        ///     It will try to add the same key as shortcut key as the provided hotkey, if any. Otherwise just the key desplay name is altered.
        /// </summary>
        public ContextMenuAttribute(string methodName)
            : this(methodName, methodName) { }
        #endregion

        #region Functions
        /// <summary>
        ///     Builds and returns the contextmenu.
        /// </summary>
        /// <param name="solutionComponentType"></param>
        /// <returns></returns>
        internal ContextMenuStrip GetContextMenuStrip(SolutionComponent target) {
            try {
                //Adding shortcuts.
                Type keysType = typeof(Keys);
                HotkeysAttribute hotkeysAttribute = null;
                object[] attributes = target.GetType().GetCustomAttributes(typeof(HotkeysAttribute), true);
                if (attributes.Length > 0)
                    hotkeysAttribute = attributes[0] as HotkeysAttribute;

                var contextMenuStrip = new ContextMenuStrip();
                for (int i = 0; i < _methodNames.Length; i++) {
                    string methodName = _methodNames[i];

                    var item = new ToolStripMenuItem(_labels[i]);
                    MethodInfo info = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);

                    if (info == null)
                        throw new Exception(string.Format("Method '{0}' not found in {1} '{2}!", methodName, target.GetType().Name, target));

                    item.Click += (s, e) => {
                        if (!target.Locked || methodName == "Activate_Click") info.Invoke(target, new object[] { target, e });
                    };
                    if (hotkeysAttribute != null) {
                        Keys shortcutKey;
                        if (hotkeysAttribute.TryGetHotkey(methodName, out shortcutKey)) {
                            if (ToolStripManager.IsValidShortcut(shortcutKey))
                                item.ShortcutKeys = shortcutKey;
                            else
                                item.ShortcutKeyDisplayString = string.Format("<{0}>", Enum.GetName(keysType, shortcutKey));
                        }
                    }
                    contextMenuStrip.Items.Add(item);
                }
                return contextMenuStrip;
            }
            catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed building the context menu.", ex, new object[] { target });
            }
            return null;
        }
        #endregion
    }

    /// <summary>
    ///     When this "class-attribute" is provided with a "SolutionComponent" handling a hotkey is automated (usage: for the auto generated treenode), the class must contain methods(s) with the same name as the methodname(s) given with (case insensitive).
    ///     Parameters are completely ignored, so you can freely use the same methods as for the auto generated context menu. (Which is a bit the point)
    ///     Standard methods for "SolutionComponent": "Clear_Click" and "SortItemsByLabel_Click", and one for "BaseItem": "Remove_Click".
    ///     A word of caution: do not use the same keys as for the main menu for instance and do provide the same method name and hotkey only once.
    ///     Control and Shift are supported, Alt is not.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HotkeysAttribute : Attribute {

        #region Fields
        private readonly Keys[] _hotkeys;
        private readonly string[] _methodNames;
        #endregion

        #region Constructors
        /// <summary>
        ///     When this "class-attribute" is provided with a "SolutionComponent" handling a hotkey is automated (usage: for the auto generated treenode), the class must contain methods(s) with the same name as the methodname(s) given with (case insensitive).
        ///     Parameters are completely ignored, so you can freely use the same methods as for the auto generated context menu. (Which is a bit the point)
        ///     Standard methods for "SolutionComponent": "Clear_Click" and "SortItemsByLabel_Click", and one for "BaseItem": "Remove_Click".
        ///     A word of caution: do not use the same keys as for the main menu for instance and do provide the same method name and hotkey only once.
        ///     Control and Shift are supported, Alt is not.
        /// </summary>
        public HotkeysAttribute(string[] methodNames, Keys[] hotkeys) {
            if (methodNames == null || hotkeys == null)
                throw new ArgumentNullException("methodNames || hotkeys");
            if (methodNames.Length == 0 || hotkeys.Length != methodNames.Length)
                throw new ArgumentOutOfRangeException(
                    "methodNames, hotkeys (make sure the length of the hotkeys array equals the length of the method names array)");
            foreach (string methodName in methodNames)
                if (methodName == null)
                    throw new ArgumentNullException("methodNames");

            _methodNames = methodNames;
            _hotkeys = hotkeys;
        }
        /// <summary>
        ///     When this "class-attribute" is provided with a "SolutionComponent" handling a hotkey is automated (usage: for the auto generated treenode), the class must contain methods(s) with the same name as the methodname(s) given with (case insensitive).
        ///     Parameters are completely ignored, so you can freely use the same methods as for the auto generated context menu. (Which is a bit the point)
        ///     Standard methods for "SolutionComponent": "Clear_Click" and "SortItemsByLabel_Click", and one for "BaseItem": "Remove_Click".
        ///     A word of caution: do not use the same keys as for the main menu for instance and do provide the same method name and hotkey only once.
        ///     Control and Shift are supported, Alt is not.
        /// </summary>
        public HotkeysAttribute(string methodName, Keys hotkey)
            : this(new[] { methodName }, new[] { hotkey }) { }
        #endregion

        #region Functions
        /// <summary>
        ///     Tries getting the hotkey for a provided method name.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="hotkey"></param>
        /// <returns></returns>
        internal bool TryGetHotkey(string methodName, out Keys hotkey) {
            for (int i = 0; i < _methodNames.Length; i++)
                if (_methodNames[i] == methodName) {
                    hotkey = _hotkeys[i];
                    return true;
                }
            hotkey = Keys.None;
            return false;
        }
        /// <summary>
        ///     Handles the hotkey in a forgiving way, meaning that if the hotkey is not found nothing will happen, however if the method is not found an exception will be thrown.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="hotKey"></param>
        internal void HandleHotkey(SolutionComponent target, Keys hotKey) {
            int index = -1;
            for (int i = 0; i < _hotkeys.Length; i++)
                if (_hotkeys[i] == hotKey) {
                    index = i;
                    break;
                }
            if (index > -1) {
                string methodName = _methodNames[index];
                if (!target.Locked || methodName == "Activate_Click") {
                    MethodInfo info = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                    info.Invoke(target, new object[info.GetParameters().Length]);
                }
            }
        }
        #endregion
    }
}