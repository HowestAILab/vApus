/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Publish {
    /// <summary>
    ///     Adds description and tags to the results database. Do not forget to set ResultsHelper.
    /// </summary>
    public partial class DescriptionAndTagsInputDialog : Form {

        #region Fields
        private string _description;
        private string[] _tags;
        #endregion

        #region Properties
        public string Description {
            get { return _description; }
            set {
                if (value.Length == 0)
                    FocusDescription();
                else {
                    _description = value;
                    txtDescription.Text = _description;
                }
            }
        }
        public string[] Tags {
            get { return _tags; }
            set {
                if (value.Length != 0) {
                    _tags = value;
                    txtTags.Text = _tags.Combine(", ");
                }
            }
        }

        public bool AutoConfirm { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        ///     Adds description and tags to the results database. Do not forget to set ResultsHelper.
        /// </summary>
        public DescriptionAndTagsInputDialog() {
            InitializeComponent();

            this.VisibleChanged += DescriptionAndTagsInputDialog_VisibleChanged;
        }
        #endregion

        #region Functions
        //Check connectivity to the publisher.
        async void DescriptionAndTagsInputDialog_VisibleChanged(object sender, EventArgs e) {
            this.VisibleChanged -= DescriptionAndTagsInputDialog_VisibleChanged;

            if (Publisher.Settings.PublisherEnabled) {

                bool connected = await Task.Run(() => Publisher.Poll());

                if (!connected) {
                    Loggers.Log(Level.Warning, "Could not connect to the publish items handler.", null, new object[] { sender, e });
                    lblCouldNotConnect.Visible = true;
                }
            }
            else {
                lblCouldNotConnect.Visible = true;
            }

            btnOK.Enabled = true;
            btnOK.Text = "OK";

            if (AutoConfirm) btnOK_Click(btnOK, null);
        }

        private void FocusDescription() {
            var t = new Thread(delegate() {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                    try {
                        txtDescription.Focus();
                    } catch {
                        //Don't care.
                    }
                }, null);
            });
            t.IsBackground = true;
            t.Start();
        }

        private void btnOK_Click(object sender, EventArgs e) {
            _description = txtDescription.ForeColor == Color.DimGray ? string.Empty : txtDescription.Text.Trim();
            string tagstring = txtTags.ForeColor == Color.DimGray ? string.Empty : txtTags.Text.Trim();
            var tags = new List<string>();
            foreach (string tag in tagstring.Split(',')) {
                string t = tag.Trim();
                if (!tags.Contains(t)) tags.Add(t);
            }

            _tags = tags.ToArray();

            DialogResult = DialogResult.OK;
            try { Close(); } catch {
                //Only fails on gui closed.
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        #endregion
    }
}