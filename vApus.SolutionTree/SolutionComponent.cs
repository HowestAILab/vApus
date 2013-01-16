/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.SolutionTree {
    /// <summary>
    ///     The base class for BaseItem and BaseProject.
    /// </summary>
    [Serializable]
    public abstract class SolutionComponent : Object, ICollection<BaseItem> {
        public SolutionComponent() {
            /// <summary>
            /// To Check if the parent has become null.
            /// 
            /// That way you can choose another base item to store in your object. Or make a new empty one with the right parent.
            ObjectExtension.ParentChanged += ObjectExtension_ParentChanged;
        }

        #region Functions

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

        public bool Contains(BaseItem item) {
            return _items.Contains(item);
        }

        public void CopyTo(BaseItem[] array, int arrayIndex) {
            _items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///     Use Parent.Remove(this) and not Remove(this) ('this' is not a part of its own items collection).
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(BaseItem item) {
            if (item == this)
                throw new Exception("Use Parent.Remove(this) instead of Remove(this).");
            if (_items.Remove(item)) {
                item.Parent = null;
                item.RemoveTag();
                InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Removed, item);

                item = null;

                return true;
            }
            return false;
        }

        public IEnumerator<BaseItem> GetEnumerator() {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _items.GetEnumerator();
        }

        /// <summary>
        ///     Checks if the parent has become null.
        ///     That way you can choose another base item to store in your object. Or make a new empty one with the right parent.
        /// </summary>
        protected void ObjectExtension_ParentChanged(
            ObjectExtension.ParentOrTagChangedEventArgs parentOrTagChangedEventArgs) {
            if (Solution.ActiveSolution != null)
                if (parentOrTagChangedEventArgs.Child == this)
                    if (parentOrTagChangedEventArgs.New == null && ParentIsNull != null)
                        foreach (EventHandler del in ParentIsNull.GetInvocationList())
                            del.BeginInvoke(this, null, null, null);
        }

        public void AddWithoutInvokingEvent(BaseItem item, bool invokeParentChanged = true) {
            _items.Add(item);
            item.SetParent(this, invokeParentChanged);
            item.ForceSettingChildsParent();
        }

        /// <summary>
        ///     Add a new item, if the same type is found marked as default item (AddAsDefaultItem in the constructor of another base item) it will be replaced.
        /// </summary>
        /// <param name="item"></param>
        internal void AddWhileLoading(BaseItem item) {
            Type itemType = item.GetType();
            int index = -1;
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
                oldItem.RemoveParent(false);
                _items.Remove(oldItem);
            }
            item.SetParent(this, false);
            item.ForceSettingChildsParent();
        }

        /// <summary>
        ///     May only be used when loading or adding when the parent has not yet been shown in the gui.
        ///     Don't worry about duplicates when loading a solution, the framework handles this.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>The index</returns>
        public int AddAsDefaultItem(BaseItem item) {
            int index = _items.Count;
            _items.Add(item);

            item.IsDefaultItem = true;
            item.SetParent(this, false);
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
        public void AddRangeWithoutInvokingEvent(IEnumerable<BaseItem> collection, bool invokeParentChanged = true) {
            _items.AddRange(collection);
            foreach (BaseItem item in collection) {
                item.SetParent(this, invokeParentChanged);
                item.ForceSettingChildsParent();
            }
        }

        /// <summary>
        ///     Use "added, true"
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void InsertWithoutInvokingEvent(int index, BaseItem item, bool invokeParentChanged = true) {
            _items.Insert(index, item);
            item.SetParent(this, invokeParentChanged);
            item.ForceSettingChildsParent();
        }

        /// <summary>
        ///     Use "added, true"
        /// </summary>
        /// <param name="index"></param>
        /// <param name="collection"></param>
        public void InserRangeWithoutInvokingEvent(int index, IEnumerable<BaseItem> collection,
                                                   bool invokeParentChanged = true) {
            _items.InsertRange(index, collection);

            foreach (BaseItem item in collection) {
                item.SetParent(this, invokeParentChanged);
                item.ForceSettingChildsParent();
            }
        }

        public virtual void ClearWithoutInvokingEvent(bool invokeParentChanged = true) {
            foreach (BaseItem item in _items) {
                item.RemoveParent(invokeParentChanged);
                item.RemoveTag();
            }
            _items.Clear();
        }

        /// <summary>
        ///     Will be -1 if this is the parent but the child is not yet added to the internal collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(BaseItem item) {
            return _items.IndexOf(item);
        }

        public bool RemoveWithoutInvokingEvent(BaseItem item) {
            if (item == this)
                throw new Exception("Use Parent.Remove(this) instead of Remove(this).");
            if (_items.Remove(item)) {
                item.Parent = null;
                item.RemoveTag();
                item = null;

                return true;
            }
            return false;
        }

        /// <summary>
        ///     Gets the tree nodes for the childs.
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
            node.Nodes.AddRange(GetChildNodes().ToArray());
            return node;
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
                } catch {
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

        /// <summary>
        ///     Only the labeled ones are sorted, (BaseItems are put at the start of the collections, but they should always be there anyways).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void SortItemsByLabel_Click(object sender, EventArgs e) {
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
                item.SetParent(this, false);
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

                return BaseItem.Empty(childType, parent);
            }
            return null;
        }

        /// <summary>
        ///     Virtual method for activation, example: SolutionComponentViewManager.Show(this, typeof(SolutionComponentPropertyView)) --> default.
        /// </summary>
        /// <returns></returns>
        public virtual void Activate() {
            SolutionComponentViewManager.Show(this, typeof(SolutionComponentPropertyView));
        }

        public void InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction doneAction) {
            if (SolutionComponentChanged != null && Solution.ActiveSolution != null)
                SolutionComponentChanged(this, new SolutionComponentChangedEventArgs(doneAction));
        }

        /// <summary>
        /// </summary>
        /// <param name="doneAction"></param>
        /// <param name="arg">true or false for added one or multiple; the removed solution component.</param>
        public void InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction doneAction,
                                                        object arg) {
            if (SolutionComponentChanged != null && Solution.ActiveSolution != null)
                SolutionComponentChanged(this, new SolutionComponentChangedEventArgs(doneAction, arg));
        }

        #endregion

        public static event EventHandler<SolutionComponentChangedEventArgs> SolutionComponentChanged;

        /// <summary>
        ///     When creating a empty base item, it checks if the parent becomes null. (Happens when removed from a collection, don't set the parent null yourself)
        ///     Call SolutionComponent.GetNextChild when this happens, don't forget to suscribe to this event again for the new item.
        ///     Note: this event is fired on another thread.
        /// </summary>
        public event EventHandler ParentIsNull;

        #region Fields

        private bool _isDefaultItem, _isEmpty;
        protected List<BaseItem> _items = new List<BaseItem>();
        private bool _noImage, _showInGui = true;

        #endregion

        #region Properties

        [Description("The name of this item.")]
        public string Name {
            get {
                if (_isEmpty) {
                    return null;
                } else {
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

        public BaseItem this[int index] {
            get { return _items[index]; }
        }

        [SavableCloneable]
        public bool ShowInGui {
            get { return _showInGui; }
            set { _showInGui = value; }
        }

        [SavableCloneable]
        public bool IsDefaultItem {
            get { return _isDefaultItem; }
            set { _isDefaultItem = value; }
        }

        [SavableCloneable]
        public bool IsEmpty {
            get { return _isEmpty; }
            set { _isEmpty = value; }
        }

        /// <summary>
        ///     The count of the child items (regardless the parent of those items).
        /// </summary>
        [Description("The count of the child items (regardless the parent of those items).")]
        public int Count {
            get { return _items.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        /// <summary>
        ///     The count of the child items that are of a derived type from BaseItem (where the parent is this).
        /// </summary>
        /// <param name="derivedType"></param>
        /// <returns></returns>
        [Description("The count of the child items that are of a derived type from BaseItem (where the parent is this)."
            )]
        public int CountOf(Type derivedType) {
            int count = 0;
            foreach (object item in _items)
                if (item.GetType() == derivedType && item.GetParent() == this)
                    ++count;

            return count;
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

        public readonly object Arg;
        public readonly DoneAction __DoneAction;

        public SolutionComponentChangedEventArgs(DoneAction doneAction) {
            __DoneAction = doneAction;
        }

        public SolutionComponentChangedEventArgs(DoneAction doneAction, object arg)
            : this(doneAction) {
            Arg = arg;
        }
    }
}