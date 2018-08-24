/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using vApus.Gui.Properties;
using vApus.Util;

namespace vApus.Gui {
    /// <summary>
    /// To set the localization for formatting numbers. DateTime is always formatted using the ISO 8601 standard. Used in vApus.Util.OptionsDialog.
    /// </summary>
    public partial class LocalizationPanel : Panel {

        #region Constructors
        /// <summary>
        /// To set the localization for formatting numbers. DateTime is always formatted using the ISO 8601 standard. Used in vApus.Util.OptionsDialog.
        /// </summary>
        public LocalizationPanel() {
            InitializeComponent();
            HandleCreated += LocalizationPanel_HandleCreated;
        }
        #endregion

        #region Functions
        private void LocalizationPanel_HandleCreated(object sender, EventArgs e) {
            if (cboCulture.Items.Count == 0)
                foreach (CultureInfo info in CultureInfo.GetCultures(CultureTypes.SpecificCultures)) {
                    object o = info.DisplayName;
                    o.SetTag(info);
                    cboCulture.Items.Add(o);
                }

            string culture = Settings.Default.Culture;
            if (culture == null || culture == string.Empty) {
                culture = Thread.CurrentThread.CurrentCulture.ToString();
                Settings.Default.Culture = culture;
                Settings.Default.Save();
            }
            foreach (Object o in cboCulture.Items)
                if (o.GetTag().ToString() == culture) {
                    cboCulture.SelectedItem = o;
                    break;
                }
        }

        private void btnSet_Click(object sender, EventArgs e) {
            var cultureInfo = cboCulture.SelectedItem.GetTag() as CultureInfo;
            //Use ISO 8601 for DateTime formatting.
            cultureInfo.DateTimeFormat.ShortDatePattern = "yyyy'-'MM'-'dd";
            cultureInfo.DateTimeFormat.LongTimePattern = "HH':'mm':'ss'.'fff";
            Thread.CurrentThread.CurrentCulture = cultureInfo;


            Settings.Default.Culture = Thread.CurrentThread.CurrentCulture.ToString();
            Settings.Default.Save();
            btnSet.Enabled = false;
        }

        private void cbo_SelectedIndexChanged(object sender, EventArgs e) {
            btnSet.Enabled = (cboCulture.SelectedItem.GetTag().ToString() != Thread.CurrentThread.CurrentCulture.ToString());
        }

        public override string ToString() {
            return "Localization";
        }
        #endregion
    }
}