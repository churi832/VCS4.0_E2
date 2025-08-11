using System.Windows.Forms;

namespace Sineva.VHL.GUI.TouchPad
{
    partial class AddVehicleForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddVehicleForm));
            this.txtUserIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnUserInfoCancel = new System.Windows.Forms.Button();
            this.btnUserInfoOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtUserIP
            // 
            this.txtUserIP.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtUserIP.Location = new System.Drawing.Point(245, 156);
            this.txtUserIP.Name = "txtUserIP";
            this.txtUserIP.Size = new System.Drawing.Size(234, 50);
            this.txtUserIP.TabIndex = 7;
            this.txtUserIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtUserIP.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtEnter_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(118, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 41);
            this.label2.TabIndex = 6;
            this.label2.Text = "IP地址";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // txtUsername
            // 
            this.txtUsername.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtUsername.Location = new System.Drawing.Point(245, 49);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(234, 50);
            this.txtUsername.TabIndex = 5;
            this.txtUsername.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtUsername.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtEnter_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(93, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 41);
            this.label1.TabIndex = 4;
            this.label1.Text = "车辆编号";
            // 
            // btnUserInfoCancel
            // 
            this.btnUserInfoCancel.BackColor = System.Drawing.Color.PowderBlue;
            this.btnUserInfoCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnUserInfoCancel.FlatAppearance.BorderSize = 0;
            this.btnUserInfoCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUserInfoCancel.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnUserInfoCancel.Location = new System.Drawing.Point(358, 265);
            this.btnUserInfoCancel.Name = "btnUserInfoCancel";
            this.btnUserInfoCancel.Size = new System.Drawing.Size(143, 60);
            this.btnUserInfoCancel.TabIndex = 9;
            this.btnUserInfoCancel.Text = "取消";
            this.btnUserInfoCancel.UseVisualStyleBackColor = true;
            this.btnUserInfoCancel.Click += new System.EventHandler(this.btnUserInfoCancel_Click);
            // 
            // btnUserInfoOK
            // 
            this.btnUserInfoOK.BackColor = System.Drawing.Color.PowderBlue;
            this.btnUserInfoOK.FlatAppearance.BorderSize = 0;
            this.btnUserInfoOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUserInfoOK.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnUserInfoOK.Location = new System.Drawing.Point(100, 265);
            this.btnUserInfoOK.Name = "btnUserInfoOK";
            this.btnUserInfoOK.Size = new System.Drawing.Size(143, 60);
            this.btnUserInfoOK.TabIndex = 8;
            this.btnUserInfoOK.Text = "确认";
            this.btnUserInfoOK.UseVisualStyleBackColor = true;
            this.btnUserInfoOK.Click += new System.EventHandler(this.btnUserInfoOK_Click);
            // 
            // AddVehicleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnUserInfoCancel;
            this.ClientSize = new System.Drawing.Size(593, 375);
            this.Controls.Add(this.btnUserInfoCancel);
            this.Controls.Add(this.btnUserInfoOK);
            this.Controls.Add(this.txtUserIP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddVehicleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "添加车辆";
            this.Load += new System.EventHandler(this.AddVehicleForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtUserIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnUserInfoCancel;
        private System.Windows.Forms.Button btnUserInfoOK;
    }
}