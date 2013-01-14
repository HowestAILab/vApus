/*
 * Copyright 2012 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Gui
{
    public partial class LogErrorToolTip : Form
    {
        public LogErrorToolTip()
        {
            InitializeComponent();
        }

        public int NumberOfErrorsOrFatals
        {
            set { lblTitle.Text = value + ((value == 1) ? " new error or fatal!" : " new errors or fatals!"); }
        }

        /// <summary>
        ///     In ms how long this is visible
        /// </summary>
        public int AutoPopDelay
        {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //Draw border
            e.Graphics.DrawRectangle(SystemPens.ControlDarkDark, 0, 0, Width - 1, Height - 1);
        }

        /// <summary>
        ///     Use this one instead of the ones for form, it selects the owner afterwards.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Show(Form owner, int x, int y)
        {
            Show(owner);
            Location = new Point(x, y);
            owner.Select();
        }

        private void LogErrorToolTip_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Hide();
        }

        private void LogErrorToolTip_VisibleChanged(object sender, EventArgs e)
        {
            timer.Stop();
            if (Visible)
            {
                if (Owner != null)
                    Owner.SizeChanged += Owner_SizeChanged;
                timer.Start();
            }
        }

        private void Owner_SizeChanged(object sender, EventArgs e)
        {
            Owner.SizeChanged -= Owner_SizeChanged;
            Hide();
        }
    }
}