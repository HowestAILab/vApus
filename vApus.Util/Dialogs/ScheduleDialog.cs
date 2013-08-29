/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    /// If you want to be able to set a fixed duration, use ExtendedSchedule.
    /// </summary>
    public partial class ScheduleDialog : Form {
        private DateTime _scheduledAt = DateTime.MinValue;

        public DateTime ScheduledAt { get { return _scheduledAt; } }

        public ScheduleDialog() { InitializeComponent(); }

        public ScheduleDialog(DateTime scheduledAt)
            : this() {
            rdbLater.Checked = (scheduledAt > dtpTime.Value);
            if (rdbLater.Checked)
                dtpDate.Value = dtpTime.Value = scheduledAt;
        }

        private void dtp_ValueChanged(object sender, EventArgs e) { rdbLater.Checked = true; }

        private void btnOK_Click(object sender, EventArgs e) {
            _scheduledAt = (rdbNow.Checked) ? DateTime.Now : (dtpDate.Value.Date + dtpTime.Value.TimeOfDay);
            if (ScheduledAt < DateTime.Now) _scheduledAt = DateTime.Now;

            DialogResult = DialogResult.OK;
        }

    }
}