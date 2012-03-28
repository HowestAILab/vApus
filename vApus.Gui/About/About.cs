/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace vApus.Gui
{
    public partial class About : Form
    {
        #region Fields
        private Font _titleFont;
        private Font _dateFont;
        private Font _itemFont;
        #endregion

        #region Properties
        /// <summary></summary>
        private string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        /// <summary></summary>
        private string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                    return "";
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }
        /// <summary></summary>
        private string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                    return "";
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }
        private string Licenses
        {
            get { return global::vApus.Gui.Properties.Resources.Licenses; }
        }
        #endregion

        public About()
        {
            InitializeComponent();

            lblDescription.Text = AssemblyDescription;

            lblVersion.Text = String.Format("Version {0}", AssemblyVersion);
            lblCopyright.Text = AssemblyCopyright;

            _titleFont = new Font(rtxtHistoryOfChanges.Font, FontStyle.Bold);
            _dateFont = new Font(rtxtHistoryOfChanges.Font, FontStyle.Italic);
            _itemFont = new Font(rtxtHistoryOfChanges.Font, FontStyle.Regular);

            ReadVersionControlIni();

            rtxtLicenses.Text = Licenses;
        }
        
        private void ReadVersionControlIni()
        {
            string ini = Path.Combine(Application.StartupPath, "versioncontrol.ini");
            string line = string.Empty;
            bool versionFound = false, historyFound = false;

            if (File.Exists(ini))
            {
                StreamReader sr = new StreamReader(ini);
                while (sr.Peek() != -1)
                {
                    line = sr.ReadLine().Trim();

                    if (line.Length == 0)
                        continue;

                    switch (line)
                    {
                        case "Version:":
                            versionFound = true;
                            continue;
                        case "HistoryOfChanges:":
                            historyFound = true;
                            continue;
                    }

                    if (historyFound)
                    {
                        FillHistoryOfChanges(line);
                        break;
                    }
                    else if (versionFound)
                    {
                        lblVersion.Text = "Version: " + line;
                        versionFound = false;
                    }
                }
                try { sr.Close(); }
                catch { }
                try { sr.Dispose(); }
                catch { }
                sr = null;
            }
        }
        private void FillHistoryOfChanges(string historyOfChanges)
        {
            List<HistoryPart> parts = new List<HistoryPart>();
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.ReadNode(XmlReader.Create(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(historyOfChanges))));

            //First filling the rtxt and then applying the style
            int previousCaretPosition = 0;
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "t":
                        foreach (XmlNode nn in n.ChildNodes)
                        {
                            switch (nn.Name)
                            {
                                case "d":
                                    rtxtHistoryOfChanges.Text = rtxtHistoryOfChanges.Text + " (" + nn.InnerText + ")" + Environment.NewLine;
                                    parts.Add(new HistoryPart("d", previousCaretPosition, rtxtHistoryOfChanges.Text.Length - previousCaretPosition));
                                    break;
                                default:
                                    if (previousCaretPosition > 0)
                                        rtxtHistoryOfChanges.Text = rtxtHistoryOfChanges.Text + Environment.NewLine + nn.InnerText;
                                    else
                                        rtxtHistoryOfChanges.Text = rtxtHistoryOfChanges.Text + nn.InnerText;
                                    parts.Add(new HistoryPart("t", previousCaretPosition, rtxtHistoryOfChanges.Text.Length - previousCaretPosition));
                                    break;
                            }
                            previousCaretPosition = rtxtHistoryOfChanges.Text.Length;
                            rtxtHistoryOfChanges.Select(rtxtHistoryOfChanges.Text.Length, 0);
                        }
                        break;
                    case "i":
                        rtxtHistoryOfChanges.Text = rtxtHistoryOfChanges.Text + n.InnerText + Environment.NewLine;
                        parts.Add(new HistoryPart("i", previousCaretPosition, rtxtHistoryOfChanges.Text.Length - previousCaretPosition));
                        break;
                }
                previousCaretPosition = rtxtHistoryOfChanges.Text.Length;
            }
            foreach (HistoryPart part in parts)
            {
                rtxtHistoryOfChanges.Select(part.SelectionStart, part.Length);
                switch (part.Type)
                {
                    case "d":
                        rtxtHistoryOfChanges.SelectionFont = _dateFont;
                        rtxtHistoryOfChanges.SelectionColor = Color.Blue;
                        break;
                    case "i":
                        rtxtHistoryOfChanges.SelectionFont = _itemFont;
                        rtxtHistoryOfChanges.SelectionBullet = true;
                        break;
                    default:
                        rtxtHistoryOfChanges.SelectionFont = _titleFont;
                        rtxtHistoryOfChanges.SelectionColor = Color.Green;
                        break;
                }
            }
            rtxtHistoryOfChanges.Select(0, 0);
        }
        private void lblWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.sizingservers.be");
        }
    }
}
