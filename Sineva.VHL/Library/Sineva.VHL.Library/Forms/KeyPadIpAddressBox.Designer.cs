namespace Sineva.VHL.Library
{
	partial class KeyPadIpAddressBox
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyPadIpAddressBox));
			this.lblFormatTitle = new System.Windows.Forms.Label();
			this.lblOldVal = new System.Windows.Forms.Label();
			this.lblNewVal = new System.Windows.Forms.Label();
			this.lblRangeTitle = new System.Windows.Forms.Label();
			this.lblFormat = new System.Windows.Forms.Label();
			this.lblRange = new System.Windows.Forms.Label();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtOldVal = new ValidationTextBox();
			this.txtNewVal = new ValidationTextBox();
			this.SuspendLayout();
			// 
			// lblFormatTitle
			// 
			this.lblFormatTitle.Location = new System.Drawing.Point(14, 14);
			this.lblFormatTitle.Name = "lblFormatTitle";
			this.lblFormatTitle.Size = new System.Drawing.Size(78, 15);
			this.lblFormatTitle.TabIndex = 0;
			this.lblFormatTitle.Text = "Format :";
			this.lblFormatTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblOldVal
			// 
			this.lblOldVal.Location = new System.Drawing.Point(14, 39);
			this.lblOldVal.Name = "lblOldVal";
			this.lblOldVal.Size = new System.Drawing.Size(78, 15);
			this.lblOldVal.TabIndex = 0;
			this.lblOldVal.Text = "Old Value :";
			this.lblOldVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblNewVal
			// 
			this.lblNewVal.Location = new System.Drawing.Point(14, 66);
			this.lblNewVal.Name = "lblNewVal";
			this.lblNewVal.Size = new System.Drawing.Size(78, 15);
			this.lblNewVal.TabIndex = 0;
			this.lblNewVal.Text = "New Value :";
			this.lblNewVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblRangeTitle
			// 
			this.lblRangeTitle.Location = new System.Drawing.Point(14, 91);
			this.lblRangeTitle.Name = "lblRangeTitle";
			this.lblRangeTitle.Size = new System.Drawing.Size(78, 15);
			this.lblRangeTitle.TabIndex = 0;
			this.lblRangeTitle.Text = "Range :";
			this.lblRangeTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblFormat
			// 
			this.lblFormat.Location = new System.Drawing.Point(98, 14);
			this.lblFormat.Name = "lblFormat";
			this.lblFormat.Size = new System.Drawing.Size(203, 15);
			this.lblFormat.TabIndex = 0;
			this.lblFormat.Text = "Data Format";
			this.lblFormat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblRange
			// 
			this.lblRange.Location = new System.Drawing.Point(98, 91);
			this.lblRange.Name = "lblRange";
			this.lblRange.Size = new System.Drawing.Size(203, 15);
			this.lblRange.TabIndex = 0;
			this.lblRange.Text = "Data Range";
			this.lblRange.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(238, 20);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(81, 35);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(237, 61);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(81, 35);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "CANCEL";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// txtOldVal
			// 
			this.txtOldVal.DataFormat = OptionFormat.None;
			this.txtOldVal.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.txtOldVal.KeyPadInfo = ((KeyPadInfo)(resources.GetObject("txtOldVal.KeyPadInfo")));
			this.txtOldVal.LimitHigh = "";
			this.txtOldVal.LimitLow = "";
			this.txtOldVal.Location = new System.Drawing.Point(103, 32);
			this.txtOldVal.Name = "txtOldVal";
			this.txtOldVal.ReadOnly = true;
			this.txtOldVal.ReferenceTag = null;
			this.txtOldVal.Size = new System.Drawing.Size(121, 21);
			this.txtOldVal.TabIndex = 3;
			this.txtOldVal.UsedInKeyPad = false;
			// 
			// txtNewVal
			// 
			this.txtNewVal.DataFormat = OptionFormat.None;
			this.txtNewVal.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.txtNewVal.KeyPadInfo = ((KeyPadInfo)(resources.GetObject("txtNewVal.KeyPadInfo")));
			this.txtNewVal.LimitHigh = "";
			this.txtNewVal.LimitLow = "";
			this.txtNewVal.Location = new System.Drawing.Point(102, 62);
			this.txtNewVal.Name = "txtNewVal";
			this.txtNewVal.ReferenceTag = null;
			this.txtNewVal.Size = new System.Drawing.Size(121, 21);
			this.txtNewVal.TabIndex = 4;
			this.txtNewVal.UsedInKeyPad = false;
            // 
            // KeyPadIpAddressBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Caption = "KeyPadIpAddressBox";
			this.ClientSize = new System.Drawing.Size(330, 121);
			this.ControlBox = false;
			this.Controls.Add(this.txtNewVal);
			this.Controls.Add(this.txtOldVal);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lblRangeTitle);
			this.Controls.Add(this.lblNewVal);
			this.Controls.Add(this.lblOldVal);
			this.Controls.Add(this.lblRange);
			this.Controls.Add(this.lblFormat);
			this.Controls.Add(this.lblFormatTitle);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "KeyPadIpAddressBox";
			this.Text = "KeyPadIpAddressBox";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblFormatTitle;
		private System.Windows.Forms.Label lblOldVal;
		private System.Windows.Forms.Label lblNewVal;
		private System.Windows.Forms.Label lblRangeTitle;
		private System.Windows.Forms.Label lblFormat;
		private System.Windows.Forms.Label lblRange;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private ValidationTextBox txtOldVal;
		private ValidationTextBox txtNewVal;
	}
}