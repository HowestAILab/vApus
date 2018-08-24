/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking {
    /// <summary>
    ///     A workaround for the HideOnClose problem, closing in code results in disposing, which shouldn't.
    /// </summary>
    public class DockablePanel : DockContent {
        private bool _cancelHide = false;

        public void CancelHide() { _cancelHide = true; }

        public DockablePanel() {
            FormClosing += DockablePanel_FormClosing;
        }

        private void DockablePanel_FormClosing(object sender, FormClosingEventArgs e) {
               //Also subscribe to this event on the owner form, to ensure it is closed when closing it (this will cancel it if the dockstate is set to document).
            //In that case, set e.cancel to false and dispose this.
            e.Cancel = true;
            if (_cancelHide) {
                _cancelHide = false;
                return;
            }
            Hide();
        }
    }
}