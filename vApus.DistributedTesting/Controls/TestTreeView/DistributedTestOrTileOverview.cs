/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using BrightIdeasSoftware;
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.DistributedTest.Controls.TestTreeView {
    public partial class DistributedTestOrTileOverview : UserControl {

        #region Fields
        private BaseItem _item;
        private Dictionary<string, int> _connectionStrings = new Dictionary<string, int>();
        private Dictionary<string, int> _monitorStrings = new Dictionary<string, int>();
        private List<object> _connectionsAndMonitors = new List<object>();
        #endregion

        public DistributedTestOrTileOverview() {
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
                if (this.IsHandleCreated && SolutionTree.Solution.ActiveSolution != null
                    && (sender == _item || _connectionsAndMonitors.Contains(sender))) {
                    if (e.__DoneAction == SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Removed)
                        _item = null;
                    Init(_item);
                }
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed refreshing tile overview.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">DistributedTest or Tile</param>
        public void Init(BaseItem item) {
            try {
                LockWindowUpdate(this.Handle);
                _item = item;
                tlvw.Roots = GetOverview();
                tlvw.ExpandAll();
                if (tlvw.Items.Count != 0)
                    tlvw.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

                if (item is DistributedTest) {
                    splitContainer.Panel2Collapsed = false;
                    var tmr = new Timer() {  Interval = 200 };
                    tmr.Tick += tmr_Tick;
                    tmr.Start();
                } else {
                    splitContainer.Panel2Collapsed = true;
                }

            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed initing tile overview.", ex);
            } finally {
                try { LockWindowUpdate(IntPtr.Zero); } catch { }
            }
        }

        private void tmr_Tick(object sender, EventArgs e) {
            (sender as Timer).Stop();
            solutionComponentPropertyPanel.SolutionComponent = _item;
        }

        private List<TLVWItem> GetOverview() {
            _connectionStrings = new Dictionary<string, int>();
            _monitorStrings = new Dictionary<string, int>();
            _connectionsAndMonitors = new List<object>();

            var items = new List<TLVWItem>();
            if (_item == null) return items;

            if (_item is Tile) {
                items = GetTileOverview(_item as Tile);
            } else if (_item is DistributedTest) {
                var test = _item as DistributedTest;
                foreach (Tile tile in test.Tiles) {
                    bool use = tile.Use;
                    if (use || !chkShowOnlyChecked.Checked) {
                        var item = new TLVWItem() { Name = tile.ToString(), Use = use, Children = new List<TLVWItem>() };
                        item.Children.AddRange(GetTileOverview(tile));
                        items.Add(item);
                    }
                }
            }

            return items;
        }

        private List<TLVWItem> GetTileOverview(Tile tile) {
            var items = new List<TLVWItem>();
            if (tile == null) return items;

            foreach (TileStressTest ts in tile) {
                bool use = ts.Use;
                if (use || !chkShowOnlyChecked.Checked) {
                    var basic = ts.BasicTileStressTest;
                    _connectionsAndMonitors.Add(basic.Connection);

                    string connectionString = basic.Connection.ConnectionString.Replace("<16 0C 02 12$>", "•");
                    if (chkShowConnectionStrings.Checked) {
                        if (_connectionStrings.ContainsKey(connectionString))
                            ++_connectionStrings[connectionString];
                        else _connectionStrings.Add(connectionString, 1);
                    } else {
                        connectionString = new string('•', connectionString.Length);
                    }

                    var item = new TLVWItem() { Name = ts.Index.ToString() + ") " + basic.Connection, ConnectionString = connectionString, Use = use, Children = new List<TLVWItem>() };
                    items.Add(item);

                    foreach (Monitor.Monitor monitor in basic.Monitors) {
                        _connectionsAndMonitors.Add(monitor);

                        string monitorString = monitor.ParameterValues.Combine("•");
                        if (chkShowConnectionStrings.Checked) {
                            if (_monitorStrings.ContainsKey(monitorString))
                                ++_monitorStrings[monitorString];
                            else _monitorStrings.Add(monitorString, 1);
                        } else {
                            monitorString = new string('•', monitorString.Length);
                        }

                        item.Children.Add(new TLVWItem() { Name = monitor.ToString(), ConnectionString = monitorString, Use = use, IsMonitor = true });
                    }
                }
            }

            return items;
        }

        private void chk_CheckedChanged(object sender, EventArgs e) { Init(_item); }

        private void tlvw_FormatRow(object sender, FormatRowEventArgs e) {
            try {
                var item = e.Item.RowObject as TLVWItem;

                if (!item.Use) e.Item.ForeColor = Color.Gray;

                int i = 1;
                if (item.ConnectionString != null)
                    if (item.IsMonitor)
                        _monitorStrings.TryGetValue(item.ConnectionString, out i);
                    else
                        _connectionStrings.TryGetValue(item.ConnectionString, out i);

                if (i > 1) e.Item.BackColor = Color.LightYellow;
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed formatting the TreeListView rows.", ex);
            }
        }

        #endregion

        public class TLVWItem {
            public string Name { get; set; }
            public string ConnectionString { get; set; }
            public bool Use { get; set; }
            public bool IsMonitor { get; set; }
            public List<TLVWItem> Children { get; set; }
        }


    }
}
