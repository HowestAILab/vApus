using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace vApus.Stresstest {
    public partial class EditLog : UserControl {
        #region Fields
        private Log _log;
        private static string[] _defaultDeny = { "facebook.com", "google-analytics.com", "linkedin.com", "twimg.com", "twitter.com", "youtube.com" };
        #endregion

        public EditLog() {
            InitializeComponent();
        }

        internal void SetLog(Log log) {
            tmrRemoveEmptyCells.Stop();
            _log = log;
            chkAllow.Checked = _log.UseAllow;
            chkDeny.Checked = _log.UseDeny;

            foreach (string s in _log.Allow) dgvAllow.Rows.Add(s);

            var deny = _log.Deny.Length == 0 ? _defaultDeny : _log.Deny;
            foreach (string s in deny) dgvDeny.Rows.Add(s);
            tmrRemoveEmptyCells.Start();
        }

        private void chkAllow_CheckedChanged(object sender, EventArgs e) {
            dgvAllow.Enabled = chkAllow.Checked;
            dgvAllow.DefaultCellStyle.ForeColor = dgvAllow.ColumnHeadersDefaultCellStyle.ForeColor = dgvAllow.Enabled ? SystemColors.ControlText : SystemColors.ControlDarkDark;

            if (tmrRemoveEmptyCells.Enabled) {
                _log.UseAllow = chkAllow.Checked;
                _log.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }
        private void chkDeny_CheckedChanged(object sender, EventArgs e) {
            dgvDeny.Enabled = chkDeny.Checked;
            dgvDeny.DefaultCellStyle.ForeColor = dgvDeny.ColumnHeadersDefaultCellStyle.ForeColor = dgvDeny.Enabled ? SystemColors.ControlText : SystemColors.ControlDarkDark;

            if (tmrRemoveEmptyCells.Enabled) {
                _log.UseDeny = chkDeny.Checked;
                _log.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }
        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            var dgv = sender as DataGridView;
            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[0];
            IPAddress ip;
            try {
                if (!IPAddress.TryParse(cell.Value.ToString(), out ip))
                    Dns.GetHostEntry(cell.Value.ToString());
            } catch {
                cell.Value = null;
            }

            if (tmrRemoveEmptyCells.Enabled) {
                string[] arr = new string[dgv.Rows.Count - 1];
                for (int i = 0; i != dgv.RowCount - 1; i++)
                    if (dgv.Rows[i].Cells[0].Value != null)
                        arr[i] = dgv.Rows[i].Cells[0].Value as string;

                if (sender == dgvAllow) _log.Allow = arr; else _log.Deny = arr;
                _log.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }
        private void tmrRemoveEmptyCells_Tick(object sender, EventArgs e) {
            try {
                if (!dgvAllow.IsCurrentCellInEditMode)
                    for (int i = 0; i != dgvAllow.RowCount - 1; i++)
                        if (dgvAllow.Rows[i].Cells[0].Value == null)
                            dgvAllow.Rows.RemoveAt(i);

                if (!dgvDeny.IsCurrentCellInEditMode)
                    for (int i = 0; i != dgvDeny.RowCount - 1; i++)
                        if (dgvDeny.Rows[i].Cells[0].Value == null)
                            dgvDeny.Rows.RemoveAt(i);

                if (dgvDeny.Rows.Count <= 1) {
                    tmrRemoveEmptyCells.Stop();
                    foreach (string s in _defaultDeny) dgvDeny.Rows.Add(s);
                    tmrRemoveEmptyCells.Start();
                }

            } catch { }
        }
    }
}
