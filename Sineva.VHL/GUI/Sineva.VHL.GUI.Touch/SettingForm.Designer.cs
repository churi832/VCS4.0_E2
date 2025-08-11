namespace Sineva.VHL.GUI.Touch
{
    partial class SettingForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbPortNumber = new System.Windows.Forms.TextBox();
            this.tbIpAddress = new System.Windows.Forms.TextBox();
            this.tbDevAddress = new System.Windows.Forms.TextBox();
            this.tbWrInsTimeout = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbSendInterval = new System.Windows.Forms.TextBox();
            this.tbConnectRetryInterval = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.tbHeartBitCheckTime = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbSlaveNo = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbOffset = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbStartAddress = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbPortNumber
            // 
            this.tbPortNumber.Location = new System.Drawing.Point(112, 55);
            this.tbPortNumber.Name = "tbPortNumber";
            this.tbPortNumber.Size = new System.Drawing.Size(164, 21);
            this.tbPortNumber.TabIndex = 0;
            // 
            // tbIpAddress
            // 
            this.tbIpAddress.Location = new System.Drawing.Point(112, 23);
            this.tbIpAddress.Name = "tbIpAddress";
            this.tbIpAddress.Size = new System.Drawing.Size(164, 21);
            this.tbIpAddress.TabIndex = 0;
            // 
            // tbDevAddress
            // 
            this.tbDevAddress.Location = new System.Drawing.Point(112, 151);
            this.tbDevAddress.Name = "tbDevAddress";
            this.tbDevAddress.Size = new System.Drawing.Size(164, 21);
            this.tbDevAddress.TabIndex = 0;
            // 
            // tbWrInsTimeout
            // 
            this.tbWrInsTimeout.Location = new System.Drawing.Point(112, 87);
            this.tbWrInsTimeout.Name = "tbWrInsTimeout";
            this.tbWrInsTimeout.Size = new System.Drawing.Size(164, 21);
            this.tbWrInsTimeout.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Port Number";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "WrInsTimeout";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "SendInterval";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "DevAddr";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 186);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(119, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "ConnetRetryInterval";
            // 
            // tbSendInterval
            // 
            this.tbSendInterval.Location = new System.Drawing.Point(112, 119);
            this.tbSendInterval.Name = "tbSendInterval";
            this.tbSendInterval.Size = new System.Drawing.Size(164, 21);
            this.tbSendInterval.TabIndex = 0;
            // 
            // tbConnectRetryInterval
            // 
            this.tbConnectRetryInterval.Location = new System.Drawing.Point(132, 183);
            this.tbConnectRetryInterval.Name = "tbConnectRetryInterval";
            this.tbConnectRetryInterval.Size = new System.Drawing.Size(144, 21);
            this.tbConnectRetryInterval.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(121, 355);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 34);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(202, 355);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 34);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 218);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(143, 12);
            this.label7.TabIndex = 4;
            this.label7.Text = "HeartBit Check Time(ms)";
            // 
            // tbHeartBitCheckTime
            // 
            this.tbHeartBitCheckTime.Location = new System.Drawing.Point(162, 215);
            this.tbHeartBitCheckTime.Name = "tbHeartBitCheckTime";
            this.tbHeartBitCheckTime.Size = new System.Drawing.Size(114, 21);
            this.tbHeartBitCheckTime.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 250);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 12);
            this.label8.TabIndex = 6;
            this.label8.Text = "SlaveNo";
            // 
            // tbSlaveNo
            // 
            this.tbSlaveNo.Location = new System.Drawing.Point(112, 247);
            this.tbSlaveNo.Name = "tbSlaveNo";
            this.tbSlaveNo.Size = new System.Drawing.Size(164, 21);
            this.tbSlaveNo.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 282);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 8;
            this.label9.Text = "Offset";
            // 
            // tbOffset
            // 
            this.tbOffset.Location = new System.Drawing.Point(112, 279);
            this.tbOffset.Name = "tbOffset";
            this.tbOffset.Size = new System.Drawing.Size(164, 21);
            this.tbOffset.TabIndex = 7;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 314);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 12);
            this.label10.TabIndex = 10;
            this.label10.Text = "StartAddress";
            // 
            // tbStartAddress
            // 
            this.tbStartAddress.Location = new System.Drawing.Point(112, 311);
            this.tbStartAddress.Name = "tbStartAddress";
            this.tbStartAddress.Size = new System.Drawing.Size(164, 21);
            this.tbStartAddress.TabIndex = 9;
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(300, 401);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tbStartAddress);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.tbOffset);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tbSlaveNo);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbHeartBitCheckTime);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbIpAddress);
            this.Controls.Add(this.tbWrInsTimeout);
            this.Controls.Add(this.tbSendInterval);
            this.Controls.Add(this.tbConnectRetryInterval);
            this.Controls.Add(this.tbDevAddress);
            this.Controls.Add(this.tbPortNumber);
            this.Name = "SettingForm";
            this.Text = "Setting";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbPortNumber;
        private System.Windows.Forms.TextBox tbIpAddress;
        private System.Windows.Forms.TextBox tbDevAddress;
        private System.Windows.Forms.TextBox tbWrInsTimeout;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbSendInterval;
        private System.Windows.Forms.TextBox tbConnectRetryInterval;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbHeartBitCheckTime;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbSlaveNo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbOffset;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbStartAddress;
    }
}

