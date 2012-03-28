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
        public Welcome()
        {
            InitializeComponent();

            HandleCreated += new EventHandler(Welcome_HandleCreated);
        }

        void Welcome_HandleCreated(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "Welcome\\welcome.htm");
            if (File.Exists(path))
                webBrowser.Navigate(path);
        }
    }
}
