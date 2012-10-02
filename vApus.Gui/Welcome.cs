/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace vApus.Gui
{
    public partial class Welcome : DockablePanel
    {
        private bool _formClosingEventHandlingEnabled = true;

        public Welcome()
        {
            InitializeComponent();

            HandleCreated += new EventHandler(Welcome_HandleCreated);
        }

        private void Welcome_HandleCreated(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "Welcome\\welcome.htm");
            if (File.Exists(path))
                webBrowser.Navigate(path);
        }

        private void Welcome_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_formClosingEventHandlingEnabled)
            {
                //Do not show the next time if you don't want to
                global::vApus.Gui.Properties.Settings.Default.GreetWithWelcomePage =
                     MessageBox.Show("Would you like to hide the welcome page by default?", string.Empty,
                     MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Cancel;

                global::vApus.Gui.Properties.Settings.Default.Save();
            }
        }

        public void DisableFormClosingEventHandling()
        {
            _formClosingEventHandlingEnabled = false;
        }
    }
}
