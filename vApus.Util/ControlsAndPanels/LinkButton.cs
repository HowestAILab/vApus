﻿/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Util {
    public class LinkButton : LinkLabel {
        [Description("Use this rather than LinkClicked, Click or KeyDown.")]
        public event EventHandler ActiveChanged;

        private bool _active, _forceInvokeEvent;

        #region Properties
        public bool Active {
            get { return _active; }
            set {
                if (_active != value) {
                    _active = value;
                    SetStateInGui();
                }
            }
        }

        [Description("Must be set to true or false for all LinkButtons in the Parent. This behaviour is only applied when clicking or pushing the enter key on the control.")]
        public bool RadioButtonBehavior { get; set; }
        #endregion

        public LinkButton() {
            this.HandleCreated += LinkButton_HandleCreated;
        }

        #region Functions
        private void LinkButton_HandleCreated(object sender, EventArgs e) {
            TextAlign = ContentAlignment.TopCenter;
            Padding = new Padding(3, 4, 3, 3);
            AutoSize = true;
            Font = new Font(Font, FontStyle.Bold);
            SetStateInGui();
        }
        private void SetStateInGui() {
            if (_active) {
                BorderStyle = BorderStyle.FixedSingle;
                LinkColor = ActiveLinkColor = VisitedLinkColor = ForeColor = Color.Black;
                LinkBehavior = LinkBehavior.NeverUnderline;
            } else {
                BorderStyle = BorderStyle.None;
                LinkColor = ActiveLinkColor = VisitedLinkColor = ForeColor = Color.DimGray;
                LinkBehavior = LinkBehavior.AlwaysUnderline;
            }
        }
        public void PerformClick() {
            _forceInvokeEvent = true;
            OnClick(new EventArgs());
            _forceInvokeEvent = false;
        }
        protected override void OnClick(EventArgs e) {
            base.OnClick(e);
            Activate();
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Enter) Activate();
        }

        private void Activate() {
            bool changed = _forceInvokeEvent ? true : false;
            if (RadioButtonBehavior) {
                if (Parent != null) {
                    var otherActiveLinkButtons = new List<LinkButton>();
                    foreach (Control ctrl in Parent.Controls)
                        if (ctrl != this && ctrl is LinkButton) {
                            var lbtn = ctrl as LinkButton;
                            if (lbtn.Active) otherActiveLinkButtons.Add(lbtn);
                        }

                    if (Active) {
                        if (otherActiveLinkButtons.Count != 0) {
                            Active = false;
                            changed = true;
                        }
                    } else {
                        Active = true;
                        foreach (LinkButton lbtn in otherActiveLinkButtons) lbtn.Active = false;

                        changed = true;
                    }
                }
            } else {
                Active = !Active;
                changed = true;
            }

            if (changed && ActiveChanged != null) ActiveChanged(this, null);
        }
        #endregion
    }
}