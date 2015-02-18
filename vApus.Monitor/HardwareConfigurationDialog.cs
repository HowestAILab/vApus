/*
 * Copyright 2007 (c) Blancke Karen, Cavaliere Leandro, Kets Brecht, Vandroemme Dieter
 * Technical University Kortrijk, department PIH
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using RandomUtils.Log;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using vApus.Util;

namespace vApus.Monitor {
    public partial class HardwareConfigurationDialog : Form {

        #region Fields
        // The XmlDocument where the formatted configuration is loaded into.
        private XmlDocument _hardwareConfiguration;
        #endregion

        #region Constructor
        public HardwareConfigurationDialog(string configuration) {
            InitializeComponent();
            LoadConfiguration(configuration);

            rtxt.DefaultContextMenu(true);
        }
        #endregion

        #region Functions
        private void LoadConfiguration(string configuration) {
            try {
                Cursor = Cursors.WaitCursor;

                var stringReader = new StringReader(configuration);
                _hardwareConfiguration = new XmlDocument();

                try {
                    _hardwareConfiguration.Load(stringReader);
                } catch {
                    throw;
                } finally {
                    stringReader.Close();
                }

                XmlNode firstChild = _hardwareConfiguration.FirstChild;
                if (firstChild.Name == "List") {
                    var sb = new StringBuilder();
                    foreach (XmlNode node in firstChild.ChildNodes)
                        if (node.Name != null && node.NodeType != XmlNodeType.Text) {
                            sb.AppendLine(node.Name);
                            foreach (string line in node.InnerText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)) {
                                sb.Append("  ");
                                sb.AppendLine(line);
                            }
                            sb.AppendLine();
                        }
                    rtxt.Text = sb.ToString().Trim();
                } else {
                    rtxt.Text = configuration;
                }

                Cursor = Cursors.Default;
            } catch (Exception ex) {
                Loggers.Log(Level.Warning, "The configuration is not a wellformed xml.", ex, new object[] { configuration });
                Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e) {
            if (sfd.ShowDialog() == DialogResult.OK)
                _hardwareConfiguration.Save(sfd.FileName);
        }
        #endregion
    }
}