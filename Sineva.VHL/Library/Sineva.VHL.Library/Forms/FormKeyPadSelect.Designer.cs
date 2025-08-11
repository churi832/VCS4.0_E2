namespace Sineva.VHL.Library
{
	partial class FormKeyPadSelect
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
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.buttonSelect = new System.Windows.Forms.Button();
			this.buttonClear = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.labelSelectedTitle = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 12;
			this.listBox1.Location = new System.Drawing.Point(12, 32);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(211, 208);
			this.listBox1.TabIndex = 0;
			// 
			// buttonSelect
			// 
			this.buttonSelect.Location = new System.Drawing.Point(229, 32);
			this.buttonSelect.Name = "buttonSelect";
			this.buttonSelect.Size = new System.Drawing.Size(79, 115);
			this.buttonSelect.TabIndex = 1;
			this.buttonSelect.Text = "button1";
			this.buttonSelect.UseVisualStyleBackColor = true;
			// 
			// buttonClear
			// 
			this.buttonClear.Location = new System.Drawing.Point(229, 158);
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.Size = new System.Drawing.Size(78, 38);
			this.buttonClear.TabIndex = 2;
			this.buttonClear.Text = "button1";
			this.buttonClear.UseVisualStyleBackColor = true;
			// 
			// buttonClose
			// 
			this.buttonClose.Location = new System.Drawing.Point(229, 202);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(78, 38);
			this.buttonClose.TabIndex = 2;
			this.buttonClose.Text = "button1";
			this.buttonClose.UseVisualStyleBackColor = true;
			// 
			// labelSelectedTitle
			// 
			this.labelSelectedTitle.AutoSize = true;
			this.labelSelectedTitle.Location = new System.Drawing.Point(21, 11);
			this.labelSelectedTitle.Name = "labelSelectedTitle";
			this.labelSelectedTitle.Size = new System.Drawing.Size(38, 12);
			this.labelSelectedTitle.TabIndex = 3;
			this.labelSelectedTitle.Text = "label1";
            // 
            // FormKeyPadSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(319, 250);
			this.ControlBox = false;
			this.Controls.Add(this.labelSelectedTitle);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.buttonClear);
			this.Controls.Add(this.buttonSelect);
			this.Controls.Add(this.listBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "FormKeyPadSelect";
			this.Text = "FormKeyPadSelect";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button buttonSelect;
		private System.Windows.Forms.Button buttonClear;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Label labelSelectedTitle;
	}
}