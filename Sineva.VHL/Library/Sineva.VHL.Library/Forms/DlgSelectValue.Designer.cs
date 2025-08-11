namespace Sineva.VHL.Library
{
	partial class DlgSelectValue
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgSelectValue));
			this.lblFrom = new System.Windows.Forms.Label();
			this.lblTo = new System.Windows.Forms.Label();
			this.cboNewValue = new System.Windows.Forms.ComboBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtCurValue = new ValidationTextBox();
			this.SuspendLayout();
			// 
			// lblFrom
			// 
			this.lblFrom.Location = new System.Drawing.Point(22, 20);
			this.lblFrom.Name = "lblFrom";
			this.lblFrom.Size = new System.Drawing.Size(48, 19);
			this.lblFrom.TabIndex = 0;
			this.lblFrom.Text = "FROM :";
			// 
			// lblTo
			// 
			this.lblTo.Location = new System.Drawing.Point(22, 51);
			this.lblTo.Name = "lblTo";
			this.lblTo.Size = new System.Drawing.Size(48, 20);
			this.lblTo.TabIndex = 0;
			this.lblTo.Text = "TO :";
			this.lblTo.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cboNewValue
			// 
			this.cboNewValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboNewValue.FormattingEnabled = true;
			this.cboNewValue.Location = new System.Drawing.Point(77, 44);
			this.cboNewValue.Name = "cboNewValue";
			this.cboNewValue.Size = new System.Drawing.Size(110, 20);
			this.cboNewValue.TabIndex = 1;
			this.cboNewValue.SelectedIndexChanged += new System.EventHandler(this.cboNewValue_SelectedIndexChanged);
			// 
			// btnOk
			// 
			this.btnOk.BackColor = System.Drawing.Color.LightCyan;
			this.btnOk.Location = new System.Drawing.Point(206, 8);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(71, 37);
			this.btnOk.TabIndex = 3;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = false;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.Color.LemonChiffon;
			this.btnCancel.Location = new System.Drawing.Point(206, 51);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(71, 35);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "CANCEL";
			this.btnCancel.UseVisualStyleBackColor = false;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// txtCurValue
			// 
			this.txtCurValue.DataFormat = OptionFormat.None;
			this.txtCurValue.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.txtCurValue.KeyPadInfo = ((KeyPadInfo)(resources.GetObject("txtCurValue.KeyPadInfo")));
			this.txtCurValue.LimitHigh = "";
			this.txtCurValue.LimitLow = "";
			this.txtCurValue.Location = new System.Drawing.Point(76, 17);
			this.txtCurValue.Name = "txtCurValue";
			this.txtCurValue.ReadOnly = true;
			this.txtCurValue.ReferenceTag = null;
			this.txtCurValue.Size = new System.Drawing.Size(110, 21);
			this.txtCurValue.TabIndex = 4;
			this.txtCurValue.UsedInKeyPad = false;
			// 
			// DlgSelectValue
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(293, 91);
			this.ControlBox = false;
			this.Controls.Add(this.txtCurValue);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.cboNewValue);
			this.Controls.Add(this.lblTo);
			this.Controls.Add(this.lblFrom);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "DlgSelectValue";
			this.Text = "Select";
			this.Load += new System.EventHandler(this.DlgSelectValue_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblFrom;
		private System.Windows.Forms.Label lblTo;
		private System.Windows.Forms.ComboBox cboNewValue;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private ValidationTextBox txtCurValue;
	}
}