using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vApus.DistributedTesting
{
    public interface ITestTreeViewItem
    {
        /// <summary>
        /// Use point to client from the cursor position here.
        /// </summary>
        void SetVisibleControls();
        /// <summary>
        /// Determine yourself.
        /// </summary>
        /// <param name="visible"></param>
        void SetVisibleControls(bool visible);

        void RefreshGui();
    }
}
