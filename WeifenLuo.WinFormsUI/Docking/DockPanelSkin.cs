using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace WeifenLuo.WinFormsUI.Docking
{

    #region DockPanelSkin classes

    /// <summary>
    ///     The skin to use when displaying the DockPanel.
    ///     The skin allows custom gradient color schemes to be used when drawing the
    ///     DockStrips and Tabs.
    /// </summary>
    [TypeConverter(typeof (DockPanelSkinConverter))]
    public class DockPanelSkin
    {
        public DockPanelSkin()
        {
            AutoHideStripSkin = new AutoHideStripSkin();
            DockPaneStripSkin = new DockPaneStripSkin();
        }

        /// <summary>
        ///     The skin used to display the auto hide strips and tabs.
        /// </summary>
        public AutoHideStripSkin AutoHideStripSkin { get; set; }

        /// <summary>
        ///     The skin used to display the Document and ToolWindow style DockStrips and Tabs.
        /// </summary>
        public DockPaneStripSkin DockPaneStripSkin { get; set; }
    }

    /// <summary>
    ///     The skin used to display the auto hide strip and tabs.
    /// </summary>
    [TypeConverter(typeof (AutoHideStripConverter))]
    public class AutoHideStripSkin
    {
        private TabGradient m_TabGradient;
        private DockPanelGradient m_dockStripGradient;

        public AutoHideStripSkin()
        {
            m_dockStripGradient = new DockPanelGradient();
            m_dockStripGradient.StartColor = SystemColors.ControlLight;
            m_dockStripGradient.EndColor = SystemColors.ControlLight;

            m_TabGradient = new TabGradient();
            m_TabGradient.TextColor = SystemColors.ControlDarkDark;
        }

        /// <summary>
        ///     The gradient color skin for the DockStrips.
        /// </summary>
        public DockPanelGradient DockStripGradient
        {
            get { return m_dockStripGradient; }
            set { m_dockStripGradient = value; }
        }

        /// <summary>
        ///     The gradient color skin for the Tabs.
        /// </summary>
        public TabGradient TabGradient
        {
            get { return m_TabGradient; }
            set { m_TabGradient = value; }
        }
    }

    /// <summary>
    ///     The skin used to display the document and tool strips and tabs.
    /// </summary>
    [TypeConverter(typeof (DockPaneStripConverter))]
    public class DockPaneStripSkin
    {
        private DockPaneStripGradient m_DocumentGradient;
        private DockPaneStripToolWindowGradient m_ToolWindowGradient;

        public DockPaneStripSkin()
        {
            m_DocumentGradient = new DockPaneStripGradient();
            m_DocumentGradient.DockStripGradient.StartColor = SystemColors.Control;
            m_DocumentGradient.DockStripGradient.EndColor = SystemColors.Control;
            m_DocumentGradient.ActiveTabGradient.StartColor = SystemColors.ControlLightLight;
            m_DocumentGradient.ActiveTabGradient.EndColor = SystemColors.ControlLightLight;
            m_DocumentGradient.InactiveTabGradient.StartColor = SystemColors.ControlLight;
            m_DocumentGradient.InactiveTabGradient.EndColor = SystemColors.ControlLight;

            m_ToolWindowGradient = new DockPaneStripToolWindowGradient();
            m_ToolWindowGradient.DockStripGradient.StartColor = SystemColors.ControlLight;
            m_ToolWindowGradient.DockStripGradient.EndColor = SystemColors.ControlLight;

            m_ToolWindowGradient.ActiveTabGradient.StartColor = SystemColors.Control;
            m_ToolWindowGradient.ActiveTabGradient.EndColor = SystemColors.Control;

            m_ToolWindowGradient.InactiveTabGradient.StartColor = Color.Transparent;
            m_ToolWindowGradient.InactiveTabGradient.EndColor = Color.Transparent;
            m_ToolWindowGradient.InactiveTabGradient.TextColor = SystemColors.ControlDarkDark;

            m_ToolWindowGradient.ActiveCaptionGradient.StartColor = SystemColors.GradientActiveCaption;
            m_ToolWindowGradient.ActiveCaptionGradient.EndColor = SystemColors.ActiveCaption;
            m_ToolWindowGradient.ActiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            m_ToolWindowGradient.ActiveCaptionGradient.TextColor = SystemColors.ActiveCaptionText;

            m_ToolWindowGradient.InactiveCaptionGradient.StartColor = SystemColors.GradientInactiveCaption;
            m_ToolWindowGradient.InactiveCaptionGradient.EndColor = SystemColors.GradientInactiveCaption;
            m_ToolWindowGradient.InactiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            m_ToolWindowGradient.InactiveCaptionGradient.TextColor = SystemColors.ControlText;
        }

        /// <summary>
        ///     The skin used to display the Document style DockPane strip and tab.
        /// </summary>
        public DockPaneStripGradient DocumentGradient
        {
            get { return m_DocumentGradient; }
            set { m_DocumentGradient = value; }
        }

        /// <summary>
        ///     The skin used to display the ToolWindow style DockPane strip and tab.
        /// </summary>
        public DockPaneStripToolWindowGradient ToolWindowGradient
        {
            get { return m_ToolWindowGradient; }
            set { m_ToolWindowGradient = value; }
        }
    }

    /// <summary>
    ///     The skin used to display the DockPane ToolWindow strip and tab.
    /// </summary>
    [TypeConverter(typeof (DockPaneStripGradientConverter))]
    public class DockPaneStripToolWindowGradient : DockPaneStripGradient
    {
        public DockPaneStripToolWindowGradient()
        {
            ActiveCaptionGradient = new TabGradient();
            InactiveCaptionGradient = new TabGradient();
        }

        /// <summary>
        ///     The skin used to display the active ToolWindow caption.
        /// </summary>
        public TabGradient ActiveCaptionGradient { get; set; }

        /// <summary>
        ///     The skin used to display the inactive ToolWindow caption.
        /// </summary>
        public TabGradient InactiveCaptionGradient { get; set; }
    }

    /// <summary>
    ///     The skin used to display the DockPane strip and tab.
    /// </summary>
    [TypeConverter(typeof (DockPaneStripGradientConverter))]
    public class DockPaneStripGradient
    {
        public DockPaneStripGradient()
        {
            DockStripGradient = new DockPanelGradient();
            ActiveTabGradient = new TabGradient();
            InactiveTabGradient = new TabGradient();
        }

        /// <summary>
        ///     The gradient color skin for the DockStrip.
        /// </summary>
        public DockPanelGradient DockStripGradient { get; set; }

        /// <summary>
        ///     The skin used to display the active DockPane tabs.
        /// </summary>
        public TabGradient ActiveTabGradient { get; set; }

        /// <summary>
        ///     The skin used to display the inactive DockPane tabs.
        /// </summary>
        public TabGradient InactiveTabGradient { get; set; }
    }

    /// <summary>
    ///     The skin used to display the dock pane tab
    /// </summary>
    [TypeConverter(typeof (DockPaneTabGradientConverter))]
    public class TabGradient : DockPanelGradient
    {
        public TabGradient()
        {
            TextColor = SystemColors.ControlText;
        }

        /// <summary>
        ///     The text color.
        /// </summary>
        [DefaultValue(typeof (SystemColors), "ControlText")]
        public Color TextColor { get; set; }
    }

    /// <summary>
    ///     The gradient color skin.
    /// </summary>
    [TypeConverter(typeof (DockPanelGradientConverter))]
    public class DockPanelGradient
    {
        public DockPanelGradient()
        {
            StartColor = SystemColors.Control;
            EndColor = SystemColors.Control;
            LinearGradientMode = LinearGradientMode.Horizontal;
        }

        /// <summary>
        ///     The beginning gradient color.
        /// </summary>
        [DefaultValue(typeof (SystemColors), "Control")]
        public Color StartColor { get; set; }

        /// <summary>
        ///     The ending gradient color.
        /// </summary>
        [DefaultValue(typeof (SystemColors), "Control")]
        public Color EndColor { get; set; }

        /// <summary>
        ///     The gradient mode to display the colors.
        /// </summary>
        [DefaultValue(LinearGradientMode.Horizontal)]
        public LinearGradientMode LinearGradientMode { get; set; }
    }

    #endregion

    #region Converters

    public class DockPanelSkinConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof (DockPanelSkin))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (destinationType == typeof (String) && value is DockPanelSkin)
            {
                return "DockPanelSkin";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class DockPanelGradientConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof (DockPanelGradient))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (destinationType == typeof (String) && value is DockPanelGradient)
            {
                return "DockPanelGradient";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class AutoHideStripConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof (AutoHideStripSkin))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (destinationType == typeof (String) && value is AutoHideStripSkin)
            {
                return "AutoHideStripSkin";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class DockPaneStripConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof (DockPaneStripSkin))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (destinationType == typeof (String) && value is DockPaneStripSkin)
            {
                return "DockPaneStripSkin";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class DockPaneStripGradientConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof (DockPaneStripGradient))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (destinationType == typeof (String) && value is DockPaneStripGradient)
            {
                return "DockPaneStripGradient";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class DockPaneTabGradientConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof (TabGradient))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (destinationType == typeof (String) && value is TabGradient)
            {
                return "DockPaneTabGradient";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    #endregion
}