namespace Sineva.VHL.Library
{
	partial class FormXyPositionInput
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
			this.tbX = new System.Windows.Forms.TextBox();
			this.tbY = new System.Windows.Forms.TextBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblX = new System.Windows.Forms.Label();
			this.lblY = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tbX
			// 
			this.tbX.Location = new System.Drawing.Point(32, 7);
			this.tbX.Name = "tbX";
			this.tbX.Size = new System.Drawing.Size(73, 21);
			this.tbX.TabIndex = 0;
			// 
			// tbY
			// 
			this.tbY.Location = new System.Drawing.Point(32, 34);
			this.tbY.Name = "tbY";
			this.tbY.Size = new System.Drawing.Size(73, 21);
			this.tbY.TabIndex = 1;
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(111, 7);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(54, 48);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(171, 7);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(54, 48);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// lblX
			// 
			this.lblX.AutoSize = true;
			this.lblX.Location = new System.Drawing.Point(12, 13);
			this.lblX.Name = "lblX";
			this.lblX.Size = new System.Drawing.Size(14, 15);
			this.lblX.TabIndex = 2;
			this.lblX.Text = "X";
			// 
			// lblY
			// 
			this.lblY.AutoSize = true;
			this.lblY.Location = new System.Drawing.Point(12, 37);
			this.lblY.Name = "lblY";
			this.lblY.Size = new System.Drawing.Size(14, 15);
			this.lblY.TabIndex = 2;
			this.lblY.Text = "Y";
			// 
			// FormXyPositionInput
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(234, 62);
			this.Controls.Add(this.lblY);
			this.Controls.Add(this.lblX);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.tbY);
			this.Controls.Add(this.tbX);
			this.Font = new System.Drawing.Font("Arial", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormXyPositionInput";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "X, Y Position Input";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbX;
		private System.Windows.Forms.TextBox tbY;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblX;
		private System.Windows.Forms.Label lblY;
	}
}