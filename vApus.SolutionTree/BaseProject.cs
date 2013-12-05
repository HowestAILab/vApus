/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using vApus.Util;

namespace vApus.SolutionTree {
    /// <summary>
    /// The first childs of a solution, used to bundle different functionalities (Distributed Tests, Monitors, Connections, Logs, Stresstests).
    /// The stuff in projects are either dirived from base items or labeled base items.
    /// This class implements functions to load to and save from vass files (used in Solution), paste, import, ...
    /// </summary>
    [Serializable]
    public abstract class BaseProject : SolutionComponent {

        #region Fields
        private Solution _parent;
        #endregion

        #region Properties
        [DisplayName("Stresstesting Solution FileName.")]
        public string StresstestingSolutionFileName {
            get { return _parent.FileName; }
        }
        public Solution Parent {
            get { return _parent; }
            internal set {
                if (value == null)
                    throw new NullReferenceException("parent");
                _parent = value;
            }
        }
        #endregion

        #region Functions
        public IEnumerable<BaseProject> GetSiblings() {
            foreach (BaseProject project in _parent.Projects)
                if (project != this)
                    yield return project;
        }

        /// <summary>
        ///     Gets a solution component by type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public SolutionComponent GetSolutionComponent(Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            return GetSolutionComponent(this, type);
        }
        /// <summary>
        ///     Gets a solution component by type and name.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public SolutionComponent GetSolutionComponent(Type type, string name) {
            if (type == null)
                throw new ArgumentNullException("type");
            if (name == null)
                throw new ArgumentNullException("name");

            return GetSolutionComponent(this, type, name);
        }
        private SolutionComponent GetSolutionComponent(SolutionComponent solutionComponent, Type type) {
            if (solutionComponent.GetType() == type)
                return solutionComponent;
            foreach (BaseItem item in solutionComponent) {
                SolutionComponent childSolutionComponent = GetSolutionComponent(item, type);
                if (childSolutionComponent != null)
                    return childSolutionComponent;
            }
            return null;
        }
        private SolutionComponent GetSolutionComponent(SolutionComponent solutionComponent, Type type, string name) {
            if (solutionComponent.GetType() == type && name == solutionComponent.Name)
                return solutionComponent;
            foreach (BaseItem item in solutionComponent) {
                SolutionComponent childSolutionComponent = GetSolutionComponent(item, type, name);
                if (childSolutionComponent != null)
                    return childSolutionComponent;
            }
            return null;
        }

        /// <summary>
        ///     Get a labeled base item by name, index and label.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public LabeledBaseItem GetLabeledBaseItem(string name, int index, string label) {
            if (name == null)
                throw new ArgumentNullException("name");
            if (label == null)
                throw new ArgumentNullException("label");

            return GetLabeledBaseItem(this, name, index, label);
        }
        private LabeledBaseItem GetLabeledBaseItem(SolutionComponent solutionComponent, string name, int index, string label) {
            if (solutionComponent is LabeledBaseItem) {
                var labeledBaseItem = solutionComponent as LabeledBaseItem;
                if (name == labeledBaseItem.Name && index == labeledBaseItem.Index && label == labeledBaseItem.Label)
                    return labeledBaseItem;
            }
            foreach (BaseItem item in solutionComponent) {
                LabeledBaseItem childLabelBaseItems = GetLabeledBaseItem(solutionComponent, name, index, label);
                if (childLabelBaseItems != null)
                    return childLabelBaseItems;
            }
            return null;
        }

        /// <summary>
        ///     Gets the xml to save based on reflection and attributes.
        /// </summary>
        /// <returns></returns>
        internal XmlDocument GetXmlToSave() {
            var xmlDocument = new XmlDocument();
            XmlElement element = xmlDocument.CreateElement(GetType().Name);
            xmlDocument.AppendChild(element);
            foreach (BaseItem item in this)
                item.GetXmlToSave(xmlDocument, element);
            return xmlDocument;
        }
        /// <summary>
        ///     Load 'this' and childs based on activation and reflection.
        /// </summary>
        /// <param name="xmlDocument"></param>
        internal void LoadFromXml(XmlDocument xmlDocument, out string errorMessage) {
            //Error reporting.
            var sb = new StringBuilder();
            //The first node is the content type, we don't need this
            XmlNode root = (xmlDocument.FirstChild.Name == GetType().Name)
                               ? xmlDocument.FirstChild
                               : xmlDocument.ChildNodes[1];
            foreach (XmlNode childNode in root.ChildNodes) {
                try {
                    var item = FastObjectCreator.CreateInstance(GetType().Assembly.GetTypeByName(childNode.Name)) as BaseItem;
                    item.SetParent(this);
                    string childErrorMessage;
                    item.LoadFromXml(childNode, out childErrorMessage);
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
            }
            errorMessage = sb.ToString();
        }

        internal void Import_Click(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Xml Files (*.xml) | *.xml";
            ofd.Title = "Import from...";
            if (ofd.ShowDialog() == DialogResult.OK) {
                var sb = new StringBuilder();
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(ofd.FileName);

                try {
                    if (xmlDocument.FirstChild.Name == GetType().Name) {
                        string errorMessage;
                        LoadFromXml(xmlDocument, out errorMessage);
                        sb.Append(errorMessage);
                        if (errorMessage.Length == 0) {
                            ResolveBranchedIndices();
                            InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);
                        }
                    } else {
                        MessageBox.Show("This xml file contains no valid structure to be loaded.", string.Empty,
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        return;
                    }
                } catch {
                    sb.Append("Unknown or non-existing item;");
                }
                if (sb.ToString().Length > 0) {
                    string s = "Failed loading: " + sb;
                    LogWrapper.LogByLevel(s, LogLevel.Error);
                    MessageBox.Show(s, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error,
                                    MessageBoxDefaultButton.Button1);
                }
            }
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
                    if (clipboardString.StartsWith('<' + GetType().Name)) {
                        var xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(clipboardString);

                        if (xmlDocument.ChildNodes.Count > 0) {
                            string errorMessage;
                            if (xmlDocument.FirstChild.Name == GetType().Name)
                                LoadFromXml(xmlDocument, out errorMessage);
                            else
                                return;
                            if (errorMessage.Length == 0) {
                                ResolveBranchedIndices();
                                InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added,
                                                                    true);
                            }
                        }
                    }
                } catch {
                }
            }
        }

        /// <summary>
        ///     An item's dependencies ae set using branched indices, they must be resolved to load the right values from "referenced" objects.
        /// </summary>
        internal void ResolveBranchedIndices() {
            foreach (BaseItem item in this)
                item.ResolveBranchedIndices();
        }

        public override string ToString() { return Name; }
        #endregion
    }
}