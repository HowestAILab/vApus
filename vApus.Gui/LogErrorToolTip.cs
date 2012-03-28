/*
 * Copyright 2012 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using System;
using System.Windows.Forms;
using System.Drawing;

namespace vApus.Gui
{
    public partial class LogErrorToolTip : Form
    {
        public int NumberOfErrorsOrFatals
        {
            set { lblTitle.Text = value + ((value == 1) ? " new error or fatal!" : " new errors or fatals!"); }
        }
        /// <summary>
        /// In ms how long this is visible
        /// </summary>
        public int AutoPopDelay
        {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }

        public LogErrorToolTip()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //Draw border
            e.Graphics.DrawRectangle(SystemPens.ControlDarkDark, 0, 0, this.Width - 1, this.Height - 1);
        }
        /// <summary>
        /// Use this one instead of the ones for form, it selects the owner afterwards.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Show(Form owner, int x, int y)
        {
            this.Show(owner);
            this.Location = new Point(x, y);
            owner.Select();
        }

        private void LogErrorToolTip_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            this.Hide();
        }
        private void LogErrorToolTip_VisibleChanged(object sender, EventArgs e)
        {
            timer.Stop();
            if (this.Visible)
            {
                if (this.Owner != null)
                    Owner.SizeChanged += new EventHandler(Owner_SizeChanged);
                timer.Start();
            }

        }
        private void Owner_SizeChanged(object sender, EventArgs e)
        {
            Owner.SizeChanged -= new EventHandler(Owner_SizeChanged);
            this.Hide();
        }
    }
}
