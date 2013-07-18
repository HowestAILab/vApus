/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace vApus.Util {
    public partial class UpdateNotifierDialog : Form {
        private readonly string _currentChannel;
        private readonly string _currentVersion;
        private readonly string _history;
        private readonly string _newChannel;
        private readonly string _newVersion;
        private Font _dateFont;
        private Font _itemFont;
        private Font _titleFont;

        public UpdateNotifierDialog() {
            InitializeComponent();
        }

        public UpdateNotifierDialog(string currentVersion, string newVersion, string currentChannel, string newChannel,
                                    string history)
            : this() {
            _currentVersion = currentVersion;
            _newVersion = newVersion;
            _currentChannel = currentChannel;
            _newChannel = newChannel;
            _history = history;

            HandleCreated += UpdateNotifierDialog_HandleCreated;
        }

        private void UpdateNotifierDialog_HandleCreated(object sender, EventArgs e) {
            _titleFont = new Font(rtxtHistoryOfChanges.Font, FontStyle.Bold);
            _dateFont = new Font(rtxtHistoryOfChanges.Font, FontStyle.Italic);
            _itemFont = new Font(rtxtHistoryOfChanges.Font, FontStyle.Regular);

            lblVersion.Text = "Version: " + _currentVersion + " --> " + _newVersion;
            lblChannel.Text = "Channel: " + _currentChannel +
                              (_currentChannel == _newChannel ? string.Empty : " --> " + _newChannel);

            FillHistory(_history);
        }

        private void FillHistory(string historyOfChanges) {
            rtxtHistoryOfChanges.Text = string.Empty;

            var parts = new List<HistoryPart>();

            var ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(historyOfChanges));
            XmlReader reader = XmlReader.Create(ms);
            var doc = new XmlDocument();
            XmlNode node = doc.ReadNode(reader);

            //First filling the rtxt and then applying the style
            int previousCaretPosition = 0;
            foreach (XmlNode n in node.ChildNodes) {
                switch (n.Name) {
                    case "t":
                        foreach (XmlNode nn in n.ChildNodes) {
                            switch (nn.Name) {
                                case "d":
                                    rtxtHistoryOfChanges.Text = rtxtHistoryOfChanges.Text + " (" + nn.InnerText + ")" +
                                                                Environment.NewLine;
                                    parts.Add(new HistoryPart("d", previousCaretPosition,
                                                              rtxtHistoryOfChanges.Text.Length - previousCaretPosition));
                                    break;
                                default:
                                    if (previousCaretPosition > 0)
                                        rtxtHistoryOfChanges.Text = rtxtHistoryOfChanges.Text + Environment.NewLine +
                                                                    nn.InnerText;
                                    else
                                        rtxtHistoryOfChanges.Text = rtxtHistoryOfChanges.Text + nn.InnerText;
                                    parts.Add(new HistoryPart("t", previousCaretPosition,
                                                              rtxtHistoryOfChanges.Text.Length - previousCaretPosition));
                                    break;
                            }
                            previousCaretPosition = rtxtHistoryOfChanges.Text.Length;
                            rtxtHistoryOfChanges.Select(rtxtHistoryOfChanges.Text.Length, 0);
                        }
                        break;
                    case "i":
                        rtxtHistoryOfChanges.Text = rtxtHistoryOfChanges.Text + n.InnerText + Environment.NewLine;
                        parts.Add(new HistoryPart("i", previousCaretPosition,
                                                  rtxtHistoryOfChanges.Text.Length - previousCaretPosition));
                        break;
                }
                previousCaretPosition = rtxtHistoryOfChanges.Text.Length;
            }
            foreach (HistoryPart part in parts) {
                rtxtHistoryOfChanges.Select(part.SelectionStart, part.Length);
                switch (part.Type) {
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

        private void btnUpdate_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        public struct HistoryPart {
            public int Length;
            public int SelectionStart;
            public string Type;

            public HistoryPart(string type, int selectionStart, int length) {
                Type = type;
                SelectionStart = selectionStart;
                Length = length;
            }
        }
    }
}