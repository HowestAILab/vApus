namespace vApus.DistributedTesting
{
    partial class TileStresstestView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TileStresstestView));
            this.lblCurrentConcurrent = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.tmrProgress = new System.Timers.Timer();
            this.tmrProgressDelayCountDown = new System.Windows.Forms.Timer(this.components);
            this.stresstestControl = new vApus.Stresstest.FastResultsControl();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tmrProgress)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCurrentConcurrent
            // 
            this.lblCurrentConcurrent.AutoSize = true;
            this.lblCurrentConcurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentConcurrent.Location = new System.Drawing.Point(102, 52);
            this.lblCurrentConcurrent.Name = "lblCurrentConcurrent";
            this.lblCurrentConcurrent.Size = new System.Drawing.Size(0, 13);
            this.lblCurrentConcurrent.TabIndex = 24;
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStop.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(90, 37);
            this.btnStop.Text = "Stop";
            this.btnStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStop});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.MinimumSize = new System.Drawing.Size(0, 40);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(843, 40);
            this.toolStrip.TabIndex = 1;
            // 
            // tmrProgress
            // 
            this.tmrProgress.SynchronizingObject = this;
            this.tmrProgress.Elapsed += new System.Timers.ElapsedEventHandler(this.tmrProgress_Tick);
            // 
            // tmrProgressDelayCountDown
            // 
            this.tmrProgressDelayCountDown.Interval = 1000;
            this.tmrProgressDelayCountDown.Tick += new System.EventHandler(this.tmrProgressDelayCountDown_Tick);
            // 
            // stresstestControl
            // 
            this.stresstestControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stresstestControl.Location = new System.Drawing.Point(0, 40);
            this.stresstestControl.MonitorConfigurationControlAndLinkButtonsVisible = false;
            this.stresstestControl.Name = "stresstestControl";
            this.stresstestControl.Size = new System.Drawing.Size(843, 592);
            this.stresstestControl.TabIndex = 2;
            // 
            // TileStresstestView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 635);
            this.Controls.Add(this.stresstestControl);
            this.Controls.Add(this.toolStrip);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "TileStresstestView";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.Text = "StresstestView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TileStresstestView_FormClosing);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tmrProgress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCurrentConcurrent;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Timers.Timer tmrProgress;
        private System.Windows.Forms.Timer tmrProgressDelayCountDown;
        private Stresstest.FastResultsControl stresstestControl;

    }
}