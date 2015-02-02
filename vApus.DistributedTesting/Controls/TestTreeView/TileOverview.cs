/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Text;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.DistributedTesting.Controls.TestTreeView {
    public partial class TileOverview : UserControl {

        #region Fields
        private Tile _tile;
        private bool _init;
        private string _tileHostsOverrideState = string.Empty;
        #endregion

        public TileOverview() { InitializeComponent(); }

        #region Functions
        public void Init(Tile tile) {
            _init = true;
            _tile = tile;
            if (_tile.UseOverride)
                lbtnOverride.PerformClick();
            else
                lbtnOverview.PerformClick();
            btnApply.Enabled = false;
            _tileHostsOverrideState = GetTileHostsOverride();
            _init = false;
        }

        private string GetTileHosts() {
            var sb = new StringBuilder();

            foreach (TileStresstest ts in _tile) {
                string index = "*" + ts.Index;
                sb.AppendLine(index);

                var basic = ts.BasicTileStresstest;
                string connectionString = basic.Connection.ConnectionString;

                sb.AppendLine(connectionString);

                foreach (Monitor.Monitor monitor in basic.Monitors) {
                    string monitorString = monitor.ParameterValues.Combine("<16 0C 02 12$>");
                    sb.AppendLine(monitorString);
                }
            }

            return sb.ToString().Trim();
        }

        private string GetTileHostsOverride() {
            var sb = new StringBuilder();
            foreach (TileStresstest ts in _tile) {
                string index = "*" + ts.Index;
                sb.AppendLine(index);
                sb.Append(ts.Override);
            }
            return sb.ToString().Trim();
        }
        private void SetTileHostsOverride() {
            string[] perTs = _tileHostsOverrideState.Split(new string[] { "*" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string block in perTs) {
                string[] lines = block.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (TileStresstest ts in _tile) {
                    if (lines[0].Trim() == ts.Index.ToString()) {
                        var sb = new StringBuilder();
                        int i = 1;
                        for (; i < lines.Length - 1; i++) sb.AppendLine(lines[i]);
                        if (i < lines.Length) sb.Append(lines[i]);

                        ts.Override = sb.ToString();
                        break;
                    }
                }
            }
        }

        private void lbtnOverview_ActiveChanged(object sender, EventArgs e) {
            fctxt.Text = GetTileHosts();
            fctxt.ReadOnly = true;
            if (!_init) btnApply.Enabled = true;
        }

        private void lbtnOverride_ActiveChanged(object sender, EventArgs e) {
            fctxt.Text = GetTileHostsOverride();
            fctxt.ReadOnly = false;
            if (!_init) btnApply.Enabled = !string.IsNullOrWhiteSpace(fctxt.Text);
        }

        private void fctxt_TextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
        }
        private void fctxt_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
            if (!_init) {
                if (lbtnOverride.Active) _tileHostsOverrideState = fctxt.Text;
                btnApply.Enabled = true;
            }
        }
        private void btnApply_Click(object sender, EventArgs e) {
            btnApply.Enabled = false;
            _tile.UseOverride = lbtnOverride.Active;
            SetTileHostsOverride();
            _tile.Parent.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited, true);
        }
        #endregion
    }
}
