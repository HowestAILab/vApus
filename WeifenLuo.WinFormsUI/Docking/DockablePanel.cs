/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

namespace WeifenLuo.WinFormsUI.Docking
{
    /// <summary>
    /// A workaround for the HideOnClose problem, closing in code results in disposing, which shouldn't.
    /// </summary>
    public class DockablePanel : DockContent
    {
        public DockablePanel()
        {
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(DockablePanel_FormClosing);
        }

        private void DockablePanel_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            //Also subscribe to this event on the owner form, to ensure it is closed when closing it (this will cancel it if the dockstate is set to document).
            //In that case, set e.cancel to false and dispose this.
            e.Cancel = true;
            this.Hide();
        }
    }
}
