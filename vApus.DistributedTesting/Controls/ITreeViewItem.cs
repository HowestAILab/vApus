/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
namespace vApus.DistributedTesting
{
    public interface ITreeViewItem
    {
        DistributedTestMode DistributedTestMode { get; }

        /// <summary>
        /// Use point to client from the cursor position here or check if the control is selected.
        /// 
        /// Then call SetVisibleControls(bool visible)
        /// </summary>
        void SetVisibleControls();
        /// <summary>
        /// Determine yourself what must be visible and what not.
        /// </summary>
        /// <param name="visible"></param>
        void SetVisibleControls(bool visible);
        void SetDistributedTestMode(DistributedTestMode distributedTestMode);
        /// <summary>
        /// Refresh properties from the base items in the gui (Label, Use).
        /// </summary>
        void RefreshGui();
        /// <summary>
        /// 
        /// </summary>
        void Unfocus();
    }
}
