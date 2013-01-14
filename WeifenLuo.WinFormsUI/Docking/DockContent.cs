using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    public class DockContent : Form, IDockContent
    {
        private readonly DockContentHandler m_dockHandler;
        [Localizable(true)] [LocalizedCategory("Category_Docking")] [LocalizedDescription("DockContent_TabText_Description")] [DefaultValue(null)] private string m_tabText;

        public DockContent()
        {
            m_dockHandler = new DockContentHandler(this, GetPersistString);
            m_dockHandler.DockStateChanged += DockHandler_DockStateChanged;
            m_dockHandler.DockStateChanging += DockHandler_DockStateChanging;
            //Suggested as a fix by bensty regarding form resize
            ParentChanged += DockContent_ParentChanged;
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_AllowEndUserDocking_Description")]
        [DefaultValue(true)]
        public bool AllowEndUserDocking
        {
            get { return DockHandler.AllowEndUserDocking; }
            set { DockHandler.AllowEndUserDocking = value; }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_DockAreas_Description")]
        [DefaultValue(
            DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom | DockAreas.Document |
            DockAreas.Float)]
        public DockAreas DockAreas
        {
            get { return DockHandler.DockAreas; }
            set { DockHandler.DockAreas = value; }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_AutoHidePortion_Description")]
        [DefaultValue(0.25)]
        public double AutoHidePortion
        {
            get { return DockHandler.AutoHidePortion; }
            set { DockHandler.AutoHidePortion = value; }
        }

        public string TabText
        {
            get { return m_tabText; }
            set { DockHandler.TabText = m_tabText = value; }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_CloseButton_Description")]
        [DefaultValue(true)]
        public bool CloseButton
        {
            get { return DockHandler.CloseButton; }
            set { DockHandler.CloseButton = value; }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_CloseButtonVisible_Description")]
        [DefaultValue(true)]
        public bool CloseButtonVisible
        {
            get { return DockHandler.CloseButtonVisible; }
            set { DockHandler.CloseButtonVisible = value; }
        }


        public DockPanel DockPanel
        {
            get { return DockHandler.DockPanel; }
            set { DockHandler.DockPanel = value; }
        }


        public DockState DockState
        {
            get { return DockHandler.DockState; }
            set { DockHandler.DockState = value; }
        }


        public DockPane Pane
        {
            get { return DockHandler.Pane; }
            set { DockHandler.Pane = value; }
        }


        public bool IsHidden
        {
            get { return DockHandler.IsHidden; }
            set { DockHandler.IsHidden = value; }
        }


        public DockState VisibleState
        {
            get { return DockHandler.VisibleState; }
            set { DockHandler.VisibleState = value; }
        }


        public bool IsFloat
        {
            get { return DockHandler.IsFloat; }
            set { DockHandler.IsFloat = value; }
        }


        public DockPane PanelPane
        {
            get { return DockHandler.PanelPane; }
            set { DockHandler.PanelPane = value; }
        }


        public DockPane FloatPane
        {
            get { return DockHandler.FloatPane; }
            set { DockHandler.FloatPane = value; }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_HideOnClose_Description")]
        [DefaultValue(false)]
        public bool HideOnClose
        {
            get { return DockHandler.HideOnClose; }
            set { DockHandler.HideOnClose = value; }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_ShowHint_Description")]
        [DefaultValue(DockState.Unknown)]
        public DockState ShowHint
        {
            get { return DockHandler.ShowHint; }
            set { DockHandler.ShowHint = value; }
        }


        public bool IsActivated
        {
            get { return DockHandler.IsActivated; }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_TabPageContextMenu_Description")]
        [DefaultValue(null)]
        public ContextMenu TabPageContextMenu
        {
            get { return DockHandler.TabPageContextMenu; }
            set { DockHandler.TabPageContextMenu = value; }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_TabPageContextMenuStrip_Description")]
        [DefaultValue(null)]
        public ContextMenuStrip TabPageContextMenuStrip
        {
            get { return DockHandler.TabPageContextMenuStrip; }
            set { DockHandler.TabPageContextMenuStrip = value; }
        }

        [Localizable(true)]
        [Category("Appearance")]
        [LocalizedDescription("DockContent_ToolTipText_Description")]
        [DefaultValue(null)]
        public string ToolTipText
        {
            get { return DockHandler.ToolTipText; }
            set { DockHandler.ToolTipText = value; }
        }

        public DockContentHandler DockHandler
        {
            get { return m_dockHandler; }
        }

        private void DockContent_ParentChanged(object Sender, EventArgs e)
        {
            if (Parent != null)
                Font = Parent.Font;
        }

        private bool ShouldSerializeTabText()
        {
            return (m_tabText != null);
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected virtual string GetPersistString()
        {
            return GetType().ToString();
        }

        public bool IsDockStateValid(DockState dockState)
        {
            return DockHandler.IsDockStateValid(dockState);
        }

        public new void Activate()
        {
            DockHandler.Activate();
        }

        public new void Hide()
        {
            DockHandler.Hide();
        }

        public new void Show()
        {
            DockHandler.Show();
        }

        public void Show(DockPanel dockPanel)
        {
            DockHandler.Show(dockPanel);
        }

        public void Show(DockPanel dockPanel, DockState dockState)
        {
            DockHandler.Show(dockPanel, dockState);
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")]
        public void Show(DockPanel dockPanel, Rectangle floatWindowBounds)
        {
            DockHandler.Show(dockPanel, floatWindowBounds);
        }

        public void Show(DockPane pane, IDockContent beforeContent)
        {
            DockHandler.Show(pane, beforeContent);
        }

        public void Show(DockPane previousPane, DockAlignment alignment, double proportion)
        {
            DockHandler.Show(previousPane, alignment, proportion);
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")]
        public void FloatAt(Rectangle floatWindowBounds)
        {
            DockHandler.FloatAt(floatWindowBounds);
        }

        public void DockTo(DockPane paneTo, DockStyle dockStyle, int contentIndex)
        {
            DockHandler.DockTo(paneTo, dockStyle, contentIndex);
        }

        public void DockTo(DockPanel panel, DockStyle dockStyle)
        {
            DockHandler.DockTo(panel, dockStyle);
        }

        #region IDockContent Members

        void IDockContent.OnActivated(EventArgs e)
        {
            OnActivated(e);
        }

        void IDockContent.OnDeactivate(EventArgs e)
        {
            OnDeactivate(e);
        }

        #endregion

        #region Events

        private static readonly object DockStateChangedEvent = new object();

        private void DockHandler_DockStateChanging(object sender, CancelEventArgs e)
        {
            if (Visible)
                OnDockStateChanging(e);
        }

        public event EventHandler<CancelEventArgs> DockStateChanging;

        protected virtual void OnDockStateChanging(CancelEventArgs e)
        {
            if (DockStateChanging != null)
                DockStateChanging(this, e);
        }

        private void DockHandler_DockStateChanged(object sender, EventArgs e)
        {
            OnDockStateChanged(e);
        }

        [LocalizedCategory("Category_PropertyChanged")]
        [LocalizedDescription("Pane_DockStateChanged_Description")]
        public event EventHandler DockStateChanged
        {
            add { Events.AddHandler(DockStateChangedEvent, value); }
            remove { Events.RemoveHandler(DockStateChangedEvent, value); }
        }

        protected virtual void OnDockStateChanged(EventArgs e)
        {
            var handler = (EventHandler) Events[DockStateChangedEvent];
            if (handler != null)
                handler(this, e);
        }

        #endregion
    }
}