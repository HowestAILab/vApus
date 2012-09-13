namespace vApus.Gui
{
	partial class AuthorGrid
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthorGrid));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.colImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.colAuthors = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLinkedIn = new System.Windows.Forms.DataGridViewImageColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGrid
            // 
            this.dataGrid.AllowUserToAddRows = false;
            this.dataGrid.AllowUserToDeleteRows = false;
            this.dataGrid.AllowUserToResizeColumns = false;
            this.dataGrid.AllowUserToResizeRows = false;
            this.dataGrid.BackgroundColor = System.Drawing.Color.White;
            this.dataGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid.ColumnHeadersVisible = false;
            this.dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colImage,
            this.colAuthors,
            this.colLinkedIn});
            this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGrid.EnableHeadersVisualStyles = false;
            this.dataGrid.Location = new System.Drawing.Point(0, 0);
            this.dataGrid.MultiSelect = false;
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.ReadOnly = true;
            this.dataGrid.RowHeadersVisible = false;
            this.dataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGrid.Size = new System.Drawing.Size(470, 328);
            this.dataGrid.TabIndex = 0;
            this.dataGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellClick);
            this.dataGrid.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellMouseEnter);
            this.dataGrid.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellMouseLeave);
            this.dataGrid.SelectionChanged += new System.EventHandler(this.dataGrid_SelectionChanged);
            // 
            // colImage
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle11.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle11.NullValue")));
            dataGridViewCellStyle11.Padding = new System.Windows.Forms.Padding(1);
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.LightGray;
            this.colImage.DefaultCellStyle = dataGridViewCellStyle11;
            this.colImage.HeaderText = "";
            this.colImage.Name = "colImage";
            this.colImage.ReadOnly = true;
            this.colImage.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colImage.Width = 5;
            // 
            // colAuthors
            // 
            this.colAuthors.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colAuthors.HeaderText = "";
            this.colAuthors.Name = "colAuthors";
            this.colAuthors.ReadOnly = true;
            this.colAuthors.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // colLinkedIn
            // 
            this.colLinkedIn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            dataGridViewCellStyle12.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle12.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.colLinkedIn.DefaultCellStyle = dataGridViewCellStyle12;
            this.colLinkedIn.HeaderText = "";
            this.colLinkedIn.Image = global::vApus.Gui.Properties.Resources.linkedIn;
            this.colLinkedIn.Name = "colLinkedIn";
            this.colLinkedIn.ReadOnly = true;
            this.colLinkedIn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colLinkedIn.Width = 5;
            // 
            // AuthorGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGrid);
            this.Name = "AuthorGrid";
            this.Size = new System.Drawing.Size(470, 328);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.DataGridViewImageColumn colImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAuthors;
        private System.Windows.Forms.DataGridViewImageColumn colLinkedIn;
	}
}
