namespace Sineva.VHL.Library.Servo
{
	partial class FormAddAxis_ServoUnit
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.listBoxAxisTypes = new System.Windows.Forms.ListBox();
			this.btnCanel = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
            this.propertyGrid1 = new Sineva.VHL.Library.FilteredPropertyGrid();
			this.cbCountSel = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.listBoxAxisTypes);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.cbCountSel);
			this.splitContainer1.Panel2.Controls.Add(this.btnCanel);
			this.splitContainer1.Panel2.Controls.Add(this.btnAdd);
			this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
			this.splitContainer1.Size = new System.Drawing.Size(609, 406);
			this.splitContainer1.SplitterDistance = 184;
			this.splitContainer1.TabIndex = 3;
			// 
			// listBoxAxisTypes
			// 
			this.listBoxAxisTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxAxisTypes.FormattingEnabled = true;
			this.listBoxAxisTypes.ItemHeight = 12;
			this.listBoxAxisTypes.Location = new System.Drawing.Point(0, 0);
			this.listBoxAxisTypes.Name = "listBoxAxisTypes";
			this.listBoxAxisTypes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxAxisTypes.Size = new System.Drawing.Size(184, 406);
			this.listBoxAxisTypes.TabIndex = 0;
			this.listBoxAxisTypes.SelectedIndexChanged += new System.EventHandler(this.listBoxAxisTypes_SelectedIndexChanged);
			// 
			// btnCanel
			// 
			this.btnCanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCanel.Location = new System.Drawing.Point(266, 352);
			this.btnCanel.Name = "btnCanel";
			this.btnCanel.Size = new System.Drawing.Size(152, 51);
			this.btnCanel.TabIndex = 2;
			this.btnCanel.Text = "CANCEL";
			this.btnCanel.UseVisualStyleBackColor = true;
			this.btnCanel.Click += new System.EventHandler(this.btnCanel_Click);
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAdd.Location = new System.Drawing.Point(109, 352);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(152, 51);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "ADD";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(415, 343);
			this.propertyGrid1.TabIndex = 1;
			// 
			// cbCountSel
			// 
			this.cbCountSel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cbCountSel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbCountSel.FormattingEnabled = true;
			this.cbCountSel.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
			this.cbCountSel.Location = new System.Drawing.Point(11, 367);
			this.cbCountSel.Name = "cbCountSel";
			this.cbCountSel.Size = new System.Drawing.Size(92, 20);
			this.cbCountSel.TabIndex = 3;
			this.cbCountSel.Visible = false;
			this.cbCountSel.SelectedIndexChanged += new System.EventHandler(this.cbCountSel_SelectedIndexChanged);
			// 
			// FormAddAxis
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(609, 406);
			this.Controls.Add(this.splitContainer1);
			this.Name = "FormAddAxis";
            this.Text = "FormAddAxis";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListBox listBoxAxisTypes;
		private System.Windows.Forms.Button btnCanel;
		private System.Windows.Forms.Button btnAdd;
        private Sineva.VHL.Library.FilteredPropertyGrid propertyGrid1;
		private System.Windows.Forms.ComboBox cbCountSel;
	}
}