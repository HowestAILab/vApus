/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Windows.Forms;

namespace vApus.Util
{
    public partial class Schedule : Form
    {
        private DateTime _scheduledAt;

        public Schedule()
        {
            InitializeComponent();
        }

        public Schedule(DateTime scheduledAt)
            : this()
        {
            rdbLater.Checked = (scheduledAt > dtpTime.Value);
            if (rdbLater.Checked)
            {
                dtpDate.Value = scheduledAt;
                dtpTime.Value = scheduledAt;
            }
        }

        public DateTime ScheduledAt
        {
            get { return _scheduledAt; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _scheduledAt = (rdbNow.Checked) ? DateTime.Now : (dtpDate.Value.Date + dtpTime.Value.TimeOfDay);
            if (_scheduledAt < DateTime.Now)
                _scheduledAt = DateTime.Now;

            DialogResult = DialogResult.OK;
        }

        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            rdbLater.Checked = true;
        }
    }
}