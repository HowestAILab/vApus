using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using vApus.Results;

namespace vApus.Stresstest {
    public partial class ChartsControl : UserControl {
        private ResultsHelper _resultsHelper;
        private ulong[] _stresstestIds = new ulong[0];
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource(); //Cancel refreshing the report.
        
        public ChartsControl() {
            InitializeComponent();
        }

        /// <summary>
        /// Clear before testing.
        /// </summary>
        public void ClearResults() {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            //foreach (var v in _config) flpConfiguration.Controls.Remove(v);
            //_config = new KeyValuePairControl[0];

            //dgvDetailedResults.DataSource = null;
        }
        /// <summary>
        /// Refresh after testing.
        /// </summary>
        /// <param name="resultsHelper">Give hte helper that made the db</param>
        /// <param name="stresstestIds">Filter on one or more stresstests, if this is empty no filter is applied.</param>
        public void RefreshResults(ResultsHelper resultsHelper, params ulong[] stresstestIds) {
            _resultsHelper = resultsHelper;
            _stresstestIds = stresstestIds;
            //foreach (var ctrl in flpConfiguration.Controls)
            //    if (ctrl is LinkButton) {
            //        var lbtn = ctrl as LinkButton;
            //        if (lbtn.Active) {
            //            lbtn.PerformClick();
            //            break;
            //        }
            //    }
            //_currentSelectedIndex = -1;
            //cboShow_SelectedIndexChanged(null, null);
        }

    }
}
