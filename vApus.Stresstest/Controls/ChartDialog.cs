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
    public partial class ChartDialog : Form {
        public ChartDialog() {
            InitializeComponent();
        }
        public ChartDialog(Image chart)
            : this() {
            pic.Image = chart;
        }
    }
}
