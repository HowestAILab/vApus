/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    /// This can be shown if an application error is repported to the Logger.
    /// </summary>
    public partial class LogErrorToolTip : Form {

        private volatile int _numberOfErrorsOrFatals = 0;

        #region Properties
        /// <summary>
        /// In ms how long this is visible
        /// </summary>
        public int AutoPopDelay {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This can be shown if an application error is repported to the Logger.
        /// </summary>
        public LogErrorToolTip() {
            InitializeComponent();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Gets decremented again when this is hidden.
        /// </summary>
        public void IncrementNumberOfErrorsOrFatals() {
            ++_numberOfErrorsOrFatals;
            lblTitle.Text = _numberOfErrorsOrFatals + ((_numberOfErrorsOrFatals == 1) ? " new error!" : " new errors!");
        }
        protected override void OnPaint(PaintEventArgs e) {
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
        public void Show(Form owner, int x, int y) {
            Show(owner);
            Location = new Point(x, y);
            owner.Select();
        }

        private void LogErrorToolTip_Click(object sender, EventArgs e) {
            Hide();
            if (sender != this) {
                var ctrl = sender as Control;
                ctrl.Click -= LogErrorToolTip_Click;
                this.InvokeOnClick(this, e);
                ctrl.Click += LogErrorToolTip_Click;
            }
        }

        private void timer_Tick(object sender, EventArgs e) {
            Hide();
        }

        private void LogErrorToolTip_VisibleChanged(object sender, EventArgs e) {
            timer.Stop();
            if (Visible) {
                if (Owner != null)
                    Owner.SizeChanged += Owner_SizeChanged;
                timer.Start();
            } else {
                _numberOfErrorsOrFatals = 0;
            }
        }

        private void Owner_SizeChanged(object sender, EventArgs e) {
            Owner.SizeChanged -= Owner_SizeChanged;
            Hide();
        }
        #endregion
    }
}