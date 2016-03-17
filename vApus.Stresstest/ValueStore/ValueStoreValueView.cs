/*
 * Copyright 2016 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vApus.SolutionTree;

namespace vApus.StressTest {
    public partial class ValueStoreValueView : BaseSolutionComponentView {
        private ValueStoreValue _valueStoreValue;

        private ValueStoreValue.ValueStoreValueTypes _previousType;

        /// <summary>
        ///     Designer time constructor.
        /// </summary>
        public ValueStoreValueView() {
            InitializeComponent();
        }
        public ValueStoreValueView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            _valueStoreValue = solutionComponent as ValueStoreValue;
            _previousType = _valueStoreValue.Type;

            InitializeComponent();

            if (IsHandleCreated)
                SetGui();
            else
                HandleCreated += _HandleCreated;
        }

        private void _HandleCreated(object sender, EventArgs e) { SetGui(); }
        private void SetGui() {
            Text = SolutionComponent.ToString();
            solutionComponentPropertyPanel.SolutionComponent = SolutionComponent;

            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;

            if (_valueStoreValue.Parent != null)
                _valueStoreValue.Parent.LockedChanged += valueStore_LockedChanged;

            btnRefresh.PerformClick();
        }

        private void valueStore_LockedChanged(object sender, LockedChangedEventArgs e) {
            try {
                if (solutionComponentPropertyPanel == null && _valueStoreValue.Parent != null) {
                    _valueStoreValue.Parent.LockedChanged -= valueStore_LockedChanged;

                    if (e.Locked) solutionComponentPropertyPanel.Lock();
                    else solutionComponentPropertyPanel.Unlock();
                }
            }
            catch {
                //Don't care.
            }
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender == _valueStoreValue) {
                if (_valueStoreValue.Type != _previousType) {
                    _previousType = _valueStoreValue.Type;
                    solutionComponentPropertyPanel.SetValues(true, solutionComponentPropertyPanel.Values);
                }
                btnRefresh.PerformClick();
            }
        }

        public override void Refresh() {
            base.Refresh();
            SetGui();
            solutionComponentPropertyPanel.Refresh();
        }

        private void btnRefresh_Click(object sender, EventArgs e) {
            var sb = new StringBuilder();
            var values = _valueStoreValue.Values.ToArray();

            if (values.Length == 0) {
                sb.AppendLine("No value set, falling back to the default value \"" + _valueStoreValue.DefaultValue + "\" (without quotes).");

                if (_valueStoreValue.Publish) {
                    sb.AppendLine();
                    sb.AppendLine("Each time the value is set it will be published, if publishing is enabled (see Tools > Options... > Publish values).");
                }
                sb.Append(@"

Usage example in connection proxy code:

  // Reference the ValueStoreValue in a field. Use the label of the ValueStoreValue as variable name.
  ValueStoreValue _myValue = ValueStore.GetValueStoreValue(Value store value label e.g. " + "\"myValue\"" + @");

  var defaultValue = _myValue.DefaultValue;

  // Respect the chosen type, otherwise setting a value will fail.
  // If the value must be unique for each connection, this call will only work in SendAndReceive(...) and sub functions.
  // If not, this will work in every function (e.g. TestConnection(out error)).
  _myValue.Set(" + "\"foobar\"" + @"); 

  // You can get the names of the threads who set a value.
  // 'shared' (without quotes) is returned when not unique for each connection.
  string[] owners = _myValue.GetOwners();

  // Get the 'shared' value or the value set for the current connection or for a chosen one (owner).
  // This value can be used to replace a token (a piece of text for instance " + "\"{myValue}\"" + @") in the request, coming through SendAndReceive(...).
  string foo = _myValue.Get<string>();
  string bar = _myValue.Get<string>(owner e.g. " + "\"vApus Thread Pool Thread #1\"" + ");"
                    );
            }
            else {

                //Sort stuff.
                string prefix = "vApus Thread Pool Thread #";
                var dic = new SortedDictionary<string, object>(KeyComparer.GetInstance());
                KeyValuePair<string, object> kvp;

                for (int i = 0; i != values.Length; i++) {
                    kvp = values[i];
                    dic.Add(kvp.Key.Substring(prefix.Length), kvp.Value);
                }
                dic.CopyTo(values, 0);

                //Print stuff.
                sb.AppendLine(prefix);

                for (int i = 0; i != values.Length - 1; i++) {
                    kvp = values[i];
                    sb.Append("  ");
                    sb.Append(kvp.Key);
                    sb.Append(":\t");
                    sb.AppendLine(kvp.Value.ToString());
                }

                kvp = values.Last();
                sb.Append("  ");
                sb.Append(kvp.Key);
                sb.Append(":\t");
                sb.Append(kvp.Value.ToString());
            }

            fctb.Text = sb.ToString().Trim();
        }

        private class KeyComparer : IComparer<string> {
            private static KeyComparer _instance;

            public static KeyComparer GetInstance() {
                if (_instance == null) _instance = new KeyComparer();
                return _instance;
            }

            private KeyComparer() { }

            public int Compare(string x, string y) {

                int i = int.Parse(x);
                int j = int.Parse(y);

                return i.CompareTo(j);
            }
        }
    }
}
