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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using vApus.Util;

namespace vApus.SolutionTree {
    /// <summary>
    ///     The base item for everything you want in the solution. When names are not unique, use LabeledBaseItem to ensure nothing can/will break.
    /// </summary>
    [Serializable]
    public abstract class BaseItem : SolutionComponent {
        #region Fields

        protected Dictionary<PropertyInfo, string> _branchedInfos;

        #endregion

        #region Properties

        /// <summary>
        ///     Sets or gets the parent, using the underlying extension methods.
        /// </summary>
        public SolutionComponent Parent {
            get {
                //base.GetParent() doesn't work, but this does
                //Casting to an object creates a shallow copy of it (same as MemberwiseClone()).
                //The main problem was that in this manner the underlying objects are static.
                //I am so smart, S.M.R.T., I mean S.M.A.R.T.
                return (this).GetParent() as SolutionComponent;
            }
            set {
                if (value == null)
                    (this).RemoveParent();
                else
                    (this).SetParent(value);
            }
        }

        #endregion

        #region Constructors

        #endregion

        #region Functions

        /// <summary>
        ///     It is adviced to use Solution.GetNextOrEmptyChild instead of this when having base items not contained in the items collection.
        ///     However this is sometimes handy to add a new and empty item to the collection.
        /// </summary>
        /// <param name="baseItemType"></param>
        /// <param name="parent">Required! Use Solution.ActiveSolution.GetSolutionComponent or .GetLabeledBaseItem to retrieve the right one.</param>
        /// <returns></returns>
        public static BaseItem Empty(Type baseItemType, SolutionComponent parent) {
            var emptyItem = Activator.CreateInstance(baseItemType) as BaseItem;
            emptyItem.Parent = parent;
            emptyItem.IsEmpty = true; //Default true;

            return emptyItem;
        }

        /// <summary>
        ///     Returns the siblings of the same type as this.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BaseItem> GetSiblings() {
            foreach (BaseItem item in Parent)
                if (item != this && item.GetType() == GetType())
                    yield return item;
        }

        /// <summary>
        ///     Gets the xml to save based on reflection and attributes.
        ///     It will append to the xml document so nothing needs to be returned (the beauty of object orientation).
        ///     Furthermore all primary datatypes and arrays/generic lists containing primary datatypes can be saved and base items not contained in the items collection.
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <param name="parent"></param>
        internal void GetXmlToSave(XmlDocument xmlDocument, XmlElement parent) {
            XmlElement element = xmlDocument.CreateElement(GetType().Name);
            parent.AppendChild(element);

            //Save the child items.
            XmlElement itemsElement = xmlDocument.CreateElement("Items");
            foreach (BaseItem item in this)
                item.GetXmlToSave(xmlDocument, itemsElement);
            element.AppendChild(itemsElement);

            //Save the other properties. (Just primairy datatypes should be saved (and "BaseItems")
            foreach (
                PropertyInfo info in
                    GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                object[] customAttributesFilter = info.GetCustomAttributes(typeof(SavableCloneableAttribute), true);
                if (customAttributesFilter.Length > 0) {
                    var savableCloneableAttribute = customAttributesFilter[0] as SavableCloneableAttribute;
                    XmlElement childElement = xmlDocument.CreateElement(info.Name);

                    object value = info.GetValue(this, null);
                    //All childs must be savable, other collections are not supported
                    if (value != null && value.GetParent() != null && value.GetParent() is IEnumerable) {
                        childElement.SetAttribute("args", "vApus.BranchedIndexType");
                        if (value is BaseItem) {
                            if (!(value as BaseItem).IsEmpty)
                                childElement.InnerText = BranchedIndex(value as BaseItem);
                        } else {
                            string thisBranchedIndex = BranchedIndex(this);
                            var valueParent = value.GetParent() as IEnumerable;
                            IEnumerator enumerator = valueParent.GetEnumerator();
                            int valueIndex = 0;
                            while (enumerator.MoveNext()) {
                                if (enumerator.Current.Equals(value))
                                    break;
                                ++valueIndex;
                            }

                            string parentName = null;
                            foreach (
                                FieldInfo fieldInfo in
                                    GetType()
                                        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                ) {
                                object fieldInfoValue = fieldInfo.GetValue(this);
                                if (fieldInfoValue != null && fieldInfoValue.Equals(valueParent)) {
                                    parentName = fieldInfo.Name;
                                    break;
                                }
                            }
                            childElement.InnerText = string.Format("{0}.{1}.{2}", thisBranchedIndex, parentName,
                                                                   valueIndex);
                        }
                    } else if (value is ICollection) {
                        var sb = new StringBuilder();
                        var collection = value as ICollection;
                        Type elementType = collection.AsQueryable().ElementType;
                        IEnumerator enumerator = collection.GetEnumerator();
                        enumerator.Reset();
                        if (elementType.BaseType == typeof(Enum))
                            while (enumerator.MoveNext()) {
                                sb.Append(Enum.GetName(elementType, enumerator.Current));
                                sb.Append(';');
                            } else
                            while (enumerator.MoveNext())
                                if (enumerator.Current != null) {
                                    sb.Append(enumerator.Current);
                                    sb.Append(';');
                                }

                        childElement.InnerText = sb.ToString();
                        if (childElement.InnerText.Length > 0)
                            childElement.InnerText = childElement.InnerText.Substring(0,
                                                                                      childElement.InnerText.Length - 1);

                        if (savableCloneableAttribute.Encrypt) {
                            childElement.InnerText =
                                childElement.InnerText.Encrypt("{A84E447C-3734-4afd-B383-149A7CC68A32}",
                                                               new byte[]
                                                                   {
                                                                       0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76
                                                                       , 0x65, 0x64, 0x65, 0x76
                                                                   });
                            childElement.SetAttribute("args", "Encrypted");
                        }
                    } else if (value != null) {
                        childElement.InnerText = (value is Enum)
                                                     ? Enum.GetName(value.GetType(), value)
                                                     : value.ToString();
                        if (savableCloneableAttribute.Encrypt) {
                            childElement.InnerText =
                                childElement.InnerText.Encrypt("{A84E447C-3734-4afd-B383-149A7CC68A32}",
                                                               new byte[]
                                                                   {
                                                                       0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76
                                                                       , 0x65, 0x64, 0x65, 0x76
                                                                   });
                            childElement.SetAttribute("args", "Encrypted");
                        }
                    }
                    element.AppendChild(childElement);
                }
            }
        }

        private string BranchedIndex(BaseItem item) {
            for (int i = 0; i < Solution.ActiveSolution.Projects.Count; i++) {
                string branchedIndex = FindIndexInBranch(Solution.ActiveSolution.Projects[i], item, i.ToString());
                if (branchedIndex != null)
                    return branchedIndex;
            }
            return null;
        }

        private string FindIndexInBranch(SolutionComponent parent, BaseItem item, string partialyBranchedIndex) {
            int index = parent.IndexOf(item);
            if (index > -1)
                return partialyBranchedIndex + '.' + index;

            for (int i = 0; i < parent.Count; i++) {
                string branchedIndex = FindIndexInBranch(parent[i], item, partialyBranchedIndex + '.' + i);
                if (branchedIndex != null)
                    return branchedIndex;
            }

            return null;
        }

        /// <summary>
        ///     Load 'this' and childs based on activation and reflection.
        ///     Furthermore all primary datatypes and arrays/generic lists containing primary datatypes can be loaded and base items not contained in the items collection.
        /// </summary>
        /// <param name="node"></param>
        public void LoadFromXml(XmlNode node, out string errorMessage) {
            //Error reporting.
            var sb = new StringBuilder();
            _branchedInfos = new Dictionary<PropertyInfo, string>();
            foreach (XmlNode childNode in node.ChildNodes)
                if (childNode.Name == "Items")
                    foreach (XmlNode elementNode in childNode.ChildNodes) {
                        try {
                            var item =
                                Activator.CreateInstance(GetType().Assembly.GetTypeByName(elementNode.Name)) as BaseItem;
                            string childErrorMessage;
                            item.LoadFromXml(elementNode, out childErrorMessage);
                            sb.Append(childErrorMessage);
                            AddWhileLoading(item);
                        } catch (Exception ex) {
                            string s = "[" + this + "] " + childNode.Name;
                            LogWrapper.LogByLevel(
                                "Failed loading " + s +
                                " from .vass\nThis is usally not a problem: Changes in functionality for this version of vApus that are not in the opened .vass file.\nTake a copy of the file to be sure and test if stresstesting works.\n" +
                                ex, LogLevel.Warning);

                            sb.Append(s);
                            sb.Append(";");
                        }
                    } else
                    foreach (
                        PropertyInfo info in
                            GetType()
                                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                        if (info.Name == childNode.Name) {
                            try {
                                bool branchedIndexType = false;
                                bool encrypted = false;

                                foreach (XmlAttribute attribute in childNode.Attributes)
                                    if (attribute.Name == "args")
                                        if (attribute.Value == "vApus.BranchedIndexType") {
                                            branchedIndexType = true;
                                            break;
                                        } else if (attribute.Value == "Encrypted") {
                                            encrypted = true;
                                            break;
                                        }

                                if (branchedIndexType) {
                                    _branchedInfos.Add(info, childNode.InnerText);
                                } else {
                                    var collection = info.GetValue(this, null) as ICollection;
                                    if (collection == null) {
                                        string innerText = childNode.InnerText;
                                        if (encrypted)
                                            innerText = innerText.Decrypt("{A84E447C-3734-4afd-B383-149A7CC68A32}",
                                                                          new byte[]
                                                                              {
                                                                                  0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65,
                                                                                  0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                                                                              });

                                        if (info.PropertyType.BaseType == typeof(Enum)) {
                                            info.SetValue(this, Enum.Parse(info.PropertyType, innerText), null);
                                        } else {
                                            object o = Convert.ChangeType(innerText, info.PropertyType);
                                            if (o == null)
                                                throw new Exception();
                                            info.SetValue(this, o, null);
                                        }
                                    } else {
                                        string[] array = childNode.InnerText.Split(';');
                                        var arrayList = new ArrayList(array.Length);
                                        Type elementType = collection.AsQueryable().ElementType;

                                        if (elementType.BaseType == typeof(Enum))
                                            foreach (string s in array) {
                                                string value = s;
                                                if (encrypted)
                                                    value = value.Decrypt("{A84E447C-3734-4afd-B383-149A7CC68A32}",
                                                                          new byte[]
                                                                              {
                                                                                  0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65,
                                                                                  0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                                                                              });

                                                arrayList.Add(Enum.Parse(elementType, value));
                                            } else
                                            foreach (string s in array) {
                                                string value = s;
                                                if (encrypted)
                                                    value = value.Decrypt("{A84E447C-3734-4afd-B383-149A7CC68A32}",
                                                                          new byte[]
                                                                              {
                                                                                  0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65,
                                                                                  0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                                                                              });

                                                try {
                                                    object o = Convert.ChangeType(value, elementType);
                                                    arrayList.Add(o);
                                                } catch {
                                                    //Do not throw this if the array contains no elements (he thinks it does, but it does not).
                                                    if (array.Length != 1)
                                                        throw new Exception();
                                                }
                                            }
                                        if (childNode.InnerText.Length > 0)
                                            if (collection is Array) {
                                                info.SetValue(this, arrayList.ToArray(elementType), null);
                                            } else {
                                                var list = collection as IList;
                                                list.Clear();
                                                for (int i = 0; i < arrayList.Count; i++)
                                                    list.Add(arrayList[i]);
                                            }
                                    }
                                }
                            } catch (Exception ex) {
                                string s = "[" + this + "] " + childNode.Name;
                                LogWrapper.LogByLevel(
                                    "Failed loading " + s +
                                    " from .vass\nThis is usally not a problem: Changes in functionality for this version of vApus that are not in the opened .vass file.\nTake a copy of the file to be sure and test if stresstesting works.\n" +
                                    ex, LogLevel.Warning);

                                sb.Append(s);
                                sb.Append(";");
                            }
                            break;
                        }
            errorMessage = sb.ToString();
        }

        /// <summary>
        ///     An item's dependencies are set using branched indices, they must be resolved to load the right values from "referenced" objects.
        /// </summary>
        public void ResolveBranchedIndices() {
            foreach (BaseItem item in this)
                item.ResolveBranchedIndices();
            if (_branchedInfos != null) {
                foreach (PropertyInfo info in _branchedInfos.Keys) {
                    object value = ResolveBranchedIndex(_branchedInfos[info]);
                    if (value != null)
                        try {
                            info.SetValue(this, value, null);
                        } catch {
                        }
                }
                _branchedInfos = null;
            }
        }

        private object ResolveBranchedIndex(string branchedIndex) {
            branchedIndex = branchedIndex.Trim();
            if (branchedIndex.Length > 0) {
                string[] splittedBranchedIndex = branchedIndex.Split('.');
                var indices = new object[splittedBranchedIndex.Length];

                for (int i = 0; i < splittedBranchedIndex.Length; i++) {
                    int index;
                    if (int.TryParse(splittedBranchedIndex[i], out index))
                        indices[i] = index;
                    else
                        indices[i] = splittedBranchedIndex[i];
                }

                SolutionComponent item = Solution.ActiveSolution.GetProject((int)indices[0]);
                for (int i = 1; i < indices.Length; i++) {
                    object o = indices[i];
                    if (o is int) {
                        try {
                            item = item[(int)o];
                        } catch {
                        }
                    } else {
                        foreach (
                            FieldInfo info in
                                item.GetType()
                                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                            if (info.Name == o as string) {
                                IEnumerator enumerator = (info.GetValue(item) as IEnumerable).GetEnumerator();
                                int valueIndex = 0;
                                while (enumerator.MoveNext()) {
                                    if (valueIndex.Equals(indices[i + 1]))
                                        return enumerator.Current;
                                    ++valueIndex;
                                }
                            }
                    }
                }

                return item;
            }
            return null;
        }

        //Standard ContextMenuItems
        internal void Import_Click(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Xml Files (*.xml) | *.xml";
            ofd.Title = (sender is ToolStripMenuItem)
                            ? (sender as ToolStripMenuItem).Text
                            : ofd.Title = "Import from...";

            if (ofd.ShowDialog() == DialogResult.OK) {
                var sb = new StringBuilder();
                foreach (string fileName in ofd.FileNames) {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(fileName);

                    try {
                        if (xmlDocument.FirstChild.Name == GetType().Name &&
                            xmlDocument.FirstChild.FirstChild.Name == "Items") {
                            string errorMessage;
                            LoadFromXml(xmlDocument.FirstChild, out errorMessage);
                            sb.Append(errorMessage);
                            if (errorMessage.Length == 0) {
                                ResolveBranchedIndices();
                                InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added,
                                                                    true);
                            }
                        } else {
                            sb.Append(fileName + " contains no valid structure to load;");
                        }
                    } catch {
                        sb.Append("Unknown or non-existing item found in " + fileName + ";");
                    }
                }
                if (sb.ToString().Length > 0) {
                    string s = "Failed loading: " + sb;
                    LogWrapper.LogByLevel(s, LogLevel.Error);
                    MessageBox.Show(s, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error,
                                    MessageBoxDefaultButton.Button1);
                }
            }
        }

        internal void Remove_Click(object sender, EventArgs e) {
            if (
                MessageBox.Show(string.Format("Are you sure you want to remove '{0}'?", this), string.Empty,
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) ==
                DialogResult.Yes)
                Parent.Remove(this);
        }

        internal void Cut_Click(object sender, EventArgs e) {
            ClipboardWrapper.SetDataObject(GetXmlStructure().InnerXml);
            Parent.Remove(this);
        }

        internal void Copy_Click(object sender, EventArgs e) {
            Copy();
        }

        protected internal void Copy() {
            ClipboardWrapper.SetDataObject(GetXmlStructure().InnerXml);
        }

        internal void Paste_Click(object sender, EventArgs e) {
            Paste();
        }

        protected internal void Paste() {
            IDataObject dataObject = ClipboardWrapper.GetDataObject();
            Type stringType = typeof(string);
            if (dataObject.GetDataPresent(stringType)) {
                try {
                    var clipboardString = dataObject.GetData(stringType) as string;
                    if (clipboardString != null) {
                        if (clipboardString.StartsWith('<' + GetType().Name)) {
                            var xmlDocument = new XmlDocument();
                            xmlDocument.LoadXml(clipboardString);

                            PasteXmlStructure(xmlDocument);
                        }
                    }
                } catch {
                }
            }
        }

        internal void Duplicate_Click(object sender, EventArgs e) {
            Copy();
            SolutionComponent parent = Parent;
            if (parent is BaseItem) (parent as BaseItem).Paste();
            else if (parent is BaseProject) (parent as BaseProject).Paste();
        }
        /// <summary>
        ///     Copy + paste
        /// </summary>
        public void Duplicate() {
            Copy();
            SolutionComponent parent = Parent;
            if (parent is BaseItem) (parent as BaseItem).Paste();
            else if (parent is BaseProject) (parent as BaseProject).Paste();
        }
        /// <summary>
        ///     Paste xml gotten from 'GetXmlStructure'.
        /// </summary>
        /// <param name="xmlDocument"></param>
        internal void PasteXmlStructure(XmlDocument xmlDocument) {
            if (xmlDocument.ChildNodes.Count > 0 && xmlDocument.FirstChild.ChildNodes.Count > 0) {
                string errorMessage;
                if (xmlDocument.FirstChild.Name == GetType().Name && xmlDocument.FirstChild.FirstChild.Name == "Items")
                    LoadFromXml(xmlDocument.FirstChild, out errorMessage);
                else
                    return;
                if (errorMessage.Length == 0) {
                    ResolveBranchedIndices();
                    InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);
                }
            }
        }

        /// <summary>
        ///     Includes parent information too.
        /// </summary>
        /// <returns></returns>
        public XmlDocument GetXmlStructure() {
            var xmlDocument = new XmlDocument();
            XmlElement parent = xmlDocument.CreateElement(Parent.GetType().Name);
            xmlDocument.AppendChild(parent);
            if (Parent is BaseProject) {
                GetXmlToSave(xmlDocument, parent);
            } else {
                XmlElement items = xmlDocument.CreateElement("Items");
                parent.AppendChild(items);
                GetXmlToSave(xmlDocument, items);
            }
            return xmlDocument;
        }

        /// <summary>
        ///     Get an empty BaseItem with the type and parent of the caller.
        /// </summary>
        /// <returns></returns>
        public BaseItem GetEmptyVariant() {
            return Empty(GetType(), Parent);
        }

        public override string ToString() {
            return IsEmpty ? "<none>" : Name;
        }

        #endregion
    }
}