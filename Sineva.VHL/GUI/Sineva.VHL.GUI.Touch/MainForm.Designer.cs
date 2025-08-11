namespace Sineva.VHL.GUI.Touch
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.btnSetting = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.lbConnectStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.tbOpMode = new System.Windows.Forms.TextBox();
            this.tbAlarm = new System.Windows.Forms.TextBox();
            this.tbEqState = new System.Windows.Forms.TextBox();
            this.tbAlarmIds = new System.Windows.Forms.TextBox();
            this.tbVelocity = new System.Windows.Forms.TextBox();
            this.tbUpObs = new System.Windows.Forms.TextBox();
            this.tbUp1 = new System.Windows.Forms.TextBox();
            this.tbUp2 = new System.Windows.Forms.TextBox();
            this.tbUp3 = new System.Windows.Forms.TextBox();
            this.tbUp4 = new System.Windows.Forms.TextBox();
            this.tbDownObs = new System.Windows.Forms.TextBox();
            this.tbDown1 = new System.Windows.Forms.TextBox();
            this.tbDown2 = new System.Windows.Forms.TextBox();
            this.tbDown3 = new System.Windows.Forms.TextBox();
            this.tbDown4 = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnServoOff = new System.Windows.Forms.Button();
            this.lbMessage = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lbServoOff = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnSetting
            // 
            this.btnSetting.Location = new System.Drawing.Point(1, 380);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new System.Drawing.Size(149, 108);
            this.btnSetting.TabIndex = 0;
            this.btnSetting.Text = "Setting";
            this.btnSetting.UseVisualStyleBackColor = true;
            this.btnSetting.Click += new System.EventHandler(this.btnSetting_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(22, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(149, 108);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(196, 12);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(149, 108);
            this.btnDisconnect.TabIndex = 2;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // lbConnectStatus
            // 
            this.lbConnectStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lbConnectStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbConnectStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbConnectStatus.Location = new System.Drawing.Point(369, 12);
            this.lbConnectStatus.Name = "lbConnectStatus";
            this.lbConnectStatus.Size = new System.Drawing.Size(169, 108);
            this.lbConnectStatus.TabIndex = 3;
            this.lbConnectStatus.Text = "Connected";
            this.lbConnectStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(60, 167);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "OpMode";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(60, 255);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "AlarmID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(60, 198);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "EqState";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(60, 228);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "Alarm";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(60, 289);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "Velocity";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(292, 198);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "UP1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(292, 228);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "UP2";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(292, 289);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(27, 12);
            this.label8.TabIndex = 7;
            this.label8.Text = "UP4";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(292, 255);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 12);
            this.label9.TabIndex = 8;
            this.label9.Text = "UP3";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(292, 167);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 12);
            this.label10.TabIndex = 9;
            this.label10.Text = "UP OBS";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(530, 198);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 12);
            this.label11.TabIndex = 10;
            this.label11.Text = "DOWN1";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(530, 228);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(47, 12);
            this.label12.TabIndex = 11;
            this.label12.Text = "DOWN2";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(530, 289);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(47, 12);
            this.label13.TabIndex = 12;
            this.label13.Text = "DOWN4";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(530, 255);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 12);
            this.label14.TabIndex = 13;
            this.label14.Text = "DOWN3";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(530, 167);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(70, 12);
            this.label15.TabIndex = 14;
            this.label15.Text = "DOWN OBS";
            // 
            // tbOpMode
            // 
            this.tbOpMode.Location = new System.Drawing.Point(119, 164);
            this.tbOpMode.Name = "tbOpMode";
            this.tbOpMode.ReadOnly = true;
            this.tbOpMode.Size = new System.Drawing.Size(152, 21);
            this.tbOpMode.TabIndex = 15;
            // 
            // tbAlarm
            // 
            this.tbAlarm.Location = new System.Drawing.Point(119, 225);
            this.tbAlarm.Name = "tbAlarm";
            this.tbAlarm.ReadOnly = true;
            this.tbAlarm.Size = new System.Drawing.Size(152, 21);
            this.tbAlarm.TabIndex = 15;
            // 
            // tbEqState
            // 
            this.tbEqState.Location = new System.Drawing.Point(119, 195);
            this.tbEqState.Name = "tbEqState";
            this.tbEqState.ReadOnly = true;
            this.tbEqState.Size = new System.Drawing.Size(152, 21);
            this.tbEqState.TabIndex = 15;
            // 
            // tbAlarmIds
            // 
            this.tbAlarmIds.Location = new System.Drawing.Point(119, 255);
            this.tbAlarmIds.Name = "tbAlarmIds";
            this.tbAlarmIds.ReadOnly = true;
            this.tbAlarmIds.Size = new System.Drawing.Size(152, 21);
            this.tbAlarmIds.TabIndex = 15;
            // 
            // tbVelocity
            // 
            this.tbVelocity.Location = new System.Drawing.Point(119, 282);
            this.tbVelocity.Name = "tbVelocity";
            this.tbVelocity.ReadOnly = true;
            this.tbVelocity.Size = new System.Drawing.Size(152, 21);
            this.tbVelocity.TabIndex = 15;
            // 
            // tbUpObs
            // 
            this.tbUpObs.Location = new System.Drawing.Point(356, 164);
            this.tbUpObs.Name = "tbUpObs";
            this.tbUpObs.ReadOnly = true;
            this.tbUpObs.Size = new System.Drawing.Size(152, 21);
            this.tbUpObs.TabIndex = 15;
            // 
            // tbUp1
            // 
            this.tbUp1.Location = new System.Drawing.Point(356, 195);
            this.tbUp1.Name = "tbUp1";
            this.tbUp1.ReadOnly = true;
            this.tbUp1.Size = new System.Drawing.Size(152, 21);
            this.tbUp1.TabIndex = 15;
            // 
            // tbUp2
            // 
            this.tbUp2.Location = new System.Drawing.Point(356, 225);
            this.tbUp2.Name = "tbUp2";
            this.tbUp2.ReadOnly = true;
            this.tbUp2.Size = new System.Drawing.Size(152, 21);
            this.tbUp2.TabIndex = 15;
            // 
            // tbUp3
            // 
            this.tbUp3.Location = new System.Drawing.Point(356, 255);
            this.tbUp3.Name = "tbUp3";
            this.tbUp3.ReadOnly = true;
            this.tbUp3.Size = new System.Drawing.Size(152, 21);
            this.tbUp3.TabIndex = 15;
            // 
            // tbUp4
            // 
            this.tbUp4.Location = new System.Drawing.Point(356, 282);
            this.tbUp4.Name = "tbUp4";
            this.tbUp4.ReadOnly = true;
            this.tbUp4.Size = new System.Drawing.Size(152, 21);
            this.tbUp4.TabIndex = 15;
            // 
            // tbDownObs
            // 
            this.tbDownObs.Location = new System.Drawing.Point(618, 164);
            this.tbDownObs.Name = "tbDownObs";
            this.tbDownObs.ReadOnly = true;
            this.tbDownObs.Size = new System.Drawing.Size(152, 21);
            this.tbDownObs.TabIndex = 15;
            // 
            // tbDown1
            // 
            this.tbDown1.Location = new System.Drawing.Point(618, 195);
            this.tbDown1.Name = "tbDown1";
            this.tbDown1.ReadOnly = true;
            this.tbDown1.Size = new System.Drawing.Size(152, 21);
            this.tbDown1.TabIndex = 15;
            // 
            // tbDown2
            // 
            this.tbDown2.Location = new System.Drawing.Point(618, 225);
            this.tbDown2.Name = "tbDown2";
            this.tbDown2.ReadOnly = true;
            this.tbDown2.Size = new System.Drawing.Size(152, 21);
            this.tbDown2.TabIndex = 15;
            // 
            // tbDown3
            // 
            this.tbDown3.Location = new System.Drawing.Point(618, 255);
            this.tbDown3.Name = "tbDown3";
            this.tbDown3.ReadOnly = true;
            this.tbDown3.Size = new System.Drawing.Size(152, 21);
            this.tbDown3.TabIndex = 15;
            // 
            // tbDown4
            // 
            this.tbDown4.Location = new System.Drawing.Point(618, 282);
            this.tbDown4.Name = "tbDown4";
            this.tbDown4.ReadOnly = true;
            this.tbDown4.Size = new System.Drawing.Size(152, 21);
            this.tbDown4.TabIndex = 15;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(647, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(149, 108);
            this.btnClose.TabIndex = 16;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnServoOff
            // 
            this.btnServoOff.Location = new System.Drawing.Point(212, 380);
            this.btnServoOff.Name = "btnServoOff";
            this.btnServoOff.Size = new System.Drawing.Size(149, 108);
            this.btnServoOff.TabIndex = 17;
            this.btnServoOff.Text = "ServoOff";
            this.btnServoOff.UseVisualStyleBackColor = true;
            this.btnServoOff.Click += new System.EventHandler(this.btnServoOff_Click);
            // 
            // lbMessage
            // 
            this.lbMessage.Location = new System.Drawing.Point(386, 380);
            this.lbMessage.Name = "lbMessage";
            this.lbMessage.Size = new System.Drawing.Size(400, 25);
            this.lbMessage.TabIndex = 18;
            this.lbMessage.Text = "label16";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(386, 346);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(44, 12);
            this.label16.TabIndex = 19;
            this.label16.Text = "label16";
            // 
            // lbServoOff
            // 
            this.lbServoOff.AutoSize = true;
            this.lbServoOff.Location = new System.Drawing.Point(386, 416);
            this.lbServoOff.Name = "lbServoOff";
            this.lbServoOff.Size = new System.Drawing.Size(44, 12);
            this.lbServoOff.TabIndex = 19;
            this.lbServoOff.Text = "label16";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(808, 488);
            this.ControlBox = false;
            this.Controls.Add(this.lbServoOff);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.lbMessage);
            this.Controls.Add(this.btnServoOff);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.tbDown4);
            this.Controls.Add(this.tbUp4);
            this.Controls.Add(this.tbVelocity);
            this.Controls.Add(this.tbDown3);
            this.Controls.Add(this.tbUp3);
            this.Controls.Add(this.tbAlarmIds);
            this.Controls.Add(this.tbDown2);
            this.Controls.Add(this.tbUp2);
            this.Controls.Add(this.tbAlarm);
            this.Controls.Add(this.tbDown1);
            this.Controls.Add(this.tbDownObs);
            this.Controls.Add(this.tbUp1);
            this.Controls.Add(this.tbUpObs);
            this.Controls.Add(this.tbEqState);
            this.Controls.Add(this.tbOpMode);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbConnectStatus);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnSetting);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSetting;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Label lbConnectStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox tbOpMode;
        private System.Windows.Forms.TextBox tbAlarm;
        private System.Windows.Forms.TextBox tbEqState;
        private System.Windows.Forms.TextBox tbAlarmIds;
        private System.Windows.Forms.TextBox tbVelocity;
        private System.Windows.Forms.TextBox tbUpObs;
        private System.Windows.Forms.TextBox tbUp1;
        private System.Windows.Forms.TextBox tbUp2;
        private System.Windows.Forms.TextBox tbUp3;
        private System.Windows.Forms.TextBox tbUp4;
        private System.Windows.Forms.TextBox tbDownObs;
        private System.Windows.Forms.TextBox tbDown1;
        private System.Windows.Forms.TextBox tbDown2;
        private System.Windows.Forms.TextBox tbDown3;
        private System.Windows.Forms.TextBox tbDown4;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnServoOff;
        private System.Windows.Forms.Label lbMessage;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lbServoOff;
    }
}