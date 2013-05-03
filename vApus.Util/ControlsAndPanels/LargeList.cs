/*
 * Copyright (C) 2008 Dieter Vandroemme  <dieter.vandroemme@gmail.com>
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 *
 * This file is available with multiple licenses, with the given permission of the original author, Dieter Vandroemme.
 * The user is free to choose the license he/she wants to use, as long as all the above copyright messages
 * and this disclaimer are included.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace vApus.Util {

    #region Enums

    /// <summary>
    /// </summary>
    public enum Hotkeys {
        None = 0,
        Ctrl,
        Shift
    }

    /// <summary>
    ///     The sizemode of a control in the LargeList.
    /// </summary>
    public enum SizeMode {
        Normal = 0,

        /// <summary>
        ///     Stretch the control horizontal en vertical.
        /// </summary>
        Stretch,
        StretchHorizontal,
        StretchVertical
    }

    #endregion

    /// <summary>
    ///     This control is actualy an advanced pager which can contain 134 217 728 controls.
    ///     Most common functionality is implemented and a few other features.
    /// </summary>
    public partial class LargeList : UserControl, IEnumerable<List<Control>> {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        #region Events

        /// <summary>
        ///     Occurs after a new view is loaded in de Gui.
        /// </summary>
        public event EventHandler<AfterSwithViewsEventArgs> AfterViewSwitch;

        /// <summary>
        ///     Occurs when the selection of controls changes.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        /// <summary>
        /// </summary>
        public event EventHandler ControlCollectionChanged;

        #endregion

        #region Fields

        /// <summary>
        ///     The maximum capacity for one list of controls. The maximum assignable memory is 2^32 bit, the size of a pointer is 32 bit.
        ///     This is also the maximum controls the largelist can contain, to make operations like sorting easier. On the other hand, having a List<List
        ///     <Control>
        ///         > makes the logic easier.
        ///         This is still a max of 134 217 728 controls.
        /// </summary>
        private const int MAXCAPACITY = int.MaxValue / 32;

        private readonly object _lock = new object();

        /// <summary>
        ///     A list with the scrollvalue to know which value has wich view.
        /// </summary>
        private readonly List<int> _scrollValues = new List<int>();

        private Control _activeControl;

        private KeyValuePair<int, int> _beginOfSelection = new KeyValuePair<int, int>(-1, -1);

        /// <summary>
        ///     Extra variable for performance.
        /// </summary>
        private int _controlCount;

        /// <summary>
        ///     A list of lists with all the controls, one list per view.
        /// </summary>
        private List<List<Control>> _controls = new List<List<Control>>();

        private int _currentView;

        /// <summary>
        ///     The height for drawing the controls.
        /// </summary>
        private int _drawHeight;

        private KeyValuePair<int, int> _endOfSelection = new KeyValuePair<int, int>(-1, -1);

        private Point _firstSelectionPoint;
        private Control _lastClickedControl;
        private Point _lastSelectionPoint;

        /// <summary>
        ///     The previous height for the resizing, if it is equal, nothing must be done.
        /// </summary>
        private int _previousHeight;

        /// <summary>
        ///     Selection
        /// </summary>
        private List<Control> _selection = new List<Control>();

        /// <summary>
        ///     Sizing the controls
        /// </summary>
        private SizeMode _sizeMode;

        //For making sure stuff happens correctly after the handle is created.
        private List<Control> _toAdd = new List<Control>();
        private int _totalControlsHeightsInLastView;

        /// <summary>
        ///     To let stuff wait on each other, because stuff could go wrong because of the Application.DoEvents.
        /// </summary>
        private AutoResetEvent _waitHandle = new AutoResetEvent(true);

        #endregion

        #region Properties

        /// <summary>
        ///     Returns the number of views.
        /// </summary>
        public int ViewCount {
            get { return _controls.Count; }
        }

        /// <summary>
        ///     Returns the number of controls.
        /// </summary>
        public int ControlCount {
            get { return _controlCount; }
        }

        /// <summary>
        ///     The active control if any.
        /// </summary>
        public new Control ActiveControl {
            get { return _activeControl; }
        }

        public Control LastClickedControl {
            get { return _lastClickedControl; }
        }

        /// <summary>
        ///     The total selection.
        /// </summary>
        public List<Control> Selection {
            get { return _selection; }
        }

        /// <summary>
        ///     Gets the view(key) and index(value) of the first control in the selection.
        /// </summary>
        public KeyValuePair<int, int> BeginOfSelection {
            get { return _beginOfSelection; }
        }

        /// <summary>
        ///     Gets the view(key) and index(value) of the last control in the selection.
        /// </summary>
        public KeyValuePair<int, int> EndOfSelection {
            get { return _endOfSelection; }
        }

        /// <summary>
        ///     The current view.
        /// </summary>
        public int CurrentView {
            get { return _currentView; }
        }

        /// <summary>
        ///     The number of controls in the current view.
        /// </summary>
        public int NumberOfControlsInCurrentView {
            get { return flpnl.Controls.Count; }
        }

        /// <summary>
        ///     Sets or gets a view.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public List<Control> this[int view] {
            get { return _controls[view]; }
            set {
                _controls[view] = value;
                RefreshControls();
            }
        }

        /// <summary>
        ///     Gets or Sets the sizemode for the controls who aren't autosizing.
        /// </summary>
        public SizeMode SizeMode {
            get { return _sizeMode; }
            set {
                if (value != _sizeMode) {
                    _sizeMode = value;
                    foreach (var controls in _controls)
                        foreach (Control control in controls)
                            if (!control.AutoSize)
                                switch (_sizeMode) {
                                    case SizeMode.Normal:
                                        break;
                                    case SizeMode.Stretch:
                                        control.Size = new Size(
                                            flpnl.Width - control.Margin.Left - control.Margin.Right,
                                            flpnl.Height - control.Margin.Top - control.Margin.Bottom);
                                        break;
                                    case SizeMode.StretchHorizontal:
                                        control.Size = new Size(
                                            flpnl.Width - control.Margin.Left - control.Margin.Right, control.Height);
                                        break;
                                    case SizeMode.StretchVertical:
                                        control.Size = new Size(control.Width,
                                                                flpnl.Height - control.Margin.Top -
                                                                control.Margin.Bottom);
                                        break;
                                }
                }
            }
        }

        /// <summary>
        ///     Enumerates all controls over all views.
        /// </summary>
        public IEnumerable<Control> AllControls {
            get {
                for (int i = 0; i != ViewCount; i++)
                    foreach (Control ctrl in _controls[i])
                        yield return ctrl;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        ///     This control is actualy an advanced pager which can contain 134 217 728 controls.
        ///     Most common functionality is implemented and a few other features.
        /// </summary>
        public LargeList() {
            InitializeComponent();
            flpnl.MouseDown += flpnl_MouseDown;
            flpnl.MouseUp += flpnl_MouseUp;
        }

        /// <summary>
        ///     This control is actualy an advanced pager which can contain 134 217 728 controls.
        ///     Most common functionality is implemented and a few other features.
        /// </summary>
        /// <param name="controls"></param>
        public LargeList(List<Control> controls)
            : this() {
            if (controls == null)
                throw new ArgumentNullException("controls");
            AddRange(controls);
        }

        /// <summary>
        ///     This control is actualy an advanced pager which can contain 134 217 728 controls.
        ///     Most common functionality is implemented and a few other features.
        /// </summary>
        /// <param name="controls"></param>
        public LargeList(List<List<Control>> controls)
            : this() {
            if (controls == null)
                throw new ArgumentNullException("controls");
            AddRange(controls);
        }

        #endregion

        #region Functions

        #region IEnumerable

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<List<Control>> GetEnumerator() {
            return _controls.GetEnumerator();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return _controls.GetEnumerator();
        }

        #endregion

        #region Add, Insert, Remove,...

        private void LargeList_HandleCreated(object sender, EventArgs e) {
            HandleCreated -= LargeList_HandleCreated;
            lock (_lock) {
                if (_toAdd != null) {
                    AddRange(_toAdd);
                    _toAdd = null;
                }
            }
        }

        /// <summary>
        ///     Adds a control.
        /// </summary>
        /// <param name="control"></param>
        public void Add(Control control) {
            if (IsHandleCreated)
                Add(control, true);
            else {
                _toAdd.Add(control);
                HandleCreated += LargeList_HandleCreated;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="refresh"></param>
        public void Add(Control control, bool refresh) {
            if (_controlCount == MAXCAPACITY)
                throw new OverflowException("The maximum capacity is 134 217 728.");

            if (_controls.Count == 0) {
                _controls.Add(new List<Control>());
                _scrollValues.Add(0);
                scrollbar.Maximum = _drawHeight;
            }

            //Do not calculate this if it will not be shown.
            if (control.Visible) {
                int controlHeight = control.Margin.Top + control.Margin.Bottom;

                //Set the size of the controls if neccessary and append the total heigth off the controls accordanly to the way of sizing.
                if (control.AutoSize)
                    controlHeight += control.PreferredSize.Height;
                else
                    controlHeight += (_sizeMode == SizeMode.Stretch || _sizeMode == SizeMode.StretchVertical)
                                         ? flpnl.Height - control.Margin.Top - control.Margin.Bottom
                                         : control.Height;

                _totalControlsHeightsInLastView += controlHeight;

                if (_totalControlsHeightsInLastView >= _drawHeight) {
                    _totalControlsHeightsInLastView = controlHeight;

                    //Add a new if the first list is filled in, this is the case when following condition is met.
                    if (_controlCount != 0) {
                        _controls.Add(new List<Control>());
                        scrollbar.Maximum += _drawHeight;
                        _scrollValues.Add(scrollbar.Maximum - _drawHeight);
                    }
                }
            }

            if (_controls.Count == _currentView + 1) {
                LockWindowUpdate(Handle.ToInt32());
                SizeControl(control);
                flpnl.Controls.Add(control);
                LockWindowUpdate(0);
            }

            lblTotalViews.Text = _controls.Count.ToString();
            pnl.Visible = lblTotalViews.Text != "1";
            _controls[_controls.Count - 1].Add(control);
            ++_controlCount;

            if (refresh && ControlCollectionChanged != null)
                ControlCollectionChanged.Invoke(this, null);
        }

        /// <summary>
        ///     Size according to the size mode if the AutoSize property == false, do this when adding a control to the flpnl.
        /// </summary>
        /// <param name="control"></param>
        private void SizeControl(Control control) {
            if (!control.AutoSize) {
                int width, height;
                switch (_sizeMode) {
                    case SizeMode.Stretch:
                        width = flpnl.Width - control.Margin.Left - control.Margin.Right - 18;
                        height = flpnl.Height - control.Margin.Top - control.Margin.Bottom;
                        if (control.Width != width && control.Height != height)
                            control.Size = new Size(width, height);
                        break;
                    case SizeMode.StretchHorizontal:
                        width = flpnl.Width - control.Margin.Left - control.Margin.Right - 18;
                        if (control.Width != width)
                            control.Size = new Size(width, control.Height);
                        break;
                    case SizeMode.StretchVertical:
                        height = flpnl.Height - control.Margin.Top - control.Margin.Bottom;
                        if (control.Height != height)
                            control.Size = new Size(control.Width, height);
                        break;
                }
            }
        }

        /// <summary>
        ///     Size according to the size mode, do this when adding a range of controls to the flpnl.
        /// </summary>
        /// <param name="controls"></param>
        private void SizeControls(List<Control> controls) {
            foreach (Control control in controls)
                SizeControl(control);
        }

        /// <summary>
        ///     Adds a range of controls.
        /// </summary>
        /// <param name="controls"></param>
        public void AddRange(List<Control> controls) {
            if (IsHandleCreated)
                AddRange(controls, true);
            else {
                _toAdd.AddRange(controls);
                HandleCreated += LargeList_HandleCreated;
            }
        }

        private void AddRange(List<Control> controls, bool refresh) {
            Cursor = Cursors.WaitCursor;
            foreach (Control control in controls)
                Add(control, false);
            if (refresh && ControlCollectionChanged != null)
                ControlCollectionChanged.Invoke(this, null);
            Cursor = Cursors.Default;
        }

        /// <summary>
        ///     Adds a range of controls.
        /// </summary>
        /// <param name="controls"></param>
        private void AddRange(List<List<Control>> controls) {
            AddRange(controls, true);
        }

        /// <summary>
        ///     Adds a range of controls.
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="refresh"></param>
        private void AddRange(List<List<Control>> controls, bool refresh) {
            foreach (var l in controls)
                AddRange(l, false);
            if (refresh && ControlCollectionChanged != null)
                ControlCollectionChanged.Invoke(this, null);
        }

        /// <summary>
        ///     Clears the controls.
        /// </summary>
        public void Clear() {
            LockWindowUpdate(Handle.ToInt32());

            ClearSelection();
            _controls.Clear();
            _lastClickedControl = null;
            _controlCount = 0;
            flpnl.Controls.Clear();
            _scrollValues.Clear();
            scrollbar.ValueChanged -= scrollbar_ValueChanged;
            scrollbar.Maximum = 0;
            scrollbar.ValueChanged += scrollbar_ValueChanged;
            _totalControlsHeightsInLastView = 0;
            _currentView = 0;
            txtView.Text = "1";
            lblTotalViews.Text = "1";
            pnl.Visible = false;
            lblTotalViews_TextChanged(null, null);
            if (ControlCollectionChanged != null)
                ControlCollectionChanged.Invoke(this, null);

            LockWindowUpdate(0);
        }

        /// <summary>
        ///     Returns if the LargeList contains the given control.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public new bool Contains(Control control) {
            foreach (var controls in _controls)
                if (controls.Contains(control))
                    return true;
            return false;
        }

        /// <summary>
        ///     Go from a flat index to a usable index for the largelist (handy for inserting / removing controls and all that)
        /// </summary>
        /// <param name="index"></param>
        /// <returns>kvp(-1,-1) if not found.</returns>
        public KeyValuePair<int, int> ParseFlatIndex(long index) {
            for (int view = 0; view != ViewCount; view++) {
                int countOfView = this[view].Count;
                if (countOfView > index)
                    return new KeyValuePair<int, int>(view, (int)index);

                index -= countOfView;
            }
            return new KeyValuePair<int, int>(-1, -1);
        }

        /// <summary>
        ///     Go from a usable index to a flat one, you can increment this for instance and parse it to a usable one again.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public long ParseIndex(KeyValuePair<int, int> index) {
            long flatIndex = 0;
            if (index.Key <= ViewCount) {
                for (int view = 0; view != ViewCount; view++)
                    if (view == index.Key) {
                        if (this[view].Count > index.Value)
                            flatIndex += index.Value;
                        else
                            return -1;
                    } else {
                        flatIndex += this[view].Count;
                    }
            } else {
                return -1;
            }

            return flatIndex;
        }

        /// <summary>
        ///     Gets the view(index) and the index(value) in view of the given control. -1 for both if not found.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public KeyValuePair<int, int> IndexOf(Control control) {
            for (int i = 0; i < _controls.Count; i++)
                for (int j = 0; j < _controls[i].Count; j++)
                    if (_controls[i][j] == control)
                        return new KeyValuePair<int, int>(i, j);
            return new KeyValuePair<int, int>(-1, -1);
            ;
        }

        public int FlatIndexOf(Control control) {
            int index = -1;
            for (int i = 0; i < _controls.Count; i++)
                for (int j = 0; j < _controls[i].Count; j++) {
                    ++index;
                    if (_controls[i][j] == control)
                        return index;
                }
            return -1;
        }

        /// <summary>
        ///     Inserts a control in a specified view(key) and at a specified index(value) in the view.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="?"></param>
        public void Insert(Control control, KeyValuePair<int, int> index) {
            Insert(control, index, true);
        }

        /// <summary>
        ///     Inserts a control in a specified view(key) and at a specified index(value) in the view.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="index"></param>
        /// <param name="refresh"></param>
        public void Insert(Control control, KeyValuePair<int, int> index, bool refresh) {
            _controls[index.Key].Insert(index.Value, control);
            switch (refresh) {
                case true:
                    RefreshControls();
                    if (ControlCollectionChanged != null)
                        ControlCollectionChanged.Invoke(this, null);
                    break;
                default:
                    ++_controlCount;
                    break;
            }
        }

        /// <summary>
        ///     Inserts a range of controls in a specified view(key) and at a specified index(value) in the view.
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="index"></param>
        public void InsertRange(List<Control> controls, KeyValuePair<int, int> index) {
            int lastIndex = controls.Count - 1;
            for (int i = lastIndex; i >= 0; i--)
                Insert(controls[i], index, false);
            RefreshControls();
            if (ControlCollectionChanged != null)
                ControlCollectionChanged.Invoke(this, null);
        }

        /// <summary>
        ///     Returns the previous control if any, else null is returned.
        /// </summary>
        /// <param name="control"></param>
        public Control PreviousControl(Control control) {
            if (!Contains(control))
                throw new Exception("Control not found");
            KeyValuePair<int, int> index = IndexOf(control);
            if (index.Value != 0)
                return _controls[index.Key][index.Value - 1];
            else if (index.Key != 0) {
                int previousView = index.Key - 1;
                return _controls[previousView][_controls[previousView].Count - 1];
            }
            return null;
        }

        /// <summary>
        ///     Returns the previous control if any, else null is returned.
        /// </summary>
        /// <param name="control"></param>
        public Control NextControl(Control control) {
            if (!Contains(control))
                throw new Exception("Control not found");
            KeyValuePair<int, int> index = IndexOf(control);
            if (index.Value != _controls[index.Key].Count - 1)
                return _controls[index.Key][index.Value + 1];
            else if (index.Key != _controls.Count - 1) {
                return _controls[index.Key + 1][0];
            }
            return null;
        }

        /// <summary>
        ///     Puts one control above another.
        /// </summary>
        /// <param name="control1"></param>
        /// <param name="control2"></param>
        public void PutControl1AboveControl2(Control control1, Control control2) {
            PutControl1AboveOrBelowControl2(control1, control2, true, true);
        }

        /// <summary>
        ///     Puts one control above another.
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        public void PutControl1AboveControl2(KeyValuePair<int, int> index1, KeyValuePair<int, int> index2) {
            PutControl1AboveControl2(_controls[index1.Key][index1.Value], _controls[index2.Key][index2.Value]);
        }

        /// <summary>
        ///     Puts one control below another.
        /// </summary>
        /// <param name="control1"></param>
        /// <param name="control2"></param>
        public void PutControl1BelowControl2(Control control1, Control control2) {
            PutControl1AboveOrBelowControl2(control1, control2, false, true);
        }

        /// <summary>
        ///     Puts one control below another.
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        public void PutControl1BelowControl2(KeyValuePair<int, int> index1, KeyValuePair<int, int> index2) {
            PutControl1BelowControl2(_controls[index1.Key][index1.Value], _controls[index2.Key][index2.Value]);
        }

        private void PutControl1AboveOrBelowControl2(Control control1, Control control2, bool above, bool refresh) {
            //Control can't be put above itself
            if (control1 != control2) {
                Control activeControl = _activeControl;
                var temp = new List<List<Control>>(_controls.Count);
                foreach (var controls in _controls) {
                    var tempPart = new List<Control>(controls.Count);
                    temp.Add(tempPart);
                    foreach (Control control in controls) {
                        if (control == control2) {
                            switch (above) {
                                case true:
                                    tempPart.Add(control1);
                                    tempPart.Add(control2);
                                    break;
                                default:
                                    tempPart.Add(control2);
                                    tempPart.Add(control1);
                                    break;
                            }
                        } else if (control != control1) {
                            tempPart.Add(control);
                        }
                    }
                }
                _controls = temp;
                if (refresh) {
                    RefreshControls();
                    if (ControlCollectionChanged != null)
                        ControlCollectionChanged.Invoke(this, null);
                }
                _activeControl = activeControl;
            }
        }

        /// <summary>
        ///     Puts a range of controls above a specified control.
        /// </summary>
        /// <param name="range">Can be for instance 'LargeList.Selection'</param>
        /// <param name="control"></param>
        public void PutRangeAboveControl(List<Control> range, Control control) {
            PutRangeAboveOrBelowControl(range, control, true, true);
        }

        /// <summary>
        ///     Puts a range of controls above a specified control.
        /// </summary>
        /// <param name="beginRangeIndex"></param>
        /// <param name="endRangeIndex"></param>
        /// <param name="controlIndex"></param>
        public void PutRangeAboveControl(KeyValuePair<int, int> beginRangeIndex, KeyValuePair<int, int> endRangeIndex,
                                         KeyValuePair<int, int> controlIndex) {
            PutRangeAboveControl(GetRange(beginRangeIndex, endRangeIndex),
                                 _controls[controlIndex.Key][controlIndex.Value]);
        }

        /// <summary>
        ///     Puts a range of controls below a specified control.
        /// </summary>
        /// <param name="range">Can be for instance 'LargeList.Selection'</param>
        /// <param name="control"></param>
        public void PutRangeBelowControl(List<Control> range, Control control) {
            PutRangeAboveOrBelowControl(range, control, false, true);
        }

        /// <summary>
        ///     Puts a range of controls below a specified control.
        /// </summary>
        /// <param name="beginRangeIndex"></param>
        /// <param name="endRangeIndex"></param>
        /// <param name="controlIndex"></param>
        public void PutRangeBelowControl(KeyValuePair<int, int> beginRangeIndex, KeyValuePair<int, int> endRangeIndex,
                                         KeyValuePair<int, int> controlIndex) {
            PutRangeBelowControl(GetRange(beginRangeIndex, endRangeIndex),
                                 _controls[controlIndex.Key][controlIndex.Value]);
        }

        private void PutRangeAboveOrBelowControl(List<Control> range, Control control, bool above, bool refresh) {
            if (range.Count == 1) {
                PutControl1AboveOrBelowControl2(range[0], control, above, refresh);
            } else {
                if (!range.Contains(control)) {
                    //Sort on keys and values.
                    OrderRange(range);
                    //Move the controls
                    var temp = new List<List<Control>>(_controls.Count);
                    int lastIndex = -1;
                    Control activecontrol = _activeControl;
                    foreach (var controls in _controls) {
                        temp.Add(new List<Control>(controls.Count));
                        ++lastIndex;
                        foreach (Control c in controls) {
                            if (control == c) {
                                switch (above) {
                                    case true:
                                        AddRange(temp, range);
                                        temp[lastIndex].Add(c);
                                        break;
                                    default:
                                        temp[lastIndex].Add(c);
                                        AddRange(temp, range);
                                        break;
                                }
                            } else if (!range.Contains(c)) {
                                temp[lastIndex].Add(c);
                            }
                        }
                    }
                    _controls = temp;
                    if (refresh) {
                        RefreshControls();
                        if (ControlCollectionChanged != null)
                            ControlCollectionChanged.Invoke(this, null);
                    }
                    _activeControl = control;
                }
            }
        }

        private void AddRange(List<List<Control>> controls, List<Control> range) {
            int lastIndex = controls.Count - 1;
            foreach (Control control in range)
                controls[lastIndex].Add(control);
        }

        /// <summary>
        ///     To get a range of controls.
        /// </summary>
        /// <param name="beginRangeIndex"></param>
        /// <param name="endRangeIndex"></param>
        /// <returns></returns>
        public List<Control> GetRange(KeyValuePair<int, int> beginRangeIndex, KeyValuePair<int, int> endRangeIndex) {
            Cursor = Cursors.WaitCursor;
            var range = new List<Control>();
            if (beginRangeIndex.Key > endRangeIndex.Key) {
                KeyValuePair<int, int> temp = beginRangeIndex;
                beginRangeIndex = endRangeIndex;
                endRangeIndex = temp;
            }
            if (beginRangeIndex.Key == endRangeIndex.Key) {
                if (beginRangeIndex.Value > endRangeIndex.Value) {
                    KeyValuePair<int, int> temp = beginRangeIndex;
                    beginRangeIndex = endRangeIndex;
                    endRangeIndex = temp;
                }
                for (int value = beginRangeIndex.Value; value <= endRangeIndex.Value; value++)
                    range.Add(_controls[beginRangeIndex.Key][value]);
            } else if (beginRangeIndex.Key < endRangeIndex.Key)
                for (int key = beginRangeIndex.Key; key <= endRangeIndex.Key; key++)
                    if (key == beginRangeIndex.Key)
                        for (int value = beginRangeIndex.Value; value < _controls[beginRangeIndex.Key].Count; value++)
                            range.Add(_controls[key][value]);
                    else if (key == endRangeIndex.Key)
                        for (int value = 0; value <= endRangeIndex.Value; value++)
                            range.Add(_controls[key][value]);
                    else
                        for (int value = 0; value < _controls[key].Count; value++)
                            range.Add(_controls[key][value]);

            Cursor = Cursors.Default;
            return range;
        }

        public void RefreshControls() {
            Cursor = Cursors.WaitCursor;
            int previousScrollValue = scrollbar.Value;
            var copy = new List<List<Control>>(_controls);
            _controls.Clear();
            _controlCount = 0;
            flpnl.Controls.Clear();
            _scrollValues.Clear();
            _totalControlsHeightsInLastView = 0;

            scrollbar.ValueChanged -= scrollbar_ValueChanged;
            AddRange(copy);
            scrollbar.Maximum = _controls.Count * _drawHeight;
            scrollbar.LargeChange = _drawHeight;
            scrollbar.SmallChange = _drawHeight;
            if (previousScrollValue > scrollbar.Maximum)
                previousScrollValue = scrollbar.Maximum;
            scrollbar.Value = previousScrollValue;
            scrollbar.ValueChanged += scrollbar_ValueChanged;

            if (_currentView > 0) {
                _currentView = -1;
                scrollbar_ValueChanged(null, null);
            }
            lblTotalViews.Text = _controls.Count == 0 ? "1" : _controls.Count.ToString();
            pnl.Visible = lblTotalViews.Text != "1";
            OrderSelection();
            Cursor = Cursors.Default;
        }

        /// <summary>
        ///     Removes a control.
        /// </summary>
        /// <param name="control"></param>
        public void Remove(Control control) {
            Remove(control, true);
        }

        /// <summary>
        ///     Removes a control.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="refresh"></param>
        public void Remove(Control control, bool refresh) {
            int key = -1;
            for (int i = 0; i < _controls.Count; i++)
                if (_controls[i].Contains(control)) {
                    _controls[i].Remove(control);
                    key = i;
                    break;
                }
            if (control == _activeControl)
                _activeControl = null;
            if (control == _lastClickedControl)
                _lastClickedControl = null;
            if (key > -1)
                switch (refresh) {
                    case true:
                        RefreshControls();
                        if (ControlCollectionChanged != null)
                            ControlCollectionChanged.Invoke(this, null);
                        break;
                    default:
                        --_controlCount;
                        break;
                }
        }

        /// <summary>
        ///     Removes all controls.
        /// </summary>
        public void RemoveAll() {
            Clear();
        }

        /// <summary>
        ///     Removes a control at a specified index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(KeyValuePair<int, int> index) {
            RemoveAt(index, true);
        }

        /// <summary>
        ///     Removes a control at a specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="refresh"></param>
        private void RemoveAt(KeyValuePair<int, int> index, bool refresh) {
            Control control = _controls[index.Key][index.Value];
            if (control == _activeControl)
                _activeControl = null;
            if (control == _lastClickedControl)
                _lastClickedControl = null;
            _controls[index.Key].RemoveAt(index.Value);
            switch (refresh) {
                case true:
                    RefreshControls();
                    if (ControlCollectionChanged != null)
                        ControlCollectionChanged.Invoke(this, null);
                    break;
                default:
                    --_controlCount;
                    break;
            }
        }

        /// <summary>
        ///     Removes a range of controls.
        /// </summary>
        /// <param name="range"></param>
        public void RemoveRange(List<Control> range) {
            foreach (Control control in range)
                Remove(control, false);
            RefreshControls();
            if (ControlCollectionChanged != null)
                ControlCollectionChanged.Invoke(this, null);
        }

        /// <summary>
        ///     Removes a range of controls.
        /// </summary>
        /// <param name="beginIndex"></param>
        /// <param name="endIndex"></param>
        public void RemoveRange(KeyValuePair<int, int> beginIndex, KeyValuePair<int, int> endIndex) {
            RemoveRange(GetRange(beginIndex, endIndex));
        }

        /// <summary>
        ///     Removes the selected controls.
        /// </summary>
        public void RemoveSelection() {
            if (_selection.Count > 0) {
                if (_selection.Count == _controlCount) {
                    Clear();
                } else {
                    Cursor = Cursors.WaitCursor;
                    foreach (Control control in _selection)
                        Remove(control, false);
                    _selection.Clear();
                    RefreshControls();
                    if (ControlCollectionChanged != null)
                        ControlCollectionChanged.Invoke(this, null);
                    Cursor = Cursors.Default;
                }
            }
        }

        /// <summary>
        ///     Scroll to a view.
        /// </summary>
        /// <param name="view"></param>
        public void ScrollTo(int view) {
            if (view != -1)
                scrollbar.Value = _scrollValues[view];
        }

        /// <summary>
        ///     Scroll a control into view.
        /// </summary>
        /// <param name="control"></param>
        public void ScrollIntoView(Control control) {
            ScrollTo(IndexOf(control).Key);
        }

        /// <summary>
        /// </summary>
        /// <param name="sortBy">Multiple values can be add up using a binairy or</param>
        public void Sort(SortBy sortBy) {
            Sort(sortBy, SortOrder.Ascending);
        }

        /// <summary>
        /// </summary>
        /// <param name="sortBy">Multiple values can be add up using a binairy or</param>
        /// <param name="sortOrder"></param>
        public void Sort(SortBy sortBy, SortOrder sortOrder) {
            if (sortOrder == SortOrder.None)
                return;
            Sort(new ControlComparer(sortBy, sortOrder));
        }

        /// <summary>
        ///     Sorts all controls using your own comparer. Tip: take a look at ControlComparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(IComparer<Control> comparer) {
            if (_controlCount < 2)
                return;
            Cursor = Cursors.WaitCursor;
            var toSort = new List<Control>(_controlCount);
            foreach (var controls in _controls)
                foreach (Control control in controls)
                    toSort.Add(control);

            //This is a quicksort, in many cases the fastest for large collections.
            toSort.Sort(comparer);
            Clear();
            AddRange(toSort);
            Cursor = Cursors.Default;
        }

        /// <summary>
        ///     Swaps one control with an other.
        /// </summary>
        /// <param name="control1"></param>
        /// <param name="control2"></param>
        public void Swap(Control control1, Control control2) {
            Swap(control1, control2, true);
        }

        /// <summary>
        ///     Swaps one control with an other.
        /// </summary>
        /// <param name="control1"></param>
        /// <param name="control2"></param>
        /// <param name="refresh"></param>
        private void Swap(Control control1, Control control2, bool refresh) {
            if (control1 != control2) {
                KeyValuePair<int, int> index1 = IndexOf(control1);
                KeyValuePair<int, int> index2 = IndexOf(control2);

                _controls[index1.Key][index1.Value] = control2;
                _controls[index2.Key][index2.Value] = control1;
                if (refresh) {
                    RefreshControls();
                    if (ControlCollectionChanged != null)
                        ControlCollectionChanged.Invoke(this, null);
                }
            }
        }

        /// <summary>
        ///     Swaps one control with an other.
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        public void Swap(KeyValuePair<int, int> index1, KeyValuePair<int, int> index2) {
            Swap(index1, index2, true);
        }

        /// <summary>
        ///     Swaps one control with an other.
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <param name="refresh"></param>
        private void Swap(KeyValuePair<int, int> index1, KeyValuePair<int, int> index2, bool refresh) {
            if (index1.Key != index2.Key || (index1.Key == index2.Key && index1.Value != index2.Value)) {
                Control control1 = _controls[index1.Key][index1.Value];
                Control control2 = _controls[index2.Key][index2.Value];

                _controls[index1.Key][index1.Value] = control2;
                _controls[index2.Key][index2.Value] = control1;
                if (refresh) {
                    RefreshControls();
                    if (ControlCollectionChanged != null)
                        ControlCollectionChanged.Invoke(this, null);
                }
            }
        }

        #endregion

        #region Selections

        /// <summary>
        ///     Clears the selection.
        ///     Visualization of selection you must do yourself (use the SelectionCHanged event).
        /// </summary>
        public void ClearSelection() {
            _selection.Clear();
            _activeControl = null;
            if (_lastClickedControl != null && _selection.Contains(_lastClickedControl))
                _lastClickedControl = null;
            _beginOfSelection = new KeyValuePair<int, int>(-1, -1);
            _endOfSelection = new KeyValuePair<int, int>(-1, -1);
            InvokeSelectionChanged();
        }

        /// <summary>
        ///     Order the selection like it is in the controls.
        ///     Begin and end of selection will be stored.
        ///     Visualization of selection you must do yourself (use the SelectionCHanged event).
        /// </summary>
        public void OrderSelection() {
            OrderRange(_selection);
            if (_selection.Count != 0) {
                _beginOfSelection = IndexOf(_selection[0]);
                _endOfSelection = IndexOf(_selection[_selection.Count - 1]);
            } else {
                _beginOfSelection = new KeyValuePair<int, int>(-1, -1);
                _endOfSelection = new KeyValuePair<int, int>(-1, -1);
            }
        }

        private void OrderRange(List<Control> range) {
            var indices = new Dictionary<int, List<int>>();
            foreach (Control control in range) {
                KeyValuePair<int, int> index = IndexOf(control);
                if (index.Key > -1 && index.Value > -1) {
                    if (!indices.ContainsKey(index.Key))
                        indices.Add(index.Key, new List<int>());
                    indices[index.Key].Add(index.Value);
                }
            }
            range.Clear();
            var sortedKeys = new List<int>();
            foreach (var subIndices in indices.Values)
                subIndices.Sort();
            foreach (int key in indices.Keys)
                sortedKeys.Add(key);
            sortedKeys.Sort();
            foreach (int key in sortedKeys)
                foreach (int value in indices[key])
                    range.Add(_controls[key][value]);
        }

        /// <summary>
        ///     Visualization of selection you must do yourself (use the SelectionCHanged event).
        /// </summary>
        /// <param name="beginIndex"></param>
        /// <param name="endIndex"></param>
        public void SelectRange(KeyValuePair<int, int> beginIndex, KeyValuePair<int, int> endIndex) {
            SelectRange(GetRange(beginIndex, endIndex));
        }

        /// <summary>
        ///     Visualization of selection you must do yourself (use the SelectionCHanged event).
        /// </summary>
        /// <param name="controls"></param>
        public void SelectRange(List<Control> controls) {
            Cursor = Cursors.WaitCursor;
            _selection.Clear();
            _activeControl = null;
            foreach (Control control in controls)
                _selection.Add(control);
            if (_selection.Count > 0) {
                _activeControl = _selection[0];
                _beginOfSelection = IndexOf(_activeControl);
                _endOfSelection = IndexOf(_selection[_selection.Count - 1]);
            }
            if (_selection.Count != 0) {
                var ctrl = _selection[0];
                ctrl.Focus();
                ctrl.Select();
            } InvokeSelectionChanged();
            Cursor = Cursors.Default;
        }

        /// <summary>
        ///     Selects a control.
        ///     Visualization of selection you must do yourself (use the SelectionCHanged event).
        /// </summary>
        /// <param name="control"></param>
        public void Select(Control control, Hotkeys hotkeys = Hotkeys.None) {
            if (control != null)
                SelectControl(control, hotkeys);
        }

        /// <summary>
        ///     Selects a control.
        ///     Visualization of selection you must do yourself (use the SelectionCHanged event).
        /// </summary>
        /// <param name="control"></param>
        /// <param name="hotkeys"></param>
        /// <param name="contains"></param>
        private void SelectControl(Control control, Hotkeys hotkeys) {
            /// Selects a control.
            _lastClickedControl = control;
            if (hotkeys == Hotkeys.Ctrl) {
                CtrlPressed(control);
            } else if (hotkeys == Hotkeys.Shift && _selection.Count > 0) {
                ShiftPressed(control);
            } else {
                _selection.Clear();
                _selection.Add(control);
                _activeControl = control;
                _beginOfSelection = IndexOf(control);
                _endOfSelection = _beginOfSelection;
            }
            if (_selection.Count != 0) {
                var ctrl = _selection[0];
                ctrl.Focus();
                ctrl.Select();
            }
            InvokeSelectionChanged();
        }

        /// <summary>
        ///     Selects a control by its view(key) and index(value) in view.
        ///     Visualization of selection you must do yourself (use the SelectionCHanged event).
        /// </summary>
        /// <param name="index"></param>
        /// <param name="hotkeys"></param>
        public void Select(KeyValuePair<int, int> index, Hotkeys hotkeys = Hotkeys.None) {
            SelectControl(_controls[index.Key][index.Value], hotkeys);
        }

        /// <summary>
        ///     Adds to, substract from selection.
        /// </summary>
        /// <param name="control"></param>
        private void CtrlPressed(Control control) {
            try {
                if (_selection.Contains(control)) {
                    _selection.Remove(control);
                    if (_selection.Count == 0) {
                        _activeControl = null;
                        _beginOfSelection = new KeyValuePair<int, int>(-1, -1);
                        _endOfSelection = new KeyValuePair<int, int>(-1, -1);
                    } else {
                        OrderSelection();
                        _activeControl = control;
                        KeyValuePair<int, int> index = IndexOf(_selection[0]);
                        if (index.Key < _beginOfSelection.Key ||
                            (index.Key == _beginOfSelection.Key && index.Value < _beginOfSelection.Value))
                            _beginOfSelection = index;

                        index = IndexOf(_selection[_selection.Count - 1]);
                        if (index.Key > _endOfSelection.Key ||
                            (index.Key == _endOfSelection.Key && index.Value > _endOfSelection.Value))
                            _endOfSelection = index;
                    }
                } else {
                    _selection.Add(control);
                    KeyValuePair<int, int> index = IndexOf(control);
                    if (index.Key < _beginOfSelection.Key ||
                        (index.Key == _beginOfSelection.Key && index.Value < _beginOfSelection.Value))
                        _beginOfSelection = index;
                    if (index.Key > _endOfSelection.Key ||
                        (index.Key == _endOfSelection.Key && index.Value > _endOfSelection.Value))
                        _endOfSelection = index;
                    _activeControl = control;
                }
            } catch {
            }
        }

        /// <summary>
        ///     Selects a range of controls.
        /// </summary>
        /// <param name="control"></param>
        private void ShiftPressed(Control control) {
            try {
                KeyValuePair<int, int> beginIndex = IndexOf(_activeControl);
                KeyValuePair<int, int> endIndex = IndexOf(control);

                _selection.Clear();

                if (beginIndex.Key > endIndex.Key ||
                    (beginIndex.Key == endIndex.Key && beginIndex.Value > endIndex.Value)) {
                    KeyValuePair<int, int> index = beginIndex;
                    beginIndex = endIndex;
                    endIndex = index;
                }
                _beginOfSelection = beginIndex;
                _endOfSelection = endIndex;
                _selection = GetRange(beginIndex, endIndex);
                _activeControl = control;
            } catch {
            }
        }

        /// <summary>
        ///     Selects all controls.
        ///     Visualization of selection you must do yourself (use the SelectionCHanged event).
        /// </summary>
        public void SelectAll() {
            _selection.Clear();
            int lastIndex;
            foreach (var controls in _controls)
                foreach (Control control in controls)
                    _selection.Add(control);
            if (_selection.Count > 0) {
                _activeControl = _selection[0];
                _beginOfSelection = new KeyValuePair<int, int>(0, 0);
                lastIndex = _controls.Count - 1;
                List<Control> last = _controls[lastIndex];
                _endOfSelection = new KeyValuePair<int, int>(lastIndex, last.Count - 1);
                if (_selection.Count != 0) {
                    var ctrl = _selection[0];
                    ctrl.Focus();
                    ctrl.Select();
                } InvokeSelectionChanged();
            }
        }

        /// <summary>
        ///     Returns if the selection contains the given control.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public bool SelectionContains(Control control) {
            return _selection.Contains(control);
        }

        private void InvokeSelectionChanged() {
            if (SelectionChanged != null)
                if (Contains(_activeControl))
                    SelectionChanged.Invoke(this, new SelectionChangedEventArgs(_activeControl, _lastClickedControl));
                else
                    SelectionChanged.Invoke(this, new SelectionChangedEventArgs(null, _lastClickedControl));
        }

        #endregion

        #region EventHandling

        #region This

        /// <summary>
        ///     For the selectionbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void flpnl_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                _firstSelectionPoint = e.Location;
                _lastSelectionPoint = e.Location;
            }
        }

        /// <summary>
        ///     For the selectionbox.
        ///     ///
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        private Rectangle GetNormalizedRectangle(Point point1, Point point2) {
            if (point1.X < point2.X)
                if (point1.Y < point2.Y)
                    return new Rectangle(point1.X, point1.Y, point2.X - point1.X, point2.Y - point1.Y);
                else
                    return new Rectangle(point1.X, point2.Y, point2.X - point1.X, point1.Y - point2.Y);
            else if (point1.Y < point2.Y)
                return new Rectangle(point2.X, point1.Y, point1.X - point2.X, point2.Y - point1.Y);
            else
                return new Rectangle(point2.X, point2.Y, point1.X - point2.X, point1.Y - point2.Y);
        }

        /// <summary>
        ///     For the selectionbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void flpnl_MouseUp(object sender, MouseEventArgs e) {
            flpnl.Invalidate();
        }

        /// <summary>
        ///     Redetermines the views on resize when needed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            PerformResize();
        }

        /// <summary>
        ///     Used internally for the resizing of this control, usable externally after setting the visibility of controls.
        /// </summary>
        /// <param name="force">Forces the rebuilding of the views even if the size has not changed.</param>
        public void PerformResize(bool force = false) {
            if (Height != 0) {
                int previousVSBValue = scrollbar.Value;
                _drawHeight = flpnl.Height;
                if (_controls.Count > 0 && _controls[0].Count > 0) {
                    Cursor = Cursors.WaitCursor;

                    if (force || Height != _previousHeight) {
                        string previousView = txtView.Text;
                        var copy = new List<List<Control>>(_controls);
                        _controls.Clear();
                        _controls.Add(new List<Control>());

                        flpnl.Controls.Clear();

                        _scrollValues.Clear();
                        _scrollValues.Add(0);
                        scrollbar.Maximum = _drawHeight;
                        _totalControlsHeightsInLastView = 0;
                        _controlCount = 0;
                        AddRange(copy, false);
                        _currentView = -1;

                        int activeControlView = IndexOf(_activeControl).Key;
                        txtView.Text = activeControlView == -1 ? previousView : (activeControlView + 1).ToString();
                    } else {
                        foreach (var controls in _controls)
                            foreach (Control control in controls) {
                                switch (_sizeMode) {
                                    case SizeMode.Stretch:
                                        control.Size =
                                            new Size(flpnl.Width - control.Margin.Left - control.Margin.Right - 18,
                                                     flpnl.Height - control.Margin.Top - control.Margin.Bottom);
                                        break;
                                    case SizeMode.StretchHorizontal:
                                        control.Size =
                                            new Size(flpnl.Width - control.Margin.Left - control.Margin.Right - 18,
                                                     control.Height);
                                        break;
                                }
                            }
                    }
                    txtView_KeyPress(null, new KeyPressEventArgs('\r'));
                    Cursor = Cursors.Default;
                }
                scrollbar.Maximum = _controls.Count * _drawHeight;
                scrollbar.LargeChange = _drawHeight;
                scrollbar.SmallChange = _drawHeight;
                _previousHeight = Height;
                lblTotalViews_TextChanged(null, null);
            }
        }

        /// <summary>
        ///     For using the arrows, for selecting next and previous control.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            switch (keyData) {
                case Keys.Home:
                    scrollbar.Value = _scrollValues[0];
                    ScrollIntoView(_controls[0][0]);
                    break;
                case Keys.End:
                    scrollbar.Value = _scrollValues[_scrollValues.Count - 1];
                    int view = _controls.Count - 1;
                    int indexInView = _controls[view].Count - 1;
                    ScrollIntoView(_controls[view][indexInView]);
                    break;
                case Keys.Left:
                    if (_currentView > 0)
                        scrollbar.Value = _scrollValues[--_currentView];
                    return true;
                case Keys.Right:
                    if (_currentView < ViewCount - 1)
                        scrollbar.Value = _scrollValues[++_currentView];
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #region scrollbar

        /// <summary>
        ///     Select the right view on valuechanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scrollbar_ValueChanged(object sender, EventArgs e) {
            int key = scrollbar.Value;
            if (!_scrollValues.Contains(key)) {
                int below = 0;
                int above = 0;
                foreach (int i in _scrollValues) {
                    if (i < key) {
                        below = i;
                    } else {
                        above = i;
                        break;
                    }
                }
                if (Math.Abs(above - key) < Math.Abs(below - key))
                    key = above;
                else
                    key = below;
                scrollbar.ValueChanged -= scrollbar_ValueChanged;
                if (key > _drawHeight)
                    scrollbar.Value = key;
                else
                    scrollbar.Value = 0;
                scrollbar.ValueChanged += scrollbar_ValueChanged;
            }

            int index = _scrollValues.IndexOf(key);
            if (index != _currentView) {
                flpnl.Controls.Clear();
                txtView.Text = (index + 1).ToString();
                List<Control> controls = _controls[index];
                SizeControls(controls);
                flpnl.Controls.AddRange(controls.ToArray());
                _currentView = index;
                _totalControlsHeightsInLastView = 0;
                foreach (Control control in flpnl.Controls)
                    if (control.AutoSize)
                        _totalControlsHeightsInLastView += control.PreferredSize.Height + control.Margin.Top +
                                                           control.Margin.Bottom;
                    else
                        _totalControlsHeightsInLastView += control.Height + control.Margin.Top + control.Margin.Bottom;
                if (AfterViewSwitch != null)
                    AfterViewSwitch.Invoke(this, new AfterSwithViewsEventArgs(_currentView, flpnl.Controls.Count));
                Thread.Sleep(0);
            }
        }

        #endregion

        #region txt

        /// <summary>
        ///     Select the right view on keypress.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtView_KeyPress(object sender, KeyPressEventArgs e) {
            if (char.IsNumber(e.KeyChar) || e.KeyChar == '\b' || e.KeyChar == '\r' || e.KeyChar == '\n') {
                e.Handled = false;
                if (e.KeyChar == '\r' || e.KeyChar == '\n') {
                    int index = 0;
                    int lastIndex = _controls.Count - 1;
                    if (txtView.Text != "")
                        try {
                            index = int.Parse(txtView.Text) - 1;
                        } catch {
                            index = lastIndex;
                        }
                    if (index >= _controls.Count)
                        index = lastIndex;
                    else if (index < 0) {
                        index = 0;
                        txtView.Text = "1";
                    }
                    if (index != _currentView) {
                        flpnl.Controls.Clear();
                        txtView.Text = (index + 1).ToString();
                        List<Control> controls = _controls[index];
                        SizeControls(controls);
                        flpnl.Controls.AddRange(controls.ToArray());
                        scrollbar.ValueChanged -= scrollbar_ValueChanged;
                        scrollbar.Value = _scrollValues[index];
                        scrollbar.ValueChanged += scrollbar_ValueChanged;
                        _currentView = index;
                        _totalControlsHeightsInLastView = 0;
                        foreach (Control control in flpnl.Controls)
                            _totalControlsHeightsInLastView += control.Height + control.Margin.Top +
                                                               control.Margin.Bottom;
                        if (AfterViewSwitch != null)
                            AfterViewSwitch.Invoke(this,
                                                   new AfterSwithViewsEventArgs(_currentView, flpnl.Controls.Count));
                    }
                }
            } else {
                e.Handled = true;
            }
        }

        #endregion

        #region lblTotalViews

        /// <summary>
        ///     To resize the selection controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblTotalViews_TextChanged(object sender, EventArgs e) {
            int lastLocationX = lblTotalViews.Location.X;
            lblTotalViews.Location = new Point(pnl.Width - lblTotalViews.Width, lblTotalViews.Location.Y);
            lblSlash.Location = new Point(lblTotalViews.Location.X - lblSlash.Width + 3, lblSlash.Location.Y);
            txtView.Location = new Point(lblSlash.Location.X - txtView.Width + 2, txtView.Location.Y);
            txtView.Width = lblTotalViews.Width;
            scrollbar.Width = txtView.Location.X - 1;
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }

    #region EventArgs

    /// <summary>
    ///     Occurs after switched view.
    /// </summary>
    public class AfterSwithViewsEventArgs : EventArgs {
        private readonly int _currentView;
        private readonly int _numberOfControlsInCurrentView;

        /// <summary>
        ///     Occurs after switched view.
        /// </summary>
        /// <param name="currentView"></param>
        /// <param name="controlsOnView"></param>
        public AfterSwithViewsEventArgs(int currentView, int numberOfControlsInCurrentView) {
            if (currentView < 0)
                throw new ArgumentOutOfRangeException("currentView");
            if (numberOfControlsInCurrentView < 0)
                throw new ArgumentOutOfRangeException("numberOfControlsInCurrentView");
            _currentView = currentView;
            _numberOfControlsInCurrentView = numberOfControlsInCurrentView;
        }

        /// <summary>
        /// </summary>
        public int CurrentView {
            get { return _currentView; }
        }

        /// <summary>
        /// </summary>
        public int NumberOfControlsInCurrentView {
            get { return _numberOfControlsInCurrentView; }
        }
    }

    /// <summary>
    /// </summary>
    public class SelectionChangedEventArgs : EventArgs {
        /// <summary>
        /// </summary>
        private readonly Control _activeControl;

        /// <summary>
        /// </summary>
        private readonly Control _lastClickedControl;

        /// <summary>
        /// </summary>
        /// <param name="activeControl"></param>
        public SelectionChangedEventArgs(Control activeControl) {
            _activeControl = activeControl;
        }

        /// <summary>
        ///     Is only used when the user selects a control.
        /// </summary>
        /// <param name="activeControl"></param>
        /// <param name="lastClickedControl">If this is not equal to ActiveControl that means this control has been deselected.</param>
        public SelectionChangedEventArgs(Control activeControl, Control lastClickedControl)
            : this(activeControl) {
            _lastClickedControl = lastClickedControl;
        }

        /// <summary>
        ///     The first control in the selection or the last selected  (LastClickedControl).
        /// </summary>
        public Control ActiveControl {
            get { return _activeControl; }
        }

        /// <summary>
        ///     If this is not equal to ActiveControl that means this control has been deselected.
        /// </summary>
        public Control LastClickedControl {
            get { return _lastClickedControl; }
        }
    }

    #endregion
}