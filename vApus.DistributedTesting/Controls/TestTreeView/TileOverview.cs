/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.DistributedTesting.Controls.TestTreeView {
    public partial class TileOverview : UserControl {

        #region Fields
        private Tile _tile;
        #endregion

        public TileOverview() {
            InitializeComponent();
            tlvw.CanExpandGetter = delegate(object x) {
                var item = x as TLVWItem;
                return item.Children != null && item.Children.Count != 0;
            };
            tlvw.ChildrenGetter = delegate(object x) { return (x as TLVWItem).Children; };

            SolutionTree.SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }

        #region Functions
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionTree.SolutionComponentChangedEventArgs e) {
            try {
                if (this.IsHandleCreated && SolutionTree.Solution.ActiveSolution != null && _tile != null && sender == _tile) {
                    if (e.__DoneAction == SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Removed)
                        _tile = null;
                    Init(_tile);
                }
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed refreshing tile overview.", ex);
            }
        }

        public void Init(Tile tile) {
            try {
                LockWindowUpdate(this.Handle);
                _tile = tile;
                tlvw.Roots = GetOverview();
                tlvw.ExpandAll();
                tlvw.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed initing tile overview.", ex);
            } finally {
                try { LockWindowUpdate(IntPtr.Zero); } catch { }
            }
        }

        private List<TLVWItem> GetOverview() {
            var items = new List<TLVWItem>();
            if (_tile == null) return items;

            foreach (TileStresstest ts in _tile) {
                var basic = ts.BasicTileStresstest;
                string connectionString = basic.Connection.ConnectionString.Replace("<16 0C 02 12$>", "•");
                if (!chkShowConnectionStrings.Checked) connectionString = new string('•', connectionString.Length);

                var item = new TLVWItem() { Name = ts.Index.ToString() + ") " + basic.Connection, ConnectionString = connectionString, Children = new List<TLVWItem>() };
                items.Add(item);

                foreach (Monitor.Monitor monitor in basic.Monitors) {
                    string monitorString = monitor.ParameterValues.Combine("•");
                    if (!chkShowConnectionStrings.Checked) monitorString = new string('•', monitorString.Length);

                    item.Children.Add(new TLVWItem() { Name = monitor.ToString(), ConnectionString = monitorString });
                }
            }

            return items;
        }

        #endregion

        public class TLVWItem {
            public string Name { get; set; }
            public string ConnectionString { get; set; }
            public List<TLVWItem> Children { get; set; }
        }

        private void chkShowConnectionStrings_CheckedChanged(object sender, EventArgs e) {
            Init(_tile);
        }
    }
}
