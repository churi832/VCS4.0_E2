namespace Sineva.VHL.Library
{
	partial class KeyPadTextBox
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyPadTextBox));
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnSelectFolder = new System.Windows.Forms.Button();
			this.txtOldVal = new System.Windows.Forms.TextBox();
			this.lblFormat = new System.Windows.Forms.Label();
			this.lblRange = new System.Windows.Forms.Label();
			this.lblFormatTitle = new System.Windows.Forms.Label();
			this.lblOldVal = new System.Windows.Forms.Label();
			this.lblNewVal = new System.Windows.Forms.Label();
			this.lblRangeTitle = new System.Windows.Forms.Label();
			this.txtNewVal = new ValidationTextBox();
			this.SuspendLayout();
			// 
			// btnOk
			// 
			this.btnOk.BackColor = System.Drawing.Color.LightCyan;
			this.btnOk.Location = new System.Drawing.Point(199, 12);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(73, 44);
			this.btnOk.TabIndex = 0;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = false;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.Color.LemonChiffon;
			this.btnCancel.Location = new System.Drawing.Point(199, 62);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(72, 40);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "CANCEL";
			this.btnCancel.UseVisualStyleBackColor = false;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnSelectFolder
			// 
			this.btnSelectFolder.Location = new System.Drawing.Point(162, 71);
			this.btnSelectFolder.Name = "btnSelectFolder";
			this.btnSelectFolder.Size = new System.Drawing.Size(25, 21);
			this.btnSelectFolder.TabIndex = 2;
			this.btnSelectFolder.Text = "...";
			this.btnSelectFolder.UseVisualStyleBackColor = true;
			this.btnSelectFolder.Visible = false;
			this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
			// 
			// txtOldVal
			// 
			this.txtOldVal.Location = new System.Drawing.Point(84, 44);
			this.txtOldVal.Name = "txtOldVal";
			this.txtOldVal.ReadOnly = true;
			this.txtOldVal.Size = new System.Drawing.Size(102, 21);
			this.txtOldVal.TabIndex = 5;
			// 
			// lblFormat
			// 
			this.lblFormat.Location = new System.Drawing.Point(82, 14);
			this.lblFormat.Name = "lblFormat";
			this.lblFormat.Size = new System.Drawing.Size(93, 21);
			this.lblFormat.TabIndex = 4;
			this.lblFormat.Text = "Data Format";
			this.lblFormat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblRange
			// 
			this.lblRange.Location = new System.Drawing.Point(84, 102);
			this.lblRange.Name = "lblRange";
			this.lblRange.Size = new System.Drawing.Size(102, 16);
			this.lblRange.TabIndex = 4;
			this.lblRange.Text = "Data Range";
			this.lblRange.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblRange.Visible = false;
			// 
			// lblFormatTitle
			// 
			this.lblFormatTitle.Location = new System.Drawing.Point(17, 14);
			this.lblFormatTitle.Name = "lblFormatTitle";
			this.lblFormatTitle.Size = new System.Drawing.Size(61, 21);
			this.lblFormatTitle.TabIndex = 5;
			this.lblFormatTitle.Text = "Format :";
			this.lblFormatTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblOldVal
			// 
			this.lblOldVal.Location = new System.Drawing.Point(4, 43);
			this.lblOldVal.Name = "lblOldVal";
			this.lblOldVal.Size = new System.Drawing.Size(74, 21);
			this.lblOldVal.TabIndex = 5;
			this.lblOldVal.Text = "Old Value :";
			this.lblOldVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblNewVal
			// 
			this.lblNewVal.Location = new System.Drawing.Point(-3, 71);
			this.lblNewVal.Name = "lblNewVal";
			this.lblNewVal.Size = new System.Drawing.Size(81, 21);
			this.lblNewVal.TabIndex = 5;
			this.lblNewVal.Text = "New Value :";
			this.lblNewVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblRangeTitle
			// 
			this.lblRangeTitle.Location = new System.Drawing.Point(17, 100);
			this.lblRangeTitle.Name = "lblRangeTitle";
			this.lblRangeTitle.Size = new System.Drawing.Size(61, 21);
			this.lblRangeTitle.TabIndex = 5;
			this.lblRangeTitle.Text = "Range :";
			this.lblRangeTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lblRangeTitle.Visible = false;
			// 
			// txtNewVal
			// 
			this.txtNewVal.DataFormat = OptionFormat.None;
			this.txtNewVal.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.txtNewVal.KeyPadInfo = ((KeyPadInfo)(resources.GetObject("txtNewVal.KeyPadInfo")));
			this.txtNewVal.LimitHigh = "";
			this.txtNewVal.LimitLow = "";
			this.txtNewVal.Location = new System.Drawing.Point(86, 72);
			this.txtNewVal.Name = "txtNewVal";
			this.txtNewVal.ReferenceTag = null;
			this.txtNewVal.Size = new System.Drawing.Size(100, 21);
			this.txtNewVal.TabIndex = 6;
			this.txtNewVal.UsedInKeyPad = false;
            // 
            // KeyPadTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Caption = "Edit";
			this.ClientSize = new System.Drawing.Size(284, 125);
			this.ControlBox = false;
			this.Controls.Add(this.lblRangeTitle);
			this.Controls.Add(this.lblNewVal);
			this.Controls.Add(this.lblOldVal);
			this.Controls.Add(this.lblFormatTitle);
			this.Controls.Add(this.lblRange);
			this.Controls.Add(this.lblFormat);
			this.Controls.Add(this.btnSelectFolder);
			this.Controls.Add(this.txtOldVal);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.txtNewVal);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "KeyPadTextBox";
			this.Text = "Edit";
			this.Load += new System.EventHandler(this.KeyPadTextBox_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnSelectFolder;
		private System.Windows.Forms.TextBox txtOldVal;
		private System.Windows.Forms.Label lblFormat;
		private System.Windows.Forms.Label lblRange;
		private System.Windows.Forms.Label lblFormatTitle;
		private System.Windows.Forms.Label lblOldVal;
		private System.Windows.Forms.Label lblNewVal;
		private System.Windows.Forms.Label lblRangeTitle;
		private ValidationTextBox txtNewVal;
	}
}