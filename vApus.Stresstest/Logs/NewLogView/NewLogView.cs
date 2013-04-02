using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.Stresstest {
    public partial class NewLogView : BaseSolutionComponentView {
        private readonly Log _log;

        public NewLogView() {
            InitializeComponent();
        }

        public NewLogView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args) {
            InitializeComponent();

            _log = solutionComponent as Log;
            if (IsHandleCreated)
                SetLog();
            else
                HandleCreated += NewLogView_HandleCreated;
        }

        void NewLogView_HandleCreated(object sender, EventArgs e) {
            HandleCreated -= NewLogView_HandleCreated;
            SetLog();
        }
        private void SetLog() {
            logTreeView.SetLog(_log);
        }
       
    }
}
