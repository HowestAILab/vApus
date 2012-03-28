/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 *    Glenn Desmadryl - Expanded with EndDate or Duration (11/10/2011)
 */
using System;
using System.Windows.Forms;
using System.Threading;

namespace vApus.Util
{
    public partial class ExtendedSchedule : Form
    {
        private DateTime _scheduledAt = DateTime.MinValue;
        private TimeSpan _duration = TimeSpan.MinValue;

        /// <summary>
        /// Returns DateTime.Now when not scheduled.
        /// </summary>
        public DateTime ScheduledAt
        {
            get { return _scheduledAt; }
        }
        /// <summary>
        /// Returns new TimeSpan(0) for no duration.
        /// </summary>
        public TimeSpan Duration
        {
            get { return _duration; }
        }
        public ExtendedSchedule()
        {
            InitializeComponent();
        }
        public ExtendedSchedule(DateTime scheduledAt)
            : this()
        {
            rdbLater.Checked = (scheduledAt > dtpTime.Value);
            if (rdbLater.Checked)
            {
                dtpDate.Value = scheduledAt;
                dtpTime.Value = scheduledAt;
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            _scheduledAt = (rdbNow.Checked) ? DateTime.Now : (dtpDate.Value.Date + dtpTime.Value.TimeOfDay);
            if (_scheduledAt < DateTime.Now)
                _scheduledAt = DateTime.Now;

            _duration = (chkDuration.Checked) ? dtpDuration.Value.TimeOfDay : new TimeSpan(0);
            DialogResult = DialogResult.OK;
        }
        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            rdbLater.Checked = true;
        }
        private void dtpDuration_ValueChanged(object sender, EventArgs e)
        {
            chkDuration.Checked = true;
        }
    }
}