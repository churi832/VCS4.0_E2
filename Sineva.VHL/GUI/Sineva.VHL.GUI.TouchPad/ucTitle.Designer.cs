namespace Sineva.VHL.GUI.TouchPad
{
    partial class ucTitle
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtEQPMode = new System.Windows.Forms.Button();
            this.labelJCSState = new System.Windows.Forms.Button();
            this.labelOCSState = new System.Windows.Forms.Button();
            this.labelEQPState = new System.Windows.Forms.Button();
            this.labelVehicleNum = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Button();
            this.labelVelocity = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtEQPMode
            // 
            this.txtEQPMode.AutoSize = true;
            this.txtEQPMode.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.Frame_Blue;
            this.txtEQPMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.txtEQPMode.FlatAppearance.BorderSize = 0;
            this.txtEQPMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.txtEQPMode.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtEQPMode.ForeColor = System.Drawing.Color.White;
            this.txtEQPMode.Location = new System.Drawing.Point(203, 12);
            this.txtEQPMode.Name = "txtEQPMode";
            this.txtEQPMode.Size = new System.Drawing.Size(86, 32);
            this.txtEQPMode.TabIndex = 152;
            this.txtEQPMode.Text = "手动控制";
            this.txtEQPMode.UseVisualStyleBackColor = true;
            this.txtEQPMode.TextChanged += new System.EventHandler(this.txtEQPMode_TextChanged);
            this.txtEQPMode.Click += new System.EventHandler(this.txtEQPMode_Click);
            // 
            // labelJCSState
            // 
            this.labelJCSState.AutoSize = true;
            this.labelJCSState.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.Frame_Red;
            this.labelJCSState.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.labelJCSState.FlatAppearance.BorderSize = 0;
            this.labelJCSState.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelJCSState.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelJCSState.ForeColor = System.Drawing.Color.White;
            this.labelJCSState.Location = new System.Drawing.Point(761, 12);
            this.labelJCSState.Name = "labelJCSState";
            this.labelJCSState.Size = new System.Drawing.Size(91, 32);
            this.labelJCSState.TabIndex = 155;
            this.labelJCSState.Text = "UnReady";
            this.labelJCSState.UseVisualStyleBackColor = true;
            this.labelJCSState.TextChanged += new System.EventHandler(this.labelJCSAndOCSState_TextChanged);
            // 
            // labelOCSState
            // 
            this.labelOCSState.AutoSize = true;
            this.labelOCSState.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.Frame_Red;
            this.labelOCSState.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.labelOCSState.FlatAppearance.BorderSize = 0;
            this.labelOCSState.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelOCSState.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelOCSState.ForeColor = System.Drawing.Color.White;
            this.labelOCSState.Location = new System.Drawing.Point(561, 12);
            this.labelOCSState.Name = "labelOCSState";
            this.labelOCSState.Size = new System.Drawing.Size(91, 32);
            this.labelOCSState.TabIndex = 154;
            this.labelOCSState.Text = "UnReady";
            this.labelOCSState.UseVisualStyleBackColor = true;
            this.labelOCSState.TextChanged += new System.EventHandler(this.labelJCSAndOCSState_TextChanged);
            // 
            // labelEQPState
            // 
            this.labelEQPState.AutoSize = true;
            this.labelEQPState.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.Frame_Red;
            this.labelEQPState.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.labelEQPState.FlatAppearance.BorderSize = 0;
            this.labelEQPState.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEQPState.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelEQPState.ForeColor = System.Drawing.Color.White;
            this.labelEQPState.Location = new System.Drawing.Point(375, 12);
            this.labelEQPState.Name = "labelEQPState";
            this.labelEQPState.Size = new System.Drawing.Size(86, 32);
            this.labelEQPState.TabIndex = 153;
            this.labelEQPState.Text = "Stop";
            this.labelEQPState.UseVisualStyleBackColor = true;
            this.labelEQPState.TextChanged += new System.EventHandler(this.labelEQPState_TextChanged);
            // 
            // labelVehicleNum
            // 
            this.labelVehicleNum.AutoSize = true;
            this.labelVehicleNum.BackColor = System.Drawing.Color.Transparent;
            this.labelVehicleNum.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelVehicleNum.ForeColor = System.Drawing.Color.Moccasin;
            this.labelVehicleNum.Location = new System.Drawing.Point(20, 14);
            this.labelVehicleNum.Name = "labelVehicleNum";
            this.labelVehicleNum.Size = new System.Drawing.Size(99, 27);
            this.labelVehicleNum.TabIndex = 156;
            this.labelVehicleNum.Text = "CRRC_01";
            this.labelVehicleNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelVehicleNum.Click += new System.EventHandler(this.labelVehicleNum_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(126, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 22);
            this.label1.TabIndex = 157;
            this.label1.Text = "设备模式";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(304, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 22);
            this.label2.TabIndex = 158;
            this.label2.Text = "设备状态";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(491, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 22);
            this.label3.TabIndex = 159;
            this.label3.Text = "JCS状态";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(681, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 22);
            this.label4.TabIndex = 160;
            this.label4.Text = "OCS状态";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(882, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 22);
            this.label5.TabIndex = 161;
            this.label5.Text = "当前速度";
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.RoundRectBlue;
            this.labelTime.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.labelTime.FlatAppearance.BorderSize = 0;
            this.labelTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelTime.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelTime.ForeColor = System.Drawing.Color.White;
            this.labelTime.Location = new System.Drawing.Point(1083, 12);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(196, 32);
            this.labelTime.TabIndex = 162;
            this.labelTime.Text = "0000/00/00  00:00:00";
            this.labelTime.UseVisualStyleBackColor = true;
            // 
            // labelVelocity
            // 
            this.labelVelocity.AutoSize = true;
            this.labelVelocity.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.RoundRectWhite;
            this.labelVelocity.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.labelVelocity.FlatAppearance.BorderSize = 0;
            this.labelVelocity.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelVelocity.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelVelocity.ForeColor = System.Drawing.Color.Black;
            this.labelVelocity.Location = new System.Drawing.Point(959, 14);
            this.labelVelocity.Name = "labelVelocity";
            this.labelVelocity.Size = new System.Drawing.Size(103, 32);
            this.labelVelocity.TabIndex = 163;
            this.labelVelocity.Text = "0.0";
            this.labelVelocity.UseVisualStyleBackColor = true;
            // 
            // ucTitle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.TitleSubBackGround2;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.labelVelocity);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelVehicleNum);
            this.Controls.Add(this.txtEQPMode);
            this.Controls.Add(this.labelJCSState);
            this.Controls.Add(this.labelOCSState);
            this.Controls.Add(this.labelEQPState);
            this.DoubleBuffered = true;
            this.Name = "ucTitle";
            this.Size = new System.Drawing.Size(1305, 57);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button txtEQPMode;
        private System.Windows.Forms.Button labelJCSState;
        private System.Windows.Forms.Button labelOCSState;
        private System.Windows.Forms.Button labelEQPState;
        private System.Windows.Forms.Label labelVehicleNum;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button labelTime;
        private System.Windows.Forms.Button labelVelocity;
    }
}
