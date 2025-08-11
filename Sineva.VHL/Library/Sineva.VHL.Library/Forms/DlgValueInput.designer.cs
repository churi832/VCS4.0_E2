namespace Sineva.VHL.Library
{
    partial class DlgValueInput
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panelNewValue = new System.Windows.Forms.Panel();
            this.textBoxNewValue = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panelOldValue = new System.Windows.Forms.Panel();
            this.textBoxOldValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.buttonSplitter = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelNewValue.SuspendLayout();
            this.panelOldValue.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panelNewValue);
            this.splitContainer1.Panel1.Controls.Add(this.panelOldValue);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnCancel);
            this.splitContainer1.Panel2.Controls.Add(this.buttonSplitter);
            this.splitContainer1.Panel2.Controls.Add(this.btnOk);
            this.splitContainer1.Size = new System.Drawing.Size(278, 114);
            this.splitContainer1.SplitterDistance = 202;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 11;
            // 
            // panelNewValue
            // 
            this.panelNewValue.Controls.Add(this.textBoxNewValue);
            this.panelNewValue.Controls.Add(this.label2);
            this.panelNewValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelNewValue.Location = new System.Drawing.Point(0, 53);
            this.panelNewValue.Name = "panelNewValue";
            this.panelNewValue.Size = new System.Drawing.Size(202, 61);
            this.panelNewValue.TabIndex = 3;
            // 
            // textBoxNewValue
            // 
            this.textBoxNewValue.Location = new System.Drawing.Point(12, 24);
            this.textBoxNewValue.Name = "textBoxNewValue";
            this.textBoxNewValue.Size = new System.Drawing.Size(175, 21);
            this.textBoxNewValue.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "NEW VALUE";
            // 
            // panelOldValue
            // 
            this.panelOldValue.Controls.Add(this.textBoxOldValue);
            this.panelOldValue.Controls.Add(this.label1);
            this.panelOldValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelOldValue.Location = new System.Drawing.Point(0, 0);
            this.panelOldValue.Name = "panelOldValue";
            this.panelOldValue.Size = new System.Drawing.Size(202, 53);
            this.panelOldValue.TabIndex = 1;
            // 
            // textBoxOldValue
            // 
            this.textBoxOldValue.Location = new System.Drawing.Point(12, 24);
            this.textBoxOldValue.Name = "textBoxOldValue";
            this.textBoxOldValue.ReadOnly = true;
            this.textBoxOldValue.Size = new System.Drawing.Size(175, 21);
            this.textBoxOldValue.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "OLD VALUE";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.DarkGray;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCancel.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(0, 47);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 45);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.iButtonCancel_Click);
            // 
            // buttonSplitter
            // 
            this.buttonSplitter.BackColor = System.Drawing.Color.Transparent;
            this.buttonSplitter.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonSplitter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSplitter.ForeColor = System.Drawing.Color.Transparent;
            this.buttonSplitter.Location = new System.Drawing.Point(0, 45);
            this.buttonSplitter.Name = "buttonSplitter";
            this.buttonSplitter.Size = new System.Drawing.Size(74, 2);
            this.buttonSplitter.TabIndex = 3;
            this.buttonSplitter.UseVisualStyleBackColor = false;
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.Color.YellowGreen;
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOk.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOk.ForeColor = System.Drawing.Color.White;
            this.btnOk.Location = new System.Drawing.Point(0, 0);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(74, 45);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.iButtonOK_Click);
            // 
            // DlgValueInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(284, 120);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DlgValueInput";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = " ";
            this.Load += new System.EventHandler(this.DlgValueInput_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelNewValue.ResumeLayout(false);
            this.panelNewValue.PerformLayout();
            this.panelOldValue.ResumeLayout(false);
            this.panelOldValue.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panelNewValue;
        private System.Windows.Forms.TextBox textBoxNewValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelOldValue;
        private System.Windows.Forms.TextBox textBoxOldValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button buttonSplitter;
        private System.Windows.Forms.Button btnOk;
    }
}