/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Util
{
    public partial class LocalizationPanel : Panel
    {
        public LocalizationPanel()
        {
            InitializeComponent();
            this.HandleCreated += new EventHandler(LocalizationPanel_HandleCreated);
        }
        private void LocalizationPanel_HandleCreated(object sender, EventArgs e)
        {
            if (cboCulture.Items.Count == 0)
                foreach (CultureInfo info in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                {
                    object o = info.DisplayName;
                    o.SetTag(info);
                    cboCulture.Items.Add(o);
                }

            string culture = global::vApus.Gui.Properties.Settings.Default.Culture;
            if (culture == null || culture == string.Empty)
            {
                culture = Thread.CurrentThread.CurrentCulture.ToString();
                global::vApus.Gui.Properties.Settings.Default.Culture = culture;
                global::vApus.Gui.Properties.Settings.Default.Save();
            }
            foreach (Object o in cboCulture.Items)
                if (o.GetTag().ToString() == culture)
                {
                    cboCulture.SelectedItem = o;
                    break;
                }
        }
        private void btnSet_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = cboCulture.SelectedItem.GetTag() as CultureInfo;
            global::vApus.Gui.Properties.Settings.Default.Culture = Thread.CurrentThread.CurrentCulture.ToString();
            global::vApus.Gui.Properties.Settings.Default.Save();
            btnSet.Enabled = false;
        }
        private void cbo_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSet.Enabled = (cboCulture.SelectedItem.GetTag().ToString() != Thread.CurrentThread.CurrentCulture.ToString());
        }
        public override string ToString()
        {
            return "Localization";
        }
    }
}
