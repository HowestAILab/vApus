/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

namespace vApus.DistributedTest {
    public interface ITreeViewItem {
        /// </summary>
        void SetVisibleControls();

        /// <summary>
        ///     Determine yourself what must be visible and what not.
        /// </summary>
        /// <param name="visible"></param>
        void SetVisibleControls(bool visible);

        /// <summary>
        ///     Set this if you start or stop the distributed test.
        /// </summary>
        /// <param name="distributedTestMode"></param>
        void SetDistributedTestMode(DistributedTestMode distributedTestMode);

        /// <summary>
        ///     Refresh properties from the base items in the gui (Label, Use).
        /// </summary>
        void RefreshGui();

        /// <summary>
        /// </summary>
        void Unfocus();
    }
}