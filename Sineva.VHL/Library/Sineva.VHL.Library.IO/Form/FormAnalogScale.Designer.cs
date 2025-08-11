
namespace Sineva.VHL.Library.IO
{
    partial class FormAnalogScale
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
            if(disposing && (components != null))
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
            this.btnApply = new System.Windows.Forms.Button();
            this.tbAdcMax = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbAdcMin = new System.Windows.Forms.TextBox();
            this.tbRealMax = new System.Windows.Forms.TextBox();
            this.tbRealMin = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbCurAdc = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.chkScaleUse = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.numDecimal = new System.Windows.Forms.NumericUpDown();
            this.cbPreset = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.cbUnitType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.numDecimal)).BeginInit();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(345, 10);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 40);
            this.btnApply.TabIndex = 8;
            this.btnApply.Text = "APPLY";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // tbAdcMax
            // 
            this.tbAdcMax.BackColor = System.Drawing.Color.LightYellow;
            this.tbAdcMax.Location = new System.Drawing.Point(82, 32);
            this.tbAdcMax.Name = "tbAdcMax";
            this.tbAdcMax.Size = new System.Drawing.Size(90, 21);
            this.tbAdcMax.TabIndex = 0;
            this.tbAdcMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.DimGray;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(11, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 20);
            this.label1.TabIndex = 100;
            this.label1.Text = "ADC SETTING";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(11, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 21);
            this.label2.TabIndex = 100;
            this.label2.Text = "ADC MAX";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(11, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 21);
            this.label3.TabIndex = 100;
            this.label3.Text = "ADC MIN";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbAdcMin
            // 
            this.tbAdcMin.BackColor = System.Drawing.Color.LightYellow;
            this.tbAdcMin.Location = new System.Drawing.Point(82, 54);
            this.tbAdcMin.Name = "tbAdcMin";
            this.tbAdcMin.Size = new System.Drawing.Size(90, 21);
            this.tbAdcMin.TabIndex = 1;
            this.tbAdcMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbRealMax
            // 
            this.tbRealMax.BackColor = System.Drawing.Color.LightYellow;
            this.tbRealMax.Location = new System.Drawing.Point(249, 32);
            this.tbRealMax.Name = "tbRealMax";
            this.tbRealMax.Size = new System.Drawing.Size(90, 21);
            this.tbRealMax.TabIndex = 2;
            this.tbRealMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbRealMin
            // 
            this.tbRealMin.BackColor = System.Drawing.Color.LightYellow;
            this.tbRealMin.Location = new System.Drawing.Point(249, 54);
            this.tbRealMin.Name = "tbRealMin";
            this.tbRealMin.Size = new System.Drawing.Size(90, 21);
            this.tbRealMin.TabIndex = 3;
            this.tbRealMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.DimGray;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(178, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(161, 20);
            this.label4.TabIndex = 100;
            this.label4.Text = "REAL SETTING";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(178, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 21);
            this.label5.TabIndex = 100;
            this.label5.Text = "REAL MAX";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(178, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 21);
            this.label6.TabIndex = 100;
            this.label6.Text = "REAL MIN";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbCurAdc
            // 
            this.tbCurAdc.Location = new System.Drawing.Point(82, 90);
            this.tbCurAdc.Name = "tbCurAdc";
            this.tbCurAdc.ReadOnly = true;
            this.tbCurAdc.Size = new System.Drawing.Size(90, 21);
            this.tbCurAdc.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.Location = new System.Drawing.Point(11, 90);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 21);
            this.label7.TabIndex = 100;
            this.label7.Text = "CUR ADC";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbValue
            // 
            this.tbValue.Location = new System.Drawing.Point(248, 112);
            this.tbValue.Name = "tbValue";
            this.tbValue.ReadOnly = true;
            this.tbValue.Size = new System.Drawing.Size(90, 21);
            this.tbValue.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(177, 112);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 21);
            this.label8.TabIndex = 100;
            this.label8.Text = "VALUE";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(345, 55);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(75, 40);
            this.btnRestore.TabIndex = 9;
            this.btnRestore.Text = "RESTORE";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(263, 147);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 40);
            this.btnConfirm.TabIndex = 10;
            this.btnConfirm.Text = "CONFIRM";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // chkScaleUse
            // 
            this.chkScaleUse.AutoSize = true;
            this.chkScaleUse.Location = new System.Drawing.Point(11, 117);
            this.chkScaleUse.Name = "chkScaleUse";
            this.chkScaleUse.Size = new System.Drawing.Size(82, 16);
            this.chkScaleUse.TabIndex = 5;
            this.chkScaleUse.Text = "Scale Use";
            this.chkScaleUse.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(344, 146);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 40);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label9.Location = new System.Drawing.Point(177, 90);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 21);
            this.label9.TabIndex = 100;
            this.label9.Text = "DECIMAL";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numDecimal
            // 
            this.numDecimal.Location = new System.Drawing.Point(248, 90);
            this.numDecimal.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.numDecimal.Name = "numDecimal";
            this.numDecimal.Size = new System.Drawing.Size(90, 21);
            this.numDecimal.TabIndex = 4;
            this.numDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numDecimal.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // cbPreset
            // 
            this.cbPreset.BackColor = System.Drawing.SystemColors.Window;
            this.cbPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPreset.FormattingEnabled = true;
            this.cbPreset.Location = new System.Drawing.Point(82, 147);
            this.cbPreset.Name = "cbPreset";
            this.cbPreset.Size = new System.Drawing.Size(160, 20);
            this.cbPreset.TabIndex = 6;
            this.cbPreset.SelectionChangeCommitted += new System.EventHandler(this.cbPreset_SelectionChangeCommitted);
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label10.Location = new System.Drawing.Point(11, 147);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(70, 20);
            this.label10.TabIndex = 100;
            this.label10.Text = "PRESET";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label11.Location = new System.Drawing.Point(11, 170);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(70, 20);
            this.label11.TabIndex = 100;
            this.label11.Text = "TYPE";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbUnitType
            // 
            this.cbUnitType.BackColor = System.Drawing.SystemColors.Window;
            this.cbUnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUnitType.FormattingEnabled = true;
            this.cbUnitType.Location = new System.Drawing.Point(82, 170);
            this.cbUnitType.Name = "cbUnitType";
            this.cbUnitType.Size = new System.Drawing.Size(113, 20);
            this.cbUnitType.TabIndex = 7;
            // 
            // FormAnalogScale
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(431, 200);
            this.ControlBox = false;
            this.Controls.Add(this.cbUnitType);
            this.Controls.Add(this.cbPreset);
            this.Controls.Add(this.numDecimal);
            this.Controls.Add(this.chkScaleUse);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbValue);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbRealMin);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbRealMax);
            this.Controls.Add(this.tbCurAdc);
            this.Controls.Add(this.tbAdcMin);
            this.Controls.Add(this.tbAdcMax);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnApply);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormAnalogScale";
            this.Text = "FormAnalogScale";
            ((System.ComponentModel.ISupportInitialize)(this.numDecimal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.TextBox tbAdcMax;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbAdcMin;
        private System.Windows.Forms.TextBox tbRealMax;
        private System.Windows.Forms.TextBox tbRealMin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbCurAdc;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbValue;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.CheckBox chkScaleUse;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numDecimal;
        private System.Windows.Forms.ComboBox cbPreset;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cbUnitType;
    }
}