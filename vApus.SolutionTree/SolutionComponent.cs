﻿/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using RandomUtils.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.SolutionTree {
    /// <summary>
    ///     The base class for BaseItem and BaseProject. The base of everything in a solution. Contains all the plumbing for ICollection amongst others.
    /// </summary>
    [Serializable]
    public abstract class SolutionComponent : Object, ICollection<BaseItem>, IDisposable {
        [field: NonSerialized]
        public static event EventHandler<SolutionComponentChangedEventArgs> SolutionComponentChanged;

        private delegate void InvokeSolutionComponentChangedEventDelegate(SolutionComponentChangedEventArgs.DoneAction doneAction, object arg);

        /// <summary>
        /// <para>LockedChanged will be invoked when Locked is set or reset.</para>
        /// <para>You can suscribe to this event to lock or unlock the views for your items.</para>
        /// <para>Context menu's and shortcut key locking is already handled for you.</para>
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<LockedChangedEventArgs> LockedChanged;

        #region Fields
        private bool _isDefaultItem, _isEmpty;
        protected List<BaseItem> _items = new List<BaseItem>();
        private List<Type> _defaultItemTypes = new List<Type>();
        private bool _noImage, _showInGui = true;
        private bool _locked;

        [NonSerialized] //Nasty bug, this class (inheritance) would not serialize sometimes
        private InvokeSolutionComponentChangedEventDelegate _invokeSolutionComponentChangedEventDelegate;
        #endregion

        #region Properties
        [Description("The name of this item.")]
        public string Name {
            get {
                if (_isEmpty) {
                    return null;
                }
                else {
                    object[] attributes = GetType().GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    return attributes.Length != 0 ? (attributes[0] as DisplayNameAttribute).DisplayName : GetType().Name;
                }
            }
        }

        /// <summary>
        ///     The count of the child items (where the parent of those items is null or not this).
        /// </summary>
        [Description("The count of the child items (where the parent of those items is null or not this).")]
        public int CountOfParentLess {
            get {
                int count = 0;
                foreach (object item in _items)
                    if (item.GetParent() != this)
                        ++count;

                return count;
            }
        }

        public BaseItem this[int index] { get { return _items[index]; } }

        /// <summary>
        /// Show this in the tree view or not.
        /// </summary>
        [SavableCloneable]
        public bool ShowInGui { get { return _showInGui; } set { _showInGui = value; } }

        [SavableCloneable]
        public bool IsDefaultItem { get { return _isDefaultItem; } set { _isDefaultItem = value; } }

        /// <summary>
        /// Can never be null (for load and save checks), but can be empty.
        /// </summary>
        [SavableCloneable]
        public bool IsEmpty { get { return _isEmpty; } set { _isEmpty = value; } }

        /// <summary>
        ///     The count of the child items (regardless the parent of those items).
        /// </summary>
        [Description("The count of the child items (regardless the parent of those items).")]
        public int Count { get { return _items.Count; } }

        /// <summary>
        /// Is overriden in base item / project.
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        ///     The count of the child items that are of a derived type from BaseItem (where the parent is this).
        /// </summary>
        /// <param name="derivedType"></param>
        /// <returns></returns>
        [Description("The count of the child items that are of a derived type from BaseItem (where the parent is this).")]
        public int CountOf(Type derivedType) {
            int count = 0;
            foreach (object item in _items)
                if (item.GetType() == derivedType && item.GetParent() == this)
                    ++count;

            return count;
        }

        /// <summary>
        /// <para>When true, context menu enties and shortcut keys will be limited to activate for this and all sub items.</para>
        /// <para>LockedChanged will be invoked, you can suscribe to this event to lock or unlock the views for your items.</para>
        /// <para>You are responsible to set and reset this property.</para>
        /// </summary>
        public bool Locked {
            get { return _locked; }
            set {
                foreach (var item in this) item.Locked = value;

                if (_locked != value) {
                    _locked = value;
                    if (LockedChanged != null) LockedChanged(this, new LockedChangedEventArgs(_locked));
                }
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        ///     The base class for BaseItem and BaseProject. The base of everything in a solution. Contains all the plumbing for ICollection amongst others.
        /// </summary>
        public SolutionComponent() {
            _invokeSolutionComponentChangedEventDelegate = InvokeSolutionComponentChangedEventCallback;
        }

        // ~SolutionComponent() {

        //Parallel.ForEach(SolutionComponentChanged.GetInvocationList(), (handler) => {
        //    SolutionComponentChanged -= handler as EventHandler<SolutionComponentChangedEventArgs>;
        //});
        //}
        #endregion

        #region Functions
        public static void UnsuscribeSolutionComponentChanged() {
            if (SolutionComponentChanged != null) {
                var arr = SolutionComponentChanged.GetInvocationList();
                Parallel.For(0, arr.LongLength, (i) => {
                    SolutionComponentChanged -= arr[i] as EventHandler<SolutionComponentChangedEventArgs>;
                });
            }
        }
        public void Dispose() {
            foreach (var item in this)
                item.Dispose();

            //Do manual de-referencing, otherwise we have a mem leak. This is expensive but effective.
            foreach (FieldInfo info in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                if (!info.FieldType.IsValueType)
                    info.SetValue(this, null);
        }
        /// <summary>
        ///     Will only invoke SolutionComponentChanged if the ShowOnGui property of the item equals true.
        /// </summary>
        /// <param name="item"></param>
        public void Add(BaseItem item) {
            AddWithoutInvokingEvent(item);
            //Added one
            if (item.ShowInGui)
                InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);
        }

        /// <summary>
        /// </summary>
        public virtual void Clear() {
            if (_items.Count != 0) {
                ClearWithoutInvokingEvent();
                InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Cleared);
            }
        }

        public bool Contains(BaseItem item) { return _items.Contains(item); }

        public void CopyTo(BaseItem[] array, int arrayIndex) { _items.CopyTo(array, arrayIndex); }

        /// <summary>
        ///     Use Parent.Remove(this) and not Remove(this) ('this' is not a part of its own items collection).
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(BaseItem item) {
            if (item == this)
                throw new Exception("Use Parent.Remove(this) instead of Remove(this).");
            if (_items.Remove(item)) {
                item.RemoveParent();
                item.RemoveTag();
                InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Removed, item);

                CloseView(item);

                item = null;

                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>True for one or more items removed.</returns>
        public bool RemoveRange(IEnumerable<BaseItem> collection) {
            bool removed = false;
            foreach (var item in collection) if (Remove(item)) removed = true;
            return removed;
        }
        public bool RemoveRangeWithoutInvokingEvent(IEnumerable<BaseItem> collection) {
            bool removed = false;
            foreach (var item in collection) if (RemoveWithoutInvokingEvent(item)) removed = true;
            return removed;
        }
        public bool RemoveWithoutInvokingEvent(BaseItem item) {
            if (item == this)
                throw new Exception("Use Parent.Remove(this) instead of Remove(this).");
            if (_items.Remove(item)) {

                item.RemoveParent();
                item.RemoveTag();

                CloseView(item);

                item = null;

                return true;
            }
            return false;
        }

        private void CloseView(BaseItem item) {
            foreach (var view in SolutionComponentViewManager.GetAllViews())
                if (view.SolutionComponent == item) {
                    try {
                        view.Close();
                    }
                    catch {
                        //Don't care.
                    }
                    break;
                }
        }

        public IEnumerator<BaseItem> GetEnumerator() { return _items.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return _items.GetEnumerator(); }

        public void AddWithoutInvokingEvent(BaseItem item) {
            _items.Add(item);
            item.SetParent(this);
            item.ForceSettingChildsParent();
        }

        /// <summary>
        /// Most basic add, do this for building fast temp structures.
        /// </summary>
        /// <param name="item"></param>
        public void AddWithoutInvokingEventDoNotSetParent(BaseItem item) {
            _items.Add(item);
        }

        /// <summary>
        ///     Add a new item, if the same type is found marked as default item (AddAsDefaultItem in the constructor of another base item) it will be replaced.
        /// </summary>
        /// <param name="item"></param>
        internal void AddWhileLoading(BaseItem item) {
            Type itemType = item.GetType();
            int index = -1;
            if (_defaultItemTypes.Contains(itemType))
                for (int i = 0; i < _items.Count; i++)
                    if (_items[i].GetType() == itemType && _items[i].IsDefaultItem) {
                        index = i;
                        break;
                    }
            if (index == -1)
                _items.Add(item);
            else {
                BaseItem oldItem = _items[index];
                _items.Insert(index, item);
                oldItem.RemoveParent();
                _items.Remove(oldItem);
            }
            item.SetParent(this);
            item.ForceSettingChildsParent();
        }

        /// <summary>
        ///     May only be used when loading or adding when the parent has not yet been shown in the gui.
        ///     Don't worry about duplicates when loading a solution, the framework handles this.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>The index</returns>
        public int AddAsDefaultItem(BaseItem item) {
            //Faster check when calling AddWhileLoading.
            var type = item.GetType();
            if (!_defaultItemTypes.Contains(type))
                _defaultItemTypes.Add(type);

            int index = _items.Count;
            _items.Add(item);

            item.IsDefaultItem = true;
            item.SetParent(this);
            item.ForceSettingChildsParent();
            return index;
        }

        /// <summary>
        ///     Pastes, if any, a item from the given child type in the items collection.
        /// </summary>
        /// <param name="childType"></param>
        public void AddRange(IEnumerable<BaseItem> collection) {
            AddRangeWithoutInvokingEvent(collection);
            //Added multiple
            InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, false);
        }

        /// <summary>
        ///     Pastes, if any, a item from the given child type in the items collection.
        /// </summary>
        /// <param name="childType"></param>
        public void AddRangeWithoutInvokingEvent(IEnumerable<BaseItem> collection) {
            _items.AddRange(collection);
            foreach (BaseItem item in collection) {
                item.SetParent(this);
                item.ForceSettingChildsParent();
            }
        }

        public void Insert(int index, BaseItem item) {
            InsertWithoutInvokingEvent(index, item);

            //Added one
            if (item.ShowInGui)
                InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);
        }

        /// <summary>
        ///     Use "added, true"
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void InsertWithoutInvokingEvent(int index, BaseItem item) {
            _items.Insert(index, item);
            item.SetParent(this);
            item.ForceSettingChildsParent();
        }

        /// <summary>
        ///     Use "added, true"
        /// </summary>
        /// <param name="index"></param>
        /// <param name="collection"></param>
        public void InserRangeWithoutInvokingEvent(int index, IEnumerable<BaseItem> collection) {
            _items.InsertRange(index, collection);

            foreach (BaseItem item in collection) {
                item.SetParent(this);
                item.ForceSettingChildsParent();
            }
        }

        public virtual void ClearWithoutInvokingEvent() {
            foreach (BaseItem item in _items) {
                item.RemoveParent();
                item.RemoveTag();

                CloseView(item);
            }
            _items.Clear();
        }

        /// <summary>
        ///     Will be -1 if this is the parent but the child is not yet added to the internal collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(BaseItem item) { return _items.IndexOf(item); }

        /// <summary>
        ///     Gets the tree nodes for the childs (where ShoInGui == true).
        /// </summary>
        /// <returns></returns>
        internal List<TreeNode> GetChildNodes() {
            var childNodes = new List<TreeNode>();
            foreach (BaseItem item in _items)
                if (item.ShowInGui) {
                    TreeNode node = item.GetTreeNode();
                    if (node != null)
                        childNodes.Add(node);
                }
            return childNodes;
        }

        /// <summary>
        ///     Gets the tree node for 'this' and the childnodes.
        /// </summary>
        /// <returns></returns>
        public TreeNode GetTreeNode() {
            var node = new TreeNode(ToString());
            node.Tag = this;
            node.ContextMenuStrip = GetContextMenuStrip();
            if (node.ContextMenuStrip != null) {
                node.ContextMenuStrip.Tag = this;
                node.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            }
            node.Nodes.AddRange(GetChildNodes().ToArray());
            return node;
        }
        /// <summary>
        /// Keep locking in mind.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            var menu = sender as ContextMenuStrip;
            var target = menu.Tag as SolutionComponent;
            foreach (ToolStripMenuItem item in menu.Items)
                item.Enabled = (!target.Locked || (item.ShortcutKeyDisplayString != null && item.ShortcutKeyDisplayString.EndsWith("<enter>", StringComparison.InvariantCultureIgnoreCase))); //Cutting corners here.
        }

        /// <summary>
        ///     Gets the image associated with 'this'.
        ///     If it is not found, it will search for it only the first time, so don't worry about performance (if you use the imagelist correctly).
        ///     Usefull for treeviews and menus.
        /// </summary>
        /// <returns></returns>
        public Image GetImage() {
            Image image = null;
            if (!_noImage) {
                Type thisType = GetType();
                //If this fails it is because you don't have a resources.resx for the type's assembly.
                var rm = new ResourceManager(thisType.Assembly.GetName().Name + ".Properties.Resources",
                                             thisType.Assembly);
                try {
                    image = rm.GetObject(thisType.Name) as Image;
                }
                catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed getting image.", ex);
                }
                _noImage = (image == null);
            }
            return image;
        }

        /// <summary>
        ///     Gets the contextmenu based on reflection and attributes. (JIT Compiling).
        /// </summary>
        /// <returns></returns>
        private ContextMenuStrip GetContextMenuStrip() {
            object[] attributes = GetType().GetCustomAttributes(typeof(ContextMenuAttribute), true);
            if (attributes.Length != 0)
                return (attributes[0] as ContextMenuAttribute).GetContextMenuStrip(this);
            return null;
        }

        internal void HandleHotkey(Keys hotkey) {
            object[] attributes = GetType().GetCustomAttributes(typeof(HotkeysAttribute), true);
            if (attributes.Length != 0)
                (attributes[0] as HotkeysAttribute).HandleHotkey(this, hotkey);
        }

        /// <summary>
        ///     If Activate_Click is specified in the hotkeys this will call that method.
        /// </summary>
        internal void HandleDoubleClick() {
            object[] attributes = GetType().GetCustomAttributes(typeof(HotkeysAttribute), true);
            if (attributes.Length != 0) {
                Keys hotkey;
                if ((attributes[0] as HotkeysAttribute).TryGetHotkey("Activate_Click", out hotkey))
                    Activate();
            }
        }

        //Standard ContextMenuItems
        internal void Activate_Click(object sender, EventArgs e) {
            Activate();
        }

        internal void Clear_Click(object sender, EventArgs e) {
            if (Count != 0 &&
                MessageBox.Show(string.Format("Are you sure you want to clear '{0}'?", this), string.Empty,
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) ==
                DialogResult.Yes)
                Clear();
        }

        internal void SortItemsByLabel_Click(object sender, EventArgs e) { SortItemsByLabel(); }
        /// <summary>
        ///     Only the labeled ones are sorted, (BaseItems are put at the start of the collections, but they should always be there anyways).
        /// </summary>
        public void SortItemsByLabel() {
            var items = new List<BaseItem>();
            var labeledBaseItems = new List<LabeledBaseItem>();
            foreach (BaseItem item in _items)
                if (item is LabeledBaseItem)
                    labeledBaseItems.Add(item as LabeledBaseItem);
                else
                    items.Add(item);
            if (!IsSorted(labeledBaseItems)) {
                labeledBaseItems.Sort(LabeledBaseItemComparer.GetInstance());
                _items.Clear();
                _items.AddRange(items);
                foreach (LabeledBaseItem item in labeledBaseItems)
                    _items.Add(item);
                //Added multiple
                InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, false);
            }
        }

        private bool IsSorted(List<LabeledBaseItem> labeledBaseItems) {
            for (int i = 0; i < labeledBaseItems.Count - 1; i++)
                if (labeledBaseItems[i].Label.CompareTo(labeledBaseItems[i + 1].Label) > 0)
                    return false;
            return true;
        }

        /// <summary>
        ///     Set the parent again, this information is lost after sending over a socket for example.
        ///     The parent changed event will not be invoked.
        /// </summary>
        public void ForceSettingChildsParent() {
            foreach (BaseItem item in this) {
                item.SetParent(this);
                item.ForceSettingChildsParent();
            }
        }

        /// <summary>
        ///     Sets the next child if available, otherwise you get an empty variant.
        ///     You are responisble to suscribe to ParentIsNull afterwards.
        /// </summary>
        /// <param name="childType">The parent can contain childs of different types, therefore this is important</param>
        /// <param name="parent"></param>
        public static BaseItem GetNextOrEmptyChild(Type childType, SolutionComponent parent) {
            if (parent != null) {
                int count = parent.CountOf(childType);
                if (count != 0)
                    foreach (object item in parent)
                        if (item.GetType() == childType && item.GetParent() == parent)
                            //GetParent is the parent from the global cache, default items in a collection don't know their parents.
                            return item as BaseItem;

                return BaseItem.GetEmpty(childType, parent);
            }
            return null;
        }

        /// <summary>
        ///     Virtual method for activation, example: SolutionComponentViewManager.Show(this, typeof(SolutionComponentPropertyView)) --> default.
        ///     Override this if you want to provide your own GUI (must inherit BaseSolutionComponentView).
        /// </summary>
        /// <returns></returns>
        public virtual BaseSolutionComponentView Activate() { return SolutionComponentViewManager.Show(this, typeof(SolutionComponentPropertyView)); }

        /// <summary>
        /// </summary>
        /// <param name="doneAction"></param>
        /// <param name="arg">true or false for added one or multiple; the removed solution component.</param>
        public void InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction doneAction, object arg = null) {
            //Fairly complicated setup to avoid gui freezes.
            try {
                BackgroundWorkQueueWrapper.BackgroundWorkQueue.EnqueueWorkItem(_invokeSolutionComponentChangedEventDelegate, doneAction, arg);
            }
            catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed invoking solution component changed.", ex, new object[] { doneAction, arg });
            }
        }
        private void InvokeSolutionComponentChangedEventCallback(SolutionComponentChangedEventArgs.DoneAction doneAction, object arg) {
            try {
                if (SolutionComponentChanged != null && Solution.ActiveSolution != null)
                    SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                        try {
                            foreach (EventHandler<SolutionComponentChangedEventArgs> del in SolutionComponentChanged.GetInvocationList())
                                if (del != null)
                                    if (del.Target != null && del.Target is Control && !(del.Target as Control).IsDisposed)
                                        del(this, new SolutionComponentChangedEventArgs(doneAction, arg));
                                    else //If the target == null no issue, disposed => crash.
                                        try { del(this, new SolutionComponentChangedEventArgs(doneAction, arg)); }
                                        catch {
                                            //Ignore in this case. Do not care.
                                        }
                        }
                        catch (Exception ex) {
                            Loggers.Log(Level.Error, "Failed invoking solution component changed.", ex, new object[] { this, doneAction, arg });
                        }
                    }, null);
            }
            catch (Exception exc) {
                Loggers.Log(Level.Error, "Failed invoking solution component changed.", exc, new object[] { this, doneAction, arg });
            }
        }
        #endregion

    }

    public class SolutionComponentChangedEventArgs : EventArgs {
        /// <summary>
        ///     To determine how the gui should be updated.
        /// </summary>
        public enum DoneAction {
            /// <summary>
            ///     Give true or false with as arg for added one or added multiple.
            /// </summary>
            Added,
            Edited,
            Cleared,

            /// <summary>
            ///     Give the removed solution component with as arg.
            /// </summary>
            Removed
        }

        #region Properties
        public object Arg { get; private set; }
        public DoneAction __DoneAction { get; private set; }
        #endregion

        #region Constructor
        public SolutionComponentChangedEventArgs(DoneAction doneAction, object arg) {
            __DoneAction = doneAction;
            Arg = arg;
        }
        #endregion
    }

    public class LockedChangedEventArgs : EventArgs {
        public bool Locked { get; private set; }
        public LockedChangedEventArgs(bool locked) { Locked = locked; }
    }
}