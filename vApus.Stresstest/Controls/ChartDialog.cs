/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Stresstest {
    /// <summary>
    /// Shows the image in the original size. A bit like Lightbox.
    /// </summary>
    public partial class ChartDialog : Form {
        public ChartDialog() {
            InitializeComponent();
        }
        public ChartDialog(string title, Image chart)
            : this() {
                Text = title;
            toolTip.SetToolTip(pic, title + "\nClick to close");
            pic.Image = chart;
        }

        private void pic_Click(object sender, System.EventArgs e) {
            this.Close();
        }
    }
}
