/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.StressTest {
    /// <summary>
    ///     For debugging purposes only.
    ///     Visualizes the old and new indices of parameters if a parameter is added or removed.
    ///     Don't forget to call VisualizeSynchronization.
    /// </summary>
    public partial class ParameterTokenSynchronizationView : BaseSolutionComponentView {

        #region Constructors
        /// <summary>
        ///     Design time constructor
        /// </summary>
        public ParameterTokenSynchronizationView() { InitializeComponent(); }
        /// <summary>
        ///     Visualizes the old and new indices of parameters if a parameter is added or removed.
        ///     Don't forget to call VisualizeSynchronization.
        /// </summary>
        /// <param name="solutionComponent"></param>
        /// <param name="args"></param>
        public ParameterTokenSynchronizationView(SolutionComponent solutionComponent)
            : base(solutionComponent) { InitializeComponent(); }
        #endregion

        #region Functions
        /// <summary>
        ///     Visualize the synchronization
        /// </summary>
        /// <param name="oldAndNewIndices"></param>
        public void VisualizeSynchronization(Dictionary<BaseParameter, KeyValuePair<int, int>> oldAndNewIndices) {
            lvw.Items.Clear();

            var l = new List<ListViewItem>(oldAndNewIndices.Count);
            foreach (BaseParameter parameter in oldAndNewIndices.Keys) {
                Color color = GetBackColor(parameter);

                var item = new ListViewItem(parameter.ToString());
                item.UseItemStyleForSubItems = false;
                item.Font = new Font(lvw.Font.FontFamily, 10f);
                item.BackColor = color;

                KeyValuePair<int, int> kvp = oldAndNewIndices[parameter];
                var n = new ListViewItem.ListViewSubItem(item, kvp.Key.ToString());
                n.BackColor = color;
                item.SubItems.Add(n);

                n = new ListViewItem.ListViewSubItem(item, kvp.Value.ToString());
                n.Font = new Font(lvw.Font.FontFamily, 12f);
                n.BackColor = color;
                item.SubItems.Add(n);

                l.Add(item);
            }

            l.Reverse();

            lvw.Items.AddRange(l.ToArray());
        }

        /// <summary>
        /// Each type of parameter has another distinctive color.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private Color GetBackColor(BaseParameter parameter) {
            if (parameter is CustomListParameter)
                return Color.LightPink;
            else if (parameter is NumericParameter)
                return Color.LightGreen;
            else if (parameter is TextParameter)
                return Color.LightBlue;
            else if (parameter is CustomRandomParameter)
                return Color.Yellow;
            return Color.Transparent;
        }

        private void ParameterTokenSynchronization_Resize(object sender, EventArgs e) { clmParameter.Width = lvw.ClientSize.Width - clmOld.Width - clmNew.Width - 18; }
        #endregion
    }
}